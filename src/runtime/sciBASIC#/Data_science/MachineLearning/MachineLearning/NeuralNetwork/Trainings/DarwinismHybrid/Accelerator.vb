#Region "Microsoft.VisualBasic::3975243c6f0d47281f422fc907d6cdca, Data_science\MachineLearning\MachineLearning\NeuralNetwork\Trainings\Accelerator.vb"

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

    '     Module GAExtensions
    ' 
    '         Function: GetSynapseGroups
    ' 
    '         Sub: doPrint, RunGAAccelerator
    ' 
    '     Class WeightVector
    ' 
    '         Properties: MutationRate, UniqueHashKey
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Clone, Crossover, Mutate, ToString
    ' 
    '     Class Fitness
    ' 
    '         Properties: Cacheable
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Calculate
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.GAF
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.GAF.Helper
Imports Microsoft.VisualBasic.MachineLearning.Darwinism.Models
Imports Microsoft.VisualBasic.MachineLearning.StoreProcedure
Imports Microsoft.VisualBasic.SecurityString

Namespace NeuralNetwork.DarwinismHybrid

    ''' <summary>
    ''' 
    ''' </summary>
    Public Module Accelerator

        <Extension>
        Public Function GetSynapseGroups(network As Network) As NamedCollection(Of Synapse)()
            Return network.PopulateAllSynapses _
                .GroupBy(Function(s) s.ToString) _
                .Select(Function(sg)
                            Return New NamedCollection(Of Synapse)(sg.Key, sg.ToArray)
                        End Function) _
                .ToArray
        End Function

        <Extension>
        Public Sub RunGAAccelerator(network As Network, trainingSet As Sample(), Optional populationSize% = 1000, Optional iterations% = 10000)
            Dim synapses = network.GetSynapseGroups
            Dim population As Population(Of WeightVector) = New WeightVector(synapses).InitialPopulation(New Population(Of WeightVector)(New PopulationList(Of WeightVector), parallel:=True) With {.capacitySize = populationSize})
            Dim fitness As Fitness(Of WeightVector) = New Fitness(network, synapses, trainingSet)
            Dim ga As New GeneticAlgorithm(Of WeightVector)(population, fitness)
            Dim engine As New EnvironmentDriver(Of WeightVector)(ga, Sub(null, nullErr)
                                                                         ' do nothing
                                                                     End Sub) With {
                .Iterations = iterations,
                .Threshold = 0.005
            }

            Call "Run GA helper!".__DEBUG_ECHO
            Call engine.AttachReporter(AddressOf doPrint)
            Call engine.Train()
        End Sub

        Private Sub doPrint(i%, e#, g As GeneticAlgorithm(Of WeightVector))
            Call EnvironmentDriver(Of WeightVector).CreateReport(i, e, g).ToString.__DEBUG_ECHO
        End Sub
    End Module
End Namespace
