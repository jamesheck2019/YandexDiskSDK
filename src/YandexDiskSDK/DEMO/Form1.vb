Imports YandexDiskSDK
Imports Newtonsoft.Json.Linq
Imports YandexDiskSDK.JSON
Imports Newtonsoft.Json
Imports YandexDiskSDK.utilitiez

Public Class Form1

    'Dim toknz As New ISisYandexSDK.AuthTokenKey("AQAAAAAqgSk1AAUjuOcWyeR4f0HXhXKog7r7XfU")
    'Dim toknz As New ISisYandexSDK.AuthTokenKey("AQAAAAAo41LsAAUjuPoR_wnK-EmXlTdexEsS0lE")
    Dim YandexClient As YandexDiskSDK.IClient = New YandexDiskSDK.YClient("AQAAAAAxclNHAAUjuEBHozP1cEwxmbrExrrNVAM")



    Private Async Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        'Dim rslt = Await YandexClient.List("/")
        'Dim rslt = Await YandexClient.Item("/").ListFiles(New List(Of FilterEnum) From {FilterEnum.executable, FilterEnum.unknown}, True, Nothing, utilitiez.SortEnum.name)
        'Dim rslt = Await YandexClient.Item("/").ListFolders()
        'Dim rslt = Await YandexClient.Item("disk:/Winter.jpg").FD_Move("disk:/hk026f", Nothing, False)
        'Dim rslt = Await YandexClient.Item("disk:/Mountains.jpg").FD_Rename("Mouins.jpg")
        'Dim rslt = Await YandexClient.Item("disk:/hk026f").D_Create("new folder")
        'Dim rslt = Await YandexClient.Item("disk:/Winter.jpg").D_Create("new folder")
        'Dim rslt = Await YandexClient.Item("disk:/Winter.jpg").F_GetDownloadUrl
        'Dim rslt = Await YandexClient.Item("disk:/hk026f").FD_Metadata
        'Dim rslt = Await YandexClient.Sharing.ParsePublicFolder("https://yadi.sk/d/rQ7HKI9lttd8Pw", Nothing, utilitiez.SortEnum.name, True, utilitiez.PreviewSize.S_150, 300, 0)
        'Dim rslt = Await YandexClient.Sharing.ListAllSharedLinks(ItemTypeEnum.both, Fields._embedded)
        'Dim rslt = Await YandexClient.Item("disk:/hk026f").FD_Privacy(PrivacyEnum.Public)
        'Dim rslt = Await YandexClient.Sharing.PublicUrlToPublicKey("https://yadi.sk/i/PsBMw1Qm4_L9QA")
        'Dim rslt = Await YandexClient.Sharing.Metadata(New Uri("https://yadi.sk/i/PsBMw1Qm4_L9QA"))
        'Dim rslt = Await YandexClient.Sharing.PublicUrlToDownloadUrl(New Uri("https://yadi.sk/i/PsBMw1Qm4_L9QA"))
        'Dim rslt = Await YandexClient.RecycleBin(Nothing).List
        'Dim rslt = Await YandexClient.Item("disk:/Winter.jpg").FD_AddTag(New With {.CustomID = 33554, .Country = "USA"})
        'Dim rslt = Await YandexClient.ListLatestUploadedFiles

        'MsgBox(rslt)
        'For Each fle In rslt._Files
        '    DataGridView1.Rows.Add(fle.name, fle.path)
        'Next
        'For Each fld In rslt._Folders
        '    DataGridView1.Rows.Add(fld.name, fld.path)
        'Next
    End Sub




    'Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
    '    '"1c9f6b4583e94fa9a9952ed092e4c4b3","a650468c1f2646448242c71cca8717f5"
    '    '"bcd61916f44a4d00a91c9b8b4b502d19","13e6141a1fb74d8b8121e36ad3879bab"
    '    Try
    '        Dim rslt = YandexDiskSDK.GetToken.OneYearToken(utilitiez.ResponseType.code, "bcd61916f44a4d00a91c9b8b4b502d19")
    '        'StatusL.Text = rslt.path
    '        'RichTextBox1.Text = rslt
    '    Catch ex As Exception
    '        MsgBox(ex.Message)
    '    End Try
    'End Sub

    'Private Async Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
    '"6935543"
    'Try
    '    Dim rslt = Await YandexDiskSDK.GetToken.Get_TokenFromCode("bcd61916f44a4d00a91c9b8b4b502d19", "2114224", "13e6141a1fb74d8b8121e36ad3879bab")
    '    'StatusL.Text = rslt.path
    '    'RichTextBox1.Text = rslt.access_token
    'Catch ex As Exception
    '    MsgBox(ex.Message)
    'End Try
    'End Sub


End Class
