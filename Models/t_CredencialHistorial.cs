using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_CredentialManager.Models
{
    [Table("CREDENCIAL_HISTORIAL", Schema = "dbo")]
    public class t_CredencialHistorial
    {
        private int _id;
        private int _credencialID;
        private string _credencialClave;
        private string _usuarioModificacion;
        private DateTime _fechaCreacion;
        private DateTime _fechaEncripcion;

        public t_CredencialHistorial()
        {
            _usuarioModificacion = string.Empty;
            _fechaCreacion = System.DateTime.Now;
            _fechaEncripcion = System.DateTime.Now;
        }

        [Key]
        [Column("CredencialHistorialID", Order = 0)]
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [Required]
        [Column("CredencialID")]
        public int CredencialID
        {
            get { return _credencialID; }
            set { _credencialID = value; }
        }

        [Column("CredencialClave")]
        public string CredencialClave
        {
            get { return _credencialClave; }
            set { _credencialClave = value; }
        }

        [Column("Usr_Modificacion")]
        public string UsuarioModificacion
        {
            get { return _usuarioModificacion; }
            set { _usuarioModificacion = value; }
        }

        [Column("Fec_Creacion")]
        public DateTime FechaCreacion
        {
            get { return _fechaCreacion; }
            set { _fechaCreacion = value; }
        }

        [Column("Fec_Encripcion")]
        public DateTime FechaEncripcion
        {
            get { return _fechaEncripcion; }
            set { _fechaEncripcion = value; }
        }
    }
}
