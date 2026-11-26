Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math.Statistics
Imports std = System.Math

Namespace Summary

    ''' <summary>
    ''' 训练摘要的<b>内存版</b>视图：把上游装配好的摘要行摆成算法需要的各种轴，
    ''' 并提供"居中对齐 / 带符号 z / 并集堆叠 / 方差映射"这几个纯函数。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 本类型不含文件 IO：每条 <see cref="SummaryRow"/> 里的矩阵已经由上游从 parquet 读好。
    ''' 这里只负责三件事：
    ''' </para>
    ''' <list type="number">
    ''' <item><description>把某行的面板<b>重索引</b>到任务基因/扰动轴（缺失填 <c>NaN</c>，重复基因取最大值）；</description></item>
    ''' <item><description>把双侧概率换成带符号正态分数（Liptak），并减去逐基因中心；</description></item>
    ''' <item><description>按"训练基因并集"堆叠主视图的效应矩阵，供相似度与高变基因选择使用。</description></item>
    ''' </list>
    ''' <para>
    ''' <b>重复基因的处理</b>：上游读 parquet 时按 <c>feature_id</c> 分组取最大值（<c>groupby(level=0).max()</c>），
    ''' 这里保持同一语义，避免同一基因的多个探针在池化时被当成两条证据。
    ''' </para>
    ''' </remarks>
    Public Class SummaryBundle

        Private Const ProbabilityFloor As Double = 1.0E-300

        ''' <summary>摘要目录（行顺序即池化顺序）。</summary>
        Public Property Catalog As SummaryCatalog

        ''' <summary>逐细胞系的方差表基因轴（键为细胞系名）。</summary>
        Public Property VarianceGenes As Dictionary(Of String, String()) = New Dictionary(Of String, String())(StringComparer.Ordinal)

        ''' <summary>逐细胞系的响应方差（与 <see cref="VarianceGenes"/> 同键、等长）。</summary>
        Public Property Variance As Dictionary(Of String, Double()) = New Dictionary(Of String, Double())(StringComparer.Ordinal)

        ''' <summary>校验目录与方差表。</summary>
        Public Sub Validate()
            If Catalog Is Nothing Then
                Throw New ArgumentNullException(NameOf(Catalog))
            End If

            Call Catalog.Validate()

            For Each line As String In Variance.Keys
                Dim genes As String() = Nothing

                If Not VarianceGenes.TryGetValue(line, genes) Then
                    Throw New ArgumentException($"the variance table of '{line}' has no gene axis", NameOf(VarianceGenes))
                End If
                If Variance(line).Length <> genes.Length Then
                    Throw New ArgumentException($"the variance table of '{line}' does not match its gene axis", NameOf(Variance))
                End If
            Next
        End Sub

#Region "axis alignment"

        ''' <summary>
        ''' 把面板重索引到任务轴：缺失为 <c>NaN</c>，重复基因取最大值。
        ''' </summary>
        ''' <param name="panelGenes">面板自己的基因轴</param>
        ''' <param name="panelPerts">面板自己的扰动轴</param>
        ''' <param name="panel">面板（行数 = 基因数，列数 = 扰动数）</param>
        ''' <param name="genes">目标基因轴</param>
        ''' <param name="perturbations">目标扰动轴</param>
        ''' <param name="excludePerts">需要排除的扰动（例如任务声明要剔除的靶点）</param>
        Public Shared Function AlignPanel(panelGenes As String(),
                                          panelPerts As String(),
                                          panel As Double()(),
                                          genes As String(),
                                          perturbations As String(),
                                          Optional excludePerts As IEnumerable(Of String) = Nothing) As Double()()

            If genes Is Nothing Then
                Throw New ArgumentNullException(NameOf(genes))
            End If
            If perturbations Is Nothing Then
                Throw New ArgumentNullException(NameOf(perturbations))
            End If

            Dim G As Integer = genes.Length
            Dim P As Integer = perturbations.Length
            Dim result As Double()() = RectangularArray.Matrix(Of Double)(G, P)

            For i As Integer = 0 To G - 1
                For j As Integer = 0 To P - 1
                    result(i)(j) = Double.NaN
                Next
            Next

            If panel Is Nothing OrElse panelGenes Is Nothing OrElse panelPerts Is Nothing Then
                Return result
            End If

            ' 目标轴上的位置
            Dim geneSlots As New Dictionary(Of String, List(Of Integer))(StringComparer.Ordinal)

            For i As Integer = 0 To G - 1
                Dim slots As List(Of Integer) = Nothing

                If Not geneSlots.TryGetValue(genes(i), slots) Then
                    slots = New List(Of Integer)
                    geneSlots(genes(i)) = slots
                End If

                Call slots.Add(i)
            Next

            Dim excluded As New HashSet(Of String)(StringComparer.Ordinal)

            If excludePerts IsNot Nothing Then
                For Each name As String In excludePerts
                    Call excluded.Add(name)
                Next
            End If

            Dim pertSlots As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For j As Integer = 0 To P - 1
                If Not excluded.Contains(perturbations(j)) Then
                    pertSlots(perturbations(j)) = j
                End If
            Next

            For r As Integer = 0 To panelGenes.Length - 1
                Dim targets As List(Of Integer) = Nothing

                If Not geneSlots.TryGetValue(panelGenes(r), targets) Then
                    Continue For
                End If

                Dim row As Double() = panel(r)

                If row Is Nothing Then
                    Continue For
                End If

                For c As Integer = 0 To panelPerts.Length - 1
                    Dim slot As Integer = -1

                    If c >= row.Length OrElse Not pertSlots.TryGetValue(panelPerts(c), slot) Then
                        Continue For
                    End If

                    Dim value As Double = row(c)

                    For Each i As Integer In targets
                        ' 重复基因取最大值（与上游 groupby.max 一致）；NaN 不参与比较
                        If Double.IsNaN(result(i)(slot)) OrElse
                            (Not Double.IsNaN(value) AndAlso value > result(i)(slot)) Then

                            result(i)(slot) = value
                        End If
                    Next
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 双侧概率 → 带符号正态分数：<c>sign(effect) × Φ⁻¹(1 − p/2)</c>，与上游 <c>signed_z</c> 同式。
        ''' </summary>
        ''' <remarks>
        ''' 用 <c>InvCCDF</c>（互补 CDF 的逆）而不是 <c>1 − p/2</c> 再取逆，
        ''' 是为了避免极小 p 值在 <c>1 − p/2</c> 处被舍入成 1 而丢掉整个尾部信息。
        ''' </remarks>
        Public Shared Function SignedZ(effect As Double()(), probability As Double()()) As Double()()
            If effect Is Nothing OrElse probability Is Nothing Then
                Throw New ArgumentNullException(NameOf(effect))
            End If
            If effect.Length <> probability.Length Then
                Throw New ArgumentException("the effect and probability panels do not match", NameOf(probability))
            End If

            Dim result As Double()() = RectangularArray.Matrix(Of Double)(effect.Length, If(effect.Length = 0, 0, effect(0).Length))

            For i As Integer = 0 To effect.Length - 1
                Dim row As Double() = effect(i)
                Dim p As Double() = probability(i)

                For j As Integer = 0 To row.Length - 1
                    Dim e As Double = row(j)
                    Dim pi As Double = p(j)

                    If Double.IsNaN(e) OrElse Double.IsNaN(pi) Then
                        result(i)(j) = Double.NaN
                        Continue For
                    End If

                    Dim clamped As Double = std.Max(ProbabilityFloor, std.Min(pi, 1.0))
                    Dim magnitude As Double = NormalDistribution.InvCCDF(clamped / 2.0)

                    result(i)(j) = std.Sign(e) * magnitude
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 减去逐基因中心：中心缺失（NaN）时<b>原值透传</b>，面板缺失处仍保持 <c>NaN</c>。
        ''' </summary>
        ''' <param name="panel">面板（G×P）</param>
        ''' <param name="offset">中心化表</param>
        ''' <param name="genes">面板的基因轴</param>
        ''' <param name="stream"><c>l2</c> 取 effect_center，<c>z</c> 取 signed_score_center</param>
        Public Shared Function ApplyOffsets(panel As Double()(),
                                            offset As OffsetTable,
                                            genes As String(),
                                            stream As String) As Double()()

            If panel Is Nothing Then
                Throw New ArgumentNullException(NameOf(panel))
            End If

            Dim result As Double()() = RectangularArray.Matrix(Of Double)(panel.Length, If(panel.Length = 0, 0, panel(0).Length))
            Dim center As Double() = Nothing

            Select Case (If(stream, "")).ToLowerInvariant()
                Case "l2"
                    center = If(offset Is Nothing, Nothing, offset.EffectCenter)
                Case "z"
                    center = If(offset Is Nothing, Nothing, offset.SignedScoreCenter)
                Case Else
                    Throw New ArgumentException($"unknown offset stream '{stream}'", NameOf(stream))
            End Select

            Dim index As Integer() = If(offset Is Nothing, Nothing, offset.IndexOf(genes))

            For i As Integer = 0 To panel.Length - 1
                Dim row As Double() = panel(i)
                Dim value As Double = 0.0

                If center IsNot Nothing AndAlso index IsNot Nothing AndAlso index(i) >= 0 Then
                    Dim c As Double = center(index(i))

                    If Not Double.IsNaN(c) Then
                        value = c
                    End If
                End If

                For j As Integer = 0 To row.Length - 1
                    result(i)(j) = If(Double.IsNaN(row(j)), Double.NaN, row(j) - value)
                Next
            Next

            Return result
        End Function

