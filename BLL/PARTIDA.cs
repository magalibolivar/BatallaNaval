using System;
using System.Collections.Generic;

namespace BLL
{
    public enum FASE_PARTIDA
    {
        ColocandoJugador1,
        ColocandoJugador2,
        Combate,
        Finalizada
    }

    // Motor de una partida de Batalla Naval. Todo el estado (fase, tableros, flotas,
    // turno) lo maneja esta clase; la GUI solo lo lee y le avisa lo que hace el usuario,
    // igual que BLL.PARTIDA en el TP de Generala.
    public class PARTIDA
    {
        public const int TAMAÑO_TABLERO = 15;

        private class TableroJugador
        {
            public bool[,] Ocupacion = new bool[TAMAÑO_TABLERO, TAMAÑO_TABLERO];
            public BE.ESTADOCELDA[,] DisparosRecibidos = new BE.ESTADOCELDA[TAMAÑO_TABLERO, TAMAÑO_TABLERO];
            public List<BE.NAVECOLOCADA> Naves = new List<BE.NAVECOLOCADA>();
        }

        private DAL.MP_PARTIDA mapper = new DAL.MP_PARTIDA();

        private BE.PARTIDA partida;
        private BE.USUARIO jugador1;
        private BE.USUARIO jugador2;
        private Dictionary<int, TableroJugador> tableros;
        private FASE_PARTIDA fase = FASE_PARTIDA.Finalizada;
        private int jugadorEnTurno;

        public event delPartidaFinalizada PartidaFinalizada;

        public BE.PARTIDA Partida { get { return partida; } }
        public BE.USUARIO Jugador1 { get { return jugador1; } }
        public BE.USUARIO Jugador2 { get { return jugador2; } }
        public FASE_PARTIDA Fase { get { return fase; } }
        public bool EnCurso { get { return fase != FASE_PARTIDA.Finalizada; } }
        public int JugadorEnTurno { get { return jugadorEnTurno; } }

        // ID del jugador que todavía tiene que colocar sus naves, o 0 si ya empezó el combate.
        public int IdJugadorColocando
        {
            get
            {
                if (fase == FASE_PARTIDA.ColocandoJugador1) return jugador1.ID;
                if (fase == FASE_PARTIDA.ColocandoJugador2) return jugador2.ID;
                return 0;
            }
        }

        private int Otro(int idJugador)
        {
            return idJugador == jugador1.ID ? jugador2.ID : jugador1.ID;
        }

        private BE.USUARIO UsuarioDe(int idJugador)
        {
            return idJugador == jugador1.ID ? jugador1 : jugador2;
        }

        public void Iniciar(BE.USUARIO j1, BE.USUARIO j2)
        {
            if (j1.ID == j2.ID)
            {
                throw new Exception("Los dos jugadores tienen que ser distintos.");
            }

            jugador1 = j1;
            jugador2 = j2;

            BE.PARTIDA nueva = new BE.PARTIDA();
            nueva.IdJugador1 = j1.ID;
            nueva.IdJugador2 = j2.ID;
            nueva.RutaXml = BLL.MOVIMIENTOXML.GenerarRuta();

            int id = mapper.Insertar(nueva);
            if (id == -1)
            {
                throw new Exception("No se pudo crear la partida en la base de datos. " +
                    "Si restauraste un backup, puede que alguno de los 2 usuarios ya no exista.");
            }
            nueva.ID = id;
            partida = nueva;

            BLL.MOVIMIENTOXML.CrearArchivo(partida.RutaXml, j1.Nombre, j2.Nombre);

            tableros = new Dictionary<int, TableroJugador>();
            tableros[j1.ID] = new TableroJugador();
            tableros[j2.ID] = new TableroJugador();

            fase = FASE_PARTIDA.ColocandoJugador1;
        }

        // -------------------- Colocación de naves --------------------

