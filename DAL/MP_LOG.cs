using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL
{
    public class MP_LOG : MAPPER<BE.LOG>
    {
        // Devuelve las filas insertadas (1), o -1 si falló.
        public override int Insertar(BE.LOG log)
        {
            acceso = new ACCESO();
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(acceso.CrearParametro("@descripcion", log.Descripcion));
            parametros.Add(acceso.CrearParametro("@idUsuario", log.IdUsuario));
            parametros.Add(acceso.CrearParametro("@idTipo", log.IdTipo));

            acceso.Abrir();
            int res;
            try
            {
                res = acceso.Escribir("LOG_INSERTAR", parametros);
            }
            finally
            {
                acceso.Cerrar();
            }
            return res;
        }
    }
}
