#Region "Microsoft.VisualBasic::3811dc4a9e5302abdae74e45baaf282a, sub-system\CellPhenotype\TRN\Regulators\OCS.vb"

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

    '     Class OCS
    ' 
    '         Properties: Effector, EffectorPathways, RegulationFunctional
    ' 
    '         Function: Internal_getPathwayEffector, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace TRN.KineticsModel.Regulators

    ''' <summary>
    ''' 需要Effector
    ''' </summary>
    ''' <remarks></remarks>
    Public Class OCS : Inherits RegulationExpression

        ''' <summary>
        ''' 这些代谢途径都是和Effector的合成相关的，在每一个对象之中，其Value值为该代谢途径之中的所有的基因，而Key的值则表示为该代谢途径的编号，每一个对象都做加法运算，而每一个对象内部的基因对象之间都做AND逻辑运算
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property EffectorPathways As KeyValuePairData(Of KineticsModel.BinaryExpression())()

        ''' <summary>
        ''' 对<see cref="EffectorPathways"></see>的计算结果，值用来表述概率事件的发生的可能性的高低，值越低则越难发生
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Effector As Double
            Get
                If EffectorPathways.IsNullOrEmpty Then
                    Dim n = Rnd() '没有在模型之中找到代谢物的合成的代谢途径，则可能为第二信使或者其他未知的原因，则在模型之中以很低的概率产生调控效应
                    If n > Conf.OCS_NONE_Effector Then
                        Return Conf.OCS_Default_EffectValue
                    Else
                        Return 0
                    End If
                End If

                Dim LQuery = (From item In EffectorPathways Select Internal_getPathwayEffector(item)).ToArray  '进行加法运算
                Dim avg As Double = LQuery.Average, sum As Double = LQuery.Sum

                If avg = 0.0R Then
                    Return 0
                End If

                Dim value As Double = avg / sum
                Return value
            End Get
        End Property

        ''' <summary>
        ''' 所有的基因为AND运算
        ''' </summary>
        ''' <param name="Pathway"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function Internal_getPathwayEffector(Pathway As KeyValuePairData(Of KineticsModel.BinaryExpression())) As Double
            Dim LQuery = (From item In Pathway.DataObject Where item.Value > 0 Select item.Value).ToArray
            If LQuery.Count < Pathway.DataObject.Count Then '只要有一个基因没有表达量，则没有效应物生成
                Return 0
            Else
                Dim value As Double = LQuery.Min / LQuery.Average  '当都存在的时候，返回均值与最小值的商，假若最小值与均值的差越大，则认为某一个节点的流量越小，则Effector的合成量越少
                Return value
            End If
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0}  <--- {1} pathway(s)", UniqueId, EffectorPathways.Count)
        End Function

        ''' <summary>
        ''' 继承的对象和基本对象之间的实现是有差异的，基本对象直接使用state&lt;&gt;0来表述，因为<see cref="FunctionalState"></see>已经包含有模糊逻辑判断了，只要返回非零值，就表示事件发生了，而本属性则是从头计算
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
                Dim p_ef As Double = Effector
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
