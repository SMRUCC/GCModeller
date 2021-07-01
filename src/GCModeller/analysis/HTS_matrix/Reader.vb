Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Text
Imports r = System.Text.RegularExpressions.Regex

Module Reader

    Const quot$ = """.+""[,\t]"

    Friend Function ParseGeneRowTokens(line As String) As NamedValue(Of Double())
        Dim quot As String = r.Match(line, Reader.quot).Value

        If Not quot.StringEmpty Then
            Return New NamedValue(Of Double()) With {
                .Name = quot.Trim(""""c, ","c),
                .Value = line _
                    .Substring(quot.Length) _
                    .Split(ASCII.TAB, ","c) _
                    .Select(AddressOf ParseDouble) _
                    .ToArray
            }
        Else
            Dim tokens As String() = line.Split(ASCII.TAB, ","c)

            Return New NamedValue(Of Double()) With {
                .Name = tokens(Scan0),
                .Value = tokens _
                    .Skip(1) _
                    .Select(AddressOf ParseDouble) _
                    .ToArray
            }
        End If
    End Function

    <Extension>
    Friend Iterator Function loadGeneMatrix(text As IEnumerable(Of String), excludes As Index(Of String), takeIndex As Integer()) As IEnumerable(Of DataFrameRow)
        For Each line As String In text
            Dim tokens As NamedValue(Of Double()) = ParseGeneRowTokens(line)
            Dim data As Double() = tokens.Value

            If Not excludes Is Nothing Then
                data = takeIndex _
                    .Select(Function(i) data(i)) _
                    .ToArray
            End If

            Yield New DataFrameRow With {
                .experiments = data,
                .geneID = tokens.Name.Trim(""""c, " "c, ASCII.TAB)
            }
        Next
    End Function
End Module
