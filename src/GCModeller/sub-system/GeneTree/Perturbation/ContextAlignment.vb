Imports System.IO
Imports std = System.Math

''' <summary>
''' 表达状态上下文（context）的分位对齐。
''' 
''' 外部的 pseudobulk 上下文与模型训练矩阵往往来自不同的归一化管线（例如 <c>mean_counts</c>
''' 与 <c>log2(CPM+1)</c>），数值尺度和分布形状都不一样。直接把这种 context 注入树模型，
''' 会让大量特征落到决策树的极值叶子上（“平台化”），局部灵敏度趋近于零，
''' 于是扰动结果看起来“几乎没有变化”。
''' 
''' 这里采用**分位数对齐**（quantile normalisation）：
''' 
''' <list type="number">
''' <item><description>把 context 的取值取 <c>log1p</c> 之后在**全部基因之间**排序，得到它自己的经验分布；</description></item>
''' <item><description>每个基因的取值在该分布上的分位 <c>p_i</c> 表示“这个基因相对于其他基因有多高”；</description></item>
''' <item><description>再到**该基因自身**的训练种群分布上取同样的分位 <c>p_i</c>，作为对齐之后的表达值。</description></item>
''' </list>
''' 
''' 这样既保留了基因之间的相对高低关系（context 的全部信息），
''' 又保证每个基因的取值落回它自己在训练数据中观测到的范围之内。
''' </summary>
Public Module ExpressionContextAlignment

    ''' <summary>
    ''' 把上下文对齐到模型自身的表达尺度
    ''' </summary>
    ''' <param name="context">基因名 → 表达值（已经过滤掉模型中不存在的基因）</param>
    ''' <param name="population">基因主序的表达种群 <c>population(gene)(cell)</c></param>
    ''' <param name="geneIndex">基因名 → 下标（大小写不敏感）</param>
    ''' <returns>对齐之后的上下文（顺序与输入无关）</returns>
    Public Function Align(context As IDictionary(Of String, Double),
                          population As Double()(),
                          geneIndex As IDictionary(Of String, Integer)) As Dictionary(Of String, Double)

        Dim result As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

        If context Is Nothing OrElse context.Count = 0 Then
            Return result
        End If
        If population Is Nothing OrElse population.Length = 0 Then
            Throw New InvalidOperationException(
                "模型没有携带表达种群数据，无法对上下文做分位对齐：请先完成训练（Fit）或者加载已经训练好的模型")
        End If

        ' ---- 1. 源分布：context 自身的跨基因经验分布（log1p 域） ----
        Dim matched As New List(Of (gene As String, index As Integer, value As Double))
        Dim source As Double() = New Double(context.Count - 1) {}
        Dim n As Integer = 0

        For Each item As KeyValuePair(Of String, Double) In context
            Dim index As Integer = -1

            If Not geneIndex.TryGetValue(item.Key, index) Then
                Continue For
            End If
            If Double.IsNaN(item.Value) OrElse Double.IsInfinity(item.Value) Then
                Throw New InvalidDataException($"上下文基因 '{item.Key}' 的表达值 {item.Value} 不是有效数值")
            End If

            source(n) = std.Log(1.0R + std.Max(0.0R, item.Value))
            Call matched.Add((item.Key, index, source(n)))
            n += 1
        Next

        If n = 0 Then
            Return result
        End If
        If n < source.Length Then
            Call Array.Resize(source, n)
        End If

        Call Array.Sort(source)

        ' ---- 2. 每个基因按“自身种群分布的同分位”取目标值 ----
        Dim buffer As Double() = Nothing

        For Each item In matched
            Dim values As Double() = population(item.index)
            Dim mapped As Double

            If values Is Nothing OrElse values.Length = 0 Then
                mapped = std.Exp(item.value) - 1.0R
            Else
                If buffer Is Nothing OrElse buffer.Length <> values.Length Then
                    buffer = New Double(values.Length - 1) {}
                End If

                Dim count As Integer = 0

                For j As Integer = 0 To values.Length - 1
                    Dim v As Double = values(j)

                    If Double.IsNaN(v) OrElse Double.IsInfinity(v) Then
                        Continue For
                    End If

                    buffer(count) = v
                    count += 1
                Next

                If count = 0 Then
                    mapped = std.Exp(item.value) - 1.0R
                Else
                    Call Array.Sort(buffer, 0, count)

                    Dim min As Double = buffer(0)
                    Dim max As Double = buffer(count - 1)

                    If count = 1 OrElse max - min <= 1e-12R Then
                        ' 常量基因：整个种群都是同一个值，对齐之后保持这个值
                        mapped = min
                    Else
                        Dim p As Double = percentileOf(source, item.value)

                        mapped = quantileOf(buffer, count, p)
                    End If
                End If
            End If

            If mapped < 0.0R Then
                mapped = 0.0R
            End If

            result(item.gene) = mapped
        Next

        Return result
    End Function

    ''' <summary>
    ''' 在已升序排序的数组上取 <paramref name="x"/> 所对应的分位（用中位秩处理重复值）
    ''' </summary>
    ''' <param name="sorted">升序数组</param>
    ''' <param name="x">查询值（必须来自同一个数组）</param>
    ''' <returns>落在 (0, 1) 区间的分位</returns>
    Public Function percentileOf(sorted As Double(), x As Double) As Double
        Dim n As Integer = sorted.Length

        If n = 0 Then
            Return 0.0R
        ElseIf n = 1 Then
            Return 0.5R
        End If

        Dim idx As Integer = Array.BinarySearch(sorted, x)
        Dim rank As Double

        If idx >= 0 Then
            Dim first As Integer = idx
            Dim last As Integer = idx

            While first > 0 AndAlso sorted(first - 1) = x
                first -= 1
            End While
            While last < n - 1 AndAlso sorted(last + 1) = x
                last += 1
            End While

            rank = (first + last) / 2.0R + 0.5R
        Else
            rank = Not idx
        End If

        Dim p As Double = rank / n

        If p <= 0.0R Then
            Return 1.0R / n
        ElseIf p >= 1.0R Then
            Return 1.0R - 1.0R / n
        Else
            Return p
        End If
    End Function

    ''' <summary>
    ''' 在升序数组的前 <paramref name="count"/> 个元素上取 <paramref name="p"/> 分位（线性插值）
    ''' </summary>
    ''' <param name="sorted">升序数组</param>
    ''' <param name="count">有效元素数量</param>
    ''' <param name="p">分位，取值 [0, 1]</param>
    ''' <returns></returns>
    Public Function quantileOf(sorted As Double(), count As Integer, p As Double) As Double
        If count <= 0 Then
            Return 0.0R
        ElseIf count = 1 Then
            Return sorted(0)
        End If

        If p <= 0.0R Then
            Return sorted(0)
        ElseIf p >= 1.0R Then
            Return sorted(count - 1)
        End If

        Dim h As Double = p * (count - 1)
        Dim lo As Integer = CInt(std.Floor(h))
        Dim frac As Double = h - lo

        If lo >= count - 1 Then
            Return sorted(count - 1)
        Else
            Return sorted(lo) + frac * (sorted(lo + 1) - sorted(lo))
        End If
    End Function

End Module
