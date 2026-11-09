using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace DAL
{
    // internal: solo los mappers de la DAL hablan con la base. Ni la BLL ni la GUI
    // pueden crear un ACCESO y ejecutar SQL por su cuenta.
    internal class ACCESO
    {
        private SqlConnection conexion;
        private SqlTransaction transaccion;

        // Instancias de SQL Server que se prueban si la del App.config no responde:
        // con nombre (SQL Express) o la instancia por defecto (como en las compus de la facu).
        // (internal y no readonly solo para que los tests puedan reemplazarlas)
        internal static string[] servidoresAlternativos = { @".\SQLEXPRESS", "." };

        // Servidor que respondió la primera vez; se reutiliza para no volver a probar.
        internal static string servidorResuelto;

        // Además se guarda en un archivo de esta compu, así en la próxima ejecución se prueba
        // primero y no hay que esperar a que fallen los que no existen. El archivo guarda
        // en la primera línea el Data Source del App.config y en la segunda el que respondió:
        // si alguien cambia el App.config, lo recordado deja de valer y se vuelve a resolver.
        internal static string archivoServidor = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BatallaNaval", "servidor.txt");

        // La cadena de conexión se lee del App.config de la GUI (connectionStrings "BATALLANAVAL"),
        // así se puede apuntar a otra instancia de SQL Server sin recompilar.
        // El Data Source se reemplaza por el servidor que efectivamente respondió.
        public static string CadenaConexion(string nombre)
        {
            SqlConnectionStringBuilder cadena = new SqlConnectionStringBuilder(
                ConfigurationManager.ConnectionStrings[nombre].ConnectionString);
            cadena.DataSource = ResolverServidor();
            return cadena.ConnectionString;
        }

        private static string ResolverServidor()
        {
            if (servidorResuelto != null)
            {
                return servidorResuelto;
            }
            return ResolverServidor(ConfigurationManager.ConnectionStrings["BATALLANAVAL"].ConnectionString);
        }

        // Prueba primero el servidor que funcionó la última vez en esta compu (si el App.config
        // no cambió desde entonces), después el Data Source del App.config y por último las
        // alternativas, y se queda con el primero donde se pueda abrir la base BATALLANAVAL.
        internal static string ResolverServidor(string cadenaConfigurada)
        {
            SqlConnectionStringBuilder cadena = new SqlConnectionStringBuilder(cadenaConfigurada);
            string configurado = cadena.DataSource;

            List<string> candidatos = new List<string>();
            string recordado = LeerServidorRecordado(configurado);
            if (recordado != null)
            {
                candidatos.Add(recordado);
            }
            if (!candidatos.Contains(configurado))
            {
                candidatos.Add(configurado);
            }
            foreach (string servidor in servidoresAlternativos)
            {
                if (!candidatos.Contains(servidor))
                {
                    candidatos.Add(servidor);
                }
            }

            cadena.ConnectTimeout = 5;
            foreach (string servidor in candidatos)
            {
                cadena.DataSource = servidor;
                try
                {
                    using (SqlConnection prueba = new SqlConnection(cadena.ConnectionString))
                    {
                        prueba.Open();
                    }
                    servidorResuelto = servidor;
                    if (servidor != recordado)
                    {
                        GuardarServidorRecordado(configurado, servidor);
                    }
                    return servidorResuelto;
                }
                catch (SqlException)
                {
                    // No responde o no tiene la base: se prueba el siguiente.
                }
            }

            // Ninguno respondió: se usa el configurado, así el error que ve el usuario es el original.
            return configurado;
        }

        // Si el archivo no existe, no se puede leer o se guardó con otro App.config,
        // no hay servidor recordado y se prueban los demás.
        private static string LeerServidorRecordado(string configurado)
        {
            try
            {
                if (File.Exists(archivoServidor))
                {
                    string[] lineas = File.ReadAllLines(archivoServidor);
                    if (lineas.Length >= 2 && lineas[0].Trim() == configurado && lineas[1].Trim() != "")
                    {
                        return lineas[1].Trim();
                    }
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        // Si no se puede guardar no pasa nada: la próxima vez se vuelve a probar.
        private static void GuardarServidorRecordado(string configurado, string servidor)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(archivoServidor));
                File.WriteAllLines(archivoServidor, new string[] { configurado, servidor });
            }
            catch (Exception)
            {
            }
        }

        public void Abrir()
        {
            conexion = new SqlConnection();
            conexion.ConnectionString = CadenaConexion("BATALLANAVAL");
            conexion.Open();
        }

        public void Cerrar()
        {
            conexion.Close();
            conexion = null;
            transaccion = null;
        }

        // -------------------- Transacciones --------------------
        // La transacción la crea la conexión (BeginTransaction). Todos los comandos que se
        // ejecuten mientras esté abierta quedan dentro de ella, hasta el Commit o el Rollback.

        public void IniciarTx()
        {
            transaccion = conexion.BeginTransaction();
        }

        public void AceptarTx()
        {
            transaccion.Commit();
            transaccion = null;
        }

        public void CancelarTx()
        {
            // Si SQL Server ya deshizo la transacción por su cuenta (por un error grave),
            // la transacción queda sin conexión y no hay nada para deshacer.
            if (transaccion.Connection != null)
            {
                transaccion.Rollback();
            }
            transaccion = null;
        }

        private SqlCommand CrearComando(string sql, List<SqlParameter> parametros = null)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conexion;
            if (transaccion != null)
            {
                cmd.Transaction = transaccion;
            }
            if (parametros != null)
            {
                cmd.Parameters.AddRange(parametros.ToArray());
            }
            return cmd;
        }

        // INSERT / UPDATE / DELETE. Devuelve las filas afectadas, o -1 si falló.
        public int Escribir(string sql, List<SqlParameter> parametros = null)
        {
            SqlCommand cmd = CrearComando(sql, parametros);

            int filas = 0;
            try
            {
                filas = cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                filas = -1;
            }
            return filas;
        }

        // Un solo valor (por ejemplo el ID generado). Devuelve -1 si falló.
        public int LeerEscalar(string sql, List<SqlParameter> parametros = null)
        {
            SqlCommand cmd = CrearComando(sql, parametros);

            int res = 0;
            try
            {
                res = int.Parse(cmd.ExecuteScalar().ToString());
            }
            catch (Exception)
            {
                res = -1;
            }
            return res;
        }

        // Varias filas, en modo desconectado: el adaptador llena un DataTable en memoria.
        public DataTable Leer(string sql, List<SqlParameter> parametros = null)
        {
            SqlDataAdapter adaptador = new SqlDataAdapter();
            adaptador.SelectCommand = CrearComando(sql, parametros);

            DataTable tabla = new DataTable();
            adaptador.Fill(tabla);
            return tabla;
        }

        public SqlParameter CrearParametro(string nombre, string valor)
        {
            SqlParameter parametro = new SqlParameter();
            parametro.ParameterName = nombre;
            parametro.Value = valor;
            parametro.DbType = DbType.String;
            return parametro;
        }

        public SqlParameter CrearParametro(string nombre, int valor)
        {
            SqlParameter parametro = new SqlParameter();
            parametro.ParameterName = nombre;
            parametro.Value = valor;
            parametro.DbType = DbType.Int32;
            return parametro;
        }
    }
}
