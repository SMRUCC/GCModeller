Imports System.Text

Namespace Clustal

    ''' <summary>
    ''' 按引导树自底向上做渐进式合并：每次取两棵子树当前的比对结果建立 profile，
    ''' 做 profile-profile 比对之后把新产生的列映射应用到两边的每一行。
    ''' </summary>
    ''' <remarks>
    ''' "once a gap, always a gap" 由 profile 结构自然保证：已经合并进簇的空位列会以
    ''' 列频度的形式进入 profile 的发射/转移统计，后续合并不会把它撤销。
    ''' </remarks>
    Public Class ProgressiveAligner

        Private Class Cluster
            Public Property Rows As String()
            Public Property Ids As Integer()
        End Class

        Private ReadOnly matrix As ScoringMatrix
        Private ReadOnly options As ClustalOmegaOptions
        Private ReadOnly aligner As PairHMMAligner
        Private ReadOnly seqs As String()

        ''' <summary>
        ''' 实际执行的 profile-profile 比对次数
        ''' </summary>
        Public Property MergeCount As Integer
        ''' <summary>
        ''' 参与运算的 DP 单元总数
        ''' </summary>
        Public Property Cells As Long
        ''' <summary>
        ''' 因为内存上限而降级为 Viterbi 解码的次数
        ''' </summary>
        Public Property Fallbacks As Integer

        Sub New(seqs As String(), matrix As ScoringMatrix, options As ClustalOmegaOptions)
            Me.seqs = seqs
            Me.matrix = matrix
            Me.options = options
            Me.aligner = New PairHMMAligner(options)
        End Sub

        ''' <summary>
        ''' 执行渐进合并，返回按输入顺序排列的比对结果行
        ''' </summary>
        Public Function Run(tree As GuideTree) As String()
            Dim n As Integer = seqs.Length

            If n = 0 Then
                Return New String() {}
            End If

            If n = 1 Then
                Return New String() {seqs(0)}
            End If

            If tree Is Nothing OrElse tree.Root Is Nothing Then
                ' 没有引导树时退化为"全部同一行长度"的朴素结果
                Return seqs.ToArray
            End If

            Dim clusters As New Dictionary(Of GuideTreeNode, Cluster)()
            Dim merges As Integer = 0
            Dim cells As Long = 0
            Dim fallbacks As Integer = 0

            For Each node As GuideTreeNode In tree.PostOrder()
                If node.IsLeaf Then
                    clusters(node) = New Cluster With {
                        .Rows = New String() {seqs(node.Index)},
                        .Ids = New Integer() {node.Index}
                    }
                Else
                    Dim left As Cluster = clusters(node.Left)
                    Dim right As Cluster = clusters(node.Right)
                    Dim stats As PairHMMAligner.PairHMMStatistics = Nothing
                    Dim rows As String() = AlignProfiles(left.Rows, right.Rows, matrix, options, aligner, stats)
                    Dim ids As Integer() = New Integer(left.Ids.Length + right.Ids.Length - 1) {}

                    Call Array.Copy(left.Ids, 0, ids, 0, left.Ids.Length)
                    Call Array.Copy(right.Ids, 0, ids, left.Ids.Length, right.Ids.Length)

                    clusters(node) = New Cluster With {.Rows = rows, .Ids = ids}

                    ' 合并完成之后立刻释放子树的结果，避免大规模输入时内存翻倍
                    Call clusters.Remove(node.Left)
                    Call clusters.Remove(node.Right)

                    merges += 1

                    If stats IsNot Nothing Then
                        cells += stats.Cells

                        If stats.FallbackToViterbi Then
                            fallbacks += 1
                        End If
                    End If
                End If
            Next

            Me.MergeCount = merges
            Me.Cells = cells
            Me.Fallbacks = fallbacks

            Dim root As Cluster = clusters(tree.Root)

            If root Is Nothing Then
                Return seqs.ToArray
            End If

            Dim result As String() = New String(n - 1) {}

            For i As Integer = 0 To root.Ids.Length - 1
                result(root.Ids(i)) = root.Rows(i)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 对两组已经对齐的行做 profile-profile 比对，返回拼接之后的行
        ''' </summary>
        Public Shared Function AlignProfiles(a As String(), b As String(),
                                             matrix As ScoringMatrix, options As ClustalOmegaOptions,
                                             aligner As PairHMMAligner,
                                             Optional ByRef stats As PairHMMAligner.PairHMMStatistics = Nothing) As String()

            Dim pa As ProfileHMM = ProfileHMM.FromAlignment(a, matrix, options)
            Dim pb As ProfileHMM = ProfileHMM.FromAlignment(b, matrix, options)
            Dim pairs As (Integer, Integer)() = aligner.Align(pa, pb, stats)

            If pairs.Length = 0 Then
                Dim empty As String() = New String(a.Length + b.Length - 1) {}

                For i As Integer = 0 To empty.Length - 1
                    empty(i) = ""
                Next

                Return empty
            End If

            Dim rowsA As String() = BuildRows(a, pairs, True)
            Dim rowsB As String() = BuildRows(b, pairs, False)
            Dim result As String() = New String(rowsA.Length + rowsB.Length - 1) {}

            Call Array.Copy(rowsA, 0, result, 0, rowsA.Length)
            Call Array.Copy(rowsB, 0, result, rowsA.Length, rowsB.Length)

            Return result
        End Function

        ''' <summary>
        ''' 按列映射重建一组行
        ''' </summary>
        ''' <param name="rows">原始行</param>
        ''' <param name="pairs">(A 的列, B 的列) 序列，-1 表示该侧为空位</param>
        ''' <param name="first">是否取映射中的 A 侧</param>
        Public Shared Function BuildRows(rows As String(), pairs As (Integer, Integer)(),
                                         first As Boolean) As String()

            Dim result As String() = New String(rows.Length - 1) {}

            For r As Integer = 0 To rows.Length - 1
                Dim row As String = rows(r)
                Dim sb As New StringBuilder(pairs.Length)

                For t As Integer = 0 To pairs.Length - 1
                    Dim col As Integer = If(first, pairs(t).Item1, pairs(t).Item2)

                    If col < 0 OrElse col >= row.Length Then
                        Call sb.Append("-"c)
                    Else
                        Call sb.Append(row(col))
                    End If
                Next

                result(r) = sb.ToString()
            Next

            Return result
        End Function
    End Class
End Namespace
