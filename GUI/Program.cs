using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BE;

namespace GUI
{
    static class Program
    {
        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        private static extern int AddFontResourceEx(string archivo, uint fl, IntPtr pdv);

        private const uint FR_PRIVATE = 0x10;

        // Carga Montserrat solo para este proceso (sin instalarla en Windows), así los
        // controles la encuentran por nombre. Si falla, WinForms usa la fuente por defecto.
        private static void CargarFuentes()
        {
            string carpeta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recursos");
            foreach (string archivo in new[] { "Montserrat-Regular.ttf", "Montserrat-Bold.ttf" })
            {
                try
                {
                    AddFontResourceEx(Path.Combine(carpeta, archivo), FR_PRIVATE, IntPtr.Zero);
                }
                catch (Exception)
                {
                }
            }
        }

        [STAThread]
        static void Main()
        {
            CargarFuentes();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Se repite solo si FormPrincipal se cerró por un restore de la base,
            // que obliga a volver a loguearse (ver FormPrincipal.btnRestore_Click).
            bool volverAlLogin = true;
            while (volverAlLogin)
            {
                Application.ThreadException -= Application_ThreadException;
                Application.ThreadException += Application_ThreadException;

                USUARIO usuarioLogueado;
                using (FormLogin formLogin = new FormLogin())
                {
                    if (formLogin.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    usuarioLogueado = formLogin.UsuarioLogueado;
                }

                FormPrincipal formPrincipal = new FormPrincipal(usuarioLogueado);
                Application.Run(formPrincipal);
                volverAlLogin = formPrincipal.VolverAlLogin;
            }
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(
                "Ocurrió un error inesperado: " + e.Exception.Message,
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
