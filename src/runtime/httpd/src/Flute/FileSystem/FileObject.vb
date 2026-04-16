#Region "Microsoft.VisualBasic::b322f5814bac473e0063b9a33407761a, G:/GCModeller/src/runtime/httpd/src/Flute//FileSystem/FileObject.vb"

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
    '    Code Lines: 63
    ' Comment Lines: 5
    '   Blank Lines: 23
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
        ''' <returns></returns>
        Public ReadOnly Property mime As ContentType
        Public ReadOnly Property fileName As String

        Public MustOverride ReadOnly Property ContentLength As Long

        Sub New(fileName$, Optional mime As ContentType = Nothing)
            Me.fileName = fileName
            Me.mime = mime

            If mime Is Nothing OrElse mime.IsEmpty Then
                Me.mime = fileName.FileMimeType
            End If
        End Sub

        Public MustOverride Function GetResource() As Stream
        Public MustOverride Function GetByteBuffer() As Byte()

        Public Overrides Function ToString() As String
            Return fileName
        End Function
    End Class

    Public Class MemoryCachedFile : Inherits FileObject

        ReadOnly cache As MemoryStream

        Public Overrides ReadOnly Property ContentLength As Long
            Get
                Return cache.Length
            End Get
        End Property

        Sub New(fileName$, data As Byte(), Optional mime As ContentType = Nothing)
            Call MyBase.New(fileName, mime)

            ' create cache data stream
            Me.cache = New MemoryStream(data)
        End Sub

        Public Overrides Function GetResource() As Stream
            Return cache
        End Function

        Public Overrides Function GetByteBuffer() As Byte()
            Return cache.ToArray
        End Function
    End Class

    Public Class VirtualMappedFile : Inherits FileObject

        Public ReadOnly Property mappedPath As String

        Public ReadOnly Property isValid As Boolean
            Get
                Return mappedPath.FileExists
            End Get
        End Property

        Public Overrides ReadOnly Property ContentLength As Long
            Get
                Return mappedPath.FileLength
            End Get
        End Property

        Sub New(fileName$, mappedPath$, Optional mime As ContentType = Nothing)
            Call MyBase.New(fileName, mime)

            Me.mappedPath = mappedPath
        End Sub

        Public Overrides Function GetResource() As Stream
            Return mappedPath.Open(FileMode.Open, doClear:=False)
        End Function

        Public Overrides Function GetByteBuffer() As Byte()
            Return mappedPath.ReadBinary
        End Function
    End Class
End Namespace
