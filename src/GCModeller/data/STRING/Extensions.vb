#Region "Microsoft.VisualBasic::c0c0adf48c418c19813264cff2859612, data\STRING\Extensions.vb"

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

    '   Total Lines: 151
    '    Code Lines: 114 (75.50%)
    ' Comment Lines: 17 (11.26%)
    '    - Xml Docs: 70.59%
    ' 
    '   Blank Lines: 20 (13.25%)
    '     File Size: 5.67 KB


    ' Module Extensions
    ' 
    '     Function: (+3 Overloads) MatchNetwork
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Data.STRING.StringDB.Tsv

Public Module Extensions

    <Extension>
    Public Function MatchNetwork(idData As Dictionary(Of String, String), actions$) As NetworkTables
        Dim edges As New List(Of NetworkEdge)
        Dim nodes As New Dictionary(Of Node)
        Dim testAdd As Action(Of String) =
            Sub(id$)
                If Not nodes.ContainsKey(id) Then
                    nodes += New Node With {
                        .ID = id,
                        .NodeType = "protein",
                        .Properties = New Dictionary(Of String, String) From {
                            {"geneID", idData(.ID)}
                        }
                    }
                End If
            End Sub

        For Each link As LinkAction In LinkAction.LoadText(actions)
            If Not idData.ContainsKey(link.item_id_a) OrElse
                Not idData.ContainsKey(link.item_id_b) Then
                ' DO NOTHING
            Else

                Call testAdd(link.item_id_a)
                Call testAdd(link.item_id_b)

                edges += New NetworkEdge With {
                    .FromNode = link.item_id_a,
                    .ToNode = link.item_id_b,
                    .Interaction = link.mode,
                    .value = link.score,
                    .Properties = New Dictionary(Of String, String) From {
                        {"action", link.action},
                        {"a_is_acting", link.a_is_acting}
                    }
                }
            End If
        Next

        Return New NetworkTables With {
            .Edges = edges,
            .Nodes = nodes.Values.ToArray
        }
    End Function

    ''' <summary>
    ''' 这个函数是获取得到所有的网络数据的指定的子集
    ''' </summary>
    ''' <param name="idData"></param>
    ''' <param name="links$"></param>
    ''' <param name="actions$"></param>
    ''' <returns></returns>
    <Extension>
    Public Function MatchNetwork(idData As Dictionary(Of String, String), links$, actions$) As NetworkTables
        Dim edges As New List(Of NetworkEdge)
        Dim nodes As New Dictionary(Of Node)
        Dim testAdd As Action(Of String) =
            Sub(id$)
                If Not nodes.ContainsKey(id) Then
                    nodes += New Node With {
                        .ID = id,
                        .NodeType = "protein",
                        .Properties = New Dictionary(Of String, String) From {
                            {"geneID", idData(.ID)}
                        }
                    }
                End If
            End Sub

        Dim linkActions As New Dictionary(Of String, LinkAction)

        ' 先取出actions的子集
        For Each link As LinkAction In LinkAction.LoadText(actions)
            If Not idData.ContainsKey(link.item_id_a) OrElse
                Not idData.ContainsKey(link.item_id_b) Then
                ' DO NOTHING
            Else

                Call testAdd(link.item_id_a)
                Call testAdd(link.item_id_b)

                Dim link_id = $"{link.item_id_a}+{link.item_id_b}"

                If Not linkActions.ContainsKey(link_id) Then
                    Call linkActions.Add(link_id, link)
                End If
            End If
        Next

        For Each link As linksDetail In linksDetail.IteratesLinks(links)
            If Not idData.ContainsKey(link.protein1) OrElse
               Not idData.ContainsKey(link.protein2) Then
                ' DO NOTHING
            Else

                Call testAdd(link.protein1)
                Call testAdd(link.protein2)

                Dim actionID = $"{link.protein1}+{link.protein2}"
                Dim properties As New Dictionary(Of String, String)
                Dim type$ = "link"

                If linkActions.ContainsKey(actionID) Then
                    With linkActions(actionID)
                        type = .mode

                        Call properties.Add("action", .action)
                        Call properties.Add("a_is_acting", .a_is_acting)
                        Call properties.Add("score", .score)
                    End With
                End If

                edges += New NetworkEdge With {
                    .FromNode = link.protein1,
                    .ToNode = link.protein2,
                    .value = link.combined_score,
                    .Interaction = type,
                    .Properties = properties
                }
            End If
        Next

        Return New NetworkTables With {
            .Edges = edges,
            .Nodes = nodes.Values.ToArray
        }
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="IDlist"><see cref="NamedValue(Of String).Name"/>为STRING之中的蛋白质的编号</param>
    ''' <param name="actions$"><see cref="LinkAction"/></param>
    ''' <returns></returns>
    <Extension>
    Public Function MatchNetwork(IDlist As IEnumerable(Of NamedValue(Of String)), actions$) As NetworkTables
        Dim idData As Dictionary(Of String, String) = IDlist _
            .ToDictionary(Function(x) x.Name,
                          Function(x) x.Value)
        Return idData.MatchNetwork(actions)
    End Function
End Module
