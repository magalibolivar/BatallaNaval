using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Unitarios
{
    [TestClass]
    public class EstadisticaTests
    {
        private MapperPartidaFalso mapperPartida;
        private MapperUsuarioFalso mapperUsuario;
        private BLL.ESTADISTICA bll;
        private BE.USUARIO ana = new BE.USUARIO { ID = 1, Nombre = "ana" };

        [TestInitialize]
        public void Inicializar()
        {
            mapperPartida = new MapperPartidaFalso();
            mapperUsuario = new MapperUsuarioFalso();
            mapperUsuario.Usuarios[2] = new BE.USUARIO { ID = 2, Nombre = "beto" };
            bll = new BLL.ESTADISTICA(mapperPartida, mapperUsuario);
        }

        private static BE.PARTIDA Partida(int j1, int j2, int ganador, DateTime inicio, int minutos, string ruta)
        {
            return new BE.PARTIDA
            {
                IdJugador1 = j1, IdJugador2 = j2, IdGanador = ganador,
                FechaInicio = inicio, FechaFin = inicio.AddMinutes(minutos), RutaXml = ruta
            };
        }

        [TestMethod]
        public void SinPartidas_TodoEnCero()
        {
            BE.ESTADISTICA e = bll.Calcular(ana);

            Assert.AreEqual(1, mapperPartida.UsuarioListado);
            Assert.AreEqual(0, e.Ganadas);
            Assert.AreEqual(0, e.Perdidas);
            Assert.AreEqual(0, e.Empatadas);
            Assert.AreEqual(0, e.PromedioVictorias);
            Assert.AreEqual(TimeSpan.Zero, e.TiempoTotal);
            Assert.AreEqual(0, e.Historial.Count);
        }

        [TestMethod]
        public void ConPartidas_CuentaResultadosTiempoEHistorial()
        {
            DateTime d = new DateTime(2026, 3, 4, 15, 30, 0);
            mapperPartida.PartidasDelUsuario.Add(Partida(1, 2, 1, d, 10, "a.xml"));          // ana J1, gana
            mapperPartida.PartidasDelUsuario.Add(Partida(2, 1, 2, d.AddDays(1), 5, "b.xml")); // ana J2, pierde
            mapperPartida.PartidasDelUsuario.Add(Partida(1, 9, 1, d.AddDays(2), 1, "c.xml")); // rival borrado
            mapperPartida.PartidasDelUsuario.Add(Partida(9, 1, 9, d.AddDays(3), 4, "d.xml")); // rival borrado, pierde

            BE.ESTADISTICA e = bll.Calcular(ana);

            Assert.AreEqual(2, e.Ganadas);
            Assert.AreEqual(2, e.Perdidas);
            Assert.AreEqual(0, e.Empatadas);
            Assert.AreEqual(50.0, e.PromedioVictorias, 0.0001);
            Assert.AreEqual(TimeSpan.FromMinutes(20), e.TiempoTotal);

            Assert.AreEqual(4, e.Historial.Count);
            Assert.AreEqual("04/03/2026 15:30", e.Historial[0].Fecha);
            Assert.AreEqual("beto", e.Historial[0].Rival);
            Assert.AreEqual("Ganada", e.Historial[0].Resultado);
            Assert.AreEqual("a.xml", e.Historial[0].RutaXml);
            Assert.AreEqual("beto", e.Historial[1].Rival);
            Assert.AreEqual("Perdida", e.Historial[1].Resultado);
            Assert.AreEqual("-", e.Historial[2].Rival);
            Assert.AreEqual("-", e.Historial[3].Rival);
        }

        [TestMethod]
        public void PromedioVictorias_EsUnPorcentaje()
        {
            DateTime d = new DateTime(2026, 1, 1);
            mapperPartida.PartidasDelUsuario.Add(Partida(1, 2, 1, d, 1, ""));
            mapperPartida.PartidasDelUsuario.Add(Partida(1, 2, 2, d, 1, ""));
            mapperPartida.PartidasDelUsuario.Add(Partida(1, 2, 2, d, 1, ""));

            Assert.AreEqual(100.0 / 3, bll.Calcular(ana).PromedioVictorias, 0.0001);
        }

        [TestMethod]
        public void ConstructorPublico_UsaLosMappersReales()
        {
            Assert.IsNotNull(new BLL.ESTADISTICA());
        }
    }
}
