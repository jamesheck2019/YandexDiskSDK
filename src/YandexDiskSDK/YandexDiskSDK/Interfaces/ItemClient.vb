Imports Newtonsoft.Json.Linq
Imports System.Net.Http.HttpMethod
Imports System.Net.Http
Imports Newtonsoft.Json
Imports YandexDiskSDK.JSON
Imports YandexDiskSDK.utilitiez

Public Class ItemClient
    Implements IItem

    Private Property Path As String

    Sub New(Path As String)
        Me.Path = Path
    End Sub


#Region "ListFilesAndFolders"
    Public Async Function Get_AllFilesandFolders_Disk(Optional PreviewCrop As Boolean = True, Optional PreviewSize As PreviewSizeEnum = PreviewSizeEnum.S_150, Optional Sort As SortEnum = SortEnum.name, Optional Limit As Integer = 20, Optional Offset As Integer = 0) As Task(Of JSON_FolderList) Implements IItem.D_List
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("path", If(Path, "disk:/"))
        parameters.Add("fields", "_embedded")
        parameters.Add("limit", Limit)
        parameters.Add("offset", Offset)
        parameters.Add("preview_crop", PreviewCrop)
        parameters.Add("preview_size", stringValueOf(PreviewSize))
        parameters.Add("sort", Sort.ToString)

        Using localHttpClient As New HttpClient(New HCHandler)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(New pUri("resources", parameters)).ConfigureAwait(False)
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

#Region "ListFiles"
    Public Async Function Get_AllFiles(Optional Filter As List(Of FilterEnum) = Nothing, Optional PreviewCrop As Boolean = True, Optional PreviewSize As PreviewSizeEnum = PreviewSizeEnum.S_150, Optional Sort As SortEnum = SortEnum.name, Optional Limit As Integer = 20, Optional Offset As Integer = 0) As Task(Of JSON_FilesList) Implements IItem.D_ListAllFiles
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("path", If(Path, "disk:/"))
        parameters.Add("fields", "_embedded")
        parameters.Add("limit", Limit)
        parameters.Add("offset", Offset)
        parameters.Add("preview_crop", PreviewCrop)
        parameters.Add("preview_size", stringValueOf(PreviewSize))
        parameters.Add("sort", Sort.ToString)
        parameters.Add("media_type", String.Join(",", Filter))

        Using localHttpClient As New HttpClient(New HCHandler)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(New pUri("resources/files", parameters)).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If response.StatusCode = Net.HttpStatusCode.OK Then
                    Return JsonConvert.DeserializeObject(Of JSON_FilesList)(result, JSONhandler)
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "ListFolders"
    'Public Async Function _ListFolders(Optional Sort As SortEnum = SortEnum.name, Optional Limit As Integer = 20, Optional Offset As Integer = 0) As Task(Of JSON_FoldersList) Implements IItem.ListFolders
    '    Dim parameters = New Dictionary(Of String, String)
    '    parameters.Add("path", ToStr(Path))
    '    parameters.Add("fields", "_embedded")
    '    parameters.Add("limit", Limit)
    '    parameters.Add("offset", Offset)
    '    parameters.Add("sort", Sort.ToString)

    '    Using localHttpClient As New HttpClient()
    '        Using response As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(New pUri("resources", parameters)).ConfigureAwait(False)
    '            Dim result As String = Await response.Content.ReadAsStringAsync()

    '            If response.StatusCode = Net.HttpStatusCode.OK Then
    '                Dim fin As New JSON_FoldersList
    '                fin.limit = result.Jobj.SelectToken("_embedded.limit").ToString
    '                fin.offset = result.Jobj.SelectToken("_embedded.offset").ToString
    '                fin._Folders = (From c In result.Jobj.SelectToken("_embedded")("items").ToList().Select(Function(i, JSON_FolderMetadata) i).Where(Function(i) i.SelectToken("type").ToString = "dir").Select(Function(i) JsonConvert.DeserializeObject(Of JSON_FolderMetadata)(i.ToString, JSONhandler))).ToList()
    '                Return fin
    '            Else
    '                ShowError(result)
    '            End If
    '        End Using
    '    End Using
    'End Function
