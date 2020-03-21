using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using YandexDiskSDK.JSON;
using static YandexDiskSDK.Basic;
using static YandexDiskSDK.utilitiez;

namespace YandexDiskSDK
{
    public class RecycleBinClient : IRecycleBin
    {

        private string Path { get; set; }
        public RecycleBinClient(string Path)
        {
            this.Path = Path;
        }


        #region "ListFilesAndFolders"
        public async Task<JSON_FolderList> List(bool PreviewCrop = true, PreviewSizeEnum PreviewSize = PreviewSizeEnum.S_150, SortEnum Sort = SortEnum.name, int Limit = 20, int Offset = 0)
        {
            var parameters = new Dictionary<string, string>
            {
                { "path", Path?? "trash:/" },
                { "fields", "_embedded" },
                { "limit", Limit.ToString() },
                { "offset", Offset.ToString() },
                { "preview_crop", PreviewCrop.ToString() },
                { "preview_size", stringValueOf(PreviewSize) },
                { "sort", "deleted" }// available choices are deleted, created, -deleted, -created.
            };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                using (HttpResponseMessage response = await localHttpClient.GetAsync(new pUri("trash/resources", parameters)).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var fin = new JSON_FolderList();
                        fin.limit = Convert.ToInt32(result.Jobj().SelectToken("_embedded.limit").ToString());
                        fin.offset = Convert.ToInt32(result.Jobj().SelectToken("_embedded.offset").ToString());
                        fin.total = Convert.ToInt32(result.Jobj().SelectToken("_embedded.total").ToString());
                        fin._Files = (from c in result.Jobj().SelectToken("_embedded")["items"].ToList().Select((i, JSON_FileMetadata) => i).Where(i => i.SelectToken("type").ToString().Equals("file")).Select(i => JsonConvert.DeserializeObject<JSON_FileMetadata>(i.ToString(), JSONhandler)) select c).ToList();
                        fin._Folders = (from c in result.Jobj().SelectToken("_embedded")["items"].ToList().Select((i, JSON_FolderMetadata) => i).Where(i => i.SelectToken("type").ToString().Equals("dir")).Select(i => JsonConvert.DeserializeObject<JSON_FolderMetadata>(i.ToString(), JSONhandler)) select c).ToList();
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

        #region "Empty TrashBin"
        public async Task<bool> EmptyTrashBin()
        {
            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                using (HttpResponseMessage response = await localHttpClient.DeleteAsync(new pUri("trash/resources")).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.Accepted)
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

        #region "Restoring File or Folder From Trash"
        public async Task<bool> Restore(string RenameTo = null, bool OverwriteIfExist = false)
        {
            var parameters = new Dictionary<string, string>
            {
                { "path", Uri.EscapeUriString(Path) },
                { "name", RenameTo },
                { "overwrite", OverwriteIfExist.ToString() }
            };
            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                var HtpReqMessage = new HttpRequestMessage();
                using (HttpResponseMessage response = await localHttpClient.PutAsync(new pUri("trash/resources/restore", parameters), null).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.Accepted)
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






    }
}
