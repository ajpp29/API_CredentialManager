using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_CredentialManager.Models
{
    [Table("CREDENCIAL", Schema = "dbo")]
    public class Credencial
    {
        private int _id;
        private int _usuarioID;
        private int _servidorID;
        private string _descripcion;
        private string _credencialUsuario;
        private DateTime _fechaCreacion;
        private string _usuarioModificacion;
        private DateTime _fechaModificacion;
        private bool _activo;

        public Credencial()
        {
            _descripcion = string.Empty;
            _fechaCreacion = System.DateTime.Now;
            _usuarioModificacion = string.Empty;
            _fechaModificacion = System.DateTime.Now;
            _activo = true;
        }

        [Key]
        [Column("CredencialID", Order = 0)]
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [Required]
        [Column("UsuarioID")]
        public int UsuarioID
        {
            get { return _usuarioID; }
            set { _usuarioID = value; }
        }

        [Required]
        [Column("ServidorID")]
        public int ServidorID
        {
            get { return _servidorID; }
            set { _servidorID = value; }
        }

        [Column("CredencialDescripcion")]
        public string Descripcion
        {
            get { return _descripcion; }
            set { _descripcion = value; }
        }

        [Column("CredencialUsuario")]
        public string CredencialUsuario
        {
            get { return _credencialUsuario; }
            set { _credencialUsuario = value; }
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
    }
}
