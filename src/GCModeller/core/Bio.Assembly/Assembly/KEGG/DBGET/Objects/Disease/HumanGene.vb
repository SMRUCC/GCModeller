Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Class HumanGene : Implements INamedValue

        Public Property Entry As String Implements IKeyedEntity(Of String).Key
        Public Property GeneName As String
        Public Property Definition As KeyValuePair
        Public Property Pathway As KeyValuePair()
        Public Property Disease As KeyValuePair()
        Public Property DrugTarget As String()
        Public Property Motif As NamedCollection(Of String)
        Public Property [Structure] As NamedCollection(Of String)
        Public Property Position As String
        Public Property AA As String
        Public Property NT As String
        Public Property OtherDBs As KeyValuePair()

        Public Overrides Function ToString() As String
            Return Definition.ToString
        End Function

    End Class
End Namespace