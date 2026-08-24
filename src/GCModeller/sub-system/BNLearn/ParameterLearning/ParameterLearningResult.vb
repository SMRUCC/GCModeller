#Region "Microsoft.VisualBasic::bf18c5c23236724dce751e424517d7ad, sub-system\BNLearn\ParameterLearning\ParameterLearningResult.vb"

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

    '   Total Lines: 24
    '    Code Lines: 9 (37.50%)
    ' Comment Lines: 8 (33.33%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (29.17%)
    '     File Size: 692 B


    '     Class ParameterLearningResult
    ' 
    '         Properties: AverageRSquared, ElapsedMs, Network, TotalBIC, TotalLogLikelihood
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace ParameterLearning

    ''' <summary>
    ''' 参数学习结果
    ''' </summary>
    Public Class ParameterLearningResult

        ''' <summary>拟合后的网络（含CPD参数）</summary>
        Public Property Network As Core.BayesianNetwork

        ''' <summary>总对数似然</summary>
        Public Property TotalLogLikelihood As Double

        ''' <summary>总 BIC</summary>
        Public Property TotalBIC As Double

        ''' <summary>平均 R²</summary>
        Public Property AverageRSquared As Double

        ''' <summary>参数学习耗时（毫秒）</summary>
        Public Property ElapsedMs As Long

    End Class
End Namespace
