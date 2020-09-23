#Region "Microsoft.VisualBasic::b919511ce0ecac59ecb4c8f47634fd45, sub-system\CellPhenotype\TRN\KineticsModel\KineticsModel.vb"

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

    '     Class BinaryExpression
    ' 
    '         Properties: __address, Handle, Identifier, Is_RegulatorType, RegulationValue
    '                     RegulatorCounts, RegulatorySites, Status, Value
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: CreateHandle, Evaluate, InternalGetMostPossibleAppearState, Regulators_DynamicsRegulation, Regulators_SiteSpecificDynamicsRegulations
    '                   SetConfigs, ToString
    ' 
    '         Sub: Assign, InternalFactorDecays, set_Mutation
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Analysis.CellPhenotype.TRN.KineticsModel.Regulators
Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver
Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.DataStorage.FileModel

Namespace TRN.KineticsModel

    ''' <summary>
    ''' 这个对象表示一个基因，即网络之中的一个节点，只有1和0这两个值的半逻辑表达式，模糊逻辑的原因是逻辑取值是基于一个随机概率的
    ''' </summary>
    ''' <remarks></remarks>
    Public Class BinaryExpression

        Implements Configs.I_Configurable
        Implements IDynamicsExpression(Of Integer)
        Implements INamedValue
        Implements IDataSource(Of Long, Integer)
        Implements IAddressOf
        Implements IObjectStatus

        ''' <summary>
        ''' 这个关系是根据footprint结果得出来的
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement("Motif.Sites")> Public Property RegulatorySites As SiteInfo()
        Public ReadOnly Property Status As Boolean Implements IObjectStatus.Status
            Get
                Return _value
            End Get
        End Property

        ''' <summary>
        ''' 每一次调用<see cref="Evaluate"></see>方法进行迭代计算，都会更新这个值，这个值由<see cref="NetworkInput">蒙特卡洛网络输入</see>进行初始化
        ''' </summary>
        ''' <remarks></remarks>
        Dim _value As Boolean, _factor As Double = 1

        ''' <summary>
        ''' 当前的这个基因所表达的mRNA分子的数量
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend _InternalQuantityValue As Integer ', semiDelta As Single

        Dim _IsRegulatorType As Boolean

        ''' <summary>
        ''' 由长度映射而得来的单次增长量
        ''' </summary>
        ''' <remarks></remarks>
        Dim _LengthDelta As Integer

        ''' <summary>
        '''  0 -- 无调控作用
        '''  1 -- 正调控作用
        ''' -1 -- 负调控作用
        ''' </summary>
        ''' <remarks></remarks>
        Dim _RegulationValue As Integer

        ''' <summary>
        ''' 当前的这个基因所受到的表达调控作用的表述
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property RegulationValue As Integer
            Get
                Return _RegulationValue
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="init"></param>
        ''' <param name="d">通过核酸链长度的映射得到的增长值</param>
        ''' <remarks></remarks>
        Sub New(init As Boolean, d As Integer)
            _LengthDelta = d
            _value = init
        End Sub

        Sub New()
        End Sub

        Public Property Is_RegulatorType As Boolean
            Get
                Return _IsRegulatorType
            End Get
            Set(value As Boolean)
                _IsRegulatorType = value

                If _IsRegulatorType Then
                    _semi_Decays_delta = 0.1
                Else
                    _semi_Decays_delta = 0.15
                End If
            End Set
        End Property

        Public ReadOnly Property RegulatorCounts As Integer
            Get
                Return (From item In RegulatorySites Select item.Regulators.TryCount).Sum
            End Get
        End Property

        ''' <summary>
        ''' 0 - 缺失突变，该基因的调控事件不会发生
        ''' 1 - 正常表达
        ''' &gt;1 - 过量表达，该基因的调控事件总会发生，因为被设置的事件概率大于1
        ''' 0-1 - 调控事件以低于平常的概率发生 
        ''' </summary>
        ''' <param name="factor"></param>
        ''' <remarks></remarks>
        Public Sub set_Mutation(factor As Double)
            If factor = 0.0R Then
                _InternalQuantityValue = 0  '缺失突变的基因不会存在于模型之中
            End If

            _factor = factor
        End Sub

        Public Property Identifier As String Implements IObjectStatus.locusId, INamedValue.Key

        Public Overrides Function ToString() As String
            Return String.Format("{0}:= {1};  {2} sites and {3} regulators", Me.Identifier, Value, Me.RegulatorySites.Count, (From item In RegulatorySites Select item.Regulators.Count).ToArray.Sum)
        End Function

        Public Function Evaluate() As Integer Implements IDynamicsExpression(Of Integer).Evaluate

            If RegulatorySites.IsNullOrEmpty Then
                _RegulationValue = 0
                Return 1 '没有任何调控因子，则值为恒定值(即所设定的初始值)
            End If

            If _factor = 0.0R Then
                _RegulationValue = 0
                _value = False
                Return 0 '该调控事件不会发生，先于降解发生，以防止被降解玩
            End If

            Call InternalFactorDecays() '无论是否有调控作用，都会有降解发生

            If RegulatorySites.Count = 1 Then
                _value = Regulators_SiteSpecificDynamicsRegulations(RegulatorySites.First.Regulators)
            Else  '有多个调控因子的情况
                _value = Regulators_DynamicsRegulation()
            End If

            If _value = True Then
                'factor 值越高，表达的可能性越高，1位正常表达，值越低则表达量越低，接近于0的时候为本底表达

                If _factor < 1 Then
                    If Rnd() > (1 - _factor) Then 'factor数值越大，越容易发生该事件
                        _InternalQuantityValue += Me._LengthDelta   '低量表达依照factor的数值大小来决定表达的量
                        _RegulationValue = 1
                    Else
                        _RegulationValue = 0
                    End If
                Else
                    _InternalQuantityValue += Me._LengthDelta       '等于1位正常表达
                    _RegulationValue = 1
                End If
            Else
                _RegulationValue = -1
            End If

            If _factor > 1 Then
                _InternalQuantityValue += _factor        '过表达突变体之中基因组的调控事件可能不会发生，但是在表达载体上却会有持续不断的表达事件，所以过表达之中总是在这里产生表达量
            End If

            Return _InternalQuantityValue
        End Function

        Dim _semi_Decays As Double, Conf As Configs, _semi_Decays_delta As Double

        ''' <summary>
        ''' 无论是否有调控作用，都会有降解发生
        ''' </summary>
        ''' <remarks>无论链的长短，降解的速度都要一致的，即都以0.01的速度降解</remarks>
        Private Sub InternalFactorDecays()
            '_semi_Decays -= _semi_Decays_delta

            'If _semi_Decays <= -1 Then
            '    _Quantity += _semi_Decays '请注意这个衰减变量是负数，需要加法运算来降解目标产物
            '    _semi_Decays = 0

            '    If _Quantity < 0 Then
            '        _Quantity = 0
            '    End If
            'End If

            Dim Decay As Integer = 0.05 * _InternalQuantityValue
            _InternalQuantityValue -= Decay

            If _InternalQuantityValue < 0 Then
                _InternalQuantityValue = 0
            End If
        End Sub

        ''' <summary>
        ''' 函数返回是否被激活其表达过程
        ''' </summary>
        ''' <param name="RegulationSites"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function Regulators_SiteSpecificDynamicsRegulations(RegulationSites As RegulationExpression()) As Boolean
            Dim DLQuery = (From item In RegulationSites Where item.RegulationFunctional = True Select (Not item.Repression)).ToArray

            If DLQuery.IsNullOrEmpty Then '没有任何调控因子是处于活跃状态的
                GoTo BASAL_EXPRESSION
            End If

            Dim Effects As Boolean = InternalGetMostPossibleAppearState(DLQuery)

            '   Call Randomize()
            If Rnd() >= Conf.SiteSpecificDynamicsRegulations Then 'factor越大，则阈值越低，即事件越容易发生
                Return Effects
            Else
