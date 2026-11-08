using System;
using System.Windows.Forms;
using BE;

namespace GUI
{
    // Pregunta qué jugador abandona la partida. No se asume que es el del turno:
    // justo después de disparar el turno pasa solo al rival y es fácil confundirse.
    public partial class FormAbandonar : Form
    {
        private USUARIO jugador1;
        private USUARIO jugador2;

        public USUARIO UsuarioQueAbandona { get; private set; }

        public FormAbandonar(USUARIO jugador1, USUARIO jugador2, string mensaje)
        {
            InitializeComponent();
            this.jugador1 = jugador1;
            this.jugador2 = jugador2;
            lblMensaje.Text = mensaje;
            btnJugador1.Text = jugador1.Nombre;
            btnJugador2.Text = jugador2.Nombre;
        }

        private void btnJugador1_Click(object sender, EventArgs e)
        {
            UsuarioQueAbandona = jugador1;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnJugador2_Click(object sender, EventArgs e)
        {
            UsuarioQueAbandona = jugador2;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
