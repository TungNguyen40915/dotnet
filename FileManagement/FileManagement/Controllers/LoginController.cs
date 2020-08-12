using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sharedfile.Services;
using sharedfile.Services.Imp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using sharedfile.Models;
using Microsoft.AspNetCore.Http;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace sharedfile.Controllers
{
    public class LoginController : Controller
    {
        private MyContext _context;
        private readonly IConfiguration _config;

        public LoginController(MyContext context, IConfiguration config)
        {
            _context = context;
            _config = config;

        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Authenticate(string username, string password)
        {
            IUserService _us = new UserServicesImpl(_context, _config);

            if (_us.login(username, password) == null)
                return Json(new { success = false, message = "Login Failed" });
            else
            {
                HttpContext.Session.SetString("token", _us.generateToken(username));
                return Json(new { success = true });
            }
        }
    }
}
