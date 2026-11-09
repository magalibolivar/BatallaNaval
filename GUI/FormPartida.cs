using System;
using System.Windows.Forms;

namespace GUI
{
    // Dueño de todo el ciclo de vida de una partida: colocación de los 2 jugadores
    // y combate completo. Igual que FormPrincipal con BLL.PARTIDA en el TP de Generala,
    // este form solo lee el estado de partidaBLL y le avisa lo que hace el usuario.
    public partial class FormPartida : Form
    {
        private BLL.PARTIDA partidaBLL;
        private BE.USUARIO jugador1;
        private BE.USUARIO jugador2;

        public FormPartida(BE.USUARIO jugador1, BE.USUARIO jugador2)
        {
            InitializeComponent();
            this.jugador1 = jugador1;
            this.jugador2 = jugador2;
            partidaBLL = new BLL.PARTIDA();
            partidaBLL.PartidaFinalizada += PartidaBLL_PartidaFinalizada;
        }

        private void FormPartida_Load(object sender, EventArgs e)
        {
            try
            {
                partidaBLL.Iniciar(jugador1, jugador2);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo iniciar la partida: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Abort;
                this.Close();
                return;
            }

            MostrarPasoYColocacion(jugador1);
        }

        // Pantalla neutra antes de revelar el tablero propio de cada jugador: ver
        // Reglas_de_Negocio_BatallaNaval.md, "Información oculta".
        private void MostrarPasoYColocacion(BE.USUARIO jugador)
        {
            using (FormPasoTurno paso = new FormPasoTurno(
                jugador.Nombre + ": coloca tus naves.\n\nPasá el dispositivo y presioná Continuar."))
            {
                paso.ShowDialog(this);
            }

            ucColocacion uc = new ucColocacion(partidaBLL, jugador.ID);
            uc.Dock = DockStyle.Fill;
            uc.Actualizado += Colocacion_Actualizado;
            CambiarControl(uc);
        }

        private void Colocacion_Actualizado(object sender, EventArgs e)
        {
            if (partidaBLL.Fase == BLL.FASE_PARTIDA.ColocandoJugador2)
            {
                MostrarPasoYColocacion(jugador2);
            }
            else if (partidaBLL.Fase == BLL.FASE_PARTIDA.Combate)
            {
                using (FormPasoTurno paso = new FormPasoTurno(
                    "Las dos flotas están listas.\n\nComienza el combate: dispara primero " + jugador1.Nombre + "."))
                {
                    paso.ShowDialog(this);
                }

                // El combate muestra 2 tableros + la columna central: necesita una ventana más ancha.
                AjustarTamaño(new System.Drawing.Size(1010, 580));

                ucCombate uc = new ucCombate(partidaBLL);
                uc.Dock = DockStyle.Fill;
                CambiarControl(uc);
            }
        }

        // Cambia el tamaño manteniendo la ventana centrada donde estaba y dentro de la pantalla.
        private void AjustarTamaño(System.Drawing.Size cliente)
        {
            System.Drawing.Point centro = new System.Drawing.Point(
                this.Left + this.Width / 2, this.Top + this.Height / 2);
            this.ClientSize = cliente;

            System.Drawing.Rectangle area = Screen.FromControl(this).WorkingArea;
            int x = centro.X - this.Width / 2;
            int y = centro.Y - this.Height / 2;
            x = Math.Max(area.Left, Math.Min(x, area.Right - this.Width));
            y = Math.Max(area.Top, Math.Min(y, area.Bottom - this.Height));
            this.Location = new System.Drawing.Point(x, y);
        }

        private void CambiarControl(Control nuevo)
        {
            pnlContenido.Controls.Clear();
            pnlContenido.Controls.Add(nuevo);
        }

        private void PartidaBLL_PartidaFinalizada(BE.PARTIDA partida, bool abandonada)
        {
            // Detrás del resultado queda el tablero final, con las flotas reveladas.
            ucCombate combate = pnlContenido.Controls.Count > 0 ? pnlContenido.Controls[0] as ucCombate : null;
            if (combate != null)
            {
                combate.MostrarFinal(abandonada);
                combate.Update();
            }

            using (FormResultado formResultado = new FormResultado(partida, jugador1, jugador2, abandonada))
            {
                formResultado.ShowDialog(this);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnAbandonar_Click(object sender, EventArgs e)
        {
            Abandonar("La partida se registra como abandonada y la gana el otro jugador.");
        }

        // Devuelve true si se abandonó (o si no había nada que confirmar), false si el usuario canceló.
        private bool Abandonar(string mensaje)
        {
            if (!partidaBLL.EnCurso)
            {
                return true;
            }

            // Durante la colocación todavía no hay 2 jugadores "activos" en pantalla a la vez:
            // el que puede abandonar es el que está colocando en ese momento.
            if (partidaBLL.Fase == BLL.FASE_PARTIDA.ColocandoJugador1 || partidaBLL.Fase == BLL.FASE_PARTIDA.ColocandoJugador2)
            {
                int idColocando = partidaBLL.IdJugadorColocando;
                try
                {
                    partidaBLL.Abandonar(idColocando);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return true;
            }

            using (FormAbandonar formAbandonar = new FormAbandonar(jugador1, jugador2, mensaje))
            {
                if (formAbandonar.ShowDialog(this) != DialogResult.OK)
                {
                    return false;
                }

                try
                {
                    partidaBLL.Abandonar(formAbandonar.UsuarioQueAbandona.ID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return true;
            }
        }

        private void FormPartida_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (partidaBLL.EnCurso)
            {
                bool abandono = Abandonar(
                    "Hay una partida en curso. Para salir, uno de los 2 jugadores tiene que abandonarla: la gana el otro.");
                if (!abandono)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
