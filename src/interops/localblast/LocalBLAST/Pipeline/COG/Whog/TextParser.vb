Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports r = System.Text.RegularExpressions.Regex

Namespace Pipeline.COG.Whog

    Module TextParser

        Const REGX_CATAGORY As String = "\[[^]]+\]"
        Const REGX_COG_ID As String = "COG\d+"

        Public Function Parse(srcText$()) As Category
            Dim list As NamedValue() = parseList(srcText.Skip(1).ToArray)
            Dim description As String = srcText(Scan0)
            Dim cat$ = r.Match(description, REGX_CATAGORY).Value
            Dim item As New Category With {
                .category = Mid(cat, 2, Len(cat) - 2),
                .COG_id = r.Match(description, REGX_COG_ID).Value,
                .description = Mid(description, Len(.category) + Len(.COG_id) + 4).Trim,
                .IdList = list
            }
            Return item
        End Function

        Private Function parseList(lines As IEnumerable(Of String)) As NamedValue()
            Dim list As New List(Of NamedValue)

            For Each line As String In lines
                Dim nid As NamedValue(Of String) = line.GetTagValue(":", trim:=True)
                Dim genome$ = nid.Name.Trim

                If Not String.IsNullOrEmpty(genome) Then
                    list += New NamedValue With {
                        .name = genome,
                        .Text = nid.Value
                    }
                Else
                    list.Last = New NamedValue With {
                        .name = list.Last.name,
                        .Text = list.Last.text & " " & Trim(nid.Value)
                    }
                End If
            Next

            Return list
        End Function
    End Module
End Namespace