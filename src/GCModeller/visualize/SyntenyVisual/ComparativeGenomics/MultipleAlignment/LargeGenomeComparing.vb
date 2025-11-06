#Region "Microsoft.VisualBasic::3ffe78aad6d40901092cbe4105b894ad, visualize\SyntenyVisual\ComparativeGenomics\MultipleAlignment\LargeGenomeComparing.vb"

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

'     Module LargeGenomeComparing
' 
'         Function: __drawing, InvokeDrawing
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Imaging.SVG.XML
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData

#If NET48 Then
Imports Brush = System.Drawing.Brush
Imports SolidBrush = System.Drawing.SolidBrush
Imports Brushes = System.Drawing.Brushes
Imports FontStyle = System.Drawing.FontStyle
#Else
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
#End If

Namespace ComparativeAlignment

    ''' <summary>
    ''' 对于很大的基因组，则直接进行画点和划线来代替原来的orf箭头以及图层区域了
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <[Namespace]("Comparative.alignment.genome.large_size")>
    Module LargeGenomeComparing

        ''' <summary>
        ''' 两个基因组之间在绘制的时候的间隔的大小
        ''' </summary>
        ''' <remarks></remarks>
        <DataFrameColumn("gene.drawing.height.interval")>
        Dim GenomeInterval As Double = 120
        <DataFrameColumn("margin")>
        Dim Margin As Double = 200


        <DataFrameColumn("triangle.length")> Dim TrangleLength As Integer = 60
        <DataFrameColumn("triangle.height")> Dim TrangleHeight As Integer = 100

        Private Function __drawing(models As ComparativeGenomics.GenomeModel,
                                   gdi As IGraphics,
                                   len As Integer,
                                   maxLenTitleSize As SizeF,
                                   height As Integer,
                                   titleFont As Font,
                                   Font As Font,
                                   DispGeneID As Boolean,
                                   DisplayRule As Boolean) As Dictionary(Of String, Rectangle) '绘制基本的ORF信息

            Dim gDrawingRegions As New Dictionary(Of String, Rectangle)
            Dim convertFactor As Double = len / models.Length
            Dim RegionLeft As Double = 1.3 * Margin + maxLenTitleSize.Width
            Dim rect As New Rectangle(New Point(RegionLeft, height), New Size(len, 10))

            gdi.FillRectangle(New SolidBrush(Color.FromArgb(200, Color.Black)), rect)
            models.genes = models.genes.OrderBy(Function(g) g.Left).ToArray

            Dim preLeft As Integer = 0
            Dim LegendHeightOffset As Integer = 20
            Dim LegendSize As SizeF = gdi.MeasureString(models.SegmentOffset, Font)
            Dim locusFont As New Font(FontFace.MicrosoftYaHei, sizeOfLocus)
            Dim loci As New PointF(RegionLeft - 0.5 * LegendSize.Width, height + LegendHeightOffset)

            If DisplayRule Then
                gdi.DrawString(models.SegmentOffset, Font, Brushes.Black, loci)
            End If

            Dim dth As Integer = TrangleHeight / 2
            Dim IdRegion As New RectangleF

            For i As Integer = 0 To models.Count - 1   '绘制基本图形
                Dim gene As ComparativeGenomics.GeneObject = models(i)

                RegionLeft += (gene.Left - preLeft) * convertFactor
                preLeft = gene.Left

                Dim rtvlRegion As New Rectangle(New Point(RegionLeft, height - dth), New Size(2, TrangleHeight))
                Dim sz As SizeF = gene.locus_tag.MeasureSize(gdi, locusFont)
                Dim r As New RectangleF(New Point(rtvlRegion.Left, rtvlRegion.Bottom + 3), sz)

                If r.Left <= IdRegion.Right Then
                    r = New RectangleF(New Point(rtvlRegion.Left, IdRegion.Bottom + 3), sz)
                End If

                IdRegion = r

                If DispGeneID Then Call gdi.DrawString(gene.locus_tag, locusFont, Brushes.DarkCyan, r.Location)

                Dim TrModel As New GraphicsPath
                Dim v1 As Point = New Point(RegionLeft, height - dth)
                Dim v2 As Point = New Point(RegionLeft, height + dth)
                Dim v3 As Point

                If gene.Direction > 0 Then    ' 正向，向右的箭头
                    v3 = New Point(RegionLeft + TrangleLength, height)
                Else
                    v3 = New Point(RegionLeft - TrangleLength, height)
                End If

                Call TrModel.AddLine(v1, v2)
                Call TrModel.AddLine(v2, v3)
                Call TrModel.AddLine(v3, v1)

                If gene.Color Is Nothing Then
                    Call gdi.FillPath(Brushes.BlueViolet, TrModel)
                Else
                    Call gdi.FillPath(gene.Color, TrModel)
                End If

                Call gDrawingRegions.Add(gene.locus_tag, rtvlRegion)
            Next

            preLeft = models.SegmentOffset + models.Length
            LegendSize = gdi.MeasureString(preLeft, Font)

            If DisplayRule Then
                loci = New Point(1.3 * Margin + maxLenTitleSize.Width + len - 0.5 * LegendSize.Width, height + LegendHeightOffset)
                gdi.DrawString(preLeft, Font, Brushes.Black, loci)
            End If

            height = height - "0".MeasureSize(gdi, titleFont).Height / 2
            gdi.DrawString(models.Title.Split.First, titleFont, Brushes.Black, New Point(Margin - 45, height))

            Dim spFont As New Font(titleFont.Name, titleFont.Size)
            loci = New Point(Margin - 45 + models.Title.Split.First.MeasureSize(gdi, titleFont).Width + 2, height)
            gdi.DrawString(Mid(models.Title, Strings.Len(models.Title.Split.First) + 1), spFont, Brushes.Black, loci)

            Return gDrawingRegions
        End Function

        <DataFrameColumn("convert.factor")>
        Dim InternalConvertFactor As Double = 0.0005
        <DataFrameColumn("font.size")>
        Dim FontSize As Integer = 20

        <DataFrameColumn("pen.width")>
        Dim PenWidth As Integer = 5
        <DataFrameColumn("gene_id.font.size")>
        Dim sizeOfLocus As Integer = 8

        Private Class PlotAlignment : Inherits Plot

            ReadOnly model As DrawingModel,
                                      DispGeneID As Boolean = True,
                                     DisplayRule As Boolean = True

            ReadOnly maxLenTitleSize As SizeF

            Public Sub New(model As DrawingModel,
                           DispGeneID As Boolean,
                           DisplayRule As Boolean,
                           maxLenTitleSize As SizeF,
                           theme As Theme)

                MyBase.New(theme)

                Me.model = model
                Me.DispGeneID = DispGeneID
                Me.DisplayRule = DisplayRule
                Me.maxLenTitleSize = maxLenTitleSize
            End Sub

            Protected Overrides Sub PlotInternal(ByRef g As IGraphics, canvas As GraphicsRegion)
                Dim Height As Integer = Margin
                Dim Length As Integer = g.Width - 3 * Margin - maxLenTitleSize.Width + 20  '基因组的绘制区域的长度已经被固定下来了
                Dim tagFont As New Font(FontFace.MicrosoftYaHei, FontSize)
                Dim titleFont As New Font("Microsoft YaHei", 32, FontStyle.Italic)

                Height += 3 * Margin + GenomeInterval

                Dim RECT_ALIGN As Dictionary(Of String, Rectangle)
                Dim RECT_1 As Dictionary(Of String, Rectangle) =
                    __drawing(model.Query,
                              g,
                              Length,
                              maxLenTitleSize,
                              Height,
                              titleFont,
                              tagFont,
                              DispGeneID,
                              DisplayRule)

                Dim linkGroups = From goLinks As ComparativeGenomics.GeneLink
                                 In model.links
                                 Select goLinks
                                 Group By goLinks.genome1 Into Group
                Dim Links As List(Of ComparativeGenomics.GeneLink) =
                    LinqAPI.MakeList(Of ComparativeGenomics.GeneLink) <= From LinkGroup
                                                                         In linkGroups
                                                                         Let id As String() = LinkGroup.Group.Select(Function(lnk) lnk.genome2)
                                                                         Let CombLocus = Comb(Of String).CreateCompleteObjectPairs(id)
                                                                         Select From pairs
                                                                                In CombLocus
                                                                                Select From lnk
                                                                                       In pairs
                                                                                       Select New ComparativeGenomics.GeneLink With {
                                                                                           .genome1 = lnk.Item1,
                                                                                           .genome2 = lnk.Item2
                                                                                       }
                Links += model.links

                Dim Up As Boolean = False
                Dim dHeight As Integer = 2.5 * GenomeInterval  '相邻的两个基因组之间的绘图的间隔距离
                Dim Height1 As Integer = Height - dHeight
                Dim Height2 As Integer = Height + dHeight
                Dim LastRegion1 As Dictionary(Of String, Rectangle) = RECT_1
                Dim LastRegion2 As Dictionary(Of String, Rectangle) = RECT_1
                Dim LinkPen As New Pen(Brushes.Brown, PenWidth)

                For Each hit As ComparativeGenomics.GenomeModel In model.aligns
                    If Up Then
                        Height = Height1
                        Height1 -= dHeight
                    Else
                        Height = Height2
                        Height2 += dHeight
                    End If

                    RECT_ALIGN = __drawing(hit, g, Length, maxLenTitleSize, Height, titleFont, tagFont, DispGeneID, DisplayRule)

                    Dim GNModel___1 = model.Query.ToDictionary(Function(gene) gene.locus_tag),
                        GNModel___2 = model.aligns.First.ToDictionary(Function(gene) gene.locus_tag)

                    ' On Error Resume Next

                    If Not Up Then
                        RECT_1 = LastRegion1
                    Else
                        RECT_1 = LastRegion2
                    End If

                    For Each lnk As ComparativeGenomics.GeneLink In Links   ' 绘制连接信息
                        Dim rect As Rectangle

                        If RECT_1.ContainsKey(lnk.genome1) Then
                            rect = RECT_1(lnk.genome1)
                        Else
                            Continue For
                        End If

                        Dim rectAlign As Rectangle
                        If RECT_ALIGN.ContainsKey(lnk.genome2) Then
                            rectAlign = RECT_ALIGN(lnk.genome2)
                        Else
                            Continue For
                        End If

                        Dim p1, p2, p3, p4 As Point
                        p1 = New Point(rect.Location.X, rect.Location.Y + rect.Height + 3)
                        p2 = New Point(rect.Right, rect.Top + rect.Height + 3)

                        If GNModel___1(lnk.genome1).Direction < 0 Then
                            Call p1.Swap(p2)
                        End If

                        p3 = New Point(rectAlign.Right, rectAlign.Top - 3)
                        p4 = New Point(rectAlign.Location.X, rectAlign.Location.Y - 3)

                        If GNModel___2(lnk.genome2).Direction < 0 Then
                            Call p3.Swap(p4)
                        End If

                        Call g.DrawLine(LinkPen, p1, p4)
                    Next

                    If Not Up Then
                        LastRegion1 = RECT_ALIGN
                    Else
                        LastRegion2 = RECT_ALIGN
                    End If
                Next

                Dim X, Y As Integer
                X = g.Width * 0.25
                Y = g.Height - Margin - "0".MeasureSize(g, titleFont).Height
                titleFont = New Font(FontFace.MicrosoftYaHei, 30, FontStyle.Bold)

                Dim gfx = g
                Dim __drawTrangle = Sub(color As Color, Direction As Integer)
                                        Dim Tr As New GraphicsPath
                                        Dim X1 As Integer = X + Direction * TrangleLength

                                        Call Tr.AddLine(New Point(X, Y), New Point(X1, Y - TrangleLength / 2))
                                        Call Tr.AddLine(New Point(X1, Y - TrangleLength / 2), New Point(X1, Y + TrangleLength / 2))
                                        Call Tr.AddLine(New Point(X1, Y + TrangleLength / 2), New Point(X, Y))

                                        Call gfx.FillPath(New SolidBrush(color), Tr)
                                    End Sub

                For Each cl In model.Legends.Values

                    Call __drawTrangle(cl.color, 1)
                    X += TrangleLength * 2 + 10
                    Call __drawTrangle(cl.color, -1)
                    Call g.DrawString(cl.type, titleFont, Brushes.Black, New PointF(X + TrangleLength + 15, Y - TrangleLength / 2))
                    X += 10 + cl.type.MeasureSize(g, titleFont).Width + TrangleLength * 2 + 10 + 10
                Next
            End Sub
        End Class

        ''' <summary>
        ''' 绘制对比对图
        ''' </summary>
        ''' <param name="model"></param>
        ''' <returns></returns>
        ''' <remarks>If the parameter color_overrides is not null then all of the gene color will overrides the cog color as the parameter specific color.</remarks>
        Public Function InvokeDrawing(model As DrawingModel,
                                      <Parameter("GeneID.Disp")> Optional DispGeneID As Boolean = True,
                                      <Parameter("Rule.Disp")> Optional DisplayRule As Boolean = True) As GraphicsData

            Dim tagFont As New Font(FontFace.MicrosoftYaHei, FontSize)
            Dim titleFont As New Font("Microsoft YaHei", 32, FontStyle.Italic)
            Dim maxLenTitle As String = model.EnumerateTitles.OrderByDescending(Function(s) Len(s)).First
            Dim maxLenTitleSize As SizeF = DriverLoad.MeasureTextSize(maxLenTitle, titleFont) '得到最长的标题字符串作为基本的绘制长度的标准
            Dim devSize As New Size(Margin * 10 + model.Query.Length * InternalConvertFactor + maxLenTitleSize.Width * 2, 5 * Margin + model.aligns.Count * (GenomeInterval + 400))
            Dim theme As New Theme
            Dim app As New PlotAlignment(model, DispGeneID, DisplayRule, maxLenTitleSize, theme)
            Return app.Plot($"{devSize.Width},{devSize.Height}")
        End Function
    End Module
End Namespace
