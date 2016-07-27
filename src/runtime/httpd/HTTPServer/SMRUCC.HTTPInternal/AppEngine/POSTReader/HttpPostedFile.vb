#Region "Microsoft.VisualBasic::27659a4c045a167c2f0e19653d88fa31, ..\httpd\HTTPServer\SMRUCC.HTTPInternal\AppEngine\POSTReader\HttpPostedFile.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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
Imports System.Security.Permissions

Namespace AppEngine.POSTParser

    Public Class HttpPostedFile
        Private name As String
        Private content_type As String
        Private stream As Stream

        Public Sub New(name As String, content_type As String, base_stream As Stream, offset As Long, length As Long)
            Me.name = name
            Me.content_type = content_type
            Me.stream = New ReadSubStream(base_stream, offset, length)
        End Sub

        Public ReadOnly Property ContentType() As String
            Get
                Return (content_type)
            End Get
        End Property

        Public ReadOnly Property ContentLength() As Integer
            Get
                Return CInt(stream.Length)
            End Get
        End Property

        Public ReadOnly Property FileName() As String
            Get
                Return (name)
            End Get
        End Property

        Public ReadOnly Property InputStream() As Stream
            Get
                Return (stream)
            End Get
        End Property

        Public Sub SaveAs(filename As String)
            Dim buffer As Byte() = New Byte(16 * 1024 - 1) {}
            Dim old_post As Long = stream.Position

            Try
                File.Delete(filename)
                Using fs As FileStream = File.Create(filename)
                    stream.Position = 0
                    Dim n As Integer

                    While stream.Read(buffer, 0, 16 * 1024).ShadowCopy(n) <> 0
                        fs.Write(buffer, 0, n)
                    End While
                End Using
            Finally
                stream.Position = old_post
            End Try
        End Sub
    End Class
End Namespace
