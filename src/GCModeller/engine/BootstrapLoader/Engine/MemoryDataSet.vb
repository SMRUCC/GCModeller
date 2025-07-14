Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine

Public Class MemoryDataSet : Implements IOmicsDataAdapter

    Public ReadOnly Property mass As OmicsTuple(Of String()) Implements IOmicsDataAdapter.mass

    Dim massRows As New List(Of Dictionary(Of String, Double))
    Dim fluxRows As New List(Of Dictionary(Of String, Double))

    Public Sub MassSnapshot(iteration As Integer, data As Dictionary(Of String, Double)) Implements IOmicsDataAdapter.MassSnapshot
        massRows.Add(data)
    End Sub

    Public Sub FluxSnapshot(iteration As Integer, data As Dictionary(Of String, Double)) Implements IOmicsDataAdapter.FluxSnapshot
        fluxRows.Add(data)
    End Sub

    Public Function getMassDataSet() As ICollection(Of Dictionary(Of String, Double))
        Return massRows
    End Function

    Public Function getFluxDataSet() As ICollection(Of Dictionary(Of String, Double))
        Return fluxRows
    End Function
End Class
