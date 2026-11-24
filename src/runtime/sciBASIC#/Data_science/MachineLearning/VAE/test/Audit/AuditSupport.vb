' ============================================================================
' AuditSupport.vb - 生产化审计的公共支撑设施
'
' 提供：报告路径约定、审计分组运行器、数值断言辅助、合成数据生成、
'       聚类准确率（最佳标签置换）评估。
'
' 设计要点：
'   - 任何未捕获异常都会被折叠成一条审计失败记录，不会中止整个审计流程；
'   - 只使用 DebugAudit 中非 <Conditional("DEBUG")> 的 API（Test/AreEqual/Mark/Reset/
'     Watch/Dump/SaveReport），保证 Release 配置下审计逻辑不会被编译器移除。
' ============================================================================

Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' VAE 生产化审计的公共支撑设施。
''' </summary>
Public Module AuditSupport

    ''' <summary>审计报告文件名（写入 <see cref="AppContext.BaseDirectory"/>）。</summary>
    Public Const ReportName As String = "vae-audit-report.json"

    ''' <summary>审计事件明细的纯文本转储文件名。</summary>
    Public Const DumpName As String = "vae-audit-dump.txt"

    ''' <summary>审计报告的绝对路径。</summary>
    Public Function ReportPath() As String
        Return IO.Path.Combine(AppContext.BaseDirectory, ReportName)
    End Function

    ''' <summary>清空审计状态并写入起始标记。</summary>
    Public Sub Begin()
        Call DebugAudit.Reset()
        DebugAudit.MaxEvents = 65536
        Call DebugAudit.Mark("vae production audit started")
        Console.WriteLine()
        Console.WriteLine(New String("="c, 78))
        Console.WriteLine("  VAE 生成模型生产化审计")
        Console.WriteLine(New String("="c, 78))
    End Sub

    ''' <summary>
    ''' 运行一个审计分组。异常被折叠为失败记录，而不是中断后续分组。
    ''' </summary>
    Public Sub Group(name As String, action As Action)
        Console.WriteLine()
        Console.WriteLine($"--- [audit] {name} ---")
        Call DebugAudit.Mark($"audit group begin: {name}")

        Using DebugAudit.Watch($"audit:{name}", logOnExit:=True)
            Try
                Call action()
            Catch ex As Exception
                Call DebugAudit.Test(False, $"audit group '{name}' aborted by {ex.GetType().Name}: {ex.Message}")
            End Try
        End Using

        Call DebugAudit.Mark($"audit group end: {name}")
    End Sub

    ''' <summary>
    ''' 运行单个审计用例。异常被折叠为一条失败记录，同一个分组内的后续用例继续执行。
    ''' </summary>
    Public Sub RunCase(name As String, action As Action)
        Try
            Call action()
        Catch ex As Exception
            Call DebugAudit.Test(False, $"audit case '{name}' aborted by {ex.GetType().Name}: {ex.Message}")
        End Try
    End Sub

    ''' <summary>写出审计报告并打印摘要，返回失败项数量。</summary>
    Public Function Finish() As Integer
        Dim path As String = ReportPath()
        Dim dump As String = DebugAudit.Dump()

        Try
            IO.File.WriteAllText(IO.Path.Combine(AppContext.BaseDirectory, DumpName), dump)
        Catch ex As Exception
            ' 明细转储失败不影响主报告
        End Try

        Try
            Call DebugAudit.SaveReport(path)
        Catch ex As Exception
            Console.WriteLine($"   [audit] 无法写入报告: {ex.Message}")
        End Try

        Console.WriteLine()
        Console.WriteLine(New String("="c, 78))
        Console.WriteLine($" 审计结束: {DebugAudit.FailureCount} 项失败")
        Console.WriteLine($"   报告 -> {path}")
        Console.WriteLine(New String("="c, 78))

        Return DebugAudit.FailureCount
    End Function

#Region "数值断言辅助"

    ''' <summary>判断标量是否为有限值（非 NaN、非 ±Infinity）。</summary>
    Public Function IsFinite(value As Double) As Boolean
        Return Not (Double.IsNaN(value) OrElse Double.IsInfinity(value))
    End Function

    ''' <summary>判断张量的全部元素是否为有限值。</summary>
    Public Function AllFinite(t As Tensor) As Boolean
        If t Is Nothing Then
            Return False
        End If

        Dim data As Double() = t.Data

        For i As Integer = 0 To data.Length - 1
            If Double.IsNaN(data(i)) OrElse Double.IsInfinity(data(i)) Then
                Return False
            End If
        Next

        Return True
    End Function

    ''' <summary>判断一维数组的全部元素是否为有限值。</summary>
    Public Function AllFinite(values As IEnumerable(Of Double)) As Boolean
        If values Is Nothing Then
            Return False
        End If

        For Each value As Double In values
            If Double.IsNaN(value) OrElse Double.IsInfinity(value) Then
                Return False
            End If
        Next

        Return True
    End Function

    ''' <summary>两个张量之间的最大绝对逐元素误差。</summary>
    Public Function MaxAbsDiff(a As Tensor, b As Tensor) As Double
        Dim n As Integer = std.Min(a.Length, b.Length)
        Dim maxValue As Double = 0

        For i As Integer = 0 To n - 1
            Dim value As Double = std.Abs(a.Data(i) - b.Data(i))

            If value > maxValue Then
                maxValue = value
            End If
        Next

        Return maxValue
    End Function

    ''' <summary>逐元素均方误差。</summary>
    Public Function Mse(a As Tensor, b As Tensor) As Double
        Dim n As Integer = std.Min(a.Length, b.Length)

        If n = 0 Then
            Return Double.NaN
        End If

        Dim acc As Double = 0

        For i As Integer = 0 To n - 1
            Dim diff As Double = a.Data(i) - b.Data(i)
            acc += diff * diff
        Next

        Return acc / n
    End Function

    ''' <summary>张量整体求和（避免依赖后端 <c>TotalSum</c> 的单精度累积）。</summary>
    Public Function SumOf(t As Tensor) As Double
        Dim data As Double() = t.Data
        Dim acc As Double = 0

        For i As Integer = 0 To data.Length - 1
            acc += data(i)
        Next

        Return acc
    End Function

