﻿#Region "Microsoft.VisualBasic::facb61ba50747eb1b37f5551b426d508, Data_science\Mathematica\Math\Math.Statistics\HypothesisTesting\T-test\TestResult.vb"

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

    '     Class TtestResult
    ' 
    '         Properties: alpha, Alternative, DegreeFreedom, Mean, Pvalue
    '                     StdErr, TestValue
    ' 
    '         Function: ToString, Valid
    ' 
    '     Class TwoSampleResult
    ' 
    '         Properties: MeanX, MeanY
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Hypothesis

    Public Class TtestResult

        ''' <summary>
        ''' the degrees of freedom for the t-statistic.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Welch–Satterthwaite equation 的计算结果为小数</remarks>
        Public Property DegreeFreedom As Double
        ''' <summary>
        ''' the p-value For the test.
        ''' </summary>
        ''' <returns></returns>
        Public Property Pvalue As Double
        ''' <summary>
        ''' the value of the t-statistic.
        ''' </summary>
        ''' <returns></returns>
        Public Property TestValue As Double
        ''' <summary>
        ''' the alternative hypothesis.
        ''' </summary>
        ''' <returns></returns>
        Public Property Alternative As Hypothesis
        Public Property alpha As Double
        ''' <summary>
        ''' Sample mean
        ''' </summary>
        ''' <returns></returns>
        Public Property Mean As Double
        Public Property StdErr As Double

        ''' <summary>
        ''' Alternative hypothesis result
        ''' </summary>
        ''' <returns></returns>
        Public Function Valid() As Boolean
            Return Pvalue >= alpha
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class TwoSampleResult : Inherits TtestResult

        Public Property MeanX As Double
        Public Property MeanY As Double

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
