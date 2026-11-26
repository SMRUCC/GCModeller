Imports System
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Summary
Imports std = System.Math

Namespace Data

    ''' <summary>装配选项。</summary>
    Public Class AssemblyOptions

        ''' <summary>做表达分层签名的目标基因（BATF / BATF3）。</summary>
        Public Property GeneTargets As String() = {"BATF", "BATF3"}

        ''' <summary>分层比例：取上/下各 25% 作为 high / low。</summary>
        Public Property StratifyQuantile As Double = 0.25

        ''' <summary>每组最少细胞数，低于该值不做对比。</summary>
        Public Property MinCells As Integer = 30

        ''' <summary>期别对比链（前一列 → 后一列，用 '|' 表示并集）。</summary>
        Public Property StageChain As String() = {"NA>II", "II>III", "III>IVB", "III>IVC", "III>IVB|IVC"}

    End Class

    ''' <summary>
    ''' 把对照矩阵 + 元数据装配成 CellularAI 的 <see cref="SummaryBundle"/>（三个 view）。
    ''' </summary>
    ''' <remarks>
    ''' <para>三个 view：</para>
    ''' <list type="bullet">
    ''' <item><description><c>stage</c>：期别对比作为伪扰动（全局计算，挂到每条患者记录下）；</description></item>
    ''' <item><description><c>patient</c>：每个患者 vs 正常细胞，逐记录独立计算；</description></item>
    ''' <item><description><c>signature</c>：按目标基因表达量分层得到的"高表达签名"（逐记录），
    ''' 承载 BATF / BATF3 过表达的效应。</description></item>
    ''' </list>
    ''' </remarks>
    Public Module SummaryAssembly

        ''' <summary>view 名常量。</summary>
        Public Const ViewStage As String = "stage"
        Public Const ViewPatient As String = "patient"
        Public Const ViewSignature As String = "signature"

        ''' <summary>期别名称展开（把 IV 展开成 IVB ∪ IVC）。</summary>
        Private Function StageMembers(metadata As CellMetadata, label As String) As String()
            If String.Equals(label, "IV", StringComparison.OrdinalIgnoreCase) Then
                Return metadata.BarcodesOfStage("IVB").
                    Concat(metadata.BarcodesOfStage("IVC")).
                    ToArray
            End If

            Return metadata.BarcodesOfStage(label)
        End Function

        ''' <summary>装配三个 view。</summary>
        Public Function Build(stream As ExpressionStream, Optional options As AssemblyOptions = Nothing) As SummaryBundle
            If stream Is Nothing Then
                Throw New ArgumentNullException(NameOf(stream))
            End If

            Dim settings As AssemblyOptions = If(options, New AssemblyOptions())
            Dim genes As String() = stream.Genes
            Dim cells As String() = stream.Cells
            Dim controls As CsrMatrix = stream.Controls
            Dim metadata As CellMetadata = stream.Metadata
            Dim major As GeneMajor = GeneMajor.FromCsr(controls, genes)
            Dim libraries As Double() = controls.RowSums()
            Dim columnSums As Double() = controls.ColumnSums()

            Dim index As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For i As Integer = 0 To cells.Length - 1
                index(cells(i)) = i
            Next

            Dim normal As Integer() = Resolve(metadata.NormalBarcodes(), index)
            Dim patients As String() = cells.
                Select(Function(id) If(metadata.Field(id, "patient_id"), "")).Distinct().
                Where(Function(id) Not String.IsNullOrEmpty(id)).
                OrderBy(Function(id) id, StringComparer.Ordinal).
                ToArray
            Dim patientCells As New Dictionary(Of String, Integer())(StringComparer.Ordinal)

            For Each patient As String In patients
                Dim members As New List(Of Integer)

                For i As Integer = 0 To cells.Length - 1
                    If String.Equals(metadata.Field(cells(i), "patient_id"), patient, StringComparison.Ordinal) Then
                        Call members.Add(i)
                    End If
                Next

                patientCells(patient) = members.ToArray
            Next

            Dim rows As New List(Of SummaryRow)

            ' ---- view: stage ----
            Dim stageContrasts As New List(Of Contrast)

            For Each pair As String In settings.StageChain
                Dim parts As String() = pair.Split(">"c)

                If parts.Length <> 2 Then
                    Continue For
                End If

                Dim caseCells As Integer() = Resolve(StageMembers(metadata, parts(1)), index)
                Dim ctrlCells As Integer() = If(CellMetadata.IsMissingStage(parts(0)),
                                                normal,
                                                Resolve(StageMembers(metadata, parts(0)), index))

                If caseCells.Length < settings.MinCells OrElse ctrlCells.Length < settings.MinCells Then
                    Continue For
                End If

                Call stageContrasts.Add(DifferentialPanel.Contrast(
                    $"{parts(0)}>{parts(1)}", major, libraries, caseCells, ctrlCells))
            Next

            For Each patient As String In patients
                Call rows.Add(MakeRow(patient, ViewStage, stageContrasts, columnSums, patientCells(patient), genes))
            Next

            ' ---- view: patient ----
            For Each patient As String In patients
                Dim members As Integer() = patientCells(patient)

                If members.Length < settings.MinCells OrElse normal.Length < settings.MinCells Then
                    Continue For
                End If

                Dim contrast As Contrast = DifferentialPanel.Contrast(patient, major, libraries, members, normal)

                Call rows.Add(MakeRow(patient, ViewPatient, New List(Of Contrast) From {contrast},
                                      columnSums, members, genes))
            Next

            ' ---- view: signature ----
            Dim geneIndex As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For i As Integer = 0 To genes.Length - 1
                geneIndex(genes(i)) = i
            Next

            For Each patient As String In patients
                Dim members As Integer() = patientCells(patient)

                If members.Length < settings.MinCells Then
                    Continue For
                End If

                Dim contrasts As New List(Of Contrast)

                For Each target As String In settings.GeneTargets
                    Dim at As Integer = -1

                    If Not geneIndex.TryGetValue(target, at) Then
                        Continue For
                    End If

                    Dim values As Double() = DifferentialPanel.GeneExpression(major, at, libraries)
                    Dim split = DifferentialPanel.Stratify(values, members, settings.StratifyQuantile)

                    If split.High.Length < settings.MinCells Then
                        Continue For
                    End If

                    Call contrasts.Add(DifferentialPanel.Contrast(
                        $"{target}_high", major, libraries, split.High, split.Low))
                Next

                Dim joint As Contrast = JointSignature(settings.GeneTargets, geneIndex, major,
                                                       libraries, members, genes, settings)

                If joint IsNot Nothing Then
                    Call contrasts.Add(joint)
                End If

                If contrasts.Count > 0 Then
                    Call rows.Add(MakeRow(patient, ViewSignature, contrasts, columnSums, members, genes))
                End If
            Next

            Return New SummaryBundle With {
                .Catalog = New SummaryCatalog With {
                    .PrimaryView = ViewStage,
                    .Rows = rows.ToArray
                }
            }
        End Function

