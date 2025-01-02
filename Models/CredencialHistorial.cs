using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_CredentialManager.Models
{
    [Table("CREDENCIAL_HISTORIAL", Schema = "dbo")]
    public class CredencialHistorial
    {
        private int _id;
        private int _credencialID;
        private string _credencialClave;
        private string _usuarioModificacion;
        private DateTime _fechaModificacion;

        public CredencialHistorial()
        {
            _credencialClave = string.Empty;
            _usuarioModificacion = string.Empty;
            _fechaModificacion = System.DateTime.Now;
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

        [Column("Fec_Modificacion")]
        public DateTime FechaModificacion
        {
            get { return _fechaModificacion; }
            set { _fechaModificacion = value; }
        }
    }
}
