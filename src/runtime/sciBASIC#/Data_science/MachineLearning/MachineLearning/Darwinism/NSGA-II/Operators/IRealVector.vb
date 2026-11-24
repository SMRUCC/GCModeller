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

Imports Microsoft.VisualBasic.MachineLearning.Darwinism.Models

Namespace Darwinism.NSGA2

    ''' <summary>
    ''' A real valued chromosome which exposes its genes as a indexable real 
    ''' valued vector, so that the real coded genetic operators such as the 
    ''' SBX crossover and the polynomial mutation can be applied on it.
    ''' </summary>
    ''' <typeparam name="Chr">The chromosome type of the genetic algorithm.</typeparam>
    ''' <remarks>
    ''' The <see cref="Genes"/> property should return the gene vector which is 
    ''' used to build the current chromosome; the <see cref="Create(Double())"/> 
    ''' function is used by the genetic operators for creating the child 
    ''' chromosomes and it should not modify the parent chromosome.
    ''' </remarks>
    Public Interface IRealVector(Of Chr As {Class, Chromosome(Of Chr)})
        Inherits Chromosome(Of Chr)

        ''' <summary>
        ''' The gene values of this chromosome, one value for each decision 
        ''' variable.
        ''' </summary>
        ''' <returns>A <see cref="Double"/> vector.</returns>
        ReadOnly Property Genes As Double()

        ''' <summary>
        ''' Create a new chromosome from the given gene vector.
        ''' </summary>
        ''' <param name="genes">
        ''' The gene values of the new chromosome, the length of this vector 
        ''' should be equals to the length of the <see cref="Genes"/> vector.
        ''' </param>
        ''' <returns>A new chromosome object.</returns>
        Function Create(genes As Double()) As Chr
    End Interface
End Namespace
