using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Company.Models.Models
{
    public class EmployeeREST
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int employeeNo { get; set; }
        
        [Required(ErrorMessage = "You need to enter the employee name.")]
        [StringLength(50, ErrorMessage = "Employee name has to have less than 50 characters.")]
        public string employeeName { get; set; }

        [Required(ErrorMessage = "You need to enter a salary")]
        public int Salary { get; set; }

        public DateTime lastModifyDate { get; set; } // will be automatically set
    }
}
