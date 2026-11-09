using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("DAL")]
[assembly: AssemblyDescription("Capa de Acceso a Datos - TP Batalla Naval")]
[assembly: AssemblyProduct("DAL")]
[assembly: ComVisible(false)]
[assembly: Guid("ab111111-1111-4111-9111-111111111202")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// Los tests acceden a los miembros internal (seams para reemplazar la base o el servidor).
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Tests.Unitarios")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Tests.Integracion")]
