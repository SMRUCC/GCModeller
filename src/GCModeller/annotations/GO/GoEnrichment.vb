#Region "Microsoft.VisualBasic::2303470c34bd99e9630f7afa5ae17c4f, GCModeller\annotations\GO\GoEnrichment.vb"

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

    '   Total Lines: 171
    '    Code Lines: 116
    ' Comment Lines: 35
    '   Blank Lines: 20
    '     File Size: 7.14 KB


    ' Module GoEnrichment
    ' 
    '     Function: (+2 Overloads) Enrichment, GOClusters, missingGoTermWarnings
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.GSEA.Imports
Imports SMRUCC.genomics.Data.GeneOntology
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Public Module GoEnrichment

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
            Dim backgroundClusterTable = genome.clusters.ToDictionary(Function(c) c.ID)

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
    ''' 一个Go term就是一个cluster
    ''' </summary>
    ''' <param name="GO_terms"></param>
    ''' <returns></returns>
    <Extension>
    Public Function GOClusters(GO_terms As IEnumerable(Of Term)) As GetClusterTerms
        Dim table As Dictionary(Of String, Term) = GO_terms.ToDictionary(Function(t) t.id)
        Dim parentPopulator As Func(Of String, NamedValue(Of String)) =
            Function(termID As String) As NamedValue(Of String)
                Dim GO_term = table.TryGetValue(termID)

                If GO_term Is Nothing Then
                    Call missingGoTermWarnings(termID).Warning
                Else
                    Dim info As Definition = Definition.Parse(GO_term)

                    ' 一个GO term类似于一个cluster
                    ' 其所有基于is_a关系派生出来的子类型都是当前的这个term的cluster成员
                    ' 在计算的时候会需要根据这个关系来展开计算
                    Return New NamedValue(Of String) With {
                        .Name = GO_term.id,
                        .Value = GO_term.name,
                        .Description = info.definition
                    }
                End If

                Return Nothing
            End Function

        Return Function(termID) {parentPopulator(termID)}
    End Function

    Private Function missingGoTermWarnings(termId As String) As String
        Return $"Missing GO term: {termId}, this go term may be obsolete or you needs update the GO obo database to the latest version."
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
End Module
