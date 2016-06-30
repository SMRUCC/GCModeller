Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization

Public Structure FlareImports : Implements sIdEnumerable

    Public Property name As String Implements sIdEnumerable.Identifier
    Public Property size As Integer
    ''' <summary>
    ''' 与本节点对象<see cref="name"/>相连接的节点对象的标识符
    ''' </summary>
    ''' <returns></returns>
    Public Property [imports] As String()

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Structure
