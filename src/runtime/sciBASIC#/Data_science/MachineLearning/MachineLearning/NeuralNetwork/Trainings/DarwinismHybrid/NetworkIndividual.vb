Imports Microsoft.VisualBasic.Linq
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

        Private Function copyLayer(layer As Layer) As Layer
            Dim neurons As Neuron() = layer.Neurons _
                .Select(Function(a)
                            Return New Neuron(Function() 0, a.activation, a.Guid) With {
                                .Bias = a.Bias,
                                .BiasDelta = a.BiasDelta,
                                .Gradient = a.Gradient,
                                .isDroppedOut = a.isDroppedOut,
                                .Value = a.Value
                            }
                        End Function) _
                .ToArray
            Dim copy As New Layer(neurons) With {
                .doDropOutMode = layer.doDropOutMode,
                .softmaxNormalization = layer.softmaxNormalization
            }

            Return copy
        End Function

        Private Shared Function GetNodeTable(network As Network) As Dictionary(Of String, Neuron)
            Dim neurons As New Dictionary(Of String, Neuron)

            For Each node In network.InputLayer
                neurons.Add(node.Guid, node)
            Next

            For Each node In network.OutputLayer
                neurons.Add(node.Guid, node)
            Next

            For Each hidden In network.HiddenLayer
                For Each node In hidden
                    neurons.Add(node.Guid, node)
                Next
            Next

            Return neurons
        End Function

        Public Function Clone() As Object Implements ICloneable.Clone
            Dim copy As New Network(target.Activations) With {
                .LearnRate = target.LearnRate,
                .Momentum = target.Momentum,
                .Truncate = target.Truncate,
                .LearnRateDecay = target.LearnRateDecay,
                .InputLayer = copyLayer(target.InputLayer),
                .OutputLayer = copyLayer(target.OutputLayer),
                .HiddenLayer = New HiddenLayers(target.HiddenLayer.Select(AddressOf copyLayer))
            }

            ' create neuron links
            Dim copyTable = GetNodeTable(copy)
            Dim rawTable = GetNodeTable(target)
            Dim linkUnit As Synapse

            For Each node As Neuron In copyTable.Values
                Dim raw As Neuron = rawTable(node.Guid)

                For Each link As Synapse In raw.InputSynapses.JoinIterates(raw.OutputSynapses)
                    linkUnit = New Synapse With {
                        .Weight = link.Weight,
                        .WeightDelta = link.WeightDelta,
                        .InputNeuron = copyTable(link.InputNeuron.Guid),
                        .OutputNeuron = copyTable(link.OutputNeuron.Guid)
                    }
                    node.InputSynapses.Add(linkUnit)
                Next
            Next

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