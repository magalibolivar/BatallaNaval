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
"1. Cada jugador tiene un tablero propio de 15x15 y coloca 5 naves, una de cada tipo: " +
"Batalla Naval (5 casilleros), Portaaviones (4), Fragatas (3), Buque (2) y Acorazados (1).\r\n\r\n" +
"2. Las naves se colocan en forma horizontal o vertical. Pueden quedar pegadas entre sí, " +
"pero no pueden superponerse ni salirse del tablero. Al apuntar en el tablero se ve dónde " +
"quedaría la nave elegida; la orientación se cambia con \"Girar\", la tecla R o click derecho. " +
"Antes de confirmar la flota se puede corregir cada nave: arrastrándola se mueve, con un click " +
"se gira y con click derecho se saca. \"Al azar\" coloca las que faltan (o, con la flota " +
"completa, la redistribuye entera) y \"Reiniciar\" saca todas. \"Confirmar flota\" cierra la colocación.\r\n\r\n" +
"3. Ningún jugador ve el tablero del otro: la aplicación muestra una pantalla para pasar " +
"el dispositivo antes de cada colocación y en cada cambio de turno del combate.\r\n\r\n" +
"4. Cuando los 2 jugadores confirmaron su flota, empieza el combate. En su turno, un jugador " +
"dispara a una casilla del tablero del rival (que se ve como niebla: solo se conocen los " +
"disparos ya hechos, nunca una nave no tocada).\r\n\r\n" +
"5. Cada disparo informa el resultado: Agua (no había nada), Tocado (una nave recibió un " +
"impacto) u Hundido (ese impacto hundió la nave completa; se anuncia cuál fue).\r\n\r\n" +
"6. El turno pasa siempre al otro jugador después de cada disparo, haya sido agua, tocado " +
"o hundido. Después de ver el resultado, el jugador presiona \"Pasar turno\".\r\n\r\n" +
"7. Gana el primer jugador que hunde las 5 naves de su rival. También se puede abandonar " +
"la partida en cualquier momento: la gana el otro jugador.\r\n\r\n" +
"Las decisiones de diseño sobre puntos que este enunciado no especifica (cantidad de " +
"naves por tipo, información oculta, turno tras un acierto, etc.) están documentadas en " +
"Documentacion/Reglas_de_Negocio_BatallaNaval.md.";
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
