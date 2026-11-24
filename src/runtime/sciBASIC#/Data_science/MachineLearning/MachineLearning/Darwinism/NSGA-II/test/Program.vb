Imports System.Linq
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.GAF.Population
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.Models
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.NSGA2
Imports std = System.Math

Namespace NSGA2_test

    ''' <summary>
    ''' The verification program of the NSGA-II framework: it runs the ZDT1 
    ''' bi-objective benchmark problem and checks that the obtained first front 
    ''' is non-dominated and is close enough to the analytical Pareto front.
    ''' </summary>
    Module Program

        Const PopulationSize As Integer = 100
        Const Generations As Integer = 250
        Const GenerationDistanceThreshold As Double = 0.01

        Sub Main()
            Environment.Exit(RunTest())
        End Sub

        Private Function RunTest() As Integer
            Dim rnd As New Random(12345)
            Dim lower As Double() = Zdt1Chromosome.LowerBound
            Dim upper As Double() = Zdt1Chromosome.UpperBound

            Dim initialPopulation As Population(Of Zdt1Chromosome) =
                New Population(Of Zdt1Chromosome)(New PopulationList(Of Zdt1Chromosome), parallel:=False) With {
                    .capacitySize = PopulationSize
                }

            For i As Integer = 1 To PopulationSize
                Call initialPopulation.Add(Zdt1Chromosome.RandomCreate(rnd))
            Next

            Dim engine As New NSGA2Engine(Of Zdt1Chromosome)(
                population:=initialPopulation,
                objectives:=New Zdt1Objectives(),
                seeds:=Function() rnd,
                crossover:=Function(a, b, rand) a.SBXCrossover(b, rand, lower, upper, pc:=0.9, etaC:=20),
                mutate:=Function(chromosome, rand) chromosome.PolynomialMutation(rand, lower, upper, rate:=1.0 / Zdt1Chromosome.Dimensions, etaM:=20),
                cacheSize:=1000
            )
            Dim started As DateTime = DateTime.Now
            Dim driver As New NSGA2Driver(Of Zdt1Chromosome)(
                model:=engine,
                takeSnapshot:=Sub(i As Integer, model As NSGA2Engine(Of Zdt1Chromosome))
                                  If i Mod 25 = 0 OrElse i = Generations Then
                                      Console.WriteLine($"generation {i,3}: front size={model.ParetoFront.Length}")
                                  End If
                              End Sub,
                generations:=Generations
            )

            Call driver.Train()

            Dim front As ParetoSolution(Of Zdt1Chromosome)() = engine.ParetoFront

            If front.Length = 0 Then
                Console.WriteLine("[FAIL] the Pareto front is empty!")
                Return 1
            End If

            Dim dominatedPairs As Integer = 0

            For i As Integer = 0 To front.Length - 1
                For j As Integer = 0 To front.Length - 1
                    If i <> j AndAlso front(i).Objectives.Dominates(front(j).Objectives) Then
                        dominatedPairs += 1
                    End If
                Next
            Next

            Dim gd As Double = GenerationalDistance(front)
            Dim spacingValue As Double = Spacing(front)
            Dim f1Values As Double() = front.Select(Function(solution) solution.Objectives(0)).ToArray()

            Console.WriteLine()
            Console.WriteLine("========== NSGA-II / ZDT1 verification ==========")
            Console.WriteLine($"elapsed          : {(DateTime.Now - started).TotalSeconds:F2} s")
            Console.WriteLine($"population size  : {PopulationSize}")
            Console.WriteLine($"generations      : {Generations}")
            Console.WriteLine($"front size       : {front.Length}")
            Console.WriteLine($"dominated pairs  : {dominatedPairs}")
            Console.WriteLine($"f1 range         : [{f1Values.Min():F6}, {f1Values.Max():F6}]")
            Console.WriteLine($"generational dist: {gd:F8} (threshold {GenerationDistanceThreshold})")
            Console.WriteLine($"spacing          : {spacingValue:F8}")

            Dim passed As Boolean = dominatedPairs = 0 AndAlso gd <= GenerationDistanceThreshold AndAlso f1Values.Min() <= 0.02 AndAlso f1Values.Max() >= 0.98

            Console.WriteLine($"result           : {If(passed, "PASS", "FAIL")}")

            Return If(passed, 0, 1)
        End Function

        ''' <summary>
        ''' The mean distance from the obtained solutions to the analytical Pareto 
        ''' front of the ZDT1 problem.
        ''' </summary>
        Private Function GenerationalDistance(front As ParetoSolution(Of Zdt1Chromosome)()) As Double
            Dim total As Double = 0

            For Each solution As ParetoSolution(Of Zdt1Chromosome) In front
                Dim minDistance As Double = Double.MaxValue

                For k As Integer = 0 To 1000
                    Dim f1 As Double = k / 1000.0
                    Dim f2 As Double = 1 - std.Sqrt(f1)
                    Dim dx As Double = solution.Objectives(0) - f1
                    Dim dy As Double = solution.Objectives(1) - f2
                    Dim distance As Double = std.Sqrt(dx * dx + dy * dy)

                    If distance < minDistance Then
                        minDistance = distance
                    End If
                Next

                total += minDistance * minDistance
            Next

            Return std.Sqrt(total / front.Length)
        End Function

        ''' <summary>
        ''' The spacing metric which describes how evenly the solutions are 
        ''' distributed along the front; a smaller value means a better 
        ''' distribution.
        ''' </summary>
        Private Function Spacing(front As ParetoSolution(Of Zdt1Chromosome)()) As Double
            If front.Length < 3 Then
                Return Double.NaN
            End If

            Dim sorted As ParetoSolution(Of Zdt1Chromosome)() = front _
                .OrderBy(Function(solution) solution.Objectives(0)) _
                .ToArray
            Dim distances As Double() = New Double(sorted.Length - 2) {}

            For i As Integer = 0 To distances.Length - 1
                distances(i) = std.Abs(sorted(i + 1).Objectives(0) - sorted(i).Objectives(0)) +
                               std.Abs(sorted(i + 1).Objectives(1) - sorted(i).Objectives(1))
            Next

            Dim mean As Double = distances.Average()
            Dim variance As Double = distances.Sum(Function(distance) (distance - mean) ^ 2)

            Return std.Sqrt(variance / (distances.Length - 1))
        End Function
    End Module
End Namespace
