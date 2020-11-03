﻿#Region "Microsoft.VisualBasic::0d7a9ce28cae3e697ecf76f2b53078b9, Microsoft.VisualBasic.Core\Extensions\WebServices\HttpGet.vb"

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

    ' Module HttpGet
    ' 
    '     Properties: HttpRequestTimeOut
    ' 
    '     Function: [GET], BuildWebRequest, Get_PageContent, httpRequest, LogException
    '               urlGet
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Net
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

''' <summary>
''' Tools for http get
''' </summary>
Public Module HttpGet

#If FRAMEWORD_CORE Then
    ''' <summary>
    ''' Get the html page content from a website request or a html file on the local filesystem.
    ''' (同时支持http位置或者本地文件，失败或者错误会返回空字符串)
    ''' </summary>
    ''' <param name="url">web http request url or a file path handle</param>
    ''' <param name="retry">发生错误的时候的重试的次数</param>
    ''' <returns>失败或者错误会返回空字符串</returns>
    ''' <remarks>这个工具只适合于文本数据的传输操作</remarks>
    <ExportAPI("Webpage.Request")>
    <Extension> Public Function [GET](url As String,
                                      <Parameter("Request.TimeOut")>
                                      Optional retry As UInt16 = 0,
                                      Optional headers As Dictionary(Of String, String) = Nothing,
                                      Optional proxy As String = Nothing,
                                      Optional doNotRetry404 As Boolean = True,
                                      Optional UA$ = Nothing,
                                      Optional refer$ = Nothing,
                                      Optional ByRef is404 As Boolean = False,
                                      Optional echo As Boolean = True) As String
#Else
    ''' <summary>
    ''' Get the html page content from a website request or a html file on the local filesystem.
    ''' </summary>
    ''' <param name="url">web http request url or a file path handle</param>
    ''' <param name="RequestTimeOut">发生错误的时候的重试的次数</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    '''
    <Extension> Public Function Get_PageContent(url As String, Optional RequestTimeOut As UInteger = 20, Optional FileSystemUrl As Boolean = False) As String
#End If
        Dim isFileUrl As String = (InStr(url, "http://", CompareMethod.Text) <> 1) AndAlso (InStr(url, "https://", CompareMethod.Text) <> 1)

        If echo Then
            Call $"GET {If(isFileUrl, url.ToFileURL, url)}".__DEBUG_ECHO
        End If

        ' do status indicator reset
        is404 = False

        ' 类似于php之中的file_get_contents函数,可以读取本地文件内容
        If File.Exists(url) Then
            If echo Then
                Call "[Job DONE!]".__DEBUG_ECHO
            End If

            Return url.ReadAllText
        Else
            If isFileUrl Then
                If echo Then
                    Call $"URL {url.ToFileURL} can not be solved on your filesystem!".Warning
                End If

                is404 = True
                Return ""
            End If
        End If

        If Not refer.StringEmpty Then
            If headers Is Nothing Then
                headers = New Dictionary(Of String, String)
            End If

            headers(NameOf(refer)) = refer
        End If

        Return url.httpRequest(retry, headers, proxy, doNotRetry404, UA, is404, echo)
    End Function

    <Extension>
    Private Function httpRequest(url$, retries%, headers As Dictionary(Of String, String), proxy$, DoNotRetry404 As Boolean, UA$, ByRef is404 As Boolean, echo As Boolean) As String
        Dim retryTime As Integer = 0

        If String.IsNullOrEmpty(proxy) Then
            proxy = WebServiceUtils.Proxy
        End If

        Try
RETRY:      Return BuildWebRequest(url, headers, proxy, UA).urlGet(echo:=echo)
        Catch ex As Exception When InStr(ex.Message, "(404) Not Found") > 0 AndAlso DoNotRetry404
            is404 = True
            Return LogException(url, New Exception(url, ex))

        Catch ex As Exception When retryTime < retries

            retryTime += 1

            Call "Data download error, retry connect to the server!".PrintException
            GoTo RETRY

        Catch ex As Exception
            ex = New Exception(url, ex)
            ex.PrintException

            Return LogException(url, ex)
        End Try
    End Function

    Private Function LogException(url$, ex As Exception) As String
        Dim exMsg As String = {
            "Unable to get the http request!",
           $"  Url:=[{url}]",
            "  EXCEPTION ===>",
            "",
            ex.ToString
        }.JoinBy(ASCII.LF)

        Return App.LogException(exMsg, NameOf([GET]) & "::HTTP_REQUEST_EXCEPTION")
    End Function

    Const doctorcomError$ = "Please login your Campus Broadband Network Client at first!"

    ''' <summary>
    ''' Request timeout unit in seconds.
    ''' </summary>
    ''' <returns></returns>
    Public Property HttpRequestTimeOut As Double

    Public Function BuildWebRequest(url$, headers As Dictionary(Of String, String), proxy$, UA$) As HttpWebRequest
        Dim webRequest As HttpWebRequest = HttpWebRequest.Create(url)

        webRequest.Headers.Add("Accept-Language", "en-US,en;q=0.8,zh-Hans-CN;q=0.5,zh-Hans;q=0.3")
        webRequest.UserAgent = UA Or DefaultUA

        If HttpRequestTimeOut > 0 Then
            webRequest.Timeout = 1000 * HttpRequestTimeOut
        End If

        If Not headers.IsNullOrEmpty Then
            For Each x In headers
                webRequest.Headers(x.Key) = x.Value
            Next
        End If
        If Not String.IsNullOrEmpty(proxy) Then
            Call webRequest.SetProxy(proxy)
        End If

        Return webRequest
    End Function

    ''' <summary>
    ''' Perform a web url query request
    ''' </summary>
    ''' <param name="webrequest"></param>
    ''' <returns></returns>
    <Extension> Private Function urlGet(webrequest As HttpWebRequest, echo As Boolean) As String
        Dim timer As Stopwatch = Stopwatch.StartNew
        Dim url As String = webrequest.RequestUri.ToString

        Using respStream As Stream = webrequest.GetResponse.GetResponseStream,
            reader As New StreamReader(respStream)

            Dim htmlBuilder As New StringBuilder
            Dim line As Value(Of String) = ""

            Do While Not (line = reader.ReadLine) Is Nothing
                htmlBuilder.AppendLine(line)
            Loop

            Dim html As String = htmlBuilder.ToString
            Dim title As String = html.HTMLTitle

            ' 判断是否是由于还没有登陆校园网客户端而导致的错误
            If InStr(html, "http://www.doctorcom.com", CompareMethod.Text) > 0 Then
                Call doctorcomError.PrintException
                Return ""
            ElseIf echo Then
                Dim time$ = ValueTypes.ReadableElapsedTime(timer.ElapsedMilliseconds)
                Dim debug$ = $"[{url}] {title} - {Len(html)} chars in {time}"

                If timer.ElapsedMilliseconds > 1000 Then
                    Call debug.Warning
                Else
                    Call debug.__INFO_ECHO
                End If
            End If

#If DEBUG Then
            Call html.SaveTo($"{App.AppSystemTemp}/{App.PID}/{url.NormalizePathString}.html")
#End If
            Return html
        End Using
    End Function
End Module
