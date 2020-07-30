using System.Collections.Generic;
using sharedfile.Models;

namespace sharedfile.Services
{
    interface ILogService
    {
        public IEnumerable<SearchLogViewModel> searchLog(string userId, string fileName, bool upload, bool download, string fromDate, string toDate);

        public void SaveLog(string userId, string division, string guid, string logType, string error);
    }
}