#End Region

#Region "MoveFileFolder"
    Public Async Function _MoveFileFolder(DestinationFolderPath As String, Optional RenameTo As String = Nothing, Optional OverwriteIfExist As Boolean = False) As Task(Of Boolean) Implements IItem.FD_Move
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("from", Path)
        parameters.Add("path", Url.Combine(DestinationFolderPath, If(RenameTo, IO.Path.GetFileName(Path))))
        parameters.Add("overwrite", OverwriteIfExist)

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Post, New pUri("resources/move", parameters))
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)

                If response.StatusCode = Net.HttpStatusCode.Created Or Net.HttpStatusCode.Accepted Then ''File or empty folder
                    Return True
                Else
                    Dim result As String = Await response.Content.ReadAsStringAsync()
                    ShowError(result)
                    Return False
                End If
            End Using
        End Using
    End Function
#End Region

#Region "CopyFileFolder"
    Public Async Function _CopyFileFolder(DestinationFolderPath As String, Optional RenameTo As String = Nothing, Optional OverwriteIfExist As Boolean = False) As Task(Of Boolean) Implements IItem.FD_Copy
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("from", Path)
        parameters.Add("path", Url.Combine(DestinationFolderPath, If(RenameTo, IO.Path.GetFileName(Path))))
        parameters.Add("overwrite", OverwriteIfExist)

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Post, New pUri("resources/copy", parameters))
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)

                If response.StatusCode = Net.HttpStatusCode.Created Or response.StatusCode = Net.HttpStatusCode.Accepted Then ''File or empty folder
                    Return True
                Else
                    Dim result As String = Await response.Content.ReadAsStringAsync()
                    ShowError(result)
                    Return False
                End If
            End Using
        End Using
    End Function
#End Region

#Region "RenameFileFolder"
    Public Async Function _RenameFileFolder(RenameTo As String) As Task(Of Boolean) Implements IItem.FD_Rename
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("from", Path)
        parameters.Add("path", Url.Combine(IO.Path.GetDirectoryName(Path), RenameTo))
        parameters.Add("overwrite", False)

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Post, New pUri("resources/move", parameters))
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)

                If response.StatusCode = Net.HttpStatusCode.Created Or Net.HttpStatusCode.Accepted Then ''File or empty folder
                    Return True
                Else
                    Dim result As String = Await response.Content.ReadAsStringAsync()
                    ShowError(result)
                    Return False
                End If
            End Using
        End Using
    End Function
#End Region

#Region "TrashFileFolder"
    Public Async Function _TrashFileFolder() As Task(Of Boolean) Implements IItem.FD_Trash
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("path", Path)
        parameters.Add("permanently", "false")

        Using localHttpClient As New HttpClient(New HCHandler)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.DeleteAsync(New pUri("resources", parameters)).ConfigureAwait(False)

                If response.StatusCode = Net.HttpStatusCode.NoContent Or Net.HttpStatusCode.Accepted Then ''File or empty folder
                    Return True
                Else
                    Dim result As String = Await response.Content.ReadAsStringAsync()
                    ShowError(result)
                    Return False
                End If
            End Using
        End Using
    End Function
#End Region

#Region "DeleteFileFolder"
    Public Async Function _DeleteFileFolder() As Task(Of Boolean) Implements IItem.FD_Delete
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("path", Path)
        parameters.Add("permanently", "true")

        Using localHttpClient As New HttpClient(New HCHandler)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.DeleteAsync(New pUri("resources", parameters)).ConfigureAwait(False)

                If response.StatusCode = Net.HttpStatusCode.NoContent Or Net.HttpStatusCode.Accepted Then ''File or empty folder
                    Return True
                Else
                    Dim result As String = Await response.Content.ReadAsStringAsync()
                    ShowError(result)
                    Return False
                End If
            End Using
        End Using
    End Function
