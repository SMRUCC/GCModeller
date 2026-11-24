''' <summary>
''' 多公共数据集合并表达矩阵的批次预处理
''' </summary>
''' <remarks>
''' 多个公共数据集即使各自做过归一化，合并后仍然存在显著的技术批次效应：
''' 同一批次内的样本会因为平台 / 建库 / 归一化方法的差异而呈现出额外的相关性，
''' 直接计算共表达会引入大量假阳性边。
'''
''' 本模块执行两步处理：
''' 1. 数据集内标准化（z-score 或仅中心化），使每个基因在各批次内具有可比的分布；
''' 2. 可选的批次中心化，进一步对齐批次间的加性差异。
'''
''' 处理按「基因行」独立进行，不改变基因之间的相对关系，也不原地修改输入矩阵。
''' </remarks>
Public Module BatchNormalizer

    ''' <summary>
    ''' 未在任何批次中出现的样本被归入的虚拟批次名称
    ''' </summary>
    Private Const UNASSIGNED As String = "__unassigned__"

    ''' <summary>
    ''' 执行批次标准化
    ''' </summary>
    ''' <param name="samples">基因 × 样本的原始（或已归一化）表达矩阵</param>
    ''' <param name="batches">数据集名称 → 该数据集的 sampleID 列表；为 Nothing 时按全局单批次处理</param>
    ''' <param name="centerBatch">
    ''' True → 批次内 z-score（减均值后除以批次内标准差，同时消除加性与尺度差异）；
    ''' False → 仅批次内中心化（只减均值，保留批次间的表达幅度差异）。
    ''' </param>
    ''' <returns>标准化后的新矩阵，行顺序与输入一致</returns>
    Public Function Normalize(samples As Matrix,
                              batches As Dictionary(Of String, String()),
                              Optional centerBatch As Boolean = True) As Matrix

        If samples Is Nothing OrElse samples.expression Is Nothing Then
            Throw New ArgumentNullException(NameOf(samples), "表达矩阵不能为空")
        End If

        Dim sampleID As String() = samples.sampleID
        Dim groups As Integer() = ResolveBatchGroups(sampleID, batches)
        Dim rows As DataFrameRow() = samples.expression
        Dim out(rows.Length - 1) As DataFrameRow

        Call Parallel.For(0, rows.Length,
            Sub(i)
                out(i) = New DataFrameRow With {
                    .geneID = rows(i).geneID,
                    .experiments = NormalizeVector(rows(i).experiments, groups, centerBatch)
                }
            End Sub)

        Return New Matrix With {
            .sampleID = sampleID,
            .tag = $"batch-normalized({samples.tag})",
            .expression = out
        }
    End Function

    ''' <summary>
    ''' 全局 z-score 标准化（不区分批次）
    ''' </summary>
    ''' <param name="samples">基因 × 样本表达矩阵</param>
    ''' <returns>标准化后的新矩阵</returns>
    Public Function ZScore(samples As Matrix) As Matrix
        Return Normalize(samples, Nothing, centerBatch:=True)
    End Function

    ''' <summary>
    ''' 将每个样本映射为批次下标
    ''' </summary>
    Private Function ResolveBatchGroups(sampleID As String(), batches As Dictionary(Of String, String())) As Integer()
        Dim n As Integer = If(sampleID Is Nothing, 0, sampleID.Length)
        Dim groups(n - 1) As Integer

        If batches Is Nothing OrElse batches.Count = 0 Then
            ' 单批次：全部样本归入同一组
            For i As Integer = 0 To n - 1
                groups(i) = 0
            Next

            Return groups
        End If

        Dim lookup As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        Dim batchNames As New List(Of String)

        For Each batch In batches
            If batch.Value Is Nothing Then Continue For
            If Not batchNames.Contains(batch.Key) Then
                batchNames.Add(batch.Key)
            End If

            For Each id As String In batch.Value
                lookup(id) = batchNames.IndexOf(batch.Key)
            Next
        Next

        ' 未分配的样本归入独立的虚拟批次，避免污染已声明的批次统计
        Dim unassigned As Integer = batchNames.Count

        For i As Integer = 0 To n - 1
            Dim id As String = If(sampleID Is Nothing, Nothing, sampleID(i))

            If id Is Nothing OrElse Not lookup.ContainsKey(id) Then
                groups(i) = unassigned
            Else
                groups(i) = lookup(id)
            End If
        Next

        Return groups
    End Function

    ''' <summary>
    ''' 对单个基因的表达向量做批次内标准化
    ''' </summary>
    Private Function NormalizeVector(x As Double(), groups As Integer(), centerBatch As Boolean) As Double()
        Dim n As Integer = x.Length
        Dim result(n - 1) As Double
        Dim nGroups As Integer = 0

        For Each g As Integer In groups
            If g + 1 > nGroups Then nGroups = g + 1
        Next

        ' 每个批次的均值与标准差
        Dim means(nGroups - 1) As Double
        Dim sds(nGroups - 1) As Double
        Dim counts(nGroups - 1) As Integer

        For i As Integer = 0 To n - 1
            If Double.IsNaN(x(i)) Then Continue For

            Dim g As Integer = groups(i)
            means(g) += x(i)
            counts(g) += 1
        Next

        For g As Integer = 0 To nGroups - 1
            If counts(g) > 0 Then means(g) /= counts(g)
        Next

        If centerBatch Then
            Dim ss(nGroups - 1) As Double

            For i As Integer = 0 To n - 1
                If Double.IsNaN(x(i)) Then Continue For

                Dim g As Integer = groups(i)
                Dim d As Double = x(i) - means(g)
                ss(g) += d * d
            Next

            For g As Integer = 0 To nGroups - 1
                If counts(g) > 1 Then
                    sds(g) = System.Math.Sqrt(ss(g) / (counts(g) - 1))
                End If
            Next
        End If

        ' 写回：缺失值用批次均值填补（标准化后即 0）
        For i As Integer = 0 To n - 1
            Dim g As Integer = groups(i)

            If Double.IsNaN(x(i)) Then
                result(i) = 0.0
                Continue For
            End If

            If centerBatch Then
                If sds(g) > 0 Then
                    result(i) = (x(i) - means(g)) / sds(g)
                Else
                    result(i) = 0.0
                End If
            Else
                result(i) = x(i) - means(g)
            End If
        Next

        Return result
    End Function
End Module

