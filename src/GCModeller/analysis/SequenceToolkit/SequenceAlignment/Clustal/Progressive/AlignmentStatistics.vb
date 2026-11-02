Imports System.Text

Namespace Clustal

    ''' <summary>
    ''' 多序列比对的统计信息：SP 得分、逐列保守性、每条序列的差异计数。
    ''' </summary>
    ''' <remarks>
    ''' SP(sum-of-pairs)得分的计算利用了"一列的得分只与列内字符的多重集有关"这一性质，
    ''' 因此可以先统计字符频度再按字符对聚合，复杂度从 O(n^2 * L) 降到 O(n * L + L * A^2)。
    ''' </remarks>
    Public Class AlignmentStatistics

        ''' <summary>
        ''' SP 得分(sum-of-pairs)
        ''' </summary>
        Public ReadOnly Property SpScore As Double
        ''' <summary>
        ''' 每条序列相对于一致序列(consensus)的差异计数
        ''' </summary>
        Public ReadOnly Property Edits As Integer()
        ''' <summary>
        ''' 逐列的保守性标记：<c>*</c> 完全保守、<c>-</c> 高比例空位列、空格表示普通列
        ''' </summary>
        Public ReadOnly Property Conservation As String
        ''' <summary>
        ''' 对齐之后的列数
        ''' </summary>
        Public ReadOnly Property ColumnCount As Integer
        ''' <summary>
        ''' 完全由空位组成的列数
        ''' </summary>
        Public ReadOnly Property GapOnlyColumns As Integer
        ''' <summary>
        ''' 完全保守的列所占的比例
        ''' </summary>
        Public ReadOnly Property Identity As Double
        ''' <summary>
        ''' 一致序列(每列出现次数最多的残基，空位列记为 <c>-</c>)
        ''' </summary>
        Public ReadOnly Property Consensus As String

        Private Sub New(sp As Double, edits As Integer(), conservation As String,
                        columns As Integer, gapOnly As Integer, identity As Double, consensus As String)

            Me.SpScore = sp
            Me.Edits = edits
            Me.Conservation = conservation
            Me.ColumnCount = columns
            Me.GapOnlyColumns = gapOnly
            Me.Identity = identity
            Me.Consensus = consensus
        End Sub

        ''' <summary>
        ''' 计算比对统计信息
        ''' </summary>
        Public Shared Function Compute(rows As String(), matrix As ScoringMatrix,
                                       Optional gapPenalty As Double = 1.0R) As AlignmentStatistics

            Dim n As Integer = If(rows Is Nothing, 0, rows.Length)

            If n = 0 OrElse rows(0).Length = 0 Then
                Return New AlignmentStatistics(0, New Integer() {}, "", 0, 0, 0, "")
            End If

            Dim A As Integer = matrix.Size
            Dim alphabet As SequenceAlphabet = matrix.Alphabet
            Dim columns As Integer = rows(0).Length
            Dim counts As Integer() = New Integer(columns * A - 1) {}
            Dim gapCounts As Integer() = New Integer(columns - 1) {}
            Dim gapChar As Char = "-"c

            For r As Integer = 0 To n - 1
                Dim row As String = rows(r)

                For c As Integer = 0 To columns - 1
                    Dim ch As Char = If(c < row.Length, row(c), gapChar)

                    If ch = gapChar OrElse ch = "."c Then
                        gapCounts(c) += 1
                    Else
                        counts(c * A + alphabet.CodeOf(ch)) += 1
                    End If
                Next
            Next

            Dim sp As Double = 0
            Dim conservation As New StringBuilder(columns)
            Dim consensus As New StringBuilder(columns)
            Dim gapOnly As Integer = 0
            Dim identityColumns As Integer = 0

            For c As Integer = 0 To columns - 1
                Dim offset As Integer = c * A
                Dim gaps As Integer = gapCounts(c)

                For x As Integer = 0 To A - 1
                    Dim cx As Integer = counts(offset + x)

                    If cx = 0 Then
                        Continue For
                    End If

                    ' 同字符配对
                    If cx > 1 Then
                        sp += CDbl(cx) * (cx - 1) / 2.0R * matrix.Scores(x * A + x)
                    End If

                    ' 与其它字符配对
                    For y As Integer = x + 1 To A - 1
                        Dim cy As Integer = counts(offset + y)

                        If cy > 0 Then
                            sp += CDbl(cx) * cy * matrix.Scores(x * A + y)
                        End If
                    Next

                    ' 与空位配对
                    If gaps > 0 Then
                        sp -= CDbl(cx) * gaps * gapPenalty
                    End If
                Next

                Dim best As Integer = 0
                Dim bestCount As Integer = 0
                Dim distinct As Integer = 0

                For x As Integer = 0 To A - 1
                    Dim cx As Integer = counts(offset + x)

                    If cx > 0 Then
                        distinct += 1

                        If cx > bestCount Then
                            bestCount = cx
                            best = x
                        End If
                    End If
                Next

                If gaps = n Then
                    Call conservation.Append("-"c)
                    Call consensus.Append(gapChar)
                    gapOnly += 1
                ElseIf distinct = 1 AndAlso gaps = 0 Then
                    Call conservation.Append("*"c)
                    Call consensus.Append(alphabet.Letters(best))
                    identityColumns += 1
                Else
                    Call conservation.Append(" "c)
                    Call consensus.Append(If(bestCount > 0, alphabet.Letters(best), gapChar))
                End If
            Next

            ' 每条序列相对一致序列的差异计数
            Dim edits As Integer() = New Integer(n - 1) {}
            Dim consensusText As String = consensus.ToString()

            For r As Integer = 0 To n - 1
                Dim row As String = rows(r)
                Dim diff As Integer = 0

                For c As Integer = 0 To columns - 1
                    Dim ch As Char = If(c < row.Length, row(c), gapChar)

                    If ch <> consensusText(c) Then
                        diff += 1
                    End If
                Next

                edits(r) = diff
            Next

            Dim identity As Double = If(columns = 0, 0, CDbl(identityColumns) / columns)

            Return New AlignmentStatistics(sp, edits, conservation.ToString(), columns, gapOnly, identity, consensusText)
        End Function

        Public Overrides Function ToString() As String
            Return $"columns={ColumnCount}, SP={Math.Round(SpScore, 3)}, identity={Math.Round(Identity * 100, 2)}%"
        End Function
    End Class
End Namespace
