#Region "Microsoft.VisualBasic::7397222dd74162558d71c602f4602d17, core\Bio.Assembly\Assembly\KEGG\Web\Map\LocalRender.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    '     Class LocalRender
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: FromRepository, getAreas, GetEnumerator, GetTitle, IEnumerable_GetEnumerator
    '                   IteratesMapNames, (+3 Overloads) Rendering
    ' 
    '         Sub: renderCompound, renderGenes
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Runtime

Namespace Assembly.KEGG.WebServices

    ''' <summary>
    ''' KEGG pathway map local rendering engine
    ''' </summary>
    Public Class LocalRender : Implements IEnumerable(Of Map)

        ''' <summary>
        ''' Index by map id
        ''' </summary>
        ReadOnly mapTable As Dictionary(Of String, Map)
        ReadOnly digitMapID As Boolean

        ''' <summary>
        ''' 因为KEGG的对应物种的pathway map都是来自于标准的pathway map
        ''' 所以他们的数字id都是一样的，在这里会将id解析为数字id
        ''' </summary>
        ''' <param name="maps"></param>
        Sub New(maps As IEnumerable(Of NamedValue(Of Map)), Optional digitID As Boolean = False)
            mapTable = maps _
                .ToDictionary(Function(map)
                                  If Not digitID Then
                                      Return map.Name
                                  Else
                                      Return map.Name.Match("\d+")
                                  End If
                              End Function,
                              Function(pathway)
                                  Return pathway.Value
                              End Function)
            digitMapID = digitID
        End Sub

        Sub New(maps As Dictionary(Of String, Map))
            mapTable = maps
            digitMapID = False
        End Sub

        ''' <summary>
        ''' Get display title of the target pathway map
        ''' </summary>
        ''' <param name="mapName$"></param>
        ''' <returns></returns>
        Public Function GetTitle(mapName$) As String
            Dim map As Map = mapTable(mapName)
            Dim rect As Area = map _
                .shapes _
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
        Public Shared Function FromRepository(repo$, Optional digitID As Boolean = False) As LocalRender
            Dim maps = (ls - l - r - "*.XML" <= repo) _
                .Select(Function(path$)
                            Return New NamedValue(Of Map) With {
                                .Name = path.BaseName,
                                .Value = path.LoadXml(Of Map),
                                .Description = path
                            }
                        End Function)
            Return New LocalRender(maps, digitID)
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
                    .shapes _
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

        ''' <summary>
        ''' 解析url之中的数据，将指定的基因按照给定的颜色进行高亮显示
        ''' </summary>
        ''' <param name="url$"></param>
        ''' <param name="font"></param>
        ''' <param name="textColor$"></param>
        ''' <param name="scale$"></param>
        ''' <returns></returns>
        Public Function Rendering(url$, Optional font As Font = Nothing, Optional textColor$ = "white", Optional scale$ = "1,1") As Image
            With URLEncoder.URLParser(url)
                Return Rendering(.name, .value, font, textColor, scale)
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

            Return Rendering(mapTable(mapName), nodes, font, textColor, scale)
        End Function

        Public Shared Function Rendering(pathway As Map, nodes As IEnumerable(Of NamedValue(Of String)),
                                         Optional font As Font = Nothing,
                                         Optional textColor$ = "white",
                                         Optional scale$ = "1,1") As Image

            Dim pen As Brush = textColor.GetBrush
            Dim scaleFactor As SizeF = scale.FloatSizeParser

            Static SimSum As [Default](Of Font) = New Font(FontFace.SimSun, 10, FontStyle.Regular)

            Using g As Graphics2D = pathway _
                .GetImage _
                .CreateCanvas2D(directAccess:=True)

                Call renderGenes(g, font Or SimSum, pen, pathway, scaleFactor, nodes)
                Call renderCompound(g, font Or SimSum, pathway, scaleFactor, nodes)

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
            Dim shapes = map.shapes _
                .Where(Function(x) x.Type = type) _
                .Select(Function(x)
                            Dim titles = x.Names

                            Return x.IDVector _
                                .SeqIterator _
                                .Select(Function(cpd)
                                            Dim titleName = titles.ElementAtOrDefault(cpd)

                                            Return New NamedValue(Of Area) With {
                                                .Name = cpd.value,
                                                .Value = x,
                                                .Description = titleName.Value Or cpd.value.AsDefault
                                            }
                                        End Function)
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

                Dim rectBrush As Brush = id.Value _
                    .TranslateColor _
                    .Alpha(100) _
                    .DoCall(Function(c)
                                Return New SolidBrush(c)
                            End Function)
                Dim brush As Brush = id.Value.GetBrush

                With shapes(id.Name)
                    Dim name As String = .Name
                    Dim strSize = g.MeasureString(name, font)

                    For Each shape As Area In .Value
                        If shape.shape = "rect" Then
                            Dim rect As RectangleF = shape.Rectangle

                            Call g.FillRectangle(rectBrush, rect)
                        Else
                            Dim rect As RectangleF = shape.Rectangle _
                                .Scale(scale) _
                                .Scale(scaleCircle)

                            Call g.FillPie(brush, rect, 0, 360)
                            ' Call g.DrawCircle(rect.Centre, rect.Width, Pens.Black, fill:=False)
                        End If
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
