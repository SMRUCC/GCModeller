#Region "Microsoft.VisualBasic::b322f5814bac473e0063b9a33407761a, src\Flute\FileSystem\FileObject.vb"

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

    '   Total Lines: 91
    '    Code Lines: 63 (69.23%)
    ' Comment Lines: 5 (5.49%)
    '    - Xml Docs: 80.00%
    ' 
    '   Blank Lines: 23 (25.27%)
    '     File Size: 2.71 KB


    '     Class FileObject
    ' 
    '         Properties: fileName, mime
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    '     Class MemoryCachedFile
    ' 
    '         Properties: ContentLength
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetByteBuffer, GetResource
    ' 
    '     Class VirtualMappedFile
    ' 
    '         Properties: ContentLength, isValid, mappedPath
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetByteBuffer, GetResource
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes

Namespace FileSystem

    Public MustInherit Class FileObject

        ''' <summary>
        ''' 文件的类型
        ''' </summary>
        ''' <returns>the content (mime) type of the file.</returns>
        Public ReadOnly Property mime As ContentType
        ''' <summary>
        ''' the file name (without path) that this object represents.
        ''' </summary>
        Public ReadOnly Property fileName As String

        ''' <summary>
        ''' the byte length of the file content.
        ''' </summary>
        ''' <returns>the content length in bytes.</returns>
        Public MustOverride ReadOnly Property ContentLength As Long

        ''' <summary>
        ''' create a file object with the given file name and an optional content
        ''' type, auto-detecting the mime type from the file name when omitted.
        ''' </summary>
        ''' <param name="fileName$">the file name of the resource.</param>
        ''' <param name="mime">the optional content type; auto-detected when empty.</param>
        Sub New(fileName$, Optional mime As ContentType = Nothing)
            Me.fileName = fileName
            Me.mime = mime

            If mime Is Nothing OrElse mime.IsEmpty Then
                Me.mime = fileName.FileMimeType
            End If
        End Sub

        ''' <summary>
        ''' open a readable stream over the file content.
        ''' </summary>
        ''' <returns>the resource stream.</returns>
        Public MustOverride Function GetResource() As Stream
        ''' <summary>
        ''' get the full byte content of the file.
        ''' </summary>
        ''' <returns>the file bytes.</returns>
        Public MustOverride Function GetByteBuffer() As Byte()

        ''' <summary>
        ''' the string representation of this file object: its file name.
        ''' </summary>
        ''' <returns>the file name.</returns>
        Public Overrides Function ToString() As String
            Return fileName
        End Function
    End Class

End Namespace
