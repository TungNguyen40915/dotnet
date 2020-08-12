using sharedfile.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using sharedfile.Commons;
using sharedfile.Services;
using sharedfile.Services.Imp;
using System;

namespace sharedfile.Controllers
{
    public class UploadController : Controller
    {
        private MyContext _context;
        private readonly IConfiguration _config;

        public UploadController(MyContext context, IConfiguration config)
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
            else
            {
                return Redirect("~/Login");
            }
        }

        [HttpPost]
        public ActionResult OnUpload(IList<IFormFile> files)
        {
            IUserService _us = new UserServicesImpl(_context, _config);
            string token = HttpContext.Session.GetString("token");
            string folderId = Request.Query["folderId"];

            if (_us.ValidateCurrentToken(token))
            {
                string username = _us.GetClaim(token, "userId");

                try
                {
                    IUploadService _uploadservice = new UploadServicesImp(_context, _config);

                    bool validate = _uploadservice.ValidationFiles(files);

                    if (!validate)
                    {
                        return Json(new { success = "false", message = "Unable to upload file" });
                    }

                    if (!_uploadservice.UploadFile(username, files, folderId))
                    {
                        return Json(new { success = "false", message = "Unable to upload file" });
                    }

                    return Json(new { success = "true" });
                }
                catch (Exception e)
                {
                    return Json(new { success = "false", message = "Unable to upload file" });
                }
            }
            else
            {
                return Json(new { success = "false", message = "Unable to upload file" });
            }
        }
    }

}