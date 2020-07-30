using log4net;
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
        private readonly ILog _logger = LogManager.GetLogger(typeof(SearchController));

        public SearchController(MyContext context, IConfiguration config)
        {
            this._context = context;
            this._config = config;
        }

        public ActionResult Index()
        {
            try
            {
                string userId = Request.Query[Constants.USER_ID];
                ViewBag.UserId = userId;

                ViewBag.MaxItems = 0;
                ViewBag.totalRecords = 0;
                ViewBag.currentPage = 1;
                ViewBag.pageCount = 0;
                return View();
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
                return View(Constants.ERROR_PATH);
            }
        }
        public ActionResult Search()
        {
            try
            {
                string fileName = Request.Query[Constants.FILE_NAME];
                string uploadDateFrom = Request.Query[Constants.UPLOAD_DATE_FROM];
                string uploadDateTo = Request.Query[Constants.UPLOAD_DATE_TO];
                string currentPage = Request.Query[Constants.CURRENT_PAGE];
                int page = !string.IsNullOrEmpty(currentPage) && Convert.ToInt32(currentPage) > 0 ? Convert.ToInt32(currentPage) : 1;

                string userId = Request.Query[Constants.USER_ID];
                ViewBag.UserId = userId;
                ViewBag.fileName = !string.IsNullOrEmpty(fileName) ? fileName : null;
                ViewBag.uploadDateFrom = !string.IsNullOrEmpty(uploadDateFrom) ? uploadDateFrom : null;
                ViewBag.uploadDateTo = !string.IsNullOrEmpty(uploadDateTo) ? uploadDateTo : null;
                ViewBag.currentPage = page;


                ISearchService _ss = new SearchServiceImp(_context, _config);
                var vm = _ss.SearchFiles(fileName, uploadDateFrom, uploadDateTo, page);

                ViewBag.MaxItems = vm.maxItemsSearchPage;
                ViewBag.totalRecords = vm.totalRecords;
                ViewBag.pageCount = vm.pageCount;
                return View("index", vm);
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
                return View(Constants.ERROR_PATH);
            }
        }

    }
}