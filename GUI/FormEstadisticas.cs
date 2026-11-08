using System;
using System.Windows.Forms;
using BE;

namespace GUI
{
    public partial class FormEstadisticas : Form
    {
        private USUARIO jugador1;
        private USUARIO jugador2;

        public FormEstadisticas(USUARIO jugador1, USUARIO jugador2)
        {
            InitializeComponent();
            this.jugador1 = jugador1;
            this.jugador2 = jugador2;
        }

        private void FormEstadisticas_Load(object sender, EventArgs e)
        {
            cmbJugador.Items.Add(jugador1.Nombre);
            if (jugador2 != null)
            {
                cmbJugador.Items.Add(jugador2.Nombre);
            }
            cmbJugador.SelectedIndex = 0;
        }

        private void cmbJugador_SelectedIndexChanged(object sender, EventArgs e)
        {
            USUARIO usuario = cmbJugador.SelectedIndex == 0 ? jugador1 : jugador2;
            CargarEstadisticas(usuario);
        }

        // Los cálculos los hace BLL.ESTADISTICA; acá solo se muestran.
        private void CargarEstadisticas(USUARIO usuario)
        {
            BLL.ESTADISTICA estadisticaBLL = new BLL.ESTADISTICA();
            ESTADISTICA estadistica;
            try
            {
                estadistica = estadisticaBLL.Calcular(usuario);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar las estadísticas: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            dgvHistorial.DataSource = null;
            dgvHistorial.DataSource = estadistica.Historial;

            dgvHistorial.Columns["Fecha"].HeaderText = "Fecha";
            dgvHistorial.Columns["Rival"].HeaderText = "Rival";
            dgvHistorial.Columns["Resultado"].HeaderText = "Resultado";
            dgvHistorial.Columns["RutaXml"].Visible = false;

            lblGanadas.Text = "Ganadas: " + estadistica.Ganadas;
            lblPerdidas.Text = "Perdidas: " + estadistica.Perdidas;
            // Empatadas siempre es 0 en Batalla Naval (ver Reglas_de_Negocio_BatallaNaval.md,
            // "Empates"); se muestra igual porque el enunciado general pide este dato.
            lblEmpatadas.Text = "Empatadas: " + estadistica.Empatadas;
            lblPromedio.Text = "Promedio de victorias: " + estadistica.PromedioVictorias.ToString("0.0") + "%";
            lblTiempoTotal.Text = "Tiempo total jugado: " + (int)estadistica.TiempoTotal.TotalHours + "h " +
                estadistica.TiempoTotal.Minutes + "m " + estadistica.TiempoTotal.Seconds + "s";
        }

        // Consulta posterior del XML de movimientos de la partida elegida en el historial.
        private void btnVerMovimientos_Click(object sender, EventArgs e)
        {
            if (dgvHistorial.CurrentRow == null)
            {
                MessageBox.Show("Elegí una partida del historial");
                return;
            }

            FILAHISTORIAL fila = (FILAHISTORIAL)dgvHistorial.CurrentRow.DataBoundItem;

            if (!BLL.MOVIMIENTOXML.Existe(fila.RutaXml))
            {
                MessageBox.Show(
                    "No se encontró el archivo XML de esa partida:\n" + fila.RutaXml,
                    "Movimientos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string titulo = "Partida del " + fila.Fecha + " vs " + fila.Rival;
            FormMovimientos formMovimientos = new FormMovimientos(fila.RutaXml, titulo);
            formMovimientos.ShowDialog();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
