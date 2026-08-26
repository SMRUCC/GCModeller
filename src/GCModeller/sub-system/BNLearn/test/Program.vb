#Region "Microsoft.VisualBasic::ec2a81bef526b03389829f04f1c6e240, sub-system\BNLearn\test\Program.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 50
    '    Code Lines: 28 (56.00%)
    ' Comment Lines: 9 (18.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 13 (26.00%)
    '     File Size: 2.10 KB


    ' Module Program
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.BNLearn.IO
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.MetabolicModel

Module Program

    Sub Main(args As String())
        ' 1. 加载数据
        Dim exprData = BnIO.ReadGeneExpressionMatrix(Matrix.LoadData("G:\GCModeller\src\GCModeller\sub-system\demo\TestData1\gene_expression_matrix.csv"))
        Dim priorNet = BnIO.ReadPriorNetwork("G:\GCModeller\src\GCModeller\sub-system\demo\TestData1\regulatory_network_prior.csv".LoadCsv(Of RegulatoryEdge))
        Dim pathways As Dictionary(Of String, MetabolicPathway) = "G:\GCModeller\src\GCModeller\sub-system\demo\TestData1\pathway_info.json".LoadJsonFile(Of Dictionary(Of String, MetabolicPathway))

        ' 2. 创建工作流
        Dim workflow As New BNLearnWorkflow()
        workflow.ExpressionData = exprData
        workflow.PriorNetwork = priorNet
        workflow.StructureParams.MaxIterations = 100

        ' 3. 结构学习（MMHC + 白名单先验）
        workflow.LearnStructure()

        ' 4. 参数学习（高斯BN MLE）
        workflow.LearnParameters()

        ' 5. 虚拟敲除（导入外部数据之前：基于训练网络自身的理论野生型基线）
        Dim koResult = workflow.KnockoutGene("codY")

        ' 6. 虚拟过表达
        Dim oeResult As InterventionResult = workflow.OverexpressGene("codY", 3.0)

        ' 7. 动态级联模拟
        Dim dynResult As InterventionResult = workflow.DynamicKnockout("codY", nTimeSteps:=10)

        ' 8. 批量敲除
        Dim batchResults As InterventionResult() = workflow.BatchKnockout({"codY", "terR", "luxR"}).ToArray

        ' =====================================================================
        ' 9. 外部转录组数据导入 + 导入前后虚拟扰动对比
        ' =====================================================================
        ' 模拟：用户新检测到的转录组数据（仅部分基因子集，单位与训练矩阵一致）
        ' 真实使用时可直接从文件导入：
        '   workflow.ImportExternalExpression("G:\...\my_new_transcriptome.csv")
        ' 下面用核心字典接口演示（基因名 → 表达值 的键值对字典）：
        Dim myTranscriptome As New Dictionary(Of String, Double) From {
            {"codY", 18.6},
            {"comK", 2.3},
            {"luxR", 9.1},
            {"terR", 5.4},
            {"spo0A", 7.2}
        }

        ' —— 关键接口：接收外部转录组数据字典，设置新的基因表达上下文 ——
        workflow.SetExternalExpression(myTranscriptome)

        Console.WriteLine()
        Console.WriteLine("========================================")
        Console.WriteLine("  外部转录组数据导入 + 虚拟扰动对比")
        Console.WriteLine("========================================")
        Console.WriteLine("导入的重叠基因（观测证据 / 初始状态）：")
        For Each kv In workflow.ExternalEvidence
            Console.WriteLine("  {0} = {1}", kv.Key, kv.Value)
        Next

        ' 9.1 导入前：理论野生型基线下敲除 codY（等价于 workflow.KnockoutGene）
        Dim koBefore As InterventionResult = workflow.KnockoutGene("codY")

        ' 9.2 导入后 - 观测证据模式：在"给定我的表达水平条件"下敲除 codY
        Dim koEvidence As InterventionResult = workflow.KnockoutGeneWithEvidence("codY")

        ' 9.3 导入后 - 动态初始状态模式：以我的表达水平为起点做动态级联敲除
        Dim koState As InterventionResult = workflow.DynamicKnockoutWithState("codY", nTimeSteps:=10)

        ' 输出三段对比（针对 codY 敲除，关注若干下游/观测基因的变化差异）
        Call PrintComparison("codY", koBefore, koEvidence, koState, workflow)

        ' 10. 输出结果
        workflow.SaveResults(App.HOME & "/output/")

        Dim save As New InterventionComparisonExporter(c({koResult, oeResult, dynResult, koBefore, koEvidence, koState}, batchResults))

        Call save.ExportAll(App.HOME & "/output/", pathways)

    End Sub

    ''' <summary>
    ''' 打印"导入前 / 导入后(观测证据) / 导入后(动态初始状态)"三段虚拟扰动结果对比
    ''' </summary>
    Private Sub PrintComparison(geneName As String,
                                before As InterventionResult,
                                afterEvidence As InterventionResult,
                                afterState As InterventionResult,
                                workflow As BNLearnWorkflow)
        Console.WriteLine()
        Console.WriteLine("------------------------------------------------------------")
        Console.WriteLine(" 敲除 {0} 的虚拟扰动效应对比（仅列出显著变化基因）", geneName)
        Console.WriteLine("------------------------------------------------------------")
        Console.WriteLine("{0,-10} {1,14} {2,16} {3,18}",
                          "Gene", "导入前Δ", "导入后·证据Δ", "导入后·状态Δ")

        Dim names As String() = before.GeneNames
        For i = 0 To names.Length - 1
            ' 仅展示在任一段中显著变化的基因，便于对比
            If before.IsSignificant(i) OrElse afterEvidence.IsSignificant(i) OrElse afterState.IsSignificant(i) Then
                Console.WriteLine("{0,-10} {1,14:F3} {2,16:F3} {3,18:F3}",
                                  names(i),
                                  before.FoldChanges(i),
                                  afterEvidence.FoldChanges(i),
                                  afterState.FoldChanges(i))
            End If
        Next
        Console.WriteLine("------------------------------------------------------------")
        Console.WriteLine("(Δ = 扰动后均值 - 基线均值；导入前基线为网络理论野生型，")
        Console.WriteLine(" 导入后基线为你的转录组上下文下条件分布 / 动态初始状态)")
    End Sub

