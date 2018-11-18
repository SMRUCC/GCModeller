#Region "Microsoft.VisualBasic::6ad9810f676cabad98b2fd93a95630b1, RDotNET.Extensions.VisualBasic\ScriptBuilder\BuilderAPI.vb"

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

    '     Module BuilderAPI
    ' 
    '         Function: __getExpr, __getName, __getScript, __getValue, __isOptional
    '                   GetAPIName, (+2 Overloads) GetScript, list
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes

Namespace SymbolBuilder

    ''' <summary>
    ''' Build script token
    ''' </summary>
    Public Module BuilderAPI

        ''' <summary>
        ''' additional arguments to be passed to or from methods.
        ''' </summary>
        ''' <param name="parameters$"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function list(ParamArray parameters$()) As ParameterList
            Return New ParameterList(parameters)
        End Function

        Const IsNotAFunc = "Target object is not a R function abstract!"

        ''' <summary>
        ''' R.func(param="",...)
        ''' </summary>
        ''' <param name="token"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function GetScript(token As Object, Optional type As Type = Nothing) As String
            If token Is Nothing Then
                Throw New NullReferenceException("Script tokens is nothing!")
            End If

            If type Is Nothing Then
                type = token.GetType
            End If

            Return type.getScript(token)
        End Function

        ''' <summary>
        ''' Create script token from the class
        ''' </summary>
        ''' <param name="token"></param>
        ''' <param name="type"></param>
        ''' <returns></returns>
        <Extension> Private Function getScript(type As Type, token As Object) As String
            Dim name As String = type.GetAPIName
            Dim props = (From prop As PropertyInfo In type.GetProperties
                         Where prop.GetAttribute(Of Ignored) Is Nothing AndAlso
                             prop.CanRead
                         Let param As Parameter = prop.GetAttribute(Of Parameter)
                         Select prop,
                             func = prop.__getName(param),
                             param.__isOptional,
                             param
                         Order By __isOptional Ascending)
            Dim parameters$() = props _
                .Select(Function(x)
                            Return getExpr(token, x.prop, x.func, x.param)
                        End Function) _
                .ToArray
            Dim args As String() = LinqAPI.Exec(Of String) _
 _
                () <= From p As String
                      In parameters
                      Where Not String.IsNullOrEmpty(p)
                      Select p

            Dim script As String = $"{name}({String.Join(", " & vbCrLf, args)})"
            Return script
        End Function

        ''' <summary>
        ''' GET API name
        ''' </summary>
        ''' <param name="type"></param>
        ''' <param name="typeNameAsFuncCalls">
        ''' 是否允许当程序在查找不到<see cref="RFunc"/>自定义属性标记的时候，直接使用类型的名称作为函数名？
        ''' 如果不的话，则在没有查找结果的时候会抛出错误
        ''' </param>
        ''' <returns></returns>
        <Extension> Public Function GetAPIName(type As Type, Optional typeNameAsFuncCalls As Boolean = False) As String
            ' Get function name
            Dim name As RFunc = type.GetAttribute(Of RFunc)

            If name Is Nothing Then
                If typeNameAsFuncCalls Then
                    Return type.Name
                Else
                    With New Exception(IsNotAFunc)
                        Throw New Exception(type.FullName, .ByRef)
                    End With
                End If
            Else
                Return name.Name
            End If
        End Function

        ''' <summary>
        ''' R.func(param="",...)
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="token"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension> Public Function GetScript(Of T)(token As T) As String
            Return GetType(T).getScript(token)
        End Function

        ''' <summary>
        ''' name=value
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="prop"></param>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Private Function getExpr(x As Object, prop As PropertyInfo, name As String, param As Parameter) As String
            Dim value As Object = prop.GetValue(x)
            Dim type = If(param Is Nothing, ValueTypes.String, param.Type)
            Dim s As String = prop.PropertyType.getRValue(value, type)

            If String.IsNullOrEmpty(s) Then
                Return ""
            Else
                Return $"{name}={s}"
            End If
        End Function

        <Extension>
        Private Function getRValue(type As Type, value As Object, valueType As ValueTypes) As String
            If value Is Nothing Then
                Return Nothing
            End If

            Select Case type

                Case GetType(String)
                    If valueType = ValueTypes.Path Then
                        Return Rstring(Scripting.ToString(value).UnixPath)
                    Else
                        Return Rstring(Scripting.ToString(value))
                    End If
                Case GetType(Boolean)
                    If True = DirectCast(value, Boolean) Then
                        Return RBoolean.TRUE.__value
                    Else
                        Return RBoolean.FALSE.__value
                    End If
                Case GetType(RExpression)
                    Return DirectCast(value, RExpression).RScript
                Case Else
                    Return Scripting.ToString(value)
            End Select
        End Function

        <Extension>
        Private Function __isOptional(param As Parameter) As Boolean
            If param Is Nothing Then
                Return False
            Else
                Return param.Optional
            End If
        End Function

        <Extension>
        Private Function __getName(prop As PropertyInfo, param As Parameter) As String
            If param Is Nothing Then
                Return prop.Name
            Else
                Return param.Name
            End If
        End Function
    End Module
End Namespace
