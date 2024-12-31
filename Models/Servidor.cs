using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_CredentialManager.Models
{
    [Table("SERVIDOR", Schema = "dbo")]
    //[PrimaryKey(nameof(ID))]
    public class Servidor
    {
        private int _id;
        private string _nombre;
        private string _ip;
        private DateTime _fechaCreacion;
        private string _usuarioModificacion;
        private DateTime _fechaModificacion;
        private bool _activo;

        public Servidor()
        {
            _fechaCreacion = System.DateTime.Now;
            _fechaModificacion = System.DateTime.Now;
            _activo = true;
            _usuarioModificacion = String.Empty;
        }

        [Key]
        [Column("ServidorID", Order = 0)]
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [Required]
        [Column("Servidor")]
        public string Nombre
        {
            get { return _nombre; }
            set { _nombre = value; }
        }

        [Required]
        [Column("ServidorIP")]
        public string IP
        {
            get { return _ip; }
            set { _ip = value; }
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
