Imports Microsoft.VisualBasic.Data.Framework.StorageProvider
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Core.WGCNADBN
Imports SMRUCC.genomics.Analysis.BNLearn.IO
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Module WGCNADemo

    ''' <summary>
    ''' 基于 WGCNA 模块划分训练多个 BNLearn 子网络，并在整合后的全局网络上
    ''' 执行全局虚拟扰动（雅可比线性传播 + 级联采样传播）。
    ''' 结果写出为 TSV（基因 × 扰动源响应矩阵 + 每源明细）并打印 Top 变化基因摘要。
    ''' </summary>
    Sub Run()
        ' 1. 读取数据
        Dim geneSet As String() = DataFrameResolver.Load("K:\hsa\WGCNA_output-demo\gene_module_assignment.csv")("geneID")
        Dim modules As GeneModuleColor() = WGCNA.ReadModuleAssignment("K:\hsa\WGCNA_output-demo\gene_module_assignment.csv")
        Dim subMat As Matrix = Matrix.LoadData("K:\hsa\Homo_sapiens_expr_advanced_all_conditions.csv")
        subMat = subMat(geneSet)

        Dim exprData = BnIO.ReadGeneExpressionMatrix(subMat)

        ' 2. 构建 WGCNA 子网络 + 全局扰动流水线
        Dim pipeline As New WGCNASubnetworkPipeline() With {
            .NormalizeData = True,
            .NSamples = 5000,
            .RandomSeed = 42,
            .MaxSteps = 30,
            .HubTopN = 20,
            .CrossModuleCorThreshold = 0.3,
            .CrossGeneCorThreshold = 0.4,
            .CrossScale = 0.5
        }
        ' 结构学习参数（与 BNLearnWorkflow 一致）
        pipeline.StructureParams.MaxIterations = 500

        ' 3. 方法一（默认）：雅可比矩阵多步线性传播
        pipeline.Propagation = PropagationMethod.Jacobian
        Dim jacResults = pipeline.Run(modules, exprData)
        Dim outDirJac = App.HOME & "/output/wgcna_global_perturbation/jacobian"
        Call pipeline.SaveResults(jacResults, outDirJac)

        ' 4. 方法二：级联采样跨模块传播（对前若干代表源演示，避免全量过慢）
        pipeline.Propagation = PropagationMethod.CascadeSampling
        Dim demoSources = jacResults.Take(5).Select(Function(r) r.SourceGene).ToArray()
        Dim casResults = pipeline.Run(modules, exprData, demoSources)
        Dim outDirCas = App.HOME & "/output/wgcna_global_perturbation/cascade"
        Call pipeline.SaveResults(casResults, outDirCas)

        Call Console.WriteLine("[WGCNADemo] 全局虚拟扰动流程完成。雅可比方法源数={0}, 级联方法源数={1}",
                               jacResults.Count, casResults.Count)
        Call Console.WriteLine("[WGCNADemo] 结果目录: " & App.HOME & "/output/wgcna_global_perturbation/")
    End Sub
End Module
