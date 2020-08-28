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
Imports System.Globalization
Imports stdNum = System.Math

Namespace SVM
    ''' <summary>
    ''' A transform which learns the mean and variance of a sample set and uses these to transform new data
    ''' so that it has zero mean and unit variance.
    ''' </summary>
    Public Class GaussianTransform
        Implements IRangeTransform

        Private _means As Double()
        Private _stddevs As Double()

        ''' <summary>
        ''' Determines the Gaussian transform for the provided problem.
        ''' </summary>
        ''' <param name="prob">The Problem to analyze</param>
        ''' <returns>The Gaussian transform for the problem</returns>
        Public Shared Function Compute(ByVal prob As Problem) As GaussianTransform
            Dim counts = New Integer(prob.MaxIndex - 1) {}
            Dim means = New Double(prob.MaxIndex - 1) {}

            For Each sample In prob.X

                For i = 0 To sample.Length - 1
                    means(sample(i).Index - 1) += sample(i).Value
                    counts(sample(i).Index - 1) += 1
                Next
            Next

            For i = 0 To prob.MaxIndex - 1
                If counts(i) = 0 Then counts(i) = 2
                means(i) /= counts(i)
            Next

            Dim stddevs = New Double(prob.MaxIndex - 1) {}

            For Each sample In prob.X

                For i = 0 To sample.Length - 1
                    Dim diff = sample(i).Value - means(sample(i).Index - 1)
                    stddevs(sample(i).Index - 1) += diff * diff
                Next
            Next

            For i = 0 To prob.MaxIndex - 1
                If stddevs(i) = 0 Then Continue For
                stddevs(i) /= counts(i) - 1
                stddevs(i) = stdNum.Sqrt(stddevs(i))
            Next

            Return New GaussianTransform(means, stddevs)
        End Function

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="means">Means in each dimension</param>
        ''' <param name="stddevs">Standard deviation in each dimension</param>
        Public Sub New(ByVal means As Double(), ByVal stddevs As Double())
            _means = means
            _stddevs = stddevs
        End Sub

        ''' <summary>
        ''' Saves the transform to the disk.  The samples are not stored, only the 
        ''' statistics.
        ''' </summary>
        ''' <param name="stream">The destination stream</param>
        ''' <param name="transform">The transform</param>
        Public Shared Sub Write(ByVal stream As Stream, ByVal transform As GaussianTransform)
            Start()
            Dim output As StreamWriter = New StreamWriter(stream)
            output.WriteLine(transform._means.Length)

            For i = 0 To transform._means.Length - 1
                output.WriteLine("{0} {1}", transform._means(i), transform._stddevs(i))
            Next

            output.Flush()
            [Stop]()
        End Sub

        ''' <summary>
        ''' Reads a GaussianTransform from the provided stream.
        ''' </summary>
        ''' <param name="stream">The source stream</param>
        ''' <returns>The transform</returns>
        Public Shared Function Read(ByVal stream As Stream) As GaussianTransform
            Start()
            Dim input As StreamReader = New StreamReader(stream)
            Dim length As Integer = Integer.Parse(input.ReadLine(), CultureInfo.InvariantCulture)
            Dim means = New Double(length - 1) {}
            Dim stddevs = New Double(length - 1) {}

            For i = 0 To length - 1
                Dim parts As String() = input.ReadLine().Split()
                means(i) = Double.Parse(parts(0), CultureInfo.InvariantCulture)
                stddevs(i) = Double.Parse(parts(1), CultureInfo.InvariantCulture)
            Next

            [Stop]()
            Return New GaussianTransform(means, stddevs)
        End Function

        ''' <summary>
        ''' Saves the transform to the disk.  The samples are not stored, only the 
        ''' statistics.
        ''' </summary>
        ''' <param name="filename">The destination filename</param>
        ''' <param name="transform">The transform</param>
        Public Shared Sub Write(ByVal filename As String, ByVal transform As GaussianTransform)
            Dim output = File.Open(filename, FileMode.Create)

            Try
                Write(output, transform)
            Finally
                output.Close()
            End Try
        End Sub

        ''' <summary>
        ''' Reads a GaussianTransform from the provided stream.
        ''' </summary>
        ''' <param name="filename">The source filename</param>
        ''' <returns>The transform</returns>
        Public Shared Function Read(ByVal filename As String) As GaussianTransform
            Dim input = File.Open(filename, FileMode.Open)

            Try
                Return Read(input)
            Finally
                input.Close()
            End Try
        End Function

#Region "IRangeTransform Members"

        ''' <summary>
        ''' Transform the input value using the transform stored for the provided index.
        ''' </summary>
        ''' <param name="input">Input value</param>
        ''' <param name="index">Index of the transform to use</param>
        ''' <returns>The transformed value</returns>
        Public Function Transform(ByVal input As Double, ByVal index As Integer) As Double Implements IRangeTransform.Transform
            index -= 1
            If _stddevs(index) = 0 Then Return 0
            Dim diff = input - _means(index)
            diff /= _stddevs(index)
            Return diff
        End Function
        ''' <summary>
        ''' Transforms the input array.
        ''' </summary>
        ''' <param name="input">The array to transform</param>
        ''' <returns>The transformed array</returns>
        Public Function Transform(ByVal input As Node()) As Node() Implements IRangeTransform.Transform
            Dim output = New Node(input.Length - 1) {}

            For i = 0 To output.Length - 1
                Dim index = input(i).Index
                Dim value = input(i).Value
                output(i) = New Node(index, Transform(value, index))
            Next

            Return output
        End Function

#End Region
    End Class
End Namespace
