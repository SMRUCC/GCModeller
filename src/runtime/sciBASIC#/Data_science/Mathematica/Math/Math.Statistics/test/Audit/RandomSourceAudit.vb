Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.Math.RandomSources
Imports std = System.Math

''' <summary>
''' 随机数源的验收测试：MT19937 标准测试向量、可复现性、抽样与派生。
''' </summary>
Friend Module RandomSourceAudit

        Private Const Section As String = "random"

        ''' <summary>
        ''' MT19937 的公开测试向量（Matsumoto 参考实现，<c>genrand_int32()</c> 的前三个输出）
        ''' </summary>
        Private ReadOnly Vectors As (seed As Long, expected As Long())() = {
            (5489L, New Long() {3499211612L, 581869302L, 3890346734L}),
            (1L, New Long() {1791095845L, 4282876139L, 3093770124L})
        }

        Friend Function Run() As Integer
            Dim failures As Integer = 0

            ' ---- 标准向量 ----
            For Each vector In Vectors
                Dim source As New MersenneTwisterSource(vector.seed)

                For i As Integer = 0 To vector.expected.Length - 1
                    Dim actual As Long = source.NextUInt32()

                    failures += Assert($"MT19937 seed={vector.seed} output[{i}]",
                                       actual = vector.expected(i),
                                       $"expected {vector.expected(i)}, actual {actual}")
                Next
            Next

            ' ---- 同种子可复现 ----
            Dim a As New MersenneTwisterSource(12345L)
            Dim b As New MersenneTwisterSource(12345L)
            Dim reproducible As Boolean = True

            For i As Integer = 0 To 99
                If a.NextDouble() <> b.NextDouble() Then
                    reproducible = False
                    Exit For
                End If
            Next

            failures += Assert("same seed reproduces the same sequence", reproducible)
            failures += Assert("IsRepeatable is true", a.IsRepeatable)

            ' ---- 不同种子应产生不同序列 ----
            Dim c As New MersenneTwisterSource(12345L)
            Dim d As New MersenneTwisterSource(54321L)
            Dim anyDifference As Boolean = False

            For i As Integer = 0 To 9
                If c.NextDouble() <> d.NextDouble() Then
                    anyDifference = True
                    Exit For
                End If
            Next

            failures += Assert("different seeds produce different sequences", anyDifference)

            ' ---- NextDouble 落在 [0,1) ----
            Dim rng As New MersenneTwisterSource(7L)
            Dim inRange As Boolean = True
            Dim mean As Double = 0.0
            Dim samples As Integer = 20000

            For i As Integer = 0 To samples - 1
                Dim v As Double = rng.NextDouble()

                If v < 0.0 OrElse v >= 1.0 Then inRange = False
                mean += v
            Next

            failures += Assert("NextDouble in [0,1)", inRange)
            failures += AssertClose("NextDouble mean ~ 0.5", mean / samples, 0.5, 0.01, Section)

            ' ---- NextInt 边界 ----
            Dim intRng As New MersenneTwisterSource(11L)
            Dim intsValid As Boolean = True

            For i As Integer = 0 To 999
                Dim v As Integer = intRng.NextInt(7)

                If v < 0 OrElse v >= 7 Then intsValid = False
            Next

            failures += Assert("NextInt stays within [0,bound)", intsValid)

            ' ---- 排列 ----
            Dim permRng As New MersenneTwisterSource(13L)
            Dim permutation As Integer() = permRng.Permutation(10)
            Dim seen As New HashSet(Of Integer)

            For Each v As Integer In permutation
                Call seen.Add(v)
            Next

            failures += Assert("Permutation(10) is a bijection",
                               permutation.Length = 10 AndAlso seen.Count = 10 AndAlso permutation.All(Function(x) x >= 0 AndAlso x < 10))

            ' ---- 无放回抽样 ----
            Dim sampleRng As New MersenneTwisterSource(17L)
            Dim picked As Integer() = sampleRng.Sample(20, 5, replace:=False)
            Dim distinct As New HashSet(Of Integer)

            For Each v As Integer In picked
                Call distinct.Add(v)
            Next

            failures += Assert("Sample(replace:=False) returns distinct indices", picked.Length = 5 AndAlso distinct.Count = 5)

            ' ---- 加权抽取 ----
            Dim choiceRng As New MersenneTwisterSource(19L)
            Dim degenerate As Boolean = True

            For i As Integer = 0 To 99
                If choiceRng.Choice(3, New Double() {0.0, 1.0, 0.0}) <> 1 Then
                    degenerate = False
                    Exit For
                End If
            Next

            failures += Assert("Choice honours degenerate weights", degenerate)

            ' ---- 派生流 ----
            Dim parent As New MersenneTwisterSource(23L)
            Dim childA As IRandomSource = parent.Derive(101L)
            Dim childB As IRandomSource = parent.Derive(101L)
            Dim childC As IRandomSource = parent.Derive(202L)
            Dim derivedReproducible As Boolean = True
            Dim derivedDiffers As Boolean = False

            For i As Integer = 0 To 9
                Dim va As Double = childA.NextDouble()
                Dim vb As Double = childB.NextDouble()
                Dim vc As Double = childC.NextDouble()

                If va <> vb Then derivedReproducible = False
                If va <> vc Then derivedDiffers = True
            Next

            failures += Assert("Derive(key) is reproducible", derivedReproducible)
            failures += Assert("Derive with different keys differs", derivedDiffers)

            ' ---- 字符串种子 ----
            failures += Assert("RandomSeeds.FromString is stable",
                               RandomSeeds.FromString("TP53") = RandomSeeds.FromString("TP53"))
            failures += Assert("RandomSeeds.FromString differs by name",
                               RandomSeeds.FromString("TP53") <> RandomSeeds.FromString("TP54"))
            failures += Assert("RandomSeeds.FromString is never zero",
                               RandomSeeds.FromString("") <> 0L)

            Call DebugAudit.Mark($"{Section} audit completed", failures)

            Return failures
        End Function

End Module
