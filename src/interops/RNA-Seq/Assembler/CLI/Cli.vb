#Region "Microsoft.VisualBasic::a89c548a445c7d35d50c09021f0cbdb0, RNA-Seq\Assembler\CLI\Cli.vb"

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
    '     Function: __genomeContext, __getTrimMethod, ass, Assembler, AssemblerTask
    '               DataView, ExportBlastnMapping, getParser, LoadBlastnMappingSource, (+2 Overloads) Merge
    '               (+2 Overloads) MergeJason, Split, Trim
    ' 
    '     Sub: __add
    ' 
    ' /********************************************************************************/

#End Region

Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application
Imports LANS.SystemsBiology.SequenceModel.NucleotideModels
Imports LANS.SystemsBiology.Toolkits.RNA_Seq.Assembler.DocumentFormat
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.Linq
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Scripting.MetaData

''' <summary>
''' Assembler <see cref="CommandLine.CommandLine"/> <see cref="CommandLine.Interpreter"/> API
''' </summary>
''' 
<PackageNamespace("Assembler.CLI_API", Category:=APICategories.CLI_MAN, Publisher:="xie.guigang@gcmodeller.org")>
Public Module CLI

    Private Function __genomeContext(context As KeyValuePair(Of SegmentRelationships, GeneBrief())(), loci As Transcript) As Transcript()
        Dim setValue As New SetValue(Of Transcript)
        Dim LQuery As Transcript() =
            LinqAPI.Exec(Of Transcript) <= From relation
                                           In context
                                           Let rr As String = relation.Key.ToString
                                           Select From gene As GeneBrief
                                                  In relation.Value
                                                  Select setValue _
                                                      .InvokeSetValue(loci.Copy(Of Transcript), NameOf(Transcript.Synonym), gene.Synonym) _
                                                      .InvokeSet(NameOf(Transcript.Position), rr).obj
        If LQuery.IsNullOrEmpty Then
            Return {loci}
        Else
            Return LQuery
        End If
    End Function

    <ExportAPI("-Split",
             Info:="Split the transcript loci data into different files base on the location relationship between the loci and their associated genes.",
             Usage:="-Split -input <TranscriptInputs> [-out <ExportDir>]")>
    Public Function Split(argvs As CommandLine.CommandLine) As Integer
        Dim Inputs As String = argvs("-input")
        Dim ExportDir As String = argvs("-out")

        If Not Inputs.FileExists Then
            Call $"The required input file ""{Inputs.ToFileURL}"" is not exists on your file system!".__DEBUG_ECHO
            Return -10
        End If

        If String.IsNullOrEmpty(ExportDir) Then
            ExportDir = FileIO.FileSystem.GetParentPath(Inputs) & "/" & IO.Path.GetFileNameWithoutExtension(Inputs) & "-Splits/"
        End If

        Dim dataBuffer = Inputs.LoadCsv(Of Transcript)(False)
        Dim GroupBuffer = (From obj In dataBuffer.AsParallel Select obj Group obj By obj.Position Into Group).ToArray
        Dim ParallelSave = (From Partition In GroupBuffer.AsParallel
                            Let Path As String = ExportDir & Partition.Position & ".csv"
                            Select Partition.Group.ToArray.SaveTo(Path, False)).ToArray
        Return 0
    End Function

    <ExportAPI("-assemble.Task", Usage:="/parser <blastnMapping/sam> /source <dir> [-out <out_dir> /ptt <ptt> /trim -delta 40 -TSSs.threshold 30]")>
    Public Function AssemblerTask(argvs As CommandLine.CommandLine) As Integer
        Dim Parser As String = argvs("/parser")
        Dim SourceDir As String = argvs("/source")
        Dim Out As String = argvs("-out")
        Dim PTT As String = argvs("/ptt")
        Dim Trim As Boolean = argvs.GetBoolean("/trim")
        Dim Delta As Integer = argvs.GetInt32("-delta")
        Dim Threshold As Integer = argvs.GetInt32("-TSSs.threshold")
        Dim Dirs = FileIO.FileSystem.GetDirectories(SourceDir, FileIO.SearchOption.SearchTopLevelOnly)

        If String.IsNullOrEmpty(Out) Then
            Out = FileIO.FileSystem.CurrentDirectory
        End If

        If Delta <= 0 Then
            Delta = 40
        End If

        If Threshold <= 0 Then
            Threshold = 30
        End If

        Dim LQuery = (From source As String In Dirs.AsParallel
                      Let outCsv As String = $"{Out}/Assembler.Task/{FileIO.FileSystem.GetDirectoryInfo(source).Name}.csv"
                      Let argvsCli As String = $"-assemble /parser {Parser} /reads {source.CliPath} /merge.source -out {outCsv } /ptt {PTT.CliPath} -delta {Delta} -TSSs.threshold {Threshold }{If(Trim, " /trim", "") }"
                      Let proc = Process.Start(App.ExecutablePath, argvsCli)
                      Select proc).ToArray

        For Each proc As System.Diagnostics.Process In LQuery
            Call proc.WaitForExit()
        Next

        Return 0
    End Function

    <ExportAPI("-assemble", Usage:="-assemble /parser <blastnMapping/sam> /reads <readsFile/source> [/Merge.Source -out <out_csv> /ptt <ptt> /trim -delta 35 -TSSs.threshold 30 /ATG.Dist 500]")>
    <ParameterInfo("/reads", False,
                          Description:="The parameter can be both reads file or a directory which contains some reads file base on the 
switch parameter /Merge.Source is exists or not. When /Merge.Source is represented then this parameter value is indicates 
a directory source, and all of the reads file in that directory will be merged into a big source.")>
    Public Function Assembler(argvs As CommandLine.CommandLine) As Integer
        Dim TrimData As Boolean = argvs.GetBoolean("/trim")
        Dim Parser = argvs.GetObject(Of Func(Of String, Boolean, LANS.SystemsBiology.SequenceModel.NucleotideModels.Contig()))("/parser", AddressOf getParser)

        If Parser Is Nothing Then
            Call $"The /parser value ""{argvs("/parser")}"" is not a valid parser name!".__DEBUG_ECHO
            Return -10
        End If

        Dim ReadsSource As String = argvs("/reads")
        Dim MergeSource As Boolean = argvs.GetBoolean("/Merge.Source")

        If Not MergeSource AndAlso Not ReadsSource.FileExists Then
            Call $"The specified reads file ""{ReadsSource.ToFileURL}"" is not exists on your file system!".__DEBUG_ECHO
            Return -20
        ElseIf MergeSource Then

            If Not FileIO.FileSystem.DirectoryExists(ReadsSource) Then
                Call $"The specified reads source directory ""{ReadsSource}"" is not exists on your file system!".__DEBUG_ECHO
                Return -25
            End If
        End If

        Dim Reads = Parser(ReadsSource, MergeSource)
        Call $"Total reads number is {Reads.LongCount}".__DEBUG_ECHO
        Dim InputReads As Long = Reads.LongCount
        Dim TrimmedReads As Long

        If TrimData Then
            Dim TrimMethod = argvs.GetObject(Of '这里不需要再检查是否存在了，因为在上面的parser代码处已经检查过了 
                Func(Of LANS.SystemsBiology.SequenceModel.NucleotideModels.Contig(), LANS.SystemsBiology.SequenceModel.NucleotideModels.Contig()))("/parser", AddressOf __getTrimMethod)

            Reads = TrimMethod(Reads)
            Call $"Left {Reads.Count} data after trimming...".__DEBUG_ECHO
            TrimmedReads = InputReads - Reads.Count
        End If

        Dim PTT = argvs.GetObject(Of LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.PTT)(
            "/ptt", AddressOf LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.PTT.Load)
        Dim DataOverView As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = Nothing
        Dim Delta As Integer = argvs.GetInt32("-delta")
        Dim sharedThreshold As Integer = If(argvs.ContainsParameter("-TSSs.threshold", False), argvs.GetInt32("-TSSs.threshold"), 30)
        Dim Transcripts = API.AssemblerAssembleAPI(Reads, PTT, If(Delta <= 0, 35, Delta), DataOverView, sharedThreshold, argvs.GetValue(Of Integer)("/ATG.Dist", 500))
        Dim OutFile As String = argvs("-out")

        If String.IsNullOrEmpty(OutFile) Then
            OutFile = ReadsSource & ".csv"
        End If

        Try
            Call DataOverView.Add({NameOf(InputReads), InputReads})
            Call DataOverView.Add({NameOf(TrimmedReads), TrimmedReads})

            Call DataOverView.Save(OutFile & ".DataOverView.csv", False)
        Catch ex As Exception
            Call ex.ToString.__DEBUG_ECHO
        End Try

        Dim b As Boolean = Transcripts.SaveTo(OutFile, False)

        Return b.CLICode
    End Function

    Public Function getParser(name As String) As Func(Of String, Boolean, LANS.SystemsBiology.SequenceModel.NucleotideModels.Contig())
        If String.Equals(name, NameOf(LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BlastnMapping), StringComparison.OrdinalIgnoreCase) Then
            Call $“Reads://{GetType(LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BlastnMapping).ToString}”.__DEBUG_ECHO
            Return AddressOf LoadBlastnMappingSource
        Else
            Return Nothing
        End If
    End Function

    Private Function LoadBlastnMappingSource(source As String, MergeSource As Boolean) As LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BlastnMapping()
        If MergeSource Then

            Call $"Merge source from location {source}".__DEBUG_ECHO

            Dim LQuery = (From path In FileIO.FileSystem.GetFiles(source, FileIO.SearchOption.SearchTopLevelOnly, "*.csv")
                          Select path.LoadCsv(Of LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BlastnMapping)(False)).ToArray.MatrixToVector

            Call $"Load {LQuery.Count} data chunk from merge source...".__DEBUG_ECHO

            Return LQuery
        Else
            Return source.LoadCsv(Of LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BlastnMapping)(False).ToArray
        End If
    End Function

    Public Function __getTrimMethod(name As String) As Func(Of Contig(), Contig())
        If String.Equals(name, NameOf(BlastnMapping), StringComparison.OrdinalIgnoreCase) Then
            Return AddressOf MapsAPI.TrimAssembly
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' 从这里导出blastn mapping的结果
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Export", Info:="Exports the result of fq reads aligned to the target genome using blastn.",
             Usage:="/Export -blastn <blastn_log> [-out <saved_csv>]")>
    Public Function ExportBlastnMapping(args As CommandLine.CommandLine) As Integer
        Dim inFile As String = args("-blastn")
        Dim out As String = args.GetValue("-out", inFile.TrimFileExt & ".Csv")

        Using IO As New DocumentStream.Linq.WriteStream(Of NCBI.Extensions.LocalBLAST.Application.BlastnMapping)(out)
            Dim handle = IO.ToArray(Of NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Query())(
                Function(query) MapsAPI.Export(query))
            Call NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Transform(inFile, 1024 * 1024 * 128, handle)
        End Using

        Return True
    End Function

    '<ExportAPI("-FQ.OverlapDeNovoAssembler", Usage:="-FQ.OverlapDeNovoAssembler -fq <fastq> [-out <output.fasta>]")>
    'Public Function FqAssemble(argvs As CommandLine.CommandLine) As Integer
    '    Dim Fq As String = argvs("-fq")
    '    Dim Output As String = argvs("-out")

    '    If String.IsNullOrEmpty(Output) Then
    '        Output = Fq & ".fasta"
    '    End If

    '    Dim Fasta = New MBF.Assembler().Assembling(Fq)
    '    Dim b As Boolean = Fasta.Save(Output)

    '    Return b.CLICode
    'End Function

    ''' <summary>
    ''' 合并同一个条件之下的实验数据，按照链的方向进行合并
    ''' </summary>
    ''' <param name="argvs"></param>
    ''' <returns></returns>
    <ExportAPI("-Merge", Usage:="-Merge /source <dir> [/support.Trim 30 -out <out_Csv> /delta 3]")>
    Public Function Merge(argvs As CommandLine.CommandLine) As Integer
        Dim sourceDir As String = argvs("/source")
        Dim Support As Integer = argvs.GetInt32("/support.Trim")
        Dim Out As String = argvs("-out")
        Dim Delta As Integer = argvs.GetInt32("/delta")

        If Delta <= 0 Then
            Delta = 3
        End If

        If String.IsNullOrEmpty(Out) Then
            Out = FileIO.FileSystem.CurrentDirectory & "/" & FileIO.FileSystem.GetDirectoryInfo(sourceDir).Name & ".csv"
        End If

        Dim LQuery As List(Of Transcript) =
            LinqAPI.MakeList(Of Transcript) <= From pathEntry As NamedValue(Of String)
                                               In sourceDir.LoadEntryList({"*.csv"})
                                               Select pathEntry.x.LoadCsv(Of Transcript)(False)

        Call $"Csv files load done!".__DEBUG_ECHO

        Dim Forwards = (From obj In LQuery.AsParallel
                        Where obj.MappingLocation.Strand = Strands.Forward
                        Select obj
                        Order By obj.MappingLocation.Start Ascending).ToArray
        Dim Reversed = (From obj In LQuery.AsParallel
                        Where obj.MappingLocation.Strand = Strands.Reverse
                        Select obj
                        Order By obj.MappingLocation.Start Descending).ToArray
        Call $"Strands partitioning job done!".__DEBUG_ECHO

        Forwards = Merge(Forwards)
        Reversed = Merge(Reversed)

        Dim Merged = MergeJason(Forwards.ToList, Reversed.ToList, Delta)
        Dim setValue = New SetValue(Of Transcript) <= NameOf(DocumentFormat.Transcript.Support)
        Merged = (From obj In Merged.AsParallel Select setValue(obj, obj.TSSsShared >= Support)).ToList
        Dim b = Merged.SaveTo(Out, False)

        Return b.CLICode
    End Function

    ''' <summary>
    ''' 调用前请先进行排序操作
    ''' </summary>
    ''' <param name="Forwards"></param>
    ''' <param name="Reversed"></param>
    ''' <param name="delta"></param>
    ''' <returns></returns>
    Private Function MergeJason(Forwards As List(Of Transcript), Reversed As List(Of Transcript), delta As Integer) As List(Of Transcript)
        Dim pre As Transcript
        Dim i As Integer = 1

        Call $"Start to merge jason {NameOf(Forwards)}".__DEBUG_ECHO

        pre = Forwards(Scan0)
        Do While i <= Forwards.Count - 1
            Dim current = Forwards(i)
            If Math.Abs(current.TSSs - pre.TSSs) <= delta AndAlso
                String.Equals(current.Operon, pre.Operon) AndAlso
                String.Equals(current.Synonym, pre.Synonym) Then
                '合并然后删掉当前的这个对象
                pre.TSSsShared += current.TSSsShared
                pre.Right = Math.Max(pre.Right, current.Right)
                Call Forwards.RemoveAt(i) '后面的元素自动前移
                'Call $"{current.TSSs} => {pre.TSSs}".__DEBUG_ECHO
            Else
                pre = current
                i += 1
            End If
        Loop

        Call $"{NameOf(Forwards)} => {Forwards.Count }".__DEBUG_ECHO
        Call $"Start to merge jason {NameOf(Reversed)}".__DEBUG_ECHO

        pre = Reversed(Scan0)
        i = 1
        Do While i <= Reversed.Count - 1
            Dim current = Reversed(i)
            If Math.Abs(pre.TSSs - current.TSSs) <= delta AndAlso String.Equals(current.Operon, pre.Operon) AndAlso
                String.Equals(current.Synonym, pre.Synonym) Then
                pre.TSSsShared += current.TSSsShared
                pre.Left = Math.Min(pre.Left, current.Left)
                Call Reversed.RemoveAt(i) '后面的元素自动前移
            Else
                pre = current
                i += 1
            End If
        Loop

        Call $"{NameOf(Reversed)} =? { Reversed.Count } ".__DEBUG_ECHO
        Call Forwards.AddRange(Reversed)
        Return Forwards
    End Function

    ''' <summary>
    ''' 3bp以内的也合并
    ''' </summary>
    ''' <param name="Transcript"></param>
    ''' <returns></returns>
    ''' <param name="delta">文献中的为3个bp的偏差</param>
    Private Function MergeJason(Transcript As List(Of Transcript), delta As Integer) As List(Of Transcript)
        Dim Forwards = (From obj In Transcript.AsParallel Where String.Equals(obj.Strand, CLI.Forwards) Select obj Order By obj.TSSs Ascending).ToList
        Dim Reversed = (From obj In Transcript.AsParallel Where String.Equals(obj.Strand, CLI.Reversed) Select obj Order By obj.TSSs Descending).ToList

        Return MergeJason(Forwards, Reversed, delta)
    End Function

    ''' <summary>
    ''' 左端一样，基因号一样，位置描述一样，在调用之前请确保都处于同一条链
    ''' </summary>
    ''' <param name="Transcript">来自于不同的条件之下，假若只有一个，则没有重复，直接添加第一个，假若有多个，则合并<see cref="DocumentFormat.Transcript.TSSsShared"/>重复计数这个属性</param>
    ''' <returns></returns>
    Private Function Merge(Transcript As Transcript()) As Transcript()
        Call $"Start to merge {Transcript.Length} into groups".__DEBUG_ECHO

        Dim sw = Stopwatch.StartNew
        Dim LQuery = (From obj In Transcript Select obj Group obj By obj.MappingLocation.Start Into Group).ToArray '并行LINQ有问题？？？
        Call $"Group LINQ {LQuery.Length} groups ==> {sw.ElapsedMilliseconds}ms".__DEBUG_ECHO
        Dim List As New List(Of Transcript)

        For Each TranscriptGroup In LQuery
            Dim arr = TranscriptGroup.Group.ToArray
            If arr.Length = 1 Then
                Call List.Add(arr(Scan0)) '这几个实验条件之下没有重复
            Else
                Dim total = (From obj In arr Select obj.TSSsShared).ToArray.Sum   '右端取最长的，然后所有的support都加起来
                Dim Max = (From obj In arr Select obj.TTSs).ToArray
                Dim Result = arr(Scan0).Copy(Of Transcript)

                If String.Equals(Transcript(Scan0).Strand, CLI.Forwards) Then
                    Result.Right = Max.Max '正向链则右端可变
                Else
                    Result.Left = Max.Min '反向链则左端可变
                End If

                Result.TSSsShared = total

                Call List.Add(Result)
            End If
        Next

        Return List.ToArray  '由于使用的是非并行化的LINQ，所以这里不需要再排序了
    End Function

    ReadOnly Forwards As String = NameOf(Strands.Forward)
    ReadOnly Reversed As String = NameOf(Strands.Reverse)

    '<ExportAPI("-Expr.Consist", Usage:="-Expr.Consist -in <Transcript.csv> /htseq-raw <raw.txt> /ptt <bacteria-genome.ptt> [-out <out_saved.csv> -reads.avg 90]")>
    'Public Function ExprConsistency(args As CommandLine.CommandLine) As Integer
    '    Dim [In] As String = args("-in")
    '    Dim raw As String = args("/htseq-raw")
    '    Dim PTT As String = args("/ptt")

    '    If String.IsNullOrEmpty([In]) OrElse String.IsNullOrEmpty(raw) OrElse String.IsNullOrEmpty(PTT) Then
    '        Return -1
    '    End If

    '    Dim readsAvgLen As Integer = If(String.IsNullOrEmpty(args("-reads.avg")), 90, args.GetInt32("-reads.avg"))
    '    Dim Transcripts = [In].LoadCsv(Of DocumentFormat.Transcript)(False).ToArray
    '    Transcripts = TSSsIdentification.ExprConsistency(Transcripts, raw, PTT, readsAvgLen)
    '    Dim Out As String = args("-out")

    '    If String.IsNullOrEmpty(Out) Then
    '        Out = [In] & "_rawconsistent.csv"
    '    End If

    '    Dim b As Boolean = Transcripts.SaveTo(Out, False)

    '    Return b.CLICode
    'End Function

    <ExportAPI("-Trim", Usage:="-Trim -in <Transcripts.csv> [-SharedReads 30]")>
    Public Function Trim(args As CommandLine.CommandLine) As Integer
        Dim [Shared] As Integer = args.GetValue(Of Integer)("-sharedreads", [default]:=30)
        Dim [In] As String = args("-in")
        Dim Transcripts = [In].LoadCsv(Of DocumentFormat.Transcript)(False)
        Dim LQuery = (From site In Transcripts Where site.TSSsShared >= [Shared] Select site).ToArray
        Dim b As Boolean = LQuery.SaveTo([In] & $".Trim_{[Shared]}.csv", False)
        Return b.CLICode
    End Function

    '<ExportAPI("--UpStream.Promoters", Usage:="--UpStream.Promoters -TSSs <TSSsites.csv> -ptt <bacterial.ptt> -nt <bacterial_nt.fasta> -deseq <diff.csv> [/Length 50 /Export <dir.Export> /diff.Log2 2 /identical.Log2 1]")>
    'Public Function UpStreamPromoter(args As CommandLine.CommandLine) As Integer
    '    Dim TSSs As String = args("-TSSs")
    '    Dim PTT As String = args("-PTT")
    '    Dim NT As String = args("-NT")
    '    Dim DESeq As String = args("-DESeq")
    '    Dim Export As String = args("/Export")
    '    Dim Length As Integer = args.GetValue(Of Integer)("/Length", 50)

    '    Dim DESeqCOGs = LANS.SystemsBiology.Toolkits.RNASeq.RTools.DESeq.DiffGeneCOGs(
    '        DESeq,
    '        LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.PTT.Load(PTT),
    '        args.GetValue(Of Double)("/diff.log2", 2),
    '        args.GetValue(Of Double)("/identical.log2", 1))
    '    Dim b As Boolean = Views.UpStreamPromoter(
    '        TSSs.LoadCsv(Of DocumentFormat.Transcript)(False),
    '        DESeqCOGs,
    '        LANS.SystemsBiology.SequenceModel.FASTA.FastaToken.LoadNucleotideData(NT),
    '        Length,
    '        Export)

    '    Return b.CLICode
    'End Function

    ''' <summary>
    ''' Display the brief summary information about your blastn mapping data.
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--Data.Views",
               Info:="Display the brief summary information about your blastn mapping data.",
               Usage:="--Data.Views -in &lt;blastnMapping.csv>")>
    Public Function DataView(args As CommandLine.CommandLine) As Integer
        Dim Reads = args("-in").LoadCsv(Of LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BlastnMapping)(False)
        Dim Uniques = (From obj In Reads.AsParallel Where obj.Unique Select 1).ToArray.Sum
        Dim Perfects = (From obj In Reads.AsParallel Where obj.PerfectAlignment Select 1).ToArray.Sum
        Dim PerfectsMapping = (From obj In Reads.AsParallel Where obj.Unique AndAlso obj.PerfectAlignment Select 1).ToArray.Sum

        Call $"{NameOf(Uniques)}:={Uniques};    {NameOf(Perfects)}:={Perfects};     {NameOf(PerfectsMapping)}:={PerfectsMapping}".__DEBUG_ECHO

        Return 0
    End Function

    Public Function ass()
        Dim reverse As New SortedDictionary(Of Long, List(Of ComponentModel.Loci.NucleotideLocation))
        Dim forward As New SortedDictionary(Of Long, List(Of ComponentModel.Loci.NucleotideLocation))
        Dim mappings As New DocumentStream.Linq.DataStream("F:\2015.12.26.TSSs\NY_MMX\M1.unique-perfects.Csv")  ' 读取测序数据的mapping结果

        Call mappings.ForEachBlock(Sub(source As NCBI.Extensions.LocalBLAST.Application.BlastnMapping())
                                       Dim LQuery = (From x In source
                                                     Select loci = DirectCast(x.MappingLocation.Normalization, LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation)).ToArray
                                       For Each r As NucleotideLocation In LQuery
                                           If r.Strand = Strands.Forward Then
                                               Call __add(forward, r)
                                           Else
                                               Call __add(reverse, r)
                                           End If
                                       Next
                                   End Sub)
        ' 由于是已经mapping到基因组上面的unique和perfect的reads了，所以在这里直接使用位置进行拼接

        ' 贪婪重叠拼接得到最长的contig


    End Function

    Private Sub __add(ByRef list As IDictionary(Of Long, List(Of ComponentModel.Loci.NucleotideLocation)), r As ComponentModel.Loci.NucleotideLocation)
        If list.ContainsKey(r.Left) Then
            Call list(r.Left).Add(r)
        Else
            Call list.Add(r.Left, New List(Of ComponentModel.Loci.NucleotideLocation) From {r})
        End If
    End Sub
End Module
