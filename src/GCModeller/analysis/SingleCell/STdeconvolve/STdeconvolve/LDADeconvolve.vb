Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.NLP.LDA
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports stdNum = System.Math

''' <summary>
''' ## Reference-free cell-type deconvolution of multi-cellular pixel-resolution spatially resolved transcriptomics data
''' 
''' do spatial data deconvolve via NLP LDA algorithm
''' 
''' Recent technological advancements have enabled spatially resolved 
''' transcriptomic profiling but at multi-cellular pixel resolution, 
''' thereby hindering the identification of cell-type-specific spatial 
''' patterns and gene expression variation. To address this challenge, 
''' we developed STdeconvolve as a reference-free approach to deconvolve 
''' underlying cell-types comprising such multi-cellular pixel 
''' resolution spatial transcriptomics (ST) datasets. Using simulated as 
''' well as real ST datasets from diverse spatial transcriptomics 
''' technologies comprising a variety of spatial resolutions such as 
''' Spatial Transcriptomics, 10X Visium, DBiT-seq, and Slide-seq, we show 
''' that STdeconvolve can effectively recover cell-type transcriptional 
''' profiles and their proportional representation within pixels without 
''' reliance on external single-cell transcriptomics references. 
''' STdeconvolve provides comparable performance to existing reference-based 
''' methods when suitable single-cell references are available, as well 
''' as potentially superior performance when suitable single-cell references 
''' are not available. STdeconvolve is available as an open-source R 
''' software package with the source code available at 
''' https://github.com/JEFworks-Lab/STdeconvolve.
''' </summary>
''' <remarks>
''' https://www.biorxiv.org/content/10.1101/2021.06.15.448381v2
''' doi: https://doi.org/10.1101/2021.06.15.448381
''' </remarks>
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
                                           Optional unify As Integer = 20,
                                           Optional logNorm As Boolean = True) As STCorpus

        Dim filter As Index(Of String) = matrix.GeneFilter(min, max)
        Dim sample As New STCorpus

        matrix = matrix _
            .Project(matrix.sampleID - filter) _  ' reduce the gene features in pixels [5% ~ 95%]
            .UnifyMatrix(unify, logNorm)          ' and then unify the count matrix via log scale and a given unify levels

        Dim geneIds As String() = matrix.sampleID
        Dim document As New List(Of String)

        For Each pixel As DataFrameRow In matrix.expression
            For i As Integer = 0 To geneIds.Length - 1
                If pixel(i) > 0 Then
                    ' convert the unify levels as document composition
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
    Private Function UnifyMatrix(matrix As Matrix, unify As Integer, log As Boolean) As Matrix
        Dim unifyFactor As New DoubleRange(1, unify)
        Dim v As Vector

        For i As Integer = 0 To matrix.sampleID.Length - 1
            v = matrix.sample(i)

            If log Then
                ' avoid negative value in count matrix unify procedure
                v = (From x As Double
                     In v
                     Let ln As Double = If(x <= 1, 0, stdNum.Log(x))
                     Select ln).AsVector
            End If

            matrix.sample(i) = v.ScaleToRange(unifyFactor)
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
    Public Function LDAModelling(spatialDoc As STCorpus, k As Integer,
                                 Optional alpha# = 2.0,
                                 Optional beta# = 0.5) As LdaGibbsSampler

        ' 2. Create a LDA sampler
        Dim ldaGibbsSampler As New LdaGibbsSampler(spatialDoc.Document(), spatialDoc.VocabularySize())

        ' 3. Train LDA model via gibbs sampling
        Call ldaGibbsSampler.gibbs(k, alpha, beta)

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
        ' 4. The phi matrix Is a LDA model, you can use LdaInterpreter to explain it.
        Dim phi = LDA.Phi()
        Dim topicMap = LdaInterpreter.translate(phi, corpus.Vocabulary, limit:=stdNum.Min(topGenes, corpus.VocabularySize))
        Dim t As DataFrameRow() = LDA.Theta _
            .Select(Function(dist, i)
                        ' each pixel Is defined as a mixture of 𝐾 cell types 
                        ' represented As a multinomial distribution Of cell-type 
                        ' probabilities
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
