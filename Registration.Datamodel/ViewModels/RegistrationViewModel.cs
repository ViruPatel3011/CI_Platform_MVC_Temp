using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registration.Datamodel.ViewModels
{
    public class RegistrationViewModel
    {
        [Required(ErrorMessage ="First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]

        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Phone Number is required")]
        public int PhoneNumber { get; set; }
    }
}
