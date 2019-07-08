﻿#Region "Microsoft.VisualBasic::faf52a4319703c35d813c929d9982e5e, Data_science\MachineLearning\MachineLearning\Darwinism\GeneticAlgorithm\Helper\GeneticHelper.vb"

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

'     Module GeneticHelper
' 
'         Function: InitialPopulation
' 
'         Sub: ByteMutate, Crossover, (+2 Overloads) Mutate
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.Models

Namespace Darwinism.GAF.Helper

    Public Module GeneticHelper

        ' 2018-4-29
        '
        ' 关于random对象的食用说明：尽量不要创建新的random对象
        ' 从下面的测试可以看得出来，当使用就的random对象的时候，可以生成一系列的伪随机数
        ' 但是如果进行random对象的创建的话，则几乎不会再生成新的随机数
        '
        ' With New Random
        '    For i As Integer = 0 To 100
        '        Call .NextDouble.__DEBUG_ECHO
        '    Next
        '
        '    Call "==============================================".__INFO_ECHO
        '
        '    For i As Integer = 0 To 100
        '        Call New Random().NextDouble.__DEBUG_ECHO
        '    Next
        ' End With

#Region "Numeric Value Mutation"

        ' 20190709
        ' integer类型不适合突变计算
        ' 所有的染色体都应该是double类型的
        ' 所以在这里将integer类型的突变帮助函数删除了

        ''' <summary>
        ''' Returns clone of current chromosome, which is mutated a bit
        ''' </summary>
        ''' <param name="v#"></param>
        ''' <param name="random"></param>
        ''' <param name="index">
        ''' + 如果这个坐标参数大于等于零,则会直接按照这个坐标值对指定位置的目标进行突变
        ''' + 反之小于零的时候,则是随机选取一个位置的目标进行突变
        ''' </param>
        ''' <remarks>
        ''' 在进行突变的时候应该是按照给定的范围来进行突变的
        ''' </remarks>
        <Extension> Public Sub Mutate(ByRef v#(), random As Random, Optional index% = -1000)
            Dim delta# = (v.Max - v.Min) / 10
            Dim mutationValue#

            ' 20190709 如果v向量全部都是零或者相等数值的话
            ' 将无法产生突变
            ' 在这里测试下，添加一个小数来完成突变
            If delta = 0R Then
                delta = 0.0000001
            End If

            mutationValue = (random.NextDouble * delta) * If(random.NextDouble >= 0.5, 1, -1)

            If index < 0 Then
                v(random.Next(v.Length)) += mutationValue
            Else
                v(index) += mutationValue
            End If
        End Sub
#End Region

        ''' <summary>
        ''' 这个函数不是数值变化，而是位值的变化，原来的某位数值为1，则突变后为零，原来某位数值为0，则突变之后为1
        ''' </summary>
        ''' <param name="v%"></param>
        ''' <param name="random"></param>
        <Extension>
        Public Sub ByteMutate(ByRef v%(), random As Random)
            Dim index = random.Next(v.Length)

            If v(index) = 0 Then
                v(index) = 1
            Else
                v(index) = 0
            End If
        End Sub

        ''' <summary>
        ''' Returns list of siblings 
        ''' Siblings are actually new chromosomes, 
        ''' created using any of crossover strategy
        ''' 
        ''' (两个向量的长度必须要一致)
        ''' </summary>
        ''' <param name="random"></param>
        ''' <param name="v1#"></param>
        ''' <param name="v2#"></param>
        <Extension>
        Public Sub Crossover(Of T)(random As Random, ByRef v1 As T(), ByRef v2 As T())
            Dim index As Integer = random.Next(v1.Length - 1)
            Dim tmp As T

            ' one point crossover
            For i As Integer = index To v1.Length - 1
                tmp = v1(i)
                v1(i) = v2(i)
                v2(i) = tmp
            Next
        End Sub

        ''' <summary>
        ''' The simplest strategy for creating initial population <br/>
        ''' in real life it could be more complex.
        ''' 
        ''' (如果<paramref name="parallel"/>计算函数是空值，则整个GA的计算过程为串行计算过程)
        ''' </summary>
        <Extension>
        Public Function InitialPopulation(Of T As Chromosome(Of T))(base As T, populationSize%, Optional parallel As ParallelComputing(Of T) = Nothing) As Population(Of T)
            Dim chr As T
            Dim population As New Population(Of T)(parallel) With {
                .parallel = True
            }

            For i As Integer = 0 To populationSize - 1
                ' each member of initial population
                ' is mutated clone of base chromosome
                chr = base.Mutate()
                population.Add(chr)
            Next

            Return population
        End Function
    End Module
End Namespace
