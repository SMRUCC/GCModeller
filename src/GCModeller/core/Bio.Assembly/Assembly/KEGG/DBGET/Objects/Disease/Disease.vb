Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Class Disease : Implements INamedValue

        Public Property Category As String
        Public Property Comment As String
        Public Property Drug As KeyValuePair()

        Public Property Entry As String Implements IKeyedEntity(Of String).Key
        Public Property Name As String

        Public Property Genes As TripleKeyValuesPair()
        Public Property Markers As TripleKeyValuesPair()
        Public Property OtherDBs As KeyValuePair()

        Public Property References As Reference()

    End Class
End Namespace