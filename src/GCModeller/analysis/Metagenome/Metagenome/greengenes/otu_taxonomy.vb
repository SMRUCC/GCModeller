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

        Public Shared Function Load(path As String) As IEnumerable(Of otu_taxonomy)
            Return path _
                .IterateAllLines _
                .AsParallel _
                .Select(AddressOf Parser)
        End Function

        Private Shared Function Parser(line As String) As otu_taxonomy
            Dim data = line.GetTagValue(vbTab, trim:=True)
            Dim table = BIOMTaxonomy.TaxonomyParser(data.Value)
            Dim taxonomy As New Taxonomy(table)

            Return New otu_taxonomy With {
                .ID = data.Name,
                .Taxonomy = taxonomy
            }
        End Function
    End Class
End Namespace