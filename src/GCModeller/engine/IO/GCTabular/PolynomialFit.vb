#Region "Microsoft.VisualBasic::bac056e46f55c35398daed176eb198bd, engine\IO\GCTabular\PolynomialFit.vb"

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

    ' Class PolynomialFit
    ' 
    '     Function: ErrorSquared, F, FindPolynomialLeastSquaresFit, GaussianElimination, TryFit
    '     Structure PointF
    ' 
    '         Function: ToString
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' The original works was comes from here: http://www.vb-helper.com/howto_net_polynomial_least_squares.html
''' </summary>
''' <remarks></remarks>
Public Class PolynomialFit

    Public Structure PointF
        Dim X, Y As Double

        Public Overrides Function ToString() As String
            Return String.Format("{0}, {1}", X, Y)
        End Function
    End Structure

    ''' <summary>
    ''' Calculate the function value for a specific X.
    ''' </summary>
    ''' <param name="coeffs">Calculation result from <see cref="PolynomialFit.FindPolynomialLeastSquaresFit">the polynomial fit function</see></param>
    ''' <param name="x"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function F(coeffs As Double(), x As Double) As Double
        Dim total As Double = 0
        Dim x_factor As Double = 1
        For i As Integer = 0 To coeffs.Count - 1
            total += x_factor * coeffs(i)
            x_factor *= x
        Next i
        Return total
    End Function

    ''' <summary>
    ''' Return the error squared.
    ''' </summary>
    ''' <param name="points"></param>
    ''' <param name="coeffs"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ErrorSquared(points As PointF(), coeffs As Double()) As Double
        Dim total As Double = 0
        For Each pt As PointF In points
            Dim dy As Double = pt.Y - F(coeffs, pt.X)
            total += dy * dy
        Next pt
        Return total
    End Function

    Public Function TryFit(RPKMDataSet As PointF()) As Double()
        Dim MinError As Double = 1000
        Dim BestCoeffs As Double() = New Double() {}

        For Degree As Integer = 1 To 6
            Dim ChunkBuffer As Double() = FindPolynomialLeastSquaresFit(RPKMDataSet, Degree)
            Dim err As Double = Global.System.Math.Sqrt(ErrorSquared(RPKMDataSet, ChunkBuffer))  ' Get the error.

            If err < MinError Then
                MinError = err
                BestCoeffs = ChunkBuffer
            End If
        Next

        Return BestCoeffs
    End Function

    ''' <summary>
    ''' Find the least squares linear fit.
    ''' </summary>
    ''' <param name="points"></param>
    ''' <param name="degree"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FindPolynomialLeastSquaresFit(Points As PointF(), Degree As Integer) As Double()
        ' Allocate space for (degree + 1) equations with 
        ' (degree + 2) terms each (including the constant term).
        Dim coeffs(Degree, Degree + 1) As Double

        ' Calculate the coefficients for the equations.
        For j As Integer = 0 To Degree
            ' Calculate the coefficients for the jth equation.

            ' Calculate the constant term for this equation.
            coeffs(j, Degree + 1) = 0
            For Each pt As PointF In Points
                coeffs(j, Degree + 1) -= Global.System.Math.Pow(pt.X, j) * pt.Y
            Next pt

            ' Calculate the other coefficients.
            For a_sub As Integer = 0 To Degree
                ' Calculate the dth coefficient.
                coeffs(j, a_sub) = 0
                For Each pt As PointF In Points
                    coeffs(j, a_sub) -= Global.System.Math.Pow(pt.X, a_sub + j)
                Next pt
            Next a_sub
        Next j

        ' Solve the equations.
        Dim answer() As Double = GaussianElimination(coeffs)

        ' Return the result converted into a List(Of Double).
        Return answer
    End Function

    ' Perform Gaussian elimination on these coefficients.
    ' Return the array of values that gives the solution.
    Private Function GaussianElimination(coeffs(,) As Double) As Double()
        Dim max_equation As Integer = coeffs.GetUpperBound(0)
        Dim max_coeff As Integer = coeffs.GetUpperBound(1)
        For i As Integer = 0 To max_equation
            ' Use equation_coeffs(i, i) to eliminate the ith
            ' coefficient in all of the other equations.

            ' Find a row with non-zero ith coefficient.
            If (coeffs(i, i) = 0) Then
                For j As Integer = i + 1 To max_equation
                    ' See if this one works.
                    If (coeffs(j, i) <> 0) Then
                        ' This one works. Swap equations i and j.
                        ' This starts at k = i because all
                        ' coefficients to the left are 0.
                        For k As Integer = i To max_coeff
                            Dim temp As Double = coeffs(i, k)
                            coeffs(i, k) = coeffs(j, k)
                            coeffs(j, k) = temp
                        Next k
                        Exit For
                    End If
                Next j
            End If

            ' Make sure we found an equation with
            ' a non-zero ith coefficient.
            Dim coeff_i_i As Double = coeffs(i, i)
            If coeff_i_i = 0 Then
                Throw New ArithmeticException(String.Format( _
                    "There is no unique solution for these points.", _
                    coeffs.GetUpperBound(0) - 1))
            End If

            ' Normalize the ith equation.
            For j As Integer = i To max_coeff
                coeffs(i, j) /= coeff_i_i
            Next j

            ' Use this equation value to zero out
            ' the other equations' ith coefficients.
            For j As Integer = 0 To max_equation
                ' Skip the ith equation.
                If (j <> i) Then
                    ' Zero the jth equation's ith coefficient.
                    Dim coef_j_i As Double = coeffs(j, i)
                    For d As Integer = 0 To max_coeff
                        coeffs(j, d) -= coeffs(i, d) * coef_j_i
                    Next d
                End If
            Next j
        Next i

        ' At this point, the ith equation contains
        ' 2 non-zero entries:
        '      The ith entry which is 1
        '      The last entry coeffs(max_coeff)
        ' This means Ai = equation_coef(max_coeff).
        Dim solution(max_equation) As Double
        For i As Integer = 0 To max_equation
            solution(i) = coeffs(i, max_coeff)
        Next i

        ' Return the solution values.
        Return solution
    End Function
End Class
