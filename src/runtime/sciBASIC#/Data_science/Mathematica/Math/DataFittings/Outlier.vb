﻿#Region "Microsoft.VisualBasic::5259795d0603abe8e84b073d48bcdc2b, Data_science\Mathematica\Math\DataFittings\Outlier.vb"

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

    ' Module Outlier
    ' 
    '     Function: DeleteOutlier, OrderSequenceOutlierIndex
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Quantile

Public Module Outlier

    ''' <summary>
    ''' 这个函数只适用于线性回归的异常点自动删除操作，算法原理是：
    ''' 
    ''' 每一个点，减去最小值之后，可以得到一个从原点出发的二维向量
    ''' 这个二维向量可以计算出一个相对于X轴的夹角
    ''' 由于是线性回归，所以所有的正常点的夹角应该是几乎一样的
    ''' 对于异常点而言，则可能会明显变大或者变小，这个时候使用四分位数就可以很容易的检测出来了
    ''' </summary>
    ''' <param name="points"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function DeleteOutlier(points As IEnumerable(Of PointF)) As IEnumerable(Of PointF)
        Dim lineVector = points.ToArray
        Dim minX = Aggregate p In lineVector Into Min(p.X)
        Dim minY = Aggregate p In lineVector Into Min(p.Y)
        Dim angles = lineVector.Select(Function(p) Trigonometric.GetAngle(minX, minY, p.X, p.Y)).AsVector
        Dim quartile = angles.Quartile
        Dim outliers = angles.Outlier(quartile).Outlier

        For i As Integer = 0 To lineVector.Length - 1
            Dim a = angles(i)

            If outliers.Any(Function(x) Math.Abs(x - a) <= 0.001) Then
                ' 这是一个异常点
            Else
                ' 返回正常点
                Yield lineVector(i)
            End If
        Next
    End Function

#Region "Order sequence outlier"

    ' 假设有这样子的一个有序序列
    ' 可以发现除了第一个数据点，序列中其他的点都是递减的
    ' 所以第一个点应该是这个有序序列之中的异常点
    '
    ' 0.010228592, 2.278282642, 0.922615588, 0.472233653, 0.234864831, 0.117762581, 0.051605649
    '
    ' 因为四分位数方法是无差别对待序列之中的所有点的，所以按照四分位数方法计算出来的异常点为2.278282642
    ' 所以在这里不可以使用四分位数方法来检查有序序列中的异常点

    ''' <summary>
    ''' 采用滑窗外加变异量计算来得到异常点
    ''' </summary>
    ''' <param name="seq"></param>
    ''' <returns></returns>
    <Extension>
    Public Function OrderSequenceOutlierIndex(seq As Vector) As IEnumerable(Of Integer)
        Dim delta = seq.SlideWindows(winSize:=2) _
                       .Select(Function(d)
                                   ' 计算当前的这个滑窗内的变异量
                                   Return d(1) - d(0)
                               End Function) _
                       .AsVector

        Dim normalSign = If(delta(delta > 0).Count > delta(delta <= 0).Count, 1, -1)

        If normalSign = 1 Then
            Return Which.IsTrue(delta < 0)
        Else
            Return Which.IsTrue(delta > 0)
        End If
    End Function
#End Region
End Module
