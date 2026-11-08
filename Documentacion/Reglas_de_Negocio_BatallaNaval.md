# Reglas de negocio — TP Batalla Naval

Reglas que implementa la aplicación. Donde la consigna no especifica algo, la decisión tomada y su motivo están en la sección 7.

## 1. Modalidad de juego

- **2 jugadores humanos, en la misma PC, en la misma ventana** (Jugador 1 y Jugador 2 se turnan frente al mismo teclado/mouse). No hay IA ni juego contra la máquina, y no hay red/multijugador remoto.
- **Ambos jugadores tienen que ser usuarios registrados y logueados por separado.** Jugador 1 es quien inició sesión al abrir la app (`FormLogin`). Jugador 2 se loguea desde `FormPrincipal` con el botón "Ingresar Jugador 2", reutilizando la misma pantalla de login — tiene que ser un usuario **distinto** al Jugador 1.
- Una vez logueado el Jugador 2, se puede arrancar la partida ("Comenzar Partida"), que abre una ventana (`FormPartida`) dedicada a esa partida: colocación de naves de ambos jugadores y combate completo.

## 2. Flota

- Cada jugador tiene un tablero propio de **15x15** y coloca exactamente **5 naves, una de cada tipo** (los 5 tipos que da la consigna, con la cantidad de casilleros que ocupa cada una):

  | Nave | Casilleros |
  |---|---|
  | Batalla Naval | 5 |
  | Portaaviones | 4 |
  | Fragatas | 3 |
  | Buque | 2 |
  | Acorazados | 1 |

- Las naves se colocan en forma horizontal o vertical, eligiendo la celda donde empiezan. Se permite que dos naves queden **adyacentes** (pegadas); solo se prohíbe que se **superpongan** o que alguna celda quede fuera del tablero.

## 3. Información oculta durante la colocación

- Mientras un jugador coloca sus naves, el otro **no puede ver ese tablero**. Antes de que cada jugador empiece a colocar, la aplicación muestra una pantalla neutra ("Jugador X: coloca tus naves — pasá el dispositivo y presioná Continuar") y solo después de confirmarla se dibuja el tablero de ese jugador con sus naves.
- Terminada la colocación de los 2 jugadores (10 naves en total), empieza automáticamente el combate.

## 4. Mecánica de combate

1. En su turno, un jugador dispara a una celda del tablero del rival, elegida sobre una grilla que muestra **niebla de guerra**: solo las celdas ya disparadas (agua, tocado o hundido); una nave que todavía no recibió ningún disparo nunca se muestra.
2. No se puede disparar dos veces a la misma celda.
3. El resultado de cada disparo es uno de tres: **Agua** (la celda estaba vacía), **Tocado** (había una nave, pero todavía le quedan celdas sin tocar) o **Hundido** (ese disparo completó todas las celdas de esa nave).
4. El turno **siempre pasa al otro jugador** después de cada disparo, sea agua, tocado o hundido: no hay disparo extra por acertar.
5. La partida termina cuando un jugador hunde las **5 naves** de su rival; gana ese jugador. También se puede abandonar en cualquier momento (botón "Abandonar partida"): la gana siempre el otro jugador. En la bitácora queda "Partida abandonada por" y el nombre de quien abandonó.

## 5. Bitácora y estadísticas

- Se registra en la bitácora: inicio de sesión, cierre de sesión, inicio de partida y fin de partida (de ambos jugadores en cada evento de partida).
- Todos los movimientos de cada partida se graban en un archivo XML propio de esa partida: cada **colocación** de nave (jugador, nave, posición y orientación), cada **disparo** (jugador, celda, resultado y, si corresponde, qué nave fue tocada/hundida) y el **cierre** (ganador y si fue por abandono), todos con fecha y hora. Se consultan desde Estadísticas → "Ver movimientos".
- Las estadísticas por jugador muestran: partidas ganadas / perdidas / empatadas, porcentaje de victorias, y tiempo total acumulado jugado (suma de la duración de todas sus partidas terminadas).

## 6. Empates

- En Batalla Naval **no existen los empates**: la partida termina siempre con un ganador, ya sea porque hundió toda la flota rival o porque el rival abandonó. El contador de "Empatadas" en las estadísticas se muestra igual (porque el enunciado general de la materia lo pide para todos los TP), pero siempre va a dar 0.

## 7. Decisiones de diseño

La consigna pide "Batalla Naval para 2 jugadores" con turnos, tablero de 15x15, naves con 5 tamaños distintos y el resultado por casillero (agua/tocado/hundido), sin más detalle. Estas son las decisiones tomadas donde no lo especifica:

- **Los 2 jugadores juegan en la misma PC.** Igual que en la consigna general de la materia, no se pide red, y jugar en red agregaría un tema (comunicación entre equipos) fuera del alcance de la materia. Cada jugador igual se identifica con su propio usuario, así la bitácora y las estadísticas quedan a nombre de cada uno.
- **Información oculta en una sola pantalla compartida.** A diferencia de un juego sin información secreta, en Batalla Naval un jugador no puede ver dónde el rival puso sus naves. La regla de diseño que resuelve esto: **el tablero propio (con naves) solo se dibuja durante la colocación de ese mismo jugador**; durante el combate, lo único que se muestra es el tablero rival en modo niebla, que por construcción nunca revela una nave no tocada. Por eso solo hacen falta 2 pantallas de "paso de dispositivo" (antes de que cada jugador coloque sus naves) y ninguna durante el combate.
- **Una nave de cada tipo por jugador (5 naves, 15 casilleros de 225).** La consigna lista 5 tipos de nave con su tamaño, pero no dice cuántas de cada una. Se tomó la lectura literal (una de cada una) para acotar el alcance: una flota más grande agregaría más casos de validación (superposición, cantidad de naves hundidas para terminar la partida) sin que la consigna lo pida.
- **Las naves pueden quedar adyacentes.** Se permite que dos naves se toquen; solo se prohíbe que se superpongan o salgan del tablero. Es la misma lógica que en Generala se usa para simplificar reglas no exigidas explícitamente por la consigna (ahí, el bonus de "tirada servida").
- **El turno siempre alterna tras cada disparo**, sin disparo extra por acierto (una variante de casa frecuente en Batalla Naval). Se eligió la versión sin bonus porque es la que requiere menos estado para administrar turnos y la consigna no la pide.
- **No hay empates.** Es una consecuencia de las reglas anteriores (turnos alternados, hundimiento total de la flota rival como única condición de fin, y el abandono siempre resuelve un ganador), no una decisión aislada — ver sección 6.
- **Nombre de usuario: de 3 a 20 caracteres, sin espacios. Contraseña: de 4 a 20 caracteres, con al menos un número** (el resto puede ser cualquier carácter), con la misma lógica que el TP de referencia. Las reglas las valida `BLL.USUARIO.ValidarRegistro`, que se usa en el formulario de registro y también dentro de `BLL.USUARIO.Insertar`, y los campos de login y registro no dejan escribir más de 20 caracteres.
