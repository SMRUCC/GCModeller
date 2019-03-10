﻿#Region "Microsoft.VisualBasic::968f0a58f3b26268510393140db7e27c, Data_science\Mathematica\Math\Math\Distributions\Bootstraping.vb"

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

    '     Module Bootstraping
    ' 
    '         Function: Distributes, Hist, Sample, (+2 Overloads) Samples, Sampling
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports sys = System.Math

Namespace Distributions

    ''' <summary>
    ''' Data sampling bootstrapping extensions
    ''' </summary>
    Public Module Bootstraping

        ''' <summary>
        ''' Generate a numeric <see cref="Vector"/> by <see cref="Permutation"/> <paramref name="x"/> times.
        ''' </summary>
        ''' <param name="x%"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Sample(x As Integer) As Vector
            Return New Random().Permutation(x, x).AsVector
        End Function

        ''' <summary>
        ''' bootstrap是一种非参数估计方法，它用到蒙特卡洛方法。bootstrap算法如下：
        ''' 假设样本容量为N
        '''
        ''' + 有放回的从样本中随机抽取N次(所以可能x1..xn中有的值会被抽取多次)，每次抽取一个元素。并将抽到的元素放到集合S中；
        ''' + 重复**步骤1** B次（例如``B = 100``）， 得到B个集合， 记作S1, S2,…, SB;
        ''' + 对每个``Si(i=1, 2, ..., B)``，用蒙特卡洛方法估计随机变量的数字特征d，分别记作d1,d2,…,dB;
        ''' + 用d1,d2,…dB来近似d的分布；
        ''' 
        ''' 本质上，bootstrap算法是最大似然估计的一种实现，它和最大似然估计相比的优点在于，它不需要用参数来刻画总体分布。
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="source"></param>
        ''' <param name="N"></param>
        ''' <param name="B"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function Samples(Of T)(source As IEnumerable(Of T), N As Integer, Optional B As Integer = 100) As IEnumerable(Of IntegerTagged(Of T()))
            Dim array As T() = source.ToArray
            Dim rnd As New Random

            For i As Integer = 0 To B
                Dim ls As New List(Of T)

                For k As Integer = 0 To N - 1
                    ls += array(rnd.Next(array.Length))
                Next

                Yield New IntegerTagged(Of T()) With {
                    .Tag = i,
                    .Value = ls.ToArray
                }
            Next
        End Function

        <Extension>
        Public Iterator Function Sampling(source As IEnumerable(Of Double), N%, Optional B% = 100) As IEnumerable(Of IntegerTagged(Of Vector))
            For Each x As IntegerTagged(Of Double()) In Samples(source, N, B)
                Yield New IntegerTagged(Of Vector) With {
                    .Tag = x.Tag,
                    .Value = New Vector(x.Value)
                }
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function Samples(Of T)(source As IEnumerable(Of T), getValue As Func(Of T, Double), N%, Optional B% = 100) As IEnumerable(Of IntegerTagged(Of Vector))
            Return source.Select(getValue).Sampling(N, B)
        End Function

        ''' <summary>
        ''' ###### 频数分布表与直方图
        ''' 
        ''' 返回来的标签数据之中的标签是在某个区间范围内的数值集合的平均值
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="base"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Distributes(data As IEnumerable(Of Double), Optional base! = 10.0F) As Dictionary(Of Integer, DoubleTagged(Of Integer))
            Dim array As DoubleTagged(Of Double)() = data _
                .Select(Function(x)
                            Return New DoubleTagged(Of Double) With {
                                .Tag = sys.Log(x, base),
                                .Value = x
                            }
                        End Function) _
                .ToArray
            Dim min As Integer = CInt(array.Min(Function(x) x.Tag)) - 1
            Dim max As Integer = CInt(array.Max(Function(x) x.Tag)) + 1
            Dim l As VBInteger = min, low As Integer = min
            Dim out As New Dictionary(Of Integer, DoubleTagged(Of Integer))

            Do While ++l < max
                Dim LQuery As DoubleTagged(Of Double)() =
                    LinqAPI.Exec(Of DoubleTagged(Of Double)) <=
 _
                    From x As DoubleTagged(Of Double)
                    In array
                    Where x.Tag >= low AndAlso
                        x.Tag < l
                    Select x

                out(l) = New DoubleTagged(Of Integer) With {
                    .Tag = If(LQuery.Length = 0, 0, LQuery.Average(Function(x) x.Value)),
                    .Value = LQuery.Length
                }
                low = l
            Loop

            If out(min + 1).Value = 0 Then
                Call out.Remove(min)
            End If
            If out(max - 1).Value = 0 Then
                Call out.Remove(max)
            End If

            Return out
        End Function

        ''' <summary>
        ''' ###### 频数分布表与直方图
        ''' 
        ''' 这个函数返回来的是频数以及区间内的所有的数的平均值
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="step!"></param>
        ''' <returns>
        ''' 返回来的数据为区间的下限 -> {频数, 平均值}
        ''' </returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function Hist(data As Double(), Optional step! = 1) As IEnumerable(Of DataBinBox(Of Double))
            Return CutBins.FixedWidthBins(Of Double)(
                v:=data.OrderBy(Function(x) x).ToArray,
                width:=[step],
                eval:=Function(x) x
            )
        End Function
    End Module
End Namespace
