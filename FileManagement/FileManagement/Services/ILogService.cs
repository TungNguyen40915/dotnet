using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using sharedfile.Models;

namespace sharedfile.Services
{
    /// <summary>
    /// Interface Log Service
    /// </summary>
    interface ILogService
    {
        public IEnumerable<SearchLogViewModel> searchLog(string userId, string fileName, bool upload, bool download, string fromDate, string toDate);

        public void SaveLog(string userId, string division, string ffid, string guid, string logType, string error);
    }
}