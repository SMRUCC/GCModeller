Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Alignment
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Model

Namespace Bayesian.Inference

    ''' <summary>
    ''' 采样器的配置
    ''' </summary>
    Public Class GibbsSamplerOptions

        Private _iterations As Integer = 2000
        Private _burnIn As Integer = 200
        Private _runs As Integer = 5
        Private _seed As Integer = 0
        Private _sampleEvery As Integer = 1

        ''' <summary>
        ''' 每一次独立运行的抽样轮数
        ''' </summary>
        Public Property Iterations As Integer
            Get
                Return _iterations
            End Get
            Set(value As Integer)
                _iterations = Math.Max(1, value)
            End Set
        End Property

        ''' <summary>
        ''' 前若干轮视为 burn-in，不计入频率统计
        ''' </summary>
        Public Property BurnIn As Integer
            Get
                Return _burnIn
            End Get
            Set(value As Integer)
                _burnIn = Math.Max(0, value)
            End Set
        End Property

        ''' <summary>
        ''' 独立运行次数(论文对每种情形做 5 次独立采样并取平均)
        ''' </summary>
        Public Property Runs As Integer
            Get
                Return _runs
            End Get
            Set(value As Integer)
                _runs = Math.Max(1, value)
            End Set
        End Property

        ''' <summary>
        ''' 随机数种子，固定之后结果完全可复现
        ''' </summary>
        Public Property RandomSeed As Integer
            Get
                Return _seed
            End Get
            Set(value As Integer)
                _seed = value
            End Set
        End Property

        ''' <summary>
        ''' 每多少轮记录一次频率(>1 时可以降低统计开销)
        ''' </summary>
        Public Property SampleEvery As Integer
            Get
                Return _sampleEvery
            End Get
            Set(value As Integer)
                _sampleEvery = Math.Max(1, value)
            End Set
        End Property

        ''' <summary>
        ''' 每一行所属的基因组分组下标；为 <c>Nothing</c> 的时候按照 <see cref="JointAlignment.Names"/> 自动分组
        ''' </summary>
        Public Property Groups As Integer() = Nothing
    End Class

    ''' <summary>
    ''' 互作分配的 Gibbs 采样器。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 论文第 4 节给出的采样流程：
    ''' </para>
    ''' <list type="number">
    ''' <item><description>
    ''' 以正比于 <c>r_g</c> 的概率抽取一个基因组 <c>g</c>，其中 <c>r_g</c> 是该基因组之内
    ''' 可交换的成员对数目；
    ''' </description></item>
    ''' <item><description>
    ''' 在该基因组之内随机取两个成员，在「当前分配」与「互换伙伴之后的分配」之间采样；
    ''' </description></item>
    ''' <item><description>
    ''' 统计每一对成员的配对频率 <c>f(x, y)</c>，跨多次独立运行取平均。
    ''' </description></item>
    ''' </list>
    ''' <para>
    ''' 采样使用 Metropolis 接受准则：接受概率为
    ''' <c>min(1, P(D|a') / P(D|a))</c>。由于交换是一次对合(involution)，
    ''' 拒绝的时候再交换一次即可精确还原状态。
    ''' </para>
    ''' <para>
    ''' <b>性能</b>：一次交换只会改变跨家族的列对计数，而依赖树的重建是 <c>O(L²)</c>，
    ''' 因此单次移动的代价与列数的平方成正比，而不是与「列数平方 × 序列数」成正比。
    ''' </para>
    ''' </remarks>
    Public Class GibbsSampler

        Private ReadOnly _alignment As JointAlignment
        Private ReadOnly _modelOptions As PpiModelOptions
        Private ReadOnly _options As GibbsSamplerOptions

        ''' <summary>
        ''' 由联合比对构造采样器
        ''' </summary>
        Public Sub New(alignment As JointAlignment,
                       Optional modelOptions As PpiModelOptions = Nothing,
                       Optional options As GibbsSamplerOptions = Nothing)

            If alignment Is Nothing Then
                Throw New ArgumentNullException(NameOf(alignment))
            End If

            _alignment = alignment
            _modelOptions = If(modelOptions, New PpiModelOptions())
            _options = If(options, New GibbsSamplerOptions())
        End Sub

        ''' <summary>
        ''' 执行采样
        ''' </summary>
        Public Function Run() As InferenceResult
            Dim rows As Integer = _alignment.Rows
            Dim groups As Integer() = ResolveGroups()
            Dim frequency As Double(,) = New Double(rows - 1, rows - 1) {}
            Dim members As List(Of Integer)() = GroupMembers(groups, rows)
            Dim rng As New Random(_options.RandomSeed)
            Dim candidates As Integer() = Enumerable.Range(0, members.Length) _
                .Where(Function(g) members(g).Count > 1) _
                .ToArray()
            Dim logEvidence As Double() = New Double(_options.Runs - 1) {}
            Dim statistics As Integer = 0

            If candidates.Length = 0 Then
                DebugAudit.Check(False,
                                 "no genome contains more than one candidate pair; the sampler has nothing to permute")
            End If

            For trial As Integer = 0 To _options.Runs - 1
                Dim model As New PpiBayesianModel(_alignment, _modelOptions)
                Dim current As Double = model.LogEvidence()

                For iteration As Integer = 1 To _options.Iterations
                    If candidates.Length = 0 Then
                        Exit For
                    End If

                    Dim genome As Integer = candidates(rng.Next(candidates.Length))
                    Dim pool As List(Of Integer) = members(genome)

                    If pool.Count < 2 Then
                        Continue For
                    End If

                    Dim first As Integer = pool(rng.Next(pool.Count))
                    Dim second As Integer = pool(rng.Next(pool.Count))

                    If first = second Then
                        Continue For
                    End If

                    ' 两行的家族 A 段相同意味着它们其实是同一个成员，交换不会有任何效果
                    If model.Alignment.SameBlockA(first, second) Then
                        Continue For
                    End If

                    Call model.ApplySwap(first, second)

                    Dim proposed As Double = model.LogEvidence()
                    Dim accept As Boolean

                    If proposed >= current Then
                        accept = True
                    ElseIf Double.IsNegativeInfinity(proposed) Then
                        accept = False
                    Else
                        accept = rng.NextDouble() < Math.Exp(proposed - current)
                    End If

                    If accept Then
                        current = proposed
                    Else
                        ' 交换是对合操作，再交换一次即可精确还原
                        Call model.ApplySwap(first, second)
                    End If

                    If iteration > _options.BurnIn AndAlso (iteration - _options.BurnIn) Mod _options.SampleEvery = 0 Then
                        Dim state As JointAlignment = model.Alignment

                        For Each g As Integer In Enumerable.Range(0, members.Length)
                            Dim list As List(Of Integer) = members(g)

                            For Each a As Integer In list
                                For Each b As Integer In list
                                    If SamePartner(state, a, b, _alignment) Then
                                        frequency(a, b) += 1.0
                                    End If
                                Next
                            Next
                        Next

                        statistics += 1
                    End If
                Next

                logEvidence(trial) = current
            Next

            Return BuildResult(frequency, groups, statistics, logEvidence)
        End Function

        ''' <summary>
        ''' 判断在新状态之中，行 <paramref name="a"/> 的 A 段是否与行 <paramref name="b"/> 的 B 段配对
        ''' </summary>
        Private Shared Function SamePartner(state As JointAlignment, a As Integer, b As Integer, template As JointAlignment) As Boolean
            For c As Integer = 0 To template.LengthB - 1
                If state(a, template.LengthA + c) <> template(b, template.LengthA + c) Then
                    Return False
                End If
            Next

            Return True
        End Function

        Private Function BuildResult(frequency As Double(,), groups As Integer(), statistics As Integer, logEvidence As Double()) As InferenceResult
            Dim rows As Integer = _alignment.Rows
            Dim cognate As Double() = New Double(rows - 1) {}
            Dim partners As List(Of InferenceResult.Partner)() = Enumerable _
                .Range(0, rows) _
                .Select(Function(i) New List(Of InferenceResult.Partner)) _
                .ToArray()
            Dim names As String() = _alignment.Names

            For r As Integer = 0 To rows - 1
                Dim list As New List(Of InferenceResult.Partner)
                Dim total As Double = 0.0

                For s As Integer = 0 To rows - 1
                    If groups(s) <> groups(r) Then
                        Continue For
                    End If

                    total += frequency(r, s)
                Next

                For s As Integer = 0 To rows - 1
                    If groups(s) <> groups(r) Then
                        Continue For
                    End If

                    Dim p As Double = If(total > 0, frequency(r, s) / total, 0.0)

                    Call list.Add(New InferenceResult.Partner With {
                        .ProteinA = _alignment.NameOfA(r),
                        .ProteinB = _alignment.NameOfB(s),
                        .Genome = names(r),
                        .Frequency = p
                    })
                Next

                Dim ordered As List(Of InferenceResult.Partner) = list.OrderByDescending(Function(x) x.Frequency).ToList()

                For k As Integer = 0 To ordered.Count - 1
                    ordered(k).Rank = k + 1
                Next

                partners(r) = ordered
                cognate(r) = If(total > 0, frequency(r, r) / total, 0.0)
            Next

            Return New InferenceResult With {
                .CognateProbability = cognate,
                .Partners = partners,
                .Genomes = _alignment.Names,
                .NamesA = Enumerable.Range(0, rows).Select(Function(i) _alignment.NameOfA(i)).ToArray(),
                .NamesB = Enumerable.Range(0, rows).Select(Function(i) _alignment.NameOfB(i)).ToArray(),
                .Runs = _options.Runs,
                .Iterations = _options.Iterations,
                .RunLogEvidence = logEvidence,
                .GenomeCount = groups.Distinct().Count()
            }
        End Function

        ''' <summary>
        ''' 解析行到基因组分组的映射
        ''' </summary>
        Private Function ResolveGroups() As Integer()
            If _options.Groups IsNot Nothing Then
                If _options.Groups.Length <> _alignment.Rows Then
                    Throw New ArgumentException(
                        $"the group array must contain {_alignment.Rows} elements but got {_options.Groups.Length}")
                End If

                Return _options.Groups
            End If

            Dim index As New Dictionary(Of String, Integer)(StringComparer.Ordinal)
            Dim groups As Integer() = New Integer(_alignment.Rows - 1) {}

            For r As Integer = 0 To _alignment.Rows - 1
                Dim name As String = _alignment.Names(r)
                Dim id As Integer = 0

                If Not index.TryGetValue(name, id) Then
                    id = index.Count
                    index(name) = id
                End If

                groups(r) = id
            Next

            Return groups
        End Function

        Private Shared Function GroupMembers(groups As Integer(), rows As Integer) As List(Of Integer)()
            Dim count As Integer = If(groups.Length = 0, 0, groups.Max() + 1)
            Dim members As List(Of Integer)() = Enumerable _
                .Range(0, count) _
                .Select(Function(i) New List(Of Integer)) _
                .ToArray()

            For r As Integer = 0 To rows - 1
                Call members(groups(r)).Add(r)
            Next

            Return members
        End Function
    End Class
End Namespace
