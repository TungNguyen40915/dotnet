using sharedfile.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using sharedfile.Commons;
using sharedfile.Services;
using sharedfile.Services.Imp;
using System.Web;
using System;
using log4net;

namespace sharedfile.Controllers
{
    public class UploadController : Controller
    {
        private MyContext _context;
        private readonly IConfiguration _config;
        private readonly ILog _logger = LogManager.GetLogger(typeof(UploadController));

        public UploadController(MyContext context, IConfiguration config)
        {
            _context = context;
            _config = config;

        }

        public ActionResult Index()
        {
            try
            {
                ViewBag.validExtension = _config.GetSection(Constants.VALID_EXTENSION).Get<List<string>>();
                ViewBag.maxFileSize = _config.GetValue<int>(Constants.MAX_FILE_SIZE);
                ViewBag.maxFileCount = _config.GetValue<int>(Constants.MAX_FILE_COUNT);
                ViewBag.maxLengthFileName = _config.GetValue<int>(Constants.MAX_LENGTH_FILE_NAME);
                return View();
            }
            catch (Exception e)
            {
                return View(Constants.ERROR_PATH);
            }
        }

        [HttpPost]
        public ActionResult OnUpload(IList<IFormFile> files)
        {
            try
            {
                IUploadService _us = new UploadServicesImp(_context, _config);

                string userId = Request.Query[Constants.USER_ID];

                bool validate = _us.ValidationFiles(files);

                if (!validate)
                {
                    return Json(new { success = "false", message = "Something Went Wrong!" });
                }

                if (!_us.UploadFile(userId, files))
                {
                    return Json(new { success = "false", message = "Something Went Wrong!" });
                }

                return Json(new { success = "true" });
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
                return Json(new { success = "false", message = "Something Went Wrong!" });
            }
        }
    }

}