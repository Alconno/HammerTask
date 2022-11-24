using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Models.Models
{
    public class Login : ILogin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int loginNo { get; set; }

        [Required(ErrorMessage = "You need to enter a username.")]
        [StringLength(20, ErrorMessage = "Username has to have less than 20 characters.")]
        public string loginUserName { get; set; }

        [Required(ErrorMessage = "You need to enter a password.")]
        [DataType(DataType.Password)]
        [StringLength(200, MinimumLength = 6, ErrorMessage = "You need to provide long enough password.")]
        public string loginPassword { get; set; }
    }
}
