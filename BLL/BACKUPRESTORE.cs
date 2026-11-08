namespace BLL
{
    public static class BACKUPRESTORE
    {
        public static string CarpetaBackupPorDefecto()
        {
            return DAL.BACKUPRESTORE.CarpetaBackupPorDefecto();
        }

        // Devuelve "" si salió bien, o el mensaje de error si falló.
        public static string HacerBackup(string ruta)
        {
            return DAL.BACKUPRESTORE.HacerBackup(ruta);
        }

        // Devuelve "" si salió bien, o el mensaje de error si falló.
        // "restaurada" indica si los datos de la base ya se reemplazaron (puede ser true aunque haya error).
        public static string HacerRestore(string ruta, out bool restaurada)
        {
            return DAL.BACKUPRESTORE.HacerRestore(ruta, out restaurada);
        }
    }
}
