Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.IO.SearchEngine
Imports SMRUCC.genomics.Assembly.NCBI
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

    <ExportAPI("/Map.Hits.Taxonomy",
           Usage:="/Map.Hits.Taxonomy /in <query.csv> /mapping <blastnMapping.csv> /tax <taxonomy.DIR:name/nodes>[/out <out.csv>]")>
    <Argument("/mapping", True,
                   AcceptTypes:={GetType(BlastnMapping)},
                   Description:="Data frame should have a ``taxid`` field.")>
    Public Function MapHitsTaxonomy(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim mapping As String = args("/mapping")
        Dim out As String = args _
            .GetValue("/out", [in].TrimSuffix & "-" & mapping.BaseName & "_MapHits.csv")
        Dim exps = [in].LoadCsv(Of QueryArgument)
        Dim expList = (From x As QueryArgument
                       In exps
                       Select x,
                           query = x.Expression.Build(allowInStr:=False),
                           bufs = New Dictionary(Of Integer, List(Of String))).ToArray
        Dim hits As New Dictionary(Of String, Dictionary(Of Integer, List(Of String)))

        For Each x In expList
            hits.Add(x.x.Name, x.bufs)
        Next
        For Each hit In mapping.LoadCsv(Of BlastnMapping)
            Dim taxid% = CInt(Val(hit.Extensions("taxid")))
            Dim LQuery = From x
                         In expList.AsParallel
                         Where x.query.Match(hit.Reference)
                         Select x.x.Name

            For Each n In LQuery
                If Not hits(n).ContainsKey(taxid) Then
                    hits(n).Add(taxid, New List(Of String))
                End If
                Call hits(n)(taxid).Add(hit.ReadQuery)
            Next
        Next

        Dim output As New List(Of QueryArgument)
        Dim taxonomy As New NcbiTaxonomyTree(args("/tax"))

        For Each x In expList
            Dim data = x.bufs

            For Each taxid In data
                Dim nodes = taxonomy.GetAscendantsWithRanksAndNames(taxid.Key, True)
                Dim tree$ = TaxonomyNode.BuildBIOM(nodes)

                output += New QueryArgument With {
                    .Name = x.x.Name,
                    .Expression = x.x.Expression,
                    .Data = New Dictionary(Of String, String) From {
                        {"taxid", taxid.Key},
                        {"MapHits", taxid.Value.Distinct.JoinBy("; ")},
                        {"Taxonomy.Name", taxonomy(taxid.Key).name},
                        {"Taxonomy", tree}
                    }
                }
            Next
        Next

        Return output.SaveTo(out).CLICode
    End Function
End Module