using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
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

        // Login del Jugador 2 (abierto desde el menú principal): cambia los textos y qué
        // invitado se usa con "Jugar como invitado".
        private bool esJugador2 = false;

        // Último usuario que ingresó con "Recordar usuario" tildado. Solo el nombre: la
        // contraseña nunca se guarda. Va en la misma carpeta que el servidor recordado por la DAL.
        private static readonly string archivoUsuarioRecordado = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BatallaNaval", "usuario.txt");

        public FormLogin()
        {
            InitializeComponent();
        }

        public FormLogin(string usuarioNoPermitido) : this()
        {
            this.usuarioNoPermitido = usuarioNoPermitido;
            this.esJugador2 = true;
            this.Text = "Batalla Naval - Ingresar Jugador 2";
            lblTituloBatalla.Text = "JUGADOR";
            lblTituloNaval.Text = "2";
            lblSubtitulo.Text = "Ingresá con un usuario distinto a " + usuarioNoPermitido + ".";
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            PonerTextoDeAyuda(txtUsuario, "Usuario");
            PonerTextoDeAyuda(txtContraseña, "Contraseña");

            string invitado = esJugador2 ? BLL.USUARIO.INVITADO_2 : BLL.USUARIO.INVITADO_1;
            lblAyudaInvitado.Text = "Sin usuario ni contraseña. Las partidas quedan registradas a nombre de " + invitado + ".";

            // Recordar el usuario tiene sentido para quien abre la app (Jugador 1); el Jugador 2
            // suele cambiar de una partida a otra.
            chkRecordar.Visible = !esJugador2;
            if (!esJugador2)
            {
                string recordado = LeerUsuarioRecordado();
                if (recordado != "")
                {
                    txtUsuario.Text = recordado;
                    chkRecordar.Checked = true;
                    this.ActiveControl = txtContraseña;
                    return;
                }
            }
            this.ActiveControl = txtUsuario;
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            string nombre = txtUsuario.Text.Trim();

            if (nombre == "")
            {
                MostrarError("Completá el usuario", txtUsuario);
                return;
            }
            if (txtContraseña.Text == "")
            {
                MostrarError("Completá la contraseña", txtContraseña);
                return;
            }
            if (BLL.USUARIO.EsInvitado(nombre))
            {
                MostrarError("Para jugar sin cuenta usá \"Jugar como invitado\"", txtUsuario);
                return;
            }
            if (usuarioNoPermitido != "" && nombre.ToLower() == usuarioNoPermitido.ToLower())
            {
                MostrarError("Tiene que ser un usuario distinto al Jugador 1", txtUsuario);
                return;
            }

            USUARIO usuario = new USUARIO();
            usuario.Nombre = nombre;
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

            if (!ok)
            {
                // No se dice cuál de los 2 está mal, para no confirmar qué usuarios existen.
                MostrarError("Usuario o contraseña incorrectos", txtContraseña);
                txtContraseña.SelectAll();
                return;
            }

            if (!esJugador2)
            {
                GuardarUsuarioRecordado(chkRecordar.Checked ? usuario.Nombre : "");
            }
            Ingresar(usuario);
        }

        // Jugador 1 anónimo = Invitado1, Jugador 2 anónimo = Invitado2: así pueden jugar
        // 2 invitados entre sí (ver Reglas_de_Negocio_BatallaNaval.md, "Jugar como invitado").
        private void btnInvitado_Click(object sender, EventArgs e)
        {
            BLL.USUARIO usuarioBLL = new BLL.USUARIO();
            USUARIO invitado;
            try
            {
                invitado = usuarioBLL.IngresarComoInvitado(esJugador2 ? 2 : 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo ingresar como invitado: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Ingresar(invitado);
        }

        private void Ingresar(USUARIO usuario)
        {
            UsuarioLogueado = usuario;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void lnkRegistrarme_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (FormRegistro formRegistro = new FormRegistro())
            {
                if (formRegistro.ShowDialog(this) == DialogResult.OK)
                {
                    txtUsuario.Text = formRegistro.NombreRegistrado;
                    txtContraseña.Text = "";
                    lblMensaje.ForeColor = Branding.Exito;
                    lblMensaje.Text = "Usuario creado. Ya podés ingresar.";
                    txtContraseña.Focus();
                }
            }
        }

        // -------------------- Detalles de los campos --------------------

        private void lnkMostrar_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            txtContraseña.UseSystemPasswordChar = !txtContraseña.UseSystemPasswordChar;
            lnkMostrar.Text = txtContraseña.UseSystemPasswordChar ? "Mostrar" : "Ocultar";
            txtContraseña.Focus();
            txtContraseña.SelectionStart = txtContraseña.Text.Length;
        }

        private void txtContraseña_Enter(object sender, EventArgs e)
        {
            ActualizarAvisoMayusculas();
        }

        private void txtContraseña_KeyUp(object sender, KeyEventArgs e)
        {
            ActualizarAvisoMayusculas();
        }

        // La contraseña distingue mayúsculas: avisar evita el error más común al ingresar.
        private void ActualizarAvisoMayusculas()
        {
            lblMayusculas.Visible = Control.IsKeyLocked(Keys.CapsLock);
        }

        // Al escribir se borra el mensaje de error anterior.
        private void Campo_TextChanged(object sender, EventArgs e)
        {
            if (lblMensaje.ForeColor == Branding.TableroHundido)
            {
                lblMensaje.Text = "";
            }
        }

        private void Recuadro_Click(object sender, EventArgs e)
        {
            ((Panel)sender).Controls[0].Focus();
        }

        private void Campo_FocoCambiado(object sender, EventArgs e)
        {
            ((Control)sender).Parent.Invalidate();
            if (sender == txtContraseña && !txtContraseña.Focused)
            {
                lblMayusculas.Visible = false;
            }
        }

        private void MostrarError(string mensaje, TextBox campo)
        {
            lblMensaje.ForeColor = Branding.TableroHundido;
            lblMensaje.Text = mensaje;
            campo.Focus();
        }

        // -------------------- Dibujo --------------------

        private void Campo_Paint(object sender, PaintEventArgs e)
        {
            Panel recuadro = (Panel)sender;
            bool conFoco = recuadro.Controls[0].Focused;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(1, 1, recuadro.Width - 3, recuadro.Height - 3);
            using (GraphicsPath camino = Redondeado(rect, 8))
            using (Pen lapiz = new Pen(conFoco ? Branding.Primario : Branding.TableroVacio, conFoco ? 2F : 1.5F))
            {
                e.Graphics.DrawPath(lapiz, camino);
            }
        }

        // Línea — o — entre "Ingresar" y "Jugar como invitado".
        private void lblSeparador_Paint(object sender, PaintEventArgs e)
        {
            Label separador = (Label)sender;
            string texto = "o";
            Size tamaño = TextRenderer.MeasureText(texto, separador.Font);
            int medio = separador.Height / 2;
            int xTexto = (separador.Width - tamaño.Width) / 2;
            using (Pen lapiz = new Pen(Branding.TableroVacio, 1.5F))
            {
                e.Graphics.DrawLine(lapiz, 0, medio, xTexto - 8, medio);
                e.Graphics.DrawLine(lapiz, xTexto + tamaño.Width + 8, medio, separador.Width, medio);
            }
            TextRenderer.DrawText(e.Graphics, texto, separador.Font, new Point(xTexto, medio - tamaño.Height / 2), Branding.Secundario);
        }

        private static GraphicsPath Redondeado(Rectangle rect, int radio)
        {
            GraphicsPath camino = new GraphicsPath();
            int d = radio * 2;
            camino.AddArc(rect.X, rect.Y, d, d, 180, 90);
            camino.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            camino.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            camino.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            camino.CloseFigure();
            return camino;
        }

        // -------------------- Texto de ayuda y usuario recordado --------------------

        // Texto gris dentro del campo vacío ("Usuario", "Contraseña"). .NET Framework no tiene
        // PlaceholderText, así que se usa el mensaje EM_SETCUEBANNER del control de Windows.
        private const int EM_SETCUEBANNER = 0x1501;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

        private static void PonerTextoDeAyuda(TextBox campo, string texto)
        {
            // wParam = 1: el texto se sigue viendo con el campo enfocado, hasta que se escribe.
            SendMessage(campo.Handle, EM_SETCUEBANNER, (IntPtr)1, texto);
        }

        // Si no se puede leer o escribir el archivo, se sigue sin recordar: no es un error para el usuario.
        private static string LeerUsuarioRecordado()
        {
            try
            {
                return File.Exists(archivoUsuarioRecordado) ? File.ReadAllText(archivoUsuarioRecordado).Trim() : "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static void GuardarUsuarioRecordado(string nombre)
        {
            try
            {
                if (nombre == "")
                {
                    if (File.Exists(archivoUsuarioRecordado))
                    {
                        File.Delete(archivoUsuarioRecordado);
                    }
                    return;
                }
                Directory.CreateDirectory(Path.GetDirectoryName(archivoUsuarioRecordado));
                File.WriteAllText(archivoUsuarioRecordado, nombre);
            }
            catch (Exception)
            {
            }
        }
    }
}
