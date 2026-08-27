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

Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
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
        Return wgcna.graphEdges.BuildPriorNetwork(TF)
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
    ''' 
    <Extension>
    Public Function BuildPriorNetwork(Of IE As INetworkEdge)(wgcna As IEnumerable(Of IE), TF As HashSet(Of String)) As Core.PriorNetwork
        Dim prior As New Core.PriorNetwork
        Dim skipped As Integer = 0
        Dim directed As Integer = 0

        If wgcna Is Nothing Then
            Throw New ArgumentNullException(NameOf(wgcna), "WGCNA 共表达网络不能为空")
        ElseIf TF Is Nothing OrElse TF.Count = 0 Then
            Throw New ArgumentException("TF 注释列表不能为空，否则无法构建调控方向", NameOf(TF))
        End If

        Call $"build bnlearn prior network based on WGCNA co-expression network and {TF.Count} TF information.".info

        For Each e As IE In wgcna
            Dim a As String = e.source
            Dim b As String = e.target

            If TF.Contains(a) AndAlso Not TF.Contains(b) Then
                prior.AddEdge(a, b, InferEffector(e.value), Math.Abs(e.value), EVIDENCE)
                directed += 1
            ElseIf TF.Contains(b) AndAlso Not TF.Contains(a) Then
                prior.AddEdge(b, a, InferEffector(e.value), Math.Abs(e.value), EVIDENCE)
                directed += 1
            Else
                ' 两端同为 TF 或同为非 TF：方向无法由共表达确定，跳过
                skipped += 1
            End If
        Next

        Call $"WGCNAGRN.BuildPriorNetwork: 共 {directed + skipped} 条边，定向 {directed} 条，跳过 {skipped} 条（无法由 TF 注释确定方向）".debug

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
    ''' <param name="expr">DBN 时间序列表达矩阵（基因 × 伪时间 bin）。</param>
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

    ' ==================== 基于 WGCNA 共表达模块的 DBN 子网络训练 + 全局级联虚拟扰动 ====================
    '
    ' 针对大型 WGCNA 网络，全局 DBN 训练与虚拟扰动极慢。本组函数实现"分而治之"优化：
    '   1) 按 WGCNA 模块划分（GeneModuleColor）将时间序列划分为模块基因子块；
    '   2) 对每个模块单独训练一个 DynamicBayesianNetwork 子网络（模块内定向边取自合并先验）；
    '   3) 基于模块 eigengene 轨迹相关度构建模块间关联图；
    '   4) 对每个显式指定的扰动基因，在其所属模块内固定 Low 并沿模块关联图做级联推演，
    '      汇总得到全局虚拟扰动响应，并导出结果文件。
    ' 与 TrainAndIntervene（全局高斯 BN）相比，单模块规模远小于全局，训练代价由 O(N^2·样本)
    ' 降为 Σ O(模块规模^2·样本)，在大型 WGCNA 网络下收益显著。

    ''' <summary>
    ''' 单个 WGCNA 模块的 DBN 子网络训练结果容器（模块内部使用，不污染公共 API）。
    ''' </summary>
    Private Class ModuleDBN
        ''' <summary>WGCNA 模块颜色标签</summary>
        Public Property ModuleColor As String
        ''' <summary>该模块参与训练的基因名（与 timeSeries 子矩阵对齐）</summary>
        Public Property Genes As String()
        ''' <summary>已构建拓扑并完成参数学习的动态贝叶斯网络子网络</summary>
        Public Property Net As DynamicBayesianNetwork
        ''' <summary>模块 eigengene 轨迹：各时间点的模块基因均值向量（用于模块间相关度）</summary>
        Public Property Eigengene As Double()
        ''' <summary>模块内基因名 → 在 Genes 数组中的索引</summary>
        Public Property GeneIndex As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
    End Class

    ''' <summary>
    ''' 基于 WGCNA 共表达模块划分，逐模块训练 DynamicBayesianNetwork 子网络，并基于模块间
    ''' 相关度构建子网络关联，对任意显式指定的扰动基因（knockGenes）做全局级联虚拟扰动推断。
    '''
    ''' 与 <see cref="TrainAndIntervene"/> 的区别：
    '''   - 本函数使用"真正动态"的 <see cref="DynamicBayesianNetwork"/>（2TBN，离散状态 Low/Medium/High，
    '''     时间序列参数学习），而非全局高斯 BN；
    '''   - 子网络按 WGCNA 模块分块训练，仅在模块内进行结构/参数学习，避免全局大网络的性能瓶颈；
    '''   - 级联虚拟扰动基于模块 eigengene 关联图逐模块推演（PredictNextState），实现全局性推断。
    ''' </summary>
    ''' <param name="timeSeries">DBN 时间序列表达矩阵（基因 × 伪时间 bin，来自 Monocle3 + PseudoVelo 链路）。</param>
    ''' <param name="modules">WGCNA 模块划分结果（geneID / moduleColor / kME），来自 WGCNA.ReadModuleAssignment。</param>
    ''' <param name="prior">合并后的因果方向先验网络（wgcna + 伪速率），用于提取模块内定向边构建 DBN 拓扑。</param>
    ''' <param name="TF">转录因子基因名称数组（用于识别模块内的调控型接口基因）。</param>
    ''' <param name="knockGenes">显式指定的虚拟扰动（敲降）目标基因列表。</param>
    ''' <param name="dynamicSteps">级联推演的时间步数，默认 10。</param>
    ''' <param name="crossModuleCorThreshold">模块 eigengene 相关阈值：|cor| 超过才建立模块间关联，默认 0.3。</param>
    ''' <param name="outputDir">结果导出目录；为空则不导出。</param>
    ''' <returns>每个扰动基因的全局最终响应向量（按全部模块基因顺序排列）与训练好的模块子网络字典。</returns>
    Public Function TrainModularDBNIntervene(timeSeries As Core.GeneExpressionData,
                                             modules As GeneModuleColor(),
                                             prior As Core.PriorNetwork,
                                             TF As String(),
                                             knockGenes As String(),
                                             Optional dynamicSteps As Integer = 10,
                                             Optional crossModuleCorThreshold As Double = 0.3,
                                             Optional outputDir As String = Nothing) As (finalResponses As Dictionary(Of String, Double()),
                                                                                        moduleNets As Dictionary(Of String, DynamicBayesianNetwork))
        If timeSeries Is Nothing Then Throw New ArgumentNullException(NameOf(timeSeries), "时间序列表达矩阵不能为空")
        If modules Is Nothing OrElse modules.Length = 0 Then Throw New ArgumentNullException(NameOf(modules), "WGCNA 模块划分不能为空")
        If prior Is Nothing Then prior = New Core.PriorNetwork()
        If TF Is Nothing Then TF = {}
        If knockGenes Is Nothing Then knockGenes = {}

        ' ① 模块划分（跳过 grey 模块，仅保留出现在时间序列中的基因）
        Dim moduleGenes = SplitModules(modules, timeSeries)
        If moduleGenes.Count = 0 Then
            Throw New InvalidOperationException("没有任何 WGCNA 模块基因匹配时间序列，无法构建子网络（请检查基因名体系是否一致）")
        End If
        Call VBDebugger.WriteLine($"GRN.TrainModularDBNIntervene: 解析到 {moduleGenes.Count} 个非灰色模块")

        ' ② 逐模块训练 DynamicBayesianNetwork 子网络
        Dim tfSet As New HashSet(Of String)(TF, StringComparer.OrdinalIgnoreCase)
        Dim moduleDBs As New List(Of ModuleDBN)
        For Each kv In moduleGenes
            Dim mcolor = kv.Key
            Dim genes = kv.Value
            If genes.Length < 2 Then
                Call VBDebugger.WriteLine($"GRN.TrainModularDBNIntervene: 模块 {mcolor} 基因数={genes.Length} < 2，跳过子网络训练")
                Continue For
            End If

            Dim subMatrix = timeSeries.GetSubMatrix(genes)
            If subMatrix Is Nothing Then
                Call VBDebugger.WriteLine($"GRN.TrainModularDBNIntervene: 模块 {mcolor} 无基因匹配时间序列，跳过")
                Continue For
            End If

            ' 模块内定向边（两端都属于本模块）转为 RegulatoryLink
            Dim links = BuildModuleRegulatoryLinks(prior, genes)
            Dim net As New DynamicBayesianNetwork()
            net.BuildFromTopology(links)

            Dim ts = ToTimeSeries(subMatrix)
            If ts IsNot Nothing AndAlso ts.Count >= 2 Then
                net.LearnParameters(ts)
            Else
                Call VBDebugger.WriteLine($"GRN.TrainModularDBNIntervene: 模块 {mcolor} 有效时间点不足，仅使用拓扑先验 CPT")
            End If

            Dim eig = ComputeModuleEigengene(ts)
            Dim mdb As New ModuleDBN With {
                .ModuleColor = mcolor,
                .Genes = genes,
                .Net = net,
                .Eigengene = eig
            }
            For i = 0 To genes.Length - 1
                mdb.GeneIndex(genes(i)) = i
            Next
            moduleDBs.Add(mdb)
            Call VBDebugger.WriteLine($"GRN.TrainModularDBNIntervene: 模块 {mcolor} 训练完成（基因={genes.Length}, 模块内边={links.Count()}）")
        Next

        If moduleDBs.Count = 0 Then
            Throw New InvalidOperationException("没有任何模块成功训练出子网络，无法执行虚拟扰动")
        End If

        ' ③ 模块间关联图（基于 eigengene 轨迹相关度）
        Dim graph = BuildModuleCorrelationGraph(moduleDBs, crossModuleCorThreshold)
        Call VBDebugger.WriteLine($"GRN.TrainModularDBNIntervene: 模块关联边数={graph.Values.Sum(Function(l) l.Count)}")

        ' ④ 全局级联虚拟扰动
        Dim allGenes = moduleDBs.SelectMany(Function(m) m.Genes).Distinct().ToArray()
        Dim finalResponses As New System.Collections.Generic.Dictionary(Of String, List(Of Double))()
        Dim trajectories As New System.Collections.Generic.Dictionary(Of String, System.Collections.Generic.Dictionary(Of String, List(Of Double)))()

        For Each g In knockGenes
            Dim respVec As Double() = CascadeIntervene(moduleDBs, graph, tfSet, g, dynamicSteps, allGenes, trajectories)
            finalResponses(g) = respVec
        Next

        ' ⑤ 导出结果
        If Not String.IsNullOrEmpty(outputDir) Then
            Call SaveModularResults(finalResponses, trajectories, allGenes, outputDir)
        End If

        Dim moduleNets As New Dictionary(Of String, DynamicBayesianNetwork)
        For Each m In moduleDBs
            moduleNets(m.ModuleColor) = m.Net
        Next

        Call VBDebugger.WriteLine($"GRN.TrainModularDBNIntervene: 全局级联虚拟扰动完成（扰动基因 {knockGenes.Length} 个，模块 {moduleDBs.Count} 个，全局基因 {allGenes.Length} 个）")

        Return (finalResponses, moduleNets)
    End Function

    ''' <summary>
    ''' 按 WGCNA 模块划分将基因分组（跳过 grey 模块，仅保留出现在 timeSeries 中的基因）。
    ''' </summary>
    Private Function SplitModules(assignment As GeneModuleColor(), timeSeries As Core.GeneExpressionData) As Dictionary(Of String, String())
        Dim result As New Dictionary(Of String, List(Of String))
        Dim present As New HashSet(Of String)(timeSeries.GeneNames, StringComparer.OrdinalIgnoreCase)

        For Each mc In assignment
            If String.Equals(mc.moduleColor, "grey", StringComparison.OrdinalIgnoreCase) Then Continue For
            If Not present.Contains(mc.geneID) Then Continue For
            If Not result.ContainsKey(mc.moduleColor) Then
                result(mc.moduleColor) = New List(Of String)
            End If
            If Not result(mc.moduleColor).Contains(mc.geneID) Then
                result(mc.moduleColor).Add(mc.geneID)
            End If
        Next

        Return result.ToDictionary(Function(kv) kv.Key, Function(kv) kv.Value.ToArray())
    End Function

    ''' <summary>
    ''' 从合并先验网络中筛选两端都属于当前模块基因的定向边，转为 DBN 拓扑所需的
    ''' <see cref="RegulatoryLink"/> 集合。调控方向沿用 prior 的 RegulationType；
    ''' 若某模块无任何模块内先验边，返回空集合（DBN 退化为无父节点拓扑，仅学习自身时序分布）。
    ''' </summary>
    Private Function BuildModuleRegulatoryLinks(prior As Core.PriorNetwork, moduleGenes As String()) As IEnumerable(Of RegulatoryLink)
        Dim inModule As New HashSet(Of String)(moduleGenes, StringComparer.OrdinalIgnoreCase)
        Dim links As New List(Of RegulatoryLink)

        For Each e In prior.Edges
            If inModule.Contains(e.TF) AndAlso inModule.Contains(e.TargetGene) Then
                links.Add(New RegulatoryLink With {
                    .TF_id = e.TF,
                    .target_operon = e.TargetGene,
                    .regulate_genes = {e.TargetGene},
                    .effector = Nothing
                })
            End If
        Next

        Return links
    End Function

    ''' <summary>
    ''' 计算模块 eigengene 轨迹：各时间点上模块基因均值（顺序与 timeSeries 一致）。
    ''' 时间点不足时返回长度为 1 的均值向量，关联图仍可工作（相关退化为常数）。
    ''' </summary>
    Private Function ComputeModuleEigengene(ts As List(Of Dictionary(Of String, Double))) As Double()
        If ts Is Nothing OrElse ts.Count = 0 Then Return {0.0}
        Dim nT = ts.Count
        Dim vec As New List(Of Double)
        For t = 0 To nT - 1
            Dim frame = ts(t)
            If frame Is Nothing OrElse frame.Count = 0 Then
                vec.Add(0.0)
                Continue For
            End If
            Dim avg = frame.Values.Average()
            vec.Add(avg)
        Next
        Return vec.ToArray()
    End Function

    ''' <summary>
    ''' 基于模块 eigengene 轨迹的 Pearson 相关构建模块间关联图（邻接表，权重 = |cor|）。
    ''' 仅保留 |cor| 超过阈值的双向关联。
    ''' </summary>
    Private Function BuildModuleCorrelationGraph(modules As List(Of ModuleDBN), threshold As Double) As Dictionary(Of String, List(Of (modColor As String, weight As Double)))
        Dim graph As New Dictionary(Of String, List(Of (String, Double)))

        For Each m In modules
            graph(m.ModuleColor) = New List(Of (String, Double))
        Next

        For i = 0 To modules.Count - 1
            For j = i + 1 To modules.Count - 1
                Dim c = Pearson(modules(i).Eigengene, modules(j).Eigengene)
                If Math.Abs(c) > threshold Then
                    graph(modules(i).ModuleColor).Add((modules(j).ModuleColor, c))
                    graph(modules(j).ModuleColor).Add((modules(i).ModuleColor, c))
                End If
            Next
        Next

        Return graph
    End Function

    ''' <summary>
    ''' 对单个扰动基因执行全局级联虚拟扰动：
    '''   - 在其所属模块内固定 Low 并多步推演本模块基因状态轨迹；
    '''   - 计算本模块 eigengene 变化，沿模块关联图 BFS 逐级注入下游模块（作为模块整体状态偏置），
    '''     在下游模块内做受迫推演，形成级联；
    '''   - 汇总所有模块基因的最终状态为全局响应向量（按 allGenes 顺序，Low=0/Med=1/High=2）。
    ''' </summary>
    Private Function CascadeIntervene(modules As List(Of ModuleDBN),
                                     graph As Dictionary(Of String, List(Of (modColor As String, weight As Double))),
                                     tfSet As HashSet(Of String),
                                     knockGene As String,
                                     steps As Integer,
                                     allGenes As String(),
                                     trajectories As System.Collections.Generic.Dictionary(Of String, System.Collections.Generic.Dictionary(Of String, List(Of Double)))()) As Double()
        ' 定位扰动基因所属模块
        Dim m0 As ModuleDBN = Nothing
        For Each m In modules
            If m.GeneIndex.ContainsKey(knockGene) Then
                m0 = m
                Exit For
            End If
        Next
        If m0 Is Nothing Then
            Call VBDebugger.WriteLine($"GRN.CascadeIntervene: 警告: 扰动基因 '{knockGene}' 不在任何模块中，跳过")
            Dim zero As Double() = allGenes.Select(Function(g) 1.0).ToArray()
            trajectories(knockGene) = New System.Collections.Generic.Dictionary(Of String, List(Of Double))
            Return zero
        End If

        ' 每个模块维护基因离散状态（初始 Medium），以及各自的轨迹容器
        Dim moduleStates As New System.Collections.Generic.Dictionary(Of String, System.Collections.Generic.Dictionary(Of String, String))
        Dim moduleTraj As New System.Collections.Generic.Dictionary(Of String, System.Collections.Generic.Dictionary(Of String, List(Of Double)))
        For Each m In modules
            Dim st As New System.Collections.Generic.Dictionary(Of String, String)
            Dim tr As New System.Collections.Generic.Dictionary(Of String, List(Of Double))
            For Each g In m.Genes
                st(g) = "Medium"
                tr(g) = New List(Of Double)(New Double(steps - 1) {})
            Next
            moduleStates(m.ModuleColor) = st
            moduleTraj(m.ModuleColor) = tr
        Next

        ' 初始步：扰动基因固定 Low
        moduleStates(m0.ModuleColor)(knockGene) = "Low"
        For Each g In m0.Genes
            moduleTraj(m0.ModuleColor)(g)(0) = StateToValue(moduleStates(m0.ModuleColor)(g))
        Next

        ' 本模块多步推演
        Dim m0Rates = RunModuleSteps(m0, moduleStates(m0.ModuleColor), knockGene, steps, tfSet, moduleTraj(m0.ModuleColor))
        ' 计算本模块 eigengene 变化（最终步 RNA 速率均值）
        Dim delta0 = If(m0Rates.Count > 0, m0Rates.Values.Average(), 0.0)

        ' 沿模块关联图 BFS 级联
        Dim visited As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {m0.ModuleColor}
        Dim queue As New Queue(Of (modColor As String, delta As Double))
        queue.Enqueue((modColor:=m0.ModuleColor, delta:=delta0))

        While queue.Count > 0
            Dim cur = queue.Dequeue()
            If Not graph.ContainsKey(cur.modColor) Then Continue While
            For Each adj In graph(cur.modColor)
                If visited.Contains(adj.modColor) Then Continue For
                visited.Add(adj.modColor)
                Dim mNext = modules.First(Function(m) String.Equals(m.ModuleColor, adj.modColor, StringComparison.OrdinalIgnoreCase))
                ' 上游变化按关联权重注入下游模块（作为模块整体状态偏置）
                Dim upstreamDelta = cur.delta * adj.weight
                Dim fixedInNext = If(mNext.GeneIndex.ContainsKey(knockGene), knockGene, Nothing)
                Dim nextRates = RunModuleForced(mNext, upstreamDelta, fixedInNext, steps, tfSet, moduleStates(mNext.ModuleColor), moduleTraj(mNext.ModuleColor))
                Dim deltaNext = If(nextRates.Count > 0, nextRates.Values.Average(), 0.0)
                queue.Enqueue((modColor:=mNext.ModuleColor, delta:=deltaNext))
            Next
        End While

        ' 汇总全局最终响应向量（显式双层循环，避免 SelectMany 对 Double() 轨迹的深层展平）
        Dim geneToTraj As New System.Collections.Generic.Dictionary(Of String, List(Of Double))(StringComparer.OrdinalIgnoreCase)
        For Each kvModule In moduleTraj
            For Each kvGene In kvModule.Value
                geneToTraj(kvGene.Key) = kvGene.Value
            Next
        Next

        Dim resp As New Double(allGenes.Length - 1)
        For i = 0 To allGenes.Length - 1
            Dim g = allGenes(i)
            If geneToTraj.ContainsKey(g) Then
                resp(i) = geneToTraj(g)(steps - 1)
            Else
                resp(i) = 1.0  ' 未参与任何模块：中性 Medium
            End If
        Next

        Dim trajMerged As New System.Collections.Generic.Dictionary(Of String, List(Of Double))(StringComparer.OrdinalIgnoreCase)
        For Each kvModule In moduleTraj
            For Each kvGene In kvModule.Value
                trajMerged(kvGene.Key) = kvGene.Value
            Next
        Next
        trajectories(knockGene) = trajMerged

        Call VBDebugger.WriteLine($"GRN.CascadeIntervene: 对基因 '{knockGene}'（模块 {m0.ModuleColor}）完成级联虚拟扰动，本模块 eigengene 变化 δ={delta0:F4}")
        Return resp
    End Function

    ''' <summary>
    ''' 在单个模块子网络内多步推演（扰动基因固定 Low）。返回各基因最终 RNA 丰度变化率。
    ''' </summary>
    Private Function RunModuleSteps(m As ModuleDBN,
                                    geneStates As Dictionary(Of String, String),
                                    fixedGene As String,
                                    steps As Integer,
                                    tfSet As HashSet(Of String),
                                    traj As System.Collections.Generic.Dictionary(Of String, List(Of Double))) As System.Collections.Generic.Dictionary(Of String, Double)
        Dim lastRates As New Dictionary(Of String, Double)

        For t = 1 To steps - 1
            ' 模块内 TF 基因的连续 abundance（由当前离散状态映射，与证据一致）
            Dim tfAbund As New Dictionary(Of String, Double)
            For Each g In m.Genes
                If tfSet.Contains(g) Then
                    tfAbund(g) = StateToScore(geneStates(g))
                End If
            Next

            Dim result = m.Net.PredictNextState(Nothing, tfAbund, geneStates)
            For Each g In m.Genes
                If result.GeneStates.ContainsKey(g) Then
                    geneStates(g) = result.GeneStates(g)
                End If
                ' 持续固定扰动基因 Low，避免被反馈回路恢复
                If Not String.IsNullOrEmpty(fixedGene) Then geneStates(fixedGene) = "Low"
                traj(g)(t) = StateToValue(geneStates(g))
            Next

            For Each g In m.Genes
                If result.RNAAbundanceChanges.ContainsKey(g) Then lastRates(g) = result.RNAAbundanceChanges(g)
            Next
        Next

        Return lastRates
    End Function

    ''' <summary>
    ''' 受迫推演：下游模块接收上游 eigengene 变化偏置，初始整体状态偏移后多步推演。
    ''' </summary>
    Private Function RunModuleForced(m As ModuleDBN,
                                     upstreamDelta As Double,
                                     fixedGene As String,
                                     steps As Integer,
                                     tfSet As HashSet(Of String),
                                     geneStates As Dictionary(Of String, String),
                                     traj As System.Collections.Generic.Dictionary(Of String, List(Of Double))) As System.Collections.Generic.Dictionary(Of String, Double)
        ' 初始整体状态偏置：上游正向变化 → High，负向 → Low，近 0 → Medium
        Dim initState As String = If(upstreamDelta > 0.1, "High", If(upstreamDelta < -0.1, "Low", "Medium"))
        For Each g In m.Genes
            geneStates(g) = initState
        Next
        If Not String.IsNullOrEmpty(fixedGene) Then geneStates(fixedGene) = "Low"
        For Each g In m.Genes
            traj(g)(0) = StateToValue(geneStates(g))
        Next

        Dim lastRates As New Dictionary(Of String, Double)
        For t = 1 To steps - 1
            Dim tfAbund As New Dictionary(Of String, Double)
            For Each g In m.Genes
                If tfSet.Contains(g) Then
                    ' 上游变化注入 TF abundance（clamp 到合理范围）
                    tfAbund(g) = Math.Max(0.0, Math.Min(2.0, StateToScore(geneStates(g)) * (1.0 + upstreamDelta)))
                End If
            Next

            Dim result = m.Net.PredictNextState(Nothing, tfAbund, geneStates)
            For Each g In m.Genes
                If result.GeneStates.ContainsKey(g) Then
                    geneStates(g) = result.GeneStates(g)
                End If
                If Not String.IsNullOrEmpty(fixedGene) Then geneStates(fixedGene) = "Low"
                traj(g)(t) = StateToValue(geneStates(g))
            Next
            For Each g In m.Genes
                If result.RNAAbundanceChanges.ContainsKey(g) Then lastRates(g) = result.RNAAbundanceChanges(g)
            Next
        Next

        Return lastRates
    End Function

    ''' <summary>
    ''' 将离散状态映射为数值分值（Low=0, Medium=0.5, High=1），供父节点证据离散化使用。
    ''' </summary>
    Private Function StateToScore(state As String) As Double
        Select Case state
            Case "Low" : Return 0.0
            Case "Medium" : Return 0.5
            Case "High" : Return 1.0
            Case Else : Return 0.5
        End Select
    End Function

    ''' <summary>
    ''' 导出全局虚拟扰动结果：基因 × 扰动源 响应矩阵 TSV + 每个扰动源的逐基因明细 TSV。
    ''' </summary>
    Private Sub SaveModularResults(finalResponses As Dictionary(Of String, Double()),
                                   trajectories As Dictionary(Of String, Dictionary(Of String, Double())),
                                   allGenes As String(),
                                   outputDir As String)
        If Not System.IO.Directory.Exists(outputDir) Then System.IO.Directory.CreateDirectory(outputDir)

        ' 全局响应矩阵（最终稳态，gene × perturbation）
        Dim sbMatrix As New StringBuilder()
        sbMatrix.Append("gene")
        For Each src In finalResponses.Keys
            sbMatrix.Append(vbTab).Append(src)
        Next
        sbMatrix.AppendLine()
        For i = 0 To allGenes.Length - 1
            sbMatrix.Append(allGenes(i))
            For Each src In finalResponses.Keys
                sbMatrix.Append(vbTab).Append(finalResponses(src)(i).ToString("F6"))
            Next
            sbMatrix.AppendLine()
        Next
        System.IO.File.WriteAllText(System.IO.Path.Combine(outputDir, "modular_global_perturbation_responses.tsv"), sbMatrix.ToString())

        ' 每个扰动源明细（基因 \t 最终效应 \t 轨迹峰值）
        For Each src In trajectories.Keys
            Dim tr = trajectories(src)
            Dim sb As New StringBuilder()
            sb.AppendLine("gene" & vbTab & "final_effect" & vbTab & "peak_effect")
            For Each g In allGenes
                If tr.ContainsKey(g) Then
                    Dim vec = tr(g)
                    Dim peak = vec.Max()
                    sb.AppendLine(String.Format("{0}{1}{2:F6}{3}{4:F6}", g, vbTab, vec(vec.Length - 1), vbTab, peak))
                Else
                    sb.AppendLine(String.Format("{0}{1}1.000000{1}1.000000", g, vbTab))
                End If
            Next
            Dim safe = New String(src.Where(Function(c) Char.IsLetterOrDigit(c)).ToArray())
            System.IO.File.WriteAllText(System.IO.Path.Combine(outputDir, "modular_pert_" & safe & ".tsv"), sb.ToString())
        Next

        Call VBDebugger.WriteLine($"GRN.SaveModularResults: 模块化全局扰动结果已导出至 {outputDir}")
    End Sub

    ''' <summary>
    ''' Pearson 相关（对不等长序列按较短者截断），用于模块 eigengene 轨迹相关度。
    ''' </summary>
    Private Function Pearson(x As Double(), y As Double()) As Double
        Dim n = Math.Min(x.Length, y.Length)
        If n < 2 Then Return 0
        Dim mx = x.Take(n).Average()
        Dim my = y.Take(n).Average()
        Dim num As Double = 0, dx As Double = 0, dy As Double = 0
        For i = 0 To n - 1
            Dim a = x(i) - mx
            Dim b = y(i) - my
            num += a * b
            dx += a * a
            dy += b * b
        Next
        If dx = 0 OrElse dy = 0 Then Return 0
        Return num / Math.Sqrt(dx * dy)
    End Function
End Module

