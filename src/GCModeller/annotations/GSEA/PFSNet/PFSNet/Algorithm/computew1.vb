#Region "Microsoft.VisualBasic::7e4c98ffedd95ec64fac29b2a3b80d86, GCModeller\annotations\GSEA\PFSNet\PFSNet\Algorithm\computew1.vb"

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

    '   Total Lines: 88
    '    Code Lines: 56
    ' Comment Lines: 27
    '   Blank Lines: 5
    '     File Size: 3.47 KB


    ' Module computew1Function
    ' 
    '     Function: computew1, getFuzzyWeight
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Correlations
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.PFSNet.R

<HideModuleName> Module computew1Function

    ''' <summary>
    ''' ```R
    ''' ranks&lt;-apply(expr,2,function(x){
    '''	   rank(x)/length(x)
    ''' })
    ''' ```
    ''' 
    ''' apply函数之中的MARGIN参数的含义：
    ''' MARGIN	
    ''' a vector giving the subscripts which the function will be applied over. E.g., for a matrix 1 indicates rows, 2 indicates columns, c(1, 2) indicates rows and columns. 
    ''' Where X has named dimnames, it can be a character vector selecting dimension names.
    ''' 即上述的R函数是对矩阵之中的每一列进行计算
    ''' </summary>
    ''' <param name="expr"></param>
    ''' <param name="theta1"></param>
    ''' <param name="theta2"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function computew1(expr As DataFrameRow(), theta1 As Double, theta2 As Double) As DataFrameRow()
        Dim ranks As Vector() = expr _
            .Select(Function(r) r.experiments.AsVector) _
            .Apply(math:=Function(x)
                             Dim data As Double() = x.ToArray
                             Dim rankVec As Vector = data.Ranking(Strategies.OrdinalRanking).AsVector
                             Return rankVec / data.Length
                         End Function,
                   axis:=ApplyOnAxis.Column,
                   aggregate:=Function(x) x.Select(Function(v) v.AsVector)) _
            .ToArray
        Dim result = ranks _
            .Apply(math:=Function(x)
                             Dim q2 = Quantile.Threshold(x, theta2)
                             Dim q1 = Quantile.Threshold(x, theta1)
                             Dim m = x.Median
                             Dim mx = x.Max
                             Dim delta_q12 As Double = q1 - q2

                             Return x.Select(Function(y) getFuzzyWeight(y, q1, q2, delta_q12))
                         End Function,
                   axis:=ApplyOnAxis.Row,
                   aggregate:=Function(x)
                                  Return x.Select(Function(v) v.ToArray)
                              End Function) _
            .MatrixTranspose _
            .ToArray

        Return expr _
            .Select(Function(r, i)
                        Return New DataFrameRow With {
                            .geneID = r.geneID,
                            .experiments = result(i)
                        }
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' 计算模糊权重
    ''' </summary>
    ''' <param name="y"></param>
    ''' <param name="q1"></param>
    ''' <param name="q2"></param>
    ''' <param name="delta_q12"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getFuzzyWeight(y As Double, q1 As Double, q2 As Double, delta_q12 As Double) As Double
        If y.IsNaNImaginary Then
            Return Double.NaN
        ElseIf y >= q1 Then
            Return 1
        ElseIf y >= q2 Then
            Return (y - q2) / delta_q12
        Else
            Return 0
        End If
    End Function
End Module
