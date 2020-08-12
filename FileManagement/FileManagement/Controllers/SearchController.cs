using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using sharedfile.Commons;
using sharedfile.Models;
using sharedfile.Services;
using sharedfile.Services.Imp;
using System;

namespace sharedfile.Controllers
{
    public class SearchController : Controller
    {
        private MyContext _context;
        private readonly IConfiguration _config;

        public SearchController(MyContext context, IConfiguration config)
        {
            this._context = context;
            this._config = config;
        }

        public ActionResult Index()
        {
            IUserService _us = new UserServicesImpl(_context, _config);
            string token = HttpContext.Session.GetString("token");

            if (_us.ValidateCurrentToken(token))
            {
                string username = _us.GetClaim(token, "userId");

                try
                {
                    ISearchService _ss = new SearchServiceImp(_context, _config);
                    var vm = _ss.SearchFiles("", "", "", username);

                    ViewBag.totalRecords = vm.Count;
                    ViewBag.files = vm;
                    return View("index", vm);
                }
                catch (Exception e)
                {
                    return View(Constants.ERROR_PATH);
                }
            }
            else
            {
                return Redirect("~/Login");
            }
        }
        public ActionResult Search()
        {
            IUserService _us = new UserServicesImpl(_context, _config);
            string token = HttpContext.Session.GetString("token");

            if (_us.ValidateCurrentToken(token))
            {
                string username = _us.GetClaim(token, "userId");

                try
                {
                    string fileName = Request.Query[Constants.FILE_NAME];
                    string uploadDateFrom = Request.Query[Constants.UPLOAD_DATE_FROM];
                    string uploadDateTo = Request.Query[Constants.UPLOAD_DATE_TO];


                    ViewBag.fileName = !string.IsNullOrEmpty(fileName) ? fileName : null;
                    ViewBag.uploadDateFrom = !string.IsNullOrEmpty(uploadDateFrom) ? uploadDateFrom : null;
                    ViewBag.uploadDateTo = !string.IsNullOrEmpty(uploadDateTo) ? uploadDateTo : null;


                    ISearchService _ss = new SearchServiceImp(_context, _config);
                    var vm = _ss.SearchFiles(fileName, uploadDateFrom, uploadDateTo, username);

                    ViewBag.totalRecords = vm.Count;
                    ViewBag.files = vm;
                    return View("index", vm);
                }
                catch (Exception e)
                {
                    return View(Constants.ERROR_PATH);
                }
            }
            else
            {
                return Redirect("~/Login");
            }
        }

    }
}