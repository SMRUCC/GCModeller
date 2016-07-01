Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.Linq
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Parallel.Linq
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Partial Module Utilities

    ''' <summary>
    ''' 自动根据文件的头部进行转换
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/SimpleSegment.AutoBuild",
               Usage:="/SimpleSegment.AutoBuild /in <locis.csv> [/out <out.csv>]")>
    <ParameterInfo("/in", False, AcceptTypes:={GetType(DocumentStream.File)})>
    <ParameterInfo("/out", True, AcceptTypes:={GetType(SimpleSegment)}, Out:=True)>
    Public Function ConvertsAuto(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & ".Locis.Csv")
        Dim df As DocumentStream.File = DocumentStream.File.Load([in])
        Dim result As SimpleSegment() = df.ConvertsAuto
        Return result.SaveTo(out).CLICode
    End Function

    <ExportAPI("/SimpleSegment.Mirrors",
               Usage:="/SimpleSegment.Mirrors /in <in.csv> [/out <out.csv>]")>
    <ParameterInfo("/in", False, AcceptTypes:={GetType(PalindromeLoci)})>
    <ParameterInfo("/out", True, AcceptTypes:={GetType(SimpleSegment)}, Out:=True)>
    Public Function ConvertMirrors(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & ".SimpleSegments.Csv")
        Dim data As PalindromeLoci() = [in].LoadCsv(Of PalindromeLoci)
        Dim sites As SimpleSegment() = data.ToArray(AddressOf MirrorsLoci)

        Return sites.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 对位点进行分组操作方便进行MEME分析
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Mirrors.Group",
               Usage:="/Mirrors.Group /in <mirrors.Csv> [/batch /fuzzy <-1> /out <out.DIR>]")>
    <ParameterInfo("/fuzzy", True,
                   Description:="-1 means group sequence by string equals compared, and value of 0-1 means using string fuzzy compare.")>
    Public Function MirrorGroups(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim outDIR As String = args.GetValue("/out", [in].TrimFileExt)
        Dim data As PalindromeLoci() = [in].LoadCsv(Of PalindromeLoci)
        Dim cut As Double = args.GetValue("/fuzzy", -1.0R)
        Dim batch As Boolean = args.GetBoolean("/batch")

        If cut > 0 Then
            For Each g As GroupResult(Of PalindromeLoci, String) In data.FuzzyGroups(
                Function(x) x.Loci, cut, parallel:=Not batch)

                Dim fa As FastaToken() =
                    LinqAPI.Exec(Of FastaToken) <= From x As PalindromeLoci In g.Group Select x.__lociFa
                Dim path As String = $"{outDIR}/{g.Tag}.fasta"

                Call New FastaFile(fa).Save(path, Encodings.ASCII)
            Next
        Else
            For Each g In (From x As PalindromeLoci In data Select x Group x By x.Loci Into Group)
                Dim fa As FastaToken() =
                    LinqAPI.Exec(Of FastaToken) <= From x As PalindromeLoci In g.Group Select x.__lociFa
                Dim path As String = $"{outDIR}/{g.Loci}.fasta"

                Call New FastaFile(fa).Save(path, Encodings.ASCII)
            Next
        End If

        Return 0
    End Function

    ''' <summary>
    ''' Converts the mirror palindrome site into a fasta sequence
    ''' </summary>
    ''' <param name="x"></param>
    ''' <returns></returns>
    <Extension>
    Private Function __lociFa(x As PalindromeLoci) As FastaToken
        Dim uid As String = x.MappingLocation.ToString.Replace(" ", "_")
        Dim atrs As String() = {uid, x.Loci}

        Return New FastaToken With {
            .Attributes = atrs,
            .SequenceData = x.Loci & x.Palindrome
        }
    End Function

    <ExportAPI("/Mirrors.Group.Batch",
               Usage:="/Mirrors.Group.Batch /in <mirrors.DIR> [/fuzzy <-1> /out <out.DIR> /num_threads <-1>]")>
    Public Function MirrorGroupsBatch(args As CommandLine) As Integer
        Dim inDIR As String = args - "/in"
        Dim CLI As New List(Of String)
        Dim fuzzy As String = args.GetValue("/fuzzy", "-1")
        Dim num_threads As Integer = args.GetValue("/num_threads", -1)
        Dim task As Func(Of String, String) =
            Function(path) _
                $"{GetType(Utilities).API(NameOf(MirrorGroups))} /in {path.CliPath} /batch /fuzzy {fuzzy}"

        For Each file As String In ls - l - r - wildcards("*.csv") <= inDIR
            CLI += task(file)
        Next

        Return App.SelfFolks(CLI, LQuerySchedule.AutoConfig(num_threads))
    End Function

    <ExportAPI("/SimpleSegment.Mirrors.Batch",
             Usage:="/SimpleSegment.Mirrors.Batch /in <in.DIR> [/out <out.DIR>]")>
    Public Function ConvertMirrorsBatch(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim out As String = args.GetValue("/out", [in].TrimDIR & ".SimpleSegments/")

        For Each file As String In ls - l - r - wildcards("*.csv") <= [in]
            Dim data As PalindromeLoci() = file.LoadCsv(Of PalindromeLoci)
            Dim path As String = $"{out}/{file.BaseName}.Csv"
            Dim sites As SimpleSegment() = data.ToArray(AddressOf MirrorsLoci)

            Call sites.SaveTo(path)
        Next

        Return 0
    End Function

    ''' <summary>
    ''' 过滤得到基因组上下文之中的上游回文位点
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Mirrors.Context",
               Info:="This function will convert the mirror data to the simple segment object data",
               Usage:="/Mirrors.Context /in <mirrors.csv> /PTT <genome.ptt> [/trans /strand <+/-> /out <out.csv> /stranded /dist <500bp>]")>
    <ParameterInfo("/trans", True,
                   Description:="Enable this option will using genome_size minus loci location for the location correction, only works in reversed strand.")>
    Public Function MirrorContext(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim PTT As String = args("/PTT")
        Dim strand As String = args.GetValue("/strand", "+")
        Dim stranded As Boolean = args.GetBoolean("/stranded")
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & "." & PTT.BaseName & "." & strand & ".csv")
        Dim context As PTT = TabularFormat.PTT.Load(PTT)
        Dim genome As New GenomeContextProvider(Of GeneBrief)(context)  ' 构建基因组的上下文模型
        Dim lStrand As Strands = strand.GetStrand
        Dim dist As Integer = args.GetValue("/dist", 500)
        Dim trans As Boolean = args.GetBoolean("/trans")

        If trans Then
            If lStrand <> Strands.Reverse Then
                trans = False   ' 只允许反向链的情况下使用
            End If
        End If

        If trans Then
            Call $"Reversed strand location will be transformed by genome size!".__DEBUG_ECHO
            out = out.TrimFileExt & ".trans.Csv"
        End If

        Dim gsize As Integer = context.Size
        Dim task As Func(Of PalindromeLoci, KeyValuePair(Of PalindromeLoci, Relationship(Of GeneBrief)())) =
           Function(x)
               Dim left As Integer = x.MappingLocation.Left
               Dim right As Integer = x.MappingLocation.Right

               If trans Then
                   left = gsize - left
                   right = gsize - right

                   x.Start = left
                   x.PalEnd = right

                   Dim null = x.MappingLocation(reset:=True)
               End If

               Dim loci As New NucleotideLocation(left, right, lStrand) ' 在这里用户自定义链的方向
               Dim rels = genome.GetAroundRelated(loci, stranded, dist)
               Return New KeyValuePair(Of PalindromeLoci, Relationship(Of GeneBrief)())(x, rels)
           End Function

        Using writer As New WriteStream(Of SimpleSegment)(out)
            Call DataStream.OpenHandle([in]) _
                .ForEachBlock(Of PalindromeLoci)(
                    Sub(array)
                        Dim result = LQuerySchedule.LQuery(
                        array,
                        task,
                        AddressOf __where,
                        TaskPartitions.PartTokens(array.Length))
                        Dim segs As SimpleSegment() =
                            LinqAPI.Exec(Of SimpleSegment) <= result.Select(AddressOf __segments)

                        Call writer.Flush(segs)
                    End Sub, 1024)
        End Using

        Return 0
    End Function

    <Extension>
    Private Iterator Function __segments(rels As KeyValuePair(Of PalindromeLoci, Relationship(Of GeneBrief)())) As IEnumerable(Of SimpleSegment)
        Dim seg As SimpleSegment = rels.Key.MirrorsLoci

        For Each gene As Relationship(Of GeneBrief)
            In rels.Value.Where(Function(x) x.Relation = SegmentRelationships.UpStream OrElse
            x.Relation = SegmentRelationships.UpStreamOverlap)

            Dim loci As New SimpleSegment(seg, gene.Gene.Synonym)
            Dim atg As Integer = loci.GetsATGDist(gene.Gene)
            loci.ID = loci.ID & ":" & atg

            Yield loci
        Next
    End Function

    Private Function __where(rels As KeyValuePair(Of PalindromeLoci, Relationship(Of GeneBrief)())) As Boolean
        Return rels.Value.Length > 0 AndAlso
            rels.Value.Any(Function(r) _
                 r.Relation = SegmentRelationships.UpStream OrElse
                 r.Relation = SegmentRelationships.UpStreamOverlap)   ' 只会将包含有上游位点的位点过滤出来
    End Function
End Module