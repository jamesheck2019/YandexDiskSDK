Imports YandexDiskSDK.JSON
Imports YandexDiskSDK.utilitiez

Public Interface IRecycleBin

    ''' <summary>
    ''' list trashed files/folders
    ''' </summary>
    ''' <param name="PreviewCrop">This parameter crops the preview to the size specified in the preview_size parameter</param>
    ''' <param name="PreviewSize">The desired size of the thumbnail (preview) file. The thumbnail link is returned in the preview key</param>
    ''' <param name="Limit">The maximum number of items tobe returned</param>
    ''' <param name="Offset">The number of resources from the top of the list that should be skipped in the response (used for paginated output).Let's say the /foo folder contains three files. For a folder metainformation request with the offset=1 parameter, the Yandex.Disk API returns only the second and third file descriptions</param>
    Function List(Optional PreviewCrop As Boolean = True, Optional PreviewSize As PreviewSizeEnum = PreviewSizeEnum.S_150, Optional Sort As SortEnum = SortEnum.name, Optional Limit As Integer = 20, Optional Offset As Integer = 0) As Task(Of JSON_FolderList)
    ''' <summary>
    ''' Cleaning the TrashBin
    ''' Files that have been moved to the Trash can be permanently deleted. The Trash is considered a folder on Yandex.Disk, so doing this will increase the space available on Yandex.Disk.
    ''' https://tech.yandex.com/disk/api/reference/trash-delete-docpage/
    ''' </summary>
    Function EmptyTrashBin() As Task(Of Boolean)
    ''' <summary>
    ''' Restoring a file or folder from the Trash
    ''' You can restore a resource that you moved To the Trash. To Return it To the previous location, enter the path To it In the Trash. You can rename the restored resource.
    ''' https://tech.yandex.com/disk/api/reference/trash-restore-docpage/
    ''' </summary>
    ''' <param name="RenameTo">The new name of the resource being restored. For example, selfie.png</param>
    ''' <param name="OverwriteIfExist">Whether to overwrite. It is used if the resource is restored to a folder that already contains a resource with the same name</param>
    Function Restore(Optional RenameTo As String = Nothing, Optional OverwriteIfExist As Boolean = Nothing) As Task(Of Boolean)
End Interface
