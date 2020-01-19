Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph.Abstract
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
        Private Sub doAppendReactionEnzyme(reactionID As IEnumerable(Of String),
                                           enzymeInfo As Dictionary(Of String, String()),
                                           networkBase As Dictionary(Of String, ReactionTable),
                                           nodes As Dictionary(Of Node),
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
                               Return rn.substrates.Any(AddressOf nodes.ContainsKey) OrElse
                                      rn.products.Any(AddressOf nodes.ContainsKey)
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

                If Not nodes.ContainsKey(reaction.entry) Then
                    nodes.Add(New Node With {
                        .ID = reaction.entry,
                        .NodeType = "reaction",
                        .Properties = New Dictionary(Of String, String) From {
                             {"name", reaction.name},
                             {"color", "yellow"}
                        }
                    })
                End If

                For Each enzyme As String In enzymies
                    If Not nodes.ContainsKey(enzyme) Then
                        nodes.Add(New Node With {
                            .ID = enzyme,
                            .NodeType = "enzyme",
                            .Properties = New Dictionary(Of String, String) From {
                                {"name", enzyme},
                                {"color", "red"}
                            }
                        })
                    End If

                    Dim edge As New NetworkEdge With {
                       .fromNode = enzyme,
                       .toNode = reaction.entry,
                       .Interaction = "catalyst",
                       .Value = 1
                    }

                    Call addNewEdge(edge)
                Next

                ' link between reaction with compounds
                ' 不添加新的代谢物节点
                ' 只添加边链接
                For Each compound In reaction.products
                    Dim edge As New NetworkEdge With {
                       .fromNode = reaction.entry,
                       .toNode = compound,
                       .Interaction = "reaction",
                       .Value = 1
                    }

                    If Not nodes.ContainsKey(compound) Then
                        ' nodes.Add(New Node With {.ID = compound, .NodeType = "KEGG Compound", .Properties = New Dictionary(Of String, String) From {{"name", compound}, {"color", gray}, {"is_extended", True}}})
                    Else
                        Call addNewEdge(edge)
                    End If
                Next

                For Each compound In reaction.substrates
                    Dim edge As New NetworkEdge With {
                       .toNode = reaction.entry,
                       .fromNode = compound,
                       .Interaction = "reaction",
                       .Value = 1
                    }

                    If Not nodes.ContainsKey(compound) Then
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