#End Region

#Region "CreateNewFolder"
    Public Async Function _CreateNewFolder(FolderName As String) As Task(Of String) Implements IItem.D_Create
        If IO.Path.HasExtension(Path) Then Throw New YandexException("DestinationPath Must be Folder Path", 889)
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("path", String.Concat(Path, "/", FolderName))
        parameters.Add("fields", "path")

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim RequestUri = New pUri("resources", parameters)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.PutAsync(RequestUri, Nothing).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If response.StatusCode = Net.HttpStatusCode.Created Then
                    Return UriExtensions.ParseQueryString(New Uri(result.Jobj.SelectToken("href").ToString)).Get("path")
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "GetDownloadUrl"
    Public Async Function _GetDownloadUrl() As Task(Of String) Implements IItem.F_GetDownloadUrl
        If Not IO.Path.HasExtension(Path) Then Throw New YandexException("DestinationPath Must be File Path", 888)
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("path", Path)
        parameters.Add("fields", "_embedded")

        Using localHttpClient As New HttpClient(New HCHandler)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(New pUri("resources/download", parameters)).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If response.StatusCode = Net.HttpStatusCode.OK Then
                    Return result.Jobj.SelectToken("href").ToString
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "CheckIfFileFolderExists"
    Public Async Function _Exists() As Task(Of Boolean) Implements IItem.FD_Exists
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("path", Path)
        parameters.Add("fields", "path")
        parameters.Add("limit", 0)
        parameters.Add("offset", 0)
        parameters.Add("preview_crop", True)
        parameters.Add("sort", "path")

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage
            HtpReqMessage.Method = New Net.Http.HttpMethod("HEAD")
            HtpReqMessage.RequestUri = New pUri("resources", parameters)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(False)

                If response.StatusCode = Net.HttpStatusCode.OK Then
                    Return True
                ElseIf response.StatusCode = Net.HttpStatusCode.NotFound Then
                    Return False
                Else
                    Throw ExceptionCls.CreateException(response.ReasonPhrase, response.StatusCode)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "AddTag"
    Public Async Function Get_Addingmetainformationforaresource(JsonObject As Object) As Task(Of Boolean) Implements IItem.FD_AddTag
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("path", Path)
        parameters.Add("fields", Fields.path.ToString)

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(New Net.Http.HttpMethod("PATCH"), New pUri("resources", parameters))
            HtpReqMessage.Content = New Net.Http.StringContent(JsonConvert.SerializeObject(New With {.custom_properties = JsonObject}), Text.Encoding.UTF8, "application/json")
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If response.IsSuccessStatusCode Then
                    Return True
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "Privacy"
    Public Async Function _Privacy(Privacy As PrivacyEnum) As Task(Of Boolean) Implements IItem.FD_Privacy
        Dim parameters = New Dictionary(Of String, String) From {{"path", Path}}

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim RequestUri = New pUri(If(Privacy = PrivacyEnum.Public, "resources/publish", "resources/unpublish"), parameters)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.PutAsync(RequestUri, Nothing).ConfigureAwait(False)

                If response.StatusCode = Net.HttpStatusCode.OK Then
                    Return True
                Else
                    Dim result As String = Await response.Content.ReadAsStringAsync()
                    ShowError(result)
                    Return False
                End If
            End Using
        End Using
    End Function
#End Region

