Imports Newtonsoft.Json
Imports YandexDiskSDK.JSON
Imports YandexDiskSDK.utilitiez

Public Class SharingClient
    Implements ISharing



#Region "ListAllSharedLinks"
    Public Async Function Get_ListAllSharedLinksOfUser(Optional Target As ItemTypeEnum = ItemTypeEnum.both, Optional Fields As Fields = Fields.nothing, Optional PreviewSize As PreviewSizeEnum = PreviewSizeEnum.S_150, Optional Limit As Integer = 20, Optional Offset As Integer = 0) As Task(Of JSON_FolderList) Implements ISharing.ListAllSharedLinks
        Dim parameters = New Dictionary(Of String, String)
        If Not Fields = Fields.items OrElse Fields = Fields.type Then parameters.Add("fields", Fields.ToString)
        parameters.Add("limit", Limit)
        parameters.Add("offset", Offset)
        parameters.Add("preview_size", stringValueOf(PreviewSize))
        If Not Target = ItemTypeEnum.both Then parameters.Add("type", Target.ToString)

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim RequestUri = New pUri("resources/public", parameters)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If response.StatusCode = Net.HttpStatusCode.OK Then
                    Dim fin As New JSON_FolderList
                    fin.limit = result.Jobj.SelectToken("limit").ToString
                    fin.offset = result.Jobj.SelectToken("offset").ToString
                    fin._Files = (From c In result.Jobj.SelectToken("items").ToList().Select(Function(i, JSON_FileMetadata) i).Where(Function(i) i.SelectToken("type").ToString = "file").Select(Function(i) JsonConvert.DeserializeObject(Of JSON_FileMetadata)(i.ToString, JSONhandler))).ToList()
                    fin._Folders = (From c In result.Jobj.SelectToken("items").ToList().Select(Function(i, JSON_FolderMetadata) i).Where(Function(i) i.SelectToken("type").ToString = "dir").Select(Function(i) JsonConvert.DeserializeObject(Of JSON_FolderMetadata)(i.ToString, JSONhandler))).ToList()
                    Return fin
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "PublicFolderContents"
    Public Async Function _ParsePublicFolder2(FolderPublicUrl As Uri, Optional FilePathInsidePublicFolder As String = Nothing, Optional Sort As SortEnum? = Nothing, Optional PreviewCrop As Boolean = True, Optional PreviewSize As PreviewSizeEnum = PreviewSizeEnum.S_150, Optional Limit As Integer = 20, Optional Offset As Integer = 0) As Task(Of JSON_PublicFolder) Implements ISharing.PublicFolderContents
        FolderPublicUrl.Validation(ItemTypeEnum.dir)
        Return Await _ParsePublicFolder(FolderPublicUrl.ToString, FilePathInsidePublicFolder, Sort, PreviewCrop, PreviewSize, Limit, Offset)
    End Function
    Public Async Function _ParsePublicFolder(FolderPublicKey As String, Optional FilePathInsidePublicFolder As String = Nothing, Optional Sort As SortEnum? = Nothing, Optional PreviewCrop As Boolean = True, Optional PreviewSize As PreviewSizeEnum = PreviewSizeEnum.S_150, Optional Limit As Integer = 20, Optional Offset As Integer = 0) As Task(Of JSON_PublicFolder) Implements ISharing.PublicFolderContents
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("public_key", System.Net.WebUtility.UrlEncode(FolderPublicKey))
        parameters.Add("path", FilePathInsidePublicFolder)
        parameters.Add("limit", Limit)
        parameters.Add("offset", Offset)
        parameters.Add("preview_crop", PreviewCrop)
        parameters.Add("preview_size", stringValueOf(PreviewSize))
        If Sort.HasValue Then parameters.Add("sort", Sort.ToString)

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim RequestUri = New pUri("public/resources", parameters)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If response.StatusCode = Net.HttpStatusCode.OK Then
                    Dim fin = JsonConvert.DeserializeObject(Of JSON_PublicFolder)(result, JSONhandler)
                    fin.ItemsList._Files = (From c In result.Jobj.SelectToken("_embedded")("items").ToList().Select(Function(i, JSON_FileMetadata) i).Where(Function(i) i.SelectToken("type").ToString = "file").Select(Function(i) JsonConvert.DeserializeObject(Of JSON_FileMetadata)(i.ToString, JSONhandler))).ToList()
                    fin.ItemsList._Folders = (From c In result.Jobj.SelectToken("_embedded")("items").ToList().Select(Function(i, JSON_FolderMetadata) i).Where(Function(i) i.SelectToken("type").ToString = "dir").Select(Function(i) JsonConvert.DeserializeObject(Of JSON_FolderMetadata)(i.ToString, JSONhandler))).ToList()
                    Return fin
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "GetDownloadUrlOfFileInPublicFolder"
    Public Async Function _GetDownloadUrl(FolderPublicUrl As Uri, FilePathInsidePublicFolder As String) As Task(Of String) Implements ISharing.GetDownloadUrlOfFileInPublicFolder
        FolderPublicUrl.Validation(ItemTypeEnum.dir)
        Return Await POST_GetDownloadUrl(FolderPublicUrl.ToString, FilePathInsidePublicFolder)
    End Function
    Public Async Function POST_GetDownloadUrl(FolderPublicKey As String, FilePathInsidePublicFolder As String) As Task(Of String) Implements ISharing.GetDownloadUrlOfFileInPublicFolder
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("public_key", System.Net.WebUtility.UrlEncode(FolderPublicKey))
        parameters.Add("path", FilePathInsidePublicFolder)

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim RequestUri = New pUri("resources/download", parameters)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri).ConfigureAwait(False)
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

