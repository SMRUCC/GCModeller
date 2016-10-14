Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Data.IO.SearchEngine
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Metagenomics
Imports SMRUCC.genomics.SequenceModel.FASTA

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

    <ExportAPI("/Search.Taxonomy",
               Usage:="/Search.Taxonomy /in <list.txt/expression.csv> /ncbi_taxonomy <taxnonmy:name/nodes.dmp> [/top 10 /expression /cut 0.65 /out <out.csv>]")>
    <ParameterInfo("/expression", True,
                   Description:="Search the taxonomy text by using query expression? If this set true, then the input should be a expression csv file.")>
    <ParameterInfo("/cut", True, Description:="This parameter will be disabled when ``/expression`` is presents.")>
    <ParameterInfo("/in", False, AcceptTypes:={GetType(String()), GetType(QueryArgument)})>
    Public Function SearchTaxonomy(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim ncbi_taxonomy As String = args("/ncbi_taxonomy")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "_" & ncbi_taxonomy.BaseName & ".csv")
        Dim taxonomy As New NcbiTaxonomyTree(ncbi_taxonomy)
        Dim output As New List(Of TaxiSummary)
        Dim isExpression As Boolean = args.GetBoolean("/expression")
        Dim evaluates As NamedValue(Of Func(Of String, Boolean))()
        Dim top% = args.GetValue("/top", 10)

        If isExpression Then
            Dim list = [in].LoadCsv(Of QueryArgument)

            evaluates = LinqAPI.Exec(Of NamedValue(Of Func(Of String, Boolean))) <=
                From x As QueryArgument
                In list
                Let exp As Expression = x.Expression.Build
                Select New NamedValue(Of Func(Of String, Boolean)) With {
                    .Name = x.Name,
                    .x = AddressOf exp.Match
                }

            Call "Search in expression query mode!".__DEBUG_ECHO
        Else
            Dim list$() = [in].ReadAllLines
            Dim cutoff# = args.GetValue("/cut", 0.65)

            evaluates = LinqAPI.Exec(Of NamedValue(Of Func(Of String, Boolean))) <=
                From name As String
                In list
                Let evl As Func(Of String, Boolean) = Function(s) Similarity.Evaluate(s, name) >= cutoff
                Select New NamedValue(Of Func(Of String, Boolean)) With {
                    .Name = name,
                    .x = evl
                }
        End If

        ' Dim ranksData As New Ranks(tree:=taxonomy)
        Dim nodes = From exp
                    In evaluates.AsParallel
                    Let evaluate = exp.x
                    Select name = exp.Name,
                        taxid = From k
                                In taxonomy.Taxonomy.AsParallel
                                Let tax_name As String = k.Value.name
                                Where evaluate(tax_name)
                                Let score As Double = If(isExpression,
                                    1,
                                    Similarity.Evaluate(tax_name, exp.Name))
                                Select score,
                                    id = k.Key,
                                    node = k.Value
        For Each group In nodes
            Dim array = group.taxid.ToArray

            If array.IsNullOrEmpty Then
                Call $"{group.name} not found!".Warning
                Call output.Add(
                    New TaxiSummary With {
                        .title = group.name
                    })

                Continue For
            End If

            If Not isExpression Then
                array = array _
                    .OrderByDescending(Function(x) x.score) _
                    .Take(top) _
                    .ToArray
            Else
                array = (From x
                         In array.AsParallel
                         Let dist As DistResult = ComputeDistance(
                             group.name.ToLower,
                             x.node.name.ToLower
                         )
                         Where Not dist Is Nothing
                         Select score = dist.MatchSimilarity,
                             x.id,
                             node = x.node
                         Order By score Descending).Take(top).ToArray
            End If

            For Each h In array
                Dim id% = h.id
                Dim tree = taxonomy _
                    .GetAscendantsWithRanksAndNames(id)

                output += New TaxiSummary(tree) With {
                    .taxid = id,
                    .title = group.name
                }
            Next
        Next

        Return output.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Split.By.Taxid",
               Usage:="/Split.By.Taxid /in <nt.fasta> [/gi2taxid <gi2taxid.txt> /out <outDIR>]")>
    Public Function SplitByTaxid(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim gi2taxid As String = args.GetValue("/gi2taxid", [in].TrimSuffix & ".txt")
        Dim EXPORT As String = args.GetValue("/out", [in].TrimSuffix)

        If Not gi2taxid.FileExists Then
            gi2taxid = gi2taxid.TrimSuffix & ".gi_match.txt"
        End If
        If Not gi2taxid.FileExists Then
            Throw New Exception($"Unable found gi2taxid file for " & [in].ToFileURL)
        End If

        Dim taxids As BucketDictionary(Of Integer, Integer) =
            Taxonomy.AcquireAuto(gi2taxid)
        Dim output As New Dictionary(Of Integer, StreamWriter)

        For Each fa As FastaToken In New StreamIterator([in]).ReadStream
            Dim gi As Integer = CInt(
                Regex.Match(fa.Title, "gi\|\d+") _
                .Value _
                .Split("|"c) _
                .Last)

            If taxids.ContainsKey(gi) Then
                Dim taxid% = taxids(gi)

                If Not output.ContainsKey(taxid) Then
                    Dim out$ = fa.Title
                    out = Regex.Replace(out, "gi\|\d+", "").NormalizePathString(False)
                    out = Mid(out, 2, 45) & $"-{taxid}.fasta"
                    out = EXPORT & "/" & out
                    output.Add(taxid, out.OpenWriter(Encodings.ASCII))
                End If

                Call output(taxid).WriteLine(fa.GenerateDocument(120))
            Else
                Call (fa.Title & " not found taxid!").PrintException
            End If
        Next

        For Each file In output.Values
            Call file.Flush()
            Call file.Close()
            Call file.Dispose()
        Next

        Return 0
    End Function

    <ExportAPI("/Split.By.Taxid.Batch",
               Usage:="/Split.By.Taxid.Batch /in <nt.fasta.DIR> [/num_threads <-1> /out <outDIR>]")>
    Public Function SplitByTaxidBatch(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimDIR & "-Split/")
        Dim CLI As Func(Of String, String) =
            Function(file) $"{GetType(CLI).API(NameOf(SplitByTaxid))} /in {file.CLIPath} /out {(out & "/" & file.BaseName.Trim).CLIPath}"
        Dim n As Integer = args.GetValue("/num_threads", -1)
        Dim tasks$() = (ls - l - r - wildcards("*.fasta") <= [in]).ToArray(CLI)

        Return App.SelfFolks(tasks, LQuerySchedule.AutoConfig(n))
    End Function

    Public Class MapHits
        <Collection("MapHits",)> Public Property MapHits As String()
        Public Property Data As Dictionary(Of String, String)
        Public Property taxid As Integer

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    <ExportAPI("/OTU.associated", Usage:="/OTU.associated /in <OTU.Data> /maps <mapsHit.csv> [/out <out.csv>]")>
    Public Function OTUAssociated(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim maps As String = args("/maps")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-" & maps.BaseName & ".OTU.associated.csv")
        Dim OTUData = [in].LoadCsv(Of OTUData).ToDictionary
        Dim mapsData = maps.LoadCsv(Of MapHits)
        Dim output As New List(Of OTUData)

        For Each x As MapHits In mapsData
            For Each OTU$ In x.MapHits
                Dim find As New OTUData(OTUData(OTU))
                For Each k In x.Data
                    find.Data(k.Key) = k.Value
                Next

                output += find
            Next
        Next

        Return output.SaveTo(out).CLICode
    End Function

    <ExportAPI("/OTU.Taxonomy", Usage:="/OTU.Taxonomy /in <OTU.Data> /maps <mapsHit.csv> /tax <taxonomy:nodes/names> [/out <out.csv>]")>
    Public Function OTU_Taxonomy(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim maps As String = args("/maps")
        Dim tax As String = args("/tax")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-" & maps.BaseName & ".OTU.Taxonomy.csv")
        Dim OTUData = [in].LoadCsv(Of OTUData).ToDictionary
        Dim mapsData = maps.LoadCsv(Of MapHits)
        Dim output As New List(Of OTUData)
        Dim taxonomy As New NcbiTaxonomyTree(tax)

        For Each x As MapHits In mapsData
            For Each OTU$ In x.MapHits
                Dim find As New OTUData(OTUData(OTU))

                For Each k In x.Data
                    find.Data(k.Key) = k.Value
                Next
                find.Data("Taxonomy") = TaxonomyNode.BuildBIOM(taxonomy.GetAscendantsWithRanksAndNames(x.taxid, True))

                output += find
            Next
        Next

        Return output.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' ref是总的数据，parts是ref里面的部分数据，则个函数则是将parts之中没有出现的都找出来
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/OTU.diff", Usage:="/OTU.diff /ref <OTU.Data1.csv> /parts <OTU.Data2.csv> [/out <out.csv>]")>
    Public Function OTUDiff(args As CommandLine) As Integer
        Dim ref = args("/ref")
        Dim parts = args("/parts")
        Dim out = args.GetValue("/out", parts.TrimSuffix & "-" & ref.BaseName & ".diff.csv")
        Dim diff As New List(Of String)
        Dim partsId = parts.LoadCsv(Of OTUData).Select(Function(x) x.OTU).Distinct.ToList

        For Each x In ref.LoadCsv(Of OTUData)
            If partsId.IndexOf(x.OTU) = -1 Then
                diff += x.OTU
            End If
        Next

        Return diff.FlushAllLines(out).CLICode
    End Function
End Module