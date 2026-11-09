namespace GUI
{
    partial class FormPartida
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

        private System.Windows.Forms.Panel pnlContenido;
        private System.Windows.Forms.Button btnAbandonar;

        private void InitializeComponent()
        {
            this.pnlContenido = new System.Windows.Forms.Panel();
            this.btnAbandonar = new System.Windows.Forms.Button();
            this.SuspendLayout();

            this.BackColor = Branding.FondoVentana;

            this.pnlContenido.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContenido.Padding = new System.Windows.Forms.Padding(15);
            this.pnlContenido.BackColor = Branding.FondoVentana;

            this.btnAbandonar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnAbandonar.Height = 40;
            this.btnAbandonar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAbandonar.FlatAppearance.BorderColor = Branding.TableroHundido;
            this.btnAbandonar.BackColor = Branding.Panel;
            this.btnAbandonar.ForeColor = Branding.TableroHundido;
            this.btnAbandonar.Text = "Abandonar partida";
            this.btnAbandonar.Click += new System.EventHandler(this.btnAbandonar_Click);

            this.ClientSize = new System.Drawing.Size(670, 540);
            this.Controls.Add(this.btnAbandonar);
            this.Controls.Add(this.pnlContenido);
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Batalla Naval - Partida en curso";
            this.Load += new System.EventHandler(this.FormPartida_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPartida_FormClosing);
            this.ResumeLayout(false);
        }
    }
}
