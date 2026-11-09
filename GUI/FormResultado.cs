using System;
using System.Windows.Forms;
using BE;

namespace GUI
{
    // Cierre de una partida: reemplaza el MessageBox simple por un resumen con
    // acceso directo al XML de movimientos, igual que se aprobó en la propuesta
    // visual (Claude Design, proyecto "Batalla Naval").
    public partial class FormResultado : Form
    {
        private readonly string rutaXml;
        private readonly string tituloMovimientos;

        public FormResultado(PARTIDA partida, USUARIO jugador1, USUARIO jugador2, bool abandonada)
        {
            InitializeComponent();

            USUARIO ganador = partida.IdGanador == jugador1.ID ? jugador1 : jugador2;
            USUARIO perdedor = partida.IdGanador == jugador1.ID ? jugador2 : jugador1;

            lblTitulo.Text = "¡Ganó " + ganador.Nombre + "!";
            lblDetalle.Text = abandonada
                ? perdedor.Nombre + " abandonó la partida."
                : "Hundió toda la flota de " + perdedor.Nombre + ".";

            lblJugador1Valor.Text = jugador1.Nombre;
            lblJugador2Valor.Text = jugador2.Nombre;

            TimeSpan duracion = partida.FechaFin.Value - partida.FechaInicio;
            lblDuracionValor.Text = (int)duracion.TotalHours > 0
                ? (int)duracion.TotalHours + "h " + duracion.Minutes + "m"
                : duracion.Minutes + "m " + duracion.Seconds + "s";
            lblResultadoValor.Text = abandonada ? "Abandono" : "Victoria completa";

            rutaXml = partida.RutaXml;
            tituloMovimientos = "Partida del " + partida.FechaInicio.ToString("dd/MM/yyyy HH:mm") +
                " — " + jugador1.Nombre + " vs " + jugador2.Nombre;
        }

        private void btnVerMovimientos_Click(object sender, EventArgs e)
        {
            if (!BLL.MOVIMIENTOXML.Existe(rutaXml))
            {
                MessageBox.Show("No se encontró el archivo XML de esta partida:\n" + rutaXml,
                    "Movimientos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FormMovimientos formMovimientos = new FormMovimientos(rutaXml, tituloMovimientos);
            formMovimientos.ShowDialog(this);
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
