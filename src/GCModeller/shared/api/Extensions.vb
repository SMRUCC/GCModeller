Imports System.Runtime.CompilerServices

Namespace Common

    Module Extensions

        <Extension> Public Function FamilyTokens(Family As String) As String()
            Family = Family.Replace("*", "")
            Family = Family.Replace("(", "")
            Family = Family.Replace(")", "").Split("-"c).First

            Dim Tokens As String() = (From s As String
                                      In Family.Split("/"c)
                                      Let ts As String = Trim(s)
                                      Where Not String.IsNullOrEmpty(ts)
                                      Select ts).ToArray
            Return Tokens
        End Function
    End Module
End Namespace