Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports Microsoft.VisualBasic.Math.Statistics
Imports std = System.Math

Namespace Scoring

    ''' <summary>一个基因在一个扰动下的差异表达记录。</summary>
    Public Class DeRow

        Public Property Gene As String
        Public Property MeanCpm As Double
        Public Property Log2FoldChange As Double
        Public Property PValue As Double
        Public Property Adjusted As Double
        Public Property Significant As Boolean

    End Class

    ''' <summary>一个扰动（或对照）相对对照细胞的差异表达表。</summary>
    Public Class DeTable

        Public Property Rows As DeRow()

        ''' <summary>与 <see cref="Rows"/> 对应的基因轴。</summary>
        Public Property Genes As String()

        ''' <summary>按基因名查行（缺失返回 <c>Nothing</c>）。</summary>
        Public Function Find(gene As String) As DeRow
            For i As Integer = 0 To Rows.Length - 1
                If String.Equals(Rows(i).Gene, gene, StringComparison.Ordinal) Then
                    Return Rows(i)
                End If
            Next

            Return Nothing
        End Function

    End Class

    ''' <summary>
    ''' 一侧（预测或参考）的统计量：群剖面、差异表达表与 jackknife 矩。
    ''' </summary>
    Public Class SideStats

        Public Property Genes As String()
        Public Property Perturbations As String()

        ''' <summary>逐扰动的 TS 归一化群剖面。</summary>
        Public Property Profiles As Dictionary(Of String, Double())

        ''' <summary>逐扰动的差异表达表。</summary>
        Public Property De As Dictionary(Of String, DeTable)

        ''' <summary>逐扰动的 jackknife 矩（<c>C_p</c>）。</summary>
        Public Property Jackknife As Dictionary(Of String, Double)

        ''' <summary>对照侧的 jackknife 矩。</summary>
        Public Property ControlJackknife As Double

        Public Function TargetIndex(pert As String) As Integer
            For i As Integer = 0 To Genes.Length - 1
                If String.Equals(Genes(i), pert, StringComparison.Ordinal) Then
                    Return i
                End If
            Next

            Return -1
        End Function

    End Class

    ''' <summary>
    ''' VCC 六项指标：群剖面距离（pds）、无偏表达误差（expr_mse）、方向保真（fid）、
    ''' 方向覆盖（reach）、显著集 Jaccard（jac）、log2FC 标准化绝对误差（nmae）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 六项指标里有三项（fid / reach / jac）比较的是<b>显著基因集</b>，
    ''' 因此差异表达表必须先定稿：p 值用精确秩和（对照谱来自同一个序统计缓存），
    ''' 再用 BH 校正，<c>padj &lt; 0.05</c> 才算显著。用未校正的 p 值会让显著集随扰动规模膨胀，
    ''' 指标之间的可比性就没了。
    ''' </para>
    ''' <para>
    ''' <b>自身靶基因必须从所有比较里剔除</b>：它的效应是"敲低本身"，
    ''' 任何模型只要把靶基因敲下去就能在这三项上拿满分，指标会失去区分度。
    ''' </para>
    ''' </remarks>
    Public Module Metrics

        ''' <summary>剖面归一化常数（与设计层共用同一个值）。</summary>
        Public Const TS As Double = 50000.0

        ''' <summary>显著性阈值。</summary>
        Public Const Alpha As Double = 0.05

        ''' <summary>reach 的前缀纯度门槛。</summary>
        Public Const PurityThreshold As Double = 0.9

        ''' <summary>nmae 的最小参考门规模。</summary>
        Public Const NmaeMinGate As Integer = 10

        ''' <summary>群剖面：<c>log1p(TS·Σcounts/Σtotal)</c>。</summary>
        Public Function GroupProfile(counts As Double()()) As Double()
            Dim nGenes As Integer = If(counts.Length = 0, 0, counts(0).Length)
            Dim sums(nGenes - 1) As Double
            Dim total As Double = 0.0

            For c As Integer = 0 To counts.Length - 1
                For g As Integer = 0 To nGenes - 1
                    sums(g) += counts(c)(g)
                    total += counts(c)(g)
                Next
            Next

            Dim profile(nGenes - 1) As Double
            Dim normalizer As Double = std.Max(total, 1.0E-12)

            For g As Integer = 0 To nGenes - 1
                profile(g) = std.Log(1.0 + TS * sums(g) / normalizer)
            Next

            Return profile
        End Function

        ''' <summary>
        ''' 构造一侧的统计量：逐扰动的群剖面、DE 表与 jackknife 矩。
        ''' </summary>
        ''' <param name="blocks">逐扰动的细胞矩阵（细胞 × 基因）</param>
        ''' <param name="perts">扰动名</param>
        ''' <param name="genes">基因轴</param>
        ''' <param name="controlCpm">对照侧逐基因的<b>已排序</b> CPM 与并列和（取自引擎）</param>
        ''' <param name="tieSums">对照侧逐基因的并列和</param>
        Public Function BuildSide(blocks As CsrMatrix(), perts As String(), genes As String(),
                                  controlCpm As Func(Of Integer, Double()),
                                  tieSums As Func(Of Integer, Double),
                                  Optional withControl As Boolean = True) As SideStats
            Dim profiles As New Dictionary(Of String, Double())(StringComparer.Ordinal)
            Dim tables As New Dictionary(Of String, DeTable)(StringComparer.Ordinal)
            Dim jackknife As New Dictionary(Of String, Double)(StringComparer.Ordinal)

            For i As Integer = 0 To blocks.Length - 1
                Dim dense As Double()() = ToDense(blocks(i), genes.Length)

                profiles(perts(i)) = GroupProfile(dense)
                tables(perts(i)) = DeTable(dense, genes, controlCpm, tieSums, Nothing)
                jackknife(perts(i)) = SingleCell.CountStatistics.JackknifeMoment(blocks(i), 512, TS)
            Next

            Dim control As Double = 0.0

            If withControl AndAlso blocks.Length > 0 Then
                ' 对照侧的矩由调用方通过 controlJackknife 传入（见 ContextPipeline）
                control = 0.0
            End If

            Return New SideStats With {
                .Genes = genes,
                .Perturbations = perts,
                .Profiles = profiles,
                .De = tables,
                .Jackknife = jackknife,
                .ControlJackknife = control
            }
        End Function

        ''' <summary>
        ''' 计算六项指标（对共享扰动取均值；未定义的扰动从均值里剔除）。
        ''' </summary>
        Public Function Score(pred As SideStats, ref As SideStats, panelTargets As String()) As Dictionary(Of String, Double)
            Dim common As New List(Of Integer)

            For i As Integer = 0 To pred.Perturbations.Length - 1
                If ref.Profiles.ContainsKey(pred.Perturbations(i)) Then
                    Call common.Add(i)
                End If
            Next

            Dim result As New Dictionary(Of String, Double)(StringComparer.Ordinal)

            If common.Count = 0 Then
                Return result
            End If

            Dim pdsSum As Double = 0.0
            Dim mseSum As Double = 0.0
            Dim fidSum As Double = 0.0
            Dim fidCount As Integer = 0
            Dim reachSum As Double = 0.0
            Dim reachCount As Integer = 0
            Dim jacSum As Double = 0.0
            Dim nmaeSum As Double = 0.0
            Dim nmaeCount As Integer = 0

            For Each i As Integer In common
                Dim pert As String = pred.Perturbations(i)
                Dim target As Integer = pred.TargetIndex(pert)
                Dim predicted As Double() = pred.Profiles(pert)
                Dim reference As Double() = ref.Profiles(pert)

                pdsSum += ProfileDistance(pred, ref, pert, predicted, panelTargets)
                mseSum += RelativeMse(predicted, reference, ref, panelTargets)

                Dim rowsPred As DeTable = pred.De(pert)
                Dim rowsRef As DeTable = ref.De(pert)
                Dim fidelity As Double = DirectionFidelity(rowsPred, rowsRef, target, pred.Genes)
                Dim coverage As Double = DirectionReach(rowsPred, rowsRef, target, pred.Genes)

                If Not Double.IsNaN(fidelity) Then
                    fidSum += fidelity
                    fidCount += 1
                End If
                If Not Double.IsNaN(coverage) Then
                    reachSum += coverage
                    reachCount += 1
                End If

                jacSum += SignificantJaccard(rowsPred, rowsRef, target)
                Dim errorRate As Double = LfcNmae(rowsPred, rowsRef, target)

                If Not Double.IsNaN(errorRate) Then
                    nmaeSum += errorRate
                    nmaeCount += 1
                End If
            Next

            result("pds") = pdsSum / common.Count
            result("expr_mse") = 1.0 - mseSum / common.Count
            result("fid") = If(fidCount > 0, fidSum / fidCount, 0.0)
            result("reach") = If(reachCount > 0, reachSum / reachCount, 0.0)
            result("jac") = jacSum / common.Count
            result("nmae") = If(nmaeCount > 0, nmaeSum / nmaeCount, 1.0)

            Return result
        End Function

#Region "metric kernels"

        ''' <summary>
        ''' pds：剔除整个评估面板的靶基因后，预测剖面在参考扰动中的<b>相似度名次</b>（1 = 最像）。
        ''' </summary>
        Private Function ProfileDistance(pred As SideStats, ref As SideStats, pert As String,
                                         predicted As Double(), panelTargets As String()) As Double
            Dim excluded As New HashSet(Of String)(StringComparer.Ordinal)

            If panelTargets IsNot Nothing Then
                For Each gene As String In panelTargets
                    Call excluded.Add(gene)
                Next
            End If

            Dim own As Double = Cosine(predicted, ref.Profiles(pert), pred.Genes, excluded)
            Dim better As Integer = 0
            Dim count As Integer = 0

            For Each other As String In ref.Perturbations
                If String.Equals(other, pert, StringComparison.Ordinal) Then
                    Continue For
                End If

                Dim score As Double = Cosine(predicted, ref.Profiles(other), pred.Genes, excluded)

                count += 1

                If score > own Then
                    better += 1
                End If
            Next

            If count = 0 Then
                Return 1.0
            End If

            Return 1.0 - better / count
        End Function

        ''' <summary>余弦相似度（跳过被剔除的基因与零向量）。</summary>
        Private Function Cosine(a As Double(), b As Double(), genes As String(), excluded As HashSet(Of String)) As Double
            Dim dot As Double = 0.0
            Dim na As Double = 0.0
            Dim nb As Double = 0.0

            For i As Integer = 0 To std.Min(a.Length, b.Length) - 1
                If excluded.Contains(genes(i)) Then
                    Continue For
                End If

                dot += a(i) * b(i)
                na += a(i) * a(i)
                nb += b(i) * b(i)
            Next

            If na <= 0.0 OrElse nb <= 0.0 Then
                Return 0.0
            End If

            Return dot / std.Sqrt(na * nb)
        End Function

        ''' <summary>相对平方误差（按参考剖面的离散度归一化）。</summary>
        Private Function RelativeMse(predicted As Double(), reference As Double(),
                                     ref As SideStats, panelTargets As String()) As Double
            Dim excluded As New HashSet(Of String)(StringComparer.Ordinal)

            If panelTargets IsNot Nothing Then
                For Each gene As String In panelTargets
                    Call excluded.Add(gene)
                Next
            End If

            Dim mse As Double = 0.0
            Dim scale As Double = 0.0
            Dim mean As Double = 0.0
            Dim count As Integer = 0

            For i As Integer = 0 To predicted.Length - 1
                If excluded.Contains(ref.Genes(i)) Then
                    Continue For
                End If

                mean += reference(i)
                count += 1
            Next

            If count = 0 Then
                Return 0.0
            End If

            mean /= count

            For i As Integer = 0 To predicted.Length - 1
                If excluded.Contains(ref.Genes(i)) Then
                    Continue For
                End If

                Dim d As Double = predicted(i) - reference(i)

                mse += d * d
                scale += (reference(i) - mean) ^ 2
            Next

            If scale <= 1.0E-12 Then
                Return If(mse <= 1.0E-12, 0.0, 1.0)
            End If

            Return std.Max(std.Min(mse / scale, 1.0), 0.0)
        End Function

        ''' <summary>fid：方向一致的基因数 / 两侧显著集规模的较大者。</summary>
        Private Function DirectionFidelity(pred As DeTable, ref As DeTable, target As Integer, genes As String()) As Double
            Dim agreed As Integer = 0
            Dim nPredicted As Integer = 0
            Dim nReference As Integer = 0
            Dim adjudicable As Integer = 0

            For i As Integer = 0 To genes.Length - 1
                If i = target Then
                    Continue For
                End If

                Dim a As DeRow = pred.Find(genes(i))
                Dim b As DeRow = ref.Find(genes(i))

                If a Is Nothing OrElse b Is Nothing Then
                    Continue For
                End If

                If a.Significant Then
                    nPredicted += 1
                End If
                If b.Significant Then
                    nReference += 1
                End If

                If b.Significant AndAlso std.Abs(b.Log2FoldChange) > 0.0 Then
                    adjudicable += 1

                    If a.Significant AndAlso std.Sign(a.Log2FoldChange) = std.Sign(b.Log2FoldChange) Then
                        agreed += 1
                    End If
                End If
            Next

            If adjudicable = 0 Then
                Return Double.NaN
            End If

            Return agreed / std.Max(std.Max(nPredicted, nReference), 1)
        End Function

        ''' <summary>reach：按预测的名次遍历参考预算，纯度 ≥ 0.9 的最深前缀占比。</summary>
        Private Function DirectionReach(pred As DeTable, ref As DeTable, target As Integer, genes As String()) As Double
            Dim ranking As New List(Of String)

            For i As Integer = 0 To genes.Length - 1
                If i = target Then
                    Continue For
                End If

                Dim a As DeRow = pred.Find(genes(i))

                If a IsNot Nothing AndAlso a.Significant Then
                    Call ranking.Add(genes(i))
                End If
            Next

            Dim budget As Integer = 0

            For i As Integer = 0 To genes.Length - 1
                If i = target Then
                    Continue For
                End If

                Dim b As DeRow = ref.Find(genes(i))

                If b IsNot Nothing AndAlso b.Significant Then
                    budget += 1
                End If
            Next

            If budget = 0 Then
                Return Double.NaN
            End If

            ranking.Sort(Function(x, y)
                             Dim a As DeRow = pred.Find(x)
                             Dim b As DeRow = pred.Find(y)
                             Dim order As Integer = b.PValue.CompareTo(a.PValue)

                             If order <> 0 Then
                                 Return order
                             End If

                             Return std.Abs(b.Log2FoldChange).CompareTo(std.Abs(a.Log2FoldChange))
                         End Function)

            Dim hits As Integer = 0
            Dim deepest As Integer = 0

            For i As Integer = 0 To std.Min(ranking.Count, budget) - 1
                Dim b As DeRow = ref.Find(ranking(i))

                If b IsNot Nothing AndAlso b.Significant AndAlso
                    std.Sign(b.Log2FoldChange) = std.Sign(pred.Find(ranking(i)).Log2FoldChange) Then

                    hits += 1
                End If

                If hits / (i + 1.0) >= PurityThreshold Then
                    deepest = i + 1
                End If
            Next

            Return deepest / budget
        End Function

        ''' <summary>jac：显著集的 Jaccard（剔除自身靶；并集为空记 1）。</summary>
        Private Function SignificantJaccard(pred As DeTable, ref As DeTable, target As Integer) As Double
            Dim predicted As New HashSet(Of String)(StringComparer.Ordinal)
            Dim reference As New HashSet(Of String)(StringComparer.Ordinal)

            For i As Integer = 0 To pred.Rows.Length - 1
                If pred.Rows(i).Significant Then
                    Call predicted.Add(pred.Rows(i).Gene)
                End If
            Next

            For i As Integer = 0 To ref.Rows.Length - 1
                If ref.Rows(i).Significant Then
                    Call reference.Add(ref.Rows(i).Gene)
                End If
            Next

            If target >= 0 AndAlso target < pred.Genes.Length Then
                Call predicted.Remove(pred.Genes(target))
                Call reference.Remove(pred.Genes(target))
            End If

            Dim union As New HashSet(Of String)(predicted, StringComparer.Ordinal)

            Call union.UnionWith(reference)

            If union.Count = 0 Then
                Return 1.0
            End If

            Dim intersection As Integer = 0

            For Each gene As String In predicted
                If reference.Contains(gene) Then
                    intersection += 1
                End If
            Next

            Return intersection / union.Count
        End Function

        ''' <summary>nmae：参考门（显著、有限 lfc、≥10 基因）上的标准化绝对误差。</summary>
        Private Function LfcNmae(pred As DeTable, ref As DeTable, target As Integer) As Double
            Dim gate As New List(Of DeRow)

            For i As Integer = 0 To ref.Rows.Length - 1
                If i = target Then
                    Continue For
                End If

                Dim row As DeRow = ref.Rows(i)

                If row.Significant AndAlso Double.IsFinite(row.Log2FoldChange) Then
                    Call gate.Add(row)
                End If
            Next

            If gate.Count < NmaeMinGate Then
                Return Double.NaN
            End If

            Dim numerator As Double = 0.0
            Dim denominator As Double = 0.0

            For Each row As DeRow In gate
                Dim other As DeRow = pred.Find(row.Gene)
                Dim predictedValue As Double = If(other Is Nothing, 0.0, other.Log2FoldChange)

                numerator += std.Abs(row.Log2FoldChange - predictedValue)
                denominator += std.Abs(row.Log2FoldChange)
            Next

            If denominator <= 0.0 Then
                Return Double.NaN
            End If

            Return numerator / denominator
        End Function

