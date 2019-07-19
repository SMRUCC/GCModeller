Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
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

    ReadOnly defaultIgnores As New [Default](Of Index(Of String))({"C00001", "C00002", "C00008"})

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
            .ToArray

        repo = repo.Subset(ecNumbers)
        ignores = ignores Or defaultIgnores

        Dim nodeTable As New Dictionary(Of String, Node)
        Dim edgeTable As New Dictionary(Of String, Edge)
        Dim bacteria As Node
        Dim metabolite As Node

        ' genome -> compound -> genome

        Dim addEdge = Sub(a As Node, b As Node)
                          Dim edge As New Edge With {.U = a, .V = b, .isDirected = True}

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

            bacteria = New Node With {
                .ID = genome.taxonID,
                .Label = genome.TaxonomyString.ToString
            }

            Call nodeTable.Add(bacteria.ID, bacteria)
            Call g.AddNode(bacteria)

            For Each enzyme As Enzyme In transporters
                reactions = enzyme.Selects(repo) _
                    .Values _
                    .ToArray

                ' A -> B
                For Each reaction As Reaction In reactions
                    With reaction.ReactionModel
                        For Each compound As String In .Reactants.Where(Function(r) Not r.ID Like ignores).Select(Function(r) r.ID)
                            If Not nodeTable.ContainsKey(compound) Then
                                metabolite = New Node With {.Label = compound}

                                Call nodeTable.Add(metabolite)
                                Call g.AddNode(metabolite)
                            End If

                            metabolite = nodeTable(compound)

                            Call addEdge(metabolite, bacteria)
                        Next

                        For Each compound As String In .Products.Where(Function(r) Not r.ID Like ignores).Select(Function(r) r.ID)
                            If Not nodeTable.ContainsKey(compound) Then
                                metabolite = New Node With {.Label = compound}

                                Call nodeTable.Add(metabolite)
                                Call g.AddNode(metabolite)
                            End If

                            metabolite = nodeTable(compound)

                            Call addEdge(bacteria, metabolite)
                        Next
                    End With
                Next
            Next

            Call genome.ToString.__INFO_ECHO
        Next

        Return g
    End Function
End Module
