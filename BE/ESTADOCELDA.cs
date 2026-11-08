namespace BE
{
    // Estado de una celda del tablero enemigo tal como lo ve el jugador atacante
    // (nunca revela una nave no tocada - ver Reglas de Negocio, "Información oculta").
    public enum ESTADOCELDA
    {
        Desconocido,
        Agua,
        Tocado,
        Hundido
    }
}
