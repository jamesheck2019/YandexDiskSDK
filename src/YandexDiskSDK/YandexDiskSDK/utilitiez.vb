
Imports System.ComponentModel

Public Class utilitiez

    Public Shared Function AsQueryString(parameters As Dictionary(Of String, String)) As String
        If Not parameters.Any() Then Return String.Empty
        Dim builder = New Text.StringBuilder("?")
        Dim separator = String.Empty
        For Each kvp In parameters.Where(Function(P) Not String.IsNullOrEmpty(P.Value))
            builder.AppendFormat("{0}{1}={2}", separator, Net.WebUtility.UrlEncode(kvp.Key), Net.WebUtility.UrlEncode(kvp.Value.ToString()))
            separator = "&"
        Next
        Return builder.ToString()
    End Function

#Region "EnumUtils"
    Public Shared Function stringValueOf(ByVal value As [Enum]) As String
        Dim fi As Reflection.FieldInfo = value.[GetType]().GetField(value.ToString())
        Dim attributes As ComponentModel.DescriptionAttribute() = CType(fi.GetCustomAttributes(GetType(ComponentModel.DescriptionAttribute), False), ComponentModel.DescriptionAttribute())

        If attributes.Length > 0 Then
            Return attributes(0).Description
        Else
            Return value.ToString()
        End If
    End Function

    Public Shared Function enumValueOf(ByVal value As String, ByVal enumType As Type) As Object
        Dim names As String() = [Enum].GetNames(enumType)

        For Each name As String In names

            If stringValueOf(CType([Enum].Parse(enumType, name), [Enum])).Equals(value) Then
                Return [Enum].Parse(enumType, name)
            End If
        Next

        Throw New ArgumentException("The string is not a description or value of the specified enum.")
    End Function
#End Region

    Enum UploadTypes
        FilePath
        Stream
        BytesArry
    End Enum
    Public Enum DestinationType
        disk
        app
        trash
    End Enum
    Enum ResponseType
        token
        code
    End Enum
    Enum Fields
        [nothing]
        name
        _embedded
        exif
        created
        modified
        path
        comment_ids
        type
        revision
        items
    End Enum
    Enum SortEnum
        [nothing]
        name
        created
        modified
        path
        size
    End Enum
    Enum FilterEnum
        audio
        backup
        book
        compressed
        data
        development
        diskimage
        document
        encoded
        executable
        flash
        font
        image
        settings
        spreadsheet
        text
        unknown
        video
        web
    End Enum
    Enum PrivacyEnum
        [Public]
        [Private]
    End Enum
    Enum ItemTypeEnum
        both
        file
        dir
    End Enum


    Enum PreviewSizeEnum
        <Description("S")> S_150
        <Description("M")> S_300
        <Description("L")> S_500
        <Description("XL")> S_800
        <Description("XXL")> S_1024
        <Description("XXXL")> S_1280
    End Enum

    'Public Class PreviewSize
    '    Public Const [Default] As String = Nothing
    '    Public Const S_150 As String = "S"
    '    Public Const S_300 As String = "M"
    '    Public Const S_500 As String = "L"
    '    Public Const S_800 As String = "XL"
    '    Public Const S_1024 As String = "XXL"
    '    Public Const S_1280 As String = "XXXL"
    '    ''' <summary>
    '    ''' The width for example, "120" or "120x"
    '    ''' </summary>
    '    Public Const Width As String = "120"
    '    ''' <summary>
    '    ''' The height for example, "x145"
    '    ''' </summary>
    '    Public Const Height As String = "x110"
    '    ''' <summary>
    '    ''' The widthxheight.. The exact size (in the widthxheight format, for example "120x240").
    '    ''' </summary>
    '    Public Const widthXheight As String = "120x240"
    'End Class
End Class


Public Class ConnectionSettings
    Public Property TimeOut As System.TimeSpan = Nothing
    Public Property CloseConnection As Boolean? = True
    Public Property Proxy As ProxyConfig = Nothing
End Class

Public Class Url
    ''' <summary>
    ''' Basically a Path.Combine for URLs. Ensures exactly one '/' seperates each segment,
    ''' and exactly on '&amp;' seperates each query parameter.
    ''' URL-encodes illegal characters but not reserved characters.
    ''' </summary>
    ''' <param name="parts">URL parts to combine.</param>
    Public Shared Function Combine(ParamArray parts As String()) As String
        If parts Is Nothing Then
            Throw New ArgumentNullException("parts")
        End If
        Dim text As String = ""
        Dim flag As Boolean = False
        Dim flag2 As Boolean = False
        For Each text2 As String In parts
            If Not String.IsNullOrEmpty(text2) Then
                If text.EndsWith("?") OrElse text2.StartsWith("?") Then
                    text = Url.CombineEnsureSingleSeperator(text, text2, "?"c)
                ElseIf text.EndsWith("#") OrElse text2.StartsWith("#") Then
                    text = Url.CombineEnsureSingleSeperator(text, text2, "#"c)
                ElseIf flag2 Then
                    text += text2
                ElseIf flag Then
                    text = Url.CombineEnsureSingleSeperator(text, text2, "&"c)
                Else
                    text = Url.CombineEnsureSingleSeperator(text, text2, "/"c)
                End If
                If text2.Contains("#") Then
                    flag = False
                    flag2 = True
                ElseIf Not flag2 AndAlso text2.Contains("?") Then
                    flag = True
                End If
            End If
        Next
        Return Url.EncodeIllegalCharacters(text, False)
    End Function
    Private Shared Function CombineEnsureSingleSeperator(a As String, b As String, seperator As Char) As String
        If String.IsNullOrEmpty(a) Then
            Return b
        End If
        If String.IsNullOrEmpty(b) Then
            Return a
        End If
        Return a.TrimEnd(New Char() {seperator}) + seperator.ToString() + b.TrimStart(New Char() {seperator})
    End Function
    ''' <summary>
    ''' URL-encodes characters in a string that are neither reserved nor unreserved. Avoids encoding reserved characters such as '/' and '?'. Avoids encoding '%' if it begins a %-hex-hex sequence (i.e. avoids double-encoding).
    ''' </summary>
    ''' <param name="s">The string to encode.</param>
    ''' <param name="encodeSpaceAsPlus">If true, spaces will be encoded as + signs. Otherwise, they'll be encoded as %20.</param>
    ''' <returns>The encoded URL.</returns>
    Private Shared Function EncodeIllegalCharacters(ByVal s As String, ByVal Optional encodeSpaceAsPlus As Boolean = False) As String
        If String.IsNullOrEmpty(s) Then Return s
        If encodeSpaceAsPlus Then s = s.Replace(" ", "+")
        If Not s.Contains("%") Then Return Uri.EscapeUriString(s)
        Return System.Text.RegularExpressions.Regex.Replace(s, "(.*?)((%[0-9A-Fa-f]{2})|$)", Function(c)
                                                                                                 Dim a = c.Groups(1).Value
                                                                                                 Dim b = c.Groups(2).Value
                                                                                                 Return Uri.EscapeUriString(a) + b
                                                                                             End Function)
    End Function

End Class