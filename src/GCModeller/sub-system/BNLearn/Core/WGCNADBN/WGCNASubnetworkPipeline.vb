' ============================================================
' WGCNASubnetworkPipeline.vb
' ------------------------------------------------------------
' 依据 DBNBlocks.md 文档思路："分而治之训练、合而为一扰动"。
'
' 流程：
'   1) 基于 WGCNA 模块划分（GeneModuleColor[]）把基因切分为若干模块；
'   2) 对每个模块子集独立训练静态高斯贝叶斯子网络（结构学习 + 参数学习）；
'   3) 把各子网的回归系数拼成块对角全局系数矩阵 A，并（关键）补全
'      模块间边（用模块 eigengene 相关 + hub 基因间相关），得到整合的
'      全局网络（含模块内 + 模块间边），并统一学习全局 CPD；
'   4) 在整合后的全局网络上做虚拟扰动传播，支持两种方法：
'        - Jacobian（默认）：沿 A^k 多步线性传播至收敛；
'        - CascadeSampling：在全局网络上做多步 do-演算（DynamicIntervention）。
'   5) 导出全局扰动响应矩阵（gene × perturbation）TSV + 控制台摘要。
' ============================================================

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.BNLearn.ParameterLearning
Imports SMRUCC.genomics.Analysis.BNLearn.StructureLearning

