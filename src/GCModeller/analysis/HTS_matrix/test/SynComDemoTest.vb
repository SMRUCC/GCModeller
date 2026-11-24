Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports std = System.Math

''' <summary>
''' 基于论文 “Herbicide degradation is driven by communication-mediated division of labor
''' in synthetic communities” (Nat Commun 2026;17:11310) 的合成菌群分解演示测试。
''' </summary>
''' <remarks>
''' <para>
''' 演示数据由 GNU R 脚本 <c>test/R/syncom_demo_data.R</c> 生成，保存在
''' <c>test/data/syncom/</c>：四个菌株 Y1/K1/B1/H1 的基因组并集 x 24 个样本
''' (SynCom YKB / HKB x 除草剂胁迫 / 无胁迫 x 野生型 / QS 干扰 x 3 重复)。
''' </para>
''' <para>
''' 本测试读取该 CSV，把 “并集定量” 的总矩阵送入 <see cref="Decomposition"/>，
''' 然后验证分解结果是否复现论文的核心结论：
''' </para>
''' <list type="number">
''' <item>YKB 中 K1 主导起始步骤降解酶、Y1 主导糖类与能量代谢、B1 主导色氨酸代谢；</item>
''' <item>HKB 中 B1(与 K1)共同承担降解、H1 主导硫代谢 / DNA 修复 / AI-2；</item>
''' <item>抑制 AHL(YKB)主要打击 K1 的降解程序，抑制 AI-2(HKB)主要打击 B1 的降解程序；</item>
''' <item>群落中缺失的成员(YKB 中的 H1、HKB 中的 Y1)其活性谱被正确压低；</item>
''' <item>无除草剂胁迫时降解模块显著下调；</item>
''' <item>各菌株矩阵之和与总矩阵守恒，且与真实单菌株矩阵高度相关、同源基因的重构误差下降。</item>
''' </list>
''' <para>
''' 由于分解流程把每个菌株建模为一个秩1因子，样本水平的调控(除草剂诱导、QS 抑制)
''' 在演示数据中被编码为对应菌株的活性谱变化；菌株内部的代谢分工则由各功能模块的
''' 基线投入(module_base)表达。功能可塑性(K1 与 B1 在两个群落中切换角色)属于
''' 组内模块组成的条件性改变，超出了秩1模型的可辨识范围，因此演示数据与断言都不包含该项。
''' </para>
''' <para>
''' 断言全部经由 <see cref="DebugAudit"/> 记录，报告写到
''' <c>report/syncom-decomposition-report.json</c>。
''' </para>
''' </remarks>
Friend Module SynComDemoTest

    Private Const ReportName As String = "syncom-decomposition-report.json"

    Private Const ConsortiumYKB As String = "YKB"
    Private Const ConsortiumHKB As String = "HKB"

    ' 菌株使用学名全名(与 genomes.csv / annotation.csv 的 strain 列一致)
    Private Const StrainY1 As String = "Comamonas sp. Y1"
    Private Const StrainK1 As String = "Acinetobacter sp. K1"
    Private Const StrainB1 As String = "Pseudomonas sp. B1"
    Private Const StrainH1 As String = "Bacillus sp. H1"

    ''' <summary>
    ''' 演示数据中的功能模块名(与 R 脚本保持一致)
    ''' </summary>
    Private Const ModuleHerbInitial As String = "herb_initial"
    Private Const ModuleHerbDownstream As String = "herb_downstream"
    Private Const ModuleQsAhl As String = "qs_ahl"
    Private Const ModuleQsAi2 As String = "qs_ai2"
    Private Const ModuleTryptophan As String = "tryptophan"
    Private Const ModuleSulfur As String = "sulfur"
    Private Const ModuleCarbohydrate As String = "carbohydrate"
    Private Const ModuleDnaRepair As String = "dna_repair"

    ''' <summary>
    ''' 执行演示用例
    ''' </summary>
    ''' <returns>失败的断言数量</returns>
    Friend Function Run() As Integer
        Dim originMute As Boolean = VBDebugger.Mute

        Call DebugAudit.Reset()
        DebugAudit.MaxEvents = 32768
        VBDebugger.Mute = True

        Call DebugAudit.Mark("syncom demo audit started")

        Try
            Dim dataDir As String = FindDataDirectory()

            If dataDir Is Nothing Then
                Call DebugAudit.Mark("syncom.demo", "skipped: demo data not found, run test/R/syncom_demo_data.R first")

                Return 0
            End If

            Call DebugAudit.Mark("syncom.demo.data", dataDir)

            Dim total As Matrix = Matrix.LoadData(Path.Combine(dataDir, "total.csv"))
            Dim genomes As Dictionary(Of String, String()) = LoadGenomes(Path.Combine(dataDir, "genomes.csv"))
            Dim geneModule As Dictionary(Of String, String) = LoadAnnotation(Path.Combine(dataDir, "annotation.csv"))
            Dim geneKO As Dictionary(Of String, String) = LoadColumn(Path.Combine(dataDir, "annotation.csv"), "KO", 0)
            Dim families As UnionFind = LoadFamilies(Path.Combine(dataDir, "families.csv"), genomes)
            Dim familyGenes As Dictionary(Of String, List(Of String)) = LoadFamilyGenes(Path.Combine(dataDir, "families.csv"))
            Dim design As Dictionary(Of String, SampleDesign) = LoadDesign(Path.Combine(dataDir, "design.csv"))
            Dim truth As Dictionary(Of String, Dictionary(Of String, Double())) = LoadTruth(dataDir, genomes.Keys)

            Call DebugAudit.AreEqual(4, genomes.Count, "the demo community should contain 4 strains")
            Call DebugAudit.AreEqual(24, total.sample_count, "the demo design should contain 24 samples")
            Call DebugAudit.Test(total.size > 4000,
                                 $"the demo gene set should be expanded to a genome-scale union, got {total.size} rows")

            ' KEGG 直系同源(KO)映射：并集里的每个基因都要有 KO 注释
            Call testKoAnnotation(total, geneKO, familyGenes, dataDir)

            ' ---- 分解 ----
            ' 两个合成菌群的功能组成不同(论文中的功能可塑性)，因此按菌群分组分别分解再拼接样本列
            Dim groups As String() = New String(total.sample_count - 1) {}

            For j As Integer = 0 To total.sample_count - 1
                Dim sample As SampleDesign = Nothing

                groups(j) = If(design.TryGetValue(total.sampleID(j), sample), sample.consortium, "")
            Next

            Dim opt As New DecompositionOptions With {
                .Method = DecompositionMethod.DNMF,
                .RandomSeed = 20261124,
                .MaxIterations = 250,
                .RefineIterations = 80,
                .Tqdm = False
            }
            Dim result As CommunityDecomposition = Decomposition.DecomposeByGroups(total, genomes, families, groups, opt)

            Call DebugAudit.Mark("syncom.method", result.method)
            Call DebugAudit.Mark("syncom.assign", result.assign)
            Call DebugAudit.Mark("syncom.residual", result.strainResidual)

            Call testMatrixShape(result, genomes, total)

            Call testDivisionOfLabor(result, genomes, geneModule, design)
            Call testQuorumSensingInhibition(result, genomes, geneModule, design)
            Call testHerbicideInduction(result, genomes, geneModule, design)
            Call testAbsentMembers(result, genomes, design)
            Call testAccuracy(result, genomes, geneModule, truth, familyGenes, total)

            ' R 脚本中的 limma + GSVA 分析结论(必须与论文机制一致)
            Call testGsvaConclusions(dataDir)
        Catch ex As Exception
            Call DebugAudit.Mark("crash", ex.ToString())
            Call DebugAudit.Test(False, $"the syncom demo audit should not throw: {ex.Message}")
        Finally
            VBDebugger.Mute = originMute

            Call Directory.CreateDirectory("report")
            Call DebugAudit.SaveReport(Path.Combine("report", ReportName))

            Call Console.WriteLine($"   [syncom] {DebugAudit.FailureCount} failure(s)")
        End Try

        Return DebugAudit.FailureCount
    End Function

    ''' <summary>
    ''' 输出矩阵的数量、行集、样本与因子指派
    ''' </summary>
    Private Sub testMatrixShape(result As CommunityDecomposition,
                                genomes As Dictionary(Of String, String()),
                                total As Matrix)

        Call DebugAudit.AreEqual(genomes.Count, result.strains.Count, "one matrix should be created per strain")

        For Each strain As String In genomes.Keys
            Dim mat As Matrix = Nothing

            If Not DebugAudit.Test(result.strains.TryGetValue(strain, mat), $"missing matrix of '{strain}'") Then
                Continue For
            End If

            ' 每个菌株矩阵的行维度都必须与原始并集矩阵完全对齐
            Call DebugAudit.AreEqual(total.size, mat.size,
                                     $"row count of '{strain}' should be aligned with the total matrix")
            Call DebugAudit.AreEqual(total.sample_count, mat.sample_count, $"sample count of '{strain}'")

            Dim aligned As Boolean = True

            For i As Integer = 0 To total.size - 1
                If Not String.Equals(total.expression(i).geneID, mat.expression(i).geneID, StringComparison.Ordinal) Then
                    aligned = False
                    Exit For
                End If
            Next

            Call DebugAudit.Test(aligned, $"the gene rows of '{strain}' should be aligned with the total matrix")

            ' 本菌株的基因行覆盖完整，且其它菌株的基因行必须为 0
            Dim own As New HashSet(Of String)(genomes(strain), StringComparer.Ordinal)
            Dim present As New HashSet(Of String)(mat.rownames, StringComparer.Ordinal)
            Dim missing As Integer = own.Where(Function(gene) Not present.Contains(gene)).Count()
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

            Call DebugAudit.AreEqual(0, missing, $"the matrix of '{strain}' should contain all of its own genome genes")
            Call DebugAudit.AreEqual(0, foreignSignal,
                                     $"the matrix of '{strain}' should not carry the signals of the other strains")

            Dim invalid As Integer = 0

            For Each row As DataFrameRow In mat.expression
                For Each v As Double In row.experiments
                    If v < 0 OrElse Not Double.IsFinite(v) Then
                        invalid += 1
                    End If
                Next
            Next

            Call DebugAudit.AreEqual(0, invalid, $"the matrix of '{strain}' should be finite and non-negative")
        Next

        Dim factors As New HashSet(Of Integer)

        For Each strain As String In genomes.Keys
            Dim factor As Integer

            If result.assign.TryGetValue(strain, factor) Then
                Call DebugAudit.Test(factors.Add(factor), $"factor[{factor}] is shared by more than one strain")
            Else
                Call DebugAudit.Test(False, $"missing factor assignment of '{strain}'")
            End If

            Call DebugAudit.Test(result.markers(strain) > 200,
                                 $"'{strain}' should be assigned by a large marker gene set, got {result.markers(strain)}")
        Next
    End Sub

    ''' <summary>
    ''' 代谢分工：YKB(K1 降解 / Y1 能量 / B1 色氨酸) 与 HKB(B1+K1 降解 / H1 硫代谢与 DNA 修复)
    ''' </summary>
    Private Sub testDivisionOfLabor(result As CommunityDecomposition,
                                    genomes As Dictionary(Of String, String()),
                                    geneModule As Dictionary(Of String, String),
                                    design As Dictionary(Of String, SampleDesign))

        Dim ykb As Boolean() = MaskOf(result.strains(StrainY1), design, ConsortiumYKB, "WT", "WT")
        Dim hkb As Boolean() = MaskOf(result.strains(StrainY1), design, ConsortiumHKB, "WT", "WT")

        ' --- YKB ---
        Dim k1Initial As Double = ModuleMean(result, genomes, geneModule, StrainK1, ModuleHerbInitial, ykb)
        Dim b1Initial As Double = ModuleMean(result, genomes, geneModule, StrainB1, ModuleHerbInitial, ykb)
        Dim y1Initial As Double = ModuleMean(result, genomes, geneModule, StrainY1, ModuleHerbInitial, ykb)

        Call DebugAudit.Mark("YKB.herb_initial.K1", k1Initial)
        Call DebugAudit.Mark("YKB.herb_initial.B1", b1Initial)
        Call DebugAudit.Mark("YKB.herb_initial.Y1", y1Initial)

        Call DebugAudit.Test(k1Initial > b1Initial AndAlso b1Initial > y1Initial,
                             "in SynCom YKB, K1 should dominate the initial-step herbicide enzymes " &
                             $"(K1={k1Initial:F0} > B1={b1Initial:F0} > Y1={y1Initial:F0})")

        Dim y1Carb As Double = ModuleMean(result, genomes, geneModule, StrainY1, ModuleCarbohydrate, ykb)
        Dim k1Carb As Double = ModuleMean(result, genomes, geneModule, StrainK1, ModuleCarbohydrate, ykb)
        Dim b1Carb As Double = ModuleMean(result, genomes, geneModule, StrainB1, ModuleCarbohydrate, ykb)

        Call DebugAudit.Mark("YKB.carbohydrate.Y1", y1Carb)
        Call DebugAudit.Mark("YKB.carbohydrate.K1", k1Carb)
        Call DebugAudit.Mark("YKB.carbohydrate.B1", b1Carb)

        Call DebugAudit.Test(y1Carb > k1Carb AndAlso y1Carb > b1Carb,
                             "in SynCom YKB, Y1 is the energy supplier and should dominate the carbohydrate/energy module " &
                             $"(Y1={y1Carb:F0}, K1={k1Carb:F0}, B1={b1Carb:F0})")

        Dim b1Trp As Double = ModuleMean(result, genomes, geneModule, StrainB1, ModuleTryptophan, ykb)
        Dim k1Trp As Double = ModuleMean(result, genomes, geneModule, StrainK1, ModuleTryptophan, ykb)
        Dim y1Trp As Double = ModuleMean(result, genomes, geneModule, StrainY1, ModuleTryptophan, ykb)

        Call DebugAudit.Mark("YKB.tryptophan.B1", b1Trp)
        Call DebugAudit.Mark("YKB.tryptophan.K1", k1Trp)
        Call DebugAudit.Mark("YKB.tryptophan.Y1", y1Trp)

        Call DebugAudit.Test(b1Trp > k1Trp AndAlso b1Trp > y1Trp,
                             "in SynCom YKB, B1 acts as the metabolic janitor and should dominate tryptophan metabolism " &
                             $"(B1={b1Trp:F0}, K1={k1Trp:F0}, Y1={y1Trp:F0})")

        Dim k1Ahl As Double = ModuleMean(result, genomes, geneModule, StrainK1, ModuleQsAhl, ykb)
        Dim b1Ahl As Double = ModuleMean(result, genomes, geneModule, StrainB1, ModuleQsAhl, ykb)

        Call DebugAudit.Test(k1Ahl > b1Ahl,
                             $"in SynCom YKB, the AHL quorum sensing should be dominated by K1 (K1={k1Ahl:F0}, B1={b1Ahl:F0})")

        ' --- HKB ---
        Dim k1Herb As Double = ModuleMean(result, genomes, geneModule, StrainK1, ModuleHerbInitial, hkb) +
                               ModuleMean(result, genomes, geneModule, StrainK1, ModuleHerbDownstream, hkb)
        Dim b1Herb As Double = ModuleMean(result, genomes, geneModule, StrainB1, ModuleHerbInitial, hkb) +
                               ModuleMean(result, genomes, geneModule, StrainB1, ModuleHerbDownstream, hkb)
        Dim h1Herb As Double = ModuleMean(result, genomes, geneModule, StrainH1, ModuleHerbInitial, hkb) +
                               ModuleMean(result, genomes, geneModule, StrainH1, ModuleHerbDownstream, hkb)

        Call DebugAudit.Mark("HKB.herb.K1", k1Herb)
        Call DebugAudit.Mark("HKB.herb.B1", b1Herb)
        Call DebugAudit.Mark("HKB.herb.H1", h1Herb)

        Call DebugAudit.Test(b1Herb > k1Herb AndAlso k1Herb > h1Herb,
                             "in SynCom HKB, B1 and K1 collaboratively degrade herbicides while H1 is auxiliary " &
                             $"(B1={b1Herb:F0} > K1={k1Herb:F0} > H1={h1Herb:F0})")

        Dim h1Sulfur As Double = ModuleMean(result, genomes, geneModule, StrainH1, ModuleSulfur, hkb)
        Dim b1Sulfur As Double = ModuleMean(result, genomes, geneModule, StrainB1, ModuleSulfur, hkb)
        Dim k1Sulfur As Double = ModuleMean(result, genomes, geneModule, StrainK1, ModuleSulfur, hkb)

        Call DebugAudit.Mark("HKB.sulfur.H1", h1Sulfur)
        Call DebugAudit.Mark("HKB.sulfur.B1", b1Sulfur)
        Call DebugAudit.Mark("HKB.sulfur.K1", k1Sulfur)

        Call DebugAudit.Test(h1Sulfur > b1Sulfur AndAlso b1Sulfur > k1Sulfur,
                             "in SynCom HKB, sulfur metabolism should be activated by H1 and B1 " &
                             $"(H1={h1Sulfur:F0} > B1={b1Sulfur:F0} > K1={k1Sulfur:F0})")

        Dim h1Repair As Double = ModuleMean(result, genomes, geneModule, StrainH1, ModuleDnaRepair, hkb)
        Dim b1Repair As Double = ModuleMean(result, genomes, geneModule, StrainB1, ModuleDnaRepair, hkb)

        Call DebugAudit.Test(h1Repair > b1Repair,
                             $"in SynCom HKB, H1 should dominate DNA repair (H1={h1Repair:F0}, B1={b1Repair:F0})")

        Dim h1Ai2 As Double = ModuleMean(result, genomes, geneModule, StrainH1, ModuleQsAi2, hkb)
        Dim b1Ai2 As Double = ModuleMean(result, genomes, geneModule, StrainB1, ModuleQsAi2, hkb)

        Call DebugAudit.Test(h1Ai2 > b1Ai2,
                             $"in SynCom HKB, AI-2 signalling should be dominated by H1 (H1={h1Ai2:F0}, B1={b1Ai2:F0})")
    End Sub

    ''' <summary>
    ''' QS 抑制：AHL 干扰打击 K1 的起始步骤酶，AI-2 干扰打击 B1 的下游步骤酶
    ''' </summary>
    Private Sub testQuorumSensingInhibition(result As CommunityDecomposition,
                                            genomes As Dictionary(Of String, String()),
                                            geneModule As Dictionary(Of String, String),
                                            design As Dictionary(Of String, SampleDesign))

        Dim ykbWt As Boolean() = MaskOf(result.strains(StrainY1), design, ConsortiumYKB, "WT", "WT")
        Dim ykbQsr As Boolean() = MaskOf(result.strains(StrainY1), design, ConsortiumYKB, "WT", "QSR")
        Dim hkbWt As Boolean() = MaskOf(result.strains(StrainY1), design, ConsortiumHKB, "WT", "WT")
        Dim hkbQsr As Boolean() = MaskOf(result.strains(StrainY1), design, ConsortiumHKB, "WT", "QSR")

        ' YKB: luxI RNAi -> AHL 通路被抑制 -> K1 起始步骤酶显著下调
        Dim k1InitialWt As Double = ModuleMean(result, genomes, geneModule, StrainK1, ModuleHerbInitial, ykbWt)
        Dim k1InitialQsr As Double = ModuleMean(result, genomes, geneModule, StrainK1, ModuleHerbInitial, ykbQsr)
        Dim k1AhlWt As Double = ModuleMean(result, genomes, geneModule, StrainK1, ModuleQsAhl, ykbWt)
        Dim k1AhlQsr As Double = ModuleMean(result, genomes, geneModule, StrainK1, ModuleQsAhl, ykbQsr)

        Call DebugAudit.Mark("YKB_QSR.K1.herb_initial", k1InitialQsr)
        Call DebugAudit.Mark("YKB_WT.K1.herb_initial", k1InitialWt)
        Call DebugAudit.Mark("YKB_QSR.K1.qs_ahl", k1AhlQsr)

        Call DebugAudit.Test(k1InitialQsr < k1InitialWt * 0.6,
                             "AHL inhibition in YKB should suppress the initial-step enzymes of K1 " &
                             $"(WT={k1InitialWt:F0} -> QSR={k1InitialQsr:F0})")
        Call DebugAudit.Test(k1AhlQsr < k1AhlWt * 0.5,
                             "AHL inhibition in YKB should suppress the AHL pathway of K1 " &
                             $"(WT={k1AhlWt:F0} -> QSR={k1AhlQsr:F0})")

        ' AHL 抑制的靶标是 YKB 中的主降解者 K1，非靶标成员 B1 的下降幅度应显著更小
        Dim b1InitialWt As Double = ModuleMean(result, genomes, geneModule, StrainB1, ModuleHerbInitial, ykbWt)
        Dim b1InitialQsr As Double = ModuleMean(result, genomes, geneModule, StrainB1, ModuleHerbInitial, ykbQsr)
        Dim k1Drop As Double = 1 - k1InitialQsr / std.Max(k1InitialWt, 1)
        Dim b1Drop As Double = 1 - b1InitialQsr / std.Max(b1InitialWt, 1)

        Call DebugAudit.Mark("YKB_QSR.drop.K1", k1Drop)
        Call DebugAudit.Mark("YKB_QSR.drop.B1", b1Drop)

        Call DebugAudit.Test(k1Drop > b1Drop + 0.2,
                             "AHL inhibition should specifically suppress K1 rather than B1 in YKB " &
                             $"(drop: K1={k1Drop:P1}, B1={b1Drop:P1})")

        ' HKB: luxS RNAi -> AI-2 通路被抑制 -> B1 下游步骤酶显著下调
        Dim b1DownWt As Double = ModuleMean(result, genomes, geneModule, StrainB1, ModuleHerbDownstream, hkbWt)
        Dim b1DownQsr As Double = ModuleMean(result, genomes, geneModule, StrainB1, ModuleHerbDownstream, hkbQsr)
        Dim h1SulfurWt As Double = ModuleMean(result, genomes, geneModule, StrainH1, ModuleSulfur, hkbWt)
        Dim h1SulfurQsr As Double = ModuleMean(result, genomes, geneModule, StrainH1, ModuleSulfur, hkbQsr)
        Dim k1DownWt As Double = ModuleMean(result, genomes, geneModule, StrainK1, ModuleHerbDownstream, hkbWt)
        Dim k1DownQsr As Double = ModuleMean(result, genomes, geneModule, StrainK1, ModuleHerbDownstream, hkbQsr)

        Call DebugAudit.Mark("HKB_QSR.B1.herb_downstream", b1DownQsr)
        Call DebugAudit.Mark("HKB_WT.B1.herb_downstream", b1DownWt)
        Call DebugAudit.Mark("HKB_QSR.H1.sulfur", h1SulfurQsr)

        Call DebugAudit.Test(b1DownQsr < b1DownWt * 0.75,
                             "AI-2 inhibition in HKB should suppress the downstream-step enzymes of B1 " &
                             $"(WT={b1DownWt:F0} -> QSR={b1DownQsr:F0})")
        Call DebugAudit.Test(h1SulfurQsr < h1SulfurWt * 0.8,
                             "AI-2 inhibition in HKB should suppress the sulfur metabolism of H1 " &
                             $"(WT={h1SulfurWt:F0} -> QSR={h1SulfurQsr:F0})")

        ' AI-2 抑制的靶标是 HKB 中的核心降解者 B1，K1 的下降幅度应显著更小
        Dim b1DownDrop As Double = 1 - b1DownQsr / std.Max(b1DownWt, 1)
        Dim k1DownDrop As Double = 1 - k1DownQsr / std.Max(k1DownWt, 1)

        Call DebugAudit.Mark("HKB_QSR.drop.B1", b1DownDrop)
        Call DebugAudit.Mark("HKB_QSR.drop.K1", k1DownDrop)

        Call DebugAudit.Test(b1DownDrop > k1DownDrop + 0.2,
                             "AI-2 inhibition should specifically suppress B1 rather than K1 in HKB " &
                             $"(drop: B1={b1DownDrop:P1}, K1={k1DownDrop:P1})")
    End Sub

    ''' <summary>
    ''' 除草剂诱导：无除草剂时降解模块显著下降
    ''' </summary>
    Private Sub testHerbicideInduction(result As CommunityDecomposition,
                                       genomes As Dictionary(Of String, String()),
                                       geneModule As Dictionary(Of String, String),
                                       design As Dictionary(Of String, SampleDesign))

        Dim ykbWt As Boolean() = MaskOf(result.strains(StrainY1), design, ConsortiumYKB, "WT", "WT")
        Dim ykbNh As Boolean() = MaskOf(result.strains(StrainY1), design, ConsortiumYKB, "NH", "WT")
        Dim hkbWt As Boolean() = MaskOf(result.strains(StrainY1), design, ConsortiumHKB, "WT", "WT")
        Dim hkbNh As Boolean() = MaskOf(result.strains(StrainY1), design, ConsortiumHKB, "NH", "WT")

        Dim k1Wt As Double = ModuleMean(result, genomes, geneModule, StrainK1, ModuleHerbInitial, ykbWt)
        Dim k1Nh As Double = ModuleMean(result, genomes, geneModule, StrainK1, ModuleHerbInitial, ykbNh)

        Call DebugAudit.Mark("YKB_NH.K1.herb_initial", k1Nh)

        Call DebugAudit.Test(k1Nh < k1Wt * 0.7,
                             $"herbicide induction should raise the herbicide enzymes of K1 in YKB (WT={k1Wt:F0}, NH={k1Nh:F0})")

        Dim b1Wt As Double = ModuleMean(result, genomes, geneModule, StrainB1, ModuleHerbDownstream, hkbWt)
        Dim b1Nh As Double = ModuleMean(result, genomes, geneModule, StrainB1, ModuleHerbDownstream, hkbNh)

        Call DebugAudit.Mark("HKB_NH.B1.herb_downstream", b1Nh)

        Call DebugAudit.Test(b1Nh < b1Wt * 0.7,
                             $"herbicide induction should raise the herbicide enzymes of B1 in HKB (WT={b1Wt:F0}, NH={b1Nh:F0})")
    End Sub

    ''' <summary>
    ''' 缺失成员：YKB 中没有 H1、HKB 中没有 Y1，其重构表达量应被正确压低
    ''' </summary>
    ''' <remarks>
    ''' 这里用“标记基因的平均重构表达量”而不是活性谱 h 本身来衡量菌株的存在量：
    ''' 活性谱与基因载荷 <c>w</c> 的绝对尺度互相耦合(可观测量是二者的乘积)，
    ''' 因此不能拿 h 的绝对值跨菌株比较。
    ''' </remarks>
    Private Sub testAbsentMembers(result As CommunityDecomposition,
                                  genomes As Dictionary(Of String, String()),
                                  design As Dictionary(Of String, SampleDesign))

        DebugAudit.Test(result.abundance IsNot Nothing, "the abundance matrix should be created")

        If result.abundance Is Nothing Then
            Return
        End If

        Dim reference As Matrix = result.strains(StrainY1)
        Dim ykb As Boolean() = MaskOf(reference, design, ConsortiumYKB, Nothing, Nothing)
        Dim hkb As Boolean() = MaskOf(reference, design, ConsortiumHKB, Nothing, Nothing)

        Dim h1Ykb As Double = MarkerMean(result, StrainH1, ykb)
        Dim h1Hkb As Double = MarkerMean(result, StrainH1, hkb)
        Dim y1Ykb As Double = MarkerMean(result, StrainY1, ykb)
        Dim y1Hkb As Double = MarkerMean(result, StrainY1, hkb)

        Call DebugAudit.Mark("absent.H1.YKB", h1Ykb)
        Call DebugAudit.Mark("absent.H1.HKB", h1Hkb)
        Call DebugAudit.Mark("absent.Y1.YKB", y1Ykb)
        Call DebugAudit.Mark("absent.Y1.HKB", y1Hkb)

        Call DebugAudit.Test(h1Hkb > h1Ykb * 5,
                             $"H1 is absent from YKB, its expression should drop accordingly (YKB={h1Ykb:G3}, HKB={h1Hkb:G3})")
        Call DebugAudit.Test(y1Ykb > y1Hkb * 5,
                             $"Y1 is absent from HKB, its expression should drop accordingly (YKB={y1Ykb:G3}, HKB={y1Hkb:G3})")
    End Sub

    ''' <summary>
    ''' 某个菌株的标记基因在指定样本组上的平均重构表达量
    ''' </summary>
    Private Function MarkerMean(result As CommunityDecomposition, strain As String, mask As Boolean()) As Double
        Dim mat As Matrix = Nothing

        If Not result.strains.TryGetValue(strain, mat) Then
            Return 0
        End If

        Dim genes As String() = Nothing

        If result.markerGenes Is Nothing OrElse Not result.markerGenes.TryGetValue(strain, genes) Then
            genes = mat.rownames
        End If

        Dim rows As Dictionary(Of String, Integer) = RowIndex(mat)
        Dim sum As Double = 0
        Dim used As Integer = 0

        For j As Integer = 0 To mat.sample_count - 1
            If Not mask(j) Then
                Continue For
            End If

            used += 1

            For Each gene As String In genes
                Dim r As Integer

                If rows.TryGetValue(gene, r) Then
                    sum += mat.expression(r).experiments(j)
                End If
            Next
        Next

        If used = 0 Then
            Return 0
        Else
            Return sum / used
        End If
    End Function

    ''' <summary>
    ''' 重构精度：与真实单菌株矩阵的相关性、同源基因的改善与整体质量守恒
    ''' </summary>
    Private Sub testAccuracy(result As CommunityDecomposition,
                             genomes As Dictionary(Of String, String()),
                             geneModule As Dictionary(Of String, String),
                             truth As Dictionary(Of String, Dictionary(Of String, Double())),
                             familyGenes As Dictionary(Of String, List(Of String)),
                             total As Matrix)

        Dim homolog As New HashSet(Of String)(StringComparer.Ordinal)

        For Each family As KeyValuePair(Of String, List(Of String)) In familyGenes
            For Each gene As String In family.Value
                Call homolog.Add(gene)
            Next
        Next

        ' 1. 标记基因(无同源基因)的重构相关性
        For Each strain As String In genomes.Keys
            If Not truth.ContainsKey(strain) Then
                Continue For
            End If

            Dim markers As String() = genomes(strain) _
                .Where(Function(g) Not homolog.Contains(g)) _
                .ToArray

            If markers.Length = 0 Then
                Continue For
            End If

            Dim actual As Dictionary(Of String, Double()) = RowsOf(result.strains(strain))
            Dim expected As Dictionary(Of String, Double()) = truth(strain)
            Dim x As New List(Of Double)
            Dim y As New List(Of Double)

            For Each gene As String In markers
                Dim a As Double() = Nothing
                Dim b As Double() = Nothing

                If actual.TryGetValue(gene, a) AndAlso expected.TryGetValue(gene, b) Then
                    Call x.AddRange(a)
                    Call y.AddRange(b)
                End If
            Next

            Dim correlation As Double = Pearson(x.ToArray, y.ToArray)

            Call DebugAudit.Mark($"accuracy.marker_correlation.{strain}", correlation)
            Call DebugAudit.Test(correlation > 0.8,
                                 $"the reconstructed matrix of '{strain}' should correlate with the truth, r={correlation:F4}")
        Next

        ' 2. 同源基因：分解后的重构比原始并集观测量更接近真实值
        Dim improved As Integer = 0
        Dim count As Integer = 0
        Dim before As Double = 0
        Dim after As Double = 0

        For Each family As KeyValuePair(Of String, List(Of String)) In familyGenes
            For Each gene As String In family.Value
                Dim strain As String = StrainOfGene(genomes, gene)

                If strain Is Nothing Then
                    Continue For
                End If
                If Not truth.ContainsKey(strain) Then
                    Continue For
                End If

                Dim reconstructed As Dictionary(Of String, Double()) = RowsOf(result.strains(strain))
                Dim expected As Double() = Nothing

                If Not reconstructed.ContainsKey(gene) Then
                    Continue For
                End If
                If Not truth(strain).TryGetValue(gene, expected) Then
                    Continue For
                End If

                Dim observedRow As DataFrameRow = total(gene)

                If observedRow Is Nothing Then
                    Continue For
                End If

                Dim errBefore As Double = RelativeError(observedRow.experiments, expected)
                Dim errAfter As Double = RelativeError(reconstructed(gene), expected)

                before += errBefore
                after += errAfter
                count += 1

                If errAfter < errBefore Then
                    improved += 1
                End If
            Next
        Next

        Call DebugAudit.Mark("accuracy.homolog.rows", count)
        Call DebugAudit.Mark("accuracy.homolog.observed_error", If(count = 0, 0, before / count))
        Call DebugAudit.Mark("accuracy.homolog.decomposed_error", If(count = 0, 0, after / count))

        Call DebugAudit.Test(count > 100, "the demo data should contain a large homolog gene set")

        ' 注意：演示数据带有 Poisson 计数噪声(低表达行尤其明显)，因此并不保证每一行都能改善；
        ' 这里要求“过半的行改善” + “平均误差显著下降”，这已经能够证明交叉分配被反解掉了。
        Call DebugAudit.Test(improved >= CLng(count) * 0.5,
                             $"at least 50% of the homolog rows should be improved, got {improved}/{count}")
        Call DebugAudit.Test(after < before * 0.85,
                             $"the mean relative error of homolog genes should drop by at least 15% " &
                             $"(observed={before:F4}, decomposed={after:F4})")

        ' 3. 质量守恒
        Dim n As Integer = total.sample_count
        Dim observedSum As Double() = New Double(n - 1) {}
        Dim restoredSum As Double() = New Double(n - 1) {}

        For Each row As DataFrameRow In total.expression
            For j As Integer = 0 To n - 1
                observedSum(j) += row.experiments(j)
            Next
        Next

        For Each strain As KeyValuePair(Of String, Matrix) In result.strains
            For Each row As DataFrameRow In strain.Value.expression
                For j As Integer = 0 To n - 1
                    restoredSum(j) += row.experiments(j)
                Next
            Next
        Next

        Dim worst As Double = 0

        For j As Integer = 0 To n - 1
            worst = std.Max(worst, std.Abs(restoredSum(j) - observedSum(j)) / std.Max(observedSum(j), 1))
        Next

        Call DebugAudit.Mark("accuracy.mass_conservation.worst", worst)
        Call DebugAudit.Test(worst < 0.2,
                             $"the sum of the strain matrices should match the total matrix, worst relative error={worst:F4}")
    End Sub

    ''' <summary>
    ''' 某个菌株某个功能模块在指定样本组上的平均表达总量
    ''' </summary>
    Private Function ModuleMean(result As CommunityDecomposition,
                                genomes As Dictionary(Of String, String()),
                                geneModule As Dictionary(Of String, String),
                                strain As String,
                                functionModule As String,
                                mask As Boolean()) As Double

        Dim mat As Matrix = Nothing

        If Not result.strains.TryGetValue(strain, mat) Then
            Return 0
        End If

        Dim rows As Dictionary(Of String, Integer) = RowIndex(mat)
        Dim sum As Double = 0
        Dim used As Integer = 0

        For j As Integer = 0 To mat.sample_count - 1
            If Not mask(j) Then
                Continue For
            End If

            used += 1

            For Each gene As String In genomes(strain)
                Dim segment As String = Nothing

                If Not geneModule.TryGetValue(gene, segment) OrElse Not String.Equals(segment, functionModule, StringComparison.Ordinal) Then
                    Continue For
                End If

                Dim r As Integer

                If rows.TryGetValue(gene, r) Then
                    sum += mat.expression(r).experiments(j)
                End If
            Next
        Next

        If used = 0 Then
            Return 0
        Else
            Return sum / used
        End If
    End Function

    ''' <summary>
    ''' 样本组掩码：consortium / herbicide / qs 为空字符串时视为通配
    ''' </summary>
    Private Function MaskOf(mat As Matrix,
                            design As Dictionary(Of String, SampleDesign),
                            consortium As String,
                            herbicide As String,
                            qs As String) As Boolean()

        Dim mask As Boolean() = New Boolean(mat.sample_count - 1) {}

        For j As Integer = 0 To mat.sample_count - 1
            Dim sample As SampleDesign = Nothing

            If Not design.TryGetValue(mat.sampleID(j), sample) Then
                Continue For
            End If

            If Not String.IsNullOrEmpty(consortium) AndAlso Not String.Equals(sample.consortium, consortium, StringComparison.Ordinal) Then
                Continue For
            End If
            If Not String.IsNullOrEmpty(herbicide) AndAlso Not String.Equals(sample.herbicide, herbicide, StringComparison.Ordinal) Then
                Continue For
            End If
            If Not String.IsNullOrEmpty(qs) AndAlso Not String.Equals(sample.qs, qs, StringComparison.Ordinal) Then
                Continue For
            End If

            mask(j) = True
        Next

        Return mask
    End Function

    Private Function MeanAbundance(abundance As Matrix, strain As String, mask As Boolean()) As Double
        Dim rows As Dictionary(Of String, Integer) = RowIndex(abundance)
        Dim r As Integer

        If Not rows.TryGetValue(strain, r) Then
            Return 0
        End If

        Dim v As Double() = abundance.expression(r).experiments
        Dim sum As Double = 0
        Dim used As Integer = 0

        For j As Integer = 0 To v.Length - 1
            If mask(j) Then
                sum += v(j)
                used += 1
            End If
        Next

        If used = 0 Then
            Return 0
        Else
            Return sum / used
        End If
    End Function

    Private Function RowIndex(mat As Matrix) As Dictionary(Of String, Integer)
        Dim index As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

        For i As Integer = 0 To mat.size - 1
            index(mat.expression(i).geneID) = i
        Next

        Return index
    End Function

    Private Function RowsOf(mat As Matrix) As Dictionary(Of String, Double())
        Dim table As New Dictionary(Of String, Double())(StringComparer.Ordinal)

        For Each row As DataFrameRow In mat.expression
            table(row.geneID) = row.experiments
        Next

        Return table
    End Function

    Private Function StrainOfGene(genomes As Dictionary(Of String, String()), gene As String) As String
        For Each strain As String In genomes.Keys
            If Array.IndexOf(genomes(strain), gene) >= 0 Then
                Return strain
            End If
        Next

        Return Nothing
    End Function

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

    Private Function RelativeError(actual As Double(), expected As Double()) As Double
        Dim numerator As Double = 0
        Dim denominator As Double = 0

        For i As Integer = 0 To expected.Length - 1
            Dim d As Double = actual(i) - expected(i)

            numerator += d * d
            denominator += expected(i) * expected(i)
        Next

        Return std.Sqrt(numerator / std.Max(denominator, 1))
    End Function

    ''' <summary>
    ''' 演示数据的实验设计行
    ''' </summary>
    Private Class SampleDesign
        Public Property consortium As String
        Public Property herbicide As String
        Public Property qs As String
    End Class

    ''' <summary>
    ''' KEGG 直系同源(KO)映射检查：每个基因都要有 KO，且同源家族成员共享同一个 KO
    ''' </summary>
    Private Sub testKoAnnotation(total As Matrix,
                                 geneKO As Dictionary(Of String, String),
                                 familyGenes As Dictionary(Of String, List(Of String)),
                                 dataDir As String)

        Dim missing As Integer = 0

        For Each row As DataFrameRow In total.expression
            Dim ko As String = Nothing

            If Not geneKO.TryGetValue(row.geneID, ko) OrElse String.IsNullOrEmpty(ko) Then
                missing += 1
            End If
        Next

        Call DebugAudit.Mark("ko.mapped", total.size - missing)
        Call DebugAudit.AreEqual(0, missing,
                                 "every gene in the union matrix should carry a KEGG ortholog (KO) id")

        Dim familyKO As Dictionary(Of String, String) =
            LoadColumn(Path.Combine(dataDir, "families.csv"), "KO", 1)
        Dim inconsistent As Integer = 0

        For Each family As KeyValuePair(Of String, List(Of String)) In familyGenes
            Dim kos As New HashSet(Of String)(StringComparer.Ordinal)

            For Each gene As String In family.Value
                Dim ko As String = Nothing

                If familyKO.TryGetValue(gene, ko) Then
                    Call kos.Add(ko)
                End If
            Next

            If kos.Count > 1 Then
                inconsistent += 1
            End If
        Next

        Call DebugAudit.AreEqual(0, inconsistent,
                                 "the members of one gene family should share the same KO assignment")
    End Sub

    ''' <summary>
    ''' R 脚本(limma + GSVA)输出的 KEGG 通路结论表：全部结论都必须成立
    ''' </summary>
    Private Sub testGsvaConclusions(dataDir As String)
        Dim conclusionFile As String = Path.Combine(dataDir, "gsva_conclusions.csv")

        If Not File.Exists(conclusionFile) Then
            Call DebugAudit.Mark("gsva.conclusions", "skipped: gsva_conclusions.csv not found")
            Return
        End If

        Dim rows As List(Of String()) = ReadTable(conclusionFile)
        Dim header As String() = rows(0)
        Dim idCol As Integer = Array.IndexOf(header, "id")
        Dim descriptionCol As Integer = Array.IndexOf(header, "description")
        Dim passedCol As Integer = Array.IndexOf(header, "passed")

        Call DebugAudit.Test(idCol >= 0 AndAlso passedCol >= 0,
                             "gsva_conclusions.csv should contain the id/passed columns")

        Dim total As Integer = 0
        Dim failed As Integer = 0

        For Each row As String() In rows.Skip(1)
            If row.Length <= std.Max(idCol, passedCol) Then
                Continue For
            End If

            total += 1

            If String.Equals(row(passedCol).Trim, "TRUE", StringComparison.OrdinalIgnoreCase) Then
                Continue For
            End If

            failed += 1

            Dim description As String = If(descriptionCol >= 0 AndAlso row.Length > descriptionCol,
                                           row(descriptionCol),
                                           "")

            Call DebugAudit.Test(False,
                                 $"the GSVA-based KEGG conclusion '{row(idCol).Trim()}' should match the paper: {description}")
        Next

        Call DebugAudit.Mark("gsva.conclusions.total", total)
        Call DebugAudit.Mark("gsva.conclusions.failed", failed)

        Call DebugAudit.Test(total >= 10, "the analysis should report the full list of GSVA-based KEGG conclusions")
        Call DebugAudit.Test(failed = 0,
                             $"every GSVA-based KEGG pathway conclusion should be satisfied, {failed}/{total} failed")
    End Sub

    ''' <summary>
    ''' 读取 CSV 的某一列并建立 “键 -> 值” 映射
    ''' </summary>
    ''' <param name="path">CSV 文件</param>
    ''' <param name="columnName">值列的表头</param>
    ''' <param name="keyColumn">作为键的列下标</param>
    Private Function LoadColumn(path As String, columnName As String, keyColumn As Integer) As Dictionary(Of String, String)
        Dim table As New Dictionary(Of String, String)(StringComparer.Ordinal)
        Dim rows As List(Of String()) = ReadTable(path)
        Dim header As String() = rows(0)
        Dim valueCol As Integer = Array.IndexOf(header, columnName)

        If valueCol < 0 Then
            Return table
        End If

        For Each row As String() In rows.Skip(1)
            If row.Length <= std.Max(valueCol, keyColumn) Then
                Continue For
            End If

            table(row(keyColumn).Trim) = row(valueCol).Trim
        Next

        Return table
    End Function

    ''' <summary>
    ''' 从程序目录向上查找 <c>data/syncom/total.csv</c>
    ''' </summary>
    Private Function FindDataDirectory() As String
        Dim env As String = Environment.GetEnvironmentVariable("SYNCOM_DEMO_DATA")

        If Not String.IsNullOrEmpty(env) AndAlso File.Exists(Path.Combine(env, "total.csv")) Then
            Return env
        End If

        Dim folder As New DirectoryInfo(AppContext.BaseDirectory)

        For level As Integer = 0 To 8
            If folder Is Nothing Then
                Exit For
            End If

            Dim candidate As String = Path.Combine(folder.FullName, "data", "syncom")

            If File.Exists(Path.Combine(candidate, "total.csv")) Then
                Return candidate
            End If

            folder = folder.Parent
        Next

        Return Nothing
    End Function

    Private Function ReadTable(path As String) As List(Of String())
        Return Document.ReadTable(path, ","c).ToList
    End Function

    Private Function LoadGenomes(path As String) As Dictionary(Of String, String())
        Dim table As New Dictionary(Of String, List(Of String))(StringComparer.Ordinal)
        Dim rows As List(Of String()) = ReadTable(path)
        Dim header As String() = rows(0)
        Dim strainCol As Integer = Array.IndexOf(header, "strain")
        Dim geneCol As Integer = Array.IndexOf(header, "gene_id")

        If strainCol < 0 Then strainCol = 0
        If geneCol < 0 Then geneCol = 1

        For Each row As String() In rows.Skip(1)
            If row.Length <= std.Max(strainCol, geneCol) Then
                Continue For
            End If

            Dim strain As String = row(strainCol).Trim
            Dim gene As String = row(geneCol).Trim

            If strain = "" OrElse gene = "" Then
                Continue For
            End If

            Dim list As List(Of String) = Nothing

            If Not table.TryGetValue(strain, list) Then
                list = New List(Of String)
                table(strain) = list
            End If

            Call list.Add(gene)
        Next

        Dim genomes As New Dictionary(Of String, String())(StringComparer.Ordinal)

        For Each strain As KeyValuePair(Of String, List(Of String)) In table
            genomes(strain.Key) = strain.Value.ToArray
        Next

        Return genomes
    End Function

    ''' <summary>
    ''' 读取基因 -> 功能模块的注释(过滤掉没有任何功能注释的行)
    ''' </summary>
    Private Function LoadAnnotation(path As String) As Dictionary(Of String, String)
        Dim table As New Dictionary(Of String, String)(StringComparer.Ordinal)
        Dim header As String() = Nothing
        Dim geneCol As Integer = 0
        Dim moduleCol As Integer = 2

        For Each row As String() In ReadTable(path)
            If header Is Nothing Then
                header = row
                geneCol = Array.IndexOf(header, "gene_id")
                moduleCol = Array.IndexOf(header, "module")

                If geneCol < 0 Then geneCol = 0
                If moduleCol < 0 Then moduleCol = 2

                Continue For
            End If

            If row.Length <= std.Max(geneCol, moduleCol) Then
                Continue For
            End If

            Dim gene As String = row(geneCol).Trim
            Dim segment As String = row(moduleCol).Trim

            If gene <> "" AndAlso segment <> "" Then
                table(gene) = segment
            End If
        Next

        Return table
    End Function

    Private Function LoadFamilyGenes(path As String) As Dictionary(Of String, List(Of String))
        Dim table As New Dictionary(Of String, List(Of String))(StringComparer.Ordinal)

        For Each row As String() In ReadTable(path).Skip(1)
            If row.Length < 2 Then
                Continue For
            End If

            Dim family As String = row(0).Trim
            Dim gene As String = row(1).Trim

            If family = "" OrElse gene = "" Then
                Continue For
            End If

            Dim list As List(Of String) = Nothing

            If Not table.TryGetValue(family, list) Then
                list = New List(Of String)
                table(family) = list
            End If

            Call list.Add(gene)
        Next

        Return table
    End Function

    ''' <summary>
    ''' 由基因家族表构建 <see cref="UnionFind"/>（先注册全部基因，再按家族合并）
    ''' </summary>
    Private Function LoadFamilies(path As String, genomes As Dictionary(Of String, String())) As UnionFind
        Dim uf As New UnionFind()

        Call uf.AddElements(genomes.Values.SelectMany(Function(genes) genes))

        For Each family As KeyValuePair(Of String, List(Of String)) In LoadFamilyGenes(path)
            Call uf.UnionRange(family.Value)
        Next

        Return uf
    End Function

    Private Function LoadDesign(path As String) As Dictionary(Of String, SampleDesign)
        Dim table As New Dictionary(Of String, SampleDesign)(StringComparer.Ordinal)

        For Each row As String() In ReadTable(path).Skip(1)
            If row.Length < 4 Then
                Continue For
            End If

            table(row(0).Trim) = New SampleDesign With {
                .consortium = row(1).Trim,
                .herbicide = row(2).Trim,
                .qs = row(3).Trim
            }
        Next

        Return table
    End Function

    Private Function LoadTruth(dataDir As String, strains As IEnumerable(Of String)) As Dictionary(Of String, Dictionary(Of String, Double()))
        Dim truth As New Dictionary(Of String, Dictionary(Of String, Double()))(StringComparer.Ordinal)

        For Each strain As String In strains
            Dim truthFile As String = Path.Combine(dataDir, "truth", $"{strain}.csv")

            If File.Exists(truthFile) Then
                truth(strain) = RowsOf(Matrix.LoadData(truthFile))
            End If
        Next

        Return truth
    End Function
End Module
