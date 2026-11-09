# Reglas de negocio - TP Batalla Naval

Reglas que implementa la aplicación. Donde la consigna no especifica algo, la decisión tomada y su motivo están en la sección 7.

## 1. Modalidad de juego

- **2 jugadores humanos, en la misma PC, en la misma ventana** (Jugador 1 y Jugador 2 se turnan frente al mismo teclado/mouse). No hay IA ni juego contra la máquina, y no hay red/multijugador remoto.
- **Cada jugador ingresa por separado**, con su usuario y contraseña o **como invitado** (botón "Jugar como invitado", sin usuario ni contraseña). Jugador 1 es quien ingresó al abrir la app (`FormLogin`). Jugador 2 ingresa desde `FormPrincipal` con el botón "Ingresar", reutilizando la misma pantalla de login. Tiene que ser un usuario **distinto** al Jugador 1.
- **Jugar como invitado:** el Jugador 1 anónimo juega como el usuario `Invitado1` y el Jugador 2 anónimo como `Invitado2` (así pueden jugar 2 invitados entre sí). Sus inicios de sesión, partidas y estadísticas se registran igual que los de cualquier usuario, pero a nombre de ese invitado, compartido por todos los que juegan sin cuenta. A un invitado no se puede entrar con usuario y contraseña, y sus nombres no se pueden registrar.
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
- Para colocar, el jugador elige una nave de su flota (la primera pendiente ya viene elegida) y, al pasar el mouse por el tablero, ve dónde quedaría y si ahí entra; con un click la coloca y queda elegida la siguiente. La orientación de la nave nueva se elige antes de colocarla con el botón **"Girar"**, la tecla **R** o click derecho sobre el tablero.
- Mientras el jugador no confirme su flota, puede **corregir** cualquier nave ya colocada: manteniendo apretado el click sobre ella y arrastrándola la mueve a otra posición, con un click (sin arrastrar) la gira entre horizontal y vertical, y con click derecho la saca del tablero (vuelve a quedar pendiente). Cada cambio se valida con las mismas reglas (no superponerse, no salirse del tablero); si no es válido, la nave vuelve a donde estaba.
- **"Al azar"** coloca en posiciones al azar (válidas) las naves que falten, sin mover las que ya están; si la flota ya está completa, la vuelve a distribuir entera (cada click da otra disposición); **"Reiniciar"** saca todas las naves. Ninguno de los dos confirma la flota.
- Con las 5 naves en el tablero, el jugador presiona **"Confirmar flota"** para cerrar su colocación. Recién ahí pasa el turno de colocación al otro jugador (o empieza el combate), y desde ese momento ya no puede modificar sus naves.

## 3. Información oculta

- Mientras un jugador coloca sus naves, el otro **no puede ver ese tablero**. Antes de que cada jugador empiece a colocar, la aplicación muestra una pantalla neutra ("Jugador X: coloca tus naves. Pasá el dispositivo y presioná Continuar") y solo después de confirmarla se dibuja el tablero de ese jugador con sus naves.
- Cuando los 2 jugadores confirmaron su flota (10 naves en total), empieza el combate.
- Durante el combate, cada jugador ve en su turno su propio tablero (con sus naves y los disparos que recibió) y el radar del rival en niebla. Después de disparar, la pantalla queda bloqueada mostrando el resultado hasta que el jugador presiona **"Pasar turno"**: ahí se ocultan los tableros y aparece la pantalla neutra "Turno de X. Pasá el dispositivo", y recién después se dibuja el punto de vista del otro jugador.
- Al terminar la partida (por hundimiento o abandono) ya no hay nada que ocultar: el radar muestra también las naves del rival que quedaron sin descubrir.

## 4. Mecánica de combate

1. En su turno, un jugador dispara a una celda del tablero del rival, elegida sobre una grilla que muestra **niebla de guerra**: solo las celdas ya disparadas (agua, tocado o hundido); una nave que todavía no recibió ningún disparo nunca se muestra.
2. No se puede disparar dos veces a la misma celda.
3. El resultado de cada disparo es uno de tres: **Agua** (la celda estaba vacía), **Tocado** (había una nave, pero todavía le quedan celdas sin tocar) o **Hundido** (ese disparo completó todas las celdas de esa nave). Al hundir una nave se anuncia **cuál** fue; en un tocado, no (el atacante solo sabe que le dio a algo).
4. El turno **siempre pasa al otro jugador** después de cada disparo, sea agua, tocado o hundido: no hay disparo extra por acertar.
5. La pantalla de combate muestra además, para orientar a los jugadores: la casilla a la que se apunta, el último disparo de cada lado marcado en el tablero, el resultado del último disparo del rival al empezar el turno, qué naves del rival siguen a flote o ya se hundieron, la cantidad de disparos y aciertos, y un registro de todos los disparos de la partida (información que ambos jugadores ya conocen).
6. La partida termina cuando un jugador hunde las **5 naves** de su rival; gana ese jugador. También se puede abandonar en cualquier momento (botón "Abandonar partida"): la gana siempre el otro jugador. En la bitácora queda "Partida abandonada por" y el nombre de quien abandonó.

## 5. Bitácora y estadísticas

- Se registra en la bitácora: inicio de sesión, cierre de sesión, inicio de partida y fin de partida (de ambos jugadores en cada evento de partida).
- Todos los movimientos de cada partida se graban en un archivo XML propio de esa partida: cada **colocación** de nave (jugador, nave, posición y orientación finales, grabadas al confirmar la flota), cada **disparo** (jugador, celda, resultado y, si corresponde, qué nave fue tocada/hundida) y el **cierre** (ganador y si fue por abandono), todos con fecha y hora. Se consultan desde Estadísticas → "Ver movimientos".
- Las estadísticas por jugador muestran: partidas ganadas / perdidas / empatadas, porcentaje de victorias, y tiempo total acumulado jugado (suma de la duración de todas sus partidas terminadas).

