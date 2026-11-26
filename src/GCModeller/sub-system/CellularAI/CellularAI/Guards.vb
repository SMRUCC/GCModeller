Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Model

''' <summary>
''' 数值护栏的计数器：记录"某一类退化情形发生了多少次 / 占多少比例"。
''' </summary>
''' <remarks>
''' <para>
''' 算法里有很多"不能说停就停"的时刻：方差被地板截断、权重过度集中、相关性的标准差过小、
''' 幅度搜索触顶……上游把这些情况统一收集成 <c>guards</c> 字典，随证据一起输出，
''' 由门控（gates）判断"触发率是否超过容忍度"，而不是在计算中途抛异常。
''' </para>
''' <para>
''' 每一项统计是三元组 <c>(命中数, 总数, 比例)</c>，与上游 <c>guard_counts</c> 的三列布局一致。
''' </para>
''' <para>
''' <b>作用域</b>：这是本程序集里唯一的全局可变状态。调用方必须在每个阶段开始处
''' <see cref="Reset"/>，结束处 <see cref="Snapshot"/> 取走结果，不要跨阶段混用。
''' </para>
''' <para>
''' <b>线程安全</b>：合成阶段会按扰动并行，所有写入都在锁内完成，
''' 因此并行与串行的计数结果一致（但计数只用于诊断，不参与数值计算）。
''' </para>
''' </remarks>
Public Module Guards

    Private ReadOnly sync As New Object()
    Private ReadOnly hits As New Dictionary(Of String, Double)(StringComparer.Ordinal)
    Private ReadOnly totals As New Dictionary(Of String, Double)(StringComparer.Ordinal)

    ''' <summary>清空全部计数。</summary>
    Public Sub Reset()
        SyncLock sync
            Call hits.Clear()
            Call totals.Clear()
        End SyncLock
    End Sub

    ''' <summary>
    ''' 记录一项：<paramref name="hit"/> 次命中，共 <paramref name="total"/> 次机会。
    ''' </summary>
    ''' <param name="name">护栏名称（同时是输出字典的键）</param>
    ''' <param name="hit">命中数；可以为小数（例如"比例型"命中）</param>
    ''' <param name="total">机会总数；为 0 时按 1 处理，避免除零</param>
    Public Sub Mark(name As String, hit As Double, total As Double)
        If String.IsNullOrEmpty(name) Then
            Return
        End If

        SyncLock sync
            Dim h As Double = Nothing
            Dim t As Double = Nothing

            Call hits.TryGetValue(name, h)
            Call totals.TryGetValue(name, t)

            hits(name) = h + hit
            totals(name) = t + If(total > 0.0, total, 1.0)
        End SyncLock
    End Sub

    ''' <summary>
    ''' 记录一个离散事件：<paramref name="count"/> 个元素里发生了 <paramref name="fired"/> 个。
    ''' </summary>
    Public Sub Fired(name As String, fired As Integer, count As Integer)
        Call Mark(name, fired, count)
    End Sub

    ''' <summary>
    ''' 记录一个布尔型事件（发生记 1，未发生记 0）。
    ''' </summary>
    Public Sub Fired(name As String, fired As Boolean)
        Call Mark(name, If(fired, 1.0, 0.0), 1.0)
    End Sub

    ''' <summary>
    ''' 把超出 ±<paramref name="limit"/> 的值裁剪回边界，并记录被裁剪的比例。
    ''' </summary>
    ''' <param name="values">待裁剪的数组（不会就地修改，返回新数组）</param>
    ''' <param name="limit">绝对值上限</param>
    ''' <param name="name">护栏名称</param>
    Public Function ClipAbs(values As Double(), limit As Double, name As String) As Double()
        If values Is Nothing Then
            Return New Double(-1) {}
        End If

        Dim result(values.Length - 1) As Double
        Dim count As Integer = 0

        For i As Integer = 0 To values.Length - 1
            Dim v As Double = values(i)

            If Double.IsNaN(v) Then
                result(i) = v
                Continue For
            End If

            If v > limit Then
                result(i) = limit
                count += 1
            ElseIf v < -limit Then
                result(i) = -limit
                count += 1
            Else
                result(i) = v
            End If
        Next

        Call Fired(name, count, values.Length)

        Return result
    End Function

    ''' <summary>
    ''' 记录"有多少值超过上限"。
    ''' </summary>
    Public Sub Over(values As Double(), limit As Double, name As String)
        Call Count(values, limit, over:=True, name:=name)
    End Sub

    ''' <summary>
    ''' 记录"有多少值低于下限"。
    ''' </summary>
    Public Sub Under(values As Double(), limit As Double, name As String)
        Call Count(values, limit, over:=False, name:=name)
    End Sub

    Private Sub Count(values As Double(), limit As Double, over As Boolean, name As String)
        If values Is Nothing Then
            Return
        End If

        Dim count As Integer = 0

        For i As Integer = 0 To values.Length - 1
            Dim v As Double = values(i)

            If Double.IsNaN(v) Then
                Continue For
            End If

            If over AndAlso v > limit Then
                count += 1
            ElseIf Not over AndAlso v < limit Then
                count += 1
            End If
        Next

        Call Fired(name, count, values.Length)
    End Sub

    ''' <summary>
    ''' 记录一组权重的集中度：最大权重占比与有效样本数 <c>(Σw)²/Σw²</c>。
    ''' </summary>
    ''' <param name="name">阶段名（例如 <c>plan</c> / <c>sim</c> / <c>pooling</c>）</param>
    ''' <param name="weights">权重（非正值按 0 处理）</param>
    ''' <param name="options">护栏阈值；提供时同时记录"是否越界"的 0/1 事件</param>
    Public Sub Share(name As String, weights As Double(), Optional options As GuardOptions = Nothing)
        If String.IsNullOrEmpty(name) OrElse weights Is Nothing Then
            Return
        End If

        Dim total As Double = 0.0
        Dim squares As Double = 0.0
        Dim largest As Double = 0.0

        For i As Integer = 0 To weights.Length - 1
            Dim w As Double = weights(i)

            If w <= 0.0 OrElse Double.IsNaN(w) Then
                Continue For
            End If

            total += w
            squares += w * w

            If w > largest Then
                largest = w
            End If
        Next

        If total <= 0.0 OrElse squares <= 0.0 Then
            Return
        End If

        Dim share As Double = largest / total
        Dim neff As Double = total * total / squares

        Call Mark(name & "_share", share, 1.0)
        Call Mark(name & "_neff", neff, 1.0)

        If options Is Nothing Then
            Return
        End If

        Dim shareLimit As Double = options.ShareMaxFor(name)
        Dim neffLimit As Double = options.NeffMinFor(name)

        If shareLimit > 0.0 Then
            Call Fired(name & "_share_exceeded", share > shareLimit)
        End If
        If neffLimit > 0.0 Then
            Call Fired(name & "_neff_below", neff < neffLimit)
        End If
    End Sub

    ''' <summary>
    ''' 取走当前全部计数的副本：<c>名称 → {命中数, 总数, 比例}</c>。
    ''' </summary>
    Public Function Snapshot() As Dictionary(Of String, Double())
        Dim result As New Dictionary(Of String, Double())(StringComparer.Ordinal)

        SyncLock sync
            For Each name As String In hits.Keys
                Dim h As Double = hits(name)
                Dim t As Double = If(totals(name) > 0.0, totals(name), 1.0)

                result(name) = New Double() {h, t, h / t}
            Next
        End SyncLock

        Return result
    End Function

    ''' <summary>
    ''' 读取单个护栏的命中比例（未记录时为 0）。
    ''' </summary>
    Public Function HitRate(name As String) As Double
        SyncLock sync
            Dim h As Double = Nothing
            Dim t As Double = Nothing

            Call hits.TryGetValue(name, h)
            Call totals.TryGetValue(name, t)

            Return If(t > 0.0, h / t, 0.0)
        End SyncLock
    End Function

End Module
