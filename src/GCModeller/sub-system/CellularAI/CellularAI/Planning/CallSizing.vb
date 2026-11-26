Imports CellularAI.Model
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math.Statistics
Imports std = System.Math

Namespace Planning

    ''' <summary>
    ''' 每个扰动"调用多少个基因"的三步计算：曲线尺寸 → 功效重排 → 相近度抬升。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <b>第一步（曲线尺寸）</b>：把训练数据里该扰动的响应广度映射成调用数量
    ''' <c>n = exp(slope·(x − median(x)))</c>，再整体缩放到"中位数等于任务给定的 <c>median_degs</c>"，
    ''' 最后夹在 <c>[n_min, min(n_max, 可用基因数)]</c> 之间。
    ''' </para>
    ''' <para>
    ''' <b>第二步（功效重排）</b>：按"预期可检测的响应基因数"给扰动打分
    ''' （<c>Φ(k·|lfc|/√(1/μ+φ) − c)</c> 再按 |z| 软截断），并把"宽带显著基因数"作为惩罚项。
    ''' 然后把第一步得到的尺寸多重集按分数重新分配：低分扰动把名额让给高分扰动，
    ''' 让出来的名额按现有尺寸比例分给中位以上的扰动（总量守恒）。
    ''' </para>
    ''' <para>
    ''' <b>第三步（相近度抬升）</b>：对"训练数据广度高于中位"的扰动，把尺寸抬向"按广度曲线算出的尺寸"，
    ''' 但单个扰动的抬升不超过 <c>xsim_cap_factor</c> 倍，整体抬升量不超过总量的 20%。
    ''' </para>
    ''' </remarks>
    Public Module CallSizing

        ''' <summary>曲线尺寸：广度 → 调用数量。</summary>
        Public Function CurveSizes(breadth As Double(), eligibleCount As Integer,
                                   model As CellularAiModel, medianDegs As Double) As Integer()
            Dim x As Double() = DirectCast(breadth.Clone(), Double())
            Dim center As Double = PoolingStatistics.NanMedian(x)

            For i As Integer = 0 To x.Length - 1
                If Double.IsNaN(x(i)) Then
                    x(i) = center
                End If
            Next

            Dim medianX As Double = PoolingStatistics.NanMedian(x)
            Dim n(x.Length - 1) As Double

            For i As Integer = 0 To x.Length - 1
                n(i) = std.Exp(model.Plan.CurveSlope * (x(i) - medianX))
            Next

            Dim medianN As Double = PoolingStatistics.NanMedian(n)

            If medianN > 0.0 Then
                For i As Integer = 0 To n.Length - 1
                    n(i) *= medianDegs / medianN
                Next
            End If

            Dim upper As Integer = std.Min(model.Calls.NMax, eligibleCount)
            Dim result(n.Length - 1) As Integer

            For i As Integer = 0 To n.Length - 1
                result(i) = CInt(std.Max(std.Min(std.Round(n(i), MidpointRounding.AwayFromZero), upper), model.Calls.NMin))
            Next

            Return result
        End Function

        ''' <summary>
        ''' 功效得分：<c>rankz(log1p(Σ 可检测基因数)) − t_pen·rankz(log1p(宽带显著基因数))</c>。
        ''' </summary>
        ''' <param name="l2">合并后的 log2 效应（基因×扰动）</param>
        ''' <param name="z">合并后的 |z|（基因×扰动）</param>
        ''' <param name="zmix">计划显著性（基因×扰动）</param>
        ''' <param name="meanCpm">对照平均 CPM（基因）</param>
        ''' <param name="own">逐点的"非自身靶"掩码（基因×扰动）</param>
        Public Function PowerScore(l2 As Double()(), z As Double()(), zmix As Double()(),
                                   meanCpm As Double(), own As Boolean()(),
                                   model As CellularAiModel) As Double()

            Dim nGenes As Integer = l2.Length
            Dim nPerts As Integer = If(nGenes = 0, 0, l2(0).Length)
            Dim plan As PlanOptions = model.Plan
            Dim detect(nPerts - 1) As Double
            Dim broad(nPerts - 1) As Double

            For g As Integer = 0 To nGenes - 1
                Dim eligible As Boolean = meanCpm(g) > model.Calls.EligibleCpm
                Dim mu As Double = std.Max(meanCpm(g) * plan.PowerDepth / 1000000.0, 0.000000001)

                For p As Integer = 0 To nPerts - 1
                    If Not eligible OrElse Not own(g)(p) Then
                        Continue For
                    End If

                    Dim sigma As Double = std.Sqrt(1.0 / std.Max(mu, 0.001) + plan.PowerPhi)
                    Dim phi As Double = NormalDistribution.CDF(plan.PowerK * std.Abs(l2(g)(p)) / sigma - plan.PowerC)
                    Dim soft As Double = 1.0 - plan.PowerSoftZ * plan.PowerSoftZ / std.Max(std.Abs(z(g)(p)), 0.001) ^ 2

                    detect(p) += phi * std.Max(std.Min(soft, 1.0), 0.0)

                    If std.Abs(zmix(g)(p)) > plan.PowerZN2 Then
                        broad(p) += 1.0
                    End If
                Next
            Next

            Dim first As Double() = RankZ(Enumerable.Range(0, nPerts).Select(Function(p) std.Log(1.0 + detect(p))).ToArray)
            Dim second As Double() = RankZ(Enumerable.Range(0, nPerts).Select(Function(p) std.Log(1.0 + broad(p))).ToArray)
            Dim result(nPerts - 1) As Double

            For p As Integer = 0 To nPerts - 1
                result(p) = first(p) - plan.PowerTPen * second(p)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 按分数重排尺寸：低分扰动收缩、把名额让给中位以上的高分扰动（总量守恒）。
        ''' </summary>
        Public Function S1Sizes(curve As Integer(), score As Double(), model As CellularAiModel) As Integer()
            Dim count As Integer = curve.Length

            If count <= 1 Then
                Return DirectCast(curve.Clone(), Integer())
            End If

            Dim nMax As Integer = model.Calls.NMax
            Dim order As Integer() = Enumerable.Range(0, count).OrderBy(Function(i) score(i)).ThenBy(Function(i) i).ToArray
            Dim sorted As Integer() = curve.OrderBy(Function(v) v).ToArray
            Dim K(count - 1) As Double

            For rank As Integer = 0 To count - 1
                K(order(rank)) = sorted(rank)
            Next

            Dim median As Double = PoolingStatistics.NanMedian(K)
            Dim percentile(count - 1) As Double

            For i As Integer = 0 To count - 1
                percentile(i) = RankOf(score)(i) / (count - 1)
            Next

            Dim freed As Double = 0.0
            Dim shrunk As New List(Of Integer)

            For i As Integer = 0 To count - 1
                If percentile(i) < model.Plan.ShrinkQuantile AndAlso K(i) < median Then
                    Dim target As Double = std.Max(1.0, K(i) * model.Plan.ShrinkFactor)

                    freed += K(i) - target
                    K(i) = target
                    Call shrunk.Add(i)
                End If
            Next

            ' 中位以上的扰动（按尺寸排序后的后半段且严格大于中位数）
            Dim bySize As Integer() = Enumerable.Range(0, count).OrderBy(Function(i) K(i)).ThenBy(Function(i) i).ToArray
            Dim upper As New HashSet(Of Integer)

            For i As Integer = count \ 2 + 1 To count - 1
                Dim index As Integer = bySize(i)

                If K(index) > median Then
                    Call upper.Add(index)
                End If
            Next

            For iteration As Integer = 1 To 20
                Dim room As Integer() = upper.Where(Function(i) K(i) < nMax).ToArray

                If freed <= 1.0E-9 OrElse room.Length = 0 Then
                    Exit For
                End If

                Dim roomTotal As Double = 0.0

                For Each i As Integer In room
                    roomTotal += K(i)
                Next

                For Each i As Integer In room
                    Dim add As Double = K(i) * freed / std.Max(roomTotal, 1.0E-300)
                    Dim updated As Double = std.Min(K(i) + add, nMax)

                    freed -= updated - K(i)
                    K(i) = updated
                Next
            Next

            Dim result(count - 1) As Integer

            For i As Integer = 0 To count - 1
                result(i) = CInt(std.Max(1.0, std.Round(K(i), MidpointRounding.AwayFromZero)))
            Next

            Return result
        End Function

        ''' <summary>相近度抬升：把上半分位扰动的尺寸抬向"按训练广度曲线算出的尺寸"。</summary>
        Public Function XsimSizes(curve As Integer(), xs As Double(), score As Double(), model As CellularAiModel) As Integer()
            Dim count As Integer = curve.Length

            If count = 0 Then
                Return New Integer(-1) {}
            End If

            Dim level As Double = PoolingStatistics.NanMedian(curve.Select(Function(v) CDbl(v)).ToArray())
            Dim total As Integer = curve.Sum
            Dim plan As PlanOptions = model.Plan
            Dim nMax As Integer = model.Calls.NMax

            ' 二分斜率，使曲线总量等于当前的平曲线总量
            Dim lo As Double = 0.0
            Dim hi As Double = 2.0 * plan.CurveSlope

            For iteration As Integer = 1 To 60
                Dim mid As Double = 0.5 * (lo + hi)

                If CurveOn(xs, level, mid, nMax).Sum > total Then
                    hi = mid
                Else
                    lo = mid
                End If
            Next

            Dim xsb As Integer() = CurveOn(xs, level, 0.5 * (lo + hi), nMax)
            Dim s1 As Integer() = S1Sizes(curve, score, model)
            Dim percentile As Double() = RankOf(xs)
            Dim add(count - 1) As Double
            Dim addTotal As Double = 0.0

            For i As Integer = 0 To count - 1
                Dim q As Double = If(count > 1, percentile(i) / (count - 1), 1.0)
                Dim target As Integer = std.Min(std.Max(s1(i), xsb(i)),
                                               std.Min(CInt(std.Truncate(plan.XsimCapFactor * s1(i))), nMax))

                If q >= plan.XsimStrength Then
                    add(i) = std.Max(target - s1(i), 0)
                    addTotal += add(i)
                End If
            Next

            Dim s1Total As Double = s1.Sum
            Dim roomTotal As Double = std.Max(plan.XsimMaxTotal * s1Total - s1Total, 0.0)

            If addTotal > roomTotal AndAlso addTotal > 0.0 Then
                For i As Integer = 0 To count - 1
                    add(i) *= roomTotal / addTotal
                Next
            End If

            Dim result(count - 1) As Integer

            For i As Integer = 0 To count - 1
                result(i) = s1(i) + CInt(std.Floor(add(i)))
            Next

            Return result
        End Function

        ''' <summary>
        ''' 每个扰动的计划尺寸：曲线尺寸 → 功效重排 → 相近度抬升，并受"可用基因数"限制。
        ''' </summary>
        Public Function PlanSizes(breadth As Double(), xs As Double(),
                                  l2 As Double()(), z As Double()(), zplan As Double()(),
                                  meanCpm As Double(), tidx As Integer(),
                                  model As CellularAiModel, medianDegs As Double) As Integer()

            Dim nGenes As Integer = l2.Length
            Dim nPerts As Integer = If(nGenes = 0, 0, l2(0).Length)
            Dim own As Boolean()() = OwnMask(nGenes, nPerts, tidx)
            Dim capacity(nPerts - 1) As Integer

            For p As Integer = 0 To nPerts - 1
                Dim count As Integer = 0

                For g As Integer = 0 To nGenes - 1
                    If meanCpm(g) > model.Calls.EligibleCpm AndAlso own(g)(p) Then
                        count += 1
                    End If
                Next

                capacity(p) = count
            Next

            Dim sizes As Integer() = CurveSizes(breadth, If(capacity.Length = 0, 0, capacity.Max), model, medianDegs)
            Dim result(nPerts - 1) As Integer
            Dim active As New List(Of Integer)

            For p As Integer = 0 To nPerts - 1
                If capacity(p) > 0 Then
                    Call active.Add(p)
                End If
            Next

            If active.Count = 0 Then
                Return result
            End If

            Dim index As Integer() = active.ToArray
            Dim subL2 As Double()() = SelectColumns(l2, index)
            Dim subZ As Double()() = SelectColumns(z, index)
            Dim subPlan As Double()() = SelectColumns(zplan, index)
            Dim subOwn As Boolean()() = SelectColumns(own, index)
            Dim score As Double() = PowerScore(subL2, subZ, subPlan, meanCpm, subOwn, model)
            Dim curve(index.Length - 1) As Integer
            Dim xsSub(index.Length - 1) As Double

            For i As Integer = 0 To index.Length - 1
                curve(i) = sizes(index(i))
                xsSub(i) = xs(index(i))
            Next

            Dim allocated As Integer() = XsimSizes(curve, xsSub, score, model)

            For i As Integer = 0 To index.Length - 1
                result(index(i)) = std.Min(allocated(i), capacity(index(i)))
            Next

            Return result
        End Function

