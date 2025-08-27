#Region "Microsoft.VisualBasic::21bb36fa231e9226d531ec4fbb780a8f, models\Networks\KEGG\PathwayMaps\ReferenceMap.vb"

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

    '   Total Lines: 567
    '    Code Lines: 466 (82.19%)
    ' Comment Lines: 38 (6.70%)
    '    - Xml Docs: 47.37%
    ' 
    '   Blank Lines: 63 (11.11%)
    '     File Size: 26.73 KB


    '     Module ReferenceMap
    ' 
    '         Function: (+2 Overloads) BuildNetworkModel, buildNetworkModelInternal, createNodeTable, getCompoundClassCategory, getCompoundIndex
    '                   (+2 Overloads) getCompoundsInMap, getKOlist, reactionKOFilter
    ' 
    '         Sub: doMapAssignment, edgesFromClassFilter, edgesFromNoneClassFilter, mapColors, removesUnmapped
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.XML
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork

#If NET48 Then
Imports Pen = System.Drawing.Pen
Imports Pens = System.Drawing.Pens
Imports Brush = System.Drawing.Brush
Imports Font = System.Drawing.Font
Imports Brushes = System.Drawing.Brushes
Imports SolidBrush = System.Drawing.SolidBrush
Imports DashStyle = System.Drawing.Drawing2D.DashStyle
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports GraphicsPath = System.Drawing.Drawing2D.GraphicsPath
Imports FontStyle = System.Drawing.FontStyle
#Else
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports Pens = Microsoft.VisualBasic.Imaging.Pens
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports DashStyle = Microsoft.VisualBasic.Imaging.DashStyle
Imports Image = Microsoft.VisualBasic.Imaging.Image
Imports Bitmap = Microsoft.VisualBasic.Imaging.Bitmap
Imports GraphicsPath = Microsoft.VisualBasic.Imaging.GraphicsPath
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
#End If

