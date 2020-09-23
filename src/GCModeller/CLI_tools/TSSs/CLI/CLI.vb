#Region "Microsoft.VisualBasic::2287167c4a4cc16fda98aee1183a9a3e, CLI_tools\TSSs\CLI\CLI.vb"

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
    '     Function: __TSSsLoci, GenomeContent, GenomeContext, IdentifyUTRs, siRNAPredictions
    '               TestSites, VisualReads
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Analysis.RNA_Seq
Imports SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools
Imports SMRUCC.genomics.Analysis.RNA_Seq.TSSsTools.DocumentFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation
Imports stdNum = System.Math

Module CLI

    <ExportAPI("/Reads.Visual", Usage:="/Reads.Visual /in <reads.count.csv> [/out <outDIR>]")>
    Public Function VisualReads(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim reads = inFile.LoadCsv(Of TSSsTools.ReadsCount)
        Dim res = TSSsTools.ReadsPlot.Plot(reads)

    End Function

    <ExportAPI("IdentifyUTRs",
               Info:="Function using the real genome context to identified each genes' 5'UTR and 3'UTR.",
               Usage:="IdentifyUTRs -ptt <genome.ptt> -reads <transcripts_reads.csv> [/unstrand -out <utr_outs.csv> /activity 0.65 /prefix <TSSs_>]")>
    <Argument("/activity", True,
                   Description:="Sets the minimum level of expression (for a UTR region and ncRNA to be considered expressed) in this Replicate based on 
the average number of reads per nucleotide in this Replicate and the specified transcript sensitivity between 0.0 and 1.0, inclusive.")>
    Public Function IdentifyUTRs(args As CommandLine) As Integer
        Dim PTT As String = args("-ptt")
        Dim Reads = args("-reads")
        Dim Unstrand As Boolean = args.GetBoolean("/unstrand")
        Dim minExpr As Double = args.GetValue("/activity", 0.65)
        Dim prefix As String = args.GetValue("/prefix", basename(Reads).Split("."c).First & ".TSSs_")
        Dim Transcripts = Transcriptome.UTRs.IdentifyUTRs.identifyUTRs(
            SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT.Load(PTT),
            Unstrand,
            Reads,
            minExpression:=minExpr,
            prefix:=prefix)
        Dim out As String = args.GetValue("-out", Reads & ".TranscriptsUTR.csv")

        Return Transcripts.SaveTo(out, False).CLICode
    End Function

    <ExportAPI("-Identify.siRNA",
               Usage:="-Identify.siRNA -in <transcripts.csv> /genome.size <nt_total_length> [/reads.shared 5,30 /unstrand /ig.min 1000]",
               Info:="You can manual setup genome.Size in a given length, or just left it blank let the program to detecting automatically but this maybe makes some mistakes..... 
               Only the sites which is testing successfully will be output.")>
    Public Function siRNAPredictions(args As CommandLine) As Integer
        Dim Transcripts As String = args("-in")
        Dim genomeSize As Long = args.GetInt64("/genome.size")
        Dim readsShared = args.GetValue(Of String)("/reads.shared", [default]:="5,30")
        Dim unstrand As Boolean = args.GetBoolean("/unstrand")
        Dim MinIG As Integer = args.GetValue(Of Integer)("/ig.min", [default]:=500)

        If genomeSize <= 0 Then
            Call $"[ERROR]  {NameOf(genomeSize)}:={genomeSize } is zero or negative!".__DEBUG_ECHO
            Return -100
        End If

        Call $"Start to load transcripts sites from {Transcripts.ToFileURL}".__DEBUG_ECHO
        Dim sw = Stopwatch.StartNew
        Dim Source = Transcripts.LoadCsv(Of DocumentFormat.Transcript)(False)
        Call $"Data load job done!   {Source.Count} sites in {sw.ElapsedMilliseconds}ms....".__DEBUG_ECHO

        Dim max = (From obj In Source.AsParallel Select stdNum.Max(obj.MappingLocation.left, obj.MappingLocation.right)).ToArray.Max

        If max > genomeSize Then
            Call $"[WARNING] The given {NameOf(genomeSize)}:={genomeSize} is smaller than max position of the reads, auto correct value to {max + 1}".__DEBUG_ECHO
            genomeSize = max + 1
        End If

        Dim sharedReads As Integer() = readsShared.Split(","c).Select(Function(s) CInt(Val(s)))
        Dim Result As DocumentFormat.Transcript() =
            Transcriptome.UTRs.IdentifyUTRs.siRNAPredictes(Source,
            genomeSize,
            unstrand,
            sharedReads:=sharedReads.Max, sharedReadsMin:=sharedReads.Min, minIGD:=MinIG)

        Call Result.SaveTo(Transcripts & "_" & NameOf(siRNAPredictions) & ".csv", False)

        Return 0
    End Function

    <ExportAPI("-Identify.TSSs",
               Usage:="-Identify.TSSs -in <transcripts.csv> /genome.size <nt_total_length> [/reads.shared 30 /unstrand /reads.Len 90 -out <out_test.csv>]",
               Info:="Unlike IdentifyUTRs function, this function using the TSSs seeds to identified TSSs.
               You can manual setup genome.Size in a given length, or just left it blank let the program to detecting automatically but this maybe makes some mistakes..... 
               Only the sites which is testing successfully will be output.")>
    <Argument("/reads.Len", True, Description:="The nt length of your raw reads in the *.fq sequence file.")>
    Public Function TestSites(args As CommandLine) As Integer
        Dim Transcripts As String = args("-in")
        Dim genomeSize As Long = args.GetInt64("/genome.size")
        Dim readsShared = args.GetValue(Of Integer)("/reads.shared", [default]:=30)
        Dim unstrand As Boolean = args.GetBoolean("/unstrand")
        Dim MinIG As Integer = args.GetValue(Of Integer)("/reads.len", [default]:=90)

        If genomeSize <= 0 Then
            Call $"[ERROR]  {NameOf(genomeSize)}:={genomeSize } is zero or negative!".__DEBUG_ECHO
            Return -100
        End If

        Call $"Start to load transcripts sites from {Transcripts.ToFileURL}".__DEBUG_ECHO
        Dim sw = Stopwatch.StartNew
        Dim source = ReadsCount.LoadDb(Transcripts)
        Call $"Data load job done!   {source.Length} sites in {sw.ElapsedMilliseconds}ms....".__DEBUG_ECHO

        Dim max = (From obj As ReadsCount In source.AsParallel Select obj.Index).ToArray.Max

        If max > genomeSize Then
            Call $"[WARNING] The given {NameOf(genomeSize)}:={genomeSize} is smaller than max position of the reads, auto correct value to {max + 1}".__DEBUG_ECHO
            genomeSize = max + 1
        End If

        Dim out As String = args.GetValue("-out", Transcripts & "_" & NameOf(TestSites) & ".csv")
        Dim Result As DocumentFormat.Transcript() =
            Transcriptome.UTRs.IdentifyUTRs.TestSites(source,
            genomeSize,
            unstrand,
            readsShared, MinIG)

        Return Result.SaveTo(out, False).CLICode
    End Function

    ''' <summary>
    ''' 这一个步骤是接着<see cref="TestSites"/>继续进行分析的，在这一步里面只会分析TSS位点在基因组上面有哪些相关的位点信息
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--genome-context",
             Info:="Just simply Associates the genome context with the sites in the target transcript site input data. 
             If the /upstream value is True, then only upstream or upstreamoverlaps site will be saved all of others loci sites will be ignored.",
             Usage:="--genome-context -in <in.csv> -ptt <genome.ptt> [/upstream /trim <shared_value:=30> -out <out.csv> /atg [ATG-distance:1000]]")>
    <Argument("-in", False, Description:="Short reads data file from DocumentFormat.Transcript object type.")>
    Public Function GenomeContent(args As CommandLine) As Integer
        Dim inFile As String = args("-in")
        Dim TrimShared = If(Not String.IsNullOrEmpty(args("/trim")), args.GetInt32("/trim"), 30)
        Dim out As String = args.GetValue("-out", inFile.TrimSuffix & ".genome-context.csv")
        Dim LociData = inFile.LoadCsv(Of Transcript)(False)
        Dim PTT As PTT = PTT.Load(args("-ptt"))
        Dim Forwards = PTT.forwards
        Dim Reversed = PTT.reversed
        Dim ATG As Integer = If(args.ContainsParameter("/atg", False), args.GetInt32("/atg"), 1000)

        Call $"{NameOf(ATG)}:={ATG}".__DEBUG_ECHO
        Call $"{NameOf(TrimShared)}:={TrimShared}".__DEBUG_ECHO
        Call $"{NameOf(Forwards)}={ Forwards.Length};   { NameOf(Reversed)}={ Reversed.Length}  from {(args <= "-ptt").ToFileURL}".__DEBUG_ECHO
        Call $"{NameOf(LociData)}={LociData.Count }".__DEBUG_ECHO

        LociData = (From loci In LociData.AsParallel Where loci.TSSsShared >= TrimShared Select loci).AsList
        Call $"{NameOf(LociData)}={LociData.Count} left after trimming the {NameOf(Transcript.TSSsShared)}...".__DEBUG_ECHO

        Dim Transcripts As List(Of DocumentFormat.Transcript)
        Dim sw = Stopwatch.StartNew
        If args.GetBoolean("/upstream") Then
            Call "Start to export all upstream loci sites.....".__DEBUG_ECHO
            Transcripts = Transcriptome.UTRs.GenomicsContext(LociData, PTT, ATG).AsList
        Else
            Transcripts = (From loc In LociData.AsParallel Select GenomeContext(loc, PTT, ATG)).ToArray.Unlist
        End If

        Call $"Genome context associate job done!  ....... {sw.ElapsedMilliseconds}ms. ".__DEBUG_ECHO
        Call $"Finally {NameOf(Transcripts)}:={Transcripts.Count} data to write....".__DEBUG_ECHO

        Return If(Transcripts.SaveTo(out, False), 0, -1)
    End Function

    <Extension> Private Function __TSSsLoci(loci As Transcript) As NucleotideLocation
        If loci.MappingLocation.Strand = Strands.Forward Then
            Return New NucleotideLocation(loci.TSSs, loci.TSSs + 1, "+")
        Else
            Return New NucleotideLocation(loci.TSSs - 1, loci.TSSs, "-")
        End If
    End Function

    ''' <summary>
    ''' 将序列拼接所得到的转录本和基因组之中的元素所关联起来
    ''' </summary>
    ''' <param name="TSSsLoci"></param>
    ''' <param name="PTT"></param>
    ''' <returns></returns>
    Public Function GenomeContext(Of TTranscript As Transcript)(TSSsLoci As TTranscript, PTT As PTT, ATGDist As Integer) As TTranscript()
        'Dim Loci As NucleotideLocation = TSSsLoci.__TSSsLoci
        'Dim Genes = PTT.GetStrandGene(Loci.Strand)  ' 筛选出链方向相关的基因列表
        'Dim association = LocationDescriptions.GetRelatedGenes(
        '    Genes, Loci.Left, Loci.Right, ATGDistance:=ATGDist)

        ''获取内部反向位点的相关基因
        'Dim Antisense = GetInnerAntisense(
        '    PTT.GeneObjects, Loci.Left, Loci.Right, Loci.Strand)

        ''假若位点和基因的关系是cover类型的话，则该位点可能是一个完整的转录本了，则还会描述出ATG和TGA的信息
        'Dim Covers = (From Gene As Relationship(Of GeneBrief) In association
        '              Where Gene.Relation = SegmentRelationships.Cover
        '              Select Gene.Gene).ToArray

        'Dim ResultList As New List(Of TTranscript)

        'If Covers.Count = 1 Then '单个的基因，则为MTU
        '    Dim Gene = Covers.First
        '    Call ResultList.Add(TSSsLoci.Copy(Of TTranscript)(Gene))
        'ElseIf Covers.Count > 1 Then
        '    Call ResultList.AddRange((From Gene In Covers Select TSSsLoci.Copy(Of TTranscript)(Gene, Operon:=Loci.ToString)))
        'End If

        'If Not Antisense.IsNullOrEmpty Then
        '    Call ResultList.AddRange((From Gene In Antisense Select TSSsLoci.Copy(Of TTranscript)(Antisense:=Gene.Synonym)).ToArray)
        'End If

        'If Not association.IsNullOrEmpty Then
        '    Dim LQuery = (From info In association
        '                  Where info.Relation <> SegmentRelationships.Cover'已经在ORF部分处理过了，所以这里没有需要了，过滤掉！
        '                  Select associatedGene = TSSsLoci.Copy(Of TTranscript)(info.Relation, info.Gene.Synonym), info).ToArray

        '    For Each obj In LQuery
        '        If obj.info.Relation = SegmentRelationships.UpStream OrElse
        '            obj.info.Relation = SegmentRelationships.UpStreamOverlap Then
        '            obj.associatedGene.Synonym = obj.info.Gene.Synonym
        '            obj.associatedGene.ATG = obj.info.Gene.ATG
        '            obj.associatedGene.TGA = obj.info.Gene.TGA

        '            If obj.info.Gene.Location.Strand = Strands.Forward Then
        '                obj.associatedGene.Right = Math.Max(obj.associatedGene.Right, obj.info.Gene.TGA)
        '            Else
        '                obj.associatedGene.Right = Math.Min(obj.associatedGene.Right, obj.info.Gene.TGA)
        '            End If
        '        End If
        '    Next

        '    Call ResultList.AddRange(LQuery.Select(Function(x) x.associatedGene))
        'ElseIf Not ResultList.Count = 0 Then
        '    TSSsLoci.Position = "Intergenic"
        '    Call ResultList.Add(TSSsLoci)
        'End If

        'For Each TranscriptLoci In ResultList
        '    If TranscriptLoci.MappingLocation.Strand = Strands.Forward Then
        '        TranscriptLoci.Left = TranscriptLoci.TSSs
        '    Else
        '        TranscriptLoci.Right = TranscriptLoci.TSSs
        '    End If
        'Next

        'Return ResultList.ToArray

        Throw New NotImplementedException
    End Function

End Module
