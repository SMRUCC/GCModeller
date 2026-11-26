Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Summary
Imports std = System.Math

''' <summary>
''' 一个完全在内存中构造的虚构任务：30 个基因、3 个扰动、2 行训练摘要、2 个上下文控制矩阵。
''' </summary>
''' <remarks>
''' <para>
''' 构造方式对齐上游 <c>examples/synthetic/run_example.py</c>：基因命名、扰动取自基因轴、
''' 效应矩阵里刻意留下"未测"（<c>NaN</c>）与"测得 0"，用于验证下游是否把两者区分开。
''' </para>
''' <para>
''' 两个训练行的基因轴<b>不完全相同</b>（第二行缺少几个基因、并且多一个第一行没有的基因），
''' 这样 <c>StackEffects</c> 的并集逻辑与 <c>AlignPanel</c> 的缺失填 <c>NaN</c> 逻辑都会被覆盖到。
''' </para>
''' </remarks>
Public Class SyntheticTask

    ''' <summary>任务基因轴（30 个）。</summary>
    Public Property Genes As String()

    ''' <summary>任务扰动轴（3 个，取值是基因轴上的名字）。</summary>
    Public Property Perturbations As String()

    ''' <summary>训练摘要（内存版）。</summary>
    Public Property Bundle As SummaryBundle

    ''' <summary>上下文名称（2 个）。</summary>
    Public Property Contexts As String()

    ''' <summary>上下文控制细胞矩阵（细胞 × 基因，按 <see cref="Genes"/>）。</summary>
    Public Property Controls As CsrMatrix()

    Public ReadOnly Property GeneCount As Integer
        Get
            Return Genes.Length
        End Get
    End Property

    Public ReadOnly Property PerturbationCount As Integer
        Get
            Return Perturbations.Length
        End Get
    End Property

End Class

''' <summary>
''' 虚构任务的构造器（确定性，同 seed 同结果）。
''' </summary>
Public Module SyntheticFixture

    Public Const DefaultSeed As Integer = 20261126

    ''' <summary>构造一个可直接用于各阶段审计的虚构任务。</summary>
    Public Function Create(Optional seed As Integer = DefaultSeed,
                           Optional nCells As Integer = 120) As SyntheticTask
        Dim rnd As New Random(seed)
        Dim genes As String() = MakeGenes(30)
        Dim perturbations As String() = {genes(0), genes(9), genes(19)}
        Dim lines As String() = {"line_A", "line_B"}
        Dim contexts As String() = {"context_A", "context_B"}

        ' ---- 两个训练行：基因轴不同 ----
        Dim axisA As String() = genes
        Dim axisB As String() = RemoveSome(genes, {"g004", "g017"})
        Dim summaryRows As SummaryRow() = {
            MakeRow(rnd, lines(0), axisA, perturbations, auxiliary:=False),
            MakeRow(rnd, lines(1), axisB, perturbations, auxiliary:=False)
        }
        Dim bundle As New SummaryBundle With {
            .Catalog = New SummaryCatalog With {
                .PrimaryView = "view_0",
                .Rows = summaryRows
            }
        }

        ' ---- 方差表（两份都覆盖完整的并集）----
        For Each line As String In lines
            Dim variance(genes.Length - 1) As Double

            For i As Integer = 0 To genes.Length - 1
                variance(i) = 0.04 + 0.01 * (i Mod 5)
            Next

            bundle.VarianceGenes(line) = DirectCast(genes.Clone(), String())
            bundle.Variance(line) = variance
        Next

        ' ---- 控制矩阵（两个上下文，各 nCells 个细胞）----
        Dim controls(contexts.Length - 1) As CsrMatrix

        For c As Integer = 0 To contexts.Length - 1
            Dim dense As Double()() = RectangularArray.Matrix(Of Double)(nCells, genes.Length)

            For i As Integer = 0 To nCells - 1
                For g As Integer = 0 To genes.Length - 1
                    ' 低表达 + 少量高表达基因，保证 eligible 与 ineligible 都存在
                    Dim rate As Double = If(g Mod 7 = 0, 12.0, 1.5)

                    dense(i)(g) = std.Floor(rate * (0.4 + rnd.NextDouble()))
                Next
            Next

            controls(c) = CompressedBuilder.FromDense(dense)
        Next

        Return New SyntheticTask With {
            .Genes = genes,
            .Perturbations = perturbations,
            .Bundle = bundle,
            .Contexts = contexts,
            .Controls = controls
        }
    End Function

    ''' <summary>构造一条摘要行。</summary>
    Private Function MakeRow(rnd As Random, line As String, genes As String(),
                             perturbations As String(), auxiliary As Boolean) As SummaryRow
        Dim nGenes As Integer = genes.Length
        Dim nPerts As Integer = perturbations.Length
        Dim effect As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)
        Dim probability As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)
        Dim center(nGenes - 1) As Double

        For g As Integer = 0 To nGenes - 1
            Dim sum As Double = 0.0

            For p As Integer = 0 To nPerts - 1
                Dim self As Boolean = String.Equals(genes(g), perturbations(p), StringComparison.Ordinal)

                ' 自身靶基因的效应最强；相邻基因弱一些；其余接近 0
                Dim magnitude As Double = If(self, -1.5 - rnd.NextDouble(),
                                          If(g Mod 9 = 0, 0.6 + 0.4 * rnd.NextDouble(), 0.02 * rnd.NextDouble()))

                ' 刻意留下"未测"：每行每列都有少量 NaN
                If rnd.NextDouble() < 0.04 Then
                    effect(g)(p) = Double.NaN
                    probability(g)(p) = Double.NaN
                Else
                    effect(g)(p) = magnitude
                    probability(g)(p) = If(std.Abs(magnitude) > 0.5, 0.001, 0.4)
                End If

                If Not Double.IsNaN(effect(g)(p)) Then
                    sum += effect(g)(p)
                End If
            Next

            center(g) = sum / nPerts
        Next

        Dim counts(nGenes - 1) As Double
        Dim signedCenter(nGenes - 1) As Double

        For g As Integer = 0 To nGenes - 1
            counts(g) = 40.0 + rnd.NextDouble() * 400.0
            signedCenter(g) = 0.0
        Next

        Dim offset As New OffsetTable With {
            .CellLine = line,
            .View = "view_0",
            .Genes = genes,
            .EffectCenter = center,
            .EffectCount = Enumerable.Repeat(CDbl(nPerts), nGenes).ToArray,
            .SignedScoreCenter = signedCenter,
            .ResponseVariance = Enumerable.Range(0, nGenes).Select(Function(i) 0.04 + 0.01 * (i Mod 5)).ToArray
        }

        Return New SummaryRow With {
            .CellLine = line,
            .View = "view_0",
            .Role = If(auxiliary, "auxiliary", "primary"),
            .Offset = offset,
            .EffectGenes = genes,
            .EffectPerts = perturbations,
            .Effect = effect,
            .Probability = probability,
            .Control = New ControlProfile With {.Genes = genes, .Counts = counts}
        }
    End Function

    Private Function MakeGenes(n As Integer) As String()
        Dim genes(n - 1) As String

        For i As Integer = 0 To n - 1
            genes(i) = $"g{(i + 1).ToString("D3")}"
        Next

        Return genes
    End Function

    Private Function RemoveSome(genes As String(), removed As String()) As String()
        Dim drop As New HashSet(Of String)(removed, StringComparer.Ordinal)

        Return genes.Where(Function(g) Not drop.Contains(g)).ToArray
    End Function

End Module
