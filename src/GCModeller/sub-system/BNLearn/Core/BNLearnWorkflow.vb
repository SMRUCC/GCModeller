#Region "Microsoft.VisualBasic::dd4cded34fac203bd75ea5356df14f5e, sub-system\BNLearn\Core\BNLearnWorkflow.vb"

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

'   Total Lines: 309
'    Code Lines: 178 (57.61%)
' Comment Lines: 64 (20.71%)
'    - Xml Docs: 65.62%
' 
'   Blank Lines: 67 (21.68%)
'     File Size: 12.88 KB


'     Class BNLearnWorkflow
' 
'         Properties: ExpressionData, FittedNetwork, NormalizeData, NSamples, ParameterResult
'                     PriorNetwork, RandomSeed, StructureParams, StructureResult
' 
'         Function: (+2 Overloads) BatchKnockout, DynamicKnockout, KnockDownGene, KnockoutGene, LearnParameters
'                   LearnStructure, OverexpressGene, RunFullLearning
' 
'         Sub: LoadData, SaveResults
' 
' 
' /********************************************************************************/

#End Region

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
Imports Microsoft.VisualBasic.Data.Framework
Imports SMRUCC.genomics.Analysis.BNLearn.IO
Imports SMRUCC.genomics.Analysis.BNLearn.ParameterLearning
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

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
        Public Property StructureParams As New StructureLearning.StructureLearningParams

        ''' <summary>是否对表达数据做标准化</summary>
        Public Property NormalizeData As Boolean = True

        ''' <summary>采样数（用于推断和干预分析）</summary>
        Public Property NSamples As Integer = 10000

        ''' <summary>随机种子</summary>
        Public Property RandomSeed As Integer = 42

        ' ==================== 输出结果 ====================

        ''' <summary>学习到的贝叶斯网络</summary>
        Public Property FittedNetwork As BayesianNetwork

        ' ==================== 外部导入的转录组数据 ====================

        ''' <summary>
        ''' 外部导入的新基因表达水平（单样本向量，基因名 → 表达值）。
        ''' 通过 <see cref="SetExternalExpression"/> 设置，通常来自用户新检测的转录组。
        ''' 设置时仅保留与训练网络重叠的基因，并自动派生观测证据与动态初始状态。
        ''' </summary>
        Public Property ExternalExpression As Dictionary(Of String, Double)

        ''' <summary>
        ''' 观测证据：按基因名对齐后仅保留与训练网络重叠的（基因名 → 表达值）。
        ''' 供"观测证据模式"虚拟扰动使用（在给定表达水平条件下做 do-演算）。
        ''' </summary>
        Public Property ExternalEvidence As Dictionary(Of String, Double)

        ''' <summary>
        ''' 动态初始状态：按训练网络节点序排列的外部表达向量。
        ''' 供"动态初始状态模式"虚拟扰动使用（作为级联模拟起点）。
        ''' 仅重叠基因位置被外部值覆盖，未覆盖位置保持训练数据均值。
        ''' </summary>
        Public Property ExternalInitialState As Double()

        ''' <summary>结构学习结果</summary>
        Public Property StructureResult As StructureLearning.StructureLearningResult

        ''' <summary>参数学习结果</summary>
        Public Property ParameterResult As ParameterLearning.ParameterLearningResult

        ' ==================== 工作流步骤 ====================

        ''' <summary>
        ''' 步骤1：加载数据
        ''' </summary>
        Public Sub LoadData(expressionMatrixPath As String, priorNetworkPath As String)
            ExpressionData = IO.BnIO.ReadGeneExpressionMatrix(Matrix.LoadData(expressionMatrixPath))
            If Not String.IsNullOrEmpty(priorNetworkPath) AndAlso System.IO.File.Exists(priorNetworkPath) Then
                PriorNetwork = IO.BnIO.ReadPriorNetwork(priorNetworkPath.LoadCsv(Of RegulatoryEdge)(mute:=True))
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
            If FittedNetwork Is Nothing Then
                Throw New Exception("请先执行结构学习")
            End If

            Dim workData As GeneExpressionData = ExpressionData
            If NormalizeData Then
                workData = ExpressionData.Standardize
            End If

            ParameterResult = BnParameterLearner.Learn(FittedNetwork, workData)

            Return ParameterResult
        End Function

        ''' <summary>
        ''' 步骤4：虚拟基因敲除
        ''' </summary>
        Public Function KnockoutGene(geneName As String, Optional nSamples As Integer = 0) As Intervention.InterventionResult
            If FittedNetwork Is Nothing OrElse ParameterResult Is Nothing Then
                Throw New Exception("请先执行结构学习和参数学习")
            End If

            If nSamples <= 0 Then nSamples = Me.NSamples

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
        Public Function OverexpressGene(geneName As String, Optional nSamples As Integer = 0) As Intervention.InterventionResult
            If FittedNetwork Is Nothing OrElse ParameterResult Is Nothing Then
                Throw New Exception("请先执行结构学习和参数学习")
            End If

            If nSamples <= 0 Then nSamples = Me.NSamples

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

        Public Function KnockDownGene(geneName As String, Optional nSamples As Integer = 0) As Intervention.InterventionResult
            If FittedNetwork Is Nothing OrElse ParameterResult Is Nothing Then
                Throw New Exception("请先执行结构学习和参数学习")
            End If

            If nSamples <= 0 Then nSamples = Me.NSamples

            Dim workData As GeneExpressionData = ExpressionData
            If NormalizeData Then
                workData = ExpressionData.Standardize
            End If

            Dim analyzer As New Intervention.BnInterventionAnalyzer(FittedNetwork, workData)
            Dim spec As New Intervention.InterventionSpec() With {
                .GeneName = geneName,
                .Mode = Intervention.InterventionMode.Knockdown
            }

            Return analyzer.AnalyzeIntervention(spec, nSamples, RandomSeed)
        End Function

        ''' <summary>
        ''' 步骤4：动态级联敲除模拟
        ''' </summary>
        Public Function DynamicKnockout(geneName As String, nTimeSteps As Integer, Optional nSamples As Integer = 0) As Intervention.InterventionResult
            If FittedNetwork Is Nothing OrElse ParameterResult Is Nothing Then
                Throw New Exception("请先执行结构学习和参数学习")
            End If

            If nSamples <= 0 Then nSamples = Me.NSamples

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

        ' ==================== 外部转录组数据导入与上下文扰动 ====================

        ''' <summary>
        ''' 从外部转录组向量文件导入新的基因表达水平数据。
        ''' 文件为两列（gene, expression）CSV/TSV，可由 <see cref="IO.BnIO.ReadExpressionVector"/> 解析。
        ''' 导入后按基因名对齐，仅保留与训练网络重叠的基因，并自动派生观测证据与动态初始状态。
        ''' </summary>
        Public Sub ImportExternalExpression(path As String)
            Dim vector As Dictionary(Of String, Double) = IO.BnIO.ReadExpressionVector(path)
            SetExternalExpression(vector)
        End Sub

        ''' <summary>
        ''' 核心导入接口：接收基因名 → 表达值的键值对字典（单样本向量 / 一组均值），
        ''' 作为外部新检测到的转录组数据设置到训练好的网络中。
        ''' 仅保留与训练网络重叠的基因（大小写不敏感），忽略未建模基因；
        ''' 若没有任何重叠基因则抛出友好异常。派生内容写入
        ''' <see cref="ExternalEvidence"/>（观测证据模式）与 <see cref="ExternalInitialState"/>（动态初始状态模式）。
        ''' </summary>
        ''' <param name="geneExpression">
        ''' 外部转录组数据：基因名 → 表达值 的键值对字典。
        ''' 例如 New Dictionary(Of String, Double) From {{"codY", 12.3}, {"comK", 4.5}}
        ''' </param>
        Public Sub SetExternalExpression(geneExpression As Dictionary(Of String, Double))
            If FittedNetwork Is Nothing Then
                Throw New Exception("请先执行结构学习和参数学习，再导入外部表达数据")
            End If
            If geneExpression Is Nothing OrElse geneExpression.Count = 0 Then
                Throw New Exception("外部表达数据为空，无法导入")
            End If

            ExternalExpression = New Dictionary(Of String, Double)(geneExpression, StringComparer.OrdinalIgnoreCase)

            Dim nG As Integer = FittedNetwork.Nodes.Count
            Dim names As String() = FittedNetwork.Nodes.Select(Function(n) n.Name).ToArray()

            ' 1) 观测证据：仅保留重叠基因
            Dim evidence As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
            ' 2) 动态初始状态：按网络节点序，默认用训练数据各基因行均值
            Dim initialState As Double() = New Double(nG - 1) {}
            Dim workData As GeneExpressionData = ExpressionData
            If NormalizeData Then workData = ExpressionData.Standardize
            For i = 0 To nG - 1
                Dim geneRow As Integer = workData.GetGeneIndex(names(i))
                If geneRow >= 0 Then
                    Dim sum As Double = 0
                    For j = 0 To workData.NSample - 1
                        sum += workData.Matrix(geneRow, j)
                    Next
                    initialState(i) = If(workData.NSample > 0, sum / workData.NSample, 0)
                Else
                    initialState(i) = 0
                End If
            Next

            Dim overlap As Integer = 0
            For Each kv In geneExpression
                Dim idx As Integer = Array.FindIndex(names, Function(n) String.Equals(n, kv.Key, StringComparison.OrdinalIgnoreCase))
                If idx >= 0 Then
                    evidence(names(idx)) = kv.Value
                    initialState(idx) = kv.Value
                    overlap += 1
                End If
            Next

            If overlap = 0 Then
                Throw New Exception("外部表达数据未包含任何与训练网络重叠的基因，无法导入（请检查基因名是否一致）")
            End If

            ExternalEvidence = evidence
            ExternalInitialState = initialState
        End Sub

        ''' <summary>
        ''' 观测证据模式：基于已导入的外部转录组数据（<see cref="ExternalEvidence"/>），
        ''' 在"给定该表达水平条件"下执行敲除虚拟扰动（do-演算 + 条件推断）。
        ''' 结果反映用户真实样本背景下的因果效应。
        ''' 必须先调用 <see cref="SetExternalExpression"/> / <see cref="ImportExternalExpression"/>。
        ''' </summary>
        Public Function KnockoutGeneWithEvidence(geneName As String, Optional nSamples As Integer = 0) As Intervention.InterventionResult
            Return RunInterventionWithEvidence(geneName, Intervention.InterventionMode.Knockout, nSamples)
        End Function

        ''' <summary>
        ''' 观测证据模式：基于已导入的外部转录组数据，在给定表达水平条件下执行过表达虚拟扰动。
        ''' </summary>
        Public Function OverexpressGeneWithEvidence(geneName As String, Optional nSamples As Integer = 0) As Intervention.InterventionResult
            Return RunInterventionWithEvidence(geneName, Intervention.InterventionMode.Overexpression, nSamples)
        End Function

        Private Function RunInterventionWithEvidence(geneName As String,
                                                     mode As Intervention.InterventionMode,
                                                     nSamples As Integer) As Intervention.InterventionResult
            If FittedNetwork Is Nothing OrElse ParameterResult Is Nothing Then
                Throw New Exception("请先执行结构学习和参数学习")
            End If
            If ExternalEvidence Is Nothing OrElse ExternalEvidence.Count = 0 Then
                Throw New Exception("请先调用 SetExternalExpression / ImportExternalExpression 导入外部表达数据")
            End If
            If nSamples <= 0 Then nSamples = Me.NSamples

            Dim workData As GeneExpressionData = ExpressionData
            If NormalizeData Then workData = ExpressionData.Standardize

            Dim analyzer As New Intervention.BnInterventionAnalyzer(FittedNetwork, workData)
            Dim spec As New Intervention.InterventionSpec() With {
                .GeneName = geneName,
                .Mode = mode
            }

            Return analyzer.AnalyzeIntervention(spec, nSamples, RandomSeed, ExternalEvidence)
        End Function

        ''' <summary>
        ''' 动态初始状态模式：基于已导入的外部转录组数据（<see cref="ExternalInitialState"/>），
        ''' 以其作为级联模拟起点执行动态敲除虚拟扰动，模拟在用户样本背景下的级联传播。
        ''' 必须先调用 <see cref="SetExternalExpression"/> / <see cref="ImportExternalExpression"/>。
        ''' </summary>
        Public Function DynamicKnockoutWithState(geneName As String, nTimeSteps As Integer, Optional nSamples As Integer = 0) As Intervention.InterventionResult
            If FittedNetwork Is Nothing OrElse ParameterResult Is Nothing Then
                Throw New Exception("请先执行结构学习和参数学习")
            End If
            If ExternalInitialState Is Nothing Then
                Throw New Exception("请先调用 SetExternalExpression / ImportExternalExpression 导入外部表达数据")
            End If
            If nSamples <= 0 Then nSamples = Me.NSamples

            Dim workData As GeneExpressionData = ExpressionData
            If NormalizeData Then workData = ExpressionData.Standardize

            Dim analyzer As New Intervention.BnInterventionAnalyzer(FittedNetwork, workData)
            Dim spec As New Intervention.InterventionSpec() With {
                .GeneName = geneName,
                .Mode = Intervention.InterventionMode.Knockout
            }

            Return analyzer.DynamicIntervention(spec, nTimeSteps, nSamples, RandomSeed, ExternalInitialState)
        End Function

        ''' <summary>
        ''' 批量敲除所有基因
        ''' </summary>
        Public Function BatchKnockout(geneNames As IEnumerable(Of String), Optional nSamples As Integer = 0) As IEnumerable(Of Intervention.InterventionResult)
            If FittedNetwork Is Nothing OrElse ParameterResult Is Nothing Then
                Throw New Exception("请先执行结构学习和参数学习")
            End If

            If nSamples <= 0 Then nSamples = Me.NSamples

            Dim workData As GeneExpressionData = ExpressionData
            If NormalizeData Then
                workData = ExpressionData.Standardize
            End If

            Dim analyzer As New Intervention.BnInterventionAnalyzer(FittedNetwork, workData)
            Dim allIndices As Integer() = geneNames.Select(Function(geneName) FittedNetwork.GetNodeIndex(geneName)).ToArray()

            Return analyzer.BatchIntervention(allIndices, Intervention.InterventionMode.Knockout, nSamples, RandomSeed)
        End Function

        ''' <summary>
        ''' 批量敲除所有基因
        ''' </summary>
        Public Function BatchKnockout(Optional nSamples As Integer = 0) As IEnumerable(Of Intervention.InterventionResult)
            If FittedNetwork Is Nothing OrElse ParameterResult Is Nothing Then
                Throw New Exception("请先执行结构学习和参数学习")
            End If

            If nSamples <= 0 Then nSamples = Me.NSamples

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
                WriteModel.WriteNetworkStructure(FittedNetwork,
                    System.IO.Path.Combine(outputDir, "network_structure.tsv"))
            End If

            If FittedNetwork IsNot Nothing Then
                WriteModel.WriteCPDParameters(FittedNetwork,
                    System.IO.Path.Combine(outputDir, "network_parameters.tsv"))
            End If
        End Sub

    End Class

End Namespace

