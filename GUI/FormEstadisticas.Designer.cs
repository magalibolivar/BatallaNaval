namespace GUI
{
    partial class FormEstadisticas
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblJugador;
        private System.Windows.Forms.ComboBox cmbJugador;
        private System.Windows.Forms.Label lblGanadas;
        private System.Windows.Forms.Label lblPerdidas;
        private System.Windows.Forms.Label lblEmpatadas;
        private System.Windows.Forms.Label lblPromedio;
        private System.Windows.Forms.Label lblTiempoTotal;
        private System.Windows.Forms.DataGridView dgvHistorial;
        private System.Windows.Forms.Button btnVerMovimientos;
        private System.Windows.Forms.Button btnCerrar;

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblJugador = new System.Windows.Forms.Label();
            this.cmbJugador = new System.Windows.Forms.ComboBox();
            this.lblGanadas = new System.Windows.Forms.Label();
            this.lblPerdidas = new System.Windows.Forms.Label();
            this.lblEmpatadas = new System.Windows.Forms.Label();
            this.lblPromedio = new System.Windows.Forms.Label();
            this.lblTiempoTotal = new System.Windows.Forms.Label();
            this.dgvHistorial = new System.Windows.Forms.DataGridView();
            this.btnVerMovimientos = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).BeginInit();
            this.SuspendLayout();

            this.BackColor = Branding.FondoVentana;

            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = Fuentes.Montserrat(14F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = Branding.Primario;
            this.lblTitulo.Location = new System.Drawing.Point(20, 15);
            this.lblTitulo.Text = "Estadísticas";

            this.lblJugador.AutoSize = true;
            this.lblJugador.ForeColor = Branding.TextoPrincipal;
            this.lblJugador.Location = new System.Drawing.Point(20, 55);
            this.lblJugador.Text = "Jugador:";

            this.cmbJugador.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbJugador.Location = new System.Drawing.Point(90, 52);
            this.cmbJugador.Size = new System.Drawing.Size(200, 25);
            this.cmbJugador.SelectedIndexChanged += new System.EventHandler(this.cmbJugador_SelectedIndexChanged);

            this.lblGanadas.AutoSize = true;
            this.lblGanadas.ForeColor = Branding.TextoPrincipal;
            this.lblGanadas.Location = new System.Drawing.Point(20, 90);

            this.lblPerdidas.AutoSize = true;
            this.lblPerdidas.ForeColor = Branding.TextoPrincipal;
            this.lblPerdidas.Location = new System.Drawing.Point(150, 90);

            this.lblEmpatadas.AutoSize = true;
            this.lblEmpatadas.ForeColor = Branding.TextoPrincipal;
            this.lblEmpatadas.Location = new System.Drawing.Point(280, 90);

            this.lblPromedio.AutoSize = true;
            this.lblPromedio.ForeColor = Branding.TextoPrincipal;
            this.lblPromedio.Location = new System.Drawing.Point(20, 115);

            this.lblTiempoTotal.AutoSize = true;
            this.lblTiempoTotal.ForeColor = Branding.TextoPrincipal;
            this.lblTiempoTotal.Location = new System.Drawing.Point(20, 140);

            this.dgvHistorial.Location = new System.Drawing.Point(20, 175);
            this.dgvHistorial.Size = new System.Drawing.Size(550, 230);
            this.dgvHistorial.AllowUserToAddRows = false;
            this.dgvHistorial.AllowUserToDeleteRows = false;
            this.dgvHistorial.ReadOnly = true;
            this.dgvHistorial.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHistorial.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistorial.MultiSelect = false;
            this.dgvHistorial.BackgroundColor = Branding.Panel;
            this.dgvHistorial.ColumnHeadersDefaultCellStyle.BackColor = Branding.Secundario;
            this.dgvHistorial.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dgvHistorial.DefaultCellStyle.SelectionBackColor = Branding.Primario;

            this.btnVerMovimientos.Location = new System.Drawing.Point(20, 415);
            this.btnVerMovimientos.Size = new System.Drawing.Size(180, 32);
            this.btnVerMovimientos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVerMovimientos.FlatAppearance.BorderColor = Branding.Secundario;
            this.btnVerMovimientos.BackColor = Branding.Panel;
            this.btnVerMovimientos.ForeColor = Branding.Secundario;
            this.btnVerMovimientos.Text = "Ver movimientos";
            this.btnVerMovimientos.Click += new System.EventHandler(this.btnVerMovimientos_Click);

            this.btnCerrar.Location = new System.Drawing.Point(390, 415);
            this.btnCerrar.Size = new System.Drawing.Size(180, 32);
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrar.FlatAppearance.BorderSize = 0;
            this.btnCerrar.BackColor = Branding.Primario;
            this.btnCerrar.ForeColor = System.Drawing.Color.White;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);

            this.ClientSize = new System.Drawing.Size(590, 465);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblJugador);
            this.Controls.Add(this.cmbJugador);
            this.Controls.Add(this.lblGanadas);
            this.Controls.Add(this.lblPerdidas);
            this.Controls.Add(this.lblEmpatadas);
            this.Controls.Add(this.lblPromedio);
            this.Controls.Add(this.lblTiempoTotal);
            this.Controls.Add(this.dgvHistorial);
            this.Controls.Add(this.btnVerMovimientos);
            this.Controls.Add(this.btnCerrar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Batalla Naval - Estadísticas";
            this.Icon = RecursosNaves.Icono();
            this.Load += new System.EventHandler(this.FormEstadisticas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
