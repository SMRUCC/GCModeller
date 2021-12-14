Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Language

Public Class MothurRankTree : Inherits Tree(Of MothurData)

    Public Function GetOTUTable() As OTUTable()

    End Function

    Public Shared Function LoadTaxonomySummary(file As String) As MothurRankTree
        Dim rows As MothurData() = file.LoadTsv(Of MothurData).ToArray
        Dim i As i32 = Scan0
        Dim root As New MothurRankTree With {
            .Data = rows(Scan0),
            .ID = ++i,
            .label = .Data.rankID,
            .Childs = New Dictionary(Of String, Tree(Of MothurData))
        }
        Dim taxonNode As MothurRankTree = root
        Dim parent As New Dictionary(Of String, MothurRankTree)

        Call parent.Add(root.label, root)

        For Each row As MothurData In rows.Skip(1)
            taxonNode = New MothurRankTree With {
                .Childs = New Dictionary(Of String, Tree(Of MothurData)),
                .Data = row,
                .ID = ++i,
                .label = row.rankID
            }
            taxonNode.Parent = parent(row.parentID).Add(taxonNode)
        Next

        Return root
    End Function

End Class

Public Class MothurData

    Public Property taxlevel As Integer
    Public Property rankID As String
    Public Property taxon As String
    Public Property daughterlevels As Integer
    Public Property total As Integer
    Public Property samples As Dictionary(Of String, Integer)

    Public ReadOnly Property parentID As String
        Get
            Dim t As String() = rankID.Split("."c)
            Dim parent As String = t.Take(t.Length - 1).JoinBy(".")

            Return parent
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"[{rankID}] ({total}){taxon}"
    End Function

End Class