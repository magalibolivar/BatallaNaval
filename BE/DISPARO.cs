namespace BE
{
    // Un disparo ya resuelto durante el combate, en memoria (para el registro y las
    // marcas de "último disparo" de la pantalla de combate). Lo que se graba en disco
    // es el XML de movimientos, no esta clase.
    public class DISPARO
    {
        public int IdAtacante { get; set; }
        public int Fila { get; set; }
        public int Columna { get; set; }
        public ESTADOCELDA Resultado { get; set; }

        // Solo se informa cuando Resultado es Hundido: en un Tocado, el atacante no sabe
        // qué nave tocó (ver Reglas de Negocio, "Mecánica de combate").
        public TIPONAVE NaveHundida { get; set; }

        // Coordenada como la muestran las reglas del tablero: columna en letra, fila en número (p. ej. "C7").
        public string Coordenada
        {
            get { return ((char)('A' + Columna)).ToString() + (Fila + 1); }
        }
    }
}
