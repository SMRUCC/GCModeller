#Region "Microsoft.VisualBasic::ca06048d91463b56685f323556778588, RDotNET.Extensions.VisualBasic\ScriptBuilder\BuilderAPI.vb"

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
    '         Function: __getName, __isOptional, GetAPIName, getExpr, getRValue
    '                   getScript, (+2 Overloads) GetScript, list
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
        ''' ``R.func(param="",...)``
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
            Dim type = If(param Is Nothing, ValueTypes.ref, param.Type)
            Dim s As String = prop.PropertyType.getRValue(value, type)

            ' 如果参数值为空值，则会返回空字符串，在后面构建表达式的时候
            ' 会忽略掉这个空字符串
            If String.IsNullOrEmpty(s) Then
                Return ""
            Else
                Return $"{name}={s}"
            End If
        End Function

        ''' <summary>
        ''' 将.NET环境之中的变量值转换为R脚本语言环境之中的变量值
        ''' </summary>
        ''' <param name="type">The type info of the property <paramref name="value"/></param>
        ''' <param name="value"></param>
        ''' <param name="valueType"></param>
        ''' <returns></returns>
        <Extension>
        Private Function getRValue(type As Type, value As Object, valueType As ValueTypes) As String
            If value Is Nothing Then
                Return Nothing
            End If

            Select Case type

                Case GetType(String)

                    ' 如果在VB.NET环境之中, 值的类型在R语言环境之中可能为:
                    '
                    ' 文件路径: 则会自动替换\符号为Linux下面的路径分隔符号/, 并添加双引号在脚本之中表示当前的值为字符串
                    ' R环境之中的对象引用: 则这个时候不进行任何字符串上面的操作处理
                    ' R环境之中的字符串: 则直接添加双引号以在R脚本之中表示其为字符串
                    If valueType = ValueTypes.path Then
                        ' 因为UnixPath拓展函数可能会自动添加双引号
                        ' 所以在这里就直接进行字符替换操作了, 避免可能由于UnixPath
                        ' 函数所可能产生的bug
                        Return Rstring(Scripting.ToString(value).Replace("\", "/"))
                    ElseIf valueType = ValueTypes.ref Then
                        ' 变量引用，则不添加双引号
                        Return Scripting.ToString(value)
                    Else
                        Return Rstring(Scripting.ToString(value))
                    End If

                Case GetType(Boolean)

                    ' 对于逻辑值,则直接根据值返回R语言环境之中的TRUE或者FALSE
                    If True = DirectCast(value, Boolean) Then
                        Return NameOf(RBoolean.TRUE)
                    Else
                        Return NameOf(RBoolean.FALSE)
                    End If
                Case GetType(RExpression)
                    Return DirectCast(value, RExpression).RScript
                Case GetType(Double()), GetType(Integer())
                    ' 是一个数字向量
                    ' 返回c()向量表达式, 因为R解释器会存在一个栈空间上线的问题, 所以在API之中会对向量内容进行分块传递
                    ' 但是在这里的脚本构建器之中, 由于source函数运行部存在这个问题, 所以在这里可以直接通过c()向量函数
                    ' 来生成最终的脚本表达式结果
                    Return RScripts.c(vector:=DirectCast(value, Array))
                Case Else

                    ' 对于枚举类型, 则会将枚举值转换为字符串之后传递到R脚本环境之中
                    If type.IsInheritsFrom(GetType([Enum])) Then
                        Dim str$ = DirectCast(value, [Enum]).Description

                        ' 如果修饰的类型表明当前的参数值应该是一个字符串
                        ' 则枚举值的描述结果字符串会被封装在双引号之中表明其为一个字符串
                        ' 反之, 其他的类型装饰则会直接认为当前的枚举值为一个R环境之中
                        ' 的对象引用表达式, 当前的枚举值的描述结果会被直接生成于脚本之中
                        If valueType = ValueTypes.string Then
                            Return Rstring(str)
                        Else
                            Return str
                        End If
                    Else
                        Return Scripting.ToString(value)
                    End If
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
