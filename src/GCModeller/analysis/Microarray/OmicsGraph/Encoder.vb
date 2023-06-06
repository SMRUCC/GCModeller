Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Quantile
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module Encoder

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mat"></param>
    ''' <param name="charSet"></param>
    ''' <returns></returns>
    <Extension>
    Public Function EncodeMatrix(mat As Matrix, Optional charSet As String = "ATGC") As Dictionary(Of String, Char)
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
        Dim z As Vector = New Vector(ranking).Z

        Call VBDebugger.EchoLine("make charSet encoder...")

        If charSet.Length = 4 Then
            Return charSet.encodeRange4(z, features)
        Else
            Return charSet.encodeRange20(z, features)
        End If
    End Function

    <Extension>
    Private Function encodeRange20(charSet As String, z As Vector, features As String()) As Dictionary(Of String, Char)
        Dim neg = z.Where(Function(zi) zi < 0).GKQuantile
        Dim pos = z.Where(Function(zi) zi >= 0).GKQuantile
        Dim rangeNeg10 As New List(Of DoubleRange)
        Dim rangePos10 As New List(Of DoubleRange)

        For d As Double = 0.0 To 1.0 Step 0.1
            Call rangeNeg10.Add(New DoubleRange(neg.Query(d), neg.Query(d + 0.1)))
            Call rangePos10.Add(New DoubleRange(pos.Query(d), pos.Query(d + 0.1)))
        Next

        Dim encodes As New Dictionary(Of String, Char)

        For i As Integer = 0 To features.Length - 1
            Dim x As Double = z.Item(i)
            Dim index As Integer = 0

            For index = 0 To rangeNeg10.Count - 1
                If rangeNeg10(index).IsInside(x) Then
                    Call encodes.Add(features(i), charSet(index))
                    Exit For
                ElseIf rangePos10(index).IsInside(x) Then
                    Call encodes.Add(features(i), charSet(10 + index))
                    Exit For
                End If
            Next

            If encodes.Count < i + 1 Then
                encodes.Add(features(i), "-")
            End If
        Next

        Return encodes
    End Function

    <Extension>
    Private Function encodeRange4(charSet As String, z As Vector, features As String()) As Dictionary(Of String, Char)
        Dim range1 As New DoubleRange(z.Min, z.Min / 2)
        Dim range2 As New DoubleRange(z.Min / 2, 0)
        Dim range3 As New DoubleRange(0, z.Max / 2)
        Dim range4 As New DoubleRange(z.Max / 2, z.Max)

        Dim encodes As New Dictionary(Of String, Char)

        For i As Integer = 0 To features.Length - 1
            Dim x As Double = z.Item(i)

            If range1.IsInside(x) Then
                Call encodes.Add(features(i), charSet(0))
            ElseIf range2.IsInside(x) Then
                Call encodes.Add(features(i), charSet(1))
            ElseIf range3.IsInside(x) Then
                Call encodes.Add(features(i), charSet(2))
            Else
                Call encodes.Add(features(i), charSet(3))
            End If
        Next

        Return encodes
    End Function

    <Extension>
    Public Iterator Function AsSequenceSet(mat As Matrix, encodes As Dictionary(Of String, Char)) As IEnumerable(Of FastaSeq)
        Dim features As String() = mat.rownames

        For Each sample As String In mat.sampleID
            Dim v As Vector = mat.sample(sample)
            Dim tag = v.Zip(features, Function(x, y) (x, y)).OrderBy(Function(a) a.x).ToArray
            Dim seq As New String(tag.Select(Function(ti) encodes(ti.y)).ToArray)

            Yield New FastaSeq With {
                .Headers = {sample},
                .SequenceData = seq
            }
        Next
    End Function
End Module
