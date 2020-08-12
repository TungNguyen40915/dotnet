using System;
using System.Collections.Generic;
using sharedfile.Models;

namespace sharedfile.Services
{
    interface IFolderService
    {
        public List<Folder> GetAllFoldersByUserId(string userId);
        public Folder GetFolderById(string folderId);
        public bool CreateFolder(string name, string userId);
    }
}
