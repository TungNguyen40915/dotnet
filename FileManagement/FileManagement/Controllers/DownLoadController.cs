using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using sharedfile.Commons;
using sharedfile.Models;
using sharedfile.Services;
using sharedfile.Services.Imp;
using System;
using System.IO;

/// <summary>
/// DownLoad Controller
/// </summary>
namespace sharedfile.Controllers
{
    public class DownLoadController : Controller
    {
        private MyContext _context;

        private readonly IConfiguration _config;

        private readonly ILog _logger = LogManager.GetLogger(typeof(DownLoadController));
        /// <summary>
        /// DownLoad Controller
        /// </summary>
        /// <param name="context"></param>
        /// <param name="config"></param>
        public DownLoadController(MyContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        /// <summary>
        /// 初期ダウンロード処理
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                //URLパラメータを取得
                string userId = Request.Query[Constants.USER_ID];
                string accessToken = Request.Query[Constants.ACCESS_TOKEN];
                string guid = Request.Query[Constants.FIID];
                ViewBag.uid = !string.IsNullOrEmpty(userId) ? userId : null;
                ViewBag.at = !string.IsNullOrEmpty(accessToken) ? accessToken : null;
                ViewBag.fiid = !string.IsNullOrEmpty(guid) ? guid : null;

                //権限をチェックするために、AuthorizeUserメソッド呼び出します
                if (!(Common.AuthorizeUser(_config, userId, accessToken)))
                {
                    return View(Constants.ERROR_PATH);
                }
                //ダウンロードファイル
                IDownloadService _ds = new DownloadServicesImp(_context, _config);
                var file = _ds.GetFileManagement(userId, guid);
                if (file == null) return View(Constants.ERROR_PATH);
                ViewBag.usertype = _config.GetValue<int>(Constants.USER_TYPE);
                return View(file);
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
                return View(Constants.ERROR_PATH);
            }
        }

        /// <summary>
        /// ダウンロードファイル
        /// </summary>
        /// <returns></returns>
        public ActionResult Download()
        {
            try
            {
                string guid = Request.Query[Constants.FIID];

                IDownloadService _ds = new DownloadServicesImp(_context, _config);

                var file = _ds.GetFileManagement(guid);

                if (file == null) return View(Constants.ERROR_PATH);

                //Stream fileBytes = _ds.DownloadFile(userId, file.GUID, file.FileUniqueName, file.FileAreaGUID);
                //if (fileBytes == null) return View(Constants.ERROR_PATH);
                //return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, file.FileName);


                return View(Constants.ERROR_PATH);
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
                return View(Constants.ERROR_PATH);
            }
        }


    }
}