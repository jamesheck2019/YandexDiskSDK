using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YandexDiskSDK.JSON;

namespace YandexDiskSDK
{
   public  interface IRecycleBin
    {

        Task<JSON_FolderList> List(bool PreviewCrop = true, utilitiez.PreviewSizeEnum PreviewSize = utilitiez.PreviewSizeEnum.S_150, utilitiez.SortEnum Sort = utilitiez.SortEnum.name, int Limit = 20, int Offset = 0);

        Task<bool> EmptyTrashBin();

        Task<bool> Restore(string RenameTo = null, bool OverwriteIfExist = false);

    }
}
