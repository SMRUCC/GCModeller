#Region "Microsoft.VisualBasic::28c92ba0c67491288f1ca4a81bf18fa0, src\Flute\Http\HttpStream\PostReader.vb"

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

    '   Total Lines: 211
    '    Code Lines: 140 (66.35%)
    ' Comment Lines: 41 (19.43%)
    '    - Xml Docs: 70.73%
    ' 
    '   Blank Lines: 30 (14.22%)
    '     File Size: 8.44 KB


    '     Class PostReader
    ' 
    '         Properties: ContentEncoding, ContentType, files, Form, InputStream
    '                     Objects
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetParameter, GetSubStream
    '         Delegate Function
    ' 
    '             Sub: loadjQueryPOST, loadMultiPart, LoadMultiPart
    '         Class ContentOutput
    ' 
    ' 
    ' 
    ' 
    ' 
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
        ''' Get a form field value by name from <see cref="Form"/> using the default indexer.
        ''' </summary>
        ''' <param name="name">the form field name to look up.</param>
        ''' <returns>the form field value, or <c>Nothing</c> when not present.</returns>
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

        ''' <summary>
        ''' the content type (mime) of the posted request body.
        ''' </summary>
        Public ReadOnly Property ContentType As String
        ''' <summary>
        ''' 所POST上传的数据的临时文件的文件路径
        ''' </summary>
        ''' <returns>the file path of the temporary file holding the posted data.</returns>
        Public ReadOnly Property InputStream As String
        ''' <summary>
        ''' the text encoding used to decode the posted byte stream into strings.
        ''' </summary>
        Public ReadOnly Property ContentEncoding As Encoding
        ''' <summary>
        ''' The web form input values (name/value pairs) parsed from the body.
        ''' </summary>
        ''' <returns>the collection of posted form fields.</returns>
        Public ReadOnly Property Form As New NameValueCollection
        ''' <summary>
        ''' the parsed json objects when the posted body is a json payload.
        ''' </summary>
        Public ReadOnly Property Objects As New Dictionary(Of String, Object)
        ''' <summary>
        ''' the uploaded files keyed by their form field name.
        ''' </summary>
        Public ReadOnly Property files As New Dictionary(Of String, List(Of HttpPostedFile))

        ''' <summary>
        ''' create a POST reader over the given temporary input file and parse it
        ''' according to its content type (json, url-encoded form, or multipart).
        ''' </summary>
        ''' <param name="input">the path of the temporary file that holds the posted data.</param>
        ''' <param name="contentType$">the content type (mime) of the posted body.</param>
        ''' <param name="encoding">the text encoding of the posted data.</param>
        ''' <param name="fileName$">the original file name, used when the body is a single file upload.</param>
        ''' <param name="parseJSON">the optional custom json parser for json payloads.</param>
        Sub New(input$, contentType$, encoding As Encoding,
                Optional fileName$ = Nothing,
                Optional parseJSON As JSONParser = Nothing)

            Me.InputStream = input
            Me.ContentType = If(contentType.StringEmpty, "application/octet-stream", contentType)
            Me.ContentEncoding = encoding
            Me.Objects = New Dictionary(Of String, Object)

            If input.FileLength > 0 Then
                Call LoadMultiPart(fileName, parseJSON)
            End If
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

        ''' <summary>
        ''' a custom parser delegate that deserializes a json request body into a
        ''' name/value dictionary, used instead of the built-in json loader.
        ''' </summary>
        ''' <param name="json_str">the raw json request body string.</param>
        ''' <returns>the parsed object dictionary.</returns>
        Public Delegate Function JSONParser(json_str As String) As Dictionary(Of String, Object)

        Private Sub loadjQueryPOST(fileName As String, parseJSON As JSONParser)
            Using inputStream As FileStream = Me.InputStream.Open(doClear:=False)
                ' 在这里可能存在两种情况：
                ' 一种是jquery POST
                ' 另外的一种就是只有单独的一个文件的POST上传，
                ' 现在我们假设jquery POST的长度很小， 而文件上传的长度很大，则在这里目前就只通过stream的长度来进行分别处理
                If ContentType = "application/json" OrElse ContentType.ToLower.StartsWith("application/json") Then
                    Dim json = New StreamReader(inputStream).ReadToEnd
                    Dim knows As Type() = {
                        GetType(Dictionary(Of String, Object)),
                        GetType(String()),
                        GetType(Double()),
                        GetType(Double),
                        GetType(String),
                        GetType(Dictionary(Of String, String)),
                        GetType(Dictionary(Of String, String()))
                    }

                    If parseJSON Is Nothing Then
                        _Objects = json.LoadJSON(Of Dictionary(Of String, Object))(knownTypes:=knows)
                    Else
                        _Objects = parseJSON(json)
                    End If
                ElseIf ContentType = "application/x-www-form-urlencoded" Then
                    ' probably is a jquery post
                    Dim byts As Byte() = inputStream _
                        .PopulateBlocks _
                        .IteratesALL _
                        .ToArray
                    Dim s As String = ContentEncoding.GetString(byts)

                    _Form = s.PostUrlDataParser(toLower:=False)
                Else
                    ' 是一个单独的文件
                    Dim [sub] As New HttpPostedFile(
                        fileName,
                        ContentType,
                        inputStream,
                        Scan0,
                        inputStream.Length
                    )

                    files("file") = New List(Of HttpPostedFile) From {[sub]}
                End If
            End Using
        End Sub

        ''' <summary>
        ''' Loads the data on the form for multipart/form-data
        ''' </summary>
        Private Sub LoadMultiPart(fileName As String, parseJSON As JSONParser)
            Dim boundary As String = GetParameter(ContentType, "; boundary=")

            If boundary Is Nothing Then
                Call loadjQueryPOST(fileName, parseJSON)
            Else
                Using input As Stream = Me.GetSubStream()
                    Call loadMultiPart(boundary, input, New ContentOutput With {
                         .files = files,
                         .form = Form
                    }, ContentEncoding)
                End Using
            End If
        End Sub

        Public Shared Sub loadMultiPart(boundary$, input As Stream, load As ContentOutput, Optional contentEncoding As Encoding = Nothing)
            Dim multi_part As New HttpMultipart(input, boundary, contentEncoding)
            Dim read As New Value(Of StreamElement)
            Dim str As String

            While (read = multi_part.ReadNextElement()) IsNot Nothing
                Dim data As StreamElement = +read

                If data.Filename Is Nothing Then
                    Dim copy As Byte() = New Byte(data.Length - 1) {}

                    input.Position = data.Start
#Disable Warning CA2022 ' Avoid inexact read with 'Stream.Read'
                    input.Read(copy, 0, CInt(data.Length))
#Enable Warning CA2022 ' Avoid inexact read with 'Stream.Read'

                    str = contentEncoding.GetString(copy)
                    load.form.Add(data.Name, str)
                Else
                    '
                    ' We use a substream, as in 2.x we will support
                    ' large uploads streamed to disk,
                    '
                    Dim [sub] As New HttpPostedFile(
                        data.Filename,
                        data.ContentType,
                        input,
                        data.Start,
                        data.Length)

                    If Not load.files.ContainsKey(data.Name) Then
                        load.files.Add(data.Name, New List(Of HttpPostedFile))
                    End If

                    load.files(data.Name) += [sub]
                End If
            End While
        End Sub

        Public Class ContentOutput

            Public form As NameValueCollection
            Public files As Dictionary(Of String, List(Of HttpPostedFile))

        End Class
    End Class
End Namespace
