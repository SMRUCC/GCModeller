' ============================================================
' BNLearnWorkflow.vb - 高层 API 入口
' ============================================================
' 将结构学习 → 参数学习 → 推断 → 干预分析 串联为完整工作流
' 
' 典型使用流程：
'   1. 加载基因表达矩阵 + 先验调控网络
'   2. 结构学习（MMHC + 白名单先验）
'   3. 参数学习（高斯BN MLE）
'   4. 虚拟干扰分析（基因敲除/过表达）
'   5. 输出结果
' ============================================================

Imports System.Text

Namespace Core

    ''' <summary>
    ''' BNLearn 工作流 —— 基因表达调控网络建模与虚拟干扰分析
    ''' </summary>
    Public Class BNLearnWorkflow

        ' ==================== 输入数据 ====================

        ''' <summary>基因表达矩阵</summary>
        Public Property ExpressionData As GeneExpressionData

        ''' <summary>先验调控网络（TF→靶基因 白名单）</summary>
        Public Property PriorNetwork As PriorNetwork

        ' ==================== 学习参数 ====================

        ''' <summary>结构学习参数</summary>
        Public Property StructureParams As New StructureLearning.StructureLearningParams()

        ''' <summary>是否对表达数据做标准化</summary>
        Public Property NormalizeData As Boolean = True

        ''' <summary>采样数（用于推断和干预分析）</summary>
        Public Property NSamples As Integer = 10000

        ''' <summary>随机种子</summary>
        Public Property RandomSeed As Integer = 42

        ' ==================== 输出结果 ====================

        ''' <summary>学习到的贝叶斯网络</summary>
        Public Property FittedNetwork As BayesianNetwork

        ''' <summary>结构学习结果</summary>
        Public Property StructureResult As StructureLearning.StructureLearningResult

        ''' <summary>参数学习结果</summary>
        Public Property ParameterResult As ParameterLearning.ParameterLearningResult

        ' ==================== 工作流步骤 ====================

        ''' <summary>
        ''' 步骤1：加载数据
        ''' </summary>
        Public Sub LoadData(expressionMatrixPath As String, priorNetworkPath As String,
                            Optional separator As Char = ChrW(9))
            ExpressionData = IO.BnIO.ReadGeneExpressionMatrix(expressionMatrixPath, True, separator)
            If Not String.IsNullOrEmpty(priorNetworkPath) AndAlso System.IO.File.Exists(priorNetworkPath) Then
                PriorNetwork = IO.BnIO.ReadPriorNetwork(priorNetworkPath, separator)
            Else
                PriorNetwork = New PriorNetwork()
            End If
        End Sub

        ''' <summary>
        ''' 步骤2：结构学习
        ''' </summary>
        Public Function LearnStructure() As StructureLearning.StructureLearningResult
            If ExpressionData Is Nothing Then Throw New Exception("请先加载基因表达数据")

            ' 数据预处理
            Dim workData As GeneExpressionData = ExpressionData
            If NormalizeData Then
                workData = ExpressionData.Standardize
            End If

            ' 结构学习
            Dim learner As New StructureLearning.BnStructureLearner()
            StructureResult = learner.Learn(workData, StructureParams, PriorNetwork)

            FittedNetwork = StructureResult.Network
            Return StructureResult
        End Function

        ''' <summary>
        ''' 步骤3：参数学习
        ''' </summary>
        Public Function LearnParameters() As ParameterLearning.ParameterLearningResult
            If FittedNetwork Is Nothing Then Throw New Exception("请先执行结构学习")

            Dim workData As GeneExpressionData = ExpressionData
            If NormalizeData Then
                workData = ExpressionData.Standardize
            End If

            Dim learner As New ParameterLearning.BnParameterLearner()
            ParameterResult = learner.Learn(FittedNetwork, workData)

            Return ParameterResult
        End Function

        ''' <summary>
        ''' 步骤4：虚拟基因敲除
        ''' </summary>
        Public Function KnockoutGene(geneName As String,
                                     Optional nSamples As Integer = 0) As Intervention.InterventionResult
            If FittedNetwork Is Nothing OrElse ParameterResult Is Nothing Then
                Throw New Exception("请先执行结构学习和参数学习")
            End If

            If nSamples <= 0 Then nSamples = NSamples

            Dim workData As GeneExpressionData = ExpressionData
            If NormalizeData Then
                workData = ExpressionData.Standardize
            End If

            Dim analyzer As New Intervention.BnInterventionAnalyzer(FittedNetwork, workData)
            Dim spec As New Intervention.InterventionSpec() With {
                .GeneName = geneName,
                .Mode = Intervention.InterventionMode.Knockout
            }

            Return analyzer.AnalyzeIntervention(spec, nSamples, RandomSeed)
        End Function

        ''' <summary>
        ''' 步骤4：虚拟基因过表达
        ''' </summary>
        Public Function OverexpressGene(geneName As String,
                                         Optional nSamples As Integer = 0) As Intervention.InterventionResult
            If FittedNetwork Is Nothing OrElse ParameterResult Is Nothing Then
                Throw New Exception("请先执行结构学习和参数学习")
            End If

            If nSamples <= 0 Then nSamples = NSamples

            Dim workData As GeneExpressionData = ExpressionData
            If NormalizeData Then
                workData = ExpressionData.Standardize
            End If

            Dim analyzer As New Intervention.BnInterventionAnalyzer(FittedNetwork, workData)
            Dim spec As New Intervention.InterventionSpec() With {
                .GeneName = geneName,
                .Mode = Intervention.InterventionMode.Overexpression
            }

            Return analyzer.AnalyzeIntervention(spec, nSamples, RandomSeed)
        End Function

        ''' <summary>
        ''' 步骤4：动态级联敲除模拟
        ''' </summary>
        Public Function DynamicKnockout(geneName As String,
                                         nTimeSteps As Integer,
                                         Optional nSamples As Integer = 0) As Intervention.InterventionResult
            If FittedNetwork Is Nothing OrElse ParameterResult Is Nothing Then
                Throw New Exception("请先执行结构学习和参数学习")
            End If

            If nSamples <= 0 Then nSamples = NSamples

            Dim workData As GeneExpressionData = ExpressionData
            If NormalizeData Then
                workData = ExpressionData.Standardize
            End If

            Dim analyzer As New Intervention.BnInterventionAnalyzer(FittedNetwork, workData)
            Dim spec As New Intervention.InterventionSpec() With {
                .GeneName = geneName,
                .Mode = Intervention.InterventionMode.Knockout
            }

            Return analyzer.DynamicIntervention(spec, nTimeSteps, nSamples, RandomSeed)
        End Function

        ''' <summary>
        ''' 批量敲除所有基因
        ''' </summary>
        Public Function BatchKnockout(Optional nSamples As Integer = 0) As List(Of Intervention.InterventionResult)
            If FittedNetwork Is Nothing OrElse ParameterResult Is Nothing Then
                Throw New Exception("请先执行结构学习和参数学习")
            End If

            If nSamples <= 0 Then nSamples = NSamples

            Dim workData As GeneExpressionData = ExpressionData
            If NormalizeData Then
                workData = ExpressionData.Standardize
            End If

            Dim analyzer As New Intervention.BnInterventionAnalyzer(FittedNetwork, workData)
            Dim allIndices As Integer() = Enumerable.Range(0, FittedNetwork.Nodes.Count).ToArray()

            Return analyzer.BatchIntervention(allIndices, Intervention.InterventionMode.Knockout, nSamples, RandomSeed)
        End Function

        ''' <summary>
        ''' 完整工作流：结构学习 + 参数学习
        ''' </summary>
        Public Function RunFullLearning() As String
            Dim sb As New StringBuilder()

            sb.AppendLine("========================================")
            sb.AppendLine("  BNLearn 基因表达调控网络建模")
            sb.AppendLine("========================================")
            sb.AppendLine()

            ' 数据信息
            sb.AppendLine(String.Format("基因数: {0}", ExpressionData.NGene))
            sb.AppendLine(String.Format("样本数: {0}", ExpressionData.NSample))
            sb.AppendLine(String.Format("先验边数: {0}", PriorNetwork.Edges.Count))
            sb.AppendLine()

            ' 结构学习
            sb.AppendLine("--- 结构学习 ---")
            Dim structResult = LearnStructure()
            sb.AppendLine(String.Format("算法: {0}", StructureParams.Algorithm.ToString()))
            sb.AppendLine(String.Format("学习到边数: {0}", FittedNetwork.EdgeCount))
            sb.AppendLine(String.Format("最终 BIC: {0:F2}", structResult.FinalBIC))
            sb.AppendLine(String.Format("耗时: {0} ms", structResult.ElapsedMs))
            sb.AppendLine()

            ' 参数学习
            sb.AppendLine("--- 参数学习 ---")
            Dim paramResult = LearnParameters()
            sb.AppendLine(String.Format("总对数似然: {0:F2}", paramResult.TotalLogLikelihood))
            sb.AppendLine(String.Format("总 BIC: {0:F2}", paramResult.TotalBIC))
            sb.AppendLine(String.Format("平均 R²: {0:F4}", paramResult.AverageRSquared))
            sb.AppendLine(String.Format("耗时: {0} ms", paramResult.ElapsedMs))
            sb.AppendLine()

            ' 网络摘要
            sb.AppendLine("--- 网络结构 ---")
            sb.AppendLine(FittedNetwork.ToString())

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' 保存所有结果到文件
        ''' </summary>
        Public Sub SaveResults(outputDir As String)
            If Not System.IO.Directory.Exists(outputDir) Then
                System.IO.Directory.CreateDirectory(outputDir)
            End If

            If FittedNetwork IsNot Nothing Then
                IO.BnIO.WriteNetworkStructure(FittedNetwork,
                    System.IO.Path.Combine(outputDir, "network_structure.tsv"))
            End If

            If FittedNetwork IsNot Nothing Then
                IO.BnIO.WriteCPDParameters(FittedNetwork,
                    System.IO.Path.Combine(outputDir, "network_parameters.tsv"))
            End If
        End Sub

    End Class

End Namespace
