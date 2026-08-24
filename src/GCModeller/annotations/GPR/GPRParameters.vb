#Region "Microsoft.VisualBasic::4821a69064eacf7a2be3e80bb3c9b0d5, annotations\GPR\GPRParameters.vb"

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

    '   Total Lines: 62
    '    Code Lines: 16 (25.81%)
    ' Comment Lines: 43 (69.35%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 3 (4.84%)
    '     File Size: 1.96 KB


    ' Class GPRParameters
    ' 
    '     Properties: BaseCoexpressionScore, BaseComplexScore, BaseContextScore, BaseSyntenyScore, CoexpressionThreshold
    '                 DiffStrandWeight, DirectMatchScore, MaxGapInPathway, MaxOperonDistance, MaxPhysicalDistance
    '                 MaxWindowSpan, PathwayCompletenessThreshold, SameOperonBonus, SameStrandWeight
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' 算法参数
''' </summary>
Public Class GPRParameters

    ''' <summary>
    ''' 操纵子内最大距离
    ''' </summary>
    ''' <returns></returns>
    Public Property MaxOperonDistance As Integer = 500
    ''' <summary>
    ''' 同操纵子奖励
    ''' </summary>
    ''' <returns></returns>
    Public Property SameOperonBonus As Double = 0.3
    ''' <summary>
    ''' 通路完整度阈值
    ''' </summary>
    ''' <returns></returns>
    Public Property PathwayCompletenessThreshold As Double = 0.7
    ''' <summary>
    ''' 上下文窗口大小（向上下游各看几个基因）
    ''' </summary>
    ''' <returns></returns>
    Public Property MaxWindowSpan As Integer = 10
    ''' <summary>
    ''' 最大物理距离阈值，超过此距离认为不在同一基因簇
    ''' </summary>
    ''' <returns></returns>
    Public Property MaxPhysicalDistance As Integer = 15000
    ''' <summary>
    ''' 基于上下文推断的基础分
    ''' </summary>
    ''' <returns></returns>
    Public Property BaseContextScore As Double = 0.5
    ''' <summary>
    ''' 直接EC匹配的满分
    ''' </summary>
    ''' <returns></returns>
    Public Property DirectMatchScore As Double = 1.0
    ''' <summary>
    ''' 同链权重
    ''' </summary>
    ''' <returns></returns>
    Public Property SameStrandWeight As Double = 1.0
    ''' <summary>
    ''' 异链权重
    ''' </summary>
    ''' <returns></returns>
    Public Property DiffStrandWeight As Double = 0.3
    ''' <summary>
    ''' 通路中允许的最大反应间隔
    ''' </summary>
    ''' <returns></returns>
    Public Property MaxGapInPathway As Integer = 3

    Public Property CoexpressionThreshold As Double = 0.7
    Public Property BaseCoexpressionScore As Double = 0.4
    Public Property BaseSyntenyScore As Double = 0.6
    Public Property BaseComplexScore As Double = 0.4

End Class
