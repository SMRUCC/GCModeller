#Region "Microsoft.VisualBasic::c4b5e78654800cd954c4169d4f7e993d, sub-system\CellPhenotype\GeneRegulatoryNetwork.vb"

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

'   Total Lines: 424
'    Code Lines: 232 (54.72%)
' Comment Lines: 130 (30.66%)
'    - Xml Docs: 81.54%
' 
'   Blank Lines: 62 (14.62%)
'     File Size: 22.44 KB


' Module GeneRegulatoryNetwork
' 
'     Function: BuildBNNetwork, BuildDBN, BuildExpressionGRN, BuildPriorNetwork, BuildRegulatoryLinks
'               InferEffector, RunPipeline, StateToValue, ToRegulatoryLink, ToTimeSeries
'               TrainAndIntervene, VirtualKnockdown
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.DBN

''' <summary>
''' WGCNA 共表达网络与 BNLearn 动态贝叶斯网络（DBN）之间的桥接模块。
'''
''' 本模块将 <see cref="NetworkGraph"/> 形式的 WGCNA 无向共表达网络，按照
''' 转录因子（TF）注释结果定向为有向调控先验，导入 BNLearn 的
''' 高斯贝叶斯网络工作流（<see cref="BNLearnWorkflow"/>）与动态贝叶斯网络
''' （<see cref="DynamicBayesianNetwork"/>），并支持基于时间序列表达矩阵
''' 进行参数学习与基因表达虚拟敲降的级联模拟计算。
''' </summary>
Public Module GeneRegulatoryNetwork

    ''' <summary>
    ''' 先验知识的证据来源标记。
    ''' </summary>
    Private Const EVIDENCE As String = "WGCNA co-expression network"

    ''' <summary>
    ''' 根据相关系数符号推断调控类型：正相关为激活因子，负相关为抑制因子。
    ''' </summary>
    ''' <param name="weight">WGCNA 网络边的相关系数（通常为 Pearson / Spearman 相关系数）。</param>
    ''' <returns>激活或抑制 effector 类型。</returns>
    Public Function InferEffector(weight As Double) As Effector
        If weight >= 0 Then
            Return Effector.Activator
        Else
            Return Effector.Inhibitor
        End If
    End Function

    ''' <summary>
    ''' 根据 WGCNA 共表达网络与 TF 注释结果，构建 BNLearn 工作流的拓扑先验网络。
    '''
    ''' 调控方向严格依据 TF 注释确定：仅当一条边的其中一端是 TF、另一端不是 TF
    ''' 时，才生成单向调控边（TF → 非 TF）；权重符号决定激活 / 抑制，权重绝对值
    ''' 作为先验置信度。两端同为 TF 或同为非 TF 的边无法由共表达确定方向，将被
    ''' 跳过并在日志中给出统计信息。
    ''' </summary>
    ''' <param name="wgcna">WGCNA "CorrelationNetwork.ExportGraph" 生成的共表达网络。</param>
    ''' <param name="TF">转录因子（上游调控因子）基因名称注释集合。</param>
    ''' <returns>定向后的 BNLearn 先验调控网络。</returns>
    Public Function BuildPriorNetwork(wgcna As NetworkGraph, TF As HashSet(Of String)) As Core.PriorNetwork
        Dim prior As New Core.PriorNetwork
        Dim skipped As Integer = 0
        Dim directed As Integer = 0

        If wgcna Is Nothing Then
            Throw New ArgumentNullException(NameOf(wgcna), "WGCNA 共表达网络不能为空")
        End If
        If TF Is Nothing OrElse TF.Count = 0 Then
            Throw New ArgumentException("TF 注释列表不能为空，否则无法构建调控方向", NameOf(TF))
        End If

        For Each e As Edge In wgcna.graphEdges
            Dim a As String = e.U.label
            Dim b As String = e.V.label

            If TF.Contains(a) AndAlso Not TF.Contains(b) Then
                prior.AddEdge(a, b, InferEffector(e.weight), Math.Abs(e.weight), EVIDENCE)
                directed += 1
            ElseIf TF.Contains(b) AndAlso Not TF.Contains(a) Then
                prior.AddEdge(b, a, InferEffector(e.weight), Math.Abs(e.weight), EVIDENCE)
                directed += 1
            Else
                ' 两端同为 TF 或同为非 TF：方向无法由共表达确定，跳过
                skipped += 1
            End If
        Next

        Call VBDebugger.WriteLine($"WGCNAGRN.BuildPriorNetwork: 共 {wgcna.graphEdges.Count} 条边，定向 {directed} 条，跳过 {skipped} 条（无法由 TF 注释确定方向）")

        Return prior
    End Function

    ''' <summary>
    ''' 将一条有向调控关系转换为 DBN 的 <see cref="RegulatoryLink"/> 拓扑链路。
    ''' </summary>
    Private Function ToRegulatoryLink(tf As String, target As String, weight As Double) As RegulatoryLink
        Return New RegulatoryLink With {
            .TF_id = tf,
            .target_operon = target,
            .regulate_genes = {target},
            .effector = New Dictionary(Of String, Effector) From {{target, InferEffector(weight)}}
        }
    End Function

    ''' <summary>
    ''' 根据 WGCNA 共表达网络与 TF 注释结果，构建 DBN 拓扑链路集合
    ''' （<see cref="RegulatoryLink"/>）。方向与 <see cref="BuildPriorNetwork"/> 一致。
    ''' </summary>
    ''' <param name="wgcna">WGCNA 共表达网络。</param>
    ''' <param name="TF">转录因子基因名称注释集合。</param>
    ''' <returns>DBN 调控链路集合（TF → 非 TF 单向）。</returns>
    Public Function BuildRegulatoryLinks(Of IE As INetworkEdge)(wgcna As IEnumerable(Of IE), TF As HashSet(Of String)) As IEnumerable(Of RegulatoryLink)
        Dim links As New List(Of RegulatoryLink)

        If wgcna Is Nothing Then
            Throw New ArgumentNullException(NameOf(wgcna), "WGCNA 共表达网络不能为空")
        End If
        If TF Is Nothing OrElse TF.Count = 0 Then
            Throw New ArgumentException("TF 注释列表不能为空，否则无法构建调控方向", NameOf(TF))
        End If

        For Each e As IE In wgcna
            Dim a As String = e.source
            Dim b As String = e.target

            If TF.Contains(a) AndAlso Not TF.Contains(b) Then
                links.Add(ToRegulatoryLink(a, b, e.value))
            ElseIf TF.Contains(b) AndAlso Not TF.Contains(a) Then
                links.Add(ToRegulatoryLink(b, a, e.value))
            End If
        Next

        Return links
    End Function

    ''' <summary>
    ''' 主入口：将 WGCNA 共表达网络按 TF 注释定向为拓扑先验，并装配到
    ''' BNLearn 工作流中返回。调用方随后可设置工作流的
    ''' <see cref="BNLearnWorkflow.ExpressionData"/> 并执行结构学习与参数学习。
    ''' </summary>
    ''' <param name="wgcna">WGCNA "CorrelationNetwork.ExportGraph" 生成的共表达网络。</param>
    ''' <param name="TF">转录因子（上游调控因子）基因名称数组。</param>
    ''' <returns>已注入拓扑先验网络的 BNLearn 工作流实例。</returns>
    Public Function BuildBNNetwork(wgcna As NetworkGraph, TF As String()) As BNLearnWorkflow
        Dim tfSet As New HashSet(Of String)(TF, StringComparer.OrdinalIgnoreCase)
        Dim prior As Core.PriorNetwork = BuildPriorNetwork(wgcna, tfSet)
        Dim workflow As New BNLearnWorkflow With {
            .PriorNetwork = prior
        }

        Call VBDebugger.WriteLine($"WGCNAGRN.BuildBNNetwork: 已构建 BNLearn 工作流，先验边数 = {prior.Edges.Count}（TF 注释 {tfSet.Count} 个）")

        Return workflow
    End Function

    ''' <summary>
    ''' 将表达矩阵（时间序列）转换为 DBN 参数学习所需的输入格式。
    '''
    ''' 每个唯一时间点被聚合为一个基因丰度字典（该时间点下全部生物学重复样本的
    ''' 均值），按时间顺序排列形成时间序列轨迹。要求至少包含 2 个时间点。
    ''' </summary>
    ''' <param name="expr">已加载的 <see cref="GeneExpressionData"/> 表达矩阵。</param>
    ''' <returns>按时间排序的基因丰度字典列表，供 <see cref="DynamicBayesianNetwork.LearnParameters"/> 使用。</returns>
    Public Function ToTimeSeries(expr As Core.GeneExpressionData) As List(Of Dictionary(Of String, Double))
        If expr Is Nothing Then
            Throw New ArgumentNullException(NameOf(expr), "表达矩阵不能为空")
        End If

        Dim timePoints As Double() = expr.UniqueTimePoints
        If timePoints Is Nothing OrElse timePoints.Length < 2 Then
            Throw New InvalidOperationException("表达矩阵需要至少包含 2 个时间点以进行动态贝叶斯网络参数学习")
        End If

        Dim series As New List(Of Dictionary(Of String, Double))

        For Each tp As Double In timePoints
            ' 收集该时间点的所有样本索引
            Dim sampleIdx As Integer() = expr.TimePoints _
                .Select(Function(t, i) (time:=t, index:=i)) _
                .Where(Function(x) Math.Abs(x.time - tp) < 0.0000000001) _
                .Select(Function(x) x.index) _
                .ToArray()

            If sampleIdx.Length = 0 Then
                Call VBDebugger.WriteLine($"WGCNAGRN.ToTimeSeries: 时间点 {tp} 无样本，已跳过")
                Continue For
            End If

            Dim frame As New Dictionary(Of String, Double)

            For gi As Integer = 0 To expr.NGene - 1
                Dim gene As String = expr.GeneNames(gi)
                Dim offset As Integer = gi
                Dim avg As Double = Aggregate i As Integer
                                    In sampleIdx
                                    Into Average(expr.Matrix(offset, i))
                frame(gene) = avg
            Next

            Call series.Add(frame)
        Next

        Call VBDebugger.WriteLine($"WGCNAGRN.ToTimeSeries: 构造时间序列，共 {series.Count} 个时间点")

        Return series
    End Function

    ''' <summary>
    ''' 构建并拟合参数的动态贝叶斯网络：将 WGCNA 共表达网络按 TF 注释定向为
    ''' DBN 拓扑，并基于时间序列表达矩阵学习条件概率表（CPT）。
    ''' </summary>
    ''' <param name="wgcna">WGCNA 共表达网络。</param>
    ''' <param name="expr">时间序列表达矩阵。</param>
    ''' <param name="TF">转录因子基因名称数组。</param>
    ''' <returns>已完成拓扑构建与参数学习的动态贝叶斯网络。</returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function BuildDBN(wgcna As NetworkGraph, expr As Core.GeneExpressionData, TF As String()) As DynamicBayesianNetwork
        Return BuildDBN(wgcna.graphEdges, expr, TF)
    End Function

    ''' <summary>
    ''' 构建并拟合参数的动态贝叶斯网络：将 WGCNA 共表达网络按 TF 注释定向为
    ''' DBN 拓扑，并基于时间序列表达矩阵学习条件概率表（CPT）。
    ''' </summary>
    ''' <param name="wgcna">WGCNA 共表达网络。</param>
    ''' <param name="expr">时间序列表达矩阵。</param>
    ''' <param name="TF">转录因子基因名称数组。</param>
    ''' <returns>已完成拓扑构建与参数学习的动态贝叶斯网络。</returns>
    Public Function BuildDBN(Of IE As INetworkEdge)(wgcna As IEnumerable(Of IE), expr As Core.GeneExpressionData, TF As String()) As DynamicBayesianNetwork
        Dim tfSet As New HashSet(Of String)(TF, StringComparer.OrdinalIgnoreCase)
        Dim links As IEnumerable(Of RegulatoryLink) = BuildRegulatoryLinks(wgcna, tfSet)

        If Not links.Any() Then
            Throw New InvalidOperationException("没有可定向的调控边，无法构建动态贝叶斯网络拓扑（请检查 TF 注释是否与网络节点匹配）")
        End If

        Dim dbn As New DynamicBayesianNetwork()
        Call dbn.BuildFromTopology(links)

        Call VBDebugger.WriteLine($"WGCNAGRN.BuildDBN: 拓扑构建完成，节点数 = {dbn.GetAllNodes().Count}，调控边 = {links.Count()}。开始参数学习...")

        Dim timeSeries As List(Of Dictionary(Of String, Double)) = ToTimeSeries(expr)
        dbn.LearnParameters(timeSeries)

        Call VBDebugger.WriteLine("WGCNAGRN.BuildDBN: 参数学习完成")

        Return dbn
    End Function

    ''' <summary>
    ''' 将离散状态映射为数值，便于以轨迹数组形式输出虚拟敲降模拟结果。
    ''' </summary>
    Private Function StateToValue(state As String) As Double
        Select Case state
            Case "Low" : Return 0.0
            Case "Medium" : Return 1.0
            Case "High" : Return 2.0
            Case Else : Return 1.0
        End Select
    End Function

    ''' <summary>
    ''' 在已构建并拟合参数的动态贝叶斯网络上，对指定基因执行虚拟敲降模拟。
    '''
    ''' 模拟逻辑：将目标基因节点状态强制固定为 ""Low""（敲降状态），并基于
    ''' 已学习的条件概率表多步推演下游基因状态的级联变化。返回每个基因随时间的
    ''' 离散状态数值化轨迹（Low=0, Medium=1, High=2）。
    ''' </summary>
    ''' <param name="dbn">已完成拓扑构建与参数学习的动态贝叶斯网络。</param>
    ''' <param name="gene">被虚拟敲降的目标基因名称。</param>
    ''' <param name="nSteps">级联推演的时间步数。</param>
    ''' <returns>基因名称 → 随时间变化的表达状态轨迹数组。</returns>
    Public Function VirtualKnockdown(dbn As DynamicBayesianNetwork, gene As String, nSteps As Integer) As Dictionary(Of String, Double())
        If dbn Is Nothing Then
            Throw New ArgumentNullException(NameOf(dbn), "动态贝叶斯网络不能为空")
        End If
        If String.IsNullOrEmpty(gene) Then
            Throw New ArgumentException("敲降目标基因名称不能为空", NameOf(gene))
        End If
        If nSteps <= 0 Then
            Throw New ArgumentException("级联推演步数必须为正整数", NameOf(nSteps))
        End If

        ' 初始化 TF 与代谢物的中性输入丰度
        Dim tfNodes As List(Of DBNNode) = dbn.GetAllNodes().Where(Function(n) n.NodeType = DBNNodeType.TranscriptionFactor).ToList()
        Dim metaboliteAbundances As New Dictionary(Of String, Double)
        Dim tfAbundances As New Dictionary(Of String, Double)

        For Each tf As DBNNode In tfNodes
            tfAbundances(tf.NodeId) = 1.0
        Next

        ' 轨迹记录：每个基因一条随时间变化的数值化状态数组
        Dim geneNodes As List(Of DBNNode) = dbn.GetGeneNodes()
        Dim trajectory As New Dictionary(Of String, Double())

        For Each g As DBNNode In geneNodes
            trajectory(g.NodeId) = New Double(nSteps - 1) {}
        Next

        ' 构建初始基因状态，并将目标基因强制置为 Low（敲降状态）
        Dim currentGeneStates As New Dictionary(Of String, String)
        For Each g As DBNNode In geneNodes
            currentGeneStates(g.NodeId) = "Medium"
        Next
        If Not currentGeneStates.ContainsKey(gene) Then
            Throw New ArgumentException($"敲降目标基因 '{gene}' 不在动态贝叶斯网络节点集合中", NameOf(gene))
        End If
        currentGeneStates(gene) = "Low"

        ' 记录初始（敲降）状态作为第 0 步基线
        For Each g As DBNNode In geneNodes
            trajectory(g.NodeId)(0) = StateToValue(currentGeneStates(g.NodeId))
        Next

        ' 多步级联推演
        For stepI As Integer = 1 To nSteps - 1
            ' 若目标基因本身是 TF，则将其输入丰度置 0（敲降）
            If tfAbundances.ContainsKey(gene) Then
                tfAbundances(gene) = 0.0
            End If

            Dim result As DBNPredictionResult = dbn.PredictNextState(metaboliteAbundances, tfAbundances, currentGeneStates)

            ' 以推演结果更新基因状态，并强制目标基因保持敲降（Low）状态，
            ' 避免被下游反馈回路恢复，从而实现持续的虚拟敲降模拟
            currentGeneStates = result.GeneStates
            currentGeneStates(gene) = "Low"

            For Each g As DBNNode In geneNodes
                trajectory(g.NodeId)(stepI) = StateToValue(currentGeneStates(g.NodeId))
            Next
        Next

        Call VBDebugger.WriteLine($"WGCNAGRN.VirtualKnockdown: 对基因 '{gene}' 完成 {nSteps} 步虚拟敲降级联模拟")

        Return trajectory
    End Function

    ''' <summary>
    ''' 端到端封装：WGCNA 共表达网络 + 时间序列表达矩阵 → 动态贝叶斯网络建模
    ''' → 指定基因的虚拟敲降级联模拟。
    ''' </summary>
    ''' <param name="wgcna">WGCNA 共表达网络。</param>
    ''' <param name="expr">已加载的时间序列表达矩阵（调用方负责从文件加载为 <see cref="GeneExpressionData"/>）。</param>
    ''' <param name="knockGene">被虚拟敲降的目标基因名称。</param>
    ''' <param name="TF">转录因子基因名称数组。</param>
    ''' <param name="nSteps">虚拟敲降级联推演步数，默认 10。</param>
    ''' <returns>目标基因被敲降后，各基因随时间的表达状态轨迹。</returns>
    Public Function RunPipeline(wgcna As NetworkGraph, expr As Core.GeneExpressionData, knockGene As String, TF As String(), Optional nSteps As Integer = 10) As Dictionary(Of String, Double())
        Dim dbn As DynamicBayesianNetwork = BuildDBN(wgcna, expr, TF)
        Return VirtualKnockdown(dbn, knockGene, nSteps)
    End Function

    ' ==================== 基于 DBN 时间序列（GeneExpressionData）的 BNLearn 工作流桥接 ====================

    ''' <summary>
    ''' 将 SingleGRN 流程产出的 DBN 时间序列（已是 <see cref="Core.GeneExpressionData"/>）装配为
    ''' BNLearn 工作流，并可选地融合由伪速率趋势构造的方向先验（<see cref="Core.PriorNetwork"/>）。
    '''
    ''' 与 <see cref="BuildBNNetwork"/>（基于 WGCNA 共表达网络）不同，本函数直接消费已经完成
    ''' 伪时间分箱的连续时间序列表达矩阵，无需 WGCNA 与 TF 注释，适用于 Monocle3 + PseudoVelo
    ''' 产出的单细胞轨迹数据。
    ''' </summary>
    ''' <param name="expr">DBN 时间序列表达矩阵（基因 × 伪时间 bin），即 <see cref="DBNPreprocessOutput.timeSeries"/>。</param>
    ''' <param name="prior">可选的因果方向先验（如由 PseudoVelo 趋势符号构造）。为 Nothing 时退化为纯数据驱动 MMHC 结构学习。</param>
    ''' <returns>已注入表达数据与先验网络的 BNLearn 工作流实例。</returns>
    Public Function BuildExpressionGRN(expr As Core.GeneExpressionData, Optional prior As Core.PriorNetwork = Nothing) As Core.BNLearnWorkflow
        Dim usePrior As Core.PriorNetwork = If(prior, New Core.PriorNetwork())
        Dim workflow As New Core.BNLearnWorkflow With {
            .ExpressionData = expr,
            .PriorNetwork = usePrior
        }

        Call VBDebugger.WriteLine($"GRN.BuildExpressionGRN: 表达矩阵 {expr.NGene} 基因 x {expr.TimePoints.Length} 伪时间点, 先验边 = {usePrior.Edges.Count}")
        Return workflow
    End Function

    ''' <summary>
    ''' 端到端封装：基于 DBN 时间序列构建基因表达调控网络 → 结构学习 → 参数学习 →
    ''' 虚拟扰动分析（敲除 / 过表达 / 动态级联敲除 / 批量敲除），并可选导出结果。
    '''
    ''' 虚拟扰动逻辑严格复用 <see cref="Core.BNLearnWorkflow"/> 的高层 API
    ''' （KnockoutGene / OverexpressGene / DynamicKnockout / BatchKnockout / SaveResults），
    ''' 与 BNLearn\test\Program.vb 的演示一致。
    ''' </summary>
    ''' <param name="expr">DBN 时间序列表达矩阵（基因 × 伪时间 bin）。</param>
    ''' <param name="prior">可选的因果方向先验网络。</param>
    ''' <param name="knockGenes">演示虚拟敲除 / 动态敲除的目标基因集合。</param>
    ''' <param name="overExpr">演示虚拟过表达的基因集合（源码 OverexpressGene 仅接受 nSamples，倍率由内部默认；此处传入的基因名用于选取目标）。</param>
    ''' <param name="dynamicSteps">动态级联敲除推演的时间步数，默认 10。</param>
    ''' <param name="outputDir">结果导出目录；为空则不导出。</param>
    ''' <returns>训练好的工作流 + 各类扰动结果（供调用方进一步分析或二次导出）。</returns>
    Public Function TrainAndIntervene(expr As Core.GeneExpressionData,
                                       prior As Core.PriorNetwork,
                                       knockGenes As String(),
                                       Optional overExpr As (Gene As String, Fold As Double)() = Nothing,
                                       Optional dynamicSteps As Integer = 10,
                                       Optional outputDir As String = Nothing) As (workflow As Core.BNLearnWorkflow,
                                                                                  knockout As Intervention.InterventionResult(),
                                                                                  overExprResults As Intervention.InterventionResult(),
                                                                                  dynamic As Intervention.InterventionResult(),
                                                                                  batch As IEnumerable(Of Intervention.InterventionResult))
        Dim workflow As Core.BNLearnWorkflow = BuildExpressionGRN(expr, prior)

        ' ① 结构学习（MMHC + 白名单先验）
        Call workflow.LearnStructure()
        ' ② 参数学习（高斯 BN MLE）
        Call workflow.LearnParameters()

        Call VBDebugger.WriteLine($"GRN.TrainAndIntervene: 网络训练完成（基因 {expr.NGene}, 伪时间点 {expr.TimePoints.Length}）")

        ' ③ 虚拟敲除
        Dim koResults As New List(Of Intervention.InterventionResult)
        For Each g As String In knockGenes
            koResults.Add(workflow.KnockoutGene(g))
        Next

        ' ④ 虚拟过表达
        Dim oeResults As New List(Of Intervention.InterventionResult)
        If overExpr IsNot Nothing Then
            For Each o In overExpr
                oeResults.Add(workflow.OverexpressGene(o.Gene))
            Next
        End If

        ' ⑤ 动态级联敲除
        Dim dynResults As New List(Of Intervention.InterventionResult)
        For Each g As String In knockGenes
            dynResults.Add(workflow.DynamicKnockout(g, dynamicSteps))
        Next

        ' ⑥ 批量敲除
        Dim batchResults As IEnumerable(Of Intervention.InterventionResult) = workflow.BatchKnockout(knockGenes)

        ' ⑦ 导出结果（与 BNLearn\test\Program.vb 一致）
        If Not String.IsNullOrEmpty(outputDir) Then
            Call workflow.SaveResults(outputDir)
            Dim merged As New List(Of Intervention.InterventionResult)
            Call merged.AddRange(koResults)
            Call merged.AddRange(oeResults)
            Call merged.AddRange(dynResults)
            Call merged.AddRange(batchResults)
            Call New Intervention.InterventionComparisonExporter(merged).ExportAll(outputDir, Nothing)
            Call VBDebugger.WriteLine($"GRN.TrainAndIntervene: 扰动结果已导出至 {outputDir}")
        End If

        Return (workflow, koResults.ToArray, oeResults.ToArray, dynResults.ToArray, batchResults)
    End Function
End Module

