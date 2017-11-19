Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Text
Imports r = System.Text.RegularExpressions.Regex

Partial Module vbhtml

    Const ForEachLoop$ = "<?vb\s+For\s+.+?\s+As\s+<%= [^>]+? %>\s+?>"

    ''' <summary>
    ''' Returns tuple: ``[expression, template_path]``
    ''' </summary>
    ''' <param name="vbhtml$"></param>
    ''' <returns></returns>
    Public Function ParseForEach(vbhtml$) As NamedValue(Of String)()
        Dim expression = r _
            .Matches(vbhtml, ForEachLoop, RegexICSng) _
            .EachValue(AddressOf parserInternal) _
            .ToArray

        Return expression
    End Function

    Private Function parserInternal(input As String) As NamedValue(Of String)
        Dim value$ = input.GetStackValue("<?", "?>").Trim
        Dim t = r _
            .Split(value, "(For\s+)|(\s+As\s+)", RegexICSng) _
            .Where(Function(s) Not s.StringEmpty) _
            .ToArray

        Return New NamedValue(Of String) With {
            .Name = t(0),
            .Value = t(1),
            .Description = input
        }
    End Function

    Public Function Iterates(wwwroot$, vbhtml$, encoding As Encodings) As String
        Dim expressions = ParseForEach(vbhtml)
        Dim html As New StringBuilder(vbhtml)

        For Each exp As NamedValue(Of String) In expressions

        Next

        Return html.ToString
    End Function
End Module