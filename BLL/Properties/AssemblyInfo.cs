using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("BLL")]
[assembly: AssemblyDescription("Capa de Logica de Negocio - TP Batalla Naval")]
[assembly: AssemblyProduct("BLL")]
[assembly: ComVisible(false)]
[assembly: Guid("ab111111-1111-4111-9111-111111111203")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// Los tests acceden a los miembros internal (seams para reemplazar la base o el servidor).
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Tests.Unitarios")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Tests.Integracion")]