#Region "PublicUrlToPublicKey"
    Public Async Function _PublicUrlToPublicKey(PublicUrl As Uri) As Task(Of String) Implements ISharing.PublicUrlToPublicKey
        PublicUrl.Validation(ItemTypeEnum.both)
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("public_key", System.Net.WebUtility.UrlEncode(PublicUrl.ToString))
        parameters.Add("limit", 0)

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim RequestUri = New pUri("public/resources", parameters)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If response.StatusCode = Net.HttpStatusCode.OK Then
                    Return result.Jobj.SelectToken("public_key").ToString
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "PublicUrlToDirectUrl"
    Public Async Function _PublicUrlToDownloadUrl(PublicUrl As Uri) As Task(Of String) Implements ISharing.PublicUrlToDirectUrl
        PublicUrl.Validation(ItemTypeEnum.both)
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("public_key", System.Net.WebUtility.UrlEncode(PublicUrl.ToString))
        parameters.Add("limit", 0)

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim RequestUri = New pUri("public/resources", parameters)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If response.StatusCode = Net.HttpStatusCode.OK Then
                    Return result.Jobj.SelectToken("file").ToString
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "Metadata"
    Public Async Function _Metadata(PublicUrl As Uri) As Task(Of JSON_FileMetadata) Implements ISharing.Metadata
        PublicUrl.Validation(ItemTypeEnum.both)
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("public_key", System.Net.WebUtility.UrlEncode(PublicUrl.ToString))
        parameters.Add("limit", 0)

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim RequestUri = New pUri("public/resources", parameters)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If response.StatusCode = Net.HttpStatusCode.OK Then
                    Return JsonConvert.DeserializeObject(Of JSON_FileMetadata)(result, JSONhandler)
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region

    '#Region "DownloadFileFromPublicFolder"
    '    Public Async Function GET_DownloadFileFromPublicFolder(PublicFolderUrl As Uri, FilePathToDownload As String, FileSaveDir As String, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task(Of Boolean) Implements IClient.DownloadFileFromPublicFolder
    '        'DownloadUrl = If(String.IsNullOrEmpty(DownloadUrl), (Await POST_GetDownloadUrl(PublicFolderUrl, FilePathToDownload)).href, DownloadUrl)
    '        'Dim Fname = Net.Http.UriExtensions.ParseQueryString(New Uri(DownloadUrl)).[Get]("filename")
    '        Dim meta = Await _Metadata(PublicFolderUrl)

    '        If ReportCls Is Nothing Then ReportCls = New Progress(Of ReportStatus)
    '        ReportCls.Report(New ReportStatus With {.Finished = False, .TextStatus = "Initializing..."})
    '        Try
    '            Dim progressHandler As New Net.Http.Handlers.ProgressMessageHandler(HCHandler)
    '            AddHandler progressHandler.HttpReceiveProgress, (Function(sender, e)
    '                                                                 ReportCls.Report(New ReportStatus With {.ProgressPercentage = e.ProgressPercentage, .BytesTransferred = e.BytesTransferred, .TotalBytes = If(e.TotalBytes Is Nothing, 0, e.TotalBytes), .TextStatus = "Downloading..."})
    '                                                             End Function)
    '            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '            Dim localHttpClient As New Net.Http.HttpClient(progressHandler)
    '            localHttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ISisYandexSDK") '' UserAgent
    '            localHttpClient.DefaultRequestHeaders.ConnectionClose = m_CloseConnection
    '            localHttpClient.Timeout = m_TimeOut
    '            Dim RequestUri = New Uri(DownloadUrl)
    '            '''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''
    '            Using ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri, Net.Http.HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(False)

    '                If ResPonse.IsSuccessStatusCode Then
    '                    ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = (String.Format("[{0}] Downloaded successfully.", Fname))})
    '                Else
    '                    ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = ((String.Format("Error code: {0}", ResPonse.ReasonPhrase)))})
    '                End If

    '                ResPonse.EnsureSuccessStatusCode()
    '                Dim stream_ = Await ResPonse.Content.ReadAsStreamAsync()
    '                Dim FPathname As String = String.Concat(OutputDirPath.TrimEnd("\"), "\", Fname)
    '                Using fileStream = New IO.FileStream(FPathname, IO.FileMode.Append, IO.FileAccess.Write)
    '                    stream_.CopyTo(fileStream)
    '                End Using
    '            End Using
    '        Catch ex As Exception
    '            ReportCls.Report(New ReportStatus With {.Finished = True})
    '            If ex.Message.ToString.ToLower.Contains("a task was canceled") Then
    '                ReportCls.Report(New ReportStatus With {.TextStatus = ex.Message})
    '            Else
    '                Throw CType(ExceptionCls.CreateException(ex.Message, ex.Message), YandexException)
    '            End If
    '        End Try
    '    End Function
    '#End Region

