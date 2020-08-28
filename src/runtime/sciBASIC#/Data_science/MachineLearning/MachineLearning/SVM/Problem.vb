#Region "Microsoft.VisualBasic::7b4d03e6721aa81c7f8c921fa70bb72f, Data_science\MachineLearning\MachineLearning\SVM\Problem.vb"

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

    '     Class Problem
    ' 
    '         Properties: Count, MaxIndex, X, Y
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: Equals, GetHashCode, (+2 Overloads) Read
    ' 
    '         Sub: (+2 Overloads) Write
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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



Imports System.IO
Imports Microsoft.VisualBasic.Text
Imports stdNum = System.Math

Namespace SVM

    ''' <summary>
    ''' Encapsulates a problem, or set of vectors which must be classified.
    ''' </summary>
    <Serializable>
    Public Class Problem

        ''' <summary>
        ''' Number of vectors.
        ''' </summary>
        Public ReadOnly Property Count As Integer
            Get
                Return X.Length
            End Get
        End Property

        ''' <summary>
        ''' Class labels.
        ''' </summary>
        Public Property Y As Double()

        ''' <summary>
        ''' Vector data.
        ''' </summary>
        Public Property X As Node()()

        ''' <summary>
        ''' Maximum index for a vector. this value is the width of each 
        ''' row in <see cref="X"/> and equals to the length of vector 
        ''' <see cref="DimensionNames"/> 
        ''' </summary>
        Public Property MaxIndex As Integer

        ''' <summary>
        ''' the width of each row in <see cref="X"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property DimensionNames As String()

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="y">The class labels</param>
        ''' <param name="x">Vector data.</param>
        ''' <param name="maxIndex">Maximum index for a vector</param>
        Public Sub New(ByVal y As Double(), ByVal x As Node()(), ByVal maxIndex As Integer)
            Me.Y = y
            Me.X = x
            Me.MaxIndex = maxIndex
        End Sub

        ''' <summary>
        ''' Empty Constructor.  Nothing is initialized.
        ''' </summary>
        Public Sub New()
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim other As Problem = TryCast(obj, Problem)
            If other Is Nothing Then Return False
            Return other.Count = Count AndAlso other.MaxIndex = MaxIndex AndAlso other.X.IsEqual(X) AndAlso other.Y.IsEqual(Y)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Count.GetHashCode() + MaxIndex.GetHashCode() + X.ComputeHashcode2() + Y.ComputeHashcode()
        End Function

        ''' <summary>
        ''' Reads a problem from a stream.
        ''' </summary>
        ''' <param name="stream">Stream to read from</param>
        ''' <returns>The problem</returns>
        Public Shared Function Read(ByVal stream As Stream) As Problem
            Start()
            Dim input As StreamReader = New StreamReader(stream)
            Dim vy As List(Of Double) = New List(Of Double)()
            Dim vx As List(Of Node()) = New List(Of Node())()
            Dim max_index = 0

            While input.Peek() > -1
                Dim parts As String() = input.ReadLine().Trim().Split()
                vy.Add(Double.Parse(parts(0)))
                Dim m = parts.Length - 1
                Dim x = New Node(m - 1) {}

                For j = 0 To m - 1
                    x(j) = New Node()
                    Dim nodeParts = parts(j + 1).Split(":"c)
                    x(j).Index = Integer.Parse(nodeParts(0))
                    x(j).Value = Double.Parse(nodeParts(1))
                Next

                If m > 0 Then max_index = stdNum.Max(max_index, x(m - 1).Index)
                vx.Add(x)
            End While

            [Stop]()
            Return New Problem(vy.ToArray(), vx.ToArray(), max_index)
        End Function

        ''' <summary>
        ''' Writes a problem to a stream.
        ''' </summary>
        ''' <param name="stream">The stream to write the problem to.</param>
        ''' <param name="problem">The problem to write.</param>
        Public Shared Sub Write(ByVal stream As Stream, ByVal problem As Problem)
            Start()
            Dim output As StreamWriter = New StreamWriter(stream)

            For i = 0 To problem.Count - 1
                output.Write(problem.Y(i))

                For j = 0 To problem.X(i).Length - 1
                    output.Write(" {0}:{1:0.000000}", problem.X(i)(j).Index, problem.X(i)(j).Value)
                Next

                output.Write(ASCII.LF)
            Next

            output.Flush()
            [Stop]()
        End Sub

        ''' <summary>
        ''' Reads a Problem from a file.
        ''' </summary>
        ''' <param name="filename">The file to read from.</param>
        ''' <returns>the Probem</returns>
        Public Shared Function Read(ByVal filename As String) As Problem
            Dim input = File.OpenRead(filename)

            Try
                Return Read(input)
            Finally
                input.Close()
            End Try
        End Function

        ''' <summary>
        ''' Writes a problem to a file.   This will overwrite any previous data in the file.
        ''' </summary>
        ''' <param name="filename">The file to write to</param>
        ''' <param name="problem">The problem to write</param>
        Public Shared Sub Write(ByVal filename As String, ByVal problem As Problem)
            Dim output = File.Open(filename, FileMode.Create)

            Try
                Write(output, problem)
            Finally
                output.Close()
            End Try
        End Sub
    End Class
End Namespace

