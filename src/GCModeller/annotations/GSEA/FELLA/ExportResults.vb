#Region "Microsoft.VisualBasic::941b474f89a1b37c73be9716f38ff399, annotations\GSEA\FELLA\ExportResults.vb"

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

    '   Total Lines: 246
    '    Code Lines: 154 (62.60%)
    ' Comment Lines: 51 (20.73%)
    '    - Xml Docs: 56.86%
    ' 
    '   Blank Lines: 41 (16.67%)
    '     File Size: 10.50 KB


    '     Class ExportResults
    ' 
    '         Function: FindShortestPath, GenerateDetailedTable, GenerateResultsGraph, GenerateResultsTable, GenerateSummaryReport
    ' 
    '         Sub: ExportToCsv
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' FELLA Algorithm - VB.NET Implementation
' ExportResults.vb - Result export and sub-network extraction
' 
' Provides functions to:
' - Generate result tables (CSV format)
' - Extract significant sub-networks
' - Export enzyme tables with gene annotations
' - Format results for reporting
' ============================================================================

Namespace Core

    ''' <summary>
    ''' Export and format FELLA analysis results.
    ''' </summary>
    Public Class ExportResults

        ''' <summary>
        ''' Generate a results table from an enrichment analysis.
        ''' Returns a list of formatted rows with node information and scores.
        ''' </summary>
        Public Shared Function GenerateResultsTable(user As FellaUser,
                                                      method As EnrichmentMethod,
                                                      Optional threshold As Double = 0.05,
                                                      Optional nLimit As Integer = 250) As String
            Dim result = user.GetResult(method)
            If result Is Nothing Then Return "No results available for the specified method."

            Dim sb As New System.Text.StringBuilder()

            ' Header
            sb.AppendLine("KEGG_ID,Name,Type,RawScore,ZScore,PScore,AdjustedPValue")

            ' Get significant nodes, sorted by p-score
            Dim significant = result.NodeResults.
                Where(Function(r) r.PScore < threshold).
                OrderBy(Function(r) r.PScore).
                Take(nLimit).
                ToList()

            For Each node In significant
                sb.AppendLine($"{node.KeggId},""{node.Name}"",{node.NodeType},{node.RawScore:F8},{node.ZScore:F6},{node.PScore:E8},{node.AdjustedPValue:E8}")
            Next

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' Generate a detailed results table with all node types separated.
        ''' </summary>
        Public Shared Function GenerateDetailedTable(user As FellaUser,
                                                       method As EnrichmentMethod,
                                                       Optional threshold As Double = 0.05) As String
            Dim result = user.GetResult(method)
            If result Is Nothing Then Return "No results available."

            Dim sb As New System.Text.StringBuilder()
            sb.AppendLine($"FELLA Analysis Results - {method} Method")
            sb.AppendLine($"Threshold: {threshold}")
            sb.AppendLine(New String("-"c, 80))

            ' Report for each node type
            For Each nodeType As KeggNodeType In [Enum].GetValues(GetType(KeggNodeType))
                Dim nodes = result.GetSignificantNodes(nodeType, threshold)
                If nodes.Count = 0 Then Continue For

                sb.AppendLine()
                sb.AppendLine($"=== {nodeType} ({nodes.Count} significant) ===")
                sb.AppendLine($"{"ID",-16} {"Name",-40} {"RawScore",12} {"ZScore",10} {"PScore",12} {"AdjP",12}")
                sb.AppendLine(New String("-"c, 100))

                For Each node In nodes
                    Dim name = If(node.Name.Length > 38, node.Name.Substring(0, 38) + "..", node.Name)
                    sb.AppendLine($"{node.KeggId,-16} {name,-40} {node.RawScore,12:F6} {node.ZScore,10:F4} {node.PScore,12:E4} {node.AdjustedPValue,12:E4}")
                Next
            Next

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' Extract the sub-network of significant nodes.
        ''' Returns the set of node indices that should be included in the
        ''' sub-network visualization.
        ''' 
        ''' The sub-network includes:
        ''' 1. All significant nodes (p-score &lt; threshold)
        ''' 2. Input compound nodes
        ''' 3. Connecting nodes on shortest paths between input and significant nodes
        ''' </summary>
        Public Shared Function GenerateResultsGraph(user As FellaUser,
                                                      data As FellaData,
                                                      method As EnrichmentMethod,
                                                      Optional threshold As Double = 0.05,
                                                      Optional nLimit As Integer = 250) As HashSet(Of Integer)
            Dim result = user.GetResult(method)
            If result Is Nothing Then Return New HashSet(Of Integer)()

            Dim subGraphNodes As New HashSet(Of Integer)

            ' Add all significant nodes
            Dim significant = result.NodeResults.
                Where(Function(r) r.PScore < threshold).
                OrderBy(Function(r) r.PScore).
                Take(nLimit)

            For Each node In significant
                subGraphNodes.Add(node.NodeIndex)
            Next

            ' Add input compound nodes
            For Each id In user.InputCompounds
                Dim idx = data.Graph.GetIndex(id)
                If idx >= 0 Then subGraphNodes.Add(idx)
            Next

            ' Add connecting nodes on shortest paths
            ' Use BFS to find shortest paths from input compounds to significant nodes
            Dim inputIndices As New HashSet(Of Integer)
            For Each id In user.InputCompounds
                Dim idx = data.Graph.GetIndex(id)
                If idx >= 0 Then inputIndices.Add(idx)
            Next

            Dim significantIndices As New HashSet(Of Integer)
            For Each node In significant
                significantIndices.Add(node.NodeIndex)
            Next

            ' For each significant non-compound node, find shortest path to any input
            For Each target In significantIndices
                If inputIndices.Contains(target) Then Continue For

                Dim path = FindShortestPath(data.Graph, inputIndices, target)
                If path IsNot Nothing Then
                    For Each idx In path
                        subGraphNodes.Add(idx)
                    Next
                End If
            Next

            Return subGraphNodes
        End Function

        ''' <summary>
        ''' Find shortest path from any source node to a target node using BFS.
        ''' Returns the path as a list of node indices, or Nothing if no path exists.
        ''' </summary>
        Private Shared Function FindShortestPath(graph As KeggGraph,
                                                   sources As HashSet(Of Integer),
                                                   target As Integer) As List(Of Integer)
            Dim visited As New Dictionary(Of Integer, Integer) ' node -> predecessor
            Dim queue As New Queue(Of Integer)

            For Each s In sources
                visited(s) = -1 ' Mark source with no predecessor
                queue.Enqueue(s)
            Next

            While queue.Count > 0
                Dim current = queue.Dequeue()

                If current = target Then
                    ' Reconstruct path
                    Dim path As New List(Of Integer)
                    Dim node = target
                    While node >= 0
                        path.Add(node)
                        node = visited(node)
                    End While
                    path.Reverse()
                    Return path
                End If

                For Each neighbor In graph.GetNeighbors(current)
                    If Not visited.ContainsKey(neighbor) Then
                        visited(neighbor) = current
                        queue.Enqueue(neighbor)
                    End If
                Next
            End While

            Return Nothing ' No path found
        End Function

        ''' <summary>
        ''' Export results to a CSV file.
        ''' </summary>
        Public Shared Sub ExportToCsv(filePath As String,
                                       user As FellaUser,
                                       method As EnrichmentMethod,
                                       Optional threshold As Double = 0.05,
                                       Optional nLimit As Integer = 250)
            Dim content = GenerateResultsTable(user, method, threshold, nLimit)
            System.IO.File.WriteAllText(filePath, content)
        End Sub

        ''' <summary>
        ''' Generate a summary report of the enrichment analysis.
        ''' </summary>
        Public Shared Function GenerateSummaryReport(user As FellaUser,
                                                       data As FellaData,
                                                       Optional threshold As Double = 0.05) As String
            Dim sb As New System.Text.StringBuilder()

            sb.AppendLine("FELLA Enrichment Analysis Summary Report")
            sb.AppendLine(New String("="c, 60))
            sb.AppendLine()

            ' Input information
            sb.AppendLine("INPUT INFORMATION")
            sb.AppendLine($"  Mapped compounds: {user.InputCompounds.Count}")
            sb.AppendLine($"  Excluded compounds: {user.ExcludedCompounds.Count}")
            sb.AppendLine($"  Background size: {user.BackgroundCompounds.Count}")
            sb.AppendLine()

            ' Results for each method
            For Each method As EnrichmentMethod In [Enum].GetValues(GetType(EnrichmentMethod))
                Dim result = user.GetResult(method)
                If result Is Nothing Then Continue For

                sb.AppendLine($"--- {method} Results ---")

                For Each nodeType As KeggNodeType In [Enum].GetValues(GetType(KeggNodeType))
                    Dim sig = result.GetSignificantNodes(nodeType, threshold)
                    If sig.Count > 0 Then
                        sb.AppendLine($"  {nodeType}: {sig.Count} significant (p < {threshold})")
                        For Each node In sig.Take(5)
                            sb.AppendLine($"    {node.KeggId}: {node.Name} (p={node.PScore:E4})")
                        Next
                        If sig.Count > 5 Then
                            sb.AppendLine($"    ... and {sig.Count - 5} more")
                        End If
                    End If
                Next

                sb.AppendLine()
            Next

            Return sb.ToString()
        End Function

    End Class

End Namespace

