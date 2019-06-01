#Region "Microsoft.VisualBasic::5e1798278a56912da2df352ead4f1c24, Proteomics\LabelFree\FoldChangeMatrix.vb"

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

    ' Module FoldChangeMatrix
    ' 
    '     Function: (+2 Overloads) TotalSumNormalize
    '     Enum InferMethods
    ' 
    '         Average, Min
    ' 
    ' 
    ' 
    '  
    ' 
    '     Function: inferByAverage, inferByMin, (+2 Overloads) iTraqMatrix, iTraqMatrixNormalized, SimulateMissingValues
    '               SimulateMissingValuesByProtein, SimulateMissingValuesBySample
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

''' <summary>
''' 直接进行FoldChange比较的误差会非常大,在这里可以将原始数据进行处理,使用iTraq方法进行数据分析
''' </summary>
Public Module FoldChangeMatrix

    ''' <summary>
    ''' 总峰归一化
    ''' </summary>
    ''' <param name="sample#"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function TotalSumNormalize(sample As Vector) As Double()
        Return sample / sample.Sum
    End Function

    ''' <summary>
    ''' 对原始峰面积矩阵进行总峰归一化
    ''' </summary>
    ''' <param name="rawMatrix"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function TotalSumNormalize(rawMatrix As IEnumerable(Of DataSet)) As IEnumerable(Of DataSet)
        Dim data As DataSet() = rawMatrix.ToArray
        Dim samples = data.PropertyNames
        Dim normalized = samples _
            .ToDictionary(Function(name) name,
                          Function(name)
                              Return TotalSumNormalize(data.Vector(name))
                          End Function)
        Dim index%

        For i As Integer = 0 To data.Length - 1
            index = i

            Yield New DataSet With {
                .ID = data(i).ID,
                .Properties = normalized _
                    .ToDictionary(Function(sample) sample.Key,
                                  Function(sample) sample.Value(index))
            }
        Next
    End Function

    ''' <summary>
    ''' 缺失值的模拟推断方法
    ''' </summary>
    Public Enum InferMethods
        Average
        Min
    End Enum

    <Extension>
    Public Function SimulateMissingValues(rawMatrix As IEnumerable(Of DataSet), Optional byRow As Boolean = True, Optional infer As InferMethods = InferMethods.Average) As IEnumerable(Of DataSet)
        Dim method As Func(Of Vector, Double)

        If infer = InferMethods.Average Then
            method = AddressOf inferByAverage
        Else
            method = AddressOf inferByMin
        End If

        If byRow Then
            Return rawMatrix.SimulateMissingValuesByProtein(infer:=method)
        Else
            Return rawMatrix.SimulateMissingValuesBySample(infer:=method)
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Private Function inferByAverage(iBAQ As Vector) As Double
        Return iBAQ.Average
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Private Function inferByMin(iBAQ As Vector) As Double
        Return iBAQ.Where(Function(x) x > 0).Min
    End Function

    <Extension>
    Public Iterator Function SimulateMissingValuesByProtein(rawMatrix As IEnumerable(Of DataSet), infer As Func(Of Vector, Double)) As IEnumerable(Of DataSet)
        For Each protein As DataSet In rawMatrix
            Dim iBAQ As Vector = protein.Vector

            If iBAQ.Min = 0R Then
                ' 有缺失值
                ' 需要对缺失值使用平均值/最小值来代替
                With infer(iBAQ)
                    For Each sampleName As String In protein.EnumerateKeys
                        If protein(sampleName) = 0R Then
                            protein(sampleName) = .ByRef
                        End If
                    Next
                End With
            End If

            Yield protein
        Next
    End Function

    <Extension>
    Public Function SimulateMissingValuesBySample(rawMatrix As IEnumerable(Of DataSet), infer As Func(Of Vector, Double)) As IEnumerable(Of DataSet)
        Dim data As DataSet() = rawMatrix.ToArray
        Dim sampleNames$() = data.PropertyNames

        For Each sampleName As String In sampleNames
            Dim iBAQ As Vector = data.Vector(sampleName)

            If iBAQ.Min = 0R Then
                ' 有缺失值
                ' 需要对缺失值使用平均值/最小值来代替
                With infer(iBAQ)
                    For Each protein As DataSet In data
                        If protein(sampleName) = 0R Then
                            protein(sampleName) = .ByRef
                        End If
                    Next
                End With
            End If
        Next

        Return data
    End Function

    ''' <summary>
    ''' 生成的matrix里面的foldchange结果是``experiment/controls``
    ''' </summary>
    ''' <param name="rawMatrix">原始的峰面积数据</param>
    ''' <param name="analysis"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Iterator Function iTraqMatrix(rawMatrix As DataSet(),
                                         analysis As (experiments As String(), controls As String()),
                                         Optional normalize As Dictionary(Of String, Double) = Nothing) As IEnumerable(Of DataSet)
        Dim a#, b#
        Dim requireNormalized As Boolean = Not normalize Is Nothing

        For Each protein As DataSet In rawMatrix
            Dim foldChanges As New Dictionary(Of String, Double)

            For Each experiment As String In analysis.experiments
                For Each control As String In analysis.controls
                    If (protein(control)) = 0R Then
                        foldChanges($"{experiment}/{control}") = 0
                    Else
                        If requireNormalized Then
                            a = protein(experiment) / normalize(experiment)
                            b = protein(control) / normalize(control)
                        Else
                            a = protein(experiment)
                            b = protein(control)
                        End If

                        foldChanges($"{experiment}/{control}") = a / b
                    End If
                Next
            Next

            Yield New DataSet With {
                .ID = protein.ID,
                .Properties = foldChanges
            }
        Next
    End Function

    <Extension>
    Public Function iTraqMatrixNormalized(rawMatrix As DataSet(), analysis As (experiments As String(), controls As String())) As IEnumerable(Of DataSet)
        Dim totalSum = (analysis.experiments.AsList + analysis.controls) _
            .ToDictionary(Function(name) name,
                          Function(name)
                              Return rawMatrix.Sum(Function(protein) protein(name))
                          End Function)
        Return rawMatrix.iTraqMatrix(analysis, totalSum)
    End Function

    Public Iterator Function iTraqMatrix(rawMatrix As IEnumerable(Of DataSet),
                                         sampleInfo As SampleGroup(),
                                         analysisDesigners As AnalysisDesigner(),
                                         Optional normalize As Boolean = False) As IEnumerable(Of NamedCollection(Of DataSet))
        Dim groups = sampleInfo _
            .GroupBy(Function(s) s.sample_group) _
            .ToDictionary(Function(g) g.Key,
                          Function(g) g.Select(Function(s) s.sample_name).ToArray)

        With rawMatrix.ToArray
            For Each designer As AnalysisDesigner In analysisDesigners
                Dim controls$() = groups(designer.Controls)
                Dim treatment$() = groups(designer.Treatment)
                Dim matrix As DataSet()

                If normalize Then
                    matrix = .iTraqMatrixNormalized((treatment, controls)) _
                             .ToArray
                Else
                    matrix = .iTraqMatrix((treatment, controls)) _
                             .ToArray
                End If

                Yield New NamedCollection(Of DataSet) With {
                    .Name = designer.Title,
                    .Value = matrix,
                    .Description = designer.ToString
                }
            Next
        End With
    End Function
End Module

