Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.NLP
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline

Public Class GenomeMetabolicEmbedding

    ReadOnly vec As New TFIDF
    ReadOnly taxonomy As New Dictionary(Of String, String)

    Public Sub Add(genome As GenomeVector)
        Call vec.Add(genome.assembly_id, genome.terms)
        Call taxonomy.Add(genome.assembly_id, genome.taxonomy)
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

        Dim df As DataFrame = vec.TfidfVectorizer(normalize)
        Call df.add("taxonomy", From id As String In df.rownames Select taxonomy(id))
        Return df
    End Function

    ''' <summary>
    ''' n-gram One-hot(Bag-of-n-grams)
    ''' </summary>
    ''' <returns></returns>
    Public Function OneHotVectorizer() As DataFrame
        Return vec.OneHotVectorizer
    End Function
End Class
