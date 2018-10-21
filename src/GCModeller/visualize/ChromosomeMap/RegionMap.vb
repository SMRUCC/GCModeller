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

    Public Function Plot(model As ChromesomeDrawingModel,
                         Optional size$ = "5000,2000",
                         Optional padding$ = g.DefaultPadding,
                         Optional bg$ = "white",
                         Optional geneShapeHeight% = 85,
                         Optional locusTagFontCSS$ = CSSFont.Win7Normal) As GraphicsData

        Dim nextLength%
        Dim rightEnd%
        Dim startLength%
        Dim preRight#
        Dim level%
        Dim locusTagFont As Font = CSSFont.TryParse(locusTagFontCSS)
        Dim plotInternal =
            Sub(ByRef g As IGraphics, region As GraphicsRegion)
                Dim width = region.Width
                Dim height = region.Height
                Dim margin As Padding = region.Padding
                Dim scaleFactor# = (width - margin.Horizontal) / model.Size

                For Each gene As SegmentObject In model.GeneObjects
                    If gene.Left > nextLength Then
                        If nextLength >= model.Size Then
                            rightEnd = width - (nextLength - model.Size) * scaleFactor - margin.Horizontal
                        End If

                        startLength = nextLength
                        nextLength = nextLength

                        ' 每换一行则首先绘制突变数据
                        'Call drawChromosomeSites(Chr,
                        '                               _start_Length:=_Start_Length,
                        '                               FlagHeight:=FlagHeight,
                        '                               FlagLength:=FlagLength,
                        '                               GrDevice:=g,
                        '                               Height:=height,
                        '                               NextLength:=nextLength,
                        '                               scale:=scaleFactor)
                    End If

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
                    Dim drawingSize = gene.Draw(g:=g,
                                                    location:=New Point(drawingLociLeft, height + 100 + level * 110),
                                                    factor:=scaleFactor,
                                                    RightLimited:=rightEnd,
                                                    locusTagFont:=locusTagFont)
                Next

            End Sub

        Return g.GraphicsPlots(
            size.SizeParser, padding,
            bg,
            plotInternal
        )
    End Function
End Module
