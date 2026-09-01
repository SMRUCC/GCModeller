Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.BNLearn.DBN

Namespace Core.WGCNADBN

    Public Module BlockDynamics

        <Extension>
        Public Iterator Function TrainBlocks(timeSeries As Core.GeneExpressionData, modules As GeneModuleColor(), TF As String()) As IEnumerable(Of ModuleDBN)
            ' ① 模块划分（跳过 grey 模块，仅保留出现在时间序列中的基因）
            Dim moduleGenes = SplitModules(modules, timeSeries)
            If moduleGenes.Count = 0 Then
                Throw New InvalidOperationException("没有任何 WGCNA 模块基因匹配时间序列，无法构建子网络（请检查基因名体系是否一致）")
            End If
            Call VBDebugger.WriteLine($"GRN.TrainModularDBNIntervene: 解析到 {moduleGenes.Count} 个非灰色模块")

            ' ② 逐模块训练 DynamicBayesianNetwork 子网络
            Dim tfSet As New HashSet(Of String)(TF, StringComparer.OrdinalIgnoreCase)
            Dim moduleDBs As Integer = 0

            For Each kv In moduleGenes
                Dim mcolor = kv.Key
                Dim genes = kv.Value
                If genes.Length < 2 Then
                    Call VBDebugger.WriteLine($"GRN.TrainModularDBNIntervene: 模块 {mcolor} 基因数={genes.Length} < 2，跳过子网络训练")
                    Continue For
                End If


                Call VBDebugger.WriteLine($"GRN.TrainModularDBNIntervene: 模块 {mcolor} 训练完成（基因={genes.Length}, 模块内边={links.Count()}）")
            Next

            If moduleDBs = 0 Then
                Throw New InvalidOperationException("没有任何模块成功训练出子网络，无法执行虚拟扰动")
            End If
        End Function

        Private Function TrainBlock(timeSeries As Core.GeneExpressionData, mcolor As String, genes As String()) As ModuleDBN
            Dim subMatrix = timeSeries.GetSubMatrix(genes)
            If subMatrix Is Nothing Then
                Call VBDebugger.WriteLine($"GRN.TrainModularDBNIntervene: 模块 {mcolor} 无基因匹配时间序列，跳过")
                Return Nothing
            End If

            ' 模块内定向边（两端都属于本模块）转为 RegulatoryLink
            Dim links = BuildModuleRegulatoryLinks(prior, genes)
            Dim net As New DynamicBayesianNetwork()
            net.BuildFromTopology(links)

            Dim ts = ToTimeSeries(subMatrix)
            If ts IsNot Nothing AndAlso ts.Count >= 2 Then
                net.LearnParameters(ts)
            Else
                Call VBDebugger.WriteLine($"GRN.TrainModularDBNIntervene: 模块 {mcolor} 有效时间点不足，仅使用拓扑先验 CPT")
            End If

            Dim eig = ComputeModuleEigengene(ts)
            Dim mdb As New ModuleDBN With {
                .ModuleColor = mcolor,
                .Genes = genes,
                .Net = net,
                .Eigengene = eig
            }
            For i = 0 To genes.Length - 1
                mdb.GeneIndex(genes(i)) = i
            Next
            moduleDBs.Add(mdb)
        End Function

    End Module
End Namespace