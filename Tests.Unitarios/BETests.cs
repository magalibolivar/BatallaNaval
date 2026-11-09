using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Unitarios
{
    [TestClass]
    public class TIPONAVETests
    {
        [TestMethod]
        public void Flota_TieneLosCincoTiposDeLaConsigna()
        {
            Assert.AreEqual(5, BE.TIPONAVE.Flota.Length);
            string[] nombres = { "Batalla Naval", "Portaaviones", "Fragatas", "Buque", "Acorazados" };
            int[] tamaños = { 5, 4, 3, 2, 1 };
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(i + 1, BE.TIPONAVE.Flota[i].Id);
                Assert.AreEqual(nombres[i], BE.TIPONAVE.Flota[i].Nombre);
                Assert.AreEqual(tamaños[i], BE.TIPONAVE.Flota[i].Tamaño);
            }
        }

        [TestMethod]
        public void Constructor_GuardaLosDatos()
        {
            BE.TIPONAVE tipo = new BE.TIPONAVE(9, "Lancha", 2);
            Assert.AreEqual(9, tipo.Id);
            Assert.AreEqual("Lancha", tipo.Nombre);
            Assert.AreEqual(2, tipo.Tamaño);
        }
    }

    [TestClass]
    public class NAVECOLOCADATests
    {
        [TestMethod]
        public void Celdas_Horizontal_AvanzaPorColumna()
        {
            BE.NAVECOLOCADA nave = new BE.NAVECOLOCADA
            {
                Tipo = BE.TIPONAVE.Flota[2], Fila = 4, Columna = 6, Orientacion = BE.ORIENTACION.Horizontal
            };

            CollectionAssert.AreEqual(
                new[] { Tuple.Create(4, 6), Tuple.Create(4, 7), Tuple.Create(4, 8) },
                nave.Celdas());
            Assert.AreEqual(4, nave.Fila);
            Assert.AreEqual(6, nave.Columna);
            Assert.AreEqual(BE.ORIENTACION.Horizontal, nave.Orientacion);
        }

        [TestMethod]
        public void Celdas_Vertical_AvanzaPorFila()
        {
            BE.NAVECOLOCADA nave = new BE.NAVECOLOCADA
            {
                Tipo = BE.TIPONAVE.Flota[3], Fila = 13, Columna = 0, Orientacion = BE.ORIENTACION.Vertical
            };

            CollectionAssert.AreEqual(new[] { Tuple.Create(13, 0), Tuple.Create(14, 0) }, nave.Celdas());
        }

        [TestMethod]
        public void Hundida_SoloCuandoLosImpactosLleganAlTamaño()
        {
            BE.NAVECOLOCADA nave = new BE.NAVECOLOCADA { Tipo = BE.TIPONAVE.Flota[3] };
            Assert.IsFalse(nave.Hundida);
            nave.CantidadImpactos = 1;
            Assert.IsFalse(nave.Hundida);
            nave.CantidadImpactos = 2;
            Assert.IsTrue(nave.Hundida);
        }
    }

    [TestClass]
    public class EntidadesTests
    {
        [TestMethod]
        public void LOG_ConstantesCoincidenConTipoLog()
        {
            Assert.AreEqual(1, BE.LOG.INICIO_SESION);
            Assert.AreEqual(2, BE.LOG.CIERRE_SESION);
            Assert.AreEqual(3, BE.LOG.INICIO_PARTIDA);
            Assert.AreEqual(4, BE.LOG.FIN_PARTIDA);

            BE.LOG log = new BE.LOG { IdUsuario = 5, IdTipo = 2, Descripcion = "x" };
            Assert.AreEqual(5, log.IdUsuario);
            Assert.AreEqual(2, log.IdTipo);
            Assert.AreEqual("x", log.Descripcion);
        }

        [TestMethod]
        public void USUARIO_Propiedades()
        {
            BE.USUARIO u = new BE.USUARIO { ID = 1, Nombre = "ana", Contraseña = "12a4" };
            Assert.AreEqual(1, u.ID);
            Assert.AreEqual("ana", u.Nombre);
            Assert.AreEqual("12a4", u.Contraseña);
        }

        [TestMethod]
        public void PARTIDA_Propiedades()
        {
            DateTime inicio = new DateTime(2026, 1, 2, 3, 4, 5);
            BE.PARTIDA p = new BE.PARTIDA
            {
                ID = 1, IdJugador1 = 2, IdJugador2 = 3, IdGanador = 2,
                FechaInicio = inicio, FechaFin = inicio.AddMinutes(1), RutaXml = "r.xml"
            };
            Assert.AreEqual(1, p.ID);
            Assert.AreEqual(2, p.IdJugador1);
            Assert.AreEqual(3, p.IdJugador2);
            Assert.AreEqual(2, p.IdGanador);
            Assert.AreEqual(inicio, p.FechaInicio);
            Assert.AreEqual(inicio.AddMinutes(1), p.FechaFin);
            Assert.AreEqual("r.xml", p.RutaXml);
            Assert.IsNull(new BE.PARTIDA().IdGanador);
            Assert.IsNull(new BE.PARTIDA().FechaFin);
        }

        [TestMethod]
        public void ESTADISTICA_Propiedades()
        {
            List<BE.FILAHISTORIAL> historial = new List<BE.FILAHISTORIAL>();
            BE.ESTADISTICA e = new BE.ESTADISTICA
            {
                Ganadas = 1, Perdidas = 2, Empatadas = 0, PromedioVictorias = 33.3,
                TiempoTotal = TimeSpan.FromMinutes(5), Historial = historial
            };
            Assert.AreEqual(1, e.Ganadas);
            Assert.AreEqual(2, e.Perdidas);
            Assert.AreEqual(0, e.Empatadas);
            Assert.AreEqual(33.3, e.PromedioVictorias);
            Assert.AreEqual(TimeSpan.FromMinutes(5), e.TiempoTotal);
            Assert.AreSame(historial, e.Historial);
        }

        [TestMethod]
        public void FILAHISTORIAL_Y_MOVIMIENTO_Propiedades()
        {
            BE.FILAHISTORIAL f = new BE.FILAHISTORIAL { Fecha = "f", Rival = "r", Resultado = "Ganada", RutaXml = "x" };
            Assert.AreEqual("f", f.Fecha);
            Assert.AreEqual("r", f.Rival);
            Assert.AreEqual("Ganada", f.Resultado);
            Assert.AreEqual("x", f.RutaXml);

            BE.MOVIMIENTO m = new BE.MOVIMIENTO { Jugador = "j", Accion = "Disparo", Detalle = "d", Resultado = "Agua", Fecha = "f" };
            Assert.AreEqual("j", m.Jugador);
            Assert.AreEqual("Disparo", m.Accion);
            Assert.AreEqual("d", m.Detalle);
            Assert.AreEqual("Agua", m.Resultado);
            Assert.AreEqual("f", m.Fecha);
        }
    }
}
