Imports System.Globalization
Imports System.IO
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports HTSMatrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix

Namespace Demo

    ''' <summary>
    ''' Limma 差异分析结果。
    ''' </summary>
    Public Class DemoLimmaResult

        ''' <summary>完整 topTable（按 adj.P.Val 升序）</summary>
        Public Property Table As LimmaTable()

        ''' <summary>上调基因子集</summary>
        Public Property Up As List(Of LimmaTable)

        ''' <summary>下调基因子集</summary>
        Public Property Down As List(Of LimmaTable)

        ''' <summary>筛选阈值说明（含回退提示）</summary>
        Public Property ThresholdNote As String

        ''' <summary>参与检验的基因数</summary>
        Public Property NGenes As Integer

        ''' <summary>每组样本数</summary>
        Public Property NSamplesPerGroup As Integer

    End Class

    ''' <summary>
    ''' 把虚拟扰动结果转换为 Limma 可消费的表达矩阵并执行差异分析。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <b>尺度约定</b>：GPS 的对照/扰动预测位于 log2 量级，而 <c>Limma.LmFit</c>
    ''' 内部会对输入矩阵再取一次 <c>log(2)</c>。因此这里先用 <c>2^x</c> 还原成线性尺度，
    ''' Limma 的 logFC 才会回到 log2 单位（每个单位 = 2 倍变化）。
    ''' </para>
    ''' <para>
    ''' <b>in-silico 重复</b>：虚拟扰动是确定性点估计，而 Limma 需要 <c>n − 2</c> 的残差自由度。
    ''' 这里为对照组与处理组各生成 <c>nSamples</c> 个样本，噪声加在 log2 空间
    ''' （<c>σ = noiseRatio × 实测同时间点重复间 SD</c>），再逐样本线性化。
    ''' 这些重复只用于给出方差估计，不是实测生物学重复，报告中必须显式标注。
    ''' </para>
    ''' <para>
    ''' <b>分组顺序</b>：<c>DataAnalysis.experiment = designs(0)</c>、<c>control = designs(1)</c>，
    ''' 因此处理组样本必须排在对照组之前，否则 logFC 符号整体翻转。
    ''' </para>
    ''' </remarks>
    Public Module DemoLimma

        ''' <summary>线性化时允许的最大指数（防止 2^x 溢出为 Infinity）。</summary>
        Private Const MaxExponent As Double = 1024.0

        ''' <summary>
        ''' 执行 Limma 差异分析。
        ''' </summary>
        ''' <param name="geneNames">基因名（与对照/扰动向量顺序一致）</param>
        ''' <param name="replicateSD">
        ''' 每个基因的测量噪声（log2 尺度，由实测同时间点重复间 SD 估计）
        ''' </param>
        ''' <param name="control">对照预测（log2 尺度）</param>
        ''' <param name="perturbed">扰动预测（log2 尺度）</param>
        ''' <param name="nSamples">每组样本数（≥2）</param>
        ''' <param name="noiseRatio">噪声强度（测量噪声的倍数）</param>
        ''' <param name="seed">随机种子</param>
        ''' <param name="logFCCutoff">logFC 阈值</param>
        ''' <param name="fdrCutoff">校正 P 值阈值</param>
        ''' <param name="topN">阈值不满足时的回退数量</param>
        ''' <returns>差异分析结果</returns>
        Public Function Run(geneNames As String(),
                            replicateSD As Double(),
                            control As Double(),
                            perturbed As Double(),
                            Optional nSamples As Integer = 6,
                            Optional noiseRatio As Double = 0.05,
                            Optional seed As Integer = 2024,
                            Optional logFCCutoff As Double = 0.5,
                            Optional fdrCutoff As Double = 0.05,
                            Optional topN As Integer = 30) As DemoLimmaResult

            If geneNames Is Nothing OrElse geneNames.Length = 0 Then
                Throw New ArgumentException("基因名不能为空", NameOf(geneNames))
            End If

            Dim n As Integer = geneNames.Length

            If control.Length <> n OrElse perturbed.Length <> n Then
                Throw New ArgumentException("对照/扰动向量的长度必须与基因数一致")
            End If

            If nSamples < 2 Then nSamples = 2

            Dim matrix As HTSMatrix = BuildMatrix(geneNames, replicateSD, control, perturbed,
                                                  nSamples, noiseRatio, seed)

            Dim samples As New List(Of SampleInfo)()

            ' 处理组在前，对照组在后（DataAnalysis 的组顺序约定）
            For k As Integer = 1 To nSamples
                Call samples.Add(New SampleInfo With {
                    .ID = $"KO_{k}",
                    .sample_name = $"KO_{k}",
                    .sample_info = "knockout"
                })
            Next

            For k As Integer = 1 To nSamples
                Call samples.Add(New SampleInfo With {
                    .ID = $"CTRL_{k}",
                    .sample_name = $"CTRL_{k}",
                    .sample_info = "control"
                })
            Next

            Dim design As New DataAnalysis(samples)
            Dim table As LimmaTable() = matrix.LmFit(design).ToArray()
            Dim selected As (Up As List(Of LimmaTable), Down As List(Of LimmaTable), Note As String) =
                SelectSignificant(table, logFCCutoff, fdrCutoff, topN)

            Return New DemoLimmaResult With {
                .Table = table,
                .Up = selected.Up,
                .Down = selected.Down,
                .ThresholdNote = selected.Note,
                .NGenes = n,
                .NSamplesPerGroup = nSamples
            }
        End Function

        ''' <summary>
        ''' 构造 Limma 输入矩阵：处理组列在前、对照组列在后，数值为线性尺度。
        ''' </summary>
        ''' <param name="geneNames">基因名</param>
        ''' <param name="replicateSD">每个基因的测量噪声（log2 尺度）</param>
        ''' <param name="control">对照预测（log2 尺度）</param>
        ''' <param name="perturbed">扰动预测（log2 尺度）</param>
        ''' <param name="nSamples">每组样本数</param>
        ''' <param name="noiseRatio">噪声强度（测量噪声的倍数）</param>
        ''' <param name="seed">随机种子</param>
        ''' <returns>HTS_matrix 表达矩阵</returns>
        Public Function BuildMatrix(geneNames As String(),
                                    replicateSD As Double(),
                                    control As Double(),
                                    perturbed As Double(),
                                    nSamples As Integer,
                                    noiseRatio As Double,
                                    seed As Integer) As HTSMatrix

            Dim n As Integer = geneNames.Length
            Dim total As Integer = nSamples * 2
            Dim sampleIDs As String() = New String(total - 1) {}

            For k As Integer = 0 To nSamples - 1
                sampleIDs(k) = $"KO_{k + 1}"
                sampleIDs(nSamples + k) = $"CTRL_{k + 1}"
            Next

            Dim random As New Random(seed)
            Dim rows As DataFrameRow() = New DataFrameRow(n - 1) {}

            For i As Integer = 0 To n - 1
                Dim values As Double() = New Double(total - 1) {}
                Dim sigma As Double = System.Math.Max(0.0, noiseRatio) *
                                      System.Math.Max(1.0E-04, If(replicateSD Is Nothing, 0.1, replicateSD(i)))

                For k As Integer = 0 To nSamples - 1
                    ' 处理组：扰动预测 + 对数正态噪声
                    values(k) = Linearize(perturbed(i) + Gaussian(random) * sigma)
                    ' 对照组：对照预测 + 同尺度噪声
                    values(nSamples + k) = Linearize(control(i) + Gaussian(random) * sigma)
                Next

                rows(i) = New DataFrameRow With {
                    .geneID = geneNames(i),
                    .experiments = values
                }
            Next

            Return New HTSMatrix With {
                .tag = "GPS virtual knockout",
                .sampleID = sampleIDs,
                .expression = rows
            }
        End Function

        ''' <summary>
        ''' 按 |logFC| 与校正 P 值筛选上下调基因；命中过少时回退为显著度最高的 Top-N。
        ''' </summary>
        ''' <param name="table">完整 topTable</param>
        ''' <param name="logFCCutoff">logFC 阈值</param>
        ''' <param name="fdrCutoff">校正 P 值阈值</param>
        ''' <param name="topN">回退数量</param>
        ''' <returns>上调集、下调集与筛选说明</returns>
        Public Function SelectSignificant(table As IEnumerable(Of LimmaTable),
                                          logFCCutoff As Double,
                                          fdrCutoff As Double,
                                          topN As Integer) As (Up As List(Of LimmaTable), Down As List(Of LimmaTable), Note As String)

            Dim all As LimmaTable() = If(table, Enumerable.Empty(Of LimmaTable)()).ToArray()
            Dim up As List(Of LimmaTable) = all.
                Where(Function(g) g.logFC >= logFCCutoff AndAlso g.adj_P_Val <= fdrCutoff).
                OrderByDescending(Function(g) g.logFC).
                ToList()
            Dim down As List(Of LimmaTable) = all.
                Where(Function(g) g.logFC <= -logFCCutoff AndAlso g.adj_P_Val <= fdrCutoff).
                OrderBy(Function(g) g.logFC).
                ToList()

            Dim note As String = $"阈值 |logFC| ≥ {logFCCutoff:G2} 且 adj.P ≤ {fdrCutoff:G2}"

            If up.Count + down.Count >= 5 Then
                Return (up, down, note)
            End If

            ' ---- 回退：按显著性取 Top-N，保证后续富集有输入 ----
            Dim fallbackUp As List(Of LimmaTable) = all.
                Where(Function(g) g.logFC > 0).
                OrderBy(Function(g) g.adj_P_Val).
                Take(topN).
                ToList()
            Dim fallbackDown As List(Of LimmaTable) = all.
                Where(Function(g) g.logFC < 0).
                OrderBy(Function(g) g.adj_P_Val).
                Take(topN).
                ToList()

            note &= $"（命中过少，已回退为上调/下调各取显著性 Top-{topN}）"

            Return (fallbackUp, fallbackDown, note)
        End Function

        ''' <summary>
        ''' 把 Limma topTable 导出为 TSV。
        ''' </summary>
        ''' <param name="file">输出路径</param>
        ''' <param name="rows">表格行</param>
        Public Sub SaveTable(outputFile As String, rows As IEnumerable(Of LimmaTable))
            Dim builder As New Text.StringBuilder()

            Call builder.AppendLine("id	logFC	AveExpr	t	P.Value	adj.P.Val	B")

            For Each row As LimmaTable In If(rows, Enumerable.Empty(Of LimmaTable)())
                Call builder.AppendLine(String.Join(vbTab,
                    row.id,
                    row.logFC.ToString("G6", CultureInfo.InvariantCulture),
                    row.AveExpr.ToString("G6", CultureInfo.InvariantCulture),
                    row.t.ToString("G6", CultureInfo.InvariantCulture),
                    row.P_Value.ToString("G6", CultureInfo.InvariantCulture),
                    row.adj_P_Val.ToString("G6", CultureInfo.InvariantCulture),
                    row.B.ToString("G6", CultureInfo.InvariantCulture)))
            Next

            Dim directory As String = System.IO.Path.GetDirectoryName(outputFile)

            If Not String.IsNullOrEmpty(directory) AndAlso Not System.IO.Directory.Exists(directory) Then
                Call System.IO.Directory.CreateDirectory(directory)
            End If

            Call System.IO.File.WriteAllText(outputFile, builder.ToString())
        End Sub

        ''' <summary>把一个 log2 尺度的值线性化，并做溢出夹断。</summary>
        ''' <param name="logValue">log2 值</param>
        ''' <returns>线性尺度值（恒正）</returns>
        Private Function Linearize(logValue As Double) As Double
            Dim exponent As Double = System.Math.Max(-MaxExponent, System.Math.Min(MaxExponent, logValue))
            Dim value As Double = System.Math.Pow(2.0, exponent)

            If Double.IsNaN(value) OrElse Double.IsInfinity(value) OrElse value <= 0.0 Then
                Return 1.0
            End If

            Return value
        End Function

        ''' <summary>标准正态随机数（Box-Muller）。</summary>
        ''' <param name="random">随机源</param>
        ''' <returns>N(0,1) 样本</returns>
        Private Function Gaussian(random As Random) As Double
            Dim u1 As Double = 1.0 - random.NextDouble()
            Dim u2 As Double = random.NextDouble()

            Return System.Math.Sqrt(-2.0 * System.Math.Log(u1)) * System.Math.Cos(2.0 * System.Math.PI * u2)
        End Function

    End Module

End Namespace
