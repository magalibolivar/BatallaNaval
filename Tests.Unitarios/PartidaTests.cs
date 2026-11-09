using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Unitarios
{
    [TestClass]
    public class PartidaTests
    {
        private const int TAM = BLL.PARTIDA.TAMAÑO_TABLERO;

        private MapperPartidaFalso mapper;
        private BLL.PARTIDA partida;
        private BE.USUARIO ana;
        private BE.USUARIO beto;
        private List<Tuple<BE.PARTIDA, bool>> finalizadas;

        [TestInitialize]
        public void Inicializar()
        {
            mapper = new MapperPartidaFalso();
            partida = new BLL.PARTIDA(mapper);
            ana = new BE.USUARIO { ID = 1, Nombre = "ana" };
            beto = new BE.USUARIO { ID = 2, Nombre = "beto" };
            finalizadas = new List<Tuple<BE.PARTIDA, bool>>();
            partida.PartidaFinalizada += (p, abandonada) => finalizadas.Add(Tuple.Create(p, abandonada));
        }

        [TestCleanup]
        public void Limpiar()
        {
            if (partida.Partida != null && File.Exists(partida.Partida.RutaXml))
            {
                File.Delete(partida.Partida.RutaXml);
            }
        }

        // Cada nave en su propia fila (fila = índice en la flota), empezando en la columna 0.
        private void ColocarFlota(int idJugador)
        {
            for (int i = 0; i < BE.TIPONAVE.Flota.Length; i++)
            {
                partida.ColocarNave(idJugador, BE.TIPONAVE.Flota[i], i, 0, BE.ORIENTACION.Horizontal);
            }
            partida.ConfirmarColocacion(idJugador);
        }

        private void IrAlCombate()
        {
            partida.Iniciar(ana, beto);
            ColocarFlota(ana.ID);
            ColocarFlota(beto.ID);
        }

        private List<XmlElement> ElementosXml(string nombre)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(partida.Partida.RutaXml);
            return doc.DocumentElement.ChildNodes.OfType<XmlElement>().Where(e => e.Name == nombre).ToList();
        }

        // -------------------- Estado inicial / Iniciar --------------------

        [TestMethod]
        public void SinIniciar_NoHayPartidaEnCurso()
        {
            Assert.AreEqual(BLL.FASE_PARTIDA.Finalizada, partida.Fase);
            Assert.IsFalse(partida.EnCurso);
            Assert.IsNull(partida.Partida);
            Assert.AreEqual(0, partida.IdJugadorColocando);
            Assert.IsFalse(partida.PuedeColocar(1));
            Assert.IsFalse(partida.PuedeDisparar(1));
        }

        [TestMethod]
        public void ConstructorPublico_UsaElMapperReal()
        {
            BLL.PARTIDA real = new BLL.PARTIDA();
            Assert.IsFalse(real.EnCurso);
        }

        [TestMethod]
        public void Iniciar_ConElMismoJugador_Falla()
        {
            Assert.ThrowsException<Exception>(() => partida.Iniciar(ana, new BE.USUARIO { ID = 1, Nombre = "ana" }));
            Assert.AreEqual(0, mapper.Insertadas.Count);
            Assert.IsFalse(partida.EnCurso);
        }

        [TestMethod]
        public void Iniciar_SiLaBaseFalla_NoQuedaPartida()
        {
            mapper.IdAlInsertar = -1;
            Exception ex = Assert.ThrowsException<Exception>(() => partida.Iniciar(ana, beto));
            StringAssert.Contains(ex.Message, "No se pudo crear la partida");
            Assert.IsNull(partida.Partida);
            Assert.IsFalse(partida.EnCurso);
        }

        [TestMethod]
        public void Iniciar_CreaLaPartidaYElXml()
        {
            partida.Iniciar(ana, beto);

            Assert.AreEqual(1, mapper.Insertadas.Count);
            Assert.AreEqual(7, partida.Partida.ID);
            Assert.AreEqual(ana.ID, partida.Partida.IdJugador1);
            Assert.AreEqual(beto.ID, partida.Partida.IdJugador2);
            Assert.AreSame(ana, partida.Jugador1);
            Assert.AreSame(beto, partida.Jugador2);
            Assert.IsTrue(partida.EnCurso);
            Assert.AreEqual(BLL.FASE_PARTIDA.ColocandoJugador1, partida.Fase);
            Assert.AreEqual(ana.ID, partida.IdJugadorColocando);

            Assert.IsTrue(File.Exists(partida.Partida.RutaXml));
            XmlDocument doc = new XmlDocument();
            doc.Load(partida.Partida.RutaXml);
            Assert.AreEqual("Partida", doc.DocumentElement.Name);
            Assert.AreEqual("ana", doc.DocumentElement.GetAttribute("jugador1"));
            Assert.AreEqual("beto", doc.DocumentElement.GetAttribute("jugador2"));
        }

        [TestMethod]
        public void Iniciar_ConUnaPartidaEnCurso_Falla()
        {
            partida.Iniciar(ana, beto);
            Assert.ThrowsException<Exception>(() => partida.Iniciar(ana, beto));
            Assert.AreEqual(1, mapper.Insertadas.Count);
        }

        // -------------------- Colocación --------------------

        [TestMethod]
        public void Colocacion_PrimeroJugador1DespuesJugador2()
        {
            partida.Iniciar(ana, beto);
            Assert.IsTrue(partida.PuedeColocar(ana.ID));
            Assert.IsFalse(partida.PuedeColocar(beto.ID));

            ColocarFlota(ana.ID);
            Assert.AreEqual(BLL.FASE_PARTIDA.ColocandoJugador2, partida.Fase);
            Assert.AreEqual(beto.ID, partida.IdJugadorColocando);
            Assert.IsFalse(partida.PuedeColocar(ana.ID));
            Assert.IsTrue(partida.PuedeColocar(beto.ID));

            ColocarFlota(beto.ID);
            Assert.AreEqual(BLL.FASE_PARTIDA.Combate, partida.Fase);
            Assert.AreEqual(0, partida.IdJugadorColocando);
            Assert.AreEqual(ana.ID, partida.JugadorEnTurno);
            Assert.IsFalse(partida.PuedeColocar(beto.ID));
        }

        [TestMethod]
        public void FlotaPendiente_YNavesDe_SeActualizanAlColocar()
        {
            partida.Iniciar(ana, beto);
            Assert.AreEqual(5, partida.FlotaPendiente(ana.ID).Count);

            partida.ColocarNave(ana.ID, BE.TIPONAVE.Flota[1], 2, 3, BE.ORIENTACION.Vertical);

            List<BE.TIPONAVE> pendiente = partida.FlotaPendiente(ana.ID);
            Assert.AreEqual(4, pendiente.Count);
            Assert.IsFalse(pendiente.Any(t => t.Id == BE.TIPONAVE.Flota[1].Id));
            Assert.AreEqual(1, partida.NavesDe(ana.ID).Count);
            Assert.AreEqual(0, partida.NavesDe(beto.ID).Count);

            bool[,] ocupacion = partida.ObtenerOcupacionPropia(ana.ID);
            for (int f = 2; f <= 5; f++)
            {
                Assert.IsTrue(ocupacion[f, 3]);
            }
            Assert.IsFalse(ocupacion[6, 3]);
            Assert.IsFalse(ocupacion[2, 4]);
        }

        [TestMethod]
        public void ColocarNave_FueraDeTurno_Falla()
        {
            partida.Iniciar(ana, beto);
            Assert.ThrowsException<Exception>(() =>
                partida.ColocarNave(beto.ID, BE.TIPONAVE.Flota[0], 0, 0, BE.ORIENTACION.Horizontal));
        }

        [TestMethod]
        public void ColocarNave_Repetida_Falla()
        {
            partida.Iniciar(ana, beto);
            partida.ColocarNave(ana.ID, BE.TIPONAVE.Flota[4], 0, 0, BE.ORIENTACION.Horizontal);
            Exception ex = Assert.ThrowsException<Exception>(() =>
                partida.ColocarNave(ana.ID, BE.TIPONAVE.Flota[4], 5, 5, BE.ORIENTACION.Horizontal));
            StringAssert.Contains(ex.Message, "ya está colocada");
        }

        [DataTestMethod]
        [DataRow(0, 11, BE.ORIENTACION.Horizontal)]
        [DataRow(11, 0, BE.ORIENTACION.Vertical)]
        [DataRow(-1, 0, BE.ORIENTACION.Horizontal)]
        [DataRow(0, -1, BE.ORIENTACION.Vertical)]
        [DataRow(15, 0, BE.ORIENTACION.Horizontal)]
        public void ColocarNave_FueraDelTablero_Falla(int fila, int columna, BE.ORIENTACION orientacion)
        {
            partida.Iniciar(ana, beto);
            Exception ex = Assert.ThrowsException<Exception>(() =>
                partida.ColocarNave(ana.ID, BE.TIPONAVE.Flota[0], fila, columna, orientacion));
            StringAssert.Contains(ex.Message, "no entra");
            Assert.AreEqual(0, partida.NavesDe(ana.ID).Count);
        }

        [TestMethod]
        public void ColocarNave_EnElBorde_Entra()
        {
            partida.Iniciar(ana, beto);
            partida.ColocarNave(ana.ID, BE.TIPONAVE.Flota[0], 14, 10, BE.ORIENTACION.Horizontal);
            partida.ColocarNave(ana.ID, BE.TIPONAVE.Flota[1], 0, 14, BE.ORIENTACION.Vertical);
            Assert.AreEqual(2, partida.NavesDe(ana.ID).Count);
        }

        [TestMethod]
        public void ColocarNave_Superpuesta_FallaYNoCambiaNada()
        {
            partida.Iniciar(ana, beto);
            partida.ColocarNave(ana.ID, BE.TIPONAVE.Flota[0], 5, 5, BE.ORIENTACION.Horizontal);
            Exception ex = Assert.ThrowsException<Exception>(() =>
                partida.ColocarNave(ana.ID, BE.TIPONAVE.Flota[1], 3, 7, BE.ORIENTACION.Vertical));
            StringAssert.Contains(ex.Message, "superpone");
            Assert.AreEqual(1, partida.NavesDe(ana.ID).Count);
            Assert.IsFalse(partida.ObtenerOcupacionPropia(ana.ID)[3, 7]);
        }

        [TestMethod]
        public void ColocarNave_Adyacente_EstaPermitido()
        {
            partida.Iniciar(ana, beto);
            partida.ColocarNave(ana.ID, BE.TIPONAVE.Flota[0], 5, 5, BE.ORIENTACION.Horizontal);
            partida.ColocarNave(ana.ID, BE.TIPONAVE.Flota[1], 6, 5, BE.ORIENTACION.Horizontal);
            partida.ColocarNave(ana.ID, BE.TIPONAVE.Flota[4], 5, 10, BE.ORIENTACION.Horizontal);
            Assert.AreEqual(3, partida.NavesDe(ana.ID).Count);
        }

        [TestMethod]
        public void ColocarNave_NoSeGrabaEnElXmlHastaConfirmar()
        {
            partida.Iniciar(ana, beto);
            partida.ColocarNave(ana.ID, BE.TIPONAVE.Flota[2], 1, 2, BE.ORIENTACION.Vertical);
            Assert.AreEqual(0, ElementosXml("Colocacion").Count);
        }

        [TestMethod]
        public void ColocarLaFlotaCompleta_NoAvanzaSinConfirmar()
        {
            partida.Iniciar(ana, beto);
            for (int i = 0; i < BE.TIPONAVE.Flota.Length; i++)
            {
                Assert.IsFalse(partida.FlotaCompleta(ana.ID));
                partida.ColocarNave(ana.ID, BE.TIPONAVE.Flota[i], i, 0, BE.ORIENTACION.Horizontal);
            }
            Assert.IsTrue(partida.FlotaCompleta(ana.ID));
            Assert.AreEqual(BLL.FASE_PARTIDA.ColocandoJugador1, partida.Fase);
            Assert.IsTrue(partida.PuedeColocar(ana.ID));
        }

        [TestMethod]
        public void ConfirmarColocacion_GrabaLasPosicionesFinalesEnElXml()
        {
            partida.Iniciar(ana, beto);
            for (int i = 0; i < BE.TIPONAVE.Flota.Length; i++)
            {
                partida.ColocarNave(ana.ID, BE.TIPONAVE.Flota[i], i, 0, BE.ORIENTACION.Horizontal);
            }
            partida.MoverNave(ana.ID, BE.TIPONAVE.Flota[2], 7, 9, BE.ORIENTACION.Vertical);
            partida.ConfirmarColocacion(ana.ID);

            List<XmlElement> colocaciones = ElementosXml("Colocacion");
            Assert.AreEqual(5, colocaciones.Count);
            XmlElement fragatas = colocaciones.Single(e => e.GetAttribute("nave") == "Fragatas");
            Assert.AreEqual("ana", fragatas.GetAttribute("jugador"));
            Assert.AreEqual("7", fragatas.GetAttribute("fila"));
            Assert.AreEqual("9", fragatas.GetAttribute("columna"));
            Assert.AreEqual("Vertical", fragatas.GetAttribute("orientacion"));
        }

        [TestMethod]
        public void ConfirmarColocacion_ConLaFlotaIncompleta_Falla()
        {
            partida.Iniciar(ana, beto);
            partida.ColocarNave(ana.ID, BE.TIPONAVE.Flota[0], 0, 0, BE.ORIENTACION.Horizontal);
            Exception ex = Assert.ThrowsException<Exception>(() => partida.ConfirmarColocacion(ana.ID));
            StringAssert.Contains(ex.Message, "faltan");
            Assert.AreEqual(BLL.FASE_PARTIDA.ColocandoJugador1, partida.Fase);
        }

        [TestMethod]
        public void ConfirmarColocacion_FueraDeTurno_Falla()
        {
            partida.Iniciar(ana, beto);
            Assert.ThrowsException<Exception>(() => partida.ConfirmarColocacion(beto.ID));
        }

        // -------------------- Mover / girar naves colocadas --------------------

        [TestMethod]
        public void MoverNave_CambiaPosicionYOcupacion()
        {
            partida.Iniciar(ana, beto);
            partida.ColocarNave(ana.ID, BE.TIPONAVE.Flota[1], 0, 0, BE.ORIENTACION.Horizontal);

            partida.MoverNave(ana.ID, BE.TIPONAVE.Flota[1], 5, 6, BE.ORIENTACION.Horizontal);

            BE.NAVECOLOCADA nave = partida.NavesDe(ana.ID).Single();
            Assert.AreEqual(5, nave.Fila);
            Assert.AreEqual(6, nave.Columna);
            bool[,] ocupacion = partida.ObtenerOcupacionPropia(ana.ID);
            Assert.IsFalse(ocupacion[0, 0]);
            for (int c = 6; c <= 9; c++)
            {
                Assert.IsTrue(ocupacion[5, c]);
            }
        }

        [TestMethod]
        public void MoverNave_GirarSobreSusPropiasCeldas_EstaPermitido()
        {
            partida.Iniciar(ana, beto);
            partida.ColocarNave(ana.ID, BE.TIPONAVE.Flota[0], 3, 3, BE.ORIENTACION.Horizontal);

            partida.MoverNave(ana.ID, BE.TIPONAVE.Flota[0], 3, 3, BE.ORIENTACION.Vertical);

            Assert.AreEqual(BE.ORIENTACION.Vertical, partida.NavesDe(ana.ID).Single().Orientacion);
            bool[,] ocupacion = partida.ObtenerOcupacionPropia(ana.ID);
            Assert.IsTrue(ocupacion[7, 3]);
            Assert.IsFalse(ocupacion[3, 4]);
        }

        [TestMethod]
        public void MoverNave_SobreOtra_FallaYNoCambiaNada()
        {
            partida.Iniciar(ana, beto);
            partida.ColocarNave(ana.ID, BE.TIPONAVE.Flota[0], 5, 5, BE.ORIENTACION.Horizontal);
            partida.ColocarNave(ana.ID, BE.TIPONAVE.Flota[1], 0, 0, BE.ORIENTACION.Horizontal);

            Exception ex = Assert.ThrowsException<Exception>(() =>
                partida.MoverNave(ana.ID, BE.TIPONAVE.Flota[1], 3, 7, BE.ORIENTACION.Vertical));
            StringAssert.Contains(ex.Message, "superpone");

            BE.NAVECOLOCADA nave = partida.NavesDe(ana.ID).Single(n => n.Tipo.Id == BE.TIPONAVE.Flota[1].Id);
            Assert.AreEqual(0, nave.Fila);
            Assert.AreEqual(BE.ORIENTACION.Horizontal, nave.Orientacion);
            Assert.IsTrue(partida.ObtenerOcupacionPropia(ana.ID)[0, 0]);
            Assert.IsFalse(partida.ObtenerOcupacionPropia(ana.ID)[3, 7]);
        }

        [TestMethod]
        public void MoverNave_FueraDelTablero_Falla()
        {
            partida.Iniciar(ana, beto);
            partida.ColocarNave(ana.ID, BE.TIPONAVE.Flota[0], 12, 0, BE.ORIENTACION.Horizontal);
            Exception ex = Assert.ThrowsException<Exception>(() =>
                partida.MoverNave(ana.ID, BE.TIPONAVE.Flota[0], 12, 0, BE.ORIENTACION.Vertical));
            StringAssert.Contains(ex.Message, "no entra");
            Assert.AreEqual(BE.ORIENTACION.Horizontal, partida.NavesDe(ana.ID).Single().Orientacion);
        }

        [TestMethod]
        public void MoverNave_NoColocada_Falla()
        {
            partida.Iniciar(ana, beto);
            Exception ex = Assert.ThrowsException<Exception>(() =>
                partida.MoverNave(ana.ID, BE.TIPONAVE.Flota[0], 0, 0, BE.ORIENTACION.Horizontal));
            StringAssert.Contains(ex.Message, "todavía no está colocada");
        }

        [TestMethod]
        public void MoverNave_DespuesDeConfirmar_Falla()
        {
            partida.Iniciar(ana, beto);
            ColocarFlota(ana.ID);
            Assert.ThrowsException<Exception>(() =>
                partida.MoverNave(ana.ID, BE.TIPONAVE.Flota[0], 10, 0, BE.ORIENTACION.Horizontal));
        }

        [TestMethod]
        public void PosicionValida_NoModificaElTablero()
        {
            partida.Iniciar(ana, beto);
            partida.ColocarNave(ana.ID, BE.TIPONAVE.Flota[0], 5, 5, BE.ORIENTACION.Horizontal);

            Assert.IsTrue(partida.PosicionValida(ana.ID, BE.TIPONAVE.Flota[0], 5, 5, BE.ORIENTACION.Vertical));
            Assert.IsTrue(partida.PosicionValida(ana.ID, BE.TIPONAVE.Flota[1], 6, 5, BE.ORIENTACION.Horizontal));
            Assert.IsFalse(partida.PosicionValida(ana.ID, BE.TIPONAVE.Flota[1], 3, 7, BE.ORIENTACION.Vertical));
            Assert.IsFalse(partida.PosicionValida(ana.ID, BE.TIPONAVE.Flota[1], 0, 12, BE.ORIENTACION.Horizontal));
            Assert.AreEqual(1, partida.NavesDe(ana.ID).Count);
            Assert.AreEqual(BE.ORIENTACION.Horizontal, partida.NavesDe(ana.ID).Single().Orientacion);
        }

        [TestMethod]
        public void ObtenerOcupacionPropia_DevuelveUnaCopia()
        {
            partida.Iniciar(ana, beto);
            bool[,] copia = partida.ObtenerOcupacionPropia(ana.ID);
            copia[0, 0] = true;
            Assert.IsFalse(partida.ObtenerOcupacionPropia(ana.ID)[0, 0]);
        }

        // -------------------- Combate --------------------

        [TestMethod]
        public void Disparar_AntesDelCombate_Falla()
        {
            partida.Iniciar(ana, beto);
            Assert.IsFalse(partida.PuedeDisparar(ana.ID));
            Assert.ThrowsException<Exception>(() => partida.Disparar(ana.ID, 0, 0));
        }

        [TestMethod]
        public void Disparar_FueraDeTurno_Falla()
        {
            IrAlCombate();
            Assert.IsTrue(partida.PuedeDisparar(ana.ID));
            Assert.IsFalse(partida.PuedeDisparar(beto.ID));
            Exception ex = Assert.ThrowsException<Exception>(() => partida.Disparar(beto.ID, 10, 10));
            StringAssert.Contains(ex.Message, "turno");
        }

        [DataTestMethod]
        [DataRow(-1, 0)]
        [DataRow(0, -1)]
        [DataRow(TAM, 0)]
        [DataRow(0, TAM)]
        public void Disparar_FueraDelTablero_Falla(int fila, int columna)
        {
            IrAlCombate();
            Exception ex = Assert.ThrowsException<Exception>(() => partida.Disparar(ana.ID, fila, columna));
            StringAssert.Contains(ex.Message, "no existe");
            Assert.AreEqual(ana.ID, partida.JugadorEnTurno);
        }

        [TestMethod]
        public void Disparar_Agua_MarcaLaCeldaYPasaElTurno()
        {
            IrAlCombate();

            Assert.AreEqual(BE.ESTADOCELDA.Agua, partida.Disparar(ana.ID, 10, 10));

            Assert.AreEqual(BE.ESTADOCELDA.Agua, partida.ObtenerDisparosRecibidos(beto.ID)[10, 10]);
            Assert.AreEqual(beto.ID, partida.JugadorEnTurno);

            XmlElement disparo = ElementosXml("Disparo").Single();
            Assert.AreEqual("ana", disparo.GetAttribute("jugador"));
            Assert.AreEqual("Agua", disparo.GetAttribute("resultado"));
            Assert.AreEqual("", disparo.GetAttribute("nave"));
        }

        [TestMethod]
        public void Disparar_Tocado_SoloMarcaLaCeldaDisparada()
        {
            IrAlCombate();

            // Batalla Naval de beto: fila 0, columnas 0 a 4.
            Assert.AreEqual(BE.ESTADOCELDA.Tocado, partida.Disparar(ana.ID, 0, 2));

            BE.ESTADOCELDA[,] fog = partida.ObtenerTableroEnemigoFog(ana.ID);
            Assert.AreEqual(BE.ESTADOCELDA.Tocado, fog[0, 2]);
            // La niebla no revela el resto de la nave (regresión: antes se marcaba la nave entera).
            Assert.AreEqual(BE.ESTADOCELDA.Desconocido, fog[0, 0]);
            Assert.AreEqual(BE.ESTADOCELDA.Desconocido, fog[0, 1]);
            Assert.AreEqual(BE.ESTADOCELDA.Desconocido, fog[0, 3]);
            Assert.AreEqual(BE.ESTADOCELDA.Desconocido, fog[0, 4]);
            Assert.AreEqual(beto.ID, partida.JugadorEnTurno);
            Assert.AreEqual("Batalla Naval", ElementosXml("Disparo").Single().GetAttribute("nave"));
        }

        [TestMethod]
        public void Disparar_UnaNaveGrande_SePuedeHundirCeldaPorCelda()
        {
            IrAlCombate();

            for (int c = 0; c < 4; c++)
            {
                Assert.AreEqual(BE.ESTADOCELDA.Tocado, partida.Disparar(ana.ID, 0, c));
                partida.Disparar(beto.ID, 14, c);
            }
            Assert.AreEqual(BE.ESTADOCELDA.Hundido, partida.Disparar(ana.ID, 0, 4));

            BE.ESTADOCELDA[,] fog = partida.ObtenerTableroEnemigoFog(ana.ID);
            for (int c = 0; c < 5; c++)
            {
                Assert.AreEqual(BE.ESTADOCELDA.Hundido, fog[0, c]);
            }
            Assert.AreEqual("4/5 naves a flote", partida.ResumenFlota(beto.ID));
            Assert.AreEqual("5/5 naves a flote", partida.ResumenFlota(ana.ID));
            Assert.IsTrue(partida.EnCurso);
        }

        [TestMethod]
        public void Disparar_MismaCeldaDosVeces_Falla()
        {
            IrAlCombate();
            partida.Disparar(ana.ID, 0, 0);
            partida.Disparar(beto.ID, 14, 14);
            Exception ex = Assert.ThrowsException<Exception>(() => partida.Disparar(ana.ID, 0, 0));
            StringAssert.Contains(ex.Message, "ya fue disparada");
            Assert.AreEqual(ana.ID, partida.JugadorEnTurno);
        }

        [TestMethod]
        public void Disparar_NaveDeUnaCelda_SeHundeDeUnTiro()
        {
            IrAlCombate();
            // Acorazados de beto: fila 4, columna 0.
            Assert.AreEqual(BE.ESTADOCELDA.Hundido, partida.Disparar(ana.ID, 4, 0));
            Assert.AreEqual(BE.ESTADOCELDA.Hundido, partida.ObtenerDisparosRecibidos(beto.ID)[4, 0]);
        }

        [TestMethod]
        public void ObtenerTableroEnemigoFog_NuncaMuestraNavesYEsUnaCopia()
        {
            IrAlCombate();
            BE.ESTADOCELDA[,] fog = partida.ObtenerTableroEnemigoFog(ana.ID);
            foreach (BE.ESTADOCELDA estado in fog)
            {
                Assert.AreEqual(BE.ESTADOCELDA.Desconocido, estado);
            }

            fog[7, 7] = BE.ESTADOCELDA.Agua;
            Assert.AreEqual(BE.ESTADOCELDA.Desconocido, partida.ObtenerTableroEnemigoFog(ana.ID)[7, 7]);
            Assert.AreEqual(BE.ESTADOCELDA.Desconocido, partida.ObtenerDisparosRecibidos(beto.ID)[7, 7]);
        }

        [TestMethod]
        public void HundirTodaLaFlota_TerminaLaPartidaConGanador()
        {
            IrAlCombate();
            int tirosDeBeto = 0;
            for (int i = 0; i < BE.TIPONAVE.Flota.Length; i++)
            {
                for (int c = 0; c < BE.TIPONAVE.Flota[i].Tamaño; c++)
                {
                    BE.ESTADOCELDA resultado = partida.Disparar(ana.ID, i, c);
                    if (!partida.EnCurso)
                    {
                        break;
                    }
                    partida.Disparar(beto.ID, 14, tirosDeBeto++);
                }
            }

            Assert.IsFalse(partida.EnCurso);
            Assert.AreEqual(BLL.FASE_PARTIDA.Finalizada, partida.Fase);
            Assert.AreEqual(ana.ID, partida.Partida.IdGanador);
            Assert.AreEqual("0/5 naves a flote", partida.ResumenFlota(beto.ID));

            Assert.AreEqual(1, mapper.Finalizadas.Count);
            Assert.AreEqual("Victoria por hundimiento total de la flota rival", mapper.Finalizadas[0].Item2);

            Assert.AreEqual(1, finalizadas.Count);
            Assert.AreSame(partida.Partida, finalizadas[0].Item1);
            Assert.IsFalse(finalizadas[0].Item2);

            XmlElement fin = ElementosXml("Fin").Single();
            Assert.AreEqual("ana", fin.GetAttribute("ganador"));
            Assert.AreEqual(10, ElementosXml("Colocacion").Count);
            Assert.AreEqual(15 + 14, ElementosXml("Disparo").Count);

            Assert.ThrowsException<Exception>(() => partida.Disparar(beto.ID, 13, 13));
        }

        [TestMethod]
        public void HundirTodaLaFlota_SinSuscriptores_NoFalla()
        {
            BLL.PARTIDA sinEventos = new BLL.PARTIDA(mapper);
            sinEventos.Iniciar(ana, beto);
            try
            {
                sinEventos.ColocarNave(ana.ID, BE.TIPONAVE.Flota[4], 0, 0, BE.ORIENTACION.Horizontal);
                sinEventos.Abandonar(ana.ID);
                Assert.IsFalse(sinEventos.EnCurso);
            }
            finally
            {
                File.Delete(sinEventos.Partida.RutaXml);
            }
        }

        [TestMethod]
        public void Finalizar_SiLaBaseFalla_LaPartidaSigueEnCurso()
        {
            partida.Iniciar(ana, beto);
            mapper.ResultadoFinalizar = false;

            Exception ex = Assert.ThrowsException<Exception>(() => partida.Abandonar(ana.ID));
            StringAssert.Contains(ex.Message, "fin de la partida");
            Assert.IsTrue(partida.EnCurso);
            Assert.AreEqual(0, finalizadas.Count);
            Assert.AreEqual(0, ElementosXml("Fin").Count);
        }

        // -------------------- Abandono --------------------

        [TestMethod]
        public void Abandonar_LaGanaElOtroJugador()
        {
            IrAlCombate();
            partida.Abandonar(ana.ID);

            Assert.IsFalse(partida.EnCurso);
            Assert.AreEqual(beto.ID, partida.Partida.IdGanador);
            Assert.AreEqual("Partida abandonada por ana", mapper.Finalizadas.Single().Item2);
            Assert.IsTrue(finalizadas.Single().Item2);
            Assert.AreEqual("beto", ElementosXml("Fin").Single().GetAttribute("ganador"));
        }

        [TestMethod]
        public void Abandonar_DuranteLaColocacion_EstaPermitido()
        {
            partida.Iniciar(ana, beto);
            partida.Abandonar(beto.ID);
            Assert.AreEqual(ana.ID, partida.Partida.IdGanador);
        }

        [TestMethod]
        public void Abandonar_UnJugadorAjeno_Falla()
        {
            partida.Iniciar(ana, beto);
            Assert.ThrowsException<Exception>(() => partida.Abandonar(99));
            Assert.IsTrue(partida.EnCurso);
        }

        [TestMethod]
        public void Abandonar_SinPartidaEnCurso_Falla()
        {
            Assert.ThrowsException<Exception>(() => partida.Abandonar(ana.ID));

            partida.Iniciar(ana, beto);
            partida.Abandonar(ana.ID);
            Assert.ThrowsException<Exception>(() => partida.Abandonar(beto.ID));
            Assert.AreEqual(1, mapper.Finalizadas.Count);
        }
    }
}
