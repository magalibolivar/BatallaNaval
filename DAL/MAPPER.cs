namespace DAL
{
    public abstract class MAPPER<T>
    {
        internal ACCESO acceso;
        public abstract int Insertar(T objeto);
    }
}
