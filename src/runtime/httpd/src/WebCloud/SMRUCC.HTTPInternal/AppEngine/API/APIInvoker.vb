#Region "Microsoft.VisualBasic::db303801ba67233f659a4c04adb23d9a, WebCloud\SMRUCC.HTTPInternal\AppEngine\API\APIInvoker.vb"

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

'     Class APIInvoker
' 
'         Properties: EntryPoint, Help, Method, Name
' 
'         Function: __handleERROR, __invoke, __invokePOST, Fakes, Invoke
'                   InvokePOST, ToString, VirtualPath
' 
' 
' /********************************************************************************/

#End Region

Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.WebCloud.HTTPInternal.Core

Namespace AppEngine.APIMethods

    Public Class APIInvoker : Implements INamedValue

        Public Property Name As String Implements INamedValue.Key
        Public Property Help As String
        Public Property Method As APIMethod

        ''' <summary>
        ''' 这个属性提供外部dll中的Api方法的执行入口点
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property EntryPoint As MethodInfo

        ReadOnly getArguments As NamedValue(Of Func(Of HttpRequest, Object))()

        Const InvalidMethodDefine$ = "Api method should have two required parameters: one is for request input and another is for response output."

        Sub New(entryPoint As MethodInfo)
            Dim parameters = entryPoint.GetParameters

            If parameters.Length < 2 Then
                Throw New InvalidProgramException(InvalidMethodDefine)
            ElseIf parameters.Length = 2 Then
                getArguments = Nothing
            Else
                getArguments = parameters _
                    .DoCall(AddressOf getParameters) _
                    .ToArray
            End If

            Me.EntryPoint = entryPoint
        End Sub

        Private Shared Iterator Function getParameters(parameters As ParameterInfo()) As IEnumerable(Of NamedValue(Of Func(Of HttpRequest, Object)))
            ' 倒数两个参数必须要是httprequest和httpresponse
            ' 所以在这里忽略掉最后的两个元素
            For Each ainput As ParameterInfo In parameters.Take(parameters.Length - 2)
                Yield New NamedValue(Of Func(Of HttpRequest, Object)) With {
                    .Name = ainput.Name,
                    .Value = ainput.DoCall(AddressOf parameterGetter)
                }
            Next
        End Function

        Private Shared Function parameterGetter(parameter As ParameterInfo) As Func(Of HttpRequest, Object)
            Dim name = parameter.Name
            Dim type As Type = parameter.ParameterType

            Return Function(request)
                       If request.HasValue(name) Then
                           Dim strVal$ = request(name)
                           Dim value As Object = Scripting.CTypeDynamic(strVal, type)

                           Return value
                       Else
                           Return parameter.DefaultValue
                       End If
                   End Function
        End Function

        Public Overrides Function ToString() As String
            Return Name
        End Function

        <POST(GetType(Boolean))>
        Public Function Invoke(App As Object, request As HttpPOSTRequest, response As HttpResponse) As Boolean
            Try
                Return doExternalInvoke(App, request, response)
            Catch ex As Exception
                Return internalHandleERROR(ex, request.URL, response)
            End Try
        End Function

        ''' <summary>
        ''' 在API的函数调用的位置，就只需要有args这一个参数
        ''' </summary>
        ''' <returns></returns>
        <[GET](GetType(Boolean))>
        Public Function Invoke(App As Object, request As HttpRequest, response As HttpResponse) As Boolean
            Try
                Return doExternalInvoke(App, request, response)
            Catch ex As Exception
                Return internalHandleERROR(ex, request.URL, response)
            End Try
        End Function

        Private Function internalHandleERROR(ex As Exception, url As String, ByRef response As HttpResponse) As Boolean
            Dim result As String
            ex = New Exception("Request page: " & url, ex)

#If DEBUG Then
            result = ex.ToString
#Else
            result = APIMethods.Fakes(ex)
#End If
            Call App.LogException(ex)
            Call ex.PrintException
            Call response.WriteError(500, result)

            Return False
        End Function

        ''' <summary>
        ''' <paramref name="app"/>.Method(<paramref name="request"/>, <paramref name="response"/>)
        ''' </summary>
        ''' <param name="app"></param>
        ''' <param name="request"></param>
        ''' <param name="response"></param>
        ''' <returns></returns>
        Private Function doExternalInvoke(app As Object, request As HttpRequest, response As HttpResponse) As Boolean
            If getArguments Is Nothing Then
                Return EntryPoint.Invoke(app, {request, response})
            Else
                Return InvokeSpecificArgumentMethod(app, request, response)
            End If
        End Function

        ''' <summary>
        ''' <paramref name="app"/>.Method(arg1, arg2, ..., argn, <paramref name="request"/>, <paramref name="response"/>)
        ''' </summary>
        ''' <param name="app"></param>
        ''' <param name="request"></param>
        ''' <param name="response"></param>
        ''' <returns></returns>
        Private Function InvokeSpecificArgumentMethod(app As Object, request As HttpRequest, response As HttpResponse) As Boolean
            Dim args As New List(Of Object)

            For Each getter In getArguments
                args += getter(request)
            Next

            Return EntryPoint.Invoke(app, args + {request, response})
        End Function
    End Class
End Namespace
