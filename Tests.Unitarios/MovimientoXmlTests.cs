using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Unitarios
{
    // El XML de movimientos es un archivo local: se prueba en una carpeta temporal.
    [TestClass]
    public class MovimientoXmlTests
    {
        private string carpeta;
        private string ruta;

        [TestInitialize]
        public void Inicializar()
        {
            carpeta = Path.Combine(Path.GetTempPath(), "BatallaNavalTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(carpeta);
            ruta = Path.Combine(carpeta, "partida.xml");
        }

        [TestCleanup]
        public void Limpiar()
        {
            Directory.Delete(carpeta, true);
        }

        [TestMethod]
        public void PartidaCompleta_SeLeeEnOrden()
        {
            BLL.MOVIMIENTOXML.CrearArchivo(ruta, "ana", "beto");
            BLL.MOVIMIENTOXML.RegistrarColocacion(ruta, "ana", BE.TIPONAVE.Flota[1], 3, 4, BE.ORIENTACION.Vertical);
            BLL.MOVIMIENTOXML.RegistrarDisparo(ruta, "beto", 5, 6, BE.ESTADOCELDA.Agua, null);
            BLL.MOVIMIENTOXML.RegistrarDisparo(ruta, "ana", 7, 8, BE.ESTADOCELDA.Hundido, BE.TIPONAVE.Flota[4]);
            BLL.MOVIMIENTOXML.RegistrarFin(ruta, "ana", "Victoria");

            Assert.IsTrue(BLL.MOVIMIENTOXML.Existe(ruta));
            List<BE.MOVIMIENTO> movimientos = BLL.MOVIMIENTOXML.LeerMovimientos(ruta);

            Assert.AreEqual(4, movimientos.Count);

            Assert.AreEqual("ana", movimientos[0].Jugador);
            Assert.AreEqual("Colocación", movimientos[0].Accion);
            Assert.AreEqual("Portaaviones en (3,4) Vertical", movimientos[0].Detalle);
            Assert.AreEqual("", movimientos[0].Resultado);

            Assert.AreEqual("beto", movimientos[1].Jugador);
            Assert.AreEqual("Disparo", movimientos[1].Accion);
            Assert.AreEqual("(5,6)", movimientos[1].Detalle);
            Assert.AreEqual("Agua", movimientos[1].Resultado);

            Assert.AreEqual("(7,8) - Acorazados", movimientos[2].Detalle);
            Assert.AreEqual("Hundido", movimientos[2].Resultado);

            Assert.AreEqual("ana", movimientos[3].Jugador);
            Assert.AreEqual("Fin de partida", movimientos[3].Accion);
            Assert.AreEqual("Victoria", movimientos[3].Detalle);
            Assert.AreEqual("", movimientos[3].Resultado);

            foreach (BE.MOVIMIENTO m in movimientos)
            {
                Assert.IsTrue(Regex.IsMatch(m.Fecha, @"^\d{2}/\d{2}/\d{4} \d{2}:\d{2}:\d{2}$"), m.Fecha);
            }
        }

        [TestMethod]
        public void Listar_IgnoraComentariosYElementosDesconocidos()
        {
            File.WriteAllText(ruta,
                "<Partida><!-- comentario --><Otro fecha='x'/>" +
                "<Disparo jugador='ana' fila='1' columna='2' resultado='Tocado' nave='Buque' fecha='f'/></Partida>");

            List<BE.MOVIMIENTO> movimientos = DAL.MOVIMIENTOXML.Listar(ruta);

            Assert.AreEqual(1, movimientos.Count);
            Assert.AreEqual("(1,2) - Buque", movimientos[0].Detalle);
            Assert.AreEqual("f", movimientos[0].Fecha);
        }

        [TestMethod]
        public void Existe_ContemplaRutasVaciasONulas()
        {
            Assert.IsFalse(BLL.MOVIMIENTOXML.Existe(null));
            Assert.IsFalse(BLL.MOVIMIENTOXML.Existe(""));
            Assert.IsFalse(BLL.MOVIMIENTOXML.Existe(ruta));
            File.WriteAllText(ruta, "<Partida/>");
            Assert.IsTrue(BLL.MOVIMIENTOXML.Existe(ruta));
        }

        [TestMethod]
        public void GenerarRuta_CreaLaCarpetaPartidasSiNoExiste()
        {
            string carpetaPartidas = AppDomain.CurrentDomain.BaseDirectory + "Partidas\\";
            // Es la carpeta de la salida de los tests (bin), no la de la aplicación.
            if (Directory.Exists(carpetaPartidas))
            {
                Directory.Delete(carpetaPartidas, true);
            }

            string generada = BLL.MOVIMIENTOXML.GenerarRuta();

            Assert.IsTrue(Directory.Exists(carpetaPartidas));
            StringAssert.StartsWith(generada, carpetaPartidas + "Partida_");
            StringAssert.EndsWith(generada, ".xml");

            // Segunda vez: la carpeta ya existe.
            StringAssert.StartsWith(BLL.MOVIMIENTOXML.GenerarRuta(), carpetaPartidas);
        }
    }
}
