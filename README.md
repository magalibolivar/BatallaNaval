# Batalla Naval - TP individual DAS (UAI)

Juego de Batalla Naval para 2 jugadores en la misma PC, en C# (Windows Forms, .NET Framework 4.7.2) con SQL Server, en arquitectura de 4 capas (BE / DAL / BLL / GUI).

Alumna: Valentina Morana

## Requisitos

- Windows con **Visual Studio** (2019 o posterior) y el workload "Desarrollo de escritorio de .NET".
- **SQL Server** (Express alcanza) y **SQL Server Management Studio** (SSMS) para correr el script.
- .NET Framework 4.7.2.

## Instalación

### 1. Crear la base de datos

Abrir `Script.sql` en SSMS, conectado a la instancia de SQL Server, y ejecutarlo completo (F5).

El script crea la base `BATALLANAVAL` con sus tablas, stored procedures y usuarios de prueba. Se puede volver a correr sin errores: solo crea lo que falte.

### 2. Configurar la conexión (solo si hace falta)

La app se conecta por defecto a la instancia `.\SQLEXPRESS` con autenticación de Windows. Si SQL Server está instalado con otro nombre de instancia, cambiar el `Data Source` de las dos cadenas (`BATALLANAVAL` y `MASTER`) en `GUI/App.config`:

```xml
<connectionStrings>
    <add name="BATALLANAVAL" connectionString="Data Source=.\SQLEXPRESS; Initial Catalog=BATALLANAVAL; Integrated Security=SSPI; Current Language=Spanish;" providerName="System.Data.SqlClient" />
    <add name="MASTER" connectionString="Data Source=.\SQLEXPRESS; Initial Catalog=master; Integrated Security=SSPI; Current Language=Spanish;" providerName="System.Data.SqlClient" />
</connectionStrings>
```

Ejemplos de `Data Source`:

| Instalación | Data Source |
|---|---|
| SQL Server Express (por defecto) | `.\SQLEXPRESS` |
| SQL Server instancia por defecto (Developer / Standard) | `.` |
| Instancia con nombre | `.\NOMBRE_INSTANCIA` |

Si ya se compiló, también se puede editar `GUI/bin/Debug/GUI.exe.config` directamente, sin recompilar.

### 3. Compilar y ejecutar

Abrir `TP_BatallaNaval.sln` en Visual Studio, dejar `GUI` como proyecto de inicio y ejecutar (F5).

## Usuarios de prueba

Los carga `Script.sql`:

| Usuario | Contraseña |
|---|---|
| valentina | 1234 |
| jugador2 | 1234 |
| jugador3 | 1234 |
| magali | 1234 |

También se pueden crear usuarios nuevos con el enlace "Creá una" de la pantalla de login. El nombre de usuario lleva de 3 a 20 caracteres, sin espacios. La contraseña lleva de 4 a 20 caracteres y al menos un número.

Para jugar sin cuenta está el botón **"Jugar como invitado"**: el Jugador 1 entra como `Invitado1` y el Jugador 2 como `Invitado2` (los crea `Script.sql`; en una base creada antes de este cambio hay que volver a correr el script). Las partidas de los invitados quedan registradas a su nombre.

## Cómo se juega

1. Ingresar con el Jugador 1 (pantalla inicial), con usuario y contraseña o como invitado.
2. En la pantalla principal, "Ingresar" el Jugador 2 con **otro** usuario (o como invitado).
3. "Comenzar Partida". Cada jugador coloca sus 5 naves en su propio tablero (se pueden arrastrar para moverlas y girar con un click hasta presionar "Confirmar flota"; la app pide pasar el dispositivo entre jugador y jugador, para que ninguno vea el tablero del otro mientras coloca).
4. Terminada la colocación, empieza el combate: en cada turno se dispara a una celda del tablero rival (que se ve sin revelar naves no tocadas) y se informa agua, tocado o hundido. El turno pasa siempre al otro jugador.
5. Gana quien hunde las 5 naves del rival. También se puede abandonar con "Abandonar Partida": la gana el otro jugador.

