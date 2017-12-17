Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection

Public Module UniProtExtensions

    <Extension>
    Public Iterator Function PopulateModels(repo As TaxonomyRepository,
                                            taxonomyList As IEnumerable(Of Metagenomics.Taxonomy),
                                            Optional distinct As Boolean = True) As IEnumerable(Of TaxonomyRef)
        Dim hitsID As New Index(Of String)

        For Each taxonomy As Metagenomics.Taxonomy In taxonomyList
            For Each hit As TaxonomyRef In repo.Selects(range:=taxonomy)
                If distinct Then
                    If Not hit.TaxonID.IsOneOfA(hitsID) Then
                        Call hitsID.Add(hit.TaxonID)
                        Yield hit
                    End If
                Else
                    Yield hit
                End If
            Next
        Next
    End Function
End Module
