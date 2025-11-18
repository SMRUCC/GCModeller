#Region "Microsoft.VisualBasic::0d09cc80cc8e4376e8df29ae89710d99, visualize\ChromosomeMap\RegionMap.vb"

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

'   Total Lines: 143
'    Code Lines: 114 (79.72%)
' Comment Lines: 15 (10.49%)
'    - Xml Docs: 86.67%
' 
'   Blank Lines: 14 (9.79%)
'     File Size: 6.09 KB


' Module RegionMap
' 
'     Function: Plot
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Visualize.ChromosomeMap.DrawingModels

#If NET48 Then
Imports Pen = System.Drawing.Pen
Imports Pens = System.Drawing.Pens
Imports Brush = System.Drawing.Brush
Imports Font = System.Drawing.Font
Imports Brushes = System.Drawing.Brushes
Imports SolidBrush = System.Drawing.SolidBrush
Imports DashStyle = System.Drawing.Drawing2D.DashStyle
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports GraphicsPath = System.Drawing.Drawing2D.GraphicsPath
Imports FontStyle = System.Drawing.FontStyle
#Else
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports Pens = Microsoft.VisualBasic.Imaging.Pens
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports DashStyle = Microsoft.VisualBasic.Imaging.DashStyle
Imports Image = Microsoft.VisualBasic.Imaging.Image
Imports Bitmap = Microsoft.VisualBasic.Imaging.Bitmap
Imports GraphicsPath = Microsoft.VisualBasic.Imaging.GraphicsPath
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
#End If

''' <summary>
''' Draw a map of the selected region
''' </summary>
Public Class RegionMap : Inherits Plot

    ReadOnly model As ChromesomeDrawingModel
    ReadOnly geneShapeHeight%
    ReadOnly disableLevelSkip As Boolean
    ReadOnly referenceLineStroke$
    ReadOnly drawShapeStroke As Boolean

    Private Sub New(model As ChromesomeDrawingModel, geneShapeHeight%, disableLevelSkip As Boolean, drawShapeStroke As Boolean, referenceLineStroke$, theme As Theme)
        MyBase.New(theme)

        Me.drawShapeStroke = drawShapeStroke
        Me.geneShapeHeight = geneShapeHeight
        Me.disableLevelSkip = disableLevelSkip
        Me.referenceLineStroke = referenceLineStroke
        Me.model = model
    End Sub

    Protected Overrides Sub PlotInternal(ByRef g As IGraphics, region As GraphicsRegion)
        Dim css As CSSEnvirnment = g.LoadEnvironment
        Dim margin As PaddingLayout = PaddingLayout.EvaluateFromCSS(css, region.Padding)
        Dim startLength% = 0
        Dim preRight#
        Dim level%
        Dim width = region.Width
        Dim top = margin.Top
        Dim scaleFactor# = (width - margin.Horizontal) / model.Size
        Dim pos As Point
        Dim locusTagFont As Font = css.GetFont(CSSFont.TryParse(theme.tagCSS))
        Dim legendFont As Font = css.GetFont(CSSFont.TryParse(theme.legendLabelCSS))

        If disableLevelSkip Then
            ' 如果都绘制在一条线上面的画，则会绘制一条水平的参考线
            Dim left As New Point(margin.Left, top + 100 + geneShapeHeight / 2)
            Dim right As New Point(width - margin.Right, left.Y)
            Dim refLineStroke As Pen = css.GetPen(Stroke.TryParse(referenceLineStroke))

            Call g.DrawLine(refLineStroke, left, right)
        End If

        For Each gene As SegmentObject In model.GeneObjects

            If disableLevelSkip Then
                If gene.Left < preRight Then
                    gene.Left = preRight
                Else
                    level = 0
                End If
            Else
                If gene.Left < preRight Then
                    ' 前后的位置有冲突，则变化下一个基因图形的位置
                    level += 1
                Else
                    level = 0
                End If
            End If

            If gene.Left > preRight Then
                preRight = gene.Right
            End If

            gene.Height = geneShapeHeight

            Dim drawingLociLeft As Integer = (gene.Left - startLength) * scaleFactor + margin.Left
            Dim Y = top + 100 + level * 110
            Dim drawingSize As Size = gene.Draw(
                g:=g,
                location:=New Point(drawingLociLeft, Y),
                factor:=scaleFactor,
                RightLimited:=model.Size,
                locusTagFont:=locusTagFont,
                drawLocusTag:=theme.drawLabels,
                drawShapeStroke:=drawShapeStroke
            )
        Next

        pos = New Point With {
            .X = margin.Left,
            .Y = region.Height - margin.Bottom * 2
        }

        Call g.DrawingCOGColors(
            model.COGs,
            ref:=pos,
            legendFont:=legendFont,
            width:=width,
            margin:=margin.Left
        )
    End Sub

    ''' <summary>
    ''' 只绘制一个局部的区域图形，所以不会出现换行的情况
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="size$"></param>
    ''' <param name="padding$"></param>
    ''' <param name="bg$"></param>
    ''' <param name="geneShapeHeight%"></param>
    ''' <param name="locusTagFontCSS$"></param>
    ''' <returns></returns>
    Public Overloads Shared Function PlotRegion(model As ChromesomeDrawingModel,
                                                Optional size$ = "5000,2000",
                                                Optional padding$ = g.DefaultPadding,
                                                Optional bg$ = "white",
                                                Optional geneShapeHeight% = 85,
                                                Optional locusTagFontCSS$ = CSSFont.Win7Normal,
                                                Optional disableLevelSkip As Boolean = False,
                                                Optional referenceLineStroke$ = "stroke: black; stroke-width: 8px; stroke-dash: solid;",
                                                Optional drawLocusTag As Boolean = False,
                                                Optional drawShapeStroke As Boolean = False,
                                                Optional legendFontCSS$ = CSSFont.Win7Large,
                                                Optional dpi As Integer = 100,
                                                Optional driver As Drivers = Drivers.Default) As GraphicsData
        Dim theme As New Theme With {
            .padding = padding,
            .background = bg,
            .tagCSS = locusTagFontCSS,
            .drawLabels = drawLocusTag,
            .legendLabelCSS = legendFontCSS
        }
        Dim map As New RegionMap(model, geneShapeHeight, disableLevelSkip, drawShapeStroke, referenceLineStroke$, theme)

        Return map.Plot(size.FloatSizeParser, dpi:=dpi, driver:=driver)
    End Function
End Class
