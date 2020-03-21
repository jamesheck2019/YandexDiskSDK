using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YandexDiskSDK.JSON;
using static YandexDiskSDK.utilitiez;

namespace YandexDiskSDK
{
    public class SharingClient : ISharing
    {

        #region "ListAllSharedLinks"
        public async Task<JSON_FolderList> ListAllSharedLinks(ItemTypeEnum Target = ItemTypeEnum.both, Fields Fields = Fields.nothing, PreviewSizeEnum PreviewSize = PreviewSizeEnum.S_150, int Limit = 20, int Offset = 0)
        {
            var parameters = new Dictionary<string, string>();
            if (Fields != Fields.items || Fields != Fields.type)
            {
                parameters.Add("fields", Fields.ToString());
            }

            parameters.Add("limit", Limit.ToString());
            parameters.Add("offset", Offset.ToString());
            parameters.Add("preview_size", stringValueOf(PreviewSize));
            if (Target != ItemTypeEnum.both)
            {
                parameters.Add("type", Target.ToString());
            }

            using (Base.HttpClient localHttpClient = new Base.HttpClient(new Base.HCHandler()))
            {
                var RequestUri = new Base.pUri("resources/public", parameters);
                using (HttpResponseMessage response = await localHttpClient.GetAsync(RequestUri).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var fin = new JSON_FolderList();
                        fin.limit = Convert.ToInt32(result.Jobj().SelectToken("limit").ToString());
                        fin.offset = Convert.ToInt32(result.Jobj().SelectToken("offset").ToString());
                        fin._Files = (from c in result.Jobj().SelectToken("items").ToList().Select((i, JSON_FileMetadata) => i).Where(i => i.SelectToken("type").ToString().Equals("file")).Select(i => JsonConvert.DeserializeObject<JSON_FileMetadata>(i.ToString(), Base.JSONhandler)) select c).ToList();
                        fin._Folders = (from c in result.Jobj().SelectToken("items").ToList().Select((i, JSON_FolderMetadata) => i).Where(i => i.SelectToken("type").ToString().Equals("dir")).Select(i => JsonConvert.DeserializeObject<JSON_FolderMetadata>(i.ToString(), Base.JSONhandler)) select c).ToList();
                        return fin;
                    }
                    else
                    {
                        Base.ShowError(result);
                        return null;
                    }
                }
            }
        }
        #endregion

        #region "PublicFolderContents"
        public async Task<JSON_PublicFolder> PublicFolderContents(Uri FolderPublicUrl, string FilePathInsidePublicFolder = null, SortEnum? Sort = null, bool PreviewCrop = true, PreviewSizeEnum PreviewSize = PreviewSizeEnum.S_150, int Limit = 20, int Offset = 0)
        {
            FolderPublicUrl.Validation(ItemTypeEnum.dir);
            return await PublicFolderContents2(FolderPublicUrl.ToString(), FilePathInsidePublicFolder, Sort, PreviewCrop, PreviewSize, Limit, Offset);
        }

        public async Task<JSON_PublicFolder> PublicFolderContents2(string FolderPublicKey, string FilePathInsidePublicFolder = null, SortEnum? Sort = null, bool PreviewCrop = true, PreviewSizeEnum PreviewSize = PreviewSizeEnum.S_150, int Limit = 20, int Offset = 0)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("public_key", WebUtility.UrlEncode(FolderPublicKey));
            parameters.Add("path", FilePathInsidePublicFolder);
            parameters.Add("limit", Limit.ToString());
            parameters.Add("offset", Offset.ToString());
            parameters.Add("preview_crop", PreviewCrop.ToString());
            parameters.Add("preview_size", stringValueOf(PreviewSize));
            if (Sort.HasValue) { parameters.Add("sort", Sort.ToString()); }

