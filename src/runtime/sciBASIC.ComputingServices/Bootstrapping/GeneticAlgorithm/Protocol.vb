Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Bootstrapping.Darwinism.GAF
Imports Microsoft.VisualBasic.DataMining.Darwinism.GAF
Imports Microsoft.VisualBasic.Mathematical.Calculus
Imports Microsoft.VisualBasic.Scripting.MetaData

Public Module Protocol

    <Extension>
    Public Function GetFitness(v As ParameterVector, model As TypeInfo, observation As ODEsOut, ynames$(), y0 As Dictionary(Of String, Double), n%, t0#, tt#) As Double
        Return model.GetType.GetFitness(v, observation, ynames, y0, n, t0, tt, False, Nothing)
    End Function

    Public Function GA_PLinq(GA As GeneticAlgorithm(Of ParameterVector), source As NamedValue(Of ParameterVector)()) As IEnumerable(Of NamedValue(Of Double))
        Dim fitness As GAFFitness = DirectCast(GA.Fitness, GAFFitness)
        Dim model As New TypeInfo(fitness.Model)
        Dim task As Func(Of ParameterVector, TypeInfo, ODEsOut, String(), Dictionary(Of String, Double), Integer, Double, Double, Double) =
            AddressOf GetFitness
        Dim observation As ODEsOut = fitness.observation
        Return source.AsDistributed(Of NamedValue(Of Double))(
            task, model,
            observation,
            fitness.modelVariables,
            fitness.y0,
            observation.x.Length,
            observation.x(0),
            observation.x.Last)
    End Function
End Module
