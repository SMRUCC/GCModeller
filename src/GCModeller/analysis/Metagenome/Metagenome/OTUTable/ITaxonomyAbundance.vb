Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.ComponentModel

Public Interface ITaxonomyAbundance : Inherits IExpressionValue

    Property ncbi_taxid As UInteger

End Interface

Public Module OTUTableBuilder

    <Extension>
    Public Function IsMissing(tax As Metagenomics.Taxonomy) As Boolean
        If tax Is Nothing Then
            Return True
        Else
            Return tax.ToArray.SafeQuery.All(Function(si) Strings.Trim(si).StringEmpty(, True))
        End If
    End Function

    <Extension>
    Public Iterator Function MakeOUTTable(Of T As ITaxonomyAbundance)(samples As IEnumerable(Of NamedCollection(Of T)), taxonomyTree As NcbiTaxonomyTree) As IEnumerable(Of OTUTable)
        Dim otu_data As New List(Of (sample_id As String, T))

        For Each sample As NamedCollection(Of T) In samples.SafeQuery
            For Each taxon As T In sample
                Call otu_data.Add((sample.name, taxon))
            Next
        Next

        Dim taxon_groups = otu_data _
            .GroupBy(Function(ti) ti.Item2.ncbi_taxid) _
            .ToArray

        For Each taxon As IGrouping(Of UInteger, (sample_id$, taxon_data As T)) In taxon_groups
            Dim abundance As New Dictionary(Of String, Double)
            Dim lineage = taxonomyTree.GetAscendantsWithRanksAndNames(taxon.Key, only_std_ranks:=True)
            Dim tree As New Metagenomics.Taxonomy(lineage)

            For Each sample As (sample_id$, taxon_data As T) In taxon
                If abundance.ContainsKey(sample.sample_id) Then
                    abundance(sample.sample_id) += sample.taxon_data.ExpressionValue
                Else
                    abundance.Add(sample.sample_id, sample.taxon_data.ExpressionValue)
                End If
            Next

            Yield New OTUTable With {
                .ID = taxon.First.taxon_data.Identity,
                .Properties = abundance,
                .taxonomy = tree
            }
        Next
    End Function

End Module