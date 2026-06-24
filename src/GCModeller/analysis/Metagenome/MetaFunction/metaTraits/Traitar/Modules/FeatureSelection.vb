' ============================================================================
'  Module 7 - FeatureSelection.vb
'  Feature selection & association analysis
'
'  Identifies the Pfam families that most strongly drive each phenotype
'  prediction, providing biological interpretability.
'
'  Procedure (per phenotype, per the Traitar paper):
'    1. From the 5-member voting committee, collect all Pfam families that
'       carry a POSITIVE weight in at least 3 of the 5 models (majority).
'    2. For each such family, compute the Pearson correlation between its
'       binary presence/absence vector (across the training set) and the
'       phenotype label vector.
'    3. Sort families by |Pearson r| descending and emit as the ranked list
'       of "key features" for downstream experimental validation.
'
'  When loading pre-trained models, the Pearson correlations are already
'  stored in the {pid}_non-zero+weights.txt file (last column "cor"), so
'  step 2 is a lookup rather than a recomputation.
' ============================================================================

Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.Models
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.Utils

Namespace Modules

    Public Module FeatureSelection

        ''' <summary>
        ''' Compute the Pearson correlation between a binary feature column
        ''' and a binary label vector, over the training set.
        ''' </summary>
        Public Function FeatureLabelCorrelation(featureColumn As Double(),
                                                 labelColumn As Integer()) As Double
            Dim n As Integer = featureColumn.Length
            If n = 0 Then Return 0.0R
            Dim y(n - 1) As Double
            For i As Integer = 0 To n - 1
                y(i) = CDbl(labelColumn(i))
            Next
            Return MathUtils.PearsonCorrelation(featureColumn, y)
        End Function

        ''' <summary>
        ''' For a freshly trained committee (list of (C, w, b) sub-models),
        ''' compute the majority-vote feature set and rank by Pearson r.
        '''
        ''' Inputs:
        '''   committee  - the K voting-committee sub-models
        '''   X          - n x d training feature matrix
        '''   y          - n label vector
        '''   featureNames - d Pfam accessions (column names of X)
        '''   minVotes   - minimum number of committee members that must give
        '''                the feature a positive weight (default = ceil(2K/3))
        ''' </summary>
        Public Function SelectKeyFeatures(
                committee As List(Of Tuple(Of Double, Double(), Double)),
                X As Double(,), y As Integer(),
                featureNames As List(Of String),
                Optional minVotes As Integer = -1) _
                As List(Of Tuple(Of String, Double, Double))

            If minVotes < 0 Then
                minVotes = CInt(Math.Ceiling(committee.Count * 2.0R / 3.0R))
            End If

            ' Count positive-weight votes per feature
            Dim posVotes(featureNames.Count - 1) As Integer
            For Each sm As Tuple(Of Double, Double(), Double) In committee
                Dim w As Double() = sm.Item2
                For j As Integer = 0 To w.Length - 1
                    If w(j) > 0.0R Then posVotes(j) += 1
                Next
            Next

            ' Extract the column for each surviving feature and compute Pearson r
            Dim result As New List(Of Tuple(Of String, Double, Double))()
            Dim n As Integer = X.GetLength(0)
            For j As Integer = 0 To featureNames.Count - 1
                If posVotes(j) < minVotes Then Continue For
                Dim col(n - 1) As Double
                For i As Integer = 0 To n - 1
                    col(i) = X(i, j)
                Next
                Dim r As Double = FeatureLabelCorrelation(col, y)
                result.Add(Tuple.Create(featureNames(j), r, CDbl(posVotes(j))))
            Next

            ' Sort by |r| descending
            result.Sort(Function(a, b) Math.Abs(b.Item2).CompareTo(Math.Abs(a.Item2)))
            Return result
        End Function

        ''' <summary>
        ''' Convenience: extract the ranked key features from a loaded
        ''' PhenotypeModel (which already stores Pearson r in the
        ''' non-zero+weights file).
        ''' </summary>
        Public Function ExtractFromModel(model As PhenotypeModel) As List(Of PhenotypeFeature)
            Return model.GetKeyFeatures()
        End Function

    End Module

End Namespace
