Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace NeuralNetwork

    Public Class IndividualParallelTraining

        Public ReadOnly Property Network As Network

        Sub New(net As Network)
            Network = net
        End Sub


    End Class
End Namespace