namespace BE
{
    // Una nave ya colocada en el tablero propio de un jugador.
    public class NAVECOLOCADA
    {
        public TIPONAVE Tipo { get; set; }
        public int Fila { get; set; }
        public int Columna { get; set; }
        public ORIENTACION Orientacion { get; set; }
        public int CantidadImpactos { get; set; }

        public bool Hundida
        {
            get { return CantidadImpactos >= Tipo.Tamaño; }
        }

        // Celdas (fila, columna) que ocupa esta nave en el tablero.
        public System.Collections.Generic.List<System.Tuple<int, int>> Celdas()
        {
            var celdas = new System.Collections.Generic.List<System.Tuple<int, int>>();
            for (int i = 0; i < Tipo.Tamaño; i++)
            {
                if (Orientacion == ORIENTACION.Horizontal)
                {
                    celdas.Add(System.Tuple.Create(Fila, Columna + i));
                }
                else
                {
                    celdas.Add(System.Tuple.Create(Fila + i, Columna));
                }
            }
            return celdas;
        }
    }
}
