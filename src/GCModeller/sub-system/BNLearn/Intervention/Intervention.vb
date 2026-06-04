' ============================================================
' Intervention.vb - 虚拟干扰分析
' ============================================================
' 实现基因虚拟敲除（Knockout）和过表达（Overexpression）分析
' 
' 核心原理（do-演算）：
'   干预 ≠ 观察！
'   - 观察 evidence：P(X | Y=y) —— 上游信号仍然传递
'   - 干预 do(Y=y)：P(X | do(Y=y)) —— 切断 Y 的入边，Y 被强制设为 y
' 
' 虚拟敲除：do(Gene = 0)
'   1. 删除被敲除基因的所有入边
'   2. 将该基因的 CPD 替换为常数分布 δ(0)
'   3. 从修改后的网络采样，观察下游基因变化
' 
' 虚拟过表达：do(Gene = high_value)
'   1. 删除目标基因的所有入边
'   2. 将该基因的 CPD 替换为常数分布 δ(high_value)
'   3. 从修改后的网络采样
' 
' 动态级联模拟：
'   通过循环将 t1 的输出作为 t0 的输入，模拟多时间步的动态变化
' ============================================================

Imports BNLearn.Core

Namespace Intervention

    ''' <summary>
    ''' 干预模式
    ''' </summary>
    Public Enum InterventionMode
        ''' <summary>基因敲除（设为0）</summary>
        Knockout
        ''' <summary>基因过表达（设为高值）</summary>
        Overexpression
        ''' <summary>基因下调（设为低值）</summary>
        Knockdown
        ''' <summary>自定义值干预</summary>
        Custom
    End Enum

    ''' <summary>
    ''' 单次干预定义
    ''' </summary>
    Public Class InterventionSpec

        ''' <summary>目标基因索引</summary>
        Public Property GeneIndex As Integer

        ''' <summary>目标基因名称</summary>
        Public Property GeneName As String = ""

        ''' <summary>干预模式</summary>
        Public Property Mode As InterventionMode = InterventionMode.Knockout

        ''' <summary>干预值（Custom 模式下使用）</summary>
        Public Property Value As Double = 0.0

        ''' <summary>根据干预模式获取实际干预值</summary>
        Public Function GetInterventionValue(wildtypeMean As Double, wildtypeSD As Double) As Double
            Select Case Mode
                Case InterventionMode.Knockout
                    Return 0.0
                Case InterventionMode.Overexpression
                    Return wildtypeMean + 3.0 * wildtypeSD  ' 3倍标准差过表达
                Case InterventionMode.Knockdown
                    Return wildtypeMean - 2.0 * wildtypeSD  ' 2倍标准差下调
                Case InterventionMode.Custom
                    Return Value
                Case Else
                    Return Value
            End Select
        End Function

    End Class

    ''' <summary>
    ''' 干预分析结果
    ''' </summary>
    Public Class InterventionResult

        ''' <summary>干预定义</summary>
        Public Property Spec As InterventionSpec

        ''' <summary>野生型（对照）表达值</summary>
        Public Property WildtypeMeans As Double()

        ''' <summary>干扰后表达值</summary>
        Public Property MutantMeans As Double()

        ''' <summary>表达变化量（Mutant - Wildtype）</summary>
        Public Property FoldChanges As Double()

        ''' <summary>表达变化百分比</summary>
        Public Property PercentChanges As Double()

        ''' <summary>变化是否显著</summary>
        Public Property IsSignificant As Boolean()

        ''' <summary>基因名称列表</summary>
        Public Property GeneNames As String()

        ''' <summary>受影响的基因数量（显著变化的）</summary>
        Public ReadOnly Property NAffected As Integer
            Get
                If IsSignificant Is Nothing Then Return 0
                Dim count As Integer = 0
                For Each s In IsSignificant
                    If s Then count += 1
                Next
                Return count
            End Get
        End Property

        ''' <summary>动态级联结果（多时间步）</summary>
        Public Property DynamicTrajectory As Double(,)()  ' [timeStep, geneIdx] = samples

        ''' <summary>获取变化最大的前N个基因</summary>
        Public Function GetTopChangedGenes(n As Integer) As List(Of (GeneName As String, FoldChange As Double, PercentChange As Double))
            Dim indexed As New List(Of (index As Integer, foldChange As Double, percentChange As Double))()
            For i = 0 To FoldChanges.Length - 1
                indexed.Add((i, FoldChanges(i), PercentChanges(i)))
            Next

            indexed.Sort(Function(a, b) Math.Abs(b.foldChange).CompareTo(Math.Abs(a.foldChange)))

            Dim result As New List(Of (String, Double, Double))()
            For i = 0 To Math.Min(n - 1, indexed.Count - 1)
                result.Add((GeneNames(indexed(i).Item1), indexed(i).foldChange, indexed(i).percentChange))
            Next
            Return result
        End Function

        ''' <summary>输出摘要字符串</summary>
        Public Overrides Function ToString() As String
            Dim sb As New System.Text.StringBuilder()
            sb.AppendLine(String.Format("=== 干预分析结果: {0} ({1}) ===", Spec.GeneName, Spec.Mode.ToString()))
            sb.AppendLine(String.Format("受影响基因数: {0}/{1}", NAffected, GeneNames.Length))
            sb.AppendLine()
            sb.AppendLine("Top 变化基因:")
            For Each item In GetTopChangedGenes(20)
                sb.AppendLine(String.Format("  {0}: FC={1:F4}  ({2:F1}%)", item.GeneName, item.FoldChange, item.PercentChange))
            Next
            Return sb.ToString()
        End Function

    End Class

    ''' <summary>
    ''' 虚拟干扰分析引擎
    ''' </summary>
    Public Class BnInterventionAnalyzer

        Private _network As Core.BayesianNetwork
        Private _data As Core.GeneExpressionData

        ''' <summary>初始化</summary>
        Public Sub New(fittedNetwork As Core.BayesianNetwork, trainingData As Core.GeneExpressionData)
            _network = fittedNetwork
            _data = trainingData
        End Sub

        ''' <summary>
        ''' 执行单基因虚拟干扰分析
        ''' </summary>
        Public Function AnalyzeIntervention(spec As InterventionSpec,
                                            Optional nSamples As Integer = 10000,
                                            Optional seed As Integer = 42) As InterventionResult

            Dim nG As Integer = _network.Nodes.Count
            Dim geneIdx As Integer = spec.GeneIndex
            If geneIdx < 0 Then
                geneIdx = Array.FindIndex(_network.Nodes.Select(Function(n) n.Name).ToArray(),
                                           Function(n) String.Equals(n, spec.GeneName, StringComparison.OrdinalIgnoreCase))
            End If
            If geneIdx < 0 Then Throw New Exception("基因未找到: " & spec.GeneName)

            spec.GeneIndex = geneIdx

            ' 1. 计算野生型基线
            Dim wildtypeMeans As Double() = ComputeWildtypeMeans(nSamples, seed)

            ' 2. 创建干预网络（do-演算）
            Dim mutantNetwork As Core.BayesianNetwork = CreateInterventionNetwork(spec, wildtypeMeans)

            ' 3. 从干预网络采样
            Dim mutantEngine As New Inference.BnInferenceEngine(mutantNetwork)
            Dim mutantSamples As Double(,) = mutantEngine.Sample(nSamples, seed + 1)

            ' 4. 计算突变型均值
            Dim mutantMeans As Double() = New Double(nG - 1) {}
            For i = 0 To nG - 1
                Dim sum As Double = 0
                For s = 0 To nSamples - 1
                    sum += mutantSamples(i, s)
                Next
                mutantMeans(i) = sum / nSamples
            Next

            ' 5. 计算变化量
            Dim foldChanges As Double() = New Double(nG - 1) {}
            Dim percentChanges As Double() = New Double(nG - 1) {}
            Dim isSignificant As Boolean() = New Boolean(nG - 1) {}

            For i = 0 To nG - 1
                foldChanges(i) = mutantMeans(i) - wildtypeMeans(i)
                If Math.Abs(wildtypeMeans(i)) > 0.0000000001 Then
                    percentChanges(i) = foldChanges(i) / Math.Abs(wildtypeMeans(i)) * 100
                Else
                    percentChanges(i) = If(Math.Abs(foldChanges(i)) > 0.01, Double.NaN, 0)
                End If

                ' 显著性判断：变化超过 0.5 个标准差
                Dim sd As Double = 0
                For s = 0 To nSamples - 1
                    sd += (mutantSamples(i, s) - mutantMeans(i)) ^ 2
                Next
                sd = Math.Sqrt(sd / (nSamples - 1))
                isSignificant(i) = Math.Abs(foldChanges(i)) > 0.5 * sd
            Next

            Return New InterventionResult() With {
                .Spec = spec,
                .WildtypeMeans = wildtypeMeans,
                .MutantMeans = mutantMeans,
                .FoldChanges = foldChanges,
                .PercentChanges = percentChanges,
                .IsSignificant = isSignificant,
                .GeneNames = _network.Nodes.Select(Function(n) n.Name).ToArray()
            }
        End Function

        ''' <summary>
        ''' 动态级联模拟：模拟敲除后多个时间步的表达变化
        ''' 每个时间步的输出作为下一个时间步的输入
        ''' </summary>
        Public Function DynamicIntervention(spec As InterventionSpec,
                                            nTimeSteps As Integer,
                                            Optional nSamples As Integer = 5000,
                                            Optional seed As Integer = 42) As InterventionResult

            Dim nG As Integer = _network.Nodes.Count
            Dim geneIdx As Integer = spec.GeneIndex
            If geneIdx < 0 Then
                geneIdx = Array.FindIndex(_network.Nodes.Select(Function(n) n.Name).ToArray(),
                                           Function(n) String.Equals(n, spec.GeneName, StringComparison.OrdinalIgnoreCase))
            End If
            If geneIdx < 0 Then Throw New Exception("基因未找到: " & spec.GeneName)
            spec.GeneIndex = geneIdx

            ' 野生型基线
            Dim wildtypeMeans As Double() = ComputeWildtypeMeans(nSamples, seed)

            ' 创建干预网络
            Dim mutantNetwork As Core.BayesianNetwork = CreateInterventionNetwork(spec, wildtypeMeans)

            ' 动态模拟
            Dim trajectory As Double(,)() = New Double(nTimeSteps - 1, nG - 1)() {}
            Dim rng As New Random(seed)
            Dim topoOrder As Integer() = _network.TopologicalSort()

            ' 初始状态：从训练数据均值开始
            Dim currentMeans As Double() = CType(wildtypeMeans.Clone(), Double())

            For t = 0 To nTimeSteps - 1
                Dim stepSamples As Double() = New Double(nSamples - 1) {}

                ' 对每个基因，根据干预网络采样
                For Each nodeIdx In topoOrder
                    If nodeIdx = geneIdx Then
                        ' 被干预基因：固定值
                        Dim intVal As Double = spec.GetInterventionValue(wildtypeMeans(geneIdx), 1.0)
                        For s = 0 To nSamples - 1
                            ' 固定值（无噪声）
                        Next
                        currentMeans(nodeIdx) = intVal
                    Else
                        ' 非干预基因：从条件分布采样
                        Dim node As Core.BnNode = mutantNetwork.Nodes(nodeIdx)
                        Dim cpd As Core.BnCPD = node.CPD
                        If cpd Is Nothing Then Continue For

                        Dim parentValues As Double() = New Double(node.Parents.Count - 1) {}
                        For p = 0 To node.Parents.Count - 1
                            parentValues(p) = currentMeans(node.Parents(p))
                        Next

                        ' 条件均值作为下一步的输入
                        currentMeans(nodeIdx) = cpd.ConditionalMean(parentValues)
                    End If
                Next

                ' 记录当前时间步的均值
                For i = 0 To nG - 1
                    trajectory(t, i) = New Double() {currentMeans(i)}
                Next
            Next

            ' 最终时间步的结果
            Dim foldChanges As Double() = New Double(nG - 1) {}
            Dim percentChanges As Double() = New Double(nG - 1) {}
            Dim isSignificant As Boolean() = New Boolean(nG - 1) {}

            For i = 0 To nG - 1
                foldChanges(i) = currentMeans(i) - wildtypeMeans(i)
                If Math.Abs(wildtypeMeans(i)) > 0.0000000001 Then
                    percentChanges(i) = foldChanges(i) / Math.Abs(wildtypeMeans(i)) * 100
                End If
                isSignificant(i) = Math.Abs(foldChanges(i)) > 0.1
            Next

            Return New InterventionResult() With {
                .Spec = spec,
                .WildtypeMeans = wildtypeMeans,
                .MutantMeans = currentMeans,
                .FoldChanges = foldChanges,
                .PercentChanges = percentChanges,
                .IsSignificant = isSignificant,
                .GeneNames = _network.Nodes.Select(Function(n) n.Name).ToArray(),
                .DynamicTrajectory = trajectory
            }
        End Function

        ''' <summary>
        ''' 批量干扰分析：对多个基因逐一进行敲除/过表达
        ''' </summary>
        Public Function BatchIntervention(geneIndices As Integer(),
                                          mode As InterventionMode,
                                          Optional nSamples As Integer = 5000,
                                          Optional seed As Integer = 42) As List(Of InterventionResult)

            Dim results As New List(Of InterventionResult)()

            For Each geneIdx In geneIndices
                Dim spec As New InterventionSpec() With {
                    .GeneIndex = geneIdx,
                    .GeneName = _network.Nodes(geneIdx).Name,
                    .Mode = mode
                }

                Try
                    Dim result As InterventionResult = AnalyzeIntervention(spec, nSamples, seed)
                    results.Add(result)
                Catch ex As Exception
                    ' 跳过失败的干预
                End Try
            Next

            Return results
        End Function

        ' ==================== 内部方法 ====================

        ''' <summary>
        ''' 计算野生型基线表达值（从原始网络采样）
        ''' </summary>
        Private Function ComputeWildtypeMeans(nSamples As Integer, seed As Integer) As Double()
            Dim engine As New Inference.BnInferenceEngine(_network)
            Dim samples As Double(,) = engine.Sample(nSamples, seed)

            Dim nG As Integer = _network.Nodes.Count
            Dim means As Double() = New Double(nG - 1) {}
            For i = 0 To nG - 1
                Dim sum As Double = 0
                For s = 0 To nSamples - 1
                    sum += samples(i, s)
                Next
                means(i) = sum / nSamples
            Next
            Return means
        End Function

        ''' <summary>
        ''' 创建干预网络（do-演算核心操作）
        ''' 1. 删除被干预基因的所有入边
        ''' 2. 将 CPD 替换为常数分布
        ''' </summary>
        Private Function CreateInterventionNetwork(spec As InterventionSpec, wildtypeMeans As Double()) As Core.BayesianNetwork

            ' 深拷贝网络
            Dim mutantNet As Core.BayesianNetwork = _network.CloneStructure()

            ' 复制 CPD 参数
            For i = 0 To _network.Nodes.Count - 1
                If _network.Nodes(i).CPD IsNot Nothing Then
                    mutantNet.Nodes(i).CPD = _network.Nodes(i).CPD.Clone()
                End If
            Next

            Dim geneIdx As Integer = spec.GeneIndex
            Dim intVal As Double = spec.GetInterventionValue(wildtypeMeans(geneIdx), 1.0)

            ' do-演算：删除被干预节点的所有入边
            Dim parentsToRemove As Integer() = mutantNet.Nodes(geneIdx).Parents.ToArray()
            For Each parentIdx In parentsToRemove
                mutantNet.RemoveEdge(parentIdx, geneIdx)
            Next

            ' 替换 CPD 为干预 CPD
            Dim originalCPD As Core.BnCPD = mutantNet.Nodes(geneIdx).CPD
            mutantNet.Nodes(geneIdx).CPD = New InterventionCPD(originalCPD, spec.Mode, intVal)

            Return mutantNet
        End Function

    End Class

End Namespace
