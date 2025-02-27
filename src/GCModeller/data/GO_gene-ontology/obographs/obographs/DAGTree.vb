Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.GeneOntology.DAG
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Public Class DAGTree

    Public Property dag As TermTree(Of Term)

    ''' <summary>
    ''' index by <see cref="OBO.Term.id"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property nodes As Dictionary(Of String, TermTree(Of Term))

    Default Public ReadOnly Property Term(id As String) As TermTree(Of Term)
        Get
            Return nodes.TryGetValue(id)
        End Get
    End Property

    Sub New(obo As GO_OBO, Optional verbose_progress As Boolean = True)
        Dim hash As Dictionary(Of String, Term) = obo.CreateTermTable
        Dim index As New Dictionary(Of String, TermTree(Of Term))

        For Each term As Term In TqdmWrapper.Wrap(hash.Values, wrap_console:=verbose_progress)
            Dim is_a As is_a() = term.is_a _
                .SafeQuery _
                .Select(Function(si) New is_a(si)) _
                .ToArray
            Dim node As TermTree(Of Term)

            If index.ContainsKey(term.id) Then
                node = index(term.id)
                node.Data = term
            Else
                node = New TermTree(Of Term) With {
                    .Data = term,
                    .label = term.name,
                    .Childs = New Dictionary(Of String, Tree(Of Term, String))
                }
                index.Add(term.id, node)
            End If

            For Each link As is_a In is_a
                If Not index.ContainsKey(link.term_id) Then
                    index.Add(link.term_id, New TermTree(Of Term) With {.label = link.name, .Childs = New Dictionary(Of String, Tree(Of Term, String))})
                End If

                node.Parent = index(link.term_id)
                index(link.term_id).Childs.Add(term.id, node)
            Next
        Next

        Dim ontology As TermTree(Of Term)

        If index.Count > 0 Then
            ontology = TermTree(Of Term).FindRoot(index.Values.First)
        Else
            ontology = Nothing
        End If

        nodes = index
        dag = ontology
    End Sub
End Class
