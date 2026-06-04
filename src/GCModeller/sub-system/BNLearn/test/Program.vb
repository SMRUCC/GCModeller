Imports BNLearn.Core
Imports BNLearn.IO

Module Program

    Sub Main(args As String())
        ' 1. 加载数据
        Dim exprData = BnIO.ReadGeneExpressionMatrix("expression_matrix.tsv")
        Dim priorNet = BnIO.ReadPriorNetwork("tf_regulatory_network.tsv")

        ' 2. 创建工作流
        Dim workflow As New BNLearnWorkflow()
        workflow.ExpressionData = exprData
        workflow.PriorNetwork = priorNet

        ' 3. 结构学习（MMHC + 白名单先验）
        workflow.LearnStructure()

        ' 4. 参数学习（高斯BN MLE）
        workflow.LearnParameters()

        ' 5. 虚拟敲除
        Dim koResult = workflow.KnockoutGene("TP53")

        ' 6. 虚拟过表达
        Dim oeResult = workflow.OverexpressGene("MYC", 3.0)

        ' 7. 动态级联模拟
        Dim dynResult = workflow.DynamicKnockout("TP53", nTimeSteps:=10)

        ' 8. 批量敲除
        Dim batchResults = workflow.BatchKnockout({"TP53", "BRCA1", "RB1"})

        ' 9. 输出结果
        workflow.SaveResults("output/")

    End Sub
End Module
