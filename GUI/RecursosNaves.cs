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

        private static readonly Dictionary<string, Image> cacheRotadas = new Dictionary<string, Image>();

        // Copia rotada 90° para naves colocadas en vertical (la imagen original mira a la derecha).
        // También se cachea: al girar naves con click se pide muchas veces.
        public static Image ObtenerRotada(string nombreNave)
        {
            Image rotada;
            if (cacheRotadas.TryGetValue(nombreNave, out rotada))
            {
                return rotada;
            }

            Bitmap copia = new Bitmap(Obtener(nombreNave));
            copia.RotateFlip(RotateFlipType.Rotate90FlipNone);
            cacheRotadas[nombreNave] = copia;
            return copia;
        }

        public static Image LogoBatallaNaval()
        {
            return Obtener("Batalla Naval");
        }
    }
}
