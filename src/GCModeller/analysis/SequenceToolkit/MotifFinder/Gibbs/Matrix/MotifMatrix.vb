#Region "Microsoft.VisualBasic::b8451bffc47c2e101abfcccb5a6a4268, analysis\SequenceToolkit\MotifFinder\Gibbs\Matrix\MotifMatrix.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 129
    '    Code Lines: 82 (63.57%)
    ' Comment Lines: 32 (24.81%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 15 (11.63%)
    '     File Size: 5.43 KB


    '     Class MotifMatrix
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: pickDecrementIndex
    ' 
    '         Sub: initCountMatrix, initMotifMatrix, stochasticGradientDescent
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

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

            Call MyBase.initMatrix(motifLength)
            Call initMotifMatrix()
        End Sub

        Public Sub New(countsLists As List(Of List(Of Integer)))
            MyBase.New(countsLists)
        End Sub

        ''' <summary>
        ''' Initializes the countsMatrix then randomly
        ''' chooses the probabilities based off of the
        ''' information content per column
        ''' </summary>
        Private Sub initMotifMatrix()
            Call initCountMatrix()
            Call Enumerable.Range(0, motifLength) _
                .AsParallel() _
                .ForEach(Sub(i, j)
                             stochasticGradientDescent(i)
                         End Sub)
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
        Private Sub stochasticGradientDescent(idx As Integer)
            Dim prevStep As Integer = rowSum / 4 / 1000
            Dim [step] = If(prevStep > 0, prevStep, 5)
            Dim row = countsMatrix(idx)
            Dim incIdx = randf.Next(4)
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
            Dim acceptableIndices As New List(Of Integer)()

            For i As Integer = 0 To row.Length - 1
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
                Return acceptableIndices(randf.Next(acceptableIndices.Count))
            End If
        End Function

        ''' <summary>
        ''' Initializes the countsMatrix with the rowSum/4 value
        ''' </summary>
        Public Overridable Sub initCountMatrix()
            Dim initCount As Integer = rowSum / 4

            countsMatrix = RectangularArray.Matrix(Of Integer)(motifLength, 4)

            Call Enumerable.Range(0, motifLength) _
                .AsParallel() _
                .ForEach(Sub(i, l)
                             For j As Integer = 0 To 3
                                 countsMatrix(i)(j) = initCount
                             Next
                         End Sub)
        End Sub
    End Class

End Namespace
