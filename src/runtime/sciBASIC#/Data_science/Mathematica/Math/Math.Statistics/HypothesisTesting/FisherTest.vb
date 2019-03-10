﻿#Region "Microsoft.VisualBasic::ad506eeaa5d74a029216b6bbf93c0c34, Data_science\Mathematica\Math\Math.Statistics\HypothesisTesting\FisherTest.vb"

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

    ' Module FisherTest
    ' 
    '     Function: FactorialDivide, FisherPvalue, product
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

''' <summary>
''' ### Fisher's exact test
''' 
''' > https://en.wikipedia.org/wiki/Fisher's_exact_test
''' 
''' Fisher 's exact test is a statistical significance test used in the 
''' analysis of contingency tables.[1][2][3] Although in practice it is 
''' employed when sample sizes are small, it is valid for all sample sizes. 
''' It is named after its inventor, Ronald Fisher, and is one of a class 
''' of exact tests, so called because the significance of the deviation 
''' from a null hypothesis (e.g., P-value) can be calculated exactly, 
''' rather than relying on an approximation that becomes exact in the limit 
''' as the sample size grows to infinity, as with many statistical tests.
''' 
''' Fisher Is said To have devised the test following a comment from Muriel 
''' Bristol, who claimed to be able to detect whether the tea Or the milk was 
''' added first to her cup. He tested her claim in the "lady tasting tea" 
''' experiment.
''' </summary>
Public Module FisherTest

    ''' <summary>
    ''' 
    ''' |                |**Men**|**Women**|*Row Total*|
    ''' |----------------|-------|---------|-----------|
    ''' |**Studying**    |   a   |    b    |   a + b   | 
    ''' |**Non-Studying**|   c   |    d    |   c + d   |
    ''' |*Column Total*  | a + c |  b + d  | a+b+c+d=n |
    ''' 
    ''' ```
    ''' p = ((a+b)!(c+d)!(a+c)!(b+d)!)/(a!b!c!d!n!)
    ''' ```
    ''' </summary>
    ''' <param name="a#"></param>
    ''' <param name="b#"></param>
    ''' <param name="c#"></param>
    ''' <param name="d#"></param>
    ''' <returns></returns>
    Public Function FisherPvalue(a#, b#, c#, d#) As Double
        Dim sX = FactorialSequence(a + b).AsList +
                 FactorialSequence(c + d) +
                 FactorialSequence(a + c) +
                 FactorialSequence(b + d)
        Dim N = a + b + c + d
        Dim sY = FactorialSequence(a).AsList +
                 FactorialSequence(b) +
                 FactorialSequence(c) +
                 FactorialSequence(d) +
                 FactorialSequence(N)
        Dim p = FisherTest.FactorialDivide(sX, sY)
        Return p
    End Function

    ''' <summary>
    ''' 在做基因富集的时候，背景基因的数量会达到上万个，直接计算阶乘的积是无法被计算出来的
    ''' 在这里使用约分来降低数量级
    ''' </summary>
    ''' <param name="X"></param>
    ''' <param name="Y"></param>
    ''' <returns></returns>
    Private Function FactorialDivide(X As List(Of Integer), Y As List(Of Integer)) As Double
        Dim gx = X.GroupBy(Function(n) n).ToDictionary(Function(n) n.Key, Function(n) n.Count)
        Dim gy = Y.GroupBy(Function(n) n).ToDictionary(Function(n) n.Key, Function(n) n.Count)

        ' 将相同的因子在分子和分母之间约掉
        Dim dx As New List(Of KeyValuePair(Of Integer, Integer))

        For Each factor In gx
            If gy.ContainsKey(factor.Key) Then
                ' 取最少的
                Dim min = VBMath.Min(factor.Value, gy(factor.Key))
                dx.Add(New KeyValuePair(Of Integer, Integer)(factor.Key, factor.Value - min))
                gy(factor.Key) -= min
            End If
        Next

        Dim px = dx.ToDictionary.product
        Dim py = gy.product

        Return px / py
    End Function

    <Extension>
    Private Function product(x As Dictionary(Of Integer, Integer)) As Double
        Return x.Select(Function(n) n.Key ^ n.Value).ProductALL
    End Function
End Module
