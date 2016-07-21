Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

Public Module ecardParser

    Public Function ParseFile(path As String, Optional ByRef tag As NamedValue(Of String) = Nothing) As IEnumerable(Of Dictionary(Of String, String()))
        Dim out As New Value(Of NamedValue(Of String))
        Dim tokens = path.ReadAllText.Parsing(out)
        tag = out.Value
        Return tokens
    End Function

    Const BlastOutput As String = "Annotation::BlastOutput"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="content"></param>
    ''' <param name="tag"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function Parsing(content As String, tag As Value(Of NamedValue(Of String))) As IEnumerable(Of Dictionary(Of String, String()))
        Dim lines As String() = content.lTokens
        Dim tokens As IEnumerable(Of String()) = lines.Skip(4).Split("//")

        lines = lines.Take(4).Where(Function(s) Not s.IsBlank).ToArray
        tag.Value = New NamedValue(Of String) With {
            .Name = Mid(lines(Scan0), 12).Trim,
            .x = Mid(lines(1), 20).Trim
        }

        For Each part As String() In tokens
            Dim tmp As New List(Of NamedValue(Of String))
            Dim isBlastOutput As Boolean = False
            Dim out As StringBuilder = Nothing
            Dim isEnd As Boolean = False

            For Each line As String In part
                If isBlastOutput Then
                    If String.IsNullOrEmpty(line) AndAlso isEnd Then
                        isBlastOutput = False
                    Else
                        out.AppendLine(line)
                    End If

                    If Regex.Match(line, "").Success Then
                        isEnd = True
                    End If
                Else
                    Dim tagValue = line.GetTagValue(, True)

                    If tagValue.x = BlastOutput Then
                        isBlastOutput = True
                    End If
                    If tagValue.Name = "VALUE" AndAlso isBlastOutput Then
                        ' 解析blast输出
                        out = New StringBuilder
                        out.AppendLine(tagValue.x)
                    Else
                        tmp += tagValue
                    End If
                End If
            Next

            If out IsNot Nothing AndAlso out.Length > 0 Then
                tmp += New NamedValue(Of String) With {
                    .Name = "VALUE",
                    .x = out.ToString
                }
            End If

            Dim Groups = From x As NamedValue(Of String)
                         In tmp
                         Select x
                         Group x By x.Name Into Group '
            Yield Groups.ToDictionary(Function(x) x.Name,
                                      Function(x) x.Group.ToArray(
                                      Function(o) o.x))
        Next
    End Function
End Module
