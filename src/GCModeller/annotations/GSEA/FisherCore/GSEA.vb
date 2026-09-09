#Region "Microsoft.VisualBasic::5354e9d1d8bb7c5840468d5455ae42b0, annotations\GSEA\FisherCore\GSEA.vb"

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


    ' Code Statistics:

    '   Total Lines: 153
    '    Code Lines: 95 (62.09%)
    ' Comment Lines: 32 (20.92%)
    '    - Xml Docs: 71.88%
    ' 
    '   Blank Lines: 26 (16.99%)
    '     File Size: 6.14 KB


    ' Module GSEACalculate
    ' 
    '     Function: Enrich, enrich_score, Enrichment
    ' 
    ' Class PermutationTest
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Score, ZeroSet
    ' 
    ' Class GeneExpressionRank
    ' 
    '     Properties: gene_id, rank
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

''' <summary>
''' Only apply for evluates of the enrichment for a vs b comparision
''' </summary>
Public Module GSEACalculate

    <Extension>
    Public Iterator Function Enrichment(background As Background,
                                        geneExpression As GeneExpressionRank(),
                                        Optional permutations As Integer = 1000) As IEnumerable(Of EnrichmentResult)

        Dim geneInputs As String() = geneExpression.Keys

        For Each pathway As Cluster In TqdmWrapper.Wrap(background.clusters, wrap_console:=App.EnableTqdm)
            Dim geneSet As Index(Of String) = pathway.memberIds
            Dim enrich = geneExpression.Enrich(geneSet, permutations)
            Dim intersect = geneInputs.Intersect(geneSet.Objects).ToArray

            Yield New EnrichmentResult With {
                .cluster = geneSet.Count,
                .description = pathway.description,
                .enriched = intersect.Length,
                .IDs = intersect,
                .name = pathway.names,
                .pvalue = enrich.pvalue,
                .score = enrich.score,
                .term = pathway.ID
            }
        Next
    End Function

    ''' <summary>
    ''' Enrichment for one specific pathway
    ''' </summary>
    ''' <param name="geneExpression">
    ''' all gene mean expression value, usually be the foldchange value of the a vs b comparision result.
    ''' </param>
    ''' <param name="geneSet">
    ''' gene set in current pathway
    ''' </param>
    ''' <param name="permutations"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Enrich(geneExpression As GeneExpressionRank(), geneSet As Index(Of String), Optional permutations As Integer = 1000) As (score As Double, pvalue As Double)
        Dim score As Double = geneExpression.enrich_score(geneSet)
        Dim nulltest As New PermutationTest(geneSet, geneExpression, permutations)
        Dim pval As Double = nulltest.Pvalue(score, Hypothesis.Greater)

        Return (score, pval)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="geneExpression"></param>
    ''' <param name="geneSet"></param>
    ''' <returns></returns>
    <Extension>
    Friend Function enrich_score(geneExpression As GeneExpressionRank(), geneSet As Index(Of String)) As Double
        Dim sortedGenes = geneExpression.OrderByDescending(Function(a) a.rank).ToArray
        Dim enrichmentScore As Double = 0
        Dim maxScore As Double = 0
        Dim minScore As Double = 0

        ' 注意：这里必须统计**在表达列表之中真实出现过的**基因集成员数量，
        ' 而不能够直接使用基因集自身的大小来计算步长；否则当基因集之中的
        ' 成员并没有全部出现在表达列表之中的时候，命中的步长就会出现偏差，
        ' 从而使得running sum的最终值不再归零
        Dim totalGenes As Integer = sortedGenes.Length
        Dim hits As Integer = 0

        For Each gene As GeneExpressionRank In sortedGenes
            If gene.gene_id Like geneSet Then
                hits += 1
            End If
        Next

        If hits = 0 OrElse hits >= totalGenes Then
            ' 一个都没有命中，或者列表之中的基因全部都属于当前的基因集，
            ' 这个时候running sum没有偏离，富集分数为零
            Return 0
        End If

        Dim hitStep As Double = 1.0 / hits
        Dim missStep As Double = 1.0 / (totalGenes - hits)

        For Each gene As GeneExpressionRank In sortedGenes
            If gene.gene_id Like geneSet Then
                enrichmentScore += hitStep

                If enrichmentScore > maxScore Then
                    maxScore = enrichmentScore
                End If
            Else
                enrichmentScore -= missStep

                If enrichmentScore < minScore Then
                    minScore = enrichmentScore
                End If
            End If
        Next

        ' 经典GSEA的ES是running sum与零之间的**最大偏离值**，
        ' 即正向偏离与负向偏离之中绝对值更大的那一个
        If maxScore >= -minScore Then
            Return maxScore
        Else
            Return minScore
        End If
    End Function
End Module

Public Class PermutationTest : Inherits NullHypothesis(Of GeneExpressionRank())

    ReadOnly geneExpression As GeneExpressionRank()
    ReadOnly geneSet As Index(Of String)

    Sub New(geneSet As Index(Of String), geneExpression As GeneExpressionRank(), permutations As Integer)
        MyBase.New(permutations)
        Me.geneExpression = geneExpression
        Me.geneSet = geneSet
    End Sub

    Public Overrides Iterator Function ZeroSet() As IEnumerable(Of GeneExpressionRank())
        For n As Integer = 1 To Permutation
            ' make copy of the raw array
            Dim permutedGeneExpression = geneExpression.ToArray

            ' 注意：旧版本的代码在这里从整个数组的范围之中随机取下标k来做交换，
            ' 这是一个有偏的naive shuffle；标准的Fisher-Yates洗牌算法要求
            ' 随机下标k必须取自[i, n-1]这个区间之中
            For i As Integer = 0 To permutedGeneExpression.Length - 2
                Dim k As Integer = i + randf.NextInteger(permutedGeneExpression.Length - i)
                Dim swapRank As Double = permutedGeneExpression(k).rank
                Dim temp As Double = permutedGeneExpression(i).rank

                permutedGeneExpression(i) = New GeneExpressionRank(permutedGeneExpression(i).gene_id, swapRank)
                permutedGeneExpression(k) = New GeneExpressionRank(permutedGeneExpression(k).gene_id, temp)
            Next

            Yield permutedGeneExpression
        Next
    End Function

    Public Overrides Function Score(x() As GeneExpressionRank) As Double
        Return x.enrich_score(geneSet)
    End Function
End Class

Public Class GeneExpressionRank
    Implements INamedValue

    Public Property gene_id As String Implements INamedValue.Key
    ''' <summary>
    ''' gene id tagged with the expression ranking value, example as -log10 of t-test pvalue
    ''' </summary>
    ''' <returns></returns>
    Public Property rank As Double

    Sub New(id As String, rank As Double)
        Me.gene_id = id
        Me.rank = rank
    End Sub

    Public Overrides Function ToString() As String
        Return $"{gene_id}, rank:{rank.ToString("F3")}"
    End Function

End Class
