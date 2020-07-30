using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using sharedfile.Commons;
using sharedfile.Models;
using sharedfile.Services;
using sharedfile.Services.Imp;
using System;
using System.IO;

namespace sharedfile.Controllers
{
    public class DownLoadController : Controller
    {
        private MyContext _context;
        private readonly IConfiguration _config;
        private readonly ILog _logger = LogManager.GetLogger(typeof(DownLoadController));

        public DownLoadController(MyContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public ActionResult Index()
        {
            try
            {
                string guid = Request.Query[Constants.FIID];
                string userId = Request.Query[Constants.USER_ID];
                ViewBag.fiid = !string.IsNullOrEmpty(guid) ? guid : null;
                ViewBag.UserId = userId;


                IDownloadService _ds = new DownloadServicesImp(_context, _config);
                var file = _ds.GetFileManagement(guid);
                if (file == null)
                    return View(Constants.ERROR_PATH);
                return View(file);
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
                return View(Constants.ERROR_PATH);
            }
        }

        public ActionResult Download()
        {
            try
            {
                string guid = Request.Query[Constants.FIID];
                string userId = Request.Query[Constants.USER_ID];

                IDownloadService _ds = new DownloadServicesImp(_context, _config);

                var file = _ds.GetFileManagement(guid);
                if (file == null) return View(Constants.ERROR_PATH);

                Stream fileBytes = _ds.DownloadFile(userId, file.GUID, file.FileUniqueName);
                if (fileBytes == null) return View(Constants.ERROR_PATH);

                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, file.FileName);
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
                return View(Constants.ERROR_PATH);
            }
        }




    }
}