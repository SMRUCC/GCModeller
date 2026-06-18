Public MustInherit Class SubNetwork

    Protected cell As VirtualCella

    Sub New(cell As VirtualCella)
        Me.cell = cell
    End Sub

    Public MustOverride Function GetStats() As Dictionary(Of String, Double)
    Public MustOverride Sub RunStep()

End Class
