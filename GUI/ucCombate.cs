using System;
using System.Drawing;
using System.Windows.Forms;

namespace GUI
{
    // Fase de combate: se muestran las 2 grillas del jugador en turno — su propio
    // tablero (con sus naves y los impactos que recibió) y el tablero rival en modo
    // niebla (nunca revela una nave no tocada). Ninguna de las 2 es información
    // sensible para ese jugador, así que no hace falta pantalla de "paso de
    // dispositivo" entre turnos (ver Reglas_de_Negocio_BatallaNaval.md, "Información oculta").
    public partial class ucCombate : UserControl
    {
        private const int TAM_CELDA = 18;

        private BLL.PARTIDA partidaBLL;
        private ucCelda[,] celdasPropias = new ucCelda[BLL.PARTIDA.TAMAÑO_TABLERO, BLL.PARTIDA.TAMAÑO_TABLERO];
        private ucCelda[,] celdasEnemigo = new ucCelda[BLL.PARTIDA.TAMAÑO_TABLERO, BLL.PARTIDA.TAMAÑO_TABLERO];

        // Se dispara después de cada disparo (acertado o no), para que FormPartida pueda
        // revisar si la partida ya terminó (evento PartidaFinalizada de BLL.PARTIDA).
        public event EventHandler Disparado;

        public ucCombate(BLL.PARTIDA partidaBLL)
        {
            InitializeComponent();
            this.partidaBLL = partidaBLL;
            ConstruirTablero(pnlTableroPropio, celdasPropias, null);
            ConstruirTablero(pnlTableroEnemigo, celdasEnemigo, celda_Click);

            TableroUI.DibujarReglas(this, OrigenTableroPropio, TAM_CELDA, BLL.PARTIDA.TAMAÑO_TABLERO, 13);
            TableroUI.DibujarReglas(this, OrigenTableroEnemigo, TAM_CELDA, BLL.PARTIDA.TAMAÑO_TABLERO, 13);
            TableroUI.DibujarLeyenda(this, new Point(OrigenTableroPropio.X, OrigenTableroPropio.Y + 271 + 48),
                Tuple.Create(ESTADO_VISUAL_CELDA.Vacia, "Vacío"),
                Tuple.Create(ESTADO_VISUAL_CELDA.Nave, "Nave propia"),
                Tuple.Create(ESTADO_VISUAL_CELDA.Agua, "Agua"),
                Tuple.Create(ESTADO_VISUAL_CELDA.Tocado, "Tocado"),
                Tuple.Create(ESTADO_VISUAL_CELDA.Hundido, "Hundido"));

            Refrescar();
        }

        private void ConstruirTablero(Panel panel, ucCelda[,] celdas, EventHandler manejadorClick)
        {
            int tam = BLL.PARTIDA.TAMAÑO_TABLERO;
            for (int f = 0; f < tam; f++)
            {
                for (int c = 0; c < tam; c++)
                {
                    ucCelda celda = new ucCelda();
                    celda.Fila = f;
                    celda.Columna = c;
                    celda.Location = new Point(c * TAM_CELDA, f * TAM_CELDA);
                    if (manejadorClick != null)
                    {
                        celda.Click += manejadorClick;
                    }
                    panel.Controls.Add(celda);
                    celdas[f, c] = celda;
                }
            }
        }

        private void celda_Click(object sender, EventArgs e)
        {
            if (!partidaBLL.EnCurso)
            {
                return;
            }

            int atacante = partidaBLL.JugadorEnTurno;
            ucCelda celda = (ucCelda)sender;

            BE.ESTADOCELDA resultado;
            try
            {
                resultado = partidaBLL.Disparar(atacante, celda.Fila, celda.Columna);
            }
            catch (Exception ex)
            {
                lblResultado.ForeColor = Branding.TableroHundido;
                lblResultado.Text = ex.Message;
                return;
            }

            MostrarResultado(resultado);
            Refrescar();

            if (Disparado != null)
            {
                Disparado(this, EventArgs.Empty);
            }
        }

