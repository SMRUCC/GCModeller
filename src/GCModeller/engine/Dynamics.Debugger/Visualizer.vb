#Region "Microsoft.VisualBasic::2ef905d589c4fd58d2f2c1db67db7723, engine\Dynamics.Debugger\Visualizer.vb"

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
        Return g.Tabular({"mass", "weight"})
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
                    .Select(Function(m) m.Mass.Value) _
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
                    left.Mass.ID,
                    reaction.ID,
                    data:=New EdgeData With {
                        .label = $"{left.Mass.ID}->{reaction.ID}",
                        .length = left.Coefficient,
                        .weight = left.Coefficient * left.Mass.Value,
                        .Properties = New Dictionary(Of String, String) From {
                            {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "reactant"}
                        }
                    })
            Next

            For Each right As Variable In reaction.GetProducts
                Call g.CreateEdge(
                    reaction.ID,
                    right.Mass.ID,
                    data:=New EdgeData With {
                        .label = $"{reaction.ID}->{right.Mass.ID}",
                        .length = right.Coefficient,
                        .weight = right.Coefficient * right.Mass.Value,
                        .Properties = New Dictionary(Of String, String) From {
                            {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "product"}
                        }
                    })
            Next

            ' add regulation controls
            If Not reaction.forward Is Nothing Then
                Call g.addRegulations(reaction.ID, reaction.forward.activation, "forward_active")
                Call g.addRegulations(reaction.ID, reaction.forward.inhibition, "forward_inhibit")
            End If

            If Not reaction.reverse Is Nothing Then
                Call g.addRegulations(reaction.ID, reaction.reverse.activation, "reverse_active")
                Call g.addRegulations(reaction.ID, reaction.reverse.inhibition, "reverse_inhibit")
            End If
        Next

        Return g
    End Function

    <Extension>
    Private Sub addRegulations(g As NetworkGraph, reactionID$, regulations As Variable(), type$)
        For Each factor In regulations.SafeQuery
            Call g.CreateEdge(
                factor.Mass.ID,
                reactionID,
                data:=New EdgeData With {
                    .label = $"{type} ({factor.Mass.ID} ~ {reactionID})",
                    .weight = factor.Coefficient * factor.Mass.Value,
                    .Properties = New Dictionary(Of String, String) From {
                        {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, type}
                    }
                })
        Next
    End Sub
End Module

