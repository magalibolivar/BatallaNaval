using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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

        private const int CANTIDAD_HISTORIAL = 5;
        private const int ALTO_FILA_HISTORIAL = 42;

        private void FormPrincipal_Load(object sender, EventArgs e)
        {
            lblJugador1.Text = jugador1.Nombre;
            lblAvatar1.Text = Inicial(jugador1.Nombre);
            lblAvatar1.Tag = Branding.Primario;
            MostrarChip(lblChip1, jugador1, "✓ Jugador 1");
            ActualizarPantalla();
        }

        private void MostrarError(Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Se llama al abrir el menú, al cambiar el Jugador 2 y al terminar cada partida,
        // así las estadísticas y el historial siempre muestran lo último.
        private void ActualizarPantalla()
        {
            if (jugador2 == null)
            {
                lblJugador2.Text = "Jugador 2";
                lblAvatar2.Text = "?";
                lblAvatar2.Tag = Branding.TableroVacio;
                lblAvatar2.ForeColor = Branding.Secundario;
                lblChip2.Text = "pendiente";
                lblChip2.ForeColor = Branding.Pendiente;
                lblStats2.Text = "Todavía no ingresó";
                btnLoginJugador2.Text = "Ingresar";
            }
            else
            {
                lblJugador2.Text = jugador2.Nombre;
                lblAvatar2.Text = Inicial(jugador2.Nombre);
                lblAvatar2.Tag = Branding.Secundario;
                lblAvatar2.ForeColor = Color.White;
                MostrarChip(lblChip2, jugador2, "✓ Jugador 2");
                btnLoginJugador2.Text = "Cambiar";
            }
            lblAvatar2.Invalidate();

            // Con FlatStyle, un botón deshabilitado sigue viéndose rosa: se le cambian los
            // colores para que se note que todavía no se puede usar, y se explica por qué.
            bool puedeComenzar = jugador2 != null;
            btnComenzarPartida.Enabled = puedeComenzar;
            btnComenzarPartida.BackColor = puedeComenzar ? Branding.Primario : Branding.TableroVacio;
            lblAyudaComenzar.Text = puedeComenzar
                ? "Primero coloca sus naves " + jugador1.Nombre + ", después " + jugador2.Nombre + "."
                : "Ingresá al Jugador 2 para poder comenzar.";

            CargarEstadisticas();
        }

        // Estadísticas de las tarjetas, cara a cara e historial. Si la base no responde, el
        // menú sigue funcionando: solo se muestra que no se pudieron cargar.
        private void CargarEstadisticas()
        {
            BLL.ESTADISTICA estadisticaBLL = new BLL.ESTADISTICA();
            ESTADISTICA estadistica1 = null;
            try
            {
                estadistica1 = estadisticaBLL.Calcular(jugador1);
                lblStats1.Text = ResumenEstadistica(estadistica1);
                if (jugador2 != null)
                {
                    lblStats2.Text = ResumenEstadistica(estadisticaBLL.Calcular(jugador2));

                    Tuple<int, int> cara = estadisticaBLL.Enfrentamientos(jugador1, jugador2);
                    lblCaraACara.Text = cara.Item1 + cara.Item2 == 0
                        ? "Primera partida entre " + jugador1.Nombre + " y " + jugador2.Nombre
                        : "Cara a cara:  " + jugador1.Nombre + "  " + cara.Item1 + " – " + cara.Item2 + "  " + jugador2.Nombre;
                }
                else
                {
                    lblCaraACara.Text = "";
                }
            }
            catch (Exception)
            {
                lblStats1.Text = "Estadísticas no disponibles";
                if (jugador2 != null)
                {
                    lblStats2.Text = "Estadísticas no disponibles";
                }
                lblCaraACara.Text = "";
            }

            lblSeccionHistorial.Text = "Últimas partidas de " + jugador1.Nombre;
            MostrarHistorial(estadistica1 != null ? estadistica1.Historial : null);
        }

        private static string ResumenEstadistica(ESTADISTICA estadistica)
        {
            int total = estadistica.Ganadas + estadistica.Perdidas;
            if (total == 0)
            {
                return "Sin partidas jugadas";
            }
            return estadistica.Ganadas + (estadistica.Ganadas == 1 ? " ganada" : " ganadas") + " · " +
                estadistica.Perdidas + (estadistica.Perdidas == 1 ? " perdida" : " perdidas") +
                " (" + estadistica.PromedioVictorias.ToString("0") + "%)";
        }

        // Una fila por partida (la más reciente arriba): insignia G/P, rival y fecha.
        private void MostrarHistorial(List<FILAHISTORIAL> historial)
        {
            pnlHistorial.SuspendLayout();
            while (pnlHistorial.Controls.Count > 0)
            {
                pnlHistorial.Controls[0].Dispose();
            }

            if (historial == null || historial.Count == 0)
            {
                Label vacio = new Label();
                vacio.AutoSize = false;
                vacio.Dock = DockStyle.Fill;
                vacio.TextAlign = ContentAlignment.MiddleCenter;
                vacio.ForeColor = Branding.Secundario;
                vacio.Text = historial == null
                    ? "No se pudo cargar el historial."
                    : "Todavía no hay partidas.\n¡Jugá la primera!";
                pnlHistorial.Controls.Add(vacio);
            }
            else
            {
                for (int i = 0; i < historial.Count && i < CANTIDAD_HISTORIAL; i++)
                {
                    AgregarFilaHistorial(historial[i], 4 + i * ALTO_FILA_HISTORIAL);
                }
            }
            pnlHistorial.ResumeLayout();
        }

        private void AgregarFilaHistorial(FILAHISTORIAL fila, int y)
        {
            bool ganada = fila.Resultado == "Ganada";

            Label insignia = new Label();
            insignia.AutoSize = false;
            insignia.Location = new Point(10, y + 7);
            insignia.Size = new Size(26, 26);
            insignia.TextAlign = ContentAlignment.MiddleCenter;
            insignia.Font = new Font("Montserrat", 9F, FontStyle.Bold);
            insignia.BackColor = ganada ? Branding.Primario : Branding.TableroVacio;
            insignia.ForeColor = ganada ? Color.White : Branding.Secundario;
            insignia.Text = ganada ? "G" : "P";
            pnlHistorial.Controls.Add(insignia);

            Label rival = new Label();
            rival.AutoSize = true;
            rival.Location = new Point(44, y + 3);
            rival.Font = new Font("Montserrat", 9F, FontStyle.Bold);
            rival.ForeColor = Branding.TextoPrincipal;
            rival.Text = "vs " + fila.Rival;
            pnlHistorial.Controls.Add(rival);

            Label detalle = new Label();
            detalle.AutoSize = true;
            detalle.Location = new Point(44, y + 21);
            detalle.Font = new Font("Montserrat", 8F);
            detalle.ForeColor = Branding.Secundario;
            detalle.Text = fila.Resultado + " · " + fila.Fecha;
            pnlHistorial.Controls.Add(detalle);
        }

        // Un invitado se distingue de un usuario con cuenta: sus estadísticas son compartidas
        // por todos los que juegan como ese invitado.
        private void MostrarChip(Label chip, USUARIO jugador, string textoConCuenta)
        {
            bool invitado = BLL.USUARIO.EsInvitado(jugador);
            chip.Text = invitado ? "Invitado" : textoConCuenta;
            chip.ForeColor = invitado ? Branding.Pendiente : Branding.Exito;
            toolTip.SetToolTip(chip, invitado
                ? "Jugador sin cuenta: las partidas y estadísticas de " + jugador.Nombre + " son compartidas por todos los invitados."
                : "");
        }

        private static string Inicial(string nombre)
        {
            return nombre.Length > 0 ? nombre.Substring(0, 1).ToUpper() : "?";
        }

        // Avatar redondo: círculo del color guardado en Tag con la inicial encima. Se dibuja
        // a mano (en vez de recortar el control con Region) para que el borde quede suavizado.
        private void DibujarAvatar(object sender, PaintEventArgs e)
        {
            Label avatar = (Label)sender;
            Color color = avatar.Tag is Color ? (Color)avatar.Tag : Branding.Secundario;
            e.Graphics.Clear(avatar.BackColor);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (SolidBrush pincel = new SolidBrush(color))
            {
                e.Graphics.FillEllipse(pincel, 0, 0, avatar.Width - 1, avatar.Height - 1);
            }
            TextRenderer.DrawText(e.Graphics, avatar.Text, avatar.Font, new Rectangle(0, 0, avatar.Width, avatar.Height),
                avatar.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        // Borde redondeado suave para las tarjetas de jugador y el panel del historial.
        private void DibujarBorde(object sender, PaintEventArgs e)
        {
            Control control = (Control)sender;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, control.Width - 1, control.Height - 1);
            using (GraphicsPath camino = new GraphicsPath())
            using (Pen lapiz = new Pen(Branding.TableroVacio, 1.5F))
            {
                int d = 12;
                camino.AddArc(rect.X, rect.Y, d, d, 180, 90);
                camino.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
                camino.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                camino.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
                camino.CloseFigure();
                e.Graphics.DrawPath(lapiz, camino);
            }
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

            // La partida que terminó ya cuenta en las estadísticas y el historial.
            ActualizarPantalla();
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
            using (FormEstadisticas formEstadisticas = new FormEstadisticas(jugador1, jugador2))
            {
                formEstadisticas.ShowDialog(this);
            }
        }

        private void lnkVerEstadisticas_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            btnEstadisticas_Click(sender, e);
        }

        private void btnReglas_Click(object sender, EventArgs e)
        {
            using (FormReglas formReglas = new FormReglas())
            {
                formReglas.ShowDialog(this);
            }
        }
    }
}
