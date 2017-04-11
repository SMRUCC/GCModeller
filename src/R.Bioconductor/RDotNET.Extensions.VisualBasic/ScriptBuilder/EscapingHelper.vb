Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace SymbolBuilder

    Public Module EscapingHelper

        ''' <summary>
        ''' 将``\``进行转义为``\\``
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
                    s = Scripting.CStrSafe([property].GetValue(o))
                    s = s.Replace("\", "\\")
                    Call [property].SetValue(o, value:=s)
                Next
            Next

            Return source
        End Function
    End Module
End Namespace