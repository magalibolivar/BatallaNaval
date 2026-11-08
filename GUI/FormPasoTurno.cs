using System.Windows.Forms;

namespace GUI
{
    // Pantalla neutra que se muestra antes de revelar el tablero propio de un jugador
    // durante la colocación de naves, para que el otro jugador no lo vea de refilón
    // (ver Reglas_de_Negocio_BatallaNaval.md, "Información oculta").
    public partial class FormPasoTurno : Form
    {
        public FormPasoTurno(string mensaje)
        {
            InitializeComponent();
            lblMensaje.Text = mensaje;
        }

        private void btnContinuar_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
