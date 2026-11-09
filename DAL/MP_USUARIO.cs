using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class MP_USUARIO : MAPPER<BE.USUARIO>
    {
        // Devuelve el ID del usuario nuevo, 0 si el nombre ya existe, o -1 si falló.
        public override int Insertar(BE.USUARIO usuario)
        {
            acceso = new ACCESO();
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(acceso.CrearParametro("@usu", usuario.Nombre));
            parametros.Add(acceso.CrearParametro("@pass", usuario.Contraseña));

            acceso.Abrir();
            int id;
            try
            {
                id = acceso.LeerEscalar("USUARIO_INSERTAR", parametros);
            }
            finally
            {
                acceso.Cerrar();
            }
            return id;
        }

        // Si el usuario y la contraseña son correctos, completa el ID y devuelve true.
        public virtual bool Login(BE.USUARIO usuario)
        {
            acceso = new ACCESO();
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(acceso.CrearParametro("@usu", usuario.Nombre));
            parametros.Add(acceso.CrearParametro("@pass", usuario.Contraseña));

            acceso.Abrir();
            DataTable tabla;
            try
            {
                tabla = acceso.Leer("USUARIO_LOGIN", parametros);
            }
            finally
            {
                acceso.Cerrar();
            }

            if (tabla.Rows.Count == 0)
            {
                return false;
            }

            DataRow fila = tabla.Rows[0];
            usuario.ID = int.Parse(fila["ID"].ToString());
            usuario.Nombre = fila["Nombre"].ToString();
            usuario.Contraseña = fila["Contraseña"].ToString();
            return true;
        }

        // Devuelve null si no existe un usuario con ese ID.
        public virtual BE.USUARIO BuscarPorId(int id)
        {
            acceso = new ACCESO();
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(acceso.CrearParametro("@id", id));
            return LeerUno("USUARIO_BUSCAR_POR_ID", parametros);
        }

        // Devuelve null si no existe un usuario con ese nombre (se usa para los invitados).
        public virtual BE.USUARIO BuscarPorNombre(string nombre)
        {
            acceso = new ACCESO();
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(acceso.CrearParametro("@usu", nombre));
            return LeerUno("USUARIO_BUSCAR_POR_NOMBRE", parametros);
        }

        private BE.USUARIO LeerUno(string procedimiento, List<SqlParameter> parametros)
        {
            acceso.Abrir();
            DataTable tabla;
            try
            {
                tabla = acceso.Leer(procedimiento, parametros);
            }
            finally
            {
                acceso.Cerrar();
            }

            if (tabla.Rows.Count == 0)
            {
                return null;
            }

            DataRow fila = tabla.Rows[0];
            BE.USUARIO usuario = new BE.USUARIO();
            usuario.ID = int.Parse(fila["ID"].ToString());
            usuario.Nombre = fila["Nombre"].ToString();
            usuario.Contraseña = fila["Contraseña"].ToString();
            return usuario;
        }
    }
}
