Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math.Correlations
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Public Class SelfOrganizingMap

    Private ReadOnly numberOfNeurons As Integer

    ''' <summary>
    ''' weight matrix
    ''' </summary>
    Private neuronWeights As Double()()

    Public Sub New(numberOfNeurons As Integer)
        Me.numberOfNeurons = numberOfNeurons
        neuronWeights = RectangularArray.Matrix(Of Double)(numberOfNeurons, 3)
    End Sub

    Public Overridable Sub train(pixels As Double()())
        Dim numberOfPixels = pixels.Length
        Dim numberOfFeatures = pixels(0).Length

        ' Initialize neuron weights randomly
        Dim w = RectangularArray.Matrix(Of Double)(numberOfNeurons, numberOfFeatures)
        For i = 0 To numberOfNeurons - 1
            For j = 0 To numberOfFeatures - 1
                w(i)(j) = randf.Next(256)
            Next
        Next

        ' SOM training parameters
        Dim learningRate = 0.9
        Dim numberOfIterations = 500

        ' SOM training algorithm
        For iteration As Integer = 0 To numberOfIterations - 1
            ' Randomly shuffle the pixels
            shufflePixels(pixels)

            ' Update neuron weights for each pixel
            For i = 0 To numberOfPixels - 1
                Dim pixel = pixels(i)
                Dim nearestNeuron = findNearestNeuron(pixel, w)

                Call updateNeuronWeights(pixel, w, nearestNeuron, learningRate)
            Next

            ' Decrease the learning rate
            learningRate *= 0.1
        Next

        ' Set the neuron weights as representative colors
        neuronWeights = w
    End Sub

    Private Sub shufflePixels(ByRef pixels As Double()())
        For i = pixels.Length - 1 To 1 Step -1
            Dim j = randf.Next(i + 1)
            Dim temp = pixels(i)
            pixels(i) = pixels(j)
            pixels(j) = temp
        Next
    End Sub

    Private Function findNearestNeuron(ByRef pixel As Double(), ByRef neuronWeightsMatrix As Double()()) As Integer
        Dim nearestNeuron = 0
        Dim minDistance = Double.MaxValue

        For i As Integer = 0 To numberOfNeurons - 1
            Dim distance = pixel.EuclideanDistance(neuronWeightsMatrix(i))

            If distance < minDistance Then
                minDistance = distance
                nearestNeuron = i
            End If
        Next

        ' returns the index
        Return nearestNeuron
    End Function

    Private Sub updateNeuronWeights(pixel As Double(), neuronWeightsMatrix As Double()(), nearestNeuron As Integer, learningRate As Double)
        Dim neuronWeights = neuronWeightsMatrix(nearestNeuron)
        For i As Integer = 0 To neuronWeights.Length - 1
            neuronWeights(i) += CInt(learningRate * (pixel(i) - neuronWeights(i)))
        Next
    End Sub

    ''' <summary>
    ''' matrix data clustering
    ''' </summary>
    ''' <param name="pixels"></param>
    ''' <returns></returns>
    Public Overridable Function assignPixels(pixels As Double()()) As Integer()
        Dim numberOfPixels = pixels.Length
        Dim assignedNeurons = New Integer(numberOfPixels - 1) {}

        For i = 0 To numberOfPixels - 1
            Dim pixel = pixels(i)
            assignedNeurons(i) = findNearestNeuron(pixel, neuronWeights)
        Next

        Return assignedNeurons
    End Function
End Class
