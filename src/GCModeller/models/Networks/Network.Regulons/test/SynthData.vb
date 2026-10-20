Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Model.Network.Regulons.LinearRegression
Imports std = System.Math

''' <summary>
''' 用于 demo 测试的人工合成数据：答案已知的基因调控网络
''' </summary>
Module SynthData

    ''' <summary>
    ''' 合成数据之中的一条真实存在的调控关系
    ''' </summary>
    Public Class TruthEdge
        ''' <summary>
        ''' 转录因子(调控因子)的基因编号
        ''' </summary>
        ''' <returns></returns>
        Public Property regulator As String
        ''' <summary>
        ''' 靶基因的编号
        ''' </summary>
        ''' <returns></returns>
        Public Property target As String
        ''' <summary>
        ''' 真实的回归系数
        ''' </summary>
        ''' <returns></returns>
        Public Property weight As Double

        ''' <summary>
        ''' 这条调控关系的唯一编号
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property UID As String
            Get
                Return $"{regulator}->{target}"
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{UID} = {weight.ToString("F3")}"
        End Function
    End Class

    ''' <summary>
    ''' 从 Gaussian 分布之中采样一个随机数(Box-Muller 变换)
    ''' </summary>
    ''' <param name="rng"></param>
    ''' <returns></returns>
    Public Function Gauss(rng As Random) As Double
        Dim u1 As Double = rng.NextDouble()

        While u1 <= 0
            u1 = rng.NextDouble()
        End While

        Return std.Sqrt(-2 * std.Log(u1)) * std.Cos(2 * std.PI * rng.NextDouble())
    End Function

    ''' <summary>
    ''' 使用 Fisher-Yates 算法打乱一个数组
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="rng"></param>
    ''' <returns></returns>
    Public Function Shuffle(source As String(), rng As Random) As String()
        Dim buf As String() = DirectCast(source.Clone(), String())

        For i As Integer = buf.Length - 1 To 1 Step -1
            Dim j As Integer = rng.Next(i + 1)
            Dim tmp As String = buf(i)

            buf(i) = buf(j)
            buf(j) = tmp
        Next

        Return buf
    End Function

    ''' <summary>
    ''' 生成一个答案已知的调控网络的表达矩阵
    ''' 
    ''' 转录因子(TF)的表达量独立采样自标准正态分布；每一个靶基因的表达量则是
    ''' 若干个转录因子的线性组合再加上高斯观测噪声：
    ''' 
    ''' ```
    ''' expr(target) = const + sum(w_i * expr(TF_i)) + noise
    ''' ```
    ''' 
    ''' 靶基因与靶基因之间没有直接的调控关系，它们之间的相关性全部来自于共调控
    ''' (即两个靶基因共享了同一个转录因子)。
    ''' </summary>
    ''' <param name="nTf">转录因子的数量</param>
    ''' <param name="nTarget">靶基因的数量</param>
    ''' <param name="nSample">样本数量</param>
    ''' <param name="noise">观测噪声的标准差</param>
    ''' <param name="seed">随机数种子，相同的种子会生成完全相同的数据</param>
    ''' <param name="intercept">模型之中的常数项</param>
    ''' <returns>表达矩阵以及真实的调控关系列表</returns>
    Public Function BuildNetwork(nTf As Integer,
                                 nTarget As Integer,
                                 nSample As Integer,
                                 Optional noise As Double = 0.15,
                                 Optional seed As Integer = 42,
                                 Optional intercept As Double = 1.0) As (matrix As Matrix, truth As TruthEdge())

        Dim rng As New Random(seed)
        Dim tfNames As String() = Enumerable.Range(1, nTf).Select(Function(i) $"TF_{i}").ToArray
        Dim targetNames As String() = Enumerable.Range(1, nTarget).Select(Function(i) $"G_{i}").ToArray
        Dim sampleNames As String() = Enumerable.Range(1, nSample).Select(Function(i) $"S{i}").ToArray
        Dim rows As New List(Of DataFrameRow)
        Dim truth As New List(Of TruthEdge)
        Dim tfExpr As New Dictionary(Of String, Double())
        Dim weights As New Dictionary(Of String, Double)

        ' 1) 转录因子的表达量：独立的标准正态分布
        For Each name As String In tfNames
            Dim v As Double() = New Double(nSample - 1) {}

            For s As Integer = 0 To nSample - 1
                v(s) = Gauss(rng)
            Next

            tfExpr(name) = v

            rows.Add(New DataFrameRow With {
                .geneID = name,
                .experiments = v
            })
        Next

        ' 2) 靶基因：随机挑选 1~3 个转录因子进行线性组合
        For Each name As String In targetNames
            Dim picks As String() = Shuffle(tfNames, rng).Take(1 + rng.Next(3)).ToArray
            Dim v As Double() = New Double(nSample - 1) {}

            For s As Integer = 0 To nSample - 1
                Dim sum As Double = intercept

                For Each tf As String In picks
                    sum += tfExpr(tf)(s) * WeightOf(weights, $"{tf}->{name}", rng)
                Next

                v(s) = sum + noise * Gauss(rng)
            Next

            rows.Add(New DataFrameRow With {
                .geneID = name,
                .experiments = v
            })

            For Each tf As String In picks
                truth.Add(New TruthEdge With {
                    .regulator = tf,
                    .target = name,
                    .weight = weights($"{tf}->{name}")
                })
            Next
        Next

        Dim matrix As New Matrix With {
            .tag = "synth",
            .sampleID = sampleNames,
            .expression = rows.ToArray
        }

        Return (matrix, truth.ToArray)
    End Function

    ''' <summary>
    ''' 评估推断出来的调控网络：按照回归系数绝对值取最强的若干条边，
    ''' 然后与真实的调控关系做比对，计算 precision / recall / F1。
    ''' </summary>
    ''' <param name="truth">真实的调控关系</param>
    ''' <param name="predicted">推断出来的调控边</param>
    ''' <param name="topM">
    ''' 参与评估的边的数量，小于零的时候取真实边的数量。
    ''' </param>
    ''' <returns></returns>
    Public Function Evaluate(truth As TruthEdge(),
                             predicted As IEnumerable(Of RegulationEdge),
                             Optional topM As Integer = -1) As (precision As Double, recall As Double, f1 As Double)

        Dim answer As New HashSet(Of String)(truth.Select(Function(e) e.UID))
        Dim size As Integer = If(topM > 0, topM, answer.Count)
        Dim top As RegulationEdge() = predicted _
            .OrderByDescending(Function(e) std.Abs(e.weight)) _
            .Take(size) _
            .ToArray
        Dim hits As Integer = top.Count(Function(e) answer.Contains(e.UID))
        Dim precision As Double = If(top.Length > 0, hits / top.Length, 0)
        Dim recall As Double = If(answer.Count > 0, hits / answer.Count, 0)
        Dim f1 As Double

        If precision + recall <= 0 Then
            f1 = 0
        Else
            f1 = 2 * precision * recall / (precision + recall)
        End If

        Return (precision, recall, f1)
    End Function

    ''' <summary>
    ''' 检查推断出来的边的权重符号与真实的调控作用方向是否一致的比例
    ''' </summary>
    ''' <param name="truth"></param>
    ''' <param name="predicted"></param>
    ''' <returns>符号一致的边在全部命中边之中的比例</returns>
    Public Function SignAccuracy(truth As TruthEdge(), predicted As IEnumerable(Of RegulationEdge)) As Double
        Dim answer As Dictionary(Of String, Double) = truth _
            .ToDictionary(Function(e) e.UID, Function(e) e.weight)
        Dim hits As Integer = 0
        Dim same As Integer = 0

        For Each edge As RegulationEdge In predicted
            If Not answer.ContainsKey(edge.UID) Then
                Continue For
            End If

            hits += 1

            If std.Sign(edge.weight) = std.Sign(answer(edge.UID)) Then
                same += 1
            End If
        Next

        Return If(hits = 0, 0, same / hits)
    End Function

    ''' <summary>
    ''' 生成(或者复用)一条调控关系之上的真实回归系数，
    ''' 权重的绝对值落在 0.5~1.2 之间，符号随机。
    ''' </summary>
    Private Function WeightOf(table As Dictionary(Of String, Double), key As String, rng As Random) As Double
        If Not table.ContainsKey(key) Then
            Dim sign As Double = If(rng.NextDouble() < 0.5, 1.0R, -1.0R)

            table(key) = sign * (0.5 + rng.NextDouble() * 0.7)
        End If

        Return table(key)
    End Function
End Module
