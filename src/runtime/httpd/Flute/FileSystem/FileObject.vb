Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes

Namespace FileSystem

    Public MustInherit Class FileObject

        ''' <summary>
        ''' 文件的类型
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property mime As ContentType
        Public ReadOnly Property fileName As String

        Sub New(fileName$, Optional mime As ContentType = Nothing)
            Me.fileName = fileName
            Me.mime = mime

            If mime.IsEmpty Then
                Me.mime = fileName.FileMimeType
            End If
        End Sub

        Public MustOverride Function GetResource() As Stream

        Public Overrides Function ToString() As String
            Return fileName
        End Function
    End Class

    Public Class MemoryCachedFile : Inherits FileObject

        ReadOnly cache As MemoryStream

        Sub New(fileName$, data As Byte(), Optional mime As ContentType = Nothing)
            Call MyBase.New(fileName, mime)

            ' create cache data stream
            Me.cache = New MemoryStream(data)
        End Sub

        Public Overrides Function GetResource() As Stream
            Return cache
        End Function
    End Class

    Public Class VirtualMappedFile : Inherits FileObject

        Public ReadOnly Property mappedPath As String

        Public ReadOnly Property isValid As Boolean
            Get
                Return mappedPath.FileExists
            End Get
        End Property

        Sub New(fileName$, mappedPath$, Optional mime As ContentType = Nothing)
            Call MyBase.New(fileName, mime)

            Me.mappedPath = mappedPath
        End Sub

        Public Overrides Function GetResource() As Stream
            Return mappedPath.Open(FileMode.Open, doClear:=False)
        End Function
    End Class
End Namespace