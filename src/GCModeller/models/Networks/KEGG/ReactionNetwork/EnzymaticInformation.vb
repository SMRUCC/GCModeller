#Region "Microsoft.VisualBasic::2ddd8720ccaa2c031a99aa7bf6243e36, GCModeller\models\Networks\KEGG\ReactionNetwork\EnzymaticInformation.vb"

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

    '   Total Lines: 176
    '    Code Lines: 139
    ' Comment Lines: 17
    '   Blank Lines: 20
    '     File Size: 7.66 KB


    '     Module EnzymaticInformation
    ' 
    '         Function: populateEnzymies
    ' 
    '         Sub: doAppendReactionEnzyme
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace ReactionNetwork

    Module EnzymaticInformation

        <Extension>
        Private Function populateEnzymies(reaction As ReactionTable, enzymeInfo As Dictionary(Of String, String())) As String()
            Dim list As New List(Of String)

            If Not reaction.KO.IsNullOrEmpty Then
                list += enzymeInfo.Takes(reaction.KO) _
                    .IteratesALL _
                    .Where(Function(s) Not s.StringEmpty)
            End If
            If Not reaction.EC.IsNullOrEmpty Then
                list += enzymeInfo.Takes(reaction.EC) _
                    .IteratesALL _
                    .Where(Function(s) Not s.StringEmpty)
            End If

            Return list.Distinct.ToArray
        End Function

        <Extension>
        Friend Sub doAppendReactionEnzyme(reactionID As IEnumerable(Of String),
                                          enzymeInfo As Dictionary(Of String, String()),
                                          networkBase As Dictionary(Of String, ReactionTable),
                                          nodes As CompoundNodeTable,
                                          addNewEdge As Action(Of Edge),
                                          enzymeRelated As Boolean)

            Dim reactions As ReactionTable()
            Dim usedEnzymies As New List(Of String)

            If enzymeInfo.IsNullOrEmpty Then
                Return
            Else
                reactions = reactionID _
                    .Select(Function(id) networkBase(id)) _
                    .Where(Function(r)
                               Return Not r.KO.IsNullOrEmpty OrElse Not r.EC.IsNullOrEmpty
                           End Function) _
                    .ToArray
            End If

            For Each reaction As ReactionTable In reactions _
                .Where(Function(rn)
                           If enzymeRelated Then
                               Return rn.substrates.Any(AddressOf nodes.containsKey) OrElse
                                      rn.products.Any(AddressOf nodes.containsKey)
                           Else
                               Return True
                           End If
                       End Function)

                Dim enzymies = reaction.populateEnzymies(enzymeInfo)

                If enzymies.IsNullOrEmpty Then
                    Continue For
                Else
                    usedEnzymies += enzymies
                End If

                If Not nodes.containsKey(reaction.entry) Then
                    Call New Node With {
                        .label = reaction.entry,
                        .data = New NodeData With {
                            .color = Brushes.Yellow,
                            .label = reaction.entry,
                            .origID = reaction.entry,
                            .Properties = New Dictionary(Of String, String) From {
                                {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "reaction"},
                                {"name", reaction.name},
                                {"kegg", reaction.entry}
                            }
                        }
                    }.DoCall(AddressOf nodes.add)
                End If

                For Each enzyme As String In enzymies
                    If Not nodes.containsKey(enzyme) Then
                        Call New Node With {
                            .label = enzyme,
                            .data = New NodeData With {
                                .label = enzyme,
                                .origID = enzyme,
                                .color = Brushes.Red,
                                .Properties = New Dictionary(Of String, String) From {
                                    {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "enzyme"},
                                    {"name", enzyme}
                                }
                            }
                        }.DoCall(AddressOf nodes.add)
                    End If

                    Dim edge As New Edge With {
                       .U = nodes(enzyme),
                       .V = nodes(reaction.entry),
                       .data = New EdgeData With {
                           .Properties = New Dictionary(Of String, String) From {
                               {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "catalyst"}
                           }
                       },
                       .weight = 1
                    }

                    Call addNewEdge(edge)
                Next

                ' link between reaction with compounds
                ' 不添加新的代谢物节点
                ' 只添加边链接
                For Each compound In reaction.products
                    Dim edge As New Edge With {
                       .U = nodes(reaction.entry),
                       .V = nodes(compound),
                       .weight = 1,
                       .data = New EdgeData With {
                           .length = 1,
                           .Properties = New Dictionary(Of String, String) From {
                               {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "reaction"}
                           }
                       }
                    }

                    If Not nodes.containsKey(compound) Then
                        ' nodes.Add(New Node With {.ID = compound, .NodeType = "KEGG Compound", .Properties = New Dictionary(Of String, String) From {{"name", compound}, {"color", gray}, {"is_extended", True}}})
                    Else
                        Call addNewEdge(edge)
                    End If
                Next

                For Each compound In reaction.substrates
                    Dim edge As New Edge With {
                       .U = nodes(reaction.entry),
                       .V = nodes(compound),
                       .weight = 1,
                       .data = New EdgeData With {
                          .length = 1,
                          .Properties = New Dictionary(Of String, String) From {
                            {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "reaction"}
                            }
                        }
                    }

                    If Not nodes.containsKey(compound) Then
                        ' nodes.Add(New Node With {.ID = compound, .NodeType = "KEGG Compound", .Properties = New Dictionary(Of String, String) From {{"name", compound}, {"color", gray}, {"is_extended", True}}})
                    Else
                        Call addNewEdge(edge)
                    End If
                Next
            Next

            'For Each unusedEnzyme In enzymeInfo.Values.IteratesALL.Distinct.Indexing - usedEnzymies
            '    If Not nodes.ContainsKey(unusedEnzyme.value) Then
            '        nodes.Add(New Node With {
            '            .ID = unusedEnzyme.value,
            '            .NodeType = "enzyme",
            '            .Properties = New Dictionary(Of String, String) From {
            '                {"name", unusedEnzyme.value},
            '                {"color", "red"}
            '            }
            '        })
            '    End If
            'Next
        End Sub
    End Module
End Namespace
