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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace sharedfile.Controllers
{
    /// <summary>
    /// Log Controller
    /// </summary>
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


        /// <summary>
        /// デフォルトのビューを返す
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            try
            {
                string userId = Request.Query[Constants.USER_ID];
                string accessToken = Request.Query[Constants.ACCESS_TOKEN];
                string logAuthority = Request.Query[Constants.LOG_AUTHORITY];

                //AuthorizeUserを確認する
                if (!Common.AuthorizeUserKanri(_config, userId, accessToken, logAuthority)) return View(Constants.ERROR_PATH);

                ViewBag.uid = !string.IsNullOrEmpty(userId) ? userId : null;
                ViewBag.at = !string.IsNullOrEmpty(accessToken) ? accessToken : null;
                ViewBag.ro = !string.IsNullOrEmpty(logAuthority) ? logAuthority : null;
                ViewBag.maxLengthSearchFileName = _config.GetValue<int>(Constants.MAX_LENGTH_SEARCH_FILENAME);
                ViewBag.maxLengthSearchUserId = _config.GetValue<int>(Constants.MAX_LENGTH_SEARCH_USERID);

                ViewBag.data = new SearchLogViewModelList
                {
                    modelList = null,
                    pageCount = 1,
                    currentPage = 1,
                    totalRecords = 0
                };

                //デフォルトビュー
                return View();
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
                return View(Constants.ERROR_PATH);
            }
        }

        /// <summary>
        /// ユーザーが検索ボタンを押したとき
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileName"></param>
        /// <param name="upload"></param>
        /// <param name="download"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public IActionResult Search()
        {
            try
            {
                string uid = Request.Query[Constants.USER_ID];
                string at = Request.Query[Constants.ACCESS_TOKEN];
                string logAuthority = Request.Query[Constants.LOG_AUTHORITY];
                string userId = Request.Query["userId"];
                string fileName = Request.Query[Constants.FILE_NAME];
                bool upload = Convert.ToBoolean(Request.Query["upload"]);
                bool download = Convert.ToBoolean(Request.Query["download"]);
                string fromDate = Request.Query["fromDate"];
                string toDate = Request.Query["toDate"];
                string currentPage = Request.Query["page"];
                int page = !string.IsNullOrEmpty(currentPage) && Convert.ToInt32(currentPage) > 0 ? Convert.ToInt32(currentPage) : 1;

                //権限をチェックするために、AuthorizeUserメソッド呼び出します
                if (!Common.AuthorizeUserKanri(_config, uid, at, logAuthority)) return View(Constants.ERROR_PATH);

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

                ViewBag.uid = !string.IsNullOrEmpty(uid) ? uid : null;
                ViewBag.at = !string.IsNullOrEmpty(at) ? at : null;
                ViewBag.ro = !string.IsNullOrEmpty(logAuthority) ? logAuthority : null;
                ViewBag.maxLengthSearchFileName = _config.GetValue<int>(Constants.MAX_LENGTH_SEARCH_FILENAME);
                ViewBag.maxLengthSearchUserId = _config.GetValue<int>(Constants.MAX_LENGTH_SEARCH_USERID);

                return View("Index");
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
                return View(Constants.ERROR_PATH);
            }
        }


        /// <summary>
        /// ユーザーがファイルのエクスポートボタンを押したとき
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileName"></param>
        /// <param name="upload"></param>
        /// <param name="download"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IActionResult ExportFile()
        {
            try
            {
                string uid = Request.Query[Constants.USER_ID];
                string at = Request.Query[Constants.ACCESS_TOKEN];
                string userId = Request.Query["userId"];
                string fileName = Request.Query[Constants.FILE_NAME];
                bool upload = Convert.ToBoolean(Request.Query["upload"]);
                bool download = Convert.ToBoolean(Request.Query["download"]);
                string fromDate = Request.Query["fromDate"];
                string toDate = Request.Query["toDate"];
                string type = Request.Query["type"];
                string logAuthority = Request.Query[Constants.LOG_AUTHORITY];

                //権限をチェックするために、AuthorizeUserメソッド呼び出します
                if (!Common.AuthorizeUserKanri(_config, uid, at, logAuthority)) return View(Constants.ERROR_PATH);

                ILogService _ls = new LogServicesImp(_context, _config);

                //ファイルデータを準備する
                var logList = _ls.searchLog(userId, fileName, upload, download, fromDate, toDate);

                //ファイル名を準備する
                string exportFileName = "ログ" + DateTime.Now.ToString("yyyyMMddhhmmss");

                if (type == "csv")
                {
                    //ファイルのコンテンツを準備する
                    var builder = new StringBuilder();

                    //ファイルヘッダーを準備する
                    builder.Append(_config.GetValue<string>(Constants.HEADER_OPERATION_DATE));
                    builder.Append(",");
                    builder.Append(_config.GetValue<string>(Constants.HEADER_USER_ID));
                    builder.Append(",");
                    builder.Append(_config.GetValue<string>(Constants.HEADER_LOG_TYPE));
                    builder.Append(",");
                    builder.Append(_config.GetValue<string>(Constants.HEADER_FILE_NAME));
                    builder.Append(Environment.NewLine);

                    //ファイルにデータを追加する
                    foreach (var logElement in logList)
                    {
                        builder.AppendLine(
                            $"{logElement.log.OperationDate}," +
                            $"{logElement.log.UserId}," +
                            $"{(logElement.log.Division == "1" ? _config.GetValue<string>(Constants.UPLOAD_KEY) : _config.GetValue<string>(Constants.DOWNLOAD_KEY))}," +
                            $"{logElement.file.FileName}"
                            );
                    }

                    //作成したファイルを返す
                    var data = Encoding.UTF8.GetBytes(builder.ToString());
                    var content = Encoding.UTF8.GetPreamble().Concat(data).ToArray();
                    return File(content, "text/csv", exportFileName + ".csv");
                }
                else if (type == "excel")
                {
                    using (var workbook = new XLWorkbook())
                    {
                        //ファイルのコンテンツを準備する
                        var worksheet = workbook.Worksheets.Add("Sheet 1");
                        var currentRow = 1;

                        //ファイルヘッダーを準備する
                        worksheet.Cell(currentRow, 1).Value = _config.GetValue<string>(Constants.HEADER_OPERATION_DATE);
                        worksheet.Cell(currentRow, 2).Value = _config.GetValue<string>(Constants.HEADER_USER_ID);
                        worksheet.Cell(currentRow, 3).Value = _config.GetValue<string>(Constants.HEADER_LOG_TYPE);
                        worksheet.Cell(currentRow, 4).Value = _config.GetValue<string>(Constants.HEADER_FILE_NAME);

                        //ファイルにデータを追加する
                        foreach (var logElement in logList)
                        {
                            currentRow++;
                            worksheet.Cell(currentRow, 1).Value = logElement.log.OperationDate.ToString();
                            worksheet.Cell(currentRow, 2).Value = logElement.log.UserId;
                            worksheet.Cell(currentRow, 3).Value = logElement.log.Division == "1" ? _config.GetValue<string>(Constants.UPLOAD_KEY) : _config.GetValue<string>(Constants.DOWNLOAD_KEY);
                            worksheet.Cell(currentRow, 4).Value = logElement.file.FileName;
                        }

                        //作成したファイルを返す
                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            var content = stream.ToArray();

                            return File(
                                content,
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                exportFileName + ".xlsx");
                        }
                    }
                }

                return View();
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
                return View(Constants.ERROR_PATH);
            }

        }

    }
}
