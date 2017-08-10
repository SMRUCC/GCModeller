
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

            Using g As Graphics2D = pathway.GetImage.CreateCanvas2D(directAccess:=True)
                Call renderGenes(g, pathway, data.Value)
                Call renderCompound(g, pathway, data.Value)

                Return g
            End Using
        End Function

        Private Shared Sub renderGenes(ByRef g As Graphics2D, map As Map, id As NamedValue(Of String)())

        End Sub

        Private Shared Sub renderCompound(ByRef g As Graphics2D, map As Map, id As NamedValue(Of String)())

        End Sub
    End Class
End Namespace