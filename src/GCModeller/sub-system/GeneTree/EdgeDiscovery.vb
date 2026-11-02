Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

''' <summary>
''' 一条被“残差二阶段”流程检验过的候选调控边。
''' </summary>
Public Class DiscoveryEdge

    ''' <summary>靶基因（被调控的一端）</summary>
    Public Property target As String

    ''' <summary>候选调控者</summary>
    Public Property regulator As String

    ''' <summary>靶基因模型残差与候选表达之间的 pearson 相关系数</summary>
    Public Property correlation As Double

    ''' <summary>把这条边加入特征之后，验证集 R² 的增益</summary>
    Public Property deltaR2 As Double

    ''' <summary>加入这条边之前的验证集 R²</summary>
    Public Property r2Before As Double

    ''' <summary>加入这条边之后的验证集 R²</summary>
    Public Property r2After As Double

    ''' <summary>该特征在最终模型之中被用作分裂特征的节点数量</summary>
    Public Property splits As Integer

    ''' <summary>该特征的偏依赖灵敏度</summary>
    Public Property sensitivity As Double

    ''' <summary>
    ''' 是否被接受为新调控边：<c>accepted</c> / <c>rejected</c>
    ''' </summary>
    Public Property status As String

    ''' <summary>被拒绝的原因（接受时为空）</summary>
    Public Property reason As String

    Public ReadOnly Property isAccepted As Boolean
        Get
            Return String.Equals(status, "accepted", StringComparison.OrdinalIgnoreCase)
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"{regulator} -> {target}: r={correlation.ToString("G3")}, dR2={deltaR2.ToString("G3")}, {status}"
    End Function
End Class

''' <summary>
''' 结构发现阶段的汇总结果
''' </summary>
Public Class DiscoveryReport

    ''' <summary>本次训练使用的发现模式</summary>
    Public Property Mode As EdgeDiscoveryMode = EdgeDiscoveryMode.AracneOnly

    ''' <summary>调用方给定的候选基因集合</summary>
    Public Property Candidates As String() = New String() {}

    ''' <summary>实际执行了发现流程的靶基因数量</summary>
    Public Property TargetsExamined As Integer

    ''' <summary>实际参与相关性筛选的候选调控者数量（已剔除常量化、不在矩阵中的基因）</summary>
    Public Property CandidatesScreened As Integer

    ''' <summary>所有被检验过的候选边（含接受与拒绝）</summary>
    Public Property Edges As DiscoveryEdge() = New DiscoveryEdge() {}

    ''' <summary>被接受的新边数量</summary>
    ''' <returns></returns>
    Public ReadOnly Property Accepted As Integer
        Get
            Return Edges.SafeQuery.Count(Function(e) e.isAccepted)
        End Get
    End Property

    ''' <summary>被拒绝的候选数量</summary>
    ''' <returns></returns>
    Public ReadOnly Property Rejected As Integer
        Get
            Return Edges.Length - Accepted
        End Get
    End Property

    ''' <summary>被接受边的 ΔR² 均值</summary>
    ''' <returns></returns>
    Public ReadOnly Property MeanDeltaR2 As Double
        Get
            Dim accepted As DiscoveryEdge() = Edges.SafeQuery.Where(Function(e) e.isAccepted).ToArray

            If accepted.Length = 0 Then
                Return 0.0R
            Else
                Return accepted.Average(Function(e) e.deltaR2)
            End If
        End Get
    End Property

    ''' <summary>
    ''' 把发现结果写成 TSV 表格
    ''' </summary>
    ''' <param name="file">输出文件路径</param>
    Public Sub Write(filePath As String)
        Dim folder As String = Path.GetDirectoryName(filePath)

        If Not String.IsNullOrEmpty(folder) AndAlso Not Directory.Exists(folder) Then
            Call Directory.CreateDirectory(folder)
        End If

        Using writer As New StreamWriter(filePath, False, Encoding.UTF8)
            Call WriteTo(writer)
        End Using
    End Sub

    ''' <summary>
    ''' 把发现结果写到一个文本流（存档与外部文件共用同一份格式）
    ''' </summary>
    ''' <param name="writer">目标写入器</param>
    Public Sub WriteTo(writer As TextWriter)
        Call writer.WriteLine("# GeneTree residual two-stage edge discovery")
        Call writer.WriteLine($"# mode: {Mode}")
        Call writer.WriteLine($"# candidates: {Candidates.Length}, screened: {CandidatesScreened}, targets: {TargetsExamined}")
        Call writer.WriteLine($"# accepted: {Accepted}, rejected: {Rejected}, mean_delta_r2: {MeanDeltaR2.ToString("G4")}")
        Call writer.WriteLine("gene,regulator,source,status,correlation,delta_r2,r2_before,r2_after,splits,sensitivity,reason")

        For Each edge As DiscoveryEdge In Edges.SafeQuery
            Call writer.WriteLine(String.Join(",",
                csv(edge.target),
                csv(edge.regulator),
                "discovered",
                csv(edge.status),
                edge.correlation.ToString("G6"),
                edge.deltaR2.ToString("G6"),
                edge.r2Before.ToString("G6"),
                edge.r2After.ToString("G6"),
                CStr(edge.splits),
                edge.sensitivity.ToString("G6"),
                csv(If(edge.reason, ""))))
        Next
    End Sub

    Private Shared Function csv(text As String) As String
        If text Is Nothing Then
            Return ""
        ElseIf text.IndexOf(","c) >= 0 OrElse text.IndexOf(""""c) >= 0 Then
            Return """" & text.Replace("""", """""") & """"
        Else
            Return text
        End If
    End Function

    Public Overrides Function ToString() As String
        Return $"discovery={Mode}, candidates={Candidates.Length}, screened={CandidatesScreened}, " &
               $"targets={TargetsExamined}, accepted={Accepted}, rejected={Rejected}, " &
               $"mean dR2={MeanDeltaR2.ToString("G4")}"
    End Function
End Class

''' <summary>
''' 结构发现阶段在全部靶基因之间共享的上下文：筛选细胞与已经标准化的候选向量。
''' </summary>
''' <remarks>
''' 候选向量只在训练细胞上标准化一次，随后每个靶基因的残差都在同一批细胞上计算，
''' 因此相关系数的分母与尺度完全一致，可以直接比较。
''' </remarks>
Public Class DiscoveryContext

    ''' <summary>
    ''' 用于计算残差相关的细胞位置（相对训练细胞数组 <c>cells</c> 的下标）
    ''' </summary>
    Public Property Cells As Integer()

    ''' <summary>已经标准化的候选调控者向量</summary>
    Public Property Candidates As ResidualScreening.CandidateVector()

    ''' <summary>
    ''' 由配置与表达矩阵创建发现上下文
    ''' </summary>
    ''' <param name="opt">训练配置（必须已经开启 <see cref="EdgeDiscoveryMode.ResidualTwoStage"/>）</param>
    ''' <param name="rows">表达矩阵的行</param>
    ''' <param name="rowIndex">基因编号 → 行号</param>
    ''' <param name="cells">参与训练的细胞列下标</param>
    ''' <returns>没有任何可用候选时返回 <c>Nothing</c></returns>
    Public Shared Function Create(opt As GeneTreeConfig,
                                  rows As DataFrameRow(),
                                  rowIndex As Dictionary(Of String, Integer),
                                  cells As Integer()) As DiscoveryContext

        If opt Is Nothing OrElse Not opt.IsDiscoveryEnabled Then
            Return Nothing
        End If

        Dim size As Integer = If(opt.DiscoverySampleSize > 0,
                                 std.Min(opt.DiscoverySampleSize, cells.Length),
                                 cells.Length)
        Dim positions As Integer() = subsample(cells.Length, size)
        Dim absolute As Integer() = positions.Select(Function(p) cells(p)).ToArray
        Dim candidates As ResidualScreening.CandidateVector() =
            ResidualScreening.PrepareCandidates(opt.DiscoveryCandidates, rows, rowIndex, absolute)

        If candidates.Length = 0 Then
            Return Nothing
        End If

        Return New DiscoveryContext With {
            .Cells = positions,
            .Candidates = candidates
        }
    End Function

    ''' <summary>
    ''' 在 <c>[0, count)</c> 上等距抽取 <paramref name="size"/> 个位置（确定性，便于复现）
    ''' </summary>
    ''' <param name="count">总数</param>
    ''' <param name="size">需要的数量</param>
    ''' <returns></returns>
    Public Shared Function subsample(count As Integer, size As Integer) As Integer()
        If count <= 0 Then
            Return New Integer() {}
        End If
        If size <= 0 OrElse size >= count Then
            Return Enumerable.Range(0, count).ToArray
        End If

        Dim result As Integer() = New Integer(size - 1) {}
        Dim stride As Double = CDbl(count) / size

        For i As Integer = 0 To size - 1
            Dim pos As Integer = CInt(std.Floor(i * stride))

            If pos >= count Then
                pos = count - 1
            End If

            result(i) = pos
        Next

        Return result
    End Function

