using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Integracion
{
    // Partidas completas de punta a punta: BLL.PARTIDA + MP_PARTIDA + XML + ESTADISTICA.
    [TestClass]
    public class PartidaIntegracionTests
    {
        private List<string> xmlCreados = new List<string>();

        [TestCleanup]
        public void Limpiar()
        {
            foreach (string ruta in xmlCreados)
            {
                if (File.Exists(ruta))
                {
                    File.Delete(ruta);
                }
            }
        }

        private BLL.PARTIDA Iniciar(BE.USUARIO j1, BE.USUARIO j2)
        {
            BLL.PARTIDA partida = new BLL.PARTIDA();
            partida.Iniciar(j1, j2);
            xmlCreados.Add(partida.Partida.RutaXml);
            return partida;
        }

        private static void ColocarFlota(BLL.PARTIDA partida, int idJugador)
        {
            for (int i = 0; i < BE.TIPONAVE.Flota.Length; i++)
            {
                partida.ColocarNave(idJugador, BE.TIPONAVE.Flota[i], i * 2, 3, BE.ORIENTACION.Horizontal);
            }
            partida.ConfirmarColocacion(idJugador);
        }

        // j1 hunde toda la flota de j2 mientras j2 tira al agua.
        private static void JugarHastaQueGaneJ1(BLL.PARTIDA partida, BE.USUARIO j1, BE.USUARIO j2)
        {
            ColocarFlota(partida, j1.ID);
            ColocarFlota(partida, j2.ID);
            int agua = 0;
            for (int i = 0; i < BE.TIPONAVE.Flota.Length && partida.EnCurso; i++)
            {
                for (int c = 0; c < BE.TIPONAVE.Flota[i].Tamaño && partida.EnCurso; c++)
                {
                    partida.Disparar(j1.ID, i * 2, 3 + c);
                    if (partida.EnCurso)
                    {
                        partida.Disparar(j2.ID, 14, agua++);
                    }
                }
            }
        }

        private static int Logs(int idUsuario, int idTipo)
        {
            return BaseDePrueba.Contar("SELECT COUNT(*) FROM LOG WHERE ID_Usuario = " + idUsuario + " AND ID_TIPO = " + idTipo);
        }

        [TestMethod]
        public void PartidaCompleta_QuedaGrabadaEnBaseBitacoraYXml()
        {
            BE.USUARIO j1 = BaseDePrueba.CrearUsuario();
            BE.USUARIO j2 = BaseDePrueba.CrearUsuario();
            BLL.PARTIDA partida = Iniciar(j1, j2);
            int id = partida.Partida.ID;

            Assert.AreEqual(1, Logs(j1.ID, BE.LOG.INICIO_PARTIDA));
            Assert.AreEqual(1, Logs(j2.ID, BE.LOG.INICIO_PARTIDA));
            Assert.AreEqual(DBNull.Value, BaseDePrueba.Escalar("SELECT FECHA_FIN FROM PARTIDA WHERE ID = " + id));

            bool? abandonada = null;
            partida.PartidaFinalizada += (p, a) => abandonada = a;
            JugarHastaQueGaneJ1(partida, j1, j2);

            Assert.IsFalse(partida.EnCurso);
            Assert.AreEqual(false, abandonada);
            Assert.AreEqual(j1.ID, BaseDePrueba.Escalar("SELECT ID_GANADOR FROM PARTIDA WHERE ID = " + id));
            Assert.AreNotEqual(DBNull.Value, BaseDePrueba.Escalar("SELECT FECHA_FIN FROM PARTIDA WHERE ID = " + id));
            Assert.AreEqual(partida.Partida.RutaXml, BaseDePrueba.Escalar("SELECT RUTA_XML FROM PARTIDA WHERE ID = " + id));
            Assert.AreEqual(1, Logs(j1.ID, BE.LOG.FIN_PARTIDA));
            Assert.AreEqual(1, Logs(j2.ID, BE.LOG.FIN_PARTIDA));

            List<BE.MOVIMIENTO> movimientos = BLL.MOVIMIENTOXML.LeerMovimientos(partida.Partida.RutaXml);
            Assert.AreEqual(10, movimientos.Count(m => m.Accion == "Colocación"));
            Assert.AreEqual(15 + 14, movimientos.Count(m => m.Accion == "Disparo"));
            Assert.AreEqual(5, movimientos.Count(m => m.Resultado == "Hundido"));
            BE.MOVIMIENTO fin = movimientos.Last();
            Assert.AreEqual("Fin de partida", fin.Accion);
            Assert.AreEqual(j1.Nombre, fin.Jugador);
        }

        [TestMethod]
        public void Abandono_LaGanaElOtroYQuedaEnLaBitacora()
        {
            BE.USUARIO j1 = BaseDePrueba.CrearUsuario();
            BE.USUARIO j2 = BaseDePrueba.CrearUsuario();
            BLL.PARTIDA partida = Iniciar(j1, j2);

            partida.Abandonar(j1.ID);

            Assert.AreEqual(j2.ID, BaseDePrueba.Escalar("SELECT ID_GANADOR FROM PARTIDA WHERE ID = " + partida.Partida.ID));
            Assert.AreEqual("Partida abandonada por " + j1.Nombre, BaseDePrueba.Escalar(
                "SELECT Descripcion FROM LOG WHERE ID_TIPO = " + BE.LOG.FIN_PARTIDA + " AND ID_Usuario = " + j2.ID));
        }

        [TestMethod]
        public void Iniciar_ConUnUsuarioInexistente_NoGrabaNada()
        {
            BE.USUARIO j1 = BaseDePrueba.CrearUsuario();
            BE.USUARIO fantasma = new BE.USUARIO { ID = 999999, Nombre = "fantasma" };
            int partidas = BaseDePrueba.Contar("SELECT COUNT(*) FROM PARTIDA");

            BLL.PARTIDA partida = new BLL.PARTIDA();
            Exception ex = Assert.ThrowsException<Exception>(() => partida.Iniciar(j1, fantasma));

            StringAssert.Contains(ex.Message, "No se pudo crear la partida");
            Assert.IsFalse(partida.EnCurso);
            Assert.AreEqual(partidas, BaseDePrueba.Contar("SELECT COUNT(*) FROM PARTIDA"));
            Assert.AreEqual(0, Logs(j1.ID, BE.LOG.INICIO_PARTIDA));
        }

        [TestMethod]
        public void Estadisticas_ReflejanLasPartidasTerminadas()
        {
            BE.USUARIO j1 = BaseDePrueba.CrearUsuario();
            BE.USUARIO j2 = BaseDePrueba.CrearUsuario();

            JugarHastaQueGaneJ1(Iniciar(j1, j2), j1, j2);   // gana j1
            Iniciar(j2, j1).Abandonar(j2.ID);                 // gana j1
            Iniciar(j1, j2).Abandonar(j1.ID);                 // gana j2
            Iniciar(j1, j2);                                  // en curso: no cuenta

            BE.ESTADISTICA e1 = new BLL.ESTADISTICA().Calcular(j1);
            Assert.AreEqual(2, e1.Ganadas);
            Assert.AreEqual(1, e1.Perdidas);
            Assert.AreEqual(0, e1.Empatadas);
            Assert.AreEqual(200.0 / 3, e1.PromedioVictorias, 0.001);
            Assert.AreEqual(3, e1.Historial.Count);
            Assert.IsTrue(e1.Historial.All(f => f.Rival == j2.Nombre));
            Assert.IsTrue(e1.TiempoTotal >= TimeSpan.Zero);

            BE.ESTADISTICA e2 = new BLL.ESTADISTICA().Calcular(j2);
            Assert.AreEqual(1, e2.Ganadas);
            Assert.AreEqual(2, e2.Perdidas);
            Assert.IsTrue(e2.Historial.All(f => f.Rival == j1.Nombre));
        }

        [TestMethod]
        public void Estadisticas_DeUnUsuarioSinPartidas()
        {
            BE.ESTADISTICA e = new BLL.ESTADISTICA().Calcular(BaseDePrueba.CrearUsuario());
            Assert.AreEqual(0, e.Ganadas + e.Perdidas);
            Assert.AreEqual(0, e.PromedioVictorias);
            Assert.AreEqual(0, e.Historial.Count);
        }
    }
}
