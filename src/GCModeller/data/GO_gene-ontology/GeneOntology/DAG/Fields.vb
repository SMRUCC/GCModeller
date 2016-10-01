Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace DAG

    Public Structure is_a

        Dim uid$, cat$
        ''' <summary>
        ''' 父节点的实例
        ''' </summary>
        Dim term As Term

        Sub New(value$)
            Dim tokens$() = Strings.Split(value$, " ! ")

            uid = tokens(Scan0%)
            cat = tokens(1%)
        End Sub

        Public Overrides Function ToString() As String
            Return $"is_a: {uid} ! {cat$}"
        End Function
    End Structure

    Public Structure def

        Public def$
        Public ref As NamedValue(Of String)()

        Sub New(value$)
            Dim refs$ = Regex.Match(value, "\s+\[.+?\]").Value

            def = Mid(value$, 1, value.Length - ref.Length)
            refs = refs.GetStackValue("[", "]")
            ref = LinqAPI.Exec(Of NamedValue(Of String)) <=
 _
                From t As String
                In refs.Split(","c)
                Select t.Trim.GetTagValue(":", trim:=True)
        End Sub

        Public Overrides Function ToString() As String
            Dim refs As String = ref.ToArray(Function(x) $"{x.Name}:{x.x}").JoinBy(", ")
            Return $"def: ""{def}"" [{refs}]"
        End Function
    End Structure

    Public Structure synonym

        Dim name$, type$
        Dim synonym As NamedValue(Of String)

        Sub New(value$)
            Dim tokens$() = CommandLine.GetTokens(value$)

            name = tokens(Scan0)
            type = tokens(1)
            synonym = tokens(2).GetStackValue("[", "]").GetTagValue(":")
        End Sub

        Public Overrides Function ToString() As String
            Return $"synonym: ""{name}"" {type} [{synonym.Name}:{synonym.x}]"
        End Function
    End Structure
End Namespace