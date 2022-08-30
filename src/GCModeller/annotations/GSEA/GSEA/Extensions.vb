#Region "Microsoft.VisualBasic::e5ead006916a315830b9bbaf46b8b11b, annotations\GSEA\GSEA\Extensions.vb"

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

    ' Module Extensions
    ' 
    '     Function: BackgroundFromCatalog, createClusters, createGenes, CreateResultProfiles, (+2 Overloads) Enrichment
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Data.GeneOntology
Imports SMRUCC.genomics.Data.GeneOntology.OBO

<HideModuleName>
Public Module Extensions

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

    <Extension>
    Public Function BackgroundFromCatalog(catalog As Dictionary(Of String, CatalogProfiling),
                                          Optional id$ = Nothing,
                                          Optional name$ = "n/a",
                                          Optional size% = -1,
                                          Optional comments$ = "none") As Background

        Dim background As New Background With {
            .build = Now,
            .clusters = catalog.createClusters.ToArray,
            .comments = comments,
            .id = id Or App.NextTempName.AsDefault,
            .name = name,
            .size = size
        }

        Return background
    End Function

    <Extension>
    Private Iterator Function createClusters(catalog As Dictionary(Of String, CatalogProfiling)) As IEnumerable(Of Cluster)
        For Each category As KeyValuePair(Of String, CatalogProfiling) In catalog
            For Each subcat As KeyValuePair(Of String, CatalogList) In category.Value.SubCategory
                Yield New Cluster With {
                    .description = subcat.Value.Description,
                    .ID = subcat.Value.Catalog,
                    .names = subcat.Value.Description,
                    .members = subcat.Value _
                        .createGenes _
                        .ToArray
                }
            Next
        Next
    End Function

    <Extension>
    Private Iterator Function createGenes(list As CatalogList) As IEnumerable(Of BackgroundGene)
        For Each id As String In list.IDs
            Yield New BackgroundGene With {
                .accessionID = id,
                .[alias] = {id},
                .locus_tag = New NamedValue With {
                    .name = id,
                    .text = id
                },
                .name = id,
                .term_id = {id}
            }
        Next
    End Function
End Module
