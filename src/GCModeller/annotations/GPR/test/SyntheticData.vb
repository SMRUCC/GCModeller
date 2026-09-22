Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.GCModeller.CompilerServices.GPRLink
Imports SMRUCC.genomics.MetabolicModel

''' <summary>
''' 一个合成测试用例的数据集合：合成基因组 + 参考代谢网络 + 已知正确答案的说明
''' </summary>
Public Class SyntheticCase

    Public Property Name As String
    Public Property Description As String
    Public Property Genes As GeneTable()
    Public Property Pathways As Pathway()

    Public Function ReactionIds() As String()
        Return Pathways _
            .Where(Function(p) p IsNot Nothing) _
            .SelectMany(Function(p) If(p.metabolicNetwork, New MetabolicReaction() {})) _
            .Select(Function(r) r.id) _
            .Distinct(StringComparer.OrdinalIgnoreCase) _
            .ToArray()
    End Function

    Public Function GeneIds() As String()
        Return Genes.Select(Function(g) g.locus_id).ToArray()
    End Function

    ''' <summary>
    ''' 基因在排序后基因组中的物理索引
    ''' </summary>
    Public Function IndexOf(geneId As String) As Integer
        Dim ordered As GeneTable() = Genes.OrderBy(Function(g) g.left).ToArray()

        For i As Integer = 0 To ordered.Length - 1
            If String.Equals(ordered(i).locus_id, geneId, StringComparison.OrdinalIgnoreCase) Then Return i
        Next

        Return -1
    End Function

End Class

''' <summary>
''' 合成测试数据的构造工厂。
''' 
''' 所有数据完全由代码构造，不依赖任何外部文件，因此可以离线、可复现地运行，
''' 并且每一个用例都附带明确的"已知正确答案"（ground truth），可以直接写出断言。
''' </summary>
Public Module SyntheticData

    Public Const PlusStrand As String = "+"
    Public Const MinusStrand As String = "-"
    Public Const GeneLength As Integer = 1000