        public bool PuedeColocar(int idJugador)
        {
            return (fase == FASE_PARTIDA.ColocandoJugador1 && idJugador == jugador1.ID)
                || (fase == FASE_PARTIDA.ColocandoJugador2 && idJugador == jugador2.ID);
        }

        // Tipos de nave que a este jugador todavía le faltan colocar.
        public List<BE.TIPONAVE> FlotaPendiente(int idJugador)
        {
            TableroJugador tablero = tableros[idJugador];
            List<BE.TIPONAVE> pendiente = new List<BE.TIPONAVE>();
            foreach (BE.TIPONAVE tipo in BE.TIPONAVE.Flota)
            {
                if (!tablero.Naves.Exists(n => n.Tipo.Id == tipo.Id))
                {
                    pendiente.Add(tipo);
                }
            }
            return pendiente;
        }

        public List<BE.NAVECOLOCADA> NavesDe(int idJugador)
        {
            return tableros[idJugador].Naves;
        }

        public void ColocarNave(int idJugador, BE.TIPONAVE tipo, int fila, int columna, BE.ORIENTACION orientacion)
        {
            if (!PuedeColocar(idJugador))
            {
                throw new Exception("No es el turno de colocación de este jugador.");
            }

            TableroJugador tablero = tableros[idJugador];
            if (tablero.Naves.Exists(n => n.Tipo.Id == tipo.Id))
            {
                throw new Exception("Esa nave ya está colocada.");
            }

            BE.NAVECOLOCADA nave = new BE.NAVECOLOCADA();
            nave.Tipo = tipo;
            nave.Fila = fila;
            nave.Columna = columna;
            nave.Orientacion = orientacion;

            List<Tuple<int, int>> celdas = nave.Celdas();
            foreach (Tuple<int, int> celda in celdas)
            {
                if (celda.Item1 < 0 || celda.Item1 >= TAMAÑO_TABLERO || celda.Item2 < 0 || celda.Item2 >= TAMAÑO_TABLERO)
                {
                    throw new Exception("La nave no entra en el tablero.");
                }
                // Se permite que las naves queden adyacentes: solo se prohíbe que se superpongan
                // (ver Reglas_de_Negocio_BatallaNaval.md, "Colocación").
                if (tablero.Ocupacion[celda.Item1, celda.Item2])
                {
                    throw new Exception("Esa nave se superpone con otra ya colocada.");
                }
            }

            // Se escribe el XML antes de mutar el estado en memoria: si falla, la colocación no cuenta.
            BLL.MOVIMIENTOXML.RegistrarColocacion(partida.RutaXml, UsuarioDe(idJugador).Nombre, tipo, fila, columna, orientacion);

            foreach (Tuple<int, int> celda in celdas)
            {
                tablero.Ocupacion[celda.Item1, celda.Item2] = true;
            }
            tablero.Naves.Add(nave);

            if (tablero.Naves.Count == BE.TIPONAVE.Flota.Length)
            {
                if (fase == FASE_PARTIDA.ColocandoJugador1)
                {
                    fase = FASE_PARTIDA.ColocandoJugador2;
                }
                else if (fase == FASE_PARTIDA.ColocandoJugador2)
                {
                    fase = FASE_PARTIDA.Combate;
                    jugadorEnTurno = jugador1.ID;
                }
            }
        }

        // -------------------- Combate --------------------

        public bool PuedeDisparar(int idJugador)
        {
            return fase == FASE_PARTIDA.Combate && jugadorEnTurno == idJugador;
        }

        // Tablero enemigo tal como lo ve idJugadorQueMira: nunca expone una nave no tocada.
        public BE.ESTADOCELDA[,] ObtenerTableroEnemigoFog(int idJugadorQueMira)
        {
            return tableros[Otro(idJugadorQueMira)].DisparosRecibidos;
        }

        public string ResumenFlota(int idJugador)
        {
            List<BE.NAVECOLOCADA> naves = tableros[idJugador].Naves;
            int aFlote = naves.FindAll(n => !n.Hundida).Count;
            return aFlote + "/" + naves.Count + " naves a flote";
        }

