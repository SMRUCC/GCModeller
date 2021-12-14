Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.GraphTheory

Public Class MothurRankTree : Inherits Tree(Of MothurData)

    Public Function GetOTUTable() As OTUTable()

    End Function

    Public Shared Function LoadTaxonomySummary(file As String) As MothurRankTree
        Dim rows As MothurData() = file.LoadTsv(Of MothurData).ToArray

    End Function

End Class

Public Class MothurData

    Public Property taxlevel As Integer
    Public Property rankID As String
    Public Property taxon As String
    Public Property daughterlevels As Integer
    Public Property total As Integer
    Public Property samples As Dictionary(Of String, Integer)

End Class