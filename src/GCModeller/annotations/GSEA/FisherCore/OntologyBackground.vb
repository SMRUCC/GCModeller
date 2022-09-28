Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models

Public Module OntologyBackground

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="tree">
    ''' the ontology tree
    ''' </param>
    ''' <param name="createTerm"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ImportsTree(Of T)(tree As Tree(Of T), createTerm As Func(Of T, BackgroundGene)) As Background
        ' skip of the first root node
        Dim allTerms = tree.PopulateAllNodes.Skip(1).ToArray
        Dim terms As Cluster() = allTerms _
            .Select(Function(term)
                        Return New Cluster With {
                            .ID = term.label,
                            .description = term.label,
                            .names = term.label,
                            .members = term _
                                .enumerateAllTerms(createTerm) _
                                .ToArray
                        }
                    End Function) _
            .ToArray

        Return New Background With {
            .build = Now,
            .clusters = terms
        }
    End Function

    <Extension>
    Public Function ImportsTree(tree As Tree(Of String)) As Background
        Return tree.ImportsTree(simpleTerm)
    End Function

    Private Function simpleTerm() As Func(Of String, BackgroundGene)
        Return Function(label)
                   Return New BackgroundGene With {
                       .accessionID = label,
                       .[alias] = {label},
                       .locus_tag = New NamedValue With {
                           .name = label,
                           .Text = label
                       },
                       .name = label,
                       .term_id = {label}
                   }
               End Function
    End Function

    <Extension>
    Private Iterator Function enumerateAllTerms(Of T)(node As Tree(Of T), gene As Func(Of T, BackgroundGene)) As IEnumerable(Of BackgroundGene)
        For Each t In node.PopulateAllNodes
            Yield gene(t.Data)
        Next
    End Function
End Module
