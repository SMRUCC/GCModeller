Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

''' <summary>
''' 分位数归一化（quantile normalization）
''' </summary>
''' <remarks>
''' <para>
''' 这个模块之前是**空的**（只有一个 <c>Public Module QuantileNormalization</c> 声明），
''' 但项目的包描述里一直声称提供 quantile normalization，调用方会得到一个编译期可见、
''' 运行期什么也不做的假 API。这里补上实现。
''' </para>
''' <para>
''' 算法与 R 端 <c>limma::normalizeBetweenArrays(method="quantile")</c>（即
''' <c>limma::normalizeQuantiles(ties=TRUE)</c>）逐行对应：
''' </para>
''' <list type="number">
''' <item>把每一列（样本）单独排序，得到该列的排序序列 <c>S[,j]</c>；</item>
''' <item>参考分布 <c>m[k] = 各列第 k 个值的均值</c>；</item>
''' <item>对每个值取其在该列之中的秩 <c>r</c>（并列取平均秩），
''' 再在参考分布的网格上按 <c>(r-1)/(n-1)</c> 做线性插值，写回原位。</item>
''' </list>
''' <para>
''' 并列值走"平均秩 + 插值"这条路径（<c>ties=TRUE</c>），而不是把并列值统统映射到同一个
''' 参考值上；这是 limma 的默认行为，也是与 <c>preprocessCore::normalize.quantiles</c>
''' 输出一致的版本。
''' </para>
''' </remarks>
Public Module QuantileNormalization

    ''' <summary>
    ''' 对表达矩阵做分位数归一化
    ''' </summary>
    ''' <param name="expr">表达矩阵（基因在行，样本在列）</param>
    ''' <returns>归一化之后的矩阵（行列标签与顺序保持不变）</returns>
    <Extension>
    Public Function QuantileNormalize(expr As Matrix) As Matrix
        If expr Is Nothing OrElse expr.expression Is Nothing Then
            Return expr
        End If

        Dim nGene As Integer = expr.size
        Dim nSample As Integer = expr.sample_count

        If nGene = 0 OrElse nSample < 2 Then
            Return New Matrix With {
                .tag = $"quantile({expr.tag})",
                .sampleID = expr.sampleID,
                .expression = expr.expression
            }
        End If

        ' 第一步：每一列单独排序
        Dim sorted As Double()() = New Double(nSample - 1)() {}
        Dim ranks As Double()() = New Double(nSample - 1)() {}

        For j As Integer = 0 To nSample - 1
            Dim column As Double() = New Double(nGene - 1) {}

            For i As Integer = 0 To nGene - 1
                column(i) = expr.expression(i).experiments(j)
            Next

            sorted(j) = column.SortedAscending
            ranks(j) = RankStatistics.AverageRanks(column)
        Next

        ' 第二步：参考分布 = 各列排序值在同一个名次上的均值
        Dim reference As Double() = New Double(nGene - 1) {}

        For k As Integer = 0 To nGene - 1
            Dim sum As Double = 0

            For j As Integer = 0 To nSample - 1
                sum += sorted(j)(k)
            Next

            reference(k) = sum / nSample
        Next

        ' 第三步：按平均秩在参考分布上插值，写回原位
        Dim rows As DataFrameRow() = New DataFrameRow(nGene - 1) {}

        For i As Integer = 0 To nGene - 1
            Dim values As Double() = New Double(nSample - 1) {}

            For j As Integer = 0 To nSample - 1
                values(j) = interpolate(reference, ranks(j)(i) - 1.0)
            Next

            rows(i) = New DataFrameRow With {
                .geneID = expr.expression(i).geneID,
                .experiments = values
            }
        Next

        Return New Matrix With {
            .tag = $"quantile({expr.tag})",
            .sampleID = expr.sampleID,
            .expression = rows
        }
    End Function

    ''' <summary>
    ''' 在等间距网格上做线性插值（对应 R 的 <c>approx(grid, y, index)</c>）
    ''' </summary>
    ''' <param name="reference">网格节点上的取值</param>
    ''' <param name="index">网格坐标（从 0 开始，可以是非整数）</param>
    Private Function interpolate(reference As Double(), index As Double) As Double
        Dim n As Integer = reference.Length

        If index <= 0 Then
            Return reference(0)
        ElseIf index >= n - 1 Then
            Return reference(n - 1)
        End If

        Dim lo As Integer = CInt(std.Floor(index))
        Dim frac As Double = index - lo

        If frac = 0 Then
            Return reference(lo)
        End If

        Return reference(lo) * (1.0 - frac) + reference(lo + 1) * frac
    End Function

End Module
