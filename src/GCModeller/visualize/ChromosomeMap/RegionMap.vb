Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
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
                         Optional geneShapeHeight% = 85) As GraphicsData

        Dim nextLength%
        Dim ff As Boolean
        Dim rightEnd%
        Dim scaleFactor#
        Dim unitLength#
        Dim startLength%
        Dim preRight#
        Dim level%
        Dim plotInternal =
            Sub(ByRef g As IGraphics, region As GraphicsRegion)

                Dim width = region.Width

                For Each gene As SegmentObject In model.GeneObjects
                    If gene.Left > nextLength Then
                        ff = True  '这个变量确保能够正确的换行，不可以修改值以及顺序！~

                        If nextLength >= model.Size Then
                            rightEnd = width - (nextLength - model.Size) * scaleFactor - 2 * margin
                        End If

                        startLength = nextLength
                        nextLength = nextLength + unitLength

                        ' 每换一行则首先绘制突变数据
                        Call drawChromosomeSites(Chr,
                                                       _start_Length:=_Start_Length,
                                                       FlagHeight:=FlagHeight,
                                                       FlagLength:=FlagLength,
                                                       GrDevice:=g,
                                                       Height:=Height,
                                                       NextLength:=nextLength,
                                                       scale:=scaleFactor)
                    End If

                    If gene.Left < preRight Then
                        level += 1
                    Else
                        level = 0
                    End If

                    If gene.Left > preRight Then preRight = gene.Right

                    gene.Height = geneShapeHeight

                    Dim drawingLociLeft As Integer = (gene.Left - startLength) * scaleFactor + margin
                    Dim drawingSize = gene.Draw(g:=g,
                                                    location:=New Point(drawingLociLeft, Height + 100 + level * 110),
                                                    factor:=scaleFactor,
                                                    RightLimited:=rightEnd,
                                                    conf:=Me.config)
                Next

            End Sub

        Return g.GraphicsPlots(
            size.SizeParser, padding,
            bg,
            plotInternal
        )
    End Function
End Module
