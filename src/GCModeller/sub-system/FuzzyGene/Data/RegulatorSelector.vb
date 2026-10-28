Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.FuzzyGene.Config
Imports std = System.Math

Namespace Data

    ''' <summary>
    ''' 一个候选调控因子（ANFIS 的一个输入变量）。
    ''' </summary>
    Public Class RegulatorPick

        ''' <summary>调控因子基因名。</summary>
        Public Property Gene As String

        ''' <summary>该因子是否来自先验调控网络。</summary>
        Public Property IsPrior As Boolean

        ''' <summary>先验置信度（<see cref="IsPrior"/> 为 False 时为 0）。</summary>
        Public Property PriorConfidence As Double

        ''' <summary>与靶基因表达的 Pearson 相关系数（先验因子也可能被补齐步骤顺带计算）。</summary>
        Public Property Correlation As Double

        ''' <summary>边的置信度：先验优先取先验置信度，否则取相关性的绝对值。</summary>
        Public ReadOnly Property Confidence As Double
            Get
                If IsPrior AndAlso PriorConfidence > 0.0 Then
                    Return PriorConfidence
                Else
                    Return std.Abs(Correlation)
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{Gene} (prior={IsPrior}, conf={Confidence:G3}, r={Correlation:G3})"
        End Function

    End Class

    ''' <summary>
    ''' 候选调控因子筛选：先验网络优先，相关性补齐。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 每个靶基因的输入维数必须压到 3–6 个（<see cref="FuzzyGeneConfig.MaxRegulators"/>）：
    ''' 网格划分的规则数是 <c>m^n</c>，否则参数规模会迅速超过样本量。
    ''' </para>
    ''' <para>
    ''' 选择顺序遵循「先验提供因果方向、数据提供相关性证据」：
    ''' 先按先验置信度降序取 TF，再用与靶基因 |Pearson| 最强的基因补齐；
    ''' 相关性来自 <see cref="GeneExpressionView.StateRows"/>（滞后路线下是时间点序列），
    ''' 因此不会因为重复样本数不同而改变排序依据。
    ''' </para>
    ''' </remarks>
    Public Module RegulatorSelector

        ''' <summary>
        ''' 为指定靶基因挑选候选调控因子。
        ''' </summary>
        ''' <param name="view">预处理视图</param>
        ''' <param name="prior">先验调控网络（可为 Nothing）</param>
        ''' <param name="config">建模配置</param>
        ''' <param name="targetIndex">靶基因在 <paramref name="view"/> 中的下标</param>
        ''' <returns>按输入顺序排列的候选因子；空数组表示该基因无法建模</returns>
        Public Function SelectRegulators(view As GeneExpressionView,
                                         prior As PriorNetwork,
                                         config As FuzzyGeneConfig,
                                         targetIndex As Integer) As RegulatorPick()

            If view Is Nothing Then
                Throw New ArgumentNullException(NameOf(view))
            End If

            Dim cfg As FuzzyGeneConfig = If(config, New FuzzyGeneConfig())
            Dim targetGene As String = view.GeneNames(targetIndex)
            Dim limit As Integer = cfg.MaxRegulators
            Dim picks As New List(Of RegulatorPick)
            Dim taken As New HashSet(Of Integer)

            ' ---- 1. 先验优先（按置信度降序） ----
            If cfg.Selection <> RegulatorSelection.CorrelationOnly AndAlso prior IsNot Nothing Then
                Dim edges As List(Of RegulatoryEdge) = prior.GetRegulators(targetGene)

                For Each edge As RegulatoryEdge In edges.OrderByDescending(Function(e) e.Confidence)
                    Dim idx As Integer = view.IndexOf(edge.TF)

                    If idx < 0 OrElse idx = targetIndex Then
                        Continue For
                    End If
                    If view.Constant(idx) Then
                        Continue For
                    End If
                    If Not taken.Add(idx) Then
                        Continue For
                    End If

                    Call picks.Add(New RegulatorPick With {
                        .Gene = view.GeneNames(idx),
                        .IsPrior = True,
                        .PriorConfidence = edge.Confidence,
                        .Correlation = Pearson(view, targetIndex, idx)
                    })

                    If picks.Count >= limit Then
                        Exit For
                    End If
                Next
            End If

            ' ---- 2. 相关性补齐 ----
            If picks.Count < limit AndAlso cfg.Selection <> RegulatorSelection.PriorOnly Then
                Dim scored As New List(Of (index As Integer, correlation As Double))

                For j As Integer = 0 To view.NGene - 1
                    If j = targetIndex OrElse view.Constant(j) OrElse taken.Contains(j) Then
                        Continue For
                    End If

                    Dim r As Double = Pearson(view, targetIndex, j)

                    If std.Abs(r) >= cfg.MinCorrelation Then
                        Call scored.Add((j, r))
                    End If
                Next

                scored.Sort(Function(a, b) std.Abs(b.correlation).CompareTo(std.Abs(a.correlation)))

                For Each item In scored
                    If picks.Count >= limit Then
                        Exit For
                    End If

                    Call taken.Add(item.index)
                    Call picks.Add(New RegulatorPick With {
                        .Gene = view.GeneNames(item.index),
                        .IsPrior = False,
                        .PriorConfidence = 0.0,
                        .Correlation = item.correlation
                    })
                Next
            End If

            Return picks.ToArray()
        End Function

        ''' <summary>两个基因在训练状态行上的 Pearson 相关系数。</summary>
        Public Function Pearson(view As GeneExpressionView, i As Integer, j As Integer) As Double
            Dim rows As Integer = view.NRows
            Dim mi As Double = view.RowMeans(i)
            Dim mj As Double = view.RowMeans(j)
            Dim covariance As Double = 0.0

            For r As Integer = 0 To rows - 1
                covariance += (view.StateRows(r)(i) - mi) * (view.StateRows(r)(j) - mj)
            Next

            Dim denominator As Double = std.Sqrt(view.RowSs(i) * view.RowSs(j))

            If denominator < 0.000000000001 Then
                Return 0.0
            End If

            Return covariance / denominator
        End Function

    End Module

End Namespace
