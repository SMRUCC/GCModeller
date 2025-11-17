#Region "Microsoft.VisualBasic::ee9957eb6540b4982bfcd4d637aca160, models\Networks\Microbiome\Membrane_transport\Membrane_transport.vb"

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

    '   Total Lines: 328
    '    Code Lines: 255 (77.74%)
    ' Comment Lines: 32 (9.76%)
    '    - Xml Docs: 40.62%
    ' 
    '   Blank Lines: 41 (12.50%)
    '     File Size: 14.95 KB


    ' Module Membrane_transport
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: BuildTransferNetwork, MembraneComponents, TransportProcessComponents
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Quantile
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
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
    ''' <summary>
    ''' Class label of <see cref="biologicalCompounds"/>
    ''' </summary>
    ReadOnly compoundClass As Dictionary(Of String, String)
    ReadOnly compoundName As Dictionary(Of String, String)
    ReadOnly commonIgnores As Index(Of String) = {"C00002", "C00003", "C00006", "C00010", "C00016", "C00019"}
    ReadOnly transporters As Index(Of String)

    Sub New()
        ' 会忽略掉下面的大类的物质
        ' Nucleic acids
        Dim classInfo = CompoundBrite.CompoundsWithBiologicalRoles _
            .Where(Function(cpd) cpd.class <> "Nucleic acids") _
            .JoinIterates(CompoundBrite.Lipids) _
            .JoinIterates(CompoundBrite.Carcinogens) _
            .JoinIterates(CompoundBrite.EndocrineDisruptingCompounds) _
            .JoinIterates(CompoundBrite.NaturalToxins) _
            .Where(Function(m) Not m.entry.Key.StringEmpty) _
            .ToArray

        compoundClass = classInfo _
            .GroupBy(Function(c) c.entry.Key) _
            .ToDictionary(Function(c) c.Key,
                          Function(g)
                              Return g.Select(Function(c) c.class).Distinct.JoinBy("/")
                          End Function)
        compoundName = classInfo _
            .GroupBy(Function(c) c.entry.Key) _
            .ToDictionary(Function(c) c.Key,
                          Function(g)
                              Return g.First.entry.Value
                          End Function)
        biologicalCompounds = classInfo _
            .Select(Function(cpd) cpd.entry.Key) _
            .ToArray

        transporters = ProteinFamily.SignalingAndCellularProcesses.BacterialToxins _
            .JoinIterates(ProteinFamily.SignalingAndCellularProcesses.Exosome) _
            .JoinIterates(ProteinFamily.SignalingAndCellularProcesses.ProkaryoticDefenseSystem) _
            .JoinIterates(ProteinFamily.SignalingAndCellularProcesses.SecretionSystem) _
            .JoinIterates(ProteinFamily.SignalingAndCellularProcesses.Transporters) _
            .Select(Function(term) term.entry.Key) _
            .Distinct _
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

    <Extension>
    Public Function TransportProcessComponents(taxon As TaxonomyRef) As String()
        Dim genome = taxon.genome

        If genome Is Nothing OrElse genome.size = 0 Then
            Return {}
        Else
            Return transporters _
                .Intersect(collection:=genome.EntityList) _
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

        Dim addEdge = Sub(a As Node, b As Node, ecNumber$, supports#, direction$)
                          Dim edge As New Edge With {
                              .U = a,
                              .V = b,
                              .isDirected = True,
                              .data = New EdgeData With {
                                  .Properties = New Dictionary(Of String, String) From {
                                      {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, direction},
                                      {"reaction", ecNumber}
                                  }
                              },
                              .weight = supports
                          }

                          If Not edgeTable.ContainsKey(edge.ID) Then
                              Call edgeTable.Add(edge)
                              Call g.AddEdge(edge)
                          End If
                      End Sub
        Dim familyName$

        ' 遍历所有的基因组
        For Each genome As TaxonomyRef In metagenome.Where(Function(tax) Not tax Is Nothing)
            Dim familyLabel$ = genome.TaxonomyString _
                .Select(Metagenomics.TaxonomyRanks.Genus) _
                .JoinBy(";")

            If Not nodeTable.ContainsKey(familyLabel) Then
                familyName = familyLabel.Split(";"c).Where(Function(s) Not s.StringEmpty).Last
                bacteria = New Node With {
                    .label = familyName,
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
                            },
                            {"title", familyName}
                        }
                    }
                }

                Call nodeTable.Add(familyLabel, bacteria)
                Call g.AddNode(bacteria)
            End If

            bacteria = nodeTable(familyLabel)
            reactions = repo _
                .GetByKOMatch(genome.MembraneComponents.AsList + genome.TransportProcessComponents) _
                .ToArray

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
                    Call $"  {reaction.ToString} [supports={supports}]".debug
                End If

                With reaction.ReactionModel
                    For Each compound As String In .Reactants _
                                                   .Where(Function(r)
                                                              Return r.ID Like biologicalCompounds AndAlso Not r.ID Like commonIgnores
                                                          End Function) _
                                                   .Select(Function(r) r.ID)

                        If Not nodeTable.ContainsKey(compound) Then
                            metabolite = New Node With {
                                .label = compound,
                                .data = New NodeData With {
                                    .label = compound,
                                    .origID = compound,
                                    .Properties = New Dictionary(Of String, String) From {
                                        {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "metabolite"},
                                        {"color", Color.SkyBlue.ToHtmlColor},
                                        {"taxonomy", compoundClass(compound)},
                                        {"title", compoundName(compound)}
                                    }
                                }
                            }

                            Call nodeTable.Add(compound, metabolite)
                            Call g.AddNode(metabolite)
                        End If

                        metabolite = nodeTable(compound)

                        Call addEdge(metabolite, bacteria, reaction.Definition, supports, "uptake")
                    Next

                    For Each compound As String In .Products _
                                                   .Where(Function(r)
                                                              Return r.ID Like biologicalCompounds AndAlso Not r.ID Like commonIgnores
                                                          End Function) _
                                                   .Select(Function(r) r.ID)

                        If Not nodeTable.ContainsKey(compound) Then
                            metabolite = New Node With {
                                .label = compound,
                                .data = New NodeData With {
                                    .label = compound,
                                    .origID = compound,
                                    .Properties = New Dictionary(Of String, String) From {
                                        {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "metabolite"},
                                        {"color", Color.SkyBlue.ToHtmlColor},
                                        {"taxonomy", compoundClass(compound)},
                                        {"title", compoundName(compound)}
                                    }
                                }
                            }

                            Call nodeTable.Add(compound, metabolite)
                            Call g.AddNode(metabolite)
                        End If

                        metabolite = nodeTable(compound)

                        Call addEdge(bacteria, metabolite, reaction.Definition, supports, "excrete")
                    Next
                End With
            Next

            Call genome.ToString.info
        Next

        ' 计算每一种代谢物节点的degree数量
        ' 将top10删除
        ' 因为这些边链接非常高的代谢物可能是细胞内的通用代谢物,而非分泌到外部的代谢物
        Call "Do graph node connectivity analysis...".info
        Call g.ApplyAnalysis

        Dim metabolites = g.vertex _
            .Where(Function(n) n.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = "metabolite") _
            .OrderByDescending(Function(n) n.data.neighborhoods) _
            .ToArray
        Dim degrees As Double() = metabolites _
            .Select(Function(v) CDbl(v.data.neighborhoods)) _
            .ToArray
        Dim quartile As DataQuartile = degrees.Quartile
        Dim threshold = quartile.Q3

        Call $"There is {metabolites.Length} metabolites in graph".debug
        Call $"Node degree distribution: {quartile.ToString}".debug

        metabolites = metabolites.Where(Function(m) m.data.neighborhoods > threshold).ToArray

        Dim edgeIndexByNodeLabel As Dictionary(Of String, Edge()) = g.graphEdges _
            .Select(Function(e) {(DirectCast(e, SparseGraph.IInteraction).source, e), (DirectCast(e, SparseGraph.IInteraction).target, e)}) _
            .IteratesALL _
            .GroupBy(Function(t) t.Item1) _
            .ToDictionary(Function(gr) gr.Key,
                          Function(gr)
                              Return gr.Select(Function(t) t.Item2).ToArray
                          End Function)
        Dim deleteEdges As New List(Of Edge)

        For Each node As Node In metabolites
            deleteEdges += edgeIndexByNodeLabel(node.label)

            Call $"Delete high connected metabolite: [{node}] {node.data!title}".debug
        Next

        ' 重新生成graph对象
        Dim leftEdges As Edge()
        Dim leftNodes As Node()

        With deleteEdges.Select(Function(e) e.ID).Distinct.Indexing
            leftEdges = g.graphEdges.Where(Function(e) .IndexOf(e.ID) = -1).ToArray
        End With
        With metabolites.Select(Function(n) n.label).Distinct.Indexing
            leftNodes = g.vertex.Where(Function(n) .IndexOf(n.label) = -1).ToArray
        End With

        Return New NetworkGraph(leftNodes, leftEdges)
    End Function
End Module
