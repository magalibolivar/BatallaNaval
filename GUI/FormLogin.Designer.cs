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

        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Label lblTituloBatalla;
        private System.Windows.Forms.Label lblTituloNaval;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Label lblContraseña;
        private System.Windows.Forms.TextBox txtContraseña;
        private System.Windows.Forms.Label lblMensaje;
        private System.Windows.Forms.Button btnIngresar;
        private System.Windows.Forms.LinkLabel lnkRegistrarme;

        private void InitializeComponent()
        {
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.lblTituloBatalla = new System.Windows.Forms.Label();
            this.lblTituloNaval = new System.Windows.Forms.Label();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.lblContraseña = new System.Windows.Forms.Label();
            this.txtContraseña = new System.Windows.Forms.TextBox();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.btnIngresar = new System.Windows.Forms.Button();
            this.lnkRegistrarme = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();

            this.BackColor = Branding.FondoVentana;

            this.picLogo.Location = new System.Drawing.Point(25, 20);
            this.picLogo.Size = new System.Drawing.Size(75, 65);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.Image = RecursosNaves.LogoBatallaNaval();

            this.lblTituloBatalla.AutoSize = true;
            this.lblTituloBatalla.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblTituloBatalla.ForeColor = Branding.TextoPrincipal;
            this.lblTituloBatalla.Location = new System.Drawing.Point(108, 22);
            this.lblTituloBatalla.Text = "BATALLA";

            this.lblTituloNaval.AutoSize = true;
            this.lblTituloNaval.Font = new System.Drawing.Font("Segoe UI", 19F, System.Drawing.FontStyle.Bold);
            this.lblTituloNaval.ForeColor = Branding.Primario;
            this.lblTituloNaval.Location = new System.Drawing.Point(105, 48);
            this.lblTituloNaval.Text = "NAVAL";

            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.ForeColor = Branding.TextoPrincipal;
            this.lblSubtitulo.Location = new System.Drawing.Point(30, 100);
            this.lblSubtitulo.Size = new System.Drawing.Size(280, 20);
            this.lblSubtitulo.Text = "Ingresá con tu usuario para jugar.";

            this.lblUsuario.AutoSize = true;
            this.lblUsuario.ForeColor = Branding.TextoPrincipal;
            this.lblUsuario.Location = new System.Drawing.Point(30, 130);
            this.lblUsuario.Text = "Usuario:";

            this.txtUsuario.Location = new System.Drawing.Point(30, 150);
            this.txtUsuario.MaxLength = 20;
            this.txtUsuario.Size = new System.Drawing.Size(270, 25);

            this.lblContraseña.AutoSize = true;
            this.lblContraseña.ForeColor = Branding.TextoPrincipal;
            this.lblContraseña.Location = new System.Drawing.Point(30, 185);
            this.lblContraseña.Text = "Contraseña:";

            this.txtContraseña.Location = new System.Drawing.Point(30, 205);
            this.txtContraseña.MaxLength = 20;
            this.txtContraseña.PasswordChar = '*';
            this.txtContraseña.Size = new System.Drawing.Size(270, 25);

            this.lblMensaje.AutoSize = false;
            this.lblMensaje.Location = new System.Drawing.Point(30, 240);
            this.lblMensaje.Size = new System.Drawing.Size(270, 40);

            this.btnIngresar.Location = new System.Drawing.Point(30, 285);
            this.btnIngresar.Size = new System.Drawing.Size(270, 34);
            this.btnIngresar.Text = "Ingresar";
            this.btnIngresar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIngresar.FlatAppearance.BorderSize = 0;
            this.btnIngresar.BackColor = Branding.Primario;
            this.btnIngresar.ForeColor = System.Drawing.Color.White;
            this.btnIngresar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnIngresar.Click += new System.EventHandler(this.btnIngresar_Click);

            this.lnkRegistrarme.AutoSize = true;
            this.lnkRegistrarme.LinkColor = Branding.Secundario;
            this.lnkRegistrarme.Location = new System.Drawing.Point(30, 330);
            this.lnkRegistrarme.Text = "Registrarme";
            this.lnkRegistrarme.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkRegistrarme_LinkClicked);

            this.AcceptButton = this.btnIngresar;
            this.ClientSize = new System.Drawing.Size(340, 375);
            this.Controls.Add(this.picLogo);
            this.Controls.Add(this.lblTituloBatalla);
            this.Controls.Add(this.lblTituloNaval);
            this.Controls.Add(this.lblSubtitulo);
            this.Controls.Add(this.lblUsuario);
            this.Controls.Add(this.txtUsuario);
            this.Controls.Add(this.lblContraseña);
            this.Controls.Add(this.txtContraseña);
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.btnIngresar);
            this.Controls.Add(this.lnkRegistrarme);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Batalla Naval - Ingresar";
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
