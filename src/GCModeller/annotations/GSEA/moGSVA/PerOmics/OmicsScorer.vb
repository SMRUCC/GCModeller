Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.DataFrame.MultipleOmics
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports GSVAEngine = SMRUCC.genomics.Analysis.HTS.GSVA.GSVA

Namespace PerOmics

    ''' <summary>
    ''' 逐组学层的富集打分器（method.md 步骤2 + 步骤3）。
    ''' </summary>
    ''' <remarks>
    ''' 关键设计：<b>每一层独立调用一次单组学 GSVA</b>，
    ''' 因此核密度估计、秩变换与随机游走全部被限制在该组学层内部，
    ''' 不会出现「不同组学的量纲相互污染」的问题。
    ''' </remarks>
    Public Module OmicsScorer

        ''' <summary>
        ''' 对数据集中的每个组学层独立打分。
        ''' </summary>
        ''' <param name="dataset">已对齐样本轴的多组学数据集</param>
        ''' <param name="geneSets">基因集（通路 → 基因符号）</param>
        ''' <param name="options">运行配置</param>
        Public Function Score(dataset As Omics.MultiOmicsDataset,
                              geneSets As Background,
                              options As MultiOmicsGsvaOptions) As OmicsScoreSet

            If dataset Is Nothing Then
                Throw New ArgumentNullException(NameOf(dataset))
            End If
            If geneSets Is Nothing Then
                Throw New ArgumentNullException(NameOf(geneSets))
            End If
            If options Is Nothing Then
                Throw New ArgumentNullException(NameOf(options))
            End If

            Dim coverage As Dictionary(Of String, Dictionary(Of String, Integer)) = dataset.Coverage(geneSets)
            Dim tables As New List(Of OmicsScoreTable)

            For Each layer As OmicsLayer In dataset.Layers
                Dim table As New OmicsScoreTable With {.Omics = layer.Name}

                For Each pair As KeyValuePair(Of String, Dictionary(Of String, Integer)) In coverage
                    Dim n As Integer

                    If pair.Value.TryGetValue(layer.Name, n) Then
                        table.Coverage(pair.Key) = n
                    End If
                Next

                Dim clock As Stopwatch = Stopwatch.StartNew()

                Try
                    Dim scores As Matrix = GSVAEngine.gsva(
                        layer.Expr,
                        geneSets,
                        method:=options.ScoreMethod,
                        kcdf:=options.Kcdf,
                        min_sz:=options.MinSize,
                        max_sz:=options.MaxSize,
                        alpha:=options.SsGseaAlpha,
                        normalize:=options.SsGseaNormalize,
                        verbose:=options.Verbose)

                    If scores Is Nothing OrElse scores.size = 0 Then
                        table.Available = False
                        table.ErrorMessage = "no gene set could be mapped to the features of this omics layer"
                    Else
                        If layer.Direction < 0 Then
                            ' 统一到"通路活性方向"：甲基化等组学的数值升高代表活性下降
                            For Each row As DataFrameRow In scores.expression
                                For j As Integer = 0 To row.experiments.Length - 1
                                    row.experiments(j) = -row.experiments(j)
                                Next
                            Next
                        End If

                        table.Scores = scores
                    End If
                Catch ex As Exception
                    ' 单层失败不应该中断整个多组学流程（例如该层全部特征恒定时 GSVA 会抛异常）
                    table.Available = False
                    table.ErrorMessage = $"{ex.GetType.Name}: {ex.Message}"

                    If options.Verbose Then
                        Call $"[moGSVA] omics layer '{layer.Name}' failed to be scored: {table.ErrorMessage}".warning
                    End If
                Finally
                    clock.Stop()
                    table.ElapsedMilliseconds = clock.ElapsedMilliseconds
                End Try

                If options.Verbose Then
                    Call $"[moGSVA] {table}".info
                End If

                tables.Add(table)
            Next

            Dim scored As OmicsScoreTable() = tables.ToArray()
            Dim usable As Integer = scored.Where(Function(t) t.Available).Count()

            If usable = 0 Then
                Throw New InvalidOperationException(
                    "none of the omics layers could be scored, please check the feature identifiers of the gene sets: " &
                    String.Join("; ", scored.Select(Function(t) $"{t.Omics}={t.ErrorMessage}")))
            End If

            Return OmicsScoreSet.Create(scored, dataset.SampleID)
        End Function

    End Module

End Namespace