#Region "GetFileFolderMetadata"
    Public Async Function _Metadata(Optional PreviewCrop As Boolean = True, Optional PreviewSize As PreviewSizeEnum = PreviewSizeEnum.S_150) As Task(Of JSON_MixedMetadata) Implements IItem.FD_Metadata
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("path", Path)
        parameters.Add("limit", 0)
        parameters.Add("offset", 0)
        parameters.Add("preview_crop", PreviewCrop)
        parameters.Add("preview_size", stringValueOf(PreviewSize))
        parameters.Add("sort", "path")
        parameters.Add("fields", "items")

        Using localHttpClient As New HttpClient(New HCHandler)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(New pUri("resources", parameters)).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If response.StatusCode = Net.HttpStatusCode.OK Then
                    Return JsonConvert.DeserializeObject(Of JSON_MixedMetadata)(result, JSONhandler)
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "Download File"
    Public Async Function DownloadFile(FileSaveDir As String, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task Implements IItem.F_Download
        If ReportCls Is Nothing Then ReportCls = New Progress(Of ReportStatus)
        ReportCls.Report(New ReportStatus With {.Finished = False, .TextStatus = "Initializing..."})
        Try
            Dim progressHandler As New Net.Http.Handlers.ProgressMessageHandler(New HCHandler)
            AddHandler progressHandler.HttpReceiveProgress, (Function(sender, e)
                                                                 ReportCls.Report(New ReportStatus With {.ProgressPercentage = e.ProgressPercentage, .BytesTransferred = e.BytesTransferred, .TotalBytes = If(e.TotalBytes Is Nothing, 0, e.TotalBytes), .TextStatus = "Downloading..."})
                                                             End Function)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim localHttpClient As New HttpClient(progressHandler)

            Dim client As New YandexDiskSDK.YClient(authToken, ConnectionSetting)
            Dim RequestUri = New Uri(Await client.Item(Path).F_GetDownloadUrl)
            '''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''
            Using ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri, Net.Http.HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(False)
                If ResPonse.IsSuccessStatusCode Then
                    ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = (String.Format("[{0}] Downloaded successfully.", IO.Path.GetFileName(Path)))})
                Else
                    ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = ((String.Format("Error code: {0}", ResPonse.StatusCode)))})
                End If

                ResPonse.EnsureSuccessStatusCode()
                Dim stream_ = Await ResPonse.Content.ReadAsStreamAsync()
                Dim FPathname As String = String.Concat(FileSaveDir.TrimEnd("\"), "\", IO.Path.GetFileName(Path))
                Using fileStream = New IO.FileStream(FPathname, IO.FileMode.Append, IO.FileAccess.Write)
                    stream_.CopyTo(fileStream)
                End Using
            End Using
        Catch ex As Exception
            ReportCls.Report(New ReportStatus With {.Finished = True})
            If ex.Message.ToString.ToLower.Contains("a task was canceled") Then
                ReportCls.Report(New ReportStatus With {.TextStatus = ex.Message})
            Else
                Throw ExceptionCls.CreateException(ex.Message, ex.Message)
            End If
        End Try
    End Function
#End Region

#Region "Download Large File"
    Public Async Function _DownloadLargeFile(FileSaveDir As String, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task Implements IItem.F_DownloadLarge
        If ReportCls Is Nothing Then ReportCls = New Progress(Of ReportStatus)
        ReportCls.Report(New ReportStatus With {.Finished = False, .TextStatus = "Initializing..."})
        Try
            Dim progressHandler As New Net.Http.Handlers.ProgressMessageHandler(New HCHandler)
            AddHandler progressHandler.HttpReceiveProgress, (Function(sender, e)
                                                                 ReportCls.Report(New ReportStatus With {.ProgressPercentage = e.ProgressPercentage, .BytesTransferred = e.BytesTransferred, .TotalBytes = If(e.TotalBytes Is Nothing, 0, e.TotalBytes), .TextStatus = "Downloading..."})
                                                             End Function)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim localHttpClient As New HttpClient(progressHandler)

            Dim client As New YandexDiskSDK.YClient(authToken, ConnectionSetting)
            Dim RequestUri = New Uri(Await client.Item(Path).F_GetDownloadUrl)
            '''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''
            Using ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri, Net.Http.HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(False)

                token.ThrowIfCancellationRequested()

                If ResPonse.IsSuccessStatusCode Then
                    ResPonse.EnsureSuccessStatusCode()
                    '''''''''''''''' write byte by byte to H.D '''''''''''''''''''''''''''''
                    Dim FPathname As String = IO.Path.Combine(FileSaveDir, IO.Path.GetFileName(Path))
                    Using streamToReadFrom As IO.Stream = Await ResPonse.Content.ReadAsStreamAsync()
                        Using streamToWriteTo As IO.Stream = IO.File.Open(FPathname, IO.FileMode.Create)
                            Await streamToReadFrom.CopyToAsync(streamToWriteTo, 1024, token)
                        End Using
                    End Using

                    ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = (String.Format("[{0}] Downloaded successfully.", IO.Path.GetFileName(Path)))})
                Else
                    ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = ((String.Format("Error code: {0}", ResPonse.ReasonPhrase)))})
                End If
            End Using
        Catch ex As Exception
            ReportCls.Report(New ReportStatus With {.Finished = True})
            If ex.Message.ToString.ToLower.Contains("a task was canceled") Then
                ReportCls.Report(New ReportStatus With {.TextStatus = ex.Message})
            Else
                Throw ExceptionCls.CreateException(ex.Message, ex.Message)
            End If
        End Try
    End Function
