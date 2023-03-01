using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Registration.Datamodel.DataModels;
using Registration.Datamodel.ViewModels;
using System.Net;
using System.Net.Mail;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Registration.Controllers
{
    public class LoginController : Controller
    {

        private readonly ILogger<LoginController> _logger;
        private readonly EmployeeDbContext _db;

        public LoginController(ILogger<LoginController> logger, EmployeeDbContext db)
        {
            _logger = logger;
            _db = db;
        }


       // Login Congtroller
        [HttpGet]
        public IActionResult Login()
        {
            LoginViewModel lvm = new LoginViewModel();
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel lvm)
        {
            var status = _db.Users.Where(x => x.Email == lvm.Email && x.Password == lvm.Password).FirstOrDefault();
                if (status!=null)
            {
                return RedirectToAction("LandingPage","Login");
            }
            else
            {
                TempData["Error Message"] = "Enter Correct details!";
            }
            return View();
        }

        [HttpGet]
        public IActionResult LandingPage()
        {
            return View();
        }


        // Forgot Controller
        [HttpGet]

        public IActionResult Forgot()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Forgot(ForgotViewModel fvm)
        {
            if (ModelState.IsValid)
            {
                var user = _db.Users.FirstOrDefault(u => u.Email == fvm.Email);
                if (user == null)
                {
                    return RedirectToAction("Login", "Login");
                }

                // Generate a password reset token for the user
                var token = Guid.NewGuid().ToString();



                // Store the token in the password resets table with the user's email
                var passwordReset = new PasswordReset
                {
                    Email = fvm.Email,
                    Token = token
                };


                _db.PasswordResets.Add(passwordReset);
                _db.SaveChanges();

                // Send an email with the password reset link to the user's email address
                var resetLink = Url.Action("ResetPassword", "Login", new { email = fvm.Email, token }, Request.Scheme);
                // Send email to user with reset password link


                var fromAddress = new MailAddress("pviral3011@gmail.com", "Sender Name");
                var toAddress = new MailAddress(fvm.Email);
                var subject = "Password reset request";
                var body = $"Hi,<br /><br />Please click on the following link to reset your password:<br /><br /><a href='{resetLink}'>{resetLink}</a>";


                var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("pviral3011@gmail.com", "xueamtwfugztnyzb"),
                    EnableSsl = true
                };
                smtpClient.Send(message);

                return RedirectToAction("ResetPassword", "Login");
            }

            return View();
        }




        /* public IActionResult Forgot()
         {
             return View();
         }

         [HttpPost]


         public IActionResult Forgot(LoginViewModel lvm)
         {
             var status = _db.Users.Where(x => x.Email == lvm.Email).FirstOrDefault();
             if (status != null)
             {
                 return RedirectToAction("ResetPassword", "Login");
             }

             return View();
         }*/





        /*  public IActionResult ForgotPasswordConfirmation()
          {
              return View();
          }*/

        // Registration Congtroller
        [HttpGet]
        public IActionResult Registration()
        {

            return View();
        }


        
        [HttpPost]
        [Route("/Login/Registration", Name ="Register")]
        public IActionResult Registration(RegistrationViewModel obj)
        {
            var User_data = new User()
            {
                FirstName = obj.FirstName,
                LastName = obj.LastName,
                Email = obj.Email,
                PhoneNumber = obj.PhoneNumber,
                Password = obj.Password,
                CityId = 2,
                CountryId = 1
            };

            _db.Users.Add(User_data);
            _db.SaveChanges();
            return RedirectToAction("Login");

        }



        // ResetPassword Congtroller
        public IActionResult ResetPassword()
        {
            return View();
        }
    }
}
