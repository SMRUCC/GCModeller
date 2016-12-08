Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Bootstrapping.Darwinism.GAF
Imports Microsoft.VisualBasic.Mathematical.Calculus
Imports Microsoft.VisualBasic.Scripting.MetaData

Public Module Protocol

    <Extension>
    Public Function GetFitness(model As TypeInfo, v As ParameterVector, observation As ODEsOut, ynames$(), y0 As Dictionary(Of String, Double), n%, t0#, tt#) As Double
        Return model.GetType.GetFitness(v, observation, ynames, y0, n, t0, tt, False, Nothing)
    End Function
End Module
