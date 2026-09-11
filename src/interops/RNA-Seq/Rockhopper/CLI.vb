' /********************************************************************************/
'
'  Rockhopper —— 命令行入口
'
'  复刻原始 Rockhopper 的命令行参数与默认值（见 Java/Rockhopper/CLI_API.vb 的帮助文本）：
'    -g/-c/-ff|fr|rf|rr/-d/-a/-p/-e/-s/-L/-o/-v/-SAM/-TIME/-m/-l/-y/-t/-z/-k/-j/-n/-b/-u/-w/-x
'  原实现把参数写入 CLI_API/Peregrine/Assembler 的静态字段；重写后统一写入
'  <see cref="RockhopperParameters"/>，再交给 Pipeline 执行。
'  外部 rockhopper.jar 调用（Install / RockhopperExternalCli / DE_NOVO_ASSEMBLY）已按计划移除。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.IO

''' <summary>
''' Rockhopper 命令行程序。
''' </summary>
Public Module CLI

    ''' <summary>程序版本号（与原始 Rockhopper 2.03 对齐）。</summary>
    Public Const Version As String = "2.03"

    ''' <summary>
    ''' 程序入口：解析参数并执行分析，返回进程退出码。
    ''' </summary>
    Public Function Main(args As String()) As Integer
        If args Is Nothing OrElse args.Length < 1 Then
            Call PrintUsage()
            Return 0
        End If

        Dim parameters As RockhopperParameters
        Try
            parameters = ParseArguments(args)
        Catch ex As ArgumentException
            Call Core.Logging.Output($"{vbLf}Error - {ex.Message}{vbLf}")
            Return 1
        End Try

        If parameters.ConditionFiles.Count = 0 Then
            Call Core.Logging.Output($"Error - sequencing reads files (in FASTQ, QSEQ, FASTA, SAM, or BAM format) are required as command line arguments.{vbLf}")
            Return 1
        End If

        Return Pipeline.RockhopperPipeline.Run(parameters)
    End Function

    ''' <summary>
    ''' 解析命令行参数为 <see cref="RockhopperParameters"/>。
    ''' </summary>
    ''' <exception cref="ArgumentException">参数缺失或非法时抛出。</exception>
    Public Function ParseArguments(args As String()) As RockhopperParameters
        Dim p As New RockhopperParameters()
        Dim i As Integer = 0

        While i < args.Length
            Dim arg As String = args(i)

            Select Case True
                Case arg = "-g"
                    Dim dirs As String() = requireValue(args, i, "-g", "the name of one or more directories")
                    p.GenomeDirectories = New List(Of String)()
                    For Each d As String In dirs
                        If String.IsNullOrEmpty(d) Then Continue For
                        p.GenomeDirectories.Add(d.TrimEnd("/"c, "\"c))
                    Next
                    i += 2

                Case arg = "-a"
                    p.StopAfterOneHit = parseBool(requireValue(args, i, "-a", "a boolean"))
                    i += 2

                Case arg = "-p"
                    p.NumThreads = parseInt(requireValue(args, i, "-p", "an integer"))
                    i += 2

                Case arg = "-e"
                    p.ComputeExpression = parseBool(requireValue(args, i, "-e", "a boolean"))
                    i += 2

                Case arg = "-s"
                    ' 命令行语义为"是否链特异"，内部使用 unstranded（取反）
                    p.Unstranded = Not parseBool(requireValue(args, i, "-s", "a boolean"))
                    i += 2

                Case arg = "-d"
                    p.MaxPairedEndLength = parseInt(requireValue(args, i, "-d", "an integer"))
                    i += 2

                Case arg = "-L"
                    p.Labels = splitList(requireValue(args, i, "-L", "a comma separated list of names for the conditions"))
                    i += 2

                Case arg = "-o"
                    p.OutputDirectory = requireValue(args, i, "-o", "the name of a directory")
                    i += 2

                Case arg = "-v"
                    p.Verbose = parseBool(requireValue(args, i, "-v", "a boolean"))
                    i += 2

                Case arg = "-m"
                    p.PercentMismatches = parseDouble(requireValue(args, i, "-m", "a decimal number"))
                    i += 2

                Case arg = "-l"
                    p.PercentSeedLength = parseDouble(requireValue(args, i, "-l", "a decimal number"))
                    i += 2

                Case arg = "-y"
                    p.ComputeOperons = parseBool(requireValue(args, i, "-y", "a boolean"))
                    i += 2

                Case arg = "-t"
                    p.ComputeTranscripts = parseBool(requireValue(args, i, "-t", "a boolean"))
                    i += 2

                Case arg = "-z"
                    p.TranscriptSensitivity = parseDouble(requireValue(args, i, "-z", "a number in the range [0.0,1.0]"))
                    i += 2

                Case arg = "-k"
                    p.K = parseInt(requireValue(args, i, "-k", "an integer"))
                    If p.K < 15 OrElse p.K > 31 Then
                        Throw New ArgumentException("-k must be in the range 15 to 31.")
                    End If
                    i += 2

                Case arg = "-j"
                    p.MinReadLength = parseInt(requireValue(args, i, "-j", "an integer"))
                    i += 2

                Case arg = "-n"
                    p.CapacityPower = parseInt(requireValue(args, i, "-n", "an integer"))
                    i += 2

                Case arg = "-b"
                    p.MinReadsMapping = parseInt(requireValue(args, i, "-b", "an integer"))
                    i += 2

                Case arg = "-u"
                    p.MinTranscriptLength = parseInt(requireValue(args, i, "-u", "an integer"))
                    i += 2

                Case arg = "-w"
                    p.MinSeedExpression = parseInt(requireValue(args, i, "-w", "an integer"))
                    i += 2

                Case arg = "-x"
                    p.MinExpression = parseInt(requireValue(args, i, "-x", "an integer"))
                    i += 2

                Case arg = "-c"
                    p.SingleEndOrientationReverseComplement = parseBool(requireValue(args, i, "-c", "a boolean"))
                    i += 2

                Case arg = "-ff"
                    p.PairedEndOrientation = "ff" : i += 1
                Case arg = "-fr"
                    p.PairedEndOrientation = "fr" : i += 1
                Case arg = "-rf"
                    p.PairedEndOrientation = "rf" : i += 1
                Case arg = "-rr"
                    p.PairedEndOrientation = "rr" : i += 1

                Case arg = "-TIME"
                    p.Time = True : i += 1
                Case arg = "-SAM"
                    p.OutputSAM = True : i += 1

                Case Else
                    p.ConditionFiles.Add(arg)
                    i += 1
            End Select
        End While

        Call p.Normalize()

        ' 校验参考基因组目录
        If p.GenomeDirectories IsNot Nothing Then
            For Each dir As String In p.GenomeDirectories
                If Not Directory.Exists(dir) AndAlso Not File.Exists(dir) Then
                    Throw New ArgumentException($"directory {dir} does not exist.")
                End If
            Next
        End If

        ' 校验读段文件存在性，并识别双端数据
        For Each condition As String In p.ConditionFiles
            For Each file As String In splitList(condition)
                Dim mates As String() = file.Split("%"c)
                If Not File.Exists(mates(0)) AndAlso Not Directory.Exists(mates(0)) Then
                    Throw New ArgumentException($"file {mates(0)} does not exist.")
                End If
                If mates.Length > 1 AndAlso Not File.Exists(mates(1)) Then
                    Throw New ArgumentException($"file {mates(1)} does not exist.")
                End If
                If mates.Length > 1 Then
                    ' 双端读段默认视为链特异
                    p.Unstranded = False
                End If
            Next
        Next

        Return p
    End Function

    ''' <summary>
    ''' 输出使用帮助（与原始 CLI_API.commandLineArguments 的帮助文本一致）。
    ''' </summary>
    Public Sub PrintUsage()
        Call Core.Logging.Output(vbLf & "*************************************************" & vbLf)
        Call Core.Logging.Output($"**********   Rockhopper version {Version}   **********" & vbLf)
        Call Core.Logging.Output("*************************************************" & vbLf)
        Call Core.Logging.Output(vbLf & "The Rockhopper application has the following required command line arguments." & vbLf)
        Call Core.Logging.Output(vbLf & "REQUIRED ARGUMENTS" & vbLf & vbLf)
        Call Core.Logging.Output(vbTab & "exp1A.fastq,exp1B.fastq,exp1C.fastq  exp2A.fastq,exp2B.fastq" & vbTab & "a comma separated list of sequencing files (in FASTQ, QSEQ, FASTA, SAM, or BAM format) for replicate experiments, one list per experimental condition (mate-pair files should be delimited by '%')" & vbLf)
        Call Core.Logging.Output(vbLf & "REFERENCE BASED ASSEMBLY VS. DE NOVO ASSEMBLY:" & vbLf)
        Call Core.Logging.Output("IF THE -g OPTION IS USED THEN ROCKHOPPER ALIGNS READS TO ONE OR MORE REFERENCE GENOMES," & vbLf)
        Call Core.Logging.Output("OTHERWISE, ROCKHOPPER PERFORMS DE NOVO TRANSCRIPT ASSEMBLY." & vbLf & vbLf)
        Call Core.Logging.Output(vbTab & "-g <DIR1,DIR2>" & vbTab & "a comma separated list of directories, each containing a genome file (*.fna), gene file (*.ptt), and rna file (*.rnt)" & vbLf)
        Call Core.Logging.Output(vbLf & "OPTIONAL ARGUMENTS FOR EITHER REFERENCE BASED ASSEMBLY OR DE NOVO ASSEMBLY" & vbLf & vbLf)
        Call Core.Logging.Output(vbTab & "-c <boolean>" & vbTab & "reverse complement single-end reads (default is false)" & vbLf)
        Call Core.Logging.Output(vbTab & "-ff/fr/rf/rr" & vbTab & "orientation of two mate reads for paired-end read, f=forward and r=reverse_complement (default is fr)" & vbLf)
        Call Core.Logging.Output(vbTab & "-d <integer>" & vbTab & "maximum number of bases between mate pairs for paired-end reads (default is 500)" & vbLf)
        Call Core.Logging.Output(vbTab & "-a <boolean>" & vbTab & "identify 1 alignment (true) or identify all optimal alignments (false), (default is true)" & vbLf)
        Call Core.Logging.Output(vbTab & "-p <integer>" & vbTab & "number of processors (default is self-identification of processors)" & vbLf)
        Call Core.Logging.Output(vbTab & "-e <boolean>" & vbTab & "compute differential expression for transcripts in pairs of experimental conditions (default is true)" & vbLf)
        Call Core.Logging.Output(vbTab & "-s <boolean>" & vbTab & "RNA-seq experiments are strand specific (true) or strand ambiguous (false), (default is true)" & vbLf)
        Call Core.Logging.Output(vbTab & "-L <comma separated list>" & vbTab & "labels for each condition" & vbLf)
        Call Core.Logging.Output(vbTab & "-o <DIR>" & vbTab & "directory where output files are written (default is Rockhopper_Results/)" & vbLf)
        Call Core.Logging.Output(vbTab & "-v <boolean>" & vbTab & "verbose output including raw/normalized counts aligning to each gene (default is false)" & vbLf)
        Call Core.Logging.Output(vbTab & "-SAM       " & vbTab & "output a SAM format file" & vbLf)
        Call Core.Logging.Output(vbTab & "-TIME      " & vbTab & "output time taken to execute program" & vbLf)
        Call Core.Logging.Output(vbLf & "OPTIONAL ARGUMENTS FOR REFERENCE BASED ASSEMBLY ONLY" & vbLf & vbLf)
        Call Core.Logging.Output(vbTab & "-m <number>" & vbTab & "allowed mismatches as percent of read length (default is 0.15)" & vbLf)
        Call Core.Logging.Output(vbTab & "-l <number>" & vbTab & "minimum seed as percent of read length (default is 0.33)" & vbLf)
        Call Core.Logging.Output(vbTab & "-y <boolean>" & vbTab & "compute operons (default is true)" & vbLf)
        Call Core.Logging.Output(vbTab & "-t <boolean>" & vbTab & "identify transcript boundaries including UTRs and ncRNAs (default is true)" & vbLf)
        Call Core.Logging.Output(vbTab & "-z <number>" & vbTab & "minimum expression of UTRs and ncRNAs, a number in range [0.0, 1.0] (default is 0.5)" & vbLf)
        Call Core.Logging.Output(vbLf & "OPTIONAL ARGUMENTS FOR DE NOVO ASSEMBLY ONLY" & vbLf & vbLf)
        Call Core.Logging.Output(vbTab & "-k <integer>" & vbTab & "size of k-mer, range of values is 15 to 31 (default is 25)" & vbLf)
        Call Core.Logging.Output(vbTab & "-j <integer>" & vbTab & "minimum length required to use a sequencing read after trimming/processing (default is 35)" & vbLf)
        Call Core.Logging.Output(vbTab & "-n <integer>" & vbTab & "size of k-mer hashtable is ~ 2^n (default is 25)" & vbLf)
        Call Core.Logging.Output(vbTab & "-b <integer>" & vbTab & "minimum number of full length reads required to map to a de novo assembled trancript (default is 20)" & vbLf)
        Call Core.Logging.Output(vbTab & "-u <integer>" & vbTab & "minimum length of de novo assembled transcripts (default is 2*k)" & vbLf)
        Call Core.Logging.Output(vbTab & "-w <integer>" & vbTab & "minimum count of k-mer to use it to seed a new de novo assembled transcript (default is 50)" & vbLf)
        Call Core.Logging.Output(vbTab & "-x <integer>" & vbTab & "minimum count of k-mer to use it to extend an existing de novo assembled transcript (default is 5)" & vbLf)
        Call Core.Logging.Output(vbLf)
    End Sub

#Region "Parsing helpers"

    Private Function requireValue(args As String(), i As Integer, [option] As String, expected As String) As String()
        If i = args.Length - 1 OrElse (args(i + 1).StartsWith("-") AndAlso Not args(i + 1).StartsWith("-1")) Then
            Throw New ArgumentException($"command line argument {[option]} must be followed by {expected}.")
        End If
        Return splitList(args(i + 1))
    End Function

    Private Function splitList(value As String) As String()
        Return value.Split(","c)
    End Function

    Private Function parseBool(values As String()) As Boolean
        Return Convert.ToBoolean(values(0))
    End Function

    Private Function parseInt(values As String()) As Integer
        Return Convert.ToInt32(values(0))
    End Function

    Private Function parseDouble(values As String()) As Double
        Return Convert.ToDouble(values(0))
    End Function

#End Region

End Module
