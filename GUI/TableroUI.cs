using System.Drawing;
using System.Windows.Forms;

namespace GUI
{
    // Reglas de coordenadas (A-O / 1-15) y leyenda de colores, compartidas entre
    // ucColocacion y ucCombate para no duplicar el mismo dibujo dos veces.
    internal static class TableroUI
    {
        // Coordenada tal como la muestran las reglas: columna en letra y fila en número (p. ej. "C7").
        public static string Coordenada(int fila, int columna)
        {
            return ((char)('A' + columna)).ToString() + (fila + 1);
        }

        public static void DibujarReglas(Control contenedor, Point origenTablero, int tamCelda, int cantidadCeldas, int grosorRegla)
        {
            for (int c = 0; c < cantidadCeldas; c++)
            {
                Label lbl = new Label();
                lbl.Text = ((char)('A' + c)).ToString();
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Font = Fuentes.Montserrat(7.5F, FontStyle.Bold);
                lbl.ForeColor = Branding.TextoPrincipal;
                lbl.Size = new Size(tamCelda, grosorRegla);
                lbl.Location = new Point(origenTablero.X + c * tamCelda, origenTablero.Y - grosorRegla - 2);
                contenedor.Controls.Add(lbl);
            }

            for (int f = 0; f < cantidadCeldas; f++)
            {
                Label lbl = new Label();
                lbl.Text = (f + 1).ToString();
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Font = Fuentes.Montserrat(7.5F, FontStyle.Bold);
                lbl.ForeColor = Branding.TextoPrincipal;
                lbl.Size = new Size(grosorRegla, tamCelda);
                lbl.Location = new Point(origenTablero.X - grosorRegla - 2, origenTablero.Y + f * tamCelda);
                contenedor.Controls.Add(lbl);
            }
        }

        // Una fila de swatches + nombre, igual para colocación (sin agua/tocado/hundido útiles
        // todavía) y combate. Los estados que no aplican a la pantalla no se listan.
        public static void DibujarLeyenda(Control contenedor, Point origen, params System.Tuple<ESTADO_VISUAL_CELDA, string>[] items)
        {
            int x = origen.X;
            foreach (System.Tuple<ESTADO_VISUAL_CELDA, string> item in items)
            {
                // Una celda de verdad (no un cuadrado de color) para que la leyenda muestre
                // también el símbolo de agua / tocado / hundido, igual que el tablero.
                ucCelda swatch = new ucCelda();
                swatch.Size = new Size(18, 18);
                swatch.Location = new Point(x, origen.Y);
                swatch.Estado = item.Item1;
                swatch.BackColor = contenedor.BackColor;
                swatch.Cursor = Cursors.Default;
                swatch.Enabled = false;
                contenedor.Controls.Add(swatch);

                Label lbl = new Label();
                lbl.AutoSize = true;
                lbl.Text = item.Item2;
                lbl.ForeColor = Branding.TextoPrincipal;
                lbl.Font = Fuentes.Montserrat(8F);
                lbl.Location = new Point(x + 22, origen.Y + 1);
                contenedor.Controls.Add(lbl);

                x += 22 + System.Windows.Forms.TextRenderer.MeasureText(item.Item2, lbl.Font).Width + 18;
            }
        }
    }
}
