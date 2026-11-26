' ------------------------------------------------------------------------
' 排序 / 查找 / 选择 的审计
'
' 每个用例都拿"待测路径 vs 独立参考实现"对比：
'   * 排序参考：System.Array.Sort（NaN 沉尾的比较器），与基数排序毫无关系
'   * 二分查找参考：线性扫描（不是二分）
'   * 前 K 名参考：全排序后截断
'   * 并列和参考：从参考排序结果重算
' 因此无论本次是否真的用上 GPU，用例都能发现实现错误。
' ------------------------------------------------------------------------

Imports Microsoft.VisualBasic.Computing.ILCuda.Math

Namespace Audit

    ''' <summary>GpuSort 的验收用例</summary>
    Public Module SortAudit

        Public Function Run() As Integer
            Call Console.WriteLine()
            Call Console.WriteLine("== 排序 / 查找 / 选择 ==")

            RandomSegmentsDouble()
            RandomSegmentsSingle()
            AllEqualSegment()
            SingleElementSegments()
            NaNAndNegativeZero()
            LargeSegment()
            BoundsQueries()
            TopKSelection()
            TieSums()

            Return AuditRunner.FailureCount
        End Function

        ' ------------------------------------------------------------------
        ' 用例
        ' ------------------------------------------------------------------

        Private Sub RandomSegmentsDouble()
            Dim segments As Integer = 8
            Dim length As Integer = 1024
            Dim keys As Double() = RandomKeys(segments * length, 7, duplicates:=True)
            Dim expected As Double() = ReferenceSort(keys, length)
            Dim actual As Double() = GpuSort.SortSegments(AuditRunner.Engine, keys, length)
            Dim err As Double = AuditRunner.MaxAbsError(actual, expected)

            Call AuditRunner.Capture("sortF64.maxAbsError", err)
            Call AuditRunner.Case("分段排序（双精度，8 x 1024，含重复值）", err = 0.0, $"maxAbsError={err:G6}")
        End Sub

        Private Sub RandomSegmentsSingle()
            Dim segments As Integer = 8
            Dim length As Integer = 1024
            Dim keys As Single() = RandomKeysSingle(segments * length, 11, duplicates:=True)
            Dim expected As Single() = ReferenceSort(keys, length)
            Dim actual As Single() = GpuSort.SortSegments(AuditRunner.Engine, keys, length)
            Dim err As Double = AuditRunner.MaxAbsError(actual, expected)

            Call AuditRunner.Capture("sortF32.maxAbsError", err)
            Call AuditRunner.Case("分段排序（单精度，8 x 1024，含重复值）", err = 0.0, $"maxAbsError={err:G6}")
        End Sub

        Private Sub AllEqualSegment()
            Dim length As Integer = 600
            Dim keys As Double() = Enumerable.Repeat(3.25, length * 2).ToArray()
            Dim actual As Double() = GpuSort.SortSegments(AuditRunner.Engine, keys, length)
            Dim err As Double = AuditRunner.MaxAbsError(actual, keys)
            Dim ties As Double() = GpuSort.TieSum(AuditRunner.Engine, actual, length)
            Dim expectedTie As Double = CDbl(length) * length * length - length

            Call AuditRunner.Case("全等值段排序保持原值", err = 0.0, $"maxAbsError={err:G6}")
            Call AuditRunner.Case("全等值段并列和 = n³-n",
                                  System.Math.Abs(ties(0) - expectedTie) = 0.0 AndAlso System.Math.Abs(ties(1) - expectedTie) = 0.0,
                                  $"actual={ties(0):G0}, expected={expectedTie:G0}")
        End Sub

        Private Sub SingleElementSegments()
            Dim keys As Double() = {5.0, -2.0, 0.0, 7.5, -1.0}
            Dim actual As Double() = GpuSort.SortSegments(AuditRunner.Engine, keys, 1)
            Dim err As Double = AuditRunner.MaxAbsError(actual, keys)

            Call AuditRunner.Case("段长为 1 时排序是恒等变换", err = 0.0, $"maxAbsError={err:G6}")
        End Sub

        Private Sub NaNAndNegativeZero()
            Dim length As Integer = 512
            Dim keys(length * 2 - 1) As Double

            For i As Integer = 0 To keys.Length - 1
                keys(i) = If(i Mod 17 = 0, Double.NaN, CDbl((i * 37) Mod 101) - 50.0)
            Next

            keys(3) = -0.0
            keys(4) = 0.0

            Dim expected As Double() = ReferenceSort(keys, length)
            Dim actual As Double() = GpuSort.SortSegments(AuditRunner.Engine, keys, length)
            Dim nanAtTail As Boolean = True

            For segment As Integer = 0 To 1
                Dim offset As Integer = segment * length
                Dim firstNaN As Integer = -1

                For i As Integer = 0 To length - 1
                    If Double.IsNaN(actual(offset + i)) Then
                        firstNaN = i
                        Exit For
                    End If
                Next

                If firstNaN >= 0 Then
                    For i As Integer = firstNaN To length - 1
                        If Not Double.IsNaN(actual(offset + i)) Then
                            nanAtTail = False
                        End If
                    Next
                End If
            Next

            Dim ok As Boolean = nanAtTail AndAlso SameOrder(actual, expected)

            Call AuditRunner.Case("NaN 沉到段尾、-0.0 与 +0.0 视为相等", ok,
                                  $"nanAtTail={nanAtTail}, sameOrder={SameOrder(actual, expected)}")
        End Sub

        Private Sub LargeSegment()
            Dim length As Integer = 18000
            Dim keys As Double() = RandomKeys(length * 2, 23, duplicates:=True)
            Dim expected As Double() = ReferenceSort(keys, length)
            Dim actual As Double() = GpuSort.SortSegments(AuditRunner.Engine, keys, length)
            Dim err As Double = AuditRunner.MaxAbsError(actual, expected)

            Call AuditRunner.Capture("sortF64.large.maxAbsError", err)
            Call AuditRunner.Case("大段排序（2 x 18000，对照细胞规模）", err = 0.0, $"maxAbsError={err:G6}")
        End Sub

        Private Sub BoundsQueries()
            Dim length As Integer = 4096
            Dim segments As Integer = 2
            Dim keys As Double() = RandomKeys(length * segments, 31, duplicates:=True)
            Dim sorted As Double() = ReferenceSort(keys, length)
            Dim perSegment As Integer = 64
            Dim queries As Double() = RandomKeys(segments * perSegment, 47, duplicates:=True)
            Dim bounds = GpuSort.Bounds(AuditRunner.Engine, sorted, queries, length, perSegment)

            Dim lowerOk As Boolean = True
            Dim upperOk As Boolean = True

            For segment As Integer = 0 To segments - 1
                Dim offset As Integer = segment * length

                For q As Integer = 0 To perSegment - 1
                    Dim index As Integer = segment * perSegment + q
                    Dim value As Double = queries(index)
                    Dim lo As Integer = 0
                    Dim hi As Integer = 0

                    ' 线性扫描参考：与二分完全独立的实现
                    While lo < length AndAlso sorted(offset + lo) < value
                        lo += 1
                    End While

                    While hi < length AndAlso sorted(offset + hi) <= value
                        hi += 1
                    End While

                    If bounds.lower(index) <> lo Then lowerOk = False
                    If bounds.upper(index) <> hi Then upperOk = False
                Next
            Next

            Call AuditRunner.Case("批量 lower bound 与线性扫描一致", lowerOk)
            Call AuditRunner.Case("批量 upper bound 与线性扫描一致", upperOk)
        End Sub

        Private Sub TopKSelection()
            Dim n As Integer = 5000
            Dim keys As Single() = RandomKeysSingle(n, 53, duplicates:=True)
            Dim k As Integer = 25
            Dim actual As Integer() = GpuSort.TopKIndices(AuditRunner.Engine, keys, k)
            Dim expected As Integer() = ReferenceTopK(keys, k)
            Dim ok As Boolean = AuditRunner.SameIntegers(actual, expected)

            Call AuditRunner.Case($"前 {k} 名（值降序、值相同按下标升序）", ok,
                                  $"actual={actual.Length}, expected={expected.Length}")

            ' 额外的语义检查：前 k 名的最小值不应小于第 k+1 名
            If actual.Length = k Then
                Dim worst As Single = Single.MaxValue

                For Each i As Integer In actual
                    If keys(i) < worst Then worst = keys(i)
                Next

                Dim outside As Boolean = False

                For i As Integer = 0 To n - 1
                    If Not actual.Contains(i) AndAlso keys(i) > worst Then outside = True
                Next

                Call AuditRunner.Case("前 K 名集合之外不存在更大的值", Not outside)
            End If
        End Sub

        Private Sub TieSums()
            Dim length As Integer = 800
            Dim keys As Double() = RandomKeys(length * 3, 67, duplicates:=True, levels:=5)
            Dim sorted As Double() = ReferenceSort(keys, length)
            Dim actual As Double() = GpuSort.TieSum(AuditRunner.Engine, sorted, length)
            Dim expected As Double() = ReferenceTieSum(sorted, length)

            Dim ok As Boolean = True

            For segment As Integer = 0 To 2
                If System.Math.Abs(actual(segment) - expected(segment)) > 1.0E-09 Then ok = False
            Next

            Call AuditRunner.Capture("tieSum.maxAbsError", AuditRunner.MaxAbsError(actual, expected))
            Call AuditRunner.Case("逐段并列和与参考实现一致", ok,
                                  $"maxAbsError={AuditRunner.MaxAbsError(actual, expected):G6}")
        End Sub

        ' ------------------------------------------------------------------
        ' 参考实现（与 GpuSort 的内部实现刻意不同）
        ' ------------------------------------------------------------------

        Private Function ReferenceSort(keys As Double(), segmentLength As Integer) As Double()
            Dim result As Double() = DirectCast(keys.Clone(), Double())
            Dim comparer As New NaNLastComparer()

            For segment As Integer = 0 To (result.Length \ segmentLength) - 1
                Array.Sort(result, segment * segmentLength, segmentLength, comparer)
            Next

            Return result
        End Function

        Private Function ReferenceSort(keys As Single(), segmentLength As Integer) As Single()
            Dim result As Single() = DirectCast(keys.Clone(), Single())
            Dim comparer As New NaNLastComparerSingle()

            For segment As Integer = 0 To (result.Length \ segmentLength) - 1
                Array.Sort(result, segment * segmentLength, segmentLength, comparer)
            Next

            Return result
        End Function

        ''' <summary>从"已排序"的结果重算并列和（单趟游程）。</summary>
        Private Function ReferenceTieSum(sorted As Double(), segmentLength As Integer) As Double()
            Dim segments As Integer = sorted.Length \ segmentLength
            Dim result(segments - 1) As Double

            For segment As Integer = 0 To segments - 1
                Dim offset As Integer = segment * segmentLength
                Dim acc As Double = 0.0
                Dim i As Integer = 0

                While i < segmentLength
                    Dim v As Double = sorted(offset + i)
                    Dim j As Integer = i + 1

                    While j < segmentLength AndAlso sorted(offset + j) = v
                        j += 1
                    End While

                    Dim t As Double = j - i

                    acc += t * t * t - t
                    i = j
                End While

                result(segment) = acc
            Next

            Return result
        End Function

        ''' <summary>全排序后截断的前 K 名（与"阈值选择 + 收集"完全不同的算法）。</summary>
        Private Function ReferenceTopK(keys As Single(), k As Integer) As Integer()
            Dim indices(keys.Length - 1) As Integer

            For i As Integer = 0 To indices.Length - 1
                indices(i) = i
            Next

            Array.Sort(indices, New TopKComparer(keys))

            Dim count As Integer = System.Math.Min(k, keys.Length)
            Dim result(count - 1) As Integer

            Array.Copy(indices, result, count)

            Return result
        End Function

        ' ------------------------------------------------------------------
        ' 数据与工具
        ' ------------------------------------------------------------------

        ''' <summary>生成带重复值的随机键（<paramref name="levels"/> 越小重复越多）。</summary>
        Private Function RandomKeys(count As Integer, seed As Integer,
                                    Optional duplicates As Boolean = False,
                                    Optional levels As Integer = 1000,
                                    Optional allowNaN As Boolean = False) As Double()
            Dim rng As New Random(seed)
            Dim result(count - 1) As Double

            For i As Integer = 0 To count - 1
                Dim v As Double = rng.NextDouble() * 200.0 - 100.0

                If duplicates Then
                    v = System.Math.Round(v, 1)
                    v = CDbl(rng.Next(0, levels))
                End If

                If allowNaN AndAlso i Mod 29 = 0 Then v = Double.NaN

                result(i) = v
            Next

            Return result
        End Function

        Private Function RandomKeysSingle(count As Integer, seed As Integer,
                                          Optional duplicates As Boolean = False,
                                          Optional levels As Integer = 1000) As Single()
            Dim rng As New Random(seed)
            Dim result(count - 1) As Single

            For i As Integer = 0 To count - 1
                Dim v As Single

                If duplicates Then
                    v = CSng(rng.Next(0, levels))
                Else
                    v = CSng(rng.NextDouble() * 200.0 - 100.0)
                End If

                result(i) = v
            Next

            Return result
        End Function

        ''' <summary>-0.0 与 +0.0 视为等价；NaN 与 NaN 视为等价（排序位置已单独校验）。</summary>
        Private Function SameOrder(actual As Double(), expected As Double()) As Boolean
            If actual.Length <> expected.Length Then Return False

            For i As Integer = 0 To actual.Length - 1
                Dim x As Double = actual(i)
                Dim y As Double = expected(i)

                If Double.IsNaN(x) AndAlso Double.IsNaN(y) Then Continue For
                If x <> y Then Return False
            Next

            Return True
        End Function

        Private Class NaNLastComparer
            Implements IComparer(Of Double)

            Public Function Compare(x As Double, y As Double) As Integer Implements IComparer(Of Double).Compare
                Dim xn As Boolean = Double.IsNaN(x)
                Dim yn As Boolean = Double.IsNaN(y)

                If xn Then Return If(yn, 0, 1)
                If yn Then Return -1

                Return x.CompareTo(y)
            End Function
        End Class

        Private Class NaNLastComparerSingle
            Implements IComparer(Of Single)

            Public Function Compare(x As Single, y As Single) As Integer Implements IComparer(Of Single).Compare
                Dim xn As Boolean = Single.IsNaN(x)
                Dim yn As Boolean = Single.IsNaN(y)

                If xn Then Return If(yn, 0, 1)
                If yn Then Return -1

                Return x.CompareTo(y)
            End Function
        End Class

        Private Class TopKComparer
            Implements IComparer(Of Integer)

            Private ReadOnly m_keys As Single()

            Public Sub New(keys As Single())
                m_keys = keys
            End Sub

            Public Function Compare(a As Integer, b As Integer) As Integer Implements IComparer(Of Integer).Compare
                Dim x As Single = m_keys(a)
                Dim y As Single = m_keys(b)
                Dim xn As Boolean = Single.IsNaN(x)
                Dim yn As Boolean = Single.IsNaN(y)

                If xn OrElse yn Then
                    If xn AndAlso yn Then Return a.CompareTo(b)
                    Return If(xn, 1, -1)
                End If

                Dim c As Integer = y.CompareTo(x)

                Return If(c <> 0, c, a.CompareTo(b))
            End Function
        End Class
    End Module
End Namespace
