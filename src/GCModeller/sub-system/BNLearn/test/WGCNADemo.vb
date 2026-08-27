Imports Microsoft.VisualBasic.Data.Framework.StorageProvider
Imports Microsoft.VisualBasic.Math.Matrix
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.BNLearn.IO
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Module WGCNADemo

    Sub Run()
        Dim geneSet As String() = DataFrameResolver.Load("K:\hsa\WGCNA_output-demo\gene_module_assignment.csv")("geneID")
        Dim modules As GeneModuleColor() = WGCNA.ReadModuleAssignment("K:\hsa\WGCNA_output-demo\gene_module_assignment.csv")
        Dim moduleCor As DataMatrix = WGCNA.ReadModuleEigengeneCorrelation("K:\hsa\WGCNA_output-demo\module_eigengene_correlation.csv")
        Dim subMat As Matrix = Matrix.LoadData("K:\hsa\Homo_sapiens_expr_advanced_all_conditions.csv")

        subMat = subMat(geneSet)

        Dim exprData = BnIO.ReadGeneExpressionMatrix(subMat)
        ' 2. 创建工作流
        Dim workflow As New BNLearnWorkflow()
        workflow.ExpressionData = exprData
        workflow.StructureParams.MaxIterations = 500

        ' 3. 结构学习（MMHC + 白名单先验）
        workflow.LearnStructure()
        ' 4. 参数学习（高斯BN MLE）
        workflow.LearnParameters()

        ' 5. 随机挑选一个基因做虚拟敲除（导入外部数据之前：基于训练网络自身的理论野生型基线）
        Dim koResult As InterventionResult = workflow.KnockoutGene(geneSet.Random)

        ' 6. 随机挑选一个基因做虚拟过表达
        Dim oeResult As InterventionResult = workflow.OverexpressGene(geneSet.Random, 3.0)

        ' 7. 随机挑选一个基因做动态级联模拟
        Dim dynResult As InterventionResult = workflow.DynamicKnockout(geneSet.Random, nTimeSteps:=10)

        ' 10. 输出结果
        workflow.SaveResults(App.HOME & "/output/bnlearn")

        Dim save As New InterventionComparisonExporter({koResult, oeResult, dynResult})

        Call save.ExportAll(App.HOME & "/output/")
    End Sub
End Module
