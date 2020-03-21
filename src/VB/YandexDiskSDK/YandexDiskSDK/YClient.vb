Imports YandexDiskSDK.utilitiez
Imports Newtonsoft.Json
Imports YandexDiskSDK.JSON
Imports Newtonsoft.Json.Linq

Public Class YClient
    Implements IClient


    Public Sub New(ByRef accessToken As String, Optional Settings As ConnectionSettings = Nothing)
        authToken = accessToken
        'Destination_Type = PathPattern
        ConnectionSetting = Settings
        If Settings Is Nothing Then
            m_proxy = Nothing
        Else
            m_proxy = Settings.Proxy
            m_CloseConnection = If(Settings.CloseConnection, True)
            m_TimeOut = If(Settings.TimeOut = Nothing, TimeSpan.FromMinutes(60), Settings.TimeOut)
        End If
        Net.ServicePointManager.Expect100Continue = True : Net.ServicePointManager.SecurityProtocol = Net.SecurityProtocolType.Tls Or Net.SecurityProtocolType.Tls11 Or Net.SecurityProtocolType.Tls12 Or Net.SecurityProtocolType.Ssl3
    End Sub


    Public ReadOnly Property Item(Path As String) As IItem Implements IClient.Item
        Get
            Return New ItemClient(Path)
        End Get
    End Property
    Public ReadOnly Property Sharing() As ISharing Implements IClient.Sharing
        Get
            Return New SharingClient()
        End Get
    End Property
    Public ReadOnly Property RecycleBin(Path As String) As IRecycleBin Implements IClient.RecycleBin
        Get
            Return New RecycleBinClient(Path)
        End Get
    End Property


    Public ReadOnly Property RootPath(PathPattern As DestinationType) As String Implements IClient.RootPath
        Get
            Select Case PathPattern
                Case DestinationType.app
                    Return "app:/"
                Case DestinationType.disk
                    Return "disk:/"
                Case DestinationType.trash
                    Return "trash:/"
            End Select
        End Get
    End Property

#Region "UserInfo"
    Public Async Function _UserInfo() As Task(Of JSON_UserInfo) Implements IClient.UserInfo
        Using localHttpClient As New HttpClient(New HCHandler)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(New pUri("")).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If response.StatusCode = Net.HttpStatusCode.OK Then
                    Return JsonConvert.DeserializeObject(Of JSON_UserInfo)(result, JSONhandler)
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region



#Region "ListLatestUploadedFiles"
    Public Async Function Get_LatestFilesUploaded(Optional Filter As FilterEnum? = Nothing, Optional Fields As Fields = Fields.nothing, Optional PreviewCrop As Boolean = True, Optional PreviewSize As PreviewSizeEnum = PreviewSizeEnum.S_150, Optional Limit As Integer = 200) As Task(Of JSON_FilesList) Implements IClient.ListLatestUploadedFiles
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("media_type", Filter.ToString)
        parameters.Add("preview_crop", PreviewCrop)
        parameters.Add("preview_size", stringValueOf(PreviewSize))
        parameters.Add("fields", Fields.ToString)
        parameters.Add("limit", Limit)

        Using localHttpClient As New HttpClient(New HCHandler)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(New pUri("resources/last-uploaded", parameters)).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If response.IsSuccessStatusCode Then
                    Return JsonConvert.DeserializeObject(Of JSON_FilesList)(result, JSONhandler)
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region


#Region "CopyingMovingDeletingUrluploadingOperationStatus"
    Public Async Function _CheckOperationStatus(OperationHref As String) As Task(Of JSON_CheckOperationStatus) Implements IClient.CheckOperationStatus
        Using localHttpClient As New HttpClient(New HCHandler)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(New Uri(OperationHref)).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If response.StatusCode = Net.HttpStatusCode.OK Then
                    Return JsonConvert.DeserializeObject(Of JSON_CheckOperationStatus)(result, JSONhandler)
                Else
                    ShowError(result)
                End If
            End Using
        End Using
    End Function
#End Region


End Class
