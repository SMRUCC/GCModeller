Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Public Module Volcano

    ReadOnly DEG_diff# = Math.Log(2, 2)

    Const UP$ = "Up"
    Const DOWN$ = "Down"

    <Extension>
    Public Function PlotDEGs(Of T)(genes As IEnumerable(Of T),
                                   x As Func(Of T, Double),
                                   y As Func(Of T, Double),
                                   Optional size As Size = Nothing,
                                   Optional margin As Size = Nothing,
                                   Optional bg$ = "white") As Bitmap

        Dim factor As Func(Of (x#, y#), String) =
            Function(DEG As (logFC#, pvalue#))
                If Math.Abs(DEG.logFC) >= DEG_diff AndAlso DEG.pvalue <= 0.05 Then
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
                               Optional ylab$ = "-log10(p.value)") As Bitmap

        Dim array As T() = genes.ToArray
        Dim DEG_matrix As (logFC#, pvalue#)() = array.ToArray(Function(g As T) (x(g), y(g)))
        Dim scaler As New Scaling(DEG_matrix)

        Return g.Allocate(size, margin, bg) <=
 _
            Sub(ByRef g As Graphics, region As GraphicsRegion)

                Call Axis.DrawAxis(g, region, scaler, True,, xlab, ylab)

            End Sub
    End Function
End Module
