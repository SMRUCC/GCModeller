Imports System.Drawing
Imports System.Runtime.CompilerServices
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
                                {"name", reaction.name}
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
                           .weight = 1,
                           .Properties = New Dictionary(Of String, String) From {
                               {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "catalyst"}
                           }
                       }
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
                       .data = New EdgeData With {
                        .length = 1,
                        .weight = 1,
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
                       .data = New EdgeData With {
                          .length = 1,
                          .weight = 1,
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