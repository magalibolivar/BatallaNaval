namespace BE
{
    // Una fila del XML de movimientos de la partida, ya re-leída para mostrar en
    // una grilla: una colocación de nave, un disparo o el fin de la partida (columna Accion).
    public class MOVIMIENTO
    {
        public string Jugador { get; set; }

        // "Colocación", "Disparo" o "Fin de partida".
        public string Accion { get; set; }

        // Texto libre según la acción: posición/orientación de la nave colocada,
        // coordenada disparada, o el resumen de cierre de la partida.
        public string Detalle { get; set; }

        // "Agua", "Tocado", "Hundido" (solo en disparos) o vacío.
        public string Resultado { get; set; }

        public string Fecha { get; set; }
    }
}
