#Region "Microsoft.VisualBasic::693e9293c674f0e0266595b5e23bfd8a, WebCloud\SMRUCC.HTTPInternal\AppEngine\APPManager.vb"

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

'     Class APPManager
' 
'         Properties: baseUrl, DefaultAPI
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: falseAsDefaultFailure, GetApp, GetEnumerator, Help, helpInfo
'                   IEnumerable_GetEnumerator, Invoke, InvokePOST, PrintHelp, Register
'                   ServerStatus, Test404
' 
'         Sub: Join, ResetAPIDefault
' 
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods
Imports SMRUCC.WebCloud.HTTPInternal.Core
Imports SMRUCC.WebCloud.HTTPInternal.Platform

Namespace AppEngine

    ''' <summary>
    ''' Help document developer user manual page
    ''' </summary>
    <[Namespace]("dashboard")> Public Class APPManager : Inherits WebApp
        Implements IEnumerable(Of APPEngine)

        ''' <summary>
        ''' 键名要求是小写的
        ''' </summary>
        Dim RunningAPP As New Dictionary(Of String, APPEngine)
        ''' <summary>
        ''' 动态添加的API，这些API的url不是标准的url命名空间
        ''' </summary>
        Dim dynamics As New Dictionary(Of String, (App As Object, API As APIInvoker))

        ''' <summary>
        ''' 生成帮助文档所需要的
        ''' </summary>
        ''' <returns></returns>
        Public Property baseUrl As String

        Sub New(API As PlatformEngine)
            Call MyBase.New(API)
            Call Register(Me)
        End Sub

        Default Public ReadOnly Property App(name As String) As APPEngine
            Get
                name = name.ToLower

                If RunningAPP.ContainsKey(name) Then
                    Return RunningAPP(name)
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="url$"></param>
        ''' <param name="method"></param>
        ''' <param name="API"></param>
        ''' <param name="app">假若是模块Module，则使用这个默认的空值，假若是Class中的实例方法，则还需要把那个Class对象传递进来</param>
        ''' <param name="help$"></param>
        Public Sub Join(url$, method As APIMethod, API As MethodInfo, Optional app As Object = Nothing, Optional help$ = "No help info...")
            dynamics(url.ToLower) = (app, New APIInvoker(API) With {
                .Help = help,
                .Method = method,
                .Name = url
            })
        End Sub

        ''' <summary>
        ''' Get running app by type.
        ''' </summary>
        ''' <typeparam name="App"></typeparam>
        ''' <returns></returns>
        Public Function GetApp(Of App As Class)() As App
            Dim appType As Type = GetType(App)
            Dim LQuery As App = LinqAPI.DefaultFirst(Of App) <=
 _
                From engine As APPEngine
                In RunningAPP.Values
                Where appType.Equals(engine.Application.GetType)
                Let appInstant As App = DirectCast(engine.Application, App)
                Select appInstant

            Return LQuery
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of APPEngine) Implements IEnumerable(Of APPEngine).GetEnumerator
            For Each obj In RunningAPP
                Yield obj.Value
            Next
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="response">HTML输出页面或者json数据</param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function InvokePOST(request As HttpPOSTRequest, response As HttpResponse) As Boolean
            Return APPEngine.InvokePOST(request, RunningAPP, response, dynamics)
        End Function

        ''' <summary>
        ''' 当WebApp查找失败的时候所执行的默认的API函数
        ''' </summary>
        ''' <returns></returns>
        Public Property DefaultAPI As APIAbstract

        ''' <summary>
        ''' 默认是API执行失败
        ''' </summary>
        ''' <param name="api"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function falseAsDefaultFailure(api As String, request As HttpRequest, response As HttpResponse) As Boolean
            Return False
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub ResetAPIDefault()
            DefaultAPI = AddressOf falseAsDefaultFailure
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="response">HTML输出页面或者json数据</param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Invoke(request As HttpRequest, response As HttpResponse) As Boolean
            Return APPEngine.Invoke(request, RunningAPP, response, dynamics, DefaultAPI)
        End Function

        Public Function PrintHelp() As String
            Dim LQuery$() = LinqAPI.Exec(Of String) <=
 _
                From app As KeyValuePair(Of String, APPEngine)
                In Me.RunningAPP
                Let head = helpInfo(app.Key, app.Value)
                Select "<div>" & vbCrLf &
                           head & vbCrLf &
                           app.Value.GetHelp & vbCrLf &
                       "</div>"

            Dim HelpPage As String = String.Join($"<br /><br />", LQuery)

            Return HelpPage
        End Function

        Private Function helpInfo(appKey$, app As APPEngine) As String
            Dim describ As String

            If String.IsNullOrEmpty(app.Namespace.Description) Then
                describ = ""
            Else
                describ = <div>
                              <p><%= app.Namespace.Description %></p>
                              <br/>
                              <br/>
                          </div>
            End If

            Dim head As String = sprintf(
                <div>
                    <h3>Application/Namespace          --- <strong><%= baseUrl %>/<%= app.Namespace.Namespace %>/</strong> ---</h3>
                    %s
                </div>, describ)

            Return head
        End Function

        <ExportAPI("/dashboard/help_doc.html")>
        <Description("Get the help documents about how to using the mipaimai platform WebAPI.")>
        <Usage("/dashboard/help_doc.html")>
        <Example("<a href=""/dashboard/help_doc.html"">/dashboard/help_doc.html</a>")>
        <[GET](GetType(String))>
        Public Function Help(request As HttpRequest, response As HttpResponse) As Boolean
            Call response.WriteHTML(PrintHelp)
            Return True
        End Function

        <ExportAPI("/dashboard/server_status.vbs")>
        <[GET](GetType(String))>
        Public Function ServerStatus(request As HttpRequest, response As HttpResponse) As Boolean
            Call response.WriteLine(PlatformEngine._threadPool.GetStatus.GetJson(indent:=True))
            Return True
        End Function

        <ExportAPI("/dashboard/404_test.vbs")>
        <[GET](GetType(String))>
        Public Function Test404(request As HttpRequest, response As HttpResponse) As Boolean
            Throw New StackOverflowException(PlatformEngine._threadPool.GetStatus.GetJson(indent:=True))
        End Function

        ''' <summary>
        ''' 向开放平台之中注册API接口
        ''' </summary>
        ''' <typeparam name="APP"></typeparam>
        ''' <param name="application"></param>
        ''' <returns></returns>
        Public Function Register(Of APP As WebApp)(application As APP) As Boolean
            Dim registerApp = APPEngine.Imports(application)

            If registerApp Is Nothing Then
                Return False
            End If

            Dim key As String = registerApp.Namespace.Namespace.ToLower

            If Me.RunningAPP.ContainsKey(key) Then
                Return False
            Else
                Call RunningAPP.Add(key, registerApp)
            End If

            Return True
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
