using System;
using System.Windows.Forms;

namespace GUI
{
    public partial class FormReglas : Form
    {
        public FormReglas()
        {
            InitializeComponent();
            txtReglas.Text =
"BATALLA NAVAL - REGLAS\r\n\r\n" +
"1. Cada jugador tiene un tablero propio de 15x15 y coloca 5 naves, una de cada tipo:\r\n" +
"   Batalla Naval (5 casilleros), Portaaviones (4), Fragatas (3), Buque (2), Acorazados (1).\r\n\r\n" +
"2. Las naves se colocan en forma horizontal o vertical. Pueden quedar pegadas entre sí,\r\n" +
"   pero no pueden superponerse ni salirse del tablero.\r\n\r\n" +
"3. Mientras un jugador coloca sus naves, el otro no puede ver su tablero: la aplicación\r\n" +
"   muestra una pantalla para pasar el dispositivo antes de cada colocación.\r\n\r\n" +
"4. Terminada la colocación de los 2 jugadores, empieza el combate. En su turno, un\r\n" +
"   jugador dispara a una casilla del tablero del rival (que se ve como niebla: solo se\r\n" +
"   conocen los disparos ya hechos, nunca una nave no tocada).\r\n\r\n" +
"5. Cada disparo informa el resultado: Agua (no había nada), Tocado (una nave recibió un\r\n" +
"   impacto) u Hundido (ese impacto hundió la nave completa).\r\n\r\n" +
"6. El turno pasa siempre al otro jugador después de cada disparo, haya sido agua, tocado\r\n" +
"   o hundido.\r\n\r\n" +
"7. Gana el primer jugador que hunde las 5 naves de su rival. También se puede abandonar\r\n" +
"   la partida en cualquier momento: la gana el otro jugador.\r\n\r\n" +
"Las decisiones de diseño sobre puntos que este enunciado no especifica (cantidad de\r\n" +
"naves por tipo, información oculta, turno tras un acierto, etc.) están documentadas en\r\n" +
"Documentacion/Reglas_de_Negocio_BatallaNaval.md.";
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
