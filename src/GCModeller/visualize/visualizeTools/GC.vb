Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Mathematical
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcidStaticsProperty

Public Module GCPlot

    <Extension>
    Public Function PlotGCSkew(mal As FastaFile, cal As NtProperty, winSize%, steps%, Optional isCircle? As Boolean = True) As Bitmap
        Return mal.PlotGC(AddressOf NucleicAcidStaticsProperty.GCSkew, winSize, steps, isCircle)
    End Function

    <Extension>
    Public Function PlotGCContent(mal As FastaFile, cal As NtProperty, winSize%, steps%, Optional isCircle? As Boolean = True) As Bitmap
        Return mal.PlotGC(AddressOf GCContent, winSize, steps, isCircle)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mal"></param>
    ''' <param name="plot$">gcskew/gccontent</param>
    ''' <param name="winSize%"></param>
    ''' <param name="steps%"></param>
    ''' <param name="isCircle"></param>
    ''' <param name="size"></param>
    ''' <param name="margin"></param>
    ''' <param name="bg$"></param>
    ''' <param name="colors$"></param>
    ''' <param name="levels%"></param>
    ''' <returns></returns>
    <Extension>
    Public Function PlotGC(mal As FastaFile, plot$,
                           winSize%,
                           steps%,
                           Optional isCircle? As Boolean = True,
                           Optional size As Size = Nothing,
                           Optional margin As Size = Nothing,
                           Optional bg$ = "white",
                           Optional colors$ = "Jet",
                           Optional levels% = 100) As Bitmap
        If plot.TextEquals("gcskew") Then
            Return mal.PlotGC(AddressOf NucleicAcidStaticsProperty.GCSkew, winSize, steps, isCircle)
        Else
            Return mal.PlotGC(AddressOf GCContent, winSize, steps, isCircle)
        End If
    End Function

    <Extension>
    Public Function PlotGC(mal As FastaFile, cal As NtProperty,
                           winSize%,
                           steps%,
                           Optional isCircle? As Boolean = True,
                           Optional size As Size = Nothing,
                           Optional margin As Size = Nothing,
                           Optional bg$ = "white",
                           Optional colors$ = "Jet",
                           Optional levels% = 100) As Bitmap

        Dim ntArray As NamedValue(Of Double())() = LinqAPI.Exec(Of NamedValue(Of Double())) <=
 _
            From seq As FastaToken
            In mal
            Select New NamedValue(Of Double()) With {
                .Name = seq.Title,
                .x = cal(seq, winSize, steps, isCircle)
            }

        Dim tick As New Font(FontFace.SegoeUI, 12)
        Dim v%() = ntArray _
            .Select(Function(s) s.x) _
            .IteratesALL _
            .GenerateMapping(levels, offset:=0)
        Dim lvMAT As NamedValue(Of Integer())() = v _
            .Split(ntArray(Scan0).x.Length) _
            .SeqIterator _
            .ToArray(Function(i) New NamedValue(Of Integer()) With {
                .Name = ntArray(i).Name,
                .x = i.obj
            })
        Dim mapColors As Color() = New ColorMap(levels * 2 + 1) _
            .ColorSequence(colors)

        If size.IsEmpty Then
            size = New Size(30000, 10000)
        End If

        Call $"max:={v.Max}, min:={v.Min}".__DEBUG_ECHO

        Return g.GraphicsPlots(
            size, margin, bg,
            Sub(ByRef g, grect)

                Dim plotWidth = grect.PlotRegion.Width
                Dim y! = margin.Height
                Dim deltaX! = plotWidth / lvMAT(Scan0).x.Length
                Dim deltaY! = grect.PlotRegion.Height / lvMAT.Length
                Dim plotTick As Boolean = True

                For Each line As NamedValue(Of Integer()) In lvMAT

                    Dim x! = margin.Width
                    Dim bp% = 1

                    For Each d As Integer In line.x
                        If d > mapColors.Length - 1 Then
                            d = mapColors.Length - 1
                        End If

                        Dim b As New SolidBrush(mapColors(d))
                        Dim rect As New RectangleF(New PointF(x!, y!), New SizeF(deltaX, deltaY))
                        Call g.FillRectangle(b, rect)

                        If plotTick Then
                            Call g.DrawString(bp, tick, Brushes.Black, New Point(x, margin.Height - 20))
                            bp += steps
                        End If

                        x += deltaX
                    Next

                    Call g.DrawString(line.Name, tick, Brushes.Black, New PointF(1, y))

                    plotTick = False
                    y += deltaY
                Next
            End Sub)
    End Function
End Module
