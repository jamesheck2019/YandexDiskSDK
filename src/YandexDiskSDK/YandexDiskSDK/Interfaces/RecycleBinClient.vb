Imports Newtonsoft.Json
Imports YandexDiskSDK.JSON
Imports YandexDiskSDK.utilitiez

Public Class RecycleBinClient
    Implements IRecycleBin


    Private Property Path As String

    Sub New(Path As String)
        Me.Path = Path
    End Sub



#Region "ListFilesAndFolders"
    Public Async Function Get_AllFilesandFolders_Disk(Optional PreviewCrop As Boolean = True, Optional PreviewSize As PreviewSizeEnum = PreviewSizeEnum.S_150, Optional Sort As SortEnum = SortEnum.name, Optional Limit As Integer = 20, Optional Offset As Integer = 0) As Task(Of JSON_FolderList) Implements IRecycleBin.List
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("path", If(Path, "trash:/"))
        parameters.Add("fields", "_embedded")
        parameters.Add("limit", Limit)
        parameters.Add("offset", Offset)
        parameters.Add("preview_crop", PreviewCrop)
        parameters.Add("preview_size", stringValueOf(PreviewSize))
        parameters.Add("sort", "deleted") '' available choices are deleted, created, -deleted, -created.

        Using localHttpClient As New HttpClient(New HCHandler)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(New pUri("trash/resources", parameters)).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If response.StatusCode = Net.HttpStatusCode.OK Then
                    Dim fin As New JSON_FolderList
                    fin.limit = result.Jobj.SelectToken("_embedded.limit").ToString
                    fin.offset = result.Jobj.SelectToken("_embedded.offset").ToString
                    fin.total = result.Jobj.SelectToken("_embedded.total").ToString
                    fin._Files = (From c In result.Jobj.SelectToken("_embedded")("items").ToList().Select(Function(i, JSON_FileMetadata) i).Where(Function(i) i.SelectToken("type").ToString = "file").Select(Function(i) JsonConvert.DeserializeObject(Of JSON_FileMetadata)(i.ToString, JSONhandler))).ToList()
                    fin._Folders = (From c In result.Jobj.SelectToken("_embedded")("items").ToList().Select(Function(i, JSON_FolderMetadata) i).Where(Function(i) i.SelectToken("type").ToString = "dir").Select(Function(i) JsonConvert.DeserializeObject(Of JSON_FolderMetadata)(i.ToString, JSONhandler))).ToList()
                    Return fin
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "Empty TrashBin"
    Public Async Function _EmptyTrash() As Task(Of Boolean) Implements IRecycleBin.EmptyTrashBin
        Using localHttpClient As New HttpClient(New HCHandler)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.DeleteAsync(New pUri("trash/resources")).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If response.StatusCode = Net.HttpStatusCode.NoContent OrElse response.StatusCode = Net.HttpStatusCode.Accepted Then
                    Return True
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "Restoring File or Folder From Trash"
    Public Async Function _RestoringFileorFolderfromTrash(Optional RenameTo As String = Nothing, Optional OverwriteIfExist As Boolean = False) As Task(Of Boolean) Implements IRecycleBin.Restore
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("path", Uri.EscapeUriString(Path))
        parameters.Add("name", RenameTo)
        parameters.Add("overwrite", OverwriteIfExist)

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage()
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.PutAsync(New pUri("trash/resources/restore", parameters), Nothing).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If response.StatusCode = Net.HttpStatusCode.Created OrElse response.StatusCode = Net.HttpStatusCode.Accepted Then
                    Return True
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region


End Class
