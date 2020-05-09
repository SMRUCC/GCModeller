Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq

Public Class DataSetDriver

    ReadOnly cache As New List(Of DataSet)

    Sub New()
    End Sub

    Public Sub SnapshotDriver(iteration%, data As Dictionary(Of String, Double))
        Call New DataSet With {.ID = iteration, .Properties = data}.DoCall(AddressOf cache.Add)
    End Sub

    Public Function Save(output As String) As Boolean
        Return cache.SaveTo(output)
    End Function
End Class
