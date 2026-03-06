
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcCorePaginacionRegistros.Models
{
    [Table("dept")]
    public class Departamento
    {
        [Key]
        [Column("dept_no")]
        public int IdDepartamento { get; set; }
        [Column("dnombre")]
        public string Nombre { get; set; }
        [Column("loc")]
        public string Localidad { get; set; }
    }
}
