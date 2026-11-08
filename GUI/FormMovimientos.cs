using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BE;

namespace GUI
{
    public partial class FormMovimientos : Form
    {
        private string rutaXml;
        private System.Drawing.Font fuenteNegrita;

        public FormMovimientos(string rutaXml, string titulo)
        {
            InitializeComponent();
            this.rutaXml = rutaXml;
            lblTitulo.Text = titulo;
        }

        private void FormMovimientos_Load(object sender, EventArgs e)
        {
            lblArchivo.Text = "Archivo: " + rutaXml;

            // Si el XML se editó a mano o quedó cortado, no se puede leer: se avisa y no se muestra nada.
            List<MOVIMIENTO> movimientos;
            try
            {
                movimientos = BLL.MOVIMIENTOXML.LeerMovimientos(rutaXml);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo leer el archivo XML de la partida (puede estar dañado):\n" + ex.Message,
                    "Movimientos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            fuenteNegrita = new System.Drawing.Font(dgvMovimientos.Font, System.Drawing.FontStyle.Bold);
            dgvMovimientos.DataSource = movimientos;
            dgvMovimientos.Columns["Jugador"].HeaderText = "Jugador";
            dgvMovimientos.Columns["Accion"].HeaderText = "Acción";
            dgvMovimientos.Columns["Detalle"].HeaderText = "Detalle";
            dgvMovimientos.Columns["Resultado"].HeaderText = "Resultado";
            dgvMovimientos.Columns["Fecha"].HeaderText = "Fecha y hora";

            dgvMovimientos.Columns["Jugador"].FillWeight = 100;
            dgvMovimientos.Columns["Accion"].FillWeight = 90;
            dgvMovimientos.Columns["Detalle"].FillWeight = 160;
            dgvMovimientos.Columns["Resultado"].FillWeight = 90;
            dgvMovimientos.Columns["Fecha"].FillWeight = 120;
        }

        // El fin de la partida va resaltado, para distinguirlo de colocaciones y disparos.
        private void dgvMovimientos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            MOVIMIENTO m = (MOVIMIENTO)dgvMovimientos.Rows[e.RowIndex].DataBoundItem;
            if (m.Accion == "Fin de partida")
            {
                e.CellStyle.Font = fuenteNegrita;
                e.CellStyle.BackColor = Branding.Secundario;
                e.CellStyle.ForeColor = System.Drawing.Color.White;
            }
            else if (m.Resultado == "Hundido")
            {
                e.CellStyle.Font = fuenteNegrita;
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
