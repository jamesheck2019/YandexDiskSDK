Imports System.ComponentModel
Imports Newtonsoft.Json
Imports YandexDiskSDK.utilitiez

Namespace JSON

    Public Class JSON_Error
        <JsonProperty("description")> Public Property _ErrorMessage As String
        <JsonProperty("error")> Public Property _ErrorCode As String
    End Class

#Region "CheckOperationStatus"
    Public Class JSON_CheckOperationStatus
        Enum OperationStatus
            success
            failure
            inprogress
            [error]
        End Enum
        <Browsable(False)> <Bindable(False)> <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> <EditorBrowsable(EditorBrowsableState.Never)>
        Public Property status As String
        Public ReadOnly Property ResultStatus As OperationStatus
            Get
                Dim rslt As New OperationStatus
                If status = "success" Then
                    rslt = OperationStatus.success
                ElseIf status = "failure" Then
                    rslt = OperationStatus.failure
                ElseIf status = "in-progress" Then
                    rslt = OperationStatus.inprogress
                Else
                    rslt = OperationStatus.error
                End If
                Return rslt
            End Get
        End Property

    End Class
#End Region



#Region "userinfo"
    Public Class JSON_UserInfo
        Public Property max_file_size As String
        Public Property unlimited_autoupload_enabled As Boolean
        Public Property total_space As String
        Public Property trash_size As String
        Public Property is_paid As Boolean
        Public Property used_space As String
        Public Property system_folders As JSONSystem_Folders
        Public Property user As JSON_User
        Public Property revision As String
    End Class
    Public Class JSONSystem_Folders
        Public Property odnoklassniki As String
        Public Property google As String
        Public Property instagram As String
        Public Property vkontakte As String
        Public Property mailru As String
        Public Property downloads As String
        Public Property applications As String
        Public Property facebook As String
        Public Property social As String
        Public Property screenshots As String
        Public Property photostream As String
    End Class
#End Region



    Public Class JSON_PublicFolder
        Public Property public_key As String
        Public Property public_url As String
        <JsonProperty("_embedded")> Public Property ItemsList As JSON_FolderList
        Public Property name As String
        Public Property created As Date
        Public Property modified As Date
        Public Property owner As JSON_User
        Public Property path As String
        Public Property type As ItemTypeEnum
        Public Property views_count As Integer
    End Class

    Public Class JSON_User
        Public Property country As String
        Public Property login As String
        Public Property display_name As String
        Public Property uid As String
    End Class

    Public Class JSON_FolderList
        Public Property _Files As List(Of JSON_FileMetadata)
        Public Property _Folders As List(Of JSON_FolderMetadata)

        Public Property public_key As String
        Public Property sort As String
        Public Property limit As Integer
        Public Property offset As Integer
        Public Property path As String
        Public Property total As Integer
        Public ReadOnly Property HasMore As Boolean
            Get
                Return If(total < limit + offset, False, True)
            End Get
        End Property
    End Class

    Public Class JSON_FilesList
        <JsonProperty("items")> Public Property _Files As List(Of JSON_FileMetadata)
        Public Property limit As Integer
        Public Property offset As Integer
    End Class

    Public Class JSON_FoldersList
        <JsonProperty("items")> Public Property _Folders As List(Of JSON_FolderMetadata)
        Public Property limit As Integer
        Public Property offset As Integer
    End Class

    Public Class JSON_MixedMetadata
        Public Property name As String
        Public Property exif As Exif
        Public Property created As Date
        Public Property modified As Date
        Public Property path As String
        Public Property type As ItemTypeEnum
        Public Property size As Integer
        Public Property mime_type As String
        Public Property media_type As String
        Public Property preview As String
        Public Property sha256 As String
        Public Property md5 As String
        Public Property public_key As String
        Public Property public_url As String
        <JsonProperty("file")> Public Property DownloadUrl As String
        Public ReadOnly Property IsShared As Boolean
            Get
                Return String.IsNullOrEmpty(public_key)
            End Get
        End Property
    End Class

    Public Class JSON_FileMetadata
        Public Property name As String
        Public Property exif As Exif
        Public Property created As Date
        Public Property modified As Date
        Public Property path As String
        Public Property type As ItemTypeEnum
        Public Property size As Integer
        Public Property mime_type As String
        Public Property media_type As String
        Public Property preview As String
        Public Property sha256 As String
        Public Property md5 As String
        Public Property public_key As String
        Public Property public_url As String
        <JsonProperty("file")> Public Property DownloadUrl As String
        Public ReadOnly Property IsShared As Boolean
            Get
                Return String.IsNullOrEmpty(public_key)
            End Get
        End Property
    End Class

    Public Class JSON_FolderMetadata
        Public Property modified As Date
        Public Property name As String
        Public Property created As Date
        Public Property path As String
        Public Property type As ItemTypeEnum
        Public Property public_key As String
        Public Property public_url As String
        Public ReadOnly Property IsShared As Boolean
            Get
                Return String.IsNullOrEmpty(public_key)
            End Get
        End Property
    End Class

    Public Class Exif
        Public Property date_time As Date
    End Class

    Public Class Comment_Ids
        Public Property private_resource As String
        Public Property public_resource As String
    End Class

#Region "token"
    Public Class JSONexchangingVerificationCodeForToken
        Public Property token_type As String
        Public Property access_token As String
        Public Property expires_in As Integer
        Public Property refresh_token As String
    End Class
#End Region




End Namespace

