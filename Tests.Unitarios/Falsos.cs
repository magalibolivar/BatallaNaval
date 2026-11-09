using System;
using System.Collections.Generic;

namespace Tests.Unitarios
{
    // Mappers falsos: reemplazan a la base de datos para probar la BLL de forma aislada.

    internal class MapperPartidaFalso : DAL.MP_PARTIDA
    {
        public int IdAlInsertar = 7;
        public bool ResultadoFinalizar = true;
        public List<BE.PARTIDA> Insertadas = new List<BE.PARTIDA>();
        public List<Tuple<BE.PARTIDA, string>> Finalizadas = new List<Tuple<BE.PARTIDA, string>>();
        public List<BE.PARTIDA> PartidasDelUsuario = new List<BE.PARTIDA>();
        public int UsuarioListado = -1;

        public override int Insertar(BE.PARTIDA partida)
        {
            Insertadas.Add(partida);
            return IdAlInsertar;
        }

        public override bool Finalizar(BE.PARTIDA partida, string descripcion)
        {
            Finalizadas.Add(Tuple.Create(partida, descripcion));
            return ResultadoFinalizar;
        }

        public override List<BE.PARTIDA> ListarPorUsuario(int idUsuario)
        {
            UsuarioListado = idUsuario;
            return PartidasDelUsuario;
        }
    }

    internal class MapperUsuarioFalso : DAL.MP_USUARIO
    {
        public int IdAlInsertar = 10;
        public bool ResultadoLogin = true;
        public int IdAlLoguear = 3;
        public Dictionary<int, BE.USUARIO> Usuarios = new Dictionary<int, BE.USUARIO>();
        public List<BE.USUARIO> Insertados = new List<BE.USUARIO>();

        public override int Insertar(BE.USUARIO usuario)
        {
            Insertados.Add(usuario);
            return IdAlInsertar;
        }

        public int LoginsLlamados = 0;

        public override bool Login(BE.USUARIO usuario)
        {
            LoginsLlamados++;
            if (ResultadoLogin)
            {
                usuario.ID = IdAlLoguear;
            }
            return ResultadoLogin;
        }

        public override BE.USUARIO BuscarPorId(int id)
        {
            BE.USUARIO usuario;
            return Usuarios.TryGetValue(id, out usuario) ? usuario : null;
        }

        public Exception ErrorAlBuscarPorNombre = null;

        public override BE.USUARIO BuscarPorNombre(string nombre)
        {
            if (ErrorAlBuscarPorNombre != null)
            {
                throw ErrorAlBuscarPorNombre;
            }
            foreach (BE.USUARIO usuario in Usuarios.Values)
            {
                if (usuario.Nombre == nombre)
                {
                    return usuario;
                }
            }
            return null;
        }
    }

    internal class MapperLogFalso : DAL.MP_LOG
    {
        public int Resultado = 1;
        public List<BE.LOG> Registrados = new List<BE.LOG>();

        public override int Insertar(BE.LOG log)
        {
            Registrados.Add(log);
            return Resultado;
        }
    }
}
