namespace GUI
{
    partial class FormAbandonar
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
        private System.Windows.Forms.Label lblMensaje;
        private System.Windows.Forms.Button btnJugador1;
        private System.Windows.Forms.Button btnJugador2;
        private System.Windows.Forms.Button btnCancelar;

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.btnJugador1 = new System.Windows.Forms.Button();
            this.btnJugador2 = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();

            this.BackColor = Branding.FondoVentana;

            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = Branding.TableroHundido;
            this.lblTitulo.Location = new System.Drawing.Point(25, 20);
            this.lblTitulo.Text = "¿Quién abandona la partida?";

            this.lblMensaje.AutoSize = false;
            this.lblMensaje.ForeColor = Branding.TextoPrincipal;
            this.lblMensaje.Location = new System.Drawing.Point(25, 55);
            this.lblMensaje.Size = new System.Drawing.Size(330, 50);

            this.btnJugador1.Location = new System.Drawing.Point(25, 115);
            this.btnJugador1.Size = new System.Drawing.Size(330, 36);
            this.btnJugador1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJugador1.FlatAppearance.BorderColor = Branding.Secundario;
            this.btnJugador1.BackColor = Branding.Panel;
            this.btnJugador1.ForeColor = Branding.Secundario;
            this.btnJugador1.Click += new System.EventHandler(this.btnJugador1_Click);

            this.btnJugador2.Location = new System.Drawing.Point(25, 160);
            this.btnJugador2.Size = new System.Drawing.Size(330, 36);
            this.btnJugador2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJugador2.FlatAppearance.BorderColor = Branding.Secundario;
            this.btnJugador2.BackColor = Branding.Panel;
            this.btnJugador2.ForeColor = Branding.Secundario;
            this.btnJugador2.Click += new System.EventHandler(this.btnJugador2_Click);

            this.btnCancelar.Location = new System.Drawing.Point(25, 210);
            this.btnCancelar.Size = new System.Drawing.Size(330, 32);
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.FlatAppearance.BorderColor = Branding.TextoPrincipal;
            this.btnCancelar.BackColor = Branding.Panel;
            this.btnCancelar.ForeColor = Branding.TextoPrincipal;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);

            this.ClientSize = new System.Drawing.Size(380, 270);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.btnJugador1);
            this.Controls.Add(this.btnJugador2);
            this.Controls.Add(this.btnCancelar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Abandonar partida";
            this.ResumeLayout(false);
        }
    }
}
