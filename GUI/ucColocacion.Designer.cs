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

        private System.Windows.Forms.Panel pnlTablero;
        private System.Windows.Forms.Label lblInstrucciones;
        private System.Windows.Forms.ListView lstFlota;
        private System.Windows.Forms.ImageList imgFlota;
        private System.Windows.Forms.Label lblRestantes;
        private System.Windows.Forms.Button btnConfirmar;
        private System.Windows.Forms.Label lblAyuda;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label lblMensaje;

        public static readonly System.Drawing.Point OrigenTablero = new System.Drawing.Point(24, 46);

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlTablero = new System.Windows.Forms.Panel();
            this.lblInstrucciones = new System.Windows.Forms.Label();
            this.lstFlota = new System.Windows.Forms.ListView();
            this.imgFlota = new System.Windows.Forms.ImageList(this.components);
            this.lblRestantes = new System.Windows.Forms.Label();
            this.btnConfirmar = new System.Windows.Forms.Button();
            this.lblAyuda = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.lblMensaje = new System.Windows.Forms.Label();
            this.SuspendLayout();

            this.BackColor = Branding.FondoVentana;

            this.lblInstrucciones.AutoSize = true;
            this.lblInstrucciones.ForeColor = Branding.TextoPrincipal;
            this.lblInstrucciones.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblInstrucciones.Location = new System.Drawing.Point(0, 0);
            this.lblInstrucciones.Text = "Elegí una nave de la lista y hacé click en tu tablero para colocarla.";

            this.pnlTablero.Location = OrigenTablero;
            this.pnlTablero.Size = new System.Drawing.Size(361, 361);
            this.pnlTablero.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlTablero.BackColor = Branding.Panel;

            this.lstFlota.Location = new System.Drawing.Point(405, 46);
            this.lstFlota.Size = new System.Drawing.Size(195, 160);
            this.lstFlota.View = System.Windows.Forms.View.List;
            this.lstFlota.SmallImageList = this.imgFlota;
            this.lstFlota.MultiSelect = false;
            this.lstFlota.HideSelection = false;
            this.lstFlota.SelectedIndexChanged += new System.EventHandler(this.lstFlota_SelectedIndexChanged);

            this.lblRestantes.AutoSize = true;
            this.lblRestantes.ForeColor = Branding.TextoPrincipal;
            this.lblRestantes.Location = new System.Drawing.Point(405, 210);

            // Reemplaza al antiguo botón de orientación: ahora las naves se giran haciendo
            // click sobre ellas, y la colocación se cierra recién al confirmar (así se puede
            // corregir también la última nave).
            this.btnConfirmar.Location = new System.Drawing.Point(405, 234);
            this.btnConfirmar.Size = new System.Drawing.Size(195, 32);
            this.btnConfirmar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirmar.FlatAppearance.BorderSize = 0;
            this.btnConfirmar.BackColor = Branding.Primario;
            this.btnConfirmar.ForeColor = System.Drawing.Color.White;
            this.btnConfirmar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnConfirmar.Text = "Confirmar flota";
            this.btnConfirmar.Enabled = false;
            this.btnConfirmar.Click += new System.EventHandler(this.btnConfirmar_Click);

            this.lblAyuda.AutoSize = false;
            this.lblAyuda.ForeColor = Branding.Secundario;
            this.lblAyuda.Location = new System.Drawing.Point(405, 274);
            this.lblAyuda.Size = new System.Drawing.Size(195, 64);
            this.lblAyuda.Text = "Mantené apretado el click sobre una nave y arrastrala para moverla. " +
                "Hacé click sobre ella para girarla.";

            this.lblMensaje.AutoSize = false;
            this.lblMensaje.ForeColor = Branding.TextoPrincipal;
            this.lblMensaje.Location = new System.Drawing.Point(405, 342);
            this.lblMensaje.Size = new System.Drawing.Size(195, 64);

            this.Controls.Add(this.pnlTablero);
            this.Controls.Add(this.lblInstrucciones);
            this.Controls.Add(this.lstFlota);
            this.Controls.Add(this.lblRestantes);
            this.Controls.Add(this.btnConfirmar);
            this.Controls.Add(this.lblAyuda);
            this.Controls.Add(this.lblMensaje);
            this.Size = new System.Drawing.Size(620, 470);
            this.ResumeLayout(false);
        }
    }
}
