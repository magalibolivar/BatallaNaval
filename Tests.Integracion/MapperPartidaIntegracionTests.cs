using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Integracion
{
    // Transacciones de MP_PARTIDA: o se graba la partida con su bitácora, o no se graba nada.
    [TestClass]
    public class MapperPartidaIntegracionTests
    {
        private DAL.MP_PARTIDA mapper = new DAL.MP_PARTIDA();

        private static BE.PARTIDA Nueva(BE.USUARIO j1, BE.USUARIO j2)
        {
            return new BE.PARTIDA { IdJugador1 = j1.ID, IdJugador2 = j2.ID, RutaXml = "test.xml" };
        }

        private static int Partidas(BE.USUARIO j)
        {
            return BaseDePrueba.Contar("SELECT COUNT(*) FROM PARTIDA WHERE ID_JUGADOR1 = " + j.ID + " OR ID_JUGADOR2 = " + j.ID);
        }

        private static int Logs(BE.USUARIO j)
        {
            return BaseDePrueba.Contar("SELECT COUNT(*) FROM LOG WHERE ID_Usuario = " + j.ID);
        }

        [TestMethod]
        public void Insertar_SiFallaLaBitacoraDelJugador1_NoQuedaLaPartida()
        {
            BE.USUARIO j1 = BaseDePrueba.CrearUsuario();
            BE.USUARIO j2 = BaseDePrueba.CrearUsuario();
            using (BaseDePrueba.BloquearBitacora(j1.ID))
            {
                Assert.AreEqual(-1, mapper.Insertar(Nueva(j1, j2)));
            }
            Assert.AreEqual(0, Partidas(j1));
            Assert.AreEqual(0, Logs(j2));
        }

        [TestMethod]
        public void Insertar_SiFallaLaBitacoraDelJugador2_NoQuedaNada()
        {
            BE.USUARIO j1 = BaseDePrueba.CrearUsuario();
            BE.USUARIO j2 = BaseDePrueba.CrearUsuario();
            using (BaseDePrueba.BloquearBitacora(j2.ID))
            {
                Assert.AreEqual(-1, mapper.Insertar(Nueva(j1, j2)));
            }
            Assert.AreEqual(0, Partidas(j1));
            Assert.AreEqual(0, Logs(j1));
        }

        [TestMethod]
        public void Finalizar_SiFallaLaBitacora_LaPartidaSigueAbierta()
        {
            BE.USUARIO j1 = BaseDePrueba.CrearUsuario();
            BE.USUARIO j2 = BaseDePrueba.CrearUsuario();

            foreach (BE.USUARIO bloqueado in new[] { j1, j2 })
            {
                BE.PARTIDA partida = Nueva(j1, j2);
                partida.ID = mapper.Insertar(partida);
                partida.IdGanador = j1.ID;
                int logsAntes = Logs(j1) + Logs(j2);

                using (BaseDePrueba.BloquearBitacora(bloqueado.ID))
                {
                    Assert.IsFalse(mapper.Finalizar(partida, "fin"));
                }

                Assert.AreEqual(DBNull.Value, BaseDePrueba.Escalar("SELECT FECHA_FIN FROM PARTIDA WHERE ID = " + partida.ID));
                Assert.AreEqual(logsAntes, Logs(j1) + Logs(j2));
            }
        }

        [TestMethod]
        public void Finalizar_UnaPartidaInexistenteOYaCerrada_DevuelveFalse()
        {
            BE.USUARIO j1 = BaseDePrueba.CrearUsuario();
            BE.USUARIO j2 = BaseDePrueba.CrearUsuario();

            BE.PARTIDA inexistente = Nueva(j1, j2);
            inexistente.ID = 999999;
            inexistente.IdGanador = j1.ID;
            Assert.IsFalse(mapper.Finalizar(inexistente, "fin"));

            BE.PARTIDA partida = Nueva(j1, j2);
            partida.ID = mapper.Insertar(partida);
            partida.IdGanador = j2.ID;
            Assert.IsTrue(mapper.Finalizar(partida, "fin"));

            // El resultado de una partida cerrada no se puede cambiar.
            partida.IdGanador = j1.ID;
            Assert.IsFalse(mapper.Finalizar(partida, "otra vez"));
            Assert.AreEqual(j2.ID, BaseDePrueba.Escalar("SELECT ID_GANADOR FROM PARTIDA WHERE ID = " + partida.ID));
            Assert.AreEqual(2, BaseDePrueba.Contar("SELECT COUNT(*) FROM LOG WHERE ID_TIPO = 4 AND (ID_Usuario = " + j1.ID + " OR ID_Usuario = " + j2.ID + ")"));
        }

        [TestMethod]
        public void ListarPorUsuario_SoloLasTerminadasYMapeaTodo()
        {
            BE.USUARIO j1 = BaseDePrueba.CrearUsuario();
            BE.USUARIO j2 = BaseDePrueba.CrearUsuario();

            BE.PARTIDA abierta = Nueva(j1, j2);
            abierta.ID = mapper.Insertar(abierta);

            BE.PARTIDA cerrada = Nueva(j2, j1);
            cerrada.RutaXml = @"C:\x\Partida_1.xml";
            cerrada.ID = mapper.Insertar(cerrada);
            cerrada.IdGanador = j1.ID;
            mapper.Finalizar(cerrada, "fin");

            List<BE.PARTIDA> lista = mapper.ListarPorUsuario(j1.ID);

            Assert.AreEqual(1, lista.Count);
            BE.PARTIDA p = lista[0];
            Assert.AreEqual(cerrada.ID, p.ID);
            Assert.AreEqual(j2.ID, p.IdJugador1);
            Assert.AreEqual(j1.ID, p.IdJugador2);
            Assert.AreEqual(j1.ID, p.IdGanador);
            Assert.IsTrue(p.FechaFin >= p.FechaInicio);
            Assert.IsTrue((DateTime.Now - p.FechaInicio).TotalMinutes < 5);
            Assert.AreEqual(@"C:\x\Partida_1.xml", p.RutaXml);

            Assert.AreEqual(0, mapper.ListarPorUsuario(999999).Count);
        }
    }
}
