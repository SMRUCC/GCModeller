Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text

Public Class DataSetDriver : Implements ISaveHandle

    ReadOnly cache As New List(Of DataSet)

    Sub New()
    End Sub

    Public Sub SnapshotDriver(iteration%, data As Dictionary(Of String, Double))
        Call New DataSet With {.ID = iteration, .Properties = data}.DoCall(AddressOf cache.Add)
    End Sub

    Public Function Save(path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
        Return cache.SaveTo(path, encoding:=encoding)
    End Function

    Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
        Return Save(path, encoding.CodePage)
    End Function
End Class
