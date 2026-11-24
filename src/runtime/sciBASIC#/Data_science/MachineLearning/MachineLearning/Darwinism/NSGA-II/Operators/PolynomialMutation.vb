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

Namespace Darwinism.NSGA2

    ''' <summary>
    ''' The polynomial mutation operator of the real coded genetic algorithm.
    ''' </summary>
    ''' <remarks>
    ''' The mutation probability of each gene is ``1/D`` by default, where ``D`` 
    ''' is the number of the decision variables.
    ''' </remarks>
    Public Partial Module RealCodedOperators

        ''' <summary>
        ''' Apply the polynomial mutation on one gene value.
        ''' </summary>
        ''' <param name="value">The original gene value.</param>
        ''' <param name="rnd">The random number generator.</param>
        ''' <param name="lower">The lower bound of the decision variable.</param>
        ''' <param name="upper">The upper bound of the decision variable.</param>
        ''' <param name="etaM">
        ''' The distribution index of the polynomial mutation, a larger value 
        ''' makes the mutation closer to the original gene value; the common 
        ''' value is ``15`` to ``30``.
        ''' </param>
        ''' <returns>The mutated gene value which is always clamped into 
        ''' ``[lower, upper]``.</returns>
        Public Function MutateGene(value As Double, rnd As Random,
                                   lower As Double, upper As Double,
                                   Optional etaM As Double = 20) As Double

            Dim range As Double = upper - lower

            If range <= 0 Then
                Return value
            End If

            Dim y As Double = clamp(value, lower, upper)
            Dim delta1 As Double = (y - lower) / range
            Dim delta2 As Double = (upper - y) / range
            Dim rand As Double = rnd.NextDouble()
            Dim exponent As Double = 1 / (etaM + 1)
            Dim deltaq As Double

            If rand <= 0.5 Then
                Dim xy As Double = 1 - delta1
                Dim val As Double = 2 * rand + (1 - 2 * rand) * (xy ^ (etaM + 1))

                deltaq = val ^ exponent - 1
            Else
                Dim xy As Double = 1 - delta2
                Dim val As Double = 2 * (1 - rand) + 2 * (rand - 0.5) * (xy ^ (etaM + 1))

                deltaq = 1 - val ^ exponent
            End If

            Return clamp(y + deltaq * range, lower, upper)
        End Function

        ''' <summary>
        ''' Apply the polynomial mutation on a real valued chromosome.
        ''' </summary>
        ''' <typeparam name="Chr">The real valued chromosome type.</typeparam>
        ''' <param name="chromosome">The parent chromosome.</param>
        ''' <param name="rnd">The random number generator.</param>
        ''' <param name="lower">The lower bound of each decision variable.</param>
        ''' <param name="upper">The upper bound of each decision variable.</param>
        ''' <param name="rate">
        ''' The mutation probability of each gene; a value which is not in the 
        ''' ``(0, 1]`` interval will fallback to the 
        ''' <see cref="Darwinism.Models.Chromosome(Of Chr).MutationRate"/> value, 
        ''' and when the mutation rate is still invalid the ``1/D`` value will be 
        ''' used finally.
        ''' </param>
        ''' <param name="etaM">The distribution index of the polynomial mutation.</param>
        ''' <returns>A new mutated chromosome.</returns>
        <Extension>
        Public Function PolynomialMutation(Of Chr As {Class, IRealVector(Of Chr)})(
                                               chromosome As Chr,
                                               rnd As Random,
                                               lower As Double(),
                                               upper As Double(),
                                               Optional rate As Double = -1,
                                               Optional etaM As Double = 20) As Chr

            If chromosome Is Nothing Then
                Throw New ArgumentNullException(NameOf(chromosome))
            End If

            Dim genes As Double() = chromosome.Genes

            If lower.Length <> genes.Length OrElse upper.Length <> genes.Length Then
                Throw New ArgumentException("The size of the search boundaries should be equals to the size of the genes.")
            End If

            Dim pm As Double = If(rate > 0 AndAlso rate <= 1, rate, chromosome.MutationRate)

            If pm <= 0 OrElse pm > 1 Then
                pm = 1 / genes.Length
            End If

            Dim mutated As Double() = DirectCast(genes.Clone, Double())

            For i As Integer = 0 To mutated.Length - 1
                If rnd.NextDouble() < pm Then
                    mutated(i) = MutateGene(mutated(i), rnd, lower(i), upper(i), etaM)
                End If
            Next

            Return chromosome.Create(mutated)
        End Function
    End Module
End Namespace
