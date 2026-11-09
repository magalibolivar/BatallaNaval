namespace GUI
{
    partial class FormReglas
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

        private System.Windows.Forms.RichTextBox txtReglas;
        private System.Windows.Forms.Button btnCerrar;

        private void InitializeComponent()
        {
            this.txtReglas = new System.Windows.Forms.RichTextBox();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.SuspendLayout();

            this.BackColor = Branding.FondoVentana;

            this.txtReglas.Location = new System.Drawing.Point(20, 20);
            this.txtReglas.Size = new System.Drawing.Size(520, 400);
            this.txtReglas.ReadOnly = true;
            this.txtReglas.BackColor = Branding.Panel;
            this.txtReglas.ForeColor = Branding.TextoPrincipal;
            this.txtReglas.Font = Fuentes.Montserrat(10F);

            this.btnCerrar.Location = new System.Drawing.Point(430, 430);
            this.btnCerrar.Size = new System.Drawing.Size(110, 32);
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrar.FlatAppearance.BorderSize = 0;
            this.btnCerrar.BackColor = Branding.Primario;
            this.btnCerrar.ForeColor = System.Drawing.Color.White;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);

            this.ClientSize = new System.Drawing.Size(560, 480);
            this.Controls.Add(this.txtReglas);
            this.Controls.Add(this.btnCerrar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Batalla Naval - Reglas";
            this.Icon = RecursosNaves.Icono();
            this.ResumeLayout(false);
        }
    }
}
