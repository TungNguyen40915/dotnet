using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using sharedfile.Models;
using sharedfile.Services;
using sharedfile.Services.Imp;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FileManagement.Controllers
{
    public class FolderController : Controller
    {
        private MyContext _context;
        private readonly IConfiguration _config;

        public FolderController(MyContext context, IConfiguration config)
        {
            _context = context;
            _config = config;

        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            IUserService _us = new UserServicesImpl(_context, _config);
            string token = HttpContext.Session.GetString("token");
            string folderId = Request.Query["id"];

            if (_us.ValidateCurrentToken(token))
            {
                string username = _us.GetClaim(token, "userId");

                ISearchService _ss = new SearchServiceImp(_context, _config);
                ViewBag.files = _ss.GetFiles(username, folderId, 0);

                IFolderService _fs = new FolderServicesImp(_context, _config);
                Folder folder = _fs.GetFolderById(folderId);
                ViewBag.FolderName = folder.FolderName;
                ViewBag.FolderId = folder.GUID;

                return View();
            }
            else
            {
                return Redirect("~/Login");
            }
        }

        // GET: /<controller>/
        [HttpGet]
        public IActionResult CreateFolder()
        {
            IUserService _us = new UserServicesImpl(_context, _config);
            string token = HttpContext.Session.GetString("token");
            string name = Request.Query["name"];

            if (_us.ValidateCurrentToken(token))
            {
                string username = _us.GetClaim(token, "userId");

                IFolderService _fs = new FolderServicesImp(_context, _config);

                return Json(new { success = _fs.CreateFolder(name, username) });

            }
            else
            {
                return Redirect("~/Login");
            }
        }
    }
}
