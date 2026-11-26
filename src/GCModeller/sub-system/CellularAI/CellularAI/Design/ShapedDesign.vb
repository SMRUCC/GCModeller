Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Model
Imports std = System.Math

Namespace Design

    ''' <summary>形状设计的结果：设计因子矩阵与三个搜索是否被截断。</summary>
    Public Class ShapedDesignResult

        ''' <summary>设计因子 <c>L</c>（基因×扰动），与 <c>fc·2^L</c> 成正比。</summary>
        Public Property L As Double()()

        ''' <summary>幅度搜索是否因"64 倍仍不够"被截断。</summary>
        Public Property AmplitudeCapped As Boolean

        ''' <summary>控制程序平滑（CP）搜索是否被截断。</summary>
        Public Property CpCapped As Boolean

        ''' <summary>程序加权（PW）搜索是否被截断。</summary>
        Public Property PwCapped As Boolean

    End Class

    ''' <summary>
    ''' 总量设计的"形状"：符号幂压缩 → 显著性整形 → 与残差剖面混合 → 幅度匹配 →
    ''' 控制程序平滑（CP）→ 程序加权（PW）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <b>符号幂压缩</b> <c>sign(x)·|x|^γ</c>（<c>γ = 0.8</c>）：效应分布是重尾的，
    ''' 直接线性使用会让少数强扰动吃掉几乎全部的总量；指数 &lt; 1 压缩尾部、抬高中间段，
    ''' 让中等强度的扰动也有足够的读段。
    ''' </para>
    ''' <para>
    ''' <b>显著性整形</b> <c>z²/(z²+kz)</c>（<c>kz = 16</c>）：把 |z| 映射到 <c>[0,1)</c> 的饱和度，
    ''' 作用是"证据不足的方向不要给大效应"，避免纯噪声基因被抬成响应基因。
    ''' </para>
    ''' <para>
    ''' <b>与残差剖面混合</b> <c>mix = 0.5</c>：总量形状同时看"池化证据"与"上下文相关的残差剖面"，
    ''' 各占一半。只用池化会丢掉上下文差异，只用残差会放大单上下文的噪声。
    ''' </para>
    ''' <para>
    ''' <b>CP 与 PW</b>：CP 把偏离投影到对照细胞的主成分子空间（只对"该程序确实活跃"的扰动生效，
    ''' 由 <see cref="StrengthGate"/> 判定），PW 按主成分特征值给程序加权
    ''' （主要程序平滑得更强、次要程序保持灵活）。两者都以<b>同一个离散度目标</b>重新标定，
    ''' 因此不会改变总量尺度，只改变形状。
    ''' </para>
    ''' </remarks>
    Public Module ShapedDesign

        ''' <summary>符号幂压缩：<c>sign(x)·|x|^γ</c>。</summary>
        Public Function Base(values As Double()(), gamma As Double) As Double()()
            Dim result(values.Length - 1)() As Double

            For g As Integer = 0 To values.Length - 1
                result(g) = New Double(values(g).Length - 1) {}

                For p As Integer = 0 To values(g).Length - 1
                    Dim v As Double = values(g)(p)

                    result(g)(p) = If(Double.IsNaN(v), 0.0,
                                      If(v < 0.0, -1.0, 1.0) * std.Pow(std.Abs(v), gamma))
                Next
            Next

            Return result
        End Function

        ''' <summary>显著性整形：<c>z²/(z²+kz)</c>。</summary>
        Public Function ZShape(z As Double()(), kz As Double) As Double()()
            Dim result(z.Length - 1)() As Double

            For g As Integer = 0 To z.Length - 1
                result(g) = New Double(z(g).Length - 1) {}

                For p As Integer = 0 To z(g).Length - 1
                    Dim v As Double = If(Double.IsFinite(z(g)(p)), z(g)(p), 0.0)
                    Dim square As Double = v * v

                    result(g)(p) = square / (square + kz)
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 强度门：按"该扰动有多少基因达到 <c>|z| ≥ gate_z</c>"（并以形状偏离范数打破并列）
        ''' 取前 <paramref name="top"/> 比例的扰动，返回 <c>1/0</c> 的门控向量。
        ''' </summary>
        Public Function StrengthGate(z As Double()(), l0 As Double()(), mask As Boolean()(),
                                     top As Double, gateZ As Double) As Double()
            Dim nGenes As Integer = l0.Length
            Dim nPerts As Integer = If(nGenes = 0, 0, l0(0).Length)
            Dim result(nPerts - 1) As Double
            Dim score(nPerts - 1) As Double
            Dim deviation(nPerts - 1) As Double

            ' 逐基因的面板均值（被屏蔽的位置按 0 计）
            For p As Integer = 0 To nPerts - 1
                For g As Integer = 0 To nGenes - 1
                    If mask(g)(p) Then
                        score(p) += std.Abs(If(Double.IsFinite(z(g)(p)), z(g)(p), 0.0))
                    End If
                Next
            Next

            For g As Integer = 0 To nGenes - 1
                Dim sum As Double = 0.0
                Dim count As Integer = 0

                For p As Integer = 0 To nPerts - 1
                    If mask(g)(p) Then
                        sum += l0(g)(p)
                        count += 1
                    End If
                Next

                Dim mean As Double = If(count > 0, sum / count, 0.0)

                For p As Integer = 0 To nPerts - 1
                    If mask(g)(p) Then
                        Dim d As Double = l0(g)(p) - mean

                        deviation(p) += d * d
                    End If
                Next
            Next

            Dim maximum As Double = 0.0

            For p As Integer = 0 To nPerts - 1
                deviation(p) = std.Sqrt(deviation(p))
                maximum = std.Max(maximum, deviation(p))
            Next

            For p As Integer = 0 To nPerts - 1
                Dim hits As Double = 0.0

                For g As Integer = 0 To nGenes - 1
                    If Not mask(g)(p) Then
                        Continue For
                    End If

                    Dim v As Double = If(Double.IsFinite(z(g)(p)), z(g)(p), 0.0)

                    If std.Abs(v) >= gateZ Then
                        hits += 1.0
                    End If
                Next

                score(p) = hits + 0.001 * deviation(p) / std.Max(maximum, 1.0E-12)
            Next

            ' 名次百分位（稳定排序，并列按出现顺序）
            Dim order As Integer() = Enumerable.Range(0, nPerts).
                OrderBy(Function(i) score(i)).ThenBy(Function(i) i).ToArray
            Dim percentile(nPerts - 1) As Double

            For rank As Integer = 0 To nPerts - 1
                percentile(order(rank)) = rank / std.Max(nPerts - 1.0, 1.0)
            Next

            For p As Integer = 0 To nPerts - 1
                result(p) = If(percentile(p) >= 1.0 - top, 1.0, 0.0)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 控制程序平滑：<c>D' = D + λ·g_p·Vᵀ(V·D)</c>，再按同一个离散度目标重新标定幅度。
        ''' </summary>
        ''' <param name="L">输入设计因子</param>
        ''' <param name="fc">对照基因比例</param>
        ''' <param name="mask">"可用"掩码</param>
        ''' <param name="vb">程序载荷（成分×基因）</param>
        ''' <param name="lam">平滑强度</param>
        ''' <param name="gate">逐扰动的门控（1/0）</param>
        ''' <param name="spread">离散度函数</param>
        ''' <param name="target">离散度目标</param>
        Public Function CpDesign(L As Double()(), fc As Double(), mask As Boolean()(),
                                 vb As Double()(), lam As Double, gate As Double(),
                                 spread As Func(Of Double()(), Double), target As Double) As ShapedDesignResult
            Dim nGenes As Integer = L.Length
            Dim nPerts As Integer = If(nGenes = 0, 0, L(0).Length)
            Dim fractions As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)
            Dim columnSums(nPerts - 1) As Double

            For g As Integer = 0 To nGenes - 1
                For p As Integer = 0 To nPerts - 1
                    Dim value As Double = fc(g) * std.Pow(2.0, L(g)(p))

                    fractions(g)(p) = If(Double.IsFinite(value), value, 0.0)
                    columnSums(p) += fractions(g)(p)
                Next
            Next

            Dim normalizer(nPerts - 1) As Double

            For p As Integer = 0 To nPerts - 1
                Dim total As Double = columnSums(p)

                For g As Integer = 0 To nGenes - 1
                    fractions(g)(p) /= std.Max(total, 1.0E-300)
                Next

                normalizer(p) = std.Log(std.Max(total, 1.0E-300), 2.0)
            Next

            ' Bm、逐基因均值、偏离 D
            Dim bm As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)
            Dim mean(nGenes - 1) As Double
            Dim deviations As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)

            For g As Integer = 0 To nGenes - 1
                Dim sum As Double = 0.0
                Dim count As Integer = 0

                For p As Integer = 0 To nPerts - 1
                    bm(g)(p) = std.Log(1.0 + ProfileSpread.TS * fractions(g)(p))

                    If mask(g)(p) Then
                        sum += bm(g)(p)
                        count += 1
                    End If
                Next

                mean(g) = If(count > 0, sum / count, 0.0)

                For p As Integer = 0 To nPerts - 1
                    deviations(g)(p) = bm(g)(p) - mean(g)
                Next
            Next

            ' 平滑后的偏离：Dn = mask ? D + λ·(Vᵀ(V·(mask?D:0)))·g : D
            Dim masked As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)

            For g As Integer = 0 To nGenes - 1
                For p As Integer = 0 To nPerts - 1
                    masked(g)(p) = If(mask(g)(p), deviations(g)(p), 0.0)
                Next
            Next

            Dim projected As Double()() = Project(vb, masked)
            Dim smoothed As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)

            For g As Integer = 0 To nGenes - 1
                For p As Integer = 0 To nPerts - 1
                    smoothed(g)(p) = If(mask(g)(p), deviations(g)(p) + lam * projected(g)(p) * gate(p), deviations(g)(p))
                Next
            Next

            ' 由平滑后的偏离反解 L，并按离散度目标二分缩放 c
            Dim positive As Boolean() = New Boolean(If(nGenes = 0, -1, nGenes - 1)) {}

            For g As Integer = 0 To nGenes - 1
                positive(g) = fc(g) > 0.0
            Next

            Dim toL As Func(Of Double, Double()()) =
                Function(scale As Double) As Double()()
                    Dim result As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)

                    For g As Integer = 0 To nGenes - 1
                        For p As Integer = 0 To nPerts - 1
                            If positive(g) AndAlso mask(g)(p) Then
                                ' f = expm1(max(μ + c·D', 0)) / TS
                                Dim fraction As Double = (std.Exp(std.Max(mean(g) + scale * smoothed(g)(p), 0.0)) - 1.0) / ProfileSpread.TS
                                Dim ratio As Double = std.Max(fraction, 1.0E-300) / fc(g)
                                Dim value As Double = std.Log(std.Max(ratio * std.Pow(2.0, normalizer(p)), 1.0E-300), 2.0)

                                result(g)(p) = std.Max(value, -60.0)
                            Else
                                result(g)(p) = L(g)(p)
                            End If
                        Next
                    Next

                    Return result
                End Function

            Dim lo As Double = 0.0
            Dim hi As Double = 1.0
            Dim s As Double = spread(toL(hi))

            While s < target AndAlso hi < 256.0
                hi *= 2.0
                s = spread(toL(hi))
            End While

            Dim capped As Boolean = s < target

            For iteration As Integer = 1 To 30
                Dim mid As Double = 0.5 * (lo + hi)

                If spread(toL(mid)) < target Then
                    lo = mid
                Else
                    hi = mid
                End If
            Next

            Return New ShapedDesignResult With {
                .L = toL(hi),
                .AmplitudeCapped = False,
                .CpCapped = capped,
                .PwCapped = False
            }
        End Function

        ''' <summary>
        ''' 完整形状设计：幅度匹配 → CP 平滑 → PW 加权，三个搜索共用同一个离散度目标。
        ''' </summary>
        Public Function Fit(l2Tot As Double()(), zTot As Double()(), z As Double()(), profile As Double()(),
                            fc As Double(), mask As Boolean()(), pcs As ControlPcs,
                            spread As Func(Of Double()(), Double), target As Double,
                            options As TotalsOptions) As ShapedDesignResult
            Dim nGenes As Integer = l2Tot.Length
            Dim nPerts As Integer = If(nGenes = 0, 0, l2Tot(0).Length)
            Dim zz As Double()() = Finite(zTot, nGenes, nPerts)
            Dim l0 As Double()() = Multiply(Base(l2Tot, options.ShapeGamma), ZShape(zz, options.ShapeKz))
            Dim za As Double()() = Finite(z, nGenes, nPerts)
            Dim fromProfile As Double()() = Multiply(Base(profile, options.ShapeGamma), ZShape(za, options.ShapeKz))

            For g As Integer = 0 To nGenes - 1
                For p As Integer = 0 To nPerts - 1
                    l0(g)(p) = (1.0 - options.Mix) * l0(g)(p) + options.Mix * fromProfile(g)(p)
                Next
            Next

            ' 逐基因的面板均值与幅度设计
            Dim lbar(nGenes - 1) As Double

            For g As Integer = 0 To nGenes - 1
                Dim sum As Double = 0.0
                Dim count As Integer = 0

                For p As Integer = 0 To nPerts - 1
                    If mask(g)(p) Then
                        sum += l0(g)(p)
                        count += 1
                    End If
                Next

                lbar(g) = If(count > 0, sum / count, 0.0)
            Next

            Dim design As Func(Of Double, Double()()) =
                Function(a As Double) As Double()()
                    Dim result As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)

                    For g As Integer = 0 To nGenes - 1
                        For p As Integer = 0 To nPerts - 1
                            result(g)(p) = lbar(g) + a * (l0(g)(p) - lbar(g))
                        Next
                    Next

                    Return result
                End Function

            Dim amplitude As AmplitudeSolution = MarginSolver.BisectAmp(spread, design, target)

            ' CP：主成分 10..49
            Dim cpRank As Integer = std.Min(std.Max(options.CpK, 0), pcs.Rank)
            Dim cpStart As Integer = std.Min(std.Max(options.CpJ0, 0), cpRank)
            Dim cpLoadings As Double()() = SliceRows(pcs.Loadings, cpStart, cpRank)
            Dim gateCp As Double() = StrengthGate(zz, l0, mask, options.CpTop, options.GateZ)
            Dim cpResult As ShapedDesignResult = CpDesign(design(amplitude.Amplitude), fc, mask,
                                                          cpLoadings, options.CpLam, gateCp, spread, target)

            ' PW：按特征值给全部主成分加权
            Dim weights(pcs.Rank - 1) As Double
            Dim reference As Double = If(pcs.Rank > 0 AndAlso pcs.Eigenvalues.Length > 0,
                                         std.Max(pcs.Eigenvalues(0), 1.0E-300), 1.0)

            For i As Integer = 0 To pcs.Rank - 1
                weights(i) = std.Pow(std.Max(pcs.Eigenvalues(i), 0.0) / reference, options.PwGamma)
            Next

            Dim pwLoadings As Double()() = RectangularArray.Matrix(Of Double)(pcs.Rank, nGenes)

            For i As Integer = 0 To pcs.Rank - 1
                Dim scale As Double = std.Sqrt(std.Max(weights(i), 0.0))

                For g As Integer = 0 To nGenes - 1
                    pwLoadings(i)(g) = scale * pcs.Loadings(i)(g)
                Next
            Next

            Dim gatePw As Double() = StrengthGate(zz, l0, mask, options.PwTop, options.GateZ)
            Dim pwResult As ShapedDesignResult = CpDesign(cpResult.L, fc, mask, pwLoadings,
                                                          options.PwLam, gatePw, spread, target)

            Return New ShapedDesignResult With {
                .L = pwResult.L,
                .AmplitudeCapped = amplitude.Capped,
                .CpCapped = cpResult.CpCapped,
                .PwCapped = pwResult.PwCapped
            }
        End Function

