Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Module test

    Sub Main()
        Dim data = "E:\2018-7-10\高粱对比筛选结果.csv".LoadCsv(Of align)

    End Sub
End Module

Public Class align

    Public Property Qseqid As String
    Public Property Sseid As String
    Public Property indent As Double
    Public Property mismatch As Double
    Public Property qlength As Double
    Public Property length As Double
    Public Property Si As Double
    Public Property qstart As Integer
    Public Property qend As Integer
    Public Property sstart As Integer
    Public Property send As Integer
    Public Property evlaue As Double

    <Column("bit-score")>
    Public Property bit_score As Double

End Class