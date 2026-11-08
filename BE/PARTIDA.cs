using System;

namespace BE
{
    // Resumen persistido de una partida. El detalle (colocación de naves, cada disparo)
    // vive solo en el XML de RutaXml, no en esta tabla (ver Reglas_de_Negocio_BatallaNaval.md).
    public class PARTIDA
    {
        public int ID { get; set; }
        public int IdJugador1 { get; set; }
        public int IdJugador2 { get; set; }

        // Null mientras la partida está en curso. Una vez que FechaFin tiene valor,
        // siempre hay un ganador: en Batalla Naval no existen los empates.
        public int? IdGanador { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string RutaXml { get; set; }
    }
}
