Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments
Imports SMRUCC.WebCloud.HTTPInternal.Platform

''' <summary>
''' Explore the server file system.
''' </summary>
<[Namespace]("file")> Public Class FileSystem
    Inherits WebApp

    Public ReadOnly Property DIR As String

    Public Sub New(main As PlatformEngine)
        MyBase.New(main)

        DIR = App.GetVariable(NameOf(DIR))

        If String.IsNullOrEmpty(DIR) Then
            DIR = main.wwwroot.FullName
        End If
    End Sub

    <ExportAPI("/file/ls.vbs", Usage:="/file/ls.vbs?d=<DIR_name>")>
    <[GET](GetType(String()))>
    Public Function listDIR(request As HttpRequest, response As HttpResponse) As Boolean
        Dim d$ = request.URLParameters(NameOf(d))
        Dim folder As String

        If d.IsBlank Then
            d = "/"
        End If
        If d = "/" Then
            folder = DIR
        Else
            folder = (DIR & "/" & d).GetDirectoryFullPath
        End If

        Dim files As File() =
            (ls - l - "*.*" <= folder) _
            .ToArray(Function(path$) File.Create(path))
        Dim folders As File() =
            (ls - l - lsDIR - ".+" <= folder) _
            .ToArray(Function(path$) File.Create(path, isDIR:=True))
        Dim json As New ListResponse With {
            .DIR = d,
            .DIRs = folders,
            .Files = files
        }

        Call response.WriteJSON(json)

        Return True
    End Function

    Public Overrides Function Page404() As String
        Return ""
    End Function
End Class

Public Structure File

    Public Property FileName As String
    Public Property Length As Long
    Public Property [Date] As Date

    Public Shared Function Create(path$, Optional isDIR As Boolean = False) As File
        If isDIR Then
            Dim dir As New DirectoryInfo(path)
            Return New File With {
                .Date = dir.LastWriteTime,
                .FileName = dir.Name,
                .Length = 0
            }
        Else
            Dim file As New FileInfo(path)
            Return New File With {
                .Date = file.LastWriteTime,
                .FileName = file.Name,
                .Length = file.Length
            }
        End If
    End Function

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Structure

Public Structure ListResponse

    Public Property DIR As String
    Public Property DIRs As File()
    Public Property Files As File()

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Structure