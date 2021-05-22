#Region "Microsoft.VisualBasic::1db27a798b864c51fda15268282243cc, CLI_tools\RNA-seq\CLI\SAMtools.vb"

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
'     Function: __writeFile, AssociateGI, chisqTest, Clustering, ContactsNNN
'               CorrelatesVector, exportInternal, ExportSAMMaps, ExportSAMMapsBySamples, FormatGI
'               Fq2fa, MergeFastQ, SAMcontigs, SelectSubs
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports RDotNET.Extensions.VisualBasic.DataFrameAPI
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Analysis.Metagenome.BEBaC
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.FQ
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.SAM
Imports R_api = RDotNET.Extensions.VisualBasic.API

Partial Module CLI

    <ExportAPI("/Format.GI", Usage:="/Format.GI /in <txt> /gi <regex> /format <gi|{gi}> /out <out.txt>")>
    Public Function FormatGI(args As CommandLine) As Integer
        Dim [in] As String = args.GetFullFilePath("/in")
        Dim gi As String = args("/gi")
        Dim format As String = args("/format")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".Format_gi." & [in].Split("."c).Last)
        Dim regex As New Regex(gi, RegexICSng)

        Using write = out.OpenWriter(Encodings.ASCII)
            write.NewLine = vbLf

            For Each line As String In [in].IterateAllLines
                Dim ms = regex.Matches(line).ToArray

                For Each s As String In ms
                    line = line.Replace(s, format.Replace("{gi}", Regex.Match(s, "\d+").Value))
                Next

                Call write.WriteLine(line)
            Next
        End Using

        Return 0
    End Function

    <ExportAPI("/fq2fa", Usage:="/fq2fa /in <fastaq> [/out <fasta>]")>
    Public Function Fq2fa(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".fasta")
        Dim fastaq As FastQFile = FastQFile.Load([in])
        Dim fasta As FastaFile = fastaq.ToFasta
        Return fasta.Save(out, Encodings.ASCII)
    End Function

    <ExportAPI("/Clustering", Usage:="/Clustering /in <fq> /kmax <int> [/out <out.Csv>]")>
    Public Function Clustering(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim kmax As Integer = args.GetInt32("/kmax")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ",kmax=" & kmax & ".Csv")
        Dim fq As FastQFile = FastQFile.Load([in])
        Dim vectors = fq.Transform
        Dim Crude = vectors.InitializePartitions(kmax)
        Dim ppppp = Crude.First.PartitionProbability
        Dim ptes = Crude.MarginalLikelihood
    End Function

    <ExportAPI("/Co.Vector", Usage:="/Co.Vector /in <co.Csv/DIR> [/min 0.01 /max 0.05 /out <out.csv>]")>
    Public Function CorrelatesVector(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim min As Double = args.GetValue("/min", 0.01)
        Dim max As Double = args.GetValue("/max", 0.05)
        Dim EXPORT As String = args.GetValue("/out", [in].TrimDIR & $"/{[in].BaseName}.EXPORT.PR.Csv")
        Dim out As New IO.File

        out += {"X", "P", "R"}

        For Each file As String In ls - l - r - wildcards("*P*.Csv") <= [in]
            Dim rp As String = file.ParentPath & "/" & file.BaseName.Replace("P", "R") & ".Csv"
            Dim R As IO.File = IO.File.Load(rp)
            Dim P As IO.File = IO.File.Load(file)
            Dim Rs = R.Skip(1).ToDictionary(Function(x) x.First, Function(x) x(1))

            Call file.__DEBUG_ECHO
            Call rp.__DEBUG_ECHO

            For Each row In P.Skip(1)
                If row(1) <> "NA" Then
                    Dim pp As Double = Val(row(1))
                    If pp <= max AndAlso pp >= min Then
                        Dim srow As New List(Of String)
                        srow += row.First
                        srow += CStr(pp)
                        srow += Rs(srow.First)

                        out += srow

                        Call Console.Write(".")
                    End If
                End If
            Next
        Next

        Return out.Save(EXPORT, Encodings.ASCII)
    End Function

    <Extension>
    Private Function __writeFile(file As IO.File, out As String, min As Double, max As Double) As Boolean
        Dim rows As New List(Of String)

        For Each row In file.Skip(1)
            If row(1) <> "NA" Then
                Dim p As Double = Val(row(1))
                If p <= max AndAlso p >= min Then
                    rows += $"{row.First},{p}"
                End If
            End If
        Next

        Return rows.SaveTo(out)
    End Function

    <ExportAPI("/Export.SAM.Maps.By_Samples",
               Usage:="/Export.SAM.Maps.By_Samples /in <in.sam> /tag <sampleTag_regex> [/ref <ref.fasta> /out <out.Csv>]")>
    Public Function ExportSAMMapsBySamples(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim tagRegex As New Regex(args("/tag"), RegexICSng)
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".Maps.Csv")
        Dim reader As New SAMStream([in])
        Dim result As New List(Of SimpleSegment)
        Dim ref As String = args("/ref")

        For Each readMaps As AlignmentReads In reader.IteratesAllReads
            Dim reads As New SimpleSegment With {
                .ID = readMaps.RNAME,
                .Start = readMaps.POS,
                .Strand = readMaps.Strand.GetBriefCode,
                .SequenceData = readMaps.QUAL,    ' .Complement = tagRegex.Match(readMaps.QNAME).Value.Trim("_"c),
                .Ends = readMaps.FLAG
            }

            result += reads
        Next


        Dim tagsHash As Dictionary(Of String, String)

        If ref.FileExists Then
            Dim rawRef As New FastaFile(ref)

            tagsHash = (From x As FastaSeq
                        In rawRef
                        Select x,
                            sid = x.Title.Split.First
                        Group By sid Into Group) _
                                .ToDictionary(Function(x) x.sid,
                                              Function(x) x.Group.First.x.Title.Replace(x.sid, "").Trim)
        Else
            tagsHash = New Dictionary(Of String, String)
        End If

        Dim getValue As Func(Of String, String) =
            If(tagsHash Is Nothing,
            Function(s) "",
            Function(s) If(tagsHash.ContainsKey(s), tagsHash(s), ""))
        Dim mapGroup = From x As SimpleSegment
                       In result
                       Select x
                       Group x By x.ID Into Group
        Dim stats = From g In mapGroup
                    Let samples As Dictionary(Of String, Integer) = (
                        From x As SimpleSegment
                        In g.Group
                        Select x
                        Group x By x.Complement Into Group) _
                             .ToDictionary(Function(x) x.Complement,
                                           Function(x) x.Group.Count)
                    Select refName = g.ID,
                        samples,
                        title = getValue(g.ID)
        Dim output = stats.ToArray
        Return output.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 导出来的结果是依靠<see cref="SimpleSegment.ID"/>属性来进行结果统计的
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <param name="genome"></param>
    ''' <param name="showDebug"></param>
    ''' <returns></returns>
    <Extension>
    Private Iterator Function exportInternal(reader As SAMStream, genome As GenomeContextProvider(Of GeneBrief), showDebug As Boolean) As IEnumerable(Of SimpleSegment)
        For Each readMaps As AlignmentReads In reader.IteratesAllReads
            Dim reads As New SimpleSegment With {
                .ID = readMaps.RNAME,
                .Start = readMaps.POS,
                .Strand = readMaps.Strand.GetBriefCode,
                .SequenceData = readMaps.QUAL,                ' .Complement = readMaps.QNAME,
                .Ends = readMaps.FLAG
            }

            If reads.ID <> "*" AndAlso genome IsNot Nothing Then
                Dim loci As New NucleotideLocation(readMaps.POS, readMaps.POS + reads.SequenceData.Length, False)
                Dim contig = genome.GetAroundRelated(loci,, 10, parallel:=True)
                If Not contig.IsNullOrEmpty Then
                    reads.ID = contig.First.Gene.Synonym.Split.First
                    If contig.Length > 1 Then
                        Call reads.GetJson.__DEBUG_ECHO
                    Else
                        If showDebug Then
                            Call reads.ID.__DEBUG_ECHO
                        End If
                    End If
                Else
                    Call (reads.ID & " " & loci.ToString & " not found!!!").__DEBUG_ECHO
                End If
            End If

            Yield reads
        Next
    End Function

    <ExportAPI("/Export.SAM.contigs")>
    <Usage("/Export.SAM.contigs /in <bwa_align_out.sam> [/ref <reference.fasta> /out <out.fasta>]")>
    <Group(CLIGroups.SAMtools)>
    Public Function SAMcontigs(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.fasta"
        Dim workspace$ = $"{out.ParentPath}/${in$.BaseName}.sam/"
        Dim ref$ = args <= "/ref"
        Dim provider As Func(Of String(), IEnumerable(Of FastaSeq))

        If ref.FileExists Then
            provider = Function(locus)
                           Dim tmp$ = TempFileSystem.GetAppSysTempFile(sessionID:=App.PID)
                           Dim subset$ = workspace & "/ref.fasta"

                           Call locus.JoinBy(ASCII.LF).SaveTo(tmp)
                           Call Apps.seqtools.SubsetFastaDb(tmp, db:=ref, out:=subset)

                           Return StreamIterator.SeqSource(handle:=subset)
                       End Function
        Else
            provider = Nothing
        End If

        Dim coverage = Assembler.SequenceCoverage(
            sam:=[in],
            workspace:=workspace,
            refProvider:=provider
        )
        Return coverage _
            .GetJson() _
            .SaveTo(out) _
            .CLICode
    End Function

    ''' <summary>
    ''' 小文件适用
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Export.SAM.Maps",
               Usage:="/Export.SAM.Maps /in <in.sam> [/large /contigs <NNNN.contig.Csv> /raw <ref.fasta> /out <out.Csv> /debug]")>
    <ArgumentAttribute("/raw", True,
              AcceptTypes:={GetType(FastaFile), GetType(FastaSeq)},
              Description:="When this command is processing the NNNNN contact data, just input the contigs csv file, this raw reference is not required for the contig information.")>
    Public Function ExportSAMMaps(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".Maps.Csv")
        Dim reader As New SAMStream([in])
        Dim NNNNcontig As String = args("/contigs")
        Dim genome As GenomeContextProvider(Of GeneBrief) = Nothing
        Dim tagsHash As Dictionary(Of String, String) = Nothing
        Dim showDebug As Boolean = args.GetBoolean("/debug")

        If NNNNcontig.FileExists Then
            Dim contigs = NNNNcontig.LoadCsv(Of SimpleSegment)
            Dim genes = contigs.Select(Function(x) x.ToPTTGene).ToArray
            genome = New GenomeContextProvider(Of GeneBrief)(genes)
            tagsHash = (From x As SimpleSegment
                        In contigs
                        Select x.ID.Split.First,
                            x
                        Group By First Into Group) _
                                .ToDictionary(Function(x) x.First,
                                              Function(x) x.Group.First.x.ID.Replace(x.First, "").Trim)
        End If

        Dim maps As New Dictionary(Of String, String) From {
 _
            {NameOf(SimpleSegment.Complement), NameOf(AlignmentReads.QNAME)},
            {NameOf(SimpleSegment.Ends), NameOf(AlignmentReads.FLAG)},
            {NameOf(SimpleSegment.SequenceData), NameOf(AlignmentReads.QUAL)}
        }

        If tagsHash Is Nothing Then
            Dim ref As String = args("/raw")

            Call "Loading raw fasta titles....".__DEBUG_ECHO

            If ref.FileExists Then
                Using rawRef As New StreamIterator(ref)
                    tagsHash = New Dictionary(Of String, String)

                    For Each x As FastaSeq In rawRef.ReadStream
                        Dim sid$ = x.Title.Split.First

                        If Not tagsHash.ContainsKey(sid) Then
                            Call tagsHash.Add(sid, x.Title.Replace(sid, "").Trim)  ' 由于sid是唯一的，所以只需要第一个出现的就行了
                        End If
                    Next
                End Using

                Call $"{ref.ToFileURL}, Jod done!".__DEBUG_ECHO
            End If
        End If

        Dim getValue As Func(Of String, String) =
            If(tagsHash Is Nothing,
            Function(s) "",
            Function(s) If(tagsHash.ContainsKey(s), tagsHash(s), ""))
        Dim stat As NamedValue(Of Integer)()
        Dim ALL As Integer

        If args.GetBoolean("/large") Then
            Using writer As New WriteStream(Of SimpleSegment)(out, maps:=maps)
                Dim IDstats As New Dictionary(Of String, Value(Of Integer))

                For Each array As SimpleSegment() In TaskPartitions.
                    SplitIterator(
                    reader.exportInternal(genome, showDebug),
                    1000)

                    For Each x As SimpleSegment In array
                        If IDstats.ContainsKey(x.ID) Then
                            IDstats(x.ID).Value += 1
                        Else
                            Call IDstats.Add(
                                x.ID,
                                New Value(Of Integer)(Scan0))
                        End If
                    Next

                    ALL += array.Length

                    Call writer.Flush(array)
                Next

                stat = IDstats _
                    .Select(Function(x) New NamedValue(Of Integer)(x.Key, +x.Value) With {
                        .Description = getValue(x.Key)
                    }).ToArray
            End Using
        Else
            Dim result As New List(Of SimpleSegment)(reader.exportInternal(genome, showDebug))
            ' 小型样本的统计
            stat = LinqAPI.Exec(Of NamedValue(Of Integer)) <=
                (From x As SimpleSegment
                 In result
                 Select x
                 Group x By x.ID Into Count) _
                 .Select(Function(x) New NamedValue(Of Integer)(x.ID, x.Count) With {
                      .Description = getValue(x.ID) ' 得到标题
                 })
            ALL = result.Count
            result.SaveTo(out, maps:=maps)
        End If

        Dim statOut As String = out.TrimSuffix & ".stat.Csv"

        Call (From x As NamedValue(Of Integer)
              In stat
              Where Not String.Equals(x.Name, "*")
              Select gi = Regex.Match(x.Name, "\d+").Value,
                  x.Name) _
                  .Select(Function(x) x.gi & vbTab & x.Name) _
                  .SaveTo(statOut.TrimSuffix & ".gi_Maps.tsv")

        Return New NamedValue(Of Integer)([in].BaseName, ALL) _
            .Join(stat) _
            .SaveTo(statOut) _
            .CLICode
    End Function

    <ExportAPI("/Associate.GI",
               Usage:="/Associate.GI /in <list.Csv.DIR> /gi <nt.gi.csv> [/out <out.DIR>]")>
    Public Function AssociateGI(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim ref As String = args("/gi")
        Dim EXPORT As String = args.GetValue("/out", [in].TrimDIR & "." & ref.BaseName & "/")
        Dim hash As Dictionary(Of String, String) =
            TaxiValue.BuildDictionary(ref.LoadCsv(Of TaxiValue))

        For Each file As String In ls - l - wildcards("*.Csv") <= [in]
            Dim inputs = file.LoadCsv(Of TaxiValue)
            Dim out As String = EXPORT & "/" & file.BaseName & ".Csv"

            For Each x In inputs
                Dim gi As String = Regex.Match(x.Name, "gi(_|\|)\d+", RegexICSng).Value
                gi = gi.Split("|"c).Last
                If hash.ContainsKey(gi) Then
                    x.Title = hash(gi)
                End If
            Next

            Call inputs.SaveTo(out)
        Next

        Return 0
    End Function

    <ExportAPI("/Select.Subs",
               Usage:="/Select.Subs /in <in.DIR> /cols <list','> [/out <out.DIR>]")>
    Public Function SelectSubs(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim cols As String() = args("/cols").Split(",")
        Dim EXPORT As String = args.GetValue("/out", [in].TrimDIR & ".SubSets/")

        For Each file As String In ls - l - r - wildcards("*.Csv") <= [in]
            Dim data As IO.File = IO.File.Load(file)
            Dim out As String = EXPORT & "/" & file.BaseName & ".Csv"
            Dim columns As New List(Of String())
            Dim rev As New List(Of String())

            columns += data.Columns.First

            For Each col As String() In data.Columns
                If Array.IndexOf(cols, col(Scan0)) > -1 Then
                    columns += col
                Else
                    rev += col
                End If
            Next

            Dim selects As IO.File = columns.JoinColumns
            Dim noSelects As IO.File = rev.JoinColumns

            Call selects.Save(out, Encodings.ASCII)
            Call noSelects.Save(out.TrimSuffix & "-NonSelected.Csv", Encodings.ASCII)
        Next

        Return 0
    End Function

    Const tbl As String = "chisq.test.Table"

    <ExportAPI("/chisq.test", Usage:="/chisq.test /in <in.DIR> [/out <out.DIR>]")>
    Public Function chisqTest(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim EXPORT As String = args.GetValue("/out", [in].TrimSuffix & ".chisq.test/")

        For Each file As String In ls - l - r - wildcards("*.Csv") <= [in]
            Dim out As String = EXPORT & file.BaseName & ".Csv"
            Dim data As IO.File = IO.File.Load(file)

            For Each row In data.Skip(1)
                Call row.Apply(Function(s) CInt(Val(s)), skip:=1)
            Next

            Dim columns As String()() = data.Columns.Skip(1).ToArray
            Dim nonZEROs As New List(Of String())

            For i As Integer = 0 To columns.First.Length - 1
                Dim index As Integer = i
                Dim noZERO As String = LinqAPI.DefaultFirst(Of String) <=
                    From col As String()
                    In columns
                    Let s As String = col(index)
                    Where s <> "0" AndAlso s <> "0.0"
                    Select s
                If Not String.IsNullOrEmpty(noZERO) Then
                    nonZEROs += columns.Select(Function(x) x(index)).ToArray
                End If
            Next

            nonZEROs = New List(Of String())(nonZEROs.MatrixTranspose)

            Dim first = nonZEROs.First
            nonZEROs = New List(Of String())(nonZEROs.Skip(1))
            Dim pvalues As New RowObject From {"p-value", "null"}

            For i As Integer = 0 To nonZEROs.Count - 1
                Dim jdt As IO.File = {first, nonZEROs(i)}.JoinColumns
                Call jdt.PushAsTable(tbl)
                Dim reuslt = R_api.stats.chisqTest(tbl)
                pvalues.Add(reuslt.pvalue)
            Next

            data += pvalues

            Call data.Save(out, Encodings.ASCII)
        Next

        Return 0
    End Function

    <ExportAPI("/Contacts.NNN",
               Info:="Using for contacts the reference sequence for the metagenome analysis. reference sequence was contact in one sequence by a interval ``NNNNNNNNNNNNNNNNNN``",
               Usage:="/Contacts /in <in.fasta/DIR> [/out <out.DIR>]")>
    Public Function ContactsNNN(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim i As Integer = 1
        Dim contigs As New List(Of SimpleSegment)
        Dim out As String = args.GetValue("/out", If([in].FileExists, [in].TrimSuffix, [in].TrimDIR) & ".Contigs/")
        Dim outNt As String = out & "/nt.fasta"
        Dim outContigs As String = out & "/contigs.csv"
        Dim il As Integer = Interval.Length

        Call "".SaveTo(outNt)

        Using writer As New StreamWriter(New FileStream(outNt, FileMode.OpenOrCreate), Encoding.ASCII)
            Call writer.WriteLine("> " & [in].BaseName)

            For Each fa As FastaSeq In StreamIterator.SeqSource(handle:=[in], debug:=True)
                Call writer.Write(fa.SequenceData)
                Call writer.Write(Interval)

                Dim nx As Integer = i + fa.Length

                contigs += New SimpleSegment With {
                    .Start = i,
                    .Ends = nx,
                    .ID = fa.ToString,
                    .Strand = "+"
                }
                i = nx + il

                ' Call Console.Write(".")
            Next

            Call contigs.SaveTo(outContigs)
        End Using

        Return 0
    End Function

    <ExportAPI("/Merge.FastQ", Usage:="/Merge.FastQ /in <in.DIR> [/out <out.fq>]")>
    Public Function MergeFastQ(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimDIR & ".fq")

        Using write As StreamWriter = out.OpenWriter
            For Each path$ In ls - l - r - {"*.fastq", "*.fq", "*.fq1", "*.fq2"} <= [in]
                For Each line As FastQ In FQ.ReadAllLines(path)
                    Call write.WriteLine(line.AsReadsNode)
                Next
            Next

            Return 0
        End Using
    End Function
End Module
