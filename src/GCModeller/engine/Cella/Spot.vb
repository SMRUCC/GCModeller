Imports Microsoft.VisualBasic.Imaging

Public Class Spot

    Public Property external As MetabolicNetwork
    Public Property cells As New List(Of VirtualCella)
    Public Property index As SpatialIndex3D

    Public Sub Tick(iteration As Integer)
        Call external.RunStep()

        For Each cell As VirtualCella In cells
            Call cell.RunStep(iteration)
        Next
    End Sub

End Class
