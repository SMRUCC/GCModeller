Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' The data model of the genes in the human genome.(人类基因组之中的基因模型)    
    ''' </summary>
    <XmlRoot("HumanGene")> Public Class Hsa_gene : Implements INamedValue

        Public Property Entry As String Implements IKeyedEntity(Of String).Key
        Public Property GeneName As String
        Public Property Definition As KeyValuePair
        Public Property Pathway As KeyValuePair()
        Public Property Disease As KeyValuePair()
        Public Property DrugTarget As KeyValuePair()
        Public Property Motif As NamedCollection(Of String)
        Public Property [Structure] As NamedCollection(Of String)
        Public Property Position As String
        Public Property AA As String
        Public Property NT As String
        Public Property OtherDBs As KeyValuePair()
        Public Property Modules As KeyValuePair()

        Public Overrides Function ToString() As String
            Return Definition.ToString
        End Function
    End Class
End Namespace