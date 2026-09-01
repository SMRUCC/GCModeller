Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.BNLearn.DBN

Namespace Core.WGCNADBN

    Public Module BlockDynamics

        <Extension>
        Public Iterator Function TrainBlocks(timeSeries As Core.GeneExpressionData, modules As IEnumerable(Of GeneModuleColor), prior As Core.PriorNetwork, TF As String()) As IEnumerable(Of ModuleDBN)
            ' ① 模块划分（跳过 grey 模块，仅保留出现在时间序列中的基因）
            Dim moduleGenes = SplitModules(modules, timeSeries)

            If moduleGenes.Count = 0 Then
                Throw New InvalidOperationException("没有任何 WGCNA 模块基因匹配时间序列，无法构建子网络（请检查基因名体系是否一致）")
            Else
                Call $"GRN.TrainModularDBNIntervene: 解析到 {moduleGenes.Count} 个非灰色模块".info
            End If

            ' ② 逐模块训练 DynamicBayesianNetwork 子网络
            Dim tfSet As New HashSet(Of String)(TF, StringComparer.OrdinalIgnoreCase)
            Dim moduleDBs As Integer = 0

            For Each kv As KeyValuePair(Of String, String()) In moduleGenes
                Dim mcolor = kv.Key
                Dim genes = kv.Value

                If genes.Length < 2 Then
                    Call $"GRN.TrainModularDBNIntervene: 模块 {mcolor} 基因数={genes.Length} < 2，跳过子网络训练".debug
                    Continue For
                End If

                Dim [module] = TrainBlock(timeSeries, prior, mcolor, genes)

                If [module] IsNot Nothing Then
                    moduleDBs += 1
                    Yield [module]
                End If

                Call $"GRN.TrainModularDBNIntervene: 模块 {mcolor} 训练完成（基因={genes.Length}, 模块内边={[module].Net.topologySize}）".info
            Next

            If moduleDBs = 0 Then
                Throw New InvalidOperationException("没有任何模块成功训练出子网络，无法执行虚拟扰动")
            End If
        End Function

        ''' <summary>
        ''' 训练某一个WGCNA模块的动态贝叶斯子网络
        ''' </summary>
        ''' <param name="timeSeries"></param>
        ''' <param name="prior"></param>
        ''' <param name="mcolor"></param>
        ''' <param name="genes"></param>
        ''' <returns></returns>
        Private Function TrainBlock(timeSeries As GeneExpressionData, prior As PriorNetwork, mcolor As String, genes As String()) As ModuleDBN
            Dim subMatrix = timeSeries.GetSubMatrix(genes)

            If subMatrix Is Nothing Then
                Call $"GRN.TrainModularDBNIntervene: 模块 {mcolor} 无基因匹配时间序列，跳过".debug
                Return Nothing
            End If

            ' 模块内定向边（两端都属于本模块）转为 RegulatoryLink
            Dim links As RegulatoryLink() = BuildModuleRegulatoryLinks(prior, genes)
            Dim net As DynamicBayesianNetwork = New DynamicBayesianNetwork().BuildFromTopology(links)
            Dim ts As List(Of Dictionary(Of String, Double)) = subMatrix.ToTimeSeries()

            If ts IsNot Nothing AndAlso ts.Count >= 2 Then
                Call net.LearnParameters(ts)
            Else
                Call $"GRN.TrainModularDBNIntervene: 模块 {mcolor} 有效时间点不足，仅使用拓扑先验 CPT".debug
            End If

            Dim eig As Double() = ComputeModuleEigengene(ts)
            Dim mdb As New ModuleDBN With {
                .ModuleColor = mcolor,
                .Genes = genes,
                .Net = net,
                .Eigengene = eig
            }

            For i As Integer = 0 To genes.Length - 1
                mdb.GeneIndex(genes(i)) = i
            Next

            Return mdb
        End Function

    End Module
End Namespace