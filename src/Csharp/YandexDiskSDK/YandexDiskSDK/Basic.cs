using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading;
using YandexDiskSDK.JSON;
using Newtonsoft.Json.Linq;
using static YandexDiskSDK.utilitiez;

namespace YandexDiskSDK
{
    public  static class Basic
    {

        public static string APIbase = "https://cloud-api.yandex.net/v1/disk/";
        public static TimeSpan m_TimeOut = Timeout.InfiniteTimeSpan;
        public static bool m_CloseConnection = true;
        public static JsonSerializerSettings JSONhandler = new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore, NullValueHandling = NullValueHandling.Ignore };
        public static string authToken = null;
        public static ConnectionSettings ConnectionSetting = null;

        private static ProxyConfig _proxy;
        public static ProxyConfig m_proxy
        {
            get
            {
                return _proxy ?? new ProxyConfig { };
            }
            set
            {
                _proxy = value;
            }
        }


        public class HCHandler : System.Net.Http.HttpClientHandler
        {
            public HCHandler() : base()
            {
                if (m_proxy.SetProxy)
                {
                    base.MaxRequestContentBufferSize = 1 * 1024 * 1024;
                    base.Proxy = new WebProxy(string.Format("http://{0}:{1}", m_proxy.ProxyIP, m_proxy.ProxyPort), true, null, new NetworkCredential(m_proxy.ProxyUsername, m_proxy.ProxyPassword));
                    base.UseProxy = m_proxy.SetProxy;
                }
            }
        }

        public class HtpClient : System.Net.Http.HttpClient
        {
            public HtpClient(HCHandler HCHandler) : base(HCHandler)
            {
                base.DefaultRequestHeaders.UserAgent.ParseAdd("YandexDiskSDK");
                base.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth", authToken);
                base.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                base.DefaultRequestHeaders.ConnectionClose = m_CloseConnection;
                base.Timeout = m_TimeOut;
            }
            public HtpClient(System.Net.Http.Handlers.ProgressMessageHandler progressHandler) : base(progressHandler)
            {
                base.DefaultRequestHeaders.UserAgent.ParseAdd("YandexDiskSDK");
                base.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth", authToken);
                base.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                base.DefaultRequestHeaders.ConnectionClose = m_CloseConnection;
                base.Timeout = m_TimeOut;
            }
        }

        public class pUri : Uri
        {
            public pUri(string ApiAction, System.Collections.Generic.Dictionary<string, string> Parameters = null) : base(Basic.APIbase + ApiAction + ((Parameters == null) ? null : utilitiez.AsQueryString(Parameters))){}
        }

        public static void  ShowError(string result)
        {
            var errorInfo = JsonConvert.DeserializeObject<JSON_Error>(result, JSONhandler);
            throw ExceptionCls.CreateException(errorInfo._ErrorMessage, errorInfo._ErrorCode);
        }

        public static  JObject Jobj(this string response)
        {
            return JObject.Parse(response);
        }



        public static bool Validation(this Uri PublicUrl, ItemTypeEnum UrlType)
        {
            bool Validation=false ;
            switch (UrlType)
            {
                case ItemTypeEnum.both:
                    Validation = (PublicUrl.ToString().ToLower().Contains("/d/") || PublicUrl.ToString().ToLower().Contains("/i/"));
                    break;
                case ItemTypeEnum.file:
                    Validation = PublicUrl.ToString().ToLower().Contains("/i/");
                    break;
                case ItemTypeEnum.dir:
                    Validation = PublicUrl.ToString().ToLower().Contains("/d/");
                    break;
            }

            if (!Validation) throw ExceptionCls.CreateException("Not a vild public url.", 202);
            
            return Validation;
        }













    }
}
