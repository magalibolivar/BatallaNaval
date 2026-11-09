namespace GUI
{
    partial class FormPrincipal
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

        // Disposición (basada en el mockup de diseño): a la
        // izquierda los 2 jugadores con sus estadísticas y el botón para comenzar; a la
        // derecha las últimas partidas del Jugador 1; abajo las acciones secundarias.
        private const int X_IZQ = 25;
        private const int ANCHO_IZQ = 340;
        private const int X_DER = 385;
        private const int ANCHO_DER = 230;

        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Label lblTituloBatalla;
        private System.Windows.Forms.Label lblTituloNaval;
        private System.Windows.Forms.Label lblSubtitulo;

        private System.Windows.Forms.Label lblSeccionJugadores;
        private System.Windows.Forms.Panel pnlJugador1;
        private System.Windows.Forms.Label lblAvatar1;
        private System.Windows.Forms.Label lblJugador1;
        private System.Windows.Forms.Label lblChip1;
        private System.Windows.Forms.Label lblStats1;
        private System.Windows.Forms.Panel pnlJugador2;
        private System.Windows.Forms.Label lblAvatar2;
        private System.Windows.Forms.Label lblJugador2;
        private System.Windows.Forms.Label lblChip2;
        private System.Windows.Forms.Label lblStats2;
        private System.Windows.Forms.Button btnLoginJugador2;
        private System.Windows.Forms.Label lblCaraACara;
        private System.Windows.Forms.Button btnComenzarPartida;
        private System.Windows.Forms.Label lblAyudaComenzar;

        private System.Windows.Forms.Label lblSeccionHistorial;
        private System.Windows.Forms.Panel pnlHistorial;
        private System.Windows.Forms.LinkLabel lnkVerEstadisticas;

        private System.Windows.Forms.Panel pnlSeparador;
        private System.Windows.Forms.Button btnEstadisticas;
        private System.Windows.Forms.Button btnReglas;
        private System.Windows.Forms.Button btnBackup;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Button btnCerrarSesion;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.SaveFileDialog sfdBackup;
        private System.Windows.Forms.OpenFileDialog ofdRestore;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.lblTituloBatalla = new System.Windows.Forms.Label();
            this.lblTituloNaval = new System.Windows.Forms.Label();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblSeccionJugadores = new System.Windows.Forms.Label();
            this.pnlJugador1 = new System.Windows.Forms.Panel();
            this.lblAvatar1 = new System.Windows.Forms.Label();
            this.lblJugador1 = new System.Windows.Forms.Label();
            this.lblChip1 = new System.Windows.Forms.Label();
            this.lblStats1 = new System.Windows.Forms.Label();
            this.pnlJugador2 = new System.Windows.Forms.Panel();
            this.lblAvatar2 = new System.Windows.Forms.Label();
            this.lblJugador2 = new System.Windows.Forms.Label();
            this.lblChip2 = new System.Windows.Forms.Label();
            this.lblStats2 = new System.Windows.Forms.Label();
            this.btnLoginJugador2 = new System.Windows.Forms.Button();
            this.lblCaraACara = new System.Windows.Forms.Label();
            this.btnComenzarPartida = new System.Windows.Forms.Button();
            this.lblAyudaComenzar = new System.Windows.Forms.Label();
            this.lblSeccionHistorial = new System.Windows.Forms.Label();
            this.pnlHistorial = new System.Windows.Forms.Panel();
            this.lnkVerEstadisticas = new System.Windows.Forms.LinkLabel();
            this.pnlSeparador = new System.Windows.Forms.Panel();
            this.btnEstadisticas = new System.Windows.Forms.Button();
            this.btnReglas = new System.Windows.Forms.Button();
            this.btnBackup = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.btnCerrarSesion = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.sfdBackup = new System.Windows.Forms.SaveFileDialog();
            this.ofdRestore = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.pnlJugador1.SuspendLayout();
            this.pnlJugador2.SuspendLayout();
            this.SuspendLayout();

            this.BackColor = Branding.FondoVentana;
            this.Font = Fuentes.Montserrat(9F);

            // ---------- Encabezado ----------
            this.picLogo.Location = new System.Drawing.Point(X_IZQ, 12);
            this.picLogo.Size = new System.Drawing.Size(75, 65);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.Image = RecursosNaves.LogoBatallaNaval();

            this.lblTituloBatalla.AutoSize = true;
            this.lblTituloBatalla.Font = Fuentes.Montserrat(13F, System.Drawing.FontStyle.Bold);
            this.lblTituloBatalla.ForeColor = Branding.TextoPrincipal;
            this.lblTituloBatalla.Location = new System.Drawing.Point(108, 12);
            this.lblTituloBatalla.Text = "BATALLA";

            this.lblTituloNaval.AutoSize = true;
            this.lblTituloNaval.Font = Fuentes.Montserrat(19F, System.Drawing.FontStyle.Bold);
            this.lblTituloNaval.ForeColor = Branding.Primario;
            this.lblTituloNaval.Location = new System.Drawing.Point(105, 34);
            this.lblTituloNaval.Text = "NAVAL";

            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.ForeColor = Branding.Secundario;
            this.lblSubtitulo.Location = new System.Drawing.Point(232, 50);
            this.lblSubtitulo.Text = "2 jugadores · misma PC · tablero de 15x15";

            // ---------- Jugadores ----------
            this.lblSeccionJugadores.AutoSize = true;
            this.lblSeccionJugadores.Font = Fuentes.Montserrat(10F, System.Drawing.FontStyle.Bold);
            this.lblSeccionJugadores.ForeColor = Branding.TextoPrincipal;
            this.lblSeccionJugadores.Location = new System.Drawing.Point(X_IZQ - 2, 94);
            this.lblSeccionJugadores.Text = "Jugadores";

            ConfigurarTarjeta(this.pnlJugador1, this.lblAvatar1, this.lblJugador1, this.lblChip1, this.lblStats1, 118);
            ConfigurarTarjeta(this.pnlJugador2, this.lblAvatar2, this.lblJugador2, this.lblChip2, this.lblStats2, 190);

            this.btnLoginJugador2.Location = new System.Drawing.Point(244, 31);
            this.btnLoginJugador2.Size = new System.Drawing.Size(84, 26);
            this.btnLoginJugador2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoginJugador2.FlatAppearance.BorderColor = Branding.Secundario;
            this.btnLoginJugador2.BackColor = Branding.Panel;
            this.btnLoginJugador2.ForeColor = Branding.Secundario;
            this.btnLoginJugador2.Font = Fuentes.Montserrat(8.5F, System.Drawing.FontStyle.Bold);
            this.btnLoginJugador2.Text = "Ingresar";
            this.btnLoginJugador2.Click += new System.EventHandler(this.btnLoginJugador2_Click);
            this.pnlJugador2.Controls.Add(this.btnLoginJugador2);
            this.btnLoginJugador2.BringToFront();

            this.lblCaraACara.AutoSize = false;
            this.lblCaraACara.ForeColor = Branding.TextoPrincipal;
            this.lblCaraACara.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCaraACara.Location = new System.Drawing.Point(X_IZQ, 262);
            this.lblCaraACara.Size = new System.Drawing.Size(ANCHO_IZQ, 22);

            this.btnComenzarPartida.Location = new System.Drawing.Point(X_IZQ, 288);
            this.btnComenzarPartida.Size = new System.Drawing.Size(ANCHO_IZQ, 46);
            this.btnComenzarPartida.Font = Fuentes.Montserrat(12F, System.Drawing.FontStyle.Bold);
            this.btnComenzarPartida.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnComenzarPartida.FlatAppearance.BorderSize = 0;
            this.btnComenzarPartida.BackColor = Branding.Primario;
            this.btnComenzarPartida.ForeColor = System.Drawing.Color.White;
            this.btnComenzarPartida.Text = "Comenzar partida";
            this.btnComenzarPartida.Click += new System.EventHandler(this.btnComenzarPartida_Click);

            this.lblAyudaComenzar.AutoSize = false;
            this.lblAyudaComenzar.ForeColor = Branding.Secundario;
            this.lblAyudaComenzar.Font = Fuentes.Montserrat(8.5F);
            this.lblAyudaComenzar.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblAyudaComenzar.Location = new System.Drawing.Point(X_IZQ, 337);
            this.lblAyudaComenzar.Size = new System.Drawing.Size(ANCHO_IZQ, 20);

            // ---------- Últimas partidas ----------
            this.lblSeccionHistorial.AutoSize = true;
            this.lblSeccionHistorial.Font = Fuentes.Montserrat(10F, System.Drawing.FontStyle.Bold);
            this.lblSeccionHistorial.ForeColor = Branding.TextoPrincipal;
            this.lblSeccionHistorial.Location = new System.Drawing.Point(X_DER - 2, 94);

            this.pnlHistorial.Location = new System.Drawing.Point(X_DER, 118);
            this.pnlHistorial.Size = new System.Drawing.Size(ANCHO_DER, 216);
            this.pnlHistorial.BackColor = Branding.Panel;
            this.pnlHistorial.Paint += new System.Windows.Forms.PaintEventHandler(this.DibujarBorde);

            this.lnkVerEstadisticas.AutoSize = true;
            this.lnkVerEstadisticas.LinkColor = Branding.Primario;
            this.lnkVerEstadisticas.ActiveLinkColor = Branding.TextoPrincipal;
            this.lnkVerEstadisticas.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkVerEstadisticas.Location = new System.Drawing.Point(X_DER, 339);
            this.lnkVerEstadisticas.Text = "Ver estadísticas completas ›";
            this.lnkVerEstadisticas.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkVerEstadisticas_LinkClicked);

            // ---------- Acciones secundarias ----------
            this.pnlSeparador.Location = new System.Drawing.Point(X_IZQ, 370);
            this.pnlSeparador.Size = new System.Drawing.Size(X_DER + ANCHO_DER - X_IZQ, 1);
            this.pnlSeparador.BackColor = Branding.TableroVacio;

            ConfigurarBotonSecundario(this.btnEstadisticas, "Estadísticas", X_IZQ, 100, Branding.Secundario);
            ConfigurarBotonSecundario(this.btnReglas, "Reglas", X_IZQ + 110, 100, Branding.Secundario);
            ConfigurarBotonSecundario(this.btnBackup, "Backup", X_IZQ + 220, 100, Branding.Secundario);
            ConfigurarBotonSecundario(this.btnRestore, "Restore", X_IZQ + 330, 100, Branding.Secundario);
            ConfigurarBotonSecundario(this.btnCerrarSesion, "Cerrar sesión", X_IZQ + 470, 120, Branding.TextoPrincipal);
            this.btnEstadisticas.Click += new System.EventHandler(this.btnEstadisticas_Click);
            this.btnReglas.Click += new System.EventHandler(this.btnReglas_Click);
            this.btnBackup.Click += new System.EventHandler(this.btnBackup_Click);
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            this.btnCerrarSesion.Click += new System.EventHandler(this.btnCerrarSesion_Click);

            this.toolTip.SetToolTip(this.btnEstadisticas, "Partidas ganadas y perdidas, tiempo jugado e historial con los movimientos de cada partida.");
            this.toolTip.SetToolTip(this.btnReglas, "Reglas del juego.");
            this.toolTip.SetToolTip(this.btnBackup, "Guarda una copia de seguridad de la base de datos.");
            this.toolTip.SetToolTip(this.btnRestore, "Reemplaza la base de datos por un backup (después hay que volver a iniciar sesión).");
            this.toolTip.SetToolTip(this.btnCerrarSesion, "Cierra la sesión de los 2 jugadores y sale de la aplicación.");

            this.sfdBackup.Filter = "Backup de SQL Server (*.bak)|*.bak";
            this.ofdRestore.Filter = "Backup de SQL Server (*.bak)|*.bak";

            this.AcceptButton = this.btnComenzarPartida;
            this.ClientSize = new System.Drawing.Size(640, 428);
            this.Controls.Add(this.picLogo);
            this.Controls.Add(this.lblTituloBatalla);
            this.Controls.Add(this.lblTituloNaval);
            this.Controls.Add(this.lblSubtitulo);
            this.Controls.Add(this.lblSeccionJugadores);
            this.Controls.Add(this.pnlJugador1);
            this.Controls.Add(this.pnlJugador2);
            this.Controls.Add(this.lblCaraACara);
            this.Controls.Add(this.btnComenzarPartida);
            this.Controls.Add(this.lblAyudaComenzar);
            this.Controls.Add(this.lblSeccionHistorial);
            this.Controls.Add(this.pnlHistorial);
            this.Controls.Add(this.lnkVerEstadisticas);
            this.Controls.Add(this.pnlSeparador);
            this.Controls.Add(this.btnEstadisticas);
            this.Controls.Add(this.btnReglas);
            this.Controls.Add(this.btnBackup);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.btnCerrarSesion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Batalla Naval";
            this.Load += new System.EventHandler(this.FormPrincipal_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPrincipal_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.pnlJugador1.ResumeLayout(false);
            this.pnlJugador1.PerformLayout();
            this.pnlJugador2.ResumeLayout(false);
            this.pnlJugador2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // Tarjeta de un jugador: avatar redondo con la inicial, nombre, estado y estadísticas.
        private void ConfigurarTarjeta(System.Windows.Forms.Panel tarjeta, System.Windows.Forms.Label avatar,
            System.Windows.Forms.Label nombre, System.Windows.Forms.Label chip, System.Windows.Forms.Label stats, int y)
        {
            tarjeta.Location = new System.Drawing.Point(X_IZQ, y);
            tarjeta.Size = new System.Drawing.Size(ANCHO_IZQ, 64);
            tarjeta.BackColor = Branding.Panel;
            tarjeta.Paint += new System.Windows.Forms.PaintEventHandler(this.DibujarBorde);

            avatar.AutoSize = false;
            avatar.Location = new System.Drawing.Point(12, 12);
            avatar.Size = new System.Drawing.Size(40, 40);
            avatar.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            avatar.Font = Fuentes.Montserrat(14F, System.Drawing.FontStyle.Bold);
            avatar.ForeColor = System.Drawing.Color.White;
            avatar.BackColor = Branding.Panel;
            avatar.Paint += new System.Windows.Forms.PaintEventHandler(this.DibujarAvatar);

            nombre.AutoSize = true;
            nombre.Font = Fuentes.Montserrat(11F, System.Drawing.FontStyle.Bold);
            nombre.ForeColor = Branding.TextoPrincipal;
            nombre.Location = new System.Drawing.Point(60, 9);

            chip.AutoSize = false;
            chip.Font = Fuentes.Montserrat(8F, System.Drawing.FontStyle.Bold);
            chip.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            chip.Location = new System.Drawing.Point(200, 8);
            chip.Size = new System.Drawing.Size(128, 18);

            stats.AutoSize = true;
            stats.Font = Fuentes.Montserrat(8.5F);
            stats.ForeColor = Branding.Secundario;
            stats.Location = new System.Drawing.Point(61, 36);

            tarjeta.Controls.Add(avatar);
            tarjeta.Controls.Add(nombre);
            tarjeta.Controls.Add(chip);
            tarjeta.Controls.Add(stats);
        }

        private void ConfigurarBotonSecundario(System.Windows.Forms.Button boton, string texto, int x, int ancho, System.Drawing.Color color)
        {
            boton.Location = new System.Drawing.Point(x, 384);
            boton.Size = new System.Drawing.Size(ancho, 30);
            boton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            boton.FlatAppearance.BorderColor = color;
            boton.BackColor = Branding.Panel;
            boton.ForeColor = color;
            boton.Text = texto;
        }
    }
}
