Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Model.Network.KEGG

Public Module Membrane_transport

    ReadOnly membraneTransportComponents As Index(Of String) = {
        "Cell membrane",        ' 细胞膜        
        "Secreted",             ' 细胞分泌
        "Cell outer membrane",  ' 细胞外膜
        "Membrane",             ' 膜
        "Cell surface",         ' 细胞表面
        "Cell inner membrane",  ' 细胞内膜
        "Periplasm"             ' 周质空间
    }

    ReadOnly biologicalCompounds As Index(Of String)

    Sub New()
        ' 会忽略掉下面的大类的物质
        ' Nucleic acids
        biologicalCompounds = CompoundBrite.GetCompoundsWithBiologicalRoles _
            .Where(Function(cpd) cpd.class <> "Nucleic acids") _
            .Select(Function(cpd) cpd.entry.Key) _
            .ToArray
    End Sub

    ''' <summary>
    ''' 获取所有亚细胞定位在膜结构上的蛋白的KO编号
    ''' </summary>
    ''' <param name="genome"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function MembraneComponents(genome As TaxonomyRef) As String()
        If genome.subcellular_components Is Nothing Then
            Return {}
        Else
            Return genome.subcellular_components.locations _
                .SafeQuery _
                .Where(Function(l)
                           Return l.name Like membraneTransportComponents
                       End Function) _
                .Select(Function(l) l.proteins.Keys) _
                .IteratesALL _
                .Distinct _
                .ToArray
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="metagenome"></param>
    ''' <param name="repo"></param>
    ''' <returns></returns>
    <Extension>
    Public Function BuildTransferNetwork(metagenome As IEnumerable(Of TaxonomyRef), repo As ReactionRepository) As NetworkGraph
        Dim g As New NetworkGraph
        Dim reactions As Reaction()
        'Dim ecNumbers As ECNumber() = enzymes.Values _
        '    .IteratesALL _
        '    .Select(Function(enz) enz.EC) _
        '    .GroupBy(Function(enz) enz.ToString) _
        '    .Select(Function(enzg) enzg.First) _
        '    .Where(Function(enz) enz.Type = EnzymeClasses.Transferase OrElse enz.Type = EnzymeClasses.Translocases) _
        '    .ToArray
        Dim taxonomyColors As LoopArray(Of Color) = ChartColors
        Dim colorTable As New Dictionary(Of String, String)
        Dim nodeTable As New Dictionary(Of String, Node)
        Dim edgeTable As New Dictionary(Of String, Edge)
        Dim bacteria As Node
        Dim metabolite As Node

        ' genome -> compound -> genome

        Dim addEdge = Sub(a As Node, b As Node, ecNumber$, supports#)
                          Dim edge As New Edge With {
                              .U = a,
                              .V = b,
                              .isDirected = True,
                              .data = New EdgeData With {
                                  .Properties = New Dictionary(Of String, String) From {
                                      {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, ecNumber}
                                  },
                                  .weight = supports
                              },
                              .weight = supports
                          }

                          If Not edgeTable.ContainsKey(edge.ID) Then
                              Call edgeTable.Add(edge)
                              Call g.AddEdge(edge)
                          End If
                      End Sub

        ' 遍历所有的基因组
        For Each genome As TaxonomyRef In metagenome
            Dim familyLabel$ = genome.TaxonomyString _
                .Select(Metagenomics.TaxonomyRanks.Family) _
                .JoinBy(";")

            If Not nodeTable.ContainsKey(familyLabel) Then
                bacteria = New Node With {
                    .Label = familyLabel.Split(";"c).Where(Function(s) Not s.StringEmpty).Last,
                    .data = New NodeData With {
                        .label = familyLabel,
                        .origID = genome.taxonID,
                        .Properties = New Dictionary(Of String, String) From {
                            {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "bacteria"},
                            {"taxonomy", familyLabel},
                            {"color", colorTable.ComputeIfAbsent(
                                key:=genome.TaxonomyString.class,
                                lazyValue:=Function(key)
                                               Return taxonomyColors.Next.ToHtmlColor
                                           End Function)
                            }
                        }
                    }
                }

                Call nodeTable.Add(familyLabel, bacteria)
                Call g.AddNode(bacteria)
            End If

            bacteria = nodeTable(familyLabel)
            reactions = repo.GetByKOMatch(genome.MembraneComponents).ToArray

            If reactions.IsNullOrEmpty Then
                Call $"{genome.TaxonomyString.ToString} have no membrane located reactions...".Warning
            End If

            ' A -> B
            For Each reaction As Reaction In reactions
                Dim supports = repo.EvidenceScore(reaction.ID, genome.KOTerms, depth:=1)

                ' 这个代谢反应在当前的基因组中可能不存在
                ' 则忽略掉
                If supports = 0R Then
                    Continue For
                Else
                    Call $"  {reaction.ToString} [supports={supports}]".__DEBUG_ECHO
                End If

                With reaction.ReactionModel
                    For Each compound As String In .Reactants.Where(Function(r) r.ID Like biologicalCompounds).Select(Function(r) r.ID)
                        If Not nodeTable.ContainsKey(compound) Then
                            metabolite = New Node With {
                                .Label = compound,
                                .data = New NodeData With {
                                    .label = compound,
                                    .origID = compound,
                                    .Properties = New Dictionary(Of String, String) From {
                                        {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "metabolite"},
                                        {"color", Color.SkyBlue.ToHtmlColor}
                                    }
                                }
                            }

                            Call nodeTable.Add(compound, metabolite)
                            Call g.AddNode(metabolite)
                        End If

                        metabolite = nodeTable(compound)

                        Call addEdge(bacteria, metabolite, reaction.Definition, supports)
                    Next

                    For Each compound As String In .Products.Where(Function(r) r.ID Like biologicalCompounds).Select(Function(r) r.ID)
                        If Not nodeTable.ContainsKey(compound) Then
                            metabolite = New Node With {
                                .Label = compound,
                                .data = New NodeData With {
                                    .label = compound,
                                    .origID = compound,
                                    .Properties = New Dictionary(Of String, String) From {
                                        {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "metabolite"},
                                        {"color", Color.SkyBlue.ToHtmlColor}
                                    }
                                }
                            }

                            Call nodeTable.Add(compound, metabolite)
                            Call g.AddNode(metabolite)
                        End If

                        metabolite = nodeTable(compound)

                        Call addEdge(bacteria, metabolite, reaction.Definition, supports)
                    Next
                End With
            Next

            Call genome.ToString.__INFO_ECHO
        Next

        '' 然后找出所有类型为metabolite的节点
        '' 拿到对应的taxonomy
        '' 删除metabolite相关的边链接,变更为细菌与细菌间的互做
        'For Each node As Node In g.vertex _
        '    .Where(Function(n) n.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = "metabolite") _
        '    .ToArray

        '    ' bacteria总是U
        '    Dim allConnectedGroups = g.graphEdges.Where(Function(e) e.V Is node).GroupBy(Function(e) e.data(NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE)).ToArray
        '    Dim bacterias As New List(Of Node)

        '    For Each allConnected In allConnectedGroups
        '        For Each connection In allConnected
        '            bacteria = connection.U

        '            bacterias.Add(bacteria)
        '            g.RemoveEdge(connection)
        '        Next

        '        For Each a In bacterias
        '            For Each b In bacterias.Where(Function(n) Not n Is a)
        '                Call addEdge(a, b, allConnected.Key)
        '            Next
        '        Next
        '    Next

        '    g.RemoveNode(node)
        'Next

        Return g
    End Function
End Module
