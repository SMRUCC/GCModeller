Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.IO.SearchEngine
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI

Partial Module CLI

    ReadOnly __dels As Char() = ASCII.Symbols.Join(" "c, ASCII.TAB).ToArray

    Private Function __getEvaluator(expression As String, compile As Boolean) As Func(Of String, Double)
        If compile Then
            Dim exp As Expression = expression _
                .Split(__dels) _
                .Select(Function(s) s.Trim(" "c, ASCII.TAB)) _
                .Where(Function(s) Not String.IsNullOrEmpty(s)) _
                .JoinBy(" AND ") _
                .Build()
            Return Function(text) If(exp.Match(text), 1.0R, 0R)
        Else
            Return Function(text) Similarity.Evaluate(text, expression)
        End If
    End Function

    <ExportAPI("/Search.Taxonomy", Usage:="/Search.Taxonomy /in <list.txt> /ncbi_taxonomy <taxnonmy:name/nodes> [/compile /cut 0.65 /out <out.csv>]")>
    Public Function SearchTaxonomy(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim ncbi_taxonomy As String = args("/ncbi_taxonomy")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "_" & ncbi_taxonomy.BaseName & ".csv")
        Dim list$() = [in].ReadAllLines
        Dim cutoff# = args.GetValue("/cut", 0.65)
        Dim taxonomy As New NcbiTaxonomyTree(ncbi_taxonomy)
        Dim output As New List(Of TaxiSummary)
        Dim compile As Boolean = args.GetBoolean("/compile")
        Dim nodes = From name As String
                    In list
                    Let evaluate As Func(Of String, Double) =
                        __getEvaluator(
                        expression:=name,
                        compile:=compile)
                    Select name,
                        taxid = From k
                                In taxonomy.Taxonomy.AsParallel
                                Let score As Double = evaluate(k.Value.name)
                                Where score >= cutoff
                                Select score,
                                    id = k.Key,
                                    node = k.Value
        For Each group In nodes
            Dim h = (From x
                     In group.taxid
                     Select x
                     Order By x.score Descending).FirstOrDefault

            If h Is Nothing Then
                Call $"{group.name} not found!".Warning
                Call output.Add(
                    New TaxiSummary With {
                        .title = group.name
                    })

                Continue For
            End If

            Call h.node.GetJson.__DEBUG_ECHO

            Dim tree = taxonomy.GetAscendantsWithRanksAndNames(h.id)

            output += New TaxiSummary(tree) With {
                .taxid = h.id,
                .title = group.name
            }
        Next

        Return output.SaveTo(out).CLICode
    End Function
End Module