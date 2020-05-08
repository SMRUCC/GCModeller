Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

Namespace Engine

    Public Class FluxAggregater

        ReadOnly fluxDynamicsCache As IGrouping(Of String, var)()

        Sub New(core As Vessel)
            fluxDynamicsCache = core.m_dynamics _
                .Select(Function(m) m.getLastFluxVariants) _
                .IteratesALL _
                .GroupBy(Function(a) a.Name) _
                .ToArray
        End Sub

        Public Function getFlux() As Dictionary(Of String, Double)
            Return fluxDynamicsCache _
                .ToDictionary(Function(a) a.Key,
                                Function(a)
                                    Return Aggregate x In a Into Sum(x.Value)
                                End Function)
        End Function
    End Class
End Namespace