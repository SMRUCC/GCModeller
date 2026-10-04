' ============================================================
' grn_prior_demo.vb
'
' 基因表达调控先验网络构建（ExpressionGRNBuilder）的演示 / 冒烟入口：
'   表达矩阵 + TF 注释 + STRING-db（K:\hsa_grn\string-db）
'       -> 按模块拆分的 PriorNetwork 集合（RegulatoryEdge）
'
' 本演示启用 CUDA GPU 加速并处理完整矩阵（不做方差截断）。
'
' 运行方式（切换启动对象）：
'   dotnet run --project test.vbproj -p:StartupObject=test.grn_prior_demo
' ============================================================

Imports SMRUCC.genomics.Analysis.CellPhenotype.RegulationNetwork
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Module grn_prior_demo

    ' 数据文件路径（按实际存放位置调整）
    Const ExprFile As String = "K:\hsa\Homo_sapiens_expr_advanced_all_conditions.csv"
    Const TfFile As String = "K:\hsa_grn\Homo_sapiens_TF.txt"
    Const StringDbFolder As String = "K:\hsa_grn\string-db"

    Sub Main()
        Call $"load expression matrix from '{ExprFile}'...".info

        Dim data As Matrix = Matrix.LoadData(ExprFile, tqdm_wrap:=True)
        Dim TFlist As String() = ExpressionGRNBuilder.ReadTfList(TfFile, "Ensembl", tsv:=True)

        ' 自动发现 STRING 数据文件（优先精简版 links + aliases 别名表）
        Dim str As (links As String, aliases As String) = ExpressionGRNBuilder.FindStringLinks(StringDbFolder)

        If str.links IsNot Nothing Then
            Call $"STRING links   = '{str.links}'".info
        Else
            Call VBDebugger.EchoLine("warning: STRING links not found, protein interaction evidence will be disabled.")
        End If

        If str.aliases IsNot Nothing Then
            Call $"STRING aliases = '{str.aliases}'".info
        End If

        ' GPU 加速 + 完整矩阵（不截断基因数）
        Dim options As New GRNBuildOptions With {
            .enableGpu = True,
            .gpuFp32Gemm = True,
            .filterTopN = 0,
            .maxMissingRate = 0.2,
            .stringLinks = str.links,
            .stringAliases = str.aliases,
            .stringMinScore = 700
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
