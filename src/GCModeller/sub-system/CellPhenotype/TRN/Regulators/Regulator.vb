#Region "Microsoft.VisualBasic::7889f416208342c5e22835e6bb324d15, sub-system\CellPhenotype\TRN\Regulators\Regulator.vb"

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

    '     Class RegulationExpression
    ' 
    '         Properties: Quantity, RegulationFunctional, Regulator, Repression, SitePosition
    '                     UniqueId, Value, Weight
    ' 
    '         Function: CopyTo, set__Regulator, set_TargetSite, SetConfigs
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DataMining.DFL_Driver
Imports SMRUCC.genomics.Analysis.CellPhenotype.TRN.Configs

Namespace TRN.KineticsModel.Regulators

    ''' <summary>
    ''' 表示调控因子与调控的基因之间的关系，<see cref="RegulationExpression.Weight"></see>用于表示调控的Effect出现的事件概率值的高低，
    ''' 当然，对于本类型的对象，你也可以将其当作为一个调控因子，并且这种类型的调控因子为自由类型的调控因子，即不需要任何外部的附加条件既
    ''' 可以产生调控功能的调控因子
    ''' </summary>
    ''' <remarks></remarks>
    Public Class RegulationExpression : Inherits I_FactorElement
        Implements Configs.I_Configurable
        Implements IReadOnlyId
        Implements IReadOnlyDataSource(Of Boolean)

        Protected RegulatorModel As BinaryExpression

        <XmlAttribute> Public Property Regulator As String

        ''' <summary>
        ''' <see cref="Weight"></see>的值有符号，其中符号表示调控效应：激活或者抑制，为了方便计算，这里取绝对值，
        ''' <see cref="Weight"></see>值越大，则<see cref="_ABS_Weight"></see>变量的值越小，即该调控事件越容易发生
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Property Weight As Double
            Get
                Return MyBase.Weight
            End Get
            Set(value As Double)
                MyBase.Weight = value
                Repression = value <= 0
            End Set
        End Property

        ''' <summary>
        ''' <see cref="Weight"></see>的符号，调控的效应，当为真的时候，为抑制作用，当为假的时候，为激活作用，Pcc符合随机事件发生的条件的时候，目标对象将会被设置为假
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Repression As Boolean
        <XmlAttribute> Public Property SitePosition As Integer

        ''' <summary>
        ''' 调控因子所调控的实际对象是一个调控位点
        ''' </summary>
        ''' <remarks></remarks>
        Public Function set_TargetSite(site As SiteInfo) As RegulationExpression
            Me._InteractionTarget = site
            Return Me
        End Function

        Public ReadOnly Property UniqueId As String Implements IReadOnlyId.Identity, IReadOnlyDataSource(Of Boolean).Key
            Get
                Return RegulatorModel.Identifier
            End Get
        End Property

        Public ReadOnly Property Value As Boolean Implements IReadOnlyDataSource(Of Boolean).Value
            Get
                Return RegulatorModel.Status
            End Get
        End Property

        ''' <summary>
        ''' 数量越多，则权重越大，则概率事件的阈值就越低，即该调控事件越容易发生
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Quantity As Double
            Get
                Return RegulatorModel.Value
            End Get
        End Property

        Public Function set__Regulator(regulator As BinaryExpression) As RegulationExpression
            Me.RegulatorModel = regulator
            Me.Regulator = regulator.Identifier
            Return Me
        End Function

        ''' <summary>
        ''' 返回调控的效果，TRUE表示激活，FALSE表示抑制；根据Pcc权重计算出来调控因子对目标基因的表达调控的可能性
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable ReadOnly Property RegulationFunctional As Boolean
            Get
                If Me.RegulatorModel.Value = 0 Then  '当调控因子蛋白质的数量为0的时候，很明显无法起作用
                    Return False '数量越多，则权重越大，则概率事件的阈值就越低，即该调控事件越容易发生
                End If

                Dim n = Me.FunctionalState
                Return n <> 0  '无论正负，都被看作为起到了调控效应，只是区别在于效果不一样
            End Get
        End Property

        Public Function CopyTo(Of T_Regulation As RegulationExpression)() As T_Regulation
            Dim NewExpression As T_Regulation = Activator.CreateInstance(Of T_Regulation)()
            NewExpression.Regulator = Regulator
            NewExpression.RegulatorModel = RegulatorModel
            NewExpression.Repression = Repression
            NewExpression.SitePosition = SitePosition
            NewExpression.Weight = Weight
            NewExpression._InteractionTarget = _InteractionTarget

            Return NewExpression
        End Function

        Protected Conf As Configs

        Public Overridable Function SetConfigs(conf As Configs) As Integer Implements I_Configurable.SetConfigs
            Me.Conf = conf
            Return 0
        End Function
    End Class
End Namespace
