#Region "Microsoft.VisualBasic::3fe1440d1816393343f4257ad72b23c8, CLI_tools\NCBI_tools\CLI\Taxonomy.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Module CLI
    ' 
    '     Function: __getEvaluator, GetMapHitsList, OTU_Taxonomy, OTUAssociated, OTUDiff
    '               OTUTaxonomyReplace, SearchTaxonomy, SplitByTaxid, SplitByTaxidBatch, TaxidMapHitViews
    '               TaxonomyTree, TaxonomyTreeData
    '     Class MapHit
    ' 
    '         Properties: Name, Taxonomy, TaxonomyName
    ' 
    '         Function: SamplesView, ToString
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Data.IO.SearchEngine
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
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
    <Argument("/expression", True,
              Description:="Search the taxonomy text by using query expression? If this set true, then the input should be a expression csv file.")>
    <Argument("/cut", True, Description:="This parameter will be disabled when ``/expression`` is presents.")>
    <Argument("/in", False, AcceptTypes:={GetType(String()), GetType(QueryArgument)})>
    <Group(CLIGrouping.TaxonomyTools)>
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
                    .Value = AddressOf exp.Match
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
                    .Value = evl
                }
        End If

        ' Dim ranksData As New Ranks(tree:=taxonomy)
        Dim nodes = From exp
                    In evaluates.AsParallel
                    Let evaluate = exp.Value
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

    <ExportAPI("/Split.By.Taxid", Info:="Split the input fasta file by taxid grouping.",
               Usage:="/Split.By.Taxid /in <nt.fasta> [/gi2taxid <gi2taxid.txt> /out <outDIR>]")>
    <Group(CLIGrouping.TaxonomyTools)>
    <Group(CLIGrouping.GITools)>
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
            NCBI.Taxonomy.AcquireAuto(gi2taxid)
        Dim output As New Dictionary(Of Integer, StreamWriter)

        For Each fa As FastaSeq In New StreamIterator([in]).ReadStream
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
    <Group(CLIGrouping.TaxonomyTools)>
    Public Function SplitByTaxidBatch(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimDIR & "-Split/")
        Dim CLI As Func(Of String, String) =
            Function(file) $"{GetType(CLI).API(NameOf(SplitByTaxid))} /in {file.CLIPath} /out {(out & "/" & file.BaseName.Trim).CLIPath}"
        Dim n As Integer = args.GetValue("/num_threads", -1)
        Dim tasks$() = (ls - l - r - wildcards("*.fasta") <= [in]).Select(CLI).ToArray

        Return App.SelfFolks(tasks, LQuerySchedule.AutoConfig(n))
    End Function

    <ExportAPI("/OTU.associated",
               Usage:="/OTU.associated /in <OTU.Data> /maps <mapsHit.csv> [/RawMap <data_mapping.csv> /OTU_Field <""#OTU_NUM""> /out <out.csv>]")>
    <Group(CLIGrouping.TaxonomyTools)>
    Public Function OTUAssociated(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim maps As String = args("/maps")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-" & maps.BaseName & ".OTU.associated.csv")
        Dim OTUData = [in].LoadCsv(Of OTUData).ToDictionary
        Dim mapsData = maps.LoadCsv(Of MapHits)
        Dim output As New List(Of OTUData)
        Dim raw As String = args("/RawMap")
        Dim fieldName As String = args("/OTU_Field")
        Dim rawMaps As Dictionary(Of NamedValue(Of Dictionary(Of String, String)))
        Dim Track As String = maps.BaseName

        If raw.FileExists Then
            rawMaps =
                IO.DataFrame _
                .Load(raw, Encoding.ASCII) _
                .EnumerateData _
                .Select(Function(row) New NamedValue(Of Dictionary(Of String, String)) With {
                    .Name = row(fieldName),
                    .Value = row
                }).ToDictionary
        Else
            rawMaps = New Dictionary(Of NamedValue(Of Dictionary(Of String, String)))
        End If

        For Each x As MapHits In mapsData.Where(Function(o) Not o.MapHits.IsNullOrEmpty)
            For Each OTU$ In x.MapHits
                Dim find As New OTUData(OTUData(OTU))
                For Each k In x.Data
                    find.data(k.Key) = k.Value
                Next
                If rawMaps.ContainsKey(OTU$) Then
                    Dim rawData = rawMaps(OTU$)

                    For Each k In rawData.Value
                        find.data(k.Key) = k.Value
                    Next

                    If rawData.Value.ContainsKey("Reference") Then
                        Dim ref = rawData.Value("Reference")
                        Dim gi = Regex.Match(ref, "gi\|\d+").Value
                        find.data("gi") = gi
                    End If
                End If

                find.data(NameOf(Track)) = Track
                output += find
            Next
        Next

        Return output.SaveTo(out).CLICode
    End Function

    <ExportAPI("/OTU.Taxonomy", Usage:="/OTU.Taxonomy /in <OTU.Data> /maps <mapsHit.csv> /tax <taxonomy:nodes/names> [/out <out.csv>]")>
    <Group(CLIGrouping.TaxonomyTools)>
    Public Function OTU_Taxonomy(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim maps As String = args("/maps")
        Dim tax As String = args("/tax")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-" & maps.BaseName & ".OTU.Taxonomy.csv")
        Dim OTUData = [in].LoadCsv(Of OTUData).ToDictionary
        Dim mapsData = maps.LoadCsv(Of MapHits)
        Dim output As New List(Of OTUData)
        Dim taxonomy As New NcbiTaxonomyTree(tax)
        Dim Track As String = maps.BaseName

        For Each x As MapHits In mapsData
            For Each OTU$ In x.MapHits
                Dim find As New OTUData(OTUData(OTU))

                For Each k In x.Data
                    find.data(k.Key) = k.Value
                Next
                find.data("Taxonomy") = taxonomy.GetAscendantsWithRanksAndNames(x.taxid, True).BuildBIOM()
                find.data(NameOf(Track)) = Track

                output += find
            Next
        Next

        Return output.SaveTo(out).CLICode
    End Function

    <ExportAPI("/OTU.Taxonomy.Replace", Info:="Using ``MapHits`` property",
               Usage:="/OTU.Taxonomy.Replace /in <otu.table.csv> /maps <maphits.csv> [/out <out.csv>]")>
    Public Function OTUTaxonomyReplace(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim maps As String = args("/maps")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-" & maps.BaseName & "_taxonomy_replaced.csv")
        Dim OTUs = [in].LoadCsv(Of OTUData)
        Dim mapsData = maps.LoadCsv(Of MapHit)
        Dim hash = OTUs.ToDictionary

        For Each x In mapsData
            For Each tag$ In x.MapHits
                If hash.ContainsKey(tag) Then
                    hash(tag).data(NameOf(MapHit.Taxonomy)) = x.Taxonomy
                End If
            Next
        Next

        Return hash.Values _
            .ToArray _
            .SaveTo(out) _
            .CLICode
    End Function

    ''' <summary>
    ''' ref是总的数据，parts是ref里面的部分数据，则个函数则是将parts之中没有出现的都找出来
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/OTU.diff", Usage:="/OTU.diff /ref <OTU.Data1.csv> /parts <OTU.Data2.csv> [/out <out.csv>]")>
    <Group(CLIGrouping.TaxonomyTools)>
    Public Function OTUDiff(args As CommandLine) As Integer
        Dim ref$ = args("/ref")
        Dim parts$ = args("/parts")
        Dim out = args.GetValue("/out", parts.TrimSuffix & "-" & ref.BaseName & ".diff.csv")
        Dim diff As New List(Of String)
        Dim partsId = parts.LoadCsv(Of OTUData).Select(Function(x) x.OTU).Distinct.AsList

        For Each x In ref.LoadCsv(Of OTUData)
            If partsId.IndexOf(x.OTU) = -1 Then
                diff += x.OTU
            End If
        Next

        Return diff.FlushAllLines(out).CLICode
    End Function

    <ExportAPI("/MapHits.list", Usage:="/MapHits.list /in <in.csv> [/out <out.txt>]")>
    <Argument("/in", AcceptTypes:={GetType(MapHits), GetType(MapHit)})>
    Public Function GetMapHitsList(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim data = [in].LoadCsv(Of MapHits)
        Dim list$() = data.Select(Function(x) x.MapHits).IteratesALL.Distinct.ToArray
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".maphits.list.txt")
        Return list.FlushAllLines(out, Encodings.ASCII).CLICode
    End Function

    <ExportAPI("/Taxonomy.Tree", Info:="Output taxonomy query info by a given NCBI taxid list.",
               Usage:="/Taxonomy.Tree /taxid <taxid.list.txt> /tax <ncbi_taxonomy:nodes/names> [/out <out.csv>]")>
    <Group(CLIGrouping.TaxonomyTools)>
    Public Function TaxonomyTree(args As CommandLine) As Integer
        Dim taxid As String = args("/taxid")
        Dim tax As String = args("/tax")
        Dim out As String = args.GetValue("/out", taxid.TrimSuffix & "-taxonomy.csv")
        Dim output As New List(Of IntegerTagged(Of String))
        Dim data As New NcbiTaxonomyTree(tax)
        Dim taxids As NamedValue(Of Integer)()
        Dim first = taxid.ReadFirstLine
        Dim tokens$() = first.Split(ASCII.TAB)

        If tokens.Length = 2 Then
            taxids = LinqAPI.Exec(Of NamedValue(Of Integer)) <=
                From s As String
                In taxid.IterateAllLines
                Let t As String() = s.Split(ASCII.TAB)
                Select New NamedValue(Of Integer) With {
                    .Name = t(Scan0),
                    .Value = CInt(Val(t(1)))
                }
        Else
            taxids = taxid.IterateAllLines.Select(Function(s) New NamedValue(Of Integer)("", CInt(Val(s)))).ToArray
        End If

        For Each x As NamedValue(Of Integer) In taxids
            Dim nodes = data.GetAscendantsWithRanksAndNames(x.Value, True)
            Dim tree = nodes.BuildBIOM

            output += New IntegerTagged(Of String) With {
                .Tag = x.Value%,
                .TagStr = x.Name,
                .Value = tree
            }
        Next

        Return output.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Taxonomy.Data",
               Usage:="/Taxonomy.Data /data <data.csv> /field.gi <GI> /gi2taxid <gi2taxid.list.txt> /tax <ncbi_taxonomy:nodes/names> [/out <out.csv>]")>
    <Group(CLIGrouping.TaxonomyTools)>
    <Group(CLIGrouping.GITools)>
    Public Function TaxonomyTreeData(args As CommandLine) As Integer
        Dim gi2taxid As String = args("/gi2taxid")
        Dim tax As String = args("/tax")
        Dim dataFile$ = args("/data")
        Dim giFieldName = args("/field.gi")
        Dim out As String = args.GetValue("/out", dataFile.TrimSuffix & "-taxonomy.csv")
        Dim data As EntityObject() =
            dataFile.LoadCsv(Of EntityObject)(
            maps:={
                {giFieldName, NameOf(EntityObject.ID)}
            })
        Dim giMapTaxid As BucketDictionary(Of Integer, Integer) =
            NCBI.Taxonomy.AcquireAuto(gi2taxid)
        Dim taxTree As New NcbiTaxonomyTree(tax)

        For Each x As EntityObject In data
            Dim gi% = CInt(x.ID)

            If giMapTaxid.ContainsKey(gi%) Then
                Dim taxid% = giMapTaxid(gi%)
                Dim nodes = taxTree.GetAscendantsWithRanksAndNames(taxid, True)
                Dim tree$ = nodes.BuildBIOM
                Dim Taxonomy$ = TaxonomyNode.Taxonomy(nodes)

                x.Properties("Taxonomy") = Taxonomy
                x.Properties("Taxonomy.BIOM") = tree
                x.Properties("taxid") = taxid
                If taxTree.Taxonomy.ContainsKey(taxid) Then
                    x.Properties("Tax.Name") = taxTree(taxid).name
                End If
            Else
                Call x.GetJson.Warning
            End If
        Next

        Return data.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Taxonomy.Maphits.Overview",
               Usage:="/Taxonomy.Maphits.Overview /in <in.DIR> [/out <out.csv>]")>
    <Group(CLIGrouping.TaxonomyTools)>
    Public Function TaxidMapHitViews(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimDIR & ".overviews.csv")
        Dim data = MapHit.SamplesView([in]).ToArray
        Return data.SaveTo(out).CLICode
    End Function

    Public Class MapHit : Inherits MapHits

        Public Property Name As String

        <Column("Taxonomy.Name")>
        Public Property TaxonomyName As String
        Public Property Taxonomy As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Iterator Function SamplesView(DIR$) As IEnumerable(Of MapHit)
            Dim data As New List(Of NamedValue(Of MapHit()))

            For Each file$ In ls - l - r - "*.csv" <= DIR
                data += New NamedValue(Of MapHit()) With {
                    .Name = file.BaseName,
                    .Value = file.LoadCsv(Of MapHit)
                }
            Next

            ' grouping by taxid
            Dim tg = (From x As NamedValue(Of MapHit())
                      In data
                      Select x.Value _
                          .Select(Function(o) (sample:=x.Name, taxid:=o))) _
                          .IteratesALL _
                          .GroupBy(Function(x) x.taxid.taxid)

            For Each tax In tg
                Dim out As (sample$, taxid As MapHit)() = tax.ToArray
                Dim samples As Dictionary(Of String, String) =
                    out _
                    .GroupBy(Function(x) x.sample) _
                    .ToDictionary(Function(x) x.Key,
                                  Function(x) x.Select(
                                  Function(m) m.taxid.MapHits.Length).Sum.ToString)

                Yield New MapHit With {
                    .MapHits = tax.Select(Function(x) x.taxid.MapHits).IteratesALL.Distinct.ToArray,
                    .taxid = tax.Key,
                    .Name = out(Scan0).taxid.Name,
                    .Taxonomy = out(Scan0).taxid.Taxonomy,
                    .TaxonomyName = out(Scan0).taxid.TaxonomyName,
                    .Data = samples
                }
            Next
        End Function
    End Class
End Module
