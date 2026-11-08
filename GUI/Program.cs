using System;
using System.Windows.Forms;
using BE;

namespace GUI
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
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
