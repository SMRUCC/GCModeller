#Region "Microsoft.VisualBasic::2d1464ab317cdea27ec0f33c88a6c5e7, visualize\DataVisualizationExtensions\CatalogProfiling\Heatmap\MultipleCatalogHeatmap.vb"

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

    '   Total Lines: 112
    '    Code Lines: 76 (67.86%)
    ' Comment Lines: 16 (14.29%)
    '    - Xml Docs: 81.25%
    ' 
    '   Blank Lines: 20 (17.86%)
    '     File Size: 4.36 KB


    '     Class MultipleCatalogHeatmap
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: getPathways
    ' 
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
Imports Microsoft.VisualBasic.MIME.Html.Render

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
                          rankOrder As Boolean,
                          theme As Theme
            )

            Call MyBase.New(multiples, theme)
            Call Z()

            Me.mapLevels = mapLevels
            Me.colorMissing = colorMissing

            If rankOrder Then
                Call Me.ReOrder(TreeOrder.OrderByTree(Me))
            End If
        End Sub

        Protected Overloads Function getPathways(groupUnions As Integer, unionAll As Integer) As Dictionary(Of String, String())
            Dim pathwayUnions = MyBase.getPathways()

            If groupUnions > 0 Then
                ' get most variant pathway in each category

            Else
                ' get most variant pathway in all data list

            End If

            Return pathwayUnions
        End Function

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
            Dim css As CSSEnvirnment = g.LoadEnvironment
            Dim padding As PaddingLayout = PaddingLayout.EvaluateFromCSS(css, canvas.Padding)
            Dim maps As New ColorMapLegend(palette:=theme.colorSet, mapLevels) With {
                .format = "F2",
                .noblank = False,
                .tickAxisStroke = css.GetPen(Stroke.TryParse(theme.legendTickAxisStroke)),
                .tickFont = css.GetFont(CSSFont.TryParse(theme.legendTickCSS)),
                .ticks = pvalues.CreateAxisTicks,
                .title = "Z-score(-log10(pvalue))",
                .titleFont = css.GetFont(CSSFont.TryParse(theme.legendTitleCSS)),
                .unmapColor = colorMissing,
                .ruleOffset = 5,
                .legendOffsetLeft = 5
            }
            Dim layout As New Rectangle With {
                .X = right,
                .Width = padding.Right * (2 / 3),
                .Height = canvas.PlotRegion(css).Height / 2,
                .Y = If(y.IsNaNImaginary, padding.Top, y)
            }

            Call maps.Draw(g, layout)
        End Sub
    End Class

End Namespace
