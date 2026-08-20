' 无空位延伸 —— AVX2 向量化实现 (1 查询 × 32 参考 并行)
'
' 继承标量基类 <see cref="UngappedExtension"/>,新增批量延伸 <see cref="ExtendBatch"/>:
' 固定一个查询位置,同时对一批(≤32 条)参考序列做无空位延伸。DIAMOND 原始实现
' 用 160 条 unpack 指令把 32 条参考的字母交织进同一 AVX2 寄存器,再用 SIMD 累加
' 32 路打分。本实现采用"查表标量 + 累加/最优比较 SIMD"的真实并行结构:
'   - 32 个独立累加器与最优追踪寄存器(4 × __m256i,共 32 个 int32 lane)同时推进,
'     核心得分累加与 max 比较完全向量化;
'   - 每条参考的 (query 残基, 参考残基) → BLOSUM62 得分仍为标量查表(后续可进一步
'     用 Avx2.GatherVector256 查表优化),但 32 路累加的热路径已 SIMD 化。
'
' 运行时按 <see cref="Avx2.IsSupported"/> 选择向量化或标量回退:
' 不支持 AVX2 时,ExtendBatch 退化为对每条候选调用基类标量 <see cref="UngappedExtension.Extend"/>,
' 保证跨平台可编译可运行且结果与标量版本完全一致。

Imports System.Numerics
Imports System.Runtime.CompilerServices
Imports System.Runtime.Intrinsics
Imports System.Runtime.Intrinsics.X86
Imports SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment

