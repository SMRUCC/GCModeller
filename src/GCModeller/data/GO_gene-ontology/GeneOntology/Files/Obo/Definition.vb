Imports System.Runtime.CompilerServices

Namespace OBO

    ''' <summary>
    ''' text parser of <see cref="Term.def"/>
    ''' </summary>
    Public Class Definition

        Public Property definition As String
        Public Property evidences As String()
        Public Property isOBSOLETE As Boolean

        Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Dim OBSOLETE = If(isOBSOLETE, "OBSOLETE. ", "")

            ' add OBSOLETE. tags if it is true
            Return $"""{OBSOLETE}{definition}"" [{evidences.JoinBy(", ")}]"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Parse(term As Term) As Definition
            Return Parse(term.def)
        End Function

        Public Shared Function Parse(dataLine As String) As Definition
            Dim info = dataLine.GetStackValue("""", """")
            Dim evidences = dataLine.Match("\[.+?\]", RegexICSng).GetStackValue("[", "]")
            Dim OBSOLETE = InStr(info, "OBSOLETE.") = 1

            If OBSOLETE Then
                info = Mid(info, 10).Trim
            End If

            Return New Definition With {
                .isOBSOLETE = OBSOLETE,
                .definition = info,
                .evidences = evidences _
                    .Split(","c) _
                    .Select(AddressOf Strings.Trim) _
                    .ToArray
            }
        End Function
    End Class
End Namespace