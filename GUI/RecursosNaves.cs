using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace GUI
{
    // Imágenes de cada tipo de nave (GUI/Recursos/Naves), vistas desde arriba y con fondo
    // transparente. Por nave hay 3 archivos:
    // - <Nave>_H_128.png: horizontal, de N casilleros × 1 (128 px por casillero).
    // - <Nave>_V_128.png: vertical, ya girada (1 × N casilleros); no hace falta rotarla.
    // - <Nave>_Lista.png: miniatura de 180×54 para las listas de la flota.
    // Las del tablero tienen la proporción exacta de la nave (N:1), así que se dibujan como
    // una sola imagen sobre todos sus casilleros sin deformarse. Se cachean porque se piden
    // muchas veces (al mover o girar naves).
    internal static class RecursosNaves
    {
        private static readonly Dictionary<string, string> prefijos = new Dictionary<string, string>
        {
            { "Batalla Naval", "Batalla_Naval" },
            { "Portaaviones", "Portaaviones" },
            { "Fragatas", "Fragatas" },
            { "Buque", "Buque" },
            { "Acorazados", "Acorazados" },
        };

        private static readonly Dictionary<string, Image> cache = new Dictionary<string, Image>();

        private static Image Cargar(string nombreNave, string variante)
        {
            string archivo = prefijos[nombreNave] + "_" + variante + ".png";
            Image imagen;
            if (!cache.TryGetValue(archivo, out imagen))
            {
                imagen = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recursos", "Naves", archivo));
                cache[archivo] = imagen;
            }
            return imagen;
        }

        // Nave horizontal para el tablero (proporción N:1).
        public static Image Obtener(string nombreNave)
        {
            return Cargar(nombreNave, "H_128");
        }

        // Nave vertical para el tablero (proporción 1:N), dibujada ya girada.
        public static Image ObtenerVertical(string nombreNave)
        {
            return Cargar(nombreNave, "V_128");
        }

        // Miniatura para las listas de la flota (colocación y flota rival en el combate).
        public static Image ObtenerLista(string nombreNave)
        {
            return Cargar(nombreNave, "Lista");
        }

        private static Image logo;
        private static Icon icono;

        // Logo completo (barco + "BATALLA NAVAL"), para el login y el menú principal. No es
        // la imagen de la nave "Batalla Naval" de la flota (Naves/Batalla_Naval_*.png).
        public static Image LogoBatallaNaval()
        {
            if (logo == null)
            {
                logo = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recursos", "Logo.png"));
            }
            return logo;
        }

        // Ícono de las ventanas (el mismo que el del .exe). Si no se encuentra el archivo,
        // se toma el ícono embebido en el ejecutable.
        public static Icon Icono()
        {
            if (icono == null)
            {
                string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recursos", "BatallaNaval.ico");
                try
                {
                    icono = File.Exists(ruta)
                        ? new Icon(ruta)
                        : Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
                }
                catch (Exception)
                {
                    icono = null;
                }
            }
            return icono;
        }
    }
}
