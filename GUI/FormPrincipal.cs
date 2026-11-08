using System;
using System.Windows.Forms;
using BE;

namespace GUI
{
    public partial class FormPrincipal : Form
    {
        private USUARIO jugador1;
        private USUARIO jugador2;

        // true cuando el form se cierra por un restore: Program vuelve a mostrar el login.
        public bool VolverAlLogin { get; private set; }

        public FormPrincipal(USUARIO jugadorLogueado)
        {
            InitializeComponent();
            jugador1 = jugadorLogueado;
        }

        private void FormPrincipal_Load(object sender, EventArgs e)
        {
            lblJugador1.Text = "Jugador 1: " + jugador1.Nombre;
            ActualizarPantalla();
        }

        private void MostrarError(Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ActualizarPantalla()
        {
            if (jugador2 == null)
            {
                lblJugador2.Text = "Jugador 2: (sin ingresar)";
                btnLoginJugador2.Text = "Ingresar Jugador 2";
            }
            else
            {
                lblJugador2.Text = "Jugador 2: " + jugador2.Nombre + "  ✓";
                btnLoginJugador2.Text = "Cambiar Jugador 2";
            }
            btnComenzarPartida.Enabled = jugador2 != null;
        }

        // -------------------- Alta de jugador 2 / inicio de partida --------------------

        private void btnLoginJugador2_Click(object sender, EventArgs e)
        {
            FormLogin formLogin = new FormLogin(jugador1.Nombre);
            if (formLogin.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            // Si entra un Jugador 2 distinto al de la partida anterior, al anterior
            // se le registra el cierre de sesión en la bitácora.
            if (jugador2 != null && jugador2.ID != formLogin.UsuarioLogueado.ID)
            {
                try
                {
                    BLL.USUARIO usuarioBLL = new BLL.USUARIO();
                    usuarioBLL.Logout(jugador2);
                }
                catch (Exception ex)
                {
                    MostrarError(ex);
                }
            }

            jugador2 = formLogin.UsuarioLogueado;
            ActualizarPantalla();
            btnComenzarPartida.Focus();
        }

        private void btnComenzarPartida_Click(object sender, EventArgs e)
        {
            if (jugador2 == null)
            {
                MessageBox.Show("Primero tenés que ingresar el Jugador 2");
                return;
            }

            using (FormPartida formPartida = new FormPartida(jugador1, jugador2))
            {
                formPartida.ShowDialog(this);
            }
        }

        // -------------------- Menú superior --------------------

        // El botón "Salir" solo cierra el form: el logout lo hace FormPrincipal_FormClosing,
        // así también se registra si la ventana se cierra con la X.
        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Cierre por un restore: no se pide confirmación ni se registra el cierre de
            // sesión, porque los jugadores en memoria pueden no existir en la base restaurada.
            if (VolverAlLogin)
            {
                return;
            }

            // Mientras hay una partida en curso, FormPartida (modal) no deja cerrarse sin
            // que alguno de los 2 jugadores la abandone, así que acá nunca hay una activa.
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult confirmar = MessageBox.Show(
                    "¿Estás seguro de que querés salir?",
                    "Salir", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (confirmar != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }

            try
            {
                BLL.USUARIO usuarioBLL = new BLL.USUARIO();
                usuarioBLL.Logout(jugador1);
                if (jugador2 != null)
                {
                    usuarioBLL.Logout(jugador2);
                }
            }
            catch (Exception ex)
            {
                MostrarError(ex);
            }
        }

        // BACKUP/RESTORE los ejecuta el servicio de SQL Server, que no suele tener permiso
        // sobre carpetas personales (Escritorio, Documentos). Por eso el diálogo arranca
        // en la carpeta de backups de la instancia, donde siempre puede leer y escribir.
        private string CarpetaInicialBackups()
        {
            string carpeta = BLL.BACKUPRESTORE.CarpetaBackupPorDefecto();
            if (carpeta != "" && System.IO.Directory.Exists(carpeta))
            {
                return carpeta;
            }
            return "";
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            sfdBackup.FileName = "BATALLANAVAL_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".bak";
            sfdBackup.InitialDirectory = CarpetaInicialBackups();

            if (sfdBackup.ShowDialog() == DialogResult.OK)
            {
                string error = BLL.BACKUPRESTORE.HacerBackup(sfdBackup.FileName);
                if (error == "")
                {
                    MessageBox.Show("Backup realizado correctamente en:\n" + sfdBackup.FileName);
                }
                else
                {
                    MessageBox.Show(
                        "Ocurrió un error al hacer el backup:\n" + error +
                        "\n\nSi es un problema de permisos, probá guardarlo en la carpeta de backups de SQL Server.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            ofdRestore.InitialDirectory = CarpetaInicialBackups();

            if (ofdRestore.ShowDialog() == DialogResult.OK)
            {
                DialogResult confirmar = MessageBox.Show(
                    "Esto va a reemplazar los datos actuales de la base por los del backup. ¿Confirmás?",
                    "Restaurar backup", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirmar == DialogResult.Yes)
                {
                    bool restaurada;
                    string error = BLL.BACKUPRESTORE.HacerRestore(ofdRestore.FileName, out restaurada);
                    if (restaurada)
                    {
                        if (error == "")
                        {
                            MessageBox.Show(
                                "Restore realizado correctamente.\n\nComo los datos de la base cambiaron, " +
                                "hay que volver a iniciar sesión.",
                                "Restaurar backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show(
                                "Los datos del backup se restauraron, pero después ocurrió un error:\n" + error +
                                "\n\nHay que volver a iniciar sesión. Si la aplicación no puede conectarse, " +
                                "ejecutá en SSMS: ALTER DATABASE BATALLANAVAL SET MULTI_USER",
                                "Restaurar backup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        VolverAlLogin = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(
                            "Ocurrió un error al restaurar el backup:\n" + error,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnEstadisticas_Click(object sender, EventArgs e)
        {
            FormEstadisticas formEstadisticas = new FormEstadisticas(jugador1, jugador2);
            formEstadisticas.ShowDialog();
        }

        private void btnReglas_Click(object sender, EventArgs e)
        {
            FormReglas formReglas = new FormReglas();
            formReglas.ShowDialog();
        }
    }
}