End Class

''' <summary>
''' 残差二阶段结构发现之中“候选筛选”这一阶段的工具。
''' 
''' 思路：第一阶段只用 ARACNE 的边拟合模型，得到靶基因的残差；残差代表了 ARACNE 拓扑没有解释掉的
''' 那部分方差。第二阶段在残差上寻找与之相关的候选基因——相当于在“控制已有调控者”之后做偏相关筛选
''' （线性近似），再把候选加入特征集重新拟合，用验证集 R² 的增益判断这条边是否真实存在。
''' </summary>
Public Module ResidualScreening

    ''' <summary>
    ''' 一个已经标准化（均值 0、标准差 1）的候选调控者向量
    ''' </summary>
    Public Class CandidateVector

        ''' <summary>基因编号</summary>
        Public Property gene As String

        ''' <summary>该基因在表达矩阵之中的行号</summary>
        Public Property index As Integer

        ''' <summary>采样细胞上的标准化表达值</summary>
        Public Property values As Double()

    End Class

    ''' <summary>
    ''' 准备候选调控者的标准化表达向量
    ''' </summary>
    ''' <param name="candidates">候选基因集合（调用方给定）</param>
    ''' <param name="rows">表达矩阵的行</param>
    ''' <param name="rowIndex">基因编号 → 行号</param>
    ''' <param name="cells">采样细胞的下标</param>
    ''' <returns>常量表达或不在矩阵中的候选会被剔除</returns>
    Public Function PrepareCandidates(candidates As IEnumerable(Of String),
                                      rows As DataFrameRow(),
                                      rowIndex As Dictionary(Of String, Integer),
                                      cells As Integer()) As CandidateVector()

        Dim result As New List(Of CandidateVector)
        Dim seen As New HashSet(Of Integer)

        For Each gene As String In candidates.SafeQuery
            Dim index As Integer = -1

            If Not rowIndex.TryGetValue(If(gene, "").Trim(), index) Then
                Continue For
            End If
            If Not seen.Add(index) Then
                Continue For
            End If

            Dim values As Double() = standardize(rows(index), cells, 1.0E-09R)

            If values Is Nothing Then
                Continue For
            End If

            Call result.Add(New CandidateVector With {
                .gene = rows(index).geneID,
                .index = index,
                .values = values
            })
        Next

        Return result.ToArray
    End Function

    ''' <summary>
    ''' 在采样细胞上标准化一行表达值（总体均值/样本标准差）
    ''' </summary>
    ''' <param name="row">表达矩阵行</param>
    ''' <param name="cells">细胞下标</param>
    ''' <param name="minSd">标准差下限，低于该值的行被视为常量并返回空</param>
    ''' <returns>长度与 <paramref name="cells"/> 一致的标准分；常量时返回 <c>Nothing</c></returns>
    Public Function standardize(row As DataFrameRow, cells As Integer(), minSd As Double) As Double()
        If row Is Nothing OrElse row.experiments Is Nothing Then
            Return Nothing
        End If

        Dim n As Integer = cells.Length
        Dim values As Double() = New Double(n - 1) {}
        Dim sum As Double = 0.0R

        For i As Integer = 0 To n - 1
            Dim v As Double = row.experiments(cells(i))

            If Double.IsNaN(v) OrElse Double.IsInfinity(v) Then
                v = 0.0R
            End If

            values(i) = v
            sum += v
        Next

        Dim mean As Double = sum / n
        Dim sse As Double = 0.0R

        For i As Integer = 0 To n - 1
            sse += (values(i) - mean) ^ 2
        Next

        Dim sd As Double = std.Sqrt(sse / std.Max(1, n - 1))

        If sd <= minSd Then
            Return Nothing
        End If

        For i As Integer = 0 To n - 1
            values(i) = (values(i) - mean) / sd
        Next

        Return values
    End Function

    ''' <summary>
    ''' 计算残差与所有候选的 pearson 相关，并按 |r| 降序返回前 <paramref name="topN"/> 个
    ''' </summary>
    ''' <param name="residualStd">已经标准化的残差向量</param>
    ''' <param name="candidates">候选向量（已标准化）</param>
    ''' <param name="exclude">需要排除的基因行号（靶基因自身与已有的调控者）</param>
    ''' <param name="minCorrelation">|r| 的下限</param>
    ''' <param name="topN">最多返回的候选数量</param>
    ''' <returns>按 |r| 降序排列的候选</returns>
    Public Function Screen(residualStd As Double(),
                           candidates As CandidateVector(),
                           exclude As HashSet(Of Integer),
                           minCorrelation As Double,
                           topN As Integer) As List(Of (candidate As CandidateVector, r As Double))

        Dim scored As New List(Of (candidate As CandidateVector, r As Double))
        Dim n As Integer = residualStd.Length

        For Each candidate As CandidateVector In candidates
            If exclude IsNot Nothing AndAlso exclude.Contains(candidate.index) Then
                Continue For
            End If

            Dim values As Double() = candidate.values
            Dim dot As Double = 0.0R

            For i As Integer = 0 To n - 1
                dot += residualStd(i) * values(i)
            Next

            ' 两边都已经标准化（均值 0、标准差 1），因此相关系数就是点积除以样本数
            Dim r As Double = dot / n

            If std.Abs(r) >= minCorrelation Then
                Call scored.Add((candidate, r))
            End If
        Next

        Return scored _
            .OrderByDescending(Function(x) std.Abs(x.r)) _
            .Take(std.Max(1, topN)) _
            .ToList()
    End Function

End Module
