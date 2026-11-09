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

        public static readonly System.Drawing.Point OrigenTableroPropio = new System.Drawing.Point(20, 62);
        public static readonly System.Drawing.Point OrigenTableroEnemigo = new System.Drawing.Point(330, 62);

        private System.Windows.Forms.Label lblTurno;
        private System.Windows.Forms.Label lblHeaderPropio;
        private System.Windows.Forms.Panel pnlTableroPropio;
        private System.Windows.Forms.Label lblHeaderEnemigo;
        private System.Windows.Forms.Panel pnlTableroEnemigo;
        private System.Windows.Forms.Label lblResultado;

        private void InitializeComponent()
        {
            this.lblTurno = new System.Windows.Forms.Label();
            this.lblHeaderPropio = new System.Windows.Forms.Label();
            this.pnlTableroPropio = new System.Windows.Forms.Panel();
            this.lblHeaderEnemigo = new System.Windows.Forms.Label();
            this.pnlTableroEnemigo = new System.Windows.Forms.Panel();
            this.lblResultado = new System.Windows.Forms.Label();
            this.SuspendLayout();

            this.BackColor = Branding.FondoVentana;

            this.lblTurno.AutoSize = true;
            this.lblTurno.ForeColor = Branding.Primario;
            this.lblTurno.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblTurno.Location = new System.Drawing.Point(0, 0);

            this.lblHeaderPropio.AutoSize = true;
            this.lblHeaderPropio.ForeColor = Branding.TextoPrincipal;
            this.lblHeaderPropio.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblHeaderPropio.Location = new System.Drawing.Point(OrigenTableroPropio.X, 28);

            this.pnlTableroPropio.Location = OrigenTableroPropio;
            this.pnlTableroPropio.Size = new System.Drawing.Size(271, 271);
            this.pnlTableroPropio.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlTableroPropio.BackColor = Branding.Panel;

            this.lblHeaderEnemigo.AutoSize = true;
            this.lblHeaderEnemigo.ForeColor = Branding.TextoPrincipal;
            this.lblHeaderEnemigo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblHeaderEnemigo.Location = new System.Drawing.Point(OrigenTableroEnemigo.X, 28);

            this.pnlTableroEnemigo.Location = OrigenTableroEnemigo;
            this.pnlTableroEnemigo.Size = new System.Drawing.Size(271, 271);
            this.pnlTableroEnemigo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlTableroEnemigo.BackColor = Branding.Panel;

            this.lblResultado.AutoSize = false;
            this.lblResultado.ForeColor = Branding.Secundario;
            this.lblResultado.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblResultado.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblResultado.Location = new System.Drawing.Point(20, 340);
            this.lblResultado.Size = new System.Drawing.Size(580, 30);

            this.Controls.Add(this.lblTurno);
            this.Controls.Add(this.lblHeaderPropio);
            this.Controls.Add(this.pnlTableroPropio);
            this.Controls.Add(this.lblHeaderEnemigo);
            this.Controls.Add(this.pnlTableroEnemigo);
            this.Controls.Add(this.lblResultado);
            this.Size = new System.Drawing.Size(640, 420);
            this.ResumeLayout(false);
        }
    }
}
