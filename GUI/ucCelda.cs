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

    // Vista previa de una nave que todavía no se colocó (colocación): dónde quedaría si
    // se hace click, y si ahí entra o no.
    public enum PREVIA_CELDA
    {
        Ninguna,
        Valida,
        Invalida
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
        private bool resaltada;
        private bool marcada;
        private PREVIA_CELDA previa = PREVIA_CELDA.Ninguna;

        public PREVIA_CELDA Previa
        {
            get { return previa; }
            set
            {
                if (previa != value)
                {
                    previa = value;
                    this.Invalidate();
                }
            }
        }

        // Borde de "apuntando acá" mientras el mouse pasa por una casilla que se puede disparar.
        public bool Resaltada
        {
            get { return resaltada; }
            set
            {
                if (resaltada != value)
                {
                    resaltada = value;
                    this.Invalidate();
                }
            }
        }

        // Anillo que señala el último disparo (el que hizo el rival sobre el tablero propio,
        // o el propio sobre el radar enemigo), para no tener que buscarlo entre las demás.
        public bool Marcada
        {
            get { return marcada; }
            set
            {
                if (marcada != value)
                {
                    marcada = value;
                    this.Invalidate();
                }
            }
        }

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
            using (SolidBrush pincel = new SolidBrush(ColorDeFondo()))
            {
                e.Graphics.FillPath(pincel, camino);
            }

            DibujarSimbolo(e.Graphics, rect);

            if (marcada)
            {
                using (GraphicsPath camino = CaminoRedondeado(rect, RADIO))
                using (Pen lapiz = new Pen(Branding.TextoPrincipal, 2.5F))
                {
                    e.Graphics.DrawPath(lapiz, camino);
                }
            }
            if (resaltada || previa == PREVIA_CELDA.Valida)
            {
                using (GraphicsPath camino = CaminoRedondeado(rect, RADIO))
                using (Pen lapiz = new Pen(Branding.Primario, 2.5F))
                {
                    e.Graphics.DrawPath(lapiz, camino);
                }
            }
        }

        private Color ColorDeFondo()
        {
            switch (previa)
            {
                // Mitad de camino entre "vacío" y "nave": se ve dónde quedaría sin confundirse con una nave ya puesta.
                case PREVIA_CELDA.Valida:
                    return Mezclar(Branding.TableroVacio, Branding.TableroNave);
                case PREVIA_CELDA.Invalida:
                    return Branding.TableroTocado;
                default:
                    return colorEstado;
            }
        }

        private static Color Mezclar(Color a, Color b)
        {
            return Color.FromArgb((a.R + b.R) / 2, (a.G + b.G) / 2, (a.B + b.B) / 2);
        }

        // Además del color, cada resultado de disparo lleva un símbolo (onda para agua,
        // cruz para tocado/hundido), así se distinguen aunque los colores se confundan.
        private void DibujarSimbolo(Graphics g, Rectangle rect)
        {
            float cx = rect.X + rect.Width / 2F;
            float cy = rect.Y + rect.Height / 2F;
            float r = rect.Width * 0.22F;

            if (estado == ESTADO_VISUAL_CELDA.Agua)
            {
                using (Pen lapiz = new Pen(Branding.TextoPrincipal, 1.3F))
                {
                    lapiz.StartCap = LineCap.Round;
                    lapiz.EndCap = LineCap.Round;
                    g.DrawBezier(lapiz,
                        cx - r * 1.3F, cy,
                        cx - r * 0.6F, cy - r * 0.9F,
                        cx + r * 0.6F, cy + r * 0.9F,
                        cx + r * 1.3F, cy);
                }
            }
            else if (estado == ESTADO_VISUAL_CELDA.Tocado || estado == ESTADO_VISUAL_CELDA.Hundido)
            {
                using (Pen lapiz = new Pen(Color.White, 2F))
                {
                    lapiz.StartCap = LineCap.Round;
                    lapiz.EndCap = LineCap.Round;
                    g.DrawLine(lapiz, cx - r, cy - r, cx + r, cy + r);
                    g.DrawLine(lapiz, cx - r, cy + r, cx + r, cy - r);
                }
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
