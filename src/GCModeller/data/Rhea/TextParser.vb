Imports System.Runtime.CompilerServices
Imports System.Text

Public Module TextParser

    <Extension>
    Public Iterator Function ParseCompounds(lines As IEnumerable(Of String)) As IEnumerable(Of Compound)
        For Each block As String() In lines.Split("///")
            Dim list As Dictionary(Of String, String) = block.ParseList
            Dim obj As New Compound With {
                .entry = list!ENTRY,
                .name = list!NAME,
                .formula = list!FORMULA,
                .reactions = (list!REACTION).Split
            }

            If list.ContainsKey("ENZYME") Then
                obj.enzyme = (list!ENZYME).Split
            End If

            Yield obj
        Next
    End Function

    <Extension>
    Private Function ParseList(block As IEnumerable(Of String)) As Dictionary(Of String, String)
        Dim data As New Dictionary(Of String, String)
        Dim sb As New StringBuilder
        Dim key As String = Nothing

        For Each line As String In block
            Dim tag As String = Mid(line, 1, 12).Trim
            Dim value As String = Mid(line, 13).Trim

            If Not tag.StringEmpty AndAlso Not key.StringEmpty Then
                data.Add(key, sb.ToString)
                sb.Clear()
                key = tag
            Else
                Call sb.Append(" ")
                Call sb.Append(value)
            End If
        Next

        If Not sb.Length = 0 Then
            Call data.Add(key, sb.ToString)
        End If

        Return data
    End Function

    <Extension>
    Public Iterator Function ParseReactions(lines As IEnumerable(Of String)) As IEnumerable(Of Reaction)
        For Each block As String() In lines.Split("///")
            Dim list As Dictionary(Of String, String) = block.ParseList
            Dim obj As New Reaction With {
                .entry = list!ENTRY,
                .definition = list!DEFINITION,
                .equation = list!EQUATION
            }

            If list.ContainsKey("ENZYME") Then
                obj.enzyme = (list!ENZYME).Split
            End If

            Yield obj
        Next
    End Function
End Module
