Imports SMRUCC.genomics.Analysis.GPS.Config

Namespace Data

    ''' <summary>
    ''' 一个滑窗训练样本：窗口内的表达序列与下一时刻的目标表达。
    ''' </summary>
    Public Class TimeSeriesWindow

        ''' <summary>样本序号（按目标时刻升序）</summary>
        Public Property Index As Integer

        ''' <summary>输入窗口 <c>[基因, w]</c>，列顺序为从早到晚</summary>
        Public Property Input As Double(,)

        ''' <summary>窗口最后一个时刻的表达 <c>[基因]</c>，方向感知损失的基准</summary>
        Public Property Previous As Double()

        ''' <summary>下一时刻的目标表达 <c>[基因]</c></summary>
        Public Property Target As Double()

        ''' <summary>窗口内各列对应的时间点标签</summary>
        Public Property WindowTimes As Double()

        ''' <summary>目标时刻的时间点标签</summary>
        Public Property TargetTime As Double

        ''' <summary>基因数量</summary>
        ''' <returns>输入窗口的行数</returns>
        Public ReadOnly Property NGene As Integer
            Get
                Return Input.GetLength(0)
            End Get
        End Property

        ''' <summary>窗口长度</summary>
        ''' <returns>输入窗口的列数</returns>
        Public ReadOnly Property WindowSize As Integer
            Get
                Return Input.GetLength(1)
            End Get
        End Property

    End Class

    ''' <summary>
    ''' 时序滑窗数据集：按唯一时间点聚合重复样本后切分窗口，
    ''' 并按时间顺序（而非随机）划分训练 / 验证 / 测试集。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' method.md §7.1 明确要求严格按时间切分：随机打乱窗口会造成时间信息泄漏，
    ''' 使评估结果虚高。本类因此只按目标时刻升序排列窗口，再按比例切成三段。
    ''' </para>
    ''' <para>
    ''' 时间点较少时（<c>窗口数 &lt; 3</c>）测试集可能为空，
    ''' 调用方应回退到在验证集上报告指标。
    ''' </para>
    ''' <para>
    ''' 重复样本（同一时间点的多个重复）默认取均值聚合为一个表达谱；
    ''' 这意味着窗口输入是"时间点 × 基因"的确定矩阵，不含重复间随机性。
    ''' </para>
    ''' </remarks>
    Public Class TimeSeriesWindowDataset

        ''' <summary>全部窗口，按目标时刻升序</summary>
        Public Property Windows As New List(Of TimeSeriesWindow)()

        ''' <summary>
        ''' 按时间点聚合后的表达谱 <c>[基因, 唯一时间点]</c>，
        ''' 供虚拟扰动的基线状态与图编码使用。
        ''' </summary>
        Public Property Profiles As Double(,)

        ''' <summary>唯一时间点标签（升序）</summary>
        Public Property UniqueTimes As Double()

        ''' <summary>基因名称，顺序与表达谱的行一致</summary>
        Public Property GeneNames As String()

        ''' <summary>窗口长度</summary>
        Public Property WindowSize As Integer

        ''' <summary>训练集窗口数量（时间上最早的连续一段）</summary>
        Public Property TrainCount As Integer

        ''' <summary>验证集窗口数量（训练集之后、测试集之前）</summary>
        Public Property ValidationCount As Integer

        ''' <summary>基因数量</summary>
        ''' <returns>表达谱的行数</returns>
        Public ReadOnly Property NGene As Integer
            Get
                Return If(GeneNames Is Nothing, 0, GeneNames.Length)
            End Get
        End Property

        ''' <summary>唯一时间点数量</summary>
        ''' <returns>表达谱的列数</returns>
        Public ReadOnly Property NTimePoint As Integer
            Get
                Return If(UniqueTimes Is Nothing, 0, UniqueTimes.Length)
            End Get
        End Property

        ''' <summary>窗口总数</summary>
        ''' <returns>全部窗口数量</returns>
        Public ReadOnly Property Count As Integer
            Get
                Return Windows.Count
            End Get
        End Property

        ''' <summary>训练集窗口（时间上最早的一段）</summary>
        ''' <returns>训练窗口列表</returns>
        Public Function Train() As List(Of TimeSeriesWindow)
            Return Windows.Take(TrainCount).ToList()
        End Function

        ''' <summary>验证集窗口（训练集与测试集之间的一段）</summary>
        ''' <returns>验证窗口列表</returns>
        Public Function Validation() As List(Of TimeSeriesWindow)
            Return Windows.Skip(TrainCount).Take(ValidationCount).ToList()
        End Function

        ''' <summary>测试集窗口（时间上最晚的一段）</summary>
        ''' <returns>测试窗口列表</returns>
        Public Function Test() As List(Of TimeSeriesWindow)
            Return Windows.Skip(TrainCount + ValidationCount).ToList()
        End Function

        ''' <summary>
        ''' 由预处理后的表达数据构建滑窗数据集。
        ''' </summary>
        ''' <param name="data">预处理后的时序表达数据</param>
        ''' <param name="config">配置对象（窗口长度与切分比例）</param>
        ''' <returns>滑窗数据集</returns>
        Public Shared Function Create(data As PreprocessedExpression, config As GPSConfig) As TimeSeriesWindowDataset
            If data Is Nothing Then Throw New ArgumentNullException(NameOf(data))
            If config Is Nothing Then Throw New ArgumentNullException(NameOf(config))

            Dim dims As (Profiles As Double(,), Times As Double()) = AggregateByTime(data)
            Dim profiles As Double(,) = dims.Profiles
            Dim times As Double() = dims.Times
            Dim nTime As Integer = times.Length

            If nTime < 2 Then
                Throw New ArgumentException($"时序表达数据至少需要 2 个不同时间点，实际 {nTime} 个")
            End If

            Dim windowSize As Integer = config.Window

            If windowSize < 1 Then windowSize = 1
            If windowSize > nTime - 1 Then windowSize = nTime - 1

            Dim result As New TimeSeriesWindowDataset With {
                .Profiles = profiles,
                .UniqueTimes = times,
                .GeneNames = data.GeneNames,
                .WindowSize = windowSize
            }

            Dim nGene As Integer = profiles.GetLength(0)

            ' ---- 滑窗切片：t = w-1 .. T-2，目标为 t+1 ----
            For t As Integer = windowSize - 1 To nTime - 2
                Dim input As Double(,) = New Double(nGene - 1, windowSize - 1) {}
                Dim windowTimes As Double() = New Double(windowSize - 1) {}
                Dim previous As Double() = New Double(nGene - 1) {}
                Dim target As Double() = New Double(nGene - 1) {}

                For c As Integer = 0 To windowSize - 1
                    Dim src As Integer = t - windowSize + 1 + c
                    windowTimes(c) = times(src)

                    For i As Integer = 0 To nGene - 1
                        input(i, c) = profiles(i, src)
                    Next
                Next

                For i As Integer = 0 To nGene - 1
                    previous(i) = profiles(i, t)
                    target(i) = profiles(i, t + 1)
                Next

                Call result.Windows.Add(New TimeSeriesWindow With {
                    .Index = result.Windows.Count,
                    .Input = input,
                    .Previous = previous,
                    .Target = target,
                    .WindowTimes = windowTimes,
                    .TargetTime = times(t + 1)
                })
            Next

            Call result.ApplySplit(config)

            Return result
        End Function

        ''' <summary>
        ''' 按时间顺序切分训练 / 验证 / 测试集（禁止随机打乱）。
        ''' </summary>
        ''' <param name="config">提供切分比例</param>
        Private Sub ApplySplit(config As GPSConfig)
            Dim total As Integer = Windows.Count

            If total = 0 Then
                TrainCount = 0
                ValidationCount = 0
                Return
            End If

            Dim nTrain As Integer = CInt(System.Math.Floor(total * config.TrainRatio))
            Dim nValidation As Integer = CInt(System.Math.Floor(total * config.ValidationRatio))

            ' 至少给训练集留一个样本，否则模型无从训练
            If config.TrainRatio > 0.0 AndAlso nTrain < 1 Then nTrain = 1

            ' 验证集不能挤占训练集；样本很少时宁可为空
            If nValidation > total - nTrain Then nValidation = total - nTrain
            If nValidation < 0 Then nValidation = 0

            TrainCount = nTrain
            ValidationCount = nValidation
        End Sub

        ''' <summary>
        ''' 把带重复样本的表达矩阵按时间点标签聚合为均值表达谱。
        ''' </summary>
        ''' <param name="data">预处理后的表达数据</param>
        ''' <returns>聚合后的表达谱与升序唯一时间点</returns>
        Private Shared Function AggregateByTime(data As PreprocessedExpression) As (Profiles As Double(,), Times As Double())
            Dim matrix As Double(,) = data.Matrix
            Dim nGene As Integer = matrix.GetLength(0)
            Dim nSample As Integer = matrix.GetLength(1)
            Dim timePoints As Double() = data.TimePoints

            ' 没有时间标签时退化为"每个样本自成一个时间点"
            If timePoints Is Nothing OrElse timePoints.Length <> nSample Then
                Dim copy As Double(,) = CType(matrix.Clone(), Double(,))
                Dim seq As Double() = New Double(nSample - 1) {}

                For j As Integer = 0 To nSample - 1
                    seq(j) = j
                Next

                Return (copy, seq)
            End If

            ' ---- 按时间排序并分组（同一时间点的重复样本归为一组） ----
            Dim order As Integer() = Enumerable.Range(0, nSample).
                OrderBy(Function(j) timePoints(j)).
                ToArray()

            Dim groups As New List(Of List(Of Integer))()

            For Each j As Integer In order
                If groups.Count = 0 OrElse
                   System.Math.Abs(timePoints(j) - timePoints(groups(groups.Count - 1)(0))) > 1.0E-09 Then
                    Call groups.Add(New List(Of Integer)())
                End If

                Call groups(groups.Count - 1).Add(j)
            Next

            Dim nTime As Integer = groups.Count
            Dim profiles As Double(,) = New Double(nGene - 1, nTime - 1) {}
            Dim times As Double() = New Double(nTime - 1) {}

            For g As Integer = 0 To nTime - 1
                Dim members As List(Of Integer) = groups(g)
                Dim inv As Double = 1.0 / members.Count

                times(g) = timePoints(members(0))

                For i As Integer = 0 To nGene - 1
                    Dim sum As Double = 0.0

                    For Each j As Integer In members
                        sum += matrix(i, j)
                    Next

                    profiles(i, g) = sum * inv
                Next
            Next

            Return (profiles, times)
        End Function

    End Class

End Namespace
