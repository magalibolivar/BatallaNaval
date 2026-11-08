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

        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Label lblTituloBatalla;
        private System.Windows.Forms.Label lblTituloNaval;
        private System.Windows.Forms.Label lblJugador1;
        private System.Windows.Forms.Label lblJugador2;
        private System.Windows.Forms.Button btnLoginJugador2;
        private System.Windows.Forms.Button btnComenzarPartida;
        private System.Windows.Forms.Button btnEstadisticas;
        private System.Windows.Forms.Button btnBackup;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Button btnReglas;
        private System.Windows.Forms.Button btnCerrarSesion;
        private System.Windows.Forms.SaveFileDialog sfdBackup;
        private System.Windows.Forms.OpenFileDialog ofdRestore;

        private void InitializeComponent()
        {
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.lblTituloBatalla = new System.Windows.Forms.Label();
            this.lblTituloNaval = new System.Windows.Forms.Label();
            this.lblJugador1 = new System.Windows.Forms.Label();
            this.lblJugador2 = new System.Windows.Forms.Label();
            this.btnLoginJugador2 = new System.Windows.Forms.Button();
            this.btnComenzarPartida = new System.Windows.Forms.Button();
            this.btnEstadisticas = new System.Windows.Forms.Button();
            this.btnBackup = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.btnReglas = new System.Windows.Forms.Button();
            this.btnCerrarSesion = new System.Windows.Forms.Button();
            this.sfdBackup = new System.Windows.Forms.SaveFileDialog();
            this.ofdRestore = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();

            this.BackColor = Branding.FondoVentana;

            this.picLogo.Location = new System.Drawing.Point(25, 15);
            this.picLogo.Size = new System.Drawing.Size(75, 65);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.Image = RecursosNaves.LogoBatallaNaval();

            this.lblTituloBatalla.AutoSize = true;
            this.lblTituloBatalla.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblTituloBatalla.ForeColor = Branding.TextoPrincipal;
            this.lblTituloBatalla.Location = new System.Drawing.Point(108, 17);
            this.lblTituloBatalla.Text = "BATALLA";

            this.lblTituloNaval.AutoSize = true;
            this.lblTituloNaval.Font = new System.Drawing.Font("Segoe UI", 19F, System.Drawing.FontStyle.Bold);
            this.lblTituloNaval.ForeColor = Branding.Primario;
            this.lblTituloNaval.Location = new System.Drawing.Point(105, 43);
            this.lblTituloNaval.Text = "NAVAL";

            this.lblJugador1.AutoSize = true;
            this.lblJugador1.ForeColor = Branding.TextoPrincipal;
            this.lblJugador1.Location = new System.Drawing.Point(30, 90);
            this.lblJugador1.Text = "Jugador 1:";

            this.lblJugador2.AutoSize = true;
            this.lblJugador2.ForeColor = Branding.TextoPrincipal;
            this.lblJugador2.Location = new System.Drawing.Point(30, 115);
            this.lblJugador2.Text = "Jugador 2: (sin ingresar)";

            this.btnLoginJugador2.Location = new System.Drawing.Point(30, 150);
            this.btnLoginJugador2.Size = new System.Drawing.Size(280, 32);
            this.btnLoginJugador2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoginJugador2.FlatAppearance.BorderColor = Branding.Secundario;
            this.btnLoginJugador2.BackColor = Branding.Panel;
            this.btnLoginJugador2.ForeColor = Branding.Secundario;
            this.btnLoginJugador2.Text = "Ingresar Jugador 2";
            this.btnLoginJugador2.Click += new System.EventHandler(this.btnLoginJugador2_Click);

            this.btnComenzarPartida.Location = new System.Drawing.Point(30, 190);
            this.btnComenzarPartida.Size = new System.Drawing.Size(280, 38);
            this.btnComenzarPartida.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnComenzarPartida.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnComenzarPartida.FlatAppearance.BorderSize = 0;
            this.btnComenzarPartida.BackColor = Branding.Primario;
            this.btnComenzarPartida.ForeColor = System.Drawing.Color.White;
            this.btnComenzarPartida.Text = "Comenzar Partida";
            this.btnComenzarPartida.Click += new System.EventHandler(this.btnComenzarPartida_Click);

            this.btnEstadisticas.Location = new System.Drawing.Point(30, 245);
            this.btnEstadisticas.Size = new System.Drawing.Size(280, 32);
            this.btnEstadisticas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEstadisticas.FlatAppearance.BorderColor = Branding.Secundario;
            this.btnEstadisticas.BackColor = Branding.Panel;
            this.btnEstadisticas.ForeColor = Branding.Secundario;
            this.btnEstadisticas.Text = "Estadísticas";
            this.btnEstadisticas.Click += new System.EventHandler(this.btnEstadisticas_Click);

            this.btnReglas.Location = new System.Drawing.Point(30, 280);
            this.btnReglas.Size = new System.Drawing.Size(280, 32);
            this.btnReglas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReglas.FlatAppearance.BorderColor = Branding.Secundario;
            this.btnReglas.BackColor = Branding.Panel;
            this.btnReglas.ForeColor = Branding.Secundario;
            this.btnReglas.Text = "Reglas";
            this.btnReglas.Click += new System.EventHandler(this.btnReglas_Click);

            this.btnBackup.Location = new System.Drawing.Point(30, 330);
            this.btnBackup.Size = new System.Drawing.Size(135, 32);
            this.btnBackup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackup.FlatAppearance.BorderColor = Branding.Secundario;
            this.btnBackup.BackColor = Branding.Panel;
            this.btnBackup.ForeColor = Branding.Secundario;
            this.btnBackup.Text = "Backup";
            this.btnBackup.Click += new System.EventHandler(this.btnBackup_Click);

            this.btnRestore.Location = new System.Drawing.Point(175, 330);
            this.btnRestore.Size = new System.Drawing.Size(135, 32);
            this.btnRestore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRestore.FlatAppearance.BorderColor = Branding.Secundario;
            this.btnRestore.BackColor = Branding.Panel;
            this.btnRestore.ForeColor = Branding.Secundario;
            this.btnRestore.Text = "Restore";
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);

            this.btnCerrarSesion.Location = new System.Drawing.Point(30, 375);
            this.btnCerrarSesion.Size = new System.Drawing.Size(280, 32);
            this.btnCerrarSesion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrarSesion.FlatAppearance.BorderColor = Branding.TextoPrincipal;
            this.btnCerrarSesion.BackColor = Branding.Panel;
            this.btnCerrarSesion.ForeColor = Branding.TextoPrincipal;
            this.btnCerrarSesion.Text = "Salir";
            this.btnCerrarSesion.Click += new System.EventHandler(this.btnCerrarSesion_Click);

            this.sfdBackup.Filter = "Backup de SQL Server (*.bak)|*.bak";
            this.ofdRestore.Filter = "Backup de SQL Server (*.bak)|*.bak";

            this.ClientSize = new System.Drawing.Size(340, 430);
            this.Controls.Add(this.picLogo);
            this.Controls.Add(this.lblTituloBatalla);
            this.Controls.Add(this.lblTituloNaval);
            this.Controls.Add(this.lblJugador1);
            this.Controls.Add(this.lblJugador2);
            this.Controls.Add(this.btnLoginJugador2);
            this.Controls.Add(this.btnComenzarPartida);
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
            this.ResumeLayout(false);
        }
    }
}
