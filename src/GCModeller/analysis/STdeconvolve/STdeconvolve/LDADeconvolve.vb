Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.NLP.LDA
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

''' <summary>
''' do spatial data deconvolve via LDA algorithm
''' </summary>
Public Module LDADeconvolve

    ''' <summary>
    ''' Create document vector for run LDA mdelling
    ''' </summary>
    ''' <param name="matrix">
    ''' row is pixels and column is gene features. each 
    ''' pixel row is a document sample in LDA model
    ''' </param>
    ''' <param name="min"></param>
    ''' <param name="max"></param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateSpatialDocuments(matrix As Matrix,
                                           Optional min As Double = 0.05,
                                           Optional max As Double = 0.95) As Corpus

        Dim filter As Index(Of String) = matrix.GeneFilter(min, max)
        Dim sample As New Corpus

        For Each pixel As DataFrameRow In matrix.expression

        Next

        Return sample
    End Function

    <Extension>
    Private Function GeneFilter(matrix As Matrix, pmin As Double, pmax As Double) As Index(Of String)
        Dim geneIds As New List(Of String)

        Return geneIds.Indexing
    End Function

    <Extension>
    Public Function LDAModelling(spatialDoc As Corpus) As LdaGibbsSampler
        ' 2. Create a LDA sampler
        Dim ldaGibbsSampler As New LdaGibbsSampler(spatialDoc.Document(), spatialDoc.VocabularySize())
        ' 3. Train it
        Call ldaGibbsSampler.gibbs(10)

        Return ldaGibbsSampler
    End Function

End Module