#End Region

#Region "合成数据"

    ''' <summary>
    ''' 生成若干个等权、可分离的高斯簇。
    ''' </summary>
    ''' <param name="nClusters">簇数量</param>
    ''' <param name="perCluster">每簇样本数</param>
    ''' <param name="dim">数据维度</param>
    ''' <param name="spread">簇内标准差</param>
    ''' <param name="seed">随机种子</param>
    ''' <param name="X">输出的样本矩阵 [nClusters*perCluster, dim]</param>
    ''' <param name="labels">输出的真实标签 [nClusters*perCluster]</param>
    Public Sub GaussianBlobs(nClusters As Integer, perCluster As Integer, dimensions As Integer,
                             spread As Double, seed As Integer,
                             ByRef X As Tensor, ByRef labels As Integer())

        Dim total As Integer = nClusters * perCluster
        Dim data As Double() = New Double(total * dimensions - 1) {}
        labels = New Integer(total - 1) {}

        Dim rng As New Random(seed)

        For c As Integer = 0 To nClusters - 1
            ' 每个簇在每个维度上取确定性偏移，保证簇心之间彼此远离
            Dim center As Double() = New Double(dimensions - 1) {}

            For d As Integer = 0 To dimensions - 1
                center(d) = 4.0 * std.Cos(2.0 * std.PI * (c + 1) * (d + 1) / (nClusters + 1))
            Next

            For s As Integer = 0 To perCluster - 1
                Dim row As Integer = c * perCluster + s
                labels(row) = c

                For d As Integer = 0 To dimensions - 1
                    data(row * dimensions + d) = center(d) + spread * Randn(rng)
                Next
            Next
        Next

        X = New Tensor(data, total, dimensions)
    End Sub

    ''' <summary>标准正态随机数（Box-Muller）。</summary>
    Public Function Randn(rng As Random) As Double
        Dim u1 As Double = 1.0 - rng.NextDouble()
        Dim u2 As Double = 1.0 - rng.NextDouble()

        Return std.Sqrt(-2.0 * std.Log(u1)) * std.Cos(2.0 * std.PI * u2)
    End Function

    ''' <summary>生成一个指定形状的随机张量（均匀分布）。</summary>
    Public Function RandomTensor(shape As Integer(), seed As Integer) As Tensor
        Dim total As Integer = 1

        For Each size As Integer In shape
            total *= size
        Next

        Dim rng As New Random(seed)
        Dim data As Double() = New Double(total - 1) {}

        For i As Integer = 0 To total - 1
            data(i) = rng.NextDouble() * 2.0 - 1.0
        Next

        Return New Tensor(data, shape)
    End Function

#End Region

#Region "聚类评估"

    ''' <summary>
    ''' 计算聚类准确率，并搜索最佳标签置换（簇数量不超过 7 时精确搜索，
    ''' 否则退化为贪心分配）。
    ''' </summary>
    Public Function ClusterAccuracy(predicted As Integer(), truth As Integer(), k As Integer) As Double
        If predicted Is Nothing OrElse truth Is Nothing OrElse k <= 0 Then
            Return 0
        End If

        Dim n As Integer = std.Min(predicted.Length, truth.Length)

        If n = 0 Then
            Return 0
        End If

        Dim confusion As Integer(,) = New Integer(k - 1, k - 1) {}

        For i As Integer = 0 To n - 1
            Dim p As Integer = predicted(i)
            Dim t As Integer = truth(i)

            If p >= 0 AndAlso p < k AndAlso t >= 0 AndAlso t < k Then
                confusion(t, p) += 1
            End If
        Next

        Dim best As Integer = 0

        If k <= 7 Then
            Dim used As Boolean() = New Boolean(k - 1) {}
            Call SearchPermutation(confusion, k, 0, used, best, 0)
        Else
            best = GreedyAssignment(confusion, k)
        End If

        Return CDbl(best) / n
    End Function

    Private Sub SearchPermutation(confusion As Integer(,), k As Integer, depth As Integer,
                                  used As Boolean(), ByRef best As Integer, current As Integer)
        If depth = k Then
            If current > best Then
                best = current
            End If

            Return
        End If

        For candidate As Integer = 0 To k - 1
            If Not used(candidate) Then
                used(candidate) = True
                Call SearchPermutation(confusion, k, depth + 1, used, best, current + confusion(depth, candidate))
                used(candidate) = False
            End If
        Next
    End Sub

    Private Function GreedyAssignment(confusion As Integer(,), k As Integer) As Integer
        Dim used As Boolean() = New Boolean(k - 1) {}
        Dim acc As Integer = 0

        For row As Integer = 0 To k - 1
            Dim bestCol As Integer = -1
            Dim bestValue As Integer = -1

            For col As Integer = 0 To k - 1
                If Not used(col) AndAlso confusion(row, col) > bestValue Then
                    bestValue = confusion(row, col)
                    bestCol = col
                End If
            Next

            If bestCol >= 0 Then
                used(bestCol) = True
                acc += bestValue
            End If
        Next

        Return acc
    End Function

#End Region

End Module
