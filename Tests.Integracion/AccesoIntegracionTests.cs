using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Integracion
{
    [TestClass]
    public class AccesoIntegracionTests
    {
        // Un named pipe que no existe: la conexión falla enseguida, sin esperar el timeout de red.
        private const string SERVIDOR_INEXISTENTE = @"np:\\.\pipe\BatallaNavalNoExiste\sql\query";

        private DAL.ACCESO acceso;
        private string[] alternativosOriginales;
        private string archivoOriginal;
        private string archivoDelTest;

        [TestInitialize]
        public void Inicializar()
        {
            acceso = new DAL.ACCESO();
            alternativosOriginales = DAL.ACCESO.servidoresAlternativos;
            archivoOriginal = DAL.ACCESO.archivoServidor;
            archivoDelTest = Path.Combine(BaseDePrueba.CarpetaTemporal, Guid.NewGuid().ToString("N"), "servidor.txt");
            DAL.ACCESO.archivoServidor = archivoDelTest;
        }

        [TestCleanup]
        public void Limpiar()
        {
            DAL.ACCESO.servidoresAlternativos = alternativosOriginales;
            DAL.ACCESO.archivoServidor = archivoOriginal;
            DAL.ACCESO.servidorResuelto = BaseDePrueba.Servidor;
        }

        private static string CadenaCon(string servidor)
        {
            SqlConnectionStringBuilder cadena = new SqlConnectionStringBuilder(BaseDePrueba.CadenaBase);
            cadena.DataSource = servidor;
            return cadena.ConnectionString;
        }

        private List<SqlParameter> ParametrosLog(int idUsuario, string descripcion)
        {
            return new List<SqlParameter>
            {
                acceso.CrearParametro("@descripcion", descripcion),
                acceso.CrearParametro("@idUsuario", idUsuario),
                acceso.CrearParametro("@idTipo", BE.LOG.INICIO_SESION),
            };
        }

        private static int LogsConDescripcion(string descripcion)
        {
            return BaseDePrueba.Contar("SELECT COUNT(*) FROM LOG WHERE Descripcion = '" + descripcion + "'");
        }

        // -------------------- Comandos --------------------

        [TestMethod]
        public void Escribir_OkYConError()
        {
            acceso.Abrir();
            try
            {
                Assert.AreEqual(1, acceso.Escribir("LOG_INSERTAR", ParametrosLog(1, "escribir ok")));
                Assert.AreEqual(-1, acceso.Escribir("LOG_INSERTAR", ParametrosLog(999999, "escribir mal")));
            }
            finally
            {
                acceso.Cerrar();
            }
            Assert.AreEqual(1, LogsConDescripcion("escribir ok"));
            Assert.AreEqual(0, LogsConDescripcion("escribir mal"));
        }

        [TestMethod]
        public void LeerEscalar_OkYConError()
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> ok = new List<SqlParameter>
                {
                    acceso.CrearParametro("@usu", "valentina"),
                    acceso.CrearParametro("@pass", "x1"),
                };
                Assert.AreEqual(0, acceso.LeerEscalar("USUARIO_INSERTAR", ok));
                Assert.AreEqual(-1, acceso.LeerEscalar("SP_QUE_NO_EXISTE"));
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        [TestMethod]
        public void Leer_SinParametrosYConError()
        {
            acceso.Abrir();
            try
            {
                List<SqlParameter> parametros = new List<SqlParameter> { acceso.CrearParametro("@id", 1) };
                DataTable tabla = acceso.Leer("USUARIO_BUSCAR_POR_ID", parametros);
                Assert.AreEqual(1, tabla.Rows.Count);
                Assert.ThrowsException<SqlException>(() => acceso.Leer("SP_QUE_NO_EXISTE"));
            }
            finally
            {
                acceso.Cerrar();
            }
        }

        [TestMethod]
        public void CrearParametro_TipaLosValores()
        {
            SqlParameter texto = acceso.CrearParametro("@a", "x");
            Assert.AreEqual("@a", texto.ParameterName);
            Assert.AreEqual("x", texto.Value);
            Assert.AreEqual(DbType.String, texto.DbType);

            SqlParameter numero = acceso.CrearParametro("@b", 5);
            Assert.AreEqual(5, numero.Value);
            Assert.AreEqual(DbType.Int32, numero.DbType);
        }

        // -------------------- Transacciones --------------------

        [TestMethod]
        public void Transaccion_AceptadaQuedaGrabada()
        {
            acceso.Abrir();
            try
            {
                acceso.IniciarTx();
                acceso.Escribir("LOG_INSERTAR", ParametrosLog(1, "tx aceptada"));
                acceso.AceptarTx();
            }
            finally
            {
                acceso.Cerrar();
            }
            Assert.AreEqual(1, LogsConDescripcion("tx aceptada"));
        }

        [TestMethod]
        public void Transaccion_CanceladaSeDeshace()
        {
            acceso.Abrir();
            try
            {
                acceso.IniciarTx();
                acceso.Escribir("LOG_INSERTAR", ParametrosLog(1, "tx cancelada"));
                acceso.CancelarTx();
            }
            finally
            {
                acceso.Cerrar();
            }
            Assert.AreEqual(0, LogsConDescripcion("tx cancelada"));
        }

        [TestMethod]
        public void Transaccion_DeshechaPorSqlServer_CancelarNoFalla()
        {
            acceso.Abrir();
            try
            {
                acceso.IniciarTx();
                acceso.Escribir("LOG_INSERTAR", ParametrosLog(1, "tx abortada"));
                // XACT_ABORT ON en el SP: el error deshace toda la transacción del lado del servidor.
                Assert.AreEqual(-1, acceso.Escribir("LOG_INSERTAR", ParametrosLog(999999, "tx abortada 2")));
                acceso.CancelarTx();
            }
            finally
            {
                acceso.Cerrar();
            }
            Assert.AreEqual(0, LogsConDescripcion("tx abortada"));
        }

        // -------------------- Resolución del servidor --------------------

        [TestMethod]
        public void CadenaConexion_UsaElServidorYaResuelto()
        {
            DAL.ACCESO.servidorResuelto = "otro";
            SqlConnectionStringBuilder cadena = new SqlConnectionStringBuilder(DAL.ACCESO.CadenaConexion("MASTER"));
            Assert.AreEqual("otro", cadena.DataSource);
            Assert.AreEqual("master", cadena.InitialCatalog);
        }

        [TestMethod]
        public void CadenaConexion_SinResolver_PruebaElDelAppConfig()
        {
            DAL.ACCESO.servidorResuelto = null;
            SqlConnectionStringBuilder cadena = new SqlConnectionStringBuilder(DAL.ACCESO.CadenaConexion("BATALLANAVAL"));
            Assert.AreEqual(BaseDePrueba.Servidor, cadena.DataSource);
            Assert.AreEqual(BaseDePrueba.Servidor, DAL.ACCESO.servidorResuelto);
        }

        [TestMethod]
        public void Resolver_ConfiguradoQueResponde_LoUsaYLoRecuerda()
        {
            DAL.ACCESO.servidoresAlternativos = new[] { BaseDePrueba.Servidor };

            Assert.AreEqual(BaseDePrueba.Servidor, DAL.ACCESO.ResolverServidor(BaseDePrueba.CadenaBase));

            CollectionAssert.AreEqual(new[] { BaseDePrueba.Servidor, BaseDePrueba.Servidor }, File.ReadAllLines(archivoDelTest));
        }

        [TestMethod]
        public void Resolver_ConRecordadoDelMismoAppConfig_LoPruebaPrimero()
        {
            // El mismo servidor escrito en minúsculas: si se devuelve así, se usó el recordado.
            string recordado = BaseDePrueba.Servidor.ToLowerInvariant();
            Assert.AreNotEqual(BaseDePrueba.Servidor, recordado);
            Directory.CreateDirectory(Path.GetDirectoryName(archivoDelTest));
            File.WriteAllLines(archivoDelTest, new[] { BaseDePrueba.Servidor, recordado });
            DateTime escrito = File.GetLastWriteTimeUtc(archivoDelTest);

            Assert.AreEqual(recordado, DAL.ACCESO.ResolverServidor(BaseDePrueba.CadenaBase));
            Assert.AreEqual(escrito, File.GetLastWriteTimeUtc(archivoDelTest));
        }

        [TestMethod]
        public void Resolver_ConRecordadoIgualAlConfigurado_NoLoRepite()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(archivoDelTest));
            File.WriteAllLines(archivoDelTest, new[] { BaseDePrueba.Servidor, BaseDePrueba.Servidor });
            Assert.AreEqual(BaseDePrueba.Servidor, DAL.ACCESO.ResolverServidor(BaseDePrueba.CadenaBase));
        }

        // Regresión: antes el servidor recordado ganaba siempre, y cambiar el App.config
        // (como indica el README) no tenía efecto mientras el viejo siguiera respondiendo.
        [TestMethod]
        public void Resolver_ConRecordadoDeOtroAppConfig_LoIgnora()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(archivoDelTest));
            File.WriteAllLines(archivoDelTest, new[] { @".\OTRA_INSTANCIA", BaseDePrueba.Servidor.ToLowerInvariant() });

            Assert.AreEqual(BaseDePrueba.Servidor, DAL.ACCESO.ResolverServidor(BaseDePrueba.CadenaBase));
            CollectionAssert.AreEqual(new[] { BaseDePrueba.Servidor, BaseDePrueba.Servidor }, File.ReadAllLines(archivoDelTest));
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow("una sola linea")]
        public void Resolver_ConArchivoIncompleto_LoIgnora(string contenido)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(archivoDelTest));
            File.WriteAllText(archivoDelTest, contenido);
            Assert.AreEqual(BaseDePrueba.Servidor, DAL.ACCESO.ResolverServidor(BaseDePrueba.CadenaBase));
        }

        [TestMethod]
        public void Resolver_ConRecordadoVacio_LoIgnora()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(archivoDelTest));
            File.WriteAllLines(archivoDelTest, new[] { BaseDePrueba.Servidor, "  " });
            Assert.AreEqual(BaseDePrueba.Servidor, DAL.ACCESO.ResolverServidor(BaseDePrueba.CadenaBase));
        }

        [TestMethod]
        public void Resolver_ConArchivoBloqueado_NoFalla()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(archivoDelTest));
            File.WriteAllText(archivoDelTest, "x");
            using (new FileStream(archivoDelTest, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                Assert.AreEqual(BaseDePrueba.Servidor, DAL.ACCESO.ResolverServidor(BaseDePrueba.CadenaBase));
            }
        }

        [TestMethod]
        public void Resolver_SiNoSePuedeGuardar_NoFalla()
        {
            // Un archivo ocupa el lugar de la carpeta: no se puede crear el directorio.
            string carpeta = Path.GetDirectoryName(archivoDelTest);
            File.WriteAllText(carpeta, "soy un archivo");
            try
            {
                Assert.AreEqual(BaseDePrueba.Servidor, DAL.ACCESO.ResolverServidor(BaseDePrueba.CadenaBase));
                Assert.IsFalse(File.Exists(archivoDelTest));
            }
            finally
            {
                File.Delete(carpeta);
            }
        }

        [TestMethod]
        public void Resolver_ConfiguradoQueNoResponde_UsaUnaAlternativa()
        {
            DAL.ACCESO.servidoresAlternativos = new[] { SERVIDOR_INEXISTENTE, BaseDePrueba.Servidor };

            Assert.AreEqual(BaseDePrueba.Servidor, DAL.ACCESO.ResolverServidor(CadenaCon(SERVIDOR_INEXISTENTE)));
            CollectionAssert.AreEqual(new[] { SERVIDOR_INEXISTENTE, BaseDePrueba.Servidor }, File.ReadAllLines(archivoDelTest));
        }

        [TestMethod]
        public void Resolver_NingunoResponde_DevuelveElConfiguradoSinRecordarlo()
        {
            DAL.ACCESO.servidorResuelto = null;
            DAL.ACCESO.servidoresAlternativos = new string[0];

            Assert.AreEqual(SERVIDOR_INEXISTENTE, DAL.ACCESO.ResolverServidor(CadenaCon(SERVIDOR_INEXISTENTE)));
            Assert.IsNull(DAL.ACCESO.servidorResuelto);
            Assert.IsFalse(File.Exists(archivoDelTest));
        }
    }
}
