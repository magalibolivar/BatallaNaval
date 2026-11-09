using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GUI
{
    // Estado visual de una celda. "Nave" solo se usa en el tablero propio durante la
    // colocación; el tablero enemigo en combate nunca llega a ese estado (ver
    // Reglas_de_Negocio_BatallaNaval.md, "Información oculta").
    public enum ESTADO_VISUAL_CELDA
    {
        Vacia,
        Nave,
        Agua,
        Tocado,
        Hundido
    }

    // Una celda de un tablero de 15x15. Pieza visual reutilizable, igual que ucDado
    // en el TP de Generala pero para una grilla en vez de un dado. Se dibuja a mano
    // (en vez de solo poner BackColor) para que las celdas queden redondeadas y con
    // un pequeño espacio entre ellas, en vez de un bloque de color pegado al vecino.
    public partial class ucCelda : UserControl
    {
        private const int RADIO = 5;

        public int Fila { get; set; }
        public int Columna { get; set; }

        private ESTADO_VISUAL_CELDA estado = ESTADO_VISUAL_CELDA.Vacia;
        private Color colorEstado;

        public ESTADO_VISUAL_CELDA Estado
        {
            get { return estado; }
            set
            {
                estado = value;
                switch (estado)
                {
                    case ESTADO_VISUAL_CELDA.Vacia:
                        colorEstado = Branding.TableroVacio;
                        break;
                    case ESTADO_VISUAL_CELDA.Nave:
                        colorEstado = Branding.TableroNave;
                        break;
                    case ESTADO_VISUAL_CELDA.Agua:
                        colorEstado = Branding.TableroAgua;
                        break;
                    case ESTADO_VISUAL_CELDA.Tocado:
                        colorEstado = Branding.TableroTocado;
                        break;
                    case ESTADO_VISUAL_CELDA.Hundido:
                        colorEstado = Branding.TableroHundido;
                        break;
                }
                this.Invalidate();
            }
        }

        public ucCelda()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            // El fondo detrás del redondeo queda del color del panel contenedor (blanco),
            // así el espacio entre celdas se ve como un hueco en vez de un cuadrado de color.
            this.BackColor = Branding.Panel;
            Estado = ESTADO_VISUAL_CELDA.Vacia;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, this.Width - 2, this.Height - 2);
            using (GraphicsPath camino = CaminoRedondeado(rect, RADIO))
            using (SolidBrush pincel = new SolidBrush(colorEstado))
            {
                e.Graphics.FillPath(pincel, camino);
            }
        }

        private static GraphicsPath CaminoRedondeado(Rectangle rect, int radio)
        {
            GraphicsPath camino = new GraphicsPath();
            int d = radio * 2;
            camino.AddArc(rect.X, rect.Y, d, d, 180, 90);
            camino.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            camino.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            camino.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            camino.CloseFigure();
            return camino;
        }
    }
}
