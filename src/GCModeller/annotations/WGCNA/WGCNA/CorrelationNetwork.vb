#Region "Microsoft.VisualBasic::f9bc7f8ac815729c564a1fa6317e5ee9, annotations\WGCNA\WGCNA\CorrelationNetwork.vb"

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

    '   Total Lines: 59
    '    Code Lines: 49 (83.05%)
    ' Comment Lines: 1 (1.69%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (15.25%)
    '     File Size: 2.51 KB


    ' Module CorrelationNetwork
    ' 
    '     Function: ExportGraph, LoadAdjacencyMatrix
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Data.Framework.IO.CSVFile
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Math.Matrix
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports std = System.Math

Public Module CorrelationNetwork

    Public Function LoadAdjacencyMatrix(file As String) As DataMatrix
        Dim rows As IEnumerable(Of String()) = From line As String In file.IterateAllLines(tqdm_wrap:=True) Skip 1 Select Tokenizer.CharsParser(line).ToArray
        Dim data As IEnumerable(Of (String, Double())) = From row As String() In rows Select (row(0), row.Skip(1).AsDouble)
        Dim adj As New DataMatrix(data)
        Return adj
    End Function

    <Extension>
    Public Function ExportGraph(adj As DataMatrix, modules As IEnumerable(Of ModuleMembershipResult), Optional adj_thres As Double = 0.8) As NetworkGraph
        Dim g As New NetworkGraph With {
            .id = "adjacency_matrix",
            .name = "WGCNA correlation network"
        }

        For Each gene As ModuleMembershipResult In modules
            Call g.CreateNode(gene.GeneId, New NodeData With {
                .color = New SolidBrush(gene.ModuleName.TranslateColor),
                .label = gene.ToString,
                .mass = gene.Correlation,
                .origID = gene.GeneId,
                .Properties = New Dictionary(Of String, String) From {
                    {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, gene.ModuleName},
                    {"kME", gene.Correlation},
                    {"pvalue", gene.PValue}
                }
            })
        Next

        For Each gene_id As String In TqdmWrapper.Wrap(g.vertex.Select(Function(a) a.label).ToArray)
            Dim u = g.GetElementByID(gene_id)

            ' 20260824 removes the selfloop node
            For Each v As Node In From vi As Node
                                  In g.vertex
                                  Where vi.label <> gene_id

                Dim cor As Double = adj(gene_id, v.label)

                If std.Abs(cor) > adj_thres Then
                    Call g.CreateEdge(u, v, weight:=cor)
                End If
            Next
        Next

        Return g
    End Function
End Module

