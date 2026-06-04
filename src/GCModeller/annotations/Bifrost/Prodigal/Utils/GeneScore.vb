Imports SMRUCC.genomics.ComponentModel.Loci
Imports std = System.Math

''' <summary>
''' gene prediction score table
''' </summary>
Public Class GeneScore

    Public Property seq_id As String
    Public Property gene_index As Integer
    Public Property start As Integer
    Public Property [end] As Integer
    Public Property strand As Strands
    Public Property frame As Integer
    Public Property start_codon As String
    Public Property stop_codon As String
    Public Property rbs_motif As String
    Public Property total_score As Double
    Public Property coding_score As Double
    Public Property start_score As Double
    Public Property rbs_score As Double
    Public Property type_score As Double
    Public Property upstream_score As Double
    Public Property rbs_spacing As Integer
    Public Property partial_type As String

    Public ReadOnly Property length As Integer
        Get
            Return std.Abs(start - [end])
        End Get
    End Property

    Public Shared Iterator Function ScoreTable(results As IReadOnlyCollection(Of PredictionResult)) As IEnumerable(Of GeneScore)
        For Each result As PredictionResult In results
            For Each gene As PredictedGene In result.Genes
                Yield New GeneScore With {
                    .seq_id = result.SeqId,
                    .gene_index = gene.GeneIndex,
                    .start = gene.Start,
                    .[end] = gene.End,
                    .strand = gene.Strand.GetStrands,
                    .frame = gene.Frame + 1,
                    .start_codon = gene.StartCodon,
                    .stop_codon = gene.StopCodon,
                    .rbs_motif = gene.RbsMotif,
                    .total_score = gene.TotalScore,
                    .coding_score = gene.CodingScore,
                    .start_score = gene.StartScore,
                    .rbs_score = gene.RbsScore,
                    .type_score = gene.TypeScore,
                    .upstream_score = gene.UpstreamScore,
                    .rbs_spacing = gene.RbsSpacing,
                    .partial_type = gene.PartialType
                }
            Next
        Next
    End Function

End Class
