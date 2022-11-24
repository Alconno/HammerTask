using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Models.Models
{
    public class DepartmentREST
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int departmentNo { get; set; }

        [Required(ErrorMessage = "You need to enter the department name.")]
        [StringLength(20, ErrorMessage = "Department Name can't be over 20 characters.")]
        public string departmentName { get; set; }

        [Required(ErrorMessage = "You need to enter the department location.")]
        [StringLength(20, ErrorMessage = "Department Locaiton can't be over 20 characters.")]
        public string departmentLocation { get; set; }
    }
}
