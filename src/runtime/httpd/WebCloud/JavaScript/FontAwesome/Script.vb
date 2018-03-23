Imports System.ComponentModel
Imports System.Text
Imports Microsoft.VisualBasic.Emit.CodeDOM_VBC
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS.Parser

Public Module VBScript

    Public Function FromCSS(fontawesome As String) As String
        Dim css As CSSFile = CssParser.GetTagWithCSS(fontawesome.SolveStream, selectorFilter:="\.fa[-]")
        Dim icons = css.Selectors.Values.Where(Function(s) s.HasProperty("content")).ToArray
        Dim members = icons _
            .Select(Function(icon As Selector)
                        Dim member = icon.Selector _
                                         .Replace(".fa-", "") _
                                         .Replace(":before", "")
                        member = CodeHelper.EnumMember(member, True, newLine:=True)
                        member = $"<Content({icon!content})>" & vbCrLf & member

                        ' 因为在CSS文件里面，content的值里面已经存在双引号了
                        ' 所以在生成自定义属性的时候不需要再添加双引号了
                        ' 否则会出错

                        ' .fa-audible: before {
                        '     content: "\f373"
                        ' }

                        Return member
                    End Function) _
            .ToArray

        With New StringBuilder

            Call .AppendLine($"Imports {GetType(DescriptionAttribute).Namespace}")
            Call .AppendLine()
            Call .AppendLine($"Public Enum {NameOf(icons)}")

            For Each member In members
                Call .AppendLine(member)
            Next

            Call .AppendLine("End Enum")

            Return .ToString
        End With
    End Function
End Module
