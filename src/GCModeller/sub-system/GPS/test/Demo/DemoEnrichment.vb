Imports System.Globalization
Imports System.IO
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Statistics
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports EnrichBackground = SMRUCC.genomics.Analysis.HTS.GSEA.Background

Namespace Demo

    ''' <summary>
    ''' 通路富集结果（含 FDR 校正与显著子集）。
    ''' </summary>
    Public Class DemoEnrichmentResult

        ''' <summary>本次富集的输入基因（已去重、去空）</summary>
        Public Property InputGenes As String()

        ''' <summary>全部通路结果（按 P 值升序，已补 FDR）</summary>
        Public Property All As EnrichmentResult()

        ''' <summary>显著富集的通路（FDR 阈值；为空时回退 Top-N）</summary>
        Public Property Significant As EnrichmentResult()

        ''' <summary>显著性判据说明</summary>
        Public Property Note As String

    End Class

    ''' <summary>
    ''' Fisher 精确检验通路富集（含 Benjamini-Hochberg FDR 校正）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 传入 <c>cutSize:=0</c> 保证所有通路都参与检验（默认值 3 会剔除成员数 ≤3 的通路）；
    ''' <c>outputAll:=True</c> 让无交集的通路也返回（便于确认"确实没有富集"而不是"被过滤掉了"）；
    ''' <c>showProgress:=False</c> 避免控制台被 Tqdm 进度条打断。
    ''' </para>
    ''' <para>
    ''' 背景规模 <see cref="Background.size"/> 由调用方显式给出（建模基因总数），
    ''' 它决定超几何检验的 N；若交由库回退为"通路成员并集大小"会系统性低估 P 值。
    ''' </para>
    ''' </remarks>
    Public Module DemoEnrichment

        ''' <summary>
        ''' 对一组基因执行通路富集。
        ''' </summary>
        ''' <param name="background">通路背景模型</param>
        ''' <param name="genes">目标基因列表（差异基因）</param>
        ''' <param name="fdrCutoff">FDR 显著阈值</param>
        ''' <param name="topN">无显著结果时回退展示的数量</param>
        ''' <returns>富集结果</returns>
        Public Function Run(background As EnrichBackground,
                            genes As IEnumerable(Of String),
                            Optional fdrCutoff As Double = 0.05,
                            Optional topN As Integer = 5) As DemoEnrichmentResult

            If background Is Nothing Then Throw New ArgumentNullException(NameOf(background))

            Dim inputGenes As String() = If(genes, Enumerable.Empty(Of String)()).
                Where(Function(g) Not String.IsNullOrWhiteSpace(g)).
                Select(Function(g) g.Trim()).
                Distinct(StringComparer.OrdinalIgnoreCase).
                ToArray()

            If inputGenes.Length = 0 Then
                Return New DemoEnrichmentResult With {
                    .InputGenes = inputGenes,
                    .All = New EnrichmentResult() {},
                    .Significant = New EnrichmentResult() {},
                    .Note = "输入基因列表为空"
                }
            End If

            Dim raw As EnrichmentResult() = background.Enrichment(
                inputGenes,
                resize:=-1,
                cutSize:=0,
                outputAll:=True,
                isLocustag:=False,
                showProgress:=False).ToArray()

            ' BH 校正（扩展方法要求元素实现 IStatFDR，EnrichmentResult 已实现）
            Dim ranked As EnrichmentResult() = raw.
                OrderBy(Function(r) r.pvalue).
                ToArray().
                FDR().
                OrderBy(Function(r) r.pvalue).
                ThenBy(Function(r) r.FDR).
                ToArray()

            Dim significant As EnrichmentResult() = ranked.
                Where(Function(r) Not Double.IsNaN(r.FDR) AndAlso r.FDR <= fdrCutoff).
                ToArray()

            Dim note As String = $"FDR ≤ {fdrCutoff:G2}（BH 校正），背景规模 N = {background.size}"

            If significant.Length = 0 Then
                significant = ranked.Take(topN).ToArray()
                note &= $"（无通路通过阈值，已回退展示 P 值最小的 Top-{topN}）"
            End If

            Return New DemoEnrichmentResult With {
                .InputGenes = inputGenes,
                .All = ranked,
                .Significant = significant,
                .Note = note
            }
        End Function

        ''' <summary>
        ''' 把富集结果导出为 TSV。
        ''' </summary>
        ''' <param name="file">输出路径</param>
        ''' <param name="rows">富集结果</param>
        Public Sub SaveTable(outputFile As String, rows As IEnumerable(Of EnrichmentResult))
            Dim builder As New Text.StringBuilder()

            Call builder.AppendLine("term	name	score	enriched	cluster	pvalue	FDR	IDs")

            For Each row As EnrichmentResult In If(rows, Enumerable.Empty(Of EnrichmentResult)())
                Call builder.AppendLine(String.Join(vbTab,
                    row.term,
                    row.name,
                    row.score.ToString("G6", CultureInfo.InvariantCulture),
                    row.enriched.ToString(CultureInfo.InvariantCulture),
                    row.cluster.ToString(CultureInfo.InvariantCulture),
                    row.pvalue.ToString("G6", CultureInfo.InvariantCulture),
                    row.FDR.ToString("G6", CultureInfo.InvariantCulture),
                    String.Join(";", If(row.IDs, New String() {}))))
            Next

            Dim directory As String = System.IO.Path.GetDirectoryName(outputFile)

            If Not String.IsNullOrEmpty(directory) AndAlso Not System.IO.Directory.Exists(directory) Then
                Call System.IO.Directory.CreateDirectory(directory)
            End If

            Call System.IO.File.WriteAllText(outputFile, builder.ToString())
        End Sub

        ''' <summary>
        ''' 生成控制台摘要文本。
        ''' </summary>
        ''' <param name="result">富集结果</param>
        ''' <param name="topN">展示条数</param>
        ''' <returns>多行文本</returns>
        Public Function Format(result As DemoEnrichmentResult, Optional topN As Integer = 8) As String
            Dim builder As New Text.StringBuilder()

            Call builder.AppendLine($"  输入基因 {result.InputGenes.Length} 个；{result.Note}")

            For Each row As EnrichmentResult In result.Significant.Take(topN)
                Call builder.AppendLine(
                    $"  {row.term,-5} {Shorten(row.name, 34),-36} " &
                    $"score={row.score:F3}  {row.enriched}/{row.cluster}  " &
                    $"P={row.pvalue:G3}  FDR={row.FDR:G3}")
            Next

            Return builder.ToString()
        End Function

        ''' <summary>截断过长的通路名，保持控制台对齐。</summary>
        ''' <param name="text">原文</param>
        ''' <param name="maxLength">最大字符数</param>
        ''' <returns>截断后的文本</returns>
        Private Function Shorten(text As String, maxLength As Integer) As String
            If String.IsNullOrEmpty(text) Then Return ""
            If text.Length <= maxLength Then Return text

            Return text.Substring(0, maxLength - 1) & "…"
        End Function

    End Module

End Namespace
