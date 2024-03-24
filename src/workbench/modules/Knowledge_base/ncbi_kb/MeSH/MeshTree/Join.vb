Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text

Namespace MeSH.Tree

    Public Module Join

        <Extension>
        Public Iterator Function JoinData(tree As IEnumerable(Of Term), terms As Dictionary(Of String, DescriptorRecord)) As IEnumerable(Of Term)
            Dim unionTree As Term() = tree _
                .GroupBy(Function(t) t.term) _
                .Select(Function(t)
                            If t.Count = 1 Then
                                Return New Term With {
                                    .term = t.Key,
                                    .tree = {t.First.tree.JoinBy(".")}
                                }
                            Else
                                Return New Term With {
                                    .term = t.Key,
                                    .tree = t _
                                        .Select(Function(ti)
                                                    Return ti.tree.JoinBy(".")
                                                End Function) _
                                        .ToArray
                                }
                            End If
                        End Function) _
                .OrderByDescending(Function(t) t.tree.Length) _
                .ToArray

            For Each term As Term In unionTree
                term.JoinData(terms)
                Yield term
            Next
        End Function

        <Extension>
        Private Sub JoinData(ByRef node As Term, terms As Dictionary(Of String, DescriptorRecord))
            Dim data As DescriptorRecord = terms.TryGetValue(node.term)

            If data Is Nothing Then
                Call $"missing metadata for mesh term: {node.term}".Warning
                Return
            End If

            node.accessionID = data.DescriptorUI
            node.description = data.ConceptList _
                .SafeQuery _
                .Select(Function(c) c.ScopeNote) _
                .JoinBy(vbCrLf) _
                .Trim(" "c, ASCII.CR, ASCII.LF, ASCII.TAB)
            node.alias = data.ConceptList _
                .SafeQuery _
                .Select(Function(c) c.TermList) _
                .IteratesALL _
                .Select(Function(t) t.String) _
                .Distinct _
                .ToArray
        End Sub
    End Module
End Namespace