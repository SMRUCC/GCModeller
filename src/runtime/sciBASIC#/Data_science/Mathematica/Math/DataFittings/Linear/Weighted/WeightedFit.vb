﻿#Region "Microsoft.VisualBasic::df4decaecef9ee6e22832cf9f435b386, Data_science\Mathematica\Math\DataFittings\Linear\Weighted\WeightedFit.vb"

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

    ' Class WeightedFit
    ' 
    '     Properties: CoefficientsStandardError, CorrelationCoefficient, ErrorTest, FisherF, Polynomial
    '                 Residuals, StandardDeviation, VarianceMatrix
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Public Class WeightedFit : Implements IFitted

    ''' <summary>
    ''' FReg: Fisher F statistic for regression
    ''' </summary>
    ''' <returns></returns>
    Public Property FisherF() As Double

    ''' <summary>
    ''' RYSQ: Multiple correlation coefficient (R2，相关系数)
    ''' </summary>
    ''' <returns></returns>
    Public Property CorrelationCoefficient As Double Implements IFitted.CorrelationCoefficient

    ''' <summary>
    ''' SDV: Standard deviation of errors
    ''' </summary>
    ''' <returns></returns>
    Public Property StandardDeviation() As Double

    ''' <summary>
    ''' DY: Residual values of Y
    ''' </summary>
    ''' <returns></returns>
    Public Property Residuals() As Double()

    ''' <summary>
    ''' SEC: Std Error of coefficients
    ''' </summary>
    ''' <returns></returns>
    Public Property CoefficientsStandardError() As Double()

    ''' <summary>
    ''' V: Least squares and var/covar matrix
    ''' </summary>
    ''' <returns></returns>
    Public Property VarianceMatrix() As Double(,)

    Default Public ReadOnly Property GetY(x As Double) As Double Implements IFitted.GetY
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return Polynomial(x)
        End Get
    End Property

    Public Property Polynomial As Polynomial Implements IFitted.Polynomial

    ''' <summary>
    ''' Ycalc: Calculated values of Y.(根据所拟合的公式所计算出来的预测值)
    ''' </summary>
    ''' <returns></returns>
    Public Property ErrorTest As TestPoint() Implements IFitted.ErrorTest

    Public Overrides Function ToString() As String
        Return $"{Polynomial.ToString("F4")} @ R2={CorrelationCoefficient.ToString("F4")}"
    End Function
End Class