#Region "helpers"

        Private Function Resolve(barcodes As String(), index As Dictionary(Of String, Integer)) As Integer()
            Dim result As New List(Of Integer)

            For Each barcode As String In barcodes
                Dim i As Integer = -1

                If index.TryGetValue(barcode, i) Then
                    Call result.Add(i)
                End If
            Next

            Return result.ToArray
        End Function

        ''' <summary>双高 vs 双低：在所有目标基因上都处于上/下分位的细胞。</summary>
        Private Function JointSignature(targets As String(), geneIndex As Dictionary(Of String, Integer),
                                        major As GeneMajor, libraries As Double(), members As Integer(),
                                        genes As String(), settings As AssemblyOptions) As Contrast
            If targets.Length < 2 Then
                Return Nothing
            End If

            Dim high As Dictionary(Of Integer, Integer) = Nothing
            Dim low As Dictionary(Of Integer, Integer) = Nothing

            For Each target As String In targets
                Dim at As Integer = -1

                If Not geneIndex.TryGetValue(target, at) Then
                    Return Nothing
                End If

                Dim values As Double() = DifferentialPanel.GeneExpression(major, at, libraries)
                Dim split = DifferentialPanel.Stratify(values, members, settings.StratifyQuantile)

                If high Is Nothing Then
                    high = split.High.ToDictionary(Function(i) i, Function(i) i)
                    low = split.Low.ToDictionary(Function(i) i, Function(i) i)
                Else
                    Dim nextHigh As New Dictionary(Of Integer, Integer)
                    Dim nextLow As New Dictionary(Of Integer, Integer)

                    For Each i As Integer In split.High
                        If high.ContainsKey(i) Then
                            nextHigh(i) = i
                        End If
                    Next
                    For Each i As Integer In split.Low
                        If low.ContainsKey(i) Then
                            nextLow(i) = i
                        End If
                    Next

                    high = nextHigh
                    low = nextLow
                End If
            Next

            If high.Count < settings.MinCells OrElse low.Count < settings.MinCells Then
                Return Nothing
            End If

            Return DifferentialPanel.Contrast(
                String.Join("+", targets.Select(Function(t) $"{t}_high")),
                major, libraries, high.Keys.ToArray, low.Keys.ToArray)
        End Function

        ''' <summary>由一组对比构造一条训练记录（面板按 基因 × 扰动 展开）。</summary>
        Private Function MakeRow(patient As String, view As String, contrasts As List(Of Contrast),
                                 columnSums As Double(), members As Integer(),
                                 genes As String()) As SummaryRow
            Dim nGenes As Integer = genes.Length
            Dim nPerts As Integer = contrasts.Count
            Dim effect(nGenes - 1)() As Double
            Dim probability(nGenes - 1)() As Double
            Dim pertNames(nPerts - 1) As String

            For g As Integer = 0 To nGenes - 1
                effect(g) = New Double(nPerts - 1) {}
                probability(g) = New Double(nPerts - 1) {}
            Next

            For p As Integer = 0 To nPerts - 1
                Dim contrast As Contrast = contrasts(p)

                pertNames(p) = contrast.Name

                For g As Integer = 0 To nGenes - 1
                    effect(g)(p) = If(Double.IsNaN(contrast.Effect(g)), 0.0, contrast.Effect(g))
                    probability(g)(p) = If(Double.IsNaN(contrast.Probability(g)), 0.5, contrast.Probability(g))
                Next
            Next

            Return New SummaryRow With {
                .CellLine = patient,
                .View = view,
                .EffectGenes = genes,
                .EffectPerts = pertNames,
                .Effect = effect,
                .Probability = probability,
                .Control = New ControlProfile With {
                    .Genes = genes,
                    .Counts = DirectCast(columnSums.Clone(), Double())
                },
                .Offset = MakeOffset(patient, view, genes, effect, nPerts)
            }
        End Function

        ''' <summary>由面板算逐基因的中心化表（池化时用来做偏移）。</summary>
        Private Function MakeOffset(patient As String, view As String, genes As String(),
                                    effect As Double()(), nPerts As Integer) As OffsetTable
            Dim center(genes.Length - 1) As Double
            Dim count(genes.Length - 1) As Double
            Dim variance(genes.Length - 1) As Double

            For g As Integer = 0 To genes.Length - 1
                Dim sum As Double = 0.0
                Dim measured As Integer = 0

                For p As Integer = 0 To nPerts - 1
                    Dim v As Double = effect(g)(p)

                    If Double.IsFinite(v) Then
                        sum += v
                        measured += 1
                    End If
                Next

                Dim mean As Double = If(measured > 0, sum / measured, 0.0)
                Dim acc As Double = 0.0

                For p As Integer = 0 To nPerts - 1
                    If Double.IsFinite(effect(g)(p)) Then
                        acc += (effect(g)(p) - mean) ^ 2
                    End If
                Next

                center(g) = mean
                count(g) = measured
                variance(g) = If(measured > 1, acc / (measured - 1), 0.0)
            Next

            Return New OffsetTable With {
                .CellLine = patient,
                .View = view,
                .Genes = genes,
                .EffectCenter = center,
                .EffectCount = count,
                .SignedScoreCenter = New Double(genes.Length - 1) {},
                .ResponseVariance = variance
            }
        End Function

#End Region

    End Module
End Namespace
