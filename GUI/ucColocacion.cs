using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GUI
{
    // Fase de colocación de un jugador: tablero propio + flota. Todo el estado real (qué
    // naves están colocadas, si una posición es válida) lo valida BLL.PARTIDA; este
    // control solo lo muestra y le avisa lo que hace el usuario.
    //
    // Interacción:
    // - Elegir una nave de la flota (la primera pendiente ya viene elegida). Al pasar el
    //   mouse por el tablero se ve dónde quedaría y si entra; click la coloca.
    // - La orientación de la nave nueva se cambia con "Girar", la tecla R o click derecho
    //   sobre el tablero. Esc deja de tener una nave elegida.
    // - Sobre una nave ya colocada: arrastrar la mueve, click la gira, click derecho la saca.
    // - "Al azar" coloca las que faltan y "Reiniciar" saca todas.
    // - "Confirmar flota" cierra la colocación (recién ahí se pasa al otro jugador).
    public partial class ucColocacion : UserControl
    {
        private const int TAM_CELDA = 24;
        private const int ALTO_FILA_FLOTA = 46;

        private BLL.PARTIDA partidaBLL;
        private int idJugador;
        private ucCelda[,] celdas = new ucCelda[BLL.PARTIDA.TAMAÑO_TABLERO, BLL.PARTIDA.TAMAÑO_TABLERO];

        // Nave nueva elegida para colocar (null: ninguna) y orientación con la que entra.
        private BE.TIPONAVE naveSeleccionada;
        private BE.ORIENTACION orientacionNueva = BE.ORIENTACION.Horizontal;

        // Celda bajo el mouse y celdas pintadas con la vista previa de la nave elegida.
        private ucCelda celdaBajoMouse;
        private List<ucCelda> celdasPrevia = new List<ucCelda>();

        // Una ilustración por nave colocada (clave: Id del tipo). Se reutilizan al mover
        // o girar en vez de crearlas de nuevo, porque se reacomodan dentro de sus propios
        // eventos de mouse.
        private Dictionary<int, PictureBox> imagenesNaves = new Dictionary<int, PictureBox>();

        // Una fila por tipo de nave en el panel de la flota (clave: Id del tipo).
        private Dictionary<int, Panel> filasFlota = new Dictionary<int, Panel>();
        private Dictionary<int, Label> estadosFlota = new Dictionary<int, Label>();

        // Estado del arrastre en curso (imagenArrastrada == null: no hay botón apretado sobre una nave).
        private PictureBox imagenArrastrada;
        private bool arrastrando;
        private Point puntoInicial;
        private int filaAgarre;
        private int columnaAgarre;
        private int filaDestino;
        private int columnaDestino;

        // Se dispara cuando el jugador confirma su flota, para que FormPartida pase
        // al siguiente jugador o al combate.
        public event EventHandler Actualizado;

        public ucColocacion(BLL.PARTIDA partidaBLL, int idJugador)
        {
            InitializeComponent();
            this.partidaBLL = partidaBLL;
            this.idJugador = idJugador;

            BE.USUARIO jugador = idJugador == partidaBLL.Jugador1.ID ? partidaBLL.Jugador1 : partidaBLL.Jugador2;
            lblTitulo.Text = "Colocación de naves: " + jugador.Nombre;
            lblIndicacion.Text = "Elegí una nave, apuntá en el tablero y hacé click para colocarla.";

            ConstruirTablero();
            ConstruirFilasFlota();

            TableroUI.DibujarReglas(this, OrigenTablero, TAM_CELDA, BLL.PARTIDA.TAMAÑO_TABLERO, 18);
            TableroUI.DibujarLeyenda(this, new Point(OrigenTablero.X, OrigenTablero.Y + LADO_TABLERO + 12),
                Tuple.Create(ESTADO_VISUAL_CELDA.Vacia, "Vacío"),
                Tuple.Create(ESTADO_VISUAL_CELDA.Nave, "Nave propia"),
                Tuple.Create(ESTADO_VISUAL_CELDA.Tocado, "No entra ahí"));

            ActualizarBotonGirar();
            SeleccionarSiguientePendiente();
            RefrescarFlota();
        }

        private void ConstruirTablero()
        {
            int tam = BLL.PARTIDA.TAMAÑO_TABLERO;
            for (int f = 0; f < tam; f++)
            {
                for (int c = 0; c < tam; c++)
                {
                    ucCelda celda = new ucCelda();
                    celda.Fila = f;
                    celda.Columna = c;
                    celda.Size = new Size(TAM_CELDA, TAM_CELDA);
                    celda.Location = new Point(c * TAM_CELDA, f * TAM_CELDA);
                    celda.MouseUp += celda_MouseUp;
                    celda.MouseEnter += celda_MouseEnter;
                    celda.MouseLeave += celda_MouseLeave;
                    pnlTablero.Controls.Add(celda);
                    celdas[f, c] = celda;
                }
            }
        }

        // Una fila por tipo de nave: ilustración, nombre, un cuadradito por casillero y estado.
        private void ConstruirFilasFlota()
        {
            for (int i = 0; i < BE.TIPONAVE.Flota.Length; i++)
            {
                BE.TIPONAVE tipo = BE.TIPONAVE.Flota[i];

                Panel fila = new Panel();
                fila.Location = new Point(0, i * ALTO_FILA_FLOTA);
                fila.Size = new Size(pnlFlota.Width, ALTO_FILA_FLOTA - 6);
                fila.BackColor = Branding.Panel;
                fila.Cursor = Cursors.Hand;
                fila.Tag = tipo;
                fila.Paint += filaFlota_Paint;
                fila.Click += filaFlota_Click;

                PictureBox imagen = new PictureBox();
                imagen.Location = new Point(10, 6);
                imagen.Size = new Size(58, 28);
                imagen.SizeMode = PictureBoxSizeMode.Zoom;
                imagen.Image = RecursosNaves.Obtener(tipo.Nombre);
                imagen.BackColor = Color.Transparent;
                fila.Controls.Add(imagen);

                Label nombre = new Label();
                nombre.AutoSize = true;
                nombre.Location = new Point(76, 3);
                nombre.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                nombre.ForeColor = Branding.TextoPrincipal;
                nombre.BackColor = Color.Transparent;
                nombre.Text = tipo.Nombre + "  (" + tipo.Tamaño + (tipo.Tamaño == 1 ? " casillero)" : " casilleros)");
                fila.Controls.Add(nombre);

                Label estado = new Label();
                estado.AutoSize = false;
                estado.Location = new Point(fila.Width - 130, 10);
                estado.Size = new Size(118, 20);
                estado.TextAlign = ContentAlignment.MiddleRight;
                estado.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
                estado.BackColor = Color.Transparent;
                fila.Controls.Add(estado);

                // Click en cualquier parte de la fila elige esa nave.
                foreach (Control hijo in fila.Controls)
                {
                    hijo.Cursor = Cursors.Hand;
                    hijo.Click += filaFlota_Click;
                }

                pnlFlota.Controls.Add(fila);
                filasFlota[tipo.Id] = fila;
                estadosFlota[tipo.Id] = estado;
            }
        }

        // -------------------- Elegir nave y orientación --------------------

        private void filaFlota_Click(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            Panel fila = control as Panel ?? (Panel)control.Parent;
            BE.TIPONAVE tipo = (BE.TIPONAVE)fila.Tag;

            if (EstaColocada(tipo))
            {
                naveSeleccionada = null;
                MostrarAviso("El " + tipo.Nombre + " ya está en el tablero: arrastralo para moverlo o hacé click derecho para sacarlo.");
            }
            else
            {
                naveSeleccionada = tipo;
                LimpiarMensaje();
            }
            RefrescarFlota();
            ActualizarPrevia();
        }

        private void SeleccionarSiguientePendiente()
        {
            List<BE.TIPONAVE> pendientes = partidaBLL.FlotaPendiente(idJugador);
            naveSeleccionada = pendientes.Count > 0 ? pendientes[0] : null;
        }

        private void btnGirar_Click(object sender, EventArgs e)
        {
            Girar();
        }

        private void Girar()
        {
            orientacionNueva = orientacionNueva == BE.ORIENTACION.Horizontal
                ? BE.ORIENTACION.Vertical
                : BE.ORIENTACION.Horizontal;
            ActualizarBotonGirar();
            ActualizarPrevia();
        }

        private void ActualizarBotonGirar()
        {
            btnGirar.Text = "⟳  Girar (R): " + (orientacionNueva == BE.ORIENTACION.Horizontal ? "Horizontal" : "Vertical");
        }

        // R gira la nave nueva y Esc la deja de elegir, sin importar qué control tenga el foco.
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.R)
            {
                Girar();
                return true;
            }
            if (keyData == Keys.Escape && naveSeleccionada != null)
            {
                naveSeleccionada = null;
                RefrescarFlota();
                ActualizarPrevia();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // -------------------- Vista previa y colocación --------------------

        private void celda_MouseEnter(object sender, EventArgs e)
        {
            celdaBajoMouse = (ucCelda)sender;
            ActualizarPrevia();
        }

        private void celda_MouseLeave(object sender, EventArgs e)
        {
            if (celdaBajoMouse == sender)
            {
                celdaBajoMouse = null;
            }
            ActualizarPrevia();
        }

        // Posición donde entraría la nave elegida si se hace click en la celda bajo el mouse:
        // si se apunta muy cerca del borde, se corre hacia adentro para que entre.
        private Point PosicionPrevia()
        {
            int fila = celdaBajoMouse.Fila;
            int columna = celdaBajoMouse.Columna;
            int maximo = BLL.PARTIDA.TAMAÑO_TABLERO - naveSeleccionada.Tamaño;
            if (orientacionNueva == BE.ORIENTACION.Horizontal)
            {
                columna = Math.Min(columna, maximo);
            }
            else
            {
                fila = Math.Min(fila, maximo);
            }
            return new Point(columna, fila);
        }

        private void ActualizarPrevia()
        {
            foreach (ucCelda celda in celdasPrevia)
            {
                celda.Previa = PREVIA_CELDA.Ninguna;
            }
            celdasPrevia.Clear();

            if (celdaBajoMouse == null || imagenArrastrada != null)
            {
                lblPosicion.Text = "";
                return;
            }

            if (naveSeleccionada == null)
            {
                lblPosicion.Text = TableroUI.Coordenada(celdaBajoMouse.Fila, celdaBajoMouse.Columna);
                return;
            }

            Point posicion = PosicionPrevia();
            bool valida = partidaBLL.PosicionValida(idJugador, naveSeleccionada, posicion.Y, posicion.X, orientacionNueva);
            for (int i = 0; i < naveSeleccionada.Tamaño; i++)
            {
                ucCelda celda = orientacionNueva == BE.ORIENTACION.Horizontal
                    ? celdas[posicion.Y, posicion.X + i]
                    : celdas[posicion.Y + i, posicion.X];
                celda.Previa = valida ? PREVIA_CELDA.Valida : PREVIA_CELDA.Invalida;
                celdasPrevia.Add(celda);
            }

            lblPosicion.Text = naveSeleccionada.Nombre + " en " + TableroUI.Coordenada(posicion.Y, posicion.X) +
                (orientacionNueva == BE.ORIENTACION.Horizontal ? " · horizontal" : " · vertical");
        }

        private void celda_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Click derecho sobre el tablero: girar la nave que se está por colocar.
                Girar();
                return;
            }
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            celdaBajoMouse = (ucCelda)sender;
            if (naveSeleccionada == null)
            {
                MostrarAviso(partidaBLL.FlotaCompleta(idJugador)
                    ? "Ya colocaste toda la flota: revisala y confirmala."
                    : "Elegí primero una nave de la flota.");
                return;
            }

            Point posicion = PosicionPrevia();
            try
            {
                partidaBLL.ColocarNave(idJugador, naveSeleccionada, posicion.Y, posicion.X, orientacionNueva);
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
                return;
            }

            LimpiarMensaje();
            SeleccionarSiguientePendiente();
            Redibujar();
            RefrescarFlota();
            ActualizarPrevia();
        }

        // -------------------- Mover (arrastrar), girar (click) y sacar (click derecho) --------------------

        private void imagenNave_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            imagenArrastrada = (PictureBox)sender;
            arrastrando = false;
            puntoInicial = Control.MousePosition;
            // Qué celda de la nave se agarró, para que esa misma celda quede bajo el cursor.
            filaAgarre = e.Y / TAM_CELDA;
            columnaAgarre = e.X / TAM_CELDA;
        }

        private void imagenNave_MouseMove(object sender, MouseEventArgs e)
        {
            if (imagenArrastrada == null)
            {
                return;
            }
            if (e.Button != MouseButtons.Left)
            {
                // Se perdió el botón sin recibir MouseUp (por ejemplo, cambio de ventana).
                CancelarArrastre();
                return;
            }

            BE.NAVECOLOCADA nave = NaveDe(imagenArrastrada);

            if (!arrastrando)
            {
                // Hasta superar el umbral del sistema se considera un click (girar), no un arrastre.
                Point actual = Control.MousePosition;
                Size umbral = SystemInformation.DragSize;
                if (Math.Abs(actual.X - puntoInicial.X) < umbral.Width && Math.Abs(actual.Y - puntoInicial.Y) < umbral.Height)
                {
                    return;
                }
                arrastrando = true;
                imagenArrastrada.Cursor = Cursors.SizeAll;
                imagenArrastrada.BringToFront();
                ActualizarPrevia();
                // Mientras se arrastra, sus celdas originales se ven libres.
                foreach (Tuple<int, int> celda in nave.Celdas())
                {
                    celdas[celda.Item1, celda.Item2].Estado = ESTADO_VISUAL_CELDA.Vacia;
                }
            }

            int tam = BLL.PARTIDA.TAMAÑO_TABLERO;
            int altoCeldas = nave.Orientacion == BE.ORIENTACION.Horizontal ? 1 : nave.Tipo.Tamaño;
            int anchoCeldas = nave.Orientacion == BE.ORIENTACION.Horizontal ? nave.Tipo.Tamaño : 1;

            Point cursor = pnlTablero.PointToClient(Control.MousePosition);
            int fila = (int)Math.Floor((double)cursor.Y / TAM_CELDA) - filaAgarre;
            int columna = (int)Math.Floor((double)cursor.X / TAM_CELDA) - columnaAgarre;
            // La nave queda siempre dentro del tablero: se "pega" al borde en vez de salirse.
            filaDestino = Math.Max(0, Math.Min(fila, tam - altoCeldas));
            columnaDestino = Math.Max(0, Math.Min(columna, tam - anchoCeldas));

            imagenArrastrada.Location = new Point(columnaDestino * TAM_CELDA, filaDestino * TAM_CELDA);
            lblPosicion.Text = nave.Tipo.Nombre + " → " + TableroUI.Coordenada(filaDestino, columnaDestino);

            bool valida = partidaBLL.PosicionValida(idJugador, nave.Tipo, filaDestino, columnaDestino, nave.Orientacion);
            imagenArrastrada.BackColor = valida ? Color.Empty : Branding.TableroTocado;
            if (valida)
            {
                LimpiarMensaje();
            }
            else
            {
                MostrarError("Ahí se superpone con otra nave.");
            }
        }

        private void imagenNave_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && imagenArrastrada == null)
            {
                SacarNave(NaveDe((PictureBox)sender));
                return;
            }
            if (imagenArrastrada == null || e.Button != MouseButtons.Left)
            {
                return;
            }

            BE.NAVECOLOCADA nave = NaveDe(imagenArrastrada);
            bool eraArrastre = arrastrando;
            imagenArrastrada = null;
            arrastrando = false;

            try
            {
                if (eraArrastre)
                {
                    if (filaDestino != nave.Fila || columnaDestino != nave.Columna)
                    {
                        partidaBLL.MoverNave(idJugador, nave.Tipo, filaDestino, columnaDestino, nave.Orientacion);
                    }
                }
                else
                {
                    BE.ORIENTACION girada = nave.Orientacion == BE.ORIENTACION.Horizontal
                        ? BE.ORIENTACION.Vertical
                        : BE.ORIENTACION.Horizontal;
                    partidaBLL.MoverNave(idJugador, nave.Tipo, nave.Fila, nave.Columna, girada);
                }
                LimpiarMensaje();
            }
            catch (Exception ex)
            {
                MostrarError((eraArrastre ? "No se puede soltar ahí: " : "No se puede girar ahí: ") + ex.Message);
            }

            // Si la BLL rechazó el cambio, esto devuelve la nave a su lugar anterior.
            Redibujar();
            lblPosicion.Text = "";
        }

        // La nave vuelve a la flota y queda elegida, lista para ponerla en otro lugar.
        private void SacarNave(BE.NAVECOLOCADA nave)
        {
            try
            {
                partidaBLL.QuitarNave(idJugador, nave.Tipo);
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
                return;
            }

            naveSeleccionada = nave.Tipo;
            orientacionNueva = nave.Orientacion;
            ActualizarBotonGirar();
            LimpiarMensaje();
            Redibujar();
            RefrescarFlota();
        }

        private void CancelarArrastre()
        {
            imagenArrastrada = null;
            arrastrando = false;
            LimpiarMensaje();
            Redibujar();
        }

        private static BE.NAVECOLOCADA NaveDe(PictureBox imagen)
        {
            return (BE.NAVECOLOCADA)imagen.Tag;
        }

        // -------------------- Al azar / reiniciar / confirmar --------------------

        // Con naves pendientes, coloca al azar solo las que faltan. Con la flota ya completa
        // (antes no hacía nada y avisaba igual que la había colocado), la vuelve a distribuir
        // entera: cada click da otra disposición.
        private void btnAzar_Click(object sender, EventArgs e)
        {
            bool redistribuir = partidaBLL.FlotaCompleta(idJugador);
            try
            {
                if (redistribuir)
                {
                    partidaBLL.ReiniciarColocacion(idJugador);
                }
                partidaBLL.ColocarAlAzar(idJugador);
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
                Redibujar();
                RefrescarFlota();
                return;
            }

            naveSeleccionada = null;
            MostrarAviso(redistribuir
                ? "Flota redistribuida al azar: podés corregirla o confirmarla."
                : "Naves que faltaban colocadas al azar: podés corregirlas o confirmar.");
            Redibujar();
            RefrescarFlota();
        }

        private void btnReiniciar_Click(object sender, EventArgs e)
        {
            if (partidaBLL.NavesDe(idJugador).Count == 0)
            {
                return;
            }

            try
            {
                partidaBLL.ReiniciarColocacion(idJugador);
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
                return;
            }

            SeleccionarSiguientePendiente();
            LimpiarMensaje();
            Redibujar();
            RefrescarFlota();
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                partidaBLL.ConfirmarColocacion(idJugador);
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
                return;
            }

            if (Actualizado != null)
            {
                Actualizado(this, EventArgs.Empty);
            }
        }

        // -------------------- Dibujo --------------------

        // Vuelve a pintar el tablero completo a partir de las naves que tiene la BLL.
        private void Redibujar()
        {
            foreach (ucCelda celda in celdas)
            {
                celda.Estado = ESTADO_VISUAL_CELDA.Vacia;
            }

            List<BE.NAVECOLOCADA> naves = partidaBLL.NavesDe(idJugador);

            // Ilustraciones de naves que se sacaron del tablero (click derecho o "Reiniciar").
            List<int> sobrantes = new List<int>();
            foreach (int idTipo in imagenesNaves.Keys)
            {
                if (!naves.Exists(n => n.Tipo.Id == idTipo))
                {
                    sobrantes.Add(idTipo);
                }
            }
            foreach (int idTipo in sobrantes)
            {
                imagenesNaves[idTipo].Dispose();
                imagenesNaves.Remove(idTipo);
            }

            foreach (BE.NAVECOLOCADA nave in naves)
            {
                MarcarNaveEnTablero(nave);
            }
        }

        // Colorea las celdas que ocupa la nave y superpone su ilustración, orientada
        // según corresponda (las imágenes originales están dibujadas mirando a la derecha).
        private void MarcarNaveEnTablero(BE.NAVECOLOCADA nave)
        {
            foreach (Tuple<int, int> celda in nave.Celdas())
            {
                celdas[celda.Item1, celda.Item2].Estado = ESTADO_VISUAL_CELDA.Nave;
            }

            bool horizontal = nave.Orientacion == BE.ORIENTACION.Horizontal;
            PictureBox imagen = ImagenDe(nave);
            imagen.Location = new Point(nave.Columna * TAM_CELDA, nave.Fila * TAM_CELDA);
            imagen.Size = horizontal
                ? new Size(nave.Tipo.Tamaño * TAM_CELDA, TAM_CELDA)
                : new Size(TAM_CELDA, nave.Tipo.Tamaño * TAM_CELDA);
            imagen.Image = horizontal
                ? RecursosNaves.Obtener(nave.Tipo.Nombre)
                : RecursosNaves.ObtenerRotada(nave.Tipo.Nombre);
            imagen.BackColor = Color.Empty;
            imagen.Cursor = Cursors.Hand;
            imagen.BringToFront();
        }

        private PictureBox ImagenDe(BE.NAVECOLOCADA nave)
        {
            PictureBox imagen;
            if (!imagenesNaves.TryGetValue(nave.Tipo.Id, out imagen))
            {
                imagen = new PictureBox();
                imagen.SizeMode = PictureBoxSizeMode.StretchImage;
                imagen.MouseDown += imagenNave_MouseDown;
                imagen.MouseMove += imagenNave_MouseMove;
                imagen.MouseUp += imagenNave_MouseUp;
                toolTip.SetToolTip(imagen, nave.Tipo.Nombre + ": click para girarla, arrastrar para moverla, click derecho para sacarla.");
                pnlTablero.Controls.Add(imagen);
                imagenesNaves[nave.Tipo.Id] = imagen;
            }
            imagen.Tag = nave;
            return imagen;
        }

        private bool EstaColocada(BE.TIPONAVE tipo)
        {
            return partidaBLL.NavesDe(idJugador).Exists(n => n.Tipo.Id == tipo.Id);
        }

        private void RefrescarFlota()
        {
            int colocadas = partidaBLL.NavesDe(idJugador).Count;
            int total = BE.TIPONAVE.Flota.Length;

            foreach (BE.TIPONAVE tipo in BE.TIPONAVE.Flota)
            {
                Label estado = estadosFlota[tipo.Id];
                if (EstaColocada(tipo))
                {
                    estado.Text = "✓ Colocada";
                    estado.ForeColor = Branding.Exito;
                }
                else if (naveSeleccionada != null && naveSeleccionada.Id == tipo.Id)
                {
                    estado.Text = "● Elegida";
                    estado.ForeColor = Branding.Primario;
                }
                else
                {
                    estado.Text = "Pendiente";
                    estado.ForeColor = Branding.Pendiente;
                }
                filasFlota[tipo.Id].Invalidate();
            }

            lblRestantes.Text = colocadas + " de " + total + " colocadas";
            pnlProgreso.Invalidate();

            bool completa = partidaBLL.FlotaCompleta(idJugador);
            btnConfirmar.Enabled = completa;
            btnConfirmar.BackColor = completa ? Branding.Primario : Branding.TableroVacio;
            btnConfirmar.Text = completa
                ? "Confirmar flota  ›"
                : "Confirmar flota (" + (total - colocadas) + (total - colocadas == 1 ? " nave pendiente)" : " naves pendientes)");
            lblIndicacion.Text = completa
                ? "Flota completa: revisala y confirmala. Hasta entonces la podés seguir corrigiendo."
                : "Elegí una nave, apuntá en el tablero y hacé click para colocarla.";
        }

        // Fondo de cada fila de la flota: borde suave, o del color principal si es la nave
        // elegida, y un cuadradito por casillero de la nave.
        private void filaFlota_Paint(object sender, PaintEventArgs e)
        {
            Panel fila = (Panel)sender;
            BE.TIPONAVE tipo = (BE.TIPONAVE)fila.Tag;
            bool elegida = naveSeleccionada != null && naveSeleccionada.Id == tipo.Id;
            bool colocada = EstaColocada(tipo);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(1, 1, fila.Width - 3, fila.Height - 3);
            using (GraphicsPath camino = Redondeado(rect, 10))
            using (Pen lapiz = new Pen(elegida ? Branding.Primario : Branding.TableroVacio, elegida ? 2F : 1.5F))
            {
                e.Graphics.DrawPath(lapiz, camino);
            }

            Color colorCasillero = elegida ? Branding.Primario : colocada ? Branding.TableroNave : Branding.TableroVacio;
            using (SolidBrush pincel = new SolidBrush(colorCasillero))
            {
                for (int i = 0; i < tipo.Tamaño; i++)
                {
                    using (GraphicsPath casillero = Redondeado(new Rectangle(78 + i * 16, 24, 13, 9), 3))
                    {
                        e.Graphics.FillPath(pincel, casillero);
                    }
                }
            }
        }

        private void pnlProgreso_Paint(object sender, PaintEventArgs e)
        {
            int colocadas = partidaBLL.NavesDe(idJugador).Count;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle fondo = new Rectangle(0, 0, pnlProgreso.Width - 1, pnlProgreso.Height - 1);
            using (GraphicsPath camino = Redondeado(fondo, 3))
            using (SolidBrush pincel = new SolidBrush(Branding.TableroVacio))
            {
                e.Graphics.FillPath(pincel, camino);
            }
            if (colocadas > 0)
            {
                int ancho = Math.Max(8, fondo.Width * colocadas / BE.TIPONAVE.Flota.Length);
                using (GraphicsPath camino = Redondeado(new Rectangle(0, 0, ancho, fondo.Height), 3))
                using (SolidBrush pincel = new SolidBrush(Branding.Primario))
                {
                    e.Graphics.FillPath(pincel, camino);
                }
            }
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

        private void MostrarError(string mensaje)
        {
            lblMensaje.ForeColor = Branding.TableroHundido;
            lblMensaje.Text = mensaje;
        }

        private void MostrarAviso(string mensaje)
        {
            lblMensaje.ForeColor = Branding.Secundario;
            lblMensaje.Text = mensaje;
        }

        private void LimpiarMensaje()
        {
            lblMensaje.Text = "";
        }
    }
}
