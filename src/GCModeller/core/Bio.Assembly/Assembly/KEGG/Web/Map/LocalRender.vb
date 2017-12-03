#Region "Microsoft.VisualBasic::e6288210130c06fcd3dce0006dec704b, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\Web\Map\LocalRender.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region


Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Runtime

Namespace Assembly.KEGG.WebServices

    ''' <summary>
    ''' KEGG pathway map local rendering engine
    ''' </summary>
    Public Class LocalRender : Implements IEnumerable(Of Map)

        ReadOnly mapTable As Dictionary(Of String, Map)

        Sub New(maps As IEnumerable(Of NamedValue(Of Map)))
            mapTable = maps.ToDictionary(
                Function(map) map.Name,
                Function(pathway) pathway.Value)
        End Sub

        ''' <summary>
        ''' Get display title of the target pathway map
        ''' </summary>
        ''' <param name="mapName$"></param>
        ''' <returns></returns>
        Public Function GetTitle(mapName$) As String
            Dim map As Map = mapTable(mapName)
            Dim rect As Area = map _
                .Areas _
                .Where(Function(ar)
                           Return ar.shape.TextEquals("rect") AndAlso ar.IDVector.IndexOf(mapName) > -1
                       End Function) _
                .FirstOrDefault

            If rect Is Nothing Then
                Return mapName
            Else
                Return rect.title
            End If
        End Function

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

        ''' <summary>
        ''' 函数返回``{mapName -> idlist}``，其中idlist为产生交集的编号列表
        ''' </summary>
        ''' <param name="list$"></param>
        ''' <param name="threshold%"></param>
        ''' <returns></returns>
        Public Iterator Function IteratesMapNames(list$(), Optional threshold% = 1) As IEnumerable(Of NamedValue(Of String()))
            For Each map As String In Me.mapTable.Keys
                Dim id$() = mapTable(map) _
                    .Areas _
                    .Select(Function(ar) ar.IDVector) _
                    .IteratesALL _
                    .ToArray
                Dim intersects = list _
                    .Intersect(id) _
                    .ToArray

                If intersects.Length >= threshold Then
                    Yield New NamedValue(Of String()) With {
                        .Name = map,
                        .Value = intersects
                    }
                End If
            Next
        End Function

        Public Function Rendering(url$, Optional font As Font = Nothing, Optional textColor$ = "white", Optional scale$ = "1,1") As Image
            With URLEncoder.URLParser(url)
                Return Rendering(.Name, .Value, font, textColor, scale)
            End With
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="mapName$"></param>
        ''' <param name="nodes">``{id -> color}``</param>
        ''' <param name="font"></param>
        ''' <param name="textColor$"></param>
        ''' <param name="scale$"></param>
        ''' <returns></returns>
        Public Function Rendering(mapName$,
                                  nodes As IEnumerable(Of NamedValue(Of String)),
                                  Optional font As Font = Nothing,
                                  Optional textColor$ = "white",
                                  Optional scale$ = "1,1") As Image

            Dim pathway As Map = mapTable(mapName)
            Dim pen As Brush = textColor.GetBrush
            Dim scaleFactor As SizeF = scale.FloatSizeParser

            If font Is Nothing Then
                font = New Font(FontFace.SimSun, 10, FontStyle.Regular)
            End If

            Using g As Graphics2D = pathway _
                .GetImage _
                .CreateCanvas2D(directAccess:=True)

                Call renderGenes(g, font, pen, pathway, scaleFactor, nodes)
                Call renderCompound(g, font, pathway, scaleFactor, nodes)

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
                            Return x.IDVector _
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
                                      .Name = name,
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
        Private Shared Sub renderCompound(ByRef g As Graphics2D, font As Font, map As Map, scale As SizeF, list As NamedValue(Of String)())
            Dim shapes = getAreas(map, "Compound")
            Dim scaleCircle As New SizeF(2, 2)  ' 通用的缩放

            For Each id As NamedValue(Of String) In list
                If Not shapes.ContainsKey(id.Name) Then
                    Continue For
                End If

                Dim brush As Brush = id.Value.GetBrush

                With shapes(id.Name)
                    Dim name As String = .Name
                    Dim strSize = g.MeasureString(name, font)

                    For Each shape As Area In .Value
                        Dim rect As RectangleF = shape.Rectangle _
                            .Scale(scale) _
                            .Scale(scaleCircle)

                        g.FillPie(brush, rect, 0, 360)
                        g.DrawCircle(rect.Centre, rect.Width, Pens.Black, fill:=False)
                    Next
                End With
            Next
        End Sub

        Public Iterator Function GetEnumerator() As IEnumerator(Of Map) Implements IEnumerable(Of Map).GetEnumerator
            For Each map As Map In Me.mapTable.Values
                Yield map
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