Namespace PathwayMaps

    Public Module ReferenceMap

        Private Function getCompoundsInMap(map As Map) As IEnumerable(Of NamedValue(Of String))
            Return map.shapes.mapdata _
                .Select(Function(a) a.Names) _
                .IteratesALL _
                .Where(Function(term)
                           Return term.Name.IsPattern("C\d+")
                       End Function) _
                .Select(Function(c)
                            Return New NamedValue(Of String) With {
                                .Name = c.Name,
                                .Value = c.Value,
                                .Description = map.EntryId
                            }
                        End Function)
        End Function

        Private Function getCompoundsInMap(map As bGetObject.Pathway) As IEnumerable(Of NamedValue(Of String))
            Return map.compound _
                .Where(Function(term)
                           Return term.name.IsPattern("C\d+")
                       End Function) _
                .Select(Function(a)
                            Return New NamedValue(Of String) With {
                                .Name = a.name,
                                .Value = a.text,
                                .Description = map.EntryId
                            }
                        End Function)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Private Function getCompoundIndex(reactionVector As ReactionTable(), getIds As Func(Of ReactionTable, String())) As Dictionary(Of String, ReactionTable())
            Return reactionVector _
                .Where(Function(r)
                           Return Not r.EC.IsNullOrEmpty AndAlso r.EC.Any(Function(num) num.IsPattern("\d(\.\d+)+"))
                       End Function) _
                .Select(Function(r)
                            Return getIds(r).Select(Function(cid) (cid:=cid, r:=r))
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(t) t.cid) _
                .ToDictionary(Function(c) c.Key,
                              Function(g)
                                  Return g.Select(Function(r) r.r).ToArray
                              End Function)
        End Function

        <Extension>
        Private Function createNodeTable(compounds As IGrouping(Of String, NamedValue(Of String))(),
                                         compoundsWithBiologicalRoles As Dictionary(Of String, String)) As NetworkGraph

            Dim node As Node, data As NodeData
            Dim mapList$
            Dim g As New NetworkGraph

            For Each compound As IGrouping(Of String, NamedValue(Of String)) In compounds
                mapList = compound.Select(Function(map) map.Description).JoinBy("|")
                data = New NodeData With {
                    .label = compound.Key,
                    .origID = compound.Key,
                    .Properties = New Dictionary(Of String, String) From {
                        {"maps", mapList},
                        {"class", compoundsWithBiologicalRoles.TryGetValue(compound.Key, [default]:="NA")},
                        {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "compound"}
                    }
                }
                node = New Node(compound.Key, data)

                Call g.AddNode(node)
            Next

            Return g
        End Function

        ''' <summary>
        ''' Ignores these generic compounds for reduce network complexity
        ''' </summary>
        ReadOnly ignores As Index(Of String) = {
            "C00001", ' H2O
            "C00007", ' O2
            "C00011",  ' CO2
            "C00012", ' Peptide
            "C00014",' Ammonia
            "C11481", ' HSO3-
            "C00283", 'Hydrogen sulfide
            "C00094", ' Sulfite
            "C00080", ' H+
            "C00027", ' Hydrogen peroxide
            "C00288", ' HCO3-
            "C00088", ' Nitrite
            "C00162", ' Fatty acid
            "C00039", ' DNA
            "C00017", ' protein
            "C00059",' Sulfate
            "C14818", ' Fe2+
            "C00151" ' L-Amino acid
        }

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function getCompoundClassCategory() As Dictionary(Of String, String)
            Return CompoundBrite _
                .CompoundsWithBiologicalRoles _
                .GroupBy(Function(c) c.entry.Key) _
                .ToDictionary(Function(c) c.Key,
                              Function(c)
                                  Return c.First.class
                              End Function)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="maps">
        ''' </param>
        ''' <param name="reactions"></param>
        ''' <returns></returns>
        Public Function BuildNetworkModel(maps As IEnumerable(Of bGetObject.Pathway),
                                          reactions As IEnumerable(Of ReactionTable),
                                          Optional reactionClass As ReactionClassifier = Nothing,
                                          Optional doRemoveUnmmaped As Boolean = False,
                                          Optional coverageCutoff As Double = 0,
                                          Optional categoryLevel2 As Boolean = False,
                                          Optional topMaps As String() = Nothing,
                                          Optional groupColors As Color() = Nothing) As NetworkGraph
            Dim mapsVector = maps.ToArray
            Dim reactionVector As ReactionTable() = reactions.ToArray
            Dim compounds = mapsVector _
                .Select(AddressOf getCompoundsInMap) _
                .IteratesALL _
                .GroupBy(Function(c) c.Name) _
                .Where(Function(c)
                           ' 因为在这里是物种特定的代谢途径模型数据
                           ' 所以在这里就不进行生物学功能分类了
                           ' 直接全部使用该物种的pathway map里面的所有的代谢物来进行作图
                           Return Not c.Key Like ignores
                       End Function) _
                .ToArray
            Dim compoundCluster = mapsVector _
                .Select(AddressOf BiologicalObjectCluster.CompoundsMap) _
                .ToArray

            Return compounds.buildNetworkModelInternal(
                reactionVector:=reactionVector,
                compoundCluster:=compoundCluster,
                reactionCluster:={},
                reactionClass:=reactionClass,
                doRemoveUnmmaped:=doRemoveUnmmaped,
                coverageCutoff:=coverageCutoff,
                categoryLevel2:=categoryLevel2,
                topMaps:=topMaps,
                groupColors:=groupColors
            )
        End Function

        <Extension>
        Private Function buildNetworkModelInternal(compounds As IGrouping(Of String, NamedValue(Of String))(),
                                                   reactionVector As ReactionTable(),
                                                   compoundCluster As NamedCollection(Of String)(),
                                                   reactionCluster As NamedCollection(Of String)(),
                                                   reactionClass As ReactionClassifier,
                                                   doRemoveUnmmaped As Boolean,
                                                   coverageCutoff As Double,
                                                   categoryLevel2 As Boolean,
                                                   topMaps As String(),
                                                   groupColors As Color()) As NetworkGraph

            Dim reactantIndex = reactionVector.getCompoundIndex(Function(r) r.substrates)
            Dim productIndex = reactionVector.getCompoundIndex(Function(r) r.products)
            Dim compoundsWithBiologicalRoles = getCompoundClassCategory()
            Dim g As NetworkGraph = compounds.createNodeTable(compoundsWithBiologicalRoles)

            ' 下面的两个for循环产生的是从reactant a到products b的反应过程边连接
            For Each a As IGrouping(Of String, NamedValue(Of String)) In compounds
                Dim forwards As ReactionTable() = reactantIndex.TryGetValue(a.Key)

                If forwards.IsNullOrEmpty Then
                    Continue For
                End If

                Dim aName$ = a.First.Value

                If reactionClass Is Nothing Then
                    Dim producs As Dictionary(Of String, ReactionTable()) = forwards _
                        .Select(Function(r) r.products.Select(Function(cid) (cid:=cid, r:=r))) _
                        .IteratesALL _
                        .GroupBy(Function(cid) cid.cid) _
                        .ToDictionary(Function(c) c.Key,
                                      Function(group)
                                          Return group.Select(Function(r) r.r).ToArray
                                      End Function)

                    Call producs.edgesFromNoneClassFilter(a, compounds, g, aName)
                Else
                    Call forwards.edgesFromClassFilter(a.Key, aName, g, reactionClass)
                End If
            Next

            Call g.vertex.ToArray.doMapAssignment(compoundCluster, reactionCluster, coverageCutoff, categoryLevel2, topMaps, groupColors)
            Call g.removesUnmapped(doRemoveUnmmaped)
            Call g.RemoveDuplicated()
            Call g.ComputeNodeDegrees
            Call g.RemovesIsolatedNodes()

            Call $"Result network size=[{g.vertex.Count} nodes, {g.graphEdges.Count} edges]".info

            Return g
        End Function

        <Extension>
        Private Sub removesUnmapped(g As NetworkGraph, doRemoveUnmmaped As Boolean)
            If Not doRemoveUnmmaped Then
                Return
            Else
                Call "All of the unmapped node and the related edges will be removed from the network graph.".debug
            End If

            Dim nodesToRemoves As Index(Of String) = g.vertex.Where(Function(n) n.data("group") = "NA").Keys.Indexing

            Call $"There are {nodesToRemoves.Count} unmapped nodes will be removes from graph".info
            Call $"Current network size=[{g.vertex.Count} nodes, {g.graphEdges.Count} edges]".info

            ' removes all of the unmapped nodes
            ' removes all of the unmapped node related edges
            Call g.graphEdges.Where(Function(e) e.U.label Like nodesToRemoves OrElse e.V.label Like nodesToRemoves).DoEach(Sub(e) Call g.RemoveEdge(e))
            Call g.vertex.Where(Function(v) v.label Like nodesToRemoves).DoEach(Sub(v) Call g.RemoveNode(v))

            Call $"Network size=[{g.vertex.Count} nodes, {g.graphEdges.Count} edges] after operation of removes unmapped nodes".info
        End Sub

        <Extension>
        Private Sub edgesFromClassFilter(forwards As ReactionTable(), aId$, aName$,
                                         g As NetworkGraph,
                                         reactionClass As ReactionClassifier)
            Dim data1, data2 As EdgeData

            For Each flux As ReactionTable In forwards
                For Each transform In reactionClass.GetReactantTransform(flux.entry, {aId}, flux.products)
                    ' compound - compound
                    ' reaction as edge
                    'edges += New NetworkEdge With {
                    '    .fromNode = aId,
                    '    .interaction = flux.entry,
                    '    .toNode = transform.to
                    '}

                    Dim bName = transform.to

                    If g.GetElementByID(flux.entry) Is Nothing Then
                        Dim fluxNode As New Node(flux.entry) With {
                            .data = New NodeData With {
                                .label = flux.EC.JoinBy("+") Or flux.name.AsDefault,
                                .origID = flux.entry,
                                .Properties = New Dictionary(Of String, String) From {
                                    {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "flux"}
                                }
                            }
                        }

                        Call g.AddNode(fluxNode)
                    End If

                    ' reactant -> reaction
                    ' reaction -> product
                    data1 = New EdgeData With {
                        .Properties = New Dictionary(Of String, String) From {
                            {"compound.name", aName},
                            {"flux.name", flux.EC.FirstOrDefault},
                            {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "substrate"}
                        }
                    }
                    g.CreateEdge(aId, flux.entry,, data1)
                    data2 = New EdgeData With {
                        .Properties = New Dictionary(Of String, String) From {
                            {"compound.name", bName},
                            {"flux.name", flux.EC.FirstOrDefault},
                            {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "product"}
                        }
                    }
                    g.CreateEdge(flux.entry, transform.to,, data2)
                Next
            Next
        End Sub

        <Extension>
        Private Sub edgesFromNoneClassFilter(producs As Dictionary(Of String, ReactionTable()),
                                             a As IGrouping(Of String, NamedValue(Of String)),
                                             compounds As IGrouping(Of String, NamedValue(Of String))(),
                                             g As NetworkGraph,
                                             aName$)
            Dim data1, data2 As EdgeData

            For Each b In compounds.Where(Function(c) c.Key <> a.Key AndAlso producs.ContainsKey(c.Key))
                Dim bName$ = b.First.Value

                ' reactant -> reaction
                ' reaction -> product
                For Each flux As ReactionTable In producs(b.Key)
                    If g.GetElementByID(flux.entry) Is Nothing Then
                        Dim fluxNode As New Node(flux.entry) With {
                            .data = New NodeData With {
                                .Properties = New Dictionary(Of String, String) From {
                                    {"label", flux.EC.JoinBy("+") Or flux.name.AsDefault},
                                    {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "flux"}
                                }
                            }
                        }

                        Call g.AddNode(fluxNode)
                    End If

                    data1 = New EdgeData With {
                        .Properties = New Dictionary(Of String, String) From {
                            {"compound.name", aName},
                            {"flux.name", flux.EC.FirstOrDefault},
                            {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "substrate"}
                        }
                    }
                    g.CreateEdge(a.Key, flux.entry,, data1)
                    data2 = New EdgeData With {
                        .Properties = New Dictionary(Of String, String) From {
                            {"compound.name", bName},
                            {"flux.name", flux.EC.FirstOrDefault},
                            {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "product"}
                        }
                    }
                    g.CreateEdge(flux.entry, b.Key,, data2)
                Next
            Next
        End Sub

        <Extension>
        Private Sub doMapAssignment(nodes As Node(),
                                    compoundCluster As NamedCollection(Of String)(),
                                    reactionCluster As NamedCollection(Of String)(),
                                    coverageCutoff As Double,
                                    categoryLevel2 As Boolean,
                                    topMaps As String(),
                                    groupColors As Color())

            Dim compoundsId = nodes.Where(Function(n) n.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) <> "flux").Keys
            Dim reactionsId = nodes.Where(Function(n) n.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = "flux").Keys
            Dim compoundsAssignment = MapAssignment.MapAssignmentByCoverage(
                objects:=compoundsId,
                maps:=compoundCluster,
                coverageCutoff:=coverageCutoff,
                topMaps:=topMaps
            ).CategoryValues

            Dim reactionsAssignment = MapAssignment.MapAssignmentByCoverage(
                objects:=reactionsId,
                maps:=reactionCluster,
                coverageCutoff:=coverageCutoff,
                topMaps:=topMaps
            ).CategoryValues

            Call "Do node map assignment.".debug

            For Each node As Node In nodes
                If node.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = "flux" Then
                    If reactionsAssignment.ContainsKey(node.ID) Then
                        node.data("group") = reactionsAssignment(node.ID)
                    Else
                        node.data("group") = "NA"
                    End If
                Else
                    If compoundsAssignment.ContainsKey(node.ID) Then
                        node.data("group") = compoundsAssignment(node.ID)
                    Else
                        node.data("group") = "NA"
                    End If
                End If
            Next

            Dim mapCategories = BiologicalObjectCluster.GetMapCategories
            Dim map As Pathway

            If categoryLevel2 Then
                Call "Do map assignment in category level 3".debug
            End If

            For Each node As Node In nodes
                If node.data("group") <> "NA" Then
                    map = mapCategories(node.data("group").Match("\d+"))

                    node.data("group.class") = map.class
                    node.data("group.category") = If(categoryLevel2, map.entry.text, map.category)
                End If
            Next

            If Not groupColors.IsNullOrEmpty Then
                Call mapColors(nodes, groupColors)
            End If
        End Sub

        Private Sub mapColors(nodes As Node(), groupColor As Color())
            Dim category As Dictionary(Of String, String) = nodes _
                .Where(Function(n) n.data("group") <> "NA") _
                .ToDictionary(Function(n) n.label,
                              Function(n)
                                  Return n.data("group.category")
                              End Function)
            Dim catIndex = category.Values.Distinct.Indexing
            Dim i As Integer

            For Each node As Node In nodes
                If node.data("group") <> "NA" Then
                    i = catIndex.IndexOf(node.data("group"))

                    If i < groupColor.Length Then
                        node.data.color = New SolidBrush(groupColor(i))
                    End If
                End If
            Next
        End Sub

        <Extension>
        Private Iterator Function reactionKOFilter(reactions As IEnumerable(Of ReactionTable), KO As Index(Of String)) As IEnumerable(Of ReactionTable)
            For Each reaction As ReactionTable In reactions
                If reaction.KO.IsNullOrEmpty Then
                    Continue For
                End If
                If reaction.KO.Any(Function(enzyme) enzyme Like KO) Then
                    Yield reaction
                End If
            Next
        End Function

        <Extension>
        Private Function getKOlist(maps As IEnumerable(Of Map)) As Index(Of String)
            Return maps _
                .Select(Function(map)
                            Return map.shapes.mapdata _
                                .Select(Function(a) a.IDVector)
                        End Function) _
                .IteratesALL _
                .IteratesALL _
                .Where(Function(id) id.IsPattern("K\d+")) _
                .Distinct _
                .Indexing
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="maps">
        ''' <see cref="MapRepository.ScanMaps(String)"/>
        ''' </param>
        ''' <param name="reactions">
        ''' <see cref="ReactionTable.LoadXmls(String)"/>
        ''' </param>
        ''' <returns></returns>
        Public Function BuildNetworkModel(maps As IEnumerable(Of Map), reactions As IEnumerable(Of ReactionTable),
                                          Optional classFilter As Boolean = True,
                                          Optional reactionClass As ReactionClassifier = Nothing,
                                          Optional doRemoveUnmmaped As Boolean = False,
                                          Optional coverageCutoff As Double = 0,
                                          Optional categoryLevel2 As Boolean = False,
                                          Optional topMaps As String() = Nothing,
                                          Optional ignores As Index(Of String) = Nothing,
                                          Optional groupColors As Color() = Nothing) As NetworkGraph

            Dim mapsVector As Map() = maps.ToArray
            Dim reactionVector As ReactionTable() = reactions.reactionKOFilter(mapsVector.getKOlist).ToArray
            Dim compoundsWithBiologicalRoles = getCompoundClassCategory()

            If ignores Is Nothing Then
                ignores = ReferenceMap.ignores
            End If

            Dim compounds = mapsVector _
                .Select(AddressOf getCompoundsInMap) _
                .IteratesALL _
                .GroupBy(Function(c) c.Name) _
                .Where(Function(c) Not c.Key Like ignores) _
                .Where(Function(c)
                           If classFilter Then
                               Return compoundsWithBiologicalRoles.ContainsKey(c.Key)
                           Else
                               Return True
                           End If
                       End Function) _
                .ToArray

            If classFilter Then
                reactionVector = reactionVector _
                    .Where(Function(r)
                               ' reaction show have EC class value
                               If r.EC.IsNullOrEmpty Then
                                   Return False
                               Else
                                   Return r.EC _
                                       .Any(Function([class])
                                                Return [class] _
                                                    .Split("."c) _
                                                    .First _
                                                    .ParseInteger > 0
                                            End Function)
                               End If
                           End Function) _
                    .ToArray
            End If

            Dim compoundCluster = mapsVector.Select(AddressOf BiologicalObjectCluster.CompoundsMap).ToArray
            Dim reactionCluster = mapsVector.Select(AddressOf BiologicalObjectCluster.ReactionMap).ToArray

            Return compounds.buildNetworkModelInternal(
                reactionVector:=reactionVector,
                compoundCluster:=compoundCluster,
                reactionCluster:=reactionCluster,
                reactionClass:=reactionClass,
                doRemoveUnmmaped:=doRemoveUnmmaped,
                coverageCutoff:=coverageCutoff,
                categoryLevel2:=categoryLevel2,
                topMaps:=topMaps,
                groupColors:=groupColors
            )
        End Function
    End Module
End Namespace