#Region "基础构造器"

    ''' <summary>
    ''' 构造一个代谢反应。底物与产物以化合物编号表示，
    ''' 化合物编号的衔接即构成了反应之间的化学连续性。
    ''' </summary>
    Public Function MakeReaction(id As String, ec As String, substrates As String(), products As String()) As MetabolicReaction
        Return New MetabolicReaction With {
            .id = id,
            .name = id,
            .ECNumbers = If(String.IsNullOrEmpty(ec), New String() {}, New String() {ec}),
            .left = substrates.Select(Function(c) New CompoundSpecieReference(1.0, c)).ToArray(),
            .right = products.Select(Function(c) New CompoundSpecieReference(1.0, c)).ToArray()
        }
    End Function

    ''' <summary>
    ''' 构造一条链式通路：``R1(C0-&gt;C1) -&gt; R2(C1-&gt;C2) -&gt; ...``
    ''' </summary>
    ''' <param name="pathwayId">通路编号，同时也是反应编号与 EC 编号的前缀</param>
    ''' <param name="name">通路名称</param>
    ''' <param name="reactionCount">链上的反应数目</param>
    Public Function MakeChain(pathwayId As String, name As String, reactionCount As Integer) As Pathway
        Dim reactions As New List(Of MetabolicReaction)

        For i As Integer = 1 To reactionCount
            reactions.Add(MakeReaction(
                ReactionId(pathwayId, i),
                EcOf(pathwayId, i),
                New String() {$"{pathwayId}_C{i - 1}"},
                New String() {$"{pathwayId}_C{i}"}))
        Next

        Return MakePathway(pathwayId, name, reactions.ToArray())
    End Function

    Public Function MakePathway(id As String, name As String, reactions As MetabolicReaction()) As Pathway
        Return New Pathway(reactions) With {
            .ID = id,
            .name = name,
            .metabolicNetwork = reactions,
            .metabolites = New MetabolicCompound() {}
        }
    End Function

    ''' <summary>
    ''' 通路 pathId 中第 index 个反应的编号（index 从 1 开始）
    ''' </summary>
    Public Function ReactionId(pathId As String, index As Integer) As String
        Return $"{pathId}_R{index}"
    End Function

    ''' <summary>
    ''' 通路 pathId 中第 index 个反应的 EC 编号
    ''' </summary>
    Public Function EcOf(pathId As String, index As Integer) As String
        Return $"{pathId}.{index}.1.1"
    End Function

    ''' <summary>
    ''' 构造一个基因（默认长度 1000bp）
    ''' </summary>
    Public Function MakeGene(locusId As String, left As Integer, strand As String, ec As String()) As GeneTable
        Return MakeGene(locusId, left, GeneLength, strand, ec)
    End Function

    Public Function MakeGene(locusId As String, left As Integer, length As Integer, strand As String, ec As String()) As GeneTable
        Return New GeneTable With {
            .locus_id = locusId,
            .geneName = locusId,
            .commonName = locusId,
            .left = left,
            .right = left + length,
            .strand = strand,
            .EC_Number = If(ec, New String() {})
        }
    End Function

    Private Function EmptyEC() As String()
        Return New String() {}
    End Function

    Private Function OneEC(ec As String) As String()
        Return New String() {ec}
    End Function

    Private Function TwoEC(first As String, second As String) As String()
        Return New String() {first, second}
    End Function

    Private Function ThreeEC(first As String, second As String, third As String) As String()
        Return New String() {first, second, third}
    End Function

    Private Function NewCase(name As String, description As String, genes As GeneTable(), pathways As Pathway()) As SyntheticCase
        Return New SyntheticCase With {
            .Name = name,
            .Description = description,
            .Genes = genes,
            .Pathways = pathways
        }
    End Function

#End Region

#Region "用例 1：直接 EC 匹配"

    ''' <summary>
    ''' 三个基因彼此相距超过 <see cref="GPRParameters.MaxPhysicalDistance"/>，
    ''' 因此除了自身 EC 匹配之外不可能获得任何上下文证据。
    ''' </summary>
    Public Function DirectEC() As SyntheticCase
        Dim p1 As Pathway = MakeChain("P1", "synthetic pathway P1", 3)
        Dim p2 As Pathway = MakeChain("P2", "synthetic pathway P2", 3)

        Dim genes As GeneTable() = New GeneTable() {
            MakeGene("g_direct1", 1000, PlusStrand, OneEC(EcOf("P1", 1))),
            MakeGene("g_direct2", 60000, PlusStrand, OneEC(EcOf("P2", 2))),
            MakeGene("g_noEC", 120000, PlusStrand, EmptyEC())
        }

        Return NewCase(
            "直接 EC 匹配",
            "g_direct1/g_direct2 各携带一个 EC，且彼此距离远超物理距离阈值；g_noEC 没有任何 EC。",
            genes, New Pathway() {p1, p2})
    End Function

#End Region

#Region "用例 2：操纵子上下文"

    ''' <summary>
    ''' 基因组末尾放置一个操纵子，用于验证"循环结束后 flush 最后一个操纵子"的修复。
    ''' </summary>
    Public Function Operon() As SyntheticCase
        Dim p1 As Pathway = MakeChain("P1", "synthetic pathway P1", 3)
        Dim p2 As Pathway = MakeChain("P2", "synthetic pathway P2", 3)

        ' g_op1 ~ g_op4 同链紧密排列构成主操纵子；g_lonely 与之相距很远；
        ' g_tail1 / g_tail2 构成位于基因组尾部的操纵子。
        Dim genes As GeneTable() = New GeneTable() {
            MakeGene("g_op1", 1000, PlusStrand, OneEC(EcOf("P1", 1))),
            MakeGene("g_op2", 2000, PlusStrand, OneEC(EcOf("P1", 2))),
            MakeGene("g_op3", 3000, PlusStrand, OneEC(EcOf("P1", 3))),
            MakeGene("g_op4", 4000, PlusStrand, EmptyEC()),
            MakeGene("g_lonely", 100000, PlusStrand, EmptyEC()),
            MakeGene("g_tail1", 200000, MinusStrand, OneEC(EcOf("P2", 1))),
            MakeGene("g_tail2", 201000, MinusStrand, OneEC(EcOf("P2", 2)))
        }

        Return NewCase(
            "操纵子上下文",
            "g_op4 / g_lonely 没有任何 EC；g_tail1+g_tail2 构成位于基因组尾部的操纵子。",
            genes, New Pathway() {p1, p2})
    End Function

#End Region

