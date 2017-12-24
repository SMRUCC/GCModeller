Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports r = System.Text.RegularExpressions.Regex

Partial Module vbhtml

    ''' <summary>
    ''' ```vbnet
    ''' Dim $var As &lt;%= path %> 
    ''' ```
    ''' </summary>
    Const DimVar$ = "<\?vb\s+Dim\s+.+?\s+As\s+" & PartialIncludes & "\s+\?>"

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function ParseDimVar(vbhtml$) As NamedValue(Of String)()
        Dim expression = r _
            .Matches(vbhtml, DimVar, RegexICSng) _
            .EachValue(AddressOf parserInternal) _
            .ToArray

        Return expression
    End Function

    <Extension>
    Public Function InterplotDimVar(html As StringBuilder, parent$, args As InterpolateArgs) As StringBuilder
        Dim expressions = ParseDimVar(html.ToString)
        Dim content$

        ' 在这里只生成variable，可能不会进行模板字符串的替换
        For Each var As NamedValue(Of String) In expressions
            With var
                content = New StringBuilder(.Value) _
                    .TemplateInterplot(parent, args)

                Call args.variables.Add(Mid(.Name, 2), content)
                Call html.Replace(.Description, "")
            End With
        Next

        Return html
    End Function
End Module