Imports System.IO
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes

Namespace Core

    Public Class CachedFile

        Public ReadOnly Property Path As String
        Public ReadOnly Property bufs As Byte()
        Public ReadOnly Property content As ContentType

        Sub New(path As String)
            Me.Path = FileIO.FileSystem.GetFileInfo(path).FullName
            Me.bufs = File.ReadAllBytes(Me.Path)
            Me.Path = Me.Path.ToLower

            Dim ext As String = "." & Me.Path.Split("."c).Last

            Me.content = If(
                MIME.ExtDict.ContainsKey(ext),
                MIME.ExtDict(ext),
                MIME.UnknownType)
        End Sub

        Public Overrides Function ToString() As String
            Return $"({content.ToString}) {Path}"
        End Function

        ''' <summary>
        ''' 假若文件更新不频繁，可以在初始化阶段一次性的读取所有文件到内存中
        ''' </summary>
        ''' <param name="wwwroot"></param>
        ''' <returns></returns>
        Public Shared Function CacheAllFiles(wwwroot As String) As Dictionary(Of String, CachedFile)
            Dim allFiles As IEnumerable(Of String) = FileIO.FileSystem.GetFiles(
                wwwroot,
                FileIO.SearchOption.SearchAllSubDirectories,
                "*.*")
            Dim hash As New Dictionary(Of String, CachedFile)

            For Each file As String In allFiles
                hash(file) = New CachedFile(file)
            Next

            Return hash
        End Function
    End Class
End Namespace