        private void MostrarResultado(BE.ESTADOCELDA resultado)
        {
            switch (resultado)
            {
                case BE.ESTADOCELDA.Agua:
                    lblResultado.ForeColor = Branding.TextoPrincipal;
                    lblResultado.Text = "¡Agua!";
                    break;
                case BE.ESTADOCELDA.Tocado:
                    lblResultado.ForeColor = Branding.TableroTocado;
                    lblResultado.Text = "¡Tocado!";
                    break;
                case BE.ESTADOCELDA.Hundido:
                    lblResultado.ForeColor = Branding.TableroHundido;
                    lblResultado.Text = "¡Hundido!";
                    break;
            }
        }

        // Se llama al construir el control y después de cada disparo: ambos tableros son
        // siempre los del jugador que tiene el turno (su propio tablero y el del rival),
        // así que cambian de dueño en cada vuelta.
        public void Refrescar()
        {
            if (!partidaBLL.EnCurso)
            {
                return;
            }

            int idAtacante = partidaBLL.JugadorEnTurno;
            BE.USUARIO atacante = idAtacante == partidaBLL.Jugador1.ID ? partidaBLL.Jugador1 : partidaBLL.Jugador2;
            BE.USUARIO defensor = idAtacante == partidaBLL.Jugador1.ID ? partidaBLL.Jugador2 : partidaBLL.Jugador1;

            lblTurno.Text = "Turno de: " + atacante.Nombre;
            lblHeaderPropio.Text = "Mi flota (" + atacante.Nombre + ") — " + partidaBLL.ResumenFlota(idAtacante);
            lblHeaderEnemigo.Text = "Radar de " + defensor.Nombre + " — " + partidaBLL.ResumenFlota(defensor.ID);

            int tam = BLL.PARTIDA.TAMAÑO_TABLERO;

            bool[,] ocupacionPropia = partidaBLL.ObtenerOcupacionPropia(idAtacante);
            BE.ESTADOCELDA[,] disparosRecibidos = partidaBLL.ObtenerDisparosRecibidos(idAtacante);
            for (int f = 0; f < tam; f++)
            {
                for (int c = 0; c < tam; c++)
                {
                    celdasPropias[f, c].Estado = MapearPropia(ocupacionPropia[f, c], disparosRecibidos[f, c]);
                }
            }

            BE.ESTADOCELDA[,] fog = partidaBLL.ObtenerTableroEnemigoFog(idAtacante);
            for (int f = 0; f < tam; f++)
            {
                for (int c = 0; c < tam; c++)
                {
                    celdasEnemigo[f, c].Estado = Mapear(fog[f, c]);
                }
            }
        }

        private ESTADO_VISUAL_CELDA MapearPropia(bool ocupada, BE.ESTADOCELDA disparo)
        {
            if (disparo == BE.ESTADOCELDA.Tocado) return ESTADO_VISUAL_CELDA.Tocado;
            if (disparo == BE.ESTADOCELDA.Hundido) return ESTADO_VISUAL_CELDA.Hundido;
            if (ocupada) return ESTADO_VISUAL_CELDA.Nave;
            if (disparo == BE.ESTADOCELDA.Agua) return ESTADO_VISUAL_CELDA.Agua;
            return ESTADO_VISUAL_CELDA.Vacia;
        }

        private ESTADO_VISUAL_CELDA Mapear(BE.ESTADOCELDA estado)
        {
            switch (estado)
            {
                case BE.ESTADOCELDA.Agua: return ESTADO_VISUAL_CELDA.Agua;
                case BE.ESTADOCELDA.Tocado: return ESTADO_VISUAL_CELDA.Tocado;
                case BE.ESTADOCELDA.Hundido: return ESTADO_VISUAL_CELDA.Hundido;
                default: return ESTADO_VISUAL_CELDA.Vacia;
            }
        }
    }
}
