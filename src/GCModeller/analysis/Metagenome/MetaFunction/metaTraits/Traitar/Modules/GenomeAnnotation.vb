' ============================================================================
' GenomeAnnotation.vb - 模块1：基因组注释与特征化模块
'
' 论文对应：
'   "基因组注释与特征提取（从DNA到蛋白质家族）"
'
' 核心功能：
'   1. 基因预测：调用Prodigal将DNA序列翻译为氨基酸序列
'   2. 蛋白质家族注释：调用HMMER3.0的hmmsearch与Pfam数据库比对
'   3. 过滤与二值化：设定阈值（比特分数≥25，E值≤1e-2），
'      将Pfam家族计数转化为存在(1)/缺失(0)的二元矩阵X
'
' 算法原理：
'   - 隐马尔可夫模型(HMM)：用于蛋白质家族比对（HMMER）
'   - 基因预测算法：基于动态规划的Prodigal
'   - 二值化处理：将Pfam家族计数转化为0/1矩阵
' ============================================================================

Imports System.IO
Imports System.Runtime.InteropServices
Imports SMRUCC.genomics.Analysis.SequenceTools.HMMER

Namespace TraitarVB.Modules

    ''' <summary>
    ''' 模块1：基因组注释与特征化模块
    ''' 将输入的DNA/蛋白质序列转化为机器可读的特征矩阵（系统发育谱）
    ''' </summary>
    Public Class GenomeAnnotation

        ' HMMER搜索的默认参数
        Public Const DEFAULT_BITSCORE_THRESHOLD As Double = 25.0
        Public Const DEFAULT_EVALUE_THRESHOLD As Double = 0.01

        ' Prodigal 和 HMMER 可执行文件路径
        Private _prodigalPath As String
        Private _hmmsearchPath As String
        Private _pfamDbPath As String

        ''' <summary>
        ''' 构造函数
        ''' </summary>
        ''' <param name="prodigalPath">Prodigal可执行文件路径</param>
        ''' <param name="hmmsearchPath">HMMER hmmsearch可执行文件路径</param>
        ''' <param name="pfamDbPath">Pfam数据库HMM文件路径</param>
        Public Sub New(Optional prodigalPath As String = "prodigal",
                       Optional hmmsearchPath As String = "hmmsearch",
                       Optional pfamDbPath As String = "Pfam-A.hmm")
            _prodigalPath = prodigalPath
            _hmmsearchPath = hmmsearchPath
            _pfamDbPath = pfamDbPath
        End Sub

        ' ================================================================
        ' 步骤1a：基因预测（Prodigal）
        ' ================================================================

        ''' <summary>
        ''' 调用Prodigal进行基因预测
        ''' 论文：如果输入的是核苷酸序列(DNA FASTA)，Traitar会使用Prodigal软件
        '''       进行基因预测，将其翻译为氨基酸序列
        ''' </summary>
        ''' <param name="dnaFastaPath">输入DNA FASTA文件路径</param>
        ''' <param name="outputProteinPath">输出蛋白质FASTA文件路径</param>
        ''' <param name="outputGffPath">输出GFF文件路径</param>
        ''' <returns>是否成功</returns>
        Public Function RunProdigal(dnaFastaPath As String,
                                    outputProteinPath As String,
                                    Optional outputGffPath As String = Nothing) As Boolean
            Try
                Dim args As String = String.Format(" -i ""{0}"" -a ""{1}""",
                                                    dnaFastaPath, outputProteinPath)
                If outputGffPath IsNot Nothing Then
                    args &= String.Format(" -f gff -o ""{0}""", outputGffPath)
                Else
                    args &= " -f gff -o /dev/null"
                End If

                Dim psi As New ProcessStartInfo()
                psi.FileName = _prodigalPath
                psi.Arguments = args
                psi.UseShellExecute = False
                psi.RedirectStandardOutput = True
                psi.RedirectStandardError = True
                psi.CreateNoWindow = True

                Using proc As Process = Process.Start(psi)
                    proc.WaitForExit()
                    Return proc.ExitCode = 0
                End Using
            Catch ex As Exception
                Console.Error.WriteLine("Prodigal运行失败: " & ex.Message)
                Return False
            End Try
        End Function

        ' ================================================================
        ' 步骤1b：蛋白质家族注释（HMMER）
        ' ================================================================

        ''' <summary>
        ''' 调用HMMER3.0的hmmsearch命令，将氨基酸序列与Pfam数据库比对
        ''' 论文：使用HMMER3.0的hmmsearch命令，将氨基酸序列与Pfam数据库(版本27.0)
        '''       进行比对，注释出包含的蛋白质家族
        ''' </summary>
        ''' <param name="proteinFastaPath">蛋白质FASTA文件路径</param>
        ''' <param name="outputDomtbloutPath">输出domtblout文件路径</param>
        ''' <returns>是否成功</returns>
        Public Function RunHmmsearch(proteinFastaPath As String,
                                     outputDomtbloutPath As String) As Boolean
            Try
                Dim args As String = String.Format(
                    " --domtblout ""{0}"" --cpu 4 ""{1}"" ""{2}""",
                    outputDomtbloutPath, _pfamDbPath, proteinFastaPath)

                Dim psi As New ProcessStartInfo()
                psi.FileName = _hmmsearchPath
                psi.Arguments = args
                psi.UseShellExecute = False
                psi.RedirectStandardOutput = True
                psi.RedirectStandardError = True
                psi.CreateNoWindow = True

                Using proc As Process = Process.Start(psi)
                    proc.WaitForExit()
                    Return proc.ExitCode = 0
                End Using
            Catch ex As Exception
                Console.Error.WriteLine("hmmsearch运行失败: " & ex.Message)
                Return False
            End Try
        End Function

        ' ================================================================
        ' 步骤1c：完整注释流程
        ' ================================================================

        ''' <summary>
        ''' 完整的基因组注释流程
        ''' 1. 解析GFF文件获取蛋白信息
        ''' 2. 解析蛋白质FASTA文件
        ''' 3. 运行HMMER搜索Pfam家族（或解析已有domtblout）
        ''' 4. 过滤并构建二值化特征矩阵
        ''' </summary>
        ''' <param name="gffPath">GFF文件路径（可为Nothing）</param>
        ''' <param name="proteinFastaPath">蛋白质FASTA文件路径</param>
        ''' <param name="domtbloutPath">HMMER domtblout文件路径（可为Nothing，则自动运行hmmsearch）</param>
        ''' <param name="bitScoreThreshold">比特分数阈值</param>
        ''' <param name="evalueThreshold">E值阈值</param>
        ''' <returns>基因组样本（含系统发育谱）</returns>
        Public Function AnnotateGenome(
            Optional gffPath As String = Nothing,
            Optional proteinFastaPath As String = Nothing,
            Optional domtbloutPath As String = Nothing,
            Optional bitScoreThreshold As Double = DEFAULT_BITSCORE_THRESHOLD,
            Optional evalueThreshold As Double = DEFAULT_EVALUE_THRESHOLD) As Models.GenomeSample

            Dim sample As New Models.GenomeSample()

            ' 1. 解析GFF文件
            If gffPath IsNot Nothing AndAlso File.Exists(gffPath) Then
                Console.WriteLine("[模块1] 解析GFF文件: " & gffPath)
                sample.Proteins = Utils.FileParser.ParseGFF(gffPath)
                sample.SourceFile = gffPath

                ' 尝试从GFF中提取Pfam注释
                Dim gffPfams As List(Of PfamAnnotation) = Utils.FileParser.ExtractPfamFromGFF(gffPath)
                If gffPfams.Count > 0 Then
                    Console.WriteLine("[模块1] 从GFF中提取到 {0} 条Pfam注释", gffPfams.Count)
                    sample.PfamAnnotations.AddRange(gffPfams)
                End If
            End If

            ' 2. 解析蛋白质FASTA文件
            If proteinFastaPath IsNot Nothing AndAlso File.Exists(proteinFastaPath) Then
                Console.WriteLine("[模块1] 解析蛋白质FASTA文件: " & proteinFastaPath)
                Dim proteins As List(Of Models.ProteinSequence) = Utils.FileParser.ParseFasta(proteinFastaPath)
                If sample.Proteins.Count = 0 Then
                    sample.Proteins = proteins
                    sample.SourceFile = proteinFastaPath
                End If

                ' 设置样本ID
                If sample.SampleId Is Nothing Then
                    sample.SampleId = Path.GetFileNameWithoutExtension(proteinFastaPath)
                End If
            End If

            ' 3. 运行HMMER或解析已有domtblout
            If domtbloutPath IsNot Nothing AndAlso File.Exists(domtbloutPath) Then
                Console.WriteLine("[模块1] 解析HMMER domtblout文件: " & domtbloutPath)
                Dim anns As List(Of PfamAnnotation) = Utils.FileParser.ParseHmmsearchDomtblout(domtbloutPath)
                sample.PfamAnnotations.AddRange(anns)
            ElseIf proteinFastaPath IsNot Nothing AndAlso File.Exists(proteinFastaPath) Then
                ' 自动运行hmmsearch
                Console.WriteLine("[模块1] 运行HMMER hmmsearch...")
                Dim tempOut As String = Path.GetTempFileName()
                If RunHmmsearch(proteinFastaPath, tempOut) Then
                    Dim anns As List(Of PfamAnnotation) = Utils.FileParser.ParseHmmsearchDomtblout(tempOut)
                    sample.PfamAnnotations.AddRange(anns)
                End If
                Try
                    File.Delete(tempOut)
                Catch
                End Try
            End If

            ' 4. 构建二值化特征矩阵（系统发育谱）
            Console.WriteLine("[模块1] 构建系统发育谱（二值化特征矩阵）...")
            Console.WriteLine("       比特分数阈值: {0}", bitScoreThreshold)
            Console.WriteLine("       E值阈值: {0}", evalueThreshold)
            sample.BuildPhyleticProfile(bitScoreThreshold, evalueThreshold)

            Console.WriteLine("[模块1] 注释完成: {0} 个蛋白, {1} 条Pfam注释, {2} 个Pfam家族",
                              sample.Proteins.Count, sample.PfamAnnotations.Count, sample.PfamCount)

            Return sample
        End Function

        ' ================================================================
        ' 辅助方法
        ' ================================================================

        ''' <summary>
        ''' 将多个样本的特征矩阵合并为二维矩阵
        ''' 论文：将每个样本中各Pfam家族的数量转化为存在(1)或缺失(0)的二元矩阵X
        ''' </summary>
        Public Function BuildFeatureMatrix(samples As List(Of Models.GenomeSample),
                                           <Out()> ByRef allPfamIds As List(Of String)) As Integer(,)
            ' 收集所有Pfam ID
            Dim pfamSet As New HashSet(Of String)()
            For Each s As Models.GenomeSample In samples
                For Each kvp As KeyValuePair(Of String, Integer) In s.PhyleticProfile
                    pfamSet.Add(kvp.Key)
                Next
            Next
            allPfamIds = pfamSet.ToList()
            allPfamIds.Sort()

            ' 构建矩阵：行=样本，列=Pfam家族
            Dim nSamples As Integer = samples.Count
            Dim nFeatures As Integer = allPfamIds.Count
            Dim matrix As Integer(,) = New Integer(nSamples - 1, nFeatures - 1) {}

            For i As Integer = 0 To nSamples - 1
                For j As Integer = 0 To nFeatures - 1
                    Dim pfamId As String = allPfamIds(j)
                    If samples(i).PhyleticProfile.ContainsKey(pfamId) AndAlso
                       samples(i).PhyleticProfile(pfamId) = 1 Then
                        matrix(i, j) = 1
                    Else
                        matrix(i, j) = 0
                    End If
                Next
            Next

            Return matrix
        End Function

        ''' <summary>
        ''' 打印特征矩阵（用于调试）
        ''' </summary>
        Public Sub PrintFeatureMatrix(samples As List(Of Models.GenomeSample),
                                      allPfamIds As List(Of String),
                                      matrix As Integer(,))
            Console.Write("Sample" & ControlChars.Tab)
            For Each pid As String In allPfamIds
                Console.Write(pid & ControlChars.Tab)
            Next
            Console.WriteLine()

            For i As Integer = 0 To samples.Count - 1
                Console.Write(samples(i).SampleId & ControlChars.Tab)
                For j As Integer = 0 To allPfamIds.Count - 1
                    Console.Write(matrix(i, j) & ControlChars.Tab)
                Next
                Console.WriteLine()
            Next
        End Sub

        ' ================================================================
        ' 便捷方法（供Program.vb调用）
        ' ================================================================

        ''' <summary>
        ''' 从HMMER domtblout文件注释基因组
        ''' </summary>
        Public Function AnnotateFromDomtblout(domtbloutPath As String,
                                              Optional bitScoreThreshold As Double = DEFAULT_BITSCORE_THRESHOLD,
                                              Optional evalueThreshold As Double = DEFAULT_EVALUE_THRESHOLD) As Models.GenomeSample
            Return AnnotateGenome(Nothing, Nothing, domtbloutPath, bitScoreThreshold, evalueThreshold)
        End Function

        ''' <summary>
        ''' 从蛋白质FASTA文件注释基因组（自动运行HMMER）
        ''' </summary>
        Public Function AnnotateFromFasta(proteinFastaPath As String,
                                          Optional bitScoreThreshold As Double = DEFAULT_BITSCORE_THRESHOLD,
                                          Optional evalueThreshold As Double = DEFAULT_EVALUE_THRESHOLD) As Models.GenomeSample
            Return AnnotateGenome(Nothing, proteinFastaPath, Nothing, bitScoreThreshold, evalueThreshold)
        End Function

        ''' <summary>
        ''' 从GFF文件注释基因组
        ''' </summary>
        Public Function AnnotateFromGFF(gffPath As String,
                                        Optional bitScoreThreshold As Double = DEFAULT_BITSCORE_THRESHOLD,
                                        Optional evalueThreshold As Double = DEFAULT_EVALUE_THRESHOLD) As Models.GenomeSample
            Return AnnotateGenome(gffPath, Nothing, Nothing, bitScoreThreshold, evalueThreshold)
        End Function

        ''' <summary>
        ''' 从GFF和蛋白质FASTA文件注释基因组
        ''' </summary>
        Public Function AnnotateFromGFFAndFasta(gffPath As String,
                                                 proteinFastaPath As String,
                                                 Optional domtbloutPath As String = Nothing,
                                                 Optional bitScoreThreshold As Double = DEFAULT_BITSCORE_THRESHOLD,
                                                 Optional evalueThreshold As Double = DEFAULT_EVALUE_THRESHOLD) As Models.GenomeSample
            Return AnnotateGenome(gffPath, proteinFastaPath, domtbloutPath, bitScoreThreshold, evalueThreshold)
        End Function

    End Class

End Namespace
