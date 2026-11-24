' *****************************************************************************
' Copyright 2018 sciBASIC.NET Foundation
'
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License.
' You may obtain a copy of the License at
'
'   http://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.
' *****************************************************************************

Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace Darwinism.NSGA2

    ''' <summary>
    ''' The Pareto dominance related algorithms of the NSGA-II: the fast 
    ''' non-dominated sorting and the crowding distance calculation.
    ''' </summary>
    ''' <remarks>
    ''' All of the functions in this module work on the raw objective matrix, 
    ''' the objective values are treated as minimization objectives. The 
    ''' ``NaN`` values are treated as the worst value, so that a failed 
    ''' evaluation will never dominate a successful one.
    ''' </remarks>
    Public Module ParetoSorting

        ''' <summary>
        ''' The replacement value of the ``NaN`` objective value; it is a very 
        ''' large but still finite value so that the normalization computation 
        ''' inside the <see cref="CrowdingDistance(Double()(), Integer())"/> 
        ''' function will not overflow.
        ''' </summary>
        Public Const NaNValue As Double = Double.MaxValue / 4

        ''' <summary>
        ''' Get the objective value which is used in the dominance comparison.
        ''' </summary>
        ''' <param name="value">The raw objective value.</param>
        ''' <returns>
        ''' The <paramref name="value"/> itself, or the <see cref="NaNValue"/> 
        ''' value when it is ``NaN``.
        ''' </returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetObjectiveValue(value As Double) As Double
            If Double.IsNaN(value) Then
                Return NaNValue
            Else
                Return value
            End If
        End Function

        ''' <summary>
        ''' Does the solution <paramref name="a"/> dominate the solution 
        ''' <paramref name="b"/>? (minimization objectives)
        ''' </summary>
        ''' <param name="a">The objective vector of the first solution.</param>
        ''' <param name="b">The objective vector of the second solution.</param>
        ''' <returns>
        ''' ``True`` when <paramref name="a"/> is not worse than 
        ''' <paramref name="b"/> in all of the objectives and is strictly better 
        ''' in at least one objective.
        ''' </returns>
        <Extension>
        Public Function Dominates(a As Double(), b As Double()) As Boolean
            If a Is Nothing Then
                Throw New ArgumentNullException(NameOf(a))
            End If
            If b Is Nothing Then
                Throw New ArgumentNullException(NameOf(b))
            End If
            If a.Length <> b.Length Then
                Throw New ArgumentException($"The size of the objective vectors should be equals to each other: {a.Length} vs {b.Length}.")
            End If

            Dim strictlyBetter As Boolean = False

            For i As Integer = 0 To a.Length - 1
                Dim x As Double = GetObjectiveValue(a(i))
                Dim y As Double = GetObjectiveValue(b(i))

                If x > y Then
                    Return False
                ElseIf x < y Then
                    strictlyBetter = True
                End If
            Next

            Return strictlyBetter
        End Function

        ''' <summary>
        ''' The fast non-dominated sorting of the target objective matrix, 
        ''' the complexity of this function is ``O(MN^2)``.
        ''' </summary>
        ''' <param name="objectives">
        ''' The objective matrix: ``objectives(i)(m)`` is the value of the 
        ''' objective ``m`` of the solution ``i``.
        ''' </param>
        ''' <returns>
        ''' The non-dominated fronts; each front is a vector of the solution 
        ''' indexes, the first front is the Pareto front.
        ''' </returns>
        Public Function FastNonDominatedSort(objectives As Double()()) As List(Of Integer())
            If objectives Is Nothing Then
                Throw New ArgumentNullException(NameOf(objectives))
            End If

            Dim n% = objectives.Length

            If n = 0 Then
                Return New List(Of Integer())()
            End If

            Dim m% = objectives(0).Length

            For i As Integer = 0 To n - 1
                If objectives(i) Is Nothing OrElse objectives(i).Length <> m Then
                    Throw New ArgumentException($"The size of the objective vectors should be equals to each other, error was found at the solution index={i}.", NameOf(objectives))
                End If
            Next

            ' S(p): the solutions which are dominated by p
            Dim dominated As List(Of Integer)() = New List(Of Integer)(n - 1) {}
            ' n(p): the number of the solutions which dominate p
            Dim dominationCount As Integer() = New Integer(n - 1) {}
            Dim first As New List(Of Integer)

            For p As Integer = 0 To n - 1
                dominated(p) = New List(Of Integer)

                For q As Integer = 0 To n - 1
                    If p = q Then
                        Continue For
                    End If

                    If objectives(p).Dominates(objectives(q)) Then
                        dominated(p).Add(q)
                    ElseIf objectives(q).Dominates(objectives(p)) Then
                        dominationCount(p) += 1
                    End If
                Next

                If dominationCount(p) = 0 Then
                    Call first.Add(p)
                End If
            Next

            Dim fronts As New List(Of Integer())
            Dim current As List(Of Integer) = first

            Do While current.Count > 0
                Call fronts.Add(current.ToArray)

                Dim nextFront As New List(Of Integer)

                For Each p As Integer In current
                    For Each q As Integer In dominated(p)
                        dominationCount(q) -= 1

                        If dominationCount(q) = 0 Then
                            Call nextFront.Add(q)
                        End If
                    Next
                Next

                current = nextFront
            Loop

            Return fronts
        End Function

        ''' <summary>
        ''' Calculate the crowding distance of the solutions inside one 
        ''' non-dominated front.
        ''' </summary>
        ''' <param name="objectives">The objective matrix of the whole population.</param>
        ''' <param name="front">
        ''' The solution indexes of one non-dominated front.
        ''' </param>
        ''' <returns>
        ''' The crowding distance values which are aligned with the 
        ''' <paramref name="front"/> indexes; the boundary solutions of each 
        ''' objective have the positive infinity distance.
        ''' </returns>
        Public Function CrowdingDistance(objectives As Double()(), front As Integer()) As Double()
            If objectives Is Nothing Then
                Throw New ArgumentNullException(NameOf(objectives))
            End If
            If front Is Nothing Then
                Throw New ArgumentNullException(NameOf(front))
            End If

            Dim n% = front.Length
            Dim distance As Double() = New Double(n - 1) {}

            If n = 0 Then
                Return distance
            End If

            If n <= 2 Then
                For i As Integer = 0 To n - 1
                    distance(i) = Double.PositiveInfinity
                Next

                Return distance
            End If

            Dim m% = objectives(front(0)).Length

            For obj As Integer = 0 To m - 1
                Dim objectiveIndex As Integer = obj
                Dim order As Integer() = Enumerable _
                    .Range(0, n) _
                    .OrderBy(Function(pos) GetObjectiveValue(objectives(front(pos))(objectiveIndex))) _
                    .ToArray

                Dim firstPos% = order(0)
                Dim lastPos% = order(n - 1)
                Dim minValue# = GetObjectiveValue(objectives(front(firstPos))(objectiveIndex))
                Dim maxValue# = GetObjectiveValue(objectives(front(lastPos))(objectiveIndex))

                ' the boundary solutions are always kept
                distance(firstPos) = Double.PositiveInfinity
                distance(lastPos) = Double.PositiveInfinity

                If maxValue - minValue <= 0 Then
                    ' the degenerate objective will not provide any density 
                    ' information, so skip this objective
                    Continue For
                End If

                For pos As Integer = 1 To n - 2
                    Dim prevValue# = GetObjectiveValue(objectives(front(order(pos - 1)))(objectiveIndex))
                    Dim nextValue# = GetObjectiveValue(objectives(front(order(pos + 1)))(objectiveIndex))
                    Dim currentPos% = order(pos)

                    distance(currentPos) += (nextValue - prevValue) / (maxValue - minValue)
                Next
            Next

            Return distance
        End Function
    End Module
End Namespace
