#Region "Microsoft.VisualBasic::f22cc40ee4559ebb9ebe2f2f487ea3b7, engine\GCModeller\EngineSystem\ObjectModels\Feature\Motif.vb"

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

    '     Class MotifSite
    ' 
    '         Properties: MappingHandler, Regulators, TypeId
    ' 
    '         Function: get_RegulationEffect, get_RegulatorQuantitySum, Initialize, MostAppears, Regulators_SiteSpecificDynamicsRegulations
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.CentralDogmaInstance

Namespace EngineSystem.ObjectModels.Feature

    ''' <summary>
    ''' The regulation motif site information.(基因组之上的调控位点的信息)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MotifSite(Of T_EventClass As EngineSystem.ObjectModels.Module.CentralDogmaInstance.ExpressionProcedureEvent.I_EventProcess) : Inherits EngineSystem.ObjectModels.Feature.MappingFeature(Of PoolMappings.MotifClass)

        Public Overrides ReadOnly Property MappingHandler As PoolMappings.MotifClass
            Get

            End Get
        End Property

        Public Property Regulators As SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Entity.Regulator(Of T_EventClass)()

        Public Function Initialize() As MotifSite(Of T_EventClass)
            For Each Regulator In Regulators
                Regulator.set_RegulatesMotifSite(Me)
            Next

            Return Me
        End Function

        ''' <summary>
        ''' 0表示被抑制，则无法转录
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function get_RegulationEffect() As Double
            Return Regulators_SiteSpecificDynamicsRegulations(Regulators)
        End Function

        Private Shared Function Regulators_SiteSpecificDynamicsRegulations(siteRegulators As Entity.Regulator(Of T_EventClass)()) As Double
            Dim DLQuery = (From item In siteRegulators Select item.RegulateValue).ToArray

            If DLQuery.IsNullOrEmpty Then '没有任何调控因子是处于活跃状态的
                GoTo BASAL_EXPRESSION
            End If

            Dim Effects As Boolean = MostAppears((From n In DLQuery Select n > 0).ToArray)
            Dim effect As Double = If(Effects, (From n In DLQuery Where n >= 0 Select n).ToArray.Sum, (From n In DLQuery Where n <= 0 Select n).ToArray.Sum)

            '   Call Randomize()
            If Rnd() >= 0.4 Then 'factor越大，则阈值越低，即事件越容易发生
                Return effect
            Else
BASAL_EXPRESSION:
                '  Call Randomize()

                If Rnd() < 0.1 Then '默认状态是不激活，有较低的概率处于激活状态，即本底表达
                    Return 0.1
                Else
                    Return -1
                End If
            End If
        End Function

        Private Shared Function MostAppears(Status As Boolean()) As Boolean
            Dim TLQuery = (From n In Status Where n = True Select 1).ToArray.Length
            Dim FLQuery = Status.Length - TLQuery

            If TLQuery > FLQuery Then  '在细胞群之中激活的数目多余抑制的数目，则在转录组水平上整体呈现激活的状态
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' 获取当前的这个位点之上的调控因子的总数目，以用于计算权重
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Function get_RegulatorQuantitySum() As Integer
            Dim LQuery = (From Regulator In Regulators Select Regulator.Quantity).ToArray.Sum
            Return LQuery
        End Function

        Public Overrides ReadOnly Property TypeId As ObjectModel.TypeIds
            Get
                Return TypeIds.FeatureMotifSite
            End Get
        End Property
    End Class
End Namespace
