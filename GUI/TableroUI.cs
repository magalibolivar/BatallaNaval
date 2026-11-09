using System.Drawing;
using System.Windows.Forms;

namespace GUI
{
    // Reglas de coordenadas (A-O / 1-15) y leyenda de colores, compartidas entre
    // ucColocacion y ucCombate para no duplicar el mismo dibujo dos veces.
    internal static class TableroUI
    {
        public static void DibujarReglas(Control contenedor, Point origenTablero, int tamCelda, int cantidadCeldas, int grosorRegla)
        {
            for (int c = 0; c < cantidadCeldas; c++)
            {
                Label lbl = new Label();
                lbl.Text = ((char)('A' + c)).ToString();
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
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
                lbl.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
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
                Panel swatch = new Panel();
                swatch.Size = new Size(16, 16);
                swatch.Location = new Point(x, origen.Y + 2);
                swatch.BackColor = ColorDe(item.Item1);
                contenedor.Controls.Add(swatch);

                Label lbl = new Label();
                lbl.AutoSize = true;
                lbl.Text = item.Item2;
                lbl.ForeColor = Branding.TextoPrincipal;
                lbl.Font = new Font("Segoe UI", 8F);
                lbl.Location = new Point(x + 20, origen.Y);
                contenedor.Controls.Add(lbl);

                x += 20 + System.Windows.Forms.TextRenderer.MeasureText(item.Item2, lbl.Font).Width + 18;
            }
        }

        private static Color ColorDe(ESTADO_VISUAL_CELDA estado)
        {
            switch (estado)
            {
                case ESTADO_VISUAL_CELDA.Nave: return Branding.TableroNave;
                case ESTADO_VISUAL_CELDA.Agua: return Branding.TableroAgua;
                case ESTADO_VISUAL_CELDA.Tocado: return Branding.TableroTocado;
                case ESTADO_VISUAL_CELDA.Hundido: return Branding.TableroHundido;
                default: return Branding.TableroVacio;
            }
        }
    }
}
