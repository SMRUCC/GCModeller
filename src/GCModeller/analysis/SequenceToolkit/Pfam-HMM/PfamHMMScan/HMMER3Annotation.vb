Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.DataMining.HiddenMarkovChain
Imports Microsoft.VisualBasic.DataMining.HiddenMarkovChain.Models
Imports Microsoft.VisualBasic.My.JavaScript

' ============================================================================
' HMMER3蛋白质序列分类注释完整模块
' 
' 基于现有HMM算法框架，实现HMMER3格式模型文件的读取和蛋白质序列分类注释功能
' 
' 包含以下组件：
'   1. HMMER3Parser - HMMER3模型文件解析器
'   2. ProfileHMM - Profile HMM模型类
'   3. FastaParser - FASTA格式序列解析器
'   4. ProteinSequence - 蛋白质序列类
'   5. AnnotationResult - 注释结果类
'   6. ProteinAnnotator - 蛋白质序列分类注释器
'   7. AnnotationOutput - 注释结果输出器
' 
' Author: 基于用户现有HMM代码框架扩展
' Copyright (c) 2024 GPL3 Licensed
' 
' 使用方法：
'   Dim annotator As New ProteinAnnotator()
'   annotator.LoadModel("K00001.hmm.txt")
'   Dim proteins As List(Of ProteinSequence) = FastaParser.Parse("proteins.fasta")
'   annotator.AnnotateAll(proteins)
'   File.WriteAllText("results.tsv", AnnotationOutput.ToTsv(proteins))
' ============================================================================

Namespace HMMER3

