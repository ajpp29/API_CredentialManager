using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace API_CredentialManager.Models
{
    [Table("USUARIO", Schema = "dbo")]
    public class t_Usuario
    {
        private int _id;
        private string _nombre;
        private string _correo;
        private string _clave;
        private string _key;
        private DateTime _fechaCreacion;
        private string _usuarioModificacion;
        private DateTime _fechaModificacion;
        private bool _activo;

        public t_Usuario()
        {
            _clave = string.Empty;
            _key = string.Empty;
            _fechaCreacion = System.DateTime.Now;
            _usuarioModificacion = string.Empty;
            _fechaModificacion = System.DateTime.Now;
            _activo = true;
        }

        [Key]
        [Column("UsuarioID", Order = 0)]
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [Required]
        [Column("Usuario")]
        public string Nombre
        {
            get { return _nombre; }
            set { _nombre = value; }
        }

        [Required]
        [Column("Correo")]
        public string Correo
        {
            get { return _correo; }
            set { _correo = value; }
        }

        [Column("Clave")]
        public string Clave
        {
            get { return _clave; }
            set { _clave = value; }
        }

        [Column("LlaveEncripcion")]
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        [Column("Fec_Creacion")]
        public DateTime FechaCreacion
        {
            get { return _fechaCreacion; }
            set { _fechaCreacion = value; }
        }

        [Column("Usr_Modificacion")]
        public string UsuarioModificacion
        {
            get { return _usuarioModificacion; }
            set { _usuarioModificacion = value; }
        } 

        [Column("Fec_Modificacion")]
        public DateTime FechaModificacion
        {
            get { return _fechaModificacion; }
            set { _fechaModificacion = value; }
        }

        [Column("Activo")]
        public bool Activo
        {
            get { return _activo; }
            set { _activo = value; }
        }

        public void ocultarClave()
        {
            _clave = String.Empty;
        }

        public void ocultarKey()
        {
            _key = String.Empty;
        }

        public void crearKey()
        {
            _key = GenerarStringAleatorio(32);
        }

        public void encriptarClave()
        {
            _clave = EncriptarSHA256(_clave);
        }

        private string GenerarStringAleatorio(int longitud)
        {
            const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder resultado = new StringBuilder(longitud);

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] buffer = new byte[1];

                for (int i = 0; i < longitud; i++)
                {
                    rng.GetBytes(buffer);
                    int index = buffer[0] % caracteres.Length; // Mapear el byte a un índice de la cadena
                    resultado.Append(caracteres[index]);
                }
            }

            return resultado.ToString();
        }

        private string EncriptarSHA256(string texto)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convertir el texto a bytes
                byte[] bytesTexto = Encoding.UTF8.GetBytes(texto);

                // Generar el hash
                byte[] hashBytes = sha256.ComputeHash(bytesTexto);

                // Convertir el hash a formato hexadecimal
                StringBuilder hashHex = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    hashHex.Append(b.ToString("x2")); // "x2" para formato hexadecimal en minúsculas
                }

                return hashHex.ToString();
            }
        }
    }
}
