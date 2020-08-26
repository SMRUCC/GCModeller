Imports Microsoft.VisualBasic.MachineLearning.Darwinism.Models

Namespace NeuralNetwork.DarwinismHybrid

    Public Class NetworkIndividual : Implements Chromosome(Of WeightVector), ICloneable

        Public Property MutationRate As Double Implements Chromosome(Of WeightVector).MutationRate
            Get
                Throw New NotImplementedException()
            End Get
            Set(value As Double)
                Throw New NotImplementedException()
            End Set
        End Property

        Public ReadOnly Property UniqueHashKey As String Implements Chromosome(Of WeightVector).UniqueHashKey
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Function Clone() As Object Implements ICloneable.Clone
            Throw New NotImplementedException()
        End Function

        Public Function Crossover(another As WeightVector) As IEnumerable(Of WeightVector) Implements Chromosome(Of WeightVector).Crossover
            Throw New NotImplementedException()
        End Function

        Public Function Mutate() As WeightVector Implements Chromosome(Of WeightVector).Mutate
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace