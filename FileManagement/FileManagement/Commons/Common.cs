using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace sharedfile.Commons
{
    public class Common
    {

        private static readonly ILog _logger = LogManager.GetLogger(typeof(Common));
        /// <summary>
        /// 認証
        /// </summary>
        /// <param name="_config"></param>
        /// <param name="userId"></param>
        /// <param name="accessToken"></param>
        /// <returns>bool</returns>
        public static bool AuthorizeUser(IConfiguration _config, string userId, string accessToken)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(accessToken)) return false;
            int usertype = _config.GetValue<int>(Constants.USER_TYPE);
            if (!(usertype == 1 || usertype == 2)) return false;
            Guid uid = new Guid(userId);
            Guid at = new Guid(accessToken);
            try
            {
                return DynTokenCheck.CheckToken(_config, uid, at);
            }
            catch (TimeoutException)
            {
                // リトライ
                try
                {
                    return DynTokenCheck.CheckToken(_config, uid, at);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
                return false;
            }
        }

        /// <summary>
        /// 認証ログ画面
        /// </summary>
        /// <param name="checkAccessTokenUrl"></param>
        /// <param name="userId"></param>
        /// <param name="accessToken"></param>
        /// <param name="urlRo"></param>
        /// <param name="roConfig"></param>
        /// <returns>bool</returns>
        public static bool AuthorizeUserKanri(IConfiguration _config, string userId, string accessToken, string urlRo)
        {
            //表現チェック
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(urlRo)) return false;
            if (!urlRo.Equals(_config.GetValue<string>(Constants.AUTHORITY_PROVISIONAL))) return false;
            //認証
            return AuthorizeUser(_config, userId, accessToken);
        }

        /// <summary>
        /// 汎用ポータルに戻る
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="d365Url"></param>
        /// <param name="destinationURL"></param>
        /// <returns>d365Url</returns>
        public static string GoBackD365(string userId, string d365Url, string destinationURL)
        {
            //POST通信を行う
            string json = string.Format("\"uid\"=\"{0}\"", userId);
            HttpResponseMessage d365Response = SendRequest(destinationURL, json);
            if (HttpStatusCode.OK.Equals(d365Response.StatusCode.ToString()) || HttpStatusCode.Accepted.Equals(d365Response.StatusCode))
            {
                //D365に遷移する。
                return d365Url;
            }
            return null;

        }

        private static HttpClient client;

        /// <summary>
        /// APIを呼び出し
        /// </summary>
        /// <param name="url"></param>
        /// <param name="json"></param>
        /// <returns>HttpResponseMessage</returns>
        public static HttpResponseMessage SendRequest(string url, string json)
        {
            client = new HttpClient();
            // メソッドにPOSTを指定
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            // 今回はJSONをPOSTしてみる
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            // リクエストを送信し、その結果を取得
            var response = client.SendAsync(request);

            // 取得した結果
            return response.Result;
        }
    }

    public static class DynTokenCheck
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(DynTokenCheck));
        /// <summary>
        /// ユーザーIDとアクセストークンを確認する
        /// </summary>
        /// <param name="_config"></param>
        /// <param name="uid"></param>
        /// <param name="at"></param>
        /// <returns>bool</returns>
        public static bool CheckToken(IConfiguration _config, Guid uid, Guid at)
        {
            // appsettings.jsonファイルからアプリの登録とサービス構成の値を取得します。
            var webConfig = new WebApiConfiguration(_config);

            HttpResponseMessage response;
            string messageUri = string.Format(Constants.MESSAGE_URI, webConfig.ServiceRoot, uid.ToString());
            try
            {
                // WebAPIメッセージリクエストを送信する
                response = SendMessageAsync(webConfig, HttpMethod.Get, messageUri).Result;
                // JSON応答をフォーマットしてコンソールに出力します。
                if (response.IsSuccessStatusCode)
                {
                    JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    Console.WriteLine(body.ToString());

                    // 結果からトークンを取得
                    JArray jarr = (JArray)body["value"];
                    var token = ((JObject)jarr[0]).GetValue("mhlw_filesystem_accesstoken");

                    // トークンが一致するか確認する
                    return at.ToString().Equals(token.ToString());
                }
                else
                {
                    _logger.Error(string.Format("The request failed with a status of '{0}'", response.ReasonPhrase));
                }
            }
            catch (TimeoutException e)
            {
                _logger.Error(e.ToString());
                throw e;
            }
            return false;
        }

        /// <summary>
        /// httpリクエストを送信する
        /// </summary>
        /// <param name="webConfig"></param>
        /// <param name="httpMethod"></param>
        /// <param name="messageUri"></param>
        /// <param name="body"></param>
        /// <returns>HttpResponseMessage</returns>
        static async Task<HttpResponseMessage> SendMessageAsync(WebApiConfiguration webConfig, HttpMethod httpMethod, string messageUri, string body = null)
        {
            // 認証に必要なアクセストークンを取得します。
            var accessToken = await GetAccessToken(webConfig);

            // 必要なWebAPIヘッダーが入力されたHTTPメッセージを作成します。
            var client = new HttpClient();
            var message = new HttpRequestMessage(httpMethod, messageUri);

            message.Headers.Add("OData-MaxVersion", "4.0");
            message.Headers.Add("OData-Version", "4.0");
            message.Headers.Add("Prefer", "odata.include-annotations=*");
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // 渡されたパラメーターで指定された本文コンテンツを追加します。  
            if (body != null)
                message.Content = new StringContent(body, UnicodeEncoding.UTF8, "application/json");

            // メッセージをWeb APIに送信します。
            return await client.SendAsync(message);
        }

        /// <summary>
        /// バッチでのAzure AD統合認証の使用
        /// </summary>
        /// <param name="webConfig"></param>
        /// <returns>string</returns>
        static async Task<string> GetAccessToken(WebApiConfiguration webConfig)
        {
            try
            {
                var credentials = new ClientCredential(webConfig.ClientId, webConfig.Secret);
                string authority = string.Format("https://login.microsoftonline.com/{0}/", webConfig.TenantId);
                AuthenticationContext authContext = new AuthenticationContext(authority);
                //Azure ADから認証トークンを取得します。
                AuthenticationResult result = await authContext.AcquireTokenAsync(webConfig.ResourceUri, credentials);
                return result.AccessToken;
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
                return null;
            }
        }
    }

    /// <summary>
    /// このプロジェクトのappsettings.configファイルから読み取られたD365 WebサービスとAzureアプリの登録情報を提供します。
    /// </summary>
    /// <remarks>
    /// このサンプルを実行する前に、appsettings.configファイルのアプリ設定に独自の値を指定する必要があります。
    /// </remarks>
    public class WebApiConfiguration
    {
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string TenantId { get; set; }
        public string ResourceUri { get; set; }
        public string ServiceRoot { get; set; }

        public WebApiConfiguration(IConfiguration _config)
        {
            ClientId = _config.GetValue<string>(Constants.CLIENT_ID_AUTHOR);
            Secret = _config.GetValue<string>(Constants.CLIENT_SECRET_AUTHOR);
            TenantId = _config.GetValue<string>(Constants.TENANT_ID_AUTHOR);
            ResourceUri = _config.GetValue<string>(Constants.RESOURCE_URL_AUTHOR);
            ServiceRoot = _config.GetValue<string>(Constants.SERVICE_ROOT_AUTHOR);
        }
    }
}
