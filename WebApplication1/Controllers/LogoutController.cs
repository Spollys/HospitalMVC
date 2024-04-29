using Hospital.Models;
using Hospital.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Hospital.Controllers
{
    public class LogoutController : Controller
    {
        private readonly ISessionService _sessionService;

        public LogoutController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public IActionResult Index()
        {
            var name = HttpContext.Session.GetString("Username");
            Trace.WriteLine($"{DateTime.Now:HH:mm:ss}: {name} left");
            _sessionService.RemoveUsername();
            return RedirectToAction("Index", "Home");
        }
    }

    public interface ISessionService
    {
        void RemoveUsername();
    }

    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void RemoveUsername()
        {
            _httpContextAccessor.HttpContext.Session.Remove("Username");
        }
    }
}
