' ============================================================================
' 蛋白质序列分类注释模块
' 
' 实现蛋白质FASTA序列的读取和基于HMMER3模型的分类注释功能
' 
' Author: 基于用户现有HMM代码框架扩展
' Copyright (c) 2024 GPL3 Licensed
' ============================================================================

Imports System.IO
Imports SMRUCC.genomics.SequenceModel.FASTA

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
    Public Function Annotate(protein As FastaSeq) As AnnotationResult
        If protein Is Nothing OrElse String.IsNullOrEmpty(protein.SequenceData) Then
            Return Nothing
        End If

        Dim bestResult As AnnotationResult = Nothing
        Dim bestScore As Double = Double.NegativeInfinity

        For Each kvp As KeyValuePair(Of String, ProfileHMM) In _models
            Dim model As ProfileHMM = kvp.Value
            Dim result As AnnotationResult = CompareSequence(protein.SequenceData, model)

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

        Return bestResult
    End Function

    ''' <summary>
    ''' 对蛋白质序列列表进行批量注释
    ''' </summary>
    ''' <param name="proteins">蛋白质序列列表</param>
    Public Sub AnnotateAll(proteins As IEnumerable(Of FastaSeq))
        For Each protein As FastaSeq In proteins
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
        For idx As Integer = 0 To modelLength
            M(idx) = NEG_INF
            I(idx) = NEG_INF
            D(idx) = NEG_INF
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

            For idx As Integer = 0 To modelLength
                newM(idx) = NEG_INF
                newI(idx) = NEG_INF
                newD(idx) = NEG_INF
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
