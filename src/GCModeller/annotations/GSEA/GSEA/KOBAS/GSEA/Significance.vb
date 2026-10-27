Imports std = System.Math

Namespace KOBAS

    ''' <summary>
    ''' 富集显著性评估（KOBAS <c>gsea.nominal_p</c> / <c>normalized</c> / <c>fdr_cal</c> 的移植）。
    ''' </summary>
    ''' <remarks>
    ''' 与 KOBAS 原实现的差异：
    ''' <list type="bullet">
    ''' <item>KOBAS 在零分布某一侧样本为空时会出现除零；这里在分母为 0 时把该侧的比例记 1，
    ''' 并把归一化因子的兜底值取 1，保证输出永远为有限值。</item>
    ''' <item>FDR 使用 KOBAS 的「同号计数比」定义（<c>top / down</c>）并截断到 1，
    ''' 与 R 中基于 BH 的 q 值不是同一口径，请按需选用。</item>
    ''' </list>
    ''' </remarks>
    Public Module Significance

        ''' <summary>
        ''' 计算每个基因集的富集分数名义 p 值。
        ''' </summary>
        ''' <param name="es">观测到的富集分数（按基因集顺序）。</param>
        ''' <param name="nulls">零分布 <c>nulls[geneSet][permutation]</c>。</param>
        ''' <returns>与 <paramref name="es"/> 等长的名义 p 值。</returns>
        Public Function NominalP(es As Double(), nulls As Double()()) As Double()
            Dim pvalues As Double() = New Double(es.Length - 1) {}

            For s As Integer = 0 To es.Length - 1
                Dim observed As Double = es(s)
                Dim column As Double() = nulls(s)
                Dim numerator As Integer = 0
                Dim denominator As Integer = 0

                For Each value As Double In column
                    If observed >= 0 Then
                        If value >= observed Then
                            numerator += 1
                        End If
                        If value >= 0 Then
                            denominator += 1
                        End If
                    Else
                        If value <= observed Then
                            numerator += 1
                        End If
                        If value <= 0 Then
                            denominator += 1
                        End If
                    End If
                Next

                pvalues(s) = If(denominator = 0, 1.0, numerator / CDbl(denominator))
            Next

            Return pvalues
        End Function

        ''' <summary>
        ''' 把富集分数归一化为 NES（以及同时归一化的零分布）。
        ''' </summary>
        ''' <param name="es">观测到的富集分数。</param>
        ''' <param name="nulls">零分布 <c>nulls[geneSet][permutation]</c>。</param>
        ''' <returns>
        ''' 元组：<c>NES</c> 为归一化富集分数数组，<c>Null</c> 为归一化之后的零分布。
        ''' </returns>
        Public Function Normalize(es As Double(), nulls As Double()()) As (NES As Double(), Null As Double()())
            Dim nes As Double() = New Double(es.Length - 1) {}
            Dim nesNull As Double()() = New Double(es.Length - 1)() {}

            For s As Integer = 0 To es.Length - 1
                Dim column As Double() = nulls(s)
                Dim positiveSum As Double = 0
                Dim positiveCount As Integer = 0
                Dim negativeSum As Double = 0
                Dim negativeCount As Integer = 0

                For Each value As Double In column
                    If value >= 0 Then
                        positiveSum += value
                        positiveCount += 1
                    Else
                        negativeSum += std.Abs(value)
                        negativeCount += 1
                    End If
                Next

                Dim meanPositive As Double = If(positiveCount = 0, 1.0, positiveSum / positiveCount)
                Dim meanNegative As Double = If(negativeCount = 0, 1.0, negativeSum / negativeCount)

                If meanPositive = 0 Then
                    meanPositive = 1.0
                End If
                If meanNegative = 0 Then
                    meanNegative = 1.0
                End If

                nesNull(s) = New Double(column.Length - 1) {}

                For p As Integer = 0 To column.Length - 1
                    nesNull(s)(p) = If(column(p) >= 0, column(p) / meanPositive, column(p) / meanNegative)
                Next

                nes(s) = If(es(s) >= 0, es(s) / meanPositive, es(s) / meanNegative)
            Next

            Return (nes, nesNull)
        End Function

        ''' <summary>
        ''' 按 KOBAS 的同号计数比计算 FDR。
        ''' </summary>
        ''' <param name="nes">观测到的 NES（按基因集顺序）。</param>
        ''' <param name="nesNull">归一化后的零分布 <c>nesNull[geneSet][permutation]</c>。</param>
        ''' <returns>与 <paramref name="nes"/> 等长且不超过 1 的 FDR。</returns>
        Public Function Fdr(nes As Double(), nesNull As Double()()) As Double()
            Dim nullPositive As Integer = 0
            Dim nullNegative As Integer = 0

            For s As Integer = 0 To nesNull.Length - 1
                For Each value As Double In nesNull(s)
                    If value >= 0 Then
                        nullPositive += 1
                    Else
                        nullNegative += 1
                    End If
                Next
            Next

            Dim observedPositive As Integer = 0
            Dim observedNegative As Integer = 0

            For Each value As Double In nes
                If value >= 0 Then
                    observedPositive += 1
                Else
                    observedNegative += 1
                End If
            Next

            Dim adjusted As Double() = New Double(nes.Length - 1) {}

            For s As Integer = 0 To nes.Length - 1
                Dim observed As Double = nes(s)
                Dim topCount As Integer = 0
                Dim downCount As Integer = 0

                For k As Integer = 0 To nesNull.Length - 1
                    For Each value As Double In nesNull(k)
                        If observed >= 0 Then
                            If value >= observed Then
                                topCount += 1
                            End If
                        Else
                            If value <= observed Then
                                topCount += 1
                            End If
                        End If
                    Next
                Next

                For Each value As Double In nes
                    If observed >= 0 Then
                        If value >= observed Then
                            downCount += 1
                        End If
                    Else
                        If value <= observed Then
                            downCount += 1
                        End If
                    End If
                Next

                Dim top As Double = If(observed >= 0,
                                       If(nullPositive = 0, 0, topCount / CDbl(nullPositive)),
                                       If(nullNegative = 0, 0, topCount / CDbl(nullNegative)))
                Dim down As Double = If(observed >= 0,
                                        If(observedPositive = 0, 0, downCount / CDbl(observedPositive)),
                                        If(observedNegative = 0, 0, downCount / CDbl(observedNegative)))

                adjusted(s) = If(down = 0, 0.0, std.Min(1.0, top / down))
            Next

            Return adjusted
        End Function
    End Module
End Namespace
