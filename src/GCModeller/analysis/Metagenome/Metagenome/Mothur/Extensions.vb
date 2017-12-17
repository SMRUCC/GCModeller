Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports r = System.Text.RegularExpressions.Regex

Public Module Extensions

    Public Function SummaryTable(summary As String) As DataSet()
        Dim table As String()() = r _
            .Split(summary, "summary\.seqs\(.+?\)", RegexICSng) _
            .Last _
            .lTokens _
            .Skip(4) _
            .Take(9) _
            .Select(Function(line) line.Split(ASCII.TAB)) _
            .ToArray
        Dim headers = table.First.Skip(2).ToArray
        Dim tsv As DataSet() = table _
            .Skip(1) _
            .Select(Function(row)
                        Return New DataSet With {
                            .ID = row(0).Trim(":"c),
                            .Properties = headers _
                                .SeqIterator(offset:=1) _
                                .ToDictionary(Function(key) key.value,
                                              Function(i) Val(row.ElementAtOrDefault(i)))
                        }
                    End Function) _
            .ToArray

        Return tsv
    End Function
End Module
