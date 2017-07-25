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
        Const R_quot_escape$ = "\R.quot;"

        ''' <summary>
        ''' MySQL和R之间的转移符不兼容，所以在这里需要将mysql之中的不兼容的转移符取消掉，否则自动生成的R脚本会出现语法错误
        ''' </summary>
        ''' <param name="value$"></param>
        ''' <returns></returns>
        <Extension> Public Function R_Escaping(value$) As String
            If value.StringEmpty Then
                Return ""
            Else
                Dim sb As New StringBuilder(value)

                Call sb.Replace("\%", "%")
                Call sb.Replace("\'", "'")
                Call sb.Replace("\Z", "[Z]")
                Call sb.Replace("\0", "")
                Call sb.Replace("\-", "-")
                Call sb.Replace("\+", "+")
                Call sb.Replace("\*", "*")
                Call sb.Replace("\\", "$")

                Call sb.Replace(R_quot, R_quot_escape)
                Call sb.Replace(ASCII.Quot, R_quot)
                Call sb.Replace(R_quot_escape, R_quot)

                Return sb.ToString
            End If
        End Function
    End Module
End Namespace