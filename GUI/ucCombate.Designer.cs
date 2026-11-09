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

        // Disposición (basada en el mockup stitch_batalla_naval_winforms_ui/partida_en_curso):
        // banner de turno arriba, "Mi flota" a la izquierda, columna central con el resultado
        // del disparo / flota rival / registro, y el radar enemigo a la derecha.
        public static readonly System.Drawing.Point OrigenTableroPropio = new System.Drawing.Point(20, 106);
        public static readonly System.Drawing.Point OrigenTableroEnemigo = new System.Drawing.Point(644, 106);
        private const int LADO_TABLERO = 332;
        private const int X_CENTRO = 372;
        private const int ANCHO_CENTRO = 236;

        private System.Windows.Forms.Panel pnlBanner;
        private System.Windows.Forms.Label lblTurno;
        private System.Windows.Forms.Label lblIndicacion;
        private System.Windows.Forms.Label lblApuntando;
        private System.Windows.Forms.Label lblHeaderPropio;
        private System.Windows.Forms.Panel pnlTableroPropio;
        private System.Windows.Forms.Label lblEstadoPropio;
        private System.Windows.Forms.Label lblHeaderEnemigo;
        private System.Windows.Forms.Panel pnlTableroEnemigo;
        private System.Windows.Forms.Label lblEstadoEnemigo;
        private System.Windows.Forms.Label lblResultado;
        private System.Windows.Forms.Label lblDetalleResultado;
        private System.Windows.Forms.Button btnPasarTurno;
        private System.Windows.Forms.Label lblFlotaRival;
        private System.Windows.Forms.Panel pnlFlotaRival;
        private System.Windows.Forms.Label lblRegistro;
        private System.Windows.Forms.ListBox lstRegistro;

        private void InitializeComponent()
        {
            this.pnlBanner = new System.Windows.Forms.Panel();
            this.lblTurno = new System.Windows.Forms.Label();
            this.lblIndicacion = new System.Windows.Forms.Label();
            this.lblApuntando = new System.Windows.Forms.Label();
            this.lblHeaderPropio = new System.Windows.Forms.Label();
            this.pnlTableroPropio = new System.Windows.Forms.Panel();
            this.lblEstadoPropio = new System.Windows.Forms.Label();
            this.lblHeaderEnemigo = new System.Windows.Forms.Label();
            this.pnlTableroEnemigo = new System.Windows.Forms.Panel();
            this.lblEstadoEnemigo = new System.Windows.Forms.Label();
            this.lblResultado = new System.Windows.Forms.Label();
            this.lblDetalleResultado = new System.Windows.Forms.Label();
            this.btnPasarTurno = new System.Windows.Forms.Button();
            this.lblFlotaRival = new System.Windows.Forms.Label();
            this.pnlFlotaRival = new System.Windows.Forms.Panel();
            this.lblRegistro = new System.Windows.Forms.Label();
            this.lstRegistro = new System.Windows.Forms.ListBox();
            this.pnlBanner.SuspendLayout();
            this.SuspendLayout();

            this.BackColor = Branding.FondoVentana;

            // ---------- Banner de turno ----------
            this.pnlBanner.Location = new System.Drawing.Point(0, 0);
            this.pnlBanner.Size = new System.Drawing.Size(980, 50);
            this.pnlBanner.BackColor = Branding.Primario;

            this.lblTurno.AutoSize = true;
            this.lblTurno.ForeColor = System.Drawing.Color.White;
            this.lblTurno.Font = new System.Drawing.Font("Montserrat", 12F, System.Drawing.FontStyle.Bold);
            this.lblTurno.Location = new System.Drawing.Point(14, 4);

            this.lblIndicacion.AutoSize = true;
            this.lblIndicacion.ForeColor = System.Drawing.Color.White;
            this.lblIndicacion.Font = new System.Drawing.Font("Montserrat", 9F);
            this.lblIndicacion.Location = new System.Drawing.Point(15, 28);

            this.lblApuntando.AutoSize = false;
            this.lblApuntando.ForeColor = System.Drawing.Color.White;
            this.lblApuntando.Font = new System.Drawing.Font("Montserrat", 11F, System.Drawing.FontStyle.Bold);
            this.lblApuntando.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblApuntando.Location = new System.Drawing.Point(700, 13);
            this.lblApuntando.Size = new System.Drawing.Size(266, 24);

            this.pnlBanner.Controls.Add(this.lblTurno);
            this.pnlBanner.Controls.Add(this.lblIndicacion);
            this.pnlBanner.Controls.Add(this.lblApuntando);

            // ---------- Mi flota (izquierda) ----------
            this.lblHeaderPropio.AutoSize = true;
            this.lblHeaderPropio.ForeColor = Branding.TextoPrincipal;
            this.lblHeaderPropio.Font = new System.Drawing.Font("Montserrat", 10F, System.Drawing.FontStyle.Bold);
            this.lblHeaderPropio.Location = new System.Drawing.Point(OrigenTableroPropio.X - 4, 62);

            this.pnlTableroPropio.Location = OrigenTableroPropio;
            this.pnlTableroPropio.Size = new System.Drawing.Size(LADO_TABLERO, LADO_TABLERO);
            this.pnlTableroPropio.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlTableroPropio.BackColor = Branding.Panel;

            this.lblEstadoPropio.AutoSize = true;
            this.lblEstadoPropio.ForeColor = Branding.TextoPrincipal;
            this.lblEstadoPropio.Font = new System.Drawing.Font("Montserrat", 8.5F);
            this.lblEstadoPropio.Location = new System.Drawing.Point(OrigenTableroPropio.X, OrigenTableroPropio.Y + LADO_TABLERO + 6);

            // ---------- Radar enemigo (derecha) ----------
            this.lblHeaderEnemigo.AutoSize = true;
            this.lblHeaderEnemigo.ForeColor = Branding.TextoPrincipal;
            this.lblHeaderEnemigo.Font = new System.Drawing.Font("Montserrat", 10F, System.Drawing.FontStyle.Bold);
            this.lblHeaderEnemigo.Location = new System.Drawing.Point(OrigenTableroEnemigo.X - 4, 62);

            this.pnlTableroEnemigo.Location = OrigenTableroEnemigo;
            this.pnlTableroEnemigo.Size = new System.Drawing.Size(LADO_TABLERO, LADO_TABLERO);
            this.pnlTableroEnemigo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlTableroEnemigo.BackColor = Branding.Panel;

            this.lblEstadoEnemigo.AutoSize = true;
            this.lblEstadoEnemigo.ForeColor = Branding.TextoPrincipal;
            this.lblEstadoEnemigo.Font = new System.Drawing.Font("Montserrat", 8.5F);
            this.lblEstadoEnemigo.Location = new System.Drawing.Point(OrigenTableroEnemigo.X, OrigenTableroEnemigo.Y + LADO_TABLERO + 6);

            // ---------- Columna central ----------
            this.lblResultado.AutoSize = false;
            this.lblResultado.ForeColor = Branding.TextoPrincipal;
            this.lblResultado.Font = new System.Drawing.Font("Montserrat", 16F, System.Drawing.FontStyle.Bold);
            this.lblResultado.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblResultado.Location = new System.Drawing.Point(X_CENTRO, 60);
            this.lblResultado.Size = new System.Drawing.Size(ANCHO_CENTRO, 34);

            this.lblDetalleResultado.AutoSize = false;
            this.lblDetalleResultado.ForeColor = Branding.TextoPrincipal;
            this.lblDetalleResultado.Font = new System.Drawing.Font("Montserrat", 9F);
            this.lblDetalleResultado.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblDetalleResultado.Location = new System.Drawing.Point(X_CENTRO, 96);
            this.lblDetalleResultado.Size = new System.Drawing.Size(ANCHO_CENTRO, 40);

            this.btnPasarTurno.Location = new System.Drawing.Point(X_CENTRO, 140);
            this.btnPasarTurno.Size = new System.Drawing.Size(ANCHO_CENTRO, 36);
            this.btnPasarTurno.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPasarTurno.FlatAppearance.BorderSize = 0;
            this.btnPasarTurno.BackColor = Branding.Primario;
            this.btnPasarTurno.ForeColor = System.Drawing.Color.White;
            this.btnPasarTurno.Font = new System.Drawing.Font("Montserrat", 10F, System.Drawing.FontStyle.Bold);
            this.btnPasarTurno.Visible = false;
            this.btnPasarTurno.Click += new System.EventHandler(this.btnPasarTurno_Click);

            this.lblFlotaRival.AutoSize = true;
            this.lblFlotaRival.ForeColor = Branding.TextoPrincipal;
            this.lblFlotaRival.Font = new System.Drawing.Font("Montserrat", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblFlotaRival.Location = new System.Drawing.Point(X_CENTRO, 188);

            this.pnlFlotaRival.Location = new System.Drawing.Point(X_CENTRO, 210);
            this.pnlFlotaRival.Size = new System.Drawing.Size(ANCHO_CENTRO, 132);
            this.pnlFlotaRival.BackColor = Branding.Panel;
            this.pnlFlotaRival.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.lblRegistro.AutoSize = true;
            this.lblRegistro.ForeColor = Branding.TextoPrincipal;
            this.lblRegistro.Font = new System.Drawing.Font("Montserrat", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblRegistro.Location = new System.Drawing.Point(X_CENTRO, 350);
            this.lblRegistro.Text = "Registro de disparos";

            this.lstRegistro.Location = new System.Drawing.Point(X_CENTRO, 372);
            this.lstRegistro.Size = new System.Drawing.Size(ANCHO_CENTRO, 94);
            this.lstRegistro.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstRegistro.Font = new System.Drawing.Font("Montserrat", 8.5F);
            this.lstRegistro.ForeColor = Branding.TextoPrincipal;
            this.lstRegistro.BackColor = Branding.Panel;
            this.lstRegistro.IntegralHeight = false;
            this.lstRegistro.SelectionMode = System.Windows.Forms.SelectionMode.None;

            this.Controls.Add(this.pnlBanner);
            this.Controls.Add(this.lblHeaderPropio);
            this.Controls.Add(this.pnlTableroPropio);
            this.Controls.Add(this.lblEstadoPropio);
            this.Controls.Add(this.lblHeaderEnemigo);
            this.Controls.Add(this.pnlTableroEnemigo);
            this.Controls.Add(this.lblEstadoEnemigo);
            this.Controls.Add(this.lblResultado);
            this.Controls.Add(this.lblDetalleResultado);
            this.Controls.Add(this.btnPasarTurno);
            this.Controls.Add(this.lblFlotaRival);
            this.Controls.Add(this.pnlFlotaRival);
            this.Controls.Add(this.lblRegistro);
            this.Controls.Add(this.lstRegistro);
            this.Size = new System.Drawing.Size(980, 505);
            this.pnlBanner.ResumeLayout(false);
            this.pnlBanner.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
