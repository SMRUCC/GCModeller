Imports System.Runtime.CompilerServices
Imports std = System.Math

Namespace KOBAS

    ''' <summary>
    ''' 一个基因集的富集打分结果。
    ''' </summary>
    Public Class EnrichmentScoreResult

        ''' <summary>
        ''' 富集分数（running sum 与零之间绝对值最大的偏离）。
        ''' </summary>
        ''' <returns></returns>
        Public Property ES As Double

        ''' <summary>
        ''' 富集分数在 running sum 序列之中的位置。
        ''' </summary>
        ''' <returns></returns>
        Public Property ESIndex As Integer

        ''' <summary>
        ''' running sum 序列；当调用方关闭 <c>keepRunningSum</c> 时为 <c>Nothing</c>。
        ''' </summary>
        ''' <returns></returns>
        Public Property RES As Double()

        ''' <summary>
        ''' 该基因集在排序基因列表之中的命中数量。
        ''' </summary>
        ''' <returns></returns>
        Public Property HitCount As Integer

        ''' <summary>
        ''' 命中基因的权重之和（<c>NR</c>）。
        ''' </summary>
        ''' <returns></returns>
        Public Property HitWeight As Double
    End Class

    ''' <summary>
    ''' 加权 running sum 与富集分数计算（KOBAS <c>gsea.ES_all</c> /
    ''' <c>gsea.ES_for_permutation</c> 的 VB.NET 移植）。
    ''' </summary>
    ''' <remarks>
    ''' 与 KOBAS 原实现的差异：
    ''' <list type="bullet">
    ''' <item>KOBAS 中未命中步长写作 <c>1.0 / (gene_num - len(hitm))</c>，其中
    ''' <c>len(hitm)</c> 是**基因集的数量**而非当前基因集的命中数，导致 running sum
    ''' 不能归零、不同基因集之间的 ES 不可比。这里按标准 GSEA 定义修正为
    ''' <c>1 / (N - k)</c>，<c>k</c> 为当前基因集在排序列表之中的命中数。</item>
    ''' <item>KOBAS 使用连续的 <c>if 0 / if 1 / if 2 else</c> 分支，最后一个
    ''' <c>else</c> 会连带覆盖前面的赋值；这里改为单一 <c>Select Case</c>，
    ''' 语义等价但不存在连带分支。</item>
    ''' <item>全部命中基因的权重都为 0（例如加权模式下所有命中基因的排序分数都是 0）
    ''' 时，退化为不加权模式，避免除零。</item>
    ''' </list>
    ''' </remarks>
    Public Module EnrichmentScore

        ''' <summary>
        ''' 计算单个基因在 running sum 之中的权重：<c>|score|^weight</c>，
        ''' 其中 <paramref name="weight"/> 为 0 时恒为 1、为 2 时取平方。
        ''' </summary>
        ''' <param name="score">该基因的排序统计量。</param>
        ''' <param name="weight">权重指数（KOBAS 的 <c>weighted_score_type</c>）。</param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function WeightOf(score As Double, weight As Double) As Double
            If weight = 0 Then
                Return 1
            ElseIf weight = 2 Then
                Return score * score
            Else
                Return std.Pow(std.Abs(score), weight)
            End If
        End Function

        ''' <summary>
        ''' 按给定的排序计算一个基因集的富集分数。
        ''' </summary>
        ''' <param name="scoreInOrder">按排名顺序排列的基因排序统计量。</param>
        ''' <param name="hitInOrder">按排名顺序排列的命中掩膜。</param>
        ''' <param name="weight">权重指数。</param>
        ''' <param name="keepRunningSum">是否保留 running sum 序列。</param>
        ''' <returns>富集分数、位置与 running sum。</returns>
        Public Function Compute(scoreInOrder As Double(),
                                hitInOrder As Boolean(),
                                weight As Double,
                                Optional keepRunningSum As Boolean = True) As EnrichmentScoreResult

            Dim n As Integer = hitInOrder.Length
            Dim hits As Integer = 0

            For j As Integer = 0 To n - 1
                If hitInOrder(j) Then
                    hits += 1
                End If
            Next

            If hits = 0 Then
                Return New EnrichmentScoreResult With {
                    .ES = 0,
                    .ESIndex = 0,
                    .RES = If(keepRunningSum, New Double(n - 1) {}, Nothing),
                    .HitCount = 0,
                    .HitWeight = 0
                }
            End If

            Dim geneWeight As Double() = New Double(n - 1) {}
            Dim hitWeight As Double = 0

            For j As Integer = 0 To n - 1
                If hitInOrder(j) Then
                    geneWeight(j) = WeightOf(scoreInOrder(j), weight)
                    hitWeight += geneWeight(j)
                End If
            Next

            If hitWeight = 0 Then
                ' 全部命中基因的权重都是 0：退化为不加权模式
                For j As Integer = 0 To n - 1
                    If hitInOrder(j) Then
                        geneWeight(j) = 1
                    End If
                Next

                hitWeight = hits
            End If

            Dim missStep As Double = If(n > hits, 1.0 / (n - hits), 0)
            Dim running As Double = 0
            Dim maxDeviation As Double = Double.NegativeInfinity
            Dim minDeviation As Double = Double.PositiveInfinity
            Dim maxIndex As Integer = 0
            Dim minIndex As Integer = 0
            Dim res As Double() = If(keepRunningSum, New Double(n - 1) {}, Nothing)

            For j As Integer = 0 To n - 1
                If hitInOrder(j) Then
                    running += geneWeight(j) / hitWeight
                Else
                    running -= missStep
                End If

                If keepRunningSum Then
                    res(j) = running
                End If

                If running > maxDeviation Then
                    maxDeviation = running
                    maxIndex = j
                End If
                If running < minDeviation Then
                    minDeviation = running
                    minIndex = j
                End If
            Next

            ' KOBAS 使用 abs(max) > abs(min)，相等时取负向偏离
            Dim es As Double
            Dim esIndex As Integer

            If std.Abs(maxDeviation) > std.Abs(minDeviation) Then
                es = maxDeviation
                esIndex = maxIndex
            Else
                es = minDeviation
                esIndex = minIndex
            End If

            Return New EnrichmentScoreResult With {
                .ES = es,
                .ESIndex = esIndex,
                .RES = res,
                .HitCount = hits,
                .HitWeight = hitWeight
            }
        End Function

        ''' <summary>
        ''' 按给定的排序计算一个基因集的富集分数（使用 <see cref="RankedGene"/> 数组）。
        ''' </summary>
        ''' <param name="ranks">按排序统计量降序排列的基因。</param>
        ''' <param name="mask">与 <paramref name="ranks"/> 同序的命中掩膜。</param>
        ''' <param name="weight">权重指数。</param>
        ''' <param name="keepRunningSum">是否保留 running sum 序列。</param>
        ''' <returns></returns>
        <Extension>
        Public Function Compute(ranks As RankedGene(),
                                mask As Boolean(),
                                weight As Double,
                                Optional keepRunningSum As Boolean = True) As EnrichmentScoreResult

            Dim scores As Double() = New Double(ranks.Length - 1) {}

            For j As Integer = 0 To ranks.Length - 1
                scores(j) = ranks(j).Score
            Next

            Return Compute(scores, mask, weight, keepRunningSum)
        End Function

        ''' <summary>
        ''' 批量计算命中矩阵之中所有基因集的富集分数。
        ''' </summary>
        ''' <param name="ranks">按排序统计量降序排列的基因。</param>
        ''' <param name="hitMatrix">基因集命中矩阵（列顺序必须与 <paramref name="ranks"/> 一致）。</param>
        ''' <param name="weight">权重指数。</param>
        ''' <param name="keepRunningSum">是否保留 running sum 序列。</param>
        ''' <returns>与 <see cref="HitMatrix.GeneSets"/> 同序的打分结果。</returns>
        Public Function ComputeAll(ranks As RankedGene(),
                                   hitMatrix As HitMatrix,
                                   weight As Double,
                                   Optional keepRunningSum As Boolean = True) As EnrichmentScoreResult()

            Dim scores As Double() = New Double(ranks.Length - 1) {}

            For j As Integer = 0 To ranks.Length - 1
                scores(j) = ranks(j).Score
            Next

            Dim result As EnrichmentScoreResult() = New EnrichmentScoreResult(hitMatrix.RowCount - 1) {}

            For i As Integer = 0 To hitMatrix.RowCount - 1
                result(i) = Compute(scores, hitMatrix.Mask(i), weight, keepRunningSum)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 提取富集分数所在的「核心富集」基因：<c>ES &gt;= 0</c> 时取 running sum
        ''' 峰值及其之前的命中基因，否则取峰值及其之后的命中基因。
        ''' </summary>
        ''' <param name="ranks">按排序统计量降序排列的基因。</param>
        ''' <param name="mask">与 <paramref name="ranks"/> 同序的命中掩膜。</param>
        ''' <param name="es">富集分数。</param>
        ''' <param name="esIndex">富集分数所在位置。</param>
        ''' <returns>核心富集基因的编号。</returns>
        <Extension>
        Public Function CoreGenes(ranks As RankedGene(), mask As Boolean(), es As Double, esIndex As Integer) As String()
            Dim genes As New List(Of String)

            If es >= 0 Then
                For j As Integer = 0 To esIndex
                    If mask(j) Then
                        genes.Add(ranks(j).GeneID)
                    End If
                Next
            Else
                For j As Integer = esIndex To ranks.Length - 1
                    If mask(j) Then
                        genes.Add(ranks(j).GeneID)
                    End If
                Next
            End If

            Return genes.ToArray
        End Function
    End Module
End Namespace
