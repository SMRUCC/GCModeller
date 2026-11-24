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
Imports System.Threading.Tasks
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.GAF.Population
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.Models
Imports Microsoft.VisualBasic.Math
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Namespace Darwinism.NSGA2

    ''' <summary>
    ''' The NSGA-II (Non-dominated Sorting Genetic Algorithm II) engine: it 
    ''' evolves a population toward the whole Pareto front of a multi-objective 
    ''' optimization problem by the fast non-dominated sorting, the crowding 
    ''' distance and the elitism preservation mechanisms.
    ''' </summary>
    ''' <typeparam name="Chr">The chromosome type of the genetic algorithm.</typeparam>
    ''' <remarks>
    ''' + All of the objectives are treated as the minimization problems.
    '''
    ''' + The crossover and the mutation operators are delegated to the 
    ''' <see cref="Chromosome(Of Chr).Crossover"/> and the 
    ''' <see cref="Chromosome(Of Chr).Mutate"/> functions by default, so the 
    ''' existing chromosome models can be reused directly; the real coded 
    ''' operators <see cref="RealCodedOperators.SBXCrossover(Of Chr)"/> and 
    ''' <see cref="RealCodedOperators.PolynomialMutation(Of Chr)"/> can be applied 
    ''' by the <see cref="IRealVector(Of Chr)"/> chromosomes.
    ''' </remarks>
    Public Class NSGA2Engine(Of Chr As {Class, Chromosome(Of Chr)}) : Inherits Model

        Shared ReadOnly randfSeeds As New [Default](Of IRandomSeeds)(Function() randf.seeds)

        ReadOnly objectiveEvaluator As IMultiObjective(Of Chr)
        ReadOnly crossoverOperator As Func(Of Chr, Chr, Random, IEnumerable(Of Chr))
        ReadOnly mutationOperator As Func(Of Chr, Random, Chr)
        ReadOnly expectedSize As Integer

        Friend ReadOnly seeds As IRandomSeeds

        Dim currentPopulation As Population(Of Chr)
        Dim currentObjectives As Double()()
        Dim currentRanks As Integer()
        Dim currentCrowding As Double()

        ''' <summary>
        ''' The number of the evolution iterations which were already finished.
        ''' </summary>
        ''' <returns>An <see cref="Integer"/> value.</returns>
        Public Property Generation As Integer

        ''' <summary>
        ''' The current parent population of this NSGA-II engine.
        ''' </summary>
        ''' <returns>A <c>Population(Of Chr)</c> object.</returns>
        Public ReadOnly Property Population As Population(Of Chr)
            Get
                Return currentPopulation
            End Get
        End Property

        ''' <summary>
        ''' The number of the chromosomes which are kept in each generation.
        ''' </summary>
        ''' <returns>An <see cref="Integer"/> value.</returns>
        Public ReadOnly Property PopulationSize As Integer
            Get
                Return expectedSize
            End Get
        End Property

        ''' <summary>
        ''' The multi-objective evaluation model which is used by this engine.
        ''' </summary>
        ''' <returns>A <see cref="IMultiObjective(Of Chr)"/> model.</returns>
        Public ReadOnly Property ObjectiveModel As IMultiObjective(Of Chr)
            Get
                Return objectiveEvaluator
            End Get
        End Property

        ''' <summary>
        ''' The mean objective sum of the current first non-dominated front; 
        ''' this value is only used as a scalar progress indicator for the text 
        ''' report and it does NOT participate in the selection procedure.
        ''' </summary>
        ''' <returns>
        ''' A <see cref="Double"/> value, or ``NaN`` when the current front is 
        ''' empty.
        ''' </returns>
        Public ReadOnly Property ProgressValue As Double
            Get
                If currentObjectives Is Nothing OrElse currentRanks Is Nothing OrElse
                    currentObjectives.Length <> currentPopulation.Size Then

                    Call EvaluateAll(parallel:=False)
                End If

                Dim count As Integer = 0
                Dim total As Double = 0

                For i As Integer = 0 To currentObjectives.Length - 1
                    If currentRanks(i) = 0 Then
                        total += currentObjectives(i).Sum()
                        count += 1
                    End If
                Next

                If count = 0 Then
                    Return Double.NaN
                Else
                    Return total / count
                End If
            End Get
        End Property

        ''' <summary>
        ''' Create a NSGA-II engine from an initial population.
        ''' </summary>
        ''' <param name="population">
        ''' The initial population; the size of this population is also used as 
        ''' the ``N`` value of the next generations.
        ''' </param>
        ''' <param name="objectives">The multi-objective evaluation model.</param>
        ''' <param name="seeds">
        ''' The random number generator; the shared global generator will be used 
        ''' when this parameter is ``Nothing``.
        ''' </param>
        ''' <param name="crossover">
        ''' The crossover operator; the 
        ''' <see cref="Chromosome(Of Chr).Crossover"/> function will be used when 
        ''' this parameter is ``Nothing``.
        ''' </param>
        ''' <param name="mutate">
        ''' The mutation operator; the <see cref="Chromosome(Of Chr).Mutate"/> 
        ''' function will be used when this parameter is ``Nothing``.
        ''' </param>
        ''' <param name="cacheSize">
        ''' The maximum number of the cached objective vectors; a value which is 
        ''' not greater than zero disables the cache table.
        ''' </param>
        Public Sub New(population As Population(Of Chr),
                       objectives As IMultiObjective(Of Chr),
                       Optional seeds As IRandomSeeds = Nothing,
                       Optional crossover As Func(Of Chr, Chr, Random, IEnumerable(Of Chr)) = Nothing,
                       Optional mutate As Func(Of Chr, Random, Chr) = Nothing,
                       Optional cacheSize As Integer = 0)

            If population Is Nothing Then
                Throw New ArgumentNullException(NameOf(population))
            End If
            If population.Size <= 0 Then
                Throw New ArgumentException("The initial population should contain at least one chromosome.", NameOf(population))
            End If
            If objectives Is Nothing Then
                Throw New ArgumentNullException(NameOf(objectives))
            End If
            If objectives.Objectives <= 0 Then
                Throw New ArgumentException("The multi-objective model should define at least one objective function.", NameOf(objectives))
            End If

            Me.seeds = seeds Or randfSeeds
            Me.currentPopulation = population
            Me.expectedSize = population.Size
            Me.objectiveEvaluator = New ObjectivePool(Of Chr)(objectives, cacheSize)
            Me.crossoverOperator = If(crossover, Function(a, b, rnd) a.Crossover(b))
            Me.mutationOperator = If(mutate, Function(chromosome, rnd) chromosome.Mutate())

            If population.capacitySize <= 0 Then
                population.capacitySize = expectedSize
            End If
        End Sub

        ''' <summary>
        ''' Evaluate all of the objective vectors of the current population and 
        ''' refresh the internal non-dominated ranks and crowding distances.
        ''' </summary>
        ''' <param name="parallel">
        ''' Whether the objective calculation should be run in parallel mode?
        ''' </param>
        Public Sub EvaluateAll(Optional parallel As Boolean = False)
            Dim items As Chr() = currentPopulation.ToArray

            currentObjectives = evaluateAll(items, parallel)

            Call analyze(currentObjectives, currentRanks, currentCrowding)
        End Sub

        ''' <summary>
        ''' Evaluate the objective vector of the given chromosome.
        ''' </summary>
        ''' <param name="chromosome">The candidate chromosome which will be evaluated.</param>
        ''' <returns>The objective vector of the given <paramref name="chromosome"/>.</returns>
        Public Function GetObjectives(chromosome As Chr) As Double()
            Return checkVector(objectiveEvaluator.Evaluate(chromosome, parallel:=False), 0)
        End Function

        ''' <summary>
        ''' Run one generation of the NSGA-II evolution: binary tournament 
        ''' selection, crossover, mutation, parent/offspring merging and the 
        ''' elitism environment selection.
        ''' </summary>
        Public Sub Evolve()
            If currentObjectives Is Nothing OrElse currentObjectives.Length <> currentPopulation.Size Then
                Call EvaluateAll(parallel:=False)
            End If

            Dim offspring As New List(Of Chr)
            Dim rnd As Random = seeds()

            Do While offspring.Count < expectedSize
                Dim parent1 As Chr = tournament(rnd)
                Dim parent2 As Chr = tournament(rnd)
                Dim children As IEnumerable(Of Chr) = crossoverOperator(parent1, parent2, rnd)

                If children Is Nothing Then
                    Throw New InvalidOperationException("The crossover operator returned Nothing.")
                End If

                Dim childList As Chr() = children.ToArray

                If childList.Length = 0 Then
                    ' the crossover did not produce any child, fallback to the 
                    ' mutation of the first parent for avoiding the dead loop
                    Call offspring.Add(mutateCheck(parent1, rnd))
                Else
                    For Each child As Chr In childList
                        If offspring.Count >= expectedSize Then
                            Exit For
                        End If

                        Call offspring.Add(mutateCheck(child, rnd))
                    Next
                End If
            Loop

            Dim merged As New List(Of Chr)(expectedSize * 2)

            Call merged.AddRange(currentPopulation)
            Call merged.AddRange(offspring)

            Dim mergedObjectives As Double()() = evaluateAll(merged, parallel:=False)
            Dim selection As SelectionResult(Of Chr) = EnvironmentalSelection.SelectNextGeneration(merged, mergedObjectives, expectedSize)

            currentPopulation = selection.Population
            currentObjectives = selection.Objectives
            currentRanks = selection.Ranks
            currentCrowding = selection.Crowding
            Generation += 1
        End Sub

        ''' <summary>
        ''' Get the solutions of the current first non-dominated front, which is 
        ''' the approximation of the Pareto front.
        ''' </summary>
        ''' <returns>An array of the Pareto solutions with the rank value ``0``.</returns>
        Public ReadOnly Property ParetoFront As ParetoSolution(Of Chr)()
            Get
                If currentObjectives Is Nothing OrElse currentObjectives.Length <> currentPopulation.Size Then
                    Call EvaluateAll(parallel:=False)
                End If

                Dim fronts As List(Of Integer()) = FastNonDominatedSort(currentObjectives)

                If fronts.Count = 0 Then
                    Return New ParetoSolution(Of Chr)() {}
                End If

                Dim front As Integer() = fronts(0)
                Dim distance As Double() = CrowdingDistance(currentObjectives, front)
                Dim result As ParetoSolution(Of Chr)() = New ParetoSolution(Of Chr)(front.Length - 1) {}

                For i As Integer = 0 To front.Length - 1
                    Dim idx As Integer = front(i)

                    result(i) = New ParetoSolution(Of Chr)(
                        chromosome:=currentPopulation(idx),
                        objectives:=DirectCast(currentObjectives(idx).Clone, Double()),
                        rank:=0,
                        crowdingDistance:=distance(i)
                    )
                Next

                Return result
            End Get
        End Property

        ''' <summary>
        ''' Update the mutation rate of all of the chromosomes in the current 
        ''' population.
        ''' </summary>
        ''' <param name="newRate">The new mutation rate value.</param>
        Public Sub UpdateMutationRate(newRate As Double)
            For i As Integer = 0 To currentPopulation.Size - 1
                currentPopulation(i).MutationRate = newRate
            Next
        End Sub

        ''' <summary>
        ''' Clear the internal objective vector cache.
        ''' </summary>
        Public Sub Clear()
            Dim pool As ObjectivePool(Of Chr) = TryCast(objectiveEvaluator, ObjectivePool(Of Chr))

            If Not pool Is Nothing Then
                Call pool.Clear()
            End If
        End Sub

        ''' <summary>
        ''' Binary tournament selection: two random chromosomes are picked and 
        ''' the better one is selected by the crowding comparison operator.
        ''' </summary>
        Private Function tournament(rnd As Random) As Chr
            Dim i As Integer = rnd.Next(expectedSize)
            Dim j As Integer = rnd.Next(expectedSize)
            Dim better As Integer = If(isBetter(i, j), i, j)

            Return currentPopulation(better)
        End Function

        ''' <summary>
        ''' The crowding comparison operator: the smaller rank is better, and 
        ''' when the ranks are equals to each other the larger crowding distance 
        ''' is better.
        ''' </summary>
        Private Function isBetter(i As Integer, j As Integer) As Boolean
            If currentRanks(i) < currentRanks(j) Then
                Return True
            ElseIf currentRanks(i) > currentRanks(j) Then
                Return False
            Else
                Return currentCrowding(i) > currentCrowding(j)
            End If
        End Function

        Private Function mutateCheck(chromosome As Chr, rnd As Random) As Chr
            Dim mutated As Chr = mutationOperator(chromosome, rnd)

            If mutated Is Nothing Then
                Throw New InvalidOperationException("The mutation operator returned Nothing.")
            End If

            Return mutated
        End Function

        ''' <summary>
        ''' Evaluate the objective vectors of the given chromosomes; the 
        ''' ``parallel`` flag of the objective model is always ``False`` when the 
        ''' evaluation loop itself is parallelized.
        ''' </summary>
        Private Function evaluateAll(chromosomes As IList(Of Chr), parallel As Boolean) As Double()()
            Dim values As Double()() = New Double(chromosomes.Count - 1)() {}

            If parallel Then
                Call System.Threading.Tasks.Parallel.For(0, chromosomes.Count,
                    Sub(i As Integer)
                        values(i) = checkVector(objectiveEvaluator.Evaluate(chromosomes(i), parallel:=False), i)
                    End Sub)
            Else
                For i As Integer = 0 To chromosomes.Count - 1
                    values(i) = checkVector(objectiveEvaluator.Evaluate(chromosomes(i), parallel:=True), i)
                Next
            End If

            Return values
        End Function

        Private Function checkVector(vector As Double(), index As Integer) As Double()
            If vector Is Nothing Then
                Throw New InvalidOperationException($"The objective model returned Nothing for the chromosome at index={index}.")
            End If
            If vector.Length <> objectiveEvaluator.Objectives Then
                Throw New InvalidOperationException(
                    $"The objective model returned a vector with {vector.Length} elements at index={index}, " &
                    $"but it was declared as {objectiveEvaluator.Objectives} objectives."
                )
            End If

            Return vector
        End Function

        ''' <summary>
        ''' Calculate the non-dominated ranks and the crowding distance of all 
        ''' of the solutions.
        ''' </summary>
        Private Shared Sub analyze(objectives As Double()(), ByRef ranks As Integer(), ByRef crowding As Double())
            Dim fronts As List(Of Integer()) = FastNonDominatedSort(objectives)

            ranks = New Integer(objectives.Length - 1) {}
            crowding = New Double(objectives.Length - 1) {}

            For rank As Integer = 0 To fronts.Count - 1
                Dim front As Integer() = fronts(rank)
                Dim distance As Double() = CrowdingDistance(objectives, front)

                For pos As Integer = 0 To front.Length - 1
                    Dim idx As Integer = front(pos)

                    ranks(idx) = rank
                    crowding(idx) = distance(pos)
                Next
            Next
        End Sub
    End Class
End Namespace