            using (Base.HttpClient localHttpClient = new Base.HttpClient(new Base.HCHandler()))
            {
                var RequestUri = new Base.pUri("public/resources", parameters);
                using (HttpResponseMessage response = await localHttpClient.GetAsync(RequestUri).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var fin = JsonConvert.DeserializeObject<JSON_PublicFolder>(result, Base.JSONhandler);
                        fin.ItemsList._Files = (from c in result.Jobj().SelectToken("_embedded")["items"].ToList().Select((i, JSON_FileMetadata) => i).Where(i => i.SelectToken("type").ToString().Equals("file")).Select(i => JsonConvert.DeserializeObject<JSON_FileMetadata>(i.ToString(), Base.JSONhandler)) select c).ToList();
                        fin.ItemsList._Folders = (from c in result.Jobj().SelectToken("_embedded")["items"].ToList().Select((i, JSON_FolderMetadata) => i).Where(i => i.SelectToken("type").ToString().Equals("dir")).Select(i => JsonConvert.DeserializeObject<JSON_FolderMetadata>(i.ToString(), Base.JSONhandler)) select c).ToList();
                        return fin;
                    }
                    else
                    {
                        Base.ShowError(result);
                        return null;
                    }
                }
            }
        }
        #endregion

        #region "GetDownloadUrlOfFileInPublicFolder"
        public async Task<string> GetDownloadUrlOfFileInPublicFolder(Uri FolderPublicUrl, string FilePathInsidePublicFolder)
        {
            FolderPublicUrl.Validation(ItemTypeEnum.dir);
            return await GetDownloadUrlOfFileInPublicFolder2(FolderPublicUrl.ToString(), FilePathInsidePublicFolder);
        }
        public async Task<string> GetDownloadUrlOfFileInPublicFolder2(string FolderPublicKey, string FilePathInsidePublicFolder)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("public_key", WebUtility.UrlEncode(FolderPublicKey));
            parameters.Add("path", FilePathInsidePublicFolder);

            using (Base.HttpClient localHttpClient = new Base.HttpClient(new Base.HCHandler()))
            {
                var RequestUri = new Base.pUri("resources/download", parameters);
                using (HttpResponseMessage response = await localHttpClient.GetAsync(RequestUri).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == HttpStatusCode.OK)
                    { return result.Jobj().SelectToken("href").ToString(); }
                    else
                    {
                        Base.ShowError(result);
                        return null;
                    }
                }
            }
        }
        #endregion

        #region "PublicUrlToPublicKey"
        public async Task<string> PublicUrlToPublicKey(Uri PublicUrl)
        {
            PublicUrl.Validation(ItemTypeEnum.both);
            var parameters = new Dictionary<string, string>();
            parameters.Add("public_key", WebUtility.UrlEncode(PublicUrl.ToString()));
            parameters.Add("limit", "0");

            using (Base.HttpClient localHttpClient = new Base.HttpClient(new Base.HCHandler()))
            {
                var RequestUri = new Base.pUri("public/resources", parameters);
                using (HttpResponseMessage response = await localHttpClient.GetAsync(RequestUri).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return result.Jobj().SelectToken("public_key").ToString();
                    }
                    else
                    {
                        Base.ShowError(result);
                        return null;
                    }
                }
            }
        }
        #endregion

        #region "PublicUrlToDirectUrl"
        public async Task<string> PublicUrlToDirectUrl(Uri PublicUrl)
        {
            PublicUrl.Validation(ItemTypeEnum.both);
            var parameters = new Dictionary<string, string>();
            parameters.Add("public_key", WebUtility.UrlEncode(PublicUrl.ToString()));
            parameters.Add("limit", "0");

            using (Base.HttpClient localHttpClient = new Base.HttpClient(new Base.HCHandler()))
            {
                var RequestUri = new Base.pUri("public/resources", parameters);
                using (HttpResponseMessage response = await localHttpClient.GetAsync(RequestUri).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return result.Jobj().SelectToken("file").ToString();
                    }
                    else
                    {
                        Base.ShowError(result);
                        return null;
                    }
                }
            }
        }
        #endregion

        #region "Metadata"
        public async Task<JSON_FileMetadata> Metadata(Uri PublicUrl)
        {
            PublicUrl.Validation(ItemTypeEnum.both);
            var parameters = new Dictionary<string, string>();
            parameters.Add("public_key", WebUtility.UrlEncode(PublicUrl.ToString()));
            parameters.Add("limit", "0");

            using (Base.HttpClient localHttpClient = new Base.HttpClient(new Base.HCHandler()))
            {
                var RequestUri = new Base.pUri("public/resources", parameters);
                using (HttpResponseMessage response = await localHttpClient.GetAsync(RequestUri).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return JsonConvert.DeserializeObject<JSON_FileMetadata>(result, Base.JSONhandler);
                    }
                    else
                    {
                        Base.ShowError(result);
                        return null;
                    }
                }
            }
        }
        #endregion

        #region "DownloadPublicFile"
        public async Task<bool> DownloadPublicFile(Uri PublicUrl, string FileSaveDir, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default)
        {
            PublicUrl.Validation(ItemTypeEnum.file);
            var meta = await Metadata(PublicUrl);

            if (ReportCls == null) { ReportCls = new Progress<ReportStatus>(); }
            ReportCls.Report(new ReportStatus { Finished = false, TextStatus = "Initializing..." });

            try
            {
                var progressHandler = new System.Net.Http.Handlers.ProgressMessageHandler(new Base.HCHandler());
                progressHandler.HttpReceiveProgress += (sender, e) => { ReportCls.Report(new ReportStatus { ProgressPercentage = e.ProgressPercentage, BytesTransferred = e.BytesTransferred, TotalBytes = e.TotalBytes, TextStatus = "Downloading..." }); };
                using (Base.HttpClient localHttpClient = new Base.HttpClient(progressHandler))
                {
                    using (HttpResponseMessage ResPonse = await localHttpClient.GetAsync(new Uri(meta.DownloadUrl), HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(false))
                    {
                        ResPonse.EnsureSuccessStatusCode();
                        var stream_ = await ResPonse.Content.ReadAsStreamAsync();
                        var FPathname = Path.Combine(FileSaveDir, meta.name);
                        using (Stream FileStrm = new FileStream(FPathname, FileMode.Append, FileAccess.Write))
                        {
                            stream_.CopyTo(FileStrm);
                        }
                        if (ResPonse.IsSuccessStatusCode)
                        {
                            ReportCls.Report(new ReportStatus { Finished = true, TextStatus = $"[{meta.name}] Downloaded successfully." });
                            return true;
                        }
                        else
                        {
                            ReportCls.Report(new ReportStatus { Finished = true, TextStatus = $"Error code: {ResPonse.ReasonPhrase}" });
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ReportCls.Report(new ReportStatus { Finished = true });
                if (ex.Message.ToString().ToLower().Contains("a task was canceled"))
                {
                    ReportCls.Report(new ReportStatus { TextStatus = ex.Message });
                }
                else
                {
                    throw ExceptionCls.CreateException(ex.Message, 1001);
                }
                return false;
            }
        }
#endregion


    }
}
