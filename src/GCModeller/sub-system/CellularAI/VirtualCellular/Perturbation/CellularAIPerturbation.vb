Imports System
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Model
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Placement
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Randomizer
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Summary
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Synthesis
Imports SMRUCC.genomics.Analysis.VirtualCellular.Data
Imports SMRUCC.genomics.Analysis.VirtualCellular.Model
Imports SMRUCC.genomics.BioSystem
Imports SMRUCC.genomics.BioSystem.Intervention
Imports std = System.Math

Namespace Perturbation

    ''' <summary>
    ''' 基于 CellularAI 的虚拟扰动实验：把 KO / KD / OE 映射成合成作业的方向与强度。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <b>扰动的两个来源</b>：
    ''' </para>
    ''' <list type="number">
    ''' <item><description><b>自身靶</b>：直接给定目标基因的 log2 变化方向与强度（OE 取正、KO 取强负、KD 取弱负）；</description></item>
    ''' <item><description><b>下游响应</b>：从模型的 <c>signature</c> view 取该基因的"高表达签名"面板，
    ''' 作为其他基因的效应方向（联合过表达时把多个签名叠加）。</description></item>
    ''' </list>
    ''' <para>
    ''' 合成完全走 CellularAI 的 <see cref="SynthesisEngine"/> + <see cref="CellSynthesizer"/>，
    ''' 因此计数守恒、<c>2²⁴</c> 上限、同 seed 可复现这些约束与算法层一致。
    ''' </para>
    ''' </remarks>
    Public Class CellularAIPerturbation : Implements InsilicoPerturbationExperiment

        Private ReadOnly m_model As VirtualCellModel
        Private ReadOnly m_seed As Integer
        Private m_mode As ContextAlignmentMode = ContextAlignmentMode.None
        Private m_context As Dictionary(Of String, Double)

        ''' <summary>最近一次扰动的诊断信息。</summary>
        Public Property Diagnostics As Dictionary(Of String, Double)

        ''' <summary>
        ''' 默认合成细胞数：接口方法的 <c>nSamples</c> 传 0 时使用。
        ''' </summary>
        ''' <remarks>
        ''' <c>InsilicoPerturbationExperiment</c> 的 <c>nSamples</c> 是可选的，
        ''' 由 <c>intervention</c> 包调用时不会传值；把规模放在对象上，
        ''' 既保持了接口签名不变，又让 R# 侧可以在 <c>as_perturbation</c> 里一次性配置。
        ''' </remarks>
        Public Property DefaultCells As Integer = 400

        Public Sub New(model As VirtualCellModel, Optional seed As Integer = 0)
            If model Is Nothing Then
                Throw New ArgumentNullException(NameOf(model))
            End If

            m_model = model
            m_seed = seed
            Me.Diagnostics = New Dictionary(Of String, Double)(StringComparer.Ordinal)
        End Sub

        ''' <summary>模型。</summary>
        Public ReadOnly Property Model As VirtualCellModel
            Get
                Return m_model
            End Get
        End Property

        ''' <summary>当前上下文对齐模式。</summary>
        Public ReadOnly Property AlignmentMode As ContextAlignmentMode
            Get
                Return m_mode
            End Get
        End Property

        ''' <summary>是否已注入上下文。</summary>
        Public ReadOnly Property HasContext As Boolean
            Get
                Return m_context IsNot Nothing AndAlso m_context.Count > 0
            End Get
        End Property

        Public Function SetContextAlignmentMode(mode As ContextAlignmentMode) As InsilicoPerturbationExperiment Implements InsilicoPerturbationExperiment.SetContextAlignmentMode
            m_mode = mode

            Return Me
        End Function

        ''' <summary>
        ''' 注入上下文：以"基因名 → 表达量"覆盖野生型基线（未覆盖的基因沿用模型对照均值）。
        ''' </summary>
        Public Function SetContext(context As Dictionary(Of String, Double)) As InsilicoPerturbationExperiment Implements InsilicoPerturbationExperiment.SetContext
            If context Is Nothing OrElse context.Count = 0 Then
                Throw New ArgumentException("the context is empty", NameOf(context))
            End If

            Dim map As Dictionary(Of String, Integer) = m_model.GeneIndex()
            Dim matched As Integer = 0

            For Each gene As String In context.Keys
                If map.ContainsKey(gene) Then
                    matched += 1
                End If
            Next

            If matched = 0 Then
                Throw New InvalidOperationException(
                    "none of the context genes matches the model gene axis; check the identifier space")
            End If

            m_context = context

            Return Me
        End Function

        ''' <summary>撤销上下文。</summary>
        Public Sub ClearContext()
            m_context = Nothing
        End Sub

        ''' <summary>虚拟敲除：目标基因置零（近似 −12 log2）。</summary>
        Public Function KnockoutGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.KnockoutGene
            Return Run({geneName}, InterventionMode.Knockout, nSamples)
        End Function

        ''' <summary>虚拟敲低：目标基因下调（−1 log2）。</summary>
        Public Function KnockDownGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.KnockDownGene
            Return Run({geneName}, InterventionMode.Knockdown, nSamples)
        End Function

        ''' <summary>虚拟过表达：目标基因上调（+2 log2）。</summary>
        Public Function OverexpressGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult Implements InsilicoPerturbationExperiment.OverexpressGene
            Return Run({geneName}, InterventionMode.Overexpression, nSamples)
        End Function

        ''' <summary>
        ''' 联合过表达（接口只定义了单基因，联合场景走这个入口）。
        ''' </summary>
        Public Function OverexpressGenes(geneNames As String(), Optional nSamples As Integer = 0) As InterventionResult
            Return Run(geneNames, InterventionMode.Overexpression, nSamples)
        End Function

#Region "run"

        ''' <summary>一次虚拟扰动的完整执行。</summary>
        Public Function Run(targets As String(), mode As InterventionMode, nSamples As Integer) As InterventionResult
            Dim genes As String() = m_model.Genes
            Dim map As Dictionary(Of String, Integer) = m_model.GeneIndex()
            Dim indices As New List(Of Integer)
            Dim names As New List(Of String)

            For Each target As String In targets
                Dim at As Integer = -1

                If map.TryGetValue(target, at) AndAlso Not indices.Contains(at) Then
                    Call indices.Add(at)
                    Call names.Add(genes(at))
                End If
            Next

            If indices.Count = 0 Then
                Return Undefined(targets, mode)
            End If

            Dim cells As Integer = If(nSamples > 0, nSamples, std.Max(DefaultCells, 1))
            Dim engine As SynthesisEngine = SynthesisEngine.Create(m_model.Controls, m_model.Options)
            Dim wildtype As Double() = ControlMeanCpm(engine)
            Dim wildtypeSd As Double() = ControlSd(engine)

            ' ---- 1) 下游响应：叠加签名面板 ----
            Dim response As Double() = SignaturePanel(names.ToArray, wildtype)

            ' ---- 2) 逐基因的目标 z ----
            Dim z(nModels - 1) As Double
            Dim scale As Double = If(mode = InterventionMode.Overexpression, 1.0, 1.0)

            For g As Integer = 0 To nModels - 1
                Dim path As Double = response(g) * 4.0

                z(g) = std.Max(std.Min(path, 8.0), -8.0)
            Next

            For Each at As Integer In indices
                Dim magnitude As Double = If(mode = InterventionMode.Knockout, -8.0,
                                         If(mode = InterventionMode.Knockdown, -4.0, 8.0))

                z(at) = magnitude * scale
            Next

            ' ---- 3) 总量与文库 ----
            Dim totals(nModels - 1) As Double
            Dim librarySize As Double = std.Max(engine.Depth, 1.0)
            Dim weights(nModels - 1) As Double
            Dim weightSum As Double = 0.0

            For g As Integer = 0 To nModels - 1
                weights(g) = std.Max(engine.Fractions(g) * std.Pow(2.0, z(g)), 1.0E-12)
                weightSum += weights(g)
            Next

            Dim grand As Double = cells * librarySize
            Dim assigned As Double = 0.0

            For g As Integer = 0 To nModels - 1
                totals(g) = std.Floor(grand * weights(g) / weightSum)

                If totals(g) < 1.0 Then
                    totals(g) = 1.0
                End If

                assigned += totals(g)
            Next

            Call Balance(totals, grand - assigned)

            Dim libraries(cells - 1) As Double

            For c As Integer = 0 To cells - 1
                libraries(c) = librarySize
            Next

            Dim job As New SynthesisJob With {
                .Name = String.Join("+", names.ToArray),
                .Totals = totals,
                .Libraries = libraries,
                .Targets = ScaleToCpm(totals, grand),
                .PlanIndex = indices.ToArray,
                .PlanSign = indices.Select(Function(i) If(mode = InterventionMode.Overexpression, 1.0, -1.0)).ToArray,
                .PlanPriority = indices.Select(Function(i) 1.0).ToArray,
                .NudgeSign = New SByte(nModels - 1) {},
                .NudgeStrength = New Double(nModels - 1) {},
                .Seed = PerturbationSeed.Combine(m_seed, job_name(names.ToArray, mode))
            }

            Dim block As CsrMatrix = CellSynthesizer.Synthesize(engine, job, m_model.Options)
            Dim mutant As Double() = BlockMeanCpm(block, librarySize)
            Dim result As InterventionResult = Summarize(names.ToArray, mode, indices, wildtype, wildtypeSd, mutant)

            Me.Diagnostics("cells") = block.RowDimension
            Me.Diagnostics("nnz") = block.NonZeros
            Me.Diagnostics("depth") = librarySize

            Return result
        End Function

        Private ReadOnly Property nModels As Integer
            Get
                Return m_model.GeneCount
            End Get
        End Property

        Private Function job_name(names As String(), mode As InterventionMode) As String
            Return $"{String.Join("+", names)}_{mode}"
        End Function

        ''' <summary>签名面板：把目标基因的 <c>&lt;gene&gt;_high</c> 面板叠加成下游响应。</summary>
        Private Function SignaturePanel(names As String(), wildtype As Double()) As Double()
            Dim response(nModels - 1) As Double
            Dim rows As SummaryRow() = If(m_model.Bundle?.Catalog?.Rows, New SummaryRow() {})
            Dim found As Integer = 0

            For g As Integer = 0 To nModels - 1
                For Each row As SummaryRow In rows
                    If Not String.Equals(row.View, SummaryAssembly.ViewSignature, StringComparison.Ordinal) Then
                        Continue For
                    End If

                    Dim perms As String() = If(row.EffectPerts, New String() {})

                    For p As Integer = 0 To perms.Length - 1
                        For Each target As String In names
                            If String.Equals(perms(p), $"{target}_high", StringComparison.OrdinalIgnoreCase) Then
                                response(g) += row.Effect(g)(p)
                                found += 1
                            End If
                        Next
                    Next
                Next
            Next

            If found > 0 Then
                For g As Integer = 0 To nModels - 1
                    response(g) /= found
                Next
            End If

            Return response
        End Function

        Private Function ControlMeanCpm(engine As SynthesisEngine) As Double()
            Dim result(nModels - 1) As Double

            For g As Integer = 0 To nModels - 1
                result(g) = engine.MeanCpm(g)
            Next

            Return result
        End Function

        Private Function ControlSd(engine As SynthesisEngine) As Double()
            Dim result(nModels - 1) As Double

            For g As Integer = 0 To nModels - 1
                result(g) = std.Max(engine.MeanCpm(g), 1.0)
            Next

            Return result
        End Function

        Private Function ScaleToCpm(totals As Double(), grand As Double) As Double()
            Dim result(nModels - 1) As Double
            Dim normalizer As Double = If(grand > 0, 1000000.0 / grand, 0.0)

            For g As Integer = 0 To nModels - 1
                result(g) = totals(g) * normalizer
            Next

            Return result
        End Function

        Private Function BlockMeanCpm(block As CsrMatrix, depth As Double) As Double()
            Dim sums(nModels - 1) As Double
            Dim rows As Integer = block.RowDimension

            For c As Integer = 0 To rows - 1
                For k As Integer = block.Indptr(c) To block.Indptr(c + 1) - 1
                    sums(block.Indices(k)) += block.Values(k)
                Next
            Next

            Dim result(nModels - 1) As Double
            Dim n As Double = std.Max(rows, 1)

            For g As Integer = 0 To nModels - 1
                result(g) = 1000000.0 * (sums(g) / n) / std.Max(depth, 1.0)
            Next

            Return result
        End Function

        Private Function Summarize(names As String(), mode As InterventionMode, indices As List(Of Integer),
                                   wildtype As Double(), wildtypeSd As Double(), mutant As Double()) As InterventionResult
            Dim fold(nModels - 1) As Double
            Dim percent(nModels - 1) As Double
            Dim z(nModels - 1) As Double
            Dim significant(nModels - 1) As Boolean
            Dim targets As New HashSet(Of Integer)(indices)

            For g As Integer = 0 To nModels - 1
                fold(g) = mutant(g) - wildtype(g)

                If std.Abs(wildtype(g)) > 1.0E-9 Then
                    percent(g) = 100.0 * fold(g) / std.Abs(wildtype(g))
                End If

                z(g) = fold(g) / std.Max(wildtypeSd(g), 1.0E-6)
                significant(g) = targets.Contains(g) OrElse std.Abs(z(g)) > 3.0
            Next

            Return New InterventionResult With {
                .Spec = New InterventionSpec With {
                    .GeneName = String.Join("+", names),
                    .GeneIndex = indices(0),
                    .Mode = mode
                },
                .WildtypeMeans = wildtype,
                .WildtypeSDs = wildtypeSd,
                .MutantMeans = mutant,
                .FoldChanges = fold,
                .PercentChanges = percent,
                .ZScores = z,
                .IsSignificant = significant,
                .GeneNames = DirectCast(m_model.Genes.Clone(), String())
            }
        End Function

        Private Function Undefined(targets As String(), mode As InterventionMode) As InterventionResult
            Dim zeros(nModels - 1) As Double

            Return New InterventionResult With {
                .Spec = New InterventionSpec With {
                    .GeneName = String.Join("+", If(targets, New String() {})),
                    .GeneIndex = -1,
                    .Mode = mode
                },
                .WildtypeMeans = zeros,
                .WildtypeSDs = zeros,
                .MutantMeans = zeros,
                .FoldChanges = zeros,
                .PercentChanges = zeros,
                .ZScores = zeros,
                .IsSignificant = New Boolean(nModels - 1) {},
                .GeneNames = DirectCast(m_model.Genes.Clone(), String()),
                .Undefined = True
            }
        End Function

        ''' <summary>把总量的小数余量按索引顺序补齐（保持总量精确）。</summary>
        Private Sub Balance(totals As Double(), remainder As Double)
            If std.Abs(remainder) < 1.0 Then
                Return
            End If

            Dim direction As Double = If(remainder > 0.0, 1.0, -1.0)
            Dim left As Double = std.Abs(remainder)
            Dim at As Integer = 0

            While left >= 1.0 AndAlso at < totals.Length
                If direction > 0.0 OrElse totals(at) > 1.0 Then
                    totals(at) += direction
                    left -= 1.0
                End If

                at += 1
            End While
        End Sub

#End Region

    End Class
End Namespace
