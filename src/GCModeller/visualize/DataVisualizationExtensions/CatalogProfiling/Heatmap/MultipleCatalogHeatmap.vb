#Region "Microsoft.VisualBasic::39936511534a319013e310ab7cabd590, GCModeller\visualize\DataVisualizationExtensions\CatalogProfiling\Heatmap\MultipleCatalogHeatmap.vb"

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


    ' Code Statistics:

    '   Total Lines: 95
    '    Code Lines: 66
    ' Comment Lines: 14
    '   Blank Lines: 15
    '     File Size: 3.72 KB


    '     Class MultipleCatalogHeatmap
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: drawColorLegends, Z
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.MIME.Html.CSS

Namespace CatalogProfiling

    Public MustInherit Class MultipleCatalogHeatmap : Inherits MultipleCategoryProfiles

        Protected ReadOnly mapLevels As Integer
        Protected ReadOnly colorMissing As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="multiples">
        ''' the pvalue value is already been transform by 
        ''' ``-log10`` scale before call this constructor 
        ''' function
        ''' </param>
        ''' <param name="mapLevels"></param>
        ''' <param name="colorMissing"></param>
        ''' <param name="theme"></param>
        Protected Sub New(multiples As IEnumerable(Of NamedValue(Of Dictionary(Of String, BubbleTerm()))),
                          mapLevels As Integer,
                          colorMissing As String,
                          theme As Theme
            )

            Call MyBase.New(multiples, theme)
            Call Z()

            Dim orders As String() = TreeOrder.OrderByTree(Me)

            Me.mapLevels = mapLevels
            Me.colorMissing = colorMissing

            Call Me.ReOrder(orders)
        End Sub

        ''' <summary>
        ''' Z_score scale of each pathway by groups
        ''' </summary>
        Private Sub Z()
            For Each group As NamedValue(Of Dictionary(Of String, BubbleTerm())) In multiples
                Dim list = group.Value
                Dim v As New List(Of Double)
                Dim keys As String() = list.Keys.ToArray
                Dim bubbles As New List(Of BubbleTerm)

                For Each key As String In keys
                    bubbles.AddRange(list(key))
                    v.AddRange(list(key).Select(Function(b) b.PValue))
                Next

                v = New Vector(v).Z.AsList

                For i As Integer = 0 To v.Count - 1
                    bubbles(i).PValue = v(i)
                Next
            Next
        End Sub

        Protected Sub drawColorLegends(pvalues As DoubleRange, right As Double, ByRef g As IGraphics, canvas As GraphicsRegion, Optional y As Double = Double.NaN)
            Dim maps As New ColorMapLegend(palette:=theme.colorSet, mapLevels) With {
                .format = "F2",
                .noblank = False,
                .tickAxisStroke = Stroke.TryParse(theme.legendTickAxisStroke).GDIObject,
                .tickFont = CSSFont.TryParse(theme.legendTickCSS).GDIObject(g.Dpi),
                .ticks = pvalues.CreateAxisTicks,
                .title = "Z-score(-log10(pvalue))",
                .titleFont = CSSFont.TryParse(theme.legendTitleCSS).GDIObject(g.Dpi),
                .unmapColor = colorMissing,
                .ruleOffset = 5,
                .legendOffsetLeft = 5
            }
            Dim layout As New Rectangle With {
                .X = right,
                .Width = canvas.Padding.Right * (2 / 3),
                .Height = canvas.PlotRegion.Height / 2,
                .Y = If(y.IsNaNImaginary, canvas.Padding.Top, y)
            }

            Call maps.Draw(g, layout)
        End Sub
    End Class

End Namespace
