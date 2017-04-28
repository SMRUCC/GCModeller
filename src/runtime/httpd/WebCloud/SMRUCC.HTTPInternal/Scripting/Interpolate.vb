Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions

Namespace Scripting

    Public Module Interpolate

        Const Expression$ = "<%= [^>]+? %>"

        ''' <summary>
        ''' ``&lt;%= relative_path %>``
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ReadHTML(path$) As String
            Dim parent$ = path.ParentPath
            Dim html As New StringBuilder(path.ReadAllText)
            Dim includes$() = Regex _
                .Matches(html.ToString, Interpolate.Expression, RegexICSng) _
                .ToArray

            For Each include As String In includes
                Dim rel_path$ = include.Trim("<"c, ">"c, "%"c).Trim
                rel_path = parent & "/" & rel_path
                rel_path = FileIO.FileSystem.GetFileInfo(rel_path).FullName
                Dim content$ = rel_path.ReadAllText
                Call html.Replace(include, content)
            Next

            Return html.ToString
        End Function
    End Module
End Namespace