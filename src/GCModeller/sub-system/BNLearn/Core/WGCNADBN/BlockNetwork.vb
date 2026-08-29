Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Math.Correlations
Imports SMRUCC.genomics.Analysis.BNLearn.ParameterLearning
Imports SMRUCC.genomics.Analysis.BNLearn.StructureLearning

Namespace Core.WGCNADBN

    Public Class BlockNetwork

        ' ---- 训练参数（与 BNLearnWorkflow 风格一致） ----
        ''' <summary>是否对表达数据做标准化（z-score），默认 True</summary>
        Public Property NormalizeData As Boolean = True

        ''' <summary>结构学习参数（算法/显著性阈值/最大父节点数/随机种子）</summary>
        Public Property StructureParams As New StructureLearningParams()

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

        Private _gIndex As New Dictionary(Of String, Integer)()
        Private _moduleGenes As New Dictionary(Of String, List(Of String))()
        Private _moduleHubs As New Dictionary(Of String, List(Of String))()
        Private _subNets As New List(Of BayesianNetwork)()

        Friend _globalNet As BayesianNetwork
        Friend _genes As String()
        Friend _A As Double(,)
        Friend _exprStd As GeneExpressionData

        ''' <summary>
        ''' 模块切分 → 子网络训练 → 全局矩阵拼接
        ''' </summary>
        ''' <param name="expr">全局表达矩阵（基因 × 样本）</param>
        Sub New(expr As GeneExpressionData, Optional normalizeData As Boolean = True)
            _NormalizeData = normalizeData
            _expr = expr
            _genes = expr.GeneNames
            _gIndex = New Dictionary(Of String, Integer)()

            For i = 0 To _genes.Length - 1
                _gIndex(_genes(i)) = i
            Next

            ' 标准化数据用于训练（与 BNLearnWorkflow 一致）
            If normalizeData Then
                _exprStd = expr.Standardize()
            Else
                _exprStd = expr
            End If
        End Sub

        Public Iterator Function GetModuleHubSources() As IEnumerable(Of String)
            For Each kv In _moduleHubs
                If kv.Value.Count > 0 Then
                    Yield kv.Value(0)
                End If
            Next
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="assignment">WGCNA 模块划分结果（geneID / moduleColor / kME）</param>
        ''' <returns></returns>
        Public Function Learn(assignment As GeneModuleColor()) As BlockNetwork
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
                Call $"[WGCNASubnetworkPipeline] 仅保留模块基因参加全局网络: {_genes.Length}/{_expr.GeneNames.Length}".debug
            End If

            Call TrainSubnetworks()
            Call BuildInitialJacobian()
            Call BuildCrossModuleEdges()
            Call BuildGlobalNetwork()

            Call $"[WGCNASubnetworkPipeline] 模块数={_moduleGenes.Count}, 全局基因数={_genes.Length}, 全局网络边数≈{CountEdges()}".debug

            Return Me
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

        Public Function GetGlobalIndex(name As String) As Integer
            Dim idx As Integer = -1
            If _gIndex.TryGetValue(name, idx) Then
                Return idx
            End If
            Return -1
        End Function

        Private Function CountEdges() As Integer
            Dim c As Integer = 0
            For i = 0 To _genes.Length - 1
                For j = 0 To _genes.Length - 1
                    If Math.Abs(_A(i, j)) > 0.000000000001 Then c += 1
                Next
            Next
            Return c
        End Function

        Private Function ModuleEigengeneCorrelation(ma As String, mb As String) As Double
            Return Correlations.GetPearson(ModuleEigengeneVector(ma), ModuleEigengeneVector(mb))
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
            Return Correlations.GetPearson(x, y)
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
    End Class
End Namespace