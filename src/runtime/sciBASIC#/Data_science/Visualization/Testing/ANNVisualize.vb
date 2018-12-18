Imports Microsoft.VisualBasic.MachineLearning.NeuralNetwork

Module ANNVisualize

    Sub Main()
        Dim ANN As New TrainingUtils(5, {10, 13, 5}, 3)

        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})
        Call ANN.Add({1, 1, 1, 1, 1}, {1, 1, 1})

        Call ANN.Train()


    End Sub
End Module
