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

        // Disposición (basada en el mockup de diseño):
        // banner con el jugador que coloca, tablero a la izquierda y, a la derecha, las
        // herramientas (girar / al azar / reiniciar), la flota con su estado y "Confirmar flota".
        public static readonly System.Drawing.Point OrigenTablero = new System.Drawing.Point(24, 86);
        private const int LADO_TABLERO = 362;
        private const int X_DER = 410;
        private const int ANCHO_DER = 420;

        private System.Windows.Forms.Panel pnlBanner;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblIndicacion;
        private System.Windows.Forms.Label lblPosicion;
        private System.Windows.Forms.Panel pnlTablero;
        private System.Windows.Forms.Button btnGirar;
        private System.Windows.Forms.Button btnAzar;
        private System.Windows.Forms.Button btnReiniciar;
        private System.Windows.Forms.Label lblFlota;
        private System.Windows.Forms.Label lblRestantes;
        private System.Windows.Forms.Panel pnlFlota;
        private System.Windows.Forms.Panel pnlProgreso;
        private System.Windows.Forms.Label lblAyuda;
        private System.Windows.Forms.Label lblMensaje;
        private System.Windows.Forms.Button btnConfirmar;
        private System.Windows.Forms.ToolTip toolTip;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlBanner = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblIndicacion = new System.Windows.Forms.Label();
            this.lblPosicion = new System.Windows.Forms.Label();
            this.pnlTablero = new System.Windows.Forms.Panel();
            this.btnGirar = new System.Windows.Forms.Button();
            this.btnAzar = new System.Windows.Forms.Button();
            this.btnReiniciar = new System.Windows.Forms.Button();
            this.lblFlota = new System.Windows.Forms.Label();
            this.lblRestantes = new System.Windows.Forms.Label();
            this.pnlFlota = new System.Windows.Forms.Panel();
            this.pnlProgreso = new System.Windows.Forms.Panel();
            this.lblAyuda = new System.Windows.Forms.Label();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.btnConfirmar = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.pnlBanner.SuspendLayout();
            this.SuspendLayout();

            this.BackColor = Branding.FondoVentana;

            // ---------- Banner ----------
            this.pnlBanner.Location = new System.Drawing.Point(0, 0);
            this.pnlBanner.Size = new System.Drawing.Size(830, 50);
            this.pnlBanner.BackColor = Branding.Primario;

            this.lblTitulo.AutoSize = true;
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(14, 4);

            this.lblIndicacion.AutoSize = true;
            this.lblIndicacion.ForeColor = System.Drawing.Color.White;
            this.lblIndicacion.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblIndicacion.Location = new System.Drawing.Point(15, 28);

            this.lblPosicion.AutoSize = false;
            this.lblPosicion.ForeColor = System.Drawing.Color.White;
            this.lblPosicion.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPosicion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblPosicion.Location = new System.Drawing.Point(500, 13);
            this.lblPosicion.Size = new System.Drawing.Size(316, 24);

            this.pnlBanner.Controls.Add(this.lblTitulo);
            this.pnlBanner.Controls.Add(this.lblIndicacion);
            this.pnlBanner.Controls.Add(this.lblPosicion);

            // ---------- Tablero ----------
            this.pnlTablero.Location = OrigenTablero;
            this.pnlTablero.Size = new System.Drawing.Size(LADO_TABLERO, LADO_TABLERO);
            this.pnlTablero.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlTablero.BackColor = Branding.Panel;

            // ---------- Herramientas ----------
            ConfigurarHerramienta(this.btnGirar, X_DER, 180, true);
            ConfigurarHerramienta(this.btnAzar, X_DER + 190, 110, false);
            ConfigurarHerramienta(this.btnReiniciar, X_DER + 310, 110, false);
            this.btnAzar.Text = "Al azar";
            this.btnReiniciar.Text = "Reiniciar";
            this.btnGirar.Click += new System.EventHandler(this.btnGirar_Click);
            this.btnAzar.Click += new System.EventHandler(this.btnAzar_Click);
            this.btnReiniciar.Click += new System.EventHandler(this.btnReiniciar_Click);
            this.toolTip.SetToolTip(this.btnGirar, "Orientación con la que se coloca la próxima nave (tecla R o click derecho en el tablero).");
            this.toolTip.SetToolTip(this.btnAzar, "Coloca al azar las naves que faltan; con la flota completa, la vuelve a distribuir entera. Después se puede seguir corrigiendo.");
            this.toolTip.SetToolTip(this.btnReiniciar, "Saca todas las naves del tablero.");

            // ---------- Flota ----------
            this.lblFlota.AutoSize = true;
            this.lblFlota.ForeColor = Branding.TextoPrincipal;
            this.lblFlota.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblFlota.Location = new System.Drawing.Point(X_DER - 2, 112);
            this.lblFlota.Text = "Tu flota";

            this.lblRestantes.AutoSize = false;
            this.lblRestantes.ForeColor = Branding.Secundario;
            this.lblRestantes.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblRestantes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblRestantes.Location = new System.Drawing.Point(X_DER + ANCHO_DER - 200, 112);
            this.lblRestantes.Size = new System.Drawing.Size(200, 20);

            this.pnlFlota.Location = new System.Drawing.Point(X_DER, 136);
            this.pnlFlota.Size = new System.Drawing.Size(ANCHO_DER, 228);
            this.pnlFlota.BackColor = Branding.FondoVentana;

            this.pnlProgreso.Location = new System.Drawing.Point(X_DER, 370);
            this.pnlProgreso.Size = new System.Drawing.Size(ANCHO_DER, 8);
            this.pnlProgreso.BackColor = Branding.FondoVentana;
            this.pnlProgreso.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlProgreso_Paint);

            this.lblAyuda.AutoSize = false;
            this.lblAyuda.ForeColor = Branding.Secundario;
            this.lblAyuda.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblAyuda.Location = new System.Drawing.Point(X_DER, 386);
            this.lblAyuda.Size = new System.Drawing.Size(ANCHO_DER, 34);
            this.lblAyuda.Text = "Sobre una nave del tablero: click para girarla, arrastrar para moverla, " +
                "click derecho para sacarla.";

            this.lblMensaje.AutoSize = false;
            this.lblMensaje.ForeColor = Branding.TableroHundido;
            this.lblMensaje.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblMensaje.Location = new System.Drawing.Point(X_DER, 422);
            this.lblMensaje.Size = new System.Drawing.Size(ANCHO_DER, 22);

            this.btnConfirmar.Location = new System.Drawing.Point(X_DER, 448);
            this.btnConfirmar.Size = new System.Drawing.Size(ANCHO_DER, 44);
            this.btnConfirmar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirmar.FlatAppearance.BorderSize = 0;
            this.btnConfirmar.BackColor = Branding.Primario;
            this.btnConfirmar.ForeColor = System.Drawing.Color.White;
            this.btnConfirmar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnConfirmar.Text = "Confirmar flota";
            this.btnConfirmar.Enabled = false;
            this.btnConfirmar.Click += new System.EventHandler(this.btnConfirmar_Click);

            this.Controls.Add(this.pnlBanner);
            this.Controls.Add(this.pnlTablero);
            this.Controls.Add(this.btnGirar);
            this.Controls.Add(this.btnAzar);
            this.Controls.Add(this.btnReiniciar);
            this.Controls.Add(this.lblFlota);
            this.Controls.Add(this.lblRestantes);
            this.Controls.Add(this.pnlFlota);
            this.Controls.Add(this.pnlProgreso);
            this.Controls.Add(this.lblAyuda);
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.btnConfirmar);
            this.Size = new System.Drawing.Size(830, 500);
            this.pnlBanner.ResumeLayout(false);
            this.pnlBanner.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void ConfigurarHerramienta(System.Windows.Forms.Button boton, int x, int ancho, bool principal)
        {
            boton.Location = new System.Drawing.Point(x, 66);
            boton.Size = new System.Drawing.Size(ancho, 34);
            boton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            boton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            if (principal)
            {
                boton.FlatAppearance.BorderSize = 0;
                boton.BackColor = Branding.Secundario;
                boton.ForeColor = System.Drawing.Color.White;
            }
            else
            {
                boton.FlatAppearance.BorderColor = Branding.Secundario;
                boton.BackColor = Branding.Panel;
                boton.ForeColor = Branding.Secundario;
            }
        }
    }
}
