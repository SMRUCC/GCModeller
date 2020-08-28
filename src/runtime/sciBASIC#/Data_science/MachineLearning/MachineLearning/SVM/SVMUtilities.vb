Namespace SVM
    Public Module SVMUtilities
        Private Const SCALE As Double = 100
        Public Const TRAINING_SEED As Integer = 20080524
        Public Const TESTING_SEED As Integer = 20140407

        Public Function CreateTwoClassProblem(ByVal count As Integer, ByVal Optional isTraining As Boolean = True) As Problem
            Dim prob As Problem = New Problem()
            prob.Count = count
            prob.MaxIndex = 2
            Dim rand As Random = New Random(If(isTraining, TRAINING_SEED, TESTING_SEED))
            ' create points on either side of the vertical axis
            Dim positive As Integer = CInt(count / 2)
            Dim labels As List(Of Double) = New List(Of Double)()
            Dim data As List(Of Node()) = New List(Of Node())()

            For i = 0 To count - 1
                Dim x As Double = rand.NextDouble() * SCALE + 10
                Dim y As Double = rand.NextDouble() * SCALE - SCALE * 0.5
                x = If(i < positive, x, -x)
                data.Add(New Node() {New Node(1, x), New Node(2, y)})
                labels.Add(If(i < positive, 1, -1))
            Next

            prob.X = data.ToArray()
            prob.Y = labels.ToArray()
            Return prob
        End Function

        Public Function CreateMulticlassProblem(ByVal numberOfClasses As Integer, ByVal count As Integer, ByVal Optional isTraining As Boolean = True) As Problem
            If numberOfClasses > 8 Then Throw New ArgumentException("Number of classes must be < 8")
            Dim prob As Problem = New Problem()
            prob.Count = count
            prob.MaxIndex = 3
            Dim samplesPerClass = New Integer(numberOfClasses - 1) {}
            Dim countPerClass = count / numberOfClasses
            Dim current = countPerClass

            For i = 1 To samplesPerClass.Length - 1
                samplesPerClass(i) = CInt(current)
                current += countPerClass
                samplesPerClass(i - 1) = samplesPerClass(i) - samplesPerClass(i - 1)
            Next

            samplesPerClass(samplesPerClass.Length - 1) = count - samplesPerClass.Last()
            Dim xSigns = New Integer(7) {-1, 1, 1, -1, -1, 1, 1, -1}
            Dim ySigns = New Integer(7) {1, 1, -1, -1, 1, 1, -1, -1}
            Dim zSigns = New Integer(7) {1, 1, 1, 1, -1, -1, -1, -1}
            Dim rand As Random = New Random(If(isTraining, TRAINING_SEED, TESTING_SEED))
            Dim labels As List(Of Double) = New List(Of Double)()
            Dim data As List(Of Node()) = New List(Of Node())()

            For i = 0 To numberOfClasses - 1

                For j = 0 To samplesPerClass(i) - 1
                    Dim x As Double = rand.NextDouble() * SCALE + 10
                    Dim y As Double = rand.NextDouble() * SCALE + 10
                    Dim z As Double = rand.NextDouble() * SCALE + 10
                    x *= xSigns(i)
                    y *= ySigns(i)
                    z *= zSigns(i)
                    data.Add(New Node() {New Node(1, x), New Node(2, y), New Node(3, z)})
                    labels.Add(i)
                Next
            Next

            prob.X = data.ToArray()
            prob.Y = labels.ToArray()
            Return prob
        End Function

        Public Function CreateRegressionProblem(ByVal count As Integer, ByVal Optional isTraining As Boolean = True) As Problem
            Dim prob As Problem = New Problem()
            prob.Count = count
            prob.MaxIndex = 2
            Dim rand As Random = New Random(If(isTraining, TRAINING_SEED, TESTING_SEED))
            Dim labels As List(Of Double) = New List(Of Double)()
            Dim data As List(Of Node()) = New List(Of Node())()

            For i = 0 To count - 1
                Dim y As Double = rand.NextDouble() * 10 - 5
                Dim z As Double = rand.NextDouble() * 10 - 5
                Dim x = 2 * y + z
                data.Add(New Node() {New Node(1, y), New Node(2, z)})
                labels.Add(x)
            Next

            prob.X = data.ToArray()
            prob.Y = labels.ToArray()
            Return prob
        End Function
    End Module
End Namespace
