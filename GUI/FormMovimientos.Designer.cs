namespace GUI
{
    partial class FormMovimientos
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
        private System.Windows.Forms.Label lblArchivo;
        private System.Windows.Forms.DataGridView dgvMovimientos;
        private System.Windows.Forms.Button btnCerrar;

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblArchivo = new System.Windows.Forms.Label();
            this.dgvMovimientos = new System.Windows.Forms.DataGridView();
            this.btnCerrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMovimientos)).BeginInit();
            this.SuspendLayout();

            this.BackColor = Branding.FondoVentana;

            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = Fuentes.Montserrat(12F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = Branding.Primario;
            this.lblTitulo.Location = new System.Drawing.Point(20, 15);

            this.lblArchivo.AutoSize = true;
            this.lblArchivo.ForeColor = System.Drawing.Color.Gray;
            this.lblArchivo.Location = new System.Drawing.Point(20, 45);

            this.dgvMovimientos.Location = new System.Drawing.Point(20, 75);
            this.dgvMovimientos.Size = new System.Drawing.Size(650, 350);
            this.dgvMovimientos.AllowUserToAddRows = false;
            this.dgvMovimientos.AllowUserToDeleteRows = false;
            this.dgvMovimientos.ReadOnly = true;
            this.dgvMovimientos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMovimientos.BackgroundColor = Branding.Panel;
            this.dgvMovimientos.ColumnHeadersDefaultCellStyle.BackColor = Branding.Secundario;
            this.dgvMovimientos.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dgvMovimientos.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvMovimientos_CellFormatting);

            this.btnCerrar.Location = new System.Drawing.Point(560, 435);
            this.btnCerrar.Size = new System.Drawing.Size(110, 32);
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrar.FlatAppearance.BorderSize = 0;
            this.btnCerrar.BackColor = Branding.Primario;
            this.btnCerrar.ForeColor = System.Drawing.Color.White;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);

            this.ClientSize = new System.Drawing.Size(690, 485);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblArchivo);
            this.Controls.Add(this.dgvMovimientos);
            this.Controls.Add(this.btnCerrar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Movimientos de la partida";
            this.Load += new System.EventHandler(this.FormMovimientos_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMovimientos)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
