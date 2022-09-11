#Region "Microsoft.VisualBasic::682239cb4f0a09f67893b6ebe9a08ca5, GCModeller\annotations\GSEA\PFSNet\PFSNet\Models\GraphEdge\GraphEdgeConnector.vb"

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

    '   Total Lines: 32
    '    Code Lines: 28
    ' Comment Lines: 0
    '   Blank Lines: 4
    '     File Size: 1.14 KB


    '     Module GraphEdgeConnector
    ' 
    '         Function: FromMetabolismNetwork
    ' 
    '         Sub: SaveTabular
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Text

Namespace DataStructure

    Public Module GraphEdgeConnector

        <Extension>
        Public Iterator Function FromMetabolismNetwork(g As NetworkGraph, Optional pathwayId As String = "pathwayId") As IEnumerable(Of GraphEdge)
            For Each edge As Edge In g.graphEdges
                Yield New GraphEdge With {
                    .g1 = edge.U.label,
                    .g2 = edge.V.label,
                    .pathwayID = edge.data(pathwayId)
                }
            Next
        End Function

        <Extension>
        Public Sub SaveTabular(edges As IEnumerable(Of GraphEdge), file As Stream)
            Using writer As New StreamWriter(file, Encodings.UTF8WithoutBOM.CodePage) With {
                .NewLine = vbLf
            }
                For Each edge As GraphEdge In edges
                    Call writer.WriteLine(edge.ToString)
                Next
            End Using
        End Sub
    End Module
End Namespace
