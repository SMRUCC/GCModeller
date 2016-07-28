#Region "Microsoft.VisualBasic::d7c3b91c9197b9e0d4a382c3daafc040, ..\httpd\HTTPServer\SMRUCC.HTTPInternal\Core\HttpFileSystem.vb"

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

Imports System.IO
Imports System.Net.Sockets
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace Core

    ''' <summary>
    ''' 不兼容IE和Edge浏览器???为什么会这样子？
    ''' </summary>
#If API_EXPORT Then
    <PackageNamespace("HTTP.HOME", Category:=APICategories.UtilityTools)>
    Public Class HttpFileSystem : Inherits HttpServer
#Else
    Public Class HttpFileSystem : Inherits HttpServer
#End If
        Public ReadOnly Property HOME As DirectoryInfo
        ''' <summary>
        ''' {url, mapping path}
        ''' </summary>
        ReadOnly _virtualMappings As Dictionary(Of String, String)
        ReadOnly _nullExists As Boolean

        Public Function AddMappings(DIR As String, url As String) As Boolean
            url = url & "/index.html"
            url = FileIO.FileSystem.GetParentPath(url).ToLower
            If _virtualMappings.ContainsKey(url) Then
                Call _virtualMappings.Remove(url)
                Call $"".__DEBUG_ECHO
            End If
            Call _virtualMappings.Add(url.Replace("\", "/"), DIR)

            Return True
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="port"></param>
        ''' <param name="root"></param>
        ''' <param name="nullExists"></param>
        Sub New(port As Integer, root As String, Optional nullExists As Boolean = False, Optional requestResource As IGetResource = Nothing)
            Call MyBase.New(port, True)
            _nullExists = nullExists
            If Not FileIO.FileSystem.DirectoryExists(root) Then
                Call FileIO.FileSystem.CreateDirectory(root)
            End If
            HOME = FileIO.FileSystem.GetDirectoryInfo(root)
            FileIO.FileSystem.CurrentDirectory = root
            _virtualMappings = New Dictionary(Of String, String)

            If requestResource Is Nothing Then
                RequestStream = AddressOf GetResource
            Else
                RequestStream = requestResource
            End If
        End Sub

        Public Function GetResource(ByRef res As String) As Byte()
            Dim mapDIR As String = __getMapDIR(res)
            Dim file As String = $"{mapDIR}/{res}"

            If Not FileExists(file) Then
                ' 检查是不是文件夹
                If file.DirectoryExists Then
                    Dim index As String = file & "/index.html"
                    If Not index.FileExists Then
                        GoTo NoData
                    Else
                        file = index
                        res = file
                    End If
                Else
NoData:             If _nullExists Then
                        Call $"[ERR_EMPTY_RESPONSE::No data send] {file.ToFileURL}".__DEBUG_ECHO
                        Return New Byte() {}
                    End If
                End If
            End If
            Return IO.File.ReadAllBytes(file)
        End Function

        ''' <summary>
        ''' 默认是使用<see cref="GetResource"/>获取得到文件数据
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property RequestStream As IGetResource

        Public Delegate Function IGetResource(ByRef res As String) As Byte()

        ''' <summary>
        ''' 长
        ''' </summary>
        ''' <param name="res"></param>
        ''' <returns></returns>
        Private Function __getMapDIR(ByRef res As String) As String
            Dim mapDIR As String = FileIO.FileSystem.GetParentPath(res).ToLower.Replace("\", "/")
            If _virtualMappings.ContainsKey(mapDIR) Then
                res = Regex.Replace(res.Replace("\", "/"), mapDIR.Replace("\", "/"), "")
                mapDIR = _virtualMappings(mapDIR)
            Else
                For Each map In _virtualMappings  ' 短
                    If InStr(res, map.Key, CompareMethod.Text) = 1 Then
                        ' 找到了
                        res = Regex.Replace(res.Replace("\", "/"), map.Key.Replace("\", "/"), "")
                        mapDIR = map.Value
                        Return mapDIR
                    End If
                Next
                mapDIR = HOME.FullName
            End If
            Return mapDIR
        End Function

        <ExportAPI("Open")>
        Public Shared Function Open(home As String, Optional port As Integer = 80) As HttpFileSystem
            Return New HttpFileSystem(port, home, True)
        End Function

        <ExportAPI("Run")>
        Public Shared Sub RunServer(server As HttpServer)
            Call server.Run()
        End Sub

        ''' <summary>
        ''' 为什么不需要添加<see cref="HttpProcessor.writeSuccess(String)"/>方法？？？
        ''' </summary>
        ''' <param name="p"></param>
        Public Overrides Sub handleGETRequest(p As HttpProcessor)
            Dim res As String = p.http_url

            If String.Equals(res, "/") Then   ' 在这里首先会检查是否是以/符号结束的，假若是，则可能是暗指该文件夹之下的index.html主页文件
                res = "index.html"
            End If

            ' The file content is null or not exists, that possible means this is a GET REST request not a Get file request.
            ' This if statement makes the file GET request compatible with the REST API
            If res.PathIllegal Then
                Call __handleREST(p)
            Else
                If res.Last = "\"c OrElse res.Last = "/"c Then
                    res = res & "index.html"
                End If
                Call __handleFileGET(res, p)
            End If
        End Sub

        Private Sub __handleFileGET(res As String, p As HttpProcessor)
            Dim buf As Byte() = RequestStream(res) ' 由于子文件夹可能会是以/的方式请求index.html，所以在这里res的值可能会变化，文件拓展名放在变化之后再解析
            Dim ext As String = FileIO.FileSystem.GetFileInfo(res).Extension.ToLower

            If String.Equals(ext, ".html") Then ' Transfer HTML document.
                Dim html As String = Encoding.UTF8.GetString(buf)

                If String.IsNullOrEmpty(html) Then
                    html = __request404()
                    html = html.Replace("%EXCEPTION%", res)
                End If

                Call p.writeSuccess()
                Call p.outputStream.WriteLine(html)
            Else
                Call __transferData(p, ext, buf)
            End If
        End Sub

        ''' <summary>
        ''' handle the GET/POST request at here
        ''' </summary>
        ''' <param name="p"></param>
        Protected Overridable Sub __handleREST(p As HttpProcessor)
            Dim pos As Integer = InStr(p.http_url, "?")
            If pos <= 0 Then
                Call p.writeFailure($"{p.http_url} have no parameter!")
                Return
            End If

            ' Gets the argument value, value is after the API name from the ? character
            ' Actually the Reflection operations method can be used at here to calling 
            ' the different API 
            Dim args As String = Mid(p.http_url, pos + 1)
            Dim Tokens = args.requestParser
            Dim query As String = Tokens("query")
            Dim subject As String = Tokens("subject")
            Dim result = LevenshteinDistance.ComputeDistance(query, subject)

            ' write API compute result to the browser
            Call p.writeSuccess()
            Call p.outputStream.WriteLine(result.Visualize)
        End Sub

        ''' <summary>
        ''' 为什么不需要添加content-type说明？？
        ''' </summary>
        ''' <param name="p"></param>
        ''' <param name="ext"></param>
        ''' <param name="buf"></param>
        Private Sub __transferData(p As HttpProcessor, ext As String, buf As Byte())
            If Not ContentTypes.ExtDict.ContainsKey(ext) Then
                ext = ".txt"
            End If

            Dim contentType = ContentTypes.ExtDict(ext)

            ' Call p.writeSuccess(contentType.MIMEType)
            Call p.outputStream.BaseStream.Write(buf, Scan0, buf.Length)
            Call $"Transfer data:  {contentType.ToString} ==> [{buf.Length} Bytes]!".__DEBUG_ECHO
        End Sub

        Public Overrides Sub handlePOSTRequest(p As HttpProcessor, inputData As MemoryStream)

        End Sub

        Private Function __request404() As String
            Dim _404 As String = HOME.FullName & "/404.html"

            If _404.FileExists Then
                _404 = FileIO.FileSystem.ReadAllText(_404)
            Else
                _404 = ""
            End If

            Return _404
        End Function

        ''' <summary>
        ''' 默认的404页面
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Page404 As String
            Get
                Return __request404()
            End Get
        End Property

        Protected Overrides Function __httpProcessor(client As TcpClient) As HttpProcessor
            Return New HttpProcessor(client, Me) With {
                ._404Page = __request404()
            }
        End Function
    End Class
End Namespace
