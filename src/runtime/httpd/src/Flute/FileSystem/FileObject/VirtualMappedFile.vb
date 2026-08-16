Imports System.IO
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes

Namespace FileSystem

    ''' <summary>
    ''' a <see cref="FileObject"/> that maps a resource url onto a physical file
    ''' path; content is read on demand rather than cached in memory.
    ''' </summary>
    Public Class VirtualMappedFile : Inherits FileObject

        ''' <summary>
        ''' the physical file path that this resource is mapped onto.
        ''' </summary>
        Public ReadOnly Property mappedPath As String

        ''' <summary>
        ''' whether the mapped physical file still exists on disk.
        ''' </summary>
        ''' <returns><c>True</c> when the mapped file exists.</returns>
        Public ReadOnly Property isValid As Boolean
            Get
                Return mappedPath.FileExists
            End Get
        End Property

        ''' <summary>
        ''' the byte length of the mapped file.
        ''' </summary>
        ''' <returns>the mapped file length in bytes.</returns>
        Public Overrides ReadOnly Property ContentLength As Long
            Get
                Return mappedPath.FileLength
            End Get
        End Property

        ''' <summary>
        ''' create a virtual mapped file pointing at the given physical path.
        ''' </summary>
        ''' <param name="fileName$">the file name of the resource (served url name).</param>
        ''' <param name="mappedPath$">the physical file path to read content from.</param>
        ''' <param name="mime">the optional content type; auto-detected when omitted.</param>
        Sub New(fileName$, mappedPath$, Optional mime As ContentType = Nothing)
            Call MyBase.New(fileName, mime)

            Me.mappedPath = mappedPath
        End Sub

        ''' <summary>
        ''' open the mapped physical file as a readable stream.
        ''' </summary>
        ''' <returns>the file stream of the mapped path.</returns>
        Public Overrides Function GetResource() As Stream
            Return mappedPath.Open(FileMode.Open, doClear:=False)
        End Function

        ''' <summary>
        ''' read the full byte content of the mapped physical file.
        ''' </summary>
        ''' <returns>the mapped file bytes.</returns>
        Public Overrides Function GetByteBuffer() As Byte()
            Return mappedPath.ReadBinary
        End Function
    End Class
End Namespace