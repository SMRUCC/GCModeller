' ============================================================================
' ResultWriter.vb — 结果导出（统一 TSV 风格 + UTF-8）
'
' readme 六的路线图要求每个阶段都有可验证的中间产物，本模块负责把它们落盘：
'   · 训练轨迹 U_seq          ← 检查"是否平滑、有无剧烈跳变"（P1 验证点）
'   · 脉冲栅格 / 发放率       ← 检查"脉冲传播路径是否合理、有无全体静默/持续发放"（P2 验证点）
'   · 先验边与学习后的权重     ← 检查"学到的调控关系是否贴合已知 TF-Target"（P3/P5 验证点）
'   · 训练历史与评估指标       ← 损失曲线、PCC/AUROC 等
'   · 虚拟扰动轨迹与汇总       ← 下游效应基因排序（P4 验证点）
'
' 全部使用 Tab 分隔（与 BNLearn 的既有导出风格一致），数值统一按不变量文化编码，
' 保证在不同区域设置下产出完全相同的文件内容。
' ============================================================================

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.DeepLearning.SpikingNeuralNetwork
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow

Namespace IO

    ''' <summary>结果表导出（TSV / UTF-8）</summary>
    Public Module ResultWriter

#Region "通用表"

        ''' <summary>写入一张文本表：可选的标签列 + 表头 + 数据行</summary>
        ''' <param name="path">输出路径</param>
        ''' <param name="header">表头列名</param>
        ''' <param name="rows">数据行（每行字段数应与表头一致）</param>
        ''' <param name="rowLabels">可选的行标签（作为第一列，表头需自带该项的列名）</param>
        Public Sub WriteTable(path As String, header As String(), rows As IEnumerable(Of String()),
                              Optional rowLabels As String() = Nothing)
            Dim sb As New StringBuilder()
            sb.AppendLine(String.Join(ControlChars.Tab, header))

            Dim i = 0
            For Each row In rows
                If rowLabels IsNot Nothing Then
                    sb.Append(If(i < rowLabels.Length, rowLabels(i), ""))
                    If row IsNot Nothing AndAlso row.Length > 0 Then sb.Append(ControlChars.Tab)
                End If

                sb.AppendLine(String.Join(ControlChars.Tab, row))
                i += 1
            Next

            WriteFile(path, sb.ToString())
        End Sub

        ''' <summary>写入一张数值表（数值按不变量文化格式化为 G8）</summary>
        Public Sub WriteNumericTable(path As String, header As String(),
                                     rows As IEnumerable(Of Double()),
                                     Optional rowLabels As String() = Nothing)
            WriteTable(path, header,
                       rows.Select(Function(r) r.Select(Function(v) FormatNumber(v)).ToArray()),
                       rowLabels)
        End Sub

        ''' <summary>写入"键 → 值"清单（用于指标报告、配置留档）</summary>
        Public Sub WriteKeyValues(path As String, title As String,
                                  items As IEnumerable(Of (name As String, value As String)))
            Dim sb As New StringBuilder()
            sb.AppendLine($"# {title}")

            For Each item In items
                sb.AppendLine($"{item.name}{ControlChars.Tab}{item.value}")
            Next

            WriteFile(path, sb.ToString())
        End Sub

#End Region

