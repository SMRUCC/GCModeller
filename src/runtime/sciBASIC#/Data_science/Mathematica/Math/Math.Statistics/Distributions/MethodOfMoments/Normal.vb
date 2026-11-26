#Region "Microsoft.VisualBasic::4185e91815d174b1d162ae8fbad15119, Data_science\Mathematica\Math\Math.Statistics\Distributions\MethodOfMoments\Normal.vb"

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

    '   Total Lines: 188
    '    Code Lines: 150 (79.79%)
    ' Comment Lines: 21 (11.17%)
    '    - Xml Docs: 66.67%
    ' 
    '   Blank Lines: 17 (9.04%)
    '     File Size: 6.94 KB


    '     Class Normal
    ' 
    '         Properties: Mean, StDev
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: FindArea, GetCDF, GetInvCDF, GetPDF, TrapazoidalIntegration
    '                   Validate
    ' 
    ' 
    ' /********************************************************************************/

#End Region

'
' * To change this license header, choose License Headers in Project Properties.
' * To change this template file, choose Tools | Templates
' * and open the template in the editor.
' 
Imports System.Runtime.CompilerServices
Imports std = System.Math

Namespace Distributions.MethodOfMoments

    ''' <summary>
    ''' normal distribution
    ''' 
    ''' @author Will_and_Sara
    ''' </summary>
    Public Class Normal : Inherits Distributions.ContinuousDistribution

        Public ReadOnly Property Mean As Double
        Public ReadOnly Property StDev As Double

        ''' <summary>
        ''' Creates a standard normal distribution
        ''' </summary>
        Public Sub New()
            Mean = 0
            StDev = 1
        End Sub
        ''' <summary>
        ''' Creates a normal distribution based on the user defined mean and standard deviation </summary>
        ''' <param name="m"> the mean of the distribution </param>
        ''' <param name="sd"> the standard deviation of the distribution </param>
        Public Sub New(m As Double, sd As Double)
            Mean = m
            StDev = sd
        End Sub
        ''' <summary>
        ''' Creates a normal distribution based on input data using the standard method of moments. </summary>
        ''' <param name="data"> an array of double data. </param>
        Public Sub New(data As Double())
            Dim bpm As New MomentFunctions.BasicProductMoments(data)
            Mean = bpm.Mean()
            StDev = bpm.StDev()
            PeriodOfRecord = (bpm.SampleSize())
        End Sub

        Public Overrides Function GetInvCDF(probability As Double) As Double
            If _StDev <= 0 Then Return Double.NaN

            ' 见 NormalDistribution.InvCDF：Acklam 逼近 + Halley 精化，端点返回 ∓∞
            Return _Mean + _StDev * NormalDistribution.InvCDF(probability)
        End Function

        Public Overrides Function GetCDF(value As Double) As Double
            If _StDev <= 0 Then Return Double.NaN

            Return NormalDistribution.CDF((value - _Mean) / _StDev)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetPDF(value As Double) As Double
            If _StDev <= 0 Then Return Double.NaN

            Return NormalDistribution.PDF((value - _Mean) / _StDev) / _StDev
        End Function

        Public Overrides Iterator Function Validate() As IEnumerable(Of Exception)
            If _StDev <= 0 Then Yield New Exception("Standard of Deviation must be greater than 0")
        End Function
    End Class

End Namespace
