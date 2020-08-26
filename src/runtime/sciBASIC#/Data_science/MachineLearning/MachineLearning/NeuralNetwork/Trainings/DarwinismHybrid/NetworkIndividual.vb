Imports Microsoft.VisualBasic.MachineLearning.Darwinism.Models
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Namespace NeuralNetwork.DarwinismHybrid

    Public Class NetworkIndividual : Implements Chromosome(Of NetworkIndividual), ICloneable

        Dim target As Network
        Dim guid As String

        Public Property MutationRate As Double Implements Chromosome(Of NetworkIndividual).MutationRate

        Public ReadOnly Property UniqueHashKey As String Implements Chromosome(Of NetworkIndividual).UniqueHashKey
            Get
                If guid Is Nothing Then
                    guid = (target.GetHashCode.ToString & Now.ToString).MD5
                End If

                Return guid
            End Get
        End Property

        Public Function Clone() As Object Implements ICloneable.Clone
            Dim copy As Network

            Return New NetworkIndividual With {
                .MutationRate = MutationRate,
                .target = copy
            }
        End Function

        Public Function Crossover(another As NetworkIndividual) As IEnumerable(Of NetworkIndividual) Implements Chromosome(Of NetworkIndividual).Crossover
            Throw New NotImplementedException()
        End Function

        Private Overloads Shared Function Mutate(target As Layer) As Neuron
            Dim neuron As Neuron = target.Neurons(randf.NextInteger(target.Count))
            Dim link As Synapse

            If Not neuron.InputSynapses.IsNullOrEmpty Then
                link = neuron.InputSynapses(randf.NextInteger(neuron.InputSynapses.Count))
                link.Weight += randf.randf(-1, 1)
                link.WeightDelta += randf.randf(-1, 1)
            End If
            If Not neuron.OutputSynapses.IsNullOrEmpty Then
                link = neuron.OutputSynapses(randf.NextInteger(neuron.OutputSynapses.Count))
                link.Weight += randf.randf(-1, 1)
                link.WeightDelta += randf.randf(-1, 1)
            End If

            Return neuron
        End Function

        Public Overloads Function Mutate() As NetworkIndividual Implements Chromosome(Of NetworkIndividual).Mutate
            Dim copy As NetworkIndividual = DirectCast(Clone(), NetworkIndividual)
            Dim neuron As Network = copy.target

            Call Mutate(neuron.InputLayer)
            Call Mutate(neuron.OutputLayer)

            For Each layer In neuron.HiddenLayer
                Call Mutate(layer)
            Next

            Return copy
        End Function
    End Class
End Namespace