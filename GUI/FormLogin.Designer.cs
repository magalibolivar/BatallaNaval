namespace GUI
{
    partial class FormLogin
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

        // Disposición (basada en stitch_batalla_naval_winforms_ui/inicio_de_sesi_n): logo y
        // título centrados, campos en recuadros redondeados con texto de ayuda adentro,
        // "Ingresar" y, debajo de un separador, "Jugar como invitado" y el registro.
        private const int X_CAMPOS = 40;
        private const int ANCHO_CAMPOS = 300;

        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Label lblTituloBatalla;
        private System.Windows.Forms.Label lblTituloNaval;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Panel pnlUsuario;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Panel pnlContraseña;
        private System.Windows.Forms.TextBox txtContraseña;
        private System.Windows.Forms.LinkLabel lnkMostrar;
        private System.Windows.Forms.CheckBox chkRecordar;
        private System.Windows.Forms.Label lblMayusculas;
        private System.Windows.Forms.Label lblMensaje;
        private System.Windows.Forms.Button btnIngresar;
        private System.Windows.Forms.Label lblSeparador;
        private System.Windows.Forms.Button btnInvitado;
        private System.Windows.Forms.Label lblAyudaInvitado;
        private System.Windows.Forms.LinkLabel lnkRegistrarme;
        private System.Windows.Forms.ToolTip toolTip;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.lblTituloBatalla = new System.Windows.Forms.Label();
            this.lblTituloNaval = new System.Windows.Forms.Label();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.pnlUsuario = new System.Windows.Forms.Panel();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.pnlContraseña = new System.Windows.Forms.Panel();
            this.txtContraseña = new System.Windows.Forms.TextBox();
            this.lnkMostrar = new System.Windows.Forms.LinkLabel();
            this.chkRecordar = new System.Windows.Forms.CheckBox();
            this.lblMayusculas = new System.Windows.Forms.Label();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.btnIngresar = new System.Windows.Forms.Button();
            this.lblSeparador = new System.Windows.Forms.Label();
            this.btnInvitado = new System.Windows.Forms.Button();
            this.lblAyudaInvitado = new System.Windows.Forms.Label();
            this.lnkRegistrarme = new System.Windows.Forms.LinkLabel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.pnlUsuario.SuspendLayout();
            this.pnlContraseña.SuspendLayout();
            this.SuspendLayout();

            this.BackColor = Branding.FondoVentana;
            this.Font = Fuentes.Montserrat(9F);

            // ---------- Logo y título ----------
            this.picLogo.Location = new System.Drawing.Point(140, 18);
            this.picLogo.Size = new System.Drawing.Size(100, 78);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.Image = RecursosNaves.LogoBatallaNaval();

            this.lblTituloBatalla.AutoSize = false;
            this.lblTituloBatalla.Font = Fuentes.Montserrat(15F, System.Drawing.FontStyle.Bold);
            this.lblTituloBatalla.ForeColor = Branding.TextoPrincipal;
            this.lblTituloBatalla.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTituloBatalla.Location = new System.Drawing.Point(0, 98);
            this.lblTituloBatalla.Size = new System.Drawing.Size(380, 28);
            this.lblTituloBatalla.Text = "BATALLA";

            this.lblTituloNaval.AutoSize = false;
            this.lblTituloNaval.Font = Fuentes.Montserrat(19F, System.Drawing.FontStyle.Bold);
            this.lblTituloNaval.ForeColor = Branding.Primario;
            this.lblTituloNaval.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTituloNaval.Location = new System.Drawing.Point(0, 124);
            this.lblTituloNaval.Size = new System.Drawing.Size(380, 36);
            this.lblTituloNaval.Text = "NAVAL";

            this.lblSubtitulo.AutoSize = false;
            this.lblSubtitulo.ForeColor = Branding.Secundario;
            this.lblSubtitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSubtitulo.Location = new System.Drawing.Point(20, 162);
            this.lblSubtitulo.Size = new System.Drawing.Size(340, 22);
            this.lblSubtitulo.Text = "Ingresá con tu usuario para jugar.";

            // ---------- Campos ----------
            ConfigurarCampo(this.pnlUsuario, this.txtUsuario, 194, ANCHO_CAMPOS - 28);
            this.txtUsuario.MaxLength = 20;
            this.txtUsuario.TextChanged += new System.EventHandler(this.Campo_TextChanged);

            ConfigurarCampo(this.pnlContraseña, this.txtContraseña, 244, ANCHO_CAMPOS - 92);
            this.txtContraseña.MaxLength = 20;
            this.txtContraseña.UseSystemPasswordChar = true;
            this.txtContraseña.TextChanged += new System.EventHandler(this.Campo_TextChanged);
            this.txtContraseña.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtContraseña_KeyUp);
            this.txtContraseña.Enter += new System.EventHandler(this.txtContraseña_Enter);

            this.lnkMostrar.AutoSize = false;
            this.lnkMostrar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lnkMostrar.Location = new System.Drawing.Point(ANCHO_CAMPOS - 72, 9);
            this.lnkMostrar.Size = new System.Drawing.Size(62, 22);
            this.lnkMostrar.LinkColor = Branding.Secundario;
            this.lnkMostrar.ActiveLinkColor = Branding.Primario;
            this.lnkMostrar.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkMostrar.Font = Fuentes.Montserrat(8F, System.Drawing.FontStyle.Bold);
            this.lnkMostrar.Text = "Mostrar";
            this.lnkMostrar.TabStop = false;
            this.lnkMostrar.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkMostrar_LinkClicked);
            this.pnlContraseña.Controls.Add(this.lnkMostrar);

            this.chkRecordar.AutoSize = true;
            this.chkRecordar.ForeColor = Branding.TextoPrincipal;
            this.chkRecordar.Font = Fuentes.Montserrat(8.5F);
            this.chkRecordar.Location = new System.Drawing.Point(X_CAMPOS, 292);
            this.chkRecordar.Text = "Recordar usuario";
            this.toolTip.SetToolTip(this.chkRecordar, "Guarda solo el nombre de usuario en esta PC (nunca la contraseña).");

            this.lblMayusculas.AutoSize = false;
            this.lblMayusculas.ForeColor = Branding.TableroHundido;
            this.lblMayusculas.Font = Fuentes.Montserrat(8F, System.Drawing.FontStyle.Bold);
            this.lblMayusculas.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblMayusculas.Location = new System.Drawing.Point(X_CAMPOS + 150, 292);
            this.lblMayusculas.Size = new System.Drawing.Size(150, 20);
            this.lblMayusculas.Text = "Bloq Mayús activado";
            this.lblMayusculas.Visible = false;

            this.lblMensaje.AutoSize = false;
            this.lblMensaje.ForeColor = Branding.TableroHundido;
            this.lblMensaje.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMensaje.Location = new System.Drawing.Point(X_CAMPOS, 316);
            this.lblMensaje.Size = new System.Drawing.Size(ANCHO_CAMPOS, 36);

            this.btnIngresar.Location = new System.Drawing.Point(X_CAMPOS, 354);
            this.btnIngresar.Size = new System.Drawing.Size(ANCHO_CAMPOS, 42);
            this.btnIngresar.Text = "Ingresar";
            this.btnIngresar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIngresar.FlatAppearance.BorderSize = 0;
            this.btnIngresar.BackColor = Branding.Primario;
            this.btnIngresar.ForeColor = System.Drawing.Color.White;
            this.btnIngresar.Font = Fuentes.Montserrat(11F, System.Drawing.FontStyle.Bold);
            this.btnIngresar.Click += new System.EventHandler(this.btnIngresar_Click);

            // ---------- Invitado y registro ----------
            this.lblSeparador.AutoSize = false;
            this.lblSeparador.Location = new System.Drawing.Point(X_CAMPOS, 404);
            this.lblSeparador.Size = new System.Drawing.Size(ANCHO_CAMPOS, 20);
            this.lblSeparador.Paint += new System.Windows.Forms.PaintEventHandler(this.lblSeparador_Paint);

            this.btnInvitado.Location = new System.Drawing.Point(X_CAMPOS, 430);
            this.btnInvitado.Size = new System.Drawing.Size(ANCHO_CAMPOS, 38);
            this.btnInvitado.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInvitado.FlatAppearance.BorderColor = Branding.Secundario;
            this.btnInvitado.BackColor = Branding.Panel;
            this.btnInvitado.ForeColor = Branding.Secundario;
            this.btnInvitado.Font = Fuentes.Montserrat(10F, System.Drawing.FontStyle.Bold);
            this.btnInvitado.Text = "Jugar como invitado";
            this.btnInvitado.Click += new System.EventHandler(this.btnInvitado_Click);

            this.lblAyudaInvitado.AutoSize = false;
            this.lblAyudaInvitado.ForeColor = Branding.Secundario;
            this.lblAyudaInvitado.Font = Fuentes.Montserrat(8F);
            this.lblAyudaInvitado.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblAyudaInvitado.Location = new System.Drawing.Point(X_CAMPOS - 10, 472);
            this.lblAyudaInvitado.Size = new System.Drawing.Size(ANCHO_CAMPOS + 20, 34);

            this.lnkRegistrarme.AutoSize = false;
            this.lnkRegistrarme.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lnkRegistrarme.Location = new System.Drawing.Point(X_CAMPOS, 512);
            this.lnkRegistrarme.Size = new System.Drawing.Size(ANCHO_CAMPOS, 22);
            this.lnkRegistrarme.ForeColor = Branding.TextoPrincipal;
            this.lnkRegistrarme.LinkColor = Branding.Primario;
            this.lnkRegistrarme.ActiveLinkColor = Branding.TextoPrincipal;
            this.lnkRegistrarme.Font = Fuentes.Montserrat(9F, System.Drawing.FontStyle.Bold);
            this.lnkRegistrarme.Text = "¿No tenés cuenta? Creá una";
            this.lnkRegistrarme.LinkArea = new System.Windows.Forms.LinkArea(18, 8);
            this.lnkRegistrarme.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkRegistrarme_LinkClicked);

            this.AcceptButton = this.btnIngresar;
            this.ClientSize = new System.Drawing.Size(380, 550);
            this.Controls.Add(this.picLogo);
            this.Controls.Add(this.lblTituloBatalla);
            this.Controls.Add(this.lblTituloNaval);
            this.Controls.Add(this.lblSubtitulo);
            this.Controls.Add(this.pnlUsuario);
            this.Controls.Add(this.pnlContraseña);
            this.Controls.Add(this.chkRecordar);
            this.Controls.Add(this.lblMayusculas);
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.btnIngresar);
            this.Controls.Add(this.lblSeparador);
            this.Controls.Add(this.btnInvitado);
            this.Controls.Add(this.lblAyudaInvitado);
            this.Controls.Add(this.lnkRegistrarme);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Batalla Naval - Ingresar";
            this.Load += new System.EventHandler(this.FormLogin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.pnlUsuario.ResumeLayout(false);
            this.pnlUsuario.PerformLayout();
            this.pnlContraseña.ResumeLayout(false);
            this.pnlContraseña.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // Recuadro blanco redondeado con un TextBox sin borde adentro; el borde se pinta
        // del color principal mientras el campo tiene el foco (ver Campo_Paint).
        private void ConfigurarCampo(System.Windows.Forms.Panel recuadro, System.Windows.Forms.TextBox texto, int y, int anchoTexto)
        {
            recuadro.Location = new System.Drawing.Point(X_CAMPOS, y);
            recuadro.Size = new System.Drawing.Size(ANCHO_CAMPOS, 40);
            recuadro.BackColor = Branding.Panel;
            recuadro.Cursor = System.Windows.Forms.Cursors.IBeam;
            recuadro.Paint += new System.Windows.Forms.PaintEventHandler(this.Campo_Paint);
            recuadro.Click += new System.EventHandler(this.Recuadro_Click);

            texto.BorderStyle = System.Windows.Forms.BorderStyle.None;
            texto.BackColor = Branding.Panel;
            texto.ForeColor = Branding.TextoPrincipal;
            texto.Font = Fuentes.Montserrat(10.5F);
            texto.Location = new System.Drawing.Point(14, 11);
            texto.Size = new System.Drawing.Size(anchoTexto, 20);
            texto.Enter += new System.EventHandler(this.Campo_FocoCambiado);
            texto.Leave += new System.EventHandler(this.Campo_FocoCambiado);
            recuadro.Controls.Add(texto);
        }
    }
}
