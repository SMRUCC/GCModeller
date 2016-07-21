Imports System.Runtime.CompilerServices

Public Module ecardParser

    Public Function ParseFile(path As String) As IEnumerable(Of Dictionary(Of String, String()))
        Return path.ReadAllText.Parsing
    End Function

    <Extension>
    Public Iterator Function Parsing(content As String) As IEnumerable(Of Dictionary(Of String, String()))
        Dim lines As String() = content.lTokens
        Dim tokens As IEnumerable(Of String()) = lines.Split("//")
    End Function
End Module
