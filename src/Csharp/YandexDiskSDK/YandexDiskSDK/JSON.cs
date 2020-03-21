using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace YandexDiskSDK.JSON
{

    public class JSON_Error
    {
        [JsonProperty("description")]public string _ErrorMessage;
        [JsonProperty("error")] public int _ErrorCode;
    }

    #region JSONexchangingVerificationCodeForToken
    public class JSONexchangingVerificationCodeForToken
    {
        public string token_type { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
    }
    #endregion

    #region JSON_CheckOperationStatus
    public class JSON_CheckOperationStatus
    {
        [Browsable(false)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string status { get; set; }

        public JSON_CheckOperationStatus.OperationStatus ResultStatus
        {
            get
            {
                OperationStatus rslt = OperationStatus.success;
                switch (this.status)
                {
                    case "success":
                        rslt = OperationStatus.success;
                        break;
                    case "failure":
                        rslt = OperationStatus.failure;
                        break;
                    case "in-progress":
                        rslt = OperationStatus.inprogress;
                        break;
                    default:
                        rslt = OperationStatus.error;
                        break;
                }
                return rslt;
            }
        }

        public enum OperationStatus
        {
            success,
            failure,
            inprogress,
            error
        }
    }
    #endregion


    #region JSON_UserInfo
    public class JSON_UserInfo
    {
        public string max_file_size { get; set; }
        public bool unlimited_autoupload_enabled { get; set; }
        public string total_space { get; set; }
        public string trash_size { get; set; }
        public bool is_paid { get; set; }
        public string used_space { get; set; }
        public JSONSystem_Folders system_folders { get; set; }
        public JSON_User user { get; set; }
        public string revision { get; set; }
    }
    public class JSONSystem_Folders
    {
        public string odnoklassniki { get; set; }
        public string google { get; set; }
        public string instagram { get; set; }
        public string vkontakte { get; set; }
        public string mailru { get; set; }
        public string downloads { get; set; }
        public string applications { get; set; }
        public string facebook { get; set; }
        public string social { get; set; }
        public string screenshots { get; set; }
        public string photostream { get; set; }
    }
    #endregion

    #region JSON_FolderList
    public class JSON_FolderList
    {
        public List<JSON_FileMetadata> _Files { get; set; }
        public List<JSON_FolderMetadata> _Folders { get; set; }
        public string public_key { get; set; }
        public string sort { get; set; }
        public int limit { get; set; }
        public int offset { get; set; }
        public string path { get; set; }
        public int total { get; set; }
        public bool HasMore
        {
            get
            {
                return this.total >= checked(this.limit + this.offset);
            }
        }
    }
    #endregion

    #region JSON_User
    public class JSON_User
    {
        public string country { get; set; }
        public string login { get; set; }
        public string display_name { get; set; }
        public string uid { get; set; }
    }
    #endregion

    #region JSON_FilesList
    public class JSON_FilesList
    {
        [JsonProperty("items")]
        public List<JSON_FileMetadata> _Files { get; set; }
        public int limit { get; set; }
        public int offset { get; set; }
    }
    #endregion

    #region JSON_FileMetadata
    public class JSON_FileMetadata
    {
        public string name { get; set; }
        public Exif exif { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public string path { get; set; }
        public utilitiez.ItemTypeEnum type { get; set; }
        public int size { get; set; }
        public string mime_type { get; set; }
        public string media_type { get; set; }
        public string preview { get; set; }
        public string sha256 { get; set; }
        public string md5 { get; set; }
        public string public_key { get; set; }
        public string public_url { get; set; }
        [JsonProperty("file")]public string DownloadUrl { get; set; }
        public bool IsShared{get{return string.IsNullOrEmpty(this.public_key);}}
    }

    public class Exif
    {
        public DateTime date_time { get; set; }
    }


    #endregion

    #region JSON_FolderMetadata
    public class JSON_FolderMetadata
    {
        public DateTime modified { get; set; }
        public string name { get; set; }
        public DateTime created { get; set; }
        public string path { get; set; }
        public utilitiez.ItemTypeEnum type { get; set; }
        public string public_key { get; set; }
        public string public_url { get; set; }
        public bool IsShared{get{return string.IsNullOrEmpty(this.public_key);}}
    }
    #endregion

    #region JSON_MixedMetadata
    public class JSON_MixedMetadata
    {
        public string name { get; set; }
        public Exif exif { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public string path { get; set; }
        public utilitiez.ItemTypeEnum type { get; set; }
        public int size { get; set; }
        public string mime_type { get; set; }
        public string media_type { get; set; }
        public string preview { get; set; }
        public string sha256 { get; set; }
        public string md5 { get; set; }
        public string public_key { get; set; }
        public string public_url { get; set; }
       [JsonProperty("file")]
        public string DownloadUrl { get; set; }
        public bool IsShared{get{return string.IsNullOrEmpty(this.public_key);}}
    }
    #endregion

    #region JSON_PublicFolder
    public class JSON_PublicFolder
    {
        public string public_key { get; set; }
        public string public_url { get; set; }
        [JsonProperty("_embedded")]public JSON_FolderList ItemsList { get; set; }
        public string name { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public JSON_User owner { get; set; }
        public string path { get; set; }
        public utilitiez.ItemTypeEnum type { get; set; }
        public int views_count { get; set; }
    }
    #endregion




}
