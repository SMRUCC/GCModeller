Imports Microsoft.VisualBasic.Data.visualize
Imports Microsoft.VisualBasic.MachineLearning.NeuralNetwork
Imports Microsoft.VisualBasic.Serialization.JSON

Module ANNVisualize

    Sub Main()
        Dim ANN As New TrainingUtils(5, {10, 13, 50}, 3)

        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({0, 1, 1, 1, 1}, {0, 0, 0})
        Call ANN.Add({0, 1, 1, 1, 1}, {0, 0, 0})
        Call ANN.Add({0, 1, 1, 1, 0.2}, {0, 0, 0})
        Call ANN.Add({0, 1, 1, 1, 1}, {0, 0, 0})
        Call ANN.Add({0, 1, 1, 1, 1}, {0, 0, 0})
        Call ANN.Add({0, 1, 0.5, 1, 1}, {0, 0, 0})
        Call ANN.Add({0, 1, 0.4, 1, 1}, {0, 0, 0})
        Call ANN.Add({1, 1, 1, 0, 1}, {1, 0, 1})
        Call ANN.Add({1, 1, 1, 0, 1}, {1, 0, 1})
        Call ANN.Add({1, 1, 1, 0, 1}, {1, 0, 1})
        Call ANN.Add({1, 0, 1, 0, 1}, {1, 0.3, 1})
        Call ANN.Add({1, 1, 1, 0, 1}, {1, 0, 1})
        Call ANN.Add({1, 0, 1, 0, 1}, {1, 0.3, 1})
        Call ANN.Add({1, 1, 1, 0, 1}, {1, 0, 1})
        Call ANN.Add({1, 1, 1, 0, 0}, {1, 0, 1})
        Call ANN.Add({1, 1, 0, 0, 1}, {1, 0, 1})

        Call ANN.Train()

        Call ANN.NeuronNetwork.Compute(1, 1, 1, 1, 1).GetJson.__DEBUG_ECHO
        Call ANN.NeuronNetwork.Compute(0, 1, 1, 1, 0).GetJson.__DEBUG_ECHO
        Call ANN.NeuronNetwork.Compute(1, 0, 1, 0, 1).GetJson.__DEBUG_ECHO


        Call ANN.TakeSnapshot.GetXml.SaveTo("./ANN_snapshot.Xml")
        Call ANN.NeuronNetwork.VisualizeModel.Save("./ANN_network/")

        Pause()
    End Sub
End Module
