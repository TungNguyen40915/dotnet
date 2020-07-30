using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using sharedfile.Models;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.OData.Edm;
using sharedfile.Commons;
using log4net;

namespace sharedfile.Services.Imp
{
    /// <summary>
    /// Log Service Implement
    /// </summary>
    public class LogServicesImp : ILogService
    {
        private MyContext _context;

        private readonly IConfiguration _config;

        private readonly ILog _logger = LogManager.GetLogger(typeof(LogServicesImp));

        public LogServicesImp(MyContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        /// <summary>
        /// 検索条件を満たすログを取得する
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileName"></param>
        /// <param name="upload"></param>
        /// <param name="download"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        IEnumerable<SearchLogViewModel> ILogService.searchLog(string userId, string fileName, bool upload, bool download, string fromDate, string toDate)
        {
            IEnumerable<SearchLogViewModel> logList = Enumerable.Empty<SearchLogViewModel>();

            if (!validate(fileName, userId, fromDate, toDate, upload, download)) throw new Exception();

            DateTime fromTime;
            DateTime toTime;

            userId = string.IsNullOrEmpty(userId) == false ? userId.Trim() : "";
            fileName = string.IsNullOrEmpty(fileName) == false ? fileName.Trim() : "";

            //ユーザーIDまたはファイル名が50文字を超えていないか確認してください
            if (userId.Length > 50 || fileName.Length > 50)
                return logList;

            //日付クエリからのハンドル
            if (string.IsNullOrEmpty(fromDate))
                fromTime = new DateTime();
            else
                fromTime = DateTime.Parse(fromDate);

            //日付クエリのハンドル
            if (string.IsNullOrEmpty(toDate))
                toTime = DateTime.Now;
            else
                toTime = DateTime.Parse(toDate);

            //開始日を今日より後にすることはできません
            if (fromTime > toTime)
                return logList;

            //検索条件を満たすデータを取得する
            if (upload != download)
            {
                string divisionUpload = _config.GetValue<string>(Constants.DIVISION_UPLOAD);
                string divisionDownload = _config.GetValue<string>(Constants.DIVISION_DOWNLOAD);
                logList = from log in _context.Logs
                          join file in _context.FileManagements on log.FileGUID equals file.GUID
                          where log.UserId.Contains(userId) && file.FileName.Contains(fileName) && fromTime.Date <= log.OperationDate.AddHours(9).Date && toTime.Date >= log.OperationDate.AddHours(9).Date && log.Division == (upload ? divisionUpload : divisionDownload) && log.LogType == _config.GetValue<string>(Constants.LOG_TYPE_NORMAL)
                          orderby log.OperationDate descending
                          select new SearchLogViewModel { log = log, file = file };
            }
            else
            {
                logList = from log in _context.Logs
                          join file in _context.FileManagements on log.FileGUID equals file.GUID
                          where log.UserId.Contains(userId) && file.FileName.Contains(fileName) && fromTime.Date <= log.OperationDate.AddHours(9).Date && toTime.Date >= log.OperationDate.AddHours(9).Date && log.LogType == _config.GetValue<string>(Constants.LOG_TYPE_NORMAL)
                          orderby log.OperationDate descending
                          select new SearchLogViewModel { log = log, file = file };
            }
            foreach (SearchLogViewModel item in logList)
            {
                if (item.file != null && item.file.UploadedDate != null) item.file.UploadedDate = item.file.UploadedDate.AddHours(9);
                if (item.log != null && item.log.OperationDate != null) item.log.OperationDate = item.log.OperationDate.AddHours(9);
            }

            return logList;
        }

        /// <summary>
        /// インプットを表現チェックする。
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="userId"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="upload"></param>
        /// <param name="download"></param>
        /// <returns>bool</returns>
        private bool validate(string fileName, string userId, string fromDate, string toDate, bool upload, bool download)
        {
            //すべてのパラメーターがnullです
            if (string.IsNullOrEmpty(fileName) && string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(fromDate) && string.IsNullOrEmpty(toDate) && !upload && !download) return false;
            //ファイル名が許容最大文字数を超えていないか確認してください。
            if (!string.IsNullOrEmpty(fileName) && fileName.Length > _config.GetValue<int>(Constants.MAX_LENGTH_SEARCH_FILENAME)) return false;
            //ユーザーIDが許可されている最大文字数を超えていないことを確認してください。
            if (!string.IsNullOrEmpty(userId) && userId.Length > _config.GetValue<int>(Constants.MAX_LENGTH_SEARCH_USERID)) return false;
            //日時Toより、日時Fromが超えているかをチェック行う。
            if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
            {
                DateTime dayFrom = DateTime.Parse(fromDate);
                DateTime dayTo = DateTime.Parse(toDate);
                if ((dayFrom - dayTo).TotalDays > 0) return false;
            }
            return true;
        }

        /// <summary>
        /// 管理テーブルに例外エラー内容を保存
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="division"></param>
        /// <param name="ffid"></param>
        /// <param name="guid"></param>
        /// <param name="logType"></param>
        /// <param name="error"></param>
        public void SaveLog(string userId, string division, string ffid, string guid, string logType, string error)
        {
            //例外が発生した場合
            //ログテーブルにレコードを挿入
            Log log = new Log();
            //ユーザーID(UserId)
            log.UserId = userId;
            //ファイル操作区分(Division)
            log.Division = division;
            //ファイル操作日時(OperationDate)：(現在時刻)
            log.OperationDate = DateTime.UtcNow;
            //ファイル領域ID(FileAreaGUID)
            log.FileAreaGUID = ffid;
            //ファイル情報GUID(FileGUID)
            log.FileGUID = guid;
            //エラーフラグ(LogType)
            log.LogType = logType;
            //エラーログ内容(ErrorLogTrace)
            log.ErrorLogTrace = error;
            //削除フラグ(DeleteFlag) = 0
            log.DeleteFlag = false;
            //レコードの挿入を開始
            _context.Logs.Add(log);
            _context.SaveChanges();
        }
    }

}