        public BE.ESTADOCELDA Disparar(int idJugadorAtacante, int fila, int columna)
        {
            if (!PuedeDisparar(idJugadorAtacante))
            {
                throw new Exception("No es el turno de este jugador.");
            }
            if (fila < 0 || fila >= TAMAÑO_TABLERO || columna < 0 || columna >= TAMAÑO_TABLERO)
            {
                throw new Exception("Esa posición no existe en el tablero.");
            }

            int idDefensor = Otro(idJugadorAtacante);
            TableroJugador tableroDefensor = tableros[idDefensor];

            if (tableroDefensor.DisparosRecibidos[fila, columna] != BE.ESTADOCELDA.Desconocido)
            {
                throw new Exception("Esa casilla ya fue disparada.");
            }

            BE.NAVECOLOCADA naveImpactada = null;
            if (tableroDefensor.Ocupacion[fila, columna])
            {
                naveImpactada = tableroDefensor.Naves.Find(n => n.Celdas().Exists(c => c.Item1 == fila && c.Item2 == columna));
            }

            BE.ESTADOCELDA resultado;
            if (naveImpactada == null)
            {
                resultado = BE.ESTADOCELDA.Agua;
            }
            else
            {
                resultado = naveImpactada.CantidadImpactos + 1 >= naveImpactada.Tipo.Tamaño
                    ? BE.ESTADOCELDA.Hundido
                    : BE.ESTADOCELDA.Tocado;
            }

            // Se escribe el XML antes de mutar el estado en memoria: si falla, el disparo no cuenta.
            BLL.MOVIMIENTOXML.RegistrarDisparo(partida.RutaXml, UsuarioDe(idJugadorAtacante).Nombre, fila, columna,
                resultado, resultado == BE.ESTADOCELDA.Agua ? null : naveImpactada.Tipo);

            if (naveImpactada != null)
            {
                naveImpactada.CantidadImpactos++;
                foreach (Tuple<int, int> celda in naveImpactada.Celdas())
                {
                    tableroDefensor.DisparosRecibidos[celda.Item1, celda.Item2] = resultado;
                }
            }
            else
            {
                tableroDefensor.DisparosRecibidos[fila, columna] = resultado;
            }

            if (resultado == BE.ESTADOCELDA.Hundido && tableroDefensor.Naves.TrueForAll(n => n.Hundida))
            {
                Finalizar(idJugadorAtacante, "Victoria por hundimiento total de la flota rival", false);
            }
            else
            {
                // El turno siempre alterna, haya sido agua, tocado o hundido: no hay disparo
                // extra por acierto (ver Reglas_de_Negocio_BatallaNaval.md, "Turno en combate").
                jugadorEnTurno = idDefensor;
            }

            return resultado;
        }

        public void Abandonar(int idJugadorQueAbandona)
        {
            if (idJugadorQueAbandona != jugador1.ID && idJugadorQueAbandona != jugador2.ID)
            {
                throw new Exception("Ese jugador no participa de esta partida.");
            }

            int idGanador = Otro(idJugadorQueAbandona);
            string nombreAbandona = UsuarioDe(idJugadorQueAbandona).Nombre;
            Finalizar(idGanador, "Partida abandonada por " + nombreAbandona, true);
        }

        private void Finalizar(int idGanador, string descripcion, bool abandonada)
        {
            partida.IdGanador = idGanador;

            bool ok = mapper.Finalizar(partida, descripcion);
            if (!ok)
            {
                throw new Exception("No se pudo registrar el fin de la partida en la base de datos.");
            }

            fase = FASE_PARTIDA.Finalizada;

            BLL.MOVIMIENTOXML.RegistrarFin(partida.RutaXml, UsuarioDe(idGanador).Nombre, descripcion);

            if (PartidaFinalizada != null)
            {
                PartidaFinalizada(partida, abandonada);
            }
        }
    }
}
