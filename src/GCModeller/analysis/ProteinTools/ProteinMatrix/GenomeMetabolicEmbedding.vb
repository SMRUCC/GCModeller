Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.NLP
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline

Public Class GenomeMetabolicEmbedding

    ReadOnly vec As New TFIDF

    Public Sub Add(genome As GenomeVector)
        Call vec.Add(genome.taxonomy, genome.terms)
    End Sub

    Public Function AddGenomes(seqs As IEnumerable(Of GenomeVector)) As GenomeMetabolicEmbedding
        For Each annotation As GenomeVector In seqs
            Call Add(annotation)
        Next

        Return Me
    End Function

    Public Function TfidfVectorizer(Optional normalize As Boolean = False) As DataFrame
        Call $"Make metabolic embedding with: ".info
        Call $"  * {vec.N} genomes".debug
        Call $"  * {vec.Words.Length} total enzyme terms".debug
        Call VBDebugger.EchoLine("")

        Return vec.TfidfVectorizer(normalize)
    End Function

    ''' <summary>
    ''' n-gram One-hot(Bag-of-n-grams)
    ''' </summary>
    ''' <returns></returns>
    Public Function OneHotVectorizer() As DataFrame
        Return vec.OneHotVectorizer
    End Function
End Class
