Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Alignment
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Statistics

Namespace Bayesian.Inference

    ''' <summary>
    ''' 系统发育置换检验：区分「真实的共进化信号」与「仅仅由系统发育关系导致的假信号」。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 论文第 5 节的思路非常巧妙：构造两组比对，它们包含<b>完全相同的蛋白序列</b>、
    ''' 也保留了<b>完全相同的系统发育关系</b>，唯一的差别是两组成员之间的配对方式 —
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description>
    ''' <b>true 比对</b>：按照真实的互作关系把两个家族的序列排在一起；
    ''' </description></item>
    ''' <item><description>
    ''' <b>false 比对</b>：在每一个基因组之内把两组的配对<b>互换</b>。
    ''' </description></item>
    ''' </list>
    ''' <para>
    ''' 如果 <c>R_ij</c> 的显著相关完全可以用系统发育解释，那么 true 与 false 两组比对的
    ''' <c>R_ij</c> 分布应当没有差别；反之 true 组的 <c>R_ij</c> 应当系统性地高于 false 组。
    ''' 因此这是一个非常干净的零假设检验。
    ''' </para>
    ''' <para>
    ''' 本实现接受一个「正交组(ortholog group)」标记数组：同一组的行互为直系同源。
    ''' 置换时把第 <c>g</c> 组的 B 段整体与第 <c>h</c> 组的 B 段交换(两组必须大小相同)，
    ''' 这样每个基因组之内的序列集合保持不变，只有配对关系发生了变化。
    ''' </para>
    ''' </remarks>
    Public Class PhylogeneticPermutation

        ''' <summary>
        ''' 检验结果
        ''' </summary>
        Public Class Result

            ''' <summary>
            ''' 真实比对之中采样得到的 <c>ln R</c> 分位数
            ''' </summary>
            Public Property TrueQuantiles As Double()
            ''' <summary>
            ''' 置换之后的比对之中采样得到的 <c>ln R</c> 分位数
            ''' </summary>
            Public Property NullQuantiles As Double()
            ''' <summary>
            ''' 两组分布之间的 Kolmogorov-Smirnov 统计量(取值 <c>[0, 1]</c>)
            ''' </summary>
            Public Property KSStatistic As Double
            ''' <summary>
            ''' 真实比对的平均 <c>ln R</c>
            ''' </summary>
            Public Property MeanTrue As Double
            ''' <summary>
            ''' 置换比对的平均 <c>ln R</c>
            ''' </summary>
            Public Property MeanNull As Double
            ''' <summary>
            ''' 实际参与的置换次数
            ''' </summary>
            Public Property Permutations As Integer

            ''' <summary>
            ''' 真实比对是否显著优于置换之后的比对
            ''' </summary>
            Public ReadOnly Property SignalDetected As Boolean
                Get
                    Return MeanTrue > MeanNull AndAlso KSStatistic > 0.1
                End Get
            End Property

            Public Overrides Function ToString() As String
                Return $"KS={KSStatistic:0.000}, mean(true)={MeanTrue:0.000}, mean(null)={MeanNull:0.000}, " &
                    $"signal={SignalDetected}, permutations={Permutations}"
            End Function
        End Class

        Private ReadOnly _alignment As JointAlignment
        Private ReadOnly _groups As Integer()
        Private ReadOnly _options As Model.PpiModelOptions

        ''' <summary>
        ''' 构造置换检验
        ''' </summary>
        ''' <param name="alignment">配对联合比对</param>
        ''' <param name="orthologGroups">每一行所属的正交组编号；为 <c>Nothing</c> 的时候每行自成一组成</param>
        ''' <param name="options">模型配置</param>
        Public Sub New(alignment As JointAlignment,
                       Optional orthologGroups As Integer() = Nothing,
                       Optional options As Model.PpiModelOptions = Nothing)

            If alignment Is Nothing Then
                Throw New ArgumentNullException(NameOf(alignment))
            End If

            _alignment = alignment
            _options = If(options, New Model.PpiModelOptions())

            If orthologGroups Is Nothing Then
                _groups = Enumerable.Range(0, alignment.Rows).ToArray()
            ElseIf orthologGroups.Length <> alignment.Rows Then
                Throw New ArgumentException(
                    $"the ortholog group array must contain {alignment.Rows} elements but got {orthologGroups.Length}",
                    NameOf(orthologGroups))
            Else
                _groups = orthologGroups
            End If
        End Sub

        ''' <summary>
        ''' 执行置换检验
        ''' </summary>
        ''' <param name="columnPairs">参与比较的列对个数</param>
        ''' <param name="permutations">最大置换次数</param>
        ''' <param name="quantiles">需要输出的分位数(例如 <c>{0.5, 0.9, 0.99}</c>)</param>
        Public Function Run(Optional columnPairs As Integer = 2000,
                            Optional permutations As Integer = 100,
                            Optional quantiles As Double() = Nothing) As Result

            Dim points As Double() = If(quantiles, {0.5, 0.9, 0.99})
            Dim truth As Double() = SampleLogR(_alignment, columnPairs)
            Dim nulls As New List(Of Double)
            Dim executed As Integer = 0
            Dim rng As New Random(_options.RandomSeed)

            For iteration As Integer = 1 To permutations
                Dim swapped As JointAlignment = BuildSwapped(rng)

                If swapped Is Nothing Then
                    Continue For
                End If

                Dim sample As Double() = SampleLogR(swapped, columnPairs)

                Call nulls.AddRange(sample)
                executed += 1

                If executed >= permutations Then
                    Exit For
                End If
            Next

            If nulls.Count = 0 Then
                DebugAudit.Check(False,
                                 "no ortholog group pair with equal size was found; the permutation test can not run")
            End If

            Dim nullValues As Double() = nulls.ToArray()

            Return New Result With {
                .TrueQuantiles = Quantiles(truth, points),
                .NullQuantiles = Quantiles(nullValues, points),
                .KSStatistic = KolmogorovSmirnov(truth, nullValues),
                .MeanTrue = If(truth.Length = 0, 0.0, truth.Average()),
                .MeanNull = If(nullValues.Length = 0, 0.0, nullValues.Average()),
                .Permutations = executed
            }
        End Function

        ''' <summary>
        ''' 构造「置换之后」的联合比对：把大小相同的一对正交组的 B 段整体互换。
        ''' </summary>
        ''' <returns>置换之后的比对；无法置换的时候返回 <c>Nothing</c></returns>
        Public Function BuildSwapped(rng As Random) As JointAlignment
            Dim members As Dictionary(Of Integer, List(Of Integer)) = MembersByGroup()
            Dim sizes As Integer() = members.Keys _
                .Where(Function(g) members(g).Count > 1) _
                .OrderBy(Function(g) g) _
                .ToArray()

            If sizes.Length < 2 Then
                Return Nothing
            End If

            For attempt As Integer = 1 To 64
                Dim left As Integer = sizes(rng.Next(sizes.Length))
                Dim right As Integer = sizes(rng.Next(sizes.Length))

                If left = right OrElse members(left).Count <> members(right).Count Then
                    Continue For
                End If

                Dim order As Integer() = Enumerable.Range(0, _alignment.Rows).ToArray()

                For k As Integer = 0 To members(left).Count - 1
                    order(members(left)(k)) = members(right)(k)
                    order(members(right)(k)) = members(left)(k)
                Next

                Return _alignment.WithPartnerOrder(order)
            Next

            Return Nothing
        End Function

        Private Function MembersByGroup() As Dictionary(Of Integer, List(Of Integer))
            Dim members As New Dictionary(Of Integer, List(Of Integer))

            For r As Integer = 0 To _alignment.Rows - 1
                Dim g As Integer = _groups(r)
                Dim list As List(Of Integer) = Nothing

                If Not members.TryGetValue(g, list) Then
                    list = New List(Of Integer)
                    members(g) = list
                End If

                Call list.Add(r)
            Next

            Return members
        End Function

        ''' <summary>
        ''' 在给定的比对之上随机采样若干跨家族列对的 <c>ln R</c>
        ''' </summary>
        Private Function SampleLogR(alignment As JointAlignment, count As Integer) As Double()
            Dim alphabet As ResidueAlphabet = If(_options.Alphabet, ResidueAlphabet.Standard21)
            Dim prior As New DirichletMultinomial(alphabet.Size, _options.PriorLambda)
            Dim total As Integer = alignment.LengthA * alignment.LengthB

            If total = 0 Then
                Return New Double() {}
            End If

            Dim rng As New Random(_options.RandomSeed + 7919)
            Dim take As Integer = Math.Min(count, total)
            Dim values As Double() = New Double(take - 1) {}
            Dim seen As New HashSet(Of Integer)

            For k As Integer = 0 To take - 1
                Dim index As Integer

                Do
                    index = rng.Next(total)
                Loop While seen.Contains(index) AndAlso seen.Count < total

                Call seen.Add(index)

                Dim i As Integer = index \ alignment.LengthB
                Dim j As Integer = alignment.LengthA + (index Mod alignment.LengthB)

                values(k) = prior.LogLikelihoodRatio(alignment, i, j)
            Next

            Return values
        End Function

        Private Shared Function Quantiles(values As Double(), points As Double()) As Double()
            If values Is Nothing OrElse values.Length = 0 Then
                Return New Double(points.Length - 1) {}
            End If

            Dim sorted As Double() = values.OrderBy(Function(v) v).ToArray()
            Dim result As Double() = New Double(points.Length - 1) {}

            For k As Integer = 0 To points.Length - 1
                Dim position As Integer = CInt(Math.Floor(points(k) * (sorted.Length - 1)))

                If position < 0 Then
                    position = 0
                ElseIf position >= sorted.Length Then
                    position = sorted.Length - 1
                End If

                result(k) = sorted(position)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 两组样本的 Kolmogorov-Smirnov 统计量
        ''' </summary>
        Public Shared Function KolmogorovSmirnov(a As Double(), b As Double()) As Double
            If a Is Nothing OrElse b Is Nothing OrElse a.Length = 0 OrElse b.Length = 0 Then
                Return 0.0
            End If

            Dim left As Double() = a.OrderBy(Function(v) v).ToArray()
            Dim right As Double() = b.OrderBy(Function(v) v).ToArray()
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim max As Double = 0.0

            While i < left.Length AndAlso j < right.Length
                Dim value As Double

                If left(i) <= right(j) Then
                    value = left(i)
                Else
                    value = right(j)
                End If

                While i < left.Length AndAlso left(i) <= value
                    i += 1
                End While
                While j < right.Length AndAlso right(j) <= value
                    j += 1
                End While

                Dim distance As Double = Math.Abs(i / CDbl(left.Length) - j / CDbl(right.Length))

                If distance > max Then
                    max = distance
                End If
            End While

            Return max
        End Function
    End Class
End Namespace
