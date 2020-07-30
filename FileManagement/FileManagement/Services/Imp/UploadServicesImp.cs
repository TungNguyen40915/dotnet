using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using sharedfile.Commons;
using sharedfile.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace sharedfile.Services.Imp
{
    public class UploadServicesImp : IUploadService
    {
        private MyContext _context;
        private readonly IConfiguration _config;

        public UploadServicesImp(MyContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public bool UploadFile(string userId, IList<IFormFile> files)
        {
            try
            {
                var accountName = _config.GetValue<string>(Constants.ACCOUNT_NAME);
                var accessKey = _config.GetValue<string>(Constants.ACCESS_KEY);
                var credential = new StorageCredentials(accountName, accessKey);
                var storageAccount = new CloudStorageAccount(credential, true);

                //blob
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                //container
                CloudBlobContainer container = blobClient.GetContainerReference(_config.GetValue<string>(Constants.CONTAINER_NAME));

                container.CreateIfNotExistsAsync().Wait();


                foreach (IFormFile file in files)
                {
                    UploadFile(userId, file, container);
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool ValidationFiles(IList<IFormFile> files)
        {
            try
            {
                List<string> validExtension = _config.GetSection(Constants.VALID_EXTENSION).Get<List<string>>();
                int maxFileSize = _config.GetValue<int>(Constants.MAX_FILE_SIZE);
                int maxFileCount = _config.GetValue<int>(Constants.MAX_FILE_COUNT);
                int maxLengthFileName = _config.GetValue<int>(Constants.MAX_LENGTH_FILE_NAME);

                string extension;
                var fileNames = new List<string>();

                if (files.Count > maxFileCount)
                {
                    return false;
                }

                foreach (IFormFile file in files)
                {
                    extension = Path.GetExtension(file.FileName).Replace(".", "");

                    if (!validExtension.Contains(extension.ToLower()) || !file.FileName.Contains("."))
                    {
                        return false;
                    }

                    if (file.Length == 0)
                    {
                        return false;
                    }

                    if (file.Length > (maxFileSize * 1024 * 1024))
                    {
                        return false;
                    }

                    if (fileNames.Contains(file.FileName))
                    {
                        return false;
                    }

                    fileNames.Add(file.FileName);
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private void UploadFile(string userId, IFormFile uploadFile, CloudBlobContainer container)
        {
            string fileGUID = "";
            try
            {
                var extension = Path.GetExtension(uploadFile.FileName);

                string generatedName = System.Guid.NewGuid().ToString() + extension;

                CloudBlockBlob blob = container.GetBlockBlobReference(generatedName);
                blob.Properties.ContentType = uploadFile.ContentType;

                blob.UploadFromStream(uploadFile.OpenReadStream());

                var blobUrl = blob.Uri.AbsoluteUri;


                Ffile fileManagement = new Ffile();
                fileManagement.BlobUrl = blobUrl;
                fileManagement.FileName = uploadFile.FileName;
                fileManagement.UserId = userId;
                fileManagement.FileUniqueName = generatedName;
                fileManagement.FileSize = uploadFile.Length;
                fileManagement.DeleteFlag = false;
                fileManagement.UploadedDate = System.DateTime.Now;
                _context.Files.Add(fileManagement);
                _context.SaveChanges();


                fileGUID = fileManagement.GUID;

                new LogServicesImp(_context, _config).SaveLog(
                    userId,
                    _config.GetValue<string>(Constants.DIVISION_UPLOAD),
                    fileGUID,
                    _config.GetValue<string>(Constants.LOG_TYPE_NORMAL),
                    null
                    );

            }
            catch (Exception e)
            {
                try
                {
                    new LogServicesImp(_context, _config).SaveLog(
                        userId,
                         _config.GetValue<string>(Constants.DIVISION_UPLOAD),
                         fileGUID,
                         _config.GetValue<string>(Constants.LOG_TYPE_ERROR),
                         (uploadFile != null ? uploadFile.FileName : "") + " | " + e.ToString()
                        );
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.ToString());
                }
                finally
                {
                    throw e;
                }
            }
        }
    }
}
