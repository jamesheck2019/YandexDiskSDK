Imports YandexDiskSDK.YClient
Imports YandexDiskSDK.utilitiez
Imports YandexDiskSDK.JSON
Imports YandexDiskSDK

Public Interface IClient

    ''' <summary>
    ''' file and folder Operations
    ''' [FD_] = file or folder
    ''' [F_] = file
    ''' [D_] = folder
    ''' </summary>
    ''' <param name="Path">file or folder path</param>
    ReadOnly Property Item(Path As String) As IItem
    ''' <summary>
    ''' get the root path of disk, trash, app
    ''' </summary>
    ''' <param name="PathPattern">disk, trash, app path tobe returned</param>
    ReadOnly Property RootPath(PathPattern As DestinationType) As String
    ''' <summary>
    ''' sharing Operations
    ''' </summary>
    ReadOnly Property Sharing As ISharing
    ''' <summary>
    ''' trash Operationss
    ''' </summary>
    ''' <param name="Path">trash path</param>
    ReadOnly Property RecycleBin(Path As String) As IRecycleBin


    ''' <summary>
    ''' The API returns general information about a user's Disk: the available space, system folder addresses, and so on.
    ''' If the request was processed without errors, the API responds with the 200 OK code and returns the information about the Disk in the Disk object in the response body. If the request caused an error, the relevant response code is returned, and the response body contains the error description.
    ''' [trash_size] 	The cumulative size of the files in the Trash, in bytes.
    ''' [total_space] 	The total space available to the user on Yandex.Disk, in bytes.
    ''' [used_space] 	The cumulative size of the files already stored on Yandex.Disk, in bytes.
    ''' [system_folders] 	Absolute addresses of Yandex.Disk system folders. Folder names depend on the user interface language that was in use when the user's personal Disk was created. For example, the Downloads folder is created for an English-speaking user, Загрузки for a Russian-speaking user, and so on.
    ''' The following folders are currently supported:
    '''  - applications — folder for application files
    '''  - downloads — folder for files downloaded from the internet (not from the user's device)
    '''  https://tech.yandex.com/disk/api/reference/capacity-docpage/
    ''' </summary>
    Function UserInfo() As Task(Of JSON_UserInfo)
    ''' <summary>
    ''' Copying, moving and deleting non-empty folders can take some time. The response to each of these requests contains the URL of the operation status
    ''' https://tech.yandex.com/disk/api/reference/operations-docpage/
    ''' </summary>
    ''' <param name="OperationHref">The URL for this request (including the ID) is returned in the response if the requested operation might take some time</param>
    Function CheckOperationStatus(OperationHref As String) As Task(Of JSON_CheckOperationStatus)
    ''' <summary>
    ''' The API returns a list of the files most recently uploaded to Yandex.Disk.
    ''' You can filter the list by file type (audio, video, image, And so On). Yandex.Disk detects the type Of Each downloaded file
    ''' https://tech.yandex.com/disk/api/reference/recent-upload-docpage/
    ''' </summary>
    ''' <param name="Filter">The type of files to include in the list. Yandex.Disk detects the type of each downloaded file</param>
    ''' <param name="Fields">List of JSON keys to include in the response. Keys that are not included in this list will be discarded when forming the response. If the parameter is omitted, the response is returned in full, without discarding anything</param>
    ''' <param name="PreviewCrop">This parameter crops the preview to the size specified in the preview_size parameter</param>
    ''' <param name="PreviewSize">The desired size of the thumbnail (preview) file. The thumbnail link is returned in the preview key</param>
    ''' <param name="Limit">The maximum number of items tobe returned</param>
    Function ListLatestUploadedFiles(Optional Filter As FilterEnum? = Nothing, Optional Fields As Fields = Fields.nothing, Optional PreviewCrop As Boolean = True, Optional PreviewSize As PreviewSizeEnum = PreviewSizeEnum.S_150, Optional Limit As Integer = 200) As Task(Of JSON_FilesList)
End Interface
