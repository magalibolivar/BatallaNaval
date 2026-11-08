using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace GUI
{
    // Ilustración de cada tipo de nave (GUI/Recursos/*.png), cacheada en memoria
    // porque se usa muchas veces (lista de flota pendiente + tablero de colocación).
    internal static class RecursosNaves
    {
        private static readonly Dictionary<string, string> archivos = new Dictionary<string, string>
        {
            { "Batalla Naval", "Nave_BatallaNaval.png" },
            { "Portaaviones", "Nave_Portaaviones.png" },
            { "Fragatas", "Nave_Fragatas.png" },
            { "Buque", "Nave_Buque.png" },
            { "Acorazados", "Nave_Acorazados.png" },
        };

        private static readonly Dictionary<string, Image> cache = new Dictionary<string, Image>();

        public static Image Obtener(string nombreNave)
        {
            Image imagen;
            if (cache.TryGetValue(nombreNave, out imagen))
            {
                return imagen;
            }

            string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recursos", archivos[nombreNave]);
            imagen = Image.FromFile(ruta);
            cache[nombreNave] = imagen;
            return imagen;
        }

        // Copia rotada 90° para naves colocadas en vertical (la imagen original mira a la derecha).
        public static Image ObtenerRotada(string nombreNave)
        {
            Bitmap rotada = new Bitmap(Obtener(nombreNave));
            rotada.RotateFlip(RotateFlipType.Rotate90FlipNone);
            return rotada;
        }

        public static Image LogoBatallaNaval()
        {
            return Obtener("Batalla Naval");
        }
    }
}
