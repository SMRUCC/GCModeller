Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ComponentModel.Annotation

    Public Class CatalogProfile

        Public Property profile As Dictionary(Of String, Double)

        Public Overrides Function ToString() As String
            Return profile.Keys.GetJson
        End Function

    End Class

    Public Class CatalogProfiles

        Public Property catalogs As Dictionary(Of String, CatalogProfile)

        Public Iterator Function GetProfiles() As IEnumerable(Of NamedValue(Of CatalogProfile))
            For Each catalog In catalogs
                Yield New NamedValue(Of CatalogProfile) With {
                    .Name = catalog.Key,
                    .Value = catalog.Value
                }
            Next
        End Function

        Public Overrides Function ToString() As String
            Return catalogs.Keys.GetJson
        End Function

    End Class
End Namespace