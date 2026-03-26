
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.Metagenomics
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports ncbi_tax = SMRUCC.genomics.Metagenomics.Taxonomy

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
            Dim lineage As TaxonomyNode() = taxonomyTree.GetAscendantsWithRanksAndNames(taxon.Key, only_std_ranks:=True)
            Dim tree As New Metagenomics.Taxonomy(lineage)

            If lineage.IsNullOrEmpty Then
                tree = New Metagenomics.Taxonomy({New TaxonomyNode(0, "Unknown")})
            End If

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

    <Extension>
    Public Iterator Function TaxonomyAssignment(Of T As ITaxonomy)(otus As IEnumerable(Of T), taxonomyTree As NcbiTaxonomyTree) As IEnumerable(Of T)
        If otus Is Nothing Then
            Return
        End If

        For Each otu As T In otus
            ' just addthe ncbi taxonomy tree lineage information at here 
            Dim node As TaxonomyNode() = taxonomyTree.GetAscendantsWithRanksAndNames(CInt(otu.ncbi_taxid), only_std_ranks:=True)

            If node Is Nothing Then
                otu.taxonomy_string = "Unknown"
            Else
                otu.taxonomy_string = New SMRUCC.genomics.Metagenomics.Taxonomy(node).ToString(BIOMstyle:=True)
            End If

            Yield otu
        Next
    End Function

    <Extension>
    Public Iterator Function AssignRepresentativeSeqID(otutable As IEnumerable(Of OTUTable), rep As IEnumerable(Of FastaSeq)) As IEnumerable(Of OTUTable)
        Dim repIndex As Dictionary(Of String, String) = rep _
            .ToDictionary(Function(fa)
                              ' fasta title is the ASV ID
                              Return fa.Title
                          End Function,
                          Function(fa)
                              Return NucleicAcid.Canonical(fa.SequenceData).ToUpper.MD5
                          End Function)

        For Each otu As OTUTable In otutable
            otu = New OTUTable With {
                .ID = repIndex(otu.ID),
                .Properties = otu.Properties,
                .taxonomy = otu.taxonomy
            }

            Yield otu
        Next
    End Function

    ''' <summary>
    ''' A helper function for merge otu table across two batch data
    ''' </summary>
    ''' <param name="batch1">otutable, otu id should be re-generated via the md5 of rep.fasta by <see cref="AssignRepresentativeSeqID"/> function.</param>
    ''' <param name="batch2">otutable, otu id should be re-generated via the md5 of rep.fasta by <see cref="AssignRepresentativeSeqID"/> function.</param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function MergePhyloseq(batch1 As IEnumerable(Of OTUTable), batch2 As IEnumerable(Of OTUTable)) As IEnumerable(Of OTUTable)
        For Each otu As IGrouping(Of String, OTUTable) In batch1 _
            .JoinIterates(batch2) _
            .GroupBy(Function(otu_seq)
                         ' direct merge two batch otu table via unique md5 representive sequence ID
                         Return otu_seq.ID
                     End Function)

            Dim taxGroup As IGrouping(Of String, OTUTable)() = otu _
                .GroupBy(Function(seq)
                             Return seq.taxonomy.BIOMTaxonomyString
                         End Function) _
                .ToArray
            Dim mergeSamples As New Dictionary(Of String, Double)

            For Each batch As OTUTable In otu
                For Each sample As KeyValuePair(Of String, Double) In batch.Properties
                    If mergeSamples.ContainsKey(sample.Key) Then
                        mergeSamples(sample.Key) += sample.Value
                    Else
                        mergeSamples.Add(sample.Key, sample.Value)
                    End If
                Next
            Next

            Dim finalTax As ncbi_tax

            If taxGroup.Length > 1 Then
                ' has conflict sequence annotation result
                ' use the longest taxonomy lineage
                finalTax = taxGroup _
                    .OrderByDescending(Function(tax)
                                           Return BIOMTaxonomy.TaxonomyParser(tax.Key).AsTaxonomy.RankLevel
                                       End Function) _
                    .ThenBy(Function(tax) tax.Key) _
                    .First _
                    .First _
                    .taxonomy
            Else
                finalTax = taxGroup.First.First.taxonomy
            End If

            Yield New OTUTable With {
                .ID = otu.Key,
                .Properties = mergeSamples,
                .taxonomy = finalTax
            }
        Next
    End Function

End Module