#Region "Microsoft.VisualBasic::38b5ee0988f576b848f8e00fd0652db0, sub-system\CellPhenotype\TRN\Regulators\TCS.vb"

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

    '     Class TCS
    ' 
    '         Properties: CrossTalkEffect, CrossTalks, RegulationFunctional
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace TRN.KineticsModel.Regulators

    ''' <summary>
    ''' 需要HisK
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TCS : Inherits RegulationExpression

        ''' <summary>
        ''' Key部分的值为HisK，Value为CrossTalk的计算得分，同源的TCS部件之家的CrossTalk的得分为1，CrossTalk的运算为加法运算
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CrossTalks As KeyValuePairObject(Of BinaryExpression, Double)()

        ''' <summary>
        ''' 对<see cref="CrossTalks"></see>的加法计算结果，用于表示双组分反应调控蛋白的调控事件的发生的可能性的高低
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property CrossTalkEffect As Double
            Get
                Dim LQuery = (From item In CrossTalks Select item.Key.Value * item.Value).ToArray         '进行加法运算
                Dim avg As Double = LQuery.Average, sum As Double = LQuery.Sum

                If avg = 0.0R Then
                    Return 0
                End If

                Dim value As Double = avg / sum
                Return value
            End Get
        End Property

        ''' <summary>
        ''' 计算原理与OCS的一致
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property RegulationFunctional As Boolean
            Get
                If Me.RegulatorModel.Value = 0 Then  '当调控因子蛋白质的数量为0的时候，很明显无法起作用
                    Return False '数量越多，则权重越大，则概率事件的阈值就越低，即该调控事件越容易发生
                End If

                Dim p As Double = Me.Internal_getEventProbabilities
                Dim p_ef As Double = CrossTalkEffect
                p = p * (1 - p_ef)

                Dim n As Double = Rnd()

                If n >= p Then
                    Return get_InteractionQuantity()
                Else
                    Return 0 '事件发生的阈值不满足条件，无法起作用
                End If
            End Get
        End Property
    End Class
End Namespace