#Region "用例 3：距离衰减与链方向权重"

    Public Function DistanceDecay() As SyntheticCase
        Dim p1 As Pathway = MakeChain("P1", "synthetic pathway P1", 2)

        ' 三组"目标基因 + 携带相同 EC 的邻居"，区别仅在于物理距离与链方向。
        Dim genes As GeneTable() = New GeneTable() {
            MakeGene("g_near", 1000, PlusStrand, EmptyEC()),
            MakeGene("g_near_ec", 3000, PlusStrand, OneEC(EcOf("P1", 1))),
            MakeGene("g_far", 100000, PlusStrand, EmptyEC()),
            MakeGene("g_far_ec", 104000, PlusStrand, OneEC(EcOf("P1", 1))),
            MakeGene("g_cross", 200000, PlusStrand, EmptyEC()),
            MakeGene("g_cross_ec", 202000, MinusStrand, OneEC(EcOf("P1", 1)))
        }

        Return NewCase(
            "距离衰减与链方向",
            "g_near / g_far / g_cross 都是无 EC 的目标基因，各自旁边有一个携带相同 EC 的邻居，" &
            "区别仅在于距离与链方向。",
            genes, New Pathway() {p1})
    End Function

#End Region

#Region "用例 4：酶复合体"

    Public Function EnzymeComplex() As SyntheticCase
        Dim p1 As Pathway = MakeChain("P1", "synthetic pathway P1", 3)

        ' g_cpx1 / g_cpx2 携带完全相同的 EC，间距 1000bp（超过操纵子阈值但未超过复合体阈值）；
        ' g_cpx3 携带不同 EC；g_cpx4 携带相同 EC 但距离过远。
        Dim genes As GeneTable() = New GeneTable() {
            MakeGene("g_cpx1", 1000, PlusStrand, OneEC(EcOf("P1", 1))),
            MakeGene("g_cpx2", 3000, PlusStrand, OneEC(EcOf("P1", 1))),
            MakeGene("g_cpx3", 5000, PlusStrand, OneEC(EcOf("P1", 2))),
            MakeGene("g_cpx4", 100000, PlusStrand, OneEC(EcOf("P1", 1)))
        }

        Return NewCase(
            "酶复合体",
            "g_cpx1 与 g_cpx2 携带相同 EC 且间距 1000bp，应当被识别为一个复合体；g_cpx3 携带不同 EC。",
            genes, New Pathway() {p1})
    End Function

#End Region

#Region "用例 5：融合基因与通路完整度"

    Public Function FusionAndCompleteness() As SyntheticCase
        ' P1 主链 4 个反应；P2 只有 1 个反应，与 P1 之间没有任何化学联系
        Dim p1 As Pathway = MakeChain("P1", "synthetic pathway P1", 4)
        Dim p2 As Pathway = MakeChain("P2", "synthetic pathway P2", 1)

        ' g_fusion 的 3 个 EC 对应的反应在 P1 主链上连续；
        ' g_split 的两个 EC 分别落在 P1 与 P2，跨通路因而不构成融合酶。
        Dim genes As GeneTable() = New GeneTable() {
            MakeGene("g_fusion", 1000, PlusStrand,
                     ThreeEC(EcOf("P1", 1), EcOf("P1", 2), EcOf("P1", 3))),
            MakeGene("g_split", 100000, PlusStrand,
                     TwoEC(EcOf("P1", 1), EcOf("P2", 1)))
        }

        Return NewCase(
            "融合基因与通路完整度",
            "g_fusion 携带 P1 主链前 3 个连续反应的 EC（覆盖率 3/4 = 0.75，应当触发通路完整度补缺）；" &
            "g_split 携带 P1 第 1 个反应与 P2 唯一反应的 EC（两个反应不在同一条通路上）。",
            genes, New Pathway() {p1, p2})
    End Function

#End Region

