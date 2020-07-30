using log4net;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using sharedfile.Commons;
using sharedfile.Models;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace sharedfile.Services.Imp
{
    /// <summary>
    /// Download Services Implement
    /// </summary>
    public class DownloadServicesImp : IDownloadService
    {
        private MyContext _context;

        private readonly IConfiguration _config;

        private readonly ILog _logger = LogManager.GetLogger(typeof(DownloadServicesImp));
        public DownloadServicesImp(MyContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        /// <summary>
        /// ファイル情報を取得する
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>FileManagement</returns>
        public FileManagement GetFileManagement(string userId, string guid)
        {
            try
            {
                var file = GetFileManagement(guid);
                if (file == null) return null;
                file.UploadedDate = file.UploadedDate.AddHours(9);

                //int usertype = _config.GetValue<int>(Constants.USER_TYPE);
                //if (usertype == 2)
                //{
                //    string url = Sanitize(file.SanitizedToken, userId, file.FileAreaGUID, guid);
                //    if (string.IsNullOrEmpty(url))
                //    {
                //        return null;
                //    }
                //    file.BlobUrl = url;
                //}
                //else
                //{
                //    file.BlobUrl = null;
                //}

                return file;
            }
            catch (Exception e)
            {
                try
                {
                    //例外を投げる
                    string divisionDownload = _config.GetValue<string>(Constants.DIVISION_DOWNLOAD);
                    string logTypeError = _config.GetValue<string>(Constants.LOG_TYPE_ERROR);
                    new LogServicesImp(_context, _config).SaveLog(userId, divisionDownload, null, guid, logTypeError, e.ToString());
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.ToString());
                }
                _logger.Error(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// キュメント情報テーブルの該当ファイル情報を検索します。
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>FileManagement</returns>
        public FileManagement GetFileManagement(string guid)
        {
            //キュメント情報テーブルの該当ファイル情報を検索します
            return _context.FileManagements.Where(f => f.GUID == guid).FirstOrDefault();
        }

        /// <summary>
        /// 無害化のAPIを呼び出し
        /// </summary>
        /// <param name="sanitizedToken"></param>
        /// <returns>bool</returns>
        private string Sanitize(string sanitizedToken, string userId, string fileAreaGUID, string guid)
        {
            string tenantid = _config.GetValue<string>(Constants.TENANT_ID);
            string lgwanflg = _config.GetValue<string>(Constants.LGWANFLG);
            //無害化ダウンロード処理を実行
            var download = new Download(sanitizedToken, tenantid, lgwanflg);
            string json = JsonConvert.SerializeObject(download);
            string url = _config.GetValue<string>(Constants.SANITIZE_DOWNLOAD_URL);

            HttpResponseMessage responseData = Common.SendRequest(url, json);
            // リクエスト送信して、タスクの返りを待つ
            if (responseData.StatusCode.ToString().Equals(Constants.OK))
            {
                //ログを保存
                string divisionDownload = _config.GetValue<string>(Constants.DIVISION_DOWNLOAD);
                string logTypeNormal = _config.GetValue<string>(Constants.LOG_TYPE_NORMAL);
                new LogServicesImp(_context, _config).SaveLog(userId, divisionDownload, fileAreaGUID, guid, logTypeNormal, null);
                return responseData.Content.ReadAsStringAsync().Result;
            }
            else
            {
                //例外を投げる
                var responseCode = responseData.StatusCode.ToString();
                var responseMessage = responseData.Content == null ? "" : responseData.Content.ReadAsStringAsync().Result;
                string divisionDownload = _config.GetValue<string>(Constants.DIVISION_DOWNLOAD);
                string logTypeError = _config.GetValue<string>(Constants.LOG_TYPE_ERROR);
                _logger.Error("Sanitize: " + responseCode + responseMessage);
                new LogServicesImp(_context, _config).SaveLog(userId, divisionDownload, fileAreaGUID, guid, logTypeError, "Sanitize: " + responseCode + responseMessage);
            }
            return null;
        }

        /// <summary>
        /// userTypeがmhlw-fs-de-as（厚生労働省）の場合ダウンロードファイルを作成
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="guid"></param>
        /// <param name="fileUniqueName"></param>
        /// <param name="fileAreaGUID"></param>
        /// <returns>Stream</returns>
        public Stream DownloadFile(string userId, string guid, string fileUniqueName, string fileAreaGUID)
        {
            try
            {
                //appsettings.jsonからuserTypeを取得します
                int usertype = _config.GetValue<int>(Constants.USER_TYPE);
                if (usertype != 1) return null;

                //BlobおよびSQLストレージの認証情報を作成する
                var accountName = _config.GetValue<string>(Constants.ACCOUNT_NAME);
                var accessKey = _config.GetValue<string>(Constants.ACCESS_KEY);
                var credential = new StorageCredentials(accountName, accessKey);
                var storageAccount = new CloudStorageAccount(credential, true);

                //ブロブ
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                //コンテナ
                CloudBlobContainer container = blobClient.GetContainerReference(_config.GetValue<string>(Constants.CONTAINER_NAME));

                //ダウンロードするファイル名を指定
                CloudBlockBlob blockBlob_download = container.GetBlockBlobReference(fileUniqueName);
                Stream fileBytes = new MemoryStream();
                if (blobClient != null) blockBlob_download.DownloadToStream(fileBytes);
                fileBytes.Position = 0;
                //ログを保存
                string divisionDownload = _config.GetValue<string>(Constants.DIVISION_DOWNLOAD);
                string logTypeNormal = _config.GetValue<string>(Constants.LOG_TYPE_NORMAL);
                new LogServicesImp(_context, _config).SaveLog(userId, divisionDownload, fileAreaGUID, guid, logTypeNormal, null);
                return fileBytes;
            }
            catch (Exception e)
            {
                try
                {
                    //例外を投げる
                    string divisionDownload = _config.GetValue<string>(Constants.DIVISION_DOWNLOAD);
                    string logTypeError = _config.GetValue<string>(Constants.LOG_TYPE_ERROR);
                    new LogServicesImp(_context, _config).SaveLog(userId, divisionDownload, null, guid, logTypeError, e.ToString());
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.ToString());
                }
                _logger.Error(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Json object
        /// </summary>
        [JsonObject]
        public class Download
        {
            [JsonProperty("token")]
            public string Token { get; private set; }

            [JsonProperty("tenantid")]
            public string Tenantid { get; private set; }

            [JsonProperty("lgwanflg")]
            public string Lgwanflg { get; private set; }

            public Download(string token, string tenantid, string lgwanflg)
            {
                this.Token = token;
                this.Tenantid = tenantid;
                this.Lgwanflg = lgwanflg;
            }

        }
    }

}
