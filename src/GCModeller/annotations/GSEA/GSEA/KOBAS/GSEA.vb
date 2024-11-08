#Region "Microsoft.VisualBasic::13f24510a930824155598c91e8fc7b87, annotations\GSEA\GSEA\KOBAS\GSEA.vb"

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

'   Total Lines: 329
'    Code Lines: 238 (72.34%)
' Comment Lines: 44 (13.37%)
'    - Xml Docs: 52.27%
' 
'   Blank Lines: 47 (14.29%)
'     File Size: 16.10 KB


'     Module KOBAS_GSEA
' 
'         Function: ES_all, ES_for_permutation, ES_null, fdr_cal, get_hit_matrix
'                   nominal_p, normalized, output_set, rank_pro
'         Structure hitMatrix
' 
' 
' 
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Namespace KOBAS

    ''' <summary>
    ''' Only apply for evluates of the enrichment for a vs b comparision
    ''' </summary>
    <HideModuleName>
    Public Module GSEA

        <Extension>
        Public Function Enrich(geneExpression As NamedValue(Of Double)(),
                               geneSet As Index(Of String),
                               Optional permutations As Integer = 1000) As (score As Double, pvalue As Double)

            Dim score As Double = geneExpression.enrich_score(geneSet)
            Dim pval As Double = geneExpression.pvalue(geneSet, score, permutations)

            Return (score, pval)
        End Function

        <Extension>
        Private Function enrich_score(geneExpression As NamedValue(Of Double)(), geneSet As Index(Of String)) As Double
            Dim sortedGenes = geneExpression.OrderByDescending(Function(a) a.Value).ToArray
            Dim enrichmentScore As Double = 0
            Dim maxEnrichmentScore As Double = Double.MinValue

            For Each gene As NamedValue(Of Double) In sortedGenes
                If gene.Name Like geneSet Then
                    enrichmentScore += gene.Value

                    If enrichmentScore > maxEnrichmentScore Then
                        maxEnrichmentScore = enrichmentScore
                    End If
                Else
                    enrichmentScore -= gene.Value
                End If
            Next

            Return maxEnrichmentScore
        End Function

        <Extension>
        Private Function pvalue(geneExpression As NamedValue(Of Double)(),
                                geneSet As Index(Of String),
                                observedEnrichmentScore As Double,
                                permutations As Integer) As Double

            Dim permutedEnrichmentScores As New List(Of Double)

            For n As Integer = 0 To permutations - 1
                Dim permutedGeneExpression = geneExpression.ToArray

                For i As Integer = 0 To permutedGeneExpression.Length - 1
                    Dim k As Integer = randf.NextInteger(permutedGeneExpression.Length)
                    Dim swapGene = permutedGeneExpression(k).Value
                    Dim temp = permutedGeneExpression(i).Value

                    permutedGeneExpression(i) = New NamedValue(Of Double)(permutedGeneExpression(i).Name, swapGene)
                    permutedGeneExpression(k) = New NamedValue(Of Double)(permutedGeneExpression(k).Name, temp)
                Next

                Call permutedEnrichmentScores.Add(permutedGeneExpression.enrich_score(geneSet))
            Next

            Dim pval = ((Aggregate score As Double
                         In permutedEnrichmentScores
                         Where score >= observedEnrichmentScore
                         Into Count) + 1) / (permutations + 1)
            Return pval
        End Function
    End Module
End Namespace
