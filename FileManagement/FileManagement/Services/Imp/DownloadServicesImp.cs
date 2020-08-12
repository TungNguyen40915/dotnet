using log4net;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using sharedfile.Commons;
using sharedfile.Models;
using System;
using System.IO;
using System.Linq;

namespace sharedfile.Services.Imp
{
    public class DownloadServicesImp : IDownloadService
    {
        private MyContext _context;
        private readonly IConfiguration _config;

        public DownloadServicesImp(MyContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }


        public Ffile GetFileManagement(string guid)
        {
            return _context.Files.Where(f => f.GUID == guid).FirstOrDefault();
        }


        public Stream DownloadFile(string userId, string guid, string fileUniqueName)
        {
            try
            {
                var accountName = _config.GetValue<string>(Constants.ACCOUNT_NAME);
                var accessKey = _config.GetValue<string>(Constants.ACCESS_KEY);
                var credential = new StorageCredentials(accountName, accessKey);
                var storageAccount = new CloudStorageAccount(credential, true);

                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(_config.GetValue<string>(Constants.CONTAINER_NAME));

                CloudBlockBlob blockBlob_download = container.GetBlockBlobReference(fileUniqueName);
                Stream fileBytes = new MemoryStream();
                if (blobClient != null) blockBlob_download.DownloadToStream(fileBytes);
                fileBytes.Position = 0;

                string divisionDownload = _config.GetValue<string>(Constants.DIVISION_DOWNLOAD);
                string logTypeNormal = _config.GetValue<string>(Constants.LOG_TYPE_NORMAL);
                new LogServicesImp(_context, _config).SaveLog(userId, divisionDownload, guid, logTypeNormal, null);
                return fileBytes;
            }
            catch (Exception e)
            {
                try
                {
                    string divisionDownload = _config.GetValue<string>(Constants.DIVISION_DOWNLOAD);
                    string logTypeError = _config.GetValue<string>(Constants.LOG_TYPE_ERROR);
                    new LogServicesImp(_context, _config).SaveLog(userId, divisionDownload, guid, logTypeError, e.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                return null;
            }
        }

        public bool DeleteFile(string userId, string guid, string fileUniqueName)
        {
            try
            {
                var accountName = _config.GetValue<string>(Constants.ACCOUNT_NAME);
                var accessKey = _config.GetValue<string>(Constants.ACCESS_KEY);
                var credential = new StorageCredentials(accountName, accessKey);
                var storageAccount = new CloudStorageAccount(credential, true);

                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(_config.GetValue<string>(Constants.CONTAINER_NAME));
                CloudBlockBlob blob = container.GetBlockBlobReference(fileUniqueName);

                var file = (from f in _context.Files
                            where f.FileUniqueName == fileUniqueName
                            select f).FirstOrDefault();

                file.DeleteFlag = true;
                _context.SaveChanges();


                string divisionDelete = _config.GetValue<string>(Constants.DIVISION_DELETE);
                string logTypeNormal = _config.GetValue<string>(Constants.LOG_TYPE_NORMAL);
                new LogServicesImp(_context, _config).SaveLog(userId, divisionDelete, guid, logTypeNormal, null);

                blob.DeleteIfExists();

                return true;
            }
            catch (Exception e)
            {
                try
                {
                    string divisionDelete = _config.GetValue<string>(Constants.DIVISION_DELETE);
                    string logTypeError = _config.GetValue<string>(Constants.LOG_TYPE_ERROR);
                    new LogServicesImp(_context, _config).SaveLog(userId, divisionDelete, guid, logTypeError, e.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                return false;
            }

        }
    }

}
