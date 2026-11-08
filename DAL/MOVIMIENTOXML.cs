using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace DAL
{
    // Persistencia del archivo XML de movimientos de cada partida. Igual que los mappers con
    // SQL Server, acá solo se lee y escribe: qué datos lleva cada registro lo arma BLL.MOVIMIENTOXML.
    // Cada partida tiene su archivo, con:
    //   <Colocacion> cada nave que un jugador coloca en su tablero
    //   <Disparo>    cada disparo (agua, tocado o hundido)
    //   <Fin>        el cierre: ganador y si fue abandono
    public static class MOVIMIENTOXML
    {
        public static string GenerarRuta()
        {
            string carpeta = AppDomain.CurrentDomain.BaseDirectory + "Partidas\\";
            if (!Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }

            return carpeta + "Partida_" + DateTime.Now.ToString("yyyyMMdd_HHmmssfff") + ".xml";
        }

        public static void CrearArchivo(string ruta, string fecha, string jugador1, string jugador2)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement raiz = doc.CreateElement("Partida");
            raiz.SetAttribute("fecha", fecha);
            raiz.SetAttribute("jugador1", jugador1);
            raiz.SetAttribute("jugador2", jugador2);
            doc.AppendChild(raiz);
            doc.Save(ruta);
        }

        public static void InsertarColocacion(string ruta, string jugador, string nave, int fila, int columna, string orientacion, string fecha)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(ruta);

            XmlElement colocacion = doc.CreateElement("Colocacion");
            colocacion.SetAttribute("jugador", jugador);
            colocacion.SetAttribute("nave", nave);
            colocacion.SetAttribute("fila", fila.ToString());
            colocacion.SetAttribute("columna", columna.ToString());
            colocacion.SetAttribute("orientacion", orientacion);
            colocacion.SetAttribute("fecha", fecha);

            doc.DocumentElement.AppendChild(colocacion);
            doc.Save(ruta);
        }

        // nave: solo se completa cuando resultado es "Tocado" u "Hundido" (si no, queda "").
        public static void InsertarDisparo(string ruta, string jugador, int fila, int columna, string resultado, string nave, string fecha)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(ruta);

            XmlElement disparo = doc.CreateElement("Disparo");
            disparo.SetAttribute("jugador", jugador);
            disparo.SetAttribute("fila", fila.ToString());
            disparo.SetAttribute("columna", columna.ToString());
            disparo.SetAttribute("resultado", resultado);
            disparo.SetAttribute("nave", nave);
            disparo.SetAttribute("fecha", fecha);

            doc.DocumentElement.AppendChild(disparo);
            doc.Save(ruta);
        }

        public static void InsertarFin(string ruta, string ganador, string descripcion, string fecha)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(ruta);

            XmlElement fin = doc.CreateElement("Fin");
            fin.SetAttribute("ganador", ganador);
            fin.SetAttribute("descripcion", descripcion);
            fin.SetAttribute("fecha", fecha);

            doc.DocumentElement.AppendChild(fin);
            doc.Save(ruta);
        }

        public static bool Existe(string ruta)
        {
            return ruta != null && ruta != "" && File.Exists(ruta);
        }

        // Lee el XML de una partida y devuelve sus registros en orden, como BE.MOVIMIENTO.
        public static List<BE.MOVIMIENTO> Listar(string ruta)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(ruta);

            List<BE.MOVIMIENTO> movimientos = new List<BE.MOVIMIENTO>();
            foreach (XmlNode nodo in doc.DocumentElement.ChildNodes)
            {
                XmlElement elemento = nodo as XmlElement;
                if (elemento == null)
                {
                    continue;
                }

                BE.MOVIMIENTO m = new BE.MOVIMIENTO();
                m.Fecha = elemento.GetAttribute("fecha");

                if (elemento.Name == "Colocacion")
                {
                    m.Jugador = elemento.GetAttribute("jugador");
                    m.Accion = "Colocación";
                    m.Detalle = elemento.GetAttribute("nave") + " en (" + elemento.GetAttribute("fila") + "," +
                        elemento.GetAttribute("columna") + ") " + elemento.GetAttribute("orientacion");
                    m.Resultado = "";
                }
                else if (elemento.Name == "Disparo")
                {
                    m.Jugador = elemento.GetAttribute("jugador");
                    m.Accion = "Disparo";
                    m.Detalle = "(" + elemento.GetAttribute("fila") + "," + elemento.GetAttribute("columna") + ")" +
                        (elemento.GetAttribute("nave") == "" ? "" : " - " + elemento.GetAttribute("nave"));
                    m.Resultado = elemento.GetAttribute("resultado");
                }
                else if (elemento.Name == "Fin")
                {
                    m.Jugador = elemento.GetAttribute("ganador");
                    m.Accion = "Fin de partida";
                    m.Detalle = elemento.GetAttribute("descripcion");
                    m.Resultado = "";
                }
                else
                {
                    continue;
                }

                movimientos.Add(m);
            }
            return movimientos;
        }
    }
}
