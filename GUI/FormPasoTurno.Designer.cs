namespace GUI
{
    partial class FormPasoTurno
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

        private System.Windows.Forms.Label lblMensaje;
        private System.Windows.Forms.Button btnContinuar;

        private void InitializeComponent()
        {
            this.lblMensaje = new System.Windows.Forms.Label();
            this.btnContinuar = new System.Windows.Forms.Button();
            this.SuspendLayout();

            this.BackColor = Branding.FondoVentana;

            this.lblMensaje.AutoSize = false;
            this.lblMensaje.Font = Fuentes.Montserrat(13F);
            this.lblMensaje.ForeColor = Branding.TextoPrincipal;
            this.lblMensaje.Location = new System.Drawing.Point(30, 30);
            this.lblMensaje.Size = new System.Drawing.Size(340, 90);
            this.lblMensaje.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.btnContinuar.Location = new System.Drawing.Point(30, 130);
            this.btnContinuar.Size = new System.Drawing.Size(340, 38);
            this.btnContinuar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnContinuar.FlatAppearance.BorderSize = 0;
            this.btnContinuar.BackColor = Branding.Primario;
            this.btnContinuar.ForeColor = System.Drawing.Color.White;
            this.btnContinuar.Font = Fuentes.Montserrat(10F, System.Drawing.FontStyle.Bold);
            this.btnContinuar.Text = "Continuar";
            this.btnContinuar.Click += new System.EventHandler(this.btnContinuar_Click);

            this.AcceptButton = this.btnContinuar;
            this.ClientSize = new System.Drawing.Size(400, 195);
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.btnContinuar);
            this.ControlBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Batalla Naval";
            this.ResumeLayout(false);
        }
    }
}
