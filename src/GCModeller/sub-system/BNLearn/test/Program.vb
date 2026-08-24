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

