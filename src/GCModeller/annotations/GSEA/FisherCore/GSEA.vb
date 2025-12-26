#Region "Microsoft.VisualBasic::cd38aa797c5aa8c03bb64c0569b1cbe4, annotations\GSEA\FisherCore\GSEA.vb"

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

    '   Total Lines: 111
    '    Code Lines: 78 (70.27%)
    ' Comment Lines: 14 (12.61%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 19 (17.12%)
    '     File Size: 4.62 KB


    ' Module GSEACalculate
    ' 
    '     Function: Enrich, enrich_score, Enrichment, pvalue
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
        Dim maxEnrichmentScore As Double = Double.MinValue

        ' --- 修改开始：计算分数所需的参数 ---
        Dim totalGenes As Integer = sortedGenes.Length
        Dim hitsCount As Integer = geneSet.Count

        ' 预先计算“命中”和“未命中”时的分数步长，避免在循环中重复除法
        Dim hitStep As Double = 1.0 / hitsCount
        Dim missStep As Double = 1.0 / (totalGenes - hitsCount)
        ' --- 修改结束 ---

        For Each gene As GeneExpressionRank In sortedGenes
            If gene.gene_id Like geneSet Then
                ' 修改前: enrichmentScore += gene.rank
                ' 修改后: 使用归一化的分数 (1 / N_hits)
                enrichmentScore += hitStep

                If enrichmentScore > maxEnrichmentScore Then
                    maxEnrichmentScore = enrichmentScore
                End If
            Else
                ' 修改前: enrichmentScore -= gene.rank
                ' 修改后: 使用归一化的分数 (1 / (N_total - N_hits))
                enrichmentScore -= missStep
            End If
        Next

        Return maxEnrichmentScore
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

            For i As Integer = 0 To permutedGeneExpression.Length - 1
                Dim k As Integer = randf.NextInteger(permutedGeneExpression.Length)
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

End Class