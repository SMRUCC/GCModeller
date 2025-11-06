#Region "Microsoft.VisualBasic::33ce18febaa6167ac17d90006fde5bcf, analysis\SequenceToolkit\SequenceTools\CLI\NTProps.vb"

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


    ' Code Statistics:

    '   Total Lines: 300
    '    Code Lines: 234 (78.00%)
    ' Comment Lines: 25 (8.33%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 41 (13.67%)
    '     File Size: 13.96 KB


    ' Module Utilities
    ' 
    '     Function: __lociFa, __segments, __where, ConvertMirrors, ConvertMirrorsBatch
    '               ConvertsAuto, MirrorContext, MirrorContextBatch, MirrorGroups, MirrorGroupsBatch
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Darwinism.HPC.Parallel.ThreadTask
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.IO.Linq
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Text
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
    <ArgumentAttribute("/in", False,
              AcceptTypes:={
                GetType(ImperfectPalindrome),
                GetType(ReverseRepeats),
                GetType(Repeats),
                GetType(PalindromeLoci)})>
    <ArgumentAttribute("/out", True, AcceptTypes:={GetType(SimpleSegment)}, Out:=True)>
    <Group(CLIGrouping.NTPropertyTools)>
    Public Function ConvertsAuto(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".Locis.Csv")
        Dim df As IO.File = IO.File.Load([in])
        Dim result As SimpleSegment() = df.ConvertsAuto
        Return result.SaveTo(out).CLICode
    End Function

    <ExportAPI("/SimpleSegment.Mirrors",
               Usage:="/SimpleSegment.Mirrors /in <in.csv> [/out <out.csv>]")>
    <ArgumentAttribute("/in", False, AcceptTypes:={GetType(PalindromeLoci)})>
    <ArgumentAttribute("/out", True, AcceptTypes:={GetType(SimpleSegment)}, Out:=True)>
    <Group(CLIGrouping.NTPropertyTools)>
    Public Function ConvertMirrors(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".SimpleSegments.Csv")
        Dim data As PalindromeLoci() = [in].LoadCsv(Of PalindromeLoci)
        Dim sites As SimpleSegment() = data.Select(AddressOf MirrorsLoci)

        Return sites.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 对位点进行分组操作方便进行MEME分析
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Mirrors.Group",
               Usage:="/Mirrors.Group /in <mirrors.Csv> [/batch /fuzzy <-1> /out <out.DIR>]")>
    <ArgumentAttribute("/fuzzy", True,
                   Description:="-1 means group sequence by string equals compared, and value of 0-1 means using string fuzzy compare.")>
    <Group(CLIGrouping.NTPropertyTools)>
    Public Function MirrorGroups(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim outDIR As String = args.GetValue("/out", [in].TrimSuffix)
        Dim data As PalindromeLoci() = [in].LoadCsv(Of PalindromeLoci)
        Dim cut As Double = args.GetValue("/fuzzy", -1.0R)
        Dim batch As Boolean = args("/batch")

        If cut > 0 Then
            For Each g As GroupResult(Of PalindromeLoci, String) In data.FuzzyGroups(
                Function(x) x.Loci, cut, parallel:=Not batch)

                Dim fa As FastaSeq() =
                    LinqAPI.Exec(Of FastaSeq) <= From x As PalindromeLoci In g.Group Select x.__lociFa
                Dim path As String = $"{outDIR}/{g.Tag}.fasta"

                Call New FastaFile(fa).Save(path, Encodings.ASCII)
            Next
        Else
            For Each g In (From x As PalindromeLoci In data Select x Group x By x.Loci Into Group)
                Dim fa As FastaSeq() =
                    LinqAPI.Exec(Of FastaSeq) <= From x As PalindromeLoci In g.Group Select x.__lociFa
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
    Private Function __lociFa(x As PalindromeLoci) As FastaSeq
        Dim uid As String = x.MappingLocation.ToString.Replace(" ", "_")
        Dim atrs As String() = {uid, x.Loci}

        Return New FastaSeq With {
            .Headers = atrs,
            .SequenceData = x.Loci & x.Palindrome
        }
    End Function

    <ExportAPI("/Mirrors.Group.Batch",
               Usage:="/Mirrors.Group.Batch /in <mirrors.DIR> [/fuzzy <-1> /out <out.DIR> /num_threads <-1>]")>
    <Group(CLIGrouping.NTPropertyTools)>
    Public Function MirrorGroupsBatch(args As CommandLine) As Integer
        Dim inDIR As String = args - "/in"
        Dim CLI As New List(Of String)
        Dim fuzzy As String = args.GetValue("/fuzzy", "-1")
        Dim num_threads As Integer = args.GetValue("/num_threads", -1)
        Dim task As Func(Of String, String) =
            Function(path) _
                $"{GetType(Utilities).API(NameOf(MirrorGroups))} /in {path.CLIPath} /batch /fuzzy {fuzzy}"

        For Each file As String In ls - l - r - wildcards("*.csv") <= inDIR
            CLI += task(file)
        Next

        Return BatchTasks.SelfFolks(CLI, LQuerySchedule.AutoConfig(num_threads))
    End Function

    <ExportAPI("/SimpleSegment.Mirrors.Batch",
             Usage:="/SimpleSegment.Mirrors.Batch /in <in.DIR> [/out <out.DIR>]")>
    <Group(CLIGrouping.NTPropertyTools)>
    Public Function ConvertMirrorsBatch(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim out As String = args.GetValue("/out", [in].TrimDIR & ".SimpleSegments/")

        For Each file As String In ls - l - r - wildcards("*.csv") <= [in]
            Dim data As PalindromeLoci() = file.LoadCsv(Of PalindromeLoci)
            Dim path As String = $"{out}/{file.BaseName}.Csv"
            Dim sites As SimpleSegment() = data.Select(AddressOf MirrorsLoci)

            Call sites.SaveTo(path)
        Next

        Return 0
    End Function

    ''' <summary>
    ''' 过滤得到基因组上下文之中的上游回文位点
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Mirrors.Context.Batch",
               Info:="This function will convert the mirror data to the simple segment object data",
               Usage:="/Mirrors.Context.Batch /in <mirrors.csv.DIR> /PTT <genome.ptt.DIR> [/trans /strand <+/-> /out <out.csv> /stranded /dist <500bp> /num_threads -1]")>
    <ArgumentAttribute("/trans", True,
                   Description:="Enable this option will using genome_size minus loci location for the location correction, only works in reversed strand.")>
    <Group(CLIGrouping.NTPropertyTools)>
    Public Function MirrorContextBatch(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim PTT_DIR As String = args("/PTT")
        Dim trans As Boolean = args("/trans")
        Dim strand As String = args.GetValue("/strand", "+")
        Dim dist As Integer = args.GetValue("/dist", 500)
        Dim stranded As Boolean = args("/stranded")
        Dim EXPORT As String = args.GetValue("/out", [in].TrimDIR & ".genomics_context_" & dist & "/")
        Dim cliTask As Func(Of String, String, String) =
            Function(mirror, PTT)
                Dim sTrans As String = If(trans, "/trans", "")
                Dim sstranded As String = If(stranded, "/stranded", "")
                Dim out As String = EXPORT & "/" & mirror.BaseName & ".Csv"
                Return $"{GetType(Utilities).API(NameOf(MirrorContext))} /in {mirror.CLIPath} /PTT {PTT.CLIPath} {sTrans} /strand {strand} /out {out.CLIPath} {sstranded} /dist {dist}"
            End Function

        Dim mirrors As String() = LinqAPI.Exec(Of String) <= (ls - l - r - wildcards("*.Csv") <= [in])
        Dim PTTs As String() = LinqAPI.Exec(Of String) <= (ls - l - r - wildcards("*.ptt") <= PTT_DIR)
        Dim CLI As New List(Of String)

        For Each mirror As String In mirrors
            Dim mName As String = mirror.BaseName

            For Each PTT As String In PTTs
                Dim pName As String = PTT.BaseName.NormalizePathString
                If InStr(mName, pName) > 0 Then
                    CLI += cliTask(mirror, PTT)
                    Continue For
                End If
            Next
        Next

        Dim n As Integer = args.GetValue("/num_threads", -1)

        Return BatchTasks.SelfFolks(CLI, LQuerySchedule.AutoConfig(n))
    End Function

    ''' <summary>
    ''' 过滤得到基因组上下文之中的上游回文位点
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Mirrors.Context",
               Info:="This function will convert the mirror data to the simple segment object data",
               Usage:="/Mirrors.Context /in <mirrors.csv> /PTT <genome.ptt> [/trans /strand <+/-> /out <out.csv> /stranded /dist <500bp>]")>
    <ArgumentAttribute("/trans", True,
                   Description:="Enable this option will using genome_size minus loci location for the location correction, only works in reversed strand.")>
    <Group(CLIGrouping.NTPropertyTools)>
    Public Function MirrorContext(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim PTT As String = args("/PTT")
        Dim strand As String = args.GetValue("/strand", "+")
        Dim stranded As Boolean = args("/stranded")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "." & PTT.BaseName & "." & strand & ".csv")
        Dim context As PTT = TabularFormat.PTT.Load(PTT)
        Dim genome As New GenomeContextProvider(Of GeneBrief)(context)  ' 构建基因组的上下文模型
        Dim lStrand As Strands = strand.GetStrand
        Dim dist As Integer = args.GetValue("/dist", 500)
        Dim trans As Boolean = args("/trans")

        If trans Then
            If lStrand <> Strands.Reverse Then
                trans = False   ' 只允许反向链的情况下使用
            End If
        End If

        If trans Then
            Call $"Reversed strand location will be transformed by genome size!".debug
            out = out.TrimSuffix & ".trans.Csv"
        End If

        Dim gsize As Integer = context.Size
        Dim task As Func(Of PalindromeLoci, KeyValuePair(Of PalindromeLoci, Relationship(Of GeneBrief)())) =
           Function(x)
               Dim left As Integer = x.MappingLocation.left
               Dim right As Integer = x.MappingLocation.right

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
