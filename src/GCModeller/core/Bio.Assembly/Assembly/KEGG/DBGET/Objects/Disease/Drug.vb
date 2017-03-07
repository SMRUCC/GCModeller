
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Class Drug : Implements INamedValue

        Public Property Entry As String Implements IKeyedEntity(Of String).Key
        Public Property Name As String
        Public Property Members As KeyValuePair()
        Public Property Remark As String
        Public Property Comment As String
        Public Property Target As TripleKeyValuesPair()
        Public Property Metabolism As String

    End Class
End Namespace