#Region "综合演示"

    ''' <summary>
    ''' 综合演示用例：把操纵子、酶复合体、融合基因、未映射 EC、无 EC 基因等各种情形
    ''' 集中在一个中等规模的合成基因组中，用于生成最终的关联结果表与 CSV 导出。
    ''' </summary>
    Public Function Demo() As SyntheticCase
        Dim p1 As Pathway = MakeChain("P1", "glycolysis (synthetic)", 5)
        Dim p2 As Pathway = MakeChain("P2", "TCA cycle (synthetic)", 6)
        Dim p3 As Pathway = MakeChain("P3", "amino acid biosynthesis (synthetic)", 4)
        Dim p4 As Pathway = MakeChain("P4", "fatty acid metabolism (synthetic)", 3)

        Dim genes As GeneTable() = New GeneTable() {
            MakeGene("GMP001", 1000, PlusStrand, OneEC(EcOf("P1", 1))),
            MakeGene("GMP002", 2000, PlusStrand, OneEC(EcOf("P1", 2))),
            MakeGene("GMP003", 3000, PlusStrand, OneEC(EcOf("P1", 3))),
            MakeGene("GMP004", 4000, PlusStrand, EmptyEC()),
            MakeGene("GMP005", 20000, PlusStrand, OneEC(EcOf("P2", 1))),
            MakeGene("GMP006", 22000, PlusStrand, OneEC(EcOf("P2", 2))),
            MakeGene("GMP007", 24000, MinusStrand, OneEC(EcOf("P2", 3))),
            MakeGene("GMP008", 26000, PlusStrand, OneEC(EcOf("P2", 4))),
            MakeGene("GMP009", 28000, PlusStrand, OneEC(EcOf("P2", 5))),
            MakeGene("GMP010", 50000, PlusStrand,
                     ThreeEC(EcOf("P3", 1), EcOf("P3", 2), EcOf("P3", 3))),
            MakeGene("GMP011", 52000, PlusStrand, OneEC(EcOf("P3", 4))),
            MakeGene("GMP012", 80000, PlusStrand, OneEC(EcOf("P4", 1))),
            MakeGene("GMP013", 82000, PlusStrand,
                     TwoEC(EcOf("P4", 2), EcOf("P4", 3))),
            MakeGene("GMP014", 100000, PlusStrand, OneEC(EcOf("P1", 4))),
            MakeGene("GMP015", 101500, PlusStrand, OneEC(EcOf("P1", 4))),
            MakeGene("GMP016", 120000, PlusStrand, OneEC("9.9.9.9")),
            MakeGene("GMP017", 140000, PlusStrand, EmptyEC()),
            MakeGene("GMP018", 141500, PlusStrand, EmptyEC())
        }

        Return NewCase(
            "综合演示",
            "4 条合成通路（5 + 6 + 4 + 3 个反应）与 18 个基因，" &
            "包含操纵子、酶复合体、融合基因、未映射 EC 以及无 EC 注释的基因。",
            genes, New Pathway() {p1, p2, p3, p4})
    End Function

#End Region

#Region "用例 6：上下文作用域收敛（噪声回归）"

    ''' <summary>
    ''' 这是整个 Demo 中最关键的一条回归断言。
    ''' 
    ''' 历史实现的做法是：只要邻居基因的某个 EC 命中了一个反应，就把"该反应所在的整条通路"的
    ''' 全部反应都灌上分数。于是在这个用例中，g_context 会因为邻居携带 P1 主链的 EC，
    ''' 而同时被关联到通路内那个与主链完全无关的 P1_R_orphan 上。
    ''' 
    ''' 重构之后的实现只把证据落在"邻居基因自己能够催化的那个反应"上，
    ''' 因此 g_context 与 P1_R_orphan 之间必须不存在任何关联。
    ''' </summary>
    Public Function NoiseConvergence() As SyntheticCase
        Dim p1 As Pathway = MakeChain("P1", "synthetic pathway P1", 4)

        ' 在 P1 中追加一个与主链完全不相连的诱饵反应
        Dim orphan As MetabolicReaction = MakeReaction("P1_R_orphan", "P1_orphan.9.9.9", New String() {"P1_X"}, New String() {"P1_Y"})
        Dim reactions As MetabolicReaction() = p1.metabolicNetwork.Concat(New MetabolicReaction() {orphan}).ToArray()
        p1 = MakePathway("P1", "synthetic pathway P1", reactions)

        ' g_context 紧邻主链的四个基因但没有自己的 EC；诱饵反应的唯一携带者 g_orphan 远在 500kbp 之外。
        ' 这里使用长度 400bp 的基因并且彼此间隔 1000bp，使它们落在同一个滑动窗口内、
        ' 但基因间距（600bp）超过操纵子阈值，从而把这段用例隔离在"窗口上下文"这一条代码路径上。
        Dim genes As GeneTable() = New GeneTable() {
            MakeGene("g_ctx1", 1000, 400, PlusStrand, OneEC(EcOf("P1", 1))),
            MakeGene("g_ctx2", 2000, 400, PlusStrand, OneEC(EcOf("P1", 2))),
            MakeGene("g_ctx3", 3000, 400, PlusStrand, OneEC(EcOf("P1", 3))),
            MakeGene("g_ctx4", 4000, 400, PlusStrand, OneEC(EcOf("P1", 4))),
            MakeGene("g_context", 5000, 400, PlusStrand, EmptyEC()),
            MakeGene("g_orphan", 500000, 400, PlusStrand, OneEC("P1_orphan.9.9.9"))
        }

        Return NewCase(
            "上下文作用域收敛",
            "g_context 紧邻 P1 主链的四个基因但没有自己的 EC；诱饵反应 P1_R_orphan 虽然属于 P1，" &
            "却与这四个邻居的 EC 完全无关，其唯一的携带者 g_orphan 远在 500kbp 之外。",
            genes, New Pathway() {p1})
    End Function

#End Region

#Region "用例 7：共表达"

    Public Function Coexpression() As SyntheticCase
        Dim p1 As Pathway = MakeChain("P1", "synthetic pathway P1", 2)

        ' 三个基因之间物理距离都超过阈值，物理上下文证据不可能把它们关联起来。
        Dim genes As GeneTable() = New GeneTable() {
            MakeGene("g_expr1", 1000, PlusStrand, OneEC(EcOf("P1", 1))),
            MakeGene("g_expr2", 300000, PlusStrand, EmptyEC()),
            MakeGene("g_expr3", 600000, PlusStrand, EmptyEC())
        }

        Return NewCase(
            "共表达",
            "g_expr1 携带 P1 主链第 1 个反应的 EC；g_expr2 与 g_expr1 完全正相关，" &
            "g_expr3 与 g_expr1 完全负相关；三者之间物理距离都超过阈值。",
            genes, New Pathway() {p1})
    End Function

    ''' <summary>
    ''' 为共表达用例构造表达矩阵：g_expr2 与 g_expr1 完全正相关，g_expr3 完全负相关。
    ''' </summary>
    Public Function CoexpressionMatrix() As SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
        Return New SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix With {
            .tag = "gpr-demo",
            .sampleID = New String() {"s1", "s2", "s3", "s4", "s5"},
            .expression = New SMRUCC.genomics.Analysis.HTS.DataFrame.DataFrameRow() {
                New SMRUCC.genomics.Analysis.HTS.DataFrame.DataFrameRow("g_expr1", New Double() {1.0, 2.0, 3.0, 4.0, 5.0}),
                New SMRUCC.genomics.Analysis.HTS.DataFrame.DataFrameRow("g_expr2", New Double() {2.0, 4.0, 6.0, 8.0, 10.0}),
                New SMRUCC.genomics.Analysis.HTS.DataFrame.DataFrameRow("g_expr3", New Double() {5.0, 4.0, 3.0, 2.0, 1.0})
            }
        }
    End Function

#End Region

#Region "用例 8：保守共线性"

    Public Function ConservedSynteny() As SyntheticCase
        Dim p1 As Pathway = MakeChain("P1", "synthetic pathway P1", 2)

        Dim genes As GeneTable() = New GeneTable() {
            MakeGene("g_syn1", 1000, PlusStrand, EmptyEC()),
            MakeGene("g_syn2", 3000, PlusStrand, EmptyEC()),
            MakeGene("g_syn3", 5000, PlusStrand, EmptyEC()),
            MakeGene("g_syn4", 7000, PlusStrand, EmptyEC()),
            MakeGene("g_syn5", 9000, PlusStrand, EmptyEC())
        }

        Return NewCase(
            "保守共线性",
            "五个基因都没有 EC；已知的保守簇 {g_syn1, g_syn2, g_syn3, g_syn4} 记录了功能 " & EcOf("P1", 1) & "。",
            genes, New Pathway() {p1})
    End Function

    ''' <summary>
    ''' 构造保守簇参考数据。
    ''' 
    ''' 历史实现在这里把 ConservedCluster 对象本身当作集合传给了 Intersect，
    ''' 在 Option Strict Off 下可以编译，但运行时必然抛出类型转换异常。
    ''' </summary>
    Public Function ConservedClusters() As Dictionary(Of String, ConservedCluster)
        Dim clusters As New Dictionary(Of String, ConservedCluster)(StringComparer.OrdinalIgnoreCase)

        clusters("CL001") = New ConservedCluster With {
            .ClusterID = "CL001",
            .geneIDs = New String() {"g_syn1", "g_syn2", "g_syn3", "g_syn4"},
            .functions = New String() {EcOf("P1", 1)}
        }

        Return clusters
    End Function

#End Region

#Region "用例 9：边界与健壮性"

    Public Function EmptyGenome() As SyntheticCase
        Return NewCase("空基因组", "既不包含基因，也不包含通路。", New GeneTable() {}, New Pathway() {})
    End Function

    Public Function Degenerate() As SyntheticCase
        ' 没有 EC、没有底物/产物的退化反应
        Dim degenerateRxn As New MetabolicReaction With {
            .id = "P1_R_degenerate",
            .name = "degenerate reaction"
        }

        ' 空通路
        Dim empty As Pathway = MakePathway("P0", "empty pathway", New MetabolicReaction() {})

        Dim chain As Pathway = MakeChain("P1", "synthetic pathway P1", 2)
        Dim p1 As Pathway = MakePathway("P1", chain.name, chain.metabolicNetwork.Concat(New MetabolicReaction() {degenerateRxn}).ToArray())

        ' 故意不按坐标顺序传入，验证基因组上下文能够自行排序
        Dim genes As GeneTable() = New GeneTable() {
            MakeGene("g_out2", 90000, PlusStrand, OneEC(EcOf("P1", 2))),
            MakeGene("g_out1", 1000, MinusStrand, OneEC(EcOf("P1", 1))),
            MakeGene("g_unmapped", 200000, PlusStrand, OneEC("R0.0.0.0")),
            MakeGene("g_emptyEC", 300000, PlusStrand, EmptyEC())
        }

        Return NewCase(
            "退化输入",
            "包含空通路、没有 EC 与底物/产物的退化反应、未映射的 EC 编号，以及未按坐标排序的基因组。",
            genes, New Pathway() {empty, p1})
    End Function

#End Region

#Region "用例 10：规模与性能"

    ''' <summary>
    ''' 生成一个较大的基因组（默认 600 个基因 / 12 条通路 / 120 个反应），
    ''' 用于验证算法在大规模输入下不会出现性能退化，并且结果完全可复现。
    ''' </summary>
    Public Function Stress(geneCount As Integer,
                           Optional pathwayCount As Integer = 12,
                           Optional reactionPerPathway As Integer = 10) As SyntheticCase

        Dim pathways As New List(Of Pathway)

        For p As Integer = 1 To pathwayCount
            pathways.Add(MakeChain($"S{p}", $"stress pathway S{p}", reactionPerPathway))
        Next

        Dim genes As New List(Of GeneTable)
        Dim position As Integer = 1000
        Dim reactionCursor As Integer = 0

        For i As Integer = 1 To geneCount
            Dim pathwayNumber As Integer = (i Mod pathwayCount) + 1
            Dim ec As String()

            ' 每 3 个基因里安排 1 个没有 EC 的基因，模拟真实的注释缺失
            If i Mod 3 = 0 Then
                ec = New String() {}
            Else
                reactionCursor += 1
                If reactionCursor > reactionPerPathway Then reactionCursor = 1

                ec = New String() {EcOf($"S{pathwayNumber}", reactionCursor)}
            End If

            Dim strand As String = If(i Mod 2 = 0, PlusStrand, MinusStrand)

            genes.Add(MakeGene($"g_stress_{i:D4}", position, strand, ec))

            ' 交替使用紧密排列与较大间隔，模拟操纵子与基因岛
            position += If(i Mod 5 = 0, 20000, 1500)
        Next

        Return NewCase(
            $"规模与性能 ({geneCount} 基因 / {pathways.Count} 通路)",
            "由确定性的规则生成的合成基因组，用于验证性能与结果可复现性。",
            genes.ToArray(),
            pathways.ToArray())
    End Function

#End Region

End Module
