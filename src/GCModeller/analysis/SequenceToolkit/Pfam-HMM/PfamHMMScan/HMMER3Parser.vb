' ============================================================================
' HMMER3模型解析器与蛋白质序列分类注释模块
' 
' 基于现有HMM算法框架，实现HMMER3格式模型文件的读取和蛋白质序列分类注释功能
' 
' Author: 基于用户现有HMM代码框架扩展
' Copyright (c) 2024 GPL3 Licensed
' ============================================================================

Imports System.IO
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.DataMining.HiddenMarkovChain.Models
Imports Microsoft.VisualBasic.My.JavaScript

Namespace HMMER3

    ''' <summary>
    ''' HMMER3模型文件解析器
    ''' 用于读取.hmm格式的HMMER3模型文件
    ''' </summary>
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

    ''' <summary>
    ''' Profile HMM模型类
    ''' 表示HMMER3格式的隐马尔可夫模型
    ''' </summary>
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

        ' 模型参数（对数几率比）
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

            ' 使用Viterbi风格的局部比对得分计算
            ' 简化版本：直接累加匹配状态的发射得分

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

End Namespace
