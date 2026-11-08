using System.Drawing;

namespace GUI
{
    // Paleta oficial de la identidad visual de Batalla Naval. Centralizar los colores
    // acá evita tener los mismos hex repetidos (y potencialmente desincronizados) en
    // cada Designer.cs.
    internal static class Branding
    {
        public static readonly Color Primario = ColorHtml("#C95D87");
        public static readonly Color Secundario = ColorHtml("#9B83BE");
        public static readonly Color FondoVentana = ColorHtml("#FCF9FB");
        public static readonly Color Panel = ColorHtml("#FFFFFF");
        public static readonly Color TextoPrincipal = ColorHtml("#392F46");

        public static readonly Color TableroVacio = ColorHtml("#EAF0F6");
        public static readonly Color TableroNave = ColorHtml("#8D79B4");
        public static readonly Color TableroAgua = ColorHtml("#A7D9E6");
        public static readonly Color TableroTocado = ColorHtml("#F18097");
        public static readonly Color TableroHundido = ColorHtml("#65415F");

        private static Color ColorHtml(string html)
        {
            return ColorTranslator.FromHtml(html);
        }
    }
}
