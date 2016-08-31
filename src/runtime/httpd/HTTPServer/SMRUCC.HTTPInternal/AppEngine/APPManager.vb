#Region "Microsoft.VisualBasic::9535ed4044f4a03205918b0af31169fe, ..\httpd\HTTPServer\SMRUCC.HTTPInternal\AppEngine\APPManager.vb"

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
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.HTTPInternal.AppEngine.APIMethods
Imports SMRUCC.HTTPInternal.AppEngine.APIMethods.Arguments
Imports SMRUCC.HTTPInternal.AppEngine.POSTParser
Imports SMRUCC.HTTPInternal.Platform

Namespace AppEngine

    <[Namespace]("sdk")>
    Public Class APPManager : Inherits WebApp
        Implements IEnumerable(Of APPEngine)

        ''' <summary>
        ''' 键名要求是小写的
        ''' </summary>
        Dim RunningAPP As New Dictionary(Of String, APPEngine)

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
                If RunningAPP.ContainsKey(name.ToLower.ShadowCopy(name)) Then
                    Return RunningAPP(name)
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public Function GetApp(Of App As Class)() As App
            Dim appType As Type = GetType(App)
            Dim LQuery As App = LinqAPI.DefaultFirst(Of App) <=
 _
                From x As APPEngine
                In RunningAPP.Values.AsParallel
                Where appType.Equals(x.Application.GetType)
                Let AppInstant As App =
                    DirectCast(x.Application, App)
                Select AppInstant

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
        Public Function InvokePOST(request As HttpRequest, response As HttpResponse) As Boolean
            Return APPEngine.InvokePOST(request, RunningAPP, response)
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
        Private Shared Function __defaultFailure(api As String, request As HttpRequest, response As HttpResponse) As Boolean
            Return False
        End Function

        Public Sub ResetAPIDefault()
            DefaultAPI = AddressOf __defaultFailure
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="response">HTML输出页面或者json数据</param>
        ''' <returns></returns>
        Public Function Invoke(request As HttpRequest, response As HttpResponse) As Boolean
            Return APPEngine.Invoke(request, RunningAPP, response, DefaultAPI)
        End Function

        Public Function PrintHelp() As String
            Dim LQuery = (From app In Me.RunningAPP
                          Let head = $"<br /><div><h3>Application/Namespace                --- <strong>{baseUrl}/{app.Value.Namespace.Namespace}/</strong> ---</h3>" &
                          If(String.IsNullOrEmpty(app.Value.Namespace.Description), "",
                          $"                <p>{app.Value.Namespace.Description}</p>
                          <br /><br />")
                          Select head & vbCrLf &
                          app.Value.GetHelp & vbCrLf &
                          "</div>").ToArray
            Dim HelpPage As String = String.Join($"<br /><br />", LQuery)
            '    HelpPage = Program.requestHtml("sdk_doc.html").Replace("%SDK_HELP%", HelpPage)
            Return HelpPage
        End Function

        <ExportAPI("/sdk/help_doc.html", Info:="Get the help documents about how to using the mipaimai platform WebAPI.",
             Usage:="/sdk/help_doc.html",
             Example:="<a href=""/sdk/help_doc.html"">/sdk/help_doc.html</a>")>
        <[GET](GetType(String))>
        Public Function Help(args As String) As String
            Return PrintHelp()
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

            Dim hash As String = registerApp.Namespace.Namespace.ToLower
            If Me.RunningAPP.ContainsKey(hash) Then
                Return False
            Else
                Call RunningAPP.Add(hash, registerApp)
            End If

            Return True
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function

        Public Overrides Function Page404() As String
            Return ""
        End Function
    End Class
End Namespace