## 6. Empates

- En Batalla Naval **no existen los empates**: la partida termina siempre con un ganador, ya sea porque hundió toda la flota rival o porque el rival abandonó. El contador de "Empatadas" en las estadísticas se muestra igual (porque el enunciado general de la materia lo pide para todos los TP), pero siempre va a dar 0.

## 7. Decisiones de diseño

La consigna pide "Batalla Naval para 2 jugadores" con turnos, tablero de 15x15, naves con 5 tamaños distintos y el resultado por casillero (agua/tocado/hundido), sin más detalle. Estas son las decisiones tomadas donde no lo especifica:

- **Los 2 jugadores juegan en la misma PC.** Igual que en la consigna general de la materia, no se pide red, y jugar en red agregaría un tema (comunicación entre equipos) fuera del alcance de la materia. Cada jugador igual se identifica con su propio usuario, así la bitácora y las estadísticas quedan a nombre de cada uno.
- **Información oculta en una sola pantalla compartida.** A diferencia de un juego sin información secreta, en Batalla Naval un jugador no puede ver dónde el rival puso sus naves. La regla de diseño que resuelve esto: **en pantalla solo se dibuja el tablero propio del jugador que está frente a ella**, y el del rival siempre en modo niebla, que por construcción nunca revela una nave no tocada. Como en combate cada jugador ve también su propia flota (para seguir qué le pegaron), cada cambio de turno pasa por una pantalla de "paso de dispositivo", con los tableros ocultos detrás. Antes se mostraba directamente la pantalla del siguiente jugador al disparar, y el que acababa de disparar veía la flota completa del rival.
- **El cambio de turno lo confirma el jugador ("Pasar turno").** En vez de pasar automáticamente al otro jugador al disparar, la pantalla queda mostrando el resultado hasta que el jugador presiona "Pasar turno". Así tiene tiempo de ver qué pasó con su disparo, y la pantalla de paso aparece justo cuando se va a cambiar de jugador. No cambia la regla de turnos: el disparo ya está hecho y no se puede disparar de nuevo.
- **Una nave de cada tipo por jugador (5 naves, 15 casilleros de 225).** La consigna lista 5 tipos de nave con su tamaño, pero no dice cuántas de cada una. Se tomó la lectura literal (una de cada una) para acotar el alcance: una flota más grande agregaría más casos de validación (superposición, cantidad de naves hundidas para terminar la partida) sin que la consigna lo pida.
- **Las naves pueden quedar adyacentes.** Se permite que dos naves se toquen; solo se prohíbe que se superpongan o salgan del tablero. Es la misma lógica que en Generala se usa para simplificar reglas no exigidas explícitamente por la consigna (ahí, el bonus de "tirada servida").
- **El turno siempre alterna tras cada disparo**, sin disparo extra por acierto (una variante de casa frecuente en Batalla Naval). Se eligió la versión sin bonus porque es la que requiere menos estado para administrar turnos y la consigna no la pide.
- **Corrección de la colocación y confirmación explícita.** Antes, la colocación de un jugador terminaba sola al poner la quinta nave, así que esa última no se podía corregir. Por eso la colocación se cierra con el botón "Confirmar flota" y, hasta entonces, las naves se pueden arrastrar y girar. Una nave ya colocada se gira con un click sobre ella. Para la nave nueva, la orientación se elige antes de colocarla ("Girar", tecla R o click derecho), y la vista previa muestra dónde quedaría y si entra, así no hace falta colocarla en horizontal para recién después girarla; si se apunta muy cerca del borde, la nave se corre hacia adentro para que entre. "Al azar" elige entre todas las posiciones válidas (de la nave más grande a la más chica), así siempre encuentra lugar sin reintentos. En el XML se graba la posición **final** de cada nave al confirmar, no cada corrección intermedia, porque los movimientos de la partida que interesa consultar son las posiciones con las que se jugó, no los reacomodos previos.
- **No hay empates.** Es una consecuencia de las reglas anteriores (turnos alternados, hundimiento total de la flota rival como única condición de fin, y el abandono siempre resuelve un ganador), no una decisión aislada (ver sección 6).
- **Invitados como 2 usuarios fijos.** Para jugar sin cuenta hacía falta igual una fila en `USUARIO`, porque `PARTIDA` y `LOG` la referencian por clave foránea (y la consigna pide registrar en la bitácora los inicios de sesión y las partidas de ambos jugadores). Se descartó crear un usuario nuevo por cada ingreso anónimo, porque la tabla acumularía invitados sin fin, y no registrar las partidas de invitados, porque rompería la bitácora y las estadísticas. `Script.sql` crea `Invitado1` e `Invitado2` con contraseña vacía; `USUARIO_LOGIN` rechaza las contraseñas vacías y la BLL rechaza esos nombres en el login y en el registro, así que solo se entra a ellos con "Jugar como invitado". Hay 2 (y no 1) porque los 2 jugadores de una partida tienen que ser usuarios distintos.
- **Nombre de usuario: de 3 a 20 caracteres, sin espacios. Contraseña: de 4 a 20 caracteres, con al menos un número** (el resto puede ser cualquier carácter), con la misma lógica que el TP de referencia. Las reglas las valida `BLL.USUARIO.ValidarRegistro`, que se usa en el formulario de registro y también dentro de `BLL.USUARIO.Insertar`, y los campos de login y registro no dejan escribir más de 20 caracteres.
