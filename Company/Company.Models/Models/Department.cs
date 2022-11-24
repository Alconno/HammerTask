using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Models.Models
{
    public class Department : IDepartment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int departmentNo { get; set; }

        [Required]
        [StringLength(20)]
        public string departmentName { get; set; }

        [Required]
        [StringLength(20)]
        public string departmentLocation { get; set; }
    }
}
