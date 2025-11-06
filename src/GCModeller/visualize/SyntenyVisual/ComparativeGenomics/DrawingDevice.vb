#Region "Microsoft.VisualBasic::39e130aff3e41648fde1a7f1806a9b08, visualize\SyntenyVisual\ComparativeGenomics\DrawingDevice.vb"

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

    '     Class DrawingDevice
    ' 
    '         Properties: DrawHeight, Font, RibbonDistance, titleFont, Type2Arrow
    ' 
    '         Function: drawBasicGenomeLayout, Plot
    ' 
    '         Sub: drawHomologousRibbon, drawRibbonColorLegend
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render
Imports Microsoft.VisualBasic.Scripting.Runtime

Namespace ComparativeGenomics

    Public Class DrawingDevice

        <DataFrameColumn> Public Property Type2Arrow As Boolean = False
        <DataFrameColumn> Public Property DrawHeight As Integer = 150
        <DataFrameColumn> Public Property Font As Font = New Font(FontFace.MicrosoftYaHei, 11)
        <DataFrameColumn> Public Property titleFont As New Font(FontFace.MicrosoftYaHei, 36)
        <DataFrameColumn> Public Property RibbonDistance As Integer = 20

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="gdi"></param>
        ''' <param name="models"></param>
        ''' <param name="height%"></param>
        ''' <param name="left%"></param>
        ''' <param name="IDDown">ID绘制的位置，对于query，位于图上部分，不需要绘制在下面，对于ref则需要绘制于下方</param>
        ''' <returns></returns>
        Private Function drawBasicGenomeLayout(gdi As IGraphics, models As GenomeModel,
                                               ByRef height%,
                                               ByRef left%,
                                               IDDown As Boolean,
                                               padding As Padding,
                                               ByRef labelYRange As IntRange) As Dictionary(Of String, Rectangle)

            Dim geneLayouts As New Dictionary(Of String, Rectangle)
            Dim overlapRegion As MapLabelLayout
            Dim labelY As New List(Of Integer)

            models.genes = (
                From gene As GeneObject
                In models.genes
                Select gene
                Order By gene.Left Ascending
            ).ToArray

            Dim css As CSSEnvirnment = gdi.LoadEnvironment
            Dim rect As New Rectangle With {
                .Location = New Point(padding.Left, height + 0.2 * DrawHeight),
                .Size = New Size(gdi.Width - padding.Horizontal(css), DrawHeight - 0.4 * DrawHeight)
            }

            Call gdi.FillRectangle(Brushes.LightGray, rect)

            Dim scaleFactor As Double = (gdi.Width - padding.Horizontal(css)) / models.Length
            Dim r As Rectangle

            left += models.First.Left * scaleFactor

            '绘制基本图形
            For i As Integer = 0 To models.Height(DrawHeight).Count - 2
                Dim gene As GeneObject = models(i)
                Dim nextGene As GeneObject = models(i + 1)

                left = gene.InvokeDrawing(
                    gdi, New Point(left, height),
                    NextLeft:=nextGene.Left,
                    scaleFactor:=scaleFactor,
                    arrowRect:=r,
                    IdDrawPositionDown:=IDDown,
                    Font:=Font,
                    AlternativeArrowStyle:=Type2Arrow,
                    overlapLayout:=overlapRegion)

                Call labelY.Add(
                    overlapRegion.OverlapRegion.Bottom,
                    overlapRegion.OverlapRegion.Top
                )
                Call geneLayouts.Add(gene.locus_tag, r)
            Next

            Call models.Last.InvokeDrawing(gdi, New Point(left, height),
                                           NextLeft:=models.Length,
                                           scaleFactor:=scaleFactor,
                                           arrowRect:=r,
                                           IdDrawPositionDown:=IDDown,
                                           Font:=Font,
                                           AlternativeArrowStyle:=Type2Arrow,
                                           overlapLayout:=overlapRegion)

            Call labelY.Add(
                overlapRegion.OverlapRegion.Bottom,
                overlapRegion.OverlapRegion.Top
            )
            Call geneLayouts.Add(models.Last.locus_tag, r)

            labelYRange = labelY

            Return geneLayouts
        End Function

        Public Function Plot(model As DrawingModel,
                             Optional canvasSize$ = "15024,2000",
                             Optional margin$ = "padding: 300px 100px 1000px 100px",
                             Optional dLabel% = 20) As GraphicsData

            Dim left, height As Integer
            Dim title$
            Dim size As SizeF
            Dim padding As Padding = margin
            Dim labelY As IntRange = Nothing

            If model.Genome1 Is Nothing OrElse model.Genome2 Is Nothing Then
                Call Console.WriteLine()
            End If

            height = padding.Top
            left = padding.Left

            Return g.GraphicsPlots(canvasSize.SizeParser, padding, "white",
                                   Sub(ByRef g, canvas)
                                       Dim layoutQuery = drawBasicGenomeLayout(g, model.Genome1, height, left, False, padding, labelY)
                                       Dim top = labelY.Min

                                       title = model.Genome1.Title
                                       size = g.MeasureString(title, titleFont)
                                       left = (g.Width - size.Width) / 2
                                       g.DrawString(title, titleFont, Brushes.Black, left, top - size.Height - dLabel)

                                       height = g.Height - DrawHeight - padding.Bottom
                                       left = padding.Left

                                       Dim layoutRef = drawBasicGenomeLayout(g, model.Genome2, height, left, True, padding, labelY)

                                       height = labelY.Max
                                       title = model.Genome2.Title
                                       size = g.MeasureString(title, titleFont)
                                       left = (g.Width - size.Width) / 2
                                       g.DrawString(title, titleFont, Brushes.Black, left, height + dLabel)

                                       Call drawHomologousRibbon(g, model, layoutQuery, layoutRef)
                                       Call drawRibbonColorLegend(g, model, top:=height + dLabel + size.Height, padding:=padding)
                                   End Sub)
        End Function

        Private Sub drawRibbonColorLegend(gdi As IGraphics, model As DrawingModel, top%, padding As Padding)
            Dim min$ = model.RibbonScoreColors.scoreRange.Min
            Dim max$ = model.RibbonScoreColors.scoreRange.Max
            Dim legendSize As New Size(350, 1000)
            Dim legend As Image = model.RibbonScoreColors _
                .profiles _
                .ColorMapLegend("Score Color", min, max,, True, lsize:=legendSize) _
                .AsGDIImage
            Dim left = gdi.Width - padding.Right - legendSize.Width - 20

            Call gdi.DrawImageUnscaled(legend, left, top)
        End Sub

        ''' <summary>
        ''' 绘制由于同源所产生的链接信息
        ''' </summary>
        Private Sub drawHomologousRibbon(gdi As IGraphics, model As DrawingModel,
                                         layoutQuery As Dictionary(Of String, Rectangle),
                                         layoutRef As Dictionary(Of String, Rectangle))

            Dim genome1 = model.Genome1.ToDictionary(Function(g) g.locus_tag)
            Dim genome2 = model.Genome2.ToDictionary(Function(g) g.locus_tag)
            Dim color As SolidBrush

            For Each link As GeneLink In model.Links
                Dim r1 As Rectangle = layoutQuery(link.genome1)
                Dim r2 As Rectangle = layoutRef(link.genome2)
                Dim path2D As New GraphicsPath
                Dim p1, p2, p3, p4 As Point

                p1 = New Point(r1.Location.X, r1.Location.Y + r1.Height + RibbonDistance)
                p2 = New Point(r1.Right, r1.Top + r1.Height + RibbonDistance)

                If genome1(link.genome1).Direction < 0 Then
                    Call p1.Swap(p2)
                End If

                p3 = New Point(r2.Right, r2.Top - RibbonDistance)
                p4 = New Point(r2.Location.X, r2.Location.Y - RibbonDistance)

                If genome2(link.genome2).Direction < 0 Then
                    Call p3.Swap(p4)
                End If

                color = New SolidBrush(link.Color Or defaultColor)

                Call path2D.AddLine(p1, p2)
                Call path2D.AddLine(p2, p3)
                Call path2D.AddLine(p3, p4)
                Call path2D.AddLine(p4, p1)

                Call gdi.FillPath(color, path2D)
            Next
        End Sub
    End Class
End Namespace
