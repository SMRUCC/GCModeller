#Region "Microsoft.VisualBasic::c7e4a28cf63608e778272034f8d095cc, analysis\SequenceToolkit\Pfam-HMM\PfamHMMScan\ProfileHMM.vb"

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

'   Total Lines: 478
'    Code Lines: 288 (60.25%)
' Comment Lines: 124 (25.94%)
'    - Xml Docs: 34.68%
' 
'   Blank Lines: 66 (13.81%)
'     File Size: 18.42 KB


' Class ProfileHMM
' 
'     Properties: Alphabet, Checksum, CompositionEmission, CompositionInsert, CompositionTransitions
'                 EffectiveNum, HMMInitialProb, HMMObservables, HMMStates, InsertEmissions
'                 Length, MatchEmissions, Name, NumSequences, StatsForward
'                 StatsMSV, StatsViterbi, Transitions, Version
' 
'     Function: CalculateBitScore, CalculateEValue, CreateHMM, GetAminoAcidIndex, GetEmissionScore
'               GetInsertEmissionScore, GetTransitionScore, InitializeHMMParameters, LogOddsToProbability
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.DataMining.HiddenMarkovChain
Imports Microsoft.VisualBasic.DataMining.HiddenMarkovChain.Models
Imports Microsoft.VisualBasic.Math.SIMD

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
    Public Property MatchEmissions As Double()()
    Public Property InsertEmissions As Double()()
    Public Property Transitions As Double()()

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
    ''' 氨基酸单字符（大写）到字母表索引的静态查找表，-1 表示未知残基。
    ''' 用于替代 CalculateBitScore 内层的逐字符线性扫描。
    ''' </summary>
    Public Shared ReadOnly AALookup As SByte() = CreateAALookup()

    Private Shared Function CreateAALookup() As SByte()
        Dim table(255) As SByte

        For i As Integer = 0 To 255
            table(i) = CSByte(-1)
        Next
        For i As Integer = 0 To AA_ALPHABET.Length - 1
            table(AscW(AA_ALPHABET(i)(0))) = CSByte(i)
        Next

        Return table
    End Function

    ' ---- SIMD 加速所需的模型参数扁平化缓存（首次计算时惰性构建，构建后只读共享） ----
    Private ReadOnly _planLock As New Object
    Private _planBuilt As Boolean

    ' 转移得分列：按候选位置 k = 1..Length 存储（索引 0 为占位），消除锯齿数组访问
    Private _trMM, _trMI, _trMD, _trIM, _trII, _trDM, _trDD As Double()

    ' 发射得分列：按氨基酸索引 0..19 存储，每列长度 Length + 1
    Private _matchEmissionCols As Double()()
    Private _insertEmissionCols As Double()()

    Public Overrides Function ToString() As String
        Return $"{Name} ({Length} positions, checksum:{Checksum})"
    End Function

    ''' <summary>
    ''' 初始化HMM参数，将HMMER3的对数几率比转换为概率
    ''' </summary>
    Public Function InitializeHMMParameters() As ProfileHMM
        ' 将对数几率比转换为概率
        ' HMMER3使用的是以2为底的对数几率比（单位：bits）
        ' 需要转换回概率形式以适配现有HMM框架

        Dim numStates As Integer = MatchEmissions.Count

        If numStates = 0 Then
            Return Me
        Else
            ' 创建状态对象
            ' 每个匹配位置对应一个状态
            ' 状态命名：M1, M2, ..., Mn
            ReDim HMMStates(numStates - 1)
            ReDim HMMInitialProb(numStates - 1)
        End If

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

        Return Me
    End Function

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
    ''' 构建模型参数的扁平化缓存（转移得分列 + 发射得分列），供 SIMD 向量化计算使用。
    ''' </summary>
    Private Sub EnsurePlan()
        If _planBuilt Then Return

        SyncLock _planLock
            If _planBuilt Then Return

            Dim L As Integer = MatchEmissions.Count

            _trMM = BuildTransColumn(L, TransitionType.M_TO_M)
            _trMI = BuildTransColumn(L, TransitionType.M_TO_I)
            _trMD = BuildTransColumn(L, TransitionType.M_TO_D)
            _trIM = BuildTransColumn(L, TransitionType.I_TO_M)
            _trII = BuildTransColumn(L, TransitionType.I_TO_I)
            _trDM = BuildTransColumn(L, TransitionType.D_TO_M)
            _trDD = BuildTransColumn(L, TransitionType.D_TO_D)

            _matchEmissionCols = BuildEmissionColumns(L, MatchEmissions)
            _insertEmissionCols = BuildEmissionColumns(L, InsertEmissions)

            _planBuilt = True
        End SyncLock
    End Sub

    ''' <summary>
    ''' 构建某一转移类型的得分列：<c>col(k) = Transitions(k - 1)(type)</c>，越界时回退为 0。
    ''' </summary>
    Private Function BuildTransColumn(L As Integer, type As TransitionType) As Double()
        Dim col(L) As Double
        Dim ti As Integer = CInt(type)

        If Transitions IsNot Nothing Then
            For k As Integer = 1 To L
                Dim idx As Integer = k - 1

                If idx < Transitions.Count Then
                    Dim row As Double() = Transitions(idx)

                    If row IsNot Nothing AndAlso ti < row.Length Then
                        col(k) = row(ti)
                    End If
                End If
            Next
        End If

        Return col
    End Function

    ''' <summary>
    ''' 构建某类发射得分的按氨基酸分列缓存：<c>col(aa)(k) = emissions(k - 1)(aa)</c>，越界时回退为 0。
    ''' </summary>
    Private Shared Function BuildEmissionColumns(L As Integer, emissions As Double()()) As Double()()
        Dim cols(AA_ALPHABET.Length - 1)() As Double

        If emissions Is Nothing Then
            For aa As Integer = 0 To cols.Length - 1
                cols(aa) = New Double(L) {}
            Next

            Return cols
        End If

        For aa As Integer = 0 To cols.Length - 1
            Dim col(L) As Double

            For k As Integer = 1 To L
                Dim idx As Integer = k - 1

                If idx < emissions.Length Then
                    Dim row As Double() = emissions(idx)

                    If row IsNot Nothing AndAlso aa < row.Length Then
                        col(k) = row(aa)
                    End If
                End If
            Next

            cols(aa) = col
        Next

        Return cols
    End Function

    Private Shared Function FillNegInf(size As Integer) As Double()
        Dim v(size - 1) As Double

        For i As Integer = 0 To size - 1
            v(i) = Double.NegativeInfinity
        Next

        Return v
    End Function

    ''' <summary>
    ''' 计算序列的比特得分（使用Viterbi算法）
    ''' </summary>
    ''' <remarks>
    ''' SIMD 性能优化版本：
    ''' 1. 氨基酸字符索引通过静态查找表 <see cref="AALookup"/> 完成，序列仅做一次预转换；
    ''' 2. 转移/发射得分预构建为按位置 k 连续的列缓存，消除锯齿数组访问与重复边界检查；
    ''' 3. DP 内层 M/I 行的三候选求值通过 <see cref="Microsoft.VisualBasic.Math.SIMD"/> 
    '''    的向量化加法（SimdAdd）与逐元素最大值（SimdEngine.Max）完成；
    ''' 4. 回溯指针以紧凑的字节数组存储，计算结果与原始逐单元格标量实现一致。
    ''' </remarks>
    Public Function CalculateBitScore(sequence As String) As BitScoreResult
        If String.IsNullOrEmpty(sequence) OrElse MatchEmissions.Count = 0 Then
            Return New BitScoreResult() With {.Score = 0.0}
        End If

        Call EnsurePlan()

        Dim seqLength As Integer = sequence.Length
        Dim modelLength As Integer = MatchEmissions.Count
        Dim colSize As Integer = modelLength + 1
        Const NEG_INF As Double = Double.NegativeInfinity

        ' 将序列一次性预转换为氨基酸索引数组（-1 表示未知残基）
        Dim aaIndex(seqLength - 1) As Integer

        For i As Integer = 0 To seqLength - 1
            Dim code As Integer = AscW(Char.ToUpper(sequence(i)))

            aaIndex(i) = If(code < 256, AALookup(code), CSByte(-1))
        Next

        ' DP 表：按序列位置 idx 分列存储，每一列为长度 colSize 的连续数组（k = 0..modelLength），
        ' 列内存布局连续，便于对整列做 SIMD 向量化运算
        Dim colsM(seqLength)() As Double
        Dim colsI(seqLength)() As Double
        Dim colsD(seqLength)() As Double

        ' 回溯指针：紧凑字节数组存储（值为 TraceState 枚举），字节零值即 NONE
        Dim colsTM(seqLength)() As Byte
        Dim colsTI(seqLength)() As Byte
        Dim colsTD(seqLength)() As Byte

        For idx As Integer = 0 To seqLength
            colsM(idx) = FillNegInf(colSize)
            colsI(idx) = FillNegInf(colSize)
            colsD(idx) = FillNegInf(colSize)
            colsTM(idx) = New Byte(colSize - 1) {}
            colsTI(idx) = New Byte(colSize - 1) {}
            colsTD(idx) = New Byte(colSize - 1) {}
        Next

        ' 开始状态
        colsM(0)(0) = 0.0

        ' 移位暂存缓冲：buf(k) = prev(k - 1)，复用避免逐行重新分配
        Dim bufM(colSize - 1) As Double
        Dim bufI(colSize - 1) As Double
        Dim bufD(colSize - 1) As Double

        For idx As Integer = 1 To seqLength
            Dim aaIdx As Integer = aaIndex(idx - 1)

            If aaIdx < 0 Then
                Continue For ' 未知残基：整列保持负无穷
            End If

            Dim prev As Integer = idx - 1
            Dim prevM As Double() = colsM(prev)
            Dim prevI As Double() = colsI(prev)
            Dim prevD As Double() = colsD(prev)

            ' ---- M(k, idx)：三个候选均来自 (k-1, idx-1)，移位后向量化求值 ----
            Array.Copy(prevM, 0, bufM, 1, modelLength)
            Array.Copy(prevI, 0, bufI, 1, modelLength)
            Array.Copy(prevD, 0, bufD, 1, modelLength)

            Dim candM As Double() = bufM.SimdAdd(_trMM)
            Dim candMI As Double() = bufI.SimdAdd(_trIM)
            Dim candMD As Double() = bufD.SimdAdd(_trDM)
            Dim colM As Double() = SimdEngine.Max(SimdEngine.Max(candM, candMI), candMD).SimdAdd(_matchEmissionCols(aaIdx))

            colM(0) = NEG_INF
            colsM(idx) = colM

            Dim tm As Byte() = colsTM(idx)

            For k As Integer = 1 To modelLength
                Dim a As Double = candM(k)
                Dim b As Double = candMI(k)
                Dim c As Double = candMD(k)
                Dim src As Byte

                If a = NEG_INF AndAlso b = NEG_INF AndAlso c = NEG_INF Then
                    src = CByte(TraceState.NONE)
                ElseIf a >= b AndAlso a >= c Then
                    src = CByte(TraceState.MATCH)
                ElseIf b >= c Then
                    src = CByte(TraceState.INSERT)
                Else
                    src = CByte(TraceState.DELETE)
                End If

                tm(k) = src
            Next

            ' ---- I(k, idx)：两个候选均来自 (k, idx-1)，直接向量化求值 ----
            Dim candIM As Double() = prevM.SimdAdd(_trMI)
            Dim candII As Double() = prevI.SimdAdd(_trII)
            Dim colI As Double() = SimdEngine.Max(candIM, candII).SimdAdd(_insertEmissionCols(aaIdx))

            colI(0) = NEG_INF
            colsI(idx) = colI

            Dim ti As Byte() = colsTI(idx)

            For k As Integer = 1 To modelLength
                Dim a As Double = candIM(k)
                Dim b As Double = candII(k)
                Dim src As Byte

                If a = NEG_INF AndAlso b = NEG_INF Then
                    src = CByte(TraceState.NONE)
                ElseIf a >= b Then
                    src = CByte(TraceState.MATCH)
                Else
                    src = CByte(TraceState.INSERT)
                End If

                ti(k) = src
            Next

            ' ---- D(k, idx)：候选依赖当前列的 (k-1) 位置，存在列内顺序依赖，保持标量扫描 ----
            Dim colD As Double() = colsD(idx)
            Dim td As Byte() = colsTD(idx)
            Dim curM As Double() = colsM(idx)
            Dim runD As Double = NEG_INF

            For k As Integer = 1 To modelLength
                Dim bestD As Double = NEG_INF
                Dim src As Byte = CByte(TraceState.NONE)

                Dim fromMD As Double = curM(k - 1) + _trMD(k)

                If fromMD > bestD Then
                    bestD = fromMD
                    src = CByte(TraceState.MATCH)
                End If

                Dim fromDD As Double = runD + _trDD(k)

                If fromDD > bestD Then
                    bestD = fromDD
                    src = CByte(TraceState.DELETE)
                End If

                colD(k) = bestD
                td(k) = src
                runD = bestD
            Next
        Next

        ' 终止列扫描：与原始实现保持完全一致的遍历顺序与严格大于判定
        Dim finalScore As Double = NEG_INF
        Dim endK As Integer = 0
        Dim endState As Byte = CByte(TraceState.NONE)
        Dim lastM As Double() = colsM(seqLength)
        Dim lastI As Double() = colsI(seqLength)
        Dim lastD As Double() = colsD(seqLength)

        For k As Integer = 1 To modelLength
            If lastM(k) > finalScore Then
                finalScore = lastM(k)
                endK = k
                endState = CByte(TraceState.MATCH)
            End If

            If lastI(k) > finalScore Then
                finalScore = lastI(k)
                endK = k
                endState = CByte(TraceState.INSERT)
            End If

            If lastD(k) > finalScore Then
                finalScore = lastD(k)
                endK = k
                endState = CByte(TraceState.DELETE)
            End If
        Next

        ' 回溯比对路径
        Dim alignmentPath As New List(Of AlignmentPosition)
        Dim currentK As Integer = endK
        Dim currentI As Integer = seqLength
        Dim currentState As Byte = endState

        While currentK > 0 AndAlso currentI > 0
            alignmentPath.Add(New AlignmentPosition With {
                .ModelPosition = currentK,
                .SequencePosition = currentI,
                .State = CType(currentState, TraceState)
            })

            Dim src As Byte = CByte(TraceState.NONE)

            Select Case currentState
                Case CByte(TraceState.MATCH) : src = colsTM(currentI)(currentK)
                Case CByte(TraceState.INSERT) : src = colsTI(currentI)(currentK)
                Case CByte(TraceState.DELETE) : src = colsTD(currentI)(currentK)
            End Select

            If src = CByte(TraceState.NONE) Then Exit While

            Select Case src
                Case CByte(TraceState.MATCH)
                    currentK -= 1
                    currentI -= 1
                Case CByte(TraceState.INSERT)
                    currentI -= 1
                Case CByte(TraceState.DELETE)
                    currentK -= 1
            End Select

            currentState = src
        End While

        alignmentPath.Reverse()

        Return New BitScoreResult() With {
            .Score = If(Double.IsNegativeInfinity(finalScore), 0.0, finalScore),
            .AlignmentPath = alignmentPath
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

    ' ============================================================================
    ' 二进制序列化
    '
    ' 模型数据以 BinaryWriter/BinaryReader 直接写入原始数值，避免 JSON 等字符串
    ' 序列化所带来的 Double <-> 字符串转换开销。写入与读取的字段顺序必须保持
    ' 完全对称。派生字段（HMMStates/HMMObservables/HMMInitialProb）不持久化，
    ' 加载后可通过 InitializeHMMParameters 按需重建。
    ' ============================================================================

    Private Const BinaryFormatVersion As Integer = 1

    ''' <summary>
    ''' 将模型参数以二进制格式写入 <see cref="BinaryWriter"/>
    ''' </summary>
    Public Sub WriteBinary(writer As BinaryWriter)
        writer.Write(BinaryFormatVersion)
        writer.Write(If(Version, ""))
        writer.Write(If(Name, ""))
        writer.Write(Length)
        writer.Write(If(Alphabet, ""))
        writer.Write(NumSequences)
        writer.Write(EffectiveNum)
        writer.Write(Checksum)

        Call WriteStats(writer, StatsMSV)
        Call WriteStats(writer, StatsViterbi)
        Call WriteStats(writer, StatsForward)

        Call WriteArray(writer, CompositionEmission)
        Call WriteArray(writer, CompositionInsert)
        Call WriteArray(writer, CompositionTransitions)

        Call WriteJagged(writer, MatchEmissions)
        Call WriteJagged(writer, InsertEmissions)
        Call WriteJagged(writer, Transitions)
    End Sub

    ''' <summary>
    ''' 从 <see cref="BinaryReader"/> 中按 <see cref="WriteBinary"/> 的布局读取并重建模型对象
    ''' </summary>
    Public Shared Function ReadBinary(reader As BinaryReader) As ProfileHMM
        Dim version As Integer = reader.ReadInt32()

        If version > BinaryFormatVersion Then
            Throw New InvalidDataException($"Unsupported profile HMM binary format version: {version}")
        End If

        Dim model As New ProfileHMM With {
            .Version = reader.ReadString(),
            .Name = reader.ReadString(),
            .Length = reader.ReadInt32(),
            .Alphabet = reader.ReadString(),
            .NumSequences = reader.ReadInt32(),
            .EffectiveNum = reader.ReadDouble(),
            .Checksum = reader.ReadInt64()
        }

        model.StatsMSV = ReadStats(reader)
        model.StatsViterbi = ReadStats(reader)
        model.StatsForward = ReadStats(reader)

        model.CompositionEmission = ReadArray(reader)
        model.CompositionInsert = ReadArray(reader)
        model.CompositionTransitions = ReadArray(reader)

        model.MatchEmissions = ReadJagged(reader)
        model.InsertEmissions = ReadJagged(reader)
        model.Transitions = ReadJagged(reader)

        Return model
    End Function

    Private Shared Sub WriteStats(writer As BinaryWriter, stats As (mu As Double, lambda As Double))
        writer.Write(stats.mu)
        writer.Write(stats.lambda)
    End Sub

    Private Shared Function ReadStats(reader As BinaryReader) As (mu As Double, lambda As Double)
        Return (reader.ReadDouble(), reader.ReadDouble())
    End Function

    Private Shared Sub WriteArray(writer As BinaryWriter, arr As Double())
        If arr Is Nothing Then
            writer.Write(-1)
        Else
            writer.Write(arr.Length)

            For i As Integer = 0 To arr.Length - 1
                writer.Write(arr(i))
            Next
        End If
    End Sub

    Private Shared Function ReadArray(reader As BinaryReader) As Double()
        Dim n As Integer = reader.ReadInt32()

        If n < 0 Then
            Return Nothing
        End If

        Dim v(n - 1) As Double

        For i As Integer = 0 To n - 1
            v(i) = reader.ReadDouble()
        Next

        Return v
    End Function

    Private Shared Sub WriteJagged(writer As BinaryWriter, rows As Double()())
        If rows Is Nothing Then
            writer.Write(-1)
        Else
            writer.Write(rows.Length)

            For i As Integer = 0 To rows.Length - 1
                Call WriteArray(writer, rows(i))
            Next
        End If
    End Sub

    Private Shared Function ReadJagged(reader As BinaryReader) As Double()()
        Dim n As Integer = reader.ReadInt32()

        If n < 0 Then
            Return Nothing
        End If

        Dim rows(n - 1)() As Double

        For i As Integer = 0 To n - 1
            rows(i) = ReadArray(reader)
        Next

        Return rows
    End Function
End Class
