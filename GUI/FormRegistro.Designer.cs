namespace GUI
{
    partial class FormRegistro
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
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label lblContraseña;
        private System.Windows.Forms.TextBox txtContraseña;
        private System.Windows.Forms.Label lblConfirmar;
        private System.Windows.Forms.TextBox txtConfirmar;
        private System.Windows.Forms.Label lblMensaje;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblContraseña = new System.Windows.Forms.Label();
            this.txtContraseña = new System.Windows.Forms.TextBox();
            this.lblConfirmar = new System.Windows.Forms.Label();
            this.txtConfirmar = new System.Windows.Forms.TextBox();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();

            this.BackColor = Branding.FondoVentana;

            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Montserrat", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = Branding.Primario;
            this.lblTitulo.Location = new System.Drawing.Point(30, 20);
            this.lblTitulo.Text = "Registrarme";

            this.lblNombre.AutoSize = true;
            this.lblNombre.ForeColor = Branding.TextoPrincipal;
            this.lblNombre.Location = new System.Drawing.Point(30, 65);
            this.lblNombre.Text = "Usuario (3 a 20 caracteres, sin espacios):";

            this.txtNombre.Location = new System.Drawing.Point(30, 85);
            this.txtNombre.MaxLength = 20;
            this.txtNombre.Size = new System.Drawing.Size(280, 25);

            this.lblContraseña.AutoSize = true;
            this.lblContraseña.ForeColor = Branding.TextoPrincipal;
            this.lblContraseña.Location = new System.Drawing.Point(30, 120);
            this.lblContraseña.Text = "Contraseña (4 a 20, con al menos un número):";

            this.txtContraseña.Location = new System.Drawing.Point(30, 140);
            this.txtContraseña.MaxLength = 20;
            this.txtContraseña.PasswordChar = '*';
            this.txtContraseña.Size = new System.Drawing.Size(280, 25);

            this.lblConfirmar.AutoSize = true;
            this.lblConfirmar.ForeColor = Branding.TextoPrincipal;
            this.lblConfirmar.Location = new System.Drawing.Point(30, 175);
            this.lblConfirmar.Text = "Confirmar contraseña:";

            this.txtConfirmar.Location = new System.Drawing.Point(30, 195);
            this.txtConfirmar.MaxLength = 20;
            this.txtConfirmar.PasswordChar = '*';
            this.txtConfirmar.Size = new System.Drawing.Size(280, 25);

            this.lblMensaje.AutoSize = false;
            this.lblMensaje.ForeColor = Branding.TableroHundido;
            this.lblMensaje.Location = new System.Drawing.Point(30, 230);
            this.lblMensaje.Size = new System.Drawing.Size(280, 40);

            this.btnGuardar.Location = new System.Drawing.Point(30, 280);
            this.btnGuardar.Size = new System.Drawing.Size(130, 32);
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.FlatAppearance.BorderSize = 0;
            this.btnGuardar.BackColor = Branding.Primario;
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);

            this.btnCancelar.Location = new System.Drawing.Point(180, 280);
            this.btnCancelar.Size = new System.Drawing.Size(130, 32);
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.FlatAppearance.BorderColor = Branding.Secundario;
            this.btnCancelar.BackColor = Branding.Panel;
            this.btnCancelar.ForeColor = Branding.Secundario;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);

            this.AcceptButton = this.btnGuardar;
            this.ClientSize = new System.Drawing.Size(340, 335);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.lblContraseña);
            this.Controls.Add(this.txtContraseña);
            this.Controls.Add(this.lblConfirmar);
            this.Controls.Add(this.txtConfirmar);
            this.Controls.Add(this.lblMensaje);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnCancelar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Batalla Naval - Registrarme";
            this.ResumeLayout(false);
        }
    }
}
