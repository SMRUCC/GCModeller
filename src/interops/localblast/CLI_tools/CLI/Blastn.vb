#Region "Microsoft.VisualBasic::693c4b5274ddc08a43ee8ebc27ea1928, ..\localblast\CLI_tools\CLI\Blastn.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Programs.CLIArgumentsBuilder
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/Export.Blastn", Usage:="/Export.Blastn /in <in.txt> [/out <out.csv>]")>
    <Group(CLIGrouping.BlastnTools)>
    Public Function ExportBlastn(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".Csv")

        Using IO As New __writeIO(out)  ' 打开文件流句柄
            Dim IOHandle As Action(Of BlastPlus.Query()) = AddressOf IO.InvokeWrite  ' 获取写文件的IO句柄函数指针
            Call BlastPlus.Transform(inFile, CHUNK_SIZE:=1024 * 1024 * 64, transform:=IOHandle)  ' 执行blast输出大文件分析的并行化查询，内存映射的缓冲块大小为 128GB 的高位内存
        End Using

        Return 0
    End Function

    Private Class __writeIO : Implements System.IDisposable

        ''' <summary>
        ''' 对象序列化串流句柄
        ''' </summary>
        ReadOnly IO As WriteStream(Of BBH.BestHit)

        ''' <summary>
        ''' 打开文件串流句柄
        ''' </summary>
        ''' <param name="handle"></param>
        Sub New(handle As String)
            IO = New WriteStream(Of BestHit)(handle)
        End Sub

        ''' <summary>
        ''' 执行流写入操作
        ''' </summary>
        ''' <param name="lstQuery"></param>
        Public Sub InvokeWrite(lstQuery As BLASTOutput.BlastPlus.Query())
            If lstQuery.IsNullOrEmpty Then
                Return
            End If

            Dim outStream As BBH.BestHit() =
                LinqAPI.Exec(Of BBH.BestHit) <= From x As Query
                                                In lstQuery.AsParallel
                                                Where Not x.SubjectHits.IsNullOrEmpty
                                                Select __creates(x)
#If DEBUG Then
            If outStream.Count > 0 Then
                Call Console.Write(".")
            End If
#End If

            Call IO.Flush(outStream)
        End Sub

        Private Shared Function __creates(query As BlastPlus.Query) As BBH.BestHit()
            Dim ntHits As IEnumerable(Of BlastnHit) =
                From x As BlastPlus.SubjectHit
                In query.SubjectHits
                Select DirectCast(x, BlastPlus.BlastnHit)
            Dim outStream As BBH.BestHit() =
                LinqAPI.Exec(Of BBH.BestHit) <= From x As BlastPlus.BlastnHit
                                                In ntHits.AsParallel
                                                Select New BBH.BestHit With {
                                                    .evalue = x.Score.Expect,
                                                    .Score = x.Score.Score,
                                                    .HitName = x.Name,
                                                    .hit_length = x.Length,
                                                    .identities = x.Score.Identities.Value,
                                                    .length_hit = x.LengthHit,
                                                    .length_hsp = x.SubjectLocation.FragmentSize,
                                                    .length_query = x.LengthQuery,
                                                    .Positive = x.Score.Positives.Value,
                                                    .QueryName = query.QueryName,
                                                    .query_length = query.QueryLength
                                                }
            Return outStream
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call IO.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class

    ''' <summary>
    ''' 这个函数是单线程执行的
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/blastn.Query",
               Info:="Using target fasta sequence query against all of the fasta sequence in target direcotry. This function is single thread.",
               Usage:="/blastn.Query /query <query.fna/faa> /db <db.DIR> [/thread /evalue 1e-5 /word_size <-1> /out <out.DIR>]")>
    <Argument("/thread", True, CLITypes.Boolean,
              Description:="Is this CLI api running in one of the processor in thread mode for a caller API ``/blastn.Query.All``")>
    <Group(CLIGrouping.BlastnTools)>
    Public Function BlastnQuery(args As CommandLine) As Integer
        Dim query As String = args("/query")
        Dim DbDIR As String = args("/db")
        Dim evalue As Double = args.GetValue("/evalue", 0.00001)
        Dim outDIR As String = args.GetValue("/out", query.TrimSuffix & ".Blastn/")
        Dim penalty As Integer = args.GetValue("/penalty", -1)
        Dim reward As Integer = args.GetValue("/reward", -1)
        Dim localblast As New Programs.BLASTPlus(GCModeller.FileSystem.GetLocalblast) With {
            .BlastnOptionalArguments = New BlastnOptionalArguments With {
                .WordSize = args.GetValue("/word_size", -1),
                .penalty = penalty,
                .reward = reward
            }
        }
        Dim isThread As Boolean = args.GetBoolean("/thread")

        For Each subject As String In ls - l - r - {"*.fna", "*.fa", "*.fsa", "*.fasta", "*.ffn"} <= DbDIR
            Dim out As String

            If Not isThread Then
                out = outDIR & "/" & basename(subject) & ".txt"
                Call localblast.FormatDb(subject, localblast.MolTypeNucleotide).Start(True)
            Else
                out = outDIR & "/" & query.BaseName & "-" & subject.BaseName & ".txt"
            End If

            Call localblast.Blastn(query, subject, out, evalue).Start(True)
        Next

        Return 0
    End Function

    <ExportAPI("/blastn.Query.All",
               Info:="Using the fasta sequence in a directory query against all of the sequence in another directory.",
               Usage:="/blastn.Query.All /query <query.fasta.DIR> /db <db.DIR> [/skip-format /evalue 10 /word_size <-1> /out <out.DIR> /parallel /penalty <penalty> /reward <reward>]")>
    <Group(CLIGrouping.BlastnTools)>
    Public Function BlastnQueryAll(args As CommandLine) As Integer
        Dim [in] As String = args("/query")
        Dim db As String = args("/db")
        Dim evalue As String = args.GetValue("/evalue", 10.0R)
        Dim out As String = args.GetValue("/out", [in].TrimDIR & "-" & db.BaseName & ".blastn.Query.All/")
        Dim ws As Integer = args.GetValue("/word_size", -1)
        Dim penalty As Integer = args.GetValue("/penalty", -1)
        Dim reward As Integer = args.GetValue("/reward", -1)
        Dim task As Func(Of String, String) =
            Function(fa) _
                $"{GetType(CLI).API(NameOf(BlastnQuery))} /query {fa.CLIPath} /db {db.CLIPath} /word_size {ws} /evalue {evalue} /thread /out {out.CLIPath} /penalty {penalty} /reward {reward}"
        Dim CLI As String() =
            (ls - l - r - {"*.fna", "*.fa", "*.fsa", "*.fasta", "*.ffn"} <= [in]).ToArray(task)

        If Not args.GetBoolean("/skip-format") Then
            Dim localblast As New Programs.BLASTPlus(GCModeller.FileSystem.GetLocalblast)

            For Each subject As String In ls - l - r - {"*.fna", "*.fa", "*.fsa", "*.fasta", "*.ffn"} <= db
                Call localblast.FormatDb(subject, localblast.MolTypeNucleotide).Start(True)
            Next
        End If

        Dim parallel As Boolean = args.GetBoolean("/parallel")
        Dim n As Integer = If(parallel, LQuerySchedule.CPU_NUMBER, 0)

        Return App.SelfFolks(CLI, parallel:=n)
    End Function

    <ExportAPI("/Export.blastnMaps",
               Usage:="/Export.blastnMaps /in <blastn.txt> [/best /out <out.csv>]")>
    <Argument("/best", True,
                   AcceptTypes:={GetType(Boolean)},
                   Description:="Only output the first hit result for each query as best?")>
    <Group(CLIGrouping.BlastnTools)>
    Public Function ExportBlastnMaps(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim best As Boolean = args.GetBoolean("/best")
        Dim out As String = args _
            .GetValue("/out", [in].TrimSuffix & $"{If(best, ".best", "")}.Csv")

        If [in].FileLength > 2L * 1024L * 1024L * 1024L Then
            ' 超大
            Using IO As New WriteStream(Of BlastnMapping)(out, metaKeys:={})
                Dim handle As Action(Of Query) =
                    IO.ToArray(Of Query)(AddressOf MapsAPI.CreateObject)
                Call BlastPlus.Transform(in$, 1024 * 1024 * 256L, handle)
            End Using
        Else
            Dim blastn As v228 = BlastPlus.TryParseUltraLarge([in])
            Dim maps As BlastnMapping() = MapsAPI.Export(blastn, best)
            Return maps.SaveTo(out)
        End If

        Return 0
    End Function

    <ExportAPI("/Export.blastnMaps.Batch", Info:="Multiple processor task.",
               Usage:="/Export.blastnMaps.Batch /in <blastn_out.DIR> [/best /out <out.DIR> /num_threads <-1>]")>
    <Group(CLIGrouping.BlastnTools)>
    Public Function ExportBlastnMapsBatch(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim out As String = args.GetValue("/out", [in].TrimDIR & "-blastnMaps/")
        Dim numThreads As Integer = args.GetValue("/num_threads", -1)
        Dim best = If(args.GetBoolean("/best"), "/best", "")
        Dim task As Func(Of String, String) =
            Function(path) _
                $"{GetType(CLI).API(NameOf(ExportBlastnMaps))} /in {path.CLIPath} {best} /out {(out & "/" & path.BaseName & ".Csv").CLIPath}"
        Dim CLI As String() = (ls - l - r - wildcards("*.txt") <= [in]).ToArray(task)

        Return App.SelfFolks(CLI, numThreads)
    End Function

    <ExportAPI("/Export.blastnMaps.Write",
               Info:="Exports large amount of blastn output files and write all data into a specific csv file.",
               Usage:="/Export.blastnMaps.Write /in <blastn_out.DIR> [/best /out <write.csv>]")>
    <Group(CLIGrouping.BlastnTools)>
    <Argument("/best", True, CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="Only export the top best blastn alignment hit?")>
    <Argument("/out", True, CLITypes.File,
              AcceptTypes:={GetType(BlastnMapping)},
              Description:="Blastn alignment maps data.")>
    <Argument("/in", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(String)},
              Description:="The directory path that contains the blastn output data.")>
    Public Function ExportBlastnMapsBatchWrite(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim best As Boolean = args.GetBoolean("/best")
        Dim out As String = args.GetValue("/out", [in].TrimDIR & $"-blastnMaps{If(best, "-top_best", "")}.csv")
        Dim LQuery = From path As String
                     In (ls - l - r - wildcards("*.txt") <= [in]).AsParallel
                     Let blastn As v228 = Parser.ParsingSizeAuto(path)
                     Let maps = MapsAPI.Export(blastn, best, path.BaseName, False)
                     Where Not maps.IsNullOrEmpty
                     Select maps

        Using writer As New WriteStream(Of BlastnMapping)(out,,, {"track"})
            For Each block As BlastnMapping() In LQuery
                Call writer.Flush(block, False)
                Call block.First.Extensions("track").__DEBUG_ECHO
            Next

            Return 0
        End Using
    End Function

    <ExportAPI("/Export.blastnMaps.littles",
               Usage:="/Export.blastnMaps.littles /in <blastn.txt.DIR> [/out <out.csv.DIR>]")>
    <Group(CLIGrouping.BlastnTools)>
    Public Function ExportBlastnMapsSmall(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim out As String = args.GetValue("/out", [in].TrimDIR & "-BlastnMaps/")

        For Each file As String In ls - l - r - wildcards("*.txt") <= [in]
            Dim blastn As v228 = BlastPlus.TryParse(file)
            Dim maps As BlastnMapping() = MapsAPI.Export(blastn)
            Dim path As String = out & "/" & file.BaseName & ".Csv"

            Call maps.SaveTo(path)
        Next

        Return 0
    End Function

    <ExportAPI("/Chromosomes.Export",
               Usage:="/Chromosomes.Export /reads <reads.fasta/DIR> /maps <blastnMappings.Csv/DIR> [/out <outDIR>]")>
    <Group(CLIGrouping.BlastnTools)>
    Public Function ChromosomesBlastnResult(args As CommandLine) As Integer
        Dim [in] As String = args("/reads")
        Dim maps As String = args("/maps")
        Dim out As String = args.GetValue("/out", maps.TrimSuffix & "-" & [in].BaseName & "/")
        Dim fasta As IEnumerable(Of FastaToken)
        Dim mappings As IEnumerable(Of BlastnMapping)

        If [in].DirectoryExists Then
            fasta = [in].__loads()
        Else
            fasta = New StreamIterator([in]).ReadStream
        End If

        If maps.DirectoryExists Then
            mappings = maps.__loadsMaps
        Else
            mappings = maps.LoadCsv(Of BlastnMapping)
        End If

        Dim chrs = (From x As BlastnMapping In mappings Select x Group x By x.Reference Into Group)
        Dim hash As Dictionary(Of String, FastaToken()) = (From x As FastaToken
                                                           In fasta
                                                           Select x
                                                           Group x By x.Title Into Group) _
                                                                .ToDictionary(Function(x) x.Title,
                                                                              Function(x) x.Group.ToArray)
        For Each chrom In chrs
            Dim path As String = out & "/" & chrom.Reference.NormalizePathString & ".fasta"
            Dim c = LinqAPI.Exec(Of FastaToken) <= From read
                                                   In (From x As BlastnMapping
                                                       In chrom.Group
                                                       Select x  ' 因为可能会有多个位置被比对上，所以在这里还需要再进行一次Group操作
                                                       Group x By x.ReadQuery Into Count)
                                                   Where hash.ContainsKey(read.ReadQuery)
                                                   Select hash(read.ReadQuery)
            Call New FastaFile(c).Save(path)
        Next

        Return 0
    End Function

    <Extension>
    Private Iterator Function __loadsMaps(DIR As String) As IEnumerable(Of BlastnMapping)
        For Each file As String In ls - l - r - wildcards("*.Csv") <= DIR
            For Each map As BlastnMapping In file.LoadCsv(Of BlastnMapping)
                Yield map
            Next
        Next
    End Function

    <Extension>
    Private Iterator Function __loads(DIR As String) As IEnumerable(Of FastaToken)
        For Each file As String In ls - l - r - wildcards("*.fasta", "*.fsa", "*.fa", "*.fna") <= DIR
            For Each fa As FastaToken In New FastaFile(file)
                Yield fa
            Next
        Next
    End Function

    <ExportAPI("/Blastn.Maps.Taxid",
               Usage:="/Blastn.Maps.Taxid /in <blastnMapping.csv> /2taxid <acc2taxid.tsv/gi2taxid.dmp> [/gi2taxid /trim /tax <NCBI_taxonomy:nodes/names> /out <out.csv>]")>
    <Group(CLIGrouping.BlastnTools)>
    <Argument("/gi2taxid", True, AcceptTypes:={GetType(Boolean)}, Description:="The 2taxid data source is comes from gi2taxid, by default is acc2taxid.")>
    Public Function BlastnMapsTaxonomy(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim x2taxid As String = args("/2taxid")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".taxid.csv")
        Dim is_gi2taxid As Boolean = args.GetBoolean("/gi2taxid")
        Dim maps As BlastnMapping() = [in].LoadCsv(Of BlastnMapping)
        Dim notFound As New List(Of String)
        Dim taxDIR$ = args("/tax")
        Dim tax As NcbiTaxonomyTree = Nothing
        Dim trimLong As Boolean = args.GetBoolean("/trim")
        Dim taxid As New Value(Of Integer)
        Dim mapping As TaxidMaps.Mapping = If(
            is_gi2taxid,
            TaxidMaps.MapByGI(x2taxid),
            TaxidMaps.MapByAcc(x2taxid))

        If taxDIR.DirectoryExists Then
            tax = New NcbiTaxonomyTree(taxDIR)
        End If

        Call "All data load done!".__DEBUG_ECHO

        Dim taxidFromRef As Mapping = Reference2Taxid(mapping, is_gi2taxid)

        For Each x As BlastnMapping In maps
            If trimLong Then
                x.Reference = Mid(x.Reference, 1, 255)
            End If

            If (taxid = taxidFromRef(x.Reference)) > -1 Then
                x.Extensions("taxid") = +taxid

                If Not tax Is Nothing Then
                    Dim nodes = tax.GetAscendantsWithRanksAndNames(+taxid, True)
                    Dim tree = TaxonomyNode.BuildBIOM(nodes)
                    Dim name = tax(taxid)?.name

                    x.Extensions("Taxonomy.Name") = name
                    x.Extensions("Taxonomy") = tree
                End If
            Else
                notFound += CStr(x.Reference)
                Call x.Reference.Warning
            End If
        Next

        Call notFound.FlushAllLines(out.TrimSuffix & ".not-found.txt")

        Return maps.SaveTo(out).CLICode
    End Function

    <ExportAPI("/BlastnMaps.Select",
               Usage:="/BlastnMaps.Select /in <reads.id.list.txt> /data <blastn.maps.csv> [/out <out.csv>]")>
    <Group(CLIGrouping.BlastnTools)>
    Public Function SelectMaps(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim data As String = args("/data")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-" & data.BaseName & ".csv")
        Dim list$() = [in].ReadAllLines
        Dim maps As BlastnMapping() = data.LoadCsv(Of BlastnMapping)
        Dim mapsData = (From x As BlastnMapping
                        In maps
                        Select x
                        Group x By x.ReadQuery Into Group) _
                             .ToDictionary(Function(x) x.ReadQuery,
                                           Function(x) x.Group.ToArray)
        Dim selects As New List(Of BlastnMapping)

        For Each id$ In list
            If mapsData.ContainsKey(id) Then
                selects += mapsData(id)
            Else
                Call id.Warning
            End If
        Next

        Return selects.SaveTo(out).CLICode
    End Function

    <ExportAPI("/BlastnMaps.Select.Top", Usage:="/BlastnMaps.Select.Top /in <maps.csv> [/out <out.csv>]")>
    <Group(CLIGrouping.BlastnTools)>
    Public Function TopBlastnMapReads(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-TopBest.csv")
        Dim data = [in].LoadCsv(Of BlastnMapping)
        Dim best = (From x As BlastnMapping
                    In data
                    Select x
                    Group x By x.ReadQuery Into Group) _
                   .ToArray(Function(x) x.Group.OrderByDescending(Function(r) r.Identities).First)
        Return best.SaveTo(out).CLICode
    End Function

    <ExportAPI("/BlastnMaps.Summery", Usage:="/BlastnMaps.Summery /in <in.DIR> [/split ""-"" /out <out.csv>]")>
    <Group(CLIGrouping.BlastnTools)>
    Public Function BlastnMapsSummery(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim out As String = args.GetValue("/out", inDIR.TrimDIR & ".BlastnMaps.Summery.csv")
        Dim deli$ = args.GetValue("/split", "-")

        Const track$ = NameOf(track)

        Using write As New WriteStream(Of BlastnMapping)(out,,, {track})
            For Each file$ In ls - l - r - "*.csv" <= inDIR
                Dim data = file.LoadCsv(Of BlastnMapping)
                Dim name$ = Strings.Split(file.BaseName, deli).JoinBy(", ")

                For Each x In data
                    x.Extensions(track) = name
                Next

                Call write.Flush(data)
            Next

            Return 0
        End Using
    End Function

    <ExportAPI("/BlastnMaps.Match.Taxid",
               Usage:="/BlastnMaps.Match.Taxid /in <maps.csv> /acc2taxid <acc2taxid.DIR> [/out <out.tsv>]")>
    <Group(CLIGrouping.BlastnTools)>
    Public Function MatchTaxid(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim acc2taxid As String = args("/acc2taxid")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".taxid_matched.tsv")

        Using writer As StreamWriter = out.OpenWriter(Encodings.ASCII)
            Dim data = [in].LoadCsv(Of BlastnMapping)
            Dim acc As IEnumerable(Of String) = data _
                .Select(Function(x) x.Reference _
                    .Split _
                    .First _
                    .Split("."c) _
                    .First)

            For Each line$ In Accession2Taxid.Matchs(acc, acc2taxid)
                Call writer.WriteLine(line$)
            Next

            Return 0
        End Using
    End Function
End Module
