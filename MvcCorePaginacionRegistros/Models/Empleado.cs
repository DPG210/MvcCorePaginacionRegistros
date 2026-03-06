using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcCorePaginacionRegistros.Models
{
    [Table("emp")]
    public class Empleado
    {
        [Key]
        [Column("emp_no")]
        public int IdEmpleado { get; set; }
        [Column("Apellido")]
        public string Apellido { get; set; }
        [Column("oficio")]
        public string Oficio { get; set; }
        [Column ("salario")]
        public int Salario { get; set; }
        [Column("dept_no")]
        public int IdDepartamento { get; set; }
    }
}
