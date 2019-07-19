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
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Data

Public Class Enzyme

    Public Property KO As String
    Public Property EC As ECNumber
    Public Property name As String

    Sub New(KO$, geneName$, EC$)
        Me.KO = KO
        Me.name = geneName
        Me.EC = ECNumber.ValueParser(EC)
    End Sub

    ''' <summary>
    ''' Select enzymes
    ''' </summary>
    ''' <param name="repo"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Selects(repo As ReactionRepository) As IReadOnlyDictionary(Of String, Reaction)
        Return repo _
            .GetWhere(Function(r)
                          Return r.Enzyme _
                              .Any(Function(id)
                                       Return Me.EC.Contains(id) OrElse ECNumber.ValueParser(id).Contains(EC)
                                   End Function)
                      End Function)
    End Function
End Class

Public Module Membrane_transport

    ReadOnly defaultIgnores As New [Default](Of Index(Of String))(
        {
            "C00001", "C00002", "C00008", "C00003", "C00006", "C00010", "C00011"
        })

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="metagenome"></param>
    ''' <param name="repo"></param>
    ''' <param name="enzymes">
    ''' ``{KO => enzyme}``
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function BuildTransferNetwork(metagenome As IEnumerable(Of TaxonomyRef),
                                         repo As ReactionRepository,
                                         enzymes As Dictionary(Of String, Enzyme()),
                                         Optional ignores As Index(Of String) = Nothing) As NetworkGraph
        Dim g As New NetworkGraph
        Dim reactions As Reaction()
        Dim ecNumbers As ECNumber() = enzymes.Values _
            .IteratesALL _
            .Select(Function(enz) enz.EC) _
            .GroupBy(Function(enz) enz.ToString) _
            .Select(Function(enzg) enzg.First) _
            .Where(Function(enz) enz.Type = ClassTypes.Transferase OrElse enz.Type = ClassTypes.Translocases) _
            .ToArray
        Dim taxonomyColors As LoopArray(Of Color) = Microsoft.VisualBasic.Imaging.ChartColors
        Dim colorTable As New Dictionary(Of String, String)

        repo = repo.Subset(ecNumbers)
        ignores = ignores Or defaultIgnores

        Dim nodeTable As New Dictionary(Of String, Node)
        Dim edgeTable As New Dictionary(Of String, Edge)
        Dim bacteria As Node
        Dim metabolite As Node

        ' genome -> compound -> genome

        Dim addEdge = Sub(a As Node, b As Node, ecNumber$)
                          Dim edge As New Edge With {
                              .U = a,
                              .V = b,
                              .isDirected = True,
                              .data = New EdgeData With {
                                  .Properties = New Dictionary(Of String, String) From {
                                      {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, ecNumber}
                                  }
                              }
                          }

                          If Not edgeTable.ContainsKey(edge.ID) Then
                              Call edgeTable.Add(edge)
                              Call g.AddEdge(edge)
                          End If
                      End Sub

        ' 遍历所有的基因组
        For Each genome As TaxonomyRef In metagenome
            ' 得到相交的跨膜转运蛋白
            Dim transporters = enzymes.Takes(genome.KOTerms) _
                .IteratesALL _
                .ToArray
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

            For Each enzyme As Enzyme In transporters
                reactions = enzyme.Selects(repo) _
                    .Values _
                    .ToArray

                ' A -> B
                For Each reaction As Reaction In reactions
                    With reaction.ReactionModel
                        For Each compound As String In .Reactants.Where(Function(r) Not r.ID Like ignores).Select(Function(r) r.ID)
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

                            Call addEdge(bacteria, metabolite, enzyme.EC.ToString)
                        Next

                        For Each compound As String In .Products.Where(Function(r) Not r.ID Like ignores).Select(Function(r) r.ID)
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

                            Call addEdge(bacteria, metabolite, enzyme.EC.ToString)
                        Next
                    End With
                Next
            Next

            Call genome.ToString.__INFO_ECHO
        Next

        ' 然后找出所有类型为metabolite的节点
        ' 拿到对应的taxonomy
        ' 删除metabolite相关的边链接,变更为细菌与细菌间的互做
        For Each node As Node In g.vertex _
            .Where(Function(n) n.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = "metabolite") _
            .ToArray

            ' bacteria总是U
            Dim allConnectedGroups = g.graphEdges.Where(Function(e) e.V Is node).GroupBy(Function(e) e.data(NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE)).ToArray
            Dim bacterias As New List(Of Node)

            For Each allConnected In allConnectedGroups
                For Each connection In allConnected
                    bacteria = connection.U

                    bacterias.Add(bacteria)
                    g.RemoveEdge(connection)
                Next

                For Each a In bacterias
                    For Each b In bacterias.Where(Function(n) Not n Is a)
                        Call addEdge(a, b, allConnected.Key)
                    Next
                Next
            Next

            g.RemoveNode(node)
        Next

        Return g
    End Function
End Module
