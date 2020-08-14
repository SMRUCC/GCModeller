Imports Microsoft.VisualBasic.DataMining.ComponentModel.Normalizer
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.NeuralNetwork.Activations
Imports Microsoft.VisualBasic.MachineLearning.NeuralNetwork.StoreProcedure
Imports Microsoft.VisualBasic.MachineLearning.StoreProcedure

Namespace NeuralNetwork

    Public Class ParallelNetwork

        Dim parallels As Func(Of Double(), Double())()

        Public Iterator Function Predicts(input As Double()) As IEnumerable(Of Double)
            For i As Integer = 0 To parallels.Length - 1
                Yield parallels(i)(input)(Scan0)
            Next
        End Function

        Public Shared Function LoadSnapshot(dir As String, normalize As NormalizeMatrix, Optional method As Methods = Methods.NormalScaler) As ParallelNetwork
            Dim parallels As New List(Of Func(Of Double(), Double()))
            Dim annLambda As Func(Of Double(), Double())

            For Each individual As String In dir _
                .ListDirectory(SearchOption.SearchTopLevelOnly) _
                .OrderBy(Function(name)
                             Return Convert.ToInt32(name.BaseName, 16)
                         End Function)

                annLambda = ScatteredLoader(store:=individual, mute:=True).GetPredictLambda2(normalize, method, mute:=True)
                parallels += annLambda

                Call $"load component: {Convert.ToInt32(individual.BaseName, 16)}".__DEBUG_ECHO
            Next

            Return New ParallelNetwork With {
                .parallels = parallels.ToArray
            }
        End Function

    End Class

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
                ) With {
                    .Truncate = network.Truncate,
                    .LearnRateDecay = network.LearnRateDecay
                }.DoCall(AddressOf clones.Add)
            Next

            individualNetworks = clones.SeqIterator.ToArray
        End Sub

        Protected Overrides Sub SaveSnapshot()
            Call Snapshot(snapshotSaveLocation)
        End Sub

        Public Sub Snapshot(snapshotSaveLocation As String)
            Dim outputSize As Integer = network.OutputLayer.Count

            For i As Integer = 0 To outputSize - 1
                Call New Snapshot(individualNetworks(i)).WriteScatteredParts($"{snapshotSaveLocation}/{(i + 666).ToHexString}/")
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
                .Select(Function(xi) xi.Err) _
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