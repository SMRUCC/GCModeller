' 无空位延伸 (Ungapped Extension)
'
' 对通过 Hamming 初筛的命中,做基于 BLOSUM62 打分矩阵的无空位(ungapped)延伸:
' 从种子中心向左右双向延伸,累计替换得分,直到得分为负或到达序列边界,记录
' 最优得分窗口。这相当于 DIAMOND 中 AVX2 向量化无空位延伸的标量等价实现。
'
' 延伸结果(outScore、对齐坐标)将用于:
'   1) 进一步削减假阳性(得分低于阈值的命中丢弃);
'   2) 为 <see cref="HitScheduler"/> 提供排序依据,优先对高分目标做带状 SW。

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment

Namespace DIAMOND

    ''' <summary>
    ''' 无空位延伸的结果。
    ''' </summary>
    Public Structure UngappedHit
        Public ReadOnly QueryStart As Integer
        Public ReadOnly QueryEnd As Integer
        Public ReadOnly SubjectStart As Integer
        Public ReadOnly SubjectEnd As Integer
        Public ReadOnly Score As Integer

        Sub New(qStart As Integer, qEnd As Integer, sStart As Integer, sEnd As Integer, score As Integer)
            Me.QueryStart = qStart
            Me.QueryEnd = qEnd
            Me.SubjectStart = sStart
            Me.SubjectEnd = sEnd
            Me.Score = score
        End Sub
    End Structure

    Public Class UngappedExtension

        ''' <summary>视为延伸终点的单个位置最低得分(低于此值即截断)。</summary>
        Public ReadOnly Dropoff As Integer

        Private ReadOnly blosum As Blosum

        Sub New(Optional dropoff As Integer = -8, Optional blosum As Blosum = Nothing)
            Me.Dropoff = dropoff
            Me.blosum = If(blosum, Blosum.FromInnerBlosum62)
        End Sub

        ''' <summary>
        ''' 从种子命中 (qPos, sPos) 向两侧做无空位延伸,返回最优得分窗口。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Extend(query As String, qPos As Integer, subject As String, sPos As Integer) As UngappedHit
            ' 种子中心(取种子窗口中点附近)作为延伸起点
            Dim n As Integer = Math.Min(query.Length - qPos, subject.Length - sPos)

            If n <= 0 Then
                Return New UngappedHit(qPos, qPos, sPos, sPos, 0)
            End If

            ' 向前(左)延伸:从 0 累积到最优
            Dim bestScore As Integer = 0
            Dim bestLeft As Integer = 0
            Dim running As Integer = 0

            For k As Integer = 0 To n - 1
                running += blosum.GetDistance(query(qPos + k), subject(sPos + k))

                If running > bestScore Then
                    bestScore = running
                    bestLeft = k
                End If

                If running < Dropoff Then
                    running = 0
                End If
            Next

            ' 向后(右)延伸:从 bestLeft+1 继续累积到窗口右端
            running = 0
            Dim bestRight As Integer = bestLeft

            For k As Integer = bestLeft To n - 1
                running += blosum.GetDistance(query(qPos + k), subject(sPos + k))

                If running > bestScore Then
                    bestScore = running
                    bestRight = k
                End If

                If running < Dropoff Then
                    running = 0
                End If
            Next

            Return New UngappedHit(qPos, qPos + bestRight, sPos, sPos + bestRight, bestScore)
        End Function
    End Class
End Namespace
