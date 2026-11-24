' grn_store_demo.vb — CorrelationMatrixStore 存储驱动路径端到端演示
' 运行: dotnet run --project test.vbproj -p:StartupObject=test.grn_store_demo

Imports Microsoft.VisualBasic.Math.Matrix
Imports SMRUCC.genomics.Analysis.CellPhenotype.RegulationNetwork
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.WGCNA

Module grn_store_demo

    Const ExprFile As String = "K:\hsa\Homo_sapiens_expr_advanced_all_conditions.csv"
    Const TfFile As String = "K:\hsa_grn\Homo_sapiens_TF.txt"
    Const StoreFile As String = "K:\hsa_grn\corstore_demo.corstore"

    Sub Main()
        Dim sw As Stopwatch = Stopwatch.StartNew()
        Dim data As Matrix = Matrix.LoadData(ExprFile, tqdm_wrap:=True)
        Dim TFlist As String() = ExpressionGRNBuilder.ReadTfList(TfFile, "Ensembl", tsv:=True)

        ' ① 高方差子集 + 批次标准化（全量 5 万基因流程与此一致，仅子集规模不同）
        Dim normalized As Matrix = BatchNormalizer.Normalize(data, Nothing, centerBatch:=True)
        Dim subset As Matrix = GeneFilter.ByVariance(normalized, topN:=2000)
        Dim geneIds As String() = subset.expression.Select(Function(r) r.geneID).ToArray

        Call $"[1] {geneIds.Length} genes selected for the correlation store".info

        ' ② 逐行 bicor 计算（模拟耗时的一次性矩阵计算），边算边写 corstore
        Using writer As New CorrelationMatrixWriter(StoreFile, geneIds, subset.sample_count,
                                                    CorrelationEncodings.Float32)
            For i As Integer = 0 To geneIds.Length - 1
                Dim row(geneIds.Length - 1) As Single
                Dim vi As Double() = subset.expression(i).experiments

                For j As Integer = 0 To geneIds.Length - 1
                    row(j) = If(j = i, 1.0F, CSng(Bicor.BiweightMidcorrelation(vi, subset.expression(j).experiments)))
                Next

                Call writer.WriteRow(geneIds(i), row)
            Next

            Call writer.Complete()
        End Using

        Call $"[2] correlation store written: {New IO.FileInfo(StoreFile).Length / 1048576.0:F1} MB in {sw.ElapsedMilliseconds}ms".info

        ' ③ WGCNA 模块划分（GPU blockwise）
        Dim config As New WGCNAConfig With {.useGpu = True, .buildGraph = False}
        Dim wgcna As Result = Analysis.RunBlockwise(subset, config)

        Call $"[3] WGCNA modules: {wgcna.modules.Count}".info

        ' ④ 存储驱动构建先验网络（跳过 bicor 重算；提供表达矩阵以启用偏相关筛选）
        Dim options As New GRNBuildOptions With {.minAbsCorrelation = 0.3, .enableGpu = True}

        sw.Restart()

        Using store = CorrelationMatrixStore.Open(StoreFile)
            Dim result = ExpressionGRNBuilder.Build(store, TFlist, wgcna.modules, options, expr:=subset)

            Call $"[4] store-driven GRN built in {sw.ElapsedMilliseconds}ms".info
            Call Console.WriteLine(result.summary.ToString)
        End Using

        ' ⑤ 快速阈值过滤实验：不再做任何矩阵计算，直接查询存储
        Call RefilterDemo(geneIds)

        Call Console.WriteLine("=== store demo finished, press ENTER to exit ===")
        Call Console.ReadLine()
    End Sub

    ''' <summary>
    ''' ⑤ 阈值过滤实验：不做任何矩阵计算，直接基于存储做点查 / 邻域 / 全库流式筛选
    ''' </summary>
    Private Sub RefilterDemo(geneIds As String())
        Using store = CorrelationMatrixStore.Open(StoreFile)
            ' 点查：(gene1, gene2) → cor + p
            Dim p0 = store.GetCorrelation(geneIds(0), geneIds(1))
            Call $"point query ({geneIds(0)}, {geneIds(1)}): cor={p0.cor:F4}, p={p0.pvalue:E2}".info

            ' 邻域查询
            Dim sw As Stopwatch = Stopwatch.StartNew()
            Dim nb = store.Neighbors(geneIds(0), 0.4)
            Call $"[5] Neighbors('{geneIds(0)}', 0.4): {nb.Length} hits in {sw.ElapsedMilliseconds}ms".info

            ' 不同阈值的全库流式过滤（阈值实验的核心：每次过滤都是分钟级以内的纯 IO）
            For Each th As Double In {0.3, 0.5, 0.7}
                sw.Restart()
                Dim n As Integer = store.StreamEdges(th).Count
                Call $"[6] StreamEdges(|cor|>={th}): {n} edges in {sw.ElapsedMilliseconds}ms".info
            Next
        End Using
    End Sub

End Module
