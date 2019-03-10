﻿#Region "Microsoft.VisualBasic::ffefea87ad0f039085f36ba5401dc3ca, annotations\Proteomics\iTraq\iTraqTtest.vb"

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

    ' Module iTraqTtest
    ' 
    '     Function: log2Test, logFCtest
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports RDotNET
Imports RDotNET.Extensions.VisualBasic.API
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Public Module iTraqTtest

    ''' <summary>
    ''' iTraq结果的差异表达蛋白的检验计算
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="level#"></param>
    ''' <param name="pvalue#"></param>
    ''' <param name="fdrThreshold#"></param>
    ''' <param name="includesZERO"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' https://github.com/xieguigang/GCModeller.cli2R/blob/master/GCModeller.cli2R/R/log2FC_t-test.R
    ''' </remarks>
    <Extension>
    Public Function logFCtest(data As IEnumerable(Of DataSet),
                              Optional level# = 1.5,
                              Optional pvalue# = 0.05,
                              Optional fdrThreshold# = 0.05,
                              Optional includesZERO As Boolean = False,
                              Optional pairInfo As IEnumerable(Of SampleTuple) = Nothing) As DEP_iTraq()

        Dim ZERO$ = base.rep(0, times:=data.First.Properties.Count)
        Dim result As New List(Of DEP_iTraq)
        Dim NA As DefaultValue(Of Double) = 1.0# _
            .AsDefault(Function(x)
                           Dim n# = DirectCast(x, Double)
                           Return n = 0R OrElse
                                  n.IsNaNImaginary
                       End Function)
        Dim sampleTuple$() = pairInfo _
            .SafeQuery _
            .Select(Function(t) t.Label) _
            .ToArray

        If sampleTuple.Length = 0 Then
            ' 如果没有设置配对，则直接选取所有的数据出来
            sampleTuple = data _
                .First _
                .Properties _
                .Keys _
                .ToArray
        End If

        For Each row As DataSet In data

            Dim value As New DEP_iTraq With {
                .ID = row.ID,
                .Properties = row.Properties _
                    .Subset(sampleTuple) _
                    .ToDictionary(Function(x) x.Key,
                                  Function(x) Math.Log(x.Value, 2).ToString)
            }

            If row.Properties.Values.All(Function(x) x = 0R) Then

                ' 所有的值都是0的话，是无法进行假设检验的
                ' 但是这种情况可能是实验A之中没有表达量，但是在实验B之中被检测到了表达

                If includesZERO Then
                    value.FCavg = 0
                    value.pvalue = 0 ' 所有的实验重复都是这种情况，则重复性很好，pvalue非常非常小？？
                Else
                    value.FCavg = Double.NaN
                    value.pvalue = Double.NaN
                End If

            Else

                ' 使用1补齐NA/0
                Dim v As Vector = row.Properties _
                    .Values _
                    .Select(Function(x) x Or NA) _
                    .AsVector

                value.FCavg = v.Average
                value.log2FC = Math.Log(value.FCavg, 2)
                value.pvalue = stats.Ttest(
                    x:=base.c(Vector.Log(v, 2)),
                    y:=ZERO,
                    varEqual:=True).pvalue
            End If

            result += value
        Next

        Return result.ApplyDEPFilter(level, pvalue, fdrThreshold)
    End Function

    ''' <summary>
    ''' 不做生物学重复检验，只计算log2FC结果来获取差异蛋白结果
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="level#"></param>
    ''' <returns></returns>
    <Extension>
    Public Function log2Test(data As IEnumerable(Of DataSet), Optional level# = 1.5) As DEP_iTraq()
        Dim log2FCThreshold# = Math.Log(level, 2)

        Return data _
            .Select(Function(protein)
                        Return createResult(protein, log2FCThreshold)
                    End Function) _
            .ToArray
    End Function

    Private Function createResult(protein As DataSet, threshold#) As DEP_iTraq
        Dim FC As Double = protein.Properties.Values.Average
        Dim log2FC = Math.Log(FC, 2)

        Return New DEP_iTraq With {
            .ID = protein.ID,
            .log2FC = log2FC,
            .FCavg = FC,
            .FDR = 0,
            .pvalue = 0,
            .Properties = protein _
                .Properties _
                .AsCharacter,
            .isDEP = Math.Abs(.log2FC) >= threshold
        }
    End Function
End Module
