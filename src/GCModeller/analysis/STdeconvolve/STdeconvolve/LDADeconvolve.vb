Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.NLP.LDA
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.LinearAlgebra
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
    ''' <returns>
    ''' document model for run LDA modelling
    ''' </returns>
    <Extension>
    Public Function CreateSpatialDocuments(matrix As Matrix,
                                           Optional min As Double = 0.05,
                                           Optional max As Double = 0.95,
                                           Optional unify As Integer = 20) As Corpus

        Dim filter As Index(Of String) = matrix.GeneFilter(min, max)
        Dim sample As New Corpus

        matrix = matrix _
            .Project(matrix.sampleID - filter) _
            .UnifyMatrix(unify)

        Dim geneIds As String() = matrix.sampleID

        For Each pixel As DataFrameRow In matrix.expression
            Dim document As New List(Of String)

            For i As Integer = 0 To geneIds.Length - 1
                If pixel(i) > 0 Then
                    document += geneIds(i).Replicate(CInt(pixel(i)))
                End If
            Next

            Call sample.addDocument(document)
        Next

        Return sample
    End Function

    ''' <summary>
    ''' unify matrix by each feature columns
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <param name="unify"></param>
    ''' <returns></returns>
    <Extension>
    Private Function UnifyMatrix(matrix As Matrix, unify As Integer) As Matrix
        Dim unifyFactor As New DoubleRange(1, unify)

        For i As Integer = 0 To matrix.sampleID.Length - 1
            matrix.sample(i) = matrix.sample(i).ScaleToRange(unifyFactor)
        Next

        Return matrix
    End Function

    ''' <summary>
    ''' removes genes that appears in less than 5% pixels or more than 95% pixels
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <param name="pmin"></param>
    ''' <param name="pmax"></param>
    ''' <returns></returns>
    <Extension>
    Private Function GeneFilter(matrix As Matrix, pmin As Double, pmax As Double) As Index(Of String)
        Dim geneIds As New List(Of String)
        Dim totalGenes As Integer = matrix.sampleID.Length
        Dim totalPixels As Integer = matrix.size

        For i As Integer = 0 To totalGenes - 1
            Dim v As Vector = matrix.sample(i)
            Dim zero As Integer = (v <= 0.0).Sum

            If zero / totalPixels >= 1 - pmin Then
                geneIds += matrix.sampleID(i)
            ElseIf (totalPixels - zero) / totalPixels >= pmax Then
                geneIds += matrix.sampleID(i)
            End If
        Next

        Return geneIds.Indexing
    End Function

    <Extension>
    Public Function LDAModelling(spatialDoc As Corpus, k As Integer) As LdaGibbsSampler
        ' 2. Create a LDA sampler
        Dim ldaGibbsSampler As New LdaGibbsSampler(spatialDoc.Document(), spatialDoc.VocabularySize())

        ' 3. Train it
        Call ldaGibbsSampler.gibbs(k)

        Return ldaGibbsSampler
    End Function

    <Extension>
    Public Function Deconvolve(LDA As LdaGibbsSampler) As Deconvolve

    End Function

End Module
