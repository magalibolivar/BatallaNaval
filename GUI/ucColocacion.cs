using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GUI
{
    // Fase de colocación de un jugador: tablero propio + lista de naves pendientes.
    // Todo el estado real (qué naves están colocadas, si una posición es válida) lo
    // valida BLL.PARTIDA; este control solo lo muestra y le avisa los clicks.
    public partial class ucColocacion : UserControl
    {
        private const int TAM_CELDA = 24;

        private BLL.PARTIDA partidaBLL;
        private int idJugador;
        private ucCelda[,] celdas = new ucCelda[BLL.PARTIDA.TAMAÑO_TABLERO, BLL.PARTIDA.TAMAÑO_TABLERO];
        private BE.ORIENTACION orientacionElegida = BE.ORIENTACION.Horizontal;
        private BE.TIPONAVE naveSeleccionada;

        // Se dispara después de cada colocación exitosa, para que FormPartida pueda
        // revisar si con esto el jugador ya terminó de colocar toda su flota.
        public event EventHandler Actualizado;

        public ucColocacion(BLL.PARTIDA partidaBLL, int idJugador)
        {
            InitializeComponent();
            this.partidaBLL = partidaBLL;
            this.idJugador = idJugador;
            ConstruirTablero();
            ConstruirImagenesFlota();
            ActualizarOrientacion();
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

        private void celda_Click(object sender, EventArgs e)
        {
            if (naveSeleccionada == null)
            {
                lblMensaje.Text = "Elegí primero una nave de la lista.";
                return;
            }

            ucCelda celda = (ucCelda)sender;
            BE.TIPONAVE tipoColocado = naveSeleccionada;
            BE.ORIENTACION orientacionColocada = orientacionElegida;
            try
            {
                partidaBLL.ColocarNave(idJugador, naveSeleccionada, celda.Fila, celda.Columna, orientacionElegida);
                lblMensaje.Text = "";
                naveSeleccionada = null;
                MarcarNaveEnTablero(tipoColocado, celda.Fila, celda.Columna, orientacionColocada);
                RefrescarFlota();
                if (Actualizado != null)
                {
                    Actualizado(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                lblMensaje.ForeColor = Branding.TableroHundido;
                lblMensaje.Text = ex.Message;
            }
        }

        // Colorea las celdas que ocupa la nave y superpone su ilustración, orientada
        // según corresponda (las imágenes originales están dibujadas mirando a la derecha).
        private void MarcarNaveEnTablero(BE.TIPONAVE tipo, int fila, int columna, BE.ORIENTACION orientacion)
        {
            int ancho = orientacion == BE.ORIENTACION.Horizontal ? tipo.Tamaño * TAM_CELDA : TAM_CELDA;
            int alto = orientacion == BE.ORIENTACION.Horizontal ? TAM_CELDA : tipo.Tamaño * TAM_CELDA;

            for (int i = 0; i < tipo.Tamaño; i++)
            {
                int f = orientacion == BE.ORIENTACION.Horizontal ? fila : fila + i;
                int c = orientacion == BE.ORIENTACION.Horizontal ? columna + i : columna;
                celdas[f, c].Estado = ESTADO_VISUAL_CELDA.Nave;
            }

            PictureBox imagen = new PictureBox();
            imagen.Location = new Point(columna * TAM_CELDA, fila * TAM_CELDA);
            imagen.Size = new Size(ancho, alto);
            imagen.SizeMode = PictureBoxSizeMode.StretchImage;
            imagen.Image = orientacion == BE.ORIENTACION.Horizontal
                ? RecursosNaves.Obtener(tipo.Nombre)
                : RecursosNaves.ObtenerRotada(tipo.Nombre);
            imagen.Cursor = Cursors.Default;
            pnlTablero.Controls.Add(imagen);
            imagen.BringToFront();
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
            lblRestantes.Text = pendientes.Count + " nave(s) por colocar";
        }

        private void lstFlota_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<BE.TIPONAVE> pendientes = partidaBLL.FlotaPendiente(idJugador);
            int indice = lstFlota.SelectedIndices.Count > 0 ? lstFlota.SelectedIndices[0] : -1;
            naveSeleccionada = (indice >= 0 && indice < pendientes.Count) ? pendientes[indice] : null;
        }

        private void btnOrientacion_Click(object sender, EventArgs e)
        {
            orientacionElegida = orientacionElegida == BE.ORIENTACION.Horizontal
                ? BE.ORIENTACION.Vertical
                : BE.ORIENTACION.Horizontal;
            ActualizarOrientacion();
        }

        private void ActualizarOrientacion()
        {
            btnOrientacion.Text = "Orientación: " + (orientacionElegida == BE.ORIENTACION.Horizontal ? "Horizontal" : "Vertical");
        }
    }
}
