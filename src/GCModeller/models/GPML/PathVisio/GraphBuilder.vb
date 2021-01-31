#Region "Microsoft.VisualBasic::cf06b360aec2b2eeff019982b5956f11, models\GPML\PathVisio\GraphBuilder.vb"

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

    ' Module GraphBuilder
    ' 
    '     Function: CreateGraph
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports SMRUCC.genomics.Model.PathVisio.GPML

Public Module GraphBuilder

    <Extension>
    Public Function CreateGraph(pathway As Pathway) As NetworkGraph
        Dim g As New NetworkGraph
        Dim nodeData As NodeData
        Dim linkData As EdgeData

        For Each node As DataNode In pathway.DataNode
            nodeData = New NodeData With {
                .label = node.TextLabel.TrimNewLine,
                .origID = node.GraphId,
                .Properties = New Dictionary(Of String, String) From {
                    {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, node.Type.ToString},
                    {"database", node.Xref.Database},
                    {"xref", node.Xref.ID}
                },
                .initialPostion = New FDGVector2(node.Graphics.CenterX, node.Graphics.CenterY)
            }

            Call g.CreateNode(node.GraphId, nodeData)
        Next

        For Each link As Interaction In pathway.Interaction
            Dim u As Point = link.Graphics.Points(0)
            Dim v As Point = link.Graphics.Points(1)

            linkData = New EdgeData With {
                .label = link.GraphId,
                .Properties = New Dictionary(Of String, String) From {
                    {"database", link.Xref.Database},
                    {"xref", link.Xref.ID}
                }
            }

            If u.GraphRef.StringEmpty OrElse v.GraphRef.StringEmpty Then
                Call $"missing one of the anchor node for link '{link}'!".Warning
            Else
                Call g.CreateEdge(u.GraphRef, v.GraphRef, data:=linkData)
            End If
        Next

        Return g
    End Function
End Module

