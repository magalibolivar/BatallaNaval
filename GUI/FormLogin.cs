using System;
using System.Windows.Forms;
using BE;

namespace GUI
{
    public partial class FormLogin : Form
    {
        public USUARIO UsuarioLogueado { get; private set; }

        // Usuario que no puede ingresar en este login (el Jugador 1, cuando se loguea el Jugador 2).
        // Se rechaza antes de llamar a BLL.USUARIO.Login, así no queda un inicio de sesión en la bitácora.
        private string usuarioNoPermitido = "";

        public FormLogin()
        {
            InitializeComponent();
        }

        public FormLogin(string usuarioNoPermitido) : this()
        {
            this.usuarioNoPermitido = usuarioNoPermitido;
            this.Text = "Batalla Naval - Ingresar Jugador 2";
            lblTituloBatalla.Text = "JUGADOR";
            lblTituloNaval.Text = "2";
            lblSubtitulo.Text = "Ingresá con un usuario distinto a " + usuarioNoPermitido + ".";
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            lblMensaje.ForeColor = System.Drawing.Color.Crimson;
            lblMensaje.Text = "";

            if (txtUsuario.Text == "" || txtContraseña.Text == "")
            {
                lblMensaje.Text = "Completá usuario y contraseña";
                return;
            }

            if (usuarioNoPermitido != "" && txtUsuario.Text.TrimEnd().ToLower() == usuarioNoPermitido.ToLower())
            {
                lblMensaje.Text = "Tiene que ser un usuario distinto al Jugador 1";
                return;
            }

            USUARIO usuario = new USUARIO();
            usuario.Nombre = txtUsuario.Text;
            usuario.Contraseña = txtContraseña.Text;

            BLL.USUARIO usuarioBLL = new BLL.USUARIO();
            bool ok;
            try
            {
                ok = usuarioBLL.Login(usuario);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo ingresar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (ok)
            {
                UsuarioLogueado = usuario;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                lblMensaje.Text = "Usuario o contraseña incorrectos";
            }
        }

        private void lnkRegistrarme_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FormRegistro formRegistro = new FormRegistro();
            if (formRegistro.ShowDialog() == DialogResult.OK)
            {
                txtUsuario.Text = formRegistro.NombreRegistrado;
                lblMensaje.ForeColor = System.Drawing.Color.SeaGreen;
                lblMensaje.Text = "Usuario creado. Ya podés ingresar.";
                txtContraseña.Focus();
            }
        }
    }
}
