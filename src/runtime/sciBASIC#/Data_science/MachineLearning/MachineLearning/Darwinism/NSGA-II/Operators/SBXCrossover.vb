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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.Models

Namespace Darwinism.NSGA2

    ''' <summary>
    ''' The real coded genetic operators which are recommended by the NSGA-II: 
    ''' the simulated binary crossover (SBX) and the polynomial mutation.
    ''' </summary>
    ''' <remarks>
    ''' The gene level functions work on the raw <see cref="Double"/> vectors 
    ''' with the search boundaries, so that they can also be reused by the 
    ''' custom encodings which are not based on the <see cref="IRealVector(Of Chr)"/> 
    ''' interface.
    ''' </remarks>
    Public Partial Module RealCodedOperators

        Const EPSILON As Double = 0.00000000000001

        ''' <summary>
        ''' Apply the simulated binary crossover (SBX) on one pair of genes.
        ''' </summary>
        ''' <param name="g1">The first gene value of the first parent.</param>
        ''' <param name="g2">The target gene value of the second parent.</param>
        ''' <param name="rnd">The random number generator.</param>
        ''' <param name="lower">The lower bound of the decision variable.</param>
        ''' <param name="upper">The upper bound of the decision variable.</param>
        ''' <param name="etaC">
        ''' The distribution index of the SBX crossover, a larger value makes 
        ''' the child genes closer to the parent genes; the common value is 
        ''' ``10`` to ``20``.
        ''' </param>
        ''' <returns>The gene values of the two child chromosomes.</returns>
        Public Function CrossoverGene(g1 As Double, g2 As Double, rnd As Random,
                                      lower As Double, upper As Double,
                                      Optional etaC As Double = 20) As Double()

            Dim range As Double = upper - lower

            If range <= 0 Then
                Return {g1, g2}
            End If

            Dim y1 As Double = Math.Min(g1, g2)
            Dim y2 As Double = Math.Max(g1, g2)

            If y2 - y1 <= EPSILON Then
                Return {g1, g2}
            End If

            Dim rand As Double = rnd.NextDouble()
            Dim exponent As Double = 1 / (etaC + 1)

            ' the first child
            Dim beta As Double = 1 + 2 * (y1 - lower) / (y2 - y1)
            Dim alpha As Double = 2 - beta ^ (-(etaC + 1))
            Dim betaq As Double = If(rand <= 1 / alpha,
                                     (rand * alpha) ^ exponent,
                                     (1 / (2 - rand * alpha)) ^ exponent)
            Dim child1 As Double = 0.5 * ((y1 + y2) - betaq * (y2 - y1))

            ' the second child
            Dim beta2 As Double = 1 + 2 * (upper - y2) / (y2 - y1)
            Dim alpha2 As Double = 2 - beta2 ^ (-(etaC + 1))
            Dim betaq2 As Double = If(rand <= 1 / alpha2,
                                      (rand * alpha2) ^ exponent,
                                      (1 / (2 - rand * alpha2)) ^ exponent)
            Dim child2 As Double = 0.5 * ((y1 + y2) + betaq2 * (y2 - y1))

            If rnd.NextDouble() <= 0.5 Then
                Dim tmp As Double = child1

                child1 = child2
                child2 = tmp
            End If

            Return {
                clamp(child1, lower, upper),
                clamp(child2, lower, upper)
            }
        End Function

        ''' <summary>
        ''' Apply the simulated binary crossover (SBX) on two real valued 
        ''' chromosomes.
        ''' </summary>
        ''' <typeparam name="Chr">The real valued chromosome type.</typeparam>
        ''' <param name="parent1">The first parent chromosome.</param>
        ''' <param name="parent2">The second parent chromosome.</param>
        ''' <param name="rnd">The random number generator.</param>
        ''' <param name="lower">The lower bound of each decision variable.</param>
        ''' <param name="upper">The upper bound of each decision variable.</param>
        ''' <param name="pc">
        ''' The crossover probability; when the crossover was skipped the parent 
        ''' chromosomes will be returned directly, the common value is ``0.9``.
        ''' </param>
        ''' <param name="etaC">The distribution index of the SBX crossover.</param>
        ''' <returns>The two child chromosomes.</returns>
        <Extension>
        Public Function SBXCrossover(Of Chr As {Class, IRealVector(Of Chr)})(
                                         parent1 As Chr,
                                         parent2 As Chr,
                                         rnd As Random,
                                         lower As Double(),
                                         upper As Double(),
                                         Optional pc As Double = 0.9,
                                         Optional etaC As Double = 20) As Chr()

            If parent1 Is Nothing Then
                Throw New ArgumentNullException(NameOf(parent1))
            End If
            If parent2 Is Nothing Then
                Throw New ArgumentNullException(NameOf(parent2))
            End If

            Dim g1 As Double() = parent1.Genes
            Dim g2 As Double() = parent2.Genes

            If g1.Length <> g2.Length Then
                Throw New ArgumentException($"The size of the genes of the parents should be equals to each other: {g1.Length} vs {g2.Length}.")
            End If
            If lower.Length <> g1.Length OrElse upper.Length <> g1.Length Then
                Throw New ArgumentException("The size of the search boundaries should be equals to the size of the genes.")
            End If

            If rnd.NextDouble() > pc Then
                Return {parent1, parent2}
            End If

            Dim child1 As Double() = New Double(g1.Length - 1) {}
            Dim child2 As Double() = New Double(g1.Length - 1) {}

            For i As Integer = 0 To g1.Length - 1
                Dim genes As Double() = CrossoverGene(g1(i), g2(i), rnd, lower(i), upper(i), etaC)

                child1(i) = genes(0)
                child2(i) = genes(1)
            Next

            Return {parent1.Create(child1), parent2.Create(child2)}
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function clamp(value As Double, lower As Double, upper As Double) As Double
            If value < lower Then
                Return lower
            ElseIf value > upper Then
                Return upper
            Else
                Return value
            End If
        End Function
    End Module
End Namespace
