Imports std = System.Math

Namespace OrderStatistics

    ''' <summary>
    ''' 由一组观测值生成的分位剖面：可以是"细胞库大小的目标剖面"，也可以是
    ''' "某个基因在目标细胞中的 CPM 剖面"。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 典型用法（目标库大小剖面）：
    ''' </para>
    ''' <code>
    ''' Dim profile = QuantileProfile.Create(controlLibrarySizes, nCells) _
    '''     .RescaleTo(totalDepth) _
    '''     .Spread(0.85) _
    '''     .RescaleTo(totalDepth)
    ''' </code>
    ''' <para>
    ''' 其中 <see cref="Spread"/> 按中位数做幂变换来控制离散度（<c>1</c> = 保持原样，
    ''' <c>&lt; 1</c> = 收拢，<c>&gt; 1</c> = 拉开），<see cref="RescaleTo"/> 保证总和精确等于给定值。
    ''' 两个操作都是纯函数、返回新实例，可以安全地共享。
    ''' </para>
    ''' </remarks>
    Public Class QuantileProfile

        Private ReadOnly m_values As Double()

        Private Sub New(values As Double())
            m_values = values
        End Sub

        ''' <summary>
        ''' 从观测值生成 <c>nCells</c> 个分层分位点：<c>p_k = (k + 0.5) / nCells</c>，
        ''' 按 R 的 <c>type = 7</c> 线性插值取值。
        ''' </summary>
        ''' <param name="values">观测值（顺序任意，内部会排序）</param>
        ''' <param name="nCells">目标分位点个数</param>
        ''' <returns></returns>
        Public Shared Function Create(values As Double(), nCells As Integer) As QuantileProfile
            If values Is Nothing Then Throw New ArgumentNullException(NameOf(values))
            If nCells < 0 Then Throw New ArgumentOutOfRangeException(NameOf(nCells), nCells, "the quantile count must not be negative")
            If values.Length = 0 OrElse nCells = 0 Then Return New QuantileProfile(New Double() {})

            Dim sorted As Double() = DirectCast(values.Clone(), Double())

            Call Array.Sort(sorted)

            Return FromSorted(sorted, nCells)
        End Function

        ''' <summary>
        ''' 从已排序的观测值生成分层分位点
        ''' </summary>
        ''' <param name="sorted">升序观测值</param>
        ''' <param name="nCells">目标分位点个数</param>
        ''' <returns></returns>
        Public Shared Function FromSorted(sorted As Double(), nCells As Integer) As QuantileProfile
            If sorted Is Nothing Then Throw New ArgumentNullException(NameOf(sorted))
            If nCells < 0 Then Throw New ArgumentOutOfRangeException(NameOf(nCells), nCells, "the quantile count must not be negative")
            If sorted.Length = 0 OrElse nCells = 0 Then Return New QuantileProfile(New Double() {})

            Dim out As Double() = New Double(nCells - 1) {}

            For k As Integer = 0 To nCells - 1
                out(k) = Quantiles.Type7(sorted, (k + 0.5) / nCells)
            Next

            Return New QuantileProfile(out)
        End Function

        ''' <summary>
        ''' 直接以给定数值构造（不做任何检查或重排）
        ''' </summary>
        ''' <param name="values"></param>
        ''' <returns></returns>
        Public Shared Function FromValues(values As Double()) As QuantileProfile
            If values Is Nothing Then Throw New ArgumentNullException(NameOf(values))

            Return New QuantileProfile(DirectCast(values.Clone(), Double()))
        End Function

        ''' <summary>
        ''' 分位数值（只读副本）
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Values As Double()
            Get
                Return m_values
            End Get
        End Property

        ''' <summary>
        ''' 分位点个数
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Count As Integer
            Get
                Return m_values.Length
            End Get
        End Property

        ''' <summary>
        ''' 分位点的均值（空剖面返回 0）
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Mean As Double
            Get
                If m_values.Length = 0 Then Return 0.0

                Dim sum As Double = 0.0

                For i As Integer = 0 To m_values.Length - 1
                    sum += m_values(i)
                Next

                Return sum / m_values.Length
            End Get
        End Property

        ''' <summary>
        ''' 按中位数做幂变换控制离散度：<c>v' = median · (v / median)^exponent</c>
        ''' </summary>
        ''' <param name="exponent">1 = 不变，&lt;1 收拢，&gt;1 拉开</param>
        ''' <returns>新实例；中位数非正时原样返回（无法做幂变换）</returns>
        Public Function Spread(exponent As Double) As QuantileProfile
            If m_values.Length = 0 Then Return New QuantileProfile(New Double() {})

            Dim median As Double = Quantiles.MedianOfSorted(m_values)

            If median <= 0.0 OrElse exponent = 1.0 Then Return New QuantileProfile(DirectCast(m_values.Clone(), Double()))

            Dim out As Double() = New Double(m_values.Length - 1) {}

            For i As Integer = 0 To m_values.Length - 1
                Dim v As Double = m_values(i)

                If v <= 0.0 Then
                    out(i) = 0.0
                Else
                    out(i) = median * std.Pow(v / median, exponent)
                End If
            Next

            Return New QuantileProfile(out)
        End Function

        ''' <summary>
        ''' 线性缩放到指定总和
        ''' </summary>
        ''' <param name="total">目标总和</param>
        ''' <returns>新实例；当前总和非正时原样返回</returns>
        Public Function RescaleTo(total As Double) As QuantileProfile
            Dim sum As Double = 0.0

            For i As Integer = 0 To m_values.Length - 1
                sum += m_values(i)
            Next

            If sum <= 0.0 Then Return New QuantileProfile(DirectCast(m_values.Clone(), Double()))

            Dim scale As Double = total / sum
            Dim out As Double() = New Double(m_values.Length - 1) {}

            For i As Integer = 0 To m_values.Length - 1
                out(i) = m_values(i) * scale
            Next

            Return New QuantileProfile(out)
        End Function

        ''' <summary>
        ''' 转为总和精确等于 <paramref name="total"/> 的非负整数（最大余数法）
        ''' </summary>
        ''' <param name="total">目标总和</param>
        ''' <returns>长度与 <see cref="Count"/> 一致的整数数组</returns>
        ''' <remarks>
        ''' 用于"把分位剖面变成整数占位"的最后一步：浮点剖面负责形状，
        ''' 整数分摊负责守恒。剖面的总和非正时抛出 <see cref="ArgumentException"/>。
        ''' </remarks>
        Public Function AsIntegers(total As Long) As Long()
            Return Apportionment.Apportion.LargestRemainder(m_values, total)
        End Function

        ''' <summary>
        ''' 复制一份新的剖面对象（内部数组独立）
        ''' </summary>
        ''' <returns></returns>
        Public Function Clone() As QuantileProfile
            Return New QuantileProfile(DirectCast(m_values.Clone(), Double()))
        End Function

        Public Overrides Function ToString() As String
            Return $"quantile profile, n={m_values.Length}, mean={Mean:G6}"
        End Function

    End Class
End Namespace
