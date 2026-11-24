Imports System.Text

Namespace Clustal

    ''' <summary>
    ''' 迭代精炼：按引导树切分出两个子群，去掉各自的全空位列之后重新做 profile-profile 比对，
    ''' 只有当 SP 得分不劣化时才接受这一轮的结果(与 clustalo 的 <c>--iterations</c> 同向)。
    ''' </summary>
    ''' <remarks>
    ''' 精炼的代价与渐进合并同阶，因此只在序列条数不超过 <see cref="MaxRefineSequences"/>
    ''' 时执行；每轮最多处理 <see cref="MaxSplitsPerRound"/> 个切分点，避免在大规模输入上失控。
    ''' </remarks>
    Public Class Refinement

        ''' <summary>
        ''' 允许执行精炼的最大序列条数
        ''' </summary>
        Public Const MaxRefineSequences As Integer = 2000

        ''' <summary>
        ''' 每轮精炼最多尝试的切分点数
        ''' </summary>
        Public Const MaxSplitsPerRound As Integer = 32

        Private ReadOnly matrix As ScoringMatrix
        Private ReadOnly options As ClustalOmegaOptions
        Private ReadOnly aligner As PairHMMAligner

        ''' <summary>
        ''' 实际被接受的精炼次数
        ''' </summary>
        Public Property Accepted As Integer
        ''' <summary>
        ''' 实际尝试的切分数
        ''' </summary>
        Public Property Attempts As Integer
        ''' <summary>
        ''' 精炼之前/之后的 SP 得分
        ''' </summary>
        Public Property ScoreBefore As Double
        Public Property ScoreAfter As Double

        Sub New(matrix As ScoringMatrix, options As ClustalOmegaOptions)
            Me.matrix = matrix
            Me.options = options
            Me.aligner = New PairHMMAligner(options)
        End Sub

        ''' <summary>
        ''' 执行迭代精炼
        ''' </summary>
        ''' <param name="rows">当前的比对结果(按输入顺序)</param>
        ''' <param name="tree">引导树</param>
        ''' <param name="iterations">精炼轮数，小于等于 0 时原样返回</param>
        Public Function Run(rows As String(), tree As GuideTree, iterations As Integer) As String()
            If iterations <= 0 OrElse rows Is Nothing OrElse rows.Length < 3 Then
                Return rows
            End If

            If rows.Length > MaxRefineSequences Then
                Return rows
            End If

            If tree Is Nothing OrElse tree.Root Is Nothing Then
                Return rows
            End If

            Dim current As String() = rows
            Dim score As Double = AlignmentStatistics.Compute(current, matrix).SpScore

            Me.ScoreBefore = score
            Me.ScoreAfter = score

            Dim clades As List(Of Integer()) = CollectClades(tree)

            If clades.Count = 0 Then
                Return current
            End If

            For round As Integer = 1 To iterations
                Dim improved As Boolean = False

                For Each clade As Integer() In clades
                    If clade.Length = 0 OrElse clade.Length >= rows.Length Then
                        Continue For
                    End If

                    Dim candidate As String() = TryRefine(current, clade)

                    If candidate Is Nothing Then
                        Continue For
                    End If

                    Me.Attempts += 1

                    Dim candidateScore As Double = AlignmentStatistics.Compute(candidate, matrix).SpScore

                    If candidateScore >= score Then
                        current = candidate
                        score = candidateScore
                        improved = True
                        Me.Accepted += 1
                    End If
                Next

                Me.ScoreAfter = score

                If Not improved Then
                    Exit For
                End If
            Next

            Return current
        End Function

        ''' <summary>
        ''' 收集引导树上的内部节点所代表的叶子集合(用于切分)
        ''' </summary>
        Private Function CollectClades(tree As GuideTree) As List(Of Integer())
            Dim result As New List(Of Integer())()

            For Each node As GuideTreeNode In tree.PostOrder()
                If node.IsLeaf Then
                    Continue For
                End If

                Dim leaves As Integer() = GuideTree.LeavesOf(node)

                If leaves.Length > 1 AndAlso leaves.Length < tree.LeafCount Then
                    Call result.Add(leaves)
                End If
            Next

            If result.Count > MaxSplitsPerRound Then
                ' 确定性抽样：按索引等距取，保证结果可复现
                Dim sampled As New List(Of Integer())(MaxSplitsPerRound)

                For t As Integer = 0 To MaxSplitsPerRound - 1
                    Dim index As Integer = CInt(Math.Floor(CDbl(t) * result.Count / MaxSplitsPerRound))

                    If index < result.Count Then
                        Call sampled.Add(result(index))
                    End If
                Next

                Return sampled
            End If

            Return result
        End Function

        ''' <summary>
        ''' 尝试用一个切分点产生新的比对结果
        ''' </summary>
        Private Function TryRefine(current As String(), clade As Integer()) As String()
            Dim marker As Boolean() = New Boolean(current.Length - 1) {}

            For Each i As Integer In clade
                If i >= 0 AndAlso i < marker.Length Then
                    marker(i) = True
                End If
            Next

            Dim left As New List(Of Integer)(clade.Length)
            Dim right As New List(Of Integer)(current.Length - clade.Length)

            For i As Integer = 0 To current.Length - 1
                If marker(i) Then
                    Call left.Add(i)
                Else
                    Call right.Add(i)
                End If
            Next

            If left.Count = 0 OrElse right.Count = 0 Then
                Return Nothing
            End If

            Dim rowsA As String() = StripAllGapColumns(Pick(current, left))
            Dim rowsB As String() = StripAllGapColumns(Pick(current, right))

            Dim merged As String() = ProgressiveAligner.AlignProfiles(rowsA, rowsB, matrix, options, aligner)

            If merged.Length <> current.Length Then
                Return Nothing
            End If

            Dim candidate As String() = New String(current.Length - 1) {}
            Dim pos As Integer = 0

            For Each i As Integer In left
                candidate(i) = merged(pos)
                pos += 1
            Next

            For Each i As Integer In right
                candidate(i) = merged(pos)
                pos += 1
            Next

            Return candidate
        End Function

        Private Shared Function Pick(rows As String(), index As List(Of Integer)) As String()
            Dim result As String() = New String(index.Count - 1) {}

            For i As Integer = 0 To index.Count - 1
                result(i) = rows(index(i))
            Next

            Return result
        End Function

        ''' <summary>
        ''' 去掉全部由空位组成的列
        ''' </summary>
        Public Shared Function StripAllGapColumns(rows As String()) As String()
            If rows Is Nothing OrElse rows.Length = 0 OrElse rows(0).Length = 0 Then
                Return rows
            End If

            Dim columns As Integer = rows(0).Length
            Dim keep As New List(Of Integer)(columns)

            For c As Integer = 0 To columns - 1
                Dim hasResidue As Boolean = False

                For r As Integer = 0 To rows.Length - 1
                    Dim row As String = rows(r)

                    If c < row.Length AndAlso row(c) <> "-"c AndAlso row(c) <> "."c Then
                        hasResidue = True
                        Exit For
                    End If
                Next

                If hasResidue Then
                    Call keep.Add(c)
                End If
            Next

            If keep.Count = columns Then
                Return rows
            End If

            Dim stripped As String() = New String(rows.Length - 1) {}

            For r As Integer = 0 To rows.Length - 1
                Dim row As String = rows(r)
                Dim sb As New StringBuilder(keep.Count)

                For Each c As Integer In keep
                    Call sb.Append(If(c < row.Length, row(c), "-"c))
                Next

                stripped(r) = sb.ToString()
            Next

            Return stripped
        End Function
    End Class
End Namespace
