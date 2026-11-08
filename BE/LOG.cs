namespace BE
{
    // Un registro de la bitácora (tabla LOG).
    public class LOG
    {
        // Tipos de evento: coinciden con los ID de la tabla TIPO_LOG.
        public const int INICIO_SESION = 1;
        public const int CIERRE_SESION = 2;
        public const int INICIO_PARTIDA = 3;
        public const int FIN_PARTIDA = 4;

        public int IdUsuario { get; set; }
        public int IdTipo { get; set; }
        public string Descripcion { get; set; }
    }
}
