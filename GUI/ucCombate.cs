using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GUI
{
    // Fase de combate. Se muestra siempre desde el punto de vista de un jugador (idVista):
    // su propio tablero (con sus naves y los impactos que recibió) y el radar del rival
    // en modo niebla (nunca revela una nave no tocada).
    //
    // Cada turno tiene 2 momentos:
    // 1. Apuntar y disparar: el jugador en turno hace click en una casilla del radar.
    // 2. Ver el resultado: el tablero queda bloqueado mostrando lo que pasó, hasta que el
    //    jugador presiona "Pasar turno". Recién ahí se ocultan los tableros, se muestra la
    //    pantalla de paso de dispositivo y se dibuja el punto de vista del otro jugador.
    //    Sin ese paso, al disparar se mostraría al instante la flota del rival al jugador
    //    que acaba de disparar (ver Reglas_de_Negocio_BatallaNaval.md, "Información oculta").
    public partial class ucCombate : UserControl
    {
        private const int TAM_CELDA = 22;
        private const int ALTO_FILA_FLOTA = 25;

        private BLL.PARTIDA partidaBLL;
        private ucCelda[,] celdasPropias = new ucCelda[BLL.PARTIDA.TAMAÑO_TABLERO, BLL.PARTIDA.TAMAÑO_TABLERO];
        private ucCelda[,] celdasEnemigo = new ucCelda[BLL.PARTIDA.TAMAÑO_TABLERO, BLL.PARTIDA.TAMAÑO_TABLERO];

        // Jugador desde cuyo punto de vista se dibuja la pantalla. Después de disparar sigue
        // siendo el atacante (aunque en BLL el turno ya pasó) hasta que presiona "Pasar turno".
        private int idVista;
        private bool esperandoPase;
        private BE.ESTADOCELDA[,] radarActual;

        // Una fila por tipo de nave en el panel "Flota de <rival>" (clave: Id del tipo).
        private Dictionary<int, Label> nombresFlotaRival = new Dictionary<int, Label>();
        private Dictionary<int, Label> estadosFlotaRival = new Dictionary<int, Label>();
        private Font fuenteNave;
        private Font fuenteNaveHundida;

        public ucCombate(BLL.PARTIDA partidaBLL)
        {
            InitializeComponent();
            this.partidaBLL = partidaBLL;

            ConstruirTablero(pnlTableroPropio, celdasPropias, false);
            ConstruirTablero(pnlTableroEnemigo, celdasEnemigo, true);
            ConstruirFlotaRival();

            TableroUI.DibujarReglas(this, OrigenTableroPropio, TAM_CELDA, BLL.PARTIDA.TAMAÑO_TABLERO, 18);
            TableroUI.DibujarReglas(this, OrigenTableroEnemigo, TAM_CELDA, BLL.PARTIDA.TAMAÑO_TABLERO, 18);
            TableroUI.DibujarLeyenda(this, new Point(OrigenTableroPropio.X, OrigenTableroPropio.Y + LADO_TABLERO + 30),
                Tuple.Create(ESTADO_VISUAL_CELDA.Vacia, "Sin disparar"),
                Tuple.Create(ESTADO_VISUAL_CELDA.Nave, "Nave propia"),
                Tuple.Create(ESTADO_VISUAL_CELDA.Agua, "Agua"),
                Tuple.Create(ESTADO_VISUAL_CELDA.Tocado, "Tocado"),
                Tuple.Create(ESTADO_VISUAL_CELDA.Hundido, "Hundido"));

            idVista = partidaBLL.JugadorEnTurno;
            EmpezarTurno();
        }

        private void ConstruirTablero(Panel panel, ucCelda[,] celdas, bool esRadar)
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
                    if (esRadar)
                    {
                        celda.Click += celda_Click;
                        celda.MouseEnter += celda_MouseEnter;
                        celda.MouseLeave += celda_MouseLeave;
                    }
                    else
                    {
                        // El tablero propio es solo informativo durante el combate.
                        celda.Cursor = Cursors.Default;
                    }
                    panel.Controls.Add(celda);
                    celdas[f, c] = celda;
                }
            }
        }

        private void ConstruirFlotaRival()
        {
            fuenteNave = new Font("Montserrat", 8.5F);
            fuenteNaveHundida = new Font("Montserrat", 8.5F, FontStyle.Strikeout);

            for (int i = 0; i < BE.TIPONAVE.Flota.Length; i++)
            {
                BE.TIPONAVE tipo = BE.TIPONAVE.Flota[i];
                int y = 4 + i * ALTO_FILA_FLOTA;

                PictureBox imagen = new PictureBox();
                imagen.Location = new Point(6, y);
                imagen.Size = new Size(44, 20);
                imagen.SizeMode = PictureBoxSizeMode.Zoom;
                imagen.Image = RecursosNaves.Obtener(tipo.Nombre);
                pnlFlotaRival.Controls.Add(imagen);

                Label nombre = new Label();
                nombre.AutoSize = true;
                nombre.Font = fuenteNave;
                nombre.ForeColor = Branding.TextoPrincipal;
                nombre.Location = new Point(56, y + 2);
                nombre.Text = tipo.Nombre + " (" + tipo.Tamaño + ")";
                pnlFlotaRival.Controls.Add(nombre);
                nombresFlotaRival[tipo.Id] = nombre;

                Label estado = new Label();
                estado.AutoSize = false;
                estado.Font = new Font("Montserrat", 8.5F, FontStyle.Bold);
                estado.TextAlign = ContentAlignment.MiddleRight;
                estado.Location = new Point(150, y);
                estado.Size = new Size(78, 20);
                pnlFlotaRival.Controls.Add(estado);
                estadosFlotaRival[tipo.Id] = estado;
            }
        }

        // -------------------- Momentos del turno --------------------

        // Arranca el turno de idVista: tableros desbloqueados y, si el rival ya disparó,
        // qué le hizo en su último disparo (así no tiene que buscarlo en el tablero).
        private void EmpezarTurno()
        {
            esperandoPase = false;
            btnPasarTurno.Visible = false;
            pnlBanner.BackColor = Branding.Primario;
            lblTurno.Text = "Tu turno, " + NombreDe(idVista);
            lblIndicacion.Text = "Hacé click en una casilla del radar enemigo para disparar.";
            lblApuntando.Text = "";

            BE.DISPARO delRival = partidaBLL.UltimoDisparo;
            if (delRival == null)
            {
                lblResultado.Text = "¡A combatir!";
                lblResultado.ForeColor = Branding.Primario;
                lblDetalleResultado.ForeColor = Branding.TextoPrincipal;
                lblDetalleResultado.Text = "Dispará primero sobre el radar de " + NombreDe(Otro(idVista)) + ".";
            }
            else
            {
                MostrarResultado(delRival, false);
            }

            Refrescar();
        }

        private void celda_Click(object sender, EventArgs e)
        {
            if (esperandoPase || !partidaBLL.EnCurso)
            {
                return;
            }

            ucCelda celda = (ucCelda)sender;
            if (radarActual[celda.Fila, celda.Columna] != BE.ESTADOCELDA.Desconocido)
            {
                lblResultado.Text = "";
                lblDetalleResultado.ForeColor = Branding.TableroHundido;
                lblDetalleResultado.Text = "Ya disparaste a " + TableroUI.Coordenada(celda.Fila, celda.Columna) + ". Elegí otra casilla.";
                return;
            }

            try
            {
                partidaBLL.Disparar(idVista, celda.Fila, celda.Columna);
            }
            catch (Exception ex)
            {
                lblResultado.Text = "";
                lblDetalleResultado.ForeColor = Branding.TableroHundido;
                lblDetalleResultado.Text = ex.Message;
                return;
            }

            // Si ese disparo terminó la partida, FormPartida ya recibió PartidaFinalizada
            // y llamó a MostrarFinal: no hay turno que pasar.
            if (!partidaBLL.EnCurso)
            {
                return;
            }

            celda.Resaltada = false;
            celda.Cursor = Cursors.Default;
            esperandoPase = true;
            MostrarResultado(partidaBLL.UltimoDisparo, true);
            Refrescar();

            string rival = NombreDe(Otro(idVista));
            pnlBanner.BackColor = Branding.Secundario;
            lblTurno.Text = "Disparo realizado";
            lblIndicacion.Text = "Mirá el resultado y pasale el turno a " + rival + ".";
            lblApuntando.Text = "";
            btnPasarTurno.Text = "Pasar turno a " + rival + "  ›";
            btnPasarTurno.Visible = true;
            btnPasarTurno.Focus();
        }

        private void btnPasarTurno_Click(object sender, EventArgs e)
        {
            int siguiente = partidaBLL.JugadorEnTurno;

            // Se ocultan los tableros antes de abrir la pantalla de paso: si no, el
            // siguiente jugador vería detrás del diálogo la flota del que acaba de jugar.
            pnlTableroPropio.Visible = false;
            pnlTableroEnemigo.Visible = false;
            btnPasarTurno.Visible = false;
            lblResultado.Text = "";
            lblDetalleResultado.Text = "";
            pnlBanner.BackColor = Branding.Secundario;
            lblTurno.Text = "Cambio de turno";
            lblIndicacion.Text = "";

            using (FormPasoTurno paso = new FormPasoTurno(
                "Turno de " + NombreDe(siguiente) + ".\n\nPasá el dispositivo y presioná Continuar."))
            {
                paso.ShowDialog(this.FindForm());
            }

            idVista = siguiente;
            pnlTableroPropio.Visible = true;
            pnlTableroEnemigo.Visible = true;
            EmpezarTurno();
        }

        // La llama FormPartida al terminar la partida (por hundimiento o abandono): ya no
        // hay nada que ocultar, así que el radar muestra también las naves que el rival
        // tenía sin descubrir.
        public void MostrarFinal(bool abandonada)
        {
            esperandoPase = true;
            btnPasarTurno.Visible = false;
            pnlTableroPropio.Visible = true;
            pnlTableroEnemigo.Visible = true;

            pnlBanner.BackColor = Branding.TextoPrincipal;
            lblTurno.Text = "Partida terminada";
            lblIndicacion.Text = "Se revelan las naves que quedaron sin descubrir.";
            lblApuntando.Text = "";

            if (abandonada || partidaBLL.UltimoDisparo == null)
            {
                lblResultado.Text = "";
                lblDetalleResultado.Text = "";
            }
            else
            {
                MostrarResultado(partidaBLL.UltimoDisparo, partidaBLL.UltimoDisparo.IdAtacante == idVista);
            }

            Refrescar();

            bool[,] ocupacionRival = partidaBLL.ObtenerOcupacionPropia(Otro(idVista));
            int tam = BLL.PARTIDA.TAMAÑO_TABLERO;
            for (int f = 0; f < tam; f++)
            {
                for (int c = 0; c < tam; c++)
                {
                    celdasEnemigo[f, c].Cursor = Cursors.Default;
                    if (ocupacionRival[f, c] && radarActual[f, c] == BE.ESTADOCELDA.Desconocido)
                    {
                        celdasEnemigo[f, c].Estado = ESTADO_VISUAL_CELDA.Nave;
                    }
                }
            }
        }

        // propio: true si el disparo lo hizo idVista (resultado de su tiro), false si fue
        // el rival (lo que le pasó a su flota).
        private void MostrarResultado(BE.DISPARO disparo, bool propio)
        {
            string coordenada = disparo.Coordenada;
            string rival = NombreDe(Otro(idVista));
            lblDetalleResultado.ForeColor = Branding.TextoPrincipal;

            switch (disparo.Resultado)
            {
                case BE.ESTADOCELDA.Agua:
                    lblResultado.ForeColor = Branding.TextoPrincipal;
                    lblResultado.Text = "Agua";
                    lblDetalleResultado.Text = propio
                        ? "Disparaste a " + coordenada + " y no había nada."
                        : rival + " disparó a " + coordenada + " y falló.";
                    break;
                case BE.ESTADOCELDA.Tocado:
                    lblResultado.ForeColor = Branding.TableroTocado;
                    lblResultado.Text = "¡Tocado!";
                    lblDetalleResultado.Text = propio
                        ? "Disparaste a " + coordenada + " y le diste a una nave."
                        : rival + " disparó a " + coordenada + " y tocó una de tus naves.";
                    break;
                case BE.ESTADOCELDA.Hundido:
                    lblResultado.ForeColor = Branding.TableroHundido;
                    lblResultado.Text = "¡Hundido!";
                    lblDetalleResultado.Text = propio
                        ? "Disparaste a " + coordenada + " y hundiste el " + disparo.NaveHundida.Nombre + " de " + rival + "."
                        : rival + " disparó a " + coordenada + " y hundió tu " + disparo.NaveHundida.Nombre + ".";
                    break;
            }
        }

        // -------------------- Apuntar --------------------

        private void celda_MouseEnter(object sender, EventArgs e)
        {
            ucCelda celda = (ucCelda)sender;
            if (esperandoPase || !partidaBLL.EnCurso)
            {
                celda.Cursor = Cursors.Default;
                return;
            }

            string coordenada = TableroUI.Coordenada(celda.Fila, celda.Columna);
            if (radarActual[celda.Fila, celda.Columna] == BE.ESTADOCELDA.Desconocido)
            {
                celda.Cursor = Cursors.Hand;
                celda.Resaltada = true;
                lblApuntando.Text = "Apuntando a " + coordenada;
            }
            else
            {
                celda.Cursor = Cursors.No;
                lblApuntando.Text = coordenada + ": ya disparada";
            }
        }

        private void celda_MouseLeave(object sender, EventArgs e)
        {
            ((ucCelda)sender).Resaltada = false;
            if (!esperandoPase)
            {
                lblApuntando.Text = "";
            }
        }

        // -------------------- Dibujo --------------------

        // Dibuja los 2 tableros, la flota rival y el registro desde el punto de vista de idVista.
        public void Refrescar()
        {
            int idRival = Otro(idVista);
            List<BE.DISPARO> disparos = partidaBLL.Disparos;
            BE.DISPARO ultimoPropio = disparos.FindLast(d => d.IdAtacante == idVista);
            BE.DISPARO ultimoRival = disparos.FindLast(d => d.IdAtacante == idRival);

            lblHeaderPropio.Text = "Mi flota — " + NombreDe(idVista);
            lblHeaderEnemigo.Text = "Radar enemigo — " + NombreDe(idRival);

            int tam = BLL.PARTIDA.TAMAÑO_TABLERO;

            bool[,] ocupacionPropia = partidaBLL.ObtenerOcupacionPropia(idVista);
            BE.ESTADOCELDA[,] recibidos = partidaBLL.ObtenerDisparosRecibidos(idVista);
            int impactosRecibidos = 0;
            for (int f = 0; f < tam; f++)
            {
                for (int c = 0; c < tam; c++)
                {
                    celdasPropias[f, c].Estado = MapearPropia(ocupacionPropia[f, c], recibidos[f, c]);
                    celdasPropias[f, c].Marcada = EsCelda(ultimoRival, f, c);
                    if (recibidos[f, c] == BE.ESTADOCELDA.Tocado || recibidos[f, c] == BE.ESTADOCELDA.Hundido)
                    {
                        impactosRecibidos++;
                    }
                }
            }

            radarActual = partidaBLL.ObtenerTableroEnemigoFog(idVista);
            for (int f = 0; f < tam; f++)
            {
                for (int c = 0; c < tam; c++)
                {
                    celdasEnemigo[f, c].Estado = Mapear(radarActual[f, c]);
                    celdasEnemigo[f, c].Marcada = EsCelda(ultimoPropio, f, c);
                }
            }

            lblEstadoPropio.Text = partidaBLL.ResumenFlota(idVista) + "  ·  Impactos recibidos: " + impactosRecibidos;

            int disparosPropios = disparos.FindAll(d => d.IdAtacante == idVista).Count;
            int aciertos = disparos.FindAll(d => d.IdAtacante == idVista && d.Resultado != BE.ESTADOCELDA.Agua).Count;
            lblEstadoEnemigo.Text = "Disparos: " + disparosPropios + "  ·  Aciertos: " + aciertos +
                (disparosPropios > 0 ? " (" + (aciertos * 100 / disparosPropios) + "%)" : "");

            RefrescarFlotaRival(idRival);
            RefrescarRegistro(disparos);
        }

        private void RefrescarFlotaRival(int idRival)
        {
            lblFlotaRival.Text = "Flota de " + NombreDe(idRival) + " — " + partidaBLL.ResumenFlota(idRival);
            List<BE.TIPONAVE> hundidas = partidaBLL.NavesHundidas(idRival);
            foreach (BE.TIPONAVE tipo in BE.TIPONAVE.Flota)
            {
                bool hundida = hundidas.Exists(t => t.Id == tipo.Id);
                Label nombre = nombresFlotaRival[tipo.Id];
                Label estado = estadosFlotaRival[tipo.Id];
                nombre.Font = hundida ? fuenteNaveHundida : fuenteNave;
                nombre.ForeColor = hundida ? Branding.Secundario : Branding.TextoPrincipal;
                estado.Text = hundida ? "Hundida" : "A flote";
                estado.ForeColor = hundida ? Branding.TableroHundido : Branding.Secundario;
            }
        }

        // Más reciente arriba. En un tocado no se dice qué nave fue (eso no lo sabe el atacante).
        private void RefrescarRegistro(List<BE.DISPARO> disparos)
        {
            lstRegistro.BeginUpdate();
            lstRegistro.Items.Clear();
            for (int i = disparos.Count - 1; i >= 0; i--)
            {
                BE.DISPARO d = disparos[i];
                string texto;
                switch (d.Resultado)
                {
                    case BE.ESTADOCELDA.Tocado: texto = "¡Tocado!"; break;
                    case BE.ESTADOCELDA.Hundido: texto = "¡Hundido! (" + d.NaveHundida.Nombre + ")"; break;
                    default: texto = "Agua"; break;
                }
                lstRegistro.Items.Add((i + 1) + ". " + NombreDe(d.IdAtacante) + " → " + d.Coordenada + ": " + texto);
            }
            lstRegistro.EndUpdate();
        }

        private static bool EsCelda(BE.DISPARO disparo, int fila, int columna)
        {
            return disparo != null && disparo.Fila == fila && disparo.Columna == columna;
        }

        private int Otro(int idJugador)
        {
            return idJugador == partidaBLL.Jugador1.ID ? partidaBLL.Jugador2.ID : partidaBLL.Jugador1.ID;
        }

        private string NombreDe(int idJugador)
        {
            return idJugador == partidaBLL.Jugador1.ID ? partidaBLL.Jugador1.Nombre : partidaBLL.Jugador2.Nombre;
        }

        private ESTADO_VISUAL_CELDA MapearPropia(bool ocupada, BE.ESTADOCELDA disparo)
        {
            if (disparo == BE.ESTADOCELDA.Tocado) return ESTADO_VISUAL_CELDA.Tocado;
            if (disparo == BE.ESTADOCELDA.Hundido) return ESTADO_VISUAL_CELDA.Hundido;
            if (ocupada) return ESTADO_VISUAL_CELDA.Nave;
            if (disparo == BE.ESTADOCELDA.Agua) return ESTADO_VISUAL_CELDA.Agua;
            return ESTADO_VISUAL_CELDA.Vacia;
        }

        private ESTADO_VISUAL_CELDA Mapear(BE.ESTADOCELDA estado)
        {
            switch (estado)
            {
                case BE.ESTADOCELDA.Agua: return ESTADO_VISUAL_CELDA.Agua;
                case BE.ESTADOCELDA.Tocado: return ESTADO_VISUAL_CELDA.Tocado;
                case BE.ESTADOCELDA.Hundido: return ESTADO_VISUAL_CELDA.Hundido;
                default: return ESTADO_VISUAL_CELDA.Vacia;
            }
        }
    }
}
