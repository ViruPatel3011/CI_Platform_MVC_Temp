using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Registration.Datamodel.DataModels;
using Registration.Datamodel.ViewModels;

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
                return RedirectToAction("Privacy","Home");
            }
            else
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        
        public IActionResult Forgot()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Forgot(LoginViewModel lvm)
        {
            var status = _db.Users.Where(x => x.Email == lvm.Email).FirstOrDefault();
            if (status != null)
            {
                return RedirectToAction("ResetPassword","Login");
            }
            else
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Registration()
        {

            return View();
        }

        [HttpPost]
        [Route("/Login/Registration", Name ="Register")]
        public IActionResult Registration(User obj)
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
        

        public IActionResult ResetPassword()
        {
            return View();
        }
    }
}
