
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

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

            Dim values As Double() = standardize(rows(index), cells, 0.000000001R)

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
