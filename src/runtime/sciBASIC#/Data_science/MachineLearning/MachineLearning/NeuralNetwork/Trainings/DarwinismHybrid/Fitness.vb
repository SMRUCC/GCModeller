Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.GAF
Imports Microsoft.VisualBasic.MachineLearning.StoreProcedure

Namespace NeuralNetwork.DarwinismHybrid

    Public Class Fitness : Implements Fitness(Of WeightVector)

        Dim network As Network
        Dim synapses As NamedCollection(Of Synapse)()
        Dim dataSets As TrainingSample()

        Sub New(network As Network, synapses As NamedCollection(Of Synapse)(), trainingSet As Sample())
            Me.dataSets = trainingSet.Select(Function(a) New TrainingSample(a)).ToArray
            Me.network = network
            Me.synapses = synapses
        End Sub

        Public ReadOnly Property Cacheable As Boolean Implements Fitness(Of WeightVector).Cacheable
            Get
                Return False
            End Get
        End Property

        Public Function Calculate(chromosome As WeightVector, parallel As Boolean) As Double Implements Fitness(Of WeightVector).Calculate
            For i As Integer = 0 To chromosome.weights.Length - 1
                For Each s In synapses(i)
                    s.Weight = chromosome.weights(i)
                Next
            Next

            Dim errors As New List(Of Double)

            For Each dataSet As TrainingSample In dataSets
                Call network.ForwardPropagate(dataSet.sample, False)
                ' 2019-1-14 因为在这里是计算误差，不是训练过程
                ' 所以在这里不需要进行反向传播修改权重和bias参数
                ' 否则会造成其他的解决方案的错误计算，因为反向传播将weights等参数更新了
                ' Call network.BackPropagate(dataSet.target, False)
                Call errors.Add(TrainingUtils.CalculateError(network, dataSet.classify).Average)
            Next

            Return errors.Average
        End Function
    End Class
End Namespace