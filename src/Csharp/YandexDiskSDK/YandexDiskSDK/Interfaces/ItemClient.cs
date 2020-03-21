using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Threading;
using System.Threading.Tasks;
using YandexDiskSDK.JSON;
using static YandexDiskSDK.Basic;

namespace YandexDiskSDK
{
    internal class ItemClient : IItem
    {
        private readonly string Path;
        public ItemClient(string Path)
        {
            this.Path = Path;
        }


        #region Get_AllFilesandFolders_Disk
        public async Task<JSON_FolderList> D_List(bool PreviewCrop = true, utilitiez.PreviewSizeEnum PreviewSize = utilitiez.PreviewSizeEnum.S_150, utilitiez.SortEnum Sort = utilitiez.SortEnum.name, int Limit = 20, int Offset = 0)
        {
            var parameters = new Dictionary<string, string>
            {
                { "path", Path ?? "disk:/" },
                { "fields", "_embedded" },
                { "limit", Limit.ToString() },
                { "offset", Offset.ToString() },
                { "preview_crop", PreviewCrop.ToString() },
                { "preview_size", utilitiez.stringValueOf(PreviewSize) },
                { "sort", Sort.ToString() }
            };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                using (HttpResponseMessage response = await localHttpClient.GetAsync(new pUri("resources", parameters)).ConfigureAwait(false))
                {
                    string result = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var fin = new JSON_FolderList
                        {
                            limit = Convert.ToInt32(result.Jobj().SelectToken("_embedded.limit").ToString()),
                            offset = Convert.ToInt32(result.Jobj().SelectToken("_embedded.offset").ToString()),
                            total = Convert.ToInt32(result.Jobj().SelectToken("_embedded.total").ToString()),
                            _Files = (from c in result.Jobj().SelectToken("_embedded")["items"].ToList().Select((i, JSON_FileMetadata) => i).Where(i => i.SelectToken("type").ToString() == "file").Select(i => JsonConvert.DeserializeObject<JSON_FileMetadata>(i.ToString(), JSONhandler)) select c).ToList(),
                            _Folders = (from c in result.Jobj().SelectToken("_embedded")["items"].ToList().Select((i, JSON_FolderMetadata) => i).Where(i => i.SelectToken("type").ToString() == "dir").Select(i => JsonConvert.DeserializeObject<JSON_FolderMetadata>(i.ToString(), JSONhandler)) select c).ToList()
                        };
                        return fin;
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

        #region Get_AllFiles
        public async Task<JSON_FilesList> D_ListAllFiles(List<utilitiez.FilterEnum> Filter = null, bool PreviewCrop = true, utilitiez.PreviewSizeEnum PreviewSize = utilitiez.PreviewSizeEnum.S_150, utilitiez.SortEnum Sort = utilitiez.SortEnum.name, int Limit = 20, int Offset = 0)
        {
            var parameters = new Dictionary<string, string>
            {
                { "path", Path ?? "disk:/" },
                { "fields", "_embedded" },
                { "limit", Limit.ToString() },
                { "offset", Offset.ToString() },
                { "preview_crop", PreviewCrop.ToString() },
                { "preview_size", utilitiez.stringValueOf(PreviewSize) },
                { "sort", Sort.ToString() },
                { "media_type", string.Join<utilitiez.FilterEnum>(",", Filter) }
            };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                using (HttpResponseMessage response = await localHttpClient.GetAsync(new pUri("resources/files", parameters)).ConfigureAwait(false))
                {
                    string result = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
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

        #region _MoveFileFolder
        public async Task<bool> FD_Move(string DestinationFolderPath, string RenameTo = null, bool OverwriteIfExist = false)
        {
            var parameters = new Dictionary<string, string>
            {
                { "from", Path },
                { "path", Hyperlink.Combine(new string[] { DestinationFolderPath, RenameTo ?? System.IO.Path.GetFileName(Path) }) },
                { "overwrite", OverwriteIfExist.ToString() }
            };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("resources/move", parameters));
                using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                {
                    if (response.StatusCode == HttpStatusCode.Created | response.StatusCode == HttpStatusCode.Accepted)
                    {
                        return true;
                    }
                    else
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        ShowError(result);
                        return false;
                    }
                }
            }
        }
        #endregion

        #region _CopyFileFolder
        public async Task<bool> FD_Copy(string DestinationFolderPath, string RenameTo = null, bool OverwriteIfExist = false)
        {
            var parameters = new Dictionary<string, string>
            {
                { "from", Path },
                { "path", Hyperlink.Combine(new string[] { DestinationFolderPath, RenameTo ?? System.IO.Path.GetFileName(Path) }) },
                { "overwrite", OverwriteIfExist.ToString() }
            };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("resources/copy", parameters));
                using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                {
                    if (response.StatusCode == HttpStatusCode.Created | response.StatusCode == HttpStatusCode.Accepted)
                    {
                        return true;
                    }
                    else
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        ShowError(result);
                        return false;
                    }
                }
            }
        }
        #endregion

        #region _RenameFileFolder
        public async Task<bool> FD_Rename(string RenameTo)
        {
            var parameters = new Dictionary<string, string>
            {
                { "from", Path },
                { "path", Hyperlink.Combine(new string[] { System.IO.Path.GetDirectoryName(Path), RenameTo }) },
                { "overwrite", "false" }
            };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("resources/move", parameters));
                using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                {
                    if (response.StatusCode == HttpStatusCode.Created | response.StatusCode == HttpStatusCode.Accepted)
                    {
                        return true;
                    }
                    else
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        ShowError(result);
                        return false;
                    }
                }
            }
        }
        #endregion

        #region _TrashFileFolder
        public async Task<bool> FD_Trash()
        {
            var parameters = new Dictionary<string, string> { { "path", Path }, { "permanently", "false" } };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                using (HttpResponseMessage response = await localHttpClient.DeleteAsync(new pUri("resources", parameters)).ConfigureAwait(false))
                {
                    if (response.StatusCode == HttpStatusCode.NoContent | response.StatusCode == HttpStatusCode.Accepted)
                    {
                        return true;
                    }
                    else
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        ShowError(result);
                        return false;
                    }
                }
            }
        }
        #endregion

        #region _DeleteFileFolder
        public async Task<bool> FD_Delete()
        {
            var parameters = new Dictionary<string, string> { { "path", Path }, { "permanently", "true" } };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                using (HttpResponseMessage response = await localHttpClient.DeleteAsync(new pUri("resources", parameters)).ConfigureAwait(false))
                {
                    if (response.StatusCode == HttpStatusCode.NoContent | response.StatusCode == HttpStatusCode.Accepted)
                    {
                        return true;
                    }
                    else
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        ShowError(result);
                        return false;
                    }
                }
            }
        }
        #endregion

        #region _CreateNewFolder
        public async Task<string> D_Create(string FolderName)
        {
            if (System.IO.Path.HasExtension(Path)) { throw new YandexDiskException("DestinationPath Must be Folder Path", 889); }

            var parameters = new Dictionary<string, string>
            {
                { "path", Path + "/" + FolderName },
                { "fields", "path" }
            };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                var RequestUri = new pUri("resources", parameters);
                using (HttpResponseMessage response = await localHttpClient.PutAsync(RequestUri, null).ConfigureAwait(false))
                {
                    string result = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.Created)
                    {
                        return new Uri(result.Jobj().SelectToken("href").ToString()).ParseQueryString().Get("path");
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

        #region _GetDownloadUrl
        public async Task<string> F_GetDownloadUrl()
        {
            if (!System.IO.Path.HasExtension(Path)) { throw new YandexDiskException("DestinationPath Must be File Path", 888); }

            var parameters = new Dictionary<string, string> { { "path", Path }, { "fields", "_embedded" } };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                using (HttpResponseMessage response = await localHttpClient.GetAsync(new pUri("resources/download", parameters)).ConfigureAwait(false))
                {
                    string result = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return result.Jobj().SelectToken("href").ToString();
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

        #region _Exists
        public async Task<bool> FD_Exists()
        {
            var parameters = new Dictionary<string, string> { { "path", Path }, { "fields", "path" }, { "limit", "0" }, { "offset", "0" }, { "preview_crop", "true" }, { "sort", "path" } };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                using (HttpResponseMessage response = await localHttpClient.SendAsync(new HttpRequestMessage { Method = new HttpMethod("HEAD"), RequestUri = new pUri("resources", parameters) }, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return true;
                    }
                    else
                    {
                        if (response.StatusCode != HttpStatusCode.NotFound)
                        {
                            throw ExceptionCls.CreateException(response.ReasonPhrase, (int)response.StatusCode);
                        }
                        return false;
                    }
                }
            }
        }
        #endregion

        #region Get_Addingmetainformationforaresource

        public async Task<bool> FD_AddTag(object JsonObject)
        {
            var parameters = new Dictionary<string, string> { { "path", Path }, { "fields", utilitiez.Fields.path.ToString() } };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                var HtpReqMessage = new HttpRequestMessage(new HttpMethod("PATCH"), new pUri("resources", parameters));
                HtpReqMessage.Content = new StringContent(JsonConvert.SerializeObject(new { custom_properties = JsonObject }), System.Text.Encoding.UTF8, "application/json");
                using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                {
                    string result = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        ShowError(result);
                        return false;
                    }
                }
            }

        }
        #endregion

        #region _Privacy

        public async Task<bool> FD_Privacy(utilitiez.PrivacyEnum Privacy)
        {
            var parameters = new Dictionary<string, string> { { "path", Path } };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                var RequestUri = new pUri((Privacy == utilitiez.PrivacyEnum.Public) ? "resources/publish" : "resources/unpublish", parameters);
                using (HttpResponseMessage response = await localHttpClient.PutAsync(RequestUri, null).ConfigureAwait(false))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return true;
                    }
                    else
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        ShowError(result);
                        return false;
                    }
                }
            }
        }
        #endregion

        #region _Metadata

        public async Task<JSON_MixedMetadata> FD_Metadata(bool PreviewCrop = true, utilitiez.PreviewSizeEnum PreviewSize = utilitiez.PreviewSizeEnum.S_150)
        {
            var parameters = new Dictionary<string, string>
            {
                { "path", Path },
                { "limit", "0" },
                { "offset", "0" },
                { "preview_crop", PreviewCrop.ToString() },
                { "preview_size", utilitiez.stringValueOf(PreviewSize) },
                { "sort", "path" },
                { "fields", "items" }
            };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                using (HttpResponseMessage response = await localHttpClient.GetAsync(new pUri("resources", parameters)).ConfigureAwait(false))
                {
                    string result = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return JsonConvert.DeserializeObject<JSON_MixedMetadata>(result, JSONhandler);
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

        #region DownloadFile
        public async Task F_Download(string FileSaveDir, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default(CancellationToken))
        {
            ReportCls = ReportCls ?? new Progress<ReportStatus>();
            ReportCls.Report(new ReportStatus { Finished = false, TextStatus = "Initializing..." });
            try
            {
                var progressHandler = new ProgressMessageHandler(new HCHandler());
                progressHandler.HttpReceiveProgress += (sender, e) => { ReportCls.Report(new ReportStatus { ProgressPercentage = e.ProgressPercentage, BytesTransferred = e.BytesTransferred, TotalBytes = e.TotalBytes ?? 0, TextStatus = "Downloading..." }); };
                var localHttpClient = new HtpClient(progressHandler);

                var client = new YClient(authToken, ConnectionSetting);
                var RequestUri = new Uri(await client.Item(Path).F_GetDownloadUrl());

                using (HttpResponseMessage ResPonse = await localHttpClient.GetAsync(RequestUri, HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(false))
                {
                    if (ResPonse.IsSuccessStatusCode)
                    {
                        ReportCls.Report(new ReportStatus { Finished = true, TextStatus = string.Format("[{0}] Downloaded successfully.", System.IO.Path.GetFileName(Path)) });
                    }
                    else
                    {
                        ReportCls.Report(new ReportStatus { Finished = true, TextStatus = string.Format("Error code: {0}", ResPonse.StatusCode) });
                    }
                    ResPonse.EnsureSuccessStatusCode();
                    Stream stream_ = await ResPonse.Content.ReadAsStreamAsync();
                    var FPathname = System.IO.Path.Combine(FileSaveDir, System.IO.Path.GetFileName(Path));
                    using (FileStream fileStream = new FileStream(FPathname, FileMode.Append, FileAccess.Write))
                    {
                        stream_.CopyTo(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                ReportCls.Report(new ReportStatus { Finished = true });
                if (!ex.Message.ToString().ToLower().Contains("a task was canceled"))
                {
                    throw ExceptionCls.CreateException(ex.Message, 1001);
                }
                ReportCls.Report(new ReportStatus { TextStatus = ex.Message });
            }
        }
        #endregion

        #region _DownloadLargeFile
        public async Task F_DownloadLarge(string FileSaveDir, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default)
        {
            ReportCls = ReportCls ?? new Progress<ReportStatus>();
            ReportCls.Report(new ReportStatus { Finished = false, TextStatus = "Initializing..." });
            try
            {
                var progressHandler = new ProgressMessageHandler(new HCHandler());
                progressHandler.HttpReceiveProgress += (sender, e) => { ReportCls.Report(new ReportStatus { ProgressPercentage = e.ProgressPercentage, BytesTransferred = e.BytesTransferred, TotalBytes = e.TotalBytes ?? 0, TextStatus = "Downloading..." }); };
                HtpClient localHttpClient = new HtpClient(progressHandler);
                var client = new YandexDiskSDK.YClient(authToken, ConnectionSetting);
                var RequestUri = new Uri(await client.Item(Path).F_GetDownloadUrl());

                using (HttpResponseMessage ResPonse = await localHttpClient.GetAsync(RequestUri, HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(false))
                {
                    token.ThrowIfCancellationRequested();
                    if (ResPonse.IsSuccessStatusCode)
                    {
                        ResPonse.EnsureSuccessStatusCode();
                        string FPathname = System.IO.Path.Combine(FileSaveDir, System.IO.Path.GetFileName(Path));
                        using (Stream streamToReadFrom = await ResPonse.Content.ReadAsStreamAsync())
                        {
                            using (Stream streamToWriteTo = File.Open(FPathname, FileMode.Create))
                            {
                                await streamToReadFrom.CopyToAsync(streamToWriteTo, 1024, token);
                            }
                        }
                        ReportCls.Report(new ReportStatus { Finished = true, TextStatus = string.Format("[{0}] Downloaded successfully.", System.IO.Path.GetFileName(Path)) });
                    }
                    else
                    {
                        ReportCls.Report(new ReportStatus { Finished = true, TextStatus = string.Format("Error code: {0}", ResPonse.ReasonPhrase) });
                    }
                }
            }
            catch (Exception ex)
            {
                ReportCls.Report(new ReportStatus { Finished = true });
                if (!ex.Message.ToString().ToLower().Contains("a task was canceled"))
                {
                    throw ExceptionCls.CreateException(ex.Message, 1001);
                }
                ReportCls.Report(new ReportStatus { TextStatus = ex.Message });
            }
        }
        #endregion

        #region DownloadFileAsStream
        public async Task<Stream> F_DownloadAsStream(IProgress<ReportStatus> ReportCls = null, CancellationToken token = default)
        {
            ReportCls = ReportCls ?? new Progress<ReportStatus>();
            ReportCls.Report(new ReportStatus { Finished = false, TextStatus = "Initializing..." });
            try
            {
                var progressHandler = new ProgressMessageHandler(new HCHandler());
                progressHandler.HttpReceiveProgress += (sender, e) => { ReportCls.Report(new ReportStatus { ProgressPercentage = e.ProgressPercentage, BytesTransferred = e.BytesTransferred, TotalBytes = e.TotalBytes ?? 0, TextStatus = "Downloading..." }); };

                HtpClient localHttpClient = new HtpClient(progressHandler);
                var client = new YClient(authToken, ConnectionSetting);
                var RequestUri = new Uri(await client.Item(Path).F_GetDownloadUrl());

                using (HttpResponseMessage ResPonse = await localHttpClient.GetAsync(RequestUri, HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(false))
                {
                    if (ResPonse.IsSuccessStatusCode)
                    {
                        ReportCls.Report(new ReportStatus { Finished = true, TextStatus = string.Format("[{0}] Downloaded successfully.", System.IO.Path.GetFileName(Path)) });
                    }
                    else
                    {
                        ReportCls.Report(new ReportStatus { Finished = true, TextStatus = string.Format("Error code: {0}", ResPonse.StatusCode) });
                    }
                    ResPonse.EnsureSuccessStatusCode();
                    Stream stream_ = await ResPonse.Content.ReadAsStreamAsync();
                    return stream_;
                }
            }
            catch (Exception ex)
            {
                ReportCls.Report(new ReportStatus { Finished = true });
                if (!ex.Message.ToString().ToLower().Contains("a task was canceled"))
                {
                    throw ExceptionCls.CreateException(ex.Message, 1001);
                }
                ReportCls.Report(new ReportStatus { TextStatus = ex.Message });
                return null;
            }
        }
        #endregion

        #region _DownloadFolderAsZip
        public async Task D_DownloadAsZip(string FileSaveDir, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default(CancellationToken))
        {
            ReportCls = ReportCls ?? new Progress<ReportStatus>();
            ReportCls.Report(new ReportStatus { Finished = false, TextStatus = "Initializing..." });
            try
            {
                var progressHandler = new ProgressMessageHandler(new HCHandler());
                progressHandler.HttpReceiveProgress += (sender, e) => { ReportCls.Report(new ReportStatus { ProgressPercentage = e.ProgressPercentage, BytesTransferred = e.BytesTransferred, TotalBytes = e.TotalBytes ?? 0, TextStatus = "Downloading..." }); };
                HtpClient localHttpClient = new HtpClient(progressHandler);
                var client = new YandexDiskSDK.YClient(authToken, ConnectionSetting);
                var RequestUri = new Uri(await client.Item(Path).F_GetDownloadUrl());

                using (HttpResponseMessage ResPonse = await localHttpClient.GetAsync(RequestUri, HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(false))
                {
                    if (ResPonse.IsSuccessStatusCode)
                    {
                        ReportCls.Report(new ReportStatus { Finished = true, TextStatus = string.Format("[{0}] Downloaded successfully.", System.IO.Path.GetFileName(Path)) });
                    }
                    else
                    {
                        ReportCls.Report(new ReportStatus { Finished = true, TextStatus = string.Format("Error code: {0}", ResPonse.StatusCode) });
                    }
                    ResPonse.EnsureSuccessStatusCode();
                    Stream stream_ = await ResPonse.Content.ReadAsStreamAsync();
                    var FPathname = System.IO.Path.Combine(FileSaveDir, System.IO.Path.GetFileNameWithoutExtension(Path) + ".ZIP");

                    using (FileStream fileStream = new FileStream(FPathname, FileMode.Append, FileAccess.Write))
                    {
                        stream_.CopyTo(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                ReportCls.Report(new ReportStatus { Finished = true });
                if (!ex.Message.ToString().ToLower().Contains("a task was canceled"))
                {
                    throw ExceptionCls.CreateException(ex.Message, 1001);
                }
                ReportCls.Report(new ReportStatus { TextStatus = ex.Message });
            }
        }
        #endregion

        #region GET_File_Preview
        public async Task<byte[]> F_GetThumbnail(Uri PreviewURL, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default(CancellationToken))
        {
            ReportCls = ReportCls ?? new Progress<ReportStatus>();
            ReportCls.Report(new ReportStatus { Finished = false, TextStatus = "Initializing..." });
            try
            {
                var progressHandler = new ProgressMessageHandler(new HCHandler());
                progressHandler.HttpReceiveProgress += (sender, e) => { ReportCls.Report(new ReportStatus { ProgressPercentage = e.ProgressPercentage, BytesTransferred = e.BytesTransferred, TotalBytes = e.TotalBytes ?? 0, TextStatus = "Downloading..." }); };
                var localHttpClient = new HtpClient(progressHandler);
                localHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/octet-stream");
                HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Get, PreviewURL);
                HttpResponseMessage ResPonse = await localHttpClient.GetAsync(HtpReqMessage.RequestUri, HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(false);
                if (ResPonse.IsSuccessStatusCode)
                {
                    ReportCls.Report(new ReportStatus { Finished = true, TextStatus = "Downloaded successfully." });
                }
                else
                {
                    ReportCls.Report(new ReportStatus { Finished = true, TextStatus = string.Format("Error code: {0}", ResPonse.ReasonPhrase) });
                }
                ResPonse.EnsureSuccessStatusCode();
                Stream stream_ = await ResPonse.Content.ReadAsStreamAsync();
                using (MemoryStream ms = new MemoryStream())
                {
                    await stream_.CopyToAsync(ms);
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                ReportCls.Report(new ReportStatus { Finished = true });
                if (!ex.Message.ToString().ToLower().Contains("a task was canceled"))
                {
                    throw ExceptionCls.CreateException(ex.Message, 1001);
                }
                ReportCls.Report(new ReportStatus { TextStatus = ex.Message });
                return null;
            }
        }
        #endregion

        #region Upload Local File

        #region Get_UploadUrl
        private async Task<string> Get_UploadUrl(string FileName, bool OverwriteIfExist = false)
        {
            var parameters = new Dictionary<string, string>
            {
                { "path", Hyperlink.Combine(new string[] { Path, FileName }) },
                { "overwrite", OverwriteIfExist.ToString() }
            };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                using (HttpResponseMessage response = await localHttpClient.GetAsync(new pUri("resources/upload", parameters)).ConfigureAwait(false))
                {
                    string result = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return result.Jobj().SelectToken("href").ToString();
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

        #region Get_UploadLocal
        public async Task D_Upload(object FileToUpload, utilitiez.UploadTypes UploadType, string FileName, bool OverwriteIfExist = false, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default(CancellationToken))
        {
            ReportCls = ReportCls ?? new Progress<ReportStatus>();
            ReportCls.Report(new ReportStatus { Finished = false, TextStatus = "Initializing..." });
            try
            {
                ProgressMessageHandler progressHandler = new ProgressMessageHandler(new HCHandler());
                progressHandler.HttpSendProgress += (sender, e) => { ReportCls.Report(new ReportStatus { ProgressPercentage = e.ProgressPercentage, BytesTransferred = e.BytesTransferred, TotalBytes = e.TotalBytes ?? 0, TextStatus = "Uploading..." }); };

                HtpClient localHttpClient = new HtpClient(progressHandler);
                HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Put, new Uri(await Get_UploadUrl(FileName, OverwriteIfExist)));

                switch (UploadType)
                {
                    case utilitiez.UploadTypes.FilePath:
                        {
                            HttpContent streamContent = new StreamContent(new FileStream(Convert.ToString(FileToUpload), FileMode.Open, FileAccess.Read));
                            HtpReqMessage.Content = streamContent;
                            break;
                        }
                    case utilitiez.UploadTypes.Stream:
                        {
                            HttpContent streamContent2 = new StreamContent((Stream)FileToUpload);
                            HtpReqMessage.Content = streamContent2;
                            break;
                        }
                    case utilitiez.UploadTypes.BytesArry:
                        {
                            ByteArrayContent streamContent3 = new ByteArrayContent(File.ReadAllBytes(Convert.ToString(FileToUpload)));
                            HtpReqMessage.Content = streamContent3;
                            break;
                        }
                }

                using (HttpResponseMessage ResPonse = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false))
                {
                    token.ThrowIfCancellationRequested();
                    if (ResPonse.IsSuccessStatusCode)
                    {
                        ReportCls.Report(new ReportStatus { Finished = true, TextStatus = "Upload completed successfully" });
                    }
                    else
                    {
                        ReportCls.Report(new ReportStatus { Finished = true, TextStatus = string.Format("The request returned with HTTP status code {0}", ResPonse.ReasonPhrase) });
                    }
                }
            }
            catch (Exception ex)
            {
                ReportCls.Report(new ReportStatus { Finished = true });
                if (!ex.Message.ToString().ToLower().Contains("a task was canceled"))
                {
                    throw ExceptionCls.CreateException(ex.Message, 1001);
                }
                ReportCls.Report(new ReportStatus { TextStatus = ex.Message });
            }
        }
        #endregion

        #endregion

        #region POST_File_RemoteUpload

        public async Task<string> D_UploadRemotely(Uri FileUrl, string FileName = null, bool DisableRedirects = false)
        {
            var parameters = new Dictionary<string, string>
            {
                { "path", Hyperlink.Combine(new string[] { Path, string.IsNullOrEmpty(FileName) ? System.IO.Path.GetFileName(FileUrl.ToString()) : FileName }) },
                { "url", FileUrl.ToString() },
                { "disable_redirects", DisableRedirects.ToString() }
            };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                using (HttpResponseMessage response = await localHttpClient.GetAsync(new pUri("resources/upload", parameters)).ConfigureAwait(false))
                {
                    string result = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return string.Format("https://cloud-api.yandex.net/v1/disk/operations?id={0}", result.Jobj().SelectToken("operation_id").ToString());
                    }
                    else if (response.StatusCode == HttpStatusCode.Accepted)
                    {
                        return result.Jobj().SelectToken("href").ToString();
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
