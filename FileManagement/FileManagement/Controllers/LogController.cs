using System;
using System.IO;
using System.Linq;
using System.Text;
using ClosedXML.Excel;
using log4net;
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
        private readonly ILog _logger = LogManager.GetLogger(typeof(LogController));

        public LogController(MyContext context, IConfiguration config)
        {
            _context = context;
            _config = config;

        }

        public IActionResult Index()
        {
            try
            {
                ViewBag.data = new SearchLogViewModelList
                {
                    modelList = null,
                    pageCount = 1,
                    currentPage = 1,
                    totalRecords = 0
                };

                return View();
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
                return View(Constants.ERROR_PATH);
            }
        }


        public IActionResult Search()
        {
            try
            {
                string userId = Request.Query["userId"];
                string fileName = Request.Query[Constants.FILE_NAME];
                bool upload = Convert.ToBoolean(Request.Query["upload"]);
                bool download = Convert.ToBoolean(Request.Query["download"]);
                string fromDate = Request.Query["fromDate"];
                string toDate = Request.Query["toDate"];
                string currentPage = Request.Query["page"];
                int page = !string.IsNullOrEmpty(currentPage) && Convert.ToInt32(currentPage) > 0 ? Convert.ToInt32(currentPage) : 1;

                ILogService _ls = new LogServicesImp(_context, _config);

                int logPerPage = _config.GetValue<int>(Constants.LOG_DISPLAY_PER_PAGE);

                var logList = _ls.searchLog(userId, fileName, upload, download, fromDate, toDate);

                int start = (page - 1) * logPerPage;

                var searchView = new SearchLogViewModelList
                {
                    modelList = logList.ToList().Skip(start).Take(logPerPage),
                    pageCount = Convert.ToInt32(Math.Ceiling(logList.ToList().Count() / (double)logPerPage)),
                    currentPage = page,
                    totalRecords = logList.Count()
                };

                ViewBag.userId = userId;
                ViewBag.fileName = fileName;
                ViewBag.upload = upload;
                ViewBag.download = download;
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                ViewBag.data = searchView;

                return View("Index");
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
                return View(Constants.ERROR_PATH);
            }
        }
    }
}
