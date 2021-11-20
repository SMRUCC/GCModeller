Imports Microsoft.VisualBasic.Data.NLP.LDA

Public Class STCorpus

    Public Property geneCount As New Corpus

    Public ReadOnly Property pixels As String()
        Get
            Return m_pixels.ToArray
        End Get
    End Property

    Public ReadOnly Property Document As Integer()()
        Get
            Return geneCount.Document
        End Get
    End Property

    Public ReadOnly Property VocabularySize As Integer
        Get
            Return geneCount.VocabularySize
        End Get
    End Property

    Public ReadOnly Property Vocabulary As Vocabulary
        Get
            Return geneCount.Vocabulary
        End Get
    End Property

    Friend ReadOnly m_pixels As New List(Of String)

    Public Sub addPixel(pixel As String, sample As IEnumerable(Of String))
        Call m_pixels.Add(pixel)
        Call geneCount.addDocument(sample)
    End Sub

End Class
