# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Two-player Battleship (same PC) for a university course (DAS, UAI). C# Windows Forms on **.NET Framework 4.7.2**, SQL Server, 4-layer architecture (BE / DAL / BLL / GUI). All code, identifiers, comments and UI text are in **Spanish** — keep it that way. Class names follow the course convention of UPPERCASE entity/class names (`BE.PARTIDA`, `BLL.USUARIO`, `DAL.MP_PARTIDA`) with the same name reused across layers.

## Commands

Run from the solution folder:

```
dotnet build TP_BatallaNaval.sln
dotnet test TP_BatallaNaval.sln --collect "Code Coverage;Format=cobertura"
dotnet test Tests.Unitarios                                   # no database needed
dotnet test Tests.Integracion                                 # needs LocalDB
dotnet test Tests.Unitarios --filter "FullyQualifiedName~PartidaTests"
dotnet test Tests.Unitarios --filter "Name=<MetodoDeTest>"
```

The app itself is run from Visual Studio with `GUI` as the startup project. The database is created by running `Script.sql` (idempotent) in SSMS; the app connects via `GUI/App.config` (`BATALLANAVAL` and `MASTER` connection strings, default `.\SQLEXPRESS`).

BE/DAL/BLL/GUI use old-style (non-SDK) `.csproj` files — new source files must be added to the `<Compile Include>` list of the project manually. The test projects are SDK-style (MSTest, `LangVersion 7.3`, so no C# 8+ features).

## Architecture

- **BE**: plain entities and enums (`ESTADOCELDA`, `ORIENTACION`, `TIPONAVE`, `NAVECOLOCADA`, `LOG` type constants such as `BE.LOG.INICIO_PARTIDA`).
- **DAL**: `ACCESO` is `internal` — only mappers touch the DB. All DB calls go through **stored procedures** (`CommandType.StoredProcedure`) defined in `Script.sql`. `Escribir`/`LeerEscalar` swallow exceptions and return `-1`; callers check for `-1`/`> 0` rather than catching. Mappers (`MP_*`) derive from `MAPPER<T>` and create a fresh `ACCESO` per operation. Multi-step writes (e.g. creating/finishing a game + writing the LOG rows for both players) are done in a single transaction inside the mapper (`IniciarTx`/`AceptarTx`/`CancelarTx`).
  - `ACCESO.ResolverServidor` tries the remembered server (`%LOCALAPPDATA%\BatallaNaval\servidor.txt`), then the App.config `Data Source`, then `.\SQLEXPRESS` and `.`, and replaces the Data Source with whichever responds. Its static fields are `internal` so tests can override them.
  - `DAL.MOVIMIENTOXML` writes one XML file per game (`Partidas/` next to the exe) with every ship placement, shot and the end of game. `DAL.BACKUPRESTORE` runs BACKUP/RESTORE via the `MASTER` connection.
- **BLL**: `BLL.PARTIDA` is the game engine and owns all game state (phases `ColocandoJugador1 → ColocandoJugador2 → Combate → Finalizada`, 15×15 boards, fleets, turn). The GUI only reads its state and forwards user actions; it raises `PartidaFinalizada` (`delPartidaFinalizada`). `BLL.MOVIMIENTOXML` decides what gets logged to XML and delegates to the DAL. `BLL.BITACORA` writes the LOG table.
  - Testability seam: BLL classes have a public parameterless constructor plus an `internal` constructor taking a DAL mapper; mapper methods are `virtual` so `Tests.Unitarios/Falsos.cs` can subclass them. `BITACORA` uses an `internal static Func<DAL.MP_LOG> CrearMapper`. DAL and BLL expose internals to both test assemblies via `InternalsVisibleTo` in `Properties/AssemblyInfo.cs`. Follow this pattern when adding BLL logic that hits the DB.
- **GUI**: `Program.Main` loops login → `FormPrincipal`, repeating if a restore forced a re-login (`FormPrincipal.VolverAlLogin`). Boards are user controls (`ucColocacion`, `ucCombate`, built from `ucCelda`); `TableroUI` draws rulers/legends; `Branding.cs` holds the official color palette — use it instead of hard-coded colors. `stitch_batalla_naval_winforms_ui/` contains the UI design mockups the forms are based on.

## Tests

- `Tests.Unitarios`: BE, BLL rules with fake mappers, and XML — no DB.
- `Tests.Integracion`: real SQL Server via `Tests.Integracion/App.config` (default `(localdb)\MSSQLLocalDB`). `BaseDePrueba` (`[AssemblyInitialize]`) **drops and recreates** `BATALLANAVAL` from `Script.sql`, but only if the existing DB carries the extended property `CreadaPorTestsBatallaNaval`; otherwise it refuses and fails. It also disables the DAL's server fallback and remembered-server file. Never point this config at the app's real instance.
- Line coverage of BE/DAL/BLL is ~99%; the GUI has no automated tests. Keep new BE/DAL/BLL code covered.

## Business rules

`Documentacion/Reglas_de_Negocio_BatallaNaval.md` is the source of truth for game rules (fleet composition, hidden information, turn always passes after a shot, no draws, abandon = the other player wins). Section 7 documents design decisions where the assignment was silent — check it before changing game behavior, and update it (and the in-app `FormReglas`) when rules change.
