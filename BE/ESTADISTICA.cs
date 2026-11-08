using System;
using System.Collections.Generic;

namespace BE
{
    // Resultado de las estadísticas de un jugador: lo calcula BLL.ESTADISTICA
    // y FormEstadisticas solo lo muestra.
    public class ESTADISTICA
    {
        public int Ganadas { get; set; }
        public int Perdidas { get; set; }

        // En Batalla Naval un empate es estructuralmente imposible (ver Reglas de Negocio,
        // sección de empates), así que este contador siempre va a dar 0. Se mantiene el
        // campo porque el enunciado general de la materia pide mostrar partidas empatadas.
        public int Empatadas { get; set; }

        // Porcentaje de partidas ganadas sobre el total (0 a 100).
        public double PromedioVictorias { get; set; }

        public TimeSpan TiempoTotal { get; set; }
        public List<FILAHISTORIAL> Historial { get; set; }
    }
}
