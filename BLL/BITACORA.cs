using System;

namespace BLL
{
    public static class BITACORA
    {
        // Los tests la reemplazan para registrar sin base de datos.
        internal static Func<DAL.MP_LOG> CrearMapper = () => new DAL.MP_LOG();

        public static void Registrar(int idUsuario, int idTipo, string descripcion)
        {
            BE.LOG log = new BE.LOG();
            log.IdUsuario = idUsuario;
            log.IdTipo = idTipo;
            log.Descripcion = descripcion;

            DAL.MP_LOG mapper = CrearMapper();
            if (mapper.Insertar(log) == -1)
            {
                throw new Exception("No se pudo registrar en la bitácora: " + descripcion);
            }
        }
    }
}
