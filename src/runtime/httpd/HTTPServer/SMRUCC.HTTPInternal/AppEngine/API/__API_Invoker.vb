#Region "Microsoft.VisualBasic::7a181883cdc27c406c5d8b28dff15cbe, ..\httpd\HTTPServer\SMRUCC.HTTPInternal\AppEngine\API\__API_Invoker.vb"

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
Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.HTTPInternal.AppEngine.POSTParser

Namespace AppEngine.APIMethods

    ''' <summary>
    ''' WebApp API的抽象接口
    ''' </summary>
    ''' <param name="api">URL</param>
    ''' <param name="args">URL后面的参数请求</param>
    ''' <param name="out">返回的html页面的文档</param>
    ''' <returns>是否执行成功</returns>
    Public Delegate Function APIAbstract(api As String, args As String, ByRef out As String) As Boolean

    Public Class __API_Invoker
        Public Property Name As String
        Public Property EntryPoint As System.Reflection.MethodInfo
        Public Property Help As String
        Public Property Method As APIMethod
        Public Property Error404 As String

        Public Overrides Function ToString() As String
            Return Name
        End Function

        Public Function InvokePOST(obj As Object, args As String, inputs As PostReader, ByRef result As String) As Boolean
            Try
                Return __invokePOST(obj, args, inputs, result)
            Catch ex As Exception
                Return __handleERROR(ex, args, result)
            End Try
        End Function

        ''' <summary>
        ''' 在API的函数调用的位置，就只需要有args这一个参数
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <param name="args"></param>
        ''' <param name="result"></param>
        ''' <returns></returns>
        Public Function Invoke(obj As Object, args As String, ByRef result As String) As Boolean
            Try
                Return __invoke(obj, args, result)
            Catch ex As Exception
                Return __handleERROR(ex, args, result)
            End Try
        End Function

        Private Function __handleERROR(ex As Exception, url As String, ByRef result As String) As Boolean
            ex = New Exception("Request page: " & url, ex)
#If DEBUG Then
            result = ex.ToString
#Else
                result = Fakes(ex.ToString)
#End If
            If Not String.IsNullOrEmpty(Error404) Then
                result = result.Replace("--->", "<br />--->")
                result = result.lTokens.JoinBy(vbCrLf & "<br />")
                result = Error404.Replace("%EXCEPTION%", $"<table><tr><td><font size=""2"">{result}</font></td></tr></table>")
            End If

            Return False
        End Function

        Private Function VirtualPath(strData As String(), prefix As String) As Dictionary(Of String, String)
            Dim LQuery = (From source As String In strData
                          Let trimPrefix = Regex.Replace(source, "in [A-Z][:]\\", "", RegexOptions.IgnoreCase)
                          Let line = Regex.Match(trimPrefix, "[:]line \d+").Value
                          Let path = trimPrefix.Replace(line, "")
                          Select source, path).ToArray
            Dim LTokens = (From obj In LQuery Let tokens = obj.path.Split("\"c) Select tokens, obj.source).ToArray
            Dim p As Integer

            For i As Integer = 0 To (From obj In LTokens Select obj.tokens.Count).ToArray.Min - 1
                p = i

                If (From n In LTokens Select n.tokens(p) Distinct).ToArray.Count > 1 Then
                    Exit For
                End If
            Next

            Dim LSkips = (From obj In LTokens Select obj.source, obj.tokens.Skip(p).ToArray).ToArray
            Dim LpreFakes = (From obj In LSkips
                             Select obj.source,
                                 virtual = String.Join("/", obj.ToArray).Replace(".vb", ".vbs")).ToArray
            Dim hash = LpreFakes.ToDictionary(
                Function(obj) obj.source,
                elementSelector:=Function(obj) $"in {prefix}/{obj.virtual}:line {CInt(5000 * RandomDouble() + 100)}")
            Return hash
        End Function

        Private Function Fakes(ex As String) As String
            Dim line As String() = (From m As Match In Regex.Matches(ex, "in .+?[:]line \d+") Select str = m.Value).ToArray
            Dim hash = VirtualPath(line, "/root/ubuntu.d~/->/wwwroot/~mipaimai.com/api.php?virtual=ms_visualBasic_sh:/")
            Dim sbr = New StringBuilder(ex)

            For Each obj In hash
                Call sbr.Replace(obj.Key, obj.Value)
            Next

            Return sbr.ToString
        End Function

        Private Function __invokePOST(obj As Object, argvs As String, inputs As PostReader, ByRef result As String) As Boolean
            Dim value As Object = EntryPoint.Invoke(obj, {argvs, inputs})
            result = DirectCast(value, String)
            Return True
        End Function

        Private Function __invoke(obj As Object, argvs As String, ByRef result As String) As Boolean
            Dim value As Object = EntryPoint.Invoke(obj, {argvs})
            result = DirectCast(value, String)
            Return True
        End Function
    End Class
End Namespace
