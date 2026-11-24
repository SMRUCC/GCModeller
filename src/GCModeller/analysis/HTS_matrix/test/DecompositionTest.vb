Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports std = System.Math

''' <summary>
''' 合成菌群表达矩阵分解流程的回归测试。
''' </summary>
''' <remarks>
''' <para>
''' 用 5 个菌株的合成数据验证 <see cref="Decomposition"/>：每个菌株包含 90 个无同源的标记基因
''' 和 8 个跨菌株同源家族(每族每株一个拷贝)，并按已知的交叉分配比例
''' (<see cref="CrossMapRatio"/>) 生成 “并集定量” 的总矩阵。
''' </para>
''' <para>
''' 断言覆盖：矩阵数量与行集、非负性、因子-菌株指派的唯一性、标记基因的重构精度、
''' 跨菌株区分度、同源基因相对原始观测值的改善、家族/整体质量守恒、CSV 导出以及
''' 负值与基因ID冲突的报错路径。所有断言经由 <see cref="DebugAudit"/> 记录，
''' 报告写到 <c>report/community-decomposition-report.json</c>。
''' </para>
''' </remarks>
Friend Module DecompositionTest

    Private Const ReportName As String = "community-decomposition-report.json"

    Private Const StrainCount As Integer = 5
    Private Const MarkerPerStrain As Integer = 90
    Private Const FamilyCount As Integer = 8
    Private Const SampleCount As Integer = 10

    ''' <summary>
    ''' 同源基因之间 reads 交叉分配的比例
    ''' </summary>
    Private Const CrossMapRatio As Double = 0.35

    ''' <summary>
    ''' 执行全部用例
    ''' </summary>
    ''' <returns>失败的断言数量</returns>
    Friend Function Run() As Integer
        Dim originMute As Boolean = VBDebugger.Mute

        Call DebugAudit.Reset()
        DebugAudit.MaxEvents = 16384
        VBDebugger.Mute = True

        Call DebugAudit.Mark("community decomposition audit started")

        Try
            Dim rnd As New Random(20261124)
            Dim genomes As Dictionary(Of String, String())
            Dim truth As Dictionary(Of String, Dictionary(Of String, Double()))
            Dim observed As Dictionary(Of String, Double())
            Dim families As UnionFind
            Dim total As Matrix

            Call MakeSyntheticCommunity(rnd, genomes, truth, observed, families, total)

            ' ---- 结构层 ----
            Call testStructure(genomes, families)

            ' ---- 分解 ----
            Dim opt As New DecompositionOptions With {
                .Method = DecompositionMethod.DNMF,
                .RandomSeed = 42,
                .MaxIterations = 200,
                .RefineIterations = 80,
                .Tqdm = False
            }
            Dim result As CommunityDecomposition = Decomposition.Decompose(total, genomes, families, opt)

            Call DebugAudit.Mark("decomposition.method", result.method)
            Call DebugAudit.Mark("decomposition.cost", result.cost)
            Call DebugAudit.Mark("decomposition.residual", result.strainResidual)
            Call DebugAudit.Mark("decomposition.assign", result.assign)

            Call DebugAudit.Test(String.Equals(result.method, "DNMF", StringComparison.Ordinal),
                                 $"the default decomposition path should be DNMF, got '{result.method}'")

            Call testMatrixShape(result, genomes, total)
            Call testAssignment(result, genomes)
            Call testMarkerAccuracy(result, genomes, truth)
            Call testStrainIdentity(result, genomes, truth)
            Call testHomologImprovement(result, genomes, truth, observed, families)
            Call testMassConservation(result, total)
            Call testAbundance(result, genomes, total)
            Call testCsvExport(result, genomes)

            ' ---- 降级路径与报错路径 ----
            Call testSplitFallback(total, genomes, families)
            Call testRejectNegative(total, genomes, families, opt)
            Call testRejectGeneConflict(genomes, families)
        Catch ex As Exception
            Call DebugAudit.Mark("crash", ex.ToString())
            Call DebugAudit.Test(False, $"community decomposition audit should not throw: {ex.Message}")
        Finally
            VBDebugger.Mute = originMute
        End Try

        Call Directory.CreateDirectory("report")
        Call DebugAudit.SaveReport(Path.Combine("report", ReportName))

        Call Console.WriteLine($"   [audit] {DebugAudit.FailureCount} failure(s)")

        Return DebugAudit.FailureCount
    End Function

    ''' <summary>
    ''' 标记基因识别、同源基因识别与基因归属
    ''' </summary>
    Private Sub testStructure(genomes As Dictionary(Of String, String()), families As UnionFind)
        Dim community As CommunityStructure = Decomposition.BuildCommunityStructure(genomes, families)

        Call DebugAudit.AreEqual(StrainCount, community.size, "structure should contain all strains")
        Call DebugAudit.AreEqual(FamilyCount * StrainCount, community.homologGenes.Count,
                                 "every member of a dupGene family should be marked as homolog")

        For Each strain As String In genomes.Keys
            Call DebugAudit.AreEqual(MarkerPerStrain, community.GetMarkers(strain).Length,
                                     $"marker gene count of {strain}")
            Call DebugAudit.AreEqual(genomes(strain).Length, community.GetGenes(strain).Length,
                                     $"genome gene count of {strain}")
        Next

        Call DebugAudit.AreEqual(FamilyCount, community.GetSharedFamilies().Count(),
                                 "dupGene gene family count")

        For Each family As GeneFamily In community.GetSharedFamilies()
            Call DebugAudit.AreEqual(StrainCount, family.strainCount, "dupGene family should cover all strains")
            Call DebugAudit.AreEqual(StrainCount, family.members.Length, "dupGene family member count")
        Next
    End Sub

    ''' <summary>
    ''' 输出矩阵的数量、行集与样本标签
    ''' </summary>
    Private Sub testMatrixShape(result As CommunityDecomposition, genomes As Dictionary(Of String, String()), total As Matrix)
        Call DebugAudit.AreEqual(StrainCount, result.strains.Count, "one expression matrix should be created per strain")
        Call DebugAudit.AreEqual(StrainCount, result.strainCount, "strainCount")
        Call DebugAudit.AreEqual(1, result.unassignedGenes.Length, "the spike-in gene should be reported as unassigned")

        For Each strain As String In genomes.Keys
            Dim mat As Matrix = Nothing

            If Not DebugAudit.Test(result.strains.TryGetValue(strain, mat), $"missing matrix for strain '{strain}'") Then
                Continue For
            End If

            ' 行维度必须与总矩阵完全对齐(同样的基因、同样的顺序)
            Call DebugAudit.AreEqual(total.size, mat.size,
                                     $"row count of strain '{strain}' should be aligned with the total matrix")
            Call DebugAudit.AreEqual(total.sample_count, mat.sample_count, $"sample count of strain '{strain}'")

            Dim aligned As Boolean = True

            For i As Integer = 0 To total.size - 1
                If Not String.Equals(total.expression(i).geneID, mat.expression(i).geneID, StringComparison.Ordinal) Then
                    aligned = False
                    Exit For
                End If
            Next

            Call DebugAudit.Test(aligned, $"the gene rows of '{strain}' should be aligned with the total matrix")

            ' 属于其它菌株的基因行必须为 0，保证每个菌株矩阵只携带自己的信号
            Dim own As New HashSet(Of String)(genomes(strain), StringComparer.Ordinal)
            Dim foreignSignal As Integer = 0

            For Each row As DataFrameRow In mat.expression
                If own.Contains(row.geneID) Then
                    Continue For
                End If

                For Each v As Double In row.experiments
                    If std.Abs(v) > 0 Then
                        foreignSignal += 1
                        Exit For
                    End If
                Next
            Next

            Call DebugAudit.AreEqual(0, foreignSignal,
                                     $"the matrix of '{strain}' should not carry the signals of the other strains")

            For j As Integer = 0 To total.sample_count - 1
                Call DebugAudit.AreEqual(total.sampleID(j), mat.sampleID(j), $"sample order of strain '{strain}'")
            Next

            Dim invalid As Integer = 0

            For Each row As DataFrameRow In mat.expression
                For Each v As Double In row.experiments
                    If v < 0 OrElse Not Double.IsFinite(v) Then
                        invalid += 1
                    End If
                Next
            Next

            Call DebugAudit.AreEqual(0, invalid, $"the matrix of strain '{strain}' should be finite and non-negative")
        Next
    End Sub

    ''' <summary>
    ''' 因子与菌株必须是一对一映射
    ''' </summary>
    Private Sub testAssignment(result As CommunityDecomposition, genomes As Dictionary(Of String, String()))
        Call DebugAudit.AreEqual(StrainCount, result.assign.Count, "every strain should get a factor")

        Dim factors As New HashSet(Of Integer)

        For Each strain As String In genomes.Keys
            Dim factor As Integer

            If DebugAudit.Test(result.assign.TryGetValue(strain, factor), $"missing factor assignment for '{strain}'") Then
                Call DebugAudit.Test(factor >= 0 AndAlso factor < StrainCount, "factor index out of range")
                Call DebugAudit.Test(factors.Add(factor), $"factor[{factor}] is assigned to more than one strain")
            End If

            Call DebugAudit.Test(result.markers(strain) >= MarkerPerStrain,
                                 $"the assignment of '{strain}' should be driven by its marker genes")
        Next
    End Sub

    ''' <summary>
    ''' 标记基因(无同源歧义)的重构精度
    ''' </summary>
    Private Sub testMarkerAccuracy(result As CommunityDecomposition,
                                  genomes As Dictionary(Of String, String()),
                                  truth As Dictionary(Of String, Dictionary(Of String, Double())))

        For Each strain As String In genomes.Keys
            Dim markers As String() = genomeMarkers(genomes(strain))
            Dim table As Dictionary(Of String, Double()) = RowsOf(result.strains(strain))
            Dim actual As Double() = Collect(table, markers)
            Dim expected As Double() = Collect(truth(strain), markers)
            Dim correlation As Double = Pearson(actual, expected)
            Dim err As Double = RelativeError(actual, expected)

            Call DebugAudit.Mark($"marker.correlation.{strain}", correlation)
            Call DebugAudit.Mark($"marker.relative_error.{strain}", err)

            Call DebugAudit.Test(correlation > 0.95,
                                 $"marker genes of '{strain}' should be recovered, correlation={correlation:F4}")
            Call DebugAudit.Test(err < 0.05,
                                 $"marker genes of '{strain}' should be recovered, relative error={err:F4}")
        Next
    End Sub

    ''' <summary>
    ''' 每个菌株的重构矩阵都应该与自己的真实表达最相似
    ''' </summary>
    Private Sub testStrainIdentity(result As CommunityDecomposition,
                                   genomes As Dictionary(Of String, String()),
                                   truth As Dictionary(Of String, Dictionary(Of String, Double())))

        For Each strain As String In genomes.Keys
            Dim markers As String() = genomeMarkers(genomes(strain))
            Dim actual As Double() = Collect(RowsOf(result.strains(strain)), markers)
            Dim self As Double = Pearson(actual, Collect(truth(strain), markers))

            For Each other As String In genomes.Keys
                If String.Equals(other, strain, StringComparison.Ordinal) Then
                    Continue For
                End If

                Dim cross As Double = Pearson(actual, Collect(truth(other), markers))

                Call DebugAudit.Test(self > cross,
                                     $"the matrix of '{strain}' should be closer to itself({self:F4}) than to '{other}'({cross:F4})")
            Next
        Next
    End Sub

    ''' <summary>
    ''' 同源家族的基因经过分解之后，应该比原始的总矩阵观测值更接近真实值
    ''' </summary>
    Private Sub testHomologImprovement(result As CommunityDecomposition,
                                       genomes As Dictionary(Of String, String()),
                                       truth As Dictionary(Of String, Dictionary(Of String, Double())),
                                       observed As Dictionary(Of String, Double()),
                                       families As UnionFind)

        Dim improved As Integer = 0
        Dim total As Integer = 0
        Dim meanBefore As Double = 0
        Dim meanAfter As Double = 0

        For Each family As KeyValuePair(Of String, List(Of String)) In families.GetClusters()
            If family.Value.Count < 2 Then
                Continue For
            End If

            For Each gene As String In family.Value
                Dim strain As String = StrainOfGene(genomes, gene)

                If strain Is Nothing Then
                    Continue For
                End If

                Dim table As Dictionary(Of String, Double()) = RowsOf(result.strains(strain))
                Dim expected As Double() = truth(strain)(gene)
                Dim before As Double = RelativeError(observed(gene), expected)
                Dim after As Double = RelativeError(table(gene), expected)

                meanBefore += before
                meanAfter += after
                total += 1

                If after < before Then
                    improved += 1
                End If
            Next
        Next

        Call DebugAudit.Mark("homolog.rows", total)
        Call DebugAudit.Mark("homolog.mean_relative_error.observed", If(total = 0, 0, meanBefore / total))
        Call DebugAudit.Mark("homolog.mean_relative_error.decomposed", If(total = 0, 0, meanAfter / total))

        Call DebugAudit.Test(total > 0, "the synthetic community should contain homolog genes")
        Call DebugAudit.Test(improved >= CLng(total) * 0.8,
                             $"at least 80% of the homolog rows should be improved, got {improved}/{total}")
        Call DebugAudit.Test(meanAfter < meanBefore,
                             "the mean relative error of homolog genes should decrease after decomposition")
    End Sub

    ''' <summary>
    ''' 各菌株矩阵之和应该与总矩阵的列总量接近(不启用 RescaleToTotal 时)
    ''' </summary>
    Private Sub testMassConservation(result As CommunityDecomposition, total As Matrix)
        Dim n As Integer = total.sample_count
        Dim observed As Double() = New Double(n - 1) {}
        Dim restored As Double() = New Double(n - 1) {}

        For Each row As DataFrameRow In total.expression
            For j As Integer = 0 To n - 1
                observed(j) += row.experiments(j)
            Next
        Next

        For Each strain As KeyValuePair(Of String, Matrix) In result.strains
            For Each row As DataFrameRow In strain.Value.expression
                For j As Integer = 0 To n - 1
                    restored(j) += row.experiments(j)
                Next
            Next
        Next

        Dim worst As Double = 0

        For j As Integer = 0 To n - 1
            Dim err As Double = std.Abs(restored(j) - observed(j)) / std.Max(observed(j), 0.000000001)

            worst = std.Max(worst, err)
        Next

        Call DebugAudit.Mark("mass_conservation.worst_relative_error", worst)

        Call DebugAudit.Test(worst < 0.15,
                             $"the sum of all strain matrices should match the total matrix, worst relative error={worst:F4}")
    End Sub

    ''' <summary>
    ''' 菌株活性谱矩阵
    ''' </summary>
    Private Sub testAbundance(result As CommunityDecomposition, genomes As Dictionary(Of String, String()), total As Matrix)
        DebugAudit.Test(result.abundance IsNot Nothing, "the abundance matrix should be created")

        If result.abundance Is Nothing Then
            Return
        End If

        Call DebugAudit.AreEqual(StrainCount, result.abundance.size, "abundance rows should be the strains")
        Call DebugAudit.AreEqual(total.sample_count, result.abundance.sample_count, "abundance columns should be the samples")

        For Each strain As String In genomes.Keys
            Dim row As DataFrameRow = result.abundance(strain)

            Call DebugAudit.Test(row IsNot Nothing, $"missing abundance profile of '{strain}'")

            If row IsNot Nothing Then
                Call DebugAudit.Test(row.experiments.All(Function(v) v > 0 AndAlso Double.IsFinite(v)),
                                     $"the abundance profile of '{strain}' should be positive")
            End If
        Next
    End Sub

    ''' <summary>
    ''' CSV 导出与回读
    ''' </summary>
    Private Sub testCsvExport(result As CommunityDecomposition, genomes As Dictionary(Of String, String()))
        Dim folder As String = Path.Combine(
            Path.GetTempPath(),
            "community_decomposition_" & Guid.NewGuid().ToString("N").Substring(0, 8))

        Call Directory.CreateDirectory(folder)

        Try
            Dim files As String() = result.SaveAll(folder)

            Call DebugAudit.AreEqual(StrainCount, files.Length, "one csv file should be written per strain")

            For Each file As String In files
                Dim strain As String = Path.GetFileNameWithoutExtension(file)
                Dim round As Matrix = Matrix.LoadData(file)

                Call DebugAudit.AreEqual(result.strains(strain).size, round.size, $"csv row count of '{strain}'")
                Call DebugAudit.AreEqual(result.strains(strain).sample_count, round.sample_count,
                                         $"csv sample count of '{strain}'")

                Dim origin As Dictionary(Of String, Double()) = RowsOf(result.strains(strain))
                Dim back As Dictionary(Of String, Double()) = RowsOf(round)

                Call DebugAudit.Test(origin.Keys.OrderBy(Function(k) k).SequenceEqual(back.Keys.OrderBy(Function(k) k)),
                                     $"csv round trip should keep every gene of '{strain}'")

                Dim mismatched As Integer = 0

                For Each gene As String In origin.Keys
                    Dim expect As Double() = origin(gene)
                    Dim actual As Double() = back(gene)

                    For j As Integer = 0 To expect.Length - 1
                        If std.Abs(expect(j) - actual(j)) > 0.000001 Then
                            mismatched += 1
                        End If
                    Next
                Next

                Call DebugAudit.AreEqual(0, mismatched, $"csv round trip values of '{strain}'")
            Next
        Finally
            Try
                Call Directory.Delete(folder, recursive:=True)
            Catch ex As Exception
                Call DebugAudit.Mark("cleanup", ex.Message)
            End Try
        End Try
    End Sub

    ''' <summary>
    ''' 样本数少于菌株数时应该走降级路径而不是抛出异常
    ''' </summary>
    Private Sub testSplitFallback(total As Matrix, genomes As Dictionary(Of String, String()), families As UnionFind)
        Dim samples As String() = total.sampleID.Take(StrainCount - 2).ToArray
        Dim subset As Matrix = total.Project(samples)
        Dim result As CommunityDecomposition = Decomposition.Decompose(subset, genomes, families)

        Call DebugAudit.AreEqual("split", result.method, "not enough samples should be degraded to the split mode")
        Call DebugAudit.AreEqual(StrainCount, result.strains.Count, "the split mode should still output every strain")

        For Each strain As String In genomes.Keys
            Call DebugAudit.AreEqual(samples.Length, result.strains(strain).sample_count,
                                     $"the split mode should keep the sample columns of '{strain}'")
        Next
    End Sub

    ''' <summary>
    ''' 含负值的矩阵应该被拒绝
    ''' </summary>
    Private Sub testRejectNegative(total As Matrix,
                                   genomes As Dictionary(Of String, String()),
                                   families As UnionFind,
                                   opt As DecompositionOptions)

        Dim bad As Matrix = Clone(total)

        bad.expression(0).experiments(0) = -1

        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub()
                Call Decomposition.Decompose(bad, genomes, families, opt)
            End Sub,
            "a matrix with negative value should be rejected")
    End Sub

    ''' <summary>
    ''' 基因ID跨基因组重复应该被拒绝(Strict 模式)
    ''' </summary>
    Private Sub testRejectGeneConflict(genomes As Dictionary(Of String, String()), families As UnionFind)
        Dim conflict As New Dictionary(Of String, String())(StringComparer.Ordinal)
        Dim first As Boolean = True
        Dim dupGene As String = Nothing

        For Each strain As String In genomes.Keys
            Dim genes As String() = genomes(strain)

            If first Then
                dupGene = genes(0)
                first = False
                conflict(strain) = genes
            Else
                Dim list As New List(Of String)(genes)

                Call list.Add(dupGene)
                conflict(strain) = list.ToArray
            End If
        Next

        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub()
                Call Decomposition.BuildCommunityStructure(conflict, families, strict:=True)
            End Sub,
            "duplicated gene id across genomes should be rejected in strict mode")

        Dim lenient As CommunityStructure = Decomposition.BuildCommunityStructure(conflict, families, strict:=False)

        Call DebugAudit.AreEqual(conflict.Keys.Count, lenient.size, "lenient mode should still build the structure")
    End Sub

    ''' <summary>
    ''' 生成 5 株菌的合成菌群：标记基因直读、同源家族按 <see cref="CrossMapRatio"/> 交叉分配
    ''' </summary>
    Private Sub MakeSyntheticCommunity(rnd As Random,
                                       ByRef genomes As Dictionary(Of String, String()),
                                       ByRef truth As Dictionary(Of String, Dictionary(Of String, Double())),
                                       ByRef observed As Dictionary(Of String, Double()),
                                       ByRef families As UnionFind,
                                       ByRef total As Matrix)

        Dim strains As String() = New String(StrainCount - 1) {}
        Dim samples As String() = New String(SampleCount - 1) {}

        For i As Integer = 0 To StrainCount - 1
            strains(i) = $"strain{i + 1}"
        Next
        For j As Integer = 0 To SampleCount - 1
            samples(j) = $"sample{j + 1}"
        Next

        genomes = New Dictionary(Of String, String())(StringComparer.Ordinal)
        truth = New Dictionary(Of String, Dictionary(Of String, Double()))(StringComparer.Ordinal)
        observed = New Dictionary(Of String, Double())(StringComparer.Ordinal)
        families = New UnionFind()

        Dim familyMembers As New List(Of List(Of String))
        Dim genes As New List(Of String)

        For f As Integer = 0 To FamilyCount - 1
            Call familyMembers.Add(New List(Of String))
        Next

        For Each strain As String In strains
            Dim table As New Dictionary(Of String, Double())(StringComparer.Ordinal)
            Dim list As New List(Of String)
            Dim activity As Double() = New Double(SampleCount - 1) {}

            For j As Integer = 0 To SampleCount - 1
                activity(j) = 1 + rnd.NextDouble() * 3
            Next

            For k As Integer = 1 To MarkerPerStrain
                Dim gene As String = $"{strain}_g{k}"
                Dim weight As Double = 0.2 + rnd.NextDouble() * 2

                table(gene) = Scale(activity, weight)
                Call list.Add(gene)
                Call genes.Add(gene)
            Next

            For f As Integer = 0 To FamilyCount - 1
                Dim gene As String = $"{strain}_fam{f + 1}"
                Dim weight As Double = 0.2 + rnd.NextDouble() * 2

                table(gene) = Scale(activity, weight)
                Call list.Add(gene)
                Call genes.Add(gene)
                Call familyMembers(f).Add(gene)
            Next

            genomes(strain) = list.ToArray
            truth(strain) = table
        Next

        ' 并集基因 + 基因家族
        Call families.AddElements(genes)

        For Each family As List(Of String) In familyMembers
            Call families.UnionRange(family)
        Next

        ' 观测值：标记基因直读；同源家族按交叉分配比例混合
        For Each strain As String In strains
            For Each gene As String In genomes(strain)
                If IsFamilyGene(gene) Then
                    Continue For
                End If

                observed(gene) = truth(strain)(gene)
            Next
        Next

        For Each family As List(Of String) In familyMembers
            For Each gene As String In family
                Dim strain As String = StrainOfGene(genomes, gene)
                Dim members As New List(Of String)(family)

                Call members.Remove(gene)

                Dim sum As Double() = New Double(SampleCount - 1) {}
                Dim own As Double() = truth(strain)(gene)

                For Each other As String In members
                    Dim v As Double() = truth(StrainOfGene(genomes, other))(other)

                    For j As Integer = 0 To SampleCount - 1
                        sum(j) += v(j)
                    Next
                Next

                Dim mixed As Double() = New Double(SampleCount - 1) {}

                For j As Integer = 0 To SampleCount - 1
                    mixed(j) = (1 - CrossMapRatio) * own(j) + CrossMapRatio / members.Count * sum(j)
                Next

                observed(gene) = mixed
            Next
        Next

        ' 总矩阵：并集基因 + 一个不属于任何基因组的 spike-in
        Dim rows As New List(Of DataFrameRow)

        For Each gene As String In genes
            Call rows.Add(New DataFrameRow With {
                .geneID = gene,
                .experiments = observed(gene)
            })
        Next

        Dim spike As Double() = New Double(SampleCount - 1) {}

        For j As Integer = 0 To SampleCount - 1
            spike(j) = 5 + rnd.NextDouble()
        Next

        Call rows.Add(New DataFrameRow With {
            .geneID = "spike_in_1",
            .experiments = spike
        })

        total = New Matrix With {
            .sampleID = samples,
            .expression = rows.ToArray,
            .tag = "synthetic_community_total"
        }
    End Sub

    Private Function IsFamilyGene(gene As String) As Boolean
        Return gene.Contains("_fam")
    End Function

    Private Function StrainOfGene(genomes As Dictionary(Of String, String()), gene As String) As String
        For Each strain As String In genomes.Keys
            If Array.IndexOf(genomes(strain), gene) >= 0 Then
                Return strain
            End If
        Next

        Return Nothing
    End Function

    Private Function genomeMarkers(genes As String()) As String()
        Return genes.Where(Function(g) Not IsFamilyGene(g)).ToArray
    End Function

    Private Function Scale(activity As Double(), weight As Double) As Double()
        Dim v As Double() = New Double(activity.Length - 1) {}

        For j As Integer = 0 To activity.Length - 1
            v(j) = activity(j) * weight
        Next

        Return v
    End Function

    Private Function Clone(mat As Matrix) As Matrix
        Dim rows As DataFrameRow() = New DataFrameRow(mat.size - 1) {}

        For i As Integer = 0 To rows.Length - 1
            rows(i) = New DataFrameRow With {
                .geneID = mat.expression(i).geneID,
                .experiments = mat.expression(i).experiments.ToArray
            }
        Next

        Return New Matrix With {
            .sampleID = mat.sampleID.ToArray,
            .expression = rows,
            .tag = mat.tag
        }
    End Function

    Private Function RowsOf(mat As Matrix) As Dictionary(Of String, Double())
        Dim table As New Dictionary(Of String, Double())(StringComparer.Ordinal)

        For Each row As DataFrameRow In mat.expression
            table(row.geneID) = row.experiments
        Next

        Return table
    End Function

    Private Function Collect(table As Dictionary(Of String, Double()), genes As String()) As Double()
        Dim values As New List(Of Double)

        For Each gene As String In genes
            Dim v As Double() = Nothing

            If table.TryGetValue(gene, v) Then
                Call values.AddRange(v)
            End If
        Next

        Return values.ToArray
    End Function

    ''' <summary>
    ''' Pearson 相关系数，任一向量为常数时返回 0
    ''' </summary>
    Private Function Pearson(x As Double(), y As Double()) As Double
        If x.Length = 0 OrElse x.Length <> y.Length Then
            Return 0
        End If

        Dim mx As Double = x.Average
        Dim my As Double = y.Average
        Dim cov As Double = 0
        Dim sx As Double = 0
        Dim sy As Double = 0

        For i As Integer = 0 To x.Length - 1
            Dim dx As Double = x(i) - mx
            Dim dy As Double = y(i) - my

            cov += dx * dy
            sx += dx * dx
            sy += dy * dy
        Next

        If sx <= 0 OrElse sy <= 0 Then
            Return 0
        End If

        Return cov / std.Sqrt(sx * sy)
    End Function

    ''' <summary>
    ''' 相对 Frobenius 误差
    ''' </summary>
    Private Function RelativeError(actual As Double(), expected As Double()) As Double
        Dim numerator As Double = 0
        Dim denominator As Double = 0

        For i As Integer = 0 To expected.Length - 1
            Dim d As Double = actual(i) - expected(i)

            numerator += d * d
            denominator += expected(i) * expected(i)
        Next

        Return std.Sqrt(numerator / std.Max(denominator, 0.000000001))
    End Function
End Module
