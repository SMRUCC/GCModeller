Imports System.Text.RegularExpressions
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace ComponentModel.DBLinkBuilder

    Public Interface IMetabolite
        Property ChEBI As String()
        Property KEGGCompound As String
    End Interface

    Public Interface IDBLink
        Property locusId As String
        Property Address As String
        ''' <summary>
        ''' 将对象模型转换为含有格式的字符串的值用以写入文件之中
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GetFormatValue() As String
    End Interface
End Namespace