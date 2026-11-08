using System;
using System.Collections.Generic;

namespace BLL
{
    // Estadísticas de un jugador a partir de sus partidas terminadas.
    public class ESTADISTICA
    {
        private DAL.MP_PARTIDA mapperPartida = new DAL.MP_PARTIDA();
        private DAL.MP_USUARIO mapperUsuario = new DAL.MP_USUARIO();

        public BE.ESTADISTICA Calcular(BE.USUARIO usuario)
        {
            List<BE.PARTIDA> partidas = mapperPartida.ListarPorUsuario(usuario.ID);

            BE.ESTADISTICA estadistica = new BE.ESTADISTICA();
            estadistica.TiempoTotal = TimeSpan.Zero;
            estadistica.Historial = new List<BE.FILAHISTORIAL>();

            foreach (BE.PARTIDA p in partidas)
            {
                // El usuario pudo haber sido el jugador 1 o el 2 de esa partida.
                int idRival = p.IdJugador1 == usuario.ID ? p.IdJugador2 : p.IdJugador1;

                // En Batalla Naval siempre hay un ganador: IdGanador nunca es null en una
                // partida finalizada (ver CK_PARTIDA_GANADOR en Script.sql). No existe "Empate".
                string resultado;
                if (p.IdGanador == usuario.ID)
                {
                    resultado = "Ganada";
                    estadistica.Ganadas++;
                }
                else
                {
                    resultado = "Perdida";
                    estadistica.Perdidas++;
                }

                estadistica.TiempoTotal = estadistica.TiempoTotal + (p.FechaFin.Value - p.FechaInicio);

                BE.FILAHISTORIAL fila = new BE.FILAHISTORIAL();
                fila.Fecha = p.FechaInicio.ToString("dd/MM/yyyy HH:mm");
                BE.USUARIO rival = mapperUsuario.BuscarPorId(idRival);
                fila.Rival = rival != null ? rival.Nombre : "-";
                fila.Resultado = resultado;
                fila.RutaXml = p.RutaXml;
                estadistica.Historial.Add(fila);
            }

            // Promedio de victorias: ganadas sobre el total de partidas, en porcentaje.
            estadistica.PromedioVictorias = partidas.Count > 0 ? estadistica.Ganadas * 100.0 / partidas.Count : 0;

            return estadistica;
        }
    }
}
