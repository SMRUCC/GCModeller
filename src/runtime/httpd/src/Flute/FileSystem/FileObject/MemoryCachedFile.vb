Imports System.IO
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes

Namespace FileSystem

    ''' <summary>
    ''' a <see cref="FileObject"/> whose content is fully held in memory as a
    ''' <see cref="MemoryStream"/>; suitable for caching small files.
    ''' </summary>
    Public Class MemoryCachedFile : Inherits FileObject

        ''' <summary>
        ''' the in-memory stream that holds the cached file bytes.
        ''' </summary>
        ReadOnly cache As MemoryStream

        ''' <summary>
        ''' the byte length of the cached content.
        ''' </summary>
        ''' <returns>the cached byte count.</returns>
        Public Overrides ReadOnly Property ContentLength As Long
            Get
                Return cache.Length
            End Get
        End Property

        ''' <summary>
        ''' create a memory cached file from the given raw bytes.
        ''' </summary>
        ''' <param name="fileName$">the file name of the resource.</param>
        ''' <param name="data">the raw bytes to cache in memory.</param>
        ''' <param name="mime">the optional content type; auto-detected when omitted.</param>
        Sub New(fileName$, data As Byte(), Optional mime As ContentType = Nothing)
            Call MyBase.New(fileName, mime)

            ' create cache data stream
            Me.cache = New MemoryStream(data)
        End Sub

        ''' <summary>
        ''' get the in-memory stream holding the cached content.
        ''' </summary>
        ''' <returns>the cached <see cref="MemoryStream"/>.</returns>
        Public Overrides Function GetResource() As Stream
            Return cache
        End Function

        ''' <summary>
        ''' get a copy of the cached bytes.
        ''' </summary>
        ''' <returns>the cached file bytes.</returns>
        Public Overrides Function GetByteBuffer() As Byte()
            Return cache.ToArray
        End Function
    End Class
End Namespace