Imports System.Runtime.CompilerServices

Namespace OBO

    ''' <summary>
    ''' text parser of <see cref="Term.def"/>
    ''' </summary>
    Public Class Definition

        Public Property definition As String
        Public Property evidences As String()

        Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return $"""{definition}"" [{evidences.JoinBy(", ")}]"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Parse(term As Term) As Definition
            Return Parse(term.def)
        End Function

        Public Shared Function Parse(dataLine As String) As Definition
            Dim info = dataLine.GetStackValue("""", """")
            Dim evidences = dataLine.Match("\[.+?\]", RegexICSng).GetStackValue("[", "]")

            Return New Definition With {
                .definition = info,
                .evidences = evidences _
                    .Split(","c) _
                    .Select(AddressOf Strings.Trim) _
                    .ToArray
            }
        End Function
    End Class
End Namespace