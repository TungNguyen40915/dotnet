using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using sharedfile.Models;
using System.Linq;
using sharedfile.Commons;
using log4net;

namespace sharedfile.Services.Imp
{

    public class LogServicesImp : ILogService
    {
        private MyContext _context;
        private readonly IConfiguration _config;

        public LogServicesImp(MyContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        IEnumerable<SearchLogViewModel> ILogService.searchLog(string userId, string fileName, bool upload, bool download, string fromDate, string toDate)
        {
            IEnumerable<SearchLogViewModel> logList = Enumerable.Empty<SearchLogViewModel>();

            if (!validate(fileName, userId, fromDate, toDate, upload, download)) throw new Exception();

            DateTime fromTime;
            DateTime toTime;

            userId = string.IsNullOrEmpty(userId) == false ? userId.Trim() : "";
            fileName = string.IsNullOrEmpty(fileName) == false ? fileName.Trim() : "";

            if (string.IsNullOrEmpty(fromDate))
                fromTime = new DateTime();
            else
                fromTime = DateTime.Parse(fromDate);

            if (string.IsNullOrEmpty(toDate))
                toTime = DateTime.Now;
            else
                toTime = DateTime.Parse(toDate);

            if (fromTime > toTime) throw new Exception();

            if (upload != download)
            {
                string divisionUpload = _config.GetValue<string>(Constants.DIVISION_UPLOAD);
                string divisionDownload = _config.GetValue<string>(Constants.DIVISION_DOWNLOAD);
                logList = from log in _context.Logs
                          join file in _context.Files on log.FileGUID equals file.GUID
                          where log.UserId.Contains(userId) && file.FileName.Contains(fileName) && fromTime.Date <= log.OperationDate.Date && toTime.Date >= log.OperationDate.Date && log.Division == (upload ? divisionUpload : divisionDownload) && log.LogType == _config.GetValue<string>(Constants.LOG_TYPE_NORMAL)
                          orderby log.OperationDate descending
                          select new SearchLogViewModel { log = log, file = file };
            }
            else
            {
                logList = from log in _context.Logs
                          join file in _context.Files on log.FileGUID equals file.GUID
                          where log.UserId.Contains(userId) && file.FileName.Contains(fileName) && fromTime.Date <= log.OperationDate.Date && toTime.Date >= log.OperationDate.Date && log.LogType == _config.GetValue<string>(Constants.LOG_TYPE_NORMAL)
                          orderby log.OperationDate descending
                          select new SearchLogViewModel { log = log, file = file };
            }

            return logList;
        }

        private bool validate(string fileName, string userId, string fromDate, string toDate, bool upload, bool download)
        {
            if (string.IsNullOrEmpty(fileName) && string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(fromDate) && string.IsNullOrEmpty(toDate) && !upload && !download) return false;
            return true;
        }


        public void SaveLog(string userId, string division, string guid, string logType, string error)
        {
            Log log = new Log();
            log.UserId = userId;
            log.Division = division;
            log.OperationDate = DateTime.Now;
            log.FileGUID = guid;
            log.LogType = logType;
            log.ErrorLogTrace = error;
            log.DeleteFlag = false;
            _context.Logs.Add(log);
            _context.SaveChanges();
        }
    }

}
