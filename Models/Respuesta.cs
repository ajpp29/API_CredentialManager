namespace API_CredentialManager.Models
{
    public class Respuesta<T>
    {
        public int Codigo { get; set; }
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public T Dato { get; set; }

        public Respuesta() 
        {
            Codigo = 200;
            Exito = true;
        }

        public Respuesta(int codigo, bool exito)
        {
            Codigo = codigo;
            Exito = exito;
        }

        public Respuesta(int codigo, bool exito, string mensaje)
        {
            Codigo = codigo;
            Exito = exito;
            Mensaje = mensaje;
        }

        public Respuesta(int codigo, bool exito, string mensaje, T dato)
        {
            Codigo = codigo;
            Exito = exito;
            Mensaje = mensaje;
            Dato = dato;
        }
    }
}
