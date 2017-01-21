Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.csv.DocumentStream
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting

Public Module Volcano

    ReadOnly DEG_diff# = Math.Log(2, 2)
    ReadOnly diffPValue# = -Math.Log10(0.05)

    Const UP$ = "Up"
    Const DOWN$ = "Down"

    <Extension>
    Public Function PlotDEGs(genes As IEnumerable(Of EntityObject),
                             Optional size As Size = Nothing,
                             Optional margin As Size = Nothing,
                             Optional bg$ = "white",
                             Optional logFC$ = "logFC",
                             Optional pvalue$ = "P.value") As Bitmap
        Return genes.PlotDEGs(
            x:=Function(gene) gene(logFC).ParseNumeric,
            y:=Function(gene) gene(pvalue).ParseNumeric,
            size:=size,
            margin:=margin,
            bg:=bg)
    End Function

    <Extension>
    Public Function PlotDEGs(Of T)(genes As IEnumerable(Of T),
                                   x As Func(Of T, Double),
                                   y As Func(Of T, Double),
                                   Optional size As Size = Nothing,
                                   Optional margin As Size = Nothing,
                                   Optional bg$ = "white") As Bitmap

        Dim factor As Func(Of (x#, y#), String) =
            Function(DEG As (logFC#, pvalue#))
                If Math.Abs(DEG.logFC) >= DEG_diff AndAlso DEG.pvalue >= diffPValue Then
                    Return UP
                Else
                    Return DOWN
                End If
            End Function
        Dim colors As New Dictionary(Of String, Color) From {
            {UP, Color.Blue},
            {DOWN, Color.Red}
        }
        Return genes.Plot(x, y, factor, colors, size, margin, bg)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="genes"></param>
    ''' <param name="x">log2FC</param>
    ''' <param name="y">``p.value``</param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function Plot(Of T)(genes As IEnumerable(Of T),
                               x As Func(Of T, Double),
                               y As Func(Of T, Double),
                               factors As Func(Of (x#, y#), String),
                               colors As Dictionary(Of String, Color),
                               Optional size As Size = Nothing,
                               Optional margin As Size = Nothing,
                               Optional bg$ = "white",
                               Optional xlab$ = "log2 Fold Change",
                               Optional ylab$ = "-log10(p.value)",
                               Optional ptSize! = 5,
                               Optional translate As Func(Of Double, Double) = Nothing) As Bitmap

        If translate Is Nothing Then
            translate = Function(pvalue) -Math.Log10(pvalue)
        End If

        Dim array As T() = genes.ToArray
        Dim DEG_matrix As (logFC#, pvalue#)() = array.ToArray(Function(g As T) (x(g), translate(y(g))))
        Dim scaler As New Scaling(DEG_matrix)
        Dim brushes As Dictionary(Of String, Brush) = colors _
            .ToDictionary(Function(k) k.Key,
                          Function(br) DirectCast(New SolidBrush(br.Value), Brush))

        If size.IsEmpty Then
            size = New Size(2000, 2000)
        End If
        If margin.IsEmpty Then
            margin = New Size(300, 300)
        End If

        Return g.Allocate(size, margin, bg) <=
 _
            Sub(ByRef g As Graphics, region As GraphicsRegion)

                Dim scalling = scaler.TupleScaler(region)

                Call Axis.DrawAxis(g, region, scaler, True,, xlab, ylab)

                For Each gene As (x#, y#) In DEG_matrix
                    Dim color As Brush = brushes(factors(gene))
                    Dim point As PointF = scalling(gene)

                    Call g.DrawCircle(point, ptSize, color)
                Next
            End Sub
    End Function
End Module
