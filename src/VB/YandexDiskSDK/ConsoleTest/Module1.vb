Imports YandexDiskSDK.utilitiez

Module Module1

    Sub Main()
        Dim interfaces As New List(Of System.Type)

        Dim YClient As New YandexDiskSDK.YClient(0)
        interfaces.AddRange(YClient.GetType().GetInterfaces().ToList)

        Dim ItemClient As New YandexDiskSDK.ItemClient(0)
        interfaces.AddRange(ItemClient.GetType().GetInterfaces().ToList)

        Dim RecycleBinClient As New YandexDiskSDK.RecycleBinClient(0)
        interfaces.AddRange(RecycleBinClient.GetType().GetInterfaces().ToList)

        Dim SharingClient As New YandexDiskSDK.SharingClient
        interfaces.AddRange(SharingClient.GetType().GetInterfaces().ToList)


        For Each iface As Type In interfaces
            Dim methods = iface.GetMethods()

            For Each method As Reflection.MethodInfo In methods
                Console.WriteLine(method.Name)
            Next
        Next

        Console.WriteLine("press any key to exit...")
        Console.ReadKey()
    End Sub



    Async Function AllTasks() As Task
        'first get auth token (one time only)
        Dim tokn = YandexDiskSDK.GetToken.OneYearToken(YandexDiskSDK.utilitiez.ResponseType.token, "yourclientid")
        'OR
        Dim cod = YandexDiskSDK.GetToken.OneYearToken(YandexDiskSDK.utilitiez.ResponseType.code, "yourclientid")
        Dim exchanecodewithtoken = YandexDiskSDK.GetToken.GetTokenFromCode("yourclientid", cod, "yourAppSectretKey")

        ''set proxy and connection options
        Dim con As New YandexDiskSDK.ConnectionSettings With {.CloseConnection = True, .TimeOut = TimeSpan.FromMinutes(30), .Proxy = New YandexDiskSDK.ProxyConfig With {.SetProxy = True, .ProxyIP = "127.0.0.1", .ProxyPort = 8888, .ProxyUsername = "user", .ProxyPassword = "pass"}}
        ''set api client
        Dim CLNT As YandexDiskSDK.IClient = New YandexDiskSDK.YClient("xxxxxtokenxxxxx", con)

        ''general
        Await CLNT.UserInfo()
        Await CLNT.CheckOperationStatus("")
        Await CLNT.ListLatestUploadedFiles()

        ''singleitem
        '' [D_] = dir
        '' [F_] = file
        '' [FD_] = file & dir
        Await CLNT.Item("folder_path").D_Create("new folder")
        Dim cts As New Threading.CancellationTokenSource()
        Dim _ReportCls As New Progress(Of YandexDiskSDK.ReportStatus)(Sub(ReportClass As YandexDiskSDK.ReportStatus) Console.WriteLine(String.Format("{0} - {1}% - {2}", String.Format("{0}/{1}", (ReportClass.BytesTransferred), (ReportClass.TotalBytes)), CInt(ReportClass.ProgressPercentage), ReportClass.TextStatus)))
        Await CLNT.Item("folder_path").D_DownloadAsZip("c:\\downloads", _ReportCls, cts.Token)
        Await CLNT.Item("file_path OR folder_path").FD_AddTag("{""foo"":1, ""bar"":2}")
        Await CLNT.Item("file_path OR folder_path").FD_Copy("folder_path", Nothing, False)
        Await CLNT.Item("file_path OR folder_path").FD_Delete
        Await CLNT.Item("file_path OR folder_path").FD_Move("folder_path", Nothing, False)
        Await CLNT.Item("file_path OR folder_path").FD_Exists
        Await CLNT.Item("file_path OR folder_path").FD_Metadata(True, PreviewSizeEnum.S_300)
        Await CLNT.Item("file_path OR folder_path").FD_Privacy(PrivacyEnum.Public)
        Await CLNT.Item("file_path OR folder_path").FD_Rename("newname.jpg")
        Await CLNT.Item("file_path OR folder_path").FD_Trash
        Await CLNT.Item("file_path").F_Download("c:\\downloads", _ReportCls, cts.Token)
        Await CLNT.Item("file_path").F_DownloadAsStream(_ReportCls, cts.Token)
        Await CLNT.Item("file_path").F_DownloadLarge("c:\\downloads", _ReportCls, cts.Token)
        Await CLNT.Item("file_path").F_GetDownloadUrl()
        Await CLNT.Item("file_path").F_GetThumbnail(New Uri("preview_url"), _ReportCls, cts.Token)
        Await CLNT.Item("root_path OR folder_path").D_List(False, PreviewSizeEnum.S_500, SortEnum.created, 20, 0)
        Await CLNT.Item("root_path OR folder_path").D_ListAllFiles(New List(Of FilterEnum) From {FilterEnum.audio, FilterEnum.video}, True, PreviewSizeEnum.S_150, SortEnum.path, 20, 0)
        Await CLNT.Item("folder_path").D_Upload("c:\\VIDO.mp4", UploadTypes.FilePath, "VIDO.mp4", True, _ReportCls, cts.Token)
        Await CLNT.Item("folder_path").D_UploadRemotely(New Uri("https://domain.com/watch.mp4"), "watch.mp4", False)

        ''RecycleBin
        Await CLNT.RecycleBin(CLNT.RootPath(DestinationType.trash)).EmptyTrashBin
        Await CLNT.RecycleBin("trashroot_path OR trashfolder_path").List(False, PreviewSizeEnum.S_150, SortEnum.size, 20, 0)
        Await CLNT.RecycleBin("trashfile_path OR trashfolder_path").Restore

        ''root 
        Dim root = CLNT.RootPath(DestinationType.disk)

        ''Sharing
        Await CLNT.Sharing.DownloadPublicFile(New Uri("https://yadi.sk/i/xxxxx"), "c:\\downloads", _ReportCls, cts.Token)
        Await CLNT.Sharing.GetDownloadUrlOfFileInPublicFolder("https://yadi.sk/d/xxxxx", "/img.jpg")
        Await CLNT.Sharing.ListAllSharedLinks(ItemTypeEnum.file, Fields.name, Nothing, 20, 0)
        Await CLNT.Sharing.Metadata(New Uri("https://yadi.sk/i/xxxxx"))
        Await CLNT.Sharing.PublicFolderContents("https://yadi.sk/d/xxxxx", Nothing, SortEnum.path, False, PreviewSizeEnum.S_1024, 20, 0)
        Await CLNT.Sharing.PublicUrlToDirectUrl(New Uri("https://yadi.sk/i/xxxxx"))
        Await CLNT.Sharing.PublicUrlToPublicKey(New Uri("https://yadi.sk/d/xxxxx"))

    End Function



End Module
