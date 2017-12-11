Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports SMRUCC.genomics.Metagenomics

Namespace greengenes

    Public Class otu_taxonomy : Implements INamedValue

        Public Property ID As String Implements IKeyedEntity(Of String).Key
        Public Property Taxonomy As Taxonomy

        Public Overrides Function ToString() As String
            Return Taxonomy.CreateTable.Value.TaxonomyString
        End Function

        Public Shared Iterator Function Load(path As String) As IEnumerable(Of otu_taxonomy)
            For Each line As String In path.IterateAllLines
                Dim data = line.GetTagValue(vbTab, trim:=True)
                Dim taxonomy As New Taxonomy(BIOMTaxonomy.TaxonomyParser(data.Value))

                Yield New otu_taxonomy With {
                    .ID = data.Name,
                    .Taxonomy = taxonomy
                }
            Next
        End Function
    End Class
End Namespace