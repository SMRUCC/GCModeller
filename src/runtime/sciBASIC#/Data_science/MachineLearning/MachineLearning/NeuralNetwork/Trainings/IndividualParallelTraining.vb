Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.NeuralNetwork.Activations
Imports Microsoft.VisualBasic.MachineLearning.NeuralNetwork.StoreProcedure

Namespace NeuralNetwork

    Public Class IndividualParallelTraining : Inherits ANNTrainer

        Dim individualNetworks As SeqValue(Of Network)()

        Sub New(net As Network)
            Call MyBase.New(net)
            Call cloneNetworks()
        End Sub

        Sub New(inputSize As Integer,
                hiddenSize() As Integer,
                outputSize As Integer,
                Optional learnRate As Double = 0.1,
                Optional momentum As Double = 0.9,
                Optional active As LayerActives = Nothing,
                Optional weightInit As Func(Of Double) = Nothing)

            Call MyBase.New(inputSize, hiddenSize, outputSize, learnRate, momentum, active, weightInit)
            Call cloneNetworks()
        End Sub

        Private Sub cloneNetworks()
            Dim clones As New List(Of Network)
            Dim inputSize = network.InputLayer.Count
            Dim outputSize = 1 ' network.OutputLayer.Count
            Dim hiddenSize As New List(Of Integer)
            Dim active As LayerActives = LayerActives.FromXmlModel(network.Activations)

            For Each layer In network.HiddenLayer
                hiddenSize.Add(layer.Count)
            Next

            For i As Integer = 0 To network.OutputLayer.Count - 1
                Call New Network(
                     inputSize:=inputSize,
                     hiddenSize:=hiddenSize,
                     outputSize:=outputSize,
                     learnRate:=network.LearnRate,
                     momentum:=network.Momentum,
                     active:=active,
                     weightInit:=Helpers.randomWeight
                ).DoCall(AddressOf clones.Add)
            Next

            individualNetworks = clones.SeqIterator.ToArray
        End Sub

        Protected Overrides Sub SaveSnapshot()
            Dim outputSize As Integer = network.OutputLayer.Count

            For i As Integer = 0 To outputSize - 1
                Call New Snapshot(individualNetworks(i)).WriteScatteredParts($"{snapshotSaveLocation}/attribute_{i}/")
            Next
        End Sub

        Protected Overrides Function runTraining(parallel As Boolean) As Double()
            ' 20190701 数据不打乱，网络极大可能拟合前面几个batch的样本分布
            ' 
            ' 训练所使用的样本数据的顺序可能会对结果产生影响
            ' 所以在训练之前会需要打乱样本的顺序来避免出现问题
            Dim dataSets As TrainingSample()() = populateDataSets().ToArray
            Dim errors As Double() = individualNetworks _
                .AsParallel _
                .Select(Function(network)
                            Dim temp = trainingImpl(network, dataSets(network.i), parallel, Selective, dropOutRate)
                            Return (network.i, Err:=temp(Scan0))
                        End Function) _
                .OrderBy(Function(rtvl) rtvl.i) _
                .Select(Function(xi) xi.err) _
                .ToArray

            Return errors
        End Function

        Private Iterator Function populateDataSets() As IEnumerable(Of TrainingSample())
            Dim raw = Me.dataSets.Shuffles
            Dim outputSize As Integer = raw(Scan0).classify.Length
            Dim j As Integer = 0

            For i As Integer = 0 To outputSize - 1
                j = i

                Yield raw _
                    .Select(Function(a)
                                Return New TrainingSample With {
                                    .classify = {a.classify(j)},
                                    .sample = a.sample,
                                    .sampleID = a.sampleID
                                }
                            End Function) _
                    .ToArray
            Next
        End Function
    End Class
End Namespace