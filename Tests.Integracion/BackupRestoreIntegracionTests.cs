using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Integracion
{
    // Backup y restore reales sobre la base de prueba (BLL.BACKUPRESTORE -> DAL.BACKUPRESTORE).
    [TestClass]
    public class BackupRestoreIntegracionTests
    {
        private string carpeta;

        [TestInitialize]
        public void Inicializar()
        {
            carpeta = Path.Combine(BaseDePrueba.CarpetaTemporal, "backups_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(carpeta);
        }

        [TestCleanup]
        public void Limpiar()
        {
            DAL.ACCESO.servidorResuelto = BaseDePrueba.Servidor;
        }

        private static bool Existe(string nombre)
        {
            return BaseDePrueba.Contar("SELECT COUNT(*) FROM USUARIO WHERE Nombre = '" + nombre + "'") == 1;
        }

        [TestMethod]
        public void CarpetaBackupPorDefecto_EsLaQueInformaElServidor()
        {
            // LocalDB (y SQL Server viejos) no la informan: en ese caso se devuelve "".
            object valor = BaseDePrueba.Escalar("SELECT SERVERPROPERTY('InstanceDefaultBackupPath')");
            string esperada = valor == DBNull.Value ? "" : valor.ToString();

            Assert.AreEqual(esperada, BLL.BACKUPRESTORE.CarpetaBackupPorDefecto());
        }

        [TestMethod]
        public void BackupYRestore_VuelveLaBaseAlEstadoDelBackup()
        {
            BE.USUARIO antes = BaseDePrueba.CrearUsuario();
            // Comilla simple en la ruta: se tiene que escapar, no romper el comando.
            string ruta = Path.Combine(carpeta, "copia d'prueba.bak");

            Assert.AreEqual("", BLL.BACKUPRESTORE.HacerBackup(ruta));
            Assert.IsTrue(File.Exists(ruta));

            BE.USUARIO despues = BaseDePrueba.CrearUsuario();
            Assert.IsTrue(Existe(despues.Nombre));

            bool restaurada;
            Assert.AreEqual("", BLL.BACKUPRESTORE.HacerRestore(ruta, out restaurada));
            Assert.IsTrue(restaurada);

            // La app sigue funcionando después del restore (se vació el pool de conexiones).
            Assert.IsTrue(Existe(antes.Nombre));
            Assert.IsFalse(Existe(despues.Nombre));
            Assert.IsTrue(new BLL.USUARIO().Login(new BE.USUARIO { Nombre = antes.Nombre, Contraseña = "clave1" }));
            Assert.AreEqual("MULTI_USER", BaseDePrueba.Escalar("SELECT user_access_desc FROM sys.databases WHERE name = 'BATALLANAVAL'"));
        }

        [TestMethod]
        public void Backup_AUnaRutaInvalida_DevuelveElError()
        {
            string error = BLL.BACKUPRESTORE.HacerBackup(Path.Combine(carpeta, "no", "existe", "x.bak"));
            Assert.AreNotEqual("", error);
        }

        [TestMethod]
        public void Restore_DeUnArchivoInexistente_DejaLaBaseUsable()
        {
            bool restaurada;
            string error = BLL.BACKUPRESTORE.HacerRestore(Path.Combine(carpeta, "no_existe.bak"), out restaurada);

            Assert.AreNotEqual("", error);
            Assert.IsFalse(restaurada);
            Assert.AreEqual("MULTI_USER", BaseDePrueba.Escalar("SELECT user_access_desc FROM sys.databases WHERE name = 'BATALLANAVAL'"));
            Assert.IsNotNull(new BLL.USUARIO().BuscarPorId(1));
        }

        [TestMethod]
        public void Restore_SiLaBaseNoExiste_DevuelveElPrimerError()
        {
            string ruta = Path.Combine(carpeta, "resguardo.bak");
            Assert.AreEqual("", BLL.BACKUPRESTORE.HacerBackup(ruta));
            BaseDePrueba.EjecutarEnMaster(
                "ALTER DATABASE BATALLANAVAL SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE BATALLANAVAL");
            try
            {
                // Falla el SINGLE_USER y también el MULTI_USER del finally: se informa el primero.
                bool restaurada;
                string error = BLL.BACKUPRESTORE.HacerRestore(Path.Combine(carpeta, "no_existe.bak"), out restaurada);
                Assert.IsFalse(restaurada);
                StringAssert.Contains(error, "BATALLANAVAL");
            }
            finally
            {
                BaseDePrueba.EjecutarEnMaster("RESTORE DATABASE BATALLANAVAL FROM DISK = '" + ruta.Replace("'", "''") + "'");
                System.Data.SqlClient.SqlConnection.ClearAllPools();
            }
            Assert.IsNotNull(new BLL.USUARIO().BuscarPorId(1));
        }

        [TestMethod]
        public void SinServidor_TodoDevuelveError()
        {
            DAL.ACCESO.servidorResuelto = @"np:\\.\pipe\BatallaNavalNoExiste\sql\query";

            Assert.AreEqual("", BLL.BACKUPRESTORE.CarpetaBackupPorDefecto());
            Assert.AreNotEqual("", BLL.BACKUPRESTORE.HacerBackup(Path.Combine(carpeta, "x.bak")));

            bool restaurada;
            Assert.AreNotEqual("", BLL.BACKUPRESTORE.HacerRestore(Path.Combine(carpeta, "x.bak"), out restaurada));
            Assert.IsFalse(restaurada);
        }
    }
}
