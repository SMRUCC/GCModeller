' 
 ' * SVM.NET Library
 ' * Copyright (C) 2008 Matthew Johnson
 ' * 
 ' * This program is free software: you can redistribute it and/or modify
 ' * it under the terms of the GNU General Public License as published by
 ' * the Free Software Foundation, either version 3 of the License, or
 ' * (at your option) any later version.
 ' * 
 ' * This program is distributed in the hope that it will be useful,
 ' * but WITHOUT ANY WARRANTY; without even the implied warranty of
 ' * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 ' * GNU General Public License for more details.
 ' * 
 ' * You should have received a copy of the GNU General Public License
 ' * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 


Imports System
Imports System.IO

Namespace SVM
    ''' <summary>
    ''' Class which encapsulates a range transformation.
    ''' </summary>
    Public Class RangeTransform
        Implements IRangeTransform
        ''' <summary>
        ''' Default lower bound for scaling (-1).
        ''' </summary>
        Public Const DEFAULT_LOWER_BOUND As Integer = -1
        ''' <summary>
        ''' Default upper bound for scaling (1).
        ''' </summary>
        Public Const DEFAULT_UPPER_BOUND As Integer = 1

        ''' <summary>
        ''' Determines the Range transform for the provided problem.  Uses the default lower and upper bounds.
        ''' </summary>
        ''' <param name="prob">The Problem to analyze</param>
        ''' <returns>The Range transform for the problem</returns>
        Public Shared Function Compute(ByVal prob As Problem) As RangeTransform
            Return Compute(prob, DEFAULT_LOWER_BOUND, DEFAULT_UPPER_BOUND)
        End Function
        ''' <summary>
        ''' Determines the Range transform for the provided problem.
        ''' </summary>
        ''' <param name="prob">The Problem to analyze</param>
        ''' <param name="lowerBound">The lower bound for scaling</param>
        ''' <param name="upperBound">The upper bound for scaling</param>
        ''' <returns>The Range transform for the problem</returns>
        Public Shared Function Compute(ByVal prob As Problem, ByVal lowerBound As Double, ByVal upperBound As Double) As RangeTransform
            Dim minVals = New Double(prob.MaxIndex - 1) {}
            Dim maxVals = New Double(prob.MaxIndex - 1) {}

            For i = 0 To prob.MaxIndex - 1
                minVals(i) = Double.MaxValue
                maxVals(i) = Double.MinValue
            Next

            For i = 0 To prob.Count - 1

                For j = 0 To prob.X(i).Length - 1
                    Dim index = prob.X(i)(j).Index - 1
                    Dim value = prob.X(i)(j).Value
                    minVals(index) = Math.Min(minVals(index), value)
                    maxVals(index) = Math.Max(maxVals(index), value)
                Next
            Next

            For i = 0 To prob.MaxIndex - 1

                If minVals(i) = Double.MaxValue OrElse maxVals(i) = Double.MinValue Then
                    minVals(i) = 0
                    maxVals(i) = 0
                End If
            Next

            Return New RangeTransform(minVals, maxVals, lowerBound, upperBound)
        End Function

        Private _inputStart As Double()
        Private _inputScale As Double()
        Private _outputStart As Double
        Private _outputScale As Double
        Private _length As Integer

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="minValues">The minimum values in each dimension.</param>
        ''' <param name="maxValues">The maximum values in each dimension.</param>
        ''' <param name="lowerBound">The desired lower bound for all dimensions.</param>
        ''' <param name="upperBound">The desired upper bound for all dimensions.</param>
        Public Sub New(ByVal minValues As Double(), ByVal maxValues As Double(), ByVal lowerBound As Double, ByVal upperBound As Double)
            _length = minValues.Length
            If maxValues.Length <> _length Then Throw New Exception("Number of max and min values must be equal.")
            _inputStart = New Double(_length - 1) {}
            _inputScale = New Double(_length - 1) {}

            For i = 0 To _length - 1
                _inputStart(i) = minValues(i)
                _inputScale(i) = maxValues(i) - minValues(i)
            Next

            _outputStart = lowerBound
            _outputScale = upperBound - lowerBound
        End Sub

        Private Sub New(ByVal inputStart As Double(), ByVal inputScale As Double(), ByVal outputStart As Double, ByVal outputScale As Double, ByVal length As Integer)
            _inputStart = inputStart
            _inputScale = inputScale
            _outputStart = outputStart
            _outputScale = outputScale
            _length = length
        End Sub
        ''' <summary>
        ''' Transforms the input array based upon the values provided.
        ''' </summary>
        ''' <param name="input">The input array</param>
        ''' <returns>A scaled array</returns>
        Public Function Transform(ByVal input As Node()) As Node() Implements IRangeTransform.Transform
            Dim output = New Node(input.Length - 1) {}

            For i = 0 To output.Length - 1
                Dim index = input(i).Index
                Dim value = input(i).Value
                output(i) = New Node(index, Transform(value, index))
            Next

            Return output
        End Function

        ''' <summary>
        ''' Transforms this an input value using the scaling transform for the provided dimension.
        ''' </summary>
        ''' <param name="input">The input value to transform</param>
        ''' <param name="index">The dimension whose scaling transform should be used</param>
        ''' <returns>The scaled value</returns>
        Public Function Transform(ByVal input As Double, ByVal index As Integer) As Double Implements IRangeTransform.Transform
            index -= 1
            Dim tmp = input - _inputStart(index)
            If _inputScale(index) = 0 Then Return 0
            tmp /= _inputScale(index)
            tmp *= _outputScale
            Return tmp + _outputStart
        End Function
        ''' <summary>
        ''' Writes this Range transform to a stream.
        ''' </summary>
        ''' <param name="stream">The stream to write to</param>
        ''' <param name="r">The range to write</param>
        Public Shared Sub Write(ByVal stream As Stream, ByVal r As RangeTransform)
            Start()
            Dim output As StreamWriter = New StreamWriter(stream)
            output.WriteLine(r._length)
            output.Write(r._inputStart(0))

            For i = 1 To r._inputStart.Length - 1
                output.Write(" " & r._inputStart(i))
            Next

            output.WriteLine()
            output.Write(r._inputScale(0))

            For i = 1 To r._inputScale.Length - 1
                output.Write(" " & r._inputScale(i))
            Next

            output.WriteLine()
            output.WriteLine("{0} {1}", r._outputStart, r._outputScale)
            output.Flush()
            [Stop]()
        End Sub

        ''' <summary>
        ''' Writes this Range transform to a file.    This will overwrite any previous data in the file.
        ''' </summary>
        ''' <param name="outputFile">The file to write to</param>
        ''' <param name="r">The Range to write</param>
        Public Shared Sub Write(ByVal outputFile As String, ByVal r As RangeTransform)
            Dim s = File.Open(outputFile, FileMode.Create)

            Try
                Write(s, r)
            Finally
                s.Close()
            End Try
        End Sub

        ''' <summary>
        ''' Reads a Range transform from a file.
        ''' </summary>
        ''' <param name="inputFile">The file to read from</param>
        ''' <returns>The Range transform</returns>
        Public Shared Function Read(ByVal inputFile As String) As RangeTransform
            Dim s = File.OpenRead(inputFile)

            Try
                Return Read(s)
            Finally
                s.Close()
            End Try
        End Function

        ''' <summary>
        ''' Reads a Range transform from a stream.
        ''' </summary>
        ''' <param name="stream">The stream to read from</param>
        ''' <returns>The Range transform</returns>
        Public Shared Function Read(ByVal stream As Stream) As RangeTransform
            Start()
            Dim input As StreamReader = New StreamReader(stream)
            Dim length As Integer = Integer.Parse(input.ReadLine())
            Dim inputStart = New Double(length - 1) {}
            Dim inputScale = New Double(length - 1) {}
            Dim parts As String() = input.ReadLine().Split()

            For i = 0 To length - 1
                inputStart(i) = Double.Parse(parts(i))
            Next

            parts = input.ReadLine().Split()

            For i = 0 To length - 1
                inputScale(i) = Double.Parse(parts(i))
            Next

            parts = input.ReadLine().Split()
            Dim outputStart = Double.Parse(parts(0))
            Dim outputScale = Double.Parse(parts(1))
            [Stop]()
            Return New RangeTransform(inputStart, inputScale, outputStart, outputScale, length)
        End Function
    End Class
End Namespace
