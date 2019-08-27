using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YandexSDK.JSONyandex;

namespace YandexSDK
{
	public interface IClient
	{
		Task<JSONAllFiles> ListAllFiles(List<string> MediaType = null, YdXutilities.Fields Fields = YdXutilities.Fields.nothing, int Limit = 20, string Offset = null, bool PreviewCrop = true, string PreviewSize = null, YdXutilities.Sort Sort = YdXutilities.Sort.nothing);

		Task<JSONLink> CopyFile(string SourceFile, string DestinationFolder, string FileName, YdXutilities.Fields Fields = YdXutilities.Fields.nothing, bool Overwriting = false);

		Task<JSONLink> MoveFile(string SourceFile, string DestinationFolder, string FileName, YdXutilities.Fields Fields = YdXutilities.Fields.nothing, bool Overwriting = false);

		Task<JSONLink> DeleteFile(string FilePath, YdXutilities.Fields Fields = YdXutilities.Fields.nothing);

		Task<JSONLink> TrashFile(string FilePath, YdXutilities.Fields Fields = YdXutilities.Fields.nothing);

		Task DownloadFile(string FullFilePath, string FileSaveDir, string FileName, IProgress<ReportStatus> ReportCls = null, ProxyConfig _proxi = null, int TimeOut = 60, CancellationToken token = default(CancellationToken));

		Task<Stream> DownloadFileAsStream(string FullFilePath, IProgress<ReportStatus> ReportCls = null, ProxyConfig _proxi = null, int TimeOut = 60, CancellationToken token = default(CancellationToken));

		Task UploadLocalFile(object FileToUpload, string FileName, string DestinationFolder, YClient.UploadTypes UploadType, bool Overwriting = false, YdXutilities.Fields Fields = YdXutilities.Fields.nothing, IProgress<ReportStatus> ReportCls = null, ProxyConfig _proxi = null, CancellationToken token = default(CancellationToken));

		Task<JSONLink> UploadRemoteFile(string Url, string DestinationFolder, string FileName = null, bool DisableRedirects = false, YdXutilities.Fields Fields = YdXutilities.Fields.nothing);

		Task<JSONLink> UnPublishFile(string FilePath);

		Task<JSONLink> PublishFile(string FilePath);

		Task<JSONLink> RenameFile(string SourceFile, string RenameTo);

		Task<bool> CheckIfFileExists(string FilePath);

		Task<Stream> PreviewFile(Uri PreviewURL, IProgress<ReportStatus> ReportCls = null, ProxyConfig _proxi = null, CancellationToken token = default(CancellationToken));

		Task<JSONLink> PublishFolder(string FolderPath);

		Task<JSONLink> DeleteFolder(string FolderPath, YdXutilities.Fields Fields = YdXutilities.Fields.nothing);

		Task<JSONLink> TrashFolder(string FolderPath, YdXutilities.Fields Fields = YdXutilities.Fields.nothing);

		Task<JSONLink> MoveFolder(string SourceFolder, string DestinationFolder, string FolderName, YdXutilities.Fields Fields = YdXutilities.Fields.nothing, bool Overwriting = false);

		Task<JSONLink> CopyFolder(string SourceFolder, string DestinationFolder, string FolderName, YdXutilities.Fields Fields = YdXutilities.Fields.nothing, bool Overwriting = false);

		Task DownloadFolderAsZip(string FullFilePath, string FileSaveDir, string FileName, IProgress<ReportStatus> ReportCls = null, ProxyConfig _proxi = null, int TimeOut = 60, CancellationToken token = default(CancellationToken));

		Task<JSONLink> UnPublishFolder(string FolderPath);

		Task<JSONLink> RenameFolder(string SourceFolder, string RenameTo);

		Task<JSONLink> CreateDir(string DestinationFolder, string FolderName, YdXutilities.Fields Fields = YdXutilities.Fields.nothing, CancellationToken token = default(CancellationToken));

		Task<bool> CheckIfFolderExists(string FolderPath);

		Task<JSONFilesandFolders> ListAllFilesAndFolders(string Path, YdXutilities.Fields? Fields = null, int Limit = 20, int Offset = 0, bool PreviewCrop = true, string PreviewSize = null, YdXutilities.Sort? Sort = null);

		Task<JSONLink> TrashFileFolder(string FileFolderPath, YdXutilities.Fields Fields = YdXutilities.Fields.nothing);

		Task<JSONLink> MoveAndRenameFileFolder(string SourceFileFolder, string DestinationFolder, string FileName, bool Overwriting = false);

		Task<JSONLink> CopyAndRenameFileFolder(string SourceFileFolder, string DestinationFolder, string FolderName, bool Overwriting = false);

		Task<JSONLink> PublishFileFolder(string SourceFileFolder);

		Task<JSONLink> UnPublishFileFolder(string SourceFileFolder);

		Task<JSONFilesandFolders> GetFileFolderMetadata(string SourceFileFolder, bool PreviewCrop = true, string PreviewSize = null);

		Task<bool> CheckIfFileFolderExists(string SourceFileFolder);

		Task<JSONLink> RenameFileFolder(string SourceFileFolder, string RenameTo);

		Task<JSON_Embedded> ListAllSharedLinksOfUser(YdXutilities.Fields Fields = YdXutilities.Fields.nothing, string Limit = null, string Offset = null, JSONFilesandFolders.File_or_Folder? FilteredBy = null, string PreviewSize = null);

		Task<JSONuserinfo> UserInfo(CancellationToken token = default(CancellationToken));

		Task<JSONFilesandFolders> AddMetaForFile(string Path, YdXutilities.Fields Fields = YdXutilities.Fields.nothing);

		Task<JSON_Embedded> ListLatestUploadedFiles(YdXutilities.Media_Type? MediaType = null, int Limit = 20, YdXutilities.Fields Fields = YdXutilities.Fields.nothing, bool PreviewCrop = true, string PreviewSize = null);

		Task<JSONLink> GetDownloadUrl(string SourceFile);

		Task<JSONLink> EmptyTrash(string disk_File_Dir_and_Name = null, CancellationToken token = default(CancellationToken));

		Task<JSONLink> RestoringFileorFolderFromTrash(string disk_File_Dir_and_Name, string TheNewNameofTheResource = null, bool Overwriting = false, CancellationToken token = default(CancellationToken));

		Task<JSON_CopyingMovingDeletingUrluploadingOperationStatus> CopyingMovingDeletingUrluploadingOperationStatus(string OperationHref);

		Task<JSONFilesandFolders> ListPublicLink(string PublicFolderUrlOrKey, string FilePathInsidePublicFolder = null, YdXutilities.Sort? Sort = null, string Limit = null, string Offset = null, bool PreviewCrop = true, string PreviewSize = null);

		Task<JSONLink> GetDownloadUrlOfFileInPublicFolder(string PublicFolderUrlOrKey, string FilePathInsidePublicFolder = null);

		Task<bool> DownloadFileFromPublicFolder(string FolderPublicLink, string FilePathToDownload, string OutputDirPath, string DownloadUrl = null, IProgress<ReportStatus> ReportCls = null, ProxyConfig _proxi = null, int TimeOut = 60, CancellationToken token = default(CancellationToken));
	}
}