#Region "helpers"

        ''' <summary>把非有限的显著性换成 0（上游 <c>finite()</c>）。</summary>
        Private Function Finite(values As Double()(), nGenes As Integer, nPerts As Integer) As Double()()
            Dim result As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)

            For g As Integer = 0 To nGenes - 1
                For p As Integer = 0 To nPerts - 1
                    result(g)(p) = If(Double.IsFinite(values(g)(p)), values(g)(p), 0.0)
                Next
            Next

            Return result
        End Function

        Private Function Multiply(a As Double()(), b As Double()()) As Double()()
            Dim result(a.Length - 1)() As Double

            For i As Integer = 0 To a.Length - 1
                result(i) = New Double(a(i).Length - 1) {}

                For j As Integer = 0 To a(i).Length - 1
                    result(i)(j) = a(i)(j) * b(i)(j)
                Next
            Next

            Return result
        End Function

        ''' <summary><c>Vᵀ(V·D)</c>：把偏离投影到载荷张成的子空间。</summary>
        Private Function Project(vb As Double()(), values As Double()()) As Double()()
            Dim nComponents As Integer = If(vb Is Nothing, 0, vb.Length)
            Dim nGenes As Integer = values.Length
            Dim nPerts As Integer = If(nGenes = 0, 0, values(0).Length)
            Dim result As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)

            If nComponents = 0 Then
                Return result
            End If

            Dim coefficients As Double()() = RectangularArray.Matrix(Of Double)(nComponents, nPerts)

            For c As Integer = 0 To nComponents - 1
                For p As Integer = 0 To nPerts - 1
                    Dim acc As Double = 0.0

                    For g As Integer = 0 To nGenes - 1
                        acc += vb(c)(g) * values(g)(p)
                    Next

                    coefficients(c)(p) = acc
                Next
            Next

            For g As Integer = 0 To nGenes - 1
                For p As Integer = 0 To nPerts - 1
                    Dim acc As Double = 0.0

                    For c As Integer = 0 To nComponents - 1
                        acc += vb(c)(g) * coefficients(c)(p)
                    Next

                    result(g)(p) = acc
                Next
            Next

            Return result
        End Function

        Private Function SliceRows(values As Double()(), from As Integer, [to] As Integer) As Double()()
            Dim count As Integer = std.Max([to] - from, 0)
            Dim result(count - 1)() As Double

            For i As Integer = 0 To count - 1
                result(i) = DirectCast(values(from + i).Clone(), Double())
            Next

            Return result
        End Function

#End Region

    End Module
End Namespace
