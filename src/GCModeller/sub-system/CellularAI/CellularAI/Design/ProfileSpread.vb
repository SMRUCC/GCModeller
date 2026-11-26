Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports std = System.Math

Namespace Design

    ''' <summary>
    ''' 剖面离散度：把"基因比例矩阵"与"整数总量矩阵"折算成同一个标量目标，
    ''' 供设计层求幅度与边距、供评分层算 <c>spread_B</c>。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 度量的是<b>剖面的形状</b>而不是绝对值：先按 <c>log1p(TS·fraction)</c> 变换（<c>TS = 5e4</c> 是
    ''' 上游固定的剖面归一化常数），再减去"逐基因的面板均值"，最后求平方和。
    ''' 因此整体放大或缩小比例不会改变它，只有基因之间的相对关系改变才会。
    ''' </para>
    ''' <para>
    ''' <b>自身靶必须屏蔽</b>：靶基因的效应是"敲低本身"，量级比其余基因高一个数量级，
    ''' 若不屏蔽，离散度会退化成"敲低强度"的度量，设计层会一直去追一个与生物学响应无关的目标。
    ''' </para>
    ''' <para>
    ''' <b>为什么评分层要复用这里</b>：设计的目标就是"让合成出来的整数总量的离散度等于信用"，
    ''' 与评分层的 <c>spread_B</c> 必须是同一个函数；两份实现一旦漂移，边距求解就会收敛到一个
    ''' 评分层不认可的点上。
    ''' </para>
    ''' </remarks>
    Public Module ProfileSpread

        ''' <summary>剖面归一化常数（上游 <c>scoring.TS</c>）。</summary>
        Public Const TS As Double = 50000.0

        ''' <summary>
        ''' "可用"掩码：<c>True</c> 表示该位置参与离散度计算；每个扰动的自身靶基因处为 <c>False</c>。
        ''' </summary>
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

        ''' <summary>逐扰动的评分权重 <c>w_p = 1/G_p</c>（<c>G_p</c> 为屏蔽自身靶后的基因数）。</summary>
        Public Function Weights(nGenes As Integer, tidx As Integer()) As Double()
            Dim result(If(tidx Is Nothing, -1, tidx.Length - 1)) As Double

            For p As Integer = 0 To result.Length - 1
                Dim count As Integer = nGenes

                If tidx(p) >= 0 Then
                    count -= 1
                End If

                result(p) = 1.0 / std.Max(count, 1)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 比例矩阵的离散度：<c>Σ (mask ? (log1p(TS·fr) − 逐基因均值)² : 0)</c>。
        ''' </summary>
        ''' <param name="fractions">基因×扰动的比例（每列和为 1）</param>
        ''' <param name="mask">"可用"掩码（基因×扰动）</param>
        Public Function FromFractions(fractions As Double()(), mask As Boolean()()) As Double
            Dim nGenes As Integer = fractions.Length
            Dim nPerts As Integer = If(nGenes = 0, 0, fractions(0).Length)
            Dim bm As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)

            For g As Integer = 0 To nGenes - 1
                For p As Integer = 0 To nPerts - 1
                    bm(g)(p) = std.Log(1.0 + TS * fractions(g)(p))
                Next
            Next

            Dim total As Double = 0.0

            For g As Integer = 0 To nGenes - 1
                Dim sum As Double = 0.0
                Dim count As Integer = 0

                For p As Integer = 0 To nPerts - 1
                    If mask(g)(p) Then
                        sum += bm(g)(p)
                        count += 1
                    End If
                Next

                Dim mean As Double = If(count > 0, sum / count, 0.0)

                For p As Integer = 0 To nPerts - 1
                    If mask(g)(p) Then
                        Dim d As Double = bm(g)(p) - mean

                        total += d * d
                    End If
                Next
            Next

            Return total
        End Function

        ''' <summary>
        ''' 构造 <c>L ↦ 离散度</c> 的函数：<c>fr = fc·2^L</c>，按列归一化后求 <see cref="FromFractions"/>。
        ''' </summary>
        ''' <param name="fc">对照基因比例（长度 G）</param>
        ''' <param name="mask">"可用"掩码</param>
        ''' <param name="log2Max">设计因子的绝对值上限（<c>guards.log2_max</c>）</param>
        Public Function Make(fc As Double(), mask As Boolean()(), log2Max As Double) As Func(Of Double()(), Double)
            Return Function(l2 As Double()()) As Double
                       Dim nGenes As Integer = fc.Length
                       Dim nPerts As Integer = If(nGenes = 0, 0, l2(0).Length)
                       Dim fractions As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)
                       Dim columnSums(nPerts - 1) As Double

                       For g As Integer = 0 To nGenes - 1
                           For p As Integer = 0 To nPerts - 1
                               Dim exponent As Double = std.Max(std.Min(l2(g)(p), log2Max), -log2Max)
                               Dim value As Double = fc(g) * std.Pow(2.0, exponent)

                               fractions(g)(p) = If(Double.IsFinite(value), value, 0.0)
                               columnSums(p) += fractions(g)(p)
                           Next
                       Next

                       For p As Integer = 0 To nPerts - 1
                           Dim total As Double = columnSums(p)

                           If total <= 0.0 Then
                               Continue For
                           End If

                           For g As Integer = 0 To nGenes - 1
                               fractions(g)(p) /= total
                           Next
                       Next

                       Return FromFractions(fractions, mask)
                   End Function
        End Function

        ''' <summary>
        ''' 整数总量的离散度（评分层的 <c>spread_B</c>）：按 <c>w_p</c> 加权的
        ''' <c>Σ_p w_p (log1p(TS·T_p/S_p) − 逐基因加权均值)²</c>。
        ''' </summary>
        ''' <param name="totals">扰动×基因的整数总量</param>
        ''' <param name="mask">"可用"掩码（基因×扰动）</param>
        ''' <param name="w">逐扰动的权重</param>
        Public Function FromTotals(totals As Double()(), mask As Boolean()(), w As Double()) As Double
            Dim nPerts As Integer = totals.Length
            Dim nGenes As Integer = If(nPerts = 0, 0, totals(0).Length)
            Dim b As Double()() = RectangularArray.Matrix(Of Double)(nPerts, nGenes)

            For p As Integer = 0 To nPerts - 1
                Dim total As Double = 0.0

                For g As Integer = 0 To nGenes - 1
                    total += totals(p)(g)
                Next

                If total <= 0.0 Then
                    total = 1.0
                End If

                For g As Integer = 0 To nGenes - 1
                    b(p)(g) = std.Log(1.0 + TS * totals(p)(g) / total)
                Next
            Next

            ' 逐基因的加权均值
            Dim weightSum(nGenes - 1) As Double
            Dim weighted(nGenes - 1) As Double

            For p As Integer = 0 To nPerts - 1
                For g As Integer = 0 To nGenes - 1
                    If mask(g)(p) Then
                        weightSum(g) += w(p)
                        weighted(g) += w(p) * b(p)(g)
                    End If
                Next
            Next

            Dim mean(nGenes - 1) As Double

            For g As Integer = 0 To nGenes - 1
                mean(g) = If(weightSum(g) > 0.0, weighted(g) / weightSum(g), 0.0)
            Next

            Dim spread As Double = 0.0

            For p As Integer = 0 To nPerts - 1
                For g As Integer = 0 To nGenes - 1
                    If mask(g)(p) Then
                        Dim d As Double = b(p)(g) - mean(g)

                        spread += w(p) * d * d
                    End If
                Next
            Next

            Return spread
        End Function

        ''' <summary>把整数总量矩阵转成 <c>Double</c>（避免每条断言都做一次转换）。</summary>
        Public Function ToDouble(totals As Integer()()) As Double()()
            Dim result(totals.Length - 1)() As Double

            For p As Integer = 0 To totals.Length - 1
                result(p) = New Double(totals(p).Length - 1) {}

                For g As Integer = 0 To totals(p).Length - 1
                    result(p)(g) = totals(p)(g)
                Next
            Next

            Return result
        End Function

    End Module
End Namespace
