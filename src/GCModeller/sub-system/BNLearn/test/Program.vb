Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention.InterventionComparisonExporter
Imports SMRUCC.genomics.Analysis.BNLearn.IO
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Module Program

    Sub Main(args As String())
        ' 1. 加载数据
        Dim exprData = BnIO.ReadGeneExpressionMatrix(Matrix.LoadData("G:\GCModeller\src\GCModeller\sub-system\demo\TestData1\gene_expression_matrix.csv"))
        Dim priorNet = BnIO.ReadPriorNetwork("G:\GCModeller\src\GCModeller\sub-system\demo\TestData1\regulatory_network_prior.csv".LoadCsv(Of RegulatoryEdge))
        Dim pathways As Dictionary(Of String, PathwayInfo) = "G:\GCModeller\src\GCModeller\sub-system\demo\TestData1\pathway_info.json".LoadJsonFile(Of Dictionary(Of String, PathwayInfo))

        ' 2. 创建工作流
        Dim workflow As New BNLearnWorkflow()
        workflow.ExpressionData = exprData
        workflow.PriorNetwork = priorNet
        workflow.StructureParams.MaxIterations = 100

        ' 3. 结构学习（MMHC + 白名单先验）
        workflow.LearnStructure()

        ' 4. 参数学习（高斯BN MLE）
        workflow.LearnParameters()

        ' 5. 虚拟敲除
        Dim koResult = workflow.KnockoutGene("codY")

        ' 6. 虚拟过表达
        Dim oeResult As InterventionResult = workflow.OverexpressGene("codY", 3.0)

        ' 7. 动态级联模拟
        Dim dynResult As InterventionResult = workflow.DynamicKnockout("codY", nTimeSteps:=10)

        ' 8. 批量敲除
        Dim batchResults As InterventionResult() = workflow.BatchKnockout({"codY", "terR", "luxR"}).ToArray

        ' 9. 输出结果
        workflow.SaveResults(App.HOME & "/output/")

        Dim save As New InterventionComparisonExporter(c({koResult, oeResult, dynResult}, batchResults))

        Call save.ExportAll(App.HOME & "/output/", pathways)

    End Sub
End Module
