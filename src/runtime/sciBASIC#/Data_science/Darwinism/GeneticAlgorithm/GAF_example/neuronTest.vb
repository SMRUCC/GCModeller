Imports Microsoft.VisualBasic.MachineLearning.NeuralNetwork

Module neuronTest

    Sub Main()

        Dim test As New TrainingUtils(6, 9, 3)

        Call test.Add({0, 0, 0, 1, 1, 1}, {1, 0, 0})
        Call test.Add({0, 0, 0, 1, 1, 1}, {1, 0, 0})
        Call test.Add({0, 0, 0, 1, 1, 1}, {1, 0, 0})
        Call test.Add({0, 0, 0, 1, 1, 1}, {1, 0, 0})
        Call test.Add({0, 0, 0, 1, 1, 1}, {1, 0, 0})
        Call test.Add({0, 0, 0, 1, 1, 1}, {1, 0, 0})
        Call test.Add({0, 0, 0, 1, 1, 1}, {1, 0, 0})
        Call test.Add({0, 0, 0, 1, 1, 1}, {1, 0, 0})
        Call test.Add({0, 0, 0, 1, 1, 1}, {1, 0, 0})
        Call test.Add({0, 0, 0, 1, 1, 1}, {1, 0, 0})
        Call test.Add({0, 0, 0, 1, 1, 1}, {1, 0, 0})
        Call test.Add({0, 0, 0, 1, 1, 1}, {1, 0, 0})
        Call test.Add({0, 0, 0, 1, 1, 1}, {1, 0, 0})
        Call test.Add({0, 0, 0, 1, 1, 1}, {1, 0, 0})
        Call test.Add({0, 0, 0, 1, 1, 1}, {1, 0, 0})

        Call test.Train()

        Dim classes = test.NeuronNetwork.Compute(0, 0, 0, 1, 1, 1)

        Pause()
    End Sub
End Module
