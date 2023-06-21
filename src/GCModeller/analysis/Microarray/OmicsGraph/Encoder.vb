Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors.Scaler
Imports Microsoft.VisualBasic.Math.Correlations
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Quantile
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module Encoder

    ''' <summary>
    ''' Translate the expression data to the global ranking data
    ''' </summary>
    ''' <param name="mat"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' the input matrix should be in format of samples 
    ''' in column and molecule features in rows.
    ''' </remarks>
    <Extension>
    Public Function EncodeRanking(mat As Matrix) As Matrix
        Dim z As Vector
        Dim q As Double

        Call VBDebugger.WriteLine("encode expression matrix with global ranking...")

        For Each gene As DataFrameRow In mat.expression
            z = New Vector(gene.experiments).Z
            z(z < 0) = Vector.Zero
            q = z.FindThreshold(0.8)
            z(z > q) = Vector.Scalar(q)
            gene.experiments = z.Ranking(Strategies.FractionalRanking, desc:=True)
        Next

        Return mat
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mat"></param>
    ''' <param name="charSet"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' the input matrix should be in format of samples 
    ''' in column and molecule features in rows.
    ''' </remarks>
    <Extension>
    Public Function EncodeMatrix(mat As Matrix,
                                 Optional charSet As String = "ATGC",
                                 Optional quantile_encoder As Boolean = True) As Dictionary(Of String, Char)

        ' total sum normalize at first, for each sample
        Dim features As String() = mat.rownames
        Dim samples As String() = mat.sampleID
        Dim norms As New List(Of Double())

        Call VBDebugger.EchoLine("do matrix normalization...")

        For Each sample As String In samples
            Dim v As Vector = mat.sample(sample)
            v = v / v.Sum
            norms.Add(v.ToArray)
        Next

        ' evaluate average ranking
        Dim ranking As Double() = New Double(features.Length - 1) {}
        Dim index As Integer

        Call VBDebugger.EchoLine("evaluate average ranking...")

        For i As Integer = 0 To ranking.Length - 1
            index = i
            ranking(i) = Aggregate sample As Double()
                         In norms
                         Into Average(sample(index))
        Next

        ' z-score standard
        ' Dim z As Vector = New Vector(ranking).Z

        Call VBDebugger.EchoLine("make charSet encoder...")

        Return charSet.encodeCharSet(ranking, features, quantile_encoder)
    End Function

    <Extension>
    Private Function encodeCharSet(charSet As String,
                                   ranking As Double(),
                                   features As String(),
                                   quantile_encoder As Boolean) As Dictionary(Of String, Char)
        Dim q As Double()

        If quantile_encoder Then
            q = ranking.QuantileLevels(steps:=1 / charSet.Length).AsVector * charSet.Length
        Else
            q = ranking.Ranking(Strategies.OrdinalRanking)
            q = (New Vector(q) / q.Max) * charSet.Length
        End If

        Dim chars As Char() = q _
            .Select(Function(i)
                        Dim index As Integer = CInt(i)
                        If index >= charSet.Length Then
                            index = charSet.Length - 1
                        End If
                        Return charSet(index)
                    End Function) _
            .ToArray
        Dim map As New Dictionary(Of String, Char)

        For i As Integer = 0 To features.Length - 1
            Call map.Add(features(i), chars(i))
        Next

        Return map
    End Function

    <Extension>
    Public Iterator Function AsSequenceSet(mat As Matrix, encodes As Dictionary(Of String, Char)) As IEnumerable(Of FastaSeq)
        Dim features As String() = mat.rownames

        Call VBDebugger.WriteLine("encode the expression matrix as sequence pack!")

        For Each sample As String In mat.sampleID
            Dim v As Vector = mat.sample(sample)
            Dim tag = v.Zip(features, Function(x, y) (x, y)) _
                .Where(Function(t) t.x > 0) _
                .OrderBy(Function(a) a.x) _
                .ToArray
            Dim seq As New String(tag.Select(Function(ti) encodes(ti.y)).ToArray)

            Yield New FastaSeq With {
                .Headers = {sample},
                .SequenceData = seq
            }
        Next
    End Function
End Module
