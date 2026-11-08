using System.Drawing;
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
    // en el TP de Generala pero para una grilla en vez de un dado.
    public partial class ucCelda : UserControl
    {
        public int Fila { get; set; }
        public int Columna { get; set; }

        private ESTADO_VISUAL_CELDA estado = ESTADO_VISUAL_CELDA.Vacia;

        public ESTADO_VISUAL_CELDA Estado
        {
            get { return estado; }
            set
            {
                estado = value;
                switch (estado)
                {
                    case ESTADO_VISUAL_CELDA.Vacia:
                        this.BackColor = Branding.TableroVacio;
                        break;
                    case ESTADO_VISUAL_CELDA.Nave:
                        this.BackColor = Branding.TableroNave;
                        break;
                    case ESTADO_VISUAL_CELDA.Agua:
                        this.BackColor = Branding.TableroAgua;
                        break;
                    case ESTADO_VISUAL_CELDA.Tocado:
                        this.BackColor = Branding.TableroTocado;
                        break;
                    case ESTADO_VISUAL_CELDA.Hundido:
                        this.BackColor = Branding.TableroHundido;
                        break;
                }
            }
        }

        public ucCelda()
        {
            InitializeComponent();
            Estado = ESTADO_VISUAL_CELDA.Vacia;
        }
    }
}
