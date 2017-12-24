#Region "Microsoft.VisualBasic::8c931698ad0f54be22195936ca677e16, ..\R.Bioconductor\RDotNET.Extensions.VisualBasic\ScriptBuilder\EscapingHelper.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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
    End Module
End Namespace
