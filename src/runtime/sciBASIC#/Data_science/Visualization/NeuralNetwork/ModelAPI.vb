#Region "Microsoft.VisualBasic::2d46eced2818f54ff0409cd8de0dcb0a, Data_science\DataMining\network\NeuralNetwork\ModelAPI.vb"

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

'     Module NetworkModelAPI
' 
'         Function: __edges, __node, __synapse, VisualizeModel
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.NeuralNetwork
Imports NeuronNetwork = Microsoft.VisualBasic.MachineLearning.NeuralNetwork.Network

Namespace NeuralNetwork.Models

    ''' <summary>
    ''' 网络可视化工具
    ''' </summary>
    Public Module NetworkModelAPI

        ''' <summary>
        ''' 将人工神经网络的对象模型转换为网络数据模型以进行可视化操作
        ''' </summary>
        ''' <param name="net"></param>
        ''' <returns></returns>
        <Extension> Public Function VisualizeModel(net As NeuronNetwork) As NetworkTables
            Dim network As New NetworkTables
            Dim neuronTable = (New List(Of Neuron) + net.HiddenLayer + net.InputLayer + net.OutputLayer) _
                .SeqIterator _
                .ToDictionary(Function(x) x.value,
                              Function(x) x.i)

            network += net.HiddenLayer.Select(Function(x, i) x.Select(Function(n) n.createNode(NameOf(net.HiddenLayer) & "_" & i, neuronTable))).IteratesALL
            network += net.InputLayer.Select(Function(x) x.createNode(NameOf(net.InputLayer), neuronTable))
            network += net.OutputLayer.Select(Function(x) x.createNode(NameOf(net.OutputLayer), neuronTable))

            network += net.HiddenLayer.Select(Function(x, i) x.Select(Function(n) n.createEdges(NameOf(net.HiddenLayer) & "_" & i, neuronTable))).IteratesALL.IteratesALL
            network += net.InputLayer.Select(Function(x) x.createEdges(NameOf(net.InputLayer), neuronTable)).IteratesALL
            network += net.OutputLayer.Select(Function(x) x.createEdges(NameOf(net.OutputLayer), neuronTable)).IteratesALL

            Return network
        End Function

        <Extension>
        Private Function createNode(neuron As Neuron, layer$, neuronTable As Dictionary(Of Neuron, Integer)) As Node
            Dim uid As String = neuronTable(neuron).ToString

            Return New Node With {
                .ID = uid,
                .NodeType = layer
            }
        End Function

        ''' <summary>
        ''' 网络模型之中的边是从神经元对象之间的突触链接构建的
        ''' </summary>
        ''' <param name="neuron"></param>
        ''' <param name="layer"></param>
        ''' <param name="neuronTable"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Private Function createEdges(neuron As Neuron, layer$, neuronTable As Dictionary(Of Neuron, Integer)) As IEnumerable(Of NetworkEdge)
            Return neuron.InputSynapses _
                .createEdges($"{layer}_in", neuronTable) _
                .AsList +
                   neuron _
                       .OutputSynapses _
                       .createEdges($"{layer}_out", neuronTable)
        End Function

        <Extension>
        Private Iterator Function createEdges(synapses As IEnumerable(Of Synapse), name$, neuronTable As Dictionary(Of Neuron, Integer)) As IEnumerable(Of NetworkEdge)
            For Each syn As Synapse In synapses _
                .Where(Function(s)
                           ' 忽略掉没有链接强度的神经元链接
                           Return s.Weight <> 0R
                       End Function)
                Yield syn.__synapse(name, neuronTable)
            Next
        End Function

        <Extension>
        Private Function __synapse(synapse As Synapse, layer$, neuronTable As Dictionary(Of Neuron, Integer)) As NetworkEdge
            Return New NetworkEdge With {
                .value = synapse.Weight,
                .FromNode = CStr(neuronTable(synapse.InputNeuron)),
                .ToNode = CStr(neuronTable(synapse.OutputNeuron)),
                .Interaction = layer
            }
        End Function
    End Module
End Namespace
