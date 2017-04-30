Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Metagenomics

Namespace Assembly.KEGG.DBGET.BriteHEntry

    ''' <summary>
    ''' br08601
    ''' </summary>
    Public Module Organism

        <Extension> Public Function FillTaxonomyTable(organisms As htext) As Taxonomy()
            Dim out As New List(Of Taxonomy)
            Dim levels As New Dictionary(Of Char, String)
            Dim h As BriteHText
            Dim sp$

            For Each htext As BriteHText In organisms _
                .Hierarchical _
                .EnumerateEntries _
                .Where(Function(hE) hE.CategoryLevel = "E"c)

                h = htext

                Do While h.CategoryLevel <> "/"c
                    h = h.Parent
                    levels(h.CategoryLevel) = h.ClassLabel
                Loop

                sp = htext.ClassLabel
                sp = sp.Split.First
                out += New Taxonomy With {
                    .scientificName = Mid(htext.ClassLabel, sp.Length + 1).Trim,
                    .species = sp,
                    .genus = levels.TryGetValue("D"c),
                    .family = levels.TryGetValue("C"c),
                    .order = levels.TryGetValue("B"c),
                    .class = levels.TryGetValue("A"c)
                }
                Call levels.Clear()
            Next

            Return out
        End Function
    End Module
End Namespace