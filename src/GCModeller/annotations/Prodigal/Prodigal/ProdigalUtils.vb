' ============================================================================
' ProdigalUtils.vb - Prodigal VB.NET 基因预测程序 工具类
' 包含：FASTA读取、DNA序列操作、密码子表、模型序列化
' ============================================================================

Imports System.IO
Imports System.Text
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' DNA序列操作工具类
''' </summary>
Public Class SequenceUtils

    ''' <summary>互补碱基映射表</summary>
    Private Shared ReadOnly ComplementMap As Dictionary(Of Char, Char) =
        New Dictionary(Of Char, Char) From {
            {"A"c, "T"c}, {"T"c, "A"c}, {"G"c, "C"c}, {"C"c, "G"c},
            {"N"c, "N"c}, {"R"c, "Y"c}, {"Y"c, "R"c}, {"M"c, "K"c},
            {"K"c, "M"c}, {"S"c, "S"c}, {"W"c, "W"c}, {"H"c, "D"c},
            {"D"c, "H"c}, {"B"c, "V"c}, {"V"c, "B"c}
        }

    ''' <summary>标准遗传密码表</summary>
    Private Shared ReadOnly CodonTable As Dictionary(Of String, String) = InitCodonTable()

    ''' <summary>起始密码子集合</summary>
    Public Shared ReadOnly StartCodons As New HashSet(Of String) From {"ATG", "GTG", "TTG"}

    ''' <summary>终止密码子集合</summary>
    Public Shared ReadOnly StopCodons As New HashSet(Of String) From {"TAA", "TAG", "TGA"}

    Private Shared Function InitCodonTable() As Dictionary(Of String, String)
        Dim table As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        ' 标准遗传密码表（细菌）
        Dim entries = {
            "TTT", "F", "TTC", "F", "TTA", "L", "TTG", "L",
            "CTT", "L", "CTC", "L", "CTA", "L", "CTG", "L",
            "ATT", "I", "ATC", "I", "ATA", "I", "ATG", "M",
            "GTT", "V", "GTC", "V", "GTA", "V", "GTG", "V",
            "TCT", "S", "TCC", "S", "TCA", "S", "TCG", "S",
            "CCT", "P", "CCC", "P", "CCA", "P", "CCG", "P",
            "ACT", "T", "ACC", "T", "ACA", "T", "ACG", "T",
            "GCT", "A", "GCC", "A", "GCA", "A", "GCG", "A",
            "TAT", "Y", "TAC", "Y", "TAA", "*", "TAG", "*",
            "CAT", "H", "CAC", "H", "CAA", "Q", "CAG", "Q",
            "AAT", "N", "AAC", "N", "AAA", "K", "AAG", "K",
            "GAT", "D", "GAC", "D", "GAA", "E", "GAG", "E",
            "TGT", "C", "TGC", "C", "TGA", "*", "TGG", "W",
            "CGT", "R", "CGC", "R", "CGA", "R", "CGG", "R",
            "AGT", "S", "AGC", "S", "AGA", "R", "AGG", "R",
            "GGT", "G", "GGC", "G", "GGA", "G", "GGG", "G"
        }
        For i As Integer = 0 To entries.Length - 2 Step 2
            table(entries(i)) = entries(i + 1)
        Next
        Return table
    End Function

    ''' <summary>
    ''' 计算反向互补链
    ''' </summary>
    Public Shared Function ReverseComplement(seq As String) As String
        Dim rc As New StringBuilder(seq.Length)
        For i As Integer = seq.Length - 1 To 0 Step -1
            Dim c = seq(i)
            If ComplementMap.ContainsKey(c) Then
                rc.Append(ComplementMap(c))
            Else
                rc.Append("N"c)
            End If
        Next
        Return rc.ToString()
    End Function

    ''' <summary>
    ''' 翻译核苷酸序列为氨基酸序列
    ''' </summary>
    Public Shared Function Translate(nucSeq As String) As String
        If String.IsNullOrEmpty(nucSeq) Then Return ""
        Dim protein As New StringBuilder()
        For i As Integer = 0 To nucSeq.Length - 3 Step 3
            If i + 2 < nucSeq.Length Then
                Dim codon = nucSeq.Substring(i, 3)
                If CodonTable.ContainsKey(codon) Then
                    protein.Append(CodonTable(codon))
                Else
                    protein.Append("X"c)
                End If
            End If
        Next
        Return protein.ToString()
    End Function

    ''' <summary>
    ''' 判断是否为起始密码子
    ''' </summary>
    Public Shared Function IsStartCodon(codon As String) As Boolean
        Return StartCodons.Contains(codon.ToUpper())
    End Function

    ''' <summary>
    ''' 判断是否为终止密码子
    ''' </summary>
    Public Shared Function IsStopCodon(codon As String) As Boolean
        Return StopCodons.Contains(codon.ToUpper())
    End Function

    ''' <summary>
    ''' 计算GC含量
    ''' </summary>
    Public Shared Function ComputeGcContent(seq As String) As Double
        If String.IsNullOrEmpty(seq) Then Return 0.0
        Dim gc As Integer = 0
        For Each c In seq
            If c = "G"c OrElse c = "C"c Then gc += 1
        Next
        Return gc / CDbl(seq.Length)
    End Function

    ''' <summary>
    ''' 将六聚体转换为0-4095的索引值
    ''' A=0, C=1, G=2, T=3
    ''' </summary>
    Public Shared Function HexamerToIndex(hexamer As String) As Integer
        If hexamer Is Nothing OrElse hexamer.Length <> 6 Then Return -1
        Dim index As Integer = 0
        For i As Integer = 0 To 5
            Select Case Char.ToUpper(hexamer(i))
                Case "A"c : index = index * 4 + 0
                Case "C"c : index = index * 4 + 1
                Case "G"c : index = index * 4 + 2
                Case "T"c : index = index * 4 + 3
                Case Else : Return -1  ' 含N等非标准碱基
            End Select
        Next
        Return index
    End Function

    ''' <summary>
    ''' 将索引值转换回六聚体字符串
    ''' </summary>
    Public Shared Function IndexToHexamer(index As Integer) As String
        Dim bases = {"A"c, "C"c, "G"c, "T"c}
        Dim chars(5) As Char
        For i As Integer = 5 To 0 Step -1
            chars(i) = bases(index Mod 4)
            index \= 4
        Next
        Return New String(chars)
    End Function

    ''' <summary>
    ''' 获取序列中指定位置的密码子
    ''' </summary>
    Public Shared Function GetCodon(seq As String, position As Integer) As String
        If position + 2 < seq.Length AndAlso position >= 0 Then
            Return seq.Substring(position, 3)
        End If
        Return ""
    End Function

    ''' <summary>
    ''' 获取指定位置上游的序列（用于RBS检测）
    ''' </summary>
    Public Shared Function GetUpstreamSequence(seq As String, startPos As Integer, upstreamLen As Integer) As String
        Dim start = Math.Max(0, startPos - upstreamLen)
        Dim len = startPos - start
        If len <= 0 Then Return ""
        Return seq.Substring(start, len)
    End Function

End Class

''' <summary>
''' 训练模型序列化/反序列化工具
''' </summary>
Public Class ModelSerializer

    ''' <summary>
    ''' 将训练模型保存到文件
    ''' </summary>
    Public Shared Sub Save(model As TrainingModel, filePath As String)
        Using writer As New System.IO.StreamWriter(filePath, False, Encoding.UTF8)
            writer.WriteLine($"PRODIGAL_VB_MODEL_v{model.Version}")
            writer.WriteLine($"GC_CONTENT={model.GcContent}")
            writer.WriteLine($"AVG_GENE_LENGTH={model.AvgGeneLength}")
            writer.WriteLine($"TRAINED={model.Trained}")
            writer.WriteLine($"ITERATION_COUNT={model.IterationCount}")
            writer.WriteLine($"TRAINING_GENE_COUNT={model.TrainingGeneCount}")
            writer.WriteLine($"CODING_HEXAMER_TOTAL={model.CodingHexamerTotal}")
            writer.WriteLine($"NONCODING_HEXAMER_TOTAL={model.NoncodingHexamerTotal}")

            ' 起始密码子频率
            writer.WriteLine("START_CODON_FREQ")
            For Each kv In model.StartCodonFreq
                writer.WriteLine($"  {kv.Key}={kv.Value}")
            Next

            ' RBS模体得分
            writer.WriteLine("RBS_MOTIF_SCORES")
            For Each kv In model.RbsMotifScores
                writer.WriteLine($"  {kv.Key}={kv.Value}")
            Next

            ' RBS PWM
            writer.WriteLine("RBS_PWM")
            For i As Integer = 0 To 5
                Dim parts As New List(Of String)
                For j As Integer = 0 To 3
                    parts.Add(model.RbsPwm(i, j).ToString("G"))
                Next
                writer.WriteLine($"  {String.Join(",", parts)}")
            Next

            ' 六聚体得分（只保存非零值以节省空间）
            writer.WriteLine("HEXAMER_SCORES")
            For i As Integer = 0 To 4095
                If Math.Abs(model.HexamerScores(i)) > 0.0001 Then
                    writer.WriteLine($"  {i}={model.HexamerScores(i):G}")
                End If
            Next

            ' 编码区六聚体计数
            writer.WriteLine("CODING_HEXAMER_COUNT")
            For i As Integer = 0 To 4095
                If model.CodingHexamerCount(i) > 0 Then
                    writer.WriteLine($"  {i}={model.CodingHexamerCount(i):G}")
                End If
            Next

            ' 非编码区六聚体计数
            writer.WriteLine("NONCODING_HEXAMER_COUNT")
            For i As Integer = 0 To 4095
                If model.NoncodingHexamerCount(i) > 0 Then
                    writer.WriteLine($"  {i}={model.NoncodingHexamerCount(i):G}")
                End If
            Next

            writer.WriteLine("END_MODEL")
        End Using
    End Sub

    ''' <summary>
    ''' 从文件加载训练模型
    ''' </summary>
    Public Shared Function Load(filePath As String) As TrainingModel
        If Not File.Exists(filePath) Then
            Throw New FileNotFoundException($"模型文件未找到: {filePath}")
        End If

        Dim model As New TrainingModel()
        Dim lines = File.ReadAllLines(filePath)
        Dim section As String = ""

        For Each line In lines
            Dim trimmed = line.Trim()
            If String.IsNullOrEmpty(trimmed) Then Continue For

            ' 检测节标题
            If trimmed = "START_CODON_FREQ" OrElse trimmed = "RBS_MOTIF_SCORES" OrElse
               trimmed = "RBS_PWM" OrElse trimmed = "HEXAMER_SCORES" OrElse
               trimmed = "CODING_HEXAMER_COUNT" OrElse trimmed = "NONCODING_HEXAMER_COUNT" OrElse
               trimmed = "END_MODEL" Then
                section = trimmed
                Continue For
            End If

            ' 解析头部信息
            If section = "" Then
                If trimmed.StartsWith("GC_CONTENT=") Then
                    model.GcContent = Double.Parse(trimmed.Substring(11))
                ElseIf trimmed.StartsWith("AVG_GENE_LENGTH=") Then
                    model.AvgGeneLength = Double.Parse(trimmed.Substring(17))
                ElseIf trimmed.StartsWith("TRAINED=") Then
                    model.Trained = Boolean.Parse(trimmed.Substring(8))
                ElseIf trimmed.StartsWith("ITERATION_COUNT=") Then
                    model.IterationCount = Integer.Parse(trimmed.Substring(16))
                ElseIf trimmed.StartsWith("TRAINING_GENE_COUNT=") Then
                    model.TrainingGeneCount = Integer.Parse(trimmed.Substring(20))
                ElseIf trimmed.StartsWith("CODING_HEXAMER_TOTAL=") Then
                    model.CodingHexamerTotal = Double.Parse(trimmed.Substring(21))
                ElseIf trimmed.StartsWith("NONCODING_HEXAMER_TOTAL=") Then
                    model.NoncodingHexamerTotal = Double.Parse(trimmed.Substring(24))
                End If
                Continue For
            End If

            ' 解析各节数据
            If section = "START_CODON_FREQ" Then
                Dim eqIdx = trimmed.IndexOf("="c)
                If eqIdx > 0 Then
                    Dim key = trimmed.Substring(0, eqIdx)
                    Dim val = Double.Parse(trimmed.Substring(eqIdx + 1))
                    model.StartCodonFreq(key) = val
                End If

            ElseIf section = "RBS_MOTIF_SCORES" Then
                Dim eqIdx = trimmed.IndexOf("="c)
                If eqIdx > 0 Then
                    Dim key = trimmed.Substring(0, eqIdx)
                    Dim val = Double.Parse(trimmed.Substring(eqIdx + 1))
                    model.RbsMotifScores(key) = val
                End If

            ElseIf section = "RBS_PWM" Then
                Static pwmRow As Integer = 0
                If pwmRow > 5 Then pwmRow = 0
                Dim parts = trimmed.Split(","c)
                For j As Integer = 0 To Math.Min(3, parts.Length - 1)
                    model.RbsPwm(pwmRow, j) = Double.Parse(parts(j))
                Next
                pwmRow += 1

            ElseIf section = "HEXAMER_SCORES" Then
                Dim eqIdx = trimmed.IndexOf("="c)
                If eqIdx > 0 Then
                    Dim idx = Integer.Parse(trimmed.Substring(0, eqIdx))
                    Dim val = Double.Parse(trimmed.Substring(eqIdx + 1))
                    If idx >= 0 AndAlso idx < 4096 Then
                        model.HexamerScores(idx) = val
                    End If
                End If

            ElseIf section = "CODING_HEXAMER_COUNT" Then
                Dim eqIdx = trimmed.IndexOf("="c)
                If eqIdx > 0 Then
                    Dim idx = Integer.Parse(trimmed.Substring(0, eqIdx))
                    Dim val = Double.Parse(trimmed.Substring(eqIdx + 1))
                    If idx >= 0 AndAlso idx < 4096 Then
                        model.CodingHexamerCount(idx) = val
                    End If
                End If

            ElseIf section = "NONCODING_HEXAMER_COUNT" Then
                Dim eqIdx = trimmed.IndexOf("="c)
                If eqIdx > 0 Then
                    Dim idx = Integer.Parse(trimmed.Substring(0, eqIdx))
                    Dim val = Double.Parse(trimmed.Substring(eqIdx + 1))
                    If idx >= 0 AndAlso idx < 4096 Then
                        model.NoncodingHexamerCount(idx) = val
                    End If
                End If
            End If
        Next

        Return model
    End Function

