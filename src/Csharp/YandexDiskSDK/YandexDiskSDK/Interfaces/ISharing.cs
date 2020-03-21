using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YandexDiskSDK.JSON;


namespace YandexDiskSDK
{
   public interface ISharing
    {

        Task<JSON_FolderList> ListAllSharedLinks(utilitiez.ItemTypeEnum Target = utilitiez.ItemTypeEnum.both, utilitiez.Fields Fields = utilitiez.Fields.nothing, utilitiez.PreviewSizeEnum PreviewSize = utilitiez.PreviewSizeEnum.S_150, int Limit = 20, int Offset = 0);

        Task<JSON_PublicFolder> PublicFolderContents2(string FolderPublicKey, string FilePathInsidePublicFolder = null, utilitiez.SortEnum? Sort = null, bool PreviewCrop = true, utilitiez.PreviewSizeEnum PreviewSize = utilitiez.PreviewSizeEnum.S_150, int Limit = 20, int Offset = 0);

        Task<JSON_PublicFolder> PublicFolderContents(Uri FolderPublicUrl, string FilePathInsidePublicFolder = null, utilitiez.SortEnum? Sort = null, bool PreviewCrop = true, utilitiez.PreviewSizeEnum PreviewSize = utilitiez.PreviewSizeEnum.S_150, int Limit = 20, int Offset = 0);

        Task<string> GetDownloadUrlOfFileInPublicFolder2(string FolderPublicKey, string FilePathInsidePublicFolder);

        Task<string> GetDownloadUrlOfFileInPublicFolder(Uri FolderPublicUrl, string FilePathInsidePublicFolder);

        Task<string> PublicUrlToPublicKey(Uri PublicUrl);

        Task<JSON_FileMetadata> Metadata(Uri PublicUrl);

        Task<string> PublicUrlToDirectUrl(Uri PublicUrl);

        Task<bool> DownloadPublicFile(Uri PublicUrl, string FileSaveDir, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default);

    }
}
