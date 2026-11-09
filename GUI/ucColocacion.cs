using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GUI
{
    // Fase de colocación de un jugador: tablero propio + lista de naves pendientes.
    // Todo el estado real (qué naves están colocadas, si una posición es válida) lo
    // valida BLL.PARTIDA; este control solo lo muestra y le avisa lo que hace el usuario.
    //
    // Interacción con el tablero:
    // - Click en una celda con una nave elegida de la lista: la coloca en horizontal.
    // - Mantener apretado el click sobre una nave colocada y arrastrarla: la mueve.
    // - Click (sin arrastrar) sobre una nave colocada: la gira (horizontal/vertical).
    // - "Confirmar flota": cierra la colocación (recién ahí se pasa al otro jugador).
    public partial class ucColocacion : UserControl
    {
        private const int TAM_CELDA = 24;

        private BLL.PARTIDA partidaBLL;
        private int idJugador;
        private ucCelda[,] celdas = new ucCelda[BLL.PARTIDA.TAMAÑO_TABLERO, BLL.PARTIDA.TAMAÑO_TABLERO];
        private BE.TIPONAVE naveSeleccionada;

        // Una ilustración por nave colocada (clave: Id del tipo). Se reutilizan al mover
        // o girar en vez de crearlas de nuevo, porque se reacomodan dentro de sus propios
        // eventos de mouse.
        private Dictionary<int, PictureBox> imagenesNaves = new Dictionary<int, PictureBox>();

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
            ConstruirTablero();
            ConstruirImagenesFlota();
            RefrescarFlota();

            TableroUI.DibujarReglas(this, OrigenTablero, TAM_CELDA, BLL.PARTIDA.TAMAÑO_TABLERO, 16);
            TableroUI.DibujarLeyenda(this, new Point(OrigenTablero.X, OrigenTablero.Y + 361 + 12),
                Tuple.Create(ESTADO_VISUAL_CELDA.Vacia, "Vacío"),
                Tuple.Create(ESTADO_VISUAL_CELDA.Nave, "Nave propia"));
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
                    celda.Location = new Point(c * TAM_CELDA, f * TAM_CELDA);
                    celda.Click += celda_Click;
                    pnlTablero.Controls.Add(celda);
                    celdas[f, c] = celda;
                }
            }
        }

        // Un ícono de 24x24 por tipo de nave, para la lista de flota pendiente.
        private void ConstruirImagenesFlota()
        {
            imgFlota.ImageSize = new Size(24, 24);
            foreach (BE.TIPONAVE tipo in BE.TIPONAVE.Flota)
            {
                imgFlota.Images.Add(tipo.Nombre, RecursosNaves.Obtener(tipo.Nombre));
            }
        }

        // -------------------- Colocar desde la lista --------------------

        private void celda_Click(object sender, EventArgs e)
        {
            if (naveSeleccionada == null)
            {
                MostrarError("Elegí primero una nave de la lista.");
                return;
            }

            ucCelda celda = (ucCelda)sender;
            // Siempre entra en horizontal; si se hace click muy cerca del borde derecho,
            // se corre hacia la izquierda para que entre (después se puede girar o arrastrar).
            int columna = Math.Min(celda.Columna, BLL.PARTIDA.TAMAÑO_TABLERO - naveSeleccionada.Tamaño);
            try
            {
                partidaBLL.ColocarNave(idJugador, naveSeleccionada, celda.Fila, columna, BE.ORIENTACION.Horizontal);
                naveSeleccionada = null;
                LimpiarMensaje();
                Redibujar();
                RefrescarFlota();
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
            }
        }

        // -------------------- Mover (arrastrar) y girar (click) --------------------

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

        // -------------------- Dibujo --------------------

        // Vuelve a pintar el tablero completo a partir de las naves que tiene la BLL.
        private void Redibujar()
        {
            foreach (ucCelda celda in celdas)
            {
                celda.Estado = ESTADO_VISUAL_CELDA.Vacia;
            }
            foreach (BE.NAVECOLOCADA nave in partidaBLL.NavesDe(idJugador))
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
                toolTip.SetToolTip(imagen, nave.Tipo.Nombre + ": arrastrala para moverla, hacé click para girarla.");
                pnlTablero.Controls.Add(imagen);
                imagenesNaves[nave.Tipo.Id] = imagen;
            }
            imagen.Tag = nave;
            return imagen;
        }

        private void RefrescarFlota()
        {
            lstFlota.Items.Clear();
            List<BE.TIPONAVE> pendientes = partidaBLL.FlotaPendiente(idJugador);
            foreach (BE.TIPONAVE tipo in pendientes)
            {
                ListViewItem item = new ListViewItem(tipo.Nombre + " (" + tipo.Tamaño + ")", tipo.Nombre);
                lstFlota.Items.Add(item);
            }
            lblRestantes.Text = pendientes.Count > 0
                ? pendientes.Count + " nave(s) por colocar"
                : "Flota completa: revisala y confirmá.";
            btnConfirmar.Enabled = partidaBLL.FlotaCompleta(idJugador);
        }

        private void lstFlota_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<BE.TIPONAVE> pendientes = partidaBLL.FlotaPendiente(idJugador);
            int indice = lstFlota.SelectedIndices.Count > 0 ? lstFlota.SelectedIndices[0] : -1;
            naveSeleccionada = (indice >= 0 && indice < pendientes.Count) ? pendientes[indice] : null;
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

        private void MostrarError(string mensaje)
        {
            lblMensaje.ForeColor = Branding.TableroHundido;
            lblMensaje.Text = mensaje;
        }

        private void LimpiarMensaje()
        {
            lblMensaje.Text = "";
        }
    }
}
