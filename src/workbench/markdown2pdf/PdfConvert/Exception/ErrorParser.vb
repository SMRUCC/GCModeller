Imports System.ComponentModel
Imports r = System.Text.RegularExpressions.Regex

Module ErrorParser

    ReadOnly errorTags As Dictionary(Of String, Errors) = Enums(Of Errors).ToDictionary(Function(e) e.Description)

    Public Iterator Function YieldErrors(output As String) As IEnumerable(Of (err As Errors, message As String))
        Dim errors = r.Matches(output, "((Error[:])|(Exit with)).+?$", RegexICMul).ToArray

        For Each err As String In errors
            Dim type As Errors = errorTags _
                .First(Function(t) InStr(err, t.Key) > 0) _
                .Value

            Yield (type, err)
        Next
    End Function
End Module

Public Enum Errors
    <Description("Error: Failed loading page")>
    FailedLoadingPage
    <Description("network error: HostNotFoundError")>
    HostNotFound
End Enum