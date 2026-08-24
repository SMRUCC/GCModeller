#Region "Microsoft.VisualBasic::9bd884ee7b758c7300ddb29ee5b7d401, analysis\HTS_matrix\Math\ExpressionScale.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 55
    '    Code Lines: 46 (83.64%)
    ' Comment Lines: 2 (3.64%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 7 (12.73%)
    '     File Size: 1.97 KB


    ' Module ExpressionScale
    ' 
    '     Function: LogScale, RelativeScale
    ' 
    ' /********************************************************************************/

#End Region

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

