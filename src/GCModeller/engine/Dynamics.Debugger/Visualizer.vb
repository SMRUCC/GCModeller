#Region "Microsoft.VisualBasic::f5b403e36b2ad2286e3d6dfdd9ec13b0, GCModeller\engine\Dynamics.Debugger\Visualizer.vb"

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

    '   Total Lines: 129
    '    Code Lines: 113
    ' Comment Lines: 1
    '   Blank Lines: 15
    '     File Size: 5.51 KB


    ' Module Visualizer
    ' 
    '     Function: CreateTabularFormat, ToGraph
    ' 
    '     Sub: addRegulations
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports gNode = Microsoft.VisualBasic.Data.visualize.Network.Graph.Node

Public Module Visualizer

    Public Function CreateTabularFormat(g As NetworkGraph) As NetworkTables
        Return g.Tabular(properties:={"mass", "weight"})
    End Function

    <Extension>
    Public Function ToGraph(cell As Vessel, Optional flux As Dictionary(Of String, Double) = Nothing) As NetworkGraph
        Dim g As New NetworkGraph
        Dim node As gNode

        For Each mass As gNode In cell.MassEnvironment _
            .Select(Function(m)
                        Return New gNode With {
                            .label = m.ID,
                            .data = New NodeData With {
                                .mass = m.Value,
                                .origID = m.ID,
                                .label = m.ID,
                                .size = {m.Value},
                                .Properties = New Dictionary(Of String, String) From {
                                    {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "metabolite"}
                                }
                            }
                        }
                    End Function)

            Call g.AddNode(mass)
        Next

        Dim reactionMass#

        For Each reaction As Channel In cell.Channels
            If flux Is Nothing Then
                reactionMass = reaction.direct * (reaction.GetReactants.AsList + reaction.GetProducts) _
                    .Select(Function(m) m.mass.Value) _
                    .Average
            Else
                reactionMass = flux(reaction.ID)
            End If

            node = New gNode With {
                .label = reaction.ID,
                .data = New NodeData With {
                    .label = reaction.ID,
                    .origID = reaction.ID,
                    .mass = reactionMass,
                    .size = {reactionMass},
                    .Properties = New Dictionary(Of String, String) From {
                        {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "reaction"}
                    }
                }
            }

            Call g.AddNode(node)
        Next

        For Each reaction As Channel In cell.Channels
            For Each left As Variable In reaction.GetReactants
                Call g.CreateEdge(
                    g.GetElementByID(left.mass.ID),
                    g.GetElementByID(reaction.ID),
                    weight:=left.coefficient * left.mass.Value,
                    data:=New EdgeData With {
                        .label = $"{left.mass.ID}->{reaction.ID}",
                        .length = left.coefficient,
                        .Properties = New Dictionary(Of String, String) From {
                            {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "reactant"}
                        }
                    })
            Next

            For Each right As Variable In reaction.GetProducts
                Call g.CreateEdge(
                    g.GetElementByID(reaction.ID),
                    g.GetElementByID(right.mass.ID),
                    weight:=right.coefficient * right.mass.Value,
                    data:=New EdgeData With {
                        .label = $"{reaction.ID}->{right.mass.ID}",
                        .length = right.coefficient,
                        .Properties = New Dictionary(Of String, String) From {
                            {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "product"}
                        }
                    })
            Next

            ' add regulation controls
            If TypeOf reaction.forward Is AdditiveControls Then
                With DirectCast(reaction.forward, AdditiveControls)
                    If Not reaction.forward Is Nothing Then
                        Call g.addRegulations(reaction.ID, .activation, "forward_active")
                        Call g.addRegulations(reaction.ID, .inhibition, "forward_inhibit")
                    End If

                    If Not reaction.reverse Is Nothing Then
                        Call g.addRegulations(reaction.ID, .activation, "reverse_active")
                        Call g.addRegulations(reaction.ID, .inhibition, "reverse_inhibit")
                    End If
                End With
            End If
        Next

        Return g
    End Function

    <Extension>
    Private Sub addRegulations(g As NetworkGraph, reactionID$, regulations As Variable(), type$)
        For Each factor In regulations.SafeQuery
            Call g.CreateEdge(
                g.GetElementByID(factor.mass.ID),
                g.GetElementByID(reactionID),
                weight:=factor.coefficient * factor.mass.Value,
                data:=New EdgeData With {
                    .label = $"{type} ({factor.mass.ID} ~ {reactionID})",
                    .Properties = New Dictionary(Of String, String) From {
                        {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, type}
                    }
                })
        Next
    End Sub
End Module
