using System;
using System.Collections.Generic;

namespace BLL
{
    // Movimientos de una partida en su archivo XML. El archivo lo lee y escribe
    // DAL.MOVIMIENTOXML; acá se arma qué datos lleva cada registro (colocación, disparo, fin).
    public static class MOVIMIENTOXML
    {
        public static string GenerarRuta()
        {
            return DAL.MOVIMIENTOXML.GenerarRuta();
        }

        public static void CrearArchivo(string ruta, string jugador1, string jugador2)
        {
            DAL.MOVIMIENTOXML.CrearArchivo(ruta, Ahora(), jugador1, jugador2);
        }

        public static void RegistrarColocacion(string ruta, string jugador, BE.TIPONAVE nave, int fila, int columna, BE.ORIENTACION orientacion)
        {
            DAL.MOVIMIENTOXML.InsertarColocacion(ruta, jugador, nave.Nombre, fila, columna, orientacion.ToString(), Ahora());
        }

        // nave: solo se pasa cuando resultado es Tocado o Hundido.
        public static void RegistrarDisparo(string ruta, string jugador, int fila, int columna, BE.ESTADOCELDA resultado, BE.TIPONAVE nave)
        {
            string nombreNave = nave != null ? nave.Nombre : "";
            DAL.MOVIMIENTOXML.InsertarDisparo(ruta, jugador, fila, columna, resultado.ToString(), nombreNave, Ahora());
        }

        public static void RegistrarFin(string ruta, string ganador, string descripcion)
        {
            DAL.MOVIMIENTOXML.InsertarFin(ruta, ganador, descripcion, Ahora());
        }

        public static bool Existe(string ruta)
        {
            return DAL.MOVIMIENTOXML.Existe(ruta);
        }

        // Lee el XML de una partida y devuelve sus movimientos en orden, para consultarlos desde la app.
        public static List<BE.MOVIMIENTO> LeerMovimientos(string ruta)
        {
            return DAL.MOVIMIENTOXML.Listar(ruta);
        }

        private static string Ahora()
        {
            return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}