#Region "DownloadPublicFile"
    Public Async Function _DownloadPublicFile(PublicUrl As Uri, FileSaveDir As String, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task(Of Boolean) Implements ISharing.DownloadPublicFile
        PublicUrl.Validation(ItemTypeEnum.file)
        Dim meta = Await _Metadata(PublicUrl)

        If ReportCls Is Nothing Then ReportCls = New Progress(Of ReportStatus)
        ReportCls.Report(New ReportStatus With {.Finished = False, .TextStatus = "Initializing..."})
        Try
            Dim progressHandler As New Net.Http.Handlers.ProgressMessageHandler(New HCHandler)
            AddHandler progressHandler.HttpReceiveProgress, (Function(sender, e)
                                                                 ReportCls.Report(New ReportStatus With {.ProgressPercentage = e.ProgressPercentage, .BytesTransferred = e.BytesTransferred, .TotalBytes = If(e.TotalBytes Is Nothing, 0, e.TotalBytes), .TextStatus = "Downloading..."})
                                                             End Function)
            Dim localHttpClient As New HttpClient(progressHandler)
            '''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''
            Using ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(New Uri(meta.DownloadUrl), Net.Http.HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(False)

                ResPonse.EnsureSuccessStatusCode()
                Dim stream_ = Await ResPonse.Content.ReadAsStreamAsync()
                Dim FPathname As String = IO.Path.Combine(FileSaveDir, meta.name)
                Using fileStream = New IO.FileStream(FPathname, IO.FileMode.Append, IO.FileAccess.Write)
                    stream_.CopyTo(fileStream)
                End Using

                If ResPonse.IsSuccessStatusCode Then
                    ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = (String.Format("[{0}] Downloaded successfully.", meta.name))})
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

End Class
