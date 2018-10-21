Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Visualize.ChromosomeMap.DrawingModels

''' <summary>
''' Draw a map of the selected region
''' </summary>
Public Module RegionMap

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
    Public Function Plot(model As ChromesomeDrawingModel,
                         Optional size$ = "5000,2000",
                         Optional padding$ = g.DefaultPadding,
                         Optional bg$ = "white",
                         Optional geneShapeHeight% = 85,
                         Optional locusTagFontCSS$ = CSSFont.Win7Normal) As GraphicsData

        Dim startLength% = 0
        Dim preRight#
        Dim level%
        Dim locusTagFont As Font = CSSFont.TryParse(locusTagFontCSS)
        Dim plotInternal =
            Sub(ByRef g As IGraphics, region As GraphicsRegion)
                Dim width = region.Width
                Dim top = region.Padding.Bottom
                Dim margin As Padding = region.Padding
                Dim scaleFactor# = (width - margin.Horizontal) / model.Size

                For Each gene As SegmentObject In model.GeneObjects
                    If gene.Left < preRight Then
                        level += 1
                    Else
                        level = 0
                    End If

                    If gene.Left > preRight Then
                        preRight = gene.Right
                    End If

                    gene.Height = geneShapeHeight

                    Dim drawingLociLeft As Integer = (gene.Left - startLength) * scaleFactor + margin.Left
                    Dim Y = top + 100 + level * 110
                    Dim drawingSize = gene.Draw(
                        g:=g,
                        location:=New Point(drawingLociLeft, Y),
                        factor:=scaleFactor,
                        RightLimited:=model.Size,
                        locusTagFont:=locusTagFont
                    )
                Next

            End Sub

        Return g.GraphicsPlots(
            size.SizeParser, padding,
            bg,
            plotInternal
        )
    End Function
End Module
