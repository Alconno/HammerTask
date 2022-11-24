using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Models.Models
{
    public class Employee : IEmployee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int employeeNo { get; set; }

        [Required(ErrorMessage = "You need to enter the employee name.")]
        [StringLength(50, ErrorMessage = "Employee name has to have less than 50 characters.")]
        [RegularExpression(@"^([a-zA-Z]*)\s+([a-zA-Z ]*)$", ErrorMessage = "Please enter only your name AND surname.")] // regex equal 2 strings that use characters a-zA-Z (name,surname)
        public string employeeName { get; set; }

        [Required(ErrorMessage = "You need to enter a salary")]
        [Range(1, 999999, ErrorMessage = "Please enter a normal salary")]
        public int Salary { get; set; }

        public DateTime lastModifyDate { get; set; } // will be automatically set

        public int EdepartmentNo { get; set; }
        public Department department { get; set; }

    }
}
