using System.Data;
using System.Diagnostics;
using Hospital.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;

namespace StudentsWebApp.Controllers
{
    public class SignupController : Controller
    {
        private readonly IAntiforgery _antiforgery;

        public SignupController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public IActionResult Index()
        {
            // Set the anti-forgery token for the view
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            ViewData["AntiforgeryToken"] = tokens.FormToken;

            return View();
        }

        [HttpPost]
        public IActionResult Index(User newUser, string antiforgeryToken)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (UserProvider.HasAccount(newUser.Name))
            {
                ModelState.AddModelError("name", $"Користувач {newUser.Name} вже зареєстрований. Спробуйте інше ім'я");
            }
            else if (UserProvider.TryAddUser(newUser))
            {
                Trace.WriteLine($"{DateTime.Now:HH:mm:ss}: \"{newUser.Name}\" is registered");
                HttpContext.Session.SetString("Username", newUser.Name);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("name", $"Збій системи. Спробуйте зареєструватися пізніше");
            }

            // Set the anti-forgery token for the view
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            ViewData["AntiforgeryToken"] = tokens.FormToken;

            return View();
        }
    }
}
