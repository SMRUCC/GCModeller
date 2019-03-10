﻿#Region "Microsoft.VisualBasic::a07f5798a2590ed0ab3510ece83ed75b, gr\network-visualization\Visualizer\Styling\Expression\Syntax.vb"

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

    '     Enum MapperTypes
    ' 
    '         Continuous, Discrete, Passthrough
    ' 
    '  
    ' 
    ' 
    ' 
    '     Module SyntaxExtensions
    ' 
    '         Function: IsMapExpression, MapExpressionParser, RangeTransform
    ' 
    '     Structure MapExpression
    ' 
    '         Properties: AsDictionary
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Styling

    ''' <summary>
    ''' 从graph的属性值到相应的图形属性(节点大小，颜色，字体，形状)的映射操作类型
    ''' </summary>
    Public Enum MapperTypes
        ''' <summary>
        ''' 连续的数值型的映射
        ''' </summary>
        Continuous
        ''' <summary>
        ''' 离散的分类映射
        ''' </summary>
        Discrete
        ''' <summary>
        ''' 直接映射
        ''' </summary>
        Passthrough
    End Enum

    Public Module SyntaxExtensions

        ''' <summary>
        ''' 表达式之中的值不可以有逗号或者括号
        ''' </summary>
        ''' <param name="expression$">
        ''' + 区间映射 map(word, [min, max])
        ''' + 离散映射 map(word, val1=map1, val2=map2, ...)
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function MapExpressionParser(expression As String) As MapExpression
            Dim t$() = expression _
                .GetStackValue("(", ")") _
                .StringSplit("\s*,\s*")
            Dim values$()

            If t.Length = 3 AndAlso t(1).First = "["c AndAlso t(2).Last = "]"c Then
                values = New String() {
                    t(1).Substring(1),
                    t(2).Substring(0, t(2).Length - 1)
                }
                Return New MapExpression With {
                    .propertyName = t(0),
                    .type = MapperTypes.Continuous,
                    .values = values
                }
            Else
                Return New MapExpression With {
                    .propertyName = t(0),
                    .type = MapperTypes.Discrete,
                    .values = t.Skip(1).ToArray
                }
            End If
        End Function

        ''' <summary>
        ''' 因为可能会存在前导或者后置的空格，所以在这里就直接做模式匹配而不是绝对模式匹配了
        ''' </summary>
        ''' <param name="expression"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function IsMapExpression(expression As String) As Boolean
            Return expression.MatchPattern("map\(.+\)", RegexICSng)
        End Function

        ''' <summary>
        ''' 将节点列表进行区间映射，这个函数返回``[节点值，目标区间值]``
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="nodes"></param>
        ''' <param name="eval"></param>
        ''' <param name="range">将节点的大小映射到这个半径大小的区间之内</param>
        ''' <returns>``[节点值，目标区间值]``， 这个函数返回来的序列之中的元素的顺序是和函数参数所输入的节点序列之中的元素顺序是一致的</returns>
        <Extension> Public Function RangeTransform(Of T As Class)(nodes As IEnumerable(Of T), eval As Func(Of T, Double), range As DoubleRange) As Map(Of T, Double)()
            Dim array As T() = nodes.ToArray
            Dim degrees#() = array.Select(eval).ToArray
            Dim size#() = degrees.RangeTransform([to]:=range)
            Dim out As Map(Of T, Double)() =
                array _
                .SeqIterator _
                .Select(Function(x)
                            Return New Map(Of T, Double) With {
                                .Key = x.value,
                                .Maps = size(x)
                            }
                        End Function) _
                .ToArray

            Return out
        End Function
    End Module

    Public Structure MapExpression

        Dim propertyName As String
        Dim type As MapperTypes
        Dim values As String()

        Public ReadOnly Property AsDictionary As Dictionary(Of String, String)
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return values _
                    .Select(Function(s) s.GetTagValue("=", trim:=True)) _
                    .ToDictionary(Function(t) t.Name,
                                  Function(t) t.Value)
            End Get
        End Property

        Public Overrides Function ToString() As String
            If type = MapperTypes.Continuous Then
                Return $"Dim '{propertyName}' = [{values.JoinBy(", ")}]"
            Else
                Return $"Dim '{propertyName}' = {Me.AsDictionary.GetJson}"
            End If
        End Function
    End Structure
End Namespace
