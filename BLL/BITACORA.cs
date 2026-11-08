using System;

namespace BLL
{
    public static class BITACORA
    {
        public static void Registrar(int idUsuario, int idTipo, string descripcion)
        {
            BE.LOG log = new BE.LOG();
            log.IdUsuario = idUsuario;
            log.IdTipo = idTipo;
            log.Descripcion = descripcion;

            DAL.MP_LOG mapper = new DAL.MP_LOG();
            if (mapper.Insertar(log) == -1)
            {
                throw new Exception("No se pudo registrar en la bitácora: " + descripcion);
            }
        }
    }
}
