#Region "Microsoft.VisualBasic::873eb68134f1e1926a7e966a045b8986, Data_science\DataMining\Microsoft.VisualBasic.DataMining.Framework\NeuralNetwork\Helpers.vb"

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

'     Module Helpers
' 
'         Function: GetRandom
' 
'         Sub: Train
' 
'     Enum TrainingType
' 
'         Epoch, MinimumError
' 
'  
' 
' 
' 
'     Class Encoder
' 
'         Function: Decode, Encode
' 
'         Sub: AddMap
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.MachineLearning.NeuralNetwork.Activations
Imports Microsoft.VisualBasic.MachineLearning.NeuralNetwork.StoreProcedure
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Namespace NeuralNetwork

    Public Module Helpers

        Public Const MaxEpochs As Integer = 10000
        Public Const MinimumError As Double = 0.01

        ''' <summary>
        ''' <see cref="Sigmoid"/> as default
        ''' </summary>
        Friend ReadOnly defaultActivation As DefaultValue(Of IActivationFunction) = New Sigmoid

        ReadOnly rand As New Random()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Friend Function GetRandom() As Double
            SyncLock rand
                Return 2 * rand.NextDouble() - 1
            End SyncLock
        End Function

        <Extension>
        Public Sub Train(ByRef neuron As Network, data As Sample(),
                         Optional trainingType As TrainingType = TrainingType.Epoch,
                         Optional minErr As Double = MinimumError,
                         Optional parallel As Boolean = False)

            If trainingType = TrainingType.Epoch Then
                Call neuron.Train(data, Helpers.MaxEpochs, parallel)
            Else
                Call neuron.Train(data, minimumError:=minErr, parallel:=parallel)
            End If
        End Sub

        <Extension>
        Friend Function PopulateAllSynapses(neuron As Neuron) As IEnumerable(Of Synapse)
            Return neuron.InputSynapses.ToArray + neuron.OutputSynapses.AsList
        End Function

        ''' <summary>
        ''' 将所有的属性结果都归一化为相同等级的``[0,1]``区间内的数
        ''' </summary>
        ''' <param name="samples"></param>
        ''' <returns></returns>
        <Extension>
        Public Function NormalizeSamples(samples As Sample()) As Sample()
            ' 每一行数据不可以直接比较
            ' 但是每一列数据是可以直接做比较的
            Dim v As Vector
            Dim m As Integer = samples(Scan0).status.Length
            Dim n As Integer = samples(Scan0).target.Length
            Dim index%

            For i As Integer = 0 To m - 1
                index = i
                v = samples.Select(Function(x) x.status(index)).AsVector
                v = v / v.Max

                For j As Integer = 0 To samples.Length - 1
                    samples(j).status(index) = v.Item(j)
                Next
            Next

            For i As Integer = 0 To n - 1
                index = i
                v = samples.Select(Function(x) x.target(index)).AsVector
                v = v / v.Max

                For j As Integer = 0 To samples.Length - 1
                    samples(j).target(index) = v.Item(j)
                Next
            Next

            Return samples
        End Function
    End Module

    Public Enum TrainingType
        ''' <summary>
        ''' 以给定的迭代次数的方式进行训练. <see cref="Helpers.MaxEpochs"/>
        ''' </summary>
        Epoch
        ''' <summary>
        ''' 以小于目标误差的方式进行训练. <see cref="Helpers.MinimumError"/>
        ''' </summary>
        MinimumError
    End Enum

    Public Class Encoder(Of T)

        Dim maps As New Dictionary(Of T, Double)

        Default Public Property item(x As T) As Double
            Get
                If maps.ContainsKey(x) Then
                    Return maps(x)
                Else
                    Return Nothing
                End If
            End Get
            Set(value As Double)
                maps(x) = value
            End Set
        End Property

        Public Sub AddMap(x As T, value As Double)
            Call maps.Add(x, value)
        End Sub

        Public Function Encode(x As T) As Double
            Return maps(x)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="out">神经网络的输出值</param>
        ''' <returns></returns>
        Public Function Decode(out As Double) As T
            Dim minX As T, minD As Double = 9999

            For Each x In maps
                Dim d As Double = Math.Abs(x.Value - out)

                If d < minD Then
                    minD = d
                    minX = x.Key
                End If
            Next

            Return minX
        End Function
    End Class
End Namespace
