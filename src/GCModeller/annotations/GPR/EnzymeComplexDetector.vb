Imports SMRUCC.genomics.ComponentModel.Annotation

''' <summary>
''' 潜在多亚基酶复合体检测。
''' 
''' 判定条件（三条必须同时满足）：
''' 
''' 1. 位于同一条链上；
''' 2. 相邻成员之间的物理距离不超过 <see cref="GPRParameters.ComplexMaxDistance"/>；
''' 3. 与种子基因共享至少一个"完全相同"的 EC 编号（多亚基酶的各个亚基通常携带相同的 EC）。
''' </summary>
Public Class EnzymeComplexDetector

    ''' <summary>
    ''' 检测基因组中的潜在酶复合体
    ''' </summary>
    ''' <param name="genes">基因组基因集合（无需预先排序）</param>
    ''' <param name="opt">算法参数</param>
    Public Function DetectComplexes(genes As GeneTable(), opt As GPRParameters) As List(Of List(Of GeneTable))
        Dim complexes As New List(Of List(Of GeneTable))

        If genes Is Nothing OrElse genes.Length < 2 Then Return complexes
        If opt Is Nothing Then opt = New GPRParameters

        Dim order As GeneTable() = genes _
            .Where(Function(g) g IsNot Nothing) _
            .OrderBy(Function(g) g.left) _
            .ToArray

        Dim visited As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To order.Length - 2
            Dim seed As GeneTable = order(i)
            If visited.Contains(seed.locus_id) Then Continue For

            Dim seedEc As String() = EcNumbers(seed)
            If seedEc.Length = 0 Then Continue For

            Dim members As New List(Of GeneTable) From {seed}

            ' 修复：距离基准改为"上一个已经入簇成员的 right"，
            ' 原实现固定使用种子基因的 right，导致复合体边界判定错误。
            Dim lastRight As Integer = seed.right

            For j As Integer = i + 1 To order.Length - 1
                Dim candidate As GeneTable = order(j)

                Dim distance As Integer = Math.Max(0, candidate.left - lastRight)
                If distance > opt.ComplexMaxDistance Then Exit For

                If visited.Contains(candidate.locus_id) Then Continue For
                If Not String.Equals(candidate.strand, seed.strand, StringComparison.Ordinal) Then Continue For

                Dim candidateEc As String() = EcNumbers(candidate)
                If candidateEc.Length = 0 Then Continue For

                ' 修复：以"完全相同 EC 编号"作为亚基相关性判据，
                ' 替换原先仅比较 EC 首段（"1"/"2"/"3"）的粗糙做法，并去掉不可达的 Return True。
                If seedEc.Intersect(candidateEc, StringComparer.OrdinalIgnoreCase).Any() Then
                    members.Add(candidate)
                    visited.Add(candidate.locus_id)
                    lastRight = Math.Max(lastRight, candidate.right)
                End If
            Next

            If members.Count >= Math.Max(2, opt.MinComplexGenes) Then
                visited.Add(seed.locus_id)
                complexes.Add(members)
            End If
        Next

        Return complexes
    End Function

    Private Shared Function EcNumbers(gene As GeneTable) As String()
        If gene Is Nothing OrElse gene.EC_Number Is Nothing Then Return New String() {}

        Return gene.EC_Number _
            .Where(Function(ec) Not String.IsNullOrEmpty(ec)) _
            .Select(Function(ec) ec.Trim) _
            .Distinct(StringComparer.OrdinalIgnoreCase) _
            .ToArray
    End Function

End Class
