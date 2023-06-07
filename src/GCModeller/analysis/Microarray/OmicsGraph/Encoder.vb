Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging
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
        ' Dim z As Vector = New Vector(ranking).Z

        Call VBDebugger.EchoLine("make charSet encoder...")

        Return charSet.encodeCharSet(ranking, features)
    End Function

    <Extension>
    Private Function encodeCharSet(charSet As String, ranking As Double(), features As String()) As Dictionary(Of String, Char)
        Dim d As Double = 1 / charSet.Length
        Dim q As Double() = ranking.QuantileLevels(steps:=d).AsVector * charSet.Length
        Dim chars As Char() = q _
            .Select(Function(i) charSet(CInt(i))) _
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
            Dim tag = v.Zip(features, Function(x, y) (x, y)).OrderBy(Function(a) a.x).ToArray
            Dim seq As New String(tag.Select(Function(ti) encodes(ti.y)).ToArray)

            Yield New FastaSeq With {
                .Headers = {sample},
                .SequenceData = seq
            }
        Next
    End Function
End Module
