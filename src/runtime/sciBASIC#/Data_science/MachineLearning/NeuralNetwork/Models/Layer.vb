﻿#Region "Microsoft.VisualBasic::400fd2bc53d886d89d84984bdbabd96d, Data_science\MachineLearning\NeuralNetwork\Models\Layer.vb"

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

    '     Class Layer
    ' 
    '         Properties: Neurons, Output
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: GetEnumerator, IEnumerable_GetEnumerator, ToString
    ' 
    '         Sub: (+2 Overloads) CalculateGradient, CalculateValue, Input, UpdateWeights
    ' 
    '     Class HiddenLayers
    ' 
    '         Properties: Layers, Output, Size
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: GetEnumerator, IEnumerable_GetEnumerator, ToString
    ' 
    '         Sub: BackPropagate, ForwardPropagate
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.MachineLearning.NeuralNetwork.Activations
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace NeuralNetwork

    ''' <summary>
    ''' 输入层和输出层对象,神经元节点的数量应该和实际的问题保持一致
    ''' </summary>
    Public Class Layer : Implements IEnumerable(Of Neuron)

        Public ReadOnly Property Neurons As Neuron()

        Public ReadOnly Property Output As Double()
            Get
                Return Neurons _
                    .Select(Function(n) n.Value) _
                    .ToArray
            End Get
        End Property

        ''' <summary>
        ''' 用于XML模型的加载操作
        ''' </summary>
        Friend Sub New(neurons As Neuron())
            Me.Neurons = neurons
        End Sub

        Sub New(size%, active As IActivationFunction, Optional input As Layer = Nothing, Optional guid As VBInteger = Nothing)
            Neurons = New Neuron(size - 1) {}

            If input Is Nothing Then
                For i As Integer = 0 To size - 1
                    Neurons(i) = New Neuron(active, guid)
                Next
            Else
                For i As Integer = 0 To size - 1
                    Neurons(i) = New Neuron(input.Neurons, active, guid)
                Next
            End If
        End Sub

        ''' <summary>
        ''' 将外界的测试数据赋值到每一个神经元的<see cref="Neuron.Value"/>之上
        ''' </summary>
        ''' <param name="data"></param>
        ''' <remarks>
        ''' 没有并行的必要
        ''' </remarks>
        Public Sub Input(data As Double())
            For i As Integer = 0 To _Neurons.Length - 1
                _Neurons(i).Value = data(i)
            Next
        End Sub

        ''' <summary>
        ''' 调用这个函数将会修改突触链接的权重值，这个函数只会在训练的时候被调用
        ''' </summary>
        ''' <param name="learnRate#"></param>
        ''' <param name="momentum#"></param>
        ''' <param name="parallel"></param>
        Public Sub UpdateWeights(learnRate#, momentum#, Optional parallel As Boolean = False)
            If Not parallel Then
                For Each neuron As Neuron In Neurons
                    Call neuron.UpdateWeights(learnRate, momentum)
                Next
            Else
                With Aggregate neuron As Neuron
                     In Neurons.AsParallel
                     Into Sum(neuron.UpdateWeights(learnRate, momentum))
                End With
            End If
        End Sub

        ''' <summary>
        ''' 计算输出层的结果值,即通过这个函数的调用计算出分类的结果值,结果值为``[0,1]``之间的小数
        ''' </summary>
        ''' <remarks>
        ''' 因为输出层的节点数量比较少,所以这里应该也没有并行的必要?
        ''' </remarks>
        Public Sub CalculateValue(Optional parallel As Boolean = False)
            If Not parallel Then
                For Each neuron As Neuron In Neurons
                    Call neuron.CalculateValue()
                Next
            Else
                ' 在这里将结果值赋值到一个临时的匿名变量中
                ' 来触发这个并行调用表达式
                '
                ' 2019-1-14 因为在计算的时候，取的neuron.value是上一层网络的值
                ' 只是修改当前网络的节点值
                ' 并没有修改上一层网络的任何参数
                ' 所以在这里的并行是没有问题的
                With Aggregate neuron As Neuron
                     In Neurons.AsParallel
                     Into Sum(neuron.CalculateValue)
                End With
            End If
        End Sub

        Public Sub CalculateGradient(targets As Double(), truncate As Double)
            For i As Integer = 0 To targets.Length - 1
                Neurons(i).CalculateGradient(targets(i), truncate)
            Next
        End Sub

        Public Sub CalculateGradient(Optional parallel As Boolean = False, Optional truncate# = -1)
            If Not parallel Then
                For Each neuron As Neuron In Neurons
                    Call neuron.CalculateGradient(truncate)
                Next
            Else
                With Aggregate neuron As Neuron
                     In Neurons.AsParallel
                     Into Sum(neuron.CalculateGradient(truncate))
                End With
            End If
        End Sub

        Public Overrides Function ToString() As String
            Dim n = Output
            Dim summary$

            If n.Length > 20 Then
                summary = n.Split(15).Select(Function(l) "   " & l.Select(Function(x) x.ToString("G3")).JoinBy(vbTab)).JoinBy(vbCrLf)
                summary = $"[{vbCrLf}{summary}{vbCrLf}]"
            Else
                summary = n.AsVector.ToString
            End If

            Return $"{Neurons.Length} neurons => {summary}"
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of Neuron) Implements IEnumerable(Of Neuron).GetEnumerator
            For Each neuron As Neuron In Neurons
                Yield neuron
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class

    ''' <summary>
    ''' 隐藏层,由多个神经元层所构成的
    ''' 
    ''' ##### 20181212
    ''' 
    ''' 请注意,这个隐藏层的大小虽然可以是任意规模的,但是并不是越大越好的
    ''' 当隐藏层越大的话,会导致训练的效率降低,并且预测能力也会下降
    ''' 如果问题比较简单的话,三层隐藏层已经足够了
    ''' 
    ''' 一般来说,隐藏层之中每一层的神经元的数量应该要大于输入和输出的节点数的最大值.
    ''' </summary>
    Public Class HiddenLayers : Implements IEnumerable(Of Layer)

        Public ReadOnly Property Layers As Layer()
        Public ReadOnly Property Size As Integer

        ''' <summary>
        ''' 使用最后一层作为输出层
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Output As Layer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Layers(Size - 1)
            End Get
        End Property

        Friend Sub New(layers As IEnumerable(Of Layer))
            Me.Layers = layers.ToArray
            Me.Size = Me.Layers.Length
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="input">s神经网络的输入层会作为隐藏层的输入</param>
        ''' <param name="size%"></param>
        ''' <param name="active"></param>
        Sub New(input As Layer, size%(), active As IActivationFunction, guid As VBInteger)
            Dim hiddenPortal As New Layer(size(Scan0), active, input, guid)

            Layers = New Layer(size.Length - 1) {}
            Layers(Scan0) = hiddenPortal

            ' 在隐藏层之中,前一层神经网络会作为后面的输出
            For i As Integer = 1 To size.Length - 1
                Layers(i) = New Layer(size(i), active, input:=hiddenPortal, guid:=guid)
                hiddenPortal = Layers(i)
            Next

            Me.Size = size.Length
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' 因为隐藏层的层数也不是很多,所以在这个函数也不需要并行?
        ''' </remarks>
        Public Sub ForwardPropagate(parallel As Boolean)
            For Each layer As Layer In Layers
                ' 2018-12-19
                ' 虽然隐藏层数量比较少,但是每一个隐藏层之中,神经元节点数量可能会很多
                ' 所以下面的这个函数的调用,内部实现应该是并行的?
                Call layer.CalculateValue(parallel)
            Next
        End Sub

        Public Sub BackPropagate(learnRate#, momentum#, truncate#, parallel As Boolean)
            Dim reverse = Layers.Reverse.ToArray

            ' 因为在调用函数计算之后,值变了
            ' 所以在这里会需要使用两个for each
            ' 不然计算会出bug
            For Each revLayer As Layer In reverse
                Call revLayer.CalculateGradient(parallel, truncate)
            Next
            For Each revLayer As Layer In reverse
                Call revLayer.UpdateWeights(learnRate, momentum, parallel)
            Next
        End Sub

        Public Overrides Function ToString() As String
            Return $"{Size} hidden layers => {Layers.Select(Function(l) l.Neurons.Length).ToArray.GetJson }"
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of Layer) Implements IEnumerable(Of Layer).GetEnumerator
            For Each layer As Layer In Layers
                Yield layer
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
