using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Integracion
{
    // Prepara una base BATALLANAVAL limpia (con el mismo Script.sql de la entrega) en la
    // instancia del App.config de los tests, y ofrece consultas directas para verificar.
    [TestClass]
    public static class BaseDePrueba
    {
        // Marca que deja la base creada por los tests: solo se borra una base que la tenga.
        private const string MARCA = "CreadaPorTestsBatallaNaval";

        public static string Servidor { get; private set; }
        public static string CarpetaTemporal { get; private set; }

        private static int contador;

        [AssemblyInitialize]
        public static void Preparar(TestContext contexto)
        {
            Servidor = new SqlConnectionStringBuilder(
                ConfigurationManager.ConnectionStrings["BATALLANAVAL"].ConnectionString).DataSource;

            CarpetaTemporal = Path.Combine(Path.GetTempPath(), "BatallaNavalIntegracion_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(CarpetaTemporal);

            // Que la DAL no use el servidor recordado de la aplicación real ni pruebe otras
            // instancias (en esta compu podría estar la base de verdad en .\SQLEXPRESS).
            DAL.ACCESO.servidoresAlternativos = new string[0];
            DAL.ACCESO.archivoServidor = Path.Combine(CarpetaTemporal, "servidor.txt");
            DAL.ACCESO.servidorResuelto = null;

            RecrearBase();

            string resuelto = new SqlConnectionStringBuilder(DAL.ACCESO.CadenaConexion("BATALLANAVAL")).DataSource;
            if (resuelto != Servidor)
            {
                throw new InvalidOperationException("La DAL resolvió " + resuelto + " en lugar de " + Servidor);
            }
        }

        [AssemblyCleanup]
        public static void Terminar()
        {
            SqlConnection.ClearAllPools();
            try
            {
                Directory.Delete(CarpetaTemporal, true);
            }
            catch (IOException)
            {
            }
        }

        private static void RecrearBase()
        {
            using (SqlConnection conexion = new SqlConnection(CadenaMaster(60)))
            {
                conexion.Open();

                object existe = new SqlCommand("SELECT DB_ID('BATALLANAVAL')", conexion).ExecuteScalar();
                if (existe != DBNull.Value)
                {
                    object marca = new SqlCommand(
                        "SELECT value FROM BATALLANAVAL.sys.extended_properties WHERE class = 0 AND name = '" + MARCA + "'",
                        conexion).ExecuteScalar();
                    if (marca == null)
                    {
                        throw new InvalidOperationException(
                            "En " + Servidor + " ya existe una base BATALLANAVAL que no crearon los tests: no se borra. " +
                            "Configurar otra instancia en Tests.Integracion/App.config.");
                    }
                    new SqlCommand(
                        "ALTER DATABASE BATALLANAVAL SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE BATALLANAVAL",
                        conexion).ExecuteNonQuery();
                }

                string script = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Script.sql"));
                foreach (string lote in Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
                {
                    if (lote.Trim() != "")
                    {
                        new SqlCommand(lote, conexion).ExecuteNonQuery();
                    }
                }

                new SqlCommand(
                    "EXEC BATALLANAVAL.sys.sp_addextendedproperty @name = N'" + MARCA + "', @value = N'1'",
                    conexion).ExecuteNonQuery();
            }
        }

        private static string CadenaMaster(int timeout)
        {
            SqlConnectionStringBuilder cadena = new SqlConnectionStringBuilder(
                ConfigurationManager.ConnectionStrings["MASTER"].ConnectionString);
            cadena.ConnectTimeout = timeout;
            return cadena.ConnectionString;
        }

        public static string CadenaBase
        {
            get { return ConfigurationManager.ConnectionStrings["BATALLANAVAL"].ConnectionString; }
        }

        // -------------------- Consultas para verificar --------------------

        public static object Escalar(string sql)
        {
            using (SqlConnection conexion = new SqlConnection(CadenaBase))
            {
                conexion.Open();
                return new SqlCommand(sql, conexion).ExecuteScalar();
            }
        }

        public static int Contar(string sql)
        {
            return Convert.ToInt32(Escalar(sql));
        }

        public static void Ejecutar(string sql)
        {
            using (SqlConnection conexion = new SqlConnection(CadenaBase))
            {
                conexion.Open();
                new SqlCommand(sql, conexion).ExecuteNonQuery();
            }
        }

        public static void EjecutarEnMaster(string sql)
        {
            using (SqlConnection conexion = new SqlConnection(CadenaMaster(30)))
            {
                conexion.Open();
                new SqlCommand(sql, conexion).ExecuteNonQuery();
            }
        }

        // Nombre de usuario único (máximo 20 caracteres) para que cada test use sus propios datos.
        public static string NombreUnico()
        {
            contador++;
            return "t" + contador + "_" + Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        // Registra un usuario nuevo con la BLL y lo devuelve con su ID.
        public static BE.USUARIO CrearUsuario()
        {
            BE.USUARIO usuario = new BE.USUARIO { Nombre = NombreUnico(), Contraseña = "clave1" };
            Assert.IsTrue(new BLL.USUARIO().Insertar(usuario));
            return usuario;
        }

        // Mientras exista, cualquier insert en LOG de ese usuario falla (para probar los rollbacks).
        public static IDisposable BloquearBitacora(int idUsuario)
        {
            Ejecutar(
                "CREATE TRIGGER TR_TEST_BLOQUEAR_LOG ON LOG AFTER INSERT AS " +
                "IF EXISTS (SELECT 1 FROM inserted WHERE ID_Usuario = " + idUsuario + ") " +
                "THROW 50000, 'Bitácora bloqueada por el test', 1;");
            return new AlTerminar(() => Ejecutar("DROP TRIGGER TR_TEST_BLOQUEAR_LOG"));
        }

        private class AlTerminar : IDisposable
        {
            private readonly Action accion;
            public AlTerminar(Action accion) { this.accion = accion; }
            public void Dispose() { accion(); }
        }
    }
}
