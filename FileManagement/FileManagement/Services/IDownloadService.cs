using sharedfile.Models;
using System.IO;

namespace sharedfile.Services
{
    /// <summary>
    /// Interface Download Service
    /// </summary>
    interface IDownloadService
    {
        public FileManagement GetFileManagement(string userId, string ffid);
        public FileManagement GetFileManagement(string ffid);
        public Stream DownloadFile(string userId, string ffid, string fileUniqueName, string fileAreaGUID);
    }
}
