Imports System.Linq
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.Models
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.NSGA2
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions
Imports std = System.Math

Namespace NSGA2_test

    ''' <summary>
    ''' The ZDT1 test problem chromosome: a real coded vector which implements 
    ''' the <see cref="IRealVector(Of Chr)"/> interface, so that the built-in 
    ''' SBX crossover and the polynomial mutation operators can be applied on it.
    ''' </summary>
    Public Class Zdt1Chromosome
        Implements IRealVector(Of Zdt1Chromosome)

        ''' <summary>
        ''' The number of the decision variables of the ZDT1 problem.
        ''' </summary>
        Public Const Dimensions As Integer = 30

        ''' <summary>
        ''' The lower bound of each decision variable.
        ''' </summary>
        Public Shared ReadOnly LowerBound As Double() = Enumerable.Repeat(0.0, Dimensions).ToArray

        ''' <summary>
        ''' The upper bound of each decision variable.
        ''' </summary>
        Public Shared ReadOnly UpperBound As Double() = Enumerable.Repeat(1.0, Dimensions).ToArray

        ReadOnly values As Double()
        Dim rate As Double = 0.1

        ''' <summary>
        ''' Create a ZDT1 chromosome from the given gene vector.
        ''' </summary>
        ''' <param name="values">The decision variable values.</param>
        Public Sub New(values As Double())
            If values Is Nothing Then
                Throw New ArgumentNullException(NameOf(values))
            End If
            If values.Length <> Dimensions Then
                Throw New ArgumentException($"The ZDT1 problem requires {Dimensions} decision variables.", NameOf(values))
            End If

            Me.values = DirectCast(values.Clone, Double())
        End Sub

        ''' <summary>
        ''' The decision variable values of this chromosome.
        ''' </summary>
        ''' <returns>A <see cref="Double"/> vector.</returns>
        Public ReadOnly Property Genes As Double() Implements IRealVector(Of Zdt1Chromosome).Genes
            Get
                Return values
            End Get
        End Property

        ''' <summary>
        ''' The mutation probability of each gene.
        ''' </summary>
        ''' <returns>A <see cref="Double"/> value.</returns>
        Public Property MutationRate As Double Implements Chromosome(Of Zdt1Chromosome).MutationRate
            Get
                Return rate
            End Get
            Set(value As Double)
                rate = value
            End Set
        End Property

        ''' <summary>
        ''' The unique identity of the current gene vector, it is used as the 
        ''' cache key of the objective vector.
        ''' </summary>
        ''' <returns>A string value.</returns>
        Public ReadOnly Property Identity As String Implements IReadOnlyId.Identity
            Get
                Return String.Join(",", values)
            End Get
        End Property

        ''' <summary>
        ''' Create a new chromosome from the given gene vector.
        ''' </summary>
        ''' <param name="values">The gene values of the new chromosome.</param>
        ''' <returns>A new <see cref="Zdt1Chromosome"/> object.</returns>
        Public Function Create(values As Double()) As Zdt1Chromosome Implements IRealVector(Of Zdt1Chromosome).Create
            Return New Zdt1Chromosome(values) With {
                .MutationRate = rate
            }
        End Function

        ''' <summary>
        ''' Apply the SBX crossover with the shared random number generator.
        ''' </summary>
        ''' <param name="another">The another parent chromosome.</param>
        ''' <returns>The two child chromosomes.</returns>
        Public Function Crossover(another As Zdt1Chromosome) As IEnumerable(Of Zdt1Chromosome) Implements Chromosome(Of Zdt1Chromosome).Crossover
            Return Me.SBXCrossover(another, randf.seeds, LowerBound, UpperBound, pc:=0.9, etaC:=20)
        End Function

        ''' <summary>
        ''' Apply the polynomial mutation with the shared random number generator.
        ''' </summary>
        ''' <returns>A new mutated chromosome.</returns>
        Public Function Mutate() As Zdt1Chromosome Implements Chromosome(Of Zdt1Chromosome).Mutate
            Return Me.PolynomialMutation(randf.seeds, LowerBound, UpperBound, rate, etaM:=20)
        End Function

        ''' <summary>
        ''' Create a randomized chromosome which is uniformly distributed in the 
        ''' search space.
        ''' </summary>
        ''' <param name="rnd">The random number generator.</param>
        ''' <returns>A new randomized chromosome.</returns>
        Public Shared Function RandomCreate(rnd As Random) As Zdt1Chromosome
            Dim values As Double() = New Double(Dimensions - 1) {}

            For i As Integer = 0 To values.Length - 1
                values(i) = rnd.NextDouble()
            Next

            Return New Zdt1Chromosome(values) With {
                .MutationRate = 0.1
            }
        End Function
    End Class

    ''' <summary>
    ''' The ZDT1 bi-objective test problem:
    ''' 
    ''' ```
    ''' f1(x) = x1
    ''' g(x)  = 1 + 9 * sum(x2..xD) / (D - 1)
    ''' f2(x) = g(x) * (1 - sqrt(f1 / g(x)))
    ''' ```
    ''' 
    ''' The true Pareto front is ``f2 = 1 - sqrt(f1)`` for ``f1`` in ``[0, 1]``.
    ''' </summary>
    Public Class Zdt1Objectives : Implements IMultiObjective(Of Zdt1Chromosome)

        ''' <summary>
        ''' The objective values of the ZDT1 problem are computed from the gene 
        ''' values directly, so the result can be cached by the chromosome 
        ''' identity.
        ''' </summary>
        ''' <returns>Always returns ``True``.</returns>
        Public ReadOnly Property Cacheable As Boolean = True Implements IMultiObjective(Of Zdt1Chromosome).Cacheable

        ''' <summary>
        ''' The ZDT1 problem has two objectives.
        ''' </summary>
        ''' <returns>Always returns ``2``.</returns>
        Public ReadOnly Property Objectives As Integer = 2 Implements IMultiObjective(Of Zdt1Chromosome).Objectives

        ''' <summary>
        ''' Evaluate the objective vector of the given ZDT1 chromosome.
        ''' </summary>
        ''' <param name="chromosome">The candidate chromosome.</param>
        ''' <param name="parallel">This parameter is ignored by the ZDT1 problem.</param>
        ''' <returns>The ``(f1, f2)`` objective vector.</returns>
        Public Function Evaluate(chromosome As Zdt1Chromosome, parallel As Boolean) As Double() Implements IMultiObjective(Of Zdt1Chromosome).Evaluate
            Dim x As Double() = chromosome.Genes
            Dim f1 As Double = x(0)
            Dim sum As Double = 0

            For i As Integer = 1 To x.Length - 1
                sum += x(i)
            Next

            Dim g As Double = 1 + 9 * sum / (x.Length - 1)
            Dim f2 As Double = g * (1 - std.Sqrt(f1 / g))

            Return {f1, f2}
        End Function
    End Class
End Namespace
