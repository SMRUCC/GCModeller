﻿#Region "Microsoft.VisualBasic::d24e4707c70feb5b94ee38c2094eb427, WebCloud\SMRUCC.HTTPInternal\AppEngine\POSTReader\HttpPostedFile.vb"

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

    '     Class HttpPostedFile
    ' 
    '         Properties: ContentLength, ContentType, FileName, TempPath
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: Summary
    ' 
    '         Sub: ensureTargetNotExists, (+2 Overloads) SaveAs
    ' 
    ' 
    ' /********************************************************************************/

#End Region

'
' System.Web.HttpPostedFile.cs
'
' Author:
'	Dick Porter  <dick@ximian.com>
'      Ben Maurer   <benm@ximian.com>
'      Miguel de Icaza <miguel@novell.com>
'
' Copyright (C) 2005 Novell, Inc (http://www.novell.com)
'
' Permission is hereby granted, free of charge, to any person obtaining
' a copy of this software and associated documentation files (the
' "Software"), to deal in the Software without restriction, including
' without limitation the rights to use, copy, modify, merge, publish,
' distribute, sublicense, and/or sell copies of the Software, and to
' permit persons to whom the Software is furnished to do so, subject to
' the following conditions:
' 
' The above copyright notice and this permission notice shall be
' included in all copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
' EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
' MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
' NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
' LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
' OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
' WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
'

Imports System.IO
Imports Microsoft.VisualBasic.Language

Namespace AppEngine.POSTParser

    Public Class HttpPostedFile

        Public ReadOnly Property FileName() As String
        ''' <summary>
        ''' 为了降低内存的使用率,在这里是将文件保存在临时文件之中的
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property TempPath As String
        Public ReadOnly Property ContentType As String
        Public ReadOnly Property ContentLength As Integer
            Get
                Return TempPath.FileLength
            End Get
        End Property

        Public Sub New(name As String, content_type As String, base_stream As Stream, offset As Long, length As Long)
            Me.FileName = name
            Me.ContentType = content_type
            Me.TempPath = App.GetAppSysTempFile(, App.PID)

            ' 数据写入到临时文件之中
            Call SaveAs(TempPath, New ReadSubStream(base_stream, offset, length))
        End Sub

        Public Function Summary() As Dictionary(Of String, String)
            Return New Dictionary(Of String, String) From {
                {NameOf(FileName), FileName},
                {NameOf(ContentType), ContentType},
                {NameOf(ContentLength), ContentLength}
            }
        End Function

        Public Sub SaveAs(filename As String)
            Call ensureTargetNotExists(filename)
            Call TempPath.FileCopy(filename)
        End Sub

        Private Shared Sub ensureTargetNotExists(filename As String)
            Try
                If filename.FileExists Then
                    Call File.Delete(filename)
                End If
            Catch ex As Exception
            Finally
                ' 当目标文件不存在的时候，可能文件夹也是不存在的
                ' 需要提前创建好文件夹，否则后面的文件保存会出错
                Call filename.ParentPath.MkDIR
            End Try
        End Sub

        ''' <summary>
        ''' 将用户上传的文件保存到指定的目标文件<paramref name="filename"/>之中
        ''' </summary>
        ''' <param name="filename"></param>
        Private Shared Sub SaveAs(filename As String, inputStream As Stream)
            Dim buffer As Byte() = New Byte(16 * 1024 - 1) {}
            Dim old_post As Long = inputStream.Position

            Call ensureTargetNotExists(filename)

            Try
                Using fs As FileStream = File.Create(filename)
                    Dim n As VBInteger = Scan0

                    inputStream.Position = 0

                    While (n = inputStream.Read(buffer, 0, 16 * 1024)) <> 0
                        fs.Write(buffer, 0, n.Value)
                    End While
                End Using
            Finally
                inputStream.Position = old_post
            End Try
        End Sub
    End Class
End Namespace