#End Region

#Region "training rows"

        ''' <summary>
        ''' 池化使用的训练行：先按配置的视图顺序取主记录，再追加辅助记录（各自保持目录顺序）。
        ''' </summary>
        ''' <param name="views">参与池化的视图名，按顺序</param>
        ''' <param name="exclude">需要排除的细胞系</param>
        Public Function TrainingRows(views As String(), Optional exclude As IEnumerable(Of String) = Nothing) As SummaryRow()
            Dim excluded As New HashSet(Of String)(StringComparer.Ordinal)

            If exclude IsNot Nothing Then
                For Each line As String In exclude
                    Call excluded.Add(line)
                Next
            End If

            Dim result As New List(Of SummaryRow)

            If views IsNot Nothing Then
                For Each view As String In views
                    For i As Integer = 0 To Catalog.Count - 1
                        Dim row As SummaryRow = Catalog.Rows(i)

                        If row.IsAuxiliary OrElse excluded.Contains(row.CellLine) Then
                            Continue For
                        End If
                        If String.Equals(row.View, view, StringComparison.Ordinal) Then
                            Call result.Add(row)
                        End If
                    Next
                Next
            End If

            For i As Integer = 0 To Catalog.Count - 1
                Dim row As SummaryRow = Catalog.Rows(i)

                If row.IsAuxiliary AndAlso Not excluded.Contains(row.CellLine) Then
                    Call result.Add(row)
                End If
            Next

            Return result.ToArray
        End Function

#End Region

#Region "stacking"

        ''' <summary>
        ''' 按"训练基因并集"堆叠主视图的效应矩阵：返回 <c>T×Gunion×P</c> 的栈、行名与并集基因轴。
        ''' </summary>
        ''' <param name="perturbations">任务扰动轴</param>
        ''' <param name="exclude">需要排除的细胞系</param>
        ''' <remarks>
        ''' 行的顺序取自 <c>Orders("effect_files")</c>（若记录过），否则退化为目录顺序，
        ''' 并且只取主视图上的主记录——这与上游 <c>primary_effect_files</c> 的语义一致。
        ''' </remarks>
        Public Function StackEffects(perturbations As String(),
                                     Optional exclude As IEnumerable(Of String) = Nothing) As (Stack As Double()()(), Lines As String(), Union As String())

            Dim excluded As New HashSet(Of String)(StringComparer.Ordinal)

            If exclude IsNot Nothing Then
                For Each line As String In exclude
                    Call excluded.Add(line)
                Next
            End If

            Dim rows As New List(Of SummaryRow)
            Dim order As String() = Catalog.RecordedOrder("effect_files")

            If order IsNot Nothing Then
                For Each line As String In order
                    If excluded.Contains(line) Then
                        Continue For
                    End If

                    Dim row As SummaryRow = Nothing

                    Try
                        row = Catalog.PrimaryRow(line)
                    Catch ex As KeyNotFoundException
                        Continue For
                    End Try

                    Call rows.Add(row)
                Next
            Else
                For i As Integer = 0 To Catalog.Count - 1
                    Dim row As SummaryRow = Catalog.Rows(i)

                    If row.IsAuxiliary OrElse excluded.Contains(row.CellLine) Then
                        Continue For
                    End If
                    If Not String.Equals(row.View, Catalog.PrimaryView, StringComparison.Ordinal) Then
                        Continue For
                    End If

                    Call rows.Add(row)
                Next
            End If

            ' 基因并集（字典序，与上游 sorted(set().union(...)) 一致）
            Dim unionSet As New HashSet(Of String)(StringComparer.Ordinal)

            For Each row As SummaryRow In rows
                If row.EffectGenes Is Nothing Then
                    Continue For
                End If

                For Each gene As String In row.EffectGenes
                    Call unionSet.Add(gene)
                Next
            Next

            Dim union As String() = unionSet.OrderBy(Function(g) g, StringComparer.Ordinal).ToArray
            Dim lines As String() = rows.Select(Function(r) r.CellLine).ToArray
            Dim stack(If(rows.Count = 0, -1, rows.Count - 1))()() As Double

            For t As Integer = 0 To rows.Count - 1
                stack(t) = AlignPanel(rows(t).EffectGenes, rows(t).EffectPerts, rows(t).Effect,
                                      union, perturbations)
            Next

            Return (stack, lines, union)
        End Function

        ''' <summary>
        ''' 取各细胞系的对照剖面（按 <c>Orders("control_files")</c> 的顺序，未记录时按目录顺序）。
        ''' </summary>
        Public Function ControlProfiles(Optional exclude As IEnumerable(Of String) = Nothing) As Dictionary(Of String, ControlProfile)
            Dim excluded As New HashSet(Of String)(StringComparer.Ordinal)

            If exclude IsNot Nothing Then
                For Each line As String In exclude
                    Call excluded.Add(line)
                Next
            End If

            Dim result As New Dictionary(Of String, ControlProfile)(StringComparer.Ordinal)
            Dim order As String() = Catalog.RecordedOrder("control_files")

            If order IsNot Nothing Then
                For Each line As String In order
                    If excluded.Contains(line) Then
                        Continue For
                    End If

                    Dim row As SummaryRow = Nothing

                    Try
                        row = Catalog.PrimaryRow(line)
                    Catch ex As KeyNotFoundException
                        Continue For
                    End Try

                    If row.Control IsNot Nothing Then
                        result(line) = row.Control
                    End If
                Next
            Else
                For i As Integer = 0 To Catalog.Count - 1
                    Dim row As SummaryRow = Catalog.Rows(i)

                    If row.IsAuxiliary OrElse excluded.Contains(row.CellLine) Then
                        Continue For
                    End If
                    If row.Control Is Nothing Then
                        Continue For
                    End If
                    If Not result.ContainsKey(row.CellLine) Then
                        result(row.CellLine) = row.Control
                    End If
                Next
            End If

            Return result
        End Function

        ''' <summary>
        ''' 把逐细胞系的响应方差映射到给定基因并集：返回 <c>T×Gunion</c>，缺失为 <c>NaN</c>。
        ''' </summary>
        Public Function NullVariance(lines As String(), union As String()) As Double()()
            Dim result As Double()() = RectangularArray.Matrix(Of Double)(If(lines Is Nothing, 0, lines.Length), If(union Is Nothing, 0, union.Length))

            For i As Integer = 0 To result.Length - 1
                For j As Integer = 0 To result(i).Length - 1
                    result(i)(j) = Double.NaN
                Next
            Next

            If lines Is Nothing OrElse union Is Nothing Then
                Return result
            End If

            For t As Integer = 0 To lines.Length - 1
                Dim genes As String() = Nothing
                Dim values As Double() = Nothing

                If Not VarianceGenes.TryGetValue(lines(t), genes) Then
                    Continue For
                End If
                If Not Variance.TryGetValue(lines(t), values) Then
                    Continue For
                End If

                Dim positions As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

                For i As Integer = 0 To genes.Length - 1
                    positions(genes(i)) = i
                Next

                For g As Integer = 0 To union.Length - 1
                    Dim at As Integer = -1

                    If positions.TryGetValue(union(g), at) Then
                        result(t)(g) = values(at)
                    End If
                Next
            Next

            Return result
        End Function

#End Region

        Public Overrides Function ToString() As String
            Return $"summary bundle: {If(Catalog Is Nothing, "no catalog", Catalog.ToString())}"
        End Function

    End Class
End Namespace
