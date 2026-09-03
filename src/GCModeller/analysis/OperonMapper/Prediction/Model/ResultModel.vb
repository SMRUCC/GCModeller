' ============================================================================
' ResultModel.vb — 结构化操纵子预测结果对象（JSON DTO，System.Text.Json）
' ============================================================================

Imports System.Text.Json.Serialization

Namespace OperonPredictor.Model

    Public Class OperonReport

        <JsonPropertyName("program")>
        Public Property Program As String

        <JsonPropertyName("version")>
        Public Property Version As String

        <JsonPropertyName("parameters")>
        Public Property Parameters As PredictionParameters

        <JsonPropertyName("summary")>
        Public Property Summary As PredictionSummary

        <JsonPropertyName("genes")>
        Public Property Genes As List(Of GeneDto)

        <JsonPropertyName("pairs")>
        Public Property Pairs As List(Of PairDto)

        <JsonPropertyName("operons")>
        Public Property Operons As List(Of OperonDto)

    End Class

    Public Class PredictionParameters

        <JsonPropertyName("num_sequences_contigs")>
        Public Property NumContigs As Integer

        <JsonPropertyName("weights")>
        Public Property Weights As Dictionary(Of String, Double)

        <JsonPropertyName("persistence")>
        Public Property Persistence As Double

        <JsonPropertyName("p_barcode_in")>
        Public Property PBarcodeIn As Double

        <JsonPropertyName("p_barcode_out")>
        Public Property PBarcodeOut As Double

        <JsonPropertyName("p_conserved_in")>
        Public Property PConservedIn As Double

        <JsonPropertyName("p_conserved_out")>
        Public Property PConservedOut As Double

        <JsonPropertyName("sequence_signals")>
        Public Property SequenceSignals As Boolean

        <JsonPropertyName("comparative_signals")>
        Public Property ComparativeSignals As Boolean

        <JsonPropertyName("num_reference_genomes")>
        Public Property NumReferenceGenomes As Integer

    End Class

    Public Class PredictionSummary

        <JsonPropertyName("num_genes")>
        Public Property NumGenes As Integer

        <JsonPropertyName("num_pairs")>
        Public Property NumPairs As Integer

        <JsonPropertyName("num_same_strand_pairs")>
        Public Property NumSameStrandPairs As Integer

        <JsonPropertyName("num_opposite_pairs")>
        Public Property NumOppositePairs As Integer

        <JsonPropertyName("uniop_prior_q")>
        Public Property UniopPriorQ As Double

        <JsonPropertyName("num_operons")>
        Public Property NumOperons As Integer

        <JsonPropertyName("num_multi_gene_operons")>
        Public Property NumMultiGeneOperons As Integer

        <JsonPropertyName("mean_operon_size")>
        Public Property MeanOperonSize As Double

    End Class

    Public Class GeneDto

        <JsonPropertyName("id")>
        Public Property Id As String

        <JsonPropertyName("contig")>
        Public Property Contig As String

        <JsonPropertyName("start")>
        Public Property Start As Integer

        <JsonPropertyName("end")>
        Public Property [End] As Integer

        <JsonPropertyName("strand")>
        Public Property Strand As String

    End Class

    Public Class PairDto

        <JsonPropertyName("gene_a")>
        Public Property GeneA As String

        <JsonPropertyName("gene_b")>
        Public Property GeneB As String

        <JsonPropertyName("strand_pattern")>
        Public Property StrandPattern As String      ' same / convergent / divergent

        <JsonPropertyName("igd")>
        Public Property Igd As Integer

        <JsonPropertyName("scores")>
        Public Property Scores As ScoreDto

        <JsonPropertyName("same_operon")>
        Public Property SameOperon As Boolean

    End Class

    Public Class ScoreDto

        <JsonPropertyName("distance_uniop_posterior")>
        Public Property DistancePosterior As Double

        <JsonPropertyName("llr_distance")>
        Public Property LlrDistance As Double

        <JsonPropertyName("barcode_hamming")>
        Public Property BarcodeHamming As Integer

        <JsonPropertyName("barcode_refs")>
        Public Property BarcodeRefs As Integer

        <JsonPropertyName("llr_barcode")>
        Public Property LlrBarcode As Double

        <JsonPropertyName("conserved_pair_count")>
        Public Property ConservedCount As Integer

        <JsonPropertyName("llr_conserved")>
        Public Property LlrConserved As Double

        <JsonPropertyName("pcbbh_count")>
        Public Property PcbbhCount As Integer

        <JsonPropertyName("terminator_strength")>
        Public Property TerminatorStrength As Double

        <JsonPropertyName("llr_terminator")>
        Public Property LlrTerminator As Double

        <JsonPropertyName("promoter_strength")>
        Public Property PromoterStrength As Double

        <JsonPropertyName("llr_promoter")>
        Public Property LlrPromoter As Double

        <JsonPropertyName("functional_match")>
        Public Property FunctionalMatch As String        ' true/false/na

        <JsonPropertyName("llr_function")>
        Public Property LlrFunction As Double

        <JsonPropertyName("combined_llr")>
        Public Property CombinedLlr As Double

        <JsonPropertyName("combined_posterior")>
        Public Property CombinedPosterior As Double

        <JsonPropertyName("hmm_posterior")>
        Public Property HmmPosterior As Double

    End Class

    Public Class OperonDto

        <JsonPropertyName("operon_id")>
        Public Property OperonId As String

        <JsonPropertyName("contig")>
        Public Property Contig As String

        <JsonPropertyName("strand")>
        Public Property Strand As String

        <JsonPropertyName("start")>
        Public Property Start As Integer

        <JsonPropertyName("end")>
        Public Property [End] As Integer

        <JsonPropertyName("num_genes")>
        Public Property NumGenes As Integer

        <JsonPropertyName("genes")>
        Public Property Genes As List(Of String)

        <JsonPropertyName("gene_starts")>
        Public Property GeneStarts As List(Of Integer)

        <JsonPropertyName("gene_ends")>
        Public Property GeneEnds As List(Of Integer)

        <JsonPropertyName("mean_pair_posterior")>
        Public Property MeanPairPosterior As Double

    End Class

End Namespace
