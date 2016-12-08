#Region "Microsoft.VisualBasic::6c951da0c0b9797f3290e55bcbd9b1a1, ..\sciBASIC.ComputingServices\RQL\RESTProvider.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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
Imports Microsoft.VisualBasic.ComputingServices.TaskHost
Imports Microsoft.VisualBasic.Linq.Framework.Provider
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.RQL.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.WebCloud.HTTPInternal.Core

''' <summary>
''' 在线查询服务提供模块，在这个模块之中只负责进行url参数的解析工作
''' </summary>
Public Class RESTProvider : Inherits HttpServer

    Public ReadOnly Property LinqProvider As LinqAPI

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="portal"></param>
    ''' <param name="repo">需要在这里将url转换为Long以进行protocol的绑定操作</param>
    Sub New(portal As Integer, repo As Linq.Repository)
        Call MyBase.New(portal)
        Me.LinqProvider = New LinqAPI(repo)
    End Sub

    Sub New()
        Call Me.New(80, Repository.LoadDefault)
    End Sub

    Public Function AddLinq(url As String, resource As String, handle As GetLinqResource) As Boolean
        Try
            Call LinqProvider.Repository.AddLinq(url, resource, handle)
        Catch ex As Exception
            ex = New Exception(url, ex)
            ex = New Exception(resource, ex)
            Call App.LogException(ex)
            Return False
        End Try

        Return True
    End Function

    ''' <summary>
    ''' http://linq.gcmodeller.org/kegg/pathways?where=test_expr(pathway)
    ''' 测试条件里面的对象实例的标识符使用资源url里面的最后一个标识符为变量名
    ''' 测试条件表达式使用VisualBasic的语法
    ''' 测试条件必须以where起头开始
    ''' </summary>
    ''' <param name="p"></param>
    ''' <return>返回一个网络终点IpEndPoint</return>
    Public Overrides Sub handleGETRequest(p As HttpProcessor)
        If p.IsWWWRoot Then
            Call p.WriteLine(__helps)  ' 返回帮助信息
        Else
            Call __apiInvoke(p)
        End If
    End Sub

    Private Function __helps() As String

    End Function

    Private Sub __apiInvoke(p As HttpProcessor)
        Dim url As String = p.http_url
        Dim pos As Integer = InStr(url, "?")
        Dim args As String = ""

        If pos = 0 Then
            ' expr为空
        Else
            args = Mid(url, pos + 1).Trim  ' 参数里面可能含有转意字符，还需要进行转意
            args = args.UrlDecode
            url = Mid(url, 1, pos - 1).ToLower
        End If

        Call p.writeSuccess()

        Select Case url
            Case "/move_next.vb"
                Call p.WriteLine(LinqProvider.MoveNext(args.requestParser()))
            Case "/helps"
                Call p.WriteLine(__helps)
            Case "/close.vb"
                Call p.WriteLine(LinqProvider.Free(args.requestParser()))
            Case Else ' 打开linq查询
                Dim Linq As LinqEntry = LinqProvider.OpenQuery(url, args)
                Call p.outputStream.WriteLine(Linq.GetJson)
        End Select
    End Sub

    Public Overrides Sub handlePOSTRequest(p As HttpProcessor, inputData As MemoryStream)
        Call p.writeFailure("Method not allowed!")
    End Sub

    Protected Overrides Function __httpProcessor(client As TcpClient) As HttpProcessor
        Return New HttpProcessor(client, Me)
    End Function

    Public Overrides Sub handleOtherMethod(p As HttpProcessor)
    End Sub
End Class