#Region "训练轨迹"

        ''' <summary>
        ''' 导出离散化后的表达轨迹 U_seq：行 = 伪时间窗，列 = 基因。
        ''' </summary>
        Public Sub WriteExpressionTrajectory(path As String, result As Data.PseudotimeResult)
            If result Is Nothing Then Return

            Dim header As String() = New String() {"pseudotime_bin"} _
                .Concat(result.GeneNames).ToArray()

            WriteNumericTable(path, header, result.Values, TimeLabels(result.NumBins))
        End Sub

        Private Function TimeLabels(bins As Integer) As String()
            Dim labels(bins - 1) As String
            For t = 0 To bins - 1
                labels(t) = $"t{t}"
            Next
            Return labels
        End Function

#End Region

#Region "脉冲轨迹"

        ''' <summary>
        ''' 导出脉冲栅格：行 = 基因（神经元），列 = 时间步，值为 0/1。
        ''' </summary>
        ''' <param name="path">输出路径</param>
        ''' <param name="sHistory">脉冲轨迹 S[t]</param>
        ''' <param name="geneNames">基因名（长度应等于神经元数）</param>
        ''' <param name="sampleIndex">批量中的样本下标</param>
        Public Sub WriteSpikeRaster(path As String, sHistory As List(Of Tensor),
                                   geneNames As String(), Optional sampleIndex As Integer = 0)
            If sHistory Is Nothing OrElse sHistory.Count = 0 Then Return

            Dim units = sHistory(0).Shape(1)
            Dim header As String() = {"gene"}.Concat(TimeLabels(sHistory.Count)).ToArray()
            Dim rows As New List(Of Double())()

            For j = 0 To units - 1
                Dim row(sHistory.Count - 1) As Double
                For t = 0 To sHistory.Count - 1
                    row(t) = sHistory(t).Data(sampleIndex * units + j)
                Next
                rows.Add(row)
            Next

            WriteNumericTable(path, header, rows, LabelFor(geneNames, units))
        End Sub

        ''' <summary>导出发放率：行 = 基因，列 = 0(计数) 与 1(平均发放率)</summary>
        Public Sub WriteFiringRate(path As String, sHistory As List(Of Tensor), geneNames As String())
            If sHistory Is Nothing OrElse sHistory.Count = 0 Then Return

            Dim counts = SpikeDecoders.SpikeCounts(sHistory)
            Dim rates = SpikeDecoders.FiringRate(sHistory)
            Dim units = counts.Shape(1)

            Dim rows As New List(Of Double())()
            For j = 0 To units - 1
                rows.Add(New Double() {counts.Data(j), rates.Data(j)})
            Next

            WriteNumericTable(path, {"gene", "spike_count", "firing_rate"}, rows, LabelFor(geneNames, units))
        End Sub

#End Region

#Region "先验图与权重"

        ''' <summary>
        ''' 导出先验边表（pre, post, weight, confidence, trainable），便于人工核对拓扑与方向。
        ''' </summary>
        Public Sub WritePriorEdges(path As String, graph As Graph.PriorGraph)
            If graph Is Nothing Then Return

            Dim rows As New List(Of Double())()
            Dim labels As New List(Of String)()

            For Each e In graph.EdgeList()
                rows.Add(New Double() {e.pre, e.post, e.weight, e.confidence, graph.Mask.Data(e.pre * graph.Units + e.post)})
                labels.Add($"{graph.GeneNames(e.pre)}→{graph.GeneNames(e.post)}")
            Next

            WriteNumericTable(path,
                              {"pre_idx", "post_idx", "prior_weight", "confidence", "trainable"},
                              rows, labels.ToArray())
        End Sub

        ''' <summary>
        ''' 导出"先验权重 vs 学习后权重"的逐边对比，用于观察权重漂移幅度。
        ''' </summary>
        ''' <param name="learnedWeight">训练后的权重矩阵 [N, N]</param>
        Public Sub WriteWeightComparison(path As String, graph As Graph.PriorGraph, learnedWeight As Tensor)
            If graph Is Nothing OrElse learnedWeight Is Nothing Then Return

            Dim rows As New List(Of Double())()
            Dim labels As New List(Of String)()
            Dim wd = learnedWeight.Data
            Dim pd = graph.WInit.Data
            Dim md = graph.Mask.Data

            For Each e In graph.EdgeList()
                Dim idx = e.pre * graph.Units + e.post
                rows.Add(New Double() {e.pre, e.post, pd(idx), wd(idx), wd(idx) - pd(idx), md(idx)})
                labels.Add($"{graph.GeneNames(e.pre)}→{graph.GeneNames(e.post)}")
            Next

            WriteNumericTable(path,
                              {"pre_idx", "post_idx", "prior_weight", "learned_weight", "delta", "trainable"},
                              rows, labels.ToArray())
        End Sub

#End Region

#Region "内部工具"

        Private Function LabelFor(geneNames As String(), units As Integer) As String()
            If geneNames Is Nothing OrElse geneNames.Length = 0 Then Return Nothing

            Dim labels(units - 1) As String
            For j = 0 To units - 1
                labels(j) = If(j < geneNames.Length, geneNames(j), $"neuron{j}")
            Next
            Return labels
        End Function

        ''' <summary>数值格式化：不变量文化的 G8（足够精度 + 紧凑体积）</summary>
        Private Function FormatNumber(v As Double) As String
            If Double.IsNaN(v) Then Return "NaN"
            If Double.IsPositiveInfinity(v) Then Return "Inf"
            If Double.IsNegativeInfinity(v) Then Return "-Inf"
            Return v.ToString("G8", Globalization.CultureInfo.InvariantCulture)
        End Function

        Private Sub WriteFile(path As String, content As String)
            ' 注意：VB 大小写不敏感，此处必须用 System.IO.Path 全名，否则 'Path' 会被解析为上面的参数 path(String)
            Dim dir = System.IO.Path.GetDirectoryName(path)
            If Not String.IsNullOrEmpty(dir) AndAlso Not Directory.Exists(dir) Then
                Directory.CreateDirectory(dir)
            End If
            File.WriteAllText(path, content, Encoding.UTF8)
        End Sub

#End Region

    End Module

End Namespace
