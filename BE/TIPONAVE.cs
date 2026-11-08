namespace BE
{
    // Catálogo fijo de naves (no es una tabla SQL: el enunciado lo define como una lista
    // cerrada de 5 tipos, uno de cada uno por jugador - ver Reglas de Negocio, "Flota").
    public class TIPONAVE
    {
        public int Id { get; private set; }
        public string Nombre { get; private set; }
        public int Tamaño { get; private set; }

        public TIPONAVE(int id, string nombre, int tamaño)
        {
            Id = id;
            Nombre = nombre;
            Tamaño = tamaño;
        }

        public static readonly TIPONAVE[] Flota = new TIPONAVE[]
        {
            new TIPONAVE(1, "Batalla Naval", 5),
            new TIPONAVE(2, "Portaaviones", 4),
            new TIPONAVE(3, "Fragatas", 3),
            new TIPONAVE(4, "Buque", 2),
            new TIPONAVE(5, "Acorazados", 1),
        };
    }
}
