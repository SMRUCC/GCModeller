Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Public Module PatternPlotExtensions

    Public Function DrawMatrix(raw As Matrix,
                               Optional dim$ = "3,3",
                               Optional size$ = "2400,2100",
                               Optional padding$ = g.DefaultPadding,
                               Optional bg$ = "white") As GraphicsData

        Return ExpressionPattern.CMeansCluster(raw, dim$.SizeParser.ToArray).DrawMatrix(
            size:=size,
            padding:=padding,
            bg:=bg
        )
    End Function

    <Extension>
    Public Function DrawMatrix(matrix As ExpressionPattern,
                               Optional size$ = "2400,2100",
                               Optional padding$ = g.DefaultPadding,
                               Optional bg$ = "white",
                               Optional title$ = "Expression Patterns",
                               Optional xlab$ = "time groups",
                               Optional ylab$ = "expression quantification") As GraphicsData

        Dim theme As New Theme With {
            .background = bg
        }

        Return New PatternPlot(theme) With {
            .main = title,
            .matrix = matrix,
            .xlabel = xlab,
            .ylabel = ylab
        }.Plot(size, padding)
    End Function
End Module