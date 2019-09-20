#Region "Microsoft.VisualBasic::ee188947f6df7b18ab05329007ae25bc, ExternalDBSource\DEG\OGEE\Extensions.vb"

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

    '     Module Extensions
    ' 
    '         Function: Join, LoadDataSet, LoadGenes, LoadGeneSet
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports SMRUCC.genomics.Data.DEG.OGEE.Models

Namespace DEG.OGEE

    Public Module Extensions

        <Extension>
        Public Iterator Function Join(genes As IEnumerable(Of genes),
                                      dataset As Dictionary(Of String, datasets),
                                      geneSet As Dictionary(Of String, gene_essentiality)) As IEnumerable(Of geneSetInfo)

            For Each gene As genes In genes
                If Not geneSet.ContainsKey(gene.locus) Then
                    Continue For
                End If

                Dim essentiality As gene_essentiality = geneSet(gene.locus)
                Dim dataInfo As datasets = dataset(essentiality.datasetID)

                Yield New geneSetInfo With {
                    .dataset = dataInfo,
                    .essentiality = essentiality,
                    .gene = gene
                }
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function LoadGenes(file As String) As IEnumerable(Of genes)
            Return file.OpenHandle(, tsv:=True).AsLinq(Of genes)
        End Function

        Public Function LoadGeneSet(file As String, Optional all As Boolean = False, Optional kingdom$ = "bacteria") As Dictionary(Of String, gene_essentiality)
            Return file.LoadTsv(Of gene_essentiality) _
                .Where(Function(gene)
                           If all Then
                               If kingdom <> "*" Then
                                   Return gene.kingdom = kingdom
                               Else
                                   Return True
                               End If
                           Else
                               If kingdom <> "*" Then
                                   Return gene.kingdom = kingdom
                               Else
                                   Return gene.essential = "E"
                               End If
                           End If
                       End Function) _
                .GroupBy(Function(g) g.locus) _
                .ToDictionary(Function(g) g.Key, Function(g)
                                                     If g.Count > 1 Then
                                                         Call g.Key.Warning
                                                     End If

                                                     Return g.First
                                                 End Function)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function LoadDataSet(file As String) As Dictionary(Of String, datasets)
            Return file.LoadTsv(Of datasets).ToDictionary(Function(d) d.datasetID)
        End Function
    End Module
End Namespace
