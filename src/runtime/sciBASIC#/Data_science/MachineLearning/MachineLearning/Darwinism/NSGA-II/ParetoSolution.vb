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

Namespace Darwinism.NSGA2

    ''' <summary>
    ''' A chromosome together with its objective vector, the non-dominated rank 
    ''' and the crowding distance inside the same rank.
    ''' </summary>
    ''' <typeparam name="Chr">The chromosome type of the genetic algorithm.</typeparam>
    Public Class ParetoSolution(Of Chr)

        ''' <summary>
        ''' The candidate solution chromosome.
        ''' </summary>
        ''' <returns>A chromosome object.</returns>
        Public ReadOnly Property Chromosome As Chr
        ''' <summary>
        ''' The objective vector of the <see cref="Chromosome"/>; all of the 
        ''' values are treated as the minimization objectives.
        ''' </summary>
        ''' <returns>A <see cref="Double"/> vector.</returns>
        Public ReadOnly Property Objectives As Double()
        ''' <summary>
        ''' The non-dominated rank of this solution, the first front is ``0``.
        ''' </summary>
        ''' <returns>An <see cref="Integer"/> value.</returns>
        Public ReadOnly Property Rank As Integer
        ''' <summary>
        ''' The crowding distance of this solution inside its rank; the boundary 
        ''' solutions of a front have the positive infinity distance.
        ''' </summary>
        ''' <returns>A <see cref="Double"/> value.</returns>
        Public ReadOnly Property CrowdingDistance As Double

        ''' <summary>
        ''' Create a Pareto solution record.
        ''' </summary>
        ''' <param name="chromosome">The candidate solution chromosome.</param>
        ''' <param name="objectives">The objective vector of the chromosome.</param>
        ''' <param name="rank">The non-dominated rank of this solution.</param>
        ''' <param name="crowdingDistance">The crowding distance of this solution.</param>
        Public Sub New(chromosome As Chr, objectives As Double(), rank As Integer, crowdingDistance As Double)
            Me.Chromosome = chromosome
            Me.Objectives = objectives
            Me.Rank = rank
            Me.CrowdingDistance = crowdingDistance
        End Sub

        ''' <summary>
        ''' Display the objective vector and the rank information of this 
        ''' solution as a text line.
        ''' </summary>
        ''' <returns>A string which describes this solution.</returns>
        Public Overrides Function ToString() As String
            Return $"[rank={Rank}, crowding={CrowdingDistance}] {String.Join(", ", Objectives)}"
        End Function
    End Class
End Namespace
