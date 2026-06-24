' ============================================================================
'  PhenotypeModel.vb - Data model for one trained phenotype classifier
'  Traitar Microbial Trait Analyzer - VB.NET Implementation
'
'  A phenotype model is the "voting committee" of L1-L2 linear SVMs trained
'  at different regularization strengths C. For each C the model stores:
'    * the bias term b
'    * the sparse weight vector w (Pfam accession -> weight)
'    * the per-feature Pearson correlation with the phenotype label
'
'  File layout produced by the original Traitar (and consumed here):
'    {pid}_bias.txt             C<TAB>bias        (one model per line, 13 C values)
'    {pid}_feats.txt            Pfam<TAB>w_C1<TAB>w_C2 ...   (full weight matrix)
'    {pid}_non-zero+weights.txt Pfam<TAB>class<TAB>w_C1 ... <TAB>desc<TAB>cor
' ============================================================================
Imports System.Collections.Generic

Namespace Models

    ''' <summary>
    ''' One SVM sub-model inside the voting committee (one C value).
    ''' </summary>
    Public Class SubModel
        Public Property C As Double
        Public Property Bias As Double
        ' Sparse weight vector: Pfam accession -> weight
        Public Property Weights As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
        ' Whether this sub-model is "active" (bias != 0 means features were selected)
        Public ReadOnly Property IsActive As Boolean
            Get
                Return Bias <> 0.0R OrElse Weights.Count > 0
            End Get
        End Property

        ''' <summary>
        ''' SVM decision function: f(x) = w . x + b.
        ''' Predicts positive (1) when f(x) &gt; 0, negative (0) otherwise.
        ''' </summary>
        Public Function Decision(sample As GenomeSample) As Double
            Dim score As Double = Bias
            For Each kv As KeyValuePair(Of String, Double) In Weights
                If kv.Value <> 0.0R Then
                    score += kv.Value * sample.FeatureValue(kv.Key)
                End If
            Next
            Return score
        End Function

        Public Function Predict(sample As GenomeSample) As Integer
            Return If(Decision(sample) > 0.0R, 1, 0)
        End Function
    End Class

    ''' <summary>
    ''' A Pfam feature that survived feature selection for this phenotype,
    ''' carrying its description and Pearson correlation with the label.
    ''' </summary>
    Public Class PhenotypeFeature
        Public Property PfamAcc As String = ""
        Public Property Sign As String = "+"        ' "+" positive class, "-" negative class
        ''' <summary>Alias for Sign (the +/- class column in non-zero+weights).</summary>
        Public Property WeightClass As String
            Get
                Return Sign
            End Get
            Set(value As String)
                Sign = value
            End Set
        End Property
        Public Property Description As String = ""
        Public Property PearsonCorrelation As Double
        ' Per-C weights (only stored for the active sub-models)
        Public Property WeightsByC As New Dictionary(Of Double, Double)()
    End Class

    ''' <summary>
    ''' A complete phenotype model = voting committee of SubModels + feature table.
    ''' </summary>
    Public Class PhenotypeModel

        Public Property PhenotypeId As String = ""
        Public Property PhenotypeName As String = ""
        Public Property Category As String = ""

        ' All 13 sub-models keyed by C value (as stored in the bias file)
        Public Property SubModels As New List(Of SubModel)()

        ' Non-zero features with descriptions and Pearson correlations
        Public Property Features As New List(Of PhenotypeFeature)()

        ' Voting committee size (Traitar default = 5)
        Public Property CommitteeSize As Integer = 5

        ''' <summary>
        ''' Returns the active sub-models in the order they appear in the bias file.
        ''' The original Traitar sorts the bias file by cross-validation accuracy
        ''' (best first), so the first CommitteeSize active entries form the
        ''' voting committee.
        ''' </summary>
        Public Function GetVotingCommittee() As List(Of SubModel)
            Dim active As New List(Of SubModel)
            For Each sm As SubModel In SubModels
                If sm.IsActive Then
                    active.Add(sm)
                    If active.Count >= CommitteeSize Then Exit For
                End If
            Next
            Return active
        End Function

        ''' <summary>
        ''' Majority-vote prediction: positive if at least ceil(N/2) of the
        ''' committee members predict positive. With N=5 this means &gt;=3.
        ''' </summary>
        Public Function Predict(sample As GenomeSample) As Integer
            Dim committee As List(Of SubModel) = GetVotingCommittee()
            If committee.Count = 0 Then Return 0
            Dim positiveVotes As Integer = 0
            For Each sm As SubModel In committee
                If sm.Predict(sample) = 1 Then positiveVotes += 1
            Next
            Dim threshold As Integer = CInt(Math.Ceiling(committee.Count / 2.0R))
            If committee.Count Mod 2 = 0 Then threshold += 1   ' strict majority for even N
            Return If(positiveVotes >= threshold, 1, 0)
        End Function

        ''' <summary>
        ''' Average decision score across the committee (used as a confidence).
        ''' </summary>
        Public Function DecisionScore(sample As GenomeSample) As Double
            Dim committee As List(Of SubModel) = GetVotingCommittee()
            If committee.Count = 0 Then Return 0.0R
            Dim sum As Double = 0.0R
            For Each sm As SubModel In committee
                sum += sm.Decision(sample)
            Next
            Return sum / committee.Count
        End Function

        ''' <summary>
        ''' Feature selection (Module 7): a Pfam is "key" for this phenotype if
        ''' it carries a positive weight in at least ceil(2N/3) of the committee
        ''' models. With N=5 this means &gt;=3 models. Returns the selected
        ''' features sorted by descending Pearson correlation.
        ''' </summary>
        Public Function SelectKeyFeatures() As List(Of PhenotypeFeature)
            Dim committee As List(Of SubModel) = GetVotingCommittee()
            If committee.Count = 0 Then Return New List(Of PhenotypeFeature)()
            Dim minVotes As Integer = CInt(Math.Ceiling(committee.Count * 2.0R / 3.0R))

            ' Count, per Pfam, how many committee members give it a positive weight
            Dim positiveVotes As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
            For Each sm As SubModel In committee
                For Each kv As KeyValuePair(Of String, Double) In sm.Weights
                    If kv.Value > 0.0R Then
                        If positiveVotes.ContainsKey(kv.Key) Then
                            positiveVotes(kv.Key) += 1
                        Else
                            positiveVotes(kv.Key) = 1
                        End If
                    End If
                Next
            Next

            ' Pick features that pass the vote threshold, then sort by Pearson correlation
            Dim selected As New List(Of PhenotypeFeature)()
            For Each f As PhenotypeFeature In Features
                Dim votes As Integer = 0
                If positiveVotes.ContainsKey(f.PfamAcc) Then votes = positiveVotes(f.PfamAcc)
                If votes >= minVotes Then
                    selected.Add(f)
                End If
            Next
            selected.Sort(Function(a, b) b.PearsonCorrelation.CompareTo(a.PearsonCorrelation))
            Return selected
        End Function

        ''' <summary>Alias for SelectKeyFeatures (used by the main pipeline).</summary>
        Public Function GetKeyFeatures() As List(Of PhenotypeFeature)
            Return SelectKeyFeatures()
        End Function

    End Class

End Namespace
