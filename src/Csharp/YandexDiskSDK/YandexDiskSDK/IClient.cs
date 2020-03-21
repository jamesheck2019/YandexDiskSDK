using System.Threading.Tasks;
using YandexDiskSDK.JSON;

namespace YandexDiskSDK
{
    public interface IClient
    {


    IItem Item(string Path);
        ISharing Sharing();
        IRecycleBin RecycleBin(string Path);

        string RootPath(utilitiez.DestinationType PathPattern);

        Task<JSON_UserInfo> UserInfo();

        Task<JSON_CheckOperationStatus> CheckOperationStatus(string OperationHref);

        Task<JSON_FilesList> ListLatestUploadedFiles(utilitiez.FilterEnum? Filter = null, utilitiez.Fields Fields = utilitiez.Fields.nothing, bool PreviewCrop = true, utilitiez.PreviewSizeEnum PreviewSize = utilitiez.PreviewSizeEnum.S_150, int Limit = 200);
    }
}
