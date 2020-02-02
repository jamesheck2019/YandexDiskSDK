Imports YandexDiskSDK.JSON
Imports Newtonsoft.Json

''' <summary>
''' https://tech.yandex.com/oauth/doc/dg/concepts/about-docpage/
''' </summary>
Public Class GetToken


    ''' <summary>
    ''' One Year Token from ClientID 
    ''' 'https://tech.yandex.com/oauth/doc/dg/reference/web-client-docpage/
    ''' </summary>
    ''' <param name="ClientID">application ID: get it from here https://oauth.yandex.com/client/new</param>
    Shared Function OneYearToken(ResponseType As utilitiez.ResponseType, ClientID As String) As String
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("response_type", ResponseType.ToString)
        parameters.Add("client_id", ClientID)
        parameters.Add("device_name", "YandexDiskSDK")
        parameters.Add("display", "popup")
        parameters.Add("force_confirm", "false")

        Dim tHErSULT = ("https://oauth.yandex.com/authorize" + utilitiez.AsQueryString(parameters)).ToLower
        Return tHErSULT
    End Function

    ''' <summary>
    ''' Exchanging an authorization code for a token
    ''' https://tech.yandex.com/oauth/doc/dg/reference/auto-code-client-docpage/
    ''' </summary>
    ''' <param name="ClientID">application ID</param>
    ''' <param name="AuthorizationCode">confirmation code</param>
    ''' <param name="ClientSecret">application password</param>
    Shared Async Function GetTokenFromCode(ClientID As String, AuthorizationCode As String, ClientSecret As String) As Task(Of JSONexchangingVerificationCodeForToken)
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("grant_type", "authorization_code")
        parameters.Add("code", AuthorizationCode)
        parameters.Add("client_id", ClientID)
        parameters.Add("client_secret", ClientSecret)
        parameters.Add("device_name", "YandexDiskSDK")

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage()
            HtpReqMessage.RequestUri = New Uri("https://oauth.yandex.com/token" + utilitiez.AsQueryString(parameters))
            HtpReqMessage.Method = Net.Http.HttpMethod.Post
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result As String = Await response.Content.ReadAsStringAsync()

                If response.IsSuccessStatusCode Then
                    Return JsonConvert.DeserializeObject(Of JSONexchangingVerificationCodeForToken)(result, New Newtonsoft.Json.JsonSerializerSettings() With {.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore, .NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore})
                Else
                    Dim errorInfo = JsonConvert.DeserializeObject(Of JSON_Error)(result)
                    Throw CType(ExceptionCls.CreateException(errorInfo._ErrorMessage, response.ReasonPhrase), YandexException)
                End If
            End Using
        End Using
    End Function

End Class
