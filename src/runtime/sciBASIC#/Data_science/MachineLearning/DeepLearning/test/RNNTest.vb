Imports Microsoft.VisualBasic.MachineLearning.RNN

Module RNNTest

    Sub Main()
        Dim opts As New Options
        Dim net = CharRNN.initialize(opts)

        Pause()
    End Sub
End Module
