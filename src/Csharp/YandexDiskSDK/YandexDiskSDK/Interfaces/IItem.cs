using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YandexDiskSDK.JSON;

namespace YandexDiskSDK
{
    public interface IItem
    {
        Task<JSON_FilesList> D_ListAllFiles(List<utilitiez.FilterEnum> Filter = null, bool PreviewCrop = true, utilitiez.PreviewSizeEnum PreviewSize = utilitiez.PreviewSizeEnum.S_150, utilitiez.SortEnum Sort = utilitiez.SortEnum.name, int Limit = 20, int Offset = 0);

        Task<JSON_FolderList> D_List(bool PreviewCrop = true, utilitiez.PreviewSizeEnum PreviewSize = utilitiez.PreviewSizeEnum.S_150, utilitiez.SortEnum Sort = utilitiez.SortEnum.name, int Limit = 20, int Offset = 0);

        Task<bool> FD_Move(string DestinationFolderPath, string RenameTo = null, bool OverwriteIfExist = false);

        Task<bool> FD_Copy(string DestinationFolderPath, string RenameTo = null, bool OverwriteIfExist = false);

        Task<bool> FD_Rename(string RenameTo);

        Task<bool> FD_Trash();

        Task<bool> FD_Delete();

        Task<string> D_Create(string FolderName);

        Task<string> F_GetDownloadUrl();

        Task<bool> FD_Privacy(utilitiez.PrivacyEnum Privacy);

        Task<JSON_MixedMetadata> FD_Metadata(bool PreviewCrop = true, utilitiez.PreviewSizeEnum PreviewSize = utilitiez.PreviewSizeEnum.S_150);

        Task F_Download(string FileSaveDir, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default(CancellationToken));

        Task F_DownloadLarge(string FileSaveDir, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default(CancellationToken));

        Task<Stream> F_DownloadAsStream(IProgress<ReportStatus> ReportCls = null, CancellationToken token = default(CancellationToken));

        Task D_DownloadAsZip(string FileSaveDir, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default(CancellationToken));

        Task D_Upload(object FileToUpload, utilitiez.UploadTypes UploadType, string FileName, bool OverwriteIfExist = false, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default(CancellationToken));

        Task<string> D_UploadRemotely(Uri FileUrl, string FileName = null, bool DisableRedirects = false);

        Task<bool> FD_Exists();

        Task<byte[]> F_GetThumbnail(Uri PreviewURL, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default(CancellationToken));

        Task<bool> FD_AddTag(object JsonObject);
    }
}
