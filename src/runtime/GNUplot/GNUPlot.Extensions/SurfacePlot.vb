Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges

Public Module SurfacePlot

    <Extension>
    Public Function ScatterHeatmap(fun As Func(Of Double, Double, (z#, color#)),
                                   xrange As DoubleRange,
                                   yrange As DoubleRange,
                                   Optional xn% = 100,
                                   Optional yn% = 100) As Bitmap

    End Function
End Module
