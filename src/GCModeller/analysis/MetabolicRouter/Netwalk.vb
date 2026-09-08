Imports SMRUCC.genomics.Analysis.RetroPath.Chem
Imports SMRUCC.genomics.Analysis.RetroPath.Search

Public Class Netwalk

    Public ReadOnly Property opts As SearchOptions
    Public ReadOnly Property w As ScoreWeights
    Public ReadOnly Property rules As List(Of Rule)
    Public ReadOnly Property sink As List(Of Tuple(Of String, String))

    Sub New(rules As List(Of Rule), sink As List(Of Tuple(Of String, String)), opts As SearchOptions, w As ScoreWeights)
        _rules = rules
        _w = w
        _opts = opts
        _sink = sink
    End Sub

    Public Function Search(targetSmiles As String)
        ' 解析目标与汇
        Dim target = SmilesIO.Parse(targetSmiles)
        Dim sinkKeys As New HashSet(Of String)()
        For Each s In sink
            sinkKeys.Add(SmilesIO.Parse(s.Item2).MolKey())
        Next
        Dim currencyKeys As New HashSet(Of String)()
        For Each cs In RuleLibrary.CurrencySmiles()
            currencyKeys.Add(SmilesIO.Parse(cs).MolKey())
        Next
    End Function
End Class
