Imports Microsoft.VisualBasic.Linq

Namespace KOBAS

    ''' <summary>
    ''' 富集计算所使用的统计检验方法，对应 KOBAS 的 <c>-m/--method</c> 参数。
    ''' </summary>
    Public Enum EnrichmentTestMethod
        ''' <summary>超几何检验（<c>h</c>，与 Fisher 单尾等价，KOBAS 的默认方法）。</summary>
        Hypergeometric
        ''' <summary>Fisher 精确检验（<c>h</c>）。</summary>
        Fisher
        ''' <summary>二项检验（<c>b</c>）。</summary>
        Binomial
        ''' <summary>卡方检验（<c>c</c>，期望频数过小时退回 Fisher）。</summary>
        ChiSquare
        ''' <summary>仅统计频数，不做检验（<c>x</c>）。</summary>
        Frequency
    End Enum

    ''' <summary>
    ''' 一行富集计算结果。
    ''' </summary>
    Public Class EnrichmentRow

        ''' <summary>
        ''' 词条名称（KOBAS 结果表的 <c>Term</c> 列）。
        ''' </summary>
        ''' <returns></returns>
        Public Property Term As String

        ''' <summary>
        ''' 词条所属数据库（KOBAS 结果表的 <c>Database</c> 列）。
        ''' </summary>
        ''' <returns></returns>
        Public Property Database As String

        ''' <summary>
        ''' 词条编号（KOBAS 结果表的 <c>ID</c> 列）。
        ''' </summary>
        ''' <returns></returns>
        Public Property ID As String

        ''' <summary>
        ''' 前景基因列表之中命中该词条的基因数量。
        ''' </summary>
        ''' <returns></returns>
        Public Property InputNumber As Integer

        ''' <summary>
        ''' 背景基因列表之中命中该词条的基因数量。
        ''' </summary>
        ''' <returns></returns>
        Public Property BackgroundNumber As Integer

        ''' <summary>
        ''' 统计检验的 p 值。
        ''' </summary>
        ''' <returns></returns>
        Public Property PValue As Double

        ''' <summary>
        ''' 多重检验校正之后的 p 值。
        ''' </summary>
        ''' <returns></returns>
        Public Property CorrectedPValue As Double

        ''' <summary>
        ''' 前景基因列表之中命中该词条的基因编号。
        ''' </summary>
        ''' <returns></returns>
        Public Property Input As String()

        Public Overrides Function ToString() As String
            Return $"{Term} ({ID}) {InputNumber}/{BackgroundNumber} p={PValue.ToString("G4")}"
        End Function
    End Class

    ''' <summary>
    ''' 基于前景/背景基因列表的两组样本富集检验（KOBAS <c>discover.TwoSampleTest</c>
    ''' 及其四个子类的 VB.NET 移植）。
    ''' </summary>
    ''' <remarks>
    ''' KOBAS 通过 <c>TestResult</c> 以字符串列表的形式输出结果，这里改为强类型的
    ''' <see cref="EnrichmentRow"/>，并交由 <see cref="EnrichmentTable"/> 负责排序与导出。
    ''' 统计检验的口径见 <see cref="ExactTests"/> 的说明。
    ''' </remarks>
    Public Module TwoSampleTest

        ''' <summary>
        ''' 对前景词条分布执行统计检验。
        ''' </summary>
        ''' <param name="foreground">前景基因列表的词条分布。</param>
        ''' <param name="background">背景基因列表的词条分布。</param>
        ''' <param name="method">检验方法，默认 Fisher 精确检验（KOBAS 的 <c>h</c>）。</param>
        ''' <param name="foregroundGeneCount">
        ''' 前景基因总数 <c>n1</c>；小于等于 0 时使用 <see cref="TermDistribution.GeneCount"/>。
        ''' </param>
        ''' <param name="backgroundGeneCount">
        ''' 背景基因总数 <c>n2</c>；小于等于 0 时使用 <see cref="TermDistribution.GeneCount"/>。
        ''' </param>
        ''' <returns>按 p 值升序排列的检验结果。</returns>
        ''' <exception cref="InvalidOperationException">
        ''' 前景基因列表不是背景基因列表的子集，而又选择了需要该前提的检验方法时抛出。
        ''' </exception>
        Public Function Compare(foreground As TermDistribution,
                                background As TermDistribution,
                                Optional method As EnrichmentTestMethod = EnrichmentTestMethod.Fisher,
                                Optional foregroundGeneCount As Integer = -1,
                                Optional backgroundGeneCount As Integer = -1) As EnrichmentRow()

            Dim n1 As Integer = If(foregroundGeneCount > 0, foregroundGeneCount, foreground.GeneCount)
            Dim n2 As Integer = If(backgroundGeneCount > 0, backgroundGeneCount, background.GeneCount)
            Dim rows As New List(Of EnrichmentRow)

            For Each item As KeyValuePair(Of String, TermMembers) In foreground.Terms
                Dim members As TermMembers = item.Value
                Dim backgroundTerm As TermMembers = Nothing

                If Not background.Terms.TryGetValue(item.Key, backgroundTerm) Then
                    Continue For
                End If

                Dim m1 As Integer = members.Size
                Dim m2 As Integer = backgroundTerm.Size
                Dim pval As Double

                If method = EnrichmentTestMethod.Frequency Then
                    pval = Double.NaN
                Else
                    pval = PValue(m1, n1, m2, n2, method)
                End If

                rows.Add(New EnrichmentRow With {
                    .Term = members.Names,
                    .Database = members.Database,
                    .ID = members.ID,
                    .InputNumber = m1,
                    .BackgroundNumber = m2,
                    .PValue = pval,
                    .CorrectedPValue = Double.NaN,
                    .Input = members.Genes.OrderBy(Function(id) id).ToArray
                })
            Next

            Return rows _
                .OrderBy(Function(row) If(Double.IsNaN(row.PValue), Double.MaxValue, row.PValue)) _
                .ToArray
        End Function

        ''' <summary>
        ''' 计算单个词条的 p 值。
        ''' </summary>
        ''' <param name="m1">前景命中数。</param>
        ''' <param name="n1">前景基因总数。</param>
        ''' <param name="m2">背景命中数。</param>
        ''' <param name="n2">背景基因总数。</param>
        ''' <param name="method">检验方法。</param>
        ''' <returns></returns>
        Public Function PValue(m1 As Integer, n1 As Integer, m2 As Integer, n2 As Integer,
                               Optional method As EnrichmentTestMethod = EnrichmentTestMethod.Fisher) As Double

            Select Case method
                Case EnrichmentTestMethod.Hypergeometric
                    Return ExactTests.HypergeometricUpperTail(m1, m2, n2, n1)
                Case EnrichmentTestMethod.Binomial
                    Return ExactTests.BinomialUpperTail(m1, n1, If(n2 = 0, 0, m2 / CDbl(n2)))
                Case EnrichmentTestMethod.ChiSquare
                    Dim table As Integer() = Contingency(m1, n1, m2, n2)

                    Return ExactTests.ImprovedChiSquare(table(0), table(1), table(2), table(3))
                Case Else
                    Dim table As Integer() = Contingency(m1, n1, m2, n2)

                    Return ExactTests.FisherGreater(table(0), table(1), table(2), table(3))
            End Select
        End Function

        ''' <summary>
        ''' 构造标准 2x2 列联表 <c>[a, b, c, d]</c>：
        ''' <c>a</c> 为前景与词条的交集，<c>b</c> 为前景但不属于词条，
        ''' <c>c</c> 为词条但不在前景，<c>d</c> 为两者都不属于。
        ''' </summary>
        ''' <param name="m1">前景命中数。</param>
        ''' <param name="n1">前景基因总数。</param>
        ''' <param name="m2">背景命中数。</param>
        ''' <param name="n2">背景基因总数。</param>
        ''' <returns>四格计数。</returns>
        ''' <exception cref="InvalidOperationException">
        ''' 前景命中数大于背景命中数，或者前景基因数大于背景基因数时抛出
        ''' （对应 KOBAS 的 <c>StatError: Input is not a subset of background.</c>）。
        ''' </exception>
        Public Function Contingency(m1 As Integer, n1 As Integer, m2 As Integer, n2 As Integer) As Integer()
            If m1 > m2 Then
                Throw New InvalidOperationException(
                    "Input is not a subset of background. Try binomial test.")
            End If
            If n1 > n2 Then
                Throw New InvalidOperationException(
                    "The foreground gene list is larger than the background gene list. " &
                    "Please make sure that the foreground is a subset of the background.")
            End If

            Dim a As Integer = m1
            Dim b As Integer = n1 - m1
            Dim c As Integer = m2 - m1
            Dim d As Integer = n2 - m2 - b

            If d < 0 Then
                Throw New InvalidOperationException(
                    "Input is not a subset of background: the contingency table contains a negative cell.")
            End If

            Return New Integer() {a, b, c, d}
        End Function

        ''' <summary>
        ''' 把命令行之中的检验方法名称解析为 <see cref="EnrichmentTestMethod"/>。
        ''' </summary>
        ''' <param name="code">
        ''' KOBAS 的单字母代码：<c>b</c> 二项、<c>c</c> 卡方、<c>h</c> Fisher/超几何、<c>x</c> 频数。
        ''' </param>
        ''' <returns></returns>
        ''' <exception cref="ArgumentException">代码不受支持时抛出。</exception>
        Public Function Parse(code As String) As EnrichmentTestMethod
            Select Case If(code, "").Trim().ToLowerInvariant()
                Case "b", "binom", "binomial"
                    Return EnrichmentTestMethod.Binomial
                Case "c", "chi", "chisq"
                    Return EnrichmentTestMethod.ChiSquare
                Case "h", "fisher", "hyper", "hypergeometric", ""
                    Return EnrichmentTestMethod.Fisher
                Case "x", "freq", "frequency"
                    Return EnrichmentTestMethod.Frequency
                Case Else
                    Throw New ArgumentException($"Unsupported statistical test method: '{code}'.")
            End Select
        End Function
    End Module
End Namespace
