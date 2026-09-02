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

Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports Microsoft.VisualBasic.Data.Framework
Imports SMRUCC.genomics.Analysis.BNLearn.IO
Imports SMRUCC.genomics.Analysis.BNLearn.ParameterLearning
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Namespace Core

    ''' <summary>
    ''' BNLearn 工作流 —— 基因表达调控网络建模与虚拟干扰分析
    ''' </summary>
    Public Class BNLearnWorkflow : Implements InsilicoPerturbationExperiment

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

        ''' <summary>
        ''' 虚拟扰动 Strict 模式开关（透传给内部创建的 <see cref="Intervention.BnInterventionAnalyzer"/>，默认 True）。
        ''' True: <see cref="KnockoutGene"/> / <see cref="OverexpressGene"/> / <see cref="KnockDownGene"/> /
        ''' <see cref="DynamicKnockout"/> 等扰动函数在找不到目标基因时抛出错误；
        ''' False: 在终端打印一条警告消息，不执行虚拟扰动，直接以野生型数据作为扰动结果返回
        ''' 并将结果的 Undefined 标记为 True。
        ''' </summary>
        Public Property Strict As Boolean = True

        ' ==================== 输出结果 ====================

        ''' <summary>学习到的贝叶斯网络</summary>
        Public Property FittedNetwork As BayesianNetwork

        ''' <summary>
        ''' 获取动态贝叶斯网络模型中被建模的目标基因 ID 集合
        ''' （即 <see cref="FittedNetwork"/> 全部节点的基因名，按网络节点顺序排列）。
        ''' 结构学习尚未完成（<see cref="FittedNetwork"/> 为空）时返回空数组而不抛出错误。
        ''' </summary>
        Public ReadOnly Property TargetGenes As String()
            Get
                If FittedNetwork Is Nothing Then
                    Return {}
                Else
                    Return FittedNetwork.Nodes.Select(Function(n) n.Name).ToArray()
                End If
            End Get
        End Property

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

            ParameterResult = New BnParameterLearner().Learn(FittedNetwork, workData)

            Return ParameterResult
        End Function

        ''' <summary>
        ''' 步骤4：虚拟基因敲除
        ''' </summary>
        Public Function KnockoutGene(geneName As String, Optional nSamples As Integer = 0) As Intervention.InterventionResult Implements InsilicoPerturbationExperiment.KnockoutGene
            If FittedNetwork Is Nothing OrElse ParameterResult Is Nothing Then
                Throw New Exception("请先执行结构学习和参数学习")
            End If

            If nSamples <= 0 Then nSamples = Me.NSamples

            Dim workData As GeneExpressionData = ExpressionData
            If NormalizeData Then
                workData = ExpressionData.Standardize
            End If

            Dim analyzer As New Intervention.BnInterventionAnalyzer(FittedNetwork, workData) With {.Strict = Me.Strict}
            Dim spec As New Intervention.InterventionSpec() With {
                .GeneName = geneName,
                .Mode = Intervention.InterventionMode.Knockout
            }

            Return analyzer.AnalyzeIntervention(spec, nSamples, RandomSeed)
        End Function

        ''' <summary>
        ''' 步骤4：虚拟基因过表达
        ''' </summary>
        Public Function OverexpressGene(geneName As String, Optional nSamples As Integer = 0) As Intervention.InterventionResult Implements InsilicoPerturbationExperiment.OverexpressGene
            If FittedNetwork Is Nothing OrElse ParameterResult Is Nothing Then
                Throw New Exception("请先执行结构学习和参数学习")
            End If

            If nSamples <= 0 Then nSamples = Me.NSamples

            Dim workData As GeneExpressionData = ExpressionData
            If NormalizeData Then
                workData = ExpressionData.Standardize
            End If

            Dim analyzer As New Intervention.BnInterventionAnalyzer(FittedNetwork, workData) With {.Strict = Me.Strict}
            Dim spec As New Intervention.InterventionSpec() With {
                .GeneName = geneName,
                .Mode = Intervention.InterventionMode.Overexpression
            }

            Return analyzer.AnalyzeIntervention(spec, nSamples, RandomSeed)
        End Function

        Public Function KnockDownGene(geneName As String, Optional nSamples As Integer = 0) As Intervention.InterventionResult Implements InsilicoPerturbationExperiment.KnockDownGene
            If FittedNetwork Is Nothing OrElse ParameterResult Is Nothing Then
                Throw New Exception("请先执行结构学习和参数学习")
            End If

            If nSamples <= 0 Then nSamples = Me.NSamples

            Dim workData As GeneExpressionData = ExpressionData
            If NormalizeData Then
                workData = ExpressionData.Standardize
            End If

            Dim analyzer As New Intervention.BnInterventionAnalyzer(FittedNetwork, workData) With {.Strict = Me.Strict}
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

            Dim analyzer As New Intervention.BnInterventionAnalyzer(FittedNetwork, workData) With {.Strict = Me.Strict}
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

            Dim analyzer As New Intervention.BnInterventionAnalyzer(FittedNetwork, workData) With {.Strict = Me.Strict}
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

            Dim analyzer As New Intervention.BnInterventionAnalyzer(FittedNetwork, workData) With {.Strict = Me.Strict}
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

            Dim analyzer As New Intervention.BnInterventionAnalyzer(FittedNetwork, workData) With {.Strict = Me.Strict}
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

            Dim analyzer As New Intervention.BnInterventionAnalyzer(FittedNetwork, workData) With {.Strict = Me.Strict}
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

        ''' <summary>
        ''' zip 压缩包内的模型格式版本号
        ''' </summary>
        Private Const ModelFormatVersion As Integer = 1

        ''' <summary>
        ''' save current model as zip file
        ''' </summary>
        ''' <param name="file">
        ''' 目标输出流。由 R# 的 ``writeBin`` 传入（文件路径或连接），调用方负责流的释放，
        ''' 因此这里以 leaveOpen 的方式使用 <see cref="ZipArchive"/>。
        ''' </param>
        ''' 
        ''' zip 布局：
        ''' ```
        ''' meta.txt                       version / nodes / edges / genes / samples / 各段存在性标志
        ''' settings.txt                   工作流开关与结构学习参数
        ''' nodes.txt                      每行一个节点名，行号即节点索引
        ''' edges.tsv                      fromIdx, toIdx
        ''' whitelist.tsv / blacklist.tsv  fromIdx, toIdx
        ''' cpt.tsv                        每个节点的高斯 CPD 参数
        ''' prior_edges.tsv                TF, target, regulationType, confidence, evidence
        ''' expression/genes.txt
        ''' expression/samples.txt
        ''' expression/timepoints.bin      二进制 double 向量
        ''' expression/matrix.bin          二进制 nG, nS, double[nG*nS]（行优先）
        ''' structure.txt                  结构学习统计量
        ''' parameter.txt                  参数学习统计量
        ''' external/expression.tsv        外部导入的转录组（gene, value）
        ''' external/evidence.tsv          派生出的观测证据（gene, value）
        ''' external/initial_state.bin     派生出的动态初始状态向量
        ''' ```
        ''' 
        ''' 说明：表达矩阵必须以二进制而非文本落盘。扰动分析入口在
        ''' <see cref="NormalizeData"/> 打开时会对 <see cref="ExpressionData"/> 调用
        ''' ``Standardize``，若缺失会直接空引用；而文本 G17 存储 2000×300 量级的矩阵
        ''' 会膨胀到十数 MB 且需要数十万次 Double.Parse，二进制块可以把它压回毫秒级。
        Public Sub SaveModel(file As Stream)
            If file Is Nothing Then
                Throw New ArgumentNullException(NameOf(file))
            End If
            If FittedNetwork Is Nothing Then
                Throw New InvalidOperationException("当前工作流尚未完成结构学习，没有可以被导出的贝叶斯网络模型。")
            End If

            Dim net As BayesianNetwork = FittedNetwork
            Dim edges As List(Of (FromIdx As Integer, ToIdx As Integer)) =
                If(net.Nodes.Count > 0, net.GetEdges(), New List(Of (FromIdx As Integer, ToIdx As Integer))())
            Dim expr As GeneExpressionData = ExpressionData
            Dim prior As PriorNetwork = If(PriorNetwork, New PriorNetwork())

            Using zip As New ZipArchive(file, ZipArchiveMode.Create, leaveOpen:=True)
                Call WriteText(zip, "meta.txt", Sub(w)
                                                    w.WriteLine($"version={ModelFormatVersion}")
                                                    w.WriteLine($"nodes={net.Nodes.Count}")
                                                    w.WriteLine($"edges={edges.Count}")
                                                    w.WriteLine($"genes={If(expr Is Nothing, 0, expr.NGene)}")
                                                    w.WriteLine($"samples={If(expr Is Nothing, 0, expr.NSample)}")
                                                    w.WriteLine($"has_expr={If(expr Is Nothing, 0, 1)}")
                                                    w.WriteLine($"has_prior={If(prior.Edges.Count > 0, 1, 0)}")
                                                    w.WriteLine($"has_struct={If(StructureResult Is Nothing, 0, 1)}")
                                                    w.WriteLine($"has_param={If(ParameterResult Is Nothing, 0, 1)}")
                                                    w.WriteLine($"has_external={If(ExternalExpression Is Nothing, 0, 1)}")
                                                End Sub)

                Call WriteText(zip, "settings.txt", Sub(w)
                                                        w.WriteLine($"NormalizeData={NormalizeData}")
                                                        w.WriteLine($"NSamples={NSamples}")
                                                        w.WriteLine($"RandomSeed={RandomSeed}")
                                                        w.WriteLine($"Strict={Strict}")
                                                        w.WriteLine($"Algorithm={StructureParams.Algorithm.ToString()}")
                                                        w.WriteLine($"Alpha={Num(StructureParams.Alpha)}")
                                                        w.WriteLine($"MaxParents={StructureParams.MaxParents}")
                                                        w.WriteLine($"TabuLength={StructureParams.TabuLength}")
                                                        w.WriteLine($"MaxIterations={StructureParams.MaxIterations}")
                                                        w.WriteLine($"BICPenalty={Num(StructureParams.BICPenalty)}")
                                                        w.WriteLine($"UseWhitelist={StructureParams.UseWhitelist}")
                                                        w.WriteLine($"UseBlacklist={StructureParams.UseBlacklist}")
                                                        w.WriteLine($"StructRandomSeed={StructureParams.RandomSeed}")
                                                    End Sub)

                Call WriteText(zip, "nodes.txt", Sub(w)
                                                     For Each node As BnNode In net.Nodes
                                                         w.WriteLine(Sanitize(node.Name))
                                                     Next
                                                 End Sub)

                Call WriteText(zip, "edges.tsv", Sub(w) Call WriteEdgeIndex(w, edges))
                Call WriteText(zip, "whitelist.tsv", Sub(w) Call WriteEdgeIndex(w, net.Whitelist))
                Call WriteText(zip, "blacklist.tsv", Sub(w) Call WriteEdgeIndex(w, net.Blacklist))

                Call WriteText(zip, "cpt.tsv", Sub(w)
                                                   For i As Integer = 0 To net.Nodes.Count - 1
                                                       Dim cpd As BnCPD = net.Nodes(i).CPD

                                                       If cpd IsNot Nothing Then
                                                           w.WriteLine(String.Join(vbTab, {
                                                               i.ToString(CultureInfo.InvariantCulture),
                                                               Num(cpd.Intercept),
                                                               JoinNums(cpd.Coeffs),
                                                               JoinInts(cpd.ParentIndices),
                                                               Num(cpd.ResidualSD),
                                                               Num(cpd.ResidualVariance),
                                                               Num(cpd.RSquared),
                                                               Num(cpd.BIC),
                                                               cpd.NSamples.ToString(CultureInfo.InvariantCulture)
                                                           }))
                                                       End If
                                                   Next
                                               End Sub)

                Call WriteText(zip, "prior_edges.tsv", Sub(w)
                                                           For Each e As RegulatoryEdge In prior.Edges
                                                               w.WriteLine(String.Join(vbTab, {
                                                                   Sanitize(e.TF),
                                                                   Sanitize(e.TargetGene),
                                                                   CInt(e.RegulationType).ToString(CultureInfo.InvariantCulture),
                                                                   Num(e.Confidence),
                                                                   Sanitize(e.Evidence)
                                                               }))
                                                           Next
                                                       End Sub)

                If expr IsNot Nothing Then
                    Call WriteText(zip, "expression/genes.txt", Sub(w)
                                                                    For Each g As String In If(expr.GeneNames, New String() {})
                                                                        w.WriteLine(Sanitize(g))
                                                                    Next
                                                                End Sub)
                    Call WriteText(zip, "expression/samples.txt", Sub(w)
                                                                      For Each s As String In If(expr.SampleNames, New String() {})
                                                                          w.WriteLine(Sanitize(s))
                                                                      Next
                                                                  End Sub)
                    Call WriteDoubles(zip, "expression/timepoints.bin", expr.TimePoints)
                    Call WriteMatrix(zip, "expression/matrix.bin", expr.Matrix)
                End If

                If StructureResult IsNot Nothing Then
                    Dim hist As List(Of Double) = If(StructureResult.BICHistory, New List(Of Double)())

                    Call WriteText(zip, "structure.txt", Sub(w)
                                                             w.WriteLine($"FinalBIC={Num(StructureResult.FinalBIC)}")
                                                             w.WriteLine($"Iterations={StructureResult.Iterations}")
                                                             w.WriteLine($"ElapsedMs={StructureResult.ElapsedMs}")
                                                             w.WriteLine($"bic_history={JoinNums(hist.ToArray())}")
                                                         End Sub)
                End If

                If ParameterResult IsNot Nothing Then
                    Call WriteText(zip, "parameter.txt", Sub(w)
                                                             w.WriteLine($"TotalLogLikelihood={Num(ParameterResult.TotalLogLikelihood)}")
                                                             w.WriteLine($"TotalBIC={Num(ParameterResult.TotalBIC)}")
                                                             w.WriteLine($"AverageRSquared={Num(ParameterResult.AverageRSquared)}")
                                                             w.WriteLine($"ElapsedMs={ParameterResult.ElapsedMs}")
                                                         End Sub)
                End If

                If ExternalExpression IsNot Nothing Then
                    Call WriteText(zip, "external/expression.tsv", Sub(w) Call WriteMap(w, ExternalExpression))
                End If
                If ExternalEvidence IsNot Nothing Then
                    Call WriteText(zip, "external/evidence.tsv", Sub(w) Call WriteMap(w, ExternalEvidence))
                End If
                If ExternalInitialState IsNot Nothing Then
                    Call WriteDoubles(zip, "external/initial_state.bin", ExternalInitialState)
                End If
            End Using

            Call $"[BNLearnWorkflow] 模型已导出: nodes={net.Nodes.Count}, edges={edges.Count}, genes={If(expr Is Nothing, 0, expr.NGene)}".info
        End Sub

        ''' <summary>
        ''' load trained model from zip file
        ''' </summary>
        ''' <param name="file">zip 压缩包输入流（由 R# 的 ``readBin`` 传入，调用方负责释放）</param>
        ''' <returns>
        ''' 还原后的工作流对象，网络结构、CPD 参数与训练表达矩阵均与保存前一致，
        ''' 可直接用于 knockouts / overexpress / knockdown 等虚拟扰动分析。
        ''' </returns>
        Public Shared Function LoadModel(file As Stream) As BNLearnWorkflow
            If file Is Nothing Then
                Throw New ArgumentNullException(NameOf(file))
            End If

            Dim workflow As New BNLearnWorkflow()
            Dim net As New BayesianNetwork()
            Dim meta As Dictionary(Of String, String)

            Using zip As New ZipArchive(file, ZipArchiveMode.Read, leaveOpen:=True)
                meta = ReadMeta(GetEntry(zip, "meta.txt"))

                Dim verText As String = Nothing
                Dim version As Integer = 0

                If meta Is Nothing OrElse Not meta.TryGetValue("version", verText) Then
                    Throw New InvalidDataException("bnlearn 模型文件缺少版本信息，可能不是有效的模型压缩包。")
                End If
                If Not Integer.TryParse(verText, NumberStyles.Integer, CultureInfo.InvariantCulture, version) OrElse
                    version <> ModelFormatVersion Then

                    Throw New InvalidDataException($"bnlearn 模型文件版本不匹配：文件为 {verText}，当前程序支持 {ModelFormatVersion}。")
                End If

                ' 工作流开关与结构学习参数
                Dim settings As Dictionary(Of String, String) = ReadMeta(GetEntry(zip, "settings.txt"))

                workflow.NormalizeData = GetBool(settings, "NormalizeData", True)
                workflow.NSamples = GetInt(settings, "NSamples", 10000)
                workflow.RandomSeed = GetInt(settings, "RandomSeed", 42)
                workflow.Strict = GetBool(settings, "Strict", True)

                Dim algoText As String = GetValue(settings, "Algorithm")
                Dim algo As StructureLearning.StructureAlgorithm = StructureLearning.StructureAlgorithm.MMHC

                If algoText.Length > 0 AndAlso Enum.TryParse(Of StructureLearning.StructureAlgorithm)(algoText, True, algo) Then
                    workflow.StructureParams.Algorithm = algo
                End If

                workflow.StructureParams.Alpha = GetDouble(settings, "Alpha", 0.05)
                workflow.StructureParams.MaxParents = GetInt(settings, "MaxParents", 5)
                workflow.StructureParams.TabuLength = GetInt(settings, "TabuLength", 20)
                workflow.StructureParams.MaxIterations = GetInt(settings, "MaxIterations", 500)
                workflow.StructureParams.BICPenalty = GetDouble(settings, "BICPenalty", 1.0)
                workflow.StructureParams.UseWhitelist = GetBool(settings, "UseWhitelist", True)
                workflow.StructureParams.UseBlacklist = GetBool(settings, "UseBlacklist", True)
                workflow.StructureParams.RandomSeed = GetInt(settings, "StructRandomSeed", 42)

                ' 网络拓扑：先按节点文件顺序建节点（AddNode 会自动维护邻接矩阵与 NameToIndex），
                ' 再逐条加边（AddEdge 会同步维护 Parents / Children）
                For Each name As String In ReadNames(GetEntry(zip, "nodes.txt"))
                    Call net.AddNode(name)
                Next

                For Each line As String In ReadLines(GetEntry(zip, "edges.tsv"))
                    Dim p As String() = line.Split(New String() {vbTab}, StringSplitOptions.None)
                    Dim fromIdx As Integer = 0, toIdx As Integer = 0

                    If p.Length >= 2 AndAlso
                        Integer.TryParse(p(0), NumberStyles.Integer, CultureInfo.InvariantCulture, fromIdx) AndAlso
                        Integer.TryParse(p(1), NumberStyles.Integer, CultureInfo.InvariantCulture, toIdx) Then

                        Call net.AddEdge(fromIdx, toIdx)
                    End If
                Next

                Call ReadIndexPairs(GetEntry(zip, "whitelist.tsv"), net.Whitelist)
                Call ReadIndexPairs(GetEntry(zip, "blacklist.tsv"), net.Blacklist)

                ' CPD 参数
                For Each line As String In ReadLines(GetEntry(zip, "cpt.tsv"))
                    Dim p As String() = line.Split(New String() {vbTab}, StringSplitOptions.None)
                    Dim idx As Integer = 0

                    If p.Length < 9 Then Continue For
                    If Not Integer.TryParse(p(0), NumberStyles.Integer, CultureInfo.InvariantCulture, idx) Then Continue For
                    If idx < 0 OrElse idx >= net.Nodes.Count Then Continue For

                    net.Nodes(idx).CPD = New BnCPD With {
                        .NodeIndex = idx,
                        .Intercept = ParseNum(p(1)),
                        .Coeffs = ParseNums(p(2)),
                        .ParentIndices = ParseInts(p(3)),
                        .ResidualSD = ParseNum(p(4)),
                        .ResidualVariance = ParseNum(p(5)),
                        .RSquared = ParseNum(p(6)),
                        .BIC = ParseNum(p(7)),
                        .NSamples = CInt(Math.Round(ParseNum(p(8))))
                    }
                Next

                workflow.FittedNetwork = net

                ' 先验网络：走 AddEdge 以自动重建 TFNames / TargetNames 两个索引
                Dim prior As New PriorNetwork()

                For Each line As String In ReadLines(GetEntry(zip, "prior_edges.tsv"))
                    Dim p As String() = line.Split(New String() {vbTab}, StringSplitOptions.None)

                    If p.Length < 2 Then Continue For

                    Dim regType As Effector = Effector.Activator
                    Dim regCode As Integer = 0

                    If p.Length > 2 AndAlso
                        Integer.TryParse(p(2), NumberStyles.Integer, CultureInfo.InvariantCulture, regCode) Then

                        regType = CType(regCode, Effector)
                    End If

                    Dim confidence As Double = If(p.Length > 3 AndAlso p(3).Length > 0, ParseNum(p(3)), 1.0)
                    Dim evidence As String = If(p.Length > 4, p(4), "")

                    Call prior.AddEdge(p(0), p(1), regType, confidence, evidence)
                Next

                workflow.PriorNetwork = prior

                ' 训练表达矩阵
                If GetBool(meta, "has_expr", False) Then
                    Dim geneNames As String() = ReadNames(GetEntry(zip, "expression/genes.txt"))
                    Dim sampleNames As String() = ReadNames(GetEntry(zip, "expression/samples.txt"))
                    Dim times As Double() = ReadDoubles(GetEntry(zip, "expression/timepoints.bin"))
                    Dim matrix As Double(,) = ReadMatrix(GetEntry(zip, "expression/matrix.bin"))
                    Dim nG As Integer = GetInt(meta, "genes", geneNames.Length)
                    Dim nS As Integer = GetInt(meta, "samples", sampleNames.Length)

                    If matrix Is Nothing Then
                        matrix = New Double(Math.Max(nG, 1) - 1, Math.Max(nS, 1) - 1) {}
                    End If
                    If times Is Nothing OrElse times.Length <> nS Then
                        times = Enumerable.Repeat(0.0, Math.Max(nS, 0)).ToArray()
                    End If

                    workflow.ExpressionData = New GeneExpressionData() With {
                        .GeneNames = geneNames,
                        .SampleNames = sampleNames,
                        .Matrix = matrix,
                        .TimePoints = times
                    }
                End If

                ' 学习结果：Network 指向同一个还原出来的网络实例，避免持有多份副本
                If GetBool(meta, "has_struct", False) Then
                    Dim st As Dictionary(Of String, String) = ReadMeta(GetEntry(zip, "structure.txt"))

                    workflow.StructureResult = New StructureLearning.StructureLearningResult() With {
                        .Network = net,
                        .FinalBIC = GetDouble(st, "FinalBIC", 0),
                        .Iterations = GetInt(st, "Iterations", 0),
                        .ElapsedMs = CLng(GetDouble(st, "ElapsedMs", 0)),
                        .BICHistory = New List(Of Double)(ParseNums(GetValue(st, "bic_history")))
                    }
                End If

                If GetBool(meta, "has_param", False) Then
                    Dim pm As Dictionary(Of String, String) = ReadMeta(GetEntry(zip, "parameter.txt"))

                    workflow.ParameterResult = New ParameterLearningResult() With {
                        .Network = net,
                        .TotalLogLikelihood = GetDouble(pm, "TotalLogLikelihood", 0),
                        .TotalBIC = GetDouble(pm, "TotalBIC", 0),
                        .AverageRSquared = GetDouble(pm, "AverageRSquared", 0),
                        .ElapsedMs = CLng(GetDouble(pm, "ElapsedMs", 0))
                    }
                End If

                ' 外部导入的转录组及其派生态
                Dim externalEntry As ZipArchiveEntry = GetEntry(zip, "external/expression.tsv")
                Dim evidenceEntry As ZipArchiveEntry = GetEntry(zip, "external/evidence.tsv")
                Dim initialEntry As ZipArchiveEntry = GetEntry(zip, "external/initial_state.bin")

                If externalEntry IsNot Nothing Then
                    workflow.ExternalExpression = ReadMap(externalEntry)
                End If
                If evidenceEntry IsNot Nothing Then
                    workflow.ExternalEvidence = ReadMap(evidenceEntry)
                End If
                If initialEntry IsNot Nothing Then
                    workflow.ExternalInitialState = ReadDoubles(initialEntry)
                End If
            End Using

            Call $"[BNLearnWorkflow] 模型已载入: nodes={net.Nodes.Count}, edges={net.EdgeCount}, genes={If(workflow.ExpressionData Is Nothing, 0, workflow.ExpressionData.NGene)}".info

            Return workflow
        End Function

        ' ==================== zip 持久化辅助 ====================

        ''' <summary>将一段文本写入 zip 包内的指定条目</summary>
        Private Shared Sub WriteText(zip As ZipArchive, name As String, write As Action(Of TextWriter))
            Dim entry As ZipArchiveEntry = zip.CreateEntry(name, CompressionLevel.Optimal)

            Using w As New StreamWriter(entry.Open())
                Call write(w)
            End Using
        End Sub

        ''' <summary>按条目名查找 zip 内的条目（路径分隔符统一为 /，大小写不敏感）</summary>
        ''' <returns>不存在时返回 Nothing，调用方按缺省值降级处理</returns>
        Private Shared Function GetEntry(zip As ZipArchive, name As String) As ZipArchiveEntry
            Dim target As String = name.Replace("\"c, "/"c)

            For Each e As ZipArchiveEntry In zip.Entries
                If String.Equals(e.FullName.Replace("\"c, "/"c), target, StringComparison.OrdinalIgnoreCase) Then
                    Return e
                End If
            Next

            Return Nothing
        End Function

        ''' <summary>读取文本条目的全部非空行</summary>
        Private Shared Function ReadLines(entry As ZipArchiveEntry) As String()
            If entry Is Nothing Then
                Return New String() {}
            End If

            Dim lines As New List(Of String)

            Using sr As New StreamReader(entry.Open())
                Do While Not sr.EndOfStream
                    Dim line As String = sr.ReadLine()

                    If Not String.IsNullOrWhiteSpace(line) Then
                        lines.Add(line)
                    End If
                Loop
            End Using

            Return lines.ToArray()
        End Function

        ''' <summary>
        ''' 读取名称清单（节点名 / 基因名 / 样本名）。
        ''' 与 <see cref="ReadLines"/> 不同，这里保留空行，否则行号会与节点索引错位，
        ''' 仅剔除文件末尾换行所产生的那一个空行。
        ''' </summary>
        Private Shared Function ReadNames(entry As ZipArchiveEntry) As String()
            If entry Is Nothing Then
                Return New String() {}
            End If

            Dim lines As New List(Of String)

            Using sr As New StreamReader(entry.Open())
                Do While Not sr.EndOfStream
                    Call lines.Add(sr.ReadLine())
                Loop
            End Using

            If lines.Count > 0 AndAlso lines(lines.Count - 1).Length = 0 Then
                Call lines.RemoveAt(lines.Count - 1)
            End If

            Return lines.ToArray()
        End Function

        ''' <summary>读取 key=value 形式的元数据条目</summary>
        Private Shared Function ReadMeta(entry As ZipArchiveEntry) As Dictionary(Of String, String)
            Dim meta As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

            For Each line As String In ReadLines(entry)
                Dim i As Integer = line.IndexOf("="c)

                If i <= 0 Then Continue For

                meta(line.Substring(0, i).Trim()) = line.Substring(i + 1).Trim()
            Next

            Return meta
        End Function

        ''' <summary>写出索引边列表（whitelist / blacklist / 网络边）</summary>
        Private Shared Sub WriteEdgeIndex(w As TextWriter, edges As IEnumerable(Of (FromIdx As Integer, ToIdx As Integer)))
            If edges Is Nothing Then Return

            For Each e In edges
                w.WriteLine($"{e.FromIdx}{vbTab}{e.ToIdx}")
            Next
        End Sub

        ''' <summary>读回索引边列表</summary>
        Private Shared Sub ReadIndexPairs(entry As ZipArchiveEntry, list As List(Of (FromIdx As Integer, ToIdx As Integer)))
            For Each line As String In ReadLines(entry)
                Dim p As String() = line.Split(New String() {vbTab}, StringSplitOptions.None)
                Dim fromIdx As Integer = 0, toIdx As Integer = 0

                If p.Length >= 2 AndAlso
                    Integer.TryParse(p(0), NumberStyles.Integer, CultureInfo.InvariantCulture, fromIdx) AndAlso
                    Integer.TryParse(p(1), NumberStyles.Integer, CultureInfo.InvariantCulture, toIdx) Then

                    Call list.Add((fromIdx, toIdx))
                End If
            Next
        End Sub

        ''' <summary>写出基因名 → 表达值 的映射表</summary>
        Private Shared Sub WriteMap(w As TextWriter, map As Dictionary(Of String, Double))
            If map Is Nothing Then Return

            For Each kv In map
                w.WriteLine($"{Sanitize(kv.Key)}{vbTab}{Num(kv.Value)}")
            Next
        End Sub

        ''' <summary>读回基因名 → 表达值 的映射表（键大小写不敏感，与训练期一致）</summary>
        Private Shared Function ReadMap(entry As ZipArchiveEntry) As Dictionary(Of String, Double)
            Dim map As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

            For Each line As String In ReadLines(entry)
                Dim p As String() = line.Split(New String() {vbTab}, StringSplitOptions.None)

                If p.Length >= 2 Then
                    map(p(0)) = ParseNum(p(1))
                End If
            Next

            Return map
        End Function

        ''' <summary>
        ''' 以二进制块写出 double 向量（Int32 长度 + 数据体）。
        ''' 大数组走文本会膨胀数倍且需要逐值 Parse，这里直接落原始 8 字节double。
        ''' </summary>
        Private Shared Sub WriteDoubles(zip As ZipArchive, name As String, values As Double())
            Dim entry As ZipArchiveEntry = zip.CreateEntry(name, CompressionLevel.Optimal)

            Using out As New BinaryWriter(entry.Open())
                If values Is Nothing Then
                    out.Write(0)
                Else
                    out.Write(values.Length)

                    For Each x As Double In values
                        out.Write(x)
                    Next
                End If
            End Using
        End Sub

        ''' <summary>读回 <see cref="WriteDoubles"/> 写出的 double 向量</summary>
        Private Shared Function ReadDoubles(entry As ZipArchiveEntry) As Double()
            If entry Is Nothing Then
                Return New Double() {}
            End If

            Using input As New BinaryReader(entry.Open())
                Dim n As Integer = input.ReadInt32()

                If n <= 0 Then
                    Return New Double() {}
                End If

                Dim buf As Double() = New Double(n - 1) {}

                For i As Integer = 0 To n - 1
                    buf(i) = input.ReadDouble()
                Next

                Return buf
            End Using
        End Function

        ''' <summary>
        ''' 以二进制块写出 [gene, sample] 表达矩阵：Int32 nG + Int32 nS + 行优先（gene-major）数据体。
        ''' </summary>
        Private Shared Sub WriteMatrix(zip As ZipArchive, name As String, m As Double(,))
            Dim entry As ZipArchiveEntry = zip.CreateEntry(name, CompressionLevel.Optimal)
            Dim nG As Integer = If(m Is Nothing, 0, m.GetLength(0))
            Dim nS As Integer = If(m Is Nothing, 0, m.GetLength(1))

            Using out As New BinaryWriter(entry.Open())
                out.Write(nG)
                out.Write(nS)

                For i As Integer = 0 To nG - 1
                    For j As Integer = 0 To nS - 1
                        out.Write(m(i, j))
                    Next
                Next
            End Using
        End Sub

        ''' <summary>读回 <see cref="WriteMatrix"/> 写出的表达矩阵，条目缺失或为空时返回 Nothing</summary>
        Private Shared Function ReadMatrix(entry As ZipArchiveEntry) As Double(,)
            If entry Is Nothing Then
                Return Nothing
            End If

            Using input As New BinaryReader(entry.Open())
                Dim nG As Integer = input.ReadInt32()
                Dim nS As Integer = input.ReadInt32()

                If nG <= 0 OrElse nS <= 0 Then
                    Return Nothing
                End If

                Dim m As Double(,) = New Double(nG - 1, nS - 1) {}

                For i As Integer = 0 To nG - 1
                    For j As Integer = 0 To nS - 1
                        m(i, j) = input.ReadDouble()
                    Next
                Next

                Return m
            End Using
        End Function

        ''' <summary>以 G17 无损格式写出数值（固定使用不变区域文化，避免受系统区域设置影响）</summary>
        Private Shared Function Num(d As Double) As String
            Return d.ToString("G17", CultureInfo.InvariantCulture)
        End Function

        ''' <summary>解析 <see cref="Num"/> 写出的数值，解析失败时返回 0 而不是抛出</summary>
        Private Shared Function ParseNum(s As String) As Double
            Dim d As Double = 0

            Call Double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, d)

            Return d
        End Function

        Private Shared Function JoinNums(values As Double()) As String
            If values Is Nothing Then Return ""
            Return String.Join(",", values.Select(Function(d) Num(d)))
        End Function

        Private Shared Function JoinInts(values As Integer()) As String
            If values Is Nothing Then Return ""
            Return String.Join(",", values.Select(Function(i) i.ToString(CultureInfo.InvariantCulture)))
        End Function

        Private Shared Function ParseNums(s As String) As Double()
            If String.IsNullOrWhiteSpace(s) Then Return New Double() {}
            Return s.Split(","c).Where(Function(x) x.Length > 0).Select(Function(x) ParseNum(x)).ToArray()
        End Function

        Private Shared Function ParseInts(s As String) As Integer()
            If String.IsNullOrWhiteSpace(s) Then Return New Integer() {}
            Return s.Split(","c).Where(Function(x) x.Length > 0).Select(Function(x) CInt(Math.Round(ParseNum(x)))).ToArray()
        End Function

        ''' <summary>清理文本字段中的制表符与换行符，避免破坏 TSV / 逐行文本格式</summary>
        Private Shared Function Sanitize(s As String) As String
            If String.IsNullOrEmpty(s) Then Return ""
            Return s.Replace(vbTab, " ").Replace(vbCr, " ").Replace(vbLf, " ")
        End Function

        ''' <summary>取元数据字符串值，缺失时返回缺省值</summary>
        Private Shared Function GetValue(meta As Dictionary(Of String, String), key As String, Optional def As String = "") As String
            Dim value As String = Nothing

            If meta IsNot Nothing AndAlso meta.TryGetValue(key, value) Then
                Return value
            End If

            Return def
        End Function

        ''' <summary>取元数据的布尔值，兼容 True/False 与 1/0 两种写法</summary>
        Private Shared Function GetBool(meta As Dictionary(Of String, String), key As String, def As Boolean) As Boolean
            Dim s As String = GetValue(meta, key)
            Dim b As Boolean = def

            If Boolean.TryParse(s, b) Then
                Return b
            End If

            Dim n As Integer = 0

            If Integer.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, n) Then
                Return n <> 0
            End If

            Return def
        End Function

        Private Shared Function GetInt(meta As Dictionary(Of String, String), key As String, def As Integer) As Integer
            Dim n As Integer = def

            If Integer.TryParse(GetValue(meta, key), NumberStyles.Integer, CultureInfo.InvariantCulture, n) Then
                Return n
            End If

            Return def
        End Function

        Private Shared Function GetDouble(meta As Dictionary(Of String, String), key As String, def As Double) As Double
            Dim d As Double = def

            If Double.TryParse(GetValue(meta, key), NumberStyles.Float, CultureInfo.InvariantCulture, d) Then
                Return d
            End If

            Return def
        End Function

    End Class

End Namespace

