
Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Runtime

Namespace Assembly.KEGG.WebServices

    ''' <summary>
    ''' KEGG pathway map local rendering engine
    ''' </summary>
    Public Class LocalRender

        ReadOnly mapTable As Dictionary(Of String, Map)

        Sub New(maps As IEnumerable(Of NamedValue(Of Map)))
            mapTable = maps.ToDictionary(
                Function(map) map.Name,
                Function(pathway) pathway.Value)
        End Sub

        ''' <summary>
        ''' Create renderer from a directory which contains required map file.
        ''' </summary>
        ''' <param name="repo$"></param>
        ''' <returns></returns>
        Public Shared Function FromRepository(repo$) As LocalRender
            Dim maps = (ls - l - r - "*.XML" <= repo) _
                .Select(Function(path$)
                            Return New NamedValue(Of Map) With {
                                .Name = path.BaseName,
                                .Value = path.LoadXml(Of Map),
                                .Description = path
                            }
                        End Function)
            Return New LocalRender(maps)
        End Function

        Public Function Rendering(url$, Optional font As Font = Nothing, Optional textColor$ = "white", Optional scale$ = "1,1") As Image
            Dim data = URLEncoder.URLParser(url)
            Dim pathway As Map = mapTable(data.Name)
            Dim pen As Brush = textColor.GetBrush
            Dim scaleFactor As SizeF = scale.FloatSizeParser

            If font Is Nothing Then
                font = New Font(FontFace.SimSun, 10, FontStyle.Regular)
            End If

            Using g As Graphics2D = pathway.GetImage.CreateCanvas2D(directAccess:=True)
                Call renderGenes(g, font, pen, pathway, scaleFactor, data.Value)
                Call renderCompound(g, font, pathway, data.Value)

                Return g
            End Using
        End Function

        ''' <summary>
        ''' rect
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="map"></param>
        ''' <param name="list"></param>
        Private Shared Sub renderGenes(ByRef g As Graphics2D, font As Font, pen As Brush, map As Map, scale As SizeF, list As NamedValue(Of String)())
            Dim shapes = getAreas(map, "Gene")

            For Each id As NamedValue(Of String) In list
                If Not shapes.ContainsKey(id.Name) Then
                    Continue For
                End If

                Dim brush As Brush = id.Value.GetBrush

                With shapes(id.Name)
                    Dim name As String = .Name
                    Dim strSize = g.MeasureString(name, font)

                    For Each shape As Area In .Value
                        Dim rect As RectangleF = shape.Rectangle.Scale(scale)

                        g.FillRectangle(brush, rect)
                        g.DrawRectangle(Pens.Black, rect)
                        g.DrawString(name, font, pen, rect.CenterAlign(strSize))
                    Next
                End With
            Next
        End Sub

        Private Shared Function getAreas(map As Map, type$) As Dictionary(Of String, NamedValue(Of Area()))
            Dim shapes = map.Areas _
                .Where(Function(x) x.Type = type) _
                .Select(Function(x)
                            Dim titles = x.Names
                            Return x.IdList _
                                .SeqIterator _
                                .Select(Function(cpd) New NamedValue(Of Area) With {
                                    .Name = cpd.value,
                                    .Value = x,
                                    .Description = titles(cpd).Value
                                })
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(x) x.Name) _
                .ToDictionary(Function(x) x.Key,
                              Function(group)
                                  Dim name$ = group.First.Description
                                  Return New NamedValue(Of Area()) With {
                                      .name = name,
                                      .Value = group.Values
                                  }
                              End Function)
            Return shapes
        End Function

        ''' <summary>
        ''' circle
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="map"></param>
        ''' <param name="list"></param>
        Private Shared Sub renderCompound(ByRef g As Graphics2D, font As Font, map As Map, list As NamedValue(Of String)())
            Dim shapes = getAreas(map, "Compound")
            Dim scaleCircle As New SizeF(2, 2)

            For Each id As NamedValue(Of String) In list
                If Not shapes.ContainsKey(id.Name) Then
                    Continue For
                End If

                Dim brush As Brush = id.Value.GetBrush

                With shapes(id.Name)
                    Dim name As String = .Name
                    Dim strSize = g.MeasureString(name, font)

                    For Each shape As Area In .Value
                        Dim rect As RectangleF = shape.Rectangle

                        g.FillPie(brush, rect.Scale(scaleCircle), 0, 360)
                        g.DrawCircle(rect.Centre, rect.Width, Pens.Black, fill:=False)
                    Next
                End With
            Next
        End Sub
    End Class
End Namespace