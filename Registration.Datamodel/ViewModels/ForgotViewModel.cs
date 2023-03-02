using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registration.Datamodel.ViewModels
{
    public class ForgotViewModel
    {

        [Required(ErrorMessage ="Enter Email Address")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
