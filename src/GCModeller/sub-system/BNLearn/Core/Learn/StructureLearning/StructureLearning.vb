#Region "Microsoft.VisualBasic::15f6960fb8e75bd42055a99c1c0bcac9, sub-system\BNLearn\StructureLearning\StructureLearning.vb"

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

'   Total Lines: 787
'    Code Lines: 538 (68.36%)
' Comment Lines: 125 (15.88%)
'    - Xml Docs: 31.20%
' 
'   Blank Lines: 124 (15.76%)
'     File Size: 31.03 KB


'     Class BnStructureLearner
' 
'         Function: BetaCF, BuildDesignMatrix, ComputeNetworkBIC, ComputeNodeBIC, GammaLn
'                   IncompleteBeta, Learn, LinearRegression, MatrixInverse, MMPCPhase
'                   NormalCDF, PartialCorrelation, TDistPValue
' 
'         Sub: HillClimbingSearch, PrecomputeStatistics, TabuSearch
' 
' 
' /********************************************************************************/

#End Region

' ============================================================
' StructureLearning.vb - 贝叶斯网络结构学习
' ============================================================
' 实现 MMHC（Max-Min Hill-Climbing）混合算法：
'   第一阶段：MMPC 约束型算法限制候选边
'   第二阶段：Hill-Climbing 评分搜索确定最优 DAG
'
' 评分函数：BIC（贝叶斯信息准则）
'   BIC = -2·LL + k·log(n)
'   其中 LL = 对数似然，k = 参数数，n = 样本数
'
' 对于高斯贝叶斯网络：
'   对数似然 = -n/2·log(2πσ²) - 1/(2σ²)·Σ(xi-μi)²
' ============================================================

Imports System.Collections.Concurrent
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Data.Bootstrapping.Multivariate
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports _rng = Microsoft.VisualBasic.Math.RandomExtensions

