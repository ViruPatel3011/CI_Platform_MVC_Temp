using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Registration.Datamodel.DataModels;
using Registration.Datamodel.ViewModels;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
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


       // Login Congtroller_________________
        [HttpGet]
        public IActionResult Login()
        {
            /*LoginViewModel lvm = new LoginViewModel();*/
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel lvm)
        {
            var status = _db.Users.Where(x => x.Email == lvm.Email && x.Password == lvm.Password).FirstOrDefault();
                if (status!=null)
            {

                /*This ClaimsIdentity method provides user information automatically so applications do not need to request it of
                the user and the user doesn't have to provide that information separately for different applications.*/
                var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, status.Email) },
                        CookieAuthenticationDefaults.AuthenticationScheme); 
                /*CookieAuthenticationDefaults.AuthenticationScheme provides “Cookies” for the scheme*/
                
                identity.AddClaim(new Claim(ClaimTypes.Name, status.FirstName));
                identity.AddClaim(new Claim(ClaimTypes.Surname, status.LastName));
                var principal = new ClaimsPrincipal(identity);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                HttpContext.Session.SetString("EmailId", status.Email);
                return RedirectToAction("LandingPage", "Login");
            }
            TempData["Error Message"] = "Enter Correct details!";
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("LandingPage", "Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult LandingPage()
        {
            List<Mission> missions = _db.Missions.ToList();  
            foreach(var mission in missions)
            {
                _db.Entry(mission).Reference(c => c.City).Load();
                _db.Entry(mission).Reference(t => t.MissionTheme).Load();
            }
            return View(missions);
        }

        [HttpGet]
        public IActionResult MissionAndRating()
        {
            return View();
        }


        // ______________________________Forgot Controller_______________________________
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
                    TempData["Error"] = "Email id is invalid";
                    //return RedirectToAction("Login", "Login");
                    return View();
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


                var fromAddress = new MailAddress("pviral3011@gmail.com", "Viral Patel");
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

                return RedirectToAction("Login", "Login");
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

        //____________________ Registration Congtroller_____________
        [HttpGet]
        public IActionResult Registration()
        {
            User user = new User();
            return View(user);
        }


        
        [HttpPost]
        [Route("/Login/Registration", Name ="Register")]
        public IActionResult Registration(User user)
        {
            var validEmail = _db.Users.FirstOrDefault(u => u.Email == user.Email);
            if (validEmail == null)
            {
                _db.Users.Add(user);
                _db.SaveChanges();
                return RedirectToAction("Login", "Login");
            }
            else
            {
                TempData["Error"] = "Email is already Exist";
                return View();
            }
                //var User_data = new User()
                //{
                //    FirstName = obj.FirstName,
                //    LastName = obj.LastName,
                //    Email = obj.Email,
                //    PhoneNumber = obj.PhoneNumber,
                //    Password = obj.Password,
                //    CityId = 2,
                //    CountryId = 1
                //};

            //_db.Users.Add(User_data);
            //_db.SaveChanges();
            //return RedirectToAction("Login");

        }

       


        // ____________________ResetPassword Controller_________________
        public IActionResult ResetPassword(string Email,string Token)
        {
            var status = _db.PasswordResets.FirstOrDefault(m => m.Email == Email && m.Token == Token);
            if (status != null) { 
                var ResetPass= new ResetPasswordViewModel { 
                    Email = Email, 
                    Token = Token 
                };
                return View(ResetPass);
            }
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel rpm)
        {
            var user = _db.Users.FirstOrDefault(m => m.Email == rpm.Email );
            if (user != null)
            {
               user.Password=rpm.Password;
                _db.SaveChanges();
                return RedirectToAction("Login", "Login");
            }
            return View();
        }


       
    }
}
