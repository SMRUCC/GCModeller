#Region "Microsoft.VisualBasic::797ddb52015f3131e9ae49e98df49fb9, analysis\OperonMapper\FeatureScores.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2018 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.



' /********************************************************************************/

' Summaries:

'     Module FeatureScores
' 
'         Function: DHamming, IntergenicDistance, L, LengthRatio, NeighborhoodConservation
'                   (+2 Overloads) P
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ContextModel
Imports std = System.Math

Namespace ContextModel.Operon

    ''' <summary>
    ''' To evaluate the contribution of selected features in operon prediction, we have calculated 
    ''' the numerical values of the features, And then used these values individually And in combination 
    ''' to train a classifier. The features used in our study are
    ''' 
    ''' + (i)   the intergenic distance, 
    ''' + (ii)  the conserved gene neighborhood, 
    ''' + (iii) distances between adjacent genes' phylogenetic profiles, 
    ''' + (iv)  the ratio between the lengths of two adjacent genes And 
    ''' + (v)   frequencies Of specific DNA motifs in the intergenic regions.
    ''' </summary>
    ''' <remarks>
    ''' Operon prediction using both genome-specific and general genomic information 
    ''' 
    ''' https://academic.oup.com/nar/article/35/1/288/2401876
    ''' </remarks>
    ''' 
    Public Class OperonPredictionFeatures
        ' 基因信息结构
        Public Structure GeneInfo
            Public GeneID As String
            Public Start As Integer
            Public [End] As Integer
            Public Length As Integer
            Public Strand As Char  ' '+'或'-'
            Public GO_Terms As List(Of String)
            Public PhylogeneticProfile As Dictionary(Of String, Boolean) ' 基因组ID -> 存在状态
        End Structure

        ' 基因组信息结构
        Public Structure GenomeInfo
            Public GenomeID As String
            Public Phylum As String
            Public GeneCount As Integer
            Public GenePositions As Dictionary(Of String, Integer) ' 基因ID -> 位置索引
        End Structure

        ' 1. 计算基因间距离 (Intergenic Distance)
        Public Shared Function CalculateIntergenicDistance(upstreamGene As GeneInfo, downstreamGene As GeneInfo) As Integer
            Return downstreamGene.Start - (upstreamGene.[End] + 1)
        End Function

        ''' <summary>
        ''' where ``L(gi, gj, Gk)`` is the loglikelihood of a gene pair to be neighbors in the kth genome Gk. 
        ''' The log-likelihood score Is computed as the probability that gi And gj are neighbors within a distance dk(i,j) In Gk, 
        ''' Or L(gi, gj, Gk) = log Pij; Pij Is defined as follows:
        ''' 
        ''' (i)   Pij = (1-pik)(1-pjk), if both genes are absent from genome Gk,
        ''' (ii)  Pij = (1-pik) pjk, if only gene j Is present In genome Gk,
        ''' (iii) Pij = pik (1-pjk), if only gene i Is present In genome Gk,
        ''' (iv)  Pij = (pikpjkdk(i, j) (2Nk-dk(i,j)-1))/(Nk(Nk-1)), If genes i And j are present In genome Gk.
        ''' 
        ''' dk(ij) Is the number of genes between gi And gj; Nk Is the number Of genes In genome Gk; And pik Is the probability 
        ''' that gene gi Is present In genome Gk.
        ''' </summary>
        ''' <param name="gene1"></param>
        ''' <param name="gene2"></param>
        ''' <param name="referenceGenomes"></param>
        ''' <param name="phylumProbabilities">基因ID -> (门 -> 概率)</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 2. 计算基因邻域保守性 (Neighborhood Conservation)
        ''' </remarks>
        Public Shared Function CalculateNeighborhoodConservation(gene1 As GeneInfo, gene2 As GeneInfo, referenceGenomes As List(Of GenomeInfo), phylumProbabilities As Dictionary(Of String, Dictionary(Of String, Double))) As Double
            Dim totalScore As Double = 0.0

            For Each genome In referenceGenomes
                Dim p1 As Double = If(phylumProbabilities.ContainsKey(gene1.GeneID) AndAlso
                                 phylumProbabilities(gene1.GeneID).ContainsKey(genome.Phylum),
                                 phylumProbabilities(gene1.GeneID)(genome.Phylum), 0.0)
                Dim p2 As Double = If(phylumProbabilities.ContainsKey(gene2.GeneID) AndAlso
                                 phylumProbabilities(gene2.GeneID).ContainsKey(genome.Phylum),
                                 phylumProbabilities(gene2.GeneID)(genome.Phylum), 0.0)

                Dim gene1Present As Boolean = gene1.PhylogeneticProfile.ContainsKey(genome.GenomeID) AndAlso
                                         gene1.PhylogeneticProfile(genome.GenomeID)
                Dim gene2Present As Boolean = gene2.PhylogeneticProfile.ContainsKey(genome.GenomeID) AndAlso
                                         gene2.PhylogeneticProfile(genome.GenomeID)

                Dim Pij As Double
                If Not gene1Present AndAlso Not gene2Present Then
                    Pij = (1 - p1) * (1 - p2)
                ElseIf Not gene1Present AndAlso gene2Present Then
                    Pij = (1 - p1) * p2
                ElseIf gene1Present AndAlso Not gene2Present Then
                    Pij = p1 * (1 - p2)
                Else
                    Dim dk As Integer = Math.Abs(genome.GenePositions(gene1.GeneID) - genome.GenePositions(gene2.GeneID)) - 1
                    Pij = (p1 * p2 * dk * (2 * genome.GeneCount - dk - 1)) / (genome.GeneCount * (genome.GeneCount - 1))
                End If

                totalScore += Math.Log(If(Pij > 0, Pij, 0.0000000001)) ' 避免log(0)
            Next

            Return -totalScore
        End Function

        ''' <summary>
        ''' For the Hamming distance between two genes A And B, we sum the number Of times that only A Or B Is found in the genome, 
        ''' DH=Sum([1,n], di)‚ where n Is the number of genomes, di=0 if the orthologs of A And B are both present Or both absent in genome i, 
        ''' And di = 1 otherwise.
        ''' </summary>
        ''' <param name="gene1"></param>
        ''' <param name="gene2"></param>
        ''' <param name="genomeIDs"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 3. 计算系统发育距离 (Hamming Distance)
        ''' </remarks>
        Public Shared Function CalculatePhylogeneticDistanceHamming(gene1 As GeneInfo, gene2 As GeneInfo, genomeIDs As List(Of String)) As Integer
            Dim distance As Integer = 0
            For Each genomeID In genomeIDs
                Dim g1Present As Boolean = gene1.PhylogeneticProfile.ContainsKey(genomeID) AndAlso
                                      gene1.PhylogeneticProfile(genomeID)
                Dim g2Present As Boolean = gene2.PhylogeneticProfile.ContainsKey(genomeID) AndAlso
                                      gene2.PhylogeneticProfile(genomeID)

                If g1Present <> g2Present Then
                    distance += 1
                End If
            Next
            Return distance
        End Function

        ''' <summary>
        ''' The score is calculated as the natural log of the length ratio of upstream gene And downstream gene, 
        ''' Or L = ln(li/lj), j = i + 1, whereas li And lj are the length of the genes.
        ''' </summary>
        ''' <param name="upstreamGene"></param>
        ''' <param name="downstreamGene"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 4. 计算基因长度比 (Length Ratio)
        ''' </remarks>
        Public Shared Function CalculateLengthRatio(upstreamGene As GeneInfo, downstreamGene As GeneInfo) As Double
            Return Math.Log(upstreamGene.Length / downstreamGene.Length)
        End Function

        ' 5. 计算DNA基序频率 (Motif Frequency)
        Public Shared Function CalculateMotifFrequency(intergenicSequence As String, motif As String, nucleotideFrequencies As Dictionary(Of Char, Double)) As Double
            Dim observedCount As Integer = 0
            Dim motifLength As Integer = motif.Length

            ' 计算基序在序列中的出现次数
            For i As Integer = 0 To intergenicSequence.Length - motifLength
                Dim substring As String = intergenicSequence.Substring(i, motifLength)
                If substring = motif Then
                    observedCount += 1
                End If
            Next

            ' 计算期望频率
            Dim expectedProbability As Double = 1.0
            For Each c In motif
                expectedProbability *= nucleotideFrequencies(c)
            Next

            Dim expectedCount As Double = (intergenicSequence.Length - motifLength + 1) * expectedProbability
            Return If(expectedCount > 0, observedCount / expectedCount, 0)
        End Function

        ''' <summary>
        ''' 6. 计算GO功能相似性 (GO Similarity)
        ''' </summary>
        ''' <param name="gene1"></param>
        ''' <param name="gene2"></param>
        ''' <param name="goHierarchy">GO术语 -> 父术语列表</param>
        ''' <returns></returns>
        Public Shared Function CalculateGOSimilarity(gene1 As GeneInfo, gene2 As GeneInfo, goHierarchy As Dictionary(Of String, List(Of String))) As Integer
            If gene1.GO_Terms Is Nothing OrElse gene2.GO_Terms Is Nothing OrElse
           gene1.GO_Terms.Count = 0 OrElse gene2.GO_Terms.Count = 0 Then
                Return 0
            End If

            Dim maxSimilarity As Integer = 0

            ' 获取两个基因的所有GO术语（包括祖先术语）
            Dim allTerms1 As New HashSet(Of String)(gene1.GO_Terms)
            Dim allTerms2 As New HashSet(Of String)(gene2.GO_Terms)

            ' 添加祖先术语
            For Each term In gene1.GO_Terms
                AddAncestorTerms(term, goHierarchy, allTerms1)
            Next

            For Each term In gene2.GO_Terms
                AddAncestorTerms(term, goHierarchy, allTerms2)
            Next

            ' 计算共同术语数量
            Dim commonTerms As Integer = allTerms1.Intersect(allTerms2).Count()
            Return commonTerms
        End Function

        ' 递归添加祖先GO术语
        Private Shared Sub AddAncestorTerms(term As String, goHierarchy As Dictionary(Of String, List(Of String)), termSet As HashSet(Of String))
            If goHierarchy.ContainsKey(term) Then
                For Each parentTerm In goHierarchy(term)
                    If termSet.Add(parentTerm) Then
                        AddAncestorTerms(parentTerm, goHierarchy, termSet)
                    End If
                Next
            End If
        End Sub

        ' 计算所有特征的综合函数
        Public Shared Function CalculateAllFeatures(upstreamGene As GeneInfo, downstreamGene As GeneInfo,
                                                    intergenicSequence As String,
                                                    referenceGenomes As List(Of GenomeInfo),
                                                    phylumProbabilities As Dictionary(Of String, Dictionary(Of String, Double)),
                                                    genomeIDs As List(Of String),
                                                    nucleotideFrequencies As Dictionary(Of Char, Double),
                                                    goHierarchy As Dictionary(Of String, List(Of String))) As Dictionary(Of String, Double)

            Dim features As New Dictionary(Of String, Double)

            ' 1. 基因间距离
            features("IntergenicDistance") = CalculateIntergenicDistance(upstreamGene, downstreamGene)

            ' 2. 基因邻域保守性
            features("NeighborhoodConservation") = CalculateNeighborhoodConservation(
            upstreamGene, downstreamGene, referenceGenomes, phylumProbabilities)

            ' 3. 系统发育距离 (Hamming)
            features("PhylogeneticDistance") = CalculatePhylogeneticDistanceHamming(
            upstreamGene, downstreamGene, genomeIDs)

            ' 4. 基因长度比
            features("LengthRatio") = CalculateLengthRatio(upstreamGene, downstreamGene)

            ' 5. DNA基序频率 (使用论文中提到的关键基序)
            Dim motifs As String() = {"TTT", "ATA", "TTTT", "TATA", "TTTTT", "TTTTC"}
            For Each motif As String In motifs
                features($"Motif_{motif}") = CalculateMotifFrequency(intergenicSequence, motif, nucleotideFrequencies)
            Next

            ' 6. GO功能相似性
            features("GOSimilarity") = CalculateGOSimilarity(upstreamGene, downstreamGene, goHierarchy)

            Return features
        End Function
    End Class
End Namespace
