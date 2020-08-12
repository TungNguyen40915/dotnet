
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using sharedfile.Commons;
using sharedfile.Models;
using sharedfile.Services;
using sharedfile.Services.Imp;

namespace sharedfile.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;
        private readonly IConfiguration _config;

        public HomeController(MyContext context, IConfiguration config)
        {
            _context = context;
            _config = config;

        }

        public ActionResult Index()
        {
            IUserService _us = new UserServicesImpl(_context, _config);
            string token = HttpContext.Session.GetString("token");

            if (_us.ValidateCurrentToken(token))
            {
                string username = _us.GetClaim(token, "userId");

                ISearchService _ss = new SearchServiceImp(_context, _config);
                ViewBag.files = _ss.GetFiles(username, null);

                ILogService _ls = new LogServicesImp(_context, _config);
                ViewBag.logs = _ls.getLog(username);

                IFolderService _fs = new FolderServicesImp(_context, _config);
                ViewBag.folders = _fs.GetAllFoldersByUserId(username);

                return View();
            }
            else
            {
                return Redirect("~/Login");
            }
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("~/Login");
        }
    }
}