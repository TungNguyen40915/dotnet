
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using sharedfile.Commons;
using sharedfile.Models;
using System;

namespace sharedfile.Controllers
{
    public class HomeController : Controller
    {

        private readonly IConfiguration _config;

        private readonly ILog _logger = LogManager.GetLogger(typeof(HomeController));

        public HomeController(IConfiguration config)
        {
            _config = config;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        /// <summary>
        /// 戻るD365
        /// </summary>
        /// <returns></returns>
        public ActionResult GoBackD365()
        {
            try
            {
                string userId = Request.Query[Constants.USER_ID];
                string action = Request.Query[Constants.ACTION_HEADER];
                string destinationUrl = _config.GetValue<string>(Constants.DESTINATION_URL);
                string d365Url = "";
                switch (action)
                {
                    case "Home":
                        d365Url = _config.GetValue<string>(Constants.PORTAL_URL_HOME);
                        break;
                    case "Questionnaire":
                        d365Url = _config.GetValue<string>(Constants.PORTAL_URL_QUESTIONNAIRE);
                        break;
                    case "BBS":
                        d365Url = _config.GetValue<string>(Constants.PORTAL_URL_BBS);
                        break;
                    case "FAQ":
                        d365Url = _config.GetValue<string>(Constants.PORTAL_URL_FAQ);
                        break;
                    case "File":
                        d365Url = _config.GetValue<string>(Constants.PORTAL_URL_FILE);
                        break;
                    case "Help":
                        d365Url = _config.GetValue<string>(Constants.PORTAL_URL_HELP);
                        break;
                    case "FileList":
                        d365Url = _config.GetValue<string>(Constants.PORTAL_URL_FILELIST);
                        break;
                    case "FileAreaDetails":
                        d365Url = _config.GetValue<string>(Constants.PORTAL_URL_FILEAREADETAILS);
                        break;
                    default:
                        return View(Constants.ERROR_PATH);
                }

                //一般的なGoBack365関数を呼び出してURLを取得する
                string url = Common.GoBackD365(userId, d365Url, destinationUrl);
                if (url != null)
                    //リダイレクトするURLを返す
                    return Redirect(url);
            }
            catch (Exception e)
            {
                //例外を処理する
                _logger.Error(e.ToString());
            }
            return View(Constants.ERROR_PATH);
        }

    }
}