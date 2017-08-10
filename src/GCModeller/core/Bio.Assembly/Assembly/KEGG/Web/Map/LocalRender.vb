
Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging

Namespace Assembly.KEGG.WebServices

    Public Class LocalRender

        ReadOnly mapTable As Dictionary(Of String, Map)

        Sub New(maps As IEnumerable(Of NamedValue(Of Map)))
            mapTable = maps.ToDictionary(
                Function(map) map.Name,
                Function(pathway) pathway.Value)
        End Sub

        Public Function Rendering(url$) As Image
            Dim data = URLEncoder.URLParser(url)
            Dim pathway As Map = mapTable(data.Name)

            Using g As Graphics2D = pathway.PathwayImage

            End Using
        End Function

        Private Shared Sub renderGenes(ByRef g As Graphics, brush As Brush, map As Map, id As NamedValue(Of String)())

        End Sub

        Private Shared Sub renderCompound(ByRef g As Graphics, map As Map, id As NamedValue(Of String)())

        End Sub
    End Class
End Namespace