using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_CredentialManager.Models
{
    [Table("SERVIDOR", Schema = "dbo")]
    [PrimaryKey(nameof(ID))]
    public class Servidor
    {
        private int _id;
        private string _nombre;
        private string _ip;
        private string _fecCreacion;
        private bool _activo;

        [Key]
        [Column("ServidorID", Order = 0)]
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [Column("Servidor")]
        public string Nombre
        {
            get { return _nombre; }
            set { _nombre = value; }
        }

        [Column("ServidorIP")]
        public string IP
        {
            get { return _ip; }
            set { _ip = value; }
        }

        [Column("Fec_Creacion")]
        public string FecCreacion
        {
            get { return _fecCreacion; }
            set { _fecCreacion = value; }
        }

        [Column("Activo")]
        public bool Activo
        {
            get { return _activo; }
            set { _activo = value; }
        }
    }
}
