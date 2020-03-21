Imports System.Threading
Imports YandexDiskSDK
Imports YandexDiskSDK.JSON
Imports YandexDiskSDK.utilitiez

Public Interface ISharing
    ''' <summary>
    ''' List of published resources
    ''' The API returns a list Of resources published On the user's Disk. Resources in the list are sorted in order of publishing, from latest to earliest.
    ''' https://tech.yandex.com/disk/api/reference/recent-public-docpage/
    ''' </summary>
    ''' <param name="Target">filtered by resource type To Get only files Or only folders or both</param>
    ''' <param name="Fields">List of JSON keys to include in the response. Keys that are not included in this list will be discarded when forming the response. If the parameter is omitted, the response is returned in full, without discarding anything</param>
    ''' <param name="PreviewSize">The desired size of the thumbnail (preview) file. The thumbnail link is returned in the preview key</param>
    ''' <param name="Limit">The maximum number of items tobe returned</param>
    ''' <param name="Offset">The number of resources from the top of the list that should be skipped in the response (used for paginated output).Let's say the /foo folder contains three files. For a folder metainformation request with the offset=1 parameter, the Yandex.Disk API returns only the second and third file descriptions</param>
    Function ListAllSharedLinks(Optional Target As ItemTypeEnum = ItemTypeEnum.both, Optional Fields As Fields = Fields.nothing, Optional PreviewSize As PreviewSizeEnum = PreviewSizeEnum.S_150, Optional Limit As Integer = 20, Optional Offset As Integer = 0) As Task(Of JSON_FolderList)
    ''' <summary>
    ''' browse public folder contents
    ''' https://tech.yandex.com/disk/api/reference/public-docpage/
    ''' </summary>
    ''' <param name="FolderPublicUrlOrKey">Key to a public resource, or the public link to a resource</param>
    ''' <param name="FilePathInsidePublicFolder">path to sub file/folder</param>
    Function PublicFolderContents(FolderPublicKey As String, Optional FilePathInsidePublicFolder As String = Nothing, Optional Sort As SortEnum? = Nothing, Optional PreviewCrop As Boolean = True, Optional PreviewSize As PreviewSizeEnum = PreviewSizeEnum.S_150, Optional Limit As Integer = 20, Optional Offset As Integer = 0) As Task(Of JSON_PublicFolder)
    Function PublicFolderContents(FolderPublicUrl As Uri, Optional FilePathInsidePublicFolder As String = Nothing, Optional Sort As SortEnum? = Nothing, Optional PreviewCrop As Boolean = True, Optional PreviewSize As PreviewSizeEnum = PreviewSizeEnum.S_150, Optional Limit As Integer = 20, Optional Offset As Integer = 0) As Task(Of JSON_PublicFolder)
    ''' <summary>
    ''' get download url of sub file locate in public folder
    ''' </summary>
    ''' <param name="FolderPublicUrlOrKey">Key to a public resource, or the public link to a resource</param>
    ''' <param name="FilePathInsidePublicFolder">path to targeted file</param>
    Function GetDownloadUrlOfFileInPublicFolder(FolderPublicKey As String, FilePathInsidePublicFolder As String) As Task(Of String)
    Function GetDownloadUrlOfFileInPublicFolder(FolderPublicUrl As Uri, FilePathInsidePublicFolder As String) As Task(Of String)
    ''' <summary>
    ''' convert public file/folder url -to- public file/folder key 
    ''' </summary>
    ''' <param name="PublicUrl">the public link to a resource</param>
    Function PublicUrlToPublicKey(PublicUrl As Uri) As Task(Of String)
    ''' <summary>
    ''' metadata of public file/folder
    ''' Metainformation about a public resource, If you know the key For a Public resource Or the Public link To it, you can request metainformation about this resource (the file properties Or properties Of the folder contents)
    ''' https://tech.yandex.com/disk/api/reference/public-docpage/
    ''' </summary>
    ''' <param name="PublicUrl">the public link to a resource</param>
    Function Metadata(PublicUrl As Uri) As Task(Of JSON_FileMetadata)
    ''' <summary>
    ''' convert public file/folder url -to- direct file/folder download url 
    ''' </summary>
    ''' <param name="PublicUrl">the public link to a resource</param>
    ''' <returns></returns>
    Function PublicUrlToDirectUrl(PublicUrl As Uri) As Task(Of String)
    ''' <summary>
    ''' download public file 
    ''' </summary>
    ''' <param name="PublicUrl">the public link to a file to be downloaded</param>
    ''' <param name="FileSaveDir">where to save it locally [c:\\downloads]</param>
    ''' <param name="ReportCls">downloading progress tracking</param>
    ''' <param name="token">cancel downloading opetation</param>
    Function DownloadPublicFile(PublicUrl As Uri, FileSaveDir As String, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As CancellationToken = Nothing) As Task(Of Boolean)
End Interface
