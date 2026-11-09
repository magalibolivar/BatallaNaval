using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Integracion
{
    // BLL.USUARIO + BITACORA contra la base real (MP_USUARIO, MP_LOG y los SP de Script.sql).
    [TestClass]
    public class UsuarioIntegracionTests
    {
        private BLL.USUARIO bll = new BLL.USUARIO();

        private static int LogsDe(int idUsuario, int idTipo)
        {
            return BaseDePrueba.Contar("SELECT COUNT(*) FROM LOG WHERE ID_Usuario = " + idUsuario + " AND ID_TIPO = " + idTipo);
        }

        [TestMethod]
        public void Registrar_YLoguear_RegistraElInicioDeSesion()
        {
            string nombre = BaseDePrueba.NombreUnico();
            BE.USUARIO nuevo = new BE.USUARIO { Nombre = nombre, Contraseña = "Clave1" };
            Assert.IsTrue(bll.Insertar(nuevo));
            Assert.IsTrue(nuevo.ID > 4);

            // El nombre no distingue mayúsculas; la base devuelve el nombre como se registró.
            BE.USUARIO login = new BE.USUARIO { Nombre = nombre.ToUpper(), Contraseña = "Clave1" };
            Assert.IsTrue(bll.Login(login));
            Assert.AreEqual(nuevo.ID, login.ID);
            Assert.AreEqual(nombre, login.Nombre);
            Assert.AreEqual("Clave1", login.Contraseña);
            Assert.AreEqual(1, LogsDe(nuevo.ID, BE.LOG.INICIO_SESION));
            Assert.AreEqual("Inicio de sesión de " + nombre,
                BaseDePrueba.Escalar("SELECT Descripcion FROM LOG WHERE ID_Usuario = " + nuevo.ID));
        }

        [TestMethod]
        public void Registrar_NombreExistente_DevuelveFalse()
        {
            Assert.IsFalse(bll.Insertar(new BE.USUARIO { Nombre = "valentina", Contraseña = "otra1" }));
            Assert.IsFalse(bll.Insertar(new BE.USUARIO { Nombre = "VALENTINA", Contraseña = "otra1" }));
        }

        [TestMethod]
        public void Registrar_Invalido_NoLlegaALaBase()
        {
            int antes = BaseDePrueba.Contar("SELECT COUNT(*) FROM USUARIO");
            Assert.ThrowsException<Exception>(() => bll.Insertar(new BE.USUARIO { Nombre = "con espacio", Contraseña = "1234" }));
            Assert.AreEqual(antes, BaseDePrueba.Contar("SELECT COUNT(*) FROM USUARIO"));
        }

        [DataTestMethod]
        [DataRow("valentina", "12345")]   // contraseña incorrecta
        [DataRow("valentina", "1234 ")]   // espacio al final
        [DataRow("noexiste", "1234")]     // usuario inexistente
        public void Login_Incorrecto_DevuelveFalse(string nombre, string contraseña)
        {
            Assert.IsFalse(bll.Login(new BE.USUARIO { Nombre = nombre, Contraseña = contraseña }));
        }

        [TestMethod]
        public void Login_LaContraseñaDistingueMayusculas()
        {
            BE.USUARIO u = BaseDePrueba.CrearUsuario();
            Assert.IsFalse(bll.Login(new BE.USUARIO { Nombre = u.Nombre, Contraseña = "CLAVE1" }));
            Assert.AreEqual(0, LogsDe(u.ID, BE.LOG.INICIO_SESION));
        }

        [TestMethod]
        public void Logout_RegistraElCierreDeSesion()
        {
            BE.USUARIO u = BaseDePrueba.CrearUsuario();
            bll.Logout(u);
            Assert.AreEqual(1, LogsDe(u.ID, BE.LOG.CIERRE_SESION));
        }

        [TestMethod]
        public void BuscarPorId_ExistenteEInexistente()
        {
            BE.USUARIO valentina = bll.BuscarPorId(1);
            Assert.AreEqual("valentina", valentina.Nombre);
            Assert.AreEqual("1234", valentina.Contraseña);
            Assert.AreEqual(1, valentina.ID);
            Assert.IsNull(bll.BuscarPorId(999999));
        }

        [TestMethod]
        public void Bitacora_ConUsuarioInexistente_Falla()
        {
            Exception ex = Assert.ThrowsException<Exception>(() =>
                BLL.BITACORA.Registrar(999999, BE.LOG.INICIO_SESION, "fantasma"));
            StringAssert.Contains(ex.Message, "fantasma");
        }
    }
}
