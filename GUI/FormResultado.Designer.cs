namespace GUI
{
    partial class FormResultado
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

        private System.Windows.Forms.Label lblMedalla;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblDetalle;
        private System.Windows.Forms.Panel pnlResumen;
        private System.Windows.Forms.Label lblJugador1Clave;
        private System.Windows.Forms.Label lblJugador1Valor;
        private System.Windows.Forms.Label lblJugador2Clave;
        private System.Windows.Forms.Label lblJugador2Valor;
        private System.Windows.Forms.Label lblDuracionClave;
        private System.Windows.Forms.Label lblDuracionValor;
        private System.Windows.Forms.Label lblResultadoClave;
        private System.Windows.Forms.Label lblResultadoValor;
        private System.Windows.Forms.Button btnVerMovimientos;
        private System.Windows.Forms.Button btnVolver;

        private void InitializeComponent()
        {
            this.lblMedalla = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblDetalle = new System.Windows.Forms.Label();
            this.pnlResumen = new System.Windows.Forms.Panel();
            this.lblJugador1Clave = new System.Windows.Forms.Label();
            this.lblJugador1Valor = new System.Windows.Forms.Label();
            this.lblJugador2Clave = new System.Windows.Forms.Label();
            this.lblJugador2Valor = new System.Windows.Forms.Label();
            this.lblDuracionClave = new System.Windows.Forms.Label();
            this.lblDuracionValor = new System.Windows.Forms.Label();
            this.lblResultadoClave = new System.Windows.Forms.Label();
            this.lblResultadoValor = new System.Windows.Forms.Label();
            this.btnVerMovimientos = new System.Windows.Forms.Button();
            this.btnVolver = new System.Windows.Forms.Button();
            this.pnlResumen.SuspendLayout();
            this.SuspendLayout();

            this.BackColor = Branding.FondoVentana;

            this.lblMedalla.Text = "🏆";
            this.lblMedalla.Font = new System.Drawing.Font("Segoe UI Emoji", 22F);
            this.lblMedalla.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMedalla.Size = new System.Drawing.Size(64, 64);
            this.lblMedalla.Location = new System.Drawing.Point(108, 24);
            this.lblMedalla.BackColor = System.Drawing.Color.FromArgb(253, 240, 245);

            this.lblTitulo.AutoSize = false;
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = Branding.Primario;
            this.lblTitulo.Location = new System.Drawing.Point(20, 96);
            this.lblTitulo.Size = new System.Drawing.Size(240, 30);

            this.lblDetalle.AutoSize = false;
            this.lblDetalle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblDetalle.ForeColor = Branding.TextoPrincipal;
            this.lblDetalle.Location = new System.Drawing.Point(20, 128);
            this.lblDetalle.Size = new System.Drawing.Size(240, 36);

            this.pnlResumen.BackColor = System.Drawing.Color.FromArgb(248, 242, 250);
            this.pnlResumen.Location = new System.Drawing.Point(30, 172);
            this.pnlResumen.Size = new System.Drawing.Size(220, 112);
            this.pnlResumen.Padding = new System.Windows.Forms.Padding(14, 10, 14, 10);

            this.lblJugador1Clave.AutoSize = true;
            this.lblJugador1Clave.ForeColor = System.Drawing.Color.Gray;
            this.lblJugador1Clave.Location = new System.Drawing.Point(14, 10);
            this.lblJugador1Clave.Text = "Jugador 1";
            this.lblJugador1Valor.AutoSize = true;
            this.lblJugador1Valor.ForeColor = Branding.TextoPrincipal;
            this.lblJugador1Valor.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblJugador1Valor.Location = new System.Drawing.Point(140, 10);

            this.lblJugador2Clave.AutoSize = true;
            this.lblJugador2Clave.ForeColor = System.Drawing.Color.Gray;
            this.lblJugador2Clave.Location = new System.Drawing.Point(14, 35);
            this.lblJugador2Clave.Text = "Jugador 2";
            this.lblJugador2Valor.AutoSize = true;
            this.lblJugador2Valor.ForeColor = Branding.TextoPrincipal;
            this.lblJugador2Valor.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblJugador2Valor.Location = new System.Drawing.Point(140, 35);

            this.lblDuracionClave.AutoSize = true;
            this.lblDuracionClave.ForeColor = System.Drawing.Color.Gray;
            this.lblDuracionClave.Location = new System.Drawing.Point(14, 60);
            this.lblDuracionClave.Text = "Duración";
            this.lblDuracionValor.AutoSize = true;
            this.lblDuracionValor.ForeColor = Branding.TextoPrincipal;
            this.lblDuracionValor.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblDuracionValor.Location = new System.Drawing.Point(140, 60);

            this.lblResultadoClave.AutoSize = true;
            this.lblResultadoClave.ForeColor = System.Drawing.Color.Gray;
            this.lblResultadoClave.Location = new System.Drawing.Point(14, 85);
            this.lblResultadoClave.Text = "Resultado";
            this.lblResultadoValor.AutoSize = true;
            this.lblResultadoValor.ForeColor = Branding.TextoPrincipal;
            this.lblResultadoValor.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblResultadoValor.Location = new System.Drawing.Point(140, 85);

            this.pnlResumen.Controls.Add(this.lblJugador1Clave);
            this.pnlResumen.Controls.Add(this.lblJugador1Valor);
            this.pnlResumen.Controls.Add(this.lblJugador2Clave);
            this.pnlResumen.Controls.Add(this.lblJugador2Valor);
            this.pnlResumen.Controls.Add(this.lblDuracionClave);
            this.pnlResumen.Controls.Add(this.lblDuracionValor);
            this.pnlResumen.Controls.Add(this.lblResultadoClave);
            this.pnlResumen.Controls.Add(this.lblResultadoValor);

            this.btnVerMovimientos.Location = new System.Drawing.Point(30, 298);
            this.btnVerMovimientos.Size = new System.Drawing.Size(220, 36);
            this.btnVerMovimientos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVerMovimientos.FlatAppearance.BorderSize = 0;
            this.btnVerMovimientos.BackColor = Branding.Primario;
            this.btnVerMovimientos.ForeColor = System.Drawing.Color.White;
            this.btnVerMovimientos.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnVerMovimientos.Text = "Ver movimientos de esta partida";
            this.btnVerMovimientos.Click += new System.EventHandler(this.btnVerMovimientos_Click);

            this.btnVolver.Location = new System.Drawing.Point(30, 340);
            this.btnVolver.Size = new System.Drawing.Size(220, 34);
            this.btnVolver.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVolver.FlatAppearance.BorderColor = Branding.Secundario;
            this.btnVolver.BackColor = Branding.Panel;
            this.btnVolver.ForeColor = Branding.Secundario;
            this.btnVolver.Text = "Volver al Menú Principal";
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);

            this.AcceptButton = this.btnVolver;
            this.ClientSize = new System.Drawing.Size(280, 395);
            this.Controls.Add(this.lblMedalla);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblDetalle);
            this.Controls.Add(this.pnlResumen);
            this.Controls.Add(this.btnVerMovimientos);
            this.Controls.Add(this.btnVolver);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Batalla Naval - Fin de la partida";
            this.pnlResumen.ResumeLayout(false);
            this.pnlResumen.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