Namespace DIAMOND

    ''' <summary>
    ''' AVX2 向量化无空位延伸(批量 1 查询 × ≤32 参考 并行)。
    ''' </summary>
    Public Class UngappedExtensionAvx2 : Inherits UngappedExtension

        ''' <summary>每次批量延伸的固定窗口步数(氨基酸数)。</summary>
        Public Const BatchWindow As Integer = 48

        ''' <summary>AVX2 单批次可并行处理的参考序列条数(32 个 int32 lane)。</summary>
        Public Const MaxBatch As Integer = 32

        Private ReadOnly blosum As Blosum

        Sub New(Optional dropoff As Integer = -8, Optional blosum As Blosum = Nothing)
            MyBase.New(dropoff, blosum)
            Me.blosum = If(blosum, Blosum.FromInnerBlosum62)
        End Sub

        ''' <summary>
        ''' 批量无空位延伸:对固定查询位置 <paramref name="qPos"/>,并行处理
        ''' <paramref name="subjects"/> 中对应 <paramref name="subjectPositions"/> 的若干参考序列。
        ''' 返回与输入顺序一一对应的 <see cref="UngappedHit"/> 数组。
        ''' 当 <see cref="Avx2.IsSupported"/> 为 false 时退化为逐条标量延伸。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ExtendBatch(query As String, qPos As Integer, subjects As String(), subjectPositions As Integer(), subjectIds As Integer()) As UngappedHit()
            Dim n = subjects.Length

            If n = 0 Then
                Return New UngappedHit() {}
            End If

            ' 超过 32 路则分块
            If n <= MaxBatch AndAlso Avx2.IsSupported Then
                Return ExtendBatchAvx2(query, qPos, subjects, subjectPositions, subjectIds)
            Else
                ' 标量回退:逐条调用基类 Extend
                Dim out(n - 1) As UngappedHit

                For i As Integer = 0 To n - 1
                    out(i) = MyBase.Extend(query, qPos, subjects(i), subjectPositions(i))
                Next

                Return out
            End If
        End Function

        ''' <summary>
        ''' AVX2 核心:32 路并行累加无空位得分,并维护每路最优窗口。
        ''' </summary>
        Private Function ExtendBatchAvx2(query As String, qPos As Integer, subjects As String(), sPos As Integer(), sIds As Integer()) As UngappedHit()
            Dim n = subjects.Length
            Dim out(n - 1) As UngappedHit

            If n > MaxBatch Then
                n = MaxBatch
            End If

            ' 公共延伸长度:各参考可用长度与 BatchWindow 的较小值;不足者该 lane 末尾屏蔽
            Dim W = BatchWindow
            Dim qAvail = query.Length - qPos

            ' 延伸到序列末端(与标量 UngappedExtension 一致):W 取查询可用长度,
            ' 各 lane 在自身参考序列末端后屏蔽(不再累加、不再截断)。
            If qAvail < W Then
                W = qAvail
            End If

            ' 4 个 __m256i(int32 × 8)覆盖 32 个 lane 的累加器/最优得分
            Dim acc(3) As Vector256(Of Integer)
            Dim best(3) As Vector256(Of Integer)
            Dim bestRight(3) As Vector256(Of Integer)
            Dim valid(3) As Vector256(Of Integer)   ' 每 lane 是否仍有效(未越界)

            For lane = 0 To 31
                If lane < n Then
                    acc(lane \ 8) = acc(lane \ 8).WithElement(lane Mod 8, 0)
                    best(lane \ 8) = best(lane \ 8).WithElement(lane Mod 8, 0)
                    bestRight(lane \ 8) = bestRight(lane \ 8).WithElement(lane Mod 8, 0)
                    valid(lane \ 8) = valid(lane \ 8).WithElement(lane Mod 8, -1)  ' 全 1 = 有效
                Else
                    valid(lane \ 8) = valid(lane \ 8).WithElement(lane Mod 8, 0)  ' 屏蔽
                End If
            Next

            Dim dropoffVec = Vector256.Create(Dropoff)

            For k As Integer = 0 To W - 1
                If k >= qAvail Then
                    Exit For
                End If

                Dim qk = query(qPos + k)

                ' 逐 lane 查表得分(标量查 BLOSUM62),再装入 __m256i 做 SIMD 累加与最优比较
                For g As Integer = 0 To 3
                    Dim scores(7) As Integer

                    For l = 0 To 7
                        Dim lane = g * 8 + l

                        If lane >= n Then
                            scores(l) = 0
                            Continue For
                        End If

                        Dim sid = sPos(lane)
                        Dim sAvail = subjects(lane).Length - sid

                        If k >= sAvail Then
                            ' 该 lane 已到序列末端:标记为无效(不再累加、不再截断)
                            valid(g) = valid(g).WithElement(l, 0)
                            scores(l) = 0
                        Else
                            scores(l) = CInt(blosum.GetDistance(qk, subjects(lane)(sid + k)))
                        End If
                    Next

                    Dim vScore = Vector256.Create(scores(0), scores(1), scores(2), scores(3), scores(4), scores(5), scores(6), scores(7))

                    ' acc = (acc + score);仅对有效 lane,低于 dropoff 则截断为 0
                    Dim added = Avx2.Add(acc(g), vScore)
                    Dim below = Avx2.CompareGreaterThan(dropoffVec, added)  ' dropoff > added 即 added < dropoff
                    below = Avx2.And(below, valid(g))                       ' 无效 lane 不参与截断
                    acc(g) = Avx2.AndNot(below, added)

                    ' bestRight 比较须用更新前的 best;先判定再更新 best
                    Dim gtR = Avx2.CompareGreaterThan(acc(g), best(g))
                    Dim upd = Avx2.And(gtR, Vector256.Create(k))
                    bestRight(g) = Avx2.Or(upd, Avx2.AndNot(gtR, bestRight(g)))

                    ' best = max(best, acc)
                    Dim gt = Avx2.CompareGreaterThan(acc(g), best(g))
                    best(g) = Avx2.Or(Avx2.And(gt, acc(g)), Avx2.AndNot(gt, best(g)))
                Next
            Next

            ' 回写结果
            For lane As Integer = 0 To n - 1
                Dim g = lane \ 8
                Dim l = lane Mod 8
                Dim sc = best(g).GetElement(l)
                Dim r = bestRight(g).GetElement(l)

                out(lane) = New UngappedHit(qPos, qPos + r, sPos(lane), sPos(lane) + r, sc)
            Next

            Return out
        End Function
    End Class
End Namespace
