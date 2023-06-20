Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' k-mers
''' </summary>
Public Class KMers

    Public Property Tag As String
    ''' <summary>
    ''' count current k-mers <see cref="Tag"/> on a given sequence
    ''' </summary>
    ''' <returns></returns>
    Public Property Count As Integer

    Public ReadOnly Property Unique As Boolean
        Get
            Return Count = 1
        End Get
    End Property

    Public ReadOnly Property Size As Integer
        Get
            Return Tag.Length
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return Tag
    End Function

    Public Shared Iterator Function Create(seq As ISequenceProvider,
                                           Optional k As Integer = 3,
                                           Optional charSet As IReadOnlyCollection(Of Char) = Nothing) As IEnumerable(Of KMers)

        Dim seq_s As String = seq.GetSequenceData

        If charSet Is Nothing OrElse charSet.Count = 0 Then
            charSet = seq_s.Distinct.ToArray
        End If

        For Each kmer As KMers In Create(k, charSet)
            kmer.Count = seq_s.Count(kmer.Tag)
            Yield kmer
        Next
    End Function

    Public Shared Iterator Function Create(k As Integer, charSet As IReadOnlyCollection(Of Char)) As IEnumerable(Of KMers)
        Dim combines As String() = charSet.Select(Function(ci) ci.ToString).ToArray

        For i As Integer = 1 To k
            combines = CombinationExtensions _
                .CreateCombos(combines, charSet) _
                .Select(Function(t) t.a & t.b.ToString) _
                .ToArray
        Next

        For Each tag As String In combines
            Yield New KMers With {.Tag = tag}
        Next
    End Function
End Class
