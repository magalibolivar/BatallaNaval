using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Unitarios
{
    [TestClass]
    public class UsuarioTests
    {
        private MapperUsuarioFalso mapper;
        private MapperLogFalso mapperLog;
        private Func<DAL.MP_LOG> crearMapperOriginal;
        private BLL.USUARIO bll;

        [TestInitialize]
        public void Inicializar()
        {
            mapper = new MapperUsuarioFalso();
            mapperLog = new MapperLogFalso();
            crearMapperOriginal = BLL.BITACORA.CrearMapper;
            BLL.BITACORA.CrearMapper = () => mapperLog;
            bll = new BLL.USUARIO(mapper);
        }

        [TestCleanup]
        public void Limpiar()
        {
            BLL.BITACORA.CrearMapper = crearMapperOriginal;
        }

        private static BE.USUARIO Usuario(string nombre, string contraseña)
        {
            return new BE.USUARIO { Nombre = nombre, Contraseña = contraseña };
        }

        // -------------------- ValidarRegistro --------------------

        [DataTestMethod]
        [DataRow("ana", "1234")]
        [DataRow("abcdefghijklmnopqrst", "abcdefghijklmnopqrs1")]
        [DataRow("ana_2", "a b 1")]
        public void ValidarRegistro_DatosValidos_DevuelveVacio(string nombre, string contraseña)
        {
            Assert.AreEqual("", bll.ValidarRegistro(Usuario(nombre, contraseña)));
        }

        [DataTestMethod]
        [DataRow(null, "1234", "Falta completar")]
        [DataRow("ana", null, "Falta completar")]
        [DataRow("an a", "1234", "espacios")]
        [DataRow("ab", "1234", "nombre de usuario tiene que tener entre 3 y 20")]
        [DataRow("abcdefghijklmnopqrstu", "1234", "nombre de usuario tiene que tener entre 3 y 20")]
        [DataRow("ana", "123", "contraseña tiene que tener entre 4 y 20")]
        [DataRow("ana", "abcdefghijklmnopqrst1", "contraseña tiene que tener entre 4 y 20")]
        [DataRow("ana", "abcd", "al menos un número")]
        public void ValidarRegistro_DatosInvalidos_DevuelveElMotivo(string nombre, string contraseña, string motivo)
        {
            StringAssert.Contains(bll.ValidarRegistro(Usuario(nombre, contraseña)), motivo);
        }

        [DataTestMethod]
        [DataRow("Invitado1")]
        [DataRow("invitado2")]
        [DataRow("INVITADO1")]
        public void ValidarRegistro_NombreDeInvitado_EstaReservado(string nombre)
        {
            StringAssert.Contains(bll.ValidarRegistro(Usuario(nombre, "1234")), "reservado para los invitados");
        }

        // -------------------- Invitados --------------------

        [DataTestMethod]
        [DataRow("Invitado1", true)]
        [DataRow("invitado2", true)]
        [DataRow(" Invitado2 ", true)]
        [DataRow("Invitado3", false)]
        [DataRow("ana", false)]
        [DataRow(null, false)]
        public void EsInvitado_ReconoceLosNombresReservados(string nombre, bool esperado)
        {
            Assert.AreEqual(esperado, BLL.USUARIO.EsInvitado(nombre));
            Assert.AreEqual(esperado, BLL.USUARIO.EsInvitado(new BE.USUARIO { Nombre = nombre }));
        }

        [TestMethod]
        public void EsInvitado_UsuarioNull_EsFalse()
        {
            Assert.IsFalse(BLL.USUARIO.EsInvitado((BE.USUARIO)null));
        }

        [TestMethod]
        public void Login_ComoInvitado_SeRechazaSinIrALaBase()
        {
            Assert.IsFalse(bll.Login(Usuario("Invitado1", "")));
            Assert.IsFalse(bll.Login(Usuario("invitado2", "1234")));
            Assert.AreEqual(0, mapper.LoginsLlamados);
            Assert.AreEqual(0, mapperLog.Registrados.Count);
        }

        [DataTestMethod]
        [DataRow(1, 20, "Invitado1")]
        [DataRow(2, 21, "Invitado2")]
        public void IngresarComoInvitado_DevuelveElInvitadoYRegistraElInicio(int numero, int idEsperado, string nombre)
        {
            mapper.Usuarios[20] = new BE.USUARIO { ID = 20, Nombre = "Invitado1", Contraseña = "" };
            mapper.Usuarios[21] = new BE.USUARIO { ID = 21, Nombre = "Invitado2", Contraseña = "" };

            BE.USUARIO invitado = bll.IngresarComoInvitado(numero);

            Assert.AreEqual(idEsperado, invitado.ID);
            Assert.AreEqual(nombre, invitado.Nombre);
            Assert.AreEqual(1, mapperLog.Registrados.Count);
            Assert.AreEqual(idEsperado, mapperLog.Registrados[0].IdUsuario);
            Assert.AreEqual(BE.LOG.INICIO_SESION, mapperLog.Registrados[0].IdTipo);
            StringAssert.Contains(mapperLog.Registrados[0].Descripcion, "(invitado)");
        }

        [TestMethod]
        public void IngresarComoInvitado_SinLosInvitadosEnLaBase_PideCorrerElScript()
        {
            Exception ex = Assert.ThrowsException<Exception>(() => bll.IngresarComoInvitado(1));
            StringAssert.Contains(ex.Message, "Script.sql");
            Assert.AreEqual(0, mapperLog.Registrados.Count);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(3)]
        public void IngresarComoInvitado_NumeroInvalido_Falla(int numero)
        {
            Assert.ThrowsException<Exception>(() => bll.IngresarComoInvitado(numero));
        }

        // -------------------- Insertar --------------------

        [TestMethod]
        public void Insertar_Invalido_FallaSinLlegarALaBase()
        {
            Exception ex = Assert.ThrowsException<Exception>(() => bll.Insertar(Usuario("a", "1234")));
            StringAssert.Contains(ex.Message, "entre 3 y 20");
            Assert.AreEqual(0, mapper.Insertados.Count);
        }

        [TestMethod]
        public void Insertar_Ok_CompletaElId()
        {
            BE.USUARIO u = Usuario("nuevo", "pass1");
            Assert.IsTrue(bll.Insertar(u));
            Assert.AreEqual(10, u.ID);
        }

        [TestMethod]
        public void Insertar_NombreRepetido_DevuelveFalse()
        {
            mapper.IdAlInsertar = 0;
            BE.USUARIO u = Usuario("repetido", "pass1");
            Assert.IsFalse(bll.Insertar(u));
            Assert.AreEqual(0, u.ID);
        }

        [TestMethod]
        public void Insertar_ErrorDeBase_Falla()
        {
            mapper.IdAlInsertar = -1;
            Exception ex = Assert.ThrowsException<Exception>(() => bll.Insertar(Usuario("nuevo", "pass1")));
            StringAssert.Contains(ex.Message, "No se pudo registrar");
        }

        // -------------------- Login / Logout --------------------

        [TestMethod]
        public void Login_Ok_RegistraElInicioDeSesion()
        {
            BE.USUARIO u = Usuario("ana", "1234");
            Assert.IsTrue(bll.Login(u));

            Assert.AreEqual(3, u.ID);
            BE.LOG log = mapperLog.Registrados[0];
            Assert.AreEqual(1, mapperLog.Registrados.Count);
            Assert.AreEqual(3, log.IdUsuario);
            Assert.AreEqual(BE.LOG.INICIO_SESION, log.IdTipo);
            Assert.AreEqual("Inicio de sesión de ana", log.Descripcion);
        }

        [TestMethod]
        public void Login_Incorrecto_NoRegistraNada()
        {
            mapper.ResultadoLogin = false;
            Assert.IsFalse(bll.Login(Usuario("ana", "mal1")));
            Assert.AreEqual(0, mapperLog.Registrados.Count);
        }

        [TestMethod]
        public void Logout_RegistraElCierreDeSesion()
        {
            bll.Logout(new BE.USUARIO { ID = 4, Nombre = "beto" });

            BE.LOG log = mapperLog.Registrados[0];
            Assert.AreEqual(4, log.IdUsuario);
            Assert.AreEqual(BE.LOG.CIERRE_SESION, log.IdTipo);
            Assert.AreEqual("Cierre de sesión de beto", log.Descripcion);
        }

        [TestMethod]
        public void BuscarPorId_DelegaEnElMapper()
        {
            BE.USUARIO ana = new BE.USUARIO { ID = 1, Nombre = "ana" };
            mapper.Usuarios[1] = ana;
            Assert.AreSame(ana, bll.BuscarPorId(1));
            Assert.IsNull(bll.BuscarPorId(2));
        }

        [TestMethod]
        public void ConstructorPublico_UsaElMapperReal()
        {
            Assert.AreEqual("", new BLL.USUARIO().ValidarRegistro(Usuario("ana", "1234")));
        }

        // -------------------- BITACORA --------------------

        [TestMethod]
        public void Bitacora_SiLaBaseFalla_Falla()
        {
            mapperLog.Resultado = -1;
            Exception ex = Assert.ThrowsException<Exception>(() => BLL.BITACORA.Registrar(1, BE.LOG.INICIO_SESION, "algo"));
            StringAssert.Contains(ex.Message, "algo");
        }

        [TestMethod]
        public void Bitacora_PorDefectoUsaElMapperReal()
        {
            Assert.IsInstanceOfType(crearMapperOriginal(), typeof(DAL.MP_LOG));
        }
    }
}
