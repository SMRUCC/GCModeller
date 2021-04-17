#Region "Microsoft.VisualBasic::3f315926e791c39b1207a6abdd4690e8, WebCloud\SMRUCC.HTTPInternal\Core\HttpFileSystem.vb"

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

'     Class HttpFileSystem
' 
' 
'     Class HttpFileSystem
' 
'         Properties: InMemoryCacheMode, RequestStream, wwwroot
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: AddMappings, GetResource, MapPath
' 
'         Sub: Dispose
'         Delegate Function
' 
'             Properties: Page404
' 
'             Function: __getMapDIR, __handleFileGET, __httpProcessor, __request404, Open
' 
'             Sub: __handleREST, __transferData, handleGETRequest, handleOtherMethod, handlePOSTRequest
'                  handlePUTMethod, RunServer, SetGetRequest
' 
' 
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Net.Sockets
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.WebCloud.HTTPInternal.Core.Cache
Imports SMRUCC.WebCloud.HTTPInternal.Platform.Plugins
Imports fs = Microsoft.VisualBasic.FileIO.FileSystem
Imports r = System.Text.RegularExpressions.Regex

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

        ''' <summary>
        ''' The home directory of the website. where the ``index.html`` was located.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property wwwroot As DirectoryInfo
        ''' <summary>
        ''' {url, mapping path}
        ''' </summary>
        ReadOnly _virtualMappings As Dictionary(Of String, String)
        ReadOnly _nullAsExists As Boolean
        ReadOnly _cache As VirtualFileSystem
        ReadOnly _defaultFavicon As Byte() = My.Resources.favicon.UnZipStream.ToArray
        ReadOnly MAX_POST_SIZE%

        ''' <summary>
        ''' Current http filesystem is running in cache mode?
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 在缓存模式下，因为减少了IO，所以可以提升服务器的性能，物理文件系统的文件变化
        ''' 可能会延迟一段时间才会更新到内存缓存之中
        ''' </remarks>
        Public ReadOnly Property InMemoryCacheMode As Boolean
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Not _cache Is Nothing
            End Get
        End Property

        Public Function AddMappings(DIR As String, url As String) As Boolean
            url = url & "/index.html"
            url = fs.GetParentPath(url).ToLower

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
        Sub New(port As Integer, root As String,
                Optional nullExists As Boolean = False,
                Optional requestResource As IGetResource = Nothing,
                Optional threads As Integer = -1,
                Optional cache As Boolean = False)

            Call MyBase.New(port, threads)

            If Not root.DirectoryExists Then
                Call root.MakeDir
            End If

            _nullAsExists = nullExists
            wwwroot = FileIO.FileSystem.GetDirectoryInfo(root)
            App.CurrentDirectory = root
            _virtualMappings = New Dictionary(Of String, String)
            RequestStream = requestResource Or defaultResource

            Call $"WWWROOT={root.GetDirectoryFullPath}".__INFO_ECHO

            Dim size$ = App.GetVariable("MAX_POST_SIZE")

            If size.StringEmpty Then
                size = 128 * 1024 * 1024
                MAX_POST_SIZE = size
            ElseIf size.IsPattern("\d+") Then
                MAX_POST_SIZE = Val(size)
            ElseIf size.IsPattern("\d+\s*[GMK]?B", RegexICSng) Then
                Dim value# = size.Match("\d+")
                Dim unit$ = r.Replace(size, "\d+", "").Trim

                Select Case unit.ToLower
                    Case "b"
                        MAX_POST_SIZE = (value)
                    Case "kb"
                        MAX_POST_SIZE = (value.Unit(ByteSize.KB) = ByteSize.B)
                    Case "mb"
                        MAX_POST_SIZE = (value.Unit(ByteSize.MB) = ByteSize.B)
                    Case "gb"
                        MAX_POST_SIZE = (value.Unit(ByteSize.GB) = ByteSize.B)
                    Case Else
                        Throw New InvalidExpressionException($"MAX_POST_SIZE={size}")
                End Select
            End If

            If Not size.StringEmpty Then
                Call $"MAX_POST_SIZE={size} ({MAX_POST_SIZE} bytes)".__INFO_ECHO
            End If

            If cache Then
                _cache = CachedFile.CacheAllFiles(wwwroot.FullName)
            End If

#If DEBUG Then
            If cache Then
                Call "Web Server running in debugging and cache mode these two options are both openned!".Warning
            End If
#End If
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            Call MyBase.Dispose(disposing)

            If InMemoryCacheMode Then
                Call _cache.Dispose()
            End If
        End Sub

        ''' <summary>
        ''' Maps the http request url as server file system path.(这个函数返回Nothing的时候表示目标资源不存在)
        ''' </summary>
        ''' <param name="res"></param>
        ''' <returns></returns>
        Public Function MapPath(ByRef res As String) As String
            Dim mapDIR As String = __getMapDIR(res)
            Dim file As String = $"{mapDIR}/{res}"

            If Not FileExists(file) Then
                ' 检查是不是文件夹
                If file.DirectoryExists Then
                    Dim index As Value(Of String) = ""

                    ' 这是一个存在的文件夹，则返回该文件夹下面的index.html或者index.vbhtml
                    If (index = file & "/index.html").FileExists Then
                        res = file
                        file = index
                    ElseIf (index = file & "/index.htm").FileExists Then
                        res = file
                        file = index
                    ElseIf (index = file & "/index.vbhtml").FileExists Then
                        res = file
                        file = index
                    Else
                        ' 2018-3-28
                        ' 使用root文件夹，但是当前的这个文件夹之中不存在index文件？？
                        Return Nothing
                    End If
                ElseIf file.Last = "/"c OrElse file.Last = "\"c Then
                    ' 这是一个不存在的文件夹
                    ' 返回Nothing
                    Return Nothing
                End If
            End If

            If file.FileExists Then
                file = fs.GetFileInfo(file).FullName
            Else
                ' 2018-1-27

                ' 在这里还是会存在文件未找到所导致的的崩溃错误
                ' 所以在这里价格if判断，如果文件不存在，
                ' 就直接不处理这个资源请求了
            End If

            Return file
        End Function

        ''' <summary>
        ''' ``[ERR_EMPTY_RESPONSE::No data send]``
        ''' </summary>
        Const NoData As String = "[ERR_EMPTY_RESPONSE::No data send]"

        ReadOnly defaultResource As [Default](Of IGetResource) = New IGetResource(AddressOf GetResource)

        ''' <summary>
        ''' 默认的资源获取函数:<see cref="HttpFileSystem.GetResource(ByRef String)"/>.(默认是获取文件数据)
        ''' </summary>
        ''' <param name="res"></param>
        ''' <returns></returns>
        Public Function GetResource(ByRef res$) As Byte()
            Dim file$

            ' 这个资源是一个网络路径，本地文件系统上面肯定是没有这个资源的，直接返回404
            If InStr(res, "http://", CompareMethod.Text) > 0 OrElse InStr(res, "https://", CompareMethod.Text) > 0 Then
                res = Nothing
                Return {}
            Else
                ' 假若存在空格之类的,会因为被js转义而无法识别
                ' 在这里需要反转义一下
                res = res.UrlDecode
            End If

            Try
                file = MapPath(res)
                res = file

                ' 目标资源不存在，则什么数据也不返回
                If file Is Nothing Then
                    Return {}
                End If
            Catch ex As Exception
                Throw New Exception(res, ex)
            End Try

            ' 在缓存模式下，将不会再读取物理文件系统
            If InMemoryCacheMode Then
                Return _cache.GetFileBuffer(file)
            End If

            If file.FileExists Then

                ' 判断是否为vbhtml文件？
                If file.ExtensionSuffix.TextEquals("vbhtml") Then
                    Dim html$ = ScriptingExtensions.ReadHTML(wwwroot.FullName, file)
                    Return Encoding.UTF8.GetBytes(html)
                Else
                    Return IO.File.ReadAllBytes(file)
                End If

            Else

                If file.FileName.TextEquals("favicon.ico") Then
                    Return _defaultFavicon
                ElseIf _nullAsExists Then
                    Call $"{NoData} {file.ToFileURL}".__DEBUG_ECHO
                    Return New Byte() {}
                Else
                    Dim message$ = New String() {res, file}.GetJson
                    Throw New NullReferenceException(message)
                End If

            End If
        End Function

        ''' <summary>
        ''' 默认是使用<see cref="GetResource"/>获取得到文件数据
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property RequestStream As IGetResource

        Public Delegate Function IGetResource(ByRef res As String) As Byte()

        ''' <summary>
        ''' Public Delegate Function <see cref="IGetResource"/>(ByRef res As <see cref="System.String"/>) As <see cref="Byte()"/>
        ''' </summary>
        ''' <param name="req"></param>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub SetGetRequest(req As IGetResource)
            _RequestStream = req
        End Sub

        ''' <summary>
        ''' 长
        ''' </summary>
        ''' <param name="res"></param>
        ''' <returns></returns>
        Private Function __getMapDIR(ByRef res As String) As String
            If res = "/" OrElse res = "\" Then
                res = wwwroot.FullName
            End If

            Dim mapDIR As String = fs.GetParentPath(res).ToLower.Replace("\", "/")

            If _virtualMappings.ContainsKey(mapDIR) Then
                res = Mid(res, mapDIR.Length + 1)
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
                mapDIR = wwwroot.FullName
            End If

            ' 2017-4-30 这里还需要替换回去，否则会出现两个wwwroot连接在一起的情况
            If res = wwwroot.FullName Then
                res = "/"
            End If

            Return mapDIR
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <ExportAPI("Open")>
        Public Shared Function Open(home As String, Optional port As Integer = 80) As HttpFileSystem
            Return New HttpFileSystem(port, home, True)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <ExportAPI("Run")>
        Public Shared Sub RunServer(server As HttpServer)
            Call server.Run()
        End Sub

        ''' <summary>
        ''' 为什么不需要添加<see cref="HttpProcessor.writeSuccess(Long, String)"/>方法？？？
        ''' </summary>
        ''' <param name="p"></param>
        Public Overrides Sub handleGETRequest(p As HttpProcessor)
            Dim res As String = p.http_url

            ' The file content is null or not exists, that possible means this is a GET REST request not a Get file request.
            ' This if statement makes the file GET request compatible with the REST API
            If res.PathIllegal Then
                Call __handleREST(p)
            Else
                If Not __handleFileGET(res, p) Then
                    Call __handleREST(p)
                End If
            End If
        End Sub

        Private Function __handleFileGET(res$, p As HttpProcessor) As Boolean

            ' 由于子文件夹可能会是以/的方式请求index.html，所以在这里res的值可能会变化，文件拓展名放在变化之后再解析
            Dim buf As Byte() = RequestStream(res)

            ' 假若目标请求是以/结尾的目录路径，并且该目录路径下的index.html并不存在
            ' 那么使用默认的GetResource函数的话，会返回空值，则直接调用下面的
            ' GetFileInfo函数就会报错，所以在这里要判断一下res字符串是否为空，
            ' 尽量减少Throw Exception来提升服务器性能
            If res.StringEmpty Then
                Return False
            End If

            Dim ext$ = fs.GetFileInfo(res) _
                .Extension _
                .ToLower

            If ext.TextEquals(".html") OrElse
               ext.TextEquals(".htm") OrElse
               ext.TextEquals(".vbhtml") Then ' Transfer HTML document.

                If buf.Length = 0 Then
                    Dim html$ = __request404()
                    html = html.Replace("%404%", res)
                    buf = Encoding.UTF8.GetBytes(html)
                End If

                ' response的头部默认是html文件类型
                Call p.writeSuccess(buf.Length,)
                Call p.outputStream.BaseStream.Write(buf, Scan0, buf.Length)
            ElseIf buf.Length = 0 Then
                Return False
            Else
                Call __transferData(p, ext, buf, res.BaseName)
            End If

            Return True
        End Function

        ''' <summary>
        ''' handle the GET/POST request at here
        ''' </summary>
        ''' <param name="p"></param>
        Protected Overridable Sub __handleREST(p As HttpProcessor)
            ' Do Nothing
        End Sub

        ''' <summary>
        ''' 为什么不需要添加content-type说明？？
        ''' </summary>
        ''' <param name="p"></param>
        ''' <param name="ext"></param>
        ''' <param name="buf"></param>
        Private Sub __transferData(p As HttpProcessor, ext As String, buf As Byte(), name As String)
            Dim type As ContentType

            If Not ContentTypes.SuffixTable.ContainsKey(ext) Then
                type = ContentTypes.SuffixTable(".bin")
            Else
                type = ContentTypes.SuffixTable(ext)
            End If

            'Dim chead As New Content With {
            '    .attachment = name,
            '    .Length = buf.Length,
            '    .Type = contentType.MIMEType
            '}

            Call p.writeSuccess(buf.Length, type.MIMEType)
            Call p.outputStream.BaseStream.Write(buf, Scan0, buf.Length)
            Call $"Transfer data:  {type.ToString} ==> [{buf.Length} Bytes]!".__DEBUG_ECHO
        End Sub

        Public Overrides Sub handlePOSTRequest(p As HttpProcessor, inputData$)

        End Sub

        ''' <summary>
        ''' 页面之中必须要有一个``%404%``占位符来让服务器放置错误信息
        ''' </summary>
        ''' <returns></returns>
        Private Function __request404() As String
            Dim _404 As String = (wwwroot.FullName & "/404.vbhtml")

            If _404.FileExists(True) Then
                _404 = wwwroot.FullName.ReadHTML(path:=_404)
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
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return __request404()
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function __httpProcessor(client As TcpClient) As HttpProcessor
            Return New HttpProcessor(client, Me, MAX_POST_SIZE) With {
                ._404Page = AddressOf __request404
            }
        End Function

        Public Overrides Sub handleOtherMethod(p As HttpProcessor)
            Dim msg As String = $"Unsupport {NameOf(p.http_method)}:={p.http_method}"
            Call msg.__DEBUG_ECHO
            Call p.writeFailure(HTTP_RFC.RFC_METHOD_NOT_ALLOWED, msg)
        End Sub

        Public Overrides Sub handlePUTMethod(p As HttpProcessor, inputData$)
            Throw New NotImplementedException()
        End Sub
    End Class
End Namespace
