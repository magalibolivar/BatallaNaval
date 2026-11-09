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

        private DAL.MP_PARTIDA mapper;

        public PARTIDA() : this(new DAL.MP_PARTIDA())
        {
        }

        // Los tests pasan un mapper falso para probar el motor sin base de datos.
        internal PARTIDA(DAL.MP_PARTIDA mapper)
        {
            this.mapper = mapper;
        }

        private BE.PARTIDA partida;
        private BE.USUARIO jugador1;
        private BE.USUARIO jugador2;
        private Dictionary<int, TableroJugador> tableros;
        private FASE_PARTIDA fase = FASE_PARTIDA.Finalizada;
        private int jugadorEnTurno;
        private List<BE.DISPARO> disparos = new List<BE.DISPARO>();

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
            // Si se iniciara otra encima, la anterior quedaría en la base sin terminar nunca.
            if (EnCurso)
            {
                throw new Exception("Ya hay una partida en curso.");
            }
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
            // La base guarda su propio GETDATE(); este valor es el de la partida en memoria
            // (lo usa FormResultado para la duración) y difiere del de la base en milisegundos.
            nueva.FechaInicio = DateTime.Now;

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
            disparos = new List<BE.DISPARO>();

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

        // true cuando el jugador ya tiene las 5 naves en el tablero y puede confirmar su flota.
        public bool FlotaCompleta(int idJugador)
        {
            return tableros[idJugador].Naves.Count == BE.TIPONAVE.Flota.Length;
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

            BE.NAVECOLOCADA nave = NuevaNave(tipo, fila, columna, orientacion);
            ValidarPosicion(tablero, nave);

            tablero.Naves.Add(nave);
            RecalcularOcupacion(tablero);
        }

        // Cambia la posición y/o la orientación de una nave ya colocada (arrastrarla o
        // girarla en la pantalla de colocación). Solo se puede mientras el jugador no
        // confirmó su flota; la nave no choca consigo misma, solo con las demás.
        public void MoverNave(int idJugador, BE.TIPONAVE tipo, int fila, int columna, BE.ORIENTACION orientacion)
        {
            if (!PuedeColocar(idJugador))
            {
                throw new Exception("No es el turno de colocación de este jugador.");
            }

            TableroJugador tablero = tableros[idJugador];
            BE.NAVECOLOCADA existente = tablero.Naves.Find(n => n.Tipo.Id == tipo.Id);
            if (existente == null)
            {
                throw new Exception("Esa nave todavía no está colocada.");
            }

            ValidarPosicion(tablero, NuevaNave(tipo, fila, columna, orientacion));

            existente.Fila = fila;
            existente.Columna = columna;
            existente.Orientacion = orientacion;
            RecalcularOcupacion(tablero);
        }

        // Saca una nave ya colocada del tablero (vuelve a quedar pendiente). Solo mientras
        // el jugador no confirmó su flota.
        public void QuitarNave(int idJugador, BE.TIPONAVE tipo)
        {
            if (!PuedeColocar(idJugador))
            {
                throw new Exception("No es el turno de colocación de este jugador.");
            }

            TableroJugador tablero = tableros[idJugador];
            if (tablero.Naves.RemoveAll(n => n.Tipo.Id == tipo.Id) == 0)
            {
                throw new Exception("Esa nave todavía no está colocada.");
            }
            RecalcularOcupacion(tablero);
        }

        // Saca todas las naves del jugador para empezar la colocación de nuevo.
        public void ReiniciarColocacion(int idJugador)
        {
            if (!PuedeColocar(idJugador))
            {
                throw new Exception("No es el turno de colocación de este jugador.");
            }

            TableroJugador tablero = tableros[idJugador];
            tablero.Naves.Clear();
            RecalcularOcupacion(tablero);
        }

        public void ColocarAlAzar(int idJugador)
        {
            ColocarAlAzar(idJugador, new Random());
        }

        // Coloca en posiciones al azar las naves que al jugador le faltan (las que ya puso
        // quedan donde están). Se elige entre todas las posiciones válidas, de la nave más
        // grande a la más chica, así nunca hace falta "reintentar" hasta que una entre.
        // No confirma la flota: el jugador todavía puede corregirla.
        public void ColocarAlAzar(int idJugador, Random azar)
        {
            if (!PuedeColocar(idJugador))
            {
                throw new Exception("No es el turno de colocación de este jugador.");
            }

            TableroJugador tablero = tableros[idJugador];
            List<BE.TIPONAVE> pendientes = FlotaPendiente(idJugador);
            pendientes.Sort((a, b) => b.Tamaño.CompareTo(a.Tamaño));

            foreach (BE.TIPONAVE tipo in pendientes)
            {
                List<BE.NAVECOLOCADA> posibles = new List<BE.NAVECOLOCADA>();
                foreach (BE.ORIENTACION orientacion in new[] { BE.ORIENTACION.Horizontal, BE.ORIENTACION.Vertical })
                {
                    for (int f = 0; f < TAMAÑO_TABLERO; f++)
                    {
                        for (int c = 0; c < TAMAÑO_TABLERO; c++)
                        {
                            if (PosicionValida(idJugador, tipo, f, c, orientacion))
                            {
                                posibles.Add(NuevaNave(tipo, f, c, orientacion));
                            }
                        }
                    }
                }

                // Con 15 casilleros de naves en un tablero de 225 siempre hay lugar; se
                // controla igual para no elegir de una lista vacía.
                if (posibles.Count == 0)
                {
                    throw new Exception("No hay lugar en el tablero para la nave " + tipo.Nombre + ".");
                }

                tablero.Naves.Add(posibles[azar.Next(posibles.Count)]);
                RecalcularOcupacion(tablero);
            }
        }

        // Sin modificar nada, indica si la nave entraría en esa posición (la GUI lo usa
        // para mostrar si el lugar es válido mientras se arrastra).
        // No usa excepciones: se llama muchas veces seguidas (en cada movimiento del mouse y
        // cientos de veces en "Al azar"), y lanzar/atrapar una excepción por cada posición
        // inválida lo hacía lento y llenaba el depurador de excepciones de BLL.dll.
        public bool PosicionValida(int idJugador, BE.TIPONAVE tipo, int fila, int columna, BE.ORIENTACION orientacion)
        {
            return ErrorDePosicion(tableros[idJugador], NuevaNave(tipo, fila, columna, orientacion)) == null;
        }

        // Cierra la colocación del jugador: recién acá se graban en el XML las posiciones
        // finales de sus 5 naves y se pasa al otro jugador (o al combate). Ver
        // Reglas_de_Negocio_BatallaNaval.md, "Corrección de la colocación".
        public void ConfirmarColocacion(int idJugador)
        {
            if (!PuedeColocar(idJugador))
            {
                throw new Exception("No es el turno de colocación de este jugador.");
            }
            if (!FlotaCompleta(idJugador))
            {
                throw new Exception("Todavía faltan colocar naves.");
            }

            // Se escribe el XML antes de cambiar de fase: si falla, la colocación sigue abierta.
            string nombre = UsuarioDe(idJugador).Nombre;
            foreach (BE.NAVECOLOCADA nave in tableros[idJugador].Naves)
            {
                BLL.MOVIMIENTOXML.RegistrarColocacion(partida.RutaXml, nombre, nave.Tipo, nave.Fila, nave.Columna, nave.Orientacion);
            }

            if (fase == FASE_PARTIDA.ColocandoJugador1)
            {
                fase = FASE_PARTIDA.ColocandoJugador2;
            }
            else
            {
                fase = FASE_PARTIDA.Combate;
                jugadorEnTurno = jugador1.ID;
            }
        }

        private static BE.NAVECOLOCADA NuevaNave(BE.TIPONAVE tipo, int fila, int columna, BE.ORIENTACION orientacion)
        {
            BE.NAVECOLOCADA nave = new BE.NAVECOLOCADA();
            nave.Tipo = tipo;
            nave.Fila = fila;
            nave.Columna = columna;
            nave.Orientacion = orientacion;
            return nave;
        }

        // Valida contra las demás naves del tablero; la nave del mismo tipo (si ya estaba
        // colocada) se ignora, así puede moverse o girar sobre sus propias celdas.
        private static void ValidarPosicion(TableroJugador tablero, BE.NAVECOLOCADA nave)
        {
            string error = ErrorDePosicion(tablero, nave);
            if (error != null)
            {
                throw new Exception(error);
            }
        }

        // Motivo por el que la nave no puede ir ahí, o null si la posición es válida.
        private static string ErrorDePosicion(TableroJugador tablero, BE.NAVECOLOCADA nave)
        {
            List<Tuple<int, int>> celdasNave = nave.Celdas();
            foreach (Tuple<int, int> celda in celdasNave)
            {
                if (celda.Item1 < 0 || celda.Item1 >= TAMAÑO_TABLERO || celda.Item2 < 0 || celda.Item2 >= TAMAÑO_TABLERO)
                {
                    return "La nave no entra en el tablero.";
                }
            }
            foreach (BE.NAVECOLOCADA otra in tablero.Naves)
            {
                if (otra.Tipo.Id == nave.Tipo.Id)
                {
                    continue;
                }
                // Se permite que las naves queden adyacentes: solo se prohíbe que se superpongan
                // (ver Reglas_de_Negocio_BatallaNaval.md, "Colocación").
                foreach (Tuple<int, int> celda in otra.Celdas())
                {
                    if (celdasNave.Contains(celda))
                    {
                        return "Esa nave se superpone con otra ya colocada.";
                    }
                }
            }
            return null;
        }

        private static void RecalcularOcupacion(TableroJugador tablero)
        {
            tablero.Ocupacion = new bool[TAMAÑO_TABLERO, TAMAÑO_TABLERO];
            foreach (BE.NAVECOLOCADA nave in tablero.Naves)
            {
                foreach (Tuple<int, int> celda in nave.Celdas())
                {
                    tablero.Ocupacion[celda.Item1, celda.Item2] = true;
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
            return ObtenerDisparosRecibidos(Otro(idJugadorQueMira));
        }

        // Tablero propio durante el combate: a diferencia del tablero enemigo, acá sí se
        // puede mostrar dónde están las naves, porque el jugador ya las colocó él mismo
        // (no hay nada que ocultarle a sí mismo).
        public bool[,] ObtenerOcupacionPropia(int idJugador)
        {
            return (bool[,])tableros[idJugador].Ocupacion.Clone();
        }

        public BE.ESTADOCELDA[,] ObtenerDisparosRecibidos(int idJugador)
        {
            return (BE.ESTADOCELDA[,])tableros[idJugador].DisparosRecibidos.Clone();
        }

        public string ResumenFlota(int idJugador)
        {
            List<BE.NAVECOLOCADA> naves = tableros[idJugador].Naves;
            int aFlote = naves.FindAll(n => !n.Hundida).Count;
            return aFlote + "/" + naves.Count + " naves a flote";
        }

        // Disparos de los 2 jugadores en el orden en que se hicieron (copia: la GUI no puede modificarla).
        public List<BE.DISPARO> Disparos
        {
            get { return new List<BE.DISPARO>(disparos); }
        }

        // Último disparo de la partida (de cualquiera de los 2), o null si todavía no hubo ninguno.
        public BE.DISPARO UltimoDisparo
        {
            get { return disparos.Count == 0 ? null : disparos[disparos.Count - 1]; }
        }

        // Tipos de nave de este jugador que ya fueron hundidos. No revela posiciones: el
        // rival ya sabe qué naves hundió, porque cada hundimiento se anuncia con su nombre.
        public List<BE.TIPONAVE> NavesHundidas(int idJugador)
        {
            List<BE.TIPONAVE> hundidas = new List<BE.TIPONAVE>();
            foreach (BE.NAVECOLOCADA nave in tableros[idJugador].Naves)
            {
                if (nave.Hundida)
                {
                    hundidas.Add(nave.Tipo);
                }
            }
            return hundidas;
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
            }

            if (resultado == BE.ESTADOCELDA.Hundido)
            {
                // Recién al hundirse se marca la nave entera (todas sus celdas ya estaban tocadas).
                foreach (Tuple<int, int> celda in naveImpactada.Celdas())
                {
                    tableroDefensor.DisparosRecibidos[celda.Item1, celda.Item2] = BE.ESTADOCELDA.Hundido;
                }
            }
            else
            {
                // Agua o Tocado: solo la celda disparada. Marcar toda la nave como tocada
                // revelaría su posición en la niebla y bloquearía el resto de sus celdas
                // ("ya fue disparada"), así que nunca se podría hundir.
                tableroDefensor.DisparosRecibidos[fila, columna] = resultado;
            }

            BE.DISPARO disparo = new BE.DISPARO();
            disparo.IdAtacante = idJugadorAtacante;
            disparo.Fila = fila;
            disparo.Columna = columna;
            disparo.Resultado = resultado;
            disparo.NaveHundida = resultado == BE.ESTADOCELDA.Hundido ? naveImpactada.Tipo : null;
            disparos.Add(disparo);

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
            if (!EnCurso)
            {
                throw new Exception("No hay ninguna partida en curso.");
            }
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
            // Sin esto FechaFin quedaba null en memoria y FormResultado fallaba al terminar
            // cualquier partida (la fecha solo se grababa en la base, con GETDATE()).
            partida.FechaFin = DateTime.Now;

            BLL.MOVIMIENTOXML.RegistrarFin(partida.RutaXml, UsuarioDe(idGanador).Nombre, descripcion);

            if (PartidaFinalizada != null)
            {
                PartidaFinalizada(partida, abandonada);
            }
        }
    }
}
