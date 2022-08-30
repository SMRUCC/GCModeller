Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.ComponentModel.Annotation

Public Module Extensions

    <Extension>
    Public Function CreateResultProfiles(enrich As EnrichmentResult(), catalogs As Dictionary(Of String, CatalogProfiling)) As CatalogProfiles
        Dim result As New CatalogProfiles
        Dim termIndex As Dictionary(Of String, EnrichmentResult) = enrich.ToDictionary(Function(a) a.term)

        For Each cat As CatalogProfiling In catalogs.Values
            For Each subcat In cat.SubCategory.Values
                If termIndex.ContainsKey(subcat.Catalog) Then
                    If termIndex(subcat.Catalog).pvalue >= 1 Then
                        Continue For
                    End If

                    If Not result.catalogs.ContainsKey(cat.Description) Then
                        result.catalogs(cat.Description) = New CatalogProfile
                    End If

                    result.catalogs(cat.Description).Add(subcat.Description, -Math.Log10(termIndex(subcat.Catalog).pvalue))
                End If
            Next
        Next

        Return result
    End Function
End Module