Las reglas completas están en el botón "Reglas" de la app y en `Documentacion/Reglas_de_Negocio_BatallaNaval.md`. Ahí, la sección 7 explica las decisiones tomadas donde la consigna no especifica (juego en la misma PC, cantidad de naves por tipo, información oculta, turno tras un acierto, empates).

## Dónde ver cada requisito

| Requisito | Dónde |
|---|---|
| Registro / Login / Logout | Pantalla de login → "Registrarme"; botón "Salir" (o cerrar la ventana) |
| Bitácora | Tabla `LOG` de la base (tipos en `TIPO_LOG`). El inicio y el fin de partida se graban en la misma transacción que la partida |
| Estadísticas, promedio de victorias, tiempo jugado | Botón "Estadísticas" |
| XML de movimientos | Un archivo por partida en `GUI/bin/Debug/Partidas/`, con cada colocación de nave, cada disparo (celda y resultado) y el cierre (ganador y si fue abandono). Se consultan desde Estadísticas → elegir partida → "Ver movimientos" |
| Backup / Restore | Botones "Backup" y "Restore" de la pantalla principal |

**Nota sobre el backup:** `BACKUP DATABASE` lo ejecuta el servicio de SQL Server, no el usuario de Windows, así que en carpetas personales (Escritorio, Documentos) suele fallar por permisos. Por eso el diálogo arranca en la carpeta de backups de SQL Server, que es donde conviene guardarlo.

**Nota sobre el restore:** después de restaurar, la aplicación vuelve a pedir el inicio de sesión, porque los usuarios de la base restaurada pueden ser otros.

## Tests

Hay dos proyectos de test (MSTest), que se ven en el Explorador de pruebas de Visual Studio:

| Proyecto | Qué prueba | Necesita base |
|---|---|---|
| `Tests.Unitarios` | Entidades (BE), reglas de la BLL (motor de la partida, validaciones de usuario, estadísticas, bitácora) con mappers falsos, y el XML de movimientos | No |
| `Tests.Integracion` | DAL y BLL contra SQL Server real: stored procedures, transacciones de partida + bitácora, login, estadísticas, backup/restore y la resolución del servidor | Sí |

Los tests de integración **borran y vuelven a crear** la base `BATALLANAVAL` con `Script.sql` en la instancia de `Tests.Integracion/App.config` (por defecto LocalDB, `(localdb)\MSSQLLocalDB`, que viene con Visual Studio), así que no usan la base de la aplicación. Si en esa instancia ya hay una `BATALLANAVAL` que no crearon los tests, no la tocan y fallan con un aviso.

Desde una consola, en la carpeta de la solución:

```
dotnet test TP_BatallaNaval.sln --collect "Code Coverage;Format=cobertura"
```

La cobertura de BE, DAL y BLL es de 99% de las líneas; quedan sin cubrir solo dos casos que dependen del servidor (la carpeta de backup por defecto, que LocalDB no informa, y que falle el `MULTI_USER` después de un restore exitoso). La GUI (Windows Forms) no tiene tests automáticos.

## Estructura

```
TP_BatallaNaval/
├── BE/             entidades (USUARIO, PARTIDA, LOG, ESTADISTICA, TIPONAVE, NAVECOLOCADA, etc.)
├── DAL/            acceso a SQL Server (ACCESO, MAPPER<T> y un mapper por tabla, backup/restore) y archivos XML de movimientos
├── BLL/            reglas del juego (motor de la partida en PARTIDA.cs), estadísticas, bitácora, qué se registra en el XML
├── GUI/            Windows Forms (ejecutable), controles ucCelda / ucColocacion / ucCombate,
│                   Branding.cs (paleta oficial) y Recursos/ (ícono .ico e ilustración de cada nave)
├── Tests.Unitarios/   tests unitarios (BE, BLL con mappers falsos, XML)
├── Tests.Integracion/ tests de integración contra SQL Server (LocalDB)
├── Documentacion/  reglas de negocio y decisiones de diseño
├── Script.sql      creación de la base, stored procedures y datos de prueba
└── TP_BatallaNaval.sln
```
