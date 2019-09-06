#Region "Microsoft.VisualBasic::6c5e8ae8d83e15f84419087cd7947bcb, RDotNET.Extensions.VisualBasic\ScriptBuilder\EscapingHelper.vb"

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

    '     Module EscapingHelper
    ' 
    '         Function: AsRid, AutoEscaping, R_Escaping
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Text

Namespace SymbolBuilder

    Public Module EscapingHelper

        ''' <summary>
        ''' 这个函数主要是处理mysql和R语言之间的不兼容的转义字符部分
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="source"></param>
        ''' <returns></returns>
        <Extension>
        Public Function AutoEscaping(Of T)(ByRef source As T()) As T()
            Dim schema = DataFramework.Schema(Of T)(PropertyAccess.ReadWrite, True)
            Dim props As PropertyInfo() = schema _
                .Values _
                .Where(Function(prop) prop.PropertyType Is GetType(String)) _
                .ToArray  ' 只需要对字符串进行转义

            For Each x As T In source
                Dim o As Object = x
                Dim s$

                For Each [property] As PropertyInfo In props
                    s = CStrSafe([property].GetValue(o))
                    s = EscapingHelper.R_Escaping(s)
                    Call [property].SetValue(o, value:=s)
                Next
            Next

            Return source
        End Function

        Const R_quot$ = "\"""
        Const R_quot_escape$ = "$R.quot;"
        Const splash$ = "\\"
        Const splash_escape$ = "$R.splash;"

        ''' <summary>
        ''' MySQL和R之间的转移符不兼容，所以在这里需要将mysql之中的不兼容的转移符取消掉，否则自动生成的R脚本会出现语法错误
        ''' </summary>
        ''' <param name="value$"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' ###### 2017-07-25
        ''' 
        ''' ```
        ''' Error: '\-' is an unrecognized escape in character string starting ""4\-"
        ''' In addition: There were 35 warnings (use warnings() To see them)
        ''' Error: unprotect_ptr: pointer Not found
        ''' ```
        ''' </remarks>
        <Extension> Public Function R_Escaping(value$) As String
            If value.StringEmpty Then
                Return ""
            Else
                Dim sb As New StringBuilder(value)

                Call sb.Replace(splash, splash_escape)

                Call sb.Replace("\0", "")
                Call sb.Replace("\Z", "[Z]")

                Call sb.Replace(R_quot, R_quot_escape)
                Call sb.Replace(ASCII.Quot, "'")
                Call sb.Replace(R_quot_escape, R_quot)

                Call sb.Replace("\"c, "")
                Call sb.Replace(splash_escape, splash)

                Return sb.ToString
            End If
        End Function

        ''' <summary>
        ''' 在R之中会自动的将变量名之中的非法字符都会替换为小数点，例如
        ''' 
        ''' ``AB/*-+~`!@#$%^&amp;*()_+:""&lt;>?,./;'\[]|C 	``将会在R之中被替换为
        ''' ``AB................_...............C..``
        ''' 
        ''' 在这个函数之中也会对字符串之中的非法字符做相同的处理
        ''' </summary>
        ''' <param name="term"></param>
        ''' <returns></returns>
        <Extension>
        Public Function AsRid(term As String) As String
            Dim s As New StringBuilder(term)

            ' _ 下划线不是非法字符，不需要做替换，否则会出错
            For Each c As Char In {
                "`"c, "~"c, "!"c, "@"c, "#"c, "$"c, "%"c, "^"c, "&"c,
                "*"c, "("c, ")"c, "-"c, "+"c, "="c, "{"c, "}"c, "["c,
                "]"c, "|"c, "\"c, ":"c, ";"c, "'"c, ","c, "/"c, "?"c,
                "<"c, ">"c, """"c
            }
                Call s.Replace(c, "."c)
            Next

            ' 如果名字的第一个字符是数字或者小数点的话，
            ' R还会额外的添加上一个大写的X
            If (s.First >= "0"c AndAlso s.First <= "9"c) OrElse s.First = "."c Then
                Return "X" & s.ToString
            Else
                Return s.ToString
            End If
        End Function
    End Module
End Namespace
