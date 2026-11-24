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
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.GAF.Population
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.Models

Namespace Darwinism.NSGA2

    ''' <summary>
    ''' The result of one environmental selection operation.
    ''' </summary>
    ''' <typeparam name="Chr">The chromosome type of the genetic algorithm.</typeparam>
    Public Class SelectionResult(Of Chr As {Class, Chromosome(Of Chr)})

        ''' <summary>
        ''' The population of the next generation, which contains exactly 
        ''' ``N`` chromosomes.
        ''' </summary>
        ''' <returns>A <see cref="Population(Of Chr)"/> object.</returns>
        Public ReadOnly Property Population As Population(Of Chr)
        ''' <summary>
        ''' The objective vectors of the selected chromosomes, which are aligned 
        ''' with the <see cref="Population"/> member indexes.
        ''' </summary>
        ''' <returns>A objective matrix.</returns>
        Public ReadOnly Property Objectives As Double()()
        ''' <summary>
        ''' The source indexes of the selected chromosomes in the merged 
        ''' population.
        ''' </summary>
        ''' <returns>A vector of the zero based solution indexes.</returns>
        Public ReadOnly Property Indices As Integer()
        ''' <summary>
        ''' The non-dominated rank of the selected chromosomes, which are aligned 
        ''' with the <see cref="Population"/> member indexes.
        ''' </summary>
        ''' <returns>A vector of the non-dominated ranks.</returns>
        Public ReadOnly Property Ranks As Integer()
        ''' <summary>
        ''' The crowding distance of the selected chromosomes, which are aligned 
        ''' with the <see cref="Population"/> member indexes.
        ''' </summary>
        ''' <returns>A vector of the crowding distance values.</returns>
        Public ReadOnly Property Crowding As Double()

        ''' <summary>
        ''' Create a environmental selection result.
        ''' </summary>
        ''' <param name="population">The population of the next generation.</param>
        ''' <param name="objectives">The objective vectors of the selected chromosomes.</param>
        ''' <param name="indices">The source indexes of the selected chromosomes.</param>
        ''' <param name="ranks">The non-dominated rank of the selected chromosomes.</param>
        ''' <param name="crowding">The crowding distance of the selected chromosomes.</param>
        Public Sub New(population As Population(Of Chr), objectives As Double()(), indices As Integer(),
                       ranks As Integer(), crowding As Double())
            Me.Population = population
            Me.Objectives = objectives
            Me.Indices = indices
            Me.Ranks = ranks
            Me.Crowding = crowding
        End Sub
    End Class

    ''' <summary>
    ''' The environmental selection (elitism) operation of the NSGA-II: the 
    ''' parent and the offspring populations are merged together, and the best 
    ''' ``N`` chromosomes are selected from the merged population by the 
    ''' non-dominated rank and the crowding distance.
    ''' </summary>
    Public Module EnvironmentalSelection

        ''' <summary>
        ''' Select the ``N`` chromosomes of the next generation from the merged 
        ''' population.
        ''' </summary>
        ''' <typeparam name="Chr">The chromosome type of the genetic algorithm.</typeparam>
        ''' <param name="chromosomes">The merged chromosomes (parent + offspring, size ``2N``).</param>
        ''' <param name="objectives">
        ''' The objective matrix of the <paramref name="chromosomes"/>, its order 
        ''' should be aligned with the <paramref name="chromosomes"/> list.
        ''' </param>
        ''' <param name="populationSize">The number of the chromosomes which will be selected.</param>
        ''' <returns>
        ''' A <see cref="SelectionResult(Of Chr)"/> object which contains the new 
        ''' population and the rank information of the merged population.
        ''' </returns>
        ''' <remarks>
        ''' The chromosomes are filled front by front. When a front cannot be 
        ''' fully accepted, its members are sorted by the crowding distance in 
        ''' descending order and the first remaining members are taken.
        ''' </remarks>
        Public Function SelectNextGeneration(Of Chr As {Class, Chromosome(Of Chr)})(
                                                 chromosomes As IList(Of Chr),
                                                 objectives As Double()(),
                                                 populationSize As Integer) As SelectionResult(Of Chr)

            If chromosomes Is Nothing Then
                Throw New ArgumentNullException(NameOf(chromosomes))
            End If
            If objectives Is Nothing Then
                Throw New ArgumentNullException(NameOf(objectives))
            End If
            If populationSize <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(populationSize), "The population size should be greater than zero.")
            End If
            If chromosomes.Count <> objectives.Length Then
                Throw New ArgumentException($"The size of the chromosomes({chromosomes.Count}) and the objective vectors({objectives.Length}) should be equals to each other.", NameOf(objectives))
            End If

            Dim fronts As List(Of Integer()) = FastNonDominatedSort(objectives)
            Dim selected As New List(Of Integer)
            Dim ranks As Integer() = New Integer(objectives.Length - 1) {}
            Dim crowding As Double() = New Double(objectives.Length - 1) {}

            For i As Integer = 0 To ranks.Length - 1
                ranks(i) = -1
            Next

            For rank As Integer = 0 To fronts.Count - 1
                Dim front As Integer() = fronts(rank)
                Dim distance As Double() = CrowdingDistance(objectives, front)

                ' the crowding distance is required by the tournament selection 
                ' even when the whole front was accepted
                For pos As Integer = 0 To front.Length - 1
                    Dim idx As Integer = front(pos)

                    ranks(idx) = rank
                    crowding(idx) = distance(pos)
                Next

                If selected.Count + front.Length <= populationSize Then
                    Call selected.AddRange(front)
                Else
                    Dim rest% = populationSize - selected.Count
                    Dim order As Integer() = Enumerable _
                        .Range(0, front.Length) _
                        .OrderByDescending(Function(pos) distance(pos)) _
                        .ToArray

                    For pos As Integer = 0 To rest - 1
                        Call selected.Add(front(order(pos)))
                    Next

                    Exit For
                End If
            Next

            Dim population As Population(Of Chr) = New Population(Of Chr)(
                New PopulationList(Of Chr),
                New ParallelDataSetCompute(Of Chr)
            ) With {
                .capacitySize = populationSize
            }
            Dim selectedObjectives As Double()() = New Double(selected.Count - 1)() {}
            Dim selectedRanks As Integer() = New Integer(selected.Count - 1) {}
            Dim selectedCrowding As Double() = New Double(selected.Count - 1) {}
            Dim indices As Integer() = New Integer(selected.Count - 1) {}

            For i As Integer = 0 To selected.Count - 1
                Dim idx As Integer = selected(i)

                indices(i) = idx
                selectedObjectives(i) = objectives(idx)
                selectedRanks(i) = ranks(idx)
                selectedCrowding(i) = crowding(idx)

                Call population.Add(chromosomes(idx))
            Next

            Return New SelectionResult(Of Chr)(population, selectedObjectives, indices, selectedRanks, selectedCrowding)
        End Function
    End Module
End Namespace