#Region "HMMER3模型解析器"

    ''' <summary>
    ''' HMMER3模型文件解析器
    ''' 用于读取.hmm格式的HMMER3模型文件
    ''' </summary>
    ''' <remarks>
    ''' HMMER3文件格式说明：
    ''' - HMMER3/f: 文件格式标识
    ''' - NAME: 模型名称
    ''' - LENG: 模型长度（匹配状态数）
    ''' - ALPH: 字母表类型（amino表示蛋白质）
    ''' - HMM: 概率矩阵部分
    '''   - COMPO行: 背景分布
    '''   - 每个状态包含:
    '''     - 匹配发射概率（20个氨基酸）
    '''     - 插入发射概率（20个氨基酸）
    '''     - 转移概率（7个：m->m, m->i, m->d, i->m, i->i, d->m, d->d）
    ''' </remarks>
    Public Class HMMER3Parser

        ' 氨基酸字母表顺序（HMMER3标准顺序）
        Public Shared ReadOnly AA_ALPHABET As String() = {
            "A", "C", "D", "E", "F", "G", "H", "I", "K", "L",
            "M", "N", "P", "Q", "R", "S", "T", "V", "W", "Y"
        }

        ''' <summary>
        ''' 解析HMMER3模型文件
        ''' </summary>
        ''' <param name="filePath">HMMER3模型文件路径</param>
        ''' <returns>解析后的ProfileHMM对象</returns>
        Public Shared Function Parse(filePath As String) As ProfileHMM
            If Not File.Exists(filePath) Then
                Throw New FileNotFoundException($"HMMER3 model file not found: {filePath}")
            End If

            Dim lines As String() = File.ReadAllLines(filePath)
            Return ParseLines(lines)
        End Function

        ''' <summary>
        ''' 解析HMMER3模型文本内容
        ''' </summary>
        ''' <param name="content">HMMER3模型文本内容</param>
        ''' <returns>解析后的ProfileHMM对象</returns>
        Public Shared Function ParseContent(content As String) As ProfileHMM
            Dim lines As String() = content.Split({vbCr, vbLf}, StringSplitOptions.RemoveEmptyEntries)
            Return ParseLines(lines)
        End Function

        ''' <summary>
        ''' 解析HMMER3模型行数据
        ''' </summary>
        Private Shared Function ParseLines(lines As String()) As ProfileHMM
            Dim model As New ProfileHMM()

            Dim i As Integer = 0
            While i < lines.Length
                Dim line As String = lines(i).Trim()

                ' 解析头部信息
                If line.StartsWith("HMMER3") Then
                    model.Version = line
                ElseIf line.StartsWith("NAME") Then
                    model.Name = ParseValue(line)
                ElseIf line.StartsWith("LENG") Then
                    model.Length = Integer.Parse(ParseValue(line))
                ElseIf line.StartsWith("ALPH") Then
                    model.Alphabet = ParseValue(line)
                ElseIf line.StartsWith("NSEQ") Then
                    model.NumSequences = Integer.Parse(ParseValue(line))
                ElseIf line.StartsWith("EFFN") Then
                    model.EffectiveNum = Double.Parse(ParseValue(line))
                ElseIf line.StartsWith("CKSUM") Then
                    model.Checksum = Long.Parse(ParseValue(line))
                ElseIf line.StartsWith("STATS LOCAL MSV") Then
                    model.StatsMSV = ParseStatsLine(line)
                ElseIf line.StartsWith("STATS LOCAL VITERBI") Then
                    model.StatsViterbi = ParseStatsLine(line)
                ElseIf line.StartsWith("STATS LOCAL FORWARD") Then
                    model.StatsForward = ParseStatsLine(line)
                ElseIf line.StartsWith("HMM") Then
                    ' 跳过HMM标题行和列标题行
                    i += 2
                    ' 解析COMPO行（背景分布）
                    If i < lines.Length AndAlso lines(i).Trim().StartsWith("COMPO") Then
                        ParseCompoLine(lines(i), model)
                        i += 1
                        ' 解析COMPO的第二行（插入发射概率）
                        If i < lines.Length Then
                            ParseCompoInsertLine(lines(i), model)
                            i += 1
                        End If
                        ' 解析COMPO的第三行（转移概率）
                        If i < lines.Length Then
                            ParseCompoTransLine(lines(i), model)
                            i += 1
                        End If
                    End If

                    ' 解析每个匹配状态的发射概率和转移概率
                    While i < lines.Length
                        Dim currentLine As String = lines(i).Trim()
                        If String.IsNullOrEmpty(currentLine) OrElse currentLine.StartsWith("//") Then
                            Exit While
                        End If

                        ' 检查是否是状态行（以数字开头）
                        Dim match As Match = Regex.Match(currentLine, "^\s*(\d+)")
                        If match.Success Then
                            Dim stateIndex As Integer = Integer.Parse(match.Groups(1).Value)
                            ParseStateBlock(lines, i, model, stateIndex)
                            i += 3 ' 每个状态块有3行
                        Else
                            i += 1
                        End If
                    End While
                End If

                i += 1
            End While

            ' 初始化模型参数
            model.InitializeHMMParameters()

            Return model
        End Function

        ''' <summary>
        ''' 解析键值对行
        ''' </summary>
        Private Shared Function ParseValue(line As String) As String
            Dim parts As String() = line.Split({" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)
            If parts.Length >= 2 Then
                Return parts(1)
            End If
            Return ""
        End Function

        ''' <summary>
        ''' 解析统计信息行
        ''' </summary>
        Private Shared Function ParseStatsLine(line As String) As (mu As Double, lambda As Double)
            Dim parts As String() = line.Split({" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)
            If parts.Length >= 4 Then
                Return (Double.Parse(parts(3)), Double.Parse(parts(4)))
            End If
            Return (0.0, 0.0)
        End Function

        ''' <summary>
        ''' 解析COMPO行（背景发射概率）
        ''' </summary>
        Private Shared Sub ParseCompoLine(line As String, model As ProfileHMM)
            Dim parts As String() = line.Split({" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)
            ' 跳过"COMPO"标识符
            Dim startIndex As Integer = 1
            Dim probs As New List(Of Double)
            For i As Integer = startIndex To Math.Min(startIndex + 19, parts.Length - 1)
                probs.Add(Double.Parse(parts(i)))
            Next
            model.CompositionEmission = probs.ToArray()
        End Sub

        ''' <summary>
        ''' 解析COMPO插入发射概率行
        ''' </summary>
        Private Shared Sub ParseCompoInsertLine(line As String, model As ProfileHMM)
            Dim parts As String() = line.Split({" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)
            Dim probs As New List(Of Double)
            For i As Integer = 0 To Math.Min(19, parts.Length - 1)
                probs.Add(Double.Parse(parts(i)))
            Next
            model.CompositionInsert = probs.ToArray()
        End Sub

        ''' <summary>
        ''' 解析COMPO转移概率行
        ''' </summary>
        Private Shared Sub ParseCompoTransLine(line As String, model As ProfileHMM)
            Dim parts As String() = line.Split({" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)
            Dim probs As New List(Of Double)
            For i As Integer = 0 To Math.Min(6, parts.Length - 1)
                Dim val As Double
                If Double.TryParse(parts(i), val) Then
                    probs.Add(val)
                ElseIf parts(i) = "*" Then
                    probs.Add(Double.NegativeInfinity)
                Else
                    probs.Add(0.0)
                End If
            Next
            model.CompositionTransitions = probs.ToArray()
        End Sub

        ''' <summary>
        ''' 解析状态块（发射概率和转移概率）
        ''' </summary>
        Private Shared Sub ParseStateBlock(lines As String(), startIndex As Integer, model As ProfileHMM, stateIndex As Integer)
            ' 第一行：匹配状态发射概率
            Dim matchLine As String = lines(startIndex).Trim()
            Dim matchParts As String() = matchLine.Split({" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)

            Dim matchEmission As New List(Of Double)
            ' 格式: 状态号 20个发射概率 状态号 残基 rf mm cs map
            ' 发射概率从索引1开始，共20个
            For i As Integer = 1 To Math.Min(20, matchParts.Length - 1)
                matchEmission.Add(Double.Parse(matchParts(i)))
            Next

            ' 第二行：插入状态发射概率
            Dim insertLine As String = lines(startIndex + 1).Trim()
            Dim insertParts As String() = insertLine.Split({" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)
            Dim insertEmission As New List(Of Double)
            For i As Integer = 0 To Math.Min(19, insertParts.Length - 1)
                insertEmission.Add(Double.Parse(insertParts(i)))
            Next

            ' 第三行：转移概率 (m->m, m->i, m->d, i->m, i->i, d->m, d->d)
            Dim transLine As String = lines(startIndex + 2).Trim()
            Dim transParts As String() = transLine.Split({" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)
            Dim transitions As New List(Of Double)
            For i As Integer = 0 To Math.Min(6, transParts.Length - 1)
                Dim val As Double
                If Double.TryParse(transParts(i), val) Then
                    transitions.Add(val)
                ElseIf transParts(i) = "*" Then
                    transitions.Add(Double.NegativeInfinity)
                Else
                    transitions.Add(0.0)
                End If
            Next

            ' 添加到模型
            model.MatchEmissions.Add(matchEmission.ToArray())
            model.InsertEmissions.Add(insertEmission.ToArray())
            model.Transitions.Add(transitions.ToArray())
        End Sub

    End Class

#End Region

#Region "Profile HMM模型类"

    ''' <summary>
    ''' Profile HMM模型类
    ''' 表示HMMER3格式的隐马尔可夫模型
    ''' </summary>
    ''' <remarks>
    ''' Profile HMM是一种特殊的隐马尔可夫模型，专门用于蛋白质序列比对。
    ''' 它包含三种状态：
    ''' - Match (M): 匹配状态，对应模型中的一个保守位置
    ''' - Insert (I): 插入状态，允许在模型位置之间插入残基
    ''' - Delete (D): 删除状态，允许跳过模型中的某个位置
    ''' 
    ''' 每个位置有：
    ''' - 20个氨基酸的匹配发射概率
    ''' - 20个氨基酸的插入发射概率
    ''' - 7个转移概率
    ''' </remarks>
    Public Class ProfileHMM

        ' 模型元数据
        Public Property Version As String
        Public Property Name As String
        Public Property Length As Integer
        Public Property Alphabet As String
        Public Property NumSequences As Integer
        Public Property EffectiveNum As Double
        Public Property Checksum As Long
        Public Property StatsMSV As (mu As Double, lambda As Double)
        Public Property StatsViterbi As (mu As Double, lambda As Double)
        Public Property StatsForward As (mu As Double, lambda As Double)

        ' 背景分布
        Public Property CompositionEmission As Double()
        Public Property CompositionInsert As Double()
        Public Property CompositionTransitions As Double()

        ' 模型参数（对数几率比，单位：bits）
        Public Property MatchEmissions As New List(Of Double())
        Public Property InsertEmissions As New List(Of Double())
        Public Property Transitions As New List(Of Double())

        ' 转换后的HMM参数（概率形式）
        Public Property HMMStates As StatesObject()
        Public Property HMMObservables As Observable()
        Public Property HMMInitialProb As Double()

        ' 氨基酸字母表
        Public Shared ReadOnly AA_ALPHABET As String() = {
            "A", "C", "D", "E", "F", "G", "H", "I", "K", "L",
            "M", "N", "P", "Q", "R", "S", "T", "V", "W", "Y"
        }

        ''' <summary>
        ''' 初始化HMM参数，将HMMER3的对数几率比转换为概率
        ''' </summary>
        Public Sub InitializeHMMParameters()
            ' 将对数几率比转换为概率
            ' HMMER3使用的是以2为底的对数几率比（单位：bits）
            ' 需要转换回概率形式以适配现有HMM框架

            Dim numStates As Integer = MatchEmissions.Count

            If numStates = 0 Then Return

            ' 创建状态对象
            ' 每个匹配位置对应一个状态
            ' 状态命名：M1, M2, ..., Mn
            ReDim HMMStates(numStates - 1)
            ReDim HMMInitialProb(numStates - 1)

            ' 初始概率：均匀分布或基于第一个状态的发射概率
            Dim initProb As Double = 1.0 / numStates
            For i As Integer = 0 To numStates - 1
                HMMInitialProb(i) = initProb
            Next

            ' 创建转移矩阵
            ' 简化处理：使用线性转移（每个状态转移到下一个状态）
            For i As Integer = 0 To numStates - 1
                Dim transProbs As Double() = New Double(numStates - 1) {}

                ' 主要转移到下一个状态
                If i < numStates - 1 Then
                    transProbs(i + 1) = 0.8 ' 主要转移概率
                    transProbs(i) = 0.1 ' 自环概率
                    ' 剩余概率分配给其他状态
                    Dim remaining As Double = 0.1
                    For j As Integer = 0 To numStates - 1
                        If j <> i AndAlso j <> i + 1 Then
                            transProbs(j) = remaining / (numStates - 2)
                        End If
                    Next
                Else
                    ' 最后一个状态
                    transProbs(i) = 0.9
                    Dim remaining As Double = 0.1
                    For j As Integer = 0 To numStates - 2
                        transProbs(j) = remaining / (numStates - 1)
                    Next
                End If

                HMMStates(i) = New StatesObject With {
                    .state = $"M{i + 1}",
                    .prob = transProbs
                }
            Next

            ' 创建观测对象（氨基酸）
            ReDim HMMObservables(AA_ALPHABET.Length - 1)

            ' 计算平均发射概率作为每个氨基酸的发射概率
            For aaIdx As Integer = 0 To AA_ALPHABET.Length - 1
                Dim emissionProbs As Double() = New Double(numStates - 1) {}

                ' 对每个状态，从对数几率比转换为概率
                For stateIdx As Integer = 0 To numStates - 1
                    If stateIdx < MatchEmissions.Count AndAlso aaIdx < MatchEmissions(stateIdx).Length Then
                        ' 将对数几率比转换为概率
                        ' log_odds = log2(p / q)，其中q是背景概率
                        ' p = q * 2^log_odds
                        Dim logOdds As Double = MatchEmissions(stateIdx)(aaIdx)
                        ' 使用softmax风格的转换
                        emissionProbs(stateIdx) = LogOddsToProbability(logOdds)
                    Else
                        emissionProbs(stateIdx) = 1.0 / AA_ALPHABET.Length
                    End If
                Next

                ' 归一化
                Dim sum As Double = emissionProbs.Sum()
                If sum > 0 Then
                    For j As Integer = 0 To emissionProbs.Length - 1
                        emissionProbs(j) /= sum
                    Next
                End If

                HMMObservables(aaIdx) = New Observable With {
                    .obs = AA_ALPHABET(aaIdx),
                    .prob = emissionProbs
                }
            Next
        End Sub

        ''' <summary>
        ''' 将对数几率比转换为概率
        ''' </summary>
        Private Function LogOddsToProbability(logOdds As Double) As Double
            ' HMMER3使用对数几率比，需要转换
            ' 使用softmax风格的转换确保概率为正
            ' 对于负值较大的对数几率比，概率接近0
            ' 对于正值较大的对数几率比，概率接近1

            If Double.IsNegativeInfinity(logOdds) Then
                Return 0.0
            ElseIf Double.IsPositiveInfinity(logOdds) Then
                Return 1.0
            Else
                ' 使用sigmoid风格的转换
                ' 将对数几率比映射到[0,1]区间
                Return 1.0 / (1.0 + Math.Exp(-logOdds * Math.Log(2)))
            End If
        End Function

        ''' <summary>
        ''' 创建标准HMM对象
        ''' </summary>
        ''' <returns>HMM对象</returns>
        Public Function CreateHMM() As HMM
            If HMMStates Is Nothing OrElse HMMObservables Is Nothing Then
                InitializeHMMParameters()
            End If
            Return New HMM(HMMStates, HMMObservables, HMMInitialProb)
        End Function

        ''' <summary>
        ''' 计算序列的比特得分
        ''' 使用原始HMMER3对数几率比进行计算
        ''' </summary>
        ''' <param name="sequence">氨基酸序列</param>
        ''' <returns>比特得分</returns>
        Public Function CalculateBitScore(sequence As String) As Double
            Dim bitScore As Double = 0.0
            Dim seqLength As Integer = sequence.Length
            Dim modelLength As Integer = MatchEmissions.Count

            If seqLength = 0 OrElse modelLength = 0 Then
                Return 0.0
            End If

            ' 使用简化的Viterbi风格得分计算
            Dim dp As Double() = New Double(modelLength) {}
            Dim prevDp As Double() = New Double(modelLength) {}

            ' 初始化
            For i As Integer = 0 To modelLength
                dp(i) = 0.0
                prevDp(i) = 0.0
            Next

            ' 动态规划
            For pos As Integer = 0 To seqLength - 1
                Dim aa As Char = Char.ToUpper(sequence(pos))
                Dim aaIdx As Integer = GetAminoAcidIndex(aa)

                If aaIdx >= 0 Then
                    For stateIdx As Integer = 1 To modelLength
                        Dim emissionScore As Double = 0.0
                        If stateIdx - 1 < MatchEmissions.Count AndAlso aaIdx < MatchEmissions(stateIdx - 1).Length Then
                            emissionScore = MatchEmissions(stateIdx - 1)(aaIdx)
                        End If

                        ' 转移得分（简化：使用线性转移）
                        Dim transScore As Double = 0.0
                        If stateIdx - 1 < Transitions.Count AndAlso Transitions(stateIdx - 1).Length > 0 Then
                            transScore = Transitions(stateIdx - 1)(0) ' m->m
                        End If

                        dp(stateIdx) = prevDp(stateIdx - 1) + emissionScore + transScore
                    Next

                    ' 复制dp到prevDp
                    Array.Copy(dp, prevDp, modelLength + 1)
                End If
            Next

            ' 返回最大得分
            Return dp.Max()
        End Function

        ''' <summary>
        ''' 获取氨基酸在字母表中的索引
        ''' </summary>
        Private Function GetAminoAcidIndex(aa As Char) As Integer
            For i As Integer = 0 To AA_ALPHABET.Length - 1
                If AA_ALPHABET(i)(0) = aa Then
                    Return i
                End If
            Next
            Return -1 ' 未知氨基酸
        End Function

        ''' <summary>
        ''' 计算E值（期望值）
        ''' </summary>
        ''' <param name="bitScore">比特得分</param>
        ''' <param name="databaseSize">数据库大小（序列数）</param>
        ''' <returns>E值</returns>
        Public Function CalculateEValue(bitScore As Double, databaseSize As Integer) As Double
            ' E = K * N * exp(-lambda * S)
            ' 使用模型中存储的统计参数
            Dim lambda As Double = StatsForward.lambda
            Dim mu As Double = StatsForward.mu

            If lambda = 0 Then lambda = 0.69886 ' 默认值

            Dim eValue As Double = databaseSize * Math.Exp(-lambda * bitScore + mu)
            Return eValue
        End Function

    End Class

#End Region

#Region "FASTA解析器和蛋白质序列类"

    ''' <summary>
    ''' FASTA格式序列解析器
    ''' </summary>
    Public Class FastaParser

        ''' <summary>
        ''' 解析FASTA文件
        ''' </summary>
        ''' <param name="filePath">FASTA文件路径</param>
        ''' <returns>蛋白质序列列表</returns>
        Public Shared Function Parse(filePath As String) As List(Of ProteinSequence)
            If Not File.Exists(filePath) Then
                Throw New FileNotFoundException($"FASTA file not found: {filePath}")
            End If

            Dim content As String = File.ReadAllText(filePath)
            Return ParseContent(content)
        End Function

        ''' <summary>
        ''' 解析FASTA格式内容
        ''' </summary>
        ''' <param name="content">FASTA格式文本内容</param>
        ''' <returns>蛋白质序列列表</returns>
        Public Shared Function ParseContent(content As String) As List(Of ProteinSequence)
            Dim sequences As New List(Of ProteinSequence)()
            Dim lines As String() = content.Split({vbCr, vbLf}, StringSplitOptions.RemoveEmptyEntries)

            Dim currentSeq As ProteinSequence = Nothing
            Dim seqBuilder As New StringBuilder()

            For Each line As String In lines
                line = line.Trim()
                If String.IsNullOrEmpty(line) Then Continue For

                If line.StartsWith(">") Then
                    ' 保存前一个序列
                    If currentSeq IsNot Nothing Then
                        currentSeq.Sequence = seqBuilder.ToString()
                        sequences.Add(currentSeq)
                    End If

                    ' 开始新序列
                    currentSeq = New ProteinSequence()
                    ParseHeader(line, currentSeq)
                    seqBuilder.Clear()
                Else
                    ' 序列行
                    If currentSeq IsNot Nothing Then
                        seqBuilder.Append(line)
                    End If
                End If
            Next

            ' 保存最后一个序列
            If currentSeq IsNot Nothing Then
                currentSeq.Sequence = seqBuilder.ToString()
                sequences.Add(currentSeq)
            End If

            Return sequences
        End Function

        ''' <summary>
        ''' 解析FASTA头部
        ''' </summary>
        Private Shared Sub ParseHeader(headerLine As String, seq As ProteinSequence)
            ' 格式: >ID description
            ' 去掉开头的>
            Dim header As String = headerLine.Substring(1).Trim()
            Dim parts As String() = header.Split({" "c, vbTab}, 2)

            If parts.Length >= 1 Then
                seq.ID = parts(0)
                ' 提取可能的基因ID
                If seq.ID.Contains("|") Then
                    Dim idParts As String() = seq.ID.Split("|"c)
                    For Each part As String In idParts
                        If part.StartsWith("gnl|") OrElse part.StartsWith("lcl|") Then
                            seq.GeneID = part.Substring(4)
                            Exit For
                        ElseIf part.StartsWith("sp|") OrElse part.StartsWith("tr|") Then
                            ' UniProt格式
                            Continue For
                        ElseIf Not String.IsNullOrEmpty(part) AndAlso Not part.StartsWith("ref|") Then
                            seq.GeneID = part
                        End If
                    Next
                End If
            End If

            If parts.Length >= 2 Then
                seq.Description = parts(1)
            End If
        End Sub

    End Class

    ''' <summary>
    ''' 蛋白质序列类
    ''' </summary>
    Public Class ProteinSequence

        ''' <summary>
        ''' 序列ID
        ''' </summary>
        Public Property ID As String

        ''' <summary>
        ''' 基因ID
        ''' </summary>
        Public Property GeneID As String

        ''' <summary>
        ''' 序列描述
        ''' </summary>
        Public Property Description As String

        ''' <summary>
        ''' 氨基酸序列
        ''' </summary>
        Public Property Sequence As String

        ''' <summary>
        ''' 序列长度
        ''' </summary>
        Public ReadOnly Property Length As Integer
            Get
                If String.IsNullOrEmpty(Sequence) Then Return 0
                Return Sequence.Length
            End Get
        End Property

        ''' <summary>
        ''' 注释结果
        ''' </summary>
        Public Property Annotation As AnnotationResult

        Public Overrides Function ToString() As String
            Return $"[{ID}] {Description} (Length: {Length})"
        End Function

    End Class

#End Region

#Region "注释结果类"

    ''' <summary>
    ''' 注释结果类
    ''' </summary>
    Public Class AnnotationResult

        ''' <summary>
        ''' 匹配的模型名称
        ''' </summary>
        Public Property ModelName As String

        ''' <summary>
        ''' 比特得分
        ''' </summary>
        Public Property BitScore As Double

        ''' <summary>
        ''' E值
        ''' </summary>
        Public Property EValue As Double

        ''' <summary>
        ''' 是否通过阈值
        ''' </summary>
        Public Property IsSignificant As Boolean

        ''' <summary>
        ''' 匹配的起始位置
        ''' </summary>
        Public Property AlignmentStart As Integer

        ''' <summary>
        ''' 匹配的结束位置
        ''' </summary>
        Public Property AlignmentEnd As Integer

        ''' <summary>
        ''' 匹配区域序列
        ''' </summary>
        Public Property AlignedSequence As String

        ''' <summary>
        ''' 置信度
        ''' </summary>
        Public Property Confidence As Double

        ''' <summary>
        ''' 功能注释
        ''' </summary>
        Public Property FunctionalAnnotation As String

        Public Overrides Function ToString() As String
            Return $"Model: {ModelName}, Score: {BitScore:F2}, E-value: {EValue:G2}, Significant: {IsSignificant}"
        End Function

    End Class

#End Region

#Region "蛋白质序列分类注释器"

    ''' <summary>
    ''' 蛋白质序列分类注释器
    ''' 使用Profile HMM对蛋白质序列进行功能注释
    ''' </summary>
    Public Class ProteinAnnotator

        ' 已加载的HMM模型字典
        Private _models As New Dictionary(Of String, ProfileHMM)()

        ' E值阈值
        Private _eValueThreshold As Double = 0.01

        ' 比特得分阈值
        Private _bitScoreThreshold As Double = 25.0

        ' 数据库大小（用于E值计算）
        Private _databaseSize As Integer = 10000

        ''' <summary>
        ''' 获取或设置E值阈值
        ''' </summary>
        Public Property EValueThreshold As Double
            Get
                Return _eValueThreshold
            End Get
            Set(value As Double)
                _eValueThreshold = value
            End Set
        End Property

        ''' <summary>
        ''' 获取或设置比特得分阈值
        ''' </summary>
        Public Property BitScoreThreshold As Double
            Get
                Return _bitScoreThreshold
            End Get
            Set(value As Double)
                _bitScoreThreshold = value
            End Set
        End Property

        ''' <summary>
        ''' 获取或设置数据库大小
        ''' </summary>
        Public Property DatabaseSize As Integer
            Get
                Return _databaseSize
            End Get
            Set(value As Integer)
                _databaseSize = value
            End Set
        End Property

        ''' <summary>
        ''' 加载单个HMMER3模型文件
        ''' </summary>
        ''' <param name="filePath">模型文件路径</param>
        Public Sub LoadModel(filePath As String)
            Dim model As ProfileHMM = HMMER3Parser.Parse(filePath)
            If model IsNot Nothing AndAlso Not String.IsNullOrEmpty(model.Name) Then
                _models(model.Name) = model
            End If
        End Sub

        ''' <summary>
        ''' 从目录加载所有HMMER3模型文件
        ''' </summary>
        ''' <param name="directoryPath">目录路径</param>
        ''' <param name="searchPattern">文件搜索模式（默认*.hmm）</param>
        Public Sub LoadModelsFromDirectory(directoryPath As String, Optional searchPattern As String = "*.hmm")
            If Not Directory.Exists(directoryPath) Then
                Throw New DirectoryNotFoundException($"Directory not found: {directoryPath}")
            End If

            Dim files As String() = Directory.GetFiles(directoryPath, searchPattern)
            For Each file As String In files
                Try
                    LoadModel(file)
                Catch ex As Exception
                    ' 记录错误但继续加载其他模型
                    Console.WriteLine($"Error loading model {file}: {ex.Message}")
                End Try
            Next
        End Sub

        ''' <summary>
        ''' 加载模型内容字符串
        ''' </summary>
        ''' <param name="modelContent">模型文本内容</param>
        Public Sub LoadModelContent(modelContent As String)
            Dim model As ProfileHMM = HMMER3Parser.ParseContent(modelContent)
            If model IsNot Nothing AndAlso Not String.IsNullOrEmpty(model.Name) Then
                _models(model.Name) = model
            End If
        End Sub

        ''' <summary>
        ''' 获取已加载的模型数量
        ''' </summary>
        Public ReadOnly Property ModelCount As Integer
            Get
                Return _models.Count
            End Get
        End Property

        ''' <summary>
        ''' 获取所有已加载的模型名称
        ''' </summary>
        Public ReadOnly Property ModelNames As IEnumerable(Of String)
            Get
                Return _models.Keys
            End Get
        End Property

        ''' <summary>
        ''' 对单个蛋白质序列进行注释
        ''' </summary>
        ''' <param name="protein">蛋白质序列</param>
        ''' <returns>最佳注释结果</returns>
        Public Function Annotate(protein As ProteinSequence) As AnnotationResult
            If protein Is Nothing OrElse String.IsNullOrEmpty(protein.Sequence) Then
                Return Nothing
            End If

            Dim bestResult As AnnotationResult = Nothing
            Dim bestScore As Double = Double.NegativeInfinity

            For Each kvp As KeyValuePair(Of String, ProfileHMM) In _models
                Dim model As ProfileHMM = kvp.Value
                Dim result As AnnotationResult = CompareSequence(protein.Sequence, model)

                If result IsNot Nothing AndAlso result.BitScore > bestScore Then
                    bestScore = result.BitScore
                    bestResult = result
                End If
            Next

            ' 判断是否显著
            If bestResult IsNot Nothing Then
                bestResult.IsSignificant = (bestResult.EValue <= _eValueThreshold AndAlso
                                           bestResult.BitScore >= _bitScoreThreshold)
                bestResult.Confidence = CalculateConfidence(bestResult.BitScore, bestResult.EValue)
            End If

            protein.Annotation = bestResult
            Return bestResult
        End Function

        ''' <summary>
        ''' 对蛋白质序列列表进行批量注释
        ''' </summary>
        ''' <param name="proteins">蛋白质序列列表</param>
        Public Sub AnnotateAll(proteins As IEnumerable(Of ProteinSequence))
            For Each protein As ProteinSequence In proteins
                Annotate(protein)
            Next
        End Sub

        ''' <summary>
        ''' 使用指定模型对序列进行比对
        ''' </summary>
        ''' <param name="sequence">氨基酸序列</param>
        ''' <param name="model">Profile HMM模型</param>
        ''' <returns>注释结果</returns>
        Private Function CompareSequence(sequence As String, model As ProfileHMM) As AnnotationResult
            Dim result As New AnnotationResult()
            result.ModelName = model.Name

            ' 计算比特得分
            result.BitScore = CalculateViterbiScore(sequence, model)

            ' 计算E值
            result.EValue = model.CalculateEValue(result.BitScore, _databaseSize)

            ' 确定比对区域（简化处理）
            result.AlignmentStart = 1
            result.AlignmentEnd = Math.Min(sequence.Length, model.Length)
            result.AlignedSequence = sequence

            Return result
        End Function

        ''' <summary>
        ''' 使用Viterbi算法计算序列得分
        ''' </summary>
        ''' <param name="sequence">氨基酸序列</param>
        ''' <param name="model">Profile HMM模型</param>
        ''' <returns>Viterbi得分</returns>
        Private Function CalculateViterbiScore(sequence As String, model As ProfileHMM) As Double
            Dim seqLength As Integer = sequence.Length
            Dim modelLength As Integer = model.MatchEmissions.Count

            If seqLength = 0 OrElse modelLength = 0 Then
                Return 0.0
            End If

            ' 使用简化的Viterbi算法
            ' 状态：M(匹配), I(插入), D(删除)
            ' 使用对数空间计算以避免下溢

            Dim NEG_INF As Double = Double.NegativeInfinity

            ' 初始化DP矩阵
            Dim M As Double() = New Double(modelLength) {}  ' 匹配状态
            Dim I As Double() = New Double(modelLength) {}  ' 插入状态
            Dim D As Double() = New Double(modelLength) {}  ' 删除状态

            ' 初始化第一列
            For i As Integer = 0 To modelLength
                M(i) = NEG_INF
                i(i) = NEG_INF
                D(i) = NEG_INF
            Next

            ' 初始状态
            M(0) = 0.0

            ' 处理第一个残基
            Dim firstAA As Char = Char.ToUpper(sequence(0))
            Dim firstAAIdx As Integer = GetAminoAcidIndex(firstAA)

            If firstAAIdx >= 0 AndAlso model.MatchEmissions.Count > 0 Then
                Dim emissionScore As Double = GetEmissionScore(model, 0, firstAAIdx)
                M(1) = emissionScore
            End If

            ' 动态规划
            For pos As Integer = 1 To seqLength - 1
                Dim aa As Char = Char.ToUpper(sequence(pos))
                Dim aaIdx As Integer = GetAminoAcidIndex(aa)

                Dim newM As Double() = New Double(modelLength) {}
                Dim newI As Double() = New Double(modelLength) {}
                Dim newD As Double() = New Double(modelLength) {}

                For i As Integer = 0 To modelLength
                    newM(i) = NEG_INF
                    newI(i) = NEG_INF
                    newD(i) = NEG_INF
                Next

                If aaIdx >= 0 Then
                    For k As Integer = 1 To modelLength
                        ' 计算匹配状态得分
                        Dim emissionScore As Double = GetEmissionScore(model, k - 1, aaIdx)

                        ' 从M(k-1)转移到M(k)
                        Dim transMM As Double = GetTransitionScore(model, k - 1, 0)
                        Dim scoreFromM As Double = M(k - 1) + transMM + emissionScore

                        ' 从I(k-1)转移到M(k)
                        Dim transIM As Double = GetTransitionScore(model, k - 1, 3)
                        Dim scoreFromI As Double = I(k - 1) + transIM + emissionScore

                        ' 从D(k-1)转移到M(k)
                        Dim transDM As Double = GetTransitionScore(model, k - 1, 5)
                        Dim scoreFromD As Double = D(k - 1) + transDM + emissionScore

                        newM(k) = Math.Max(Math.Max(scoreFromM, scoreFromI), scoreFromD)

                        ' 计算插入状态得分
                        Dim transMI As Double = GetTransitionScore(model, k - 1, 1)
                        Dim insertScore As Double = GetInsertEmissionScore(model, k - 1, aaIdx)
                        newI(k) = M(k) + transMI + insertScore

                        ' 计算删除状态得分
                        Dim transMD As Double = GetTransitionScore(model, k - 1, 2)
                        Dim transDD As Double = GetTransitionScore(model, k - 1, 6)
                        newD(k) = Math.Max(M(k - 1) + transMD, D(k - 1) + transDD)
                    Next
                End If

                ' 更新状态
                Array.Copy(newM, M, modelLength + 1)
                Array.Copy(newI, I, modelLength + 1)
                Array.Copy(newD, D, modelLength + 1)
            Next

            ' 返回最终得分
            Dim finalScore As Double = NEG_INF
            For k As Integer = 1 To modelLength
                finalScore = Math.Max(finalScore, M(k))
            Next

            If Double.IsNegativeInfinity(finalScore) Then
                Return 0.0
            End If

            Return finalScore
        End Function

        ''' <summary>
        ''' 获取氨基酸在字母表中的索引
        ''' </summary>
        Private Function GetAminoAcidIndex(aa As Char) As Integer
            Dim alphabet As String() = ProfileHMM.AA_ALPHABET
            For i As Integer = 0 To alphabet.Length - 1
                If alphabet(i)(0) = aa Then
                    Return i
                End If
            Next
            Return -1 ' 未知氨基酸
        End Function

        ''' <summary>
        ''' 获取发射得分
        ''' </summary>
        Private Function GetEmissionScore(model As ProfileHMM, stateIdx As Integer, aaIdx As Integer) As Double
            If stateIdx >= 0 AndAlso stateIdx < model.MatchEmissions.Count Then
                Dim emissions As Double() = model.MatchEmissions(stateIdx)
                If aaIdx >= 0 AndAlso aaIdx < emissions.Length Then
                    Return emissions(aaIdx)
                End If
            End If
            Return 0.0
        End Function

        ''' <summary>
        ''' 获取插入发射得分
        ''' </summary>
        Private Function GetInsertEmissionScore(model As ProfileHMM, stateIdx As Integer, aaIdx As Integer) As Double
            If stateIdx >= 0 AndAlso stateIdx < model.InsertEmissions.Count Then
                Dim emissions As Double() = model.InsertEmissions(stateIdx)
                If aaIdx >= 0 AndAlso aaIdx < emissions.Length Then
                    Return emissions(aaIdx)
                End If
            End If
            Return 0.0
        End Function

        ''' <summary>
        ''' 获取转移得分
        ''' </summary>
        Private Function GetTransitionScore(model As ProfileHMM, stateIdx As Integer, transIdx As Integer) As Double
            If stateIdx >= 0 AndAlso stateIdx < model.Transitions.Count Then
                Dim transitions As Double() = model.Transitions(stateIdx)
                If transIdx >= 0 AndAlso transIdx < transitions.Length Then
                    Return transitions(transIdx)
                End If
            End If
            Return 0.0
        End Function

        ''' <summary>
        ''' 计算置信度
        ''' </summary>
        Private Function CalculateConfidence(bitScore As Double, eValue As Double) As Double
            ' 基于比特得分和E值计算置信度
            ' 使用sigmoid函数将得分映射到[0,1]区间
            Dim x As Double = bitScore / 100.0 - Math.Log10(Math.Max(eValue, 1.0E-300))
            Return 1.0 / (1.0 + Math.Exp(-x))
        End Function

    End Class

#End Region

#Region "注释结果输出器"

    ''' <summary>
    ''' 注释结果输出器
    ''' </summary>
    Public Class AnnotationOutput

        ''' <summary>
        ''' 将注释结果输出为制表符分隔格式（类似HMMER输出）
        ''' </summary>
        ''' <param name="proteins">已注释的蛋白质序列列表</param>
        ''' <returns>格式化的输出字符串</returns>
        Public Shared Function ToTsv(proteins As IEnumerable(Of ProteinSequence)) As String
            Dim sb As New StringBuilder()

            ' 表头
            sb.AppendLine("target name" & vbTab & "query name" & vbTab & "accession" & vbTab &
                          "e-value" & vbTab & "score" & vbTab & "bias" & vbTab &
                          "best domain e-value" & vbTab & "best domain score" & vbTab & "best domain bias" & vbTab &
                          "exp" & vbTab & "reg" & vbTab & "clu" & vbTab &
                          "ov" & vbTab & "env" & vbTab & "dom" & vbTab & "rep" & vbTab & "inc" & vbTab &
                          "description")

            ' 数据行
            For Each protein As ProteinSequence In proteins
                If protein.Annotation IsNot Nothing Then
                    Dim ann As AnnotationResult = protein.Annotation
                    sb.AppendLine(String.Join(vbTab, {
                        protein.ID,
                        ann.ModelName,
                        "-",
                        ann.EValue.ToString("G3"),
                        ann.BitScore.ToString("F1"),
                        "0.0",
                        ann.EValue.ToString("G3"),
                        ann.BitScore.ToString("F1"),
                        "0.0",
                        "1",
                        "1",
                        "0",
                        "0",
                        "1",
                        "1",
                        "1",
                        "1",
                        protein.Description
                    }))
                Else
                    sb.AppendLine(String.Join(vbTab, {
                        protein.ID,
                        "-",
                        "-",
                        "-",
                        "-",
                        "-",
                        "-",
                        "-",
                        "-",
                        "-",
                        "-",
                        "-",
                        "-",
                        "-",
                        "-",
                        "-",
                        "-",
                        protein.Description
                    }))
                End If
            Next

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' 将注释结果输出为CSV格式
        ''' </summary>
        ''' <param name="proteins">已注释的蛋白质序列列表</param>
        ''' <returns>CSV格式字符串</returns>
        Public Shared Function ToCsv(proteins As IEnumerable(Of ProteinSequence)) As String
            Dim sb As New StringBuilder()

            ' 表头
            sb.AppendLine("ID,GeneID,Description,Length,ModelName,BitScore,EValue,IsSignificant,Confidence")

            ' 数据行
            For Each protein As ProteinSequence In proteins
                Dim fields As New List(Of String) From {
                    EscapeCsvField(protein.ID),
                    EscapeCsvField(protein.GeneID),
                    EscapeCsvField(protein.Description),
                    protein.Length.ToString()
                }

                If protein.Annotation IsNot Nothing Then
                    fields.Add(EscapeCsvField(protein.Annotation.ModelName))
                    fields.Add(protein.Annotation.BitScore.ToString("F2"))
                    fields.Add(protein.Annotation.EValue.ToString("G3"))
                    fields.Add(protein.Annotation.IsSignificant.ToString())
                    fields.Add(protein.Annotation.Confidence.ToString("F3"))
                Else
                    fields.AddRange({"", "", "", "", ""})
                End If

                sb.AppendLine(String.Join(",", fields))
            Next

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' 转义CSV字段
        ''' </summary>
        Private Shared Function EscapeCsvField(field As String) As String
            If String.IsNullOrEmpty(field) Then
                Return ""
            End If

            If field.Contains(",") OrElse field.Contains("""") OrElse field.Contains(vbCr) OrElse field.Contains(vbLf) Then
                Return """" & field.Replace("""", """""") & """"
            End If

            Return field
        End Function

        ''' <summary>
        ''' 将注释结果输出为JSON格式
        ''' </summary>
        ''' <param name="proteins">已注释的蛋白质序列列表</param>
        ''' <returns>JSON格式字符串</returns>
        Public Shared Function ToJson(proteins As IEnumerable(Of ProteinSequence)) As String
            Dim sb As New StringBuilder()
            sb.AppendLine("[")

            Dim proteinList As List(Of ProteinSequence) = proteins.ToList()
            For i As Integer = 0 To proteinList.Count - 1
                Dim protein As ProteinSequence = proteinList(i)
                sb.AppendLine("  {")
                sb.AppendLine($"    ""id"": ""{EscapeJsonString(protein.ID)}"",")
                sb.AppendLine($"    ""geneId"": ""{EscapeJsonString(protein.GeneID)}"",")
                sb.AppendLine($"    ""description"": ""{EscapeJsonString(protein.Description)}"",")
                sb.AppendLine($"    ""length"": {protein.Length},")

                If protein.Annotation IsNot Nothing Then
                    sb.AppendLine("    ""annotation"": {")
                    sb.AppendLine($"      ""modelName"": ""{EscapeJsonString(protein.Annotation.ModelName)}"",")
                    sb.AppendLine($"      ""bitScore"": {protein.Annotation.BitScore},")
                    sb.AppendLine($"      ""eValue"": {protein.Annotation.EValue},")
                    sb.AppendLine($"      ""isSignificant"": {protein.Annotation.IsSignificant.ToString().ToLower()},")
                    sb.AppendLine($"      ""confidence"": {protein.Annotation.Confidence}")
                    sb.AppendLine("    }")
                Else
                    sb.AppendLine("    ""annotation"": null")
                End If

                If i < proteinList.Count - 1 Then
                    sb.AppendLine("  },")
                Else
                    sb.AppendLine("  }")
                End If
            Next

            sb.AppendLine("]")
            Return sb.ToString()
        End Function

        ''' <summary>
        ''' 转义JSON字符串
        ''' </summary>
        Private Shared Function EscapeJsonString(s As String) As String
            If String.IsNullOrEmpty(s) Then
                Return ""
            End If

            Return s.Replace("\", "\\") _
                    .Replace("""", "\""") _
                    .Replace(vbCr, "\r") _
                    .Replace(vbLf, "\n") _
                    .Replace(vbTab, "\t")
        End Function

    End Class

#End Region

End Namespace
