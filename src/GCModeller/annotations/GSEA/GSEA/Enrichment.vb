#Region "Microsoft.VisualBasic::1dc033bb39184b94ca696a52f651fac7, GSEA\GSEA\Enrichment.vb"

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
    '     Function: calcResult, Enrichment, FDRCorrection
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Terminal.ProgressBar
Imports F = Microsoft.VisualBasic.Math.Statistics.Hypothesis.FishersExact.FishersExactTest

''' <summary>
''' 基于Fisher Extract test算法的富集分析
''' </summary>
Public Module Enrichment

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="genome"></param>
    ''' <param name="list">需要进行富集计算分析的目标基因列表</param>
    ''' <param name="outputAll"></param>
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
        Dim tick As New ProgressProvider(genome.clusters.Length)
        Dim ETA$
        Dim termResult As New Value(Of EnrichmentResult)

        If showProgress Then
            progress = New ProgressBar("Do enrichment...")
            doProgress = Sub(id)
                             ETA = $"{id}.... ETA: {tick.ETA(progress.ElapsedMilliseconds)}"
                             progress.SetProgress(tick.StepProgress, ETA)
                         End Sub
        Else
            doProgress = Sub()
                             ' Do Nothing
                         End Sub
        End If

        Dim genes As Integer = genome.clusters _
            .Select(Function(c) c.members) _
            .IteratesALL _
            .Distinct _
            .Count

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
    ''' 计算富集结果
    ''' </summary>
    ''' <param name="cluster"></param>
    ''' <param name="enriched$"></param>
    ''' <param name="inputSize%"></param>
    ''' <param name="genes%"></param>
    ''' <param name="outputAll"></param>
    ''' <returns></returns>
    <Extension>
    Private Function calcResult(cluster As Cluster, enriched$(), inputSize%, genes%, outputAll As Boolean) As EnrichmentResult
        Dim a% = enriched.Length
        Dim b% = cluster.members.Length
        Dim c% = inputSize - a
        Dim d% = genes - b
        Dim pvalue# = F.FishersExact(a, b, c, d).two_tail_pvalue
        Dim score# = a / b

        If (pvalue.IsNaNImaginary OrElse enriched.Length = 0) AndAlso Not outputAll Then
            Return Nothing
        End If

        Return New EnrichmentResult With {
            .term = cluster.ID,
            .name = cluster.names,
            .description = cluster.description,
            .geneIDs = enriched,
            .pvalue = pvalue,
            .score = score,
            .cluster = b,
            .enriched = $"{a}/{c}"
        }
    End Function

    <Extension>
    Public Function FDRCorrection(enrichments As IEnumerable(Of EnrichmentResult)) As EnrichmentResult()
        With enrichments.Shadows
            !FDR = !Pvalue.FDR
            Return .ToArray
        End With
    End Function
End Module

