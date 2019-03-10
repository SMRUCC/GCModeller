#Region "Microsoft.VisualBasic::97388ba09a9b1f39802a7137b7dc71d0, RNA-Seq\Rockhopper\Java\Math_lib\SmoothDistribution.vb"

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

    ' Class SmoothDistribution
    ' 
    '     Properties: pseudocount
    ' 
    '     Constructor: (+3 Overloads) Sub New
    ' 
    '     Function: binarySearch, getSmoothedValue, ToString
    ' 
    '     Sub: generateHistogram, Main, normalize, smoothData
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic
Imports System.Text

'
' * Copyright 2013 Brian Tjaden
' *
' * This file is part of Rockhopper.
' *
' * Rockhopper is free software: you can redistribute it and/or modify
' * it under the terms of the GNU General Public License as published by
' * the Free Software Foundation, either version 3 of the License, or
' * any later version.
' *
' * Rockhopper is distributed in the hope that it will be useful,
' * but WITHOUT ANY WARRANTY; without even the implied warranty of
' * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' * GNU General Public License for more details.
' *
' * You should have received a copy of the GNU General Public License
' * (in the file gpl.txt) along with Rockhopper.  
' * If not, see <http://www.gnu.org/licenses/>.
' 



Public Class SmoothDistribution

    ''' <summary>
    '''******************************************
    ''' **********   INSTANCE VARIABLES   **********
    ''' </summary>

    Private data As List(Of Integer)
    Private bandwidth As Double
    Private BIN_SIZE As Integer
    Private minimum As Integer
    Private maximum As Integer
    Private _pseudocount As Double

    Private histogram As List(Of Integer)
    Private histogramNormalized As List(Of Double)
    Private values As List(Of Integer)
    Private smoothed As List(Of Double)
    Private smoothedNormalized As List(Of Double)



    ''' <summary>
    '''************************************
    ''' **********   CONSTRUCTORS   **********
    ''' </summary>

    Public Sub New(data As List(Of Integer))
        Me.New(data, 1.0, 1)
    End Sub

    Public Sub New(data As List(Of Integer), bandwidth As Double, BIN_SIZE As Integer)
        Me.New(data, bandwidth, BIN_SIZE, Collections.Min(data), Collections.Max(data))
    End Sub

    Public Sub New(data As List(Of Integer), bandwidth As Double, BIN_SIZE As Integer, minimum As Integer, maximum As Integer)
        Me.data = data
        Me.minimum = minimum
        Me.maximum = maximum
        Me.bandwidth = bandwidth
        Me.BIN_SIZE = BIN_SIZE
        Me._pseudocount = 0.0
        generateHistogram()
        smoothData()
        normalize()
    End Sub



    ''' <summary>
    '''***********************************************
    ''' **********   PUBLIC INSTANCE METHODS   **********
    ''' </summary>

    ''' <summary>
    ''' Get smoothed value at point "x".
    ''' </summary>
    Public Overridable Function getSmoothedValue(x As Double) As Double
        If x < Me.minimum Then
            Return Me._pseudocount
        End If
        If x > Me.maximum Then
            Return Me._pseudocount
        End If
        Dim index As Integer = binarySearch(values, x, 0, values.Count - 1)
        ' If "x" is not in our discrete distribution, the index may be off by 1.
        ' For example, if our distribution includes 4,5,6,7 and x=5.3, we
        ' may return index 1 (corresponding to 5) or index 2 (corresponding to 6).
        If index > 0 Then
            If Math.Abs(CDbl(x - values(index))) > Math.Abs(CDbl(x - values(index - 1))) Then
                Return Math.Max(CDbl(smoothedNormalized(index - 1)), Me._pseudocount)
            End If
        End If
        If index < smoothedNormalized.Count - 1 Then
            If Math.Abs(CDbl(x - values(index))) > Math.Abs(CDbl(x - values(index + 1))) Then
                Return Math.Max(CDbl(smoothedNormalized(index + 1)), Me._pseudocount)
            End If
        End If
        Return Math.Max(CDbl(smoothedNormalized(index)), Me._pseudocount)
    End Function

    ''' <summary>
    ''' Set the pseudocount for this distribution.
    ''' </summary>
    Public Overridable WriteOnly Property pseudocount() As Double
        Set
            Me._pseudocount = Value
        End Set
    End Property

    ''' <summary>
    ''' Returns a String representation of this smoothed distribution.
    ''' </summary>
    Public Overridable Overloads Function ToString() As String
        Dim sb As New StringBuilder()
        sb.Append("VALUES" & vbTab & "SMOOTHED" & vbTab & "NORMALIZED" & vbLf)
        For i As Integer = 0 To smoothedNormalized.Count - 1
            sb.Append(Convert.ToString(values(i)) & vbTab & Convert.ToString(histogram(i)) & vbTab & Convert.ToString(histogramNormalized(i)) & vbTab & Convert.ToString(smoothedNormalized(i)) & vbLf)
        Next
        Return sb.ToString()
    End Function



    ''' <summary>
    '''************************************************
    ''' **********   PRIVATE INSTANCE METHODS   **********
    ''' </summary>

    ''' <summary>
    ''' Generate histogram of data.
    ''' </summary>
    Private Sub generateHistogram()
        Me.histogram = New List(Of Integer)()
        Me.histogramNormalized = New List(Of Double)()
        Dim i As Integer = Me.minimum
        While i <= maximum
            histogram.Add(0)
            histogramNormalized.Add(0.0)
            i += BIN_SIZE
        End While
        For i = 0 To data.Count - 1
            If data(i) < minimum Then
                histogram(0) = histogram(0) + 1
            ElseIf data(i) > maximum Then
                histogram(histogram.Count - 1) = histogram(histogram.Count - 1) + 1
            Else
                histogram((data(i) - minimum) / BIN_SIZE) = histogram((data(i) - minimum) / BIN_SIZE) + 1
            End If
        Next
        For i = 0 To histogram.Count - 1
            histogramNormalized(i) = histogram(i) / CDbl(data.Count)
        Next
    End Sub

    ''' <summary>
    ''' Generate smooth distibution of data (based on Epanechnikov kernel).
    ''' </summary>
    Private Sub smoothData()
        values = New List(Of Integer)()
        smoothed = New List(Of Double)()
        Dim i As Integer = Me.minimum
        While i <= Me.maximum
            ' Compute weighted value for index i
            Dim sum As Double = 0.0
            For j As Integer = 0 To data.Count - 1
                If Math.Abs(CDbl(i - data(j))) <= Me.bandwidth Then
                    Dim u As Double = (i - data(j)) / Me.bandwidth
                    sum += (3.0 / 4.0) * (1.0 - u * u)
                End If
            Next
            values.Add(i)
            smoothed.Add(sum / (data.Count * Me.bandwidth))
            i += Me.BIN_SIZE
        End While
    End Sub

    ''' <summary>
    ''' Determine pseudocounts and normalize distribution so it sums to 1.0.
    ''' </summary>
    Private Sub normalize()
        smoothedNormalized = New List(Of Double)()
        Dim sum As Double = 0.0
        For i As Integer = 0 To smoothed.Count - 1
            sum += smoothed(i)
        Next
        Dim min As Double = Double.MaxValue
        For i As Integer = 0 To smoothed.Count - 1
            Dim normalizedValue As Double = smoothed(i) / sum
            smoothedNormalized.Add(normalizedValue)
            If (normalizedValue > 0) AndAlso (normalizedValue < min) Then
                min = normalizedValue
            End If
        Next
        Me._pseudocount = min / 10.0
    End Sub

    ''' <summary>
    ''' Perform a binary search for the specified value.
    ''' </summary>
    Private Function binarySearch(a As List(Of Integer), value As Double, lo As Integer, hi As Integer) As Integer
        If hi <= lo Then
            Return lo
        End If
        Dim mid As Integer = lo + (hi - lo) \ 2
        If a(mid) > value Then
            Return binarySearch(a, value, lo, mid - 1)
        End If
        If a(mid) < value Then
            Return binarySearch(a, value, mid + 1, hi)
        End If
        Return mid
    End Function



    ''' <summary>
    '''***********************************
    ''' **********   MAIN METHOD   **********
    ''' </summary>

    Private Shared Sub Main(args As String())
        Oracle.Java.System.Err.println(vbLf & "The SmoothDistribution application cannot be executed from the command line. It must be instantiated from another Java application. It takes a set of data and generates a smoothed version of the data distribution based on the Epanechnikov kernel. Smoothing occurs at each point over an interval [-bandwidth,bandwidth]. The BIN_SIZE indicates how big each BIN_SIZE should be." & vbLf)
    End Sub

End Class