BASAL_EXPRESSION:
                Dim n# = Rnd()

                If n < Conf.BasalThreshold Then '默认状态是不激活，有较低的概率处于激活状态，即本底表达
                    '  Call Console.WriteLine("[DEBUG] {0} for basal expression.....", n)
                    Return True
                Else
                    Return False
                End If
            End If
        End Function

        ''' <summary>
        ''' Gets the most possible node regulation state in current time point.(获取当前时间点之下的最有可能的节点的调控状态值)
        ''' </summary>
        ''' <param name="Status"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function InternalGetMostPossibleAppearState(Status As Boolean()) As Boolean
            Dim TLQuery = (From n As Boolean In Status Where n = True Select 1).ToArray.Count
            Dim FLQuery = Status.Count - TLQuery

            If TLQuery > FLQuery Then  '在细胞群之中激活的数目多余抑制的数目，则在转录组水平上整体呈现激活的状态
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' 有多个调控因子的时候的表达的计算公式
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 算法要点
        ''' 
        ''' 0. 对于所有随机试验低于阈值的调控事件，都默认为不激活(即，没有调控因子的激活的话，基因不表达)
        ''' 1. 对于同一个位点之上，假若激活的数目多余抑制的数目，则激活的权重比较大，该位点计算为激活的可能性比较高(假设转录组数据是建立在大细胞宗系的条件之下测定的)
        ''' 2. 在Promoter区之内，假若任意一个位点被抑制，则整个基因的表达过程被抑制(单纯的分子动力学行为)
        ''' </remarks>
        Private Function Regulators_DynamicsRegulation() As Boolean
            Dim LQuery = (From SiteInfoData As SiteInfo In RegulatorySites.AsParallel Where Regulators_SiteSpecificDynamicsRegulations(SiteInfoData.Regulators) = False Select 1).ToArray

            If LQuery.IsNullOrEmpty Then
                Return True '没有处于False状态的位点，则该基因的表达处于激活状态
            Else
                Return False '有某些位点是被抑制的，则整个表达被抑制
            End If
        End Function

        Public ReadOnly Property Value As Integer Implements IDataSource(Of Long, Integer).Value, IDynamicsExpression(Of Integer).Value
            Get
                Return _InternalQuantityValue
            End Get
        End Property

        ''' <summary>
        ''' Handle value of this node object in the network.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Handle As Long Implements IDataSource(Of Long, Integer).Address
            Get
                Return CLng(__address)
            End Get
            Set(value As Long)
                __address = CInt(value)
            End Set
        End Property

        Private Property __address As Integer Implements IAddressOf.Address

        Private Sub Assign(address As Integer) Implements IAddress(Of Integer).Assign
            Me.__address = address
        End Sub

        Public Function CreateHandle() As ObjectHandle Implements IDynamicsExpression(Of Integer).get_ObjectHandle
            Return New ObjectHandle With {
                .Handle = Handle,
                .ID = Identifier
            }
        End Function

        Public Function SetConfigs(conf As Configs) As Integer Implements Configs.I_Configurable.SetConfigs
            Me._semi_Decays_delta = If(Is_RegulatorType, conf.Regulator_Decays, conf.Enzyme_Decays)
            Me.Conf = conf
            Return (From regulator As RegulationExpression
                    In (From site As SiteInfo In Me.RegulatorySites Select site.Regulators).ToArray.Unlist
                    Select regulator.SetConfigs(conf)).ToArray.Length
        End Function
    End Class
End Namespace
