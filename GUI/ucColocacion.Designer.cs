namespace GUI
{
    partial class ucColocacion
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

        private System.Windows.Forms.Panel pnlTablero;
        private System.Windows.Forms.Label lblInstrucciones;
        private System.Windows.Forms.ListView lstFlota;
        private System.Windows.Forms.ImageList imgFlota;
        private System.Windows.Forms.Label lblRestantes;
        private System.Windows.Forms.Button btnOrientacion;
        private System.Windows.Forms.Label lblMensaje;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlTablero = new System.Windows.Forms.Panel();
            this.lblInstrucciones = new System.Windows.Forms.Label();
            this.lstFlota = new System.Windows.Forms.ListView();
            this.imgFlota = new System.Windows.Forms.ImageList(this.components);
            this.lblRestantes = new System.Windows.Forms.Label();
            this.btnOrientacion = new System.Windows.Forms.Button();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.SuspendLayout();

            this.BackColor = Branding.FondoVentana;

            this.pnlTablero.Location = new System.Drawing.Point(0, 30);
            this.pnlTablero.Size = new System.Drawing.Size(361, 361);
            this.pnlTablero.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlTablero.BackColor = Branding.Panel;

            this.lblInstrucciones.AutoSize = true;
            this.lblInstrucciones.ForeColor = Branding.TextoPrincipal;
            this.lblInstrucciones.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblInstrucciones.Location = new System.Drawing.Point(0, 0);
            this.lblInstrucciones.Text = "Elegí una nave de la lista y hacé click en tu tablero para colocarla.";

            this.lstFlota.Location = new System.Drawing.Point(375, 30);
            this.lstFlota.Size = new System.Drawing.Size(200, 160);
            this.lstFlota.View = System.Windows.Forms.View.List;
            this.lstFlota.SmallImageList = this.imgFlota;
            this.lstFlota.MultiSelect = false;
            this.lstFlota.HideSelection = false;
            this.lstFlota.SelectedIndexChanged += new System.EventHandler(this.lstFlota_SelectedIndexChanged);

            this.lblRestantes.AutoSize = true;
            this.lblRestantes.ForeColor = Branding.TextoPrincipal;
            this.lblRestantes.Location = new System.Drawing.Point(375, 195);

            this.btnOrientacion.Location = new System.Drawing.Point(375, 220);
            this.btnOrientacion.Size = new System.Drawing.Size(200, 32);
            this.btnOrientacion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOrientacion.FlatAppearance.BorderColor = Branding.Secundario;
            this.btnOrientacion.BackColor = Branding.Panel;
            this.btnOrientacion.ForeColor = Branding.Secundario;
            this.btnOrientacion.Click += new System.EventHandler(this.btnOrientacion_Click);

            this.lblMensaje.AutoSize = false;
            this.lblMensaje.ForeColor = Branding.TextoPrincipal;
            this.lblMensaje.Location = new System.Drawing.Point(375, 260);
            this.lblMensaje.Size = new System.Drawing.Size(200, 80);

            this.Controls.Add(this.pnlTablero);
            this.Controls.Add(this.lblInstrucciones);
            this.Controls.Add(this.lstFlota);
            this.Controls.Add(this.lblRestantes);
            this.Controls.Add(this.btnOrientacion);
            this.Controls.Add(this.lblMensaje);
            this.Size = new System.Drawing.Size(600, 400);
            this.ResumeLayout(false);
        }
    }
}
