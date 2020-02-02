Imports Newtonsoft.Json
Imports YandexDiskSDK.JSON
Imports YandexDiskSDK.utilitiez

Module Base


    Public Property APIbase As String = "https://cloud-api.yandex.net/v1/disk/"
    Public Property m_TimeOut As System.TimeSpan = Threading.Timeout.InfiniteTimeSpan
    Public Property m_CloseConnection As Boolean = True
    Public Property JSONhandler As New JsonSerializerSettings() With {.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore, .NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore}
    Public Property authToken() As String
    Friend Property ConnectionSetting As ConnectionSettings

    Private _proxy As ProxyConfig
    Public Property m_proxy As ProxyConfig
        Get
            Return If(_proxy, New ProxyConfig)
        End Get
        Set(value As ProxyConfig)
            _proxy = value
        End Set
    End Property

    Public Class HCHandler
        Inherits Net.Http.HttpClientHandler
        Sub New()
            MyBase.New()
            If m_proxy.SetProxy Then
                MaxRequestContentBufferSize = 1 * 1024 * 1024
                Proxy = New Net.WebProxy(String.Format("http://{0}:{1}", m_proxy.ProxyIP, m_proxy.ProxyPort), True, Nothing, New Net.NetworkCredential(m_proxy.ProxyUsername, m_proxy.ProxyPassword))
                UseProxy = m_proxy.SetProxy
            End If
        End Sub
    End Class

    Public Class HttpClient
        Inherits Net.Http.HttpClient
        Sub New(HCHandler As HCHandler)
            MyBase.New(HCHandler)
            MyBase.DefaultRequestHeaders.UserAgent.ParseAdd("YandexDiskSDK")
            MyBase.DefaultRequestHeaders.Authorization = New Net.Http.Headers.AuthenticationHeaderValue("OAuth", authToken)
            MyBase.DefaultRequestHeaders.Accept.Add(New Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"))
            MyBase.DefaultRequestHeaders.ConnectionClose = m_CloseConnection
            Timeout = m_TimeOut
        End Sub
        Sub New(progressHandler As Net.Http.Handlers.ProgressMessageHandler)
            MyBase.New(progressHandler)
            MyBase.DefaultRequestHeaders.UserAgent.ParseAdd("YandexDiskSDK")
            MyBase.DefaultRequestHeaders.Authorization = New Net.Http.Headers.AuthenticationHeaderValue("OAuth", authToken)
            MyBase.DefaultRequestHeaders.Accept.Add(New Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"))
            MyBase.DefaultRequestHeaders.ConnectionClose = m_CloseConnection
            Timeout = m_TimeOut
        End Sub
    End Class

    Public Class pUri
        Inherits Uri
        Sub New(Action As String, Optional Parameters As Dictionary(Of String, String) = Nothing)
            MyBase.New(APIbase + Action + If(Parameters Is Nothing, Nothing, utilitiez.AsQueryString(Parameters)))
        End Sub
    End Class

    Public Function ShowError(result As String)
        Dim errorInfo = JsonConvert.DeserializeObject(Of JSON_Error)(result, JSONhandler)
        Throw CType(ExceptionCls.CreateException(errorInfo._ErrorMessage, errorInfo._ErrorCode), YandexException)
    End Function

    <Runtime.CompilerServices.Extension()>
    Public Function Jobj(response As String) As Linq.JObject
        Return Linq.JObject.Parse(response)
    End Function

    <Runtime.CompilerServices.Extension()>
    Public Function Validation(PublicUrl As Uri, UrlType As ItemTypeEnum) As Boolean
        Select Case UrlType
            Case ItemTypeEnum.dir
                Validation = PublicUrl.ToString.ToLower.Contains("/d/")
            Case ItemTypeEnum.file
                Validation = PublicUrl.ToString.ToLower.Contains("/i/")
            Case ItemTypeEnum.both
                Validation = If(PublicUrl.ToString.ToLower.Contains("/d/") OrElse PublicUrl.ToString.ToLower.Contains("/i/"), True, False)
        End Select
        If Validation = False Then Throw ExceptionCls.CreateException("Not a vild public url.", 202)
    End Function

End Module