Namespace Core.WGCNADBN

    ''' <summary>
    ''' 基于 WGCNA 模块划分的贝叶斯子网络训练 + 全局虚拟扰动流水线
    ''' </summary>
    Public Class WGCNASubnetworkPipeline

        ' ---- 训练参数（与 BNLearnWorkflow 风格一致） ----
        ''' <summary>是否对表达数据做标准化（z-score），默认 True</summary>
        Public Property NormalizeData As Boolean = True

        ''' <summary>结构学习参数（算法/显著性阈值/最大父节点数/随机种子）</summary>
        Public Property StructureParams As New StructureLearningParams()

        ''' <summary>参数学习与采样所用样本数</summary>
        Public Property NSamples As Integer = 10000

        ''' <summary>随机种子</summary>
        Public Property RandomSeed As Integer = 42

        ' ---- 全局扰动参数 ----
        ''' <summary>传播方法，默认 Jacobian（线性化雅可比多步传播）</summary>
        Public Property Propagation As PropagationMethod = PropagationMethod.Jacobian

        ''' <summary>最大传播步数（雅可比收敛上限 / 级联采样时间步数）</summary>
        Public Property MaxSteps As Integer = 50

        ''' <summary>雅可比收敛阈值：||e_{t+1}|| / ||e_t|| 小于该值即停止</summary>
        Public Property Tolerance As Double = 0.000001

        ''' <summary>每个模块取 kME 最高的前 N 个基因作为模块接口（hub）</summary>
        Public Property HubTopN As Integer = 20

        ''' <summary>模块 eigengene 相关阈值：|cor| 超过才尝试补模块间边</summary>
        Public Property CrossModuleCorThreshold As Double = 0.3

        ''' <summary>hub 基因间相关阈值：|r| 超过才在对应基因间补跨模块边</summary>
        Public Property CrossGeneCorThreshold As Double = 0.4

        ''' <summary>跨模块边的初始权重缩放（最终由全局参数学习覆盖）</summary>
        Public Property CrossScale As Double = 0.5

        ' ---- 内部状态 ----
        Private _expr As GeneExpressionData
        Private _exprStd As GeneExpressionData
        Private _genes As String()
        Private _gIndex As New Dictionary(Of String, Integer)()
        Private _moduleGenes As New Dictionary(Of String, List(Of String))()
        Private _moduleHubs As New Dictionary(Of String, List(Of String))()
        Private _subNets As New List(Of BayesianNetwork)()
        Private _A As Double(,)
        Private _globalNet As BayesianNetwork

        ' ============================================================
        ' 1. 主入口
        ' ============================================================

        ''' <summary>
        ''' 运行完整流程：模块切分 → 子网络训练 → 全局矩阵拼接 → 各源基因全局扰动。
        ''' </summary>
        ''' <param name="assignment">WGCNA 模块划分结果（geneID / moduleColor / kME）</param>
        ''' <param name="expr">全局表达矩阵（基因 × 样本）</param>
        ''' <param name="sources">扰动源基因列表；为 Nothing 时自动取每模块 kME 最高的代表基因</param>
        ''' <returns>每个扰动源的全局扰动结果</returns>
        Public Function Run(assignment As GeneModuleColor(),
                           expr As GeneExpressionData,
                           Optional sources As String() = Nothing) As List(Of GlobalPerturbationResult)

            _expr = expr
            _genes = expr.GeneNames
            _gIndex = New Dictionary(Of String, Integer)()
            For i = 0 To _genes.Length - 1
                _gIndex(_genes(i)) = i
            Next

            ' 标准化数据用于训练（与 BNLearnWorkflow 一致）
            If NormalizeData Then
                _exprStd = expr.Standardize()
            Else
                _exprStd = expr
            End If

            Call SplitModules(assignment)

            ' 仅保留参与 WGCNA 模块的基因进入后续训练与全局网络，缩小全局规模。
            ' 背景非模块基因既无意义又会让全局参数学习（O(N^2·样本)）变得极慢。
            Dim allModuleGenes = _moduleGenes.Values _
                .SelectMany(Function(lst) lst) _
                .Distinct() _
                .ToArray()
            Dim exprModule = _exprStd.GetSubMatrix(allModuleGenes)
            If exprModule IsNot Nothing AndAlso exprModule.NSample > 0 Then
                _exprStd = exprModule
                _genes = _exprStd.GeneNames
                _gIndex = New Dictionary(Of String, Integer)()
                For i = 0 To _genes.Length - 1
                    _gIndex(_genes(i)) = i
                Next
                Call $"[WGCNASubnetworkPipeline] 仅保留模块基因参加全局网络: {_genes.Length}/{expr.GeneNames.Length}".debug
            End If

            Call TrainSubnetworks()
            Call BuildInitialJacobian()
            Call BuildCrossModuleEdges()
            Call BuildGlobalNetwork()

            Call $"[WGCNASubnetworkPipeline] 模块数={_moduleGenes.Count}, 全局基因数={_genes.Length}, 全局网络边数≈{CountEdges()}".debug

            ' 确定扰动源
            Dim srcList As List(Of String)
            If sources Is Nothing OrElse sources.Length = 0 Then
                srcList = GetDefaultSources()
                Call $"[WGCNASubnetworkPipeline] 未指定扰动源，自动取每模块代表基因共 {srcList.Count} 个".debug
            Else
                srcList = New List(Of String)(sources)
            End If

            Dim results As New List(Of GlobalPerturbationResult)()
            For Each src In srcList
                Dim gi As Integer = GetGlobalIndex(src)
                If gi < 0 Then
                    Call $"[WGCNASubnetworkPipeline] 警告: 扰动源 '{src}' 不在表达矩阵中，跳过".debug
                    Continue For
                End If
                Dim r As GlobalPerturbationResult
                If Propagation = PropagationMethod.Jacobian Then
                    r = PropagateJacobian(gi)
                Else
                    r = PropagateCascade(gi)
                End If
                results.Add(r)
                Call r.ToString().debug
            Next

            Return results
        End Function

        ' ============================================================
        ' 2. 模块切分
        ' ============================================================

        Private Sub SplitModules(assignment As GeneModuleColor())
            _moduleGenes = New Dictionary(Of String, List(Of String))()
            _moduleHubs = New Dictionary(Of String, List(Of String))()
            Dim kmeOf As New Dictionary(Of String, Double)()

            For Each mc In assignment
                ' 灰色模块（未分配）不参与子网络建模
                If String.Equals(mc.moduleColor, "grey", StringComparison.OrdinalIgnoreCase) Then
                    Continue For
                End If
                kmeOf(mc.geneID) = mc.kME
                If Not _moduleGenes.ContainsKey(mc.moduleColor) Then
                    _moduleGenes(mc.moduleColor) = New List(Of String)()
                End If
                _moduleGenes(mc.moduleColor).Add(mc.geneID)
            Next

            ' 每个模块按 kME 降序取 Top-N 作为 hub（模块接口）
            For Each kv In _moduleGenes
                Dim sorted = kv.Value _
                    .OrderByDescending(Function(g) If(kmeOf.ContainsKey(g), kmeOf(g), -1.0)) _
                    .Take(HubTopN) _
                    .ToList()
                _moduleHubs(kv.Key) = sorted
            Next

            Call $"[WGCNASubnetworkPipeline] 解析到 {_moduleGenes.Count} 个非灰色模块".debug
        End Sub

        ' ============================================================
        ' 3. 子网络训练（结构学习 + 参数学习）
        ' ============================================================

        Private Sub TrainSubnetworks()
            _subNets = New List(Of BayesianNetwork)()

            For Each kv In _moduleGenes
                Dim moduleColor = kv.Key
                Dim genes = kv.Value.Distinct().ToArray()

                ' 单基因 / 空模块无法训练结构，跳过（对应 A 行保持零）
                If genes.Length < 2 Then
                    Call $"[WGCNASubnetworkPipeline] 模块 {moduleColor} 基因数={genes.Length} < 2，跳过结构学习".debug
                    Continue For
                End If

                Dim subData = _exprStd.GetSubMatrix(genes)
                If subData Is Nothing Then
                    Call $"[WGCNASubnetworkPipeline] 模块 {moduleColor} 无基因匹配表达矩阵，跳过".debug
                    Continue For
                End If

                ' 结构学习
                Dim structResult = New BnStructureLearner().Learn(subData, StructureParams, Nothing)
                Dim net = structResult.Network

                ' 参数学习（在标准化子矩阵上，系数与全局 A 自洽）
                Dim learned = New BnParameterLearner().Learn(net, subData).Network

                _subNets.Add(learned)
                Call $"[WGCNASubnetworkPipeline] 模块 {moduleColor}: 训练 {genes.Length} 基因, 节点 {learned.Nodes.Count}".debug
            Next
        End Sub

        ' ============================================================
        ' 4. 全局雅可比矩阵拼接（模块内边）
        ' ============================================================

        Private Sub BuildInitialJacobian()
            Dim n As Integer = _genes.Length
            _A = New Double(n - 1, n - 1) {}

            For Each net As BayesianNetwork In _subNets
                For Each node In net.Nodes
                    Dim childGlobal = GetGlobalIndex(node.Name)
                    If childGlobal < 0 OrElse node.CPD Is Nothing Then
                        Continue For
                    End If
                    For k = 0 To node.CPD.ParentIndices.Length - 1
                        Dim parentLocal = node.CPD.ParentIndices(k)
                        Dim parentName = net.Nodes(parentLocal).Name
                        Dim parentGlobal = GetGlobalIndex(parentName)
                        If parentGlobal >= 0 Then
                            _A(childGlobal, parentGlobal) = node.CPD.Coeffs(k)
                        End If
                    Next
                Next
            Next
        End Sub

        ' ============================================================
        ' 5. 补全模块间边（DBNBlocks.md 强调的关键步骤）
        ' ============================================================

        Private Sub BuildCrossModuleEdges()
            Dim moduleColors = _moduleHubs.Keys.ToList()

            For a = 0 To moduleColors.Count - 1
                For b = 0 To moduleColors.Count - 1
                    If a = b Then Continue For
                    Dim ma = moduleColors(a)
                    Dim mb = moduleColors(b)

                    ' 模块间 eigengene 相关不足则不连
                    If Math.Abs(ModuleEigengeneCorrelation(ma, mb)) <= CrossModuleCorThreshold Then
                        Continue For
                    End If

                    For Each ga In _moduleHubs(ma)
                        Dim iga = GetGlobalIndex(ga)
                        If iga < 0 Then Continue For
                        For Each gb In _moduleHubs(mb)
                            Dim igb = GetGlobalIndex(gb)
                            If igb < 0 Then Continue For
                            Dim r = GeneCorrelation(ga, gb)
                            If Math.Abs(r) > CrossGeneCorThreshold Then
                                ' 仅当目标方向尚无边（保留模块内学到的精确边）
                                If _A(igb, iga) = 0 Then
                                    _A(igb, iga) = r * CrossScale
                                End If
                            End If
                        Next
                    Next
                Next
            Next

            Call $"[WGCNASubnetworkPipeline] 跨模块边补全完成".debug
        End Sub

        ' ============================================================
        ' 6. 构建全局聚合网络并统一学习 CPD
        ' ============================================================

        Private Sub BuildGlobalNetwork()
            Dim n As Integer = _genes.Length
            _globalNet = New BayesianNetwork()

            Call "build a global bayesian network.".debug
            Call "add nodes.".debug

            For Each g In TqdmWrapper.Wrap(_genes)
                _globalNet.AddNode(g)
            Next

            Call "add network edges.".debug

            For Each i As Integer In TqdmWrapper.Range(0, n)          ' child
                For j = 0 To n - 1      ' parent
                    If Math.Abs(_A(i, j)) > 0.000000000001 Then
                        ' AddEdge(parent, child)
                        _globalNet.AddEdge(j, i)
                    End If
                Next
            Next

            ' 在完整标准化数据上统一学习全局 CPD（含模块内 + 跨模块边）
            _globalNet = New BnParameterLearner().Learn(_globalNet, _exprStd).Network

            ' 用全局网络 CPD 重写 A（ParentIndices 此时直接是全局基因索引）
            _A = New Double(n - 1, n - 1) {}
            For Each node In _globalNet.Nodes
                Dim ci = GetGlobalIndex(node.Name)
                If node.CPD Is Nothing Then Continue For
                For k = 0 To node.CPD.ParentIndices.Length - 1
                    Dim pj = node.CPD.ParentIndices(k)
                    _A(ci, pj) = node.CPD.Coeffs(k)
                Next
            Next
        End Sub

        ' ============================================================
        ' 7. 传播方法
        ' ============================================================

        ''' <summary>雅可比矩阵多步线性传播</summary>
        Private Function PropagateJacobian(sourceIdx As Integer) As GlobalPerturbationResult
            Dim n As Integer = _genes.Length
            Dim delta = New Double(n - 1) {}
            delta(sourceIdx) = InterventionValue(sourceIdx)

            Dim current = CType(delta.Clone(), Double())
            Dim total = New Double(n - 1) {}
            Dim result As New GlobalPerturbationResult() With {
                .SourceGene = _genes(sourceIdx),
                .Method = PropagationMethod.Jacobian,
                .Mode = DefaultMode(),
                .GeneNames = _genes
            }
            result.StepEffects.Add(CType(delta.Clone(), Double()))

            Dim steps As Integer = 0
            For t = 1 To MaxSteps
                Dim [next] = MatrixVectorMul(_A, current)
                For i = 0 To n - 1
                    total(i) += [next](i)
                Next
                result.StepEffects.Add([next])
                steps = t

                Dim normCur = Norm(current)
                Dim normNxt = Norm([next])
                If normCur < 1.0E-9 Then Exit For
                If normNxt / normCur < Tolerance Then Exit For
                current = [next]
            Next

            result.Effects = total
            result.Steps = steps
            Return result
        End Function

        ''' <summary>级联采样：在全局聚合网络上做多步 do-演算传播</summary>
        Private Function PropagateCascade(sourceIdx As Integer) As GlobalPerturbationResult
            Dim spec As New InterventionSpec() With {
                .GeneName = _genes(sourceIdx),
                .GeneIndex = sourceIdx,
                .Mode = DefaultMode()
            }

            Dim analyzer As New BnInterventionAnalyzer(_globalNet, _exprStd)
            Dim res = analyzer.DynamicIntervention(spec, MaxSteps, NSamples, RandomSeed)

            Dim result As New GlobalPerturbationResult() With {
                .SourceGene = _genes(sourceIdx),
                .Method = PropagationMethod.CascadeSampling,
                .Mode = DefaultMode(),
                .GeneNames = _genes,
                .Effects = CType(res.FoldChanges.Clone(), Double()),
                .Steps = MaxSteps
            }
            result.StepEffects.Add(CType(res.FoldChanges.Clone(), Double()))
            Return result
        End Function

        ' ============================================================
        ' 8. 结果导出
        ' ============================================================

        ''' <summary>
        ''' 写出全局扰动响应矩阵（行=基因，列=各扰动源）与每个源的明细 TSV，并打印摘要。
        ''' </summary>
        Public Sub SaveResults(results As List(Of GlobalPerturbationResult), outputDir As String)
            If Not Directory.Exists(outputDir) Then
                Directory.CreateDirectory(outputDir)
            End If

            ' 全局响应矩阵
            Dim sbMatrix As New StringBuilder()
            sbMatrix.Append("gene")
            For Each r In results
                sbMatrix.Append(vbTab).Append(r.SourceGene)
            Next
            sbMatrix.AppendLine()

            For i = 0 To _genes.Length - 1
                sbMatrix.Append(_genes(i))
                For Each r In results
                    sbMatrix.Append(vbTab).Append(r.Effects(i).ToString("F6"))
                Next
                sbMatrix.AppendLine()
            Next
            File.WriteAllText(Path.Combine(outputDir, "global_perturbation_responses.tsv"), sbMatrix.ToString())

            ' 每个源的明细
            For Each r In results
                Dim safe = New String(r.SourceGene.Where(Function(c) Char.IsLetterOrDigit(c)).ToArray())
                File.WriteAllText(Path.Combine(outputDir, "pert_" & safe & ".tsv"), r.ToTSV())
            Next

            ' 控制台摘要
            For Each r In results
                Console.WriteLine(r.ToString())
            Next
        End Sub

        ' ============================================================
        ' 内部辅助
        ' ============================================================

        Private Function GetGlobalIndex(name As String) As Integer
            Dim idx As Integer = -1
            If _gIndex.TryGetValue(name, idx) Then
                Return idx
            End If
            Return -1
        End Function

        Private Function GetDefaultSources() As List(Of String)
            Dim src As New List(Of String)()
            For Each kv In _moduleHubs
                If kv.Value.Count > 0 Then
                    src.Add(kv.Value(0))
                End If
            Next
            Return src
        End Function

        Private Function DefaultMode() As Intervention.InterventionMode
            ' 默认做敲低（Knockout），与 BNLearnWorkflow.KnockoutGene 一致
            Return Intervention.InterventionMode.Knockout
        End Function

        Private Function InterventionValue(sourceIdx As Integer) As Double
            ' 雅可比传播需要的是「相对野生型的扰动增量 Δx0」，而非绝对干预值。
            ' 标准化数据野生型均值≈0、SD≈1；Knockout 下调 1 个 SD、Overexpression 上调 3 倍、
            ' Knockdown 下调 2 倍（与 BnInterventionAnalyzer 中采样所用的偏离尺度一致）。
            ' 注意：不能用 GetInterventionValue(0,1) —— Knockout 返回绝对干预值 0，
            ' 在标准化数据（野生型均值已是 0）下扰动增量为 0，导致传播全 0。
            Select Case DefaultMode()
                Case Intervention.InterventionMode.Knockout
                    Return -1.0
                Case Intervention.InterventionMode.Overexpression
                    Return 3.0
                Case Intervention.InterventionMode.Knockdown
                    Return -2.0
                Case Else
                    Return 0.0
            End Select
        End Function

        Private Function CountEdges() As Integer
            Dim c As Integer = 0
            For i = 0 To _genes.Length - 1
                For j = 0 To _genes.Length - 1
                    If Math.Abs(_A(i, j)) > 1.0E-12 Then c += 1
                Next
            Next
            Return c
        End Function

        ' ---- 模块 eigengene 相关 ----
        Private Function ModuleEigengeneVector(moduleColor As String) As Double()
            Dim genes = _moduleGenes(moduleColor)
            Dim nS = _exprStd.NSample
            Dim vec = New Double(nS - 1) {}
            For j = 0 To nS - 1
                Dim sum As Double = 0
                Dim cnt As Integer = 0
                For Each g In genes
                    Dim gi = GetGlobalIndex(g)
                    If gi >= 0 Then
                        sum += _exprStd.Matrix(gi, j)
                        cnt += 1
                    End If
                Next
                vec(j) = If(cnt > 0, sum / cnt, 0)
            Next
            Return vec
        End Function

        Private Function ModuleEigengeneCorrelation(ma As String, mb As String) As Double
            Return Pearson(ModuleEigengeneVector(ma), ModuleEigengeneVector(mb))
        End Function

        Private Function GeneCorrelation(ga As String, gb As String) As Double
            Dim iga = GetGlobalIndex(ga)
            Dim igb = GetGlobalIndex(gb)
            If iga < 0 OrElse igb < 0 Then Return 0
            Dim nS = _exprStd.NSample
            Dim x = New Double(nS - 1) {}
            Dim y = New Double(nS - 1) {}
            For j = 0 To nS - 1
                x(j) = _exprStd.Matrix(iga, j)
                y(j) = _exprStd.Matrix(igb, j)
            Next
            Return Pearson(x, y)
        End Function

        ' ---- 线性代数辅助 ----
        Private Function MatrixVectorMul(A As Double(,), v As Double()) As Double()
            Dim n = v.Length
            Dim out = New Double(n - 1) {}
            For i = 0 To n - 1
                Dim s As Double = 0
                For j = 0 To n - 1
                    s += A(i, j) * v(j)
                Next
                out(i) = s
            Next
            Return out
        End Function

        Private Function Norm(v As Double()) As Double
            Dim s As Double = 0
            For i = 0 To v.Length - 1
                s += v(i) * v(i)
            Next
            Return Math.Sqrt(s)
        End Function

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

    End Class

End Namespace
