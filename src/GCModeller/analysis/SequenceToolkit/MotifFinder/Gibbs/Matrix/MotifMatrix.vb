Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace Matrix

    Public Class MotifMatrix : Inherits WeightMatrix

        Private motifLength As Integer
        Private informationContentPerColumn As Double

        ''' <summary>
        ''' Creates a countsMatrix that may be sampled from where
        ''' columns have informationContentPerColumn information content
        ''' and there are motifLength columns </summary>
        ''' <param name="informationContentPerColumn">, information content per column </param>
        ''' <param name="motifLength">, number of columns </param>
        Public Sub New(informationContentPerColumn As Double, motifLength As Integer)
            Me.informationContentPerColumn = informationContentPerColumn
            Me.motifLength = motifLength
            MyBase.initMatrix(motifLength)
            initMotifMatrix()
        End Sub

        Public Sub New(countsLists As IList(Of IList(Of Integer)))
            MyBase.New(countsLists)
        End Sub

        ''' <summary>
        ''' Initializes the countsMatrix then randomly
        ''' chooses the probabilities based off of the
        ''' information content per column
        ''' </summary>
        Private Sub initMotifMatrix()
            initCountMatrix()
            Dim r As Random = New Random()
            Enumerable.Range(0, motifLength).AsParallel().ForEach(Sub(i, j) stochasticGradientDescent(r, i))
        End Sub

        ''' <summary>
        ''' Call this upon each row of the countsMatrix
        ''' 1. Picks a random base to emphasize
        ''' 2. Maintains an epoch and step counter to add less as epochs go on
        ''' 3. Randomly picks a base other than the target base and takes step
        '''      counts from it and gives it to the target base
        '''     4. Calculates the Information Content after each loop
        '''     Loop terminates after the Information Content has been
        '''     surpassed through taking a step of size 1. If a step of size
        '''     greater than 1 surpasses the threshold, then the
        '''     epoch will be undone. </summary>
        ''' <param name="idx">, row index of motif to perform work on </param>
        Private Sub stochasticGradientDescent(r As Random, idx As Integer)
            Dim prevStep As Integer = rowSum / 4 / 1000
            Dim [step] = If(prevStep > 0, prevStep, 5)
            Dim row = countsMatrix(idx)
            Dim incIdx = r.Next(4)
            Dim ic = Utils.calcInformationContent(rowSum, row)
            Dim decIdx = incIdx
            While [step] > 0
                While ic < informationContentPerColumn
                    decIdx = pickDecrementIndex(row, incIdx, [step])
                    If decIdx = -1 Then
                        prevStep = [step]
                        [step] = prevStep / 2
                        decIdx = incIdx
                    Else
                        row(incIdx) += [step]
                        row(decIdx) -= [step]
                        ic = Utils.calcInformationContent(rowSum, row)
                    End If
                End While
                prevStep = [step]
                [step] = prevStep / 2
                If ic > informationContentPerColumn Then
                    row(incIdx) -= [step]
                    row(decIdx) += [step]
                End If
                ic = Utils.calcInformationContent(rowSum, row)
            End While
            countsMatrix(idx) = row
        End Sub

        ''' <summary>
        ''' Picks the index in a row that may be decremented without going negative </summary>
        ''' <param name="row">, row of counts </param>
        ''' <param name="targetIdx">, target index that may not be decremented </param>
        ''' <param name="step">, amount of decrement </param>
        ''' <returns> index to decrement </returns>
        Private Function pickDecrementIndex(row As Integer(), targetIdx As Integer, [step] As Integer) As Integer
            Dim acceptableIndices As List(Of Integer) = New List(Of Integer)()
            For i = 0 To row.Length - 1
                If targetIdx = i Then
                    Continue For
                End If
                If row(i) - [step] >= 0 Then
                    acceptableIndices.Add(i)
                End If
            Next

            If acceptableIndices.Count = 0 Then
                Return -1
            Else
                Return acceptableIndices((New Random()).Next(acceptableIndices.Count))
            End If
        End Function

        ''' <summary>
        ''' Initializes the countsMatrix with the rowSum/4 value
        ''' </summary>
        Public Overridable Sub initCountMatrix()
            countsMatrix = RectangularArray.Matrix(Of Integer)(motifLength, 4)
            Dim initCount As Integer = rowSum / 4
            Enumerable.Range(0, motifLength).AsParallel().ForEach(Sub(i, l)
                                                                      Enumerable.Range(0, 4).ForEach(Sub(j, k)
                                                                                                         countsMatrix(i)(j) = initCount
                                                                                                     End Sub)
                                                                  End Sub)
        End Sub

        ''' <summary>
        ''' Use the weights to randomly select a base ml times to form a sampled motif </summary>
        ''' <returns> motif, String </returns>
        Public Overridable Function sample(r As Random) As String
            Dim stringBuilder As StringBuilder = New StringBuilder()
            Enumerable.Range(0, motifLength).ForEach(Sub(i, z)
                                                         Dim randomWeight = r.Next(rowSum)
                                                         Dim j = -1, k = 0
                                                         Do
                                                             k += countsMatrix(i)(Threading.Interlocked.Increment(j))
                                                         Loop While k < randomWeight
                                                         stringBuilder.Append(Utils.ACGT(j))
                                                     End Sub)
            Return stringBuilder.ToString()
        End Function
    End Class

End Namespace
