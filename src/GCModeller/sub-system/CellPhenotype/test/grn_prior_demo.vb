' ============================================================
' grn_prior_demo.vb
'
' 基因表达调控先验网络构建（ExpressionGRNBuilder）的演示 / 冒烟入口：
'   表达矩阵 + TF 注释 +（可选）STRING links
'       -> 按模块拆分的 PriorNetwork 集合（RegulatoryEdge）
'
' 运行方式（切换启动对象）：
'   dotnet run --project test.vbproj -p:StartupObject=test.grn_prior_demo
' ============================================================

Imports SMRUCC.genomics.Analysis.CellPhenotype.RegulationNetwork
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Module grn_prior_demo

    Sub Main()
        ' 数据文件路径（按实际存放位置调整）
        Dim exprFile As String = "K:\hsa\Homo_sapiens_expr_advanced_all_conditions.csv"
        Dim tfFile As String = "K:\hsa_grn\Homo_sapiens_TF.txt"

        ' 可选：STRING 9606.protein.links 文件路径；为空则不启用 STRING 证据
        Dim stringLinks As String = Nothing

        Dim args As String() = Environment.GetCommandLineArgs()

        If args.Length >= 2 Then
            stringLinks = args(1)
        End If

        Call $"load expression matrix from '{exprFile}'...".info

        Dim data As Matrix = Matrix.LoadData(exprFile, tqdm_wrap:=True)
        Dim TFlist As String() = ExpressionGRNBuilder.ReadTfList(tfFile, "Ensembl", tsv:=True)

        ' 冒烟运行使用较小的规模参数；正式分析请改回默认值（直接 New GRNBuildOptions 即可）
        Dim options As New GRNBuildOptions With {
            .filterTopN = 2000,
            .maxModuleGenes = 300,
            .maxDpiGenes = 200,
            .maxCrossModulePairs = 10,
            .maxCrossModuleTargets = 100,
            .stringLinks = stringLinks
        }

        Dim result = ExpressionGRNBuilder.Build(data, TFlist, options)

        ' ① 各模块统计
        Call Console.WriteLine()
        Call Console.WriteLine("=== module statistics ===")

        For Each m In result.modules
            Call Console.WriteLine(m.statistics.ToString)
        Next

        ' ② 汇总
        Call Console.WriteLine()
        Call Console.WriteLine("=== summary ===")
        Call Console.WriteLine(result.summary.ToString)

        ' ③ Evidence 标签分布
        Call Console.WriteLine()
        Call Console.WriteLine("=== evidence tag distribution ===")

        Dim tags = result.allEdges _
            .SelectMany(Function(e) If(e.Evidence Is Nothing, {}, e.Evidence.Split("+"c))) _
            .Where(Function(t) Not String.IsNullOrEmpty(t)) _
            .GroupBy(Function(t) t) _
            .OrderByDescending(Function(g) g.Count)

        For Each tag In tags
            Call Console.WriteLine($"{tag.Key}: {tag.Count}")
        Next

        ' ④ 合并为单一先验网络（供 DBN / GNN 使用）
        Dim merged = result.ToPriorNetwork()

        Call Console.WriteLine()
        Call Console.WriteLine($"merged PriorNetwork: {merged.Edges.Count} edges, {merged.TFNames.Count} TF, {merged.TargetNames.Count} targets")

        ' ⑤ 高置信度子网（供 bnlearn 结构学习的白名单）
        Dim strict = result.ToPriorNetwork(0.6)
        Dim whitelist = strict.ToWhitelist(data.expression.Select(Function(r) r.geneID).ToArray).Count

        Call Console.WriteLine($"strict PriorNetwork (conf >= 0.6): {strict.Edges.Count} edges, whitelist pairs = {whitelist}")

        Pause("press <ENTER> to exit...")
    End Sub
End Module
