Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Public Class toGO

    Public Property entry As String
    Public Property name As String
    Public Property map2GO_term As String
    Public Property map2GO_id As String

    Public Shared Iterator Function Parse2GO(file As String) As IEnumerable(Of toGO)
        Dim lines As String() = file.SolveStream _
            .LineTokens _
            .SkipWhile(Function(line) line.First = "!"c) _
            .ToArray
        Dim tokens$()
        Dim from As NamedValue(Of String)
        Dim mapTo As NamedValue(Of String)
        Dim mapping As toGO

        For Each line As String In lines
            tokens = line.Split(">"c)
            from = tokens(0).GetTagValue(" ", trim:=True)
            mapTo = tokens(1).GetTagValue(";", trim:=True)
            mapping = New toGO With {
                .entry = from.Name,
                .name = from.Value,
                .map2GO_term = mapTo.Name,
                .map2GO_id = mapTo.Value
            }

            Yield mapping
        Next
    End Function

End Class
