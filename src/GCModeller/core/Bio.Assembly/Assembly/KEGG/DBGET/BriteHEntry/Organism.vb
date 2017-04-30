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
            Dim levels As New List(Of String)
            Dim h As BriteHText

            For Each htext In organisms.Hierarchical.EnumerateEntries
                h = htext

                Do While h.CategoryLevel <> "/"
                    h = h.Parent
                    levels += h.ClassLabel
                Loop

                out += New Taxonomy With {
                    .scientificName = htext.ClassLabel,
                    .species = .scientificName.Split.First,
                    .genus = levels(0),
                    .family = levels(1),
                    .order = levels(2),
                    .class = levels(3)
                }
                levels *= 0
            Next

            Return out
        End Function
    End Module
End Namespace