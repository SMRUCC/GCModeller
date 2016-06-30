Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Linq

<Extension> Module Extensions

    Public Const Scan0 = 0

    <Extension> Public Function GenerateLine(Line As Double()) As String
        Return String.Join(", ", Line.ToArray(Function(x) CStr(x)))
    End Function

    <Extension> Public Function PToken(s As Match) As KeyValuePair(Of Integer, Double)
        Dim m As MatchCollection = Regex.Matches(s.Value, "\d+")

        If m.Count = 2 Then
            Return New KeyValuePair(Of Integer, Double)(Val(m(1).Value), Val(m(0).Value))
        Else
            Return New KeyValuePair(Of Integer, Double)(Val(m(0).Value), 1)
        End If
    End Function

    <Extension> Public Function Width(m As List(Of Double())) As Long
        Return m.First.Length
    End Function
End Module
