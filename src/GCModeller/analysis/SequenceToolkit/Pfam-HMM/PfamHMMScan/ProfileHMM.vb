Imports System.TimeZoneInfo
Imports Microsoft.VisualBasic.DataMining.HiddenMarkovChain
Imports Microsoft.VisualBasic.DataMining.HiddenMarkovChain.Models

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
    ''' 计算序列的比特得分（使用Viterbi算法）
    ''' </summary>
    Public Function CalculateBitScore(sequence As String) As BitScoreResult
        If String.IsNullOrEmpty(sequence) OrElse MatchEmissions.Count = 0 Then
            Return New BitScoreResult() With {.Score = 0.0}
        End If

        Dim seqLength As Integer = sequence.Length
        Dim modelLength As Integer = MatchEmissions.Count

        ' 动态规划表
        ' M(k, i): 在模型位置k，序列位置i，处于匹配状态的最大得分
        ' I(k, i): 在模型位置k，序列位置i，处于插入状态的最大得分
        ' D(k, i): 在模型位置k，序列位置i，处于删除状态的最大得分
        Dim M(modelLength + 1, seqLength + 1) As Double
        Dim I(modelLength + 1, seqLength + 1) As Double
        Dim D(modelLength + 1, seqLength + 1) As Double

        ' 回溯指针
        Dim traceM(modelLength + 1, seqLength + 1) As TracePointer
        Dim traceI(modelLength + 1, seqLength + 1) As TracePointer
        Dim traceD(modelLength + 1, seqLength + 1) As TracePointer

        Const NEG_INF As Double = Double.NegativeInfinity

        ' 初始化：所有状态初始化为负无穷
        For k = 0 To modelLength
            For I = 0 To seqLength
                M(k, I) = NEG_INF
                I(k, I) = NEG_INF
                D(k, I) = NEG_INF
            Next
        Next

        ' 设置开始状态
        M(0, 0) = 0.0

        ' 第一个残基的特殊处理
        If seqLength > 0 Then
            Dim firstAA As Char = Char.ToUpper(sequence(0))
            Dim aaIdx As Integer = GetAminoAcidIndex(firstAA)

            If aaIdx >= 0 Then
                ' 可以从M(0,0)开始匹配第一个残基
                Dim emissionScore = GetEmissionScore(1, aaIdx)
                Dim transScore = GetTransitionScore(0, TransitionType.M_TO_M)
                M(1, 1) = emissionScore + transScore
                traceM(1, 1) = New TracePointer(TraceState.MATCH, 0, 0)
            End If
        End If

        ' 动态规划填充
        For I = 1 To seqLength
            Dim aa As Char = Char.ToUpper(sequence(I - 1))
            Dim aaIdx As Integer = GetAminoAcidIndex(aa)

            If aaIdx < 0 Then Continue For

            For k = 1 To modelLength
                ' 计算M(k,i)
                Dim bestM As Double = NEG_INF
                Dim bestTrace As New TracePointer(TraceState.NONE, 0, 0)

                ' 从M(k-1, i-1)转移
                If I > 1 Then
                    Dim fromM = M(k - 1, I - 1) + GetTransitionScore(k - 1, TransitionType.M_TO_M)
                    If fromM > bestM Then
                        bestM = fromM
                        bestTrace = New TracePointer(TraceState.MATCH, k - 1, I - 1)
                    End If
                End If

                ' 从I(k-1, i-1)转移
                If I > 1 Then
                    Dim fromI = I(k - 1, I - 1) + GetTransitionScore(k - 1, TransitionType.I_TO_M)
                    If fromI > bestM Then
                        bestM = fromI
                        bestTrace = New TracePointer(TraceState.INSERT, k - 1, I - 1)
                    End If
                End If

                ' 从D(k-1, i-1)转移
                If I > 1 Then
                    Dim fromD = D(k - 1, I - 1) + GetTransitionScore(k - 1, TransitionType.D_TO_M)
                    If fromD > bestM Then
                        bestM = fromD
                        bestTrace = New TracePointer(TraceState.DELETE, k - 1, I - 1)
                    End If
                End If

                ' 加上发射得分
                Dim emissionScore = GetEmissionScore(k, aaIdx)
                M(k, I) = bestM + emissionScore
                traceM(k, I) = bestTrace

                ' 计算I(k,i)
                Dim bestI As Double = NEG_INF
                Dim bestTraceI As New TracePointer(TraceState.NONE, 0, 0)

                ' 从M(k, i-1)转移
                If I > 1 Then
                    Dim fromM = M(k, I - 1) + GetTransitionScore(k, TransitionType.M_TO_I)
                    If fromM > bestI Then
                        bestI = fromM
                        bestTraceI = New TracePointer(TraceState.MATCH, k, I - 1)
                    End If
                End If

                ' 从I(k, i-1)转移
                If I > 1 Then
                    Dim fromI = I(k, I - 1) + GetTransitionScore(k, TransitionType.I_TO_I)
                    If fromI > bestI Then
                        bestI = fromI
                        bestTraceI = New TracePointer(TraceState.INSERT, k, I - 1)
                    End If
                End If

                ' 加上插入发射得分
                Dim insertEmissionScore = GetInsertEmissionScore(k, aaIdx)
                I(k, I) = bestI + insertEmissionScore
                traceI(k, I) = bestTraceI

                ' 计算D(k,i)
                Dim bestD As Double = NEG_INF
                Dim bestTraceD As New TracePointer(TraceState.NONE, 0, 0)

                ' 从M(k-1, i)转移
                Dim fromM_d = M(k - 1, I) + GetTransitionScore(k - 1, TransitionType.M_TO_D)
                If fromM_d > bestD Then
                    bestD = fromM_d
                    bestTraceD = New TracePointer(TraceState.MATCH, k - 1, I)
                End If

                ' 从D(k-1, i)转移
                Dim fromD_d = D(k - 1, I) + GetTransitionScore(k - 1, TransitionType.D_TO_D)
                If fromD_d > bestD Then
                    bestD = fromD_d
                    bestTraceD = New TracePointer(TraceState.DELETE, k - 1, I)
                End If

                D(k, I) = bestD
                traceD(k, I) = bestTraceD
            Next
        Next

        ' 查找最终得分
        Dim finalScore As Double = NEG_INF
        Dim endK As Integer = 0
        Dim endI As Integer = seqLength
        Dim endState As TraceState = TraceState.NONE

        ' 尝试在所有模型位置结束
        For k = 1 To modelLength
            If M(k, seqLength) > finalScore Then
                finalScore = M(k, seqLength)
                endK = k
                endState = TraceState.MATCH
            End If

            If I(k, seqLength) > finalScore Then
                finalScore = I(k, seqLength)
                endK = k
                endState = TraceState.INSERT
            End If

            If D(k, seqLength) > finalScore Then
                finalScore = D(k, seqLength)
                endK = k
                endState = TraceState.DELETE
            End If
        Next

        ' 回溯路径
        Dim alignmentPath As New List(Of AlignmentPosition)
        Dim currentK = endK
        Dim currentI = endI
        Dim currentState = endState

        While currentK > 0 AndAlso currentI > 0
            Dim pos As New AlignmentPosition With {
                .ModelPosition = currentK,
                .SequencePosition = currentI,
                .State = currentState
            }

            alignmentPath.Add(pos)

            Dim trace As TracePointer = Nothing
            Select Case currentState
                Case TraceState.MATCH
                    trace = traceM(currentK, currentI)
                Case TraceState.INSERT
                    trace = traceI(currentK, currentI)
                Case TraceState.DELETE
                    trace = traceD(currentK, currentI)
            End Select

            If trace.State = TraceState.NONE Then Exit While

            currentK = trace.ModelPos
            currentI = trace.SeqPos
            currentState = trace.State
        End While

        alignmentPath.Reverse()

        Return New BitScoreResult() With {
            .Score = If(Double.IsNegativeInfinity(finalScore), 0.0, finalScore),
            .alignmentPath = alignmentPath
        }
    End Function

    ''' <summary>
    ''' 获取氨基酸在字母表中的索引
    ''' </summary>
    Private Function GetAminoAcidIndex(aa As Char) As Integer
        For i As Integer = 0 To AA_ALPHABET.Length - 1
            If AA_ALPHABET(i)(0) = aa Then Return i
        Next
        Return -1 ' 未知氨基酸
    End Function

    ''' <summary>
    ''' 获取匹配发射得分
    ''' </summary>
    Private Function GetEmissionScore(stateIdx As Integer, aaIdx As Integer) As Double
        If stateIdx >= 1 AndAlso stateIdx <= MatchEmissions.Count AndAlso
           aaIdx >= 0 AndAlso aaIdx < MatchEmissions(stateIdx - 1).Length Then
            Return MatchEmissions(stateIdx - 1)(aaIdx)
        End If
        Return 0.0
    End Function

    ''' <summary>
    ''' 获取插入发射得分
    ''' </summary>
    Private Function GetInsertEmissionScore(stateIdx As Integer, aaIdx As Integer) As Double
        If stateIdx >= 1 AndAlso stateIdx <= InsertEmissions.Count AndAlso
           aaIdx >= 0 AndAlso aaIdx < InsertEmissions(stateIdx - 1).Length Then
            Return InsertEmissions(stateIdx - 1)(aaIdx)
        End If
        Return 0.0
    End Function

    ''' <summary>
    ''' 获取转移得分
    ''' </summary>
    Private Function GetTransitionScore(stateIdx As Integer, transType As TransitionType) As Double
        If stateIdx >= 0 AndAlso stateIdx < Transitions.Count Then
            Dim transIdx As Integer = CInt(transType)
            If transIdx >= 0 AndAlso transIdx < Transitions(stateIdx).Length Then
                Return Transitions(stateIdx)(transIdx)
            End If
        End If
        Return 0.0
    End Function

    ''' <summary>
    ''' 计算E值
    ''' </summary>
    Public Function CalculateEValue(bitScore As Double, databaseSize As Integer) As Double
        Dim lambda As Double = StatsForward.lambda
        Dim mu As Double = StatsForward.mu

        If lambda = 0 Then lambda = 0.69886 ' 默认值

        Dim p = -lambda * bitScore + mu
        Dim eValue As Double = databaseSize * Math.Exp(p)

        Return eValue
    End Function
End Class

