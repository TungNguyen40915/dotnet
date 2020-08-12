using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace sharedfile.Services
{
    interface IUploadService
    {
        public bool UploadFile(string userId, IList<IFormFile> files, string folderId);

        public bool ValidationFiles(IList<IFormFile> files);
    }
}
