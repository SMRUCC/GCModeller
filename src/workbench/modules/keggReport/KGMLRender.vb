#Region "Microsoft.VisualBasic::dae6273d61412dba31a6d0d5c9a6667b, modules\keggReport\KGMLRender.vb"

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

    '   Total Lines: 131
    '    Code Lines: 109 (83.21%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 22 (16.79%)
    '     File Size: 4.88 KB


    ' Class KGMLRender
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetNetwork, MakeEntryIndex, Render
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.KGML

Public Class KGMLRender

    ReadOnly kgml As pathway

    Friend ReadOnly graph As NetworkGraph

    ReadOnly entryIndex As New Dictionary(Of entry)

    Default Public ReadOnly Property Item(key As String) As entry
        Get
            Return entryIndex.TryGetValue(key)
        End Get
    End Property

    Sub New(kgml As pathway)
        For Each entry As (id$, entry As entry) In MakeEntryIndex(kgml)
            entryIndex(entry.id) = entry.entry
        Next

        Me.kgml = kgml
        Me.graph = GetNetwork(kgml)
    End Sub

    Public Shared Iterator Function MakeEntryIndex(kgml As pathway) As IEnumerable(Of (String, entry))
        For Each entry As entry In kgml.entries.SafeQuery
            Yield (entry.id, entry)

            If Not entry.reaction.StringEmpty Then
                Yield (entry.reaction.GetTagValue(":").Value, entry)
            End If

            For Each name As String In entry.name
                Yield (name.GetTagValue(":").Value, entry)
            Next
        Next
    End Function

    Public Shared Function GetNetwork(pathway As pathway) As NetworkGraph
        Dim g As New NetworkGraph

        For Each entry As entry In pathway.entries.SafeQuery
            Dim gr As KGML.graphics = entry.graphics
            Dim w As Double = gr.width
            Dim h As Double = gr.height
            Dim x As Double = gr.x
            Dim y As Double = gr.y

            Call g.CreateNode("entry_" & entry.id, New NodeData With {
                .label = entry.name.JoinBy("; "),
                .mass = entry.name.Length,
                .origID = entry.id,
                .size = {w, h},
                .initialPostion = New FDGVector2(x, y),
                .Properties = New Dictionary(Of String, String) From {
                    {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, entry.type},
                    {"reaction", entry.reaction},
                    {"link", entry.link}
                }
            })
        Next

        For Each rel As relation In pathway.relations.SafeQuery
            Call g.CreateEdge(
                g.GetElementByID("entry_" & rel.entry1),
                g.GetElementByID("entry_" & rel.entry2),
                1,
                New EdgeData With {
                    .label = "relation",
                    .Properties = New Dictionary(Of String, String) From {
                        {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, rel.type}
                    }
                }
            )
        Next

        Dim reactionEntries As Dictionary(Of String, entry()) = pathway.entries _
            .SafeQuery _
            .Where(Function(e) Not e.reaction.StringEmpty) _
            .GroupBy(Function(e) e.reaction) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a.ToArray
                          End Function)

        For Each rxn As reaction In pathway.reactions.SafeQuery
            For Each entry As entry In reactionEntries.TryGetValue(rxn.name).SafeQuery
                Dim node As Node = g.GetElementByID("entry_" & entry.id)

                For Each left As compound In rxn.substrates.SafeQuery
                    Call g.CreateEdge(g.GetElementByID("entry_" & left.id), node, 1)
                Next
                For Each right As compound In rxn.products.SafeQuery
                    Call g.CreateEdge(node, g.GetElementByID("entry_" & right.id), 1)
                Next
            Next
        Next

        Return g
    End Function

    Public Function Render(size As Size, nodes As NodeRepresentation,
                           Optional padding$ = "padding: 5% 5% 5% 5%;",
                           Optional driver As Drivers = Drivers.Default) As GraphicsData

        Dim g As NetworkGraph = nodes.MakeSubNetwork(Me)
        Dim size_str As String = $"{size.Width},{size.Height}"
        Dim img As GraphicsData = NetworkVisualizer.DrawImage(
            net:=g,
            canvasSize:=size_str,
            padding:=padding,
            drawNodeShape:=AddressOf nodes.DrawNodeShape,
            displayId:=False,
            throwEx:=False,
            driver:=driver
        )

        Return img
    End Function

End Class

