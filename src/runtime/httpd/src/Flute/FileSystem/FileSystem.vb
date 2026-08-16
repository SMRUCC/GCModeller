#Region "Microsoft.VisualBasic::3601f271fedb735ecd1581fd42ebfe24, src\Flute\FileSystem\FileSystem.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 264
    '    Code Lines: 169 (64.02%)
    ' Comment Lines: 55 (20.83%)
    '    - Xml Docs: 69.09%
    ' 
    '   Blank Lines: 40 (15.15%)
    '     File Size: 10.62 KB


    '     Class FileSystem
    ' 
    '         Properties: wwwroot
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: (+2 Overloads) AddCache, AddMapping, (+2 Overloads) AttachFolder, FileExists, GetByteBuffer
    '                   GetContentType, GetFileSize, GetResource, resolveFile, resourceUrl
    '                   ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports FolderHandle = Microsoft.VisualBasic.FileIO.Directory

Namespace FileSystem

    ''' <summary>
    ''' Physical file system combine with logical file mapping 
    ''' </summary>
    Public Class FileSystem

        ''' <summary>
        ''' the root filesystem environment (physical folder or virtual archive)
        ''' that this web file system serves files from.
        ''' </summary>
        Public ReadOnly Property wwwroot As IFileSystemEnvironment

        ''' <summary>
        ''' the registered virtual (cache/mapping) resources keyed by their trimmed url.
        ''' </summary>
        ReadOnly virtualMaps As New Dictionary(Of String, FileObject)

        ''' <summary>
        ''' create a new filesystem proxy for http web services, backed by the
        ''' given physical wwwroot folder.
        ''' </summary>
        ''' <param name="wwwroot">the physical root directory path to serve.</param>
        Sub New(wwwroot As String)
            Me.wwwroot = New FolderHandle(directory:=wwwroot)
        End Sub

        ''' <summary>
        ''' create a new filesystem proxy backed by a virtual filesystem
        ''' environment, attaching all of its contents in cache mode.
        ''' </summary>
        ''' <param name="virtual">the virtual filesystem environment to serve.</param>
        Sub New(virtual As IFileSystemEnvironment)
            Me.wwwroot = virtual
            Call AttachFolder(virtual).ToArray
        End Sub

        ''' <summary>
        ''' cache a physical file's bytes under the given resource url (suitable
        ''' for small files only).
        ''' </summary>
        ''' <param name="resourceUrl$">the url the resource is served at.</param>
        ''' <param name="file$">the path of the local file to cache.</param>
        ''' <param name="mime">the optional content type; auto-detected when omitted.</param>
        ''' <returns>the created <see cref="FileObject"/> cache entry.</returns>
        Public Function AddCache(resourceUrl$, file$, Optional mime As ContentType = Nothing) As FileObject
            Return AddCache(resourceUrl, file.ReadBinary, mime)
        End Function

        ''' <summary>
        ''' cache raw bytes in memory under the given resource url (suitable for
        ''' small files only).
        ''' </summary>
        ''' <param name="resourceUrl$">the url the resource is served at.</param>
        ''' <param name="data">the raw bytes to cache.</param>
        ''' <param name="mime">the optional content type; auto-detected when omitted.</param>
        ''' <returns>the created <see cref="MemoryCachedFile"/> entry.</returns>
        Public Function AddCache(resourceUrl$, data As Byte(), Optional mime As ContentType = Nothing) As FileObject
            Dim resource As New MemoryCachedFile(resourceUrl.FileName, data, mime)
            Dim key$ = resourceUrl.Trim("."c, "/"c, "\"c)

            ' add new cache resource or update current 
            ' existed resource
            virtualMaps(key) = resource

            Return resource
        End Function

        ''' <summary>
        ''' map a resource url onto a physical file path (mapping mode: content
        ''' is read on demand rather than cached in memory).
        ''' </summary>
        ''' <param name="resourceUrl$">the url the resource is served at.</param>
        ''' <param name="file$">the path of the local file to map.</param>
        ''' <param name="mime">the optional content type; auto-detected when omitted.</param>
        ''' <returns>the created <see cref="VirtualMappedFile"/> entry.</returns>
        Public Function AddMapping(resourceUrl$, file$, Optional mime As ContentType = Nothing) As FileObject
            Dim resource As New VirtualMappedFile(resourceUrl.FileName, file, mime)
            Dim key$ = resourceUrl.Trim("."c, "/"c, "\"c)

            ' add new cache resource or update current 
            ' existed resource
            virtualMaps(key) = resource

            Return resource
        End Function

        ''' <summary>
        ''' recursively attach every file in the given physical directory to the
        ''' virtual maps, either in cache or mapping mode.
        ''' </summary>
        ''' <param name="directory">the physical directory to scan recursively.</param>
        ''' <param name="attachTo">the url prefix the files are served under, defaults to "/".</param>
        ''' <param name="cacheMode">Work in cache mode or mapping mode?</param>
        ''' <returns>the enumerated file objects that were attached.</returns>
        Public Iterator Function AttachFolder(directory$,
                                              Optional attachTo$ = "/",
                                              Optional cacheMode As Boolean = False) As IEnumerable(Of NamedValue(Of FileObject))
            Dim resourceUrl$
            Dim fileObj As FileObject
            Dim type As ContentType

            For Each file As String In ls - l - r - "*.*" <= directory
                resourceUrl = attachTo & RelativePath(directory, file, appendParent:=False) _
                    .Trim("/"c, "\"c) _
                    .Replace("\", "/") _
                    .Split("/"c) _
                    .Where(Function(t) Not t.StringEmpty) _
                    .Skip(1) _
                    .JoinBy("/")
                type = Utils.FileMimeType(file)

                If cacheMode Then
                    fileObj = AddCache(resourceUrl, file, mime:=type)
                Else
                    fileObj = AddMapping(resourceUrl, file, mime:=type)
                End If

                Yield New NamedValue(Of FileObject) With {
                    .Name = resourceUrl,
                    .Description = file,
                    .Value = fileObj
                }
            Next
        End Function

        ''' <summary>
        ''' attach all contents of a virtual filesystem environment to the virtual
        ''' maps (always in cache mode, reading each file fully into memory).
        ''' </summary>
        ''' <param name="fs">the virtual filesystem environment to attach.</param>
        ''' <param name="attachTo">the url prefix the files are served under, defaults to "/".</param>
        ''' <returns>the enumerated file objects that were attached.</returns>
        ''' <remarks>
        ''' attach the contents from a archive file, always running in cache mode
        ''' </remarks>
        Public Iterator Function AttachFolder(fs As IFileSystemEnvironment, Optional attachTo As String = "/") As IEnumerable(Of NamedValue(Of FileObject))
            Dim resourceUrl$
            Dim fileObj As FileObject
            Dim s As Stream
            Dim buf As MemoryStream
            Dim type As ContentType

            For Each file As String In fs.GetFiles
                resourceUrl = attachTo & file _
                    .Trim("/"c, "\"c) _
                    .Replace("\", "/") _
                    .Split("/"c) _
                    .Where(Function(t) Not t.StringEmpty) _
                    .JoinBy("/")
                s = fs.OpenFile(file, FileMode.Open, FileAccess.Read)
                s.Seek(0, SeekOrigin.Begin)
                buf = New MemoryStream
                s.CopyTo(buf)
                buf.Flush()
                type = Utils.FileMimeType(file)
                fileObj = AddCache(resourceUrl, buf.ToArray, mime:=type)

                Yield New NamedValue(Of FileObject) With {
                    .Name = resourceUrl,
                    .Description = file,
                    .Value = fileObj
                }
            Next
        End Function

        Private Function resourceUrl(pathRelative As String) As String
            pathRelative = pathRelative.Trim("."c, "/"c, "\"c)
            pathRelative = wwwroot.GetFullPath(pathRelative)

            Return pathRelative
        End Function

        ''' <summary>
        ''' resolve a request path against the physical filesystem and the virtual maps.
        ''' returns the physical full path when it exists, otherwise Nothing; the
        ''' trimmed virtual map key is always returned so callers can probe virtualMaps.
        ''' </summary>
        Private Function resolveFile(pathRelative As String, ByRef virtualKey As String) As String
            Dim physical As String = resourceUrl(pathRelative)

            If physical.FileExists Then
                virtualKey = pathRelative.Trim("."c, "\"c, "/"c)
                Return physical
            Else
                virtualKey = pathRelative.Trim("."c, "\"c, "/"c)
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' resolve the content type (mime) of a resource, checking the physical
        ''' file first and then the virtual maps; falls back to
        ''' <c>application/javascript</c> for .js and <see cref="MIME.UnknownType"/> otherwise.
        ''' </summary>
        ''' <param name="pathRelative">the request relative path of the resource.</param>
        ''' <returns>the resolved <see cref="ContentType"/>.</returns>
        Public Function GetContentType(pathRelative As String) As ContentType
            Dim extName As String = "." & pathRelative.ExtensionSuffix.ToLower
            Dim virtualKey As String = Nothing
            Dim physical As String = resolveFile(pathRelative, virtualKey)

            ' test of the physical file at first
            If physical IsNot Nothing Then
                If MIME.SuffixTable.ContainsKey(extName) Then
                    Return MIME.SuffixTable(extName)
                ElseIf extName = ".js" Then
                    Return New ContentType("ECMAScript Module JavaScript", "application/javascript", ".js")
                Else
                    Return MIME.UnknownType
                End If
            ElseIf virtualMaps.ContainsKey(virtualKey) Then
                ' and then test for the logical file
                Return virtualMaps(virtualKey).mime
            ElseIf extName = ".js" Then
                ' 20260810 try to handling of the bug of the esmodule js file mime type
                Return New ContentType("ECMAScript Module JavaScript", "application/javascript", ".js")
            End If

            Return MIME.UnknownType
        End Function

        ''' <summary>
        ''' get the byte size of a resource, checking the physical file first and
        ''' then the virtual maps; returns -1 when the resource does not exist.
        ''' </summary>
        ''' <param name="pathRelative">the request relative path of the resource.</param>
        ''' <returns>the content length in bytes, or -1 when not found.</returns>
        Public Function GetFileSize(pathRelative As String) As Integer
            Dim virtualKey As String = Nothing
            Dim physical As String = resolveFile(pathRelative, virtualKey)

            ' test of the physical file at first
            If physical IsNot Nothing Then
                Return resourceUrl(pathRelative).FileLength
            ElseIf virtualMaps.ContainsKey(virtualKey) Then
                ' and then test for the logical file
                Return virtualMaps(virtualKey).ContentLength
            End If

            Return -1
        End Function

        ''' <summary>
        ''' open a readable stream for the resource, checking the physical file
        ''' first and then the virtual maps; returns an empty stream when missing.
        ''' </summary>
        ''' <param name="pathRelative">the request relative path of the resource.</param>
        ''' <returns>the resource stream.</returns>
        Public Function GetResource(pathRelative As String) As Stream
            Dim virtualKey As String = Nothing
            Dim physical As String = resolveFile(pathRelative, virtualKey)

            ' test of the physical file at first
            If physical IsNot Nothing Then
                Return physical.Open(FileMode.Open, doClear:=False)
            ElseIf virtualMaps.ContainsKey(virtualKey) Then
                ' and then test for the logical file
                Return virtualMaps(virtualKey).GetResource
            End If

            Return New MemoryStream(buffer:={})
        End Function

        ''' <summary>
        ''' get the full byte content of a resource, checking the physical file
        ''' first and then the virtual maps; returns an empty array when missing.
        ''' </summary>
        ''' <param name="pathRelative">the request relative path of the resource.</param>
        ''' <returns>the resource bytes.</returns>
        Public Function GetByteBuffer(pathRelative As String) As Byte()
            Dim virtualKey As String = Nothing
            Dim physical As String = resolveFile(pathRelative, virtualKey)

            ' test of the physical file at first
            If physical IsNot Nothing Then
                Return physical.ReadBinary
            ElseIf virtualMaps.ContainsKey(virtualKey) Then
                ' and then test for the logical file
                Return virtualMaps(virtualKey).GetByteBuffer
            End If

            Return {}
        End Function

        ''' <summary>
        ''' test whether a resource exists, checking the physical file first and
        ''' then the virtual maps (a <see cref="VirtualMappedFile"/> is only valid
        ''' when its underlying file still exists).
        ''' </summary>
        ''' <param name="pathRelative">the request relative path of the resource.</param>
        ''' <returns><c>True</c> when the resource exists.</returns>
        Public Function FileExists(pathRelative As String) As Boolean
            Dim virtualKey As String = Nothing
            Dim physical As String = resolveFile(pathRelative, virtualKey)

            ' test of the physical file at first
            If physical IsNot Nothing Then
                Return True
            ElseIf virtualMaps.ContainsKey(virtualKey) Then
                ' and then test for the logical file
                If TypeOf virtualMaps(virtualKey) Is VirtualMappedFile Then
                    Return DirectCast(virtualMaps(virtualKey), VirtualMappedFile).isValid
                Else
                    Return True
                End If
            End If

            Return False
        End Function

        ''' <summary>
        ''' the string representation of this file system: its wwwroot environment.
        ''' </summary>
        ''' <returns>the wwwroot environment description.</returns>
        Public Overrides Function ToString() As String
            Return wwwroot.ToString
        End Function
    End Class
End Namespace
