# YandexSDK

`Download:`
[https://github.com/jamesheck2019/YandexSDK/releases](https://github.com/jamesheck2019/YandexSDK/releases)<br>
`NuGet:`
[![NuGet version (BlackBeltCoder.Silk)](https://img.shields.io/nuget/v/DeQmaTech.YandexDiskSDK.svg?style=plastic)](https://www.nuget.org/packages/DeQmaTech.YandexDiskSDK/)<br>
`Help:`
[https://github.com/jamesheck2019/YandexSDK/wiki](https://github.com/jamesheck2019/YandexSDK/wiki)<br>


<ul>
	<li>.NET 4.5.2</li>
	<li>One dependency library [Newtonsoft.Json]</li>
</ul>

========
<ul>
	<li>Functions list:</li>
	<li>GetToken</li>
	<li>ListAllFiles</li>
	<li>CopyFile</li>
	<li>MoveFile</li>
	<li>DeleteFile</li>
	<li>TrashFile</li>
	<li>DownloadFile</li>
	<li>DownloadFileAsStream</li>
	<li>UploadLocalFile</li>
	<li>UploadRemoteFile</li>
	<li>UnPublishFile</li>
	<li>PublishFile</li>
	<li>RenameFile</li>
	<li>CheckIfFileExists</li>
	<li>PreviewFile</li>
	<li>==</li>
	<li>PublishFolder</li>
	<li>DeleteFolder</li>
	<li>TrashFolder</li>
	<li>MoveFolder</li>
	<li>CopyFolder</li>
	<li>DownloadFolderAsZip</li>
	<li>UnPublishFolder</li>
	<li>RenameFolder</li>
	<li>CreateDir</li>
	<li>CheckIfFolderExists</li>
	<li>==</li>
	<li>ListAllFilesAndFolders</li>
	<li>TrashFileFolder</li>
	<li>MoveAndRenameFileFolder</li>
	<li>CopyAndRenameFileFolder</li>
	<li>PublishFileFolder</li>
	<li>UnPublishFileFolder</li>
	<li>GetFileFolderMetadata</li>
	<li>CheckIfFileFolderExists</li>
	<li>RenameFileFolder</li>
	<li>==</li>
	<li>ListAllSharedLinksOfUser</li>
	<li>UserInfo</li>
	<li>AddMetaForFile</li>
	<li>ListLatestUploadedFiles</li>
	<li>GetDownloadUrl</li>
	<li>EmptyTrash</li>
	<li>RestoringFileorFolderFromTrash</li>
	<li>CopyingMovingDeletingUrluploadingOperationStatus</li>
	<li>ListPublicLink</li>
	<li>GetDownloadUrlOfFileInPublicFolder</li>
	<li>DownloadFileFromPublicFolder</li>
	<li>==</li>
</ul>

# List of functions:
[https://github.com/jamesheck2019/YandexSDK/blob/master/IClient.cs](https://github.com/jamesheck2019/YandexSDK/blob/master/IClient.cs)

# Code simple:
```vb.net
Dim Client As YandexSDK.IClient = New YandexSDK.YClient("xxxxxxxxxxxx", YClient.DestinationType.disk)
Dim GTokn= YandexSDK.GetToken.Get_Token(GetToken.ResponseType.code, "xxxxxxxx")
Dim rslt = Await Client.UserInfo()
Dim rslt = Await Client.ListAllFilesAndFolders("/", YdXutilities.Fields._embedded, Nothing, Nothing, True, Nothing, YdXutilities.Sort.name)
```
