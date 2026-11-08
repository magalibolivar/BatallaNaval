using System;
using System.Drawing;
using System.Windows.Forms;

namespace GUI
{
    // Fase de combate. El único tablero que se muestra es el del rival en modo niebla
    // (nunca revela una nave no tocada), así que no hace falta pantalla de "paso de
    // dispositivo" entre turnos (ver Reglas_de_Negocio_BatallaNaval.md, "Información oculta").
    public partial class ucCombate : UserControl
    {
        private const int TAM_CELDA = 24;

        private BLL.PARTIDA partidaBLL;
        private ucCelda[,] celdas = new ucCelda[BLL.PARTIDA.TAMAÑO_TABLERO, BLL.PARTIDA.TAMAÑO_TABLERO];

        // Se dispara después de cada disparo (acertado o no), para que FormPartida pueda
        // revisar si la partida ya terminó (evento PartidaFinalizada de BLL.PARTIDA).
        public event EventHandler Disparado;

        public ucCombate(BLL.PARTIDA partidaBLL)
        {
            InitializeComponent();
            this.partidaBLL = partidaBLL;
            ConstruirTablero();
            Refrescar();
        }

        private void ConstruirTablero()
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
                    celda.Click += celda_Click;
                    pnlTablero.Controls.Add(celda);
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
                    lblResultado.ForeColor = Branding.TableroAgua;
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

        // Se llama al construir el control y después de cada disparo: el tablero que se ve
        // es siempre el del jugador que NO tiene el turno, así que cambia de dueño en cada vuelta.
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
            lblResumenPropio.Text = "Tu flota: " + partidaBLL.ResumenFlota(idAtacante);
            lblResumenRival.Text = "Flota de " + defensor.Nombre + ": " + partidaBLL.ResumenFlota(defensor.ID);

            BE.ESTADOCELDA[,] fog = partidaBLL.ObtenerTableroEnemigoFog(idAtacante);
            int tam = BLL.PARTIDA.TAMAÑO_TABLERO;
            for (int f = 0; f < tam; f++)
            {
                for (int c = 0; c < tam; c++)
                {
                    celdas[f, c].Estado = Mapear(fog[f, c]);
                }
            }
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
