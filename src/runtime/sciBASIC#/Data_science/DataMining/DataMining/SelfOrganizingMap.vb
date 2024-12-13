Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math.Correlations
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

''' <summary>
''' SOM: Self-Organizing Map
''' </summary>
Public Class SelfOrganizingMap

    ReadOnly numberOfNeurons As Integer

    ''' <summary>
    ''' data channel depth
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property depth As Integer

    ''' <summary>
    ''' weight matrix
    ''' </summary>
    Dim neuronWeights As Double()()

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="numberOfNeurons">number of the neurons</param>
    Public Sub New(numberOfNeurons As Integer, depth As Integer)
        Me.numberOfNeurons = numberOfNeurons
        Me.depth = depth

        neuronWeights = RectangularArray.Matrix(Of Double)(numberOfNeurons, depth)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pixels">dataset for run the training, data should be an rectangle array, with 2nd dimension size should be equals to <see cref="depth"/>.</param>
    Public Overridable Sub train(pixels As Double()(), Optional learningRate As Double = 0.9, Optional numberOfIterations As Integer = 500)
        Dim numberOfPixels = pixels.Length
        Dim numberOfFeatures = pixels(0).Length
        Dim globalMax As Double = Aggregate f In pixels Into Max(f.Max)

        ' Initialize neuron weights randomly
        Dim w = RectangularArray.Matrix(Of Double)(numberOfNeurons, numberOfFeatures)
        For i As Integer = 0 To numberOfNeurons - 1
            For j As Integer = 0 To numberOfFeatures - 1
                w(i)(j) = randf.Next(globalMax)
            Next
        Next

        ' SOM training algorithm
        For iteration As Integer = 0 To numberOfIterations - 1
            ' Randomly shuffle the pixels
            Call shufflePixels(pixels)

            ' Update neuron weights for each pixel
            For i As Integer = 0 To numberOfPixels - 1
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
        For i As Integer = pixels.Length - 1 To 1 Step -1
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
    Public Overridable Function ClusterId(pixels As Double()()) As Integer()
        Dim numberOfPixels = pixels.Length
        Dim assignedNeurons = New Integer(numberOfPixels - 1) {}

        For i As Integer = 0 To numberOfPixels - 1
            assignedNeurons(i) = findNearestNeuron(pixels(i), neuronWeights)
        Next

        Return assignedNeurons
    End Function
End Class
