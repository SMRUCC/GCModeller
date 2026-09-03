' ============================================================================
' ResultModel.vb — 结构化 motif 发现结果对象（JSON DTO，System.Text.Json）
' ============================================================================

Imports System.Text.Json.Serialization

Namespace EmMotif.Model

    Public Class MotifReport

        <JsonPropertyName("program")>
        Public Property Program As String

        <JsonPropertyName("version")>
        Public Property Version As String

        <JsonPropertyName("alphabet")>
        Public Property Alphabet As String

        <JsonPropertyName("parameters")>
        Public Property Parameters As MotifParameters

        <JsonPropertyName("sequences")>
        Public Property Sequences As List(Of SeqSummary)

        <JsonPropertyName("background_frequencies")>
        Public Property BackgroundFrequencies As Dictionary(Of String, Double)

        <JsonPropertyName("motifs")>
        Public Property Motifs As List(Of MotifDto)

    End Class

    Public Class MotifParameters

        <JsonPropertyName("model")>
        Public Property Model As String

        <JsonPropertyName("min_width")>
        Public Property MinWidth As Integer

        <JsonPropertyName("max_width")>
        Public Property MaxWidth As Integer

        <JsonPropertyName("num_motifs")>
        Public Property NumMotifs As Integer

        <JsonPropertyName("revcomp")>
        Public Property Revcomp As Boolean

        <JsonPropertyName("seed_strategy")>
        Public Property SeedStrategy As String

        <JsonPropertyName("seed_count")>
        Public Property SeedCount As Integer

        <JsonPropertyName("pseudocount")>
        Public Property Pseudocount As Double

        <JsonPropertyName("max_iterations")>
        Public Property MaxIterations As Integer

        <JsonPropertyName("epsilon")>
        Public Property Epsilon As Double

        <JsonPropertyName("evalue_max")>
        Public Property EvalueMax As Double

        <JsonPropertyName("rng_seed")>
        Public Property RngSeed As Integer

        <JsonPropertyName("num_sequences")>
        Public Property NumSequences As Integer

    End Class

    Public Class SeqSummary

        <JsonPropertyName("id")>
        Public Property Id As String

        <JsonPropertyName("length")>
        Public Property Length As Integer

        <JsonPropertyName("ambiguous_positions")>
        Public Property AmbiguousPositions As Integer

    End Class

    Public Class MotifDto

        <JsonPropertyName("id")>
        Public Property Id As String

        <JsonPropertyName("width")>
        Public Property Width As Integer

        <JsonPropertyName("model")>
        Public Property Model As String

        <JsonPropertyName("consensus")>
        Public Property Consensus As String

        <JsonPropertyName("lambda")>
        Public Property Lambda As Double

        <JsonPropertyName("log_likelihood")>
        Public Property LogLikelihood As Double

        <JsonPropertyName("log_likelihood_ratio")>
        Public Property LogLikelihoodRatio As Double

        <JsonPropertyName("evalue")>
        Public Property Evalue As Double

        <JsonPropertyName("iterations")>
        Public Property Iterations As Integer

        <JsonPropertyName("converged")>
        Public Property Converged As Boolean

        <JsonPropertyName("letters")>
        Public Property Letters As String

        <JsonPropertyName("pwm")>
        Public Property Pwm As Dictionary(Of String, Double())

        <JsonPropertyName("background")>
        Public Property Background As Dictionary(Of String, Double)

        <JsonPropertyName("sites")>
        Public Property Sites As List(Of SiteDto)

        <JsonPropertyName("log_likelihood_trace")>
        Public Property LogLikTrace As List(Of Double)

    End Class

    Public Class SiteDto

        <JsonPropertyName("sequence")>
        Public Property Sequence As String

        <JsonPropertyName("start")>
        Public Property Start As Integer

        <JsonPropertyName("strand")>
        Public Property Strand As String

        <JsonPropertyName("posterior")>
        Public Property Posterior As Double

        <JsonPropertyName("log_likelihood_ratio")>
        Public Property WindowLogR As Double

        <JsonPropertyName("segment")>
        Public Property Segment As String

    End Class

End Namespace
