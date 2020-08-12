using System.Collections.Generic;
using sharedfile.Models;

namespace sharedfile.Services
{
    interface ILogService
    {
        public List<SearchLogViewModel> searchLog(string userId, string fileName, bool upload, bool download, bool delete, string fromDate, string toDate);

        public List<SearchLogViewModel> getLog(string userId);

        public void SaveLog(string userId, string division, string guid, string logType, string error);
    }
}