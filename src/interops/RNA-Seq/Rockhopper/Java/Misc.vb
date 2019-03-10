#Region "Microsoft.VisualBasic::0b6c1ba257ec39b62a7576a2e7752aa3, RNA-Seq\Rockhopper\Java\Misc.vb"

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

    '     Module Misc
    ' 
    '         Function: correlation, mean, partition, partitionDouble, (+2 Overloads) select_Double
    '                   (+2 Overloads) select_Long
    ' 
    '         Sub: Main, swap
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic

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

Namespace Java

    Public Module Misc

        ''' <summary>
        '''***********************************************
        ''' **********   PRIVATE CLASS VARIABLES   **********
        ''' </summary>

        Private rand As Random
        ' Random number generator


        ''' <summary>
        '''*********************************************
        ''' **********   PUBLIC STATIC METHODS   **********
        ''' </summary>

        ''' <summary>
        ''' Return the element in the ArrayList corresponding to 
        ''' the specified order statistic.
        ''' </summary>
        Public Function select_Long(a As List(Of Long), orderStatistic As Integer) As Long
            Return select_Long(a, 0, a.Count - 1, orderStatistic)
        End Function

        ''' <summary>
        ''' Return the element in the ArrayList corresponding to 
        ''' the specified order statistic.
        ''' </summary>
        Public Function select_Double(a As List(Of Double), orderStatistic As Integer) As Double
            Return select_Double(a, 0, a.Count - 1, orderStatistic)
        End Function

        ''' <summary>
        ''' Returns the correlation coefficient of two ArrayLists.
        ''' </summary>
        Public Function correlation(x As List(Of Long), y As List(Of Long)) As Double
            If x.Count <> y.Count Then
                ' Lists have different lengths
                Return 0.0
            End If
            Dim meanX As Double = mean(x)
            Dim meanY As Double = mean(y)
            Dim sumXY As Double = 0.0
            Dim sumXX As Double = 0.0
            Dim sumYY As Double = 0.0
            For i As Integer = 0 To x.Count - 1
                sumXY += (x(i) - meanX) * (y(i) - meanY)
                sumXX += (x(i) - meanX) * (x(i) - meanX)
                sumYY += (y(i) - meanY) * (y(i) - meanY)
            Next
            If (sumXX = 0.0) OrElse (sumYY = 0.0) Then
                Return 0.0
            End If
            Return sumXY / (Math.Sqrt(sumXX) * Math.Sqrt(sumYY))
        End Function

        ''' <summary>
        ''' Returns the mean of an ArrayList.
        ''' </summary>
        Public Function mean(a As List(Of Long)) As Double
            Dim sum As Double = 0
            Dim iter As IEnumerator(Of Long) = a.GetEnumerator()
            While iter.MoveNext()
                sum += iter.Current
            End While
            Return sum / a.Count
        End Function



        ''' <summary>
        '''**********************************************
        ''' **********   PRIVATE STATIC METHODS   **********
        ''' </summary>

        ''' <summary>
        ''' Return the element in the ArrayList corresponding to
        ''' the specified order statistic.
        ''' </summary>
        Private Function select_Long(a As List(Of Long), lo As Integer, hi As Integer, orderStatistic As Integer) As Long
            If lo >= hi Then
                If a.Count = 0 Then
                    Return 0
                End If
                Return a(lo)
            End If
            Dim p As Integer = partition(a, lo, hi)
            If orderStatistic = p - lo + 1 Then
                Return a(p)
            ElseIf orderStatistic < p - lo + 1 Then
                Return select_Long(a, lo, p - 1, orderStatistic)
            Else
                Return select_Long(a, p + 1, hi, orderStatistic - (p - lo + 1))
            End If
        End Function

        ''' <summary>
        ''' Partitions "a" around random pivot element. 
        ''' Returns the index of the pivot.
        ''' </summary>
        Private Function partition(a As List(Of Long), lo As Integer, hi As Integer) As Integer
            If rand Is Nothing Then
                rand = New Random()
            End If
            swap(a, rand.[Next](hi - lo + 1) + lo, hi)
            ' Choose pivot randomly and place at end of "a"
            Dim x As Long = a(hi)
            Dim i As Integer = lo - 1
            For j As Integer = lo To hi - 1
                If a(j) <= x Then
                    i += 1
                    swap(a, i, j)
                End If
            Next
            swap(a, i + 1, hi)
            Return i + 1
        End Function

        ''' <summary>
        ''' Return the element in the ArrayList corresponding to
        ''' the specified order statistic.
        ''' </summary>
        Private Function select_Double(a As List(Of Double), lo As Integer, hi As Integer, orderStatistic As Integer) As Double
            If lo = hi Then
                Return a(lo)
            End If
            Dim p As Integer = partitionDouble(a, lo, hi)
            If orderStatistic = p - lo + 1 Then
                Return a(p)
            ElseIf orderStatistic < p - lo + 1 Then
                Return select_Double(a, lo, p - 1, orderStatistic)
            Else
                Return select_Double(a, p + 1, hi, orderStatistic - (p - lo + 1))
            End If
        End Function

        ''' <summary>
        ''' Partitions "a" around random pivot element. 
        ''' Returns the index of the pivot.
        ''' </summary>
        Private Function partitionDouble(a As List(Of Double), lo As Integer, hi As Integer) As Integer
            If rand Is Nothing Then
                rand = New Random()
            End If
            swap(a, rand.[Next](hi - lo + 1) + lo, hi)
            ' Choose pivot randomly and place at end of "a"
            Dim x As Double = a(hi)
            Dim i As Integer = lo - 1
            For j As Integer = lo To hi - 1
                If a(j) <= x Then
                    i += 1
                    swap(a, i, j)
                End If
            Next
            swap(a, i + 1, hi)
            Return i + 1
        End Function

        ''' <summary>
        ''' Swaps two elements in an ArrayList at the specified indices.
        ''' </summary>
        Private Sub swap(Of E)(a As List(Of E), i As Integer, j As Integer)
            Dim temp As E = a(i)
            a(i) = a(j)
            a(j) = temp
        End Sub


        ''' <summary>
        '''***********************************
        ''' **********   MAIN METHOD   **********
        ''' </summary>
        Private Sub Main(args As String())

            Oracle.Java.System.Err.println(vbLf & "Class Methods:" & vbLf)
            Oracle.Java.System.Err.println(vbTab & "select(ArrayList<Integer or Double> a, int orderStatistic)" & vbTab & "Computes the specified order statistic in the list." & vbLf)
            Oracle.Java.System.Err.println(vbTab & "correlation(ArrayList<Integer> x, ArrayList<Integer> y)" & vbTab & "Computes the correlation coefficient of two lists." & vbLf)
            Oracle.Java.System.Err.println(vbTab & "mean(ArrayList<Integer> a)" & vbTab & "Computes the mean of a list." & vbLf)

        End Sub

    End Module

End Namespace
