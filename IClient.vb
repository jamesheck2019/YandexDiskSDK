Imports System.Collections.Generic
Imports System.IO
Imports System.Threading.Tasks
Imports ISisYandexSDK.JSONyandex
Imports ISisYandexSDK.YClient
Imports ISisYandexSDK.YdXutilities


Public Interface IClient



#Region "file"
    Function ListAllFiles(Optional MediaType As List(Of String) = Nothing, Optional Fields As Fields = Nothing, Optional Limit As Integer = 20, Optional Offset As String = Nothing, Optional PreviewCrop As Boolean = True, Optional PreviewSize As String = Nothing, Optional Sort As Sort = Nothing) As Task(Of JSONAllFiles)
    Function CopyFile(SourceFile As String, DestinationFolder As String, FileName As String, Optional Fields As Fields = Nothing, Optional Overwriting As Boolean = False) As Task(Of JSONLink)
    Function MoveFile(SourceFile As String, DestinationFolder As String, FileName As String, Optional Fields As Fields = Nothing, Optional Overwriting As Boolean = False) As Task(Of JSONLink)
    Function DeleteFile(FilePath As String, Optional Fields As Fields = Nothing) As Task(Of JSONLink)
    Function TrashFile(FilePath As String, Optional Fields As Fields = Nothing) As Task(Of JSONLink)
    Function DownloadFile(FullFilePath As String, FileSaveDir As String, FileName As String, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional _proxi As ProxyConfig = Nothing, Optional TimeOut As Integer = 60, Optional token As Threading.CancellationToken = Nothing) As Task
    Function DownloadFileAsStream(FullFilePath As String, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional _proxi As ProxyConfig = Nothing, Optional TimeOut As Integer = 60, Optional token As Threading.CancellationToken = Nothing) As Task(Of IO.Stream)
    Function UploadLocalFile(FileToUpload As Object, FileName As String, DestinationFolder As String, UploadType As UploadTypes, Optional Overwriting As Boolean = False, Optional Fields As Fields = Nothing, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional _proxi As ProxyConfig = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task
    Function UploadRemoteFile(Url As String, DestinationFolder As String, Optional FileName As String = Nothing, Optional DisableRedirects As Boolean = False, Optional Fields As Fields = Nothing) As Task(Of JSONLink)
    Function UnPublishFile(FilePath As String) As Task(Of JSONLink)
    Function PublishFile(FilePath As String) As Task(Of JSONLink)
    Function RenameFile(SourceFile As String, RenameTo As String) As Task(Of JSONLink)
    Function CheckIfFileExists(FilePath As String) As Task(Of Boolean)
    Function PreviewFile(PreviewURL As Uri, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional _proxi As ProxyConfig = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task(Of IO.Stream)


#End Region

#Region "folder"
    Function PublishFolder(FolderPath As String) As Task(Of JSONLink)
    Function DeleteFolder(FolderPath As String, Optional Fields As Fields = Nothing) As Task(Of JSONLink)
    Function TrashFolder(FolderPath As String, Optional Fields As Fields = Nothing) As Task(Of JSONLink)
    Function MoveFolder(SourceFolder As String, DestinationFolder As String, FolderName As String, Optional Fields As Fields = Nothing, Optional Overwriting As Boolean = False) As Task(Of JSONLink)
    Function CopyFolder(SourceFolder As String, DestinationFolder As String, FolderName As String, Optional Fields As Fields = Nothing, Optional Overwriting As Boolean = False) As Task(Of JSONLink)
    Function DownloadFolderAsZip(FullFilePath As String, FileSaveDir As String, FileName As String, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional _proxi As ProxyConfig = Nothing, Optional TimeOut As Integer = 60, Optional token As Threading.CancellationToken = Nothing) As Task
    Function UnPublishFolder(FolderPath As String) As Task(Of JSONLink)
    Function RenameFolder(SourceFolder As String, RenameTo As String) As Task(Of JSONLink)
    Function CreateDir(DestinationFolder As String, FolderName As String, Optional Fields As Fields = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task(Of JSONLink)
    Function CheckIfFolderExists(FolderPath As String) As Task(Of Boolean)

#End Region

#Region "Files And Folders"
    Function ListAllFilesAndFolders(Path As String, Optional Fields As Fields? = Nothing, Optional Limit As String = Nothing, Optional Offset As String = Nothing, Optional PreviewCrop As Boolean = True, Optional PreviewSize As String = Nothing, Optional Sort As Sort? = Nothing) As Task(Of JSONFilesandFolders)
    Function TrashFileFolder(FileFolderPath As String, Optional Fields As Fields = Nothing) As Task(Of JSONLink)
    Function MoveAndRenameFileFolder(SourceFileFolder As String, DestinationFolder As String, FileName As String, Optional Overwriting As Boolean = False) As Task(Of JSONLink)
    Function CopyAndRenameFileFolder(SourceFileFolder As String, DestinationFolder As String, FolderName As String, Optional Overwriting As Boolean = False) As Task(Of JSONLink)
    Function PublishFileFolder(SourceFileFolder As String) As Task(Of JSONLink)
    Function UnPublishFileFolder(SourceFileFolder As String) As Task(Of JSONLink)
    Function GetFileFolderMetadata(SourceFileFolder As String, Optional PreviewCrop As Boolean = True, Optional PreviewSize As String = Nothing) As Task(Of JSONFilesandFolders)
    Function CheckIfFileFolderExists(SourceFileFolder As String) As Task(Of Boolean)
    Function RenameFileFolder(SourceFileFolder As String, RenameTo As String) As Task(Of JSONLink)

#End Region

    Function ListAllSharedLinksOfUser(Optional Fields As Fields = Nothing, Optional Limit As String = Nothing, Optional Offset As String = Nothing, Optional FilteredBy As JSONFilesandFolders.File_or_Folder? = Nothing, Optional PreviewSize As String = Nothing) As Task(Of JSON_Embedded)
    Function UserInfo(Optional token As Threading.CancellationToken = Nothing) As Task(Of JSONuserinfo)
    Function AddMetaForFile(Path As String, Optional Fields As Fields = Nothing) As Task(Of JSONFilesandFolders)
    Function ListLatestUploadedFiles(Optional MediaType As Media_Type? = Nothing, Optional Limit As Integer = 20, Optional Fields As Fields = Nothing, Optional PreviewCrop As Boolean = True, Optional PreviewSize As String = Nothing) As Task(Of JSON_Embedded)
    
    Function GetDownloadUrl(SourceFile As String) As Task(Of JSONLink)
    
    Function EmptyTrash(Optional disk_File_Dir_and_Name As String = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task(Of JSONyandex.JSONLink)
    Function RestoringFileorFolderFromTrash(disk_File_Dir_and_Name As String, Optional TheNewNameofTheResource As String = Nothing, Optional Overwriting As Boolean = False, Optional token As Threading.CancellationToken = Nothing) As Task(Of JSONyandex.JSONLink)

    Function CopyingMovingDeletingUrluploadingOperationStatus(OperationHref As String) As Task(Of JSON_CopyingMovingDeletingUrluploadingOperationStatus)

    'Function TrashMultipleFileFolder(FileFolderPaths As List(Of String)) As Task(Of Boolean)
    Function ListPublicLink(PublicFolderUrlOrKey As String, Optional FilePathInsidePublicFolder As String = Nothing, Optional Sort As Sort? = Nothing, Optional Limit As String = Nothing, Optional Offset As String = Nothing, Optional PreviewCrop As Boolean = True, Optional PreviewSize As String = Nothing) As Task(Of JSONFilesandFolders)

    Function GetDownloadUrlOfFileInPublicFolder(PublicFolderUrlOrKey As String, Optional FilePathInsidePublicFolder As String = Nothing) As Task(Of JSONLink)
    Function DownloadFileFromPublicFolder(FolderPublicLink As String, FilePathToDownload As String, OutputDirPath As String, Optional DownloadUrl As String = Nothing, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional _proxi As ProxyConfig = Nothing, Optional TimeOut As Integer = 60, Optional token As Threading.CancellationToken = Nothing) As Task(Of Boolean)
    

End Interface
