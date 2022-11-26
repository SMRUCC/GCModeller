#Region "Microsoft.VisualBasic::ef1afee280f3efa2307e001efc1ea430, GCModeller\annotations\GSEA\GSVA\C\kernel_estimation.vb"

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

    '   Total Lines: 148
    '    Code Lines: 97
    ' Comment Lines: 19
    '   Blank Lines: 32
    '     File Size: 5.07 KB


    '     Module kernel_estimation
    ' 
    '         Function: _precomputed_cdf, matrix_density_R, ppois, row_d, sd1
    ' 
    '         Sub: initCdfs
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math.Distributions
Imports stdNum = System.Math

Namespace C

    Module kernel_estimation

        Public Function matrix_density_R(X As Double()(),
                                         Y As Double()(),
                                         dims As (m%, n%),
                                         n_density_samples As Integer,
                                         n_test_samples As Integer,
                                         n_genes As Integer,
                                         rnaseq As Boolean) As Double()()

            Dim R As Double()() = MAT(Of Double)(dims.n, dims.m)

            For j As Integer = 0 To n_genes - 1
                'Dim offset_density = j * n_density_samples
                'Dim offset_test = j * n_test_samples

                ' row_d(X(offset_density), Y(offset_test), R(offset_test), n_density_samples, n_test_samples, rnaseq)
                row_d(X(j), Y(j), R(j), n_density_samples, n_test_samples, rnaseq)
            Next

            Return R
        End Function

        Const SIGMA_FACTOR = 4.0
        Const PRECOMPUTE_RESOLUTION = 10000
        Const MAX_PRECOMPUTE = 10.0

        Dim is_precomputed As Integer = 0
        Dim precomputed_cdf As Double() = New Double(PRECOMPUTE_RESOLUTION) {}

        ''' <summary>
        ''' for resampling, x are the resampled points and y are the
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <param name="r"></param>
        ''' <param name="size_density_n"></param>
        ''' <param name="size_test_n"></param>
        ''' <param name="rnaseq"></param>
        ''' <returns></returns>
        Private Function row_d(x As Double(),
                               y As Double(),
                               ByRef r As Double(),
                               size_density_n As Integer,
                               size_test_n As Integer,
                               rnaseq As Boolean)

            Dim bw As Double = If(rnaseq, 0.5, (sd1(x, size_density_n) / SIGMA_FACTOR))
            Dim left_tail As Double

            If Not rnaseq AndAlso is_precomputed = 0 Then
                initCdfs()
                is_precomputed = 1
            End If

            For j As Integer = 0 To size_test_n - 1
                left_tail = 0

                For i As Integer = 0 To size_density_n - 1
                    left_tail += If(rnaseq, ppois(y(j), x(i) + bw, True, False), _precomputed_cdf(y(j) - x(i), bw))
                Next

                left_tail = left_tail / size_density_n
                r(j) = -1 * stdNum.Log((1 - left_tail) / left_tail)
            Next

            Return r
        End Function

        Private Function _precomputed_cdf(x As Double, sigma As Double) As Double
            Dim v As Double = x / sigma

            If v < -1 * MAX_PRECOMPUTE Then
                Return 0
            ElseIf v > MAX_PRECOMPUTE Then
                Return 1
            Else
                Dim i As Integer = stdNum.Abs(v) / MAX_PRECOMPUTE * PRECOMPUTE_RESOLUTION
                Dim cdf As Double = precomputed_cdf(i)

                If v < 0 Then
                    Return 1 - cdf
                Else
                    Return cdf
                End If
            End If
        End Function

        Private Function ppois(y As Double, x As Double, f1 As Boolean, f2 As Boolean) As Double
            Throw New NotImplementedException
        End Function

        Private Sub initCdfs()
            Dim divisor = PRECOMPUTE_RESOLUTION * 1.0

            For i As Integer = 0 To PRECOMPUTE_RESOLUTION
                precomputed_cdf(i) = pnorm.Eval(MAX_PRECOMPUTE * i / divisor, 0.0, 1.0, lower_tail:=True, logP:=False)
            Next
        End Sub

        ''' <summary>
        ''' calculates standard deviation, largely borrowed from C code in R's src/main/cov.c */
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="n"></param>
        ''' <returns></returns>
        Private Function sd1(x As Double(), n As Integer) As Double
            Dim n1 As Integer
            Dim mean, sd As Double
            Dim sum As Double = 0.0
            Dim tmp As Double

            For i As Integer = 0 To n - 1
                sum += x(i)
            Next

            tmp = sum / n

            If Not tmp.IsNaNImaginary Then
                sum = 0

                For i As Integer = 0 To n - 1
                    sum += x(i) - tmp
                Next

                tmp = tmp + sum / n
            End If

            mean = tmp
            n1 = n - 1
            sum = 0

            For i As Integer = 0 To n - 1
                sum += (x(i) - mean) ^ 2
            Next

            sd = stdNum.Sqrt(sum / n1)

            Return sd
        End Function
    End Module
End Namespace
