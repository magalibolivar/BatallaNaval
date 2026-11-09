using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace GUI
{
    // Tipografía de la app (Montserrat, en GUI/Recursos). Se usa así en vez de
    // new Font("Montserrat", ...) porque la fuente no está instalada en Windows:
    // - GDI+ (el que crea los Font) solo la encuentra si se carga en una
    //   PrivateFontCollection y el Font se crea a partir de esa familia; con el nombre
    //   solo, cae en "Microsoft Sans Serif" sin avisar.
    // - GDI (con el que los Label y Button dibujan el texto) además necesita que el
    //   archivo esté registrado para el proceso con AddFontResourceEx.
    // Si no se puede cargar, se usa Segoe UI y la app sigue funcionando.
    internal static class Fuentes
    {
        private static readonly string[] archivos = { "Montserrat-Regular.ttf", "Montserrat-Bold.ttf" };
        private const uint FR_PRIVATE = 0x10;

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        private static extern int AddFontResourceEx(string archivo, uint fl, IntPtr pdv);

        // Se guarda en un campo estático: si la colección se liberara, sus familias dejarían de servir.
        private static PrivateFontCollection coleccion;
        private static FontFamily familia;
        private static bool cargada;

        public static void Cargar()
        {
            if (cargada)
            {
                return;
            }
            cargada = true;

            string carpeta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recursos");
            try
            {
                coleccion = new PrivateFontCollection();
                foreach (string archivo in archivos)
                {
                    string ruta = Path.Combine(carpeta, archivo);
                    if (!File.Exists(ruta))
                    {
                        continue;
                    }
                    coleccion.AddFontFile(ruta);
                    AddFontResourceEx(ruta, FR_PRIVATE, IntPtr.Zero);
                }
                familia = coleccion.Families.Length > 0 ? coleccion.Families[0] : null;
            }
            catch (Exception)
            {
                familia = null;
            }
        }

        // true si Montserrat se cargó (si no, Montserrat(...) devuelve Segoe UI).
        public static bool Disponible
        {
            get
            {
                Cargar();
                return familia != null;
            }
        }

        public static Font Montserrat(float tamaño, FontStyle estilo = FontStyle.Regular)
        {
            Cargar();
            if (familia != null && familia.IsStyleAvailable(estilo & (FontStyle.Bold | FontStyle.Italic)))
            {
                return new Font(familia, tamaño, estilo);
            }
            return new Font("Segoe UI", tamaño, estilo);
        }
    }
}
