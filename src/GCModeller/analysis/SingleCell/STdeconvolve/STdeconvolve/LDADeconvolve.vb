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
    ''' [1] Create document vector for run LDA mdelling
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
                                           Optional unify As Integer = 20) As STCorpus

        Dim filter As Index(Of String) = matrix.GeneFilter(min, max)
        Dim sample As New STCorpus

        matrix = matrix _
            .Project(matrix.sampleID - filter) _
            .UnifyMatrix(unify)

        Dim geneIds As String() = matrix.sampleID
        Dim document As New List(Of String)

        For Each pixel As DataFrameRow In matrix.expression
            For i As Integer = 0 To geneIds.Length - 1
                If pixel(i) > 0 Then
                    document += geneIds(i).Replicate(CInt(pixel(i)))
                End If
            Next

            Call sample.addPixel(pixel.geneID, document)
            Call document.Clear()
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

    ''' <summary>
    ''' [2] run LDA modelling
    ''' </summary>
    ''' <param name="spatialDoc"></param>
    ''' <param name="k"></param>
    ''' <returns></returns>
    <Extension>
    Public Function LDAModelling(spatialDoc As STCorpus, k As Integer) As LdaGibbsSampler
        ' 2. Create a LDA sampler
        Dim ldaGibbsSampler As New LdaGibbsSampler(spatialDoc.Document(), spatialDoc.VocabularySize())

        ' 3. Train it
        Call ldaGibbsSampler.gibbs(k)

        Return ldaGibbsSampler
    End Function

    ''' <summary>
    ''' [3] get deconvolve result matrix
    ''' </summary>
    ''' <param name="LDA"></param>
    ''' <param name="corpus"></param>
    ''' <param name="topGenes"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Deconvolve(LDA As LdaGibbsSampler, corpus As STCorpus, Optional topGenes As Integer = 25) As Deconvolve
        ' 4. The phi matrix Is a LDA model, you can use LdaUtil to explain it.
        Dim phi = LDA.Phi()
        Dim topicMap = LdaUtil.translate(phi, corpus.Vocabulary(), limit:=topGenes)
        Dim t As DataFrameRow() = LDA.Theta _
            .Select(Function(dist, i)
                        Return New DataFrameRow With {
                            .geneID = corpus.m_pixels(i),
                            .experiments = dist
                        }
                    End Function) _
            .ToArray

        Return New Deconvolve With {
            .topicMap = topicMap,
            .theta = New Matrix With {
                .expression = t,
                .sampleID = Enumerable _
                    .Range(1, 10) _
                    .Select(Function(i) $"topic_{i}") _
                    .ToArray,
                .tag = "theta"
            }
        }
    End Function

End Module
