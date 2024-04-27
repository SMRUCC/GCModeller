#Region "Microsoft.VisualBasic::9207ec4fe3def594b61c45c1fda7ee41, G:/GCModeller/src/runtime/httpd/src/Flute//FileSystem/FileSystem.vb"

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

    '   Total Lines: 243
    '    Code Lines: 156
    ' Comment Lines: 49
    '   Blank Lines: 38
    '     File Size: 9.39 KB


    '     Class FileSystem
    ' 
    '         Properties: wwwroot
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) AddCache, AddMapping, (+2 Overloads) AttachFolder, FileExists, GetByteBuffer
    '                   GetContentType, GetFileSize, GetResource, resourceUrl, ToString
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

        Public ReadOnly Property wwwroot As FolderHandle

        ReadOnly virtualMaps As New Dictionary(Of String, FileObject)

        ''' <summary>
        ''' Create a new filesystem proxy for http web services
        ''' </summary>
        ''' <param name="wwwroot"></param>
        Sub New(wwwroot As String)
            Me.wwwroot = New FolderHandle(directory:=wwwroot)
        End Sub

        ''' <summary>
        ''' 这个函数只适用于小文件的缓存
        ''' </summary>
        ''' <param name="resourceUrl$"></param>
        ''' <param name="file$"></param>
        ''' <param name="mime"></param>
        ''' <returns></returns>
        Public Function AddCache(resourceUrl$, file$, Optional mime As ContentType = Nothing) As FileObject
            Return AddCache(resourceUrl, file.ReadBinary, mime)
        End Function

        Public Function AddCache(resourceUrl$, data As Byte(), Optional mime As ContentType = Nothing) As FileObject
            Dim resource As New MemoryCachedFile(resourceUrl.FileName, data, mime)
            Dim key$ = resourceUrl.Trim("."c, "/"c, "\"c)

            ' add new cache resource or update current 
            ' existed resource
            virtualMaps(key) = resource

            Return resource
        End Function

        Public Function AddMapping(resourceUrl$, file$, Optional mime As ContentType = Nothing) As FileObject
            Dim resource As New VirtualMappedFile(resourceUrl.FileName, file, mime)
            Dim key$ = resourceUrl.Trim("."c, "/"c, "\"c)

            ' add new cache resource or update current 
            ' existed resource
            virtualMaps(key) = resource

            Return resource
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="directory"></param>
        ''' <param name="attachTo"></param>
        ''' <param name="cacheMode">Work in cache mode or mapping mode?</param>
        ''' <returns></returns>
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
        ''' 
        ''' </summary>
        ''' <param name="fs"></param>
        ''' <param name="attachTo"></param>
        ''' <returns></returns>
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

        Public Function GetContentType(pathRelative As String) As ContentType
            ' test of the physical file at first
            If resourceUrl(pathRelative).FileExists Then
                Dim extName As String = "." & pathRelative.ExtensionSuffix.ToLower

                If MIME.SuffixTable.ContainsKey(extName) Then
                    Return MIME.SuffixTable(extName)
                Else
                    Return MIME.UnknownType
                End If
            Else
                pathRelative = pathRelative.Trim("."c, "\"c, "/"c)

                ' and then test for the logical file
                If virtualMaps.ContainsKey(pathRelative) Then
                    Return virtualMaps(pathRelative).mime
                End If
            End If

            Return MIME.UnknownType
        End Function

        Public Function GetFileSize(pathRelative As String) As Integer
            ' test of the physical file at first
            If resourceUrl(pathRelative).FileExists Then
                Return resourceUrl(pathRelative).FileLength
            Else
                pathRelative = pathRelative.Trim("."c, "\"c, "/"c)

                ' and then test for the logical file
                If virtualMaps.ContainsKey(pathRelative) Then
                    Return virtualMaps(pathRelative).ContentLength
                End If
            End If

            Return -1
        End Function

        Public Function GetResource(pathRelative As String) As Stream
            ' test of the physical file at first
            If resourceUrl(pathRelative).FileExists Then
                Return resourceUrl(pathRelative).Open(FileMode.Open, doClear:=False)
            Else
                pathRelative = pathRelative.Trim("."c, "\"c, "/"c)

                ' and then test for the logical file
                If virtualMaps.ContainsKey(pathRelative) Then
                    Return virtualMaps(pathRelative).GetResource
                End If
            End If

            Return New MemoryStream(buffer:={})
        End Function

        ''' <summary>
        ''' get file data
        ''' </summary>
        ''' <param name="pathRelative"></param>
        ''' <returns></returns>
        Public Function GetByteBuffer(pathRelative As String) As Byte()
            ' test of the physical file at first
            If resourceUrl(pathRelative).FileExists Then
                Return resourceUrl(pathRelative).ReadBinary
            Else
                pathRelative = pathRelative.Trim("."c, "\"c, "/"c)

                ' and then test for the logical file
                If virtualMaps.ContainsKey(pathRelative) Then
                    Return virtualMaps(pathRelative).GetByteBuffer
                End If
            End If

            Return {}
        End Function

        Public Function FileExists(pathRelative As String) As Boolean
            ' test of the physical file at first
            If resourceUrl(pathRelative).FileExists Then
                Return True
            Else
                pathRelative = pathRelative.Trim("."c, "/"c, "\"c)

                ' and then test for the logical file
                If virtualMaps.ContainsKey(pathRelative) Then
                    If TypeOf virtualMaps(pathRelative) Is VirtualMappedFile Then
                        Return DirectCast(virtualMaps(pathRelative), VirtualMappedFile).isValid
                    Else
                        Return True
                    End If
                End If
            End If

            Return False
        End Function

        Public Overrides Function ToString() As String
            Return wwwroot.ToString
        End Function
    End Class
End Namespace