#End Region

#Region "DownloadFileAsStream"
    Public Async Function DownloadFileAsStream(Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task(Of IO.Stream) Implements IItem.F_DownloadAsStream
        If ReportCls Is Nothing Then ReportCls = New Progress(Of ReportStatus)
        ReportCls.Report(New ReportStatus With {.Finished = False, .TextStatus = "Initializing..."})
        Try
            Dim progressHandler As New Net.Http.Handlers.ProgressMessageHandler(New HCHandler)
            AddHandler progressHandler.HttpReceiveProgress, (Function(sender, e)
                                                                 ReportCls.Report(New ReportStatus With {.ProgressPercentage = e.ProgressPercentage, .BytesTransferred = e.BytesTransferred, .TotalBytes = If(e.TotalBytes Is Nothing, 0, e.TotalBytes), .TextStatus = "Downloading..."})
                                                             End Function)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim localHttpClient As New HttpClient(progressHandler)

            Dim client As New YandexDiskSDK.YClient(authToken, ConnectionSetting)
            Dim RequestUri = New Uri(Await client.Item(Path).F_GetDownloadUrl)
            '''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri, Net.Http.HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(False)
            If ResPonse.IsSuccessStatusCode Then
                ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = (String.Format("[{0}] Downloaded successfully.", IO.Path.GetFileName(Path)))})
            Else
                ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = ((String.Format("Error code: {0}", ResPonse.StatusCode)))})
            End If

            ResPonse.EnsureSuccessStatusCode()
            Dim stream_ As IO.Stream = Await ResPonse.Content.ReadAsStreamAsync()
            Return stream_
        Catch ex As Exception
            ReportCls.Report(New ReportStatus With {.Finished = True})
            If ex.Message.ToString.ToLower.Contains("a task was canceled") Then
                ReportCls.Report(New ReportStatus With {.TextStatus = ex.Message})
            Else
                Throw ExceptionCls.CreateException(ex.Message, ex.Message)
            End If
        End Try
    End Function
#End Region

#Region "DownloadFolderAsZip"
    Public Async Function _DownloadFolderAsZip(FileSaveDir As String, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task Implements IItem.D_DownloadAsZip
        If ReportCls Is Nothing Then ReportCls = New Progress(Of ReportStatus)
        ReportCls.Report(New ReportStatus With {.Finished = False, .TextStatus = "Initializing..."})
        Try
            Dim progressHandler As New Net.Http.Handlers.ProgressMessageHandler(New HCHandler)
            AddHandler progressHandler.HttpReceiveProgress, (Function(sender, e)
                                                                 ReportCls.Report(New ReportStatus With {.ProgressPercentage = e.ProgressPercentage, .BytesTransferred = e.BytesTransferred, .TotalBytes = If(e.TotalBytes Is Nothing, 0, e.TotalBytes), .TextStatus = "Downloading..."})
                                                             End Function)
            Dim localHttpClient As New HttpClient(progressHandler)

            Dim client As New YandexDiskSDK.YClient(authToken, ConnectionSetting)
            Dim RequestUri = New Uri(Await client.Item(Path).F_GetDownloadUrl)
            '''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''
            Using ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri, Net.Http.HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(False)
                If ResPonse.IsSuccessStatusCode Then
                    ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = (String.Format("[{0}] Downloaded successfully.", IO.Path.GetFileName(Path)))})
                Else
                    ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = ((String.Format("Error code: {0}", ResPonse.StatusCode)))})
                End If

                ResPonse.EnsureSuccessStatusCode()
                Dim stream_ = Await ResPonse.Content.ReadAsStreamAsync()
                Dim FPathname As String = String.Concat(FileSaveDir.TrimEnd("\"), "\", IO.Path.GetFileNameWithoutExtension(Path) + ".ZIP")
                Using fileStream = New IO.FileStream(FPathname, IO.FileMode.Append, IO.FileAccess.Write)
                    stream_.CopyTo(fileStream)
                End Using
            End Using
        Catch ex As Exception
            ReportCls.Report(New ReportStatus With {.Finished = True})
            If ex.Message.ToString.ToLower.Contains("a task was canceled") Then
                ReportCls.Report(New ReportStatus With {.TextStatus = ex.Message})
            Else
                Throw ExceptionCls.CreateException(ex.Message, ex.Message)
            End If
        End Try
    End Function
#End Region

#Region "get File Preview"
    Public Async Function GET_File_Preview(PreviewURL As Uri, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task(Of Byte()) Implements IItem.F_GetThumbnail
        If ReportCls Is Nothing Then ReportCls = New Progress(Of ReportStatus)
        ReportCls.Report(New ReportStatus With {.Finished = False, .TextStatus = "Initializing..."})
        Try
            Dim progressHandler As New Net.Http.Handlers.ProgressMessageHandler(New HCHandler)
            AddHandler progressHandler.HttpReceiveProgress, (Function(sender, e)
                                                                 ReportCls.Report(New ReportStatus With {.ProgressPercentage = e.ProgressPercentage, .BytesTransferred = e.BytesTransferred, .TotalBytes = If(e.TotalBytes Is Nothing, 0, e.TotalBytes), .TextStatus = "Downloading..."})
                                                             End Function)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim localHttpClient As New HttpClient(progressHandler)
            localHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/octet-stream")
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage() With {.Method = Net.Http.HttpMethod.Get, .RequestUri = PreviewURL}
            '''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(HtpReqMessage.RequestUri, Net.Http.HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(False)
            If ResPonse.IsSuccessStatusCode Then
                ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = ("Downloaded successfully.")})
            Else
                ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = ((String.Format("Error code: {0}", ResPonse.ReasonPhrase)))})
            End If
            ResPonse.EnsureSuccessStatusCode()
            Dim stream_ = Await ResPonse.Content.ReadAsStreamAsync()
            Using ms As New IO.MemoryStream
                Await stream_.CopyToAsync(ms)
                Return ms.ToArray
            End Using
        Catch ex As Exception
            ReportCls.Report(New ReportStatus With {.Finished = True})
            If ex.Message.ToString.ToLower.Contains("a task was canceled") Then
                ReportCls.Report(New ReportStatus With {.TextStatus = ex.Message})
            Else
                Throw ExceptionCls.CreateException(ex.Message, ex.Message)
            End If
        End Try
    End Function
#End Region

#Region "Upload Local File"

#Region "Get Upload Url"
    Private Async Function Get_UploadUrl(FileName As String, Optional OverwriteIfExist As Boolean = False) As Task(Of String)
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("path", Url.Combine(Path, FileName))
        parameters.Add("overwrite", OverwriteIfExist)

        Using localHttpClient As New HttpClient(New HCHandler)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(New pUri("resources/upload", parameters)).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If response.StatusCode = Net.HttpStatusCode.OK Then
                    Return result.Jobj.SelectToken("href").ToString
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

    Public Async Function Get_UploadLocal(FileToUpload As Object, UploadType As UploadTypes, FileName As String, Optional OverwriteIfExist As Boolean = False, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task Implements IItem.D_Upload
        If ReportCls Is Nothing Then ReportCls = New Progress(Of ReportStatus)
        ReportCls.Report(New ReportStatus With {.Finished = False, .TextStatus = "Initializing..."})
        Try
            Dim progressHandler As New Net.Http.Handlers.ProgressMessageHandler(New HCHandler)
            AddHandler progressHandler.HttpSendProgress, (Function(sender, e)
                                                              ReportCls.Report(New ReportStatus With {.ProgressPercentage = e.ProgressPercentage, .BytesTransferred = e.BytesTransferred, .TotalBytes = If(e.TotalBytes Is Nothing, 0, e.TotalBytes), .TextStatus = "Uploading..."})
                                                          End Function)
            Dim localHttpClient As New HttpClient(progressHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage()
            HtpReqMessage.Method = Net.Http.HttpMethod.Put
            ''''''''''''''''''''''''''''''''''
            Select Case UploadType
                Case UploadTypes.FilePath
                    Dim streamContent As Net.Http.HttpContent = New Net.Http.StreamContent(New IO.FileStream(FileToUpload, IO.FileMode.Open, IO.FileAccess.Read))
                    HtpReqMessage.Content = streamContent
                Case UploadTypes.Stream
                    Dim streamContent As Net.Http.HttpContent = New Net.Http.StreamContent(CType(FileToUpload, IO.Stream))
                    HtpReqMessage.Content = streamContent
                Case UploadTypes.BytesArry
                    Dim streamContent As Net.Http.ByteArrayContent = New Net.Http.ByteArrayContent(IO.File.ReadAllBytes(FileToUpload))
                    HtpReqMessage.Content = streamContent
            End Select
            ''''''''''''''''''''''''''
            HtpReqMessage.RequestUri = New Uri(Await Get_UploadUrl(FileName, OverwriteIfExist))
            '''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''
            Using ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(False)

                token.ThrowIfCancellationRequested()
                If ResPonse.IsSuccessStatusCode Then
                    ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = "Upload completed successfully"})
                Else
                    ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = String.Format("The request returned with HTTP status code {0}", ResPonse.ReasonPhrase)})
                End If
            End Using
        Catch ex As Exception
            ReportCls.Report(New ReportStatus With {.Finished = True})
            If ex.Message.ToString.ToLower.Contains("a task was canceled") Then
                ReportCls.Report(New ReportStatus With {.TextStatus = ex.Message})
            Else
                Throw ExceptionCls.CreateException(ex.Message, ex.Message)
            End If
        End Try
    End Function
#End Region

#Region "Upload Remote File"
    Public Async Function POST_File_RemoteUpload(FileUrl As Uri, Optional FileName As String = Nothing, Optional DisableRedirects As Boolean = False) As Task(Of String) Implements IItem.D_UploadRemotely
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("path", Url.Combine(Path, If(String.IsNullOrEmpty(FileName), IO.Path.GetFileName(FileUrl.ToString), FileName)))
        parameters.Add("url", FileUrl.ToString)
        parameters.Add("disable_redirects", DisableRedirects)

        Using localHttpClient As New HttpClient(New HCHandler)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(New pUri("resources/upload", parameters)).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If response.StatusCode = Net.HttpStatusCode.OK Then
                    Return String.Format("https://cloud-api.yandex.net/v1/disk/operations?id={0}", result.Jobj.SelectToken("operation_id").ToString)
                ElseIf response.StatusCode = Net.HttpStatusCode.Accepted Then
                    Return result.Jobj.SelectToken("href").ToString
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region




    '#Region "UploadLocal"
    '    Public Async Function Get_UploadLocal(FileToUpload As Object, UploadType As UploadTypes, FileName As String, Optional DestinationWorkspaceID As String = Nothing, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task(Of JSON_Upload) Implements IItem.D_Upload
    '        If ReportCls Is Nothing Then ReportCls = New Progress(Of ReportStatus)
    '        ReportCls.Report(New ReportStatus With {.Finished = False, .TextStatus = "Initializing..."})
    '        Try
    '            Dim progressHandler As New Net.Http.Handlers.ProgressMessageHandler(New HCHandler)
    '            AddHandler progressHandler.HttpSendProgress, (Function(sender, e)
    '                                                              ReportCls.Report(New ReportStatus With {.ProgressPercentage = e.ProgressPercentage, .BytesTransferred = e.BytesTransferred, .TotalBytes = If(e.TotalBytes Is Nothing, 0, e.TotalBytes), .TextStatus = "Uploading..."})
    '                                                          End Function)
    '            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '            Dim localHttpClient As New HttpClient(progressHandler)
    '            Dim HtpReqMessage As New Net.Http.HttpRequestMessage()
    '            HtpReqMessage.Method = Net.Http.HttpMethod.Post
    '            ''''''''''''''''''''''''''''''''''
    '            Dim MultipartsformData = New Net.Http.MultipartFormDataContent()
    '            Dim streamContent As Net.Http.HttpContent
    '            Select Case UploadType
    '                Case UploadTypes.FilePath
    '                    streamContent = New Net.Http.StreamContent(New IO.FileStream(FileToUpload, IO.FileMode.Open, IO.FileAccess.Read))
    '                Case UploadTypes.Stream
    '                    streamContent = New Net.Http.StreamContent(CType(FileToUpload, IO.Stream))
    '                Case UploadTypes.BytesArry
    '                    streamContent = New Net.Http.StreamContent(New IO.MemoryStream(CType(FileToUpload, Byte())))
    '                Case UploadTypes.String
    '                    streamContent = New Net.Http.StringContent(System.IO.File.ReadAllText(FileToUpload))
    '            End Select
    '            MultipartsformData.Add(streamContent, "content", FileName)
    '            MultipartsformData.Add(New Net.Http.StringContent(authToken), "authtoken")
    '            MultipartsformData.Add(New Net.Http.StringContent("docsapi"), "scope")
    '            MultipartsformData.Add(New Net.Http.StringContent(FileName), "filename")
    '            If ID IsNot Nothing Then MultipartsformData.Add(New Net.Http.StringContent(ID), "fid")
    '            If DestinationWorkspaceID IsNot Nothing Then MultipartsformData.Add(New Net.Http.StringContent(DestinationWorkspaceID), "wsid")
    '            HtpReqMessage.Content = MultipartsformData

    '            HtpReqMessage.RequestUri = New pUri("upload", New AuthDictionary)
    '            '''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''
    '            Using ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(False)
    '                Dim result As String = Await ResPonse.Content.ReadAsStringAsync()

    '                token.ThrowIfCancellationRequested()
    '                If ResPonse.StatusCode = Net.HttpStatusCode.OK Then
    '                    ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = "Upload completed successfully"})
    '                    Return JsonConvert.DeserializeObject(Of JSON_Upload)(result.Jobj.SelectToken("response[2].result[0]").ToString, JSONhandler)
    '                Else
    '                    ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = String.Format("The request returned with HTTP status code {0}", result.Jobj.SelectToken("response[1].message").ToString)})
    '                    Throw ExceptionCls.CreateException(result.Jobj.SelectToken("response[1].message").ToString, ResPonse.StatusCode)
    '                End If
    '            End Using
    '        Catch ex As Exception
    '            ReportCls.Report(New ReportStatus With {.Finished = True})
    '            If ex.Message.ToString.ToLower.Contains("a task was canceled") Then
    '                ReportCls.Report(New ReportStatus With {.TextStatus = ex.Message})
    '            Else
    '                Throw CType(ExceptionCls.CreateException(ex.Message, ex.Message), ZohoDocsException)
    '            End If
    '        End Try
    '    End Function
    '#End Region




End Class