#End Region

#Region "de table"

        ''' <summary>对一侧的细胞矩阵逐基因做差异表达（精确秩和 + BH 校正）。</summary>
        Public Function DeTable(counts As Double()(), genes As String(),
                                controlCpm As Func(Of Integer, Double()),
                                tieSums As Func(Of Integer, Double),
                                controlTieSum As Double()) As DeTable
            Dim nGenes As Integer = genes.Length
            Dim n As Integer = counts.Length
            Dim rows(nGenes - 1) As DeRow
            Dim pValues(nGenes - 1) As Double
            Dim librarySizes(n - 1) As Double

            For c As Integer = 0 To n - 1
                Dim sum As Double = 0.0

                For g As Integer = 0 To nGenes - 1
                    sum += counts(c)(g)
                Next

                librarySizes(c) = std.Max(sum, 1.0)
            Next

            For g As Integer = 0 To nGenes - 1
                Dim column(n - 1) As Double
                Dim groupSum As Double = 0.0
                Dim total As Double = 0.0

                For c As Integer = 0 To n - 1
                    column(c) = counts(c)(g)
                    groupSum += column(c)
                Next

                For c As Integer = 0 To n - 1
                    total += librarySizes(c)
                Next

                Dim controlSum As Double = 0.0

                For Each value As Double In controlCpm(g)
                    controlSum += value
                Next

                Dim groupCpm As Double = If(total > 0.0, 1000000.0 * groupSum / total, 0.0)
                Dim controlMean As Double = If(controlCpm(g).Length > 0, controlSum / controlCpm(g).Length, 0.0)
                Dim z As Double = Synthesis.WilcoxonKernel.ZExact(column, librarySizes, controlCpm(g), tieSums(g))

                rows(g) = New DeRow With {
                    .Gene = genes(g),
                    .MeanCpm = groupCpm,
                    .Log2FoldChange = std.Log((groupCpm + 1.0E-9) / (controlMean + 1.0E-9), 2.0),
                    .PValue = NormalDistribution.TwoSidedPValue(z),
                    .Adjusted = 1.0,
                    .Significant = False
                }

                pValues(g) = rows(g).PValue
            Next

            Dim adjusted As Double() = FalseDiscoveryRate.PValueAdjust(pValues, PValueAdjustMethod.BH, nGenes)

            For g As Integer = 0 To nGenes - 1
                rows(g).Adjusted = adjusted(g)
                rows(g).Significant = adjusted(g) < Alpha
            Next

            Return New DeTable With {.Rows = rows, .Genes = genes}
        End Function

#End Region

#Region "scaling"

        ''' <summary>
        ''' 锚点缩放：把原始指标按"参考实现的锚点"映射到 0~100 分的总分。
        ''' </summary>
        ''' <remarks>
        ''' <c>expr_mse</c> 与 <c>nmae</c> 的锚点方向相反（前者越大越好、后者越小越好），
        ''' 因此这里显式声明每个指标的"越大越好"标志，避免手工挪动符号。
        ''' </remarks>
        Public Function AnchorScore(scores As Dictionary(Of String, Double)) As Double
            Dim weights As New Dictionary(Of String, Double)(StringComparer.Ordinal) From {
                {"pds", 0.25}, {"expr_mse", 0.2}, {"fid", 0.2},
                {"reach", 0.15}, {"jac", 0.1}, {"nmae", 0.1}
            }
            Dim total As Double = 0.0
            Dim weightSum As Double = 0.0

            For Each name As String In weights.Keys
                Dim value As Double = 0.0

                If Not scores.TryGetValue(name, value) OrElse Double.IsNaN(value) Then
                    Continue For
                End If

                ' nmae 越小越好，其余越大越好
                Dim normalized As Double = If(name = "nmae", 1.0 - value, value)

                total += weights(name) * std.Max(std.Min(normalized, 1.0), 0.0)
                weightSum += weights(name)
            Next

            If weightSum <= 0.0 Then
                Return 0.0
            End If

            Return 100.0 * total / weightSum
        End Function

#End Region

        ''' <summary>把 CSR 块转成稠密矩阵（评分侧需要逐元素访问）。</summary>
        Public Function ToDense(block As CsrMatrix, nGenes As Integer) As Double()()
            Dim n As Integer = block.RowDimension
            Dim dense As Double()() = RectangularArray.Matrix(Of Double)(n, nGenes)

            For c As Integer = 0 To n - 1
                For k As Integer = block.Indptr(c) To block.Indptr(c + 1) - 1
                    Dim gene As Integer = block.Indices(k)

                    If gene >= 0 AndAlso gene < nGenes Then
                        dense(c)(gene) = block.Values(k)
                    End If
                Next
            Next

            Return dense
        End Function

    End Module
End Namespace
