namespace BE
{
    public class FILAHISTORIAL
    {
        public string Fecha { get; set; }
        public string Rival { get; set; }

        // "Ganada" o "Perdida" (no hay "Empate" posible en Batalla Naval).
        public string Resultado { get; set; }

        // No se muestra en la grilla: se usa para abrir el XML de movimientos de esa partida.
        public string RutaXml { get; set; }
    }
}
