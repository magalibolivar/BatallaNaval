namespace GUI
{
    partial class ucCombate
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

        private System.Windows.Forms.Label lblTurno;
        private System.Windows.Forms.Panel pnlTablero;
        private System.Windows.Forms.Label lblResumenPropio;
        private System.Windows.Forms.Label lblResumenRival;
        private System.Windows.Forms.Label lblResultado;

        private void InitializeComponent()
        {
            this.lblTurno = new System.Windows.Forms.Label();
            this.pnlTablero = new System.Windows.Forms.Panel();
            this.lblResumenPropio = new System.Windows.Forms.Label();
            this.lblResumenRival = new System.Windows.Forms.Label();
            this.lblResultado = new System.Windows.Forms.Label();
            this.SuspendLayout();

            this.BackColor = Branding.FondoVentana;

            this.lblTurno.AutoSize = true;
            this.lblTurno.ForeColor = Branding.Primario;
            this.lblTurno.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblTurno.Location = new System.Drawing.Point(0, 0);

            this.pnlTablero.Location = new System.Drawing.Point(0, 30);
            this.pnlTablero.Size = new System.Drawing.Size(361, 361);
            this.pnlTablero.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlTablero.BackColor = Branding.Panel;

            this.lblResumenPropio.AutoSize = true;
            this.lblResumenPropio.ForeColor = Branding.TextoPrincipal;
            this.lblResumenPropio.Location = new System.Drawing.Point(375, 30);

            this.lblResumenRival.AutoSize = true;
            this.lblResumenRival.ForeColor = Branding.TextoPrincipal;
            this.lblResumenRival.Location = new System.Drawing.Point(375, 55);

            this.lblResultado.AutoSize = false;
            this.lblResultado.ForeColor = Branding.Secundario;
            this.lblResultado.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblResultado.Location = new System.Drawing.Point(375, 90);
            this.lblResultado.Size = new System.Drawing.Size(200, 50);

            this.Controls.Add(this.lblTurno);
            this.Controls.Add(this.pnlTablero);
            this.Controls.Add(this.lblResumenPropio);
            this.Controls.Add(this.lblResumenRival);
            this.Controls.Add(this.lblResultado);
            this.Size = new System.Drawing.Size(600, 400);
            this.ResumeLayout(false);
        }
    }
}
