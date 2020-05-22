#Region "Microsoft.VisualBasic::ae883b54063cf84ffc537fe9861fd14c, WebCloud\SMRUCC.HTTPInternal\Core\HttpRequest\POSTReader\PostReader.vb"

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

    '     Class PostReader
    ' 
    '         Properties: ContentEncoding, ContentType, Files, Form, InputStream
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetParameter, GetSubStream
    ' 
    '         Sub: loadjQueryPOST, loadMultiPart, LoadMultiPart
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Specialized
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Core.HttpStream

    ''' <summary>
    ''' POST参数的解析工具
    ''' </summary>
    Public Class PostReader

        ''' <summary>
        ''' Get value from <see cref="Form"/>
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Default Public ReadOnly Property param(name As String) As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Form(name)
            End Get
        End Property

        Private Shared Function GetParameter(header As String, attr As String) As String
            Dim ap As Integer = header.IndexOf(attr)
            If ap = -1 Then
                Return Nothing
            End If

            ap += attr.Length
            If ap >= header.Length Then
                Return Nothing
            End If

            Dim ending As Char = header(ap)
            If ending <> """"c Then
                ending = " "c
            End If

            Dim [end] As Integer = header.IndexOf(ending, ap + 1)
            If [end] = -1 Then
                Return If((ending = """"c), Nothing, header.Substring(ap))
            End If

            Return header.Substring(ap + 1, [end] - ap - 1)
        End Function

        Public ReadOnly Property ContentType As String
        ''' <summary>
        ''' 所POST上传的数据的临时文件的文件路径
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property InputStream As String
        Public ReadOnly Property ContentEncoding As Encoding
        ''' <summary>
        ''' The web form input values
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Form As New NameValueCollection
        Public ReadOnly Property Objects As Dictionary(Of String, Object)
        Public ReadOnly Property files As New Dictionary(Of String, List(Of HttpPostedFile))

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="input">所POST上传的数据是保存在一个临时文件之中的</param>
        ''' <param name="contentType$"></param>
        ''' <param name="encoding"></param>
        ''' <param name="fileName$"></param>
        Sub New(input$, contentType$, encoding As Encoding, Optional fileName$ = Nothing)
            Me.InputStream = input
            Me.ContentType = contentType
            Me.ContentEncoding = encoding

            Call LoadMultiPart(fileName)
        End Sub

        ''' <summary>
        ''' GetSubStream returns a 'copy' of the InputStream with Position set to 0.
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function GetSubStream() As Stream
            Return InputStream.Open(doClear:=False)
        End Function

        Private Sub loadjQueryPOST(fileName As String)
            Using inputStream As FileStream = Me.InputStream.Open(doClear:=False)
                ' 在这里可能存在两种情况：
                ' 一种是jquery POST
                ' 另外的一种就是只有单独的一个文件的POST上传，
                ' 现在我们假设jquery POST的长度很小， 而文件上传的长度很大，则在这里目前就只通过stream的长度来进行分别处理
                If ContentType = "application/json" Then
                    Dim json = New StreamReader(inputStream).ReadToEnd
                    Dim knows As Type() = {
                        GetType(Dictionary(Of String, Object)),
                        GetType(String()),
                        GetType(Double()),
                        GetType(Double),
                        GetType(String)
                    }

                    _Objects = json.LoadJSON(Of Dictionary(Of String, Object))(knownTypes:=knows)
                Else
                    If inputStream.Length >= 2048 Then
                        ' 是一个单独的文件
                        Dim [sub] As New HttpPostedFile(
                            fileName,
                            ContentType,
                            inputStream,
                            Scan0,
                            inputStream.Length
                        )

                        files("file") = New List(Of HttpPostedFile) From {[sub]}
                    Else
                        ' probably is a jquery post
                        Dim byts As Byte() = inputStream _
                            .PopulateBlocks _
                            .IteratesALL _
                            .ToArray
                        Dim s As String = ContentEncoding.GetString(byts)

                        _Form = s.PostUrlDataParser
                    End If
                End If
            End Using
        End Sub

        ''' <summary>
        ''' Loads the data on the form for multipart/form-data
        ''' </summary>
        Private Sub LoadMultiPart(fileName As String)
            Dim boundary As String = GetParameter(ContentType, "; boundary=")

            If boundary Is Nothing Then
                Call loadjQueryPOST(fileName)
            Else
                Using input As Stream = Me.GetSubStream()
                    Call loadMultiPart(boundary, input)
                End Using
            End If
        End Sub

        Private Sub loadMultiPart(boundary$, input As Stream)
            Dim multi_part As New HttpMultipart(input, boundary, ContentEncoding)
            Dim read As New Value(Of StreamElement)
            Dim str As String

            While (read = multi_part.ReadNextElement()) IsNot Nothing
                Dim data As StreamElement = +read

                If data.Filename Is Nothing Then
                    Dim copy As Byte() = New Byte(data.Length - 1) {}

                    input.Position = data.Start
                    input.Read(copy, 0, CInt(data.Length))

                    str = ContentEncoding.GetString(copy)
                    Call Form.Add(data.Name, str)
                Else
                    '
                    ' We use a substream, as in 2.x we will support large uploads streamed to disk,
                    '
                    Dim [sub] As New HttpPostedFile(
                        data.Filename,
                        data.ContentType,
                        input,
                        data.Start,
                        data.Length)

                    If Not files.ContainsKey(data.Name) Then
                        files.Add(data.Name, New List(Of HttpPostedFile))
                    End If

                    files(data.Name) += [sub]
                End If
            End While
        End Sub
    End Class
End Namespace
