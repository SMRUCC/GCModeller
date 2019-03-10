﻿#Region "Microsoft.VisualBasic::60e641d7bea6527af76fd62c05687e93, Data_science\Mathematica\Math\Math\Algebra\Matrix.NET\Extensions.vb"

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

    '     Module Extensions
    ' 
    '         Function: CenterNormalize, ColumnVector, Covariance, rand, size
    ' 
    '         Sub: Print
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports Microsoft.VisualBasic.Text

Namespace Matrix

    Public Module Extensions

        <Extension>
        Public Function ColumnVector(matrix As GeneralMatrix, i%) As Vector
            Return New Vector(matrix({i}).Select(Function(r) r(Scan0)))
        End Function

        Public Function size(M As GeneralMatrix, d%) As Integer
            If d = 1 Then
                Return M.RowDimension
            ElseIf d = 2 Then
                Return M.ColumnDimension
            Else
                Throw New NotImplementedException
            End If
        End Function

        ''' <summary>Generate matrix with random elements</summary>
        ''' <param name="m">   Number of rows.
        ''' </param>
        ''' <param name="n">   Number of colums.
        ''' </param>
        ''' <returns>     An m-by-n matrix with uniformly distributed random elements.
        ''' </returns>

        Public Function rand(m%, n%) As GeneralMatrix
            With New Random()
                Dim A As New GeneralMatrix(m, n)
                Dim X As Double()() = A.Array

                For i As Integer = 0 To m - 1
                    For j As Integer = 0 To n - 1
                        X(i)(j) = .NextDouble()
                    Next
                Next

                Return A
            End With
        End Function

        ''' <summary>
        ''' Centers each column of the data matrix at its mean.
        ''' Normalizes the input matrix so that each column is centered at 0.
        ''' </summary>
        <Extension> Public Function CenterNormalize(m As GeneralMatrix) As GeneralMatrix
            'ORIGINAL LINE: double[][] @out = new double[input.Length][input[0].Length];
            Dim input = m.Array
            Dim out As Double()() = MAT(Of Double)(input.Length, input(0).Length)

            For i As Integer = 0 To input.Length - 1
                Dim meanValue As Double = input(i).Average
                For j As Integer = 0 To input(i).Length - 1
                    out(i)(j) = input(i)(j) - meanValue
                Next
            Next

            Return out
        End Function

        ''' <summary>
        ''' Constructs the covariance matrix for this data set.
        ''' @return	the covariance matrix of this data set
        ''' </summary>
        <Extension> Public Function Covariance(matrix As GeneralMatrix) As GeneralMatrix
            'ORIGINAL LINE: double[][] @out = new double[matrix.Length][matrix.Length];
            Dim out As Double()() = MAT(Of Double)(matrix.Length, matrix.Length)
            For i As Integer = 0 To out.Length - 1
                For j As Integer = 0 To out.Length - 1
                    Dim dataA As Double() = matrix.Array(i)
                    Dim dataB As Double() = matrix.Array(j)
                    out(i)(j) = dataA.Covariance(dataB)
                Next
            Next
            Return out
        End Function

        ''' <summary>
        ''' Print the matrix data onto the console or a specific stream.
        ''' </summary>
        ''' <param name="m"></param>
        ''' <param name="format$"></param>
        ''' <param name="out"></param>
        <Extension> Public Sub Print(m As GeneralMatrix, Optional format$ = "F4", Optional out As StreamWriter = Nothing)
            Dim openSTD As Boolean = False
            Dim line$

            If out Is Nothing Then
                out = New StreamWriter(Console.OpenStandardOutput)
                openSTD = True
            End If

            For Each row As Double() In m
                line = row _
                    .Select(Function(x)
                                If x >= 0 Then
                                    Return " " & x.ToString(format)
                                Else
                                    Return x.ToString(format)
                                End If
                            End Function) _
                    .JoinBy(ASCII.TAB)
                out.WriteLine(line)
            Next

            Call out.Flush()

            If openSTD Then
                out.Dispose()
            End If
        End Sub
    End Module
End Namespace
