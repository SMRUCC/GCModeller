﻿#Region "Microsoft.VisualBasic::c67f38bde7bcac06d65ec738a32ee4de, engine\Dynamics\Core\Vessel.vb"

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

'   Total Lines: 208
'    Code Lines: 104 (50.00%)
' Comment Lines: 74 (35.58%)
'    - Xml Docs: 79.73%
' 
'   Blank Lines: 30 (14.42%)
'     File Size: 7.80 KB


'     Class Vessel
' 
'         Properties: Channels, MassEnvironment
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: ContainerIterator, factorsByCount, getMassValues, Initialize, (+2 Overloads) load
'                   Reset
' 
'         Sub: fp_dfdx_parallel, fp_dfdx_sequence
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine
Imports std_vec = Microsoft.VisualBasic.Math.LinearAlgebra.Vector

Namespace Core

    ''' <summary>
    ''' 一个反应容器，也是一个微环境，这在这个反应容器之中包含有所有的反应过程
    ''' 
    ''' (这是一个通用的生物化学反应过程模拟器，是整个虚拟细胞计算引擎的核心部件)
    ''' </summary>
    ''' <remarks>
    ''' a container of a set of the reaction <see cref="Channel"/> and mass <see cref="Factor"/>.
    ''' </remarks>
    Public Class Vessel

        ''' <summary>
        ''' 当前的这个微环境之中的所有的反应过程列表，在这个集合之中包括有：
        ''' 
        ''' 1. 代谢过程
        ''' 2. 转录过程
        ''' 3. 翻译过程
        ''' 4. 跨膜转运过程
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 虚拟细胞中的生命活动过程事件网络
        ''' 
        ''' 相当于常微分方程系统之中的一个y方程?
        ''' </remarks>
        Public ReadOnly Property Channels As Channel()
            Get
                Return m_channels
            End Get
        End Property

        ''' <summary>
        ''' 当前的这个微环境之中的所有的物质列表，会包括代谢物，氨基酸，RNA等物质信息
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property MassEnvironment As MassFactor()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return m_massIndex.Values.ToArray
            End Get
        End Property

        ReadOnly is_debug As Boolean = False

        ''' <summary>
        ''' 因为在现实中这些反应过程是同时发生的，所以在这里使用这个共享因子来模拟并行事件
        ''' </summary>
        Friend shareFactors As (left As Dictionary(Of String, Double), right As Dictionary(Of String, Double))

        Friend m_massIndex As Dictionary(Of String, MassFactor)
        Friend m_channels As Channel()

        ''' <summary>
        ''' Dynamics wrapper to the RK4 odes solver
        ''' </summary>
        Friend m_dynamics As MassDynamics()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub New(Optional is_debug As Boolean = False)
            Me.is_debug = is_debug

            If is_debug Then
                Call VBDebugger.EchoLine("virtual cell engine will be running in debug mode.")
            End If
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function getMassValues() As Dictionary(Of String, Double)
            Return m_massIndex.Values _
                .IteratesALL _
                .ToDictionary(Function(m) m.ID & "@" & m.cellular_compartment,
                              Function(m)
                                  Return m.Value
                              End Function)
        End Function

        ''' <summary>
        ''' set mass factors to the container runtime
        ''' </summary>
        ''' <param name="massEnvir"></param>
        ''' <returns></returns>
        Public Function load(massEnvir As IEnumerable(Of Factor)) As Vessel
            m_massIndex = massEnvir _
                .GroupBy(Function(m) m.ID) _
                .ToDictionary(Function(c) c.Key,
                              Function(c)
                                  Return New MassFactor(c.Key, c)
                              End Function)
            Return Me
        End Function

        ''' <summary>
        ''' set flux network for the cellular kinetics simulation
        ''' </summary>
        ''' <param name="flux"></param>
        ''' <returns></returns>
        Public Function load(flux As IEnumerable(Of Channel)) As Vessel
            m_channels = flux.ToArray

            ' check for broken flux
            ' and then write to logfile?
            'Dim brokens = m_channels _
            '    .Where(Function(fx) fx.left.IsNullOrEmpty OrElse fx.right.IsNullOrEmpty) _
            '    .ToArray

            'm_channels = m_channels _
            '    .Where(Function(fx)
            '               Return Not (fx.left.IsNullOrEmpty OrElse fx.right.IsNullOrEmpty)
            '           End Function) _
            '    .ToArray

            Return Me
        End Function

        Public Function Initialize() As Vessel
            Dim sharedLeft = factorsByCount(True)
            Dim sharedRight = factorsByCount(False)

            shareFactors = (sharedLeft, sharedRight)
            ' create dynamics equation for 
            ' RK4 ODEs solver
            m_dynamics = MassDynamics _
                .PopulateDynamics(Me) _
                .ToArray

            Return Me
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function factorsByCount(isLeft As Boolean) As Dictionary(Of String, Double)
            Return Channels _
                .Select(Function(r)
                            If isLeft Then
                                Return r.left
                            Else
                                Return r.right
                            End If
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(var) var.mass.ID) _
                .ToDictionary(Function(m) m.Key,
                              Function(m)
                                  Return CDbl(m.Count)
                              End Function)
        End Function

        ''' <summary>
        ''' 当前的这个微环境的迭代器
        ''' </summary>
        ''' <param name="maxTime">最大的模拟时间长度</param>
        ''' <param name="resolution">时间分辨率，这个值应该是大于<paramref name="maxTime"/>参数的一个值</param>
        Public Function ContainerIterator(maxTime As Integer, resolution As Integer) As SolverIterator
            Dim df As [Function]

            If resolution < maxTime Then
                Call "invalid config of the time resolution parameter: resolution should greater than maxTime!".Warning
            End If

            If is_debug Then
                df = AddressOf fp_dfdx_sequence
            Else
                df = AddressOf fp_dfdx_parallel
            End If

            Dim ODEs As New GenericODEs(m_dynamics, df)
            Dim iterator = New SolverIterator(New RungeKutta4(ODEs)).Config(ODEs.GetY0(False), resolution, 0, maxTime)

            Return iterator
        End Function

        ''' <summary>
        ''' debug mode, run ODEs in sequential
        ''' </summary>
        ''' <param name="dx"></param>
        ''' <param name="dy"></param>
        Private Sub fp_dfdx_sequence(dx As Double, ByRef dy As std_vec)
            For Each x As MassDynamics In m_dynamics
                dy(x) = x.Evaluate()
            Next
        End Sub

        ''' <summary>
        ''' product mode, run ODEs in parallel
        ''' </summary>
        ''' <param name="dx"></param>
        ''' <param name="dy"></param>
        Private Sub fp_dfdx_parallel(dx As Double, ByRef dy As std_vec)
            For Each y In m_dynamics _
                .AsParallel _
                .WithDegreeOfParallelism(8) _
                .Select(Function(di)
                            Return (i:=di, dy:=di.Evaluate)
                        End Function)

                dy(y.i) = y.dy
            Next
        End Sub

        ''' <summary>
        ''' 重置反应环境模拟器之中的内容
        ''' </summary>
        ''' <param name="massInit"></param>
        ''' <returns></returns>
        Public Function Reset(massInit As Dictionary(Of String, Double)) As Vessel
            For Each mass As MassFactor In Me.MassEnvironment
                Call mass.reset(massInit(mass.id))
            Next

            Return Me
        End Function
    End Class
End Namespace
