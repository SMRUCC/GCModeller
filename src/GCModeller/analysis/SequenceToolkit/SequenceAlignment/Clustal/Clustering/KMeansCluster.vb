Namespace Clustal

    ''' <summary>
    ''' 在 mBed 的低维坐标空间上做 k-means 聚类，用于把大规模序列集合切分成
    ''' 若干条数受控的子簇(对应 clustalo 的 <c>--cluster-size</c> 语义)。
    ''' </summary>
    ''' <remarks>
    ''' 初始化使用 k-means++(固定随机种子)，迭代使用标准 Lloyd 算法。所有平局都按
    ''' "较小簇编号优先"处理，因此同一份输入与同一个种子必然得到完全一致的分簇结果。
    ''' </remarks>
    Public Module KMeansCluster

        Private Const MaxIterations As Integer = 32

        ''' <summary>
        ''' 对低维坐标做 k-means 聚类
        ''' </summary>
        ''' <param name="feature">N x d 的坐标矩阵</param>
        ''' <param name="clusterCount">簇数量(会被限制在 [1, N] 之间)</param>
        ''' <param name="seed">随机种子</param>
        ''' <returns>每条序列所属的簇编号</returns>
        Public Function Cluster(feature As Double()(), clusterCount As Integer, seed As Integer) As Integer()
            Dim n As Integer = If(feature Is Nothing, 0, feature.Length)

            If n = 0 Then
                Return New Integer() {}
            End If

            Dim k As Integer = Math.Max(1, Math.Min(clusterCount, n))
            Dim assignments As Integer() = New Integer(n - 1) {}

            If k = 1 Then
                Return assignments
            End If

            Dim d As Integer = If(feature(0) Is Nothing, 0, feature(0).Length)

            If d = 0 Then
                ' 没有任何坐标信息时退化为平均切分，保证每个簇的条数都受控
                For i As Integer = 0 To n - 1
                    assignments(i) = Math.Min(k - 1, CInt(Math.Floor(CDbl(i) * k / n)))
                Next

                Return assignments
            End If

            Dim rnd As New Random(seed)
            Dim centroids As Double()() = InitCentroids(feature, k, d, rnd)
            Dim dist As Double() = New Double(n - 1) {}
            Dim counts As Integer() = New Integer(k - 1) {}

            For iteration As Integer = 1 To MaxIterations
                Dim changed As Boolean = False

                ' 分配
                For i As Integer = 0 To n - 1
                    Dim best As Integer = 0
                    Dim bestDist As Double = Double.MaxValue

                    For c As Integer = 0 To k - 1
                        Dim value As Double = SquaredDistance(feature(i), centroids(c))

                        If value < bestDist Then
                            bestDist = value
                            best = c
                        End If
                    Next

                    If assignments(i) <> best Then
                        assignments(i) = best
                        changed = True
                    End If

                    dist(i) = bestDist
                Next

                ' 更新质心
                Dim sums As Double()() = New Double(k - 1)() {}

                For c As Integer = 0 To k - 1
                    sums(c) = New Double(d - 1) {}
                    counts(c) = 0
                Next

                For i As Integer = 0 To n - 1
                    Dim c As Integer = assignments(i)
                    Dim sum As Double() = sums(c)
                    Dim row As Double() = feature(i)

                    For t As Integer = 0 To d - 1
                        sum(t) += row(t)
                    Next

                    counts(c) += 1
                Next

                For c As Integer = 0 To k - 1
                    If counts(c) = 0 Then
                        ' 空簇：把它重新安置到当前离质心最远的一点上，避免簇退化
                        Dim farthest As Integer = ArgMax(dist)

                        If farthest >= 0 Then
                            centroids(c) = CType(feature(farthest).Clone(), Double())
                            dist(farthest) = 0
                        End If
                    Else
                        Dim sum As Double() = sums(c)

                        For t As Integer = 0 To d - 1
                            centroids(c)(t) = sum(t) / counts(c)
                        Next
                    End If
                Next

                If Not changed AndAlso iteration > 1 Then
                    Exit For
                End If
            Next

            Return assignments
        End Function

        ''' <summary>
        ''' k-means++ 初始化
        ''' </summary>
        Private Function InitCentroids(feature As Double()(), k As Integer, d As Integer, rnd As Random) As Double()()
            Dim n As Integer = feature.Length
            Dim centroids As Double()() = New Double(k - 1)() {}
            Dim nearest As Double() = New Double(n - 1) {}

            For i As Integer = 0 To n - 1
                nearest(i) = Double.MaxValue
            Next

            Dim first As Integer = rnd.Next(n)
            centroids(0) = CType(feature(first).Clone(), Double())

            For i As Integer = 0 To n - 1
                nearest(i) = SquaredDistance(feature(i), centroids(0))
            Next

            For c As Integer = 1 To k - 1
                Dim total As Double = 0

                For i As Integer = 0 To n - 1
                    total += nearest(i)
                Next

                Dim chosen As Integer = -1

                If total <= 0 Then
                    ' 所有点已经完全重合，直接按顺序取下一个不同的点
                    chosen = c Mod n
                Else
                    Dim target As Double = rnd.NextDouble() * total
                    Dim acc As Double = 0

                    For i As Integer = 0 To n - 1
                        acc += nearest(i)

                        If acc >= target Then
                            chosen = i
                            Exit For
                        End If
                    Next

                    If chosen < 0 Then
                        chosen = n - 1
                    End If
                End If

                centroids(c) = CType(feature(chosen).Clone(), Double())

                For i As Integer = 0 To n - 1
                    Dim value As Double = SquaredDistance(feature(i), centroids(c))

                    If value < nearest(i) Then
                        nearest(i) = value
                    End If
                Next
            Next

            Return centroids
        End Function

        Private Function SquaredDistance(a As Double(), b As Double()) As Double
            Dim acc As Double = 0
            Dim n As Integer = Math.Min(a.Length, b.Length)

            For i As Integer = 0 To n - 1
                Dim diff As Double = a(i) - b(i)
                acc += diff * diff
            Next

            Return acc
        End Function

        Private Function ArgMax(data As Double()) As Integer
            If data.Length = 0 Then
                Return -1
            End If

            Dim index As Integer = 0

            For i As Integer = 1 To data.Length - 1
                If data(i) > data(index) Then
                    index = i
                End If
            Next

            Return index
        End Function
    End Module
End Namespace
