using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using YandexDiskSDK.JSON;
using static YandexDiskSDK.utilitiez;
using static YandexDiskSDK.Basic;


namespace YandexDiskSDK
{
    public class YClient : IClient
    {
        public YClient(string accessToken, ConnectionSettings Settings = null)
        {
            authToken = accessToken;
            ConnectionSetting = Settings;
            if (Settings == null)
            {
                m_proxy = null;
            }
            else
            {
                m_proxy = Settings.Proxy;
                m_CloseConnection = Settings.CloseConnection ?? true;
                m_TimeOut = Settings.TimeOut ?? TimeSpan.FromMinutes(60);
            }
            ServicePointManager.Expect100Continue = true; ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
        }

        public IItem Item(string Path) => new ItemClient(Path);
        public ISharing Sharing() => new SharingClient();
        public IRecycleBin RecycleBin(string Path) => new RecycleBinClient(Path);


        public string RootPath(DestinationType PathPattern)
        {
            string pth = string.Empty;
            switch (PathPattern)
            {
                case DestinationType.app:
                    pth = "app:/";
                    break;
                case DestinationType.disk:
                    pth = "disk:/";
                    break;
                case DestinationType.trash:
                    pth = "trash:/";
                    break;
            }
            return pth;
        }

        #region UserInfo
        public async Task<JSON_UserInfo> UserInfo()
        {
            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                using (HttpResponseMessage response = await localHttpClient.GetAsync(new pUri("")).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return JsonConvert.DeserializeObject<JSON_UserInfo>(result, JSONhandler);
                    }
                    else
                    {
                        ShowError(result);
                        return null;
                    }
                }
            }
        }
        #endregion

        #region "ListLatestUploadedFiles"
        public async Task<JSON_FilesList> ListLatestUploadedFiles(FilterEnum? Filter = null, Fields Fields = Fields.nothing, bool PreviewCrop = true, PreviewSizeEnum PreviewSize = PreviewSizeEnum.S_150, int Limit = 200)
        {
            var parameters = new Dictionary<string, string>
            {
                { "media_type", Filter.ToString() },
                { "preview_crop", PreviewCrop.ToString() },
                { "preview_size", stringValueOf(PreviewSize) },
                { "fields", Fields.ToString() },
                { "limit", Limit.ToString() }
            };
            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                using (HttpResponseMessage response = await localHttpClient.GetAsync(new pUri("resources/last-uploaded", parameters)).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<JSON_FilesList>(result, JSONhandler);
                    }

                    else
                    {
                        ShowError(result);
                        return null;
                    }


                }
            }
        }
        #endregion

        #region "CopyingMovingDeletingUrluploadingOperationStatus"
        public async Task<JSON_CheckOperationStatus> CheckOperationStatus(string OperationHref)
        {
            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                using (HttpResponseMessage response = await localHttpClient.GetAsync(new Uri(OperationHref)).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return JsonConvert.DeserializeObject<JSON_CheckOperationStatus>(result, JSONhandler);
                    }
                    else
                    {
                        ShowError(result);
                        return null;
                    }
                }
            }
        }
        #endregion



    }
}
