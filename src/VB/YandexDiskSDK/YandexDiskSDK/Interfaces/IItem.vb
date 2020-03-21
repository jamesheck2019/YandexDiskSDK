Imports System.IO
Imports System.Threading
Imports YandexDiskSDK
Imports YandexDiskSDK.JSON
Imports YandexDiskSDK.utilitiez

Public Interface IItem

    ''' <summary>
    ''' returns a flat list of all files on Yandex.Disk in alphabetical order. The flat list does not reflect the folder structure, so it is convenient to search for a particular file type spread across different folders
    ''' https://tech.yandex.com/disk/api/reference/all-files-docpage/
    ''' </summary>
    ''' <param name="Filter">The type of files to include in the list. Yandex.Disk detects the type of each downloaded file</param>
    ''' <param name="Fields">List of JSON keys to include in the response. Keys that are not included in this list will be discarded when forming the response. If the parameter is omitted, the response is returned in full, without discarding anything</param>
    ''' <param name="PreviewCrop">This parameter crops the preview to the size specified in the preview_size parameter</param>
    ''' <param name="PreviewSize">The desired size of the thumbnail (preview) file. The thumbnail link is returned in the preview key</param>
    ''' <param name="Limit">The maximum number of items tobe returned</param>
    ''' <param name="Offset">The number of resources from the top of the list that should be skipped in the response (used for paginated output).Let's say the /foo folder contains three files. For a folder metainformation request with the offset=1 parameter, the Yandex.Disk API returns only the second and third file descriptions</param>
    Function D_ListAllFiles(Optional Filter As List(Of FilterEnum) = Nothing, Optional PreviewCrop As Boolean = True, Optional PreviewSize As PreviewSizeEnum = PreviewSizeEnum.S_150, Optional Sort As SortEnum = SortEnum.name, Optional Limit As Integer = 20, Optional Offset As Integer = 0) As Task(Of JSON_FilesList)
    ''' <summary>
    ''' To request metainformation about files and folders, specify the path to the corresponding resource on Yandex.Disk. Metainformation includes the properties of files and folders, and the properties and contents of subfolders
    ''' https://tech.yandex.com/disk/api/reference/meta-docpage/
    ''' </summary>
    ''' <param name="PreviewCrop">This parameter crops the preview to the size specified in the preview_size parameter</param>
    ''' <param name="PreviewSize">The desired size of the thumbnail (preview) file. The thumbnail link is returned in the preview key</param>
    ''' <param name="Limit">The maximum number of items tobe returned</param>
    ''' <param name="Offset">The number of resources from the top of the list that should be skipped in the response (used for paginated output).Let's say the /foo folder contains three files. For a folder metainformation request with the offset=1 parameter, the Yandex.Disk API returns only the second and third file descriptions</param>
    Function D_List(Optional PreviewCrop As Boolean = True, Optional PreviewSize As PreviewSizeEnum = PreviewSizeEnum.S_150, Optional Sort As SortEnum = SortEnum.name, Optional Limit As Integer = 20, Optional Offset As Integer = 0) As Task(Of JSON_FolderList)
    ''' <summary>
    ''' To move files and folders on Yandex.Disk, specify the current path to the resource and its new location
    ''' https://tech.yandex.com/disk/api/reference/move-docpage/
    ''' </summary>
    ''' <param name="DestinationFolderPath">The path to the copy of the resource that is being created. For example, %2Fbar%2Fphoto.png (the new resource path can be up to 32760 characters long)</param>
    ''' <param name="RenameTo">rename copied file/folder</param>
    ''' <param name="OverwriteIfExist">Whether to overwrite. It is used if the resource is copied to a folder that already contains a resource with the same name</param>
    Function FD_Move(DestinationFolderPath As String, Optional RenameTo As String = Nothing, Optional OverwriteIfExist As Boolean = False) As Task(Of Boolean)
    ''' <summary>
    ''' To copy files and folders on a user's Disk, specify the path to the resource, and the desired path for its copy
    ''' https://tech.yandex.com/disk/api/reference/copy-docpage/
    ''' </summary>
    ''' <param name="DestinationFolderPath">The path to the copy of the resource that is being created. For example, %2Fbar%2Fphoto.png (the new resource path can be up to 32760 characters long)</param>
    ''' <param name="RenameTo">rename copied file/folder</param>
    ''' <param name="OverwriteIfExist">Whether to overwrite. It is used if the resource is copied to a folder that already contains a resource with the same name</param>
    Function FD_Copy(DestinationFolderPath As String, Optional RenameTo As String = Nothing, Optional OverwriteIfExist As Boolean = False) As Task(Of Boolean)
    ''' <summary>
    ''' rename file/folder
    ''' </summary>
    ''' <param name="RenameTo">the new file/folder name</param>
    Function FD_Rename(RenameTo As String) As Task(Of Boolean)
    ''' <summary>
    ''' move file/folder to trash bin
    ''' https://tech.yandex.com/disk/api/reference/delete-docpage/
    ''' </summary>
    Function FD_Trash() As Task(Of Boolean)
    ''' <summary>
    ''' To delete files and folders on a user's Disk permanently , specify the path to the resource to delete.
    ''' https://tech.yandex.com/disk/api/reference/delete-docpage/
    ''' </summary>
    Function FD_Delete() As Task(Of Boolean)
    ''' <summary>
    ''' To create a folder on Yandex.Disk, specify the desired path to the new folder
    ''' https://tech.yandex.com/disk/api/reference/create-folder-docpage/
    ''' </summary>
    Function D_Create(FolderName As String) As Task(Of String)
    ''' <summary>
    ''' To get a URL for directly downloading a file. send the API the path on Yandex.Disk where the file to download should be accessible
    ''' https://tech.yandex.com/disk/api/reference/content-docpage/
    ''' </summary>
    Function F_GetDownloadUrl() As Task(Of String)
    ''' <summary>
    ''' A resource becomes accessible by a direct link
    ''' https://tech.yandex.com/disk/api/reference/publish-docpage/
    ''' </summary>
    ''' <param name="Privacy"> puplic or private</param>
    Function FD_Privacy(Privacy As PrivacyEnum) As Task(Of Boolean)
    ''' <summary>
    ''' get file/folder metadata
    ''' </summary>
    ''' <param name="PreviewCrop">This parameter crops the preview to the size specified in the preview_size parameter</param>
    ''' <param name="PreviewSize">The desired size of the thumbnail (preview) file. The thumbnail link is returned in the preview key</param>
    Function FD_Metadata(Optional PreviewCrop As Boolean = True, Optional PreviewSize As PreviewSizeEnum = PreviewSizeEnum.S_150) As Task(Of JSON_MixedMetadata)
    ''' <summary>
    ''' download a file from user's Disk
    ''' </summary>
    ''' <param name="FileSaveDir">where to save it locally [c:\\downloads]</param>
    ''' <param name="ReportCls">downloading progress tracking</param>
    ''' <param name="token">cancel downloading opetation</param>
    Function F_Download(FileSaveDir As String, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As CancellationToken = Nothing) As Task
    ''' <summary>
    ''' download a file from user's Disk
    ''' downloading file saves to local disk byte by byte
    ''' </summary>
    ''' <param name="FileSaveDir">where to save it locally [c:\\downloads]</param>
    ''' <param name="ReportCls">downloading progress tracking</param>
    ''' <param name="token">cancel downloading opetation</param>
    Function F_DownloadLarge(FileSaveDir As String, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As CancellationToken = Nothing) As Task
    ''' <summary>
    ''' get file as io.stream
    ''' </summary>
    ''' <param name="ReportCls">downloading progress tracking</param>
    ''' <param name="token">cancel downloading opetation</param>
    Function F_DownloadAsStream(Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As CancellationToken = Nothing) As Task(Of Stream)
    ''' <summary>
    ''' download folder as zip file
    ''' </summary>
    ''' <param name="FileSaveDir">where to save it locally [c:\\downloads]</param>
    ''' <param name="ReportCls">downloading progress tracking</param>
    ''' <param name="token">cancel downloading opetation</param>
    Function D_DownloadAsZip(FileSaveDir As String, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As CancellationToken = Nothing) As Task
    ''' <summary>
    ''' Uploading a file to Yandex.Disk
    ''' https://tech.yandex.com/disk/api/reference/upload-docpage/
    ''' </summary>
    ''' <param name="FileToUpload"> the file path, stream, bytearray to be uploaded</param>
    ''' <param name="UploadType">type of input file</param>
    ''' <param name="FileName">file name to be saved in Yandex.Disk account</param>
    ''' <param name="OverwriteIfExist">Whether to overwrite the file. It is used if the file is uploaded to a folder that already contains a file with the same name</param>
    ''' <param name="ReportCls">uploading progress tracking</param>
    ''' <param name="token">cancel uploading opetation</param>
    Function D_Upload(FileToUpload As Object, UploadType As UploadTypes, FileName As String, Optional OverwriteIfExist As Boolean = Nothing, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As CancellationToken = Nothing) As Task
    ''' <summary>
    ''' Yandex.Disk can download a file to the user's Disk. To do this, pass the file URL in the request and track the progress of the operation. If an error occurs during the download, Yandex.Disk doesn't reattempt the download again.If the file fails to download directly to Yandex.Disk, you can try downloading it yourself and uploading it with the Upload a file to Disk request..
    ''' </summary>
    ''' <param name="FileUrl">The URL.</param>
    ''' <param name="FileName">Name of the file.</param>
    ''' <param name="DisableRedirects">Use this parameter to disable redirects for the address specified in the url parameter. ..Acceptable values: “false” — If a redirect is detected, Yandex.Disk downloads the file from the new address. This is the default value. <c>true</c> [If a redirect is detected, Yandex.Disk doesn't follow it or download anything.].</param>
    ''' <returns>JobID</returns>
    Function D_UploadRemotely(FileUrl As Uri, Optional FileName As String = Nothing, Optional DisableRedirects As Boolean = Nothing) As Task(Of String)
    ''' <summary>
    ''' check if file exists in user's Disk
    ''' </summary>
    Function FD_Exists() As Task(Of Boolean)
    ''' <summary>
    ''' get file thumnail as bytearray
    ''' </summary>
    ''' <param name="PreviewURL">file preview url [locate in file metadata]</param>
    Function F_GetThumbnail(PreviewURL As Uri, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As CancellationToken = Nothing) As Task(Of Byte())
    ''' <summary>
    ''' add file/folder json tag
    ''' https://tech.yandex.com/disk/api/reference/meta-add-docpage/
    ''' </summary>
    ''' <param name="JsonObject">{"foo":"1", "bar":"2"}</param>
    Function FD_AddTag(JsonObject As Object) As Task(Of Boolean)
End Interface
