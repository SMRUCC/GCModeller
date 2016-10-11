Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.IO.SearchEngine
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application

Partial Module CLI

    <ExportAPI("/Map.Hits",
               Usage:="/Map.Hits /in <query.csv> /mapping <blastnMapping.csv> [/out <out.csv>]")>
    Public Function MapHits(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim mapping As String = args("/mapping")
        Dim out As String = args _
            .GetValue("/out", [in].TrimSuffix & "-" & mapping.BaseName & "_MapHits.csv")
        Dim exps = [in].LoadCsv(Of QueryArgument)
        Dim expList = (From x As QueryArgument
                       In exps
                       Select x,
                           query = x.Expression.Build(allowInStr:=False),
                           bufs = New List(Of String)).ToArray
        Dim hits As New Dictionary(Of String, List(Of String))

        For Each x In expList
            hits.Add(x.x.Name, x.bufs)
        Next
        For Each hit In mapping.LoadCsv(Of BlastnMapping)
            Dim LQuery = From x
                         In expList.AsParallel
                         Where x.query.Match(hit.Reference)
                         Select x.x.Name

            For Each n In LQuery
                hits(n).Add(hit.ReadQuery)
            Next
        Next

        For Each x In expList
            x.x.Data.Add("MapHits",
                x.bufs _
                .Distinct _
                .OrderBy(Function(s) s) _
                .JoinBy("; "))
        Next

        Return exps.SaveTo(out).CLICode
    End Function
End Module