﻿#Region "Microsoft.VisualBasic::de599077299f58909ed99459b82739f3, Data_science\Visualization\NeuronNetworkExtensions.vb"

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

    ' Module NeuronNetworkExtensions
    ' 
    '     Function: CastTo, VisualizeModel
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.DataMining.Kernel.Classifier
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.NeuralNetwork.StoreProcedure
Imports Microsoft.VisualBasic.Math.Quantile
Imports NeuronNetwork = Microsoft.VisualBasic.MachineLearning.NeuralNetwork.Network

''' <summary>
''' 网络可视化工具
''' </summary>
Public Module NeuronNetworkExtensions

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="row">第一个元素为分类，其余元素为属性</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CastTo(row As RowObject) As NeuronEntity
        Dim LQuery = From s As String In row.Skip(1) Select Val(s) '

        Return New NeuronEntity With {
            .Y = Val(row.First),
            .Properties = LQuery.ToArray
        }
    End Function

    ''' <summary>
    ''' 将人工神经网络的对象模型转换为网络数据模型以进行可视化操作
    ''' </summary>
    ''' <param name="net"></param>
    ''' <returns></returns>
    <Extension> Public Function VisualizeModel(net As NeuronNetwork, Optional connectionCutoff# = 0.6) As NetworkTables
        Dim model = NeuralNetwork.Snapshot(net)
        Dim inputLayer = model.inputlayer.neurons.Indexing
        Dim outputLayer = model.outputlayer.neurons.Indexing
        Dim hiddens = model.hiddenlayers _
            .layers _
            .Select(Function(l, i)
                        Return New NamedValue(Of Index(Of String)) With {
                            .Name = $"hidden_layer{i}",
                            .Value = l.neurons.Indexing
                        }
                    End Function) _
            .ToArray
        Dim nodes = model.neurons _
            .Select(Function(n)
                        Dim type$

                        If n.id.IsOneOfA(inputLayer) Then
                            type = "input"
                        ElseIf n.id.IsOneOfA(outputLayer) Then
                            type = "output"
                        Else
                            type = "NA"

                            For Each layer In hiddens
                                If layer.Value.IndexOf(n.id) > -1 Then
                                    type = layer.Name
                                    Exit For
                                End If
                            Next
                        End If

                        Return New Node With {
                            .ID = n.id,
                            .NodeType = type,
                            .Properties = New Dictionary(Of String, String) From {
                                {"bias", n.bias},
                                {"delta", n.delta},
                                {"gradient", n.gradient}
                            }
                        }
                    End Function) _
            .ToArray
        Dim weights As QuantileEstimationGK = model _
            .connections _
            .Select(Function(syn) Math.Abs(syn.w)) _
            .GKQuantile
        Dim threshold# = weights.Query(connectionCutoff)
        Dim edges = model.connections _
            .Where(Function(syn) Math.Abs(syn.w) >= threshold) _
            .Select(Function(syn)
                        Return New NetworkEdge With {
                            .FromNode = syn.in,
                            .ToNode = syn.out,
                            .value = syn.w
                        }
                    End Function) _
            .ToArray

        Return New NetworkTables With {
            .Nodes = nodes,
            .Edges = edges
        }
    End Function

End Module
