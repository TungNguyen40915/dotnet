using System;
using System.IO;
using System.Linq;
using System.Text;
using ClosedXML.Excel;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using sharedfile.Commons;
using sharedfile.Models;
using sharedfile.Services;
using sharedfile.Services.Imp;

namespace sharedfile.Controllers
{

    public class LogController : Controller
    {
        private MyContext _context;
        private readonly IConfiguration _config;

        public LogController(MyContext context, IConfiguration config)
        {
            _context = context;
            _config = config;

        }

        public IActionResult Index()
        {
            IUserService _us = new UserServicesImpl(_context, _config);
            string token = HttpContext.Session.GetString("token");

            if (_us.ValidateCurrentToken(token))
            {
                string username = _us.GetClaim(token, "userId");

                try
                {
                    ILogService _ss = new LogServicesImp(_context, _config);
                    var vm = _ss.searchLog(username, "", true, true, true, "", "");

                    ViewBag.totalRecords = vm.Count;
                    ViewBag.data = vm;
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


        public IActionResult Search()
        {
            IUserService _us = new UserServicesImpl(_context, _config);
            string token = HttpContext.Session.GetString("token");

            if (_us.ValidateCurrentToken(token))
            {
                string username = _us.GetClaim(token, "userId");

                try
                {
                    string fileName = Request.Query[Constants.FILE_NAME];
                    bool upload = Convert.ToBoolean(Request.Query["upload"]);
                    bool download = Convert.ToBoolean(Request.Query["download"]);
                    bool delete = Convert.ToBoolean(Request.Query["delete"]);
                    string fromDate = Request.Query["fromDate"];
                    string toDate = Request.Query["toDate"];


                    ILogService _ls = new LogServicesImp(_context, _config);

                    int logPerPage = _config.GetValue<int>(Constants.LOG_DISPLAY_PER_PAGE);

                    var logList = _ls.searchLog(username, fileName, upload, download, delete, fromDate, toDate);

                    ViewBag.fileName = fileName;
                    ViewBag.upload = upload;
                    ViewBag.download = download;
                    ViewBag.delete = delete;
                    ViewBag.fromDate = fromDate;
                    ViewBag.toDate = toDate;
                    ViewBag.totalRecords = logList.Count;
                    ViewBag.data = logList;

                    return View("Index");
                }
                catch
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