#Region "helpers"

        ''' <summary>由扰动轴构造"非自身靶"掩码。</summary>
        Public Function OwnMask(nGenes As Integer, nPerts As Integer, tidx As Integer()) As Boolean()()
            Dim mask As Boolean()() = RectangularArray.Matrix(Of Boolean)(nGenes, nPerts)

            For g As Integer = 0 To nGenes - 1
                For p As Integer = 0 To nPerts - 1
                    mask(g)(p) = True
                Next
            Next

            For p As Integer = 0 To std.Min(nPerts, tidx.Length) - 1
                If tidx(p) >= 0 AndAlso tidx(p) < nGenes Then
                    mask(tidx(p))(p) = False
                End If
            Next

            Return mask
        End Function

        ''' <summary>秩变换到零均值单位方差。</summary>
        Public Function RankZ(values As Double()) As Double()
            Dim count As Integer = values.Length

            If count = 0 Then
                Return New Double(-1) {}
            End If

            Dim ranks As Double() = RankOf(values)
            Dim mean As Double = ranks.Average
            Dim acc As Double = 0.0

            For i As Integer = 0 To count - 1
                Dim d As Double = ranks(i) - mean

                acc += d * d
            Next

            Dim sd As Double = std.Sqrt(acc / count)
            Dim result(count - 1) As Double

            For i As Integer = 0 To count - 1
                result(i) = If(sd > 0.0, (ranks(i) - mean) / sd, 0.0)
            Next

            Return result
        End Function

        ''' <summary>升序秩（0 起，并列按出现顺序）。</summary>
        Public Function RankOf(values As Double()) As Double()
            Dim count As Integer = values.Length
            Dim order As Integer() = Enumerable.Range(0, count).OrderBy(Function(i) values(i)).ThenBy(Function(i) i).ToArray
            Dim result(count - 1) As Double

            For rank As Integer = 0 To count - 1
                result(order(rank)) = rank
            Next

            Return result
        End Function

        Private Function CurveOn(x As Double(), level As Double, slope As Double, nMax As Integer) As Integer()
            Dim median As Double = PoolingStatistics.NanMedian(x)
            Dim n(x.Length - 1) As Double
            Dim medianN As Double

            For i As Integer = 0 To x.Length - 1
                n(i) = std.Exp(slope * (x(i) - median))
            Next

            medianN = PoolingStatistics.NanMedian(n)

            Dim result(x.Length - 1) As Integer

            For i As Integer = 0 To x.Length - 1
                Dim value As Double = If(medianN > 0.0, level * n(i) / medianN, level)

                result(i) = CInt(std.Max(std.Min(std.Round(value, MidpointRounding.AwayFromZero), nMax), 1))
            Next

            Return result
        End Function

        Private Function SelectColumns(values As Double()(), columns As Integer()) As Double()()
            Dim result As Double()() = RectangularArray.Matrix(Of Double)(values.Length, columns.Length)

            For i As Integer = 0 To values.Length - 1
                For j As Integer = 0 To columns.Length - 1
                    result(i)(j) = values(i)(columns(j))
                Next
            Next

            Return result
        End Function

        Private Function SelectColumns(values As Boolean()(), columns As Integer()) As Boolean()()
            Dim result As Boolean()() = RectangularArray.Matrix(Of Boolean)(values.Length, columns.Length)

            For i As Integer = 0 To values.Length - 1
                For j As Integer = 0 To columns.Length - 1
                    result(i)(j) = values(i)(columns(j))
                Next
            Next

            Return result
        End Function

#End Region

    End Module

    ''' <summary>计划层内部使用的描述统计（避免与证据层的同名成员混淆）。</summary>
    Friend Module PoolingStatistics

        ''' <summary>忽略 NaN 的中位数。</summary>
        Public Function NanMedian(values As Double()) As Double
            If values Is Nothing Then
                Return Double.NaN
            End If

            Dim finite As Double() = values.Where(Function(v) Not Double.IsNaN(v)).OrderBy(Function(v) v).ToArray

            If finite.Length = 0 Then
                Return Double.NaN
            End If
            If finite.Length Mod 2 = 1 Then
                Return finite(finite.Length \ 2)
            End If

            Return (finite(finite.Length \ 2 - 1) + finite(finite.Length \ 2)) / 2.0
        End Function

    End Module
End Namespace
