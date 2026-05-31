Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports std_vec = Microsoft.VisualBasic.Math.LinearAlgebra.Vector

Public Module ExpressionScale

    <Extension>
    Public Function RelativeScale(gene As DataFrameRow, Optional median As Boolean = False) As DataFrameRow
        Dim factor As Double = If(median, gene.experiments.Median, gene.experiments.Max)

        If median AndAlso factor = 0.0 Then
            Dim minmax As DoubleRange = gene.experiments

            ' try to avoid divid zero
            If minmax.Length = 0 Then
                ' all zero
                Return New DataFrameRow With {
                    .geneID = gene.geneID,
                    .experiments = gene.experiments.ToArray
                }
            Else
                factor = minmax.Max / 2
            End If
        End If

        Return New DataFrameRow With {
            .geneID = gene.geneID,
            .experiments = New std_vec(gene.experiments) / factor
        }
    End Function
End Module
