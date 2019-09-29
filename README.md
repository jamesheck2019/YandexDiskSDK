# YandexSDK

`Download:`
[https://github.com/jamesheck2019/YandexSDK/releases](https://github.com/jamesheck2019/YandexSDK/releases)<br>
`NuGet:`
[![NuGet](https://img.shields.io/nuget/v/DeQmaTech.YandexDiskSDK.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/DeQmaTech.YandexDiskSDK/)<br>
`Help:`
[https://github.com/jamesheck2019/YandexSDK/wiki](https://github.com/jamesheck2019/YandexSDK/wiki)<br>


**Features**
* Assemblies for .NET 4.5.2 and .NET Standard 2.0
* Just one external reference (Newtonsoft.Json)
* Easy installation using NuGet
* Upload/Download tracking support
* Proxy Support
* Upload/Download cancellation support

# List of functions:
* ListAllFiles
* DeleteFile
* DownloadFile
* DownloadFileAsStream
* Upload
* UploadRemoteFile
* CheckIfFileFolderExists
* PreviewFile
* DeleteFolder
* DownloadFolderAsZip
* CreateNewFolder
* ListAllFilesAndFolders
* TrashFileFolder
* MoveAndRenameFileFolder
* CopyAndRenameFileFolder
* PublishFileFolder
* UnPublishFileFolder
* GetFileFolderMetadata
* RenameFileFolder
* ListAllSharedLinksOfUser
* UserInfo
* AddMetaForFile
* ListLatestUploadedFiles
* GetDownloadUrl
* EmptyTrash
* RestoringFileorFolderFromTrash
* CopyingMovingDeletingUrluploadingOperationStatus
* ListPublicLink
* GetDownloadUrlOfFileInPublicFolder
* DownloadFileFromPublicFolder



# Code simple:
**get token**
```vb
Dim tkn = YandexDiskSDK.GetToken.OneYearToken(utilities.ResponseType.token, "ClientID_xxxxxx")
```
**set client**
```vb
Dim Clnt As YandexSDK.IClient = New YandexSDK.ZClient("token_xxxxx",YClient.DestinationType.disk)
```
**set client with proxy**
```vb
Dim m_proxy = New ZohoDocsSDK.ProxyConfig With {.SetProxy = True, .ProxyIP = "172.0.0.0", .ProxyPort = 80, .ProxyUsername = "usr", .ProxyPassword = "pas"}
Dim Clnt As YandexSDK.IClient = New YandexSDK.ZClient("token_xxxxxx",YClient.DestinationType.disk,m_proxy)
```
**list root files/folders**
``vb
Dim rslt = Await Clnt.ListAllFilesAndFolders("", utilities.Fields._embedded, 20, 0, True, Nothing, utilities.Sort.name)
For Each fle In rslt.FilesList
    DataGridView1.Rows.Add(fle.Name, fle.PublicUrl, fle.Path, fle.PreviewUrl)
Next
For Each fld In rslt.FoldersList
    DataGridView1.Rows.Add(fld.Name, fld.PublicUrl, fld.Path, fld.PreviewUrl)
Next
```
**create new folder**
```vb
 Dim rslt = Await Clnt.CreateNewFolder(Nothing, "thefoldername")
```
**delete file**
```vb
Dim rslt = Await Clnt.DeleteFile("/mymu.mp3")
```