Namespace StructureLearning

    ''' <summary>
    ''' 贝叶斯网络结构学习器
    ''' </summary>
    Public Class BnStructureLearner

        Private _data As Core.GeneExpressionData
        Private _params As StructureLearningParams

        ' 缓存统计量
        Private _means As Double()
        Private _sds As Double()
        Private _corrMatrix As Double(,)

        ''' <summary>物化的基因表达列向量 _geneExpr(g)：避免热路径上反复分配数组</summary>
        Private _geneExpr As Double()()

        ''' <summary>预计算的无父模型局部BIC评分（常数项，奇异矩阵时的回退目标）</summary>
        Private _rootlessScores As Double()

        ''' <summary>当前迭代的节点局部BIC缓存（每次搜索迭代开头由 CacheLocalScores 刷新）</summary>
        Private _localScores As Double()

        ''' <summary>白名单正向边索引集：保护白名单边不被结构搜索删除/反转</summary>
        Private _wlEdges As HashSet(Of (Integer, Integer))

        ''' <summary>并行归约各线程局部最优候选时的合并锁</summary>
        Private ReadOnly _mergeLock As New Object

        ''' <summary>
        ''' 从基因表达数据学习网络结构
        ''' </summary>
        Public Function Learn(data As Core.GeneExpressionData, params As StructureLearningParams, Optional prior As Core.PriorNetwork = Nothing) As StructureLearningResult
            Dim t0 As Date = Now

            _data = data
            _params = params
            _rng.SetSeed(params.RandomSeed)

            ' 物化基因表达列（后续热点路径统一从列向量读取）
            MaterializeColumns()

            ' 预计算统计量
            PrecomputeStatistics()

            ' 预计算无父模型评分（回归奇异时的回退目标）
            InitRootlessScores()

            ' 创建初始空网络
            Dim net As New Core.BayesianNetwork(data.GeneNames)

            Call "loading white list and black list from bayesian piror network".debug

            ' 构建白名单索引集合（保护白名单边不被删除/反转）
            _wlEdges = New HashSet(Of (Integer, Integer))

            ' 注入先验知识（白名单/黑名单）
            If prior IsNot Nothing Then
                Dim wl = prior.ToWhitelist(data.GeneNames)
                For Each edge In wl
                    net.AddWhitelistEdge(edge.fromIdx, edge.toIdx)
                    _wlEdges.Add((edge.fromIdx, edge.toIdx))
                Next
            End If

            ' 添加基因名互不调控的黑名单（可选：禁止自环）
            For i = 0 To data.NGene - 1
                net.AddBlacklistEdge(i, i)
            Next

            ' 根据算法选择学习策略
            Select Case params.Algorithm
                Case StructureAlgorithm.HillClimbing
                    HillClimbingSearch(net)

                Case StructureAlgorithm.Tabu
                    TabuSearch(net)

                Case StructureAlgorithm.MMHC
                    ' 第一阶段：MMPC 约束型搜索限制候选边
                    Dim candidateEdges As HashSet(Of (Integer, Integer)) = MMPCPhase(net)
                    ' 第二阶段：HC 评分搜索
                    HillClimbingSearch(net, candidateEdges)

            End Select

            ' 计算最终 BIC
            Dim finalBIC As Double = ComputeNetworkBIC(net)

            Call $"final network BIC is {finalBIC}".debug

            Return New StructureLearningResult() With {
                .Network = net,
                .FinalBIC = finalBIC,
                .Iterations = _params.MaxIterations,
                .ElapsedMs = (Now - t0).TotalMilliseconds
            }
        End Function

        ' ==================== 预计算统计量 ====================

        Private Sub PrecomputeStatistics()
            Dim nG As Integer = _data.NGene
            Dim nS As Integer = _data.NSample

            Call "make pre-compute statistics...".debug
            Call "  - means & sds".debug

            ' 均值和标准差（从物化的列向量读取）
            _means = New Double(nG - 1) {}
            _sds = New Double(nG - 1) {}
            For i = 0 To nG - 1
                Dim col As Double() = _geneExpr(i)

                Dim sum As Double = 0
                For j = 0 To nS - 1
                    sum += col(j)
                Next
                Dim mean As Double = sum / nS
                _means(i) = mean

                Dim ss As Double = 0
                For j = 0 To nS - 1
                    ss += (col(j) - mean) ^ 2
                Next
                _sds(i) = Math.Sqrt(ss / (nS - 1))
                If _sds(i) < 0.000000000000001 Then _sds(i) = 1.0
            Next

            Call $" - correlation matrix".debug

            ' 相关系数矩阵：行所有权分区并行
            ' 第 i 行的计算拥有所有 j >= i 的格子及其镜像位置，不同行的写入地址集两两不相交，无需加锁
            _corrMatrix = New Double(nG - 1, nG - 1) {}

            Call Parallel.For(
                0, nG,
                body:=Sub(i)
                          Dim xi As Double() = _geneExpr(i)
                          Dim mi As Double = _means(i)
                          Dim si As Double = _sds(i)

                          For j = i To nG - 1
                              If i = j Then
                                  _corrMatrix(i, j) = 1.0
                              Else
                                  Dim xj As Double() = _geneExpr(j)
                                  Dim mj As Double = _means(j)
                                  Dim sj As Double = _sds(j)

                                  Dim cov As Double = 0
                                  For k = 0 To nS - 1
                                      cov += (xi(k) - mi) * (xj(k) - mj)
                                  Next
                                  cov /= (nS - 1)
                                  Dim r As Double = cov / (si * sj)
                                  _corrMatrix(i, j) = r
                                  _corrMatrix(j, i) = r
                              End If
                          Next
                      End Sub)
        End Sub

        ' ==================== MMPC 约束型阶段 ====================

        ''' <summary>
        ''' MMPC 阶段：为每个节点找到候选父节点集合
        ''' 使用偏相关系数的条件独立性检验
        ''' </summary>
        ''' <remarks>
        ''' 以目标节点为粒度并行分发：各 target 的 CPC 推断相互独立且只读共享统计量，
        ''' 各自写入私有列表，结束后由主线程合并入共享候选集（消除共享 HashSet 写竞争）
        ''' </remarks>
        Private Function MMPCPhase(net As Core.BayesianNetwork) As HashSet(Of (Integer, Integer))
            Dim nG As Integer = _data.NGene
            Dim candidates As New HashSet(Of (Integer, Integer))()

            Call "do MMPC phase (parallel over target nodes)".debug

            ' 白名单边直接加入候选
            For Each wl In net.Whitelist
                candidates.Add((wl.FromIdx, wl.ToIdx))
            Next

            ' 对每个目标节点，寻找候选父节点（独立任务，并行分发）
            Dim results As New ConcurrentBag(Of (Target As Integer, Cpc As List(Of Integer)))

            Call Parallel.For(
                0, nG,
                body:=Sub(target)
                          results.Add((target, MMPCCollect(target)))
                      End Sub)

            ' 合并候选边
            For Each item In results
                For Each parent In item.Cpc
                    candidates.Add((parent, item.Target))
                Next
            Next

            Return candidates
        End Function

        ''' <summary>
        ''' 单个目标节点的 MMPC 推断（前向 + 后向阶段）
        ''' 只读共享统计量（相关矩阵等），返回该目标节点的候选父节点列表
        ''' </summary>
        Private Function MMPCCollect(target As Integer) As List(Of Integer)
            Dim nG As Integer = _data.NGene
            Dim CPC As New List(Of Integer)()  ' 候选父节点集

            ' 前向阶段：逐步加入最相关的变量
            Dim remaining As New List(Of Integer)()
            For i = 0 To nG - 1
                If i <> target Then
                    remaining.Add(i)
                End If
            Next

            While remaining.Count > 0
                ' 找到与 target 条件依赖最强的变量
                Dim bestVar As Integer = -1
                Dim bestAssoc As Double = 0

                For Each v In remaining
                    ' 计算偏相关系数 |ρ(target, v | CPC)|
                    Dim pCorr As Double = PartialCorrelation(target, v, CPC)
                    Dim absPcorr As Double = Math.Abs(pCorr)

                    If absPcorr > bestAssoc Then
                        bestAssoc = absPcorr
                        bestVar = v
                    End If
                Next

                If bestVar < 0 Then Exit While

                ' 独立性检验（基于偏相关的 t 检验）
                Dim nS As Integer = _data.NSample
                Dim df As Integer = nS - 2 - CPC.Count
                If df < 1 Then Exit While

                Dim tStat As Double = bestAssoc * Math.Sqrt(df / (1.0 - bestAssoc * bestAssoc + 0.000000000000001))
                Dim pValue As Double = TDistPValue(Math.Abs(tStat), df)

                If pValue < _params.Alpha Then
                    ' 显著依赖，加入候选集
                    CPC.Add(bestVar)
                    remaining.Remove(bestVar)

                    ' 限制最大候选数
                    If CPC.Count >= _params.MaxParents * 2 Then Exit While
                Else
                    ' 不显著，移除
                    remaining.Remove(bestVar)
                End If
            End While

            ' 后向阶段：移除在更大条件集下变得独立的变量
            For i = CPC.Count - 1 To 0 Step -1
                Dim testVar As Integer = CPC(i)
                Dim conditionSet As New List(Of Integer)(CPC)
                conditionSet.RemoveAt(i)

                Dim pCorr As Double = PartialCorrelation(target, testVar, conditionSet)
                Dim df As Integer = nG - 2 - conditionSet.Count
                If df < 1 Then Continue For

                Dim tStat As Double = Math.Abs(pCorr) * Math.Sqrt(df / (1.0 - pCorr * pCorr + 0.000000000000001))
                Dim pValue As Double = TDistPValue(tStat, df)

                If pValue >= _params.Alpha Then
                    CPC.RemoveAt(i)
                End If
            Next

            Return CPC
        End Function

        ''' <summary>
        ''' 计算偏相关系数 ρ(x, y | Z)
        ''' 使用递推公式从全相关矩阵计算
        ''' </summary>
        Private Function PartialCorrelation(x As Integer, y As Integer, zSet As List(Of Integer)) As Double
            If zSet.Count = 0 Then
                Return _corrMatrix(x, y)
            End If

            If zSet.Count = 1 Then
                Dim z As Integer = zSet(0)
                Dim rxy As Double = _corrMatrix(x, y)
                Dim rxz As Double = _corrMatrix(x, z)
                Dim ryz As Double = _corrMatrix(y, z)
                Dim denom As Double = Math.Sqrt((1 - rxz * rxz) * (1 - ryz * ryz))
                If Math.Abs(denom) < 0.000000000000001 Then Return 0
                Return (rxy - rxz * ryz) / denom
            End If

            ' 多变量偏相关：使用矩阵求逆法
            ' 构建增广矩阵 [x, y, z1, z2, ...]
            Dim vars As New List(Of Integer)() From {x, y}
            vars.AddRange(zSet)

            Dim k As Integer = vars.Count

            ' 提取子相关矩阵
            Dim R As Double(,) = New Double(k - 1, k - 1) {}
            For i = 0 To k - 1
                For j = 0 To k - 1
                    R(i, j) = _corrMatrix(vars(i), vars(j))
                Next
            Next

            ' 求逆矩阵
            Dim invR As Double(,) = MatrixOps.Inverse(R, strict:=True, throwSingularity:=False)
            If invR Is Nothing Then Return 0

            ' 偏相关 = -invR(0,1) / sqrt(invR(0,0) * invR(1,1))
            Dim denomVal As Double = Math.Sqrt(Math.Abs(invR(0, 0) * invR(1, 1)))
            If denomVal < 0.000000000000001 Then Return 0
            Return -invR(0, 1) / denomVal
        End Function

        ' ==================== Delta 评分与并行归约基础设施 ====================

        ''' <summary>将基因表达矩阵按基因物化为列向量（一次构建，全程复用）</summary>
        Private Sub MaterializeColumns()
            Dim nG As Integer = _data.NGene
            Dim nS As Integer = _data.NSample

            _geneExpr = New Double(nG - 1)() {}

            For g = 0 To nG - 1
                Dim col As Double() = New Double(nS - 1) {}

                For s = 0 To nS - 1
                    col(s) = _data.Matrix(g, s)
                Next

                _geneExpr(g) = col
            Next
        End Sub

        ''' <summary>预计算所有节点的无父模型BIC评分（常数项）</summary>
        Private Sub InitRootlessScores()
            Dim nG As Integer = _data.NGene
            _rootlessScores = New Double(nG - 1) {}

            Call Parallel.For(
                0, nG,
                body:=Sub(k)
                          _rootlessScores(k) = RootlessBic(k)
                      End Sub)
        End Sub

        ''' <summary>
        ''' 无父模型下的局部 BIC：边际分布 N(μ, σ²)，参数数 k=2
        ''' </summary>
        Private Function RootlessBic(nodeIdx As Integer) As Double
            Dim nS As Integer = _data.NSample
            Dim y As Double() = _geneExpr(nodeIdx)
            Dim mean As Double = _means(nodeIdx)

            Dim rss As Double = 0
            For j = 0 To nS - 1
                rss += (y(j) - mean) ^ 2
            Next

            Dim sigma2 As Double = rss / nS
            If sigma2 < 0.000000000000001 Then sigma2 = 0.000000000000001

            Dim ll As Double = -nS / 2.0 * Math.Log(2 * Math.PI * sigma2) - rss / (2 * sigma2)
            Return -2 * ll + 2 * Math.Log(nS) * _params.BICPenalty
        End Function

        ''' <summary>
        ''' 在每次搜索迭代开头预缓存全部节点的当前局部BIC评分
        ''' （迭代周期内的网络级唯一一次 fork/join，粗粒度任务适合并行）
        ''' </summary>
        Private Sub CacheLocalScores(net As Core.BayesianNetwork)
            Dim scores As Double() = New Double(net.Nodes.Count - 1) {}

            Call Parallel.For(
                0, net.Nodes.Count,
                body:=Sub(k)
                          scores(k) = ScoreNodeWithParents(k, net.Nodes(k).Parents)
                      End Sub)

            _localScores = scores
        End Sub

        ''' <summary>
        ''' 给定任意假想父母集合，计算目标节点的局部BIC
        ''' 纯函数式评估：不读取/不修改共享网络的 Parents 视图，多线程天然安全
        ''' </summary>
        Private Function ScoreNodeWithParents(targetIdx As Integer, parents As List(Of Integer)) As Double
            If parents.Count = 0 Then
                Return _rootlessScores(targetIdx)
            End If

            Dim nS As Integer = _data.NSample
            Dim y As Double() = _geneExpr(targetIdx)
            Dim x As Double(,) = BuildDesignMatrix(parents, nS)
            Dim coeffs As Double() = NormalEquation.LinearRegression(x, y, nS, parents.Count)

            If coeffs Is Nothing Then
                ' 回归设计矩阵奇异：退化为无父模型评分
                ' （修复原先“以均值数值当作索引查表”导致的越界缺陷）
                Return _rootlessScores(targetIdx)
            End If

            Return RegressedBic(targetIdx, coeffs, parents)
        End Function

        ''' <summary>给定回归系数的局部高斯BIC</summary>
        Private Function RegressedBic(nodeIdx As Integer, coeffs As Double(), parents As List(Of Integer)) As Double
            Dim nS As Integer = _data.NSample
            Dim y As Double() = _geneExpr(nodeIdx)
            Dim nP As Integer = parents.Count

            ' 缓存父母列引用，避免最内层循环反复做二维索引
            Dim px As Double()() = New Double(nP - 1)() {}
            For p = 0 To nP - 1
                px(p) = _geneExpr(parents(p))
            Next

            Dim rss As Double = 0
            For j = 0 To nS - 1
                Dim predicted As Double = coeffs(0)  ' 截距

                For p = 0 To nP - 1
                    predicted += coeffs(p + 1) * px(p)(j)
                Next

                rss += (y(j) - predicted) ^ 2
            Next

            Dim sigma2 As Double = rss / nS
            If sigma2 < 0.000000000000001 Then sigma2 = 0.000000000000001

            Dim ll As Double = -nS / 2.0 * Math.Log(2 * Math.PI * sigma2) - rss / (2 * sigma2)
            Dim k As Integer = nP + 2  ' β0, β1...βp, σ²
            Return -2 * ll + k * Math.Log(nS) * _params.BICPenalty
        End Function

        ''' <summary>
        ''' 只读检测是否存在 fromV ⇒ toV 的有向路径（可选排除 forbidFrom→forbidTo 一条直接边）
        ''' 用于 Reverse 操作的无环判定：删除 i→j 后再加 j→i 合法 ⇔ 图中不存在不含直接边(i,j)的 i⇒j 路径
        ''' </summary>
        Private Function PathExistsExcluding(net As Core.BayesianNetwork,
                                             fromV As Integer, toV As Integer,
                                             forbidFrom As Integer, forbidTo As Integer) As Boolean
            If fromV = toV Then Return True

            Dim visited As Boolean() = New Boolean(net.Nodes.Count - 1) {}
            visited(fromV) = True
            Dim stack As New Stack(Of Integer)()
            stack.Push(fromV)

            Do While stack.Count > 0
                Dim cur As Integer = stack.Pop()

                For nxt As Integer = 0 To net.Nodes.Count - 1
                    If net.Adjacency(cur, nxt) AndAlso Not visited(nxt) Then
                        If cur = forbidFrom AndAlso nxt = forbidTo Then
                            Continue For
                        End If

                        If nxt = toV Then Return True

                        visited(nxt) = True
                        stack.Push(nxt)
                    End If
                Next
            Loop

            Return False
        End Function

        ''' <summary>
        ''' 边操作候选项 —— 供外层并行搜索的 thread-local 局部最优归约使用
        ''' </summary>
        Private Class EdgeCandidate
            Public Op As EdgeOp = EdgeOp.None
            Public FromIdx As Integer = -1
            Public ToIdx As Integer = -1
            Public Delta As Double = 0

            ''' <summary>扫描序号：用于多个候选 delta 并列时的确定性 tie-break</summary>
            Public Rank As Long = Long.MaxValue
        End Class

        ''' <summary>
        ''' 计算候选操作的串行扫描序号（与原串行实现的嵌套遍历次序一一对应）：
        ''' i 升序 × j 升序，且同一 (i,j) 组合内 Remove 先于 Reverse 先于 Add
        ''' </summary>
        Private Shared Function ScanRank(nG As Integer, i As Integer, j As Integer, op As EdgeOp) As Long
            Dim opIdx As Integer

            Select Case op
                Case EdgeOp.Remove : opIdx = 0
                Case EdgeOp.Reverse : opIdx = 1
                Case Else : opIdx = 2  ' EdgeOp.Add
            End Select

            Return CLng(i) * CLng(nG) * 3L + CLng(j) * 3L + opIdx
        End Function

        ''' <summary>
        ''' (delta, rank) 全序下的候选优劣判定：
        ''' delta 严格更小者胜；delta 并列时取扫描序更靠前者 ——
        ''' 使并行归约结果与串行实现的“字典序首个最小值”语义逐位一致，消除线程调度引起的非确定性
        ''' </summary>
        Private Shared Function IsBetterCandidate(candidate As EdgeCandidate, current As EdgeCandidate) As Boolean
            If candidate.Op = EdgeOp.None Then Return False
            If current.Op = EdgeOp.None Then Return True

            Return candidate.Delta < current.Delta OrElse
                   (candidate.Delta = current.Delta AndAlso candidate.Rank < current.Rank)
        End Function

        ''' <summary>尝试以更优的 (delta, rank) 全序更新局部最优候选</summary>
        Private Shared Sub TryUpdateCandidate(candidate As EdgeCandidate,
                                              op As EdgeOp, delta As Double,
                                              fromIdx As Integer, toIdx As Integer,
                                              scanRank As Long)
            If delta < candidate.Delta OrElse
                (delta = candidate.Delta AndAlso scanRank < candidate.Rank) Then

                candidate.Delta = delta
                candidate.Op = op
                candidate.FromIdx = fromIdx
                candidate.ToIdx = toIdx
                candidate.Rank = scanRank
            End If
        End Sub

        ''' <summary>
        ''' 枚举 (i,j) 组合上的全部合法操作并通过 report 汇报各自 delta 评分。
        ''' 纯只读评估：不修改 net 结构、不写任何共享状态，多线程下天然安全。
        ''' delta 依据 BIC 的马尔可夫局部性：
        '''   删除/添加边 i→j 只改变节点 j 的局部评分；
        '''   反转 i→j 为 j→i 同时改变 j（失去父 i）与 i（获得父 j）两端评分。
        ''' </summary>
        Private Sub EnumerateLegalOps(net As Core.BayesianNetwork,
                                      i As Integer, j As Integer,
                                      candidateEdges As HashSet(Of (Integer, Integer)),
                                      blacks As HashSet(Of (Integer, Integer)),
                                      includeReverse As Boolean,
                                      report As Action(Of EdgeOp, Double, Long))

            If i = j Then Return

            ' 检查是否在候选集中（MMPC阶段限制）
            If candidateEdges IsNot Nothing Then
                Dim hasForward As Boolean = candidateEdges.Contains((i, j))
                Dim hasReverseDir As Boolean = candidateEdges.Contains((j, i))

                If Not hasForward AndAlso Not hasReverseDir AndAlso Not net.HasEdge(i, j) Then
                    Return
                End If
            End If

            ' 黑名单边既不能建立也不能维护（保持原实现对该方向的跳过语义）
            If blacks.Contains((i, j)) Then Return

            Dim maxParents As Integer = _params.MaxParents

            If net.HasEdge(i, j) Then
                ' 白名单正向边必须被保留（修复白名单边可被误删除/反转的缺陷）
                If _wlEdges.Contains((i, j)) Then Return

                ' ---- 操作1：删除边 i→j（仅影响节点 j）----
                Dim newParentsJ As New List(Of Integer)(net.Nodes(j).Parents)
                newParentsJ.Remove(i)

                Call report(
                    EdgeOp.Remove,
                    ScoreNodeWithParents(j, newParentsJ) - _localScores(j),
                    ScanRank(net.Nodes.Count, i, j, EdgeOp.Remove))

                ' ---- 操作2：反转 i→j ⇒ j→i（影响节点 j 与 i）----
                If includeReverse AndAlso Not net.HasEdge(j, i) AndAlso Not blacks.Contains((j, i)) Then
                    Dim parentsI As List(Of Integer) = net.Nodes(i).Parents

                    ' 反转后 i 将新增一个父节点，不得超过 MaxParents（补充原实现缺失的上限检查）
                    If parentsI.Count < maxParents Then
                        ' 等价于原实现“先删 i→j 再 AddEdge(j,i)”的无环校验
                        If Not PathExistsExcluding(net, i, j, i, j) Then
                            Dim newParentsI As New List(Of Integer)(parentsI)
                            newParentsI.Add(j)

                            Dim revDelta As Double =
                                (ScoreNodeWithParents(j, newParentsJ) - _localScores(j)) +
                                (ScoreNodeWithParents(i, newParentsI) - _localScores(i))

                            Call report(
                                EdgeOp.Reverse, revDelta,
                                ScanRank(net.Nodes.Count, i, j, EdgeOp.Reverse))
                        End If
                    End If
                End If
            Else
                ' ---- 操作3：添加边 i→j（仅影响节点 j）----
                If net.Nodes(j).Parents.Count < maxParents Then
                    ' 与 net.AddEdge 的无环校验等价，但为纯只读操作
                    If Not net.WouldCreateCycle(i, j) Then
                        Dim newParentsJ As New List(Of Integer)(net.Nodes(j).Parents)
                        newParentsJ.Add(i)

                        Call report(
                            EdgeOp.Add,
                            ScoreNodeWithParents(j, newParentsJ) - _localScores(j),
                            ScanRank(net.Nodes.Count, i, j, EdgeOp.Add))
                    End If
                End If
            End If
        End Sub

        ' ==================== Hill-Climbing 搜索 ====================

        ''' <summary>
        ''' Hill-Climbing 结构搜索（delta 评分 + 外层并行归约）
        ''' </summary>
        ''' <remarks>
        ''' 与传统“试探修改网络 + 全网重评分”的模式不同：
        '''   1. 每轮迭代开头只做一次网络级 Parallel.For，预缓存全部节点的局部BIC；
        '''   2. 内层 (i,j) 操作评估纯只读，仅重算受影响节点的局部分（delta 评估），
        '''      单次评估成本由 O(nG·cost) 降为 O(cost)；
        '''   3. 外层 i 循环以 thread-local 归约方式展开到全部CPU核心。
        ''' 收敛后校验内部跟踪 BIC 与重新计算 BIC 的漂移量（应≈0），并断言网络为合法 DAG。
        ''' </remarks>
        Private Sub HillClimbingSearch(net As Core.BayesianNetwork, Optional candidateEdges As HashSet(Of (Integer, Integer)) = Nothing)
            Dim bar As ProgressBar = Nothing
            Dim eps As Double = 0.0000000001

            Call "do hill climbing search (delta-scoring, parallel-reduce)".info

            For Each iter As Integer In TqdmWrapper.Range(0, _params.MaxIterations, bar:=bar)
                ' ---- 步骤1: 主线程重建索引，预缓存当前网络的节点局部评分 ----
                Call net.MakeBlackIndex()
                Call CacheLocalScores(net)

                Dim currentBIC As Double = _localScores.Sum
                Dim blacks As HashSet(Of (Integer, Integer)) = net.blackEdges
                Dim nG As Integer = net.Nodes.Count

                ' ---- 步骤2: 外层 i 循环并行 —— thread-local 局部最优 + 锁合并归约 ----
                Dim best As EdgeCandidate = Nothing

                Call Parallel.For(
                    0, nG,
                    localInit:=Function() New EdgeCandidate(),
                    body:=Function(i, loopState, localBest)
                              For j = 0 To nG - 1
                                  ' 迭代变量的值语义副本（消除 BC42324 捕获歧义）
                                  Dim jj As Integer = j

                                  Call EnumerateLegalOps(
                                      net, i, jj, candidateEdges, blacks, includeReverse:=True,
                                      report:=Sub(op As EdgeOp, delta As Double, scanRank As Long)
                                                  Call TryUpdateCandidate(localBest, op, delta, i, jj, scanRank)
                                              End Sub)
                              Next
                              Return localBest
                          End Function,
                    localFinally:=Sub(localBest)
                                      SyncLock _mergeLock
                                          If best Is Nothing OrElse IsBetterCandidate(localBest, best) Then
                                              best = localBest
                                          End If
                                      End SyncLock
                                  End Sub)

                Dim bestDeltaShown As Double = If(best IsNot Nothing, best.Delta, 0.0)
                Call bar.SetLabel($"best-delta={bestDeltaShown:F6}; current-BIC={currentBIC:F4}")

                ' ---- 步骤3: 主线程执行最优操作（对网络的唯一写入位置）----
                If best Is Nothing OrElse best.Op = EdgeOp.None OrElse best.Delta >= -eps Then
                    Exit For  ' 无法改善，停止
                End If

                Select Case best.Op
                    Case EdgeOp.Add
                        net.AddEdge(best.FromIdx, best.ToIdx)
                    Case EdgeOp.Remove
                        net.RemoveEdge(best.FromIdx, best.ToIdx)
                    Case EdgeOp.Reverse
                        net.RemoveEdge(best.FromIdx, best.ToIdx)
                        net.AddEdge(best.ToIdx, best.FromIdx)
                End Select

                ' ---- 步骤4: 仅增量刷新被操作波及的节点缓存（其余节点缓存依旧精确有效）----
                _localScores(best.ToIdx) = ScoreNodeWithParents(best.ToIdx, net.Nodes(best.ToIdx).Parents)

                If best.Op = EdgeOp.Reverse Then
                    _localScores(best.FromIdx) = ScoreNodeWithParents(best.FromIdx, net.Nodes(best.FromIdx).Parents)
                End If
            Next

            ' ---- 收尾自检：并行记账正确性 ----
            Dim trackedBIC As Double = If(_localScores Is Nothing, ComputeNetworkBIC(net), _localScores.Sum)
            Dim recomputedBIC As Double = ComputeNetworkBIC(net, parallelOp:=True)
            Dim drift As Double = Math.Abs(trackedBIC - recomputedBIC)

            Call $"[self-check] tracked-BIC={trackedBIC:F6}; recomputed-BIC={recomputedBIC:F6}; drift={drift:E3}".debug

            If drift > 0.001 Then
                Call "[self-check] WARNING: BIC bookkeeping drift detected!".warning
            End If

            If Not net.IsDAG() Then
                Call "[self-check] ERROR: learned network is NOT a DAG!".warning
            Else
                Call "[self-check] network DAG validation passed".debug
            End If
        End Sub

        ' ==================== Tabu 搜索 ====================

        ''' <summary>
        ''' Tabu 结构搜索（delta 评分版，复用 Hill-Climbing 的并行归约骨架）
        ''' </summary>
        ''' <remarks>
        ''' 保持原实现的禁忌表与“渴望准则”（优于历史最优的禁忌操作可解禁）语义；
        ''' 评估方式改为纯只读 delta 评分，并统一纳入黑名单/白名单/父数上限等合法性约束。
        ''' 原实现不含反转操作，此处同样仅枚举 删除/添加 两类操作。
        ''' </remarks>
        Private Sub TabuSearch(net As Core.BayesianNetwork)
            Dim bestBIC As Double = ComputeNetworkBIC(net, parallelOp:=True)
            Dim bestNet As Core.BayesianNetwork = net.CloneStructure()
            Dim tabuList As New Queue(Of String)()

            Call "do tabu search (delta-scoring, parallel-reduce)".debug

            For Each iter As Integer In TqdmWrapper.Range(0, _params.MaxIterations)
                Call net.MakeBlackIndex()
                Call CacheLocalScores(net)

                Dim currentBIC As Double = _localScores.Sum
                Dim escapeThreshold As Double = bestBIC - currentBIC   ' delta 低于此值即优于历史最优
                Dim blacks As HashSet(Of (Integer, Integer)) = net.blackEdges
                Dim tabuSnapshot As New HashSet(Of String)(tabuList)   ' 并行段内使用的只读快照
                Dim nG As Integer = net.Nodes.Count

                Dim best As EdgeCandidate = Nothing

                Call Parallel.For(
                    0, nG,
                    localInit:=Function() New EdgeCandidate() With {.Delta = Double.MaxValue},
                    body:=Function(i, loopState, localBest)
                              For j = 0 To nG - 1
                                  ' 迭代变量的值语义副本（消除 BC42324 捕获歧义）
                                  Dim jj As Integer = j

                                  Call EnumerateLegalOps(
                                      net, i, jj, Nothing, blacks, includeReverse:=False,
                                      report:=Sub(op As EdgeOp, delta As Double, scanRank As Long)
                                                  Dim opKey As String = $"{CInt(op)}_{i}_{jj}"

                                                  If tabuSnapshot.Contains(opKey) Then
                                                      ' 渴望准则：禁忌操作若能改进历史全局最优则允许解禁
                                                      If delta < escapeThreshold Then
                                                          localBest.Delta = delta
                                                          localBest.Op = op
                                                          localBest.FromIdx = i
                                                          localBest.ToIdx = jj
                                                          localBest.Rank = scanRank
                                                      End If
                                                  Else
                                                      Call TryUpdateCandidate(localBest, op, delta, i, jj, scanRank)
                                                  End If
                                              End Sub)
                              Next
                              Return localBest
                          End Function,
                    localFinally:=Sub(localBest)
                                      SyncLock _mergeLock
                                          If best Is Nothing OrElse IsBetterCandidate(localBest, best) Then
                                              best = localBest
                                          End If
                                      End SyncLock
                                  End Sub)

                ' 无任何可行操作（候选全为禁忌且未满足渴望条件）
                If best Is Nothing OrElse best.Op = EdgeOp.None Then
                    Exit For
                End If

                ' 执行操作
                Select Case best.Op
                    Case EdgeOp.Add : net.AddEdge(best.FromIdx, best.ToIdx)
                    Case EdgeOp.Remove : net.RemoveEdge(best.FromIdx, best.ToIdx)
                End Select

                ' 增量刷新受影响节点的局部评分
                _localScores(best.ToIdx) = ScoreNodeWithParents(best.ToIdx, net.Nodes(best.ToIdx).Parents)
                currentBIC = _localScores.Sum

                ' 更新全局最优
                If currentBIC < bestBIC Then
                    bestBIC = currentBIC
                    bestNet = net.CloneStructure()
                End If

                ' 更新禁忌表（键格式统一为 "{op}_{from}_{to}"，修复原先增删前缀与更新键格式不一致的问题）
                Dim key As String = $"{CInt(best.Op)}_{best.FromIdx}_{best.ToIdx}"
                tabuList.Enqueue(key)
                If tabuList.Count > _params.TabuLength Then
                    tabuList.Dequeue()
                End If
            Next

            ' 恢复最优网络
            ' （简化处理：保留当前网络，因为 Tabu 搜索中当前解通常接近最优）
        End Sub


        ' ==================== BIC 评分 ====================

        ''' <summary>
        ''' 计算整个网络的 BIC 评分
        ''' BIC = Σ BIC_node
        ''' BIC_node = -2·LL_node + k_node·log(n)
        ''' 对于高斯BN：LL_node = -n/2·log(2πσ²) - 1/(2σ²)·RSS
        ''' </summary>
        ''' <remarks>
        ''' 粗粒度并行：以节点为单位执行一次网络级 <see cref="Parallel.For"/> fork/join。
        ''' 本方法仅供搜索起点/终点做完整评分校验使用；
        ''' 搜索热路径内部应采用基于马尔可夫局部性的 delta 增量评估，避免全网重复计算。
        ''' </remarks>
        Public Function ComputeNetworkBIC(net As Core.BayesianNetwork, Optional parallelOp As Boolean = False) As Double
            Dim totalBIC As Double() = New Double(net.Nodes.Count - 1) {}

            If parallelOp Then
                Call Parallel.For(
                    0, net.Nodes.Count,
                    body:=Sub(i)
                              totalBIC(i) = ScoreNodeWithParents(i, net.Nodes(i).Parents)
                          End Sub)
            Else
                For i = 0 To net.Nodes.Count - 1
                    totalBIC(i) = ScoreNodeWithParents(i, net.Nodes(i).Parents)
                Next
            End If

            Return totalBIC.Sum
        End Function

        ''' <summary>
        ''' 计算单个节点在其当前父母集合下的 BIC
        ''' </summary>
        ''' <remarks>
        ''' 委托至共享实现 <see cref="ScoreNodeWithParents"/>：
        ''' 回归设计矩阵奇异时退化为无父模型评分
        ''' （修复原先将均值数值误用作索引导致 Array.IndexOf 返回 -1 越界的缺陷）
        ''' </remarks>
        Private Function ComputeNodeBIC(net As Core.BayesianNetwork, nodeIdx As Integer, nS As Integer) As Double
            Return ScoreNodeWithParents(nodeIdx, net.Nodes(nodeIdx).Parents)
        End Function

        ' ==================== 数学工具 ====================

        ''' <summary>构建回归设计矩阵（自物化的基因表达列向量快速构建）</summary>
        Private Function BuildDesignMatrix(parents As List(Of Integer), nS As Integer) As Double(,)
            Dim nP As Integer = parents.Count
            Dim X As Double(,) = New Double(nS - 1, nP) {}  ' 第一列截距=1

            For j = 0 To nS - 1
                X(j, 0) = 1.0  ' 截距
                For p = 0 To nP - 1
                    X(j, p + 1) = _geneExpr(parents(p))(j)
                Next
            Next

            Return X
        End Function

        ''' <summary>
        ''' t 分布 P 值（双侧）的近似计算
        ''' 使用正态近似（大样本）或查表插值
        ''' </summary>
        Private Function TDistPValue(tStat As Double, df As Integer) As Double
            ' 对于大 df，t 分布近似正态分布
            ' 使用近似公式
            If df >= 30 Then
                ' 正态近似
                Dim z As Double = tStat
                Dim p As Double = 2.0 * (1.0 - NormalCDF(z))
                Return Math.Max(0, Math.Min(1, p))
            Else
                ' 小样本 t 分布近似
                ' 使用 Hills 逼近
                Dim x As Double = df / (df + tStat * tStat)
                Dim p As Double = IncompleteBeta(df / 2.0, 0.5, x)
                Return p
            End If
        End Function

        ''' <summary>标准正态 CDF 近似</summary>
        Private Function NormalCDF(z As Double) As Double
            ' Abramowitz and Stegun 近似
            Dim a1 As Double = 0.254829592
            Dim a2 As Double = -0.284496736
            Dim a3 As Double = 1.421413741
            Dim a4 As Double = -1.453152027
            Dim a5 As Double = 1.061405429
            Dim p As Double = 0.3275911

            Dim sign As Integer = If(z < 0, -1, 1)
            z = Math.Abs(z) / Math.Sqrt(2.0)
            Dim t As Double = 1.0 / (1.0 + p * z)
            Dim y As Double = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-z * z)
            Return 0.5 * (1.0 + sign * y)
        End Function

        ''' <summary>不完全 Beta 函数近似</summary>
        Private Function IncompleteBeta(a As Double, b As Double, x As Double) As Double
            If x <= 0 Then Return 0
            If x >= 1 Then Return 1

            ' 使用连分数展开近似
            Dim bt As Double = Math.Exp(
                GammaLn(a) + GammaLn(b) - GammaLn(a + b) +
                a * Math.Log(x) + b * Math.Log(1 - x))

            If x < (a + 1) / (a + b + 2) Then
                Return bt * BetaCF(a, b, x) / a
            Else
                Return 1.0 - bt * BetaCF(b, a, 1 - x) / b
            End If
        End Function

        ''' <summary>Beta 连分数展开</summary>
        Private Function BetaCF(a As Double, b As Double, x As Double) As Double
            Dim maxIter As Integer = 200
            Dim eps As Double = 0.0000000001

            Dim qab As Double = a + b
            Dim qap As Double = a + 1
            Dim qam As Double = a - 1
            Dim c As Double = 1
            Dim d As Double = 1 - qab * x / qap
            If Math.Abs(d) < 1.0E-30 Then d = 1.0E-30
            d = 1.0 / d
            Dim h As Double = d

            For m = 1 To maxIter
                Dim m2 As Integer = 2 * m
                Dim aa As Double = m * (b - m) * x / ((qam + m2) * (a + m2))
                d = 1 + aa * d
                If Math.Abs(d) < 1.0E-30 Then d = 1.0E-30
                c = 1 + aa / c
                If Math.Abs(c) < 1.0E-30 Then c = 1.0E-30
                d = 1.0 / d
                h *= d * c

                aa = -(a + m) * (qab + m) * x / ((a + m2) * (qap + m2))
                d = 1 + aa * d
                If Math.Abs(d) < 1.0E-30 Then d = 1.0E-30
                c = 1 + aa / c
                If Math.Abs(c) < 1.0E-30 Then c = 1.0E-30
                d = 1.0 / d
                Dim del As Double = d * c
                h *= del

                If Math.Abs(del - 1) < eps Then Exit For
            Next

            Return h
        End Function

        ''' <summary>Gamma 函数对数（Stirling 近似）</summary>
        Private Function GammaLn(x As Double) As Double
            Dim cof As Double() = {76.180091729471457, -86.505320329416776,
                                    24.014098240830911, -1.231739572450155,
                                    0.001208650973866179, -0.000005395239384953}
            Dim y As Double = x
            Dim tmp As Double = x + 5.5
            tmp -= (x + 0.5) * Math.Log(tmp)
            Dim ser As Double = 1.0000000001900149
            For j = 0 To 5
                y += 1
                ser += cof(j) / y
            Next
            Return -tmp + Math.Log(2.5066282746310007 * ser / x)
        End Function

    End Class

End Namespace