End Class

''' <summary>
''' 结果输出工具
''' </summary>
Public Class ResultWriter

    ''' <summary>
    ''' 输出GFF3格式基因预测结果
    ''' </summary>
    Public Shared Sub WriteGff3(results As List(Of PredictionResult), filePath As String)
        Dim geneList As New List(Of Feature)

        For Each result In results
            For Each gene As PredictedGene In result.Genes
                Dim partialTag = If(String.IsNullOrEmpty(gene.PartialType), ".", gene.PartialType)

                Call geneList.Add(New Feature With {
                    .ID = "gene_" & gene.GeneIndex,
                    .strand = gene.Strand.GetStrands,
                    .left = gene.Start,
                    .right = gene.End,
                    .score = gene.TotalScore,
                    .frame = gene.Frame + 1,
                    .seqname = result.SeqId,
                    .feature = "CDS",
                    .source = "Prodigal",
                    .Product = "-",
                    .comments = "-",
                    .COG = "-",
                    .attributes = New Dictionary(Of String, String) From {
                        {"start_codon", gene.StartCodon},
                        {"rbs_motif", gene.RbsMotif},
                        {"cscore", gene.CodingScore},
                        {"sscore", gene.StartScore},
                        {"rscore", gene.RbsScore},
                        {"tscore", gene.TypeScore},
                        {"uscore", gene.UpstreamScore},
                        {"partial", partialTag}
                    }
                })
            Next
        Next

        Dim gff As New GFFTable With {.features = geneList.ToArray, .[date] = Now.ToString, .GffVersion = 3, .processor = "Prodigal"}

        Call gff.Save(filePath)
    End Sub

    ''' <summary>
    ''' 输出蛋白质FASTA文件
    ''' </summary>
    Public Shared Sub WriteProteinFasta(results As List(Of PredictionResult), filePath As String)
        Dim seqs As New List(Of FastaSeq)
        For Each result In results
            For Each gene As PredictedGene In result.Genes
                Call seqs.Add(gene.CreateProteinFasta(result.SeqId))
            Next
        Next

        Call New FastaFile(seqs).Save(filePath)
    End Sub

    ''' <summary>
    ''' 输出核苷酸FASTA文件
    ''' </summary>
    Public Shared Sub WriteNucleotideFasta(results As List(Of PredictionResult), filePath As String)
        Dim seqs As New List(Of FastaSeq)
        For Each result In results
            For Each gene As PredictedGene In result.Genes
                Call seqs.Add(gene.CreateGeneFasta(result.SeqId))
            Next
        Next

        Call New FastaFile(seqs).Save(filePath)
    End Sub

    ''' <summary>
    ''' 输出详细得分表（制表符分隔）
    ''' </summary>
    Public Shared Sub WriteScoreTable(results As List(Of PredictionResult), filePath As String)
        Using writer As New System.IO.StreamWriter(filePath, False, Encoding.UTF8)
            writer.WriteLine($"SeqID{vbTab}GeneIndex{vbTab}Start{vbTab}End{vbTab}Strand{vbTab}Length{vbTab}" &
                $"StartCodon{vbTab}StopCodon{vbTab}TotalScore{vbTab}CodingScore{vbTab}StartScore{vbTab}" &
                $"RbsScore{vbTab}TypeScore{vbTab}UpstreamScore{vbTab}RbsMotif{vbTab}RbsSpacing{vbTab}Partial")
            For Each result In results
                For Each gene As PredictedGene In result.Genes
                    writer.WriteLine($"{result.SeqId}{vbTab}{gene.GeneIndex}{vbTab}{gene.Start}{vbTab}{gene.End}{vbTab}" &
                        $"{gene.Strand}{vbTab}{gene.Length}{vbTab}{gene.StartCodon}{vbTab}{gene.StopCodon}{vbTab}" &
                        $"{gene.TotalScore:F4}{vbTab}{gene.CodingScore:F4}{vbTab}{gene.StartScore:F4}{vbTab}" &
                        $"{gene.RbsScore:F4}{vbTab}{gene.TypeScore:F4}{vbTab}{gene.UpstreamScore:F4}{vbTab}" &
                        $"{gene.RbsMotif}{vbTab}{gene.RbsSpacing}{vbTab}{gene.PartialType}")
                Next
            Next
        End Using
    End Sub

    ''' <summary>
    ''' 控制台输出预测结果摘要
    ''' </summary>
    Public Shared Sub PrintSummary(results As List(Of PredictionResult))
        Console.WriteLine()
        Console.WriteLine("="c, 70)
        Console.WriteLine("  Prodigal VB.NET 基因预测结果摘要")
        Console.WriteLine("="c, 70)

        Dim totalGenes As Integer = 0
        For Each result In results
            Console.WriteLine($"  序列: {result.SeqId}  (长度: {result.SeqLength:N0} bp)")
            Console.WriteLine($"    预测基因数: {result.Genes.Count}")
            If result.Genes.Count > 0 Then
                Dim avgLen = result.Genes.Average(Function(g) g.Length)
                Dim avgScore = result.Genes.Average(Function(g) g.TotalScore)
                Console.WriteLine($"    平均基因长度: {avgLen:F0} bp")
                Console.WriteLine($"    平均得分: {avgScore:F2}")
            End If
            totalGenes += result.Genes.Count
            Console.WriteLine()
        Next

        Console.WriteLine($"  总计预测基因数: {totalGenes}")
        Console.WriteLine("="c, 70)
        Console.WriteLine()
    End Sub

End Class

