Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports std = System.Math
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

    <Extension>
    Public Function LogScale(exp As DataFrameRow, base As Double) As DataFrameRow
        Dim min As Double = exp.experiments _
            .Where(Function(v) v > 0 AndAlso Not v.IsNaNImaginary) _
            .DefaultIfEmpty(0) _
            .Min

        Return New DataFrameRow With {
            .geneID = exp.geneID,
            .experiments = exp.experiments _
                .Select(Function(v)
                            If v <= 0 Then
                                Return 0
                            Else
                                Return std.Log(v + 1 - min, newBase:=base)
                            End If
                        End Function) _
                .ToArray
        }
    End Function
End Module
