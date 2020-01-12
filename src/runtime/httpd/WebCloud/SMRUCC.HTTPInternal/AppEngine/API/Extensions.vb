Imports System.Text
Imports System.Text.RegularExpressions

Namespace AppEngine.APIMethods

    <HideModuleName> Module Extensions

        Private Function VirtualPath(strData As String(), prefix As String) As Dictionary(Of String, String)
            Dim LQuery = From source As String
                         In strData
                         Let trimPrefix = Regex.Replace(source, "in [A-Z][:]\\", "", RegexOptions.IgnoreCase)
                         Let line = Regex.Match(trimPrefix, "[:]line \d+").Value
                         Let path = trimPrefix.Replace(line, "")
                         Select source,
                             path

            Dim LTokens = (From obj
                           In LQuery
                           Let tokens As String() = obj.path.Split("\"c)
                           Select tokens,
                               obj.source).ToArray
            Dim p As Integer

            If LTokens.Length = 0 Then
                Return New Dictionary(Of String, String)
            End If

            For i As Integer = 0 To (From obj In LTokens Select obj.tokens.Length).Min - 1
                p = i

                If (From n In LTokens Select n.tokens(p) Distinct).Count > 1 Then
                    Exit For
                End If
            Next

            Dim LSkips = (From obj In LTokens Select obj.source, obj.tokens.Skip(p).ToArray).ToArray
            Dim LpreFakes = (From obj In LSkips
                             Select obj.source,
                                 virtual = String.Join("/", obj.ToArray).Replace(".vb", ".vbs")).ToArray
            Dim hash As Dictionary(Of String, String) = LpreFakes.ToDictionary(
                Function(obj) obj.source,
                Function(obj) $"in {prefix}/{obj.virtual}:line {CInt(5000 * Rnd() + 100)}")
            Return hash
        End Function

        Const virtual As String = "/root/ubuntu.d~/->/wwwroot/~azure.microsoft.com/api.vbs?virtual=ms_visualBasic_sh:/"

        Friend Function Fakes(ex As Exception) As String
            Dim exText As String = ex.ToString
            Dim line As String() = Regex.Matches(exText, "in .+?[:]line \d+").ToArray
            Dim hash As Dictionary(Of String, String) = VirtualPath(line, virtual)
            Dim sbr As New StringBuilder(exText)

            For Each obj In hash
                Call sbr.Replace(obj.Key, obj.Value)
            Next

            Return sbr.ToString
        End Function
    End Module
End Namespace