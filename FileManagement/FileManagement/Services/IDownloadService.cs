using sharedfile.Models;
using System.IO;

namespace sharedfile.Services
{
    interface IDownloadService
    {
        public Ffile GetFileManagement(string ffid);
        public Stream DownloadFile(string userId, string guid, string fileUniqueName);
        public bool DeleteFile(string userId, string guid, string fileUniqueName);
    }
}
