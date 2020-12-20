﻿#Region "Microsoft.VisualBasic::d862a3a1aa18870b6475b9d2104967e5, annotations\GSEA\GSEA\Enrichment.vb"

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

    ' Module Enrichment
    ' 
    '     Function: calcResult, (+3 Overloads) Enrichment, FDRCorrection
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.genomics.Data.GeneOntology
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports F = Microsoft.VisualBasic.Math.Statistics.Hypothesis.FishersExact.FishersExactTest

''' <summary>
''' 基于Fisher Extract test算法的富集分析
''' </summary>
Public Module Enrichment

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="genome"></param>
    ''' <param name="list"></param>
    ''' <param name="go"></param>
    ''' <param name="outputAll">将会忽略掉所有没有交集的结果</param>
    ''' <param name="isLocustag"></param>
    ''' <param name="showProgress"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Enrichment(genome As Background,
                               list As IEnumerable(Of String),
                               go As GO_OBO,
                               Optional outputAll As Boolean = False,
                               Optional isLocustag As Boolean = False,
                               Optional showProgress As Boolean = True) As IEnumerable(Of EnrichmentResult)

        Call "Create GO DAG graph... please wait for a while...".__DEBUG_ECHO

        With New DAG.Graph(go.AsEnumerable)
            Return .DoCall(Function(dag)
                               Return genome.Enrichment(list, dag, outputAll, isLocustag, showProgress)
                           End Function)
        End With
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="genome"></param>
    ''' <param name="list"></param>
    ''' <param name="outputAll">将会忽略掉所有没有交集的结果</param>
    ''' <param name="isLocustag"></param>
    ''' <param name="showProgress"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function Enrichment(genome As Background,
                                        list As IEnumerable(Of String),
                                        goClusters As DAG.Graph,
                                        Optional outputAll As Boolean = False,
                                        Optional isLocustag As Boolean = False,
                                        Optional showProgress As Boolean = True) As IEnumerable(Of EnrichmentResult)

        Dim doProgress As Action(Of String)
        Dim progress As ProgressBar = Nothing
        Dim ETA$
        Dim termResult As New Value(Of EnrichmentResult)
        Dim genes As Integer

        If showProgress Then
            Dim tick As ProgressProvider

            progress = New ProgressBar("Do enrichment...")
            tick = New ProgressProvider(progress, genome.clusters.Length)
            doProgress = Sub(id)
                             ETA = $"{id}.... ETA: {tick.ETA().FormatTime}"
                             progress.SetProgress(tick.StepProgress, ETA)
                         End Sub
        Else
            doProgress = Sub()
                             ' Do Nothing
                         End Sub
        End If

        If genome.size <= 0 Then
            genes = genome.clusters _
                .Select(Function(c) c.members) _
                .IteratesALL _
                .Distinct _
                .Count
        Else
            genes = genome.size
        End If

        With list.ToArray
            Dim backgroundClusterTable = genome.clusters.ToDictionary()

            ' 一个cluster就是一个Go term
            For Each cluster As Cluster In genome.clusters
                ' 除了当前的这个GO term之外
                ' 还要找出当前的这个GO term之下的所有继承当前的这个Go term的子条目
                Dim members = goClusters.GetClusterMembers(cluster.ID) _
                    .Where(Function(c)
                               ' 因为有些Go term是在目标基因组中不存在的
                               ' 所以会在这里判断一下是否包含有当前cluster中的目标go term成员
                               Return backgroundClusterTable.ContainsKey(c.id)
                           End Function) _
                    .Select(Function(c) backgroundClusterTable(c.id).members) _
                    .IteratesALL _
                    .JoinIterates(cluster.members) _
                    .GroupBy(Function(g) g.accessionID) _
                    .Select(Function(g)
                                Return g.First
                            End Function) _
                    .ToArray
                ' 构建出一个完整的cluster集合
                ' 然后再在这个完整的cluster集合的基础之上进行富集计算分析
                Dim newCluster As New Cluster With {
                    .ID = cluster.ID,
                    .description = cluster.description,
                    .names = cluster.names,
                    .members = members,
                    .size = members.Length
                }
                Dim enriched$() = newCluster _
                    .Intersect(.ByRef, isLocustag) _
                    .ToArray

                Call doProgress(cluster.ID)

                If Not (termResult = newCluster.calcResult(enriched, .Length, genes, outputAll)) Is Nothing Then
                    Yield termResult
                End If
            Next
        End With

        If Not progress Is Nothing Then
            progress.Dispose()
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="genome"></param>
    ''' <param name="list">需要进行富集计算分析的目标基因列表</param>
    ''' <param name="outputAll">将会忽略掉所有没有交集的结果</param>
    ''' <param name="showProgress"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function Enrichment(genome As Background,
                                        list As IEnumerable(Of String),
                                        Optional outputAll As Boolean = False,
                                        Optional isLocustag As Boolean = False,
                                        Optional showProgress As Boolean = True) As IEnumerable(Of EnrichmentResult)

        Dim doProgress As Action(Of String)
        Dim progress As ProgressBar = Nothing
        Dim ETA$
        Dim termResult As New Value(Of EnrichmentResult)
        Dim genes As Integer

        If showProgress Then
            Dim tick As ProgressProvider

            progress = New ProgressBar("Do enrichment...")
            tick = New ProgressProvider(progress, genome.clusters.Length)
            doProgress = Sub(id)
                             ETA = $"{id}.... ETA: {tick.ETA().FormatTime}"
                             progress.SetProgress(tick.StepProgress, ETA)
                         End Sub
        Else
            doProgress = Sub()
                             ' Do Nothing
                         End Sub
        End If

        If genome.size <= 0 Then
            genes = genome.clusters _
                .Select(Function(c) c.members) _
                .IteratesALL _
                .Distinct _
                .Count
        Else
            genes = genome.size
        End If

        With list.ToArray
            For Each cluster As Cluster In genome.clusters
                Dim enriched$() = cluster.Intersect(.ByRef, isLocustag).ToArray

                Call doProgress(cluster.ID)

                If Not (termResult = cluster.calcResult(enriched, .Length, genes, outputAll)) Is Nothing Then
                    Yield termResult
                End If
            Next
        End With

        If Not progress Is Nothing Then
            progress.Dispose()
        End If
    End Function

    ''' <summary>
    ''' 计算基因集合的功能富集结果
    ''' </summary>
    ''' <param name="cluster">根据我们的先验知识所创建的一个基因集合，一般为KEGG代谢途径或者GO词条</param>
    ''' <param name="enriched">在当前的基因集合中与我们所给定的基因列表所产生交集的基因id的列表，即我们的差异基因列表中属于当前的代谢途径的基因的列表</param>
    ''' <param name="inputSize">输入的原始的通过实验所获取得到的基因列表的大小，即我们的差异基因的id的数量</param>
    ''' <param name="genes">背景基因组中的总的基因数量</param>
    ''' <param name="outputAll"></param>
    ''' <returns></returns>
    <Extension>
    Private Function calcResult(cluster As Cluster, enriched$(), inputSize%, genes%, outputAll As Boolean) As EnrichmentResult
        ' 我们的差异基因列表中，属于目标代谢途径的基因的数量
        Dim a% = enriched.Length
        ' 在目标基因组中，属于当前的代谢途径中的基因的数量
        Dim b% = cluster.members.Length
        ' 在我们的差异基因列表中，不属于当前的代谢途径的基因的数量
        Dim c% = inputSize - a
        ' 在目标基因组中，不属于当前的代谢途径中的基因的数量
        Dim d% = genes - b
        ' 最后将得到的个数变量，进行F双尾检验
        Dim pvalue# = F.FishersExact(a, b, c, d).two_tail_pvalue
        Dim score# = a / b

        If a = 0 Then
            pvalue = 1
        End If

        If (pvalue.IsNaNImaginary OrElse enriched.Length = 0) AndAlso Not outputAll Then
            Return Nothing
        End If

        Return New EnrichmentResult With {
            .term = cluster.ID,
            .name = cluster.names.TrimNewLine,
            .description = cluster.description.TrimNewLine,
            .geneIDs = enriched,
            .pvalue = pvalue,
            .score = score,
            .cluster = b,
            .enriched = $"{a}/{c}"
        }
    End Function

    ''' <summary>
    ''' 进行计算结果的假阳性FDR控制计算
    ''' </summary>
    ''' <param name="enrichments">根据我们所提供的基因列表，对每一个代谢途径的富集计算结果的集合</param>
    ''' <returns></returns>
    <Extension>
    Public Function FDRCorrection(enrichments As IEnumerable(Of EnrichmentResult)) As EnrichmentResult()
        With enrichments.Shadows
            !FDR = !Pvalue.FDR
            Return .ToArray
        End With
    End Function
End Module
