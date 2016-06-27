Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language

Namespace Assembly.NCBI.Entrez.ComponentModels

    ''' <summary>
    ''' 用于表示获取查询结果的一个入口点
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class I_QueryEntry : Inherits ClassObject
        Implements IKeyValuePairObject(Of String, String)

        Public Property Title As String Implements IKeyValuePairObject(Of String, String).Value
        Public Property URL As String Implements IKeyValuePairObject(Of String, String).Identifier

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class
End Namespace