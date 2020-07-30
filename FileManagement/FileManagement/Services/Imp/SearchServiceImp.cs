using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using sharedfile.Commons;
using sharedfile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace sharedfile.Services.Imp
{
    /// <summary>
    /// Search Service Implement
    /// </summary>
    public class SearchServiceImp : ISearchService
    {
        private MyContext _context;

        private readonly IConfiguration _config;

        private static HttpClient client = new HttpClient();

        public SearchServiceImp(MyContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        /// <summary>
        /// 検索ファイル
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="uploadDayFrom"></param>
        /// <param name="uploadDayTo"></param>
        /// <param name="keyWord"></param>
        /// <returns>List<FileManagement></returns>
        public SearchVM SearchFiles(string fileName, string uploadDayFrom, string uploadDayTo, string keyWord, int currentPage)
        {
            List<FileManagement> files = new List<FileManagement>();
            List<string> fileNames = new List<string>();
            //検索条件が入力かどうかをチェック行う。
            if (!validate(fileName, uploadDayFrom, uploadDayTo, keyWord)) throw new Exception();

            if (!string.IsNullOrEmpty(keyWord))
            {
                //検索キーワードによってBLOBストレージからリストファイル名を取得します
                fileNames = SearchFileByKeyWord(keyWord);
            }
            //fileNameとuploadDateFromとuploadDateToの両方がnullです
            if (string.IsNullOrEmpty(fileName) && string.IsNullOrEmpty(uploadDayFrom) && string.IsNullOrEmpty(uploadDayTo) && fileNames.Count > 0)
            {
                //キーワード検索でファイルが戻ってきた場合
                files = _context.FileManagements.Where(x => fileNames.Contains(x.FileUniqueName)).OrderByDescending(x => x.UploadedDate).ToList();
            }
            //uploadDateFromとuploadDateToの両方が入力されている
            else if (!string.IsNullOrEmpty(uploadDayFrom) && !string.IsNullOrEmpty(uploadDayTo))
            {
                DateTime dayFrom = DateTime.Parse(uploadDayFrom);
                DateTime dayTo = DateTime.Parse(uploadDayTo);
                if (fileNames.Count > 0)
                {
                    //キーワード検索でファイルが戻ってきた場合
                    files = _context.FileManagements.Where(x => x.FileName.Contains(fileName) && x.UploadedDate.AddHours(9).Date >= dayFrom.Date
                                                             && x.UploadedDate.AddHours(9).Date <= dayTo.Date || fileNames.Contains(x.FileUniqueName)).OrderByDescending(x => x.UploadedDate).ToList();
                }
                else
                {
                    //キーワード検索でファイルが返ってこない場合
                    files = _context.FileManagements.Where(x => x.FileName.Contains(fileName) && x.UploadedDate.AddHours(9).Date >= dayFrom.Date && x.UploadedDate.AddHours(9).Date <= dayTo.Date).OrderByDescending(x => x.UploadedDate).ToList();
                }
            }
            //uploadDateToがnullで、uploadDateFromが入力されている
            else if (!string.IsNullOrEmpty(uploadDayFrom))
            {
                DateTime dayFrom = DateTime.Parse(uploadDayFrom);
                if (fileNames.Count > 0)
                {
                    //キーワード検索でファイルが戻ってきた場合
                    files = _context.FileManagements.Where(x => x.FileName.Contains(fileName) && x.UploadedDate.AddHours(9).Date >= dayFrom.Date
                                                             || fileNames.Contains(x.FileUniqueName)).OrderByDescending(x => x.UploadedDate).ToList();
                }
                else
                {
                    //キーワード検索でファイルが返ってこない場合
                    files = _context.FileManagements.Where(x => x.FileName.Contains(fileName) && x.UploadedDate.AddHours(9).Date >= dayFrom.Date).OrderByDescending(x => x.UploadedDate).ToList();
                }
            }
            //uploadDateFromがnullで、uploadDateToが入力されている
            else if (!string.IsNullOrEmpty(uploadDayTo))
            {
                DateTime dayTo = DateTime.Parse(uploadDayTo);
                if (fileNames.Count > 0)
                {
                    //キーワード検索でファイルが戻ってきた場合
                    files = _context.FileManagements.Where(x => x.FileName.Contains(fileName) && x.UploadedDate.AddHours(9).Date <= dayTo.Date
                                                             || fileNames.Contains(x.FileUniqueName)).OrderByDescending(x => x.UploadedDate).ToList();
                }
                else
                {
                    //キーワード検索でファイルが返ってこない場合
                    files = _context.FileManagements.Where(x => x.FileName.Contains(fileName) && x.UploadedDate.AddHours(9).Date <= dayTo.Date).OrderByDescending(x => x.UploadedDate).ToList();
                }
            }
            //uploadDateFromとuploadDateToの両方がnullです
            else if (!string.IsNullOrEmpty(fileName))
            {
                if (fileNames.Count > 0)
                {
                    //キーワード検索でファイルが戻ってきた場合
                    files = _context.FileManagements.Where(x => x.FileName.Contains(fileName) || fileNames.Contains(x.FileUniqueName)).OrderByDescending(x => x.UploadedDate).ToList();
                }
                else
                {
                    //キーワード検索でファイルが返ってこない場合
                    files = _context.FileManagements.Where(x => x.FileName.Contains(fileName)).OrderByDescending(x => x.UploadedDate).ToList();
                }

            }
            foreach (FileManagement file in files)
            {
                if (file.UploadedDate != null) file.UploadedDate = file.UploadedDate.AddHours(9);
            }
            return paging(files, currentPage);
        }
        /// <summary>
        /// ページング
        /// </summary>
        /// <param name="files"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        private SearchVM paging(List<FileManagement> files, int currentPage)
        {
            //appsettings.jsonで最大数の項目を取得します
            int maxItems = _config.GetValue<int>(Constants.MAX_ITEMS_SEARCH_PAGE);
            int start = (currentPage - 1) * maxItems;
            SearchVM vm = new SearchVM();
            //現在のページのファイルを取得
            vm.files = files.ToList().Skip(start).Take(maxItems).ToList();
            vm.currentPage = currentPage;
            vm.maxItemsSearchPage = maxItems;
            vm.totalRecords = files.Count();
            vm.pageCount = Convert.ToInt32(Math.Ceiling(files.Count() / (double)maxItems));
            return vm;
        }

        /// <summary>
        /// インプットを表現チェックする。
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="uploadDayFrom"></param>
        /// <param name="uploadDayTo"></param>
        /// <param name="keyWord"></param>
        /// <returns>bool</returns>
        private bool validate(string fileName, string uploadDayFrom, string uploadDayTo, string keyWord)
        {
            //すべてのパラメーターがnullです
            if (string.IsNullOrEmpty(fileName) && string.IsNullOrEmpty(uploadDayFrom) && string.IsNullOrEmpty(uploadDayTo) && string.IsNullOrEmpty(keyWord)) return false;
            //ファイル名は30文字を超えているかをチェックを行う。
            if (!string.IsNullOrEmpty(fileName) && fileName.Length > _config.GetValue<int>(Constants.MAX_LENGTH_SEARCH_FILENAME)) return false;
            //「キーワードを設定して表示する」は30文字を超えているかをチェック行う。
            if (!string.IsNullOrEmpty(keyWord) && keyWord.Length > _config.GetValue<int>(Constants.MAX_LENGTH_SEARCH_KEYWORD)) return false;
            //日時Toより、日時Fromが超えているかをチェック行う。
            if (!string.IsNullOrEmpty(uploadDayFrom) && !string.IsNullOrEmpty(uploadDayTo))
            {
                DateTime dayFrom = DateTime.Parse(uploadDayFrom);
                DateTime dayTo = DateTime.Parse(uploadDayTo);
                if ((dayFrom - dayTo).TotalDays > 0) return false;
            }
            return true;
        }

        /// <summary>
        /// 検索サービスを使用してBLOBストレージ内のファイルを検索する
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns>List<string></returns>
        private List<string> SearchFileByKeyWord(string keyWord)
        {
            List<string> fileNames = new List<string>();
            // リクエスト送信して、タスクの返りを待つ
            HttpResponseMessage responseData = SendRequestSearch(keyWord);
            if (responseData.StatusCode.ToString().Equals(Constants.OK))
            {
                //json応答からリストfileNameを取得します
                JObject json1 = JObject.Parse(responseData.Content.ReadAsStringAsync().Result);
                foreach (var item in json1[Constants.VALUE])
                {
                    string fileName = item[Constants.METADATA_STORAGE_NAME].ToString();
                    fileNames.Add(fileName);
                }
            }
            return fileNames;
        }

        /// <summary>
        /// 検索サービスリクエストを送信する
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns>HttpResponseMessage</returns>
        private HttpResponseMessage SendRequestSearch(string keyWord)
        {
            //appsettings.jsonから値を取得する
            string apiKey = _config.GetValue<string>(Constants.COGNITIVE_API_KEY);
            string cognitiveAPI = _config.GetValue<string>(Constants.COGNITIVE_API);
            string url = string.Format(cognitiveAPI, keyWord);
            // メソッドにGETを指定
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Content = new StringContent("", Encoding.UTF8, "application/json");
            //APIキーを設定する
            request.Content.Headers.Add(Constants.API_KEY_TYPE, apiKey);
            // リクエストを送信し、その結果を取得
            var response = client.SendAsync(request);

            // 取得した結果
            return response.Result;
        }
    }
}
