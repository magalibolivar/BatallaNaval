using System;

namespace BLL
{
    public class USUARIO
    {
        // Largos permitidos. Los TextBox de login y registro tienen MaxLength = 20.
        public const int MIN_NOMBRE = 3;
        public const int MAX_NOMBRE = 20;
        public const int MIN_CONTRASEÑA = 4;
        public const int MAX_CONTRASEÑA = 20;

        // Usuarios fijos para jugar sin cuenta ("Jugar como invitado"): el Jugador 1 anónimo
        // usa INVITADO_1 y el Jugador 2 anónimo, INVITADO_2. Los crea Script.sql con
        // contraseña vacía (ver Reglas_de_Negocio_BatallaNaval.md, "Jugar como invitado").
        public const string INVITADO_1 = "Invitado1";
        public const string INVITADO_2 = "Invitado2";

        public static bool EsInvitado(string nombre)
        {
            return nombre != null
                && (string.Equals(nombre.Trim(), INVITADO_1, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(nombre.Trim(), INVITADO_2, StringComparison.OrdinalIgnoreCase));
        }

        public static bool EsInvitado(BE.USUARIO usuario)
        {
            return usuario != null && EsInvitado(usuario.Nombre);
        }

        private DAL.MP_USUARIO mapper;

        public USUARIO() : this(new DAL.MP_USUARIO())
        {
        }

        // Los tests pasan un mapper falso para probar las reglas sin base de datos.
        internal USUARIO(DAL.MP_USUARIO mapper)
        {
            this.mapper = mapper;
        }

        // Reglas para registrar un usuario nuevo. Devuelve "" si está bien,
        // o el motivo por el que no se puede registrar.
        public string ValidarRegistro(BE.USUARIO usuario)
        {
            if (usuario.Nombre == null || usuario.Contraseña == null)
            {
                return "Falta completar el nombre de usuario o la contraseña";
            }
            if (usuario.Nombre.Contains(" "))
            {
                return "El nombre de usuario no puede tener espacios";
            }
            if (EsInvitado(usuario.Nombre))
            {
                return "Ese nombre está reservado para los invitados";
            }
            if (usuario.Nombre.Length < MIN_NOMBRE || usuario.Nombre.Length > MAX_NOMBRE)
            {
                return "El nombre de usuario tiene que tener entre " + MIN_NOMBRE + " y " + MAX_NOMBRE + " caracteres";
            }
            if (usuario.Contraseña.Length < MIN_CONTRASEÑA || usuario.Contraseña.Length > MAX_CONTRASEÑA)
            {
                return "La contraseña tiene que tener entre " + MIN_CONTRASEÑA + " y " + MAX_CONTRASEÑA + " caracteres";
            }
            if (!TieneNumero(usuario.Contraseña))
            {
                return "La contraseña tiene que tener al menos un número";
            }
            return "";
        }

        private bool TieneNumero(string texto)
        {
            foreach (char c in texto)
            {
                if (char.IsDigit(c))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Login(BE.USUARIO usuario)
        {
            // A un invitado se entra solo con IngresarComoInvitado, nunca con usuario y contraseña.
            if (EsInvitado(usuario))
            {
                return false;
            }

            bool ok = mapper.Login(usuario);
            if (ok)
            {
                BITACORA.Registrar(usuario.ID, BE.LOG.INICIO_SESION, "Inicio de sesión de " + usuario.Nombre);
            }
            return ok;
        }

        // Ingreso sin usuario ni contraseña. numeroJugador (1 o 2) elige el invitado, así
        // pueden jugar 2 jugadores anónimos entre sí. Queda en la bitácora como cualquier inicio de sesión.
        public BE.USUARIO IngresarComoInvitado(int numeroJugador)
        {
            if (numeroJugador != 1 && numeroJugador != 2)
            {
                throw new Exception("El invitado tiene que ser el Jugador 1 o el Jugador 2.");
            }

            string nombre = numeroJugador == 1 ? INVITADO_1 : INVITADO_2;
            BE.USUARIO invitado = mapper.BuscarPorNombre(nombre);
            if (invitado == null)
            {
                throw new Exception("No existe el usuario " + nombre + " en la base de datos. " +
                    "Ejecutá Script.sql para crear los usuarios invitados.");
            }

            BITACORA.Registrar(invitado.ID, BE.LOG.INICIO_SESION, "Inicio de sesión de " + invitado.Nombre + " (invitado)");
            return invitado;
        }

        public void Logout(BE.USUARIO usuario)
        {
            BITACORA.Registrar(usuario.ID, BE.LOG.CIERRE_SESION, "Cierre de sesión de " + usuario.Nombre);
        }

        // Devuelve false si el nombre de usuario ya existe.
        // Si falla la base de datos tira una excepción con el motivo.
        public bool Insertar(BE.USUARIO usuario)
        {
            // Las reglas se controlan acá también, así no se pueden saltear llamando directo a la BLL.
            string error = ValidarRegistro(usuario);
            if (error != "")
            {
                throw new Exception(error);
            }

            int id = mapper.Insertar(usuario);
            if (id == -1)
            {
                throw new Exception("No se pudo registrar el usuario en la base de datos.");
            }
            if (id == 0)
            {
                return false;
            }
            usuario.ID = id;
            return true;
        }

        public BE.USUARIO BuscarPorId(int id)
        {
            return mapper.BuscarPorId(id);
        }
    }
}
