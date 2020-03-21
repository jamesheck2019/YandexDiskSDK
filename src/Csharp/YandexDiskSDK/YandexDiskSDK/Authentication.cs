using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using YandexDiskSDK.JSON;
using static YandexDiskSDK.Basic;

namespace YandexDiskSDK
{
    public class Authentication
    {
        /// <summary>
        /// One Year Token from ClientID 
        ///https://tech.yandex.com/oauth/doc/dg/reference/web-client-docpage/
        /// </summary>
        /// <param name="ResponseType"></param>
        /// <param name="ClientID">application ID: get it from here https://oauth.yandex.com/client/new</param>
        /// <returns></returns>
        public static string OneYearToken(utilitiez.ResponseType ResponseType, string ClientID)
        {
            var parameters = new Dictionary<string, string> { };
            parameters.Add("response_type", ResponseType.ToString());
            parameters.Add("client_id", ClientID);
            parameters.Add("device_name", "YandexDiskSDK");
            parameters.Add("display", "popup");
            parameters.Add("force_confirm", "false");

            var tHErSULT = ("https://oauth.yandex.com/authorize" + utilitiez.AsQueryString(parameters)).ToLower();
            return tHErSULT;
        }


        public static async Task<JSONexchangingVerificationCodeForToken> ExchangeCodeToToken(string ClientID, string AuthorizationCode, string ClientSecret)
        {
            var parameters = new Dictionary<string, string> { };
            parameters.Add("grant_type", "authorization_code");
            parameters.Add("code", AuthorizationCode);
            parameters.Add("client_id", ClientID);
            parameters.Add("client_secret", ClientSecret);
            parameters.Add("device_name", "YandexDiskSDK");

            using (HtpClient localHttpClient = new HtpClient(new HCHandler { }))
            {
                HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new Uri("https://oauth.yandex.com/token" + utilitiez.AsQueryString(parameters)));
                using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                {
                    string result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<JSONexchangingVerificationCodeForToken>(result, JSONhandler);
                    }

                    else
                    {
                        var errorInfo = JsonConvert.DeserializeObject<JSON_Error>(result, JSONhandler);
                        throw ExceptionCls.CreateException(errorInfo._ErrorMessage, (int)response.StatusCode);
                    }

                }
            }
        }


    }
}
