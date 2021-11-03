using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hoteles.Data.DTOs
{
    public class UserDTO : LoginUserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [DataType(System.ComponentModel.DataAnnotations.DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        public List<string> Roles { get; set; }
        
    }

    public class LoginUserDTO
    {

        [Required]
        [DataType(System.ComponentModel.DataAnnotations.DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "Your password is limited to {2} to {1} characters", MinimumLength = 2)]
        public string Password { get; set; }

    }
}
