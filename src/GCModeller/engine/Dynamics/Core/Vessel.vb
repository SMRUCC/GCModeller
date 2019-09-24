#Region "Microsoft.VisualBasic::49469831fd643c138dac9f5c329b9eee, engine\Dynamics\Core\Vessel.vb"

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

    '     Class Vessel
    ' 
    '         Properties: Channels, Mass
    ' 
    '         Function: ContainerIterator, factorsByCount, iterateFlux, Reset
    ' 
    '         Sub: Initialize
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

Namespace Core

    ''' <summary>
    ''' 一个反应容器，也是一个微环境，这在这个反应容器之中包含有所有的反应过程
    ''' </summary>
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
        ''' </remarks>
        Public Property Channels As Channel()
        ''' <summary>
        ''' 当前的这个微环境之中的所有的物质列表，会包括代谢物，氨基酸，RNA等物质信息
        ''' </summary>
        ''' <returns></returns>
        Public Property Mass As Factor()

        ''' <summary>
        ''' 因为在现实中这些反应过程是同时发生的，所以在这里使用这个共享因子来模拟并行事件
        ''' </summary>
        Dim shareFactors As (left As Dictionary(Of String, Double), right As Dictionary(Of String, Double))

        Public Sub Initialize()
            Dim sharedLeft = factorsByCount(True)
            Dim sharedRight = factorsByCount(False)

            shareFactors = (sharedLeft, sharedRight)
        End Sub

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
                .GroupBy(Function(x) x.Mass.ID) _
                .ToDictionary(Function(m) m.Key,
                              Function(m)
                                  Return CDbl(m.Count)
                              End Function)
        End Function

        ''' <summary>
        ''' 当前的这个微环境的迭代器
        ''' </summary>
        Public Iterator Function ContainerIterator() As IEnumerable(Of NamedValue(Of Double))
            ' 在这里将原始序列随机打乱来模拟现实世界中的平行发生的事件
            ' 因为在这里会涉及到mass对象的值的修改
            ' 所以无法使用多线程进行并行计算
            ' 在这里只能够使用随机+串联来模拟平行事件
            For Each reaction As Channel In Channels.Shuffles
                ' 不可以使用Where直接在for循环外进行筛选
                ' 因为环境是不断地变化的
                Yield iterateFlux(reaction)
            Next
        End Function

        Private Function iterateFlux(reaction As Channel) As NamedValue(Of Double)
            Dim regulate#
            Dim flow As Directions = reaction.Direction

            Select Case flow
                Case Directions.forward
                    ' 消耗左边，产生右边
                    regulate = reaction.Forward.Coefficient

                    If regulate > 0 Then
                        ' 当前是具有调控效应的
                        ' 接着计算最小的反应单位
                        regulate = reaction.CoverLeft(shareFactors.left, regulate)
                    End If
                    If regulate > 0 Then
                        ' 当前的过程是可以进行的
                        ' 则进行物质的转义的计算
                        Call reaction.Transition(regulate, flow)
                    End If
                Case Directions.reverse
                    regulate = reaction.Reverse.Coefficient

                    If regulate > 0 Then
                        regulate = reaction.CoverRight(shareFactors.right, regulate)
                    End If
                    If regulate > 0 Then
                        Call reaction.Transition(regulate, flow)
                    End If
                Case Else
                    ' no reaction will be happends
                    regulate = 0
            End Select

            Return New NamedValue(Of Double)(reaction.ID, flow * regulate)
        End Function

        ''' <summary>
        ''' 重置反应环境模拟器之中的内容
        ''' </summary>
        ''' <param name="massInit"></param>
        ''' <returns></returns>
        Public Function Reset(massInit As Dictionary(Of String, Double)) As Vessel
            For Each mass As Factor In Me.Mass
                mass.Value = massInit(mass.ID)
            Next

            Return Me
        End Function
    End Class
End Namespace
