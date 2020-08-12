using log4net;
using Microsoft.AspNetCore.Http;
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

        public DownLoadController(MyContext context, IConfiguration config)
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
                    string guid = Request.Query[Constants.FIID];
                    ViewBag.fiid = !string.IsNullOrEmpty(guid) ? guid : null;

                    IDownloadService _ds = new DownloadServicesImp(_context, _config);
                    var file = _ds.GetFileManagement(guid);
                    if (file == null)
                        return View(Constants.ERROR_PATH);
                    return View(file);
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

        public ActionResult Download()
        {
            IUserService _us = new UserServicesImpl(_context, _config);
            string token = HttpContext.Session.GetString("token");
            string action = Request.Query["action"];

            if (_us.ValidateCurrentToken(token))
            {
                string username = _us.GetClaim(token, "userId");

                try
                {
                    string guid = Request.Query[Constants.FIID];
                    IDownloadService _ds = new DownloadServicesImp(_context, _config);
                    var file = _ds.GetFileManagement(guid);
                    if (file == null) return View(Constants.ERROR_PATH);


                    if (action == "download")
                    {
                        Stream fileBytes = _ds.DownloadFile(username, file.GUID, file.FileUniqueName);
                        if (fileBytes == null) return View(Constants.ERROR_PATH);

                        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, file.FileName);
                    }

                    else if (action == "delete")
                    {
                        if (_ds.DeleteFile(username, file.GUID, file.FileUniqueName))
                            return Redirect("~/home");
                        else
                            return View(Constants.ERROR_PATH);
                    }
                    else
                    {
                        return View(Constants.ERROR_PATH);
                    }
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