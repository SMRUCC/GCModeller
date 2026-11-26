Imports CellularAI.Model
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Bootstrapping.Ridge
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports std = System.Math

Namespace Residual

    ''' <summary>
    ''' 控制条件残差模型：学习"训练记录的控制表达 → 残差低秩坐标"的线性映射，
    ''' 再用新上下文的控制表达预测修正量。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 池化效应只依赖"训练行与扰动的相似度"，看不到"这个上下文的整体表达状态"。
    ''' 残差模型补上这一层：对每个基因，把"该记录相对跨记录均值的残差"投影到低秩响应基上，
    ''' 再用<b>该记录的控制表达特征</b>（标准化的控制 logCPM + 少量全局主成分）做岭回归。
    ''' 预测时把新上下文的控制特征代入，就得到"这个上下文应当往哪个方向修正"。
    ''' </para>
    ''' <para>
    ''' 三个必须遵守的细节：
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description>训练时的<b>自身靶</b>列（基因名 = 扰动名）不参与残差，
    ''' 因为它的效应是"敲低本身"，与上下文的转录状态无关；</description></item>
    ''' <item><description>每个基因只在"实测记录数 ≥ <c>residual_min_measured</c>"的记录上回归，
    ''' 行掩码由 <see cref="MultiResponseRidge"/> 的原生支持；</description></item>
    ''' <item><description>只有被判定为"支持"的基因才会被修正，其余保持池化结果。</description></item>
    ''' </list>
    ''' </remarks>
    Public Class ControlResidual

        Private ReadOnly m_options As TotalsOptions

        Private m_geneIndex As Dictionary(Of String, Integer)
        Private m_basis As Double()()
        Private m_intercept As Double()
        Private m_responseScale As Double()
        Private m_projection As Double()()          ' 扰动×秩
        Private m_normalizationGenes As Boolean()
        Private m_controlMean As Double()
        Private m_controlScale As Double()
        Private m_globalLoadings As Double()()      ' 基因×主成分
        Private m_globalScale As Double()
        Private m_featureCenter As Double()()       ' 基因×特征
        Private m_coefficients As Double()()()      ' 基因×特征×秩
        Private m_supported As Boolean()

        Public Sub New(options As TotalsOptions)
            If options Is Nothing Then
                Throw New ArgumentNullException(NameOf(options))
            End If

            m_options = options
        End Sub

        ''' <summary>该基因是否被残差模型支持（未支持的基因保持池化结果）。</summary>
        Public ReadOnly Property Supported As Boolean()
            Get
                Return m_supported
            End Get
        End Property

        ''' <summary>实际使用的秩。</summary>
        Public ReadOnly Property Rank As Integer
            Get
                Return If(m_basis Is Nothing, 0, m_basis(0).Length)
            End Get
        End Property

        ''' <summary>
        ''' 在训练记录上拟合残差模型。
        ''' </summary>
        ''' <param name="stack">逐记录效应（记录×基因×扰动）</param>
        ''' <param name="counts">逐记录的控制计数（记录×基因，缺失为 NaN）</param>
        Public Function Fit(stack As Double()()(), counts As Double()(),
                            genes As String(), perts As String()) As ControlResidual

            Dim nRecords As Integer = stack.Length
            Dim nGenes As Integer = If(genes Is Nothing, 0, genes.Length)
            Dim nPerts As Integer = If(perts Is Nothing, 0, perts.Length)

            m_geneIndex = New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For g As Integer = 0 To nGenes - 1
                m_geneIndex(genes(g)) = g
            Next

            ' 自身靶：基因名同时是扰动名的位置
            Dim pertIndex As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For p As Integer = 0 To nPerts - 1
                pertIndex(perts(p)) = p
            Next

            Dim ownRows As New List(Of Integer)
            Dim ownColumns As New List(Of Integer)

            For g As Integer = 0 To nGenes - 1
                Dim at As Integer = -1

                If pertIndex.TryGetValue(genes(g), at) Then
                    Call ownRows.Add(g)
                    Call ownColumns.Add(at)
                End If
            Next

            ' ---- 1) 自身靶替换为"该行其余扰动的均值" ----
            Dim values As Double()()() = Cube(Of Double)(nRecords, nGenes, nPerts)

            For t As Integer = 0 To nRecords - 1
                For g As Integer = 0 To nGenes - 1
                    For p As Integer = 0 To nPerts - 1
                        values(t)(g)(p) = stack(t)(g)(p)
                    Next
                Next
            Next

            For i As Integer = 0 To ownRows.Count - 1
                Dim row As Integer = ownRows(i)
                Dim column As Integer = ownColumns(i)

                For t As Integer = 0 To nRecords - 1
                    Dim block As Double() = values(t)(row)
                    Dim measured As Boolean = Not Double.IsNaN(block(column))

                    block(column) = Double.NaN

                    Dim sum As Double = 0.0
                    Dim count As Integer = 0

                    For p As Integer = 0 To nPerts - 1
                        If Not Double.IsNaN(block(p)) Then
                            sum += block(p)
                            count += 1
                        End If
                    Next

                    If measured Then
                        values(t)(row)(column) = If(count > 0, sum / count, 0.0)
                    End If
                Next
            Next

            ' ---- 2) 跨记录的逆方差均值 ----
            Dim baseline As Double()() = IvwBaseline(values)

            ' ---- 3) 响应基 ----
            Dim basis As ResponseBasis = ResponseBasis.Fit(
                baseline, ownRows.ToArray, ownColumns.ToArray,
                std.Min(m_options.ResidualRank, std.Min(nGenes, nPerts)), m_options.PcsSeed)

            m_basis = basis.Basis
            m_intercept = basis.Intercept
            m_responseScale = basis.ResponseScale

            ' ---- 4) 控制表达特征 ----
            Dim normalization As Boolean() = New Boolean(If(nGenes = 0, -1, nGenes - 1)) {}

            For g As Integer = 0 To nGenes - 1
                Dim complete As Boolean = True
                Dim total As Double = 0.0

                For t As Integer = 0 To nRecords - 1
                    Dim v As Double = CountsAt(counts, t, g, nRecords, nGenes)

                    If Double.IsNaN(v) Then
                        complete = False
                        Exit For
                    End If

                    total += v
                Next

                normalization(g) = complete AndAlso total > 0.0
            Next

            m_normalizationGenes = normalization

            Dim logControls As Double()() = CountsLogCpm(counts, normalization, nRecords, nGenes)
            Dim observed As Boolean()() = RectangularArray.Matrix(Of Boolean)(nRecords, nGenes)

            For t As Integer = 0 To nRecords - 1
                For g As Integer = 0 To nGenes - 1
                    observed(t)(g) = Not Double.IsNaN(logControls(t)(g))
                Next
            Next

            m_controlMean = New Double(If(nGenes = 0, -1, nGenes - 1)) {}
            m_controlScale = New Double(If(nGenes = 0, -1, nGenes - 1)) {}

            For g As Integer = 0 To nGenes - 1
                Dim sum As Double = 0.0
                Dim count As Integer = 0

                For t As Integer = 0 To nRecords - 1
                    If observed(t)(g) Then
                        sum += logControls(t)(g)
                        count += 1
                    End If
                Next

                m_controlMean(g) = If(count > 0, sum / count, 0.0)

                Dim acc As Double = 0.0

                For t As Integer = 0 To nRecords - 1
                    If observed(t)(g) Then
                        Dim d As Double = logControls(t)(g) - m_controlMean(g)

                        acc += d * d
                    End If
                Next

                m_controlScale(g) = std.Max(std.Sqrt(If(count > 0, acc / count, 0.0)), m_options.ResidualScaleFloor)
            Next

            Dim standardized As Double()() = RectangularArray.Matrix(Of Double)(nRecords, nGenes)

            For t As Integer = 0 To nRecords - 1
                For g As Integer = 0 To nGenes - 1
                    standardized(t)(g) = If(observed(t)(g),
                                            (logControls(t)(g) - m_controlMean(g)) / m_controlScale(g),
                                            0.0)
                Next
            Next

            ' 只在"整行控制都可用"的记录上做主成分
            Dim valid As New List(Of Integer)

            For t As Integer = 0 To nRecords - 1
                Dim ok As Boolean = True

                For g As Integer = 0 To nGenes - 1
                    If Not observed(t)(g) Then
                        ok = False
                        Exit For
                    End If
                Next

                If ok Then
                    Call valid.Add(t)
                End If
            Next

            If valid.Count > 0 AndAlso nGenes > 0 Then
                Dim permitted As Double()() = RectangularArray.Matrix(Of Double)(valid.Count, nGenes)

                For i As Integer = 0 To valid.Count - 1
                    For g As Integer = 0 To nGenes - 1
                        permitted(i)(g) = standardized(valid(i))(g)
                    Next
                Next

                Dim width As Integer = std.Min(m_options.ResidualGlobal, std.Min(valid.Count, nGenes))

                If width > 0 Then
                    Dim svd As New RandomizedSVD(permitted, width, oversampling:=8, powerIterations:=2, seed:=m_options.PcsSeed)
                    Dim components As Double()() = svd.Components      ' width×基因

                    m_globalLoadings = RectangularArray.Matrix(Of Double)(nGenes, width)

                    For c As Integer = 0 To width - 1
                        For g As Integer = 0 To nGenes - 1
                            m_globalLoadings(g)(c) = components(c)(g)
                        Next
                    Next

                    ' 符号定向：让每个主成分的最大绝对值载荷为正
                    For c As Integer = 0 To width - 1
                        Dim pivot As Integer = 0
                        Dim largest As Double = 0.0

                        For g As Integer = 0 To nGenes - 1
                            If std.Abs(m_globalLoadings(g)(c)) > largest Then
                                largest = std.Abs(m_globalLoadings(g)(c))
                                pivot = g
                            End If
                        Next

                        If m_globalLoadings(pivot)(c) < 0.0 Then
                            For g As Integer = 0 To nGenes - 1
                                m_globalLoadings(g)(c) = -m_globalLoadings(g)(c)
                            Next
                        End If
                    Next

                    m_globalScale = New Double(width - 1) {}

                    For c As Integer = 0 To width - 1
                        Dim acc As Double = 0.0

                        For i As Integer = 0 To valid.Count - 1
                            Dim value As Double = 0.0

                            For g As Integer = 0 To nGenes - 1
                                value += permitted(i)(g) * m_globalLoadings(g)(c)
                            Next

                            acc += value * value
                        Next

                        Dim rms As Double = std.Sqrt(acc / std.Max(valid.Count, 1))

                        m_globalScale(c) = If(rms > 0.0, rms, 1.0)
                    Next
                End If
            End If

            If m_globalLoadings Is Nothing Then
                m_globalLoadings = RectangularArray.Matrix(Of Double)(nGenes, 0)
                m_globalScale = New Double(-1) {}
            End If

            Dim featureCount As Integer = 1 + m_globalLoadings(0).Length
            Dim features As Double()()() = Cube(Of Double)(nRecords, nGenes, featureCount)

            For t As Integer = 0 To nRecords - 1
                For g As Integer = 0 To nGenes - 1
                    features(t)(g)(0) = standardized(t)(g)

                    For c As Integer = 0 To m_globalLoadings(0).Length - 1
                        features(t)(g)(1 + c) = standardized(t)(g) * m_globalLoadings(g)(c) / m_globalScale(c)
                    Next
                Next
            Next

            ' ---- 5) 残差坐标投影 ----
            Dim rank As Integer = basis.Rank

            If rank > 0 Then
                Dim vtrain As Double()() = RectangularArray.Matrix(Of Double)(rank, nPerts)

                For c As Integer = 0 To rank - 1
                    For p As Integer = 0 To nPerts - 1
                        Dim value As Double = 0.0

                        For g As Integer = 0 To nGenes - 1
                            value += m_basis(g)(c) * basis.Centered(g)(p)
                        Next

                        vtrain(c)(p) = value / std.Max(m_responseScale(c), 1.0E-300)
                    Next
                Next

                m_projection = Projection(vtrain, rank, nPerts, m_options.ResidualProjRidge)
            Else
                m_projection = RectangularArray.Matrix(Of Double)(nPerts, 0)
            End If

            ' ---- 6) 逐基因岭回归 ----
            m_featureCenter = RectangularArray.Matrix(Of Double)(nGenes, featureCount)
            m_coefficients = Cube(Of Double)(nGenes, featureCount, std.Max(rank, 0))
            m_supported = New Boolean(If(nGenes = 0, -1, nGenes - 1)) {}

            Dim support(If(nRecords = 0, -1, nRecords - 1), If(nGenes = 0, -1, nGenes - 1)) As Boolean
            Dim measuredCount(nRecords - 1, If(nGenes = 0, -1, nGenes - 1)) As Integer
            Dim coordinates As Double()()() = Cube(Of Double)(nRecords, nGenes, std.Max(rank, 0))

            For t As Integer = 0 To nRecords - 1
                Dim residual As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)

                For g As Integer = 0 To nGenes - 1
                    For p As Integer = 0 To nPerts - 1
                        Dim measured As Boolean = Not Double.IsNaN(values(t)(g)(p))

                        residual(g)(p) = If(measured, values(t)(g)(p) - baseline(g)(p), 0.0)
                    Next
                Next

                For i As Integer = 0 To ownRows.Count - 1
                    residual(ownRows(i))(ownColumns(i)) = 0.0
                Next

                ' 记录每行（基因）的实测数
                For g As Integer = 0 To nGenes - 1
                    Dim count As Integer = 0

                    For p As Integer = 0 To nPerts - 1
                        If Not Double.IsNaN(values(t)(g)(p)) Then
                            count += 1
                        End If
                    Next

                    measuredCount(t, g) = count
                Next

                If rank > 0 Then
                    For g As Integer = 0 To nGenes - 1
                        For c As Integer = 0 To rank - 1
                            Dim acc As Double = 0.0

                            For p As Integer = 0 To nPerts - 1
                                acc += residual(g)(p) * m_projection(p)(c)
                            Next

                            coordinates(t)(g)(c) = acc
                        Next
                    Next
                End If
            Next

            For g As Integer = 0 To nGenes - 1
                Dim any As Boolean = False

                For t As Integer = 0 To nRecords - 1
                    If measuredCount(t, g) >= m_options.ResidualMinMeasured AndAlso observed(t)(g) Then
                        support(t, g) = True
                        any = True
                    End If
                Next

                m_supported(g) = any AndAlso rank > 0
            Next

            If rank = 0 Then
                Return Me
            End If

            Dim x As Double()() = Nothing
            Dim y As Double()() = Nothing
            Dim mask(nRecords - 1) As Boolean

            For g As Integer = 0 To nGenes - 1
                If Not m_supported(g) Then
                    Continue For
                End If

                x = RectangularArray.Matrix(Of Double)(nRecords, featureCount)
                y = RectangularArray.Matrix(Of Double)(nRecords, rank)

                For t As Integer = 0 To nRecords - 1
                    mask(t) = support(t, g)

                    For f As Integer = 0 To featureCount - 1
                        x(t)(f) = features(t)(g)(f)
                    Next

                    For c As Integer = 0 To rank - 1
                        y(t)(c) = coordinates(t)(g)(c)
                    Next
                Next

                Dim model As MultiResponseRidgeFit = MultiResponseRidge.Fit(
                    x, y, mask, m_options.ResidualRidge, center:=True)

                For f As Integer = 0 To featureCount - 1
                    m_featureCenter(g)(f) = model.FeatureCenter(f)

                    For c As Integer = 0 To rank - 1
                        m_coefficients(g)(f)(c) = model.Coefficients(f)(c)
                    Next
                Next
            Next

            Return Me
        End Function

        ''' <summary>
        ''' 用新上下文的控制表达预测修正后的效应。
        ''' </summary>
        ''' <param name="query">池化效应（基因×扰动）</param>
        ''' <param name="counts">新上下文的控制计数（按 <paramref name="genes"/> 求和后的向量）</param>
        Public Function Predict(query As Double()(), counts As Double(),
                                genes As String(), perts As String()) As Double()()
            Dim nGenes As Integer = query.Length
            Dim nPerts As Integer = If(nGenes = 0, 0, query(0).Length)
            Dim result As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)
            Dim rank As Integer = Me.Rank

            For g As Integer = 0 To nGenes - 1
                For p As Integer = 0 To nPerts - 1
                    result(g)(p) = query(g)(p)
                Next
            Next

            If rank = 0 OrElse m_coefficients Is Nothing Then
                Return result
            End If

            ' 目标特征
            Dim features As Double()() = TargetFeatures(counts)
            Dim featureCount As Integer = features(0).Length

            ' 查询坐标：basisᵀ·(query − intercept)/scale
            Dim active As New List(Of Integer)

            For p As Integer = 0 To nPerts - 1
                For g As Integer = 0 To nGenes - 1
                    If query(g)(p) <> 0.0 Then
                        Call active.Add(p)
                        Exit For
                    End If
                Next
            Next

            If active.Count = 0 Then
                Return result
            End If

            Dim localIndex As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For g As Integer = 0 To nGenes - 1
                localIndex(genes(g)) = g
            Next

            Dim vquery As Double()() = RectangularArray.Matrix(Of Double)(rank, active.Count)

            For i As Integer = 0 To active.Count - 1
                Dim p As Integer = active(i)
                Dim own As Integer = -1

                ' 自身靶在查询里保持原值，不参与坐标
                Call localIndex.TryGetValue(perts(p), own)

                For c As Integer = 0 To rank - 1
                    Dim acc As Double = 0.0

                    For g As Integer = 0 To nGenes - 1
                        Dim value As Double = query(g)(p)

                        If g = own Then
                            Continue For
                        End If

                        acc += m_basis(g)(c) * (value - m_intercept(g))
                    Next

                    vquery(c)(i) = acc / std.Max(m_responseScale(c), 1.0E-300)
                Next
            Next

            ' 预测残差坐标
            Dim coords(nGenes - 1)() As Double

            For g As Integer = 0 To nGenes - 1
                coords(g) = New Double(rank - 1) {}

                If Not m_supported(g) Then
                    Continue For
                End If

                For c As Integer = 0 To rank - 1
                    Dim acc As Double = 0.0

                    For f As Integer = 0 To featureCount - 1
                        acc += features(g)(f) * m_coefficients(g)(f)(c)
                    Next

                    coords(g)(c) = acc
                Next
            Next

            Dim correction(nGenes - 1)() As Double

            For g As Integer = 0 To nGenes - 1
                correction(g) = New Double(active.Count - 1) {}

                If Not m_supported(g) Then
                    Continue For
                End If

                For i As Integer = 0 To active.Count - 1
                    Dim acc As Double = 0.0

                    For c As Integer = 0 To rank - 1
                        acc += coords(g)(c) * vquery(c)(i)
                    Next

                    correction(g)(i) = m_options.ResidualAlpha * acc
                Next
            Next

            For i As Integer = 0 To active.Count - 1
                Dim p As Integer = active(i)
                Dim column(nGenes - 1) As Double
                Dim delta(nGenes - 1) As Double

                For g As Integer = 0 To nGenes - 1
                    column(g) = query(g)(p)
                    delta(g) = correction(g)(i)
                Next

                Dim adjusted As Double() = EffectShaping.BoundedRatio(
                    New Double()() {column}, New Double()() {delta})(0)
                Dim own As Integer = -1

                If localIndex.TryGetValue(perts(p), own) Then
                    adjusted(own) = column(own)
                End If

                For g As Integer = 0 To nGenes - 1
                    result(g)(p) = If(m_supported(g), adjusted(g), column(g))
                Next
            Next

            Return EffectShaping.PreserveSigns(query, result, genes, perts)
        End Function

        ''' <summary>
        ''' 一步式：拟合残差模型并把修正融合回池化效应。
        ''' </summary>
        Public Shared Function ProfileEffects(l2 As Double()(), l2Tot As Double()(),
                                              stack As Double()()(), counts As Double()(),
                                              srcGenes As String(), srcPerts As String(),
                                              ctrlCounts As Double(), genes As String(), perts As String(),
                                              options As TotalsOptions) As Double()()
            Dim model As ControlResidual = New ControlResidual(options).Fit(stack, counts, srcGenes, srcPerts)

            Return EffectShaping.EvidenceAgreement(l2, l2Tot,
                                                   model.Predict(l2, ctrlCounts, genes, perts))
        End Function

#Region "kernels"

        ''' <summary>跨记录的逆方差均值（缺失按 0 填、方差缺失用该基因的典型值）。</summary>
        Private Shared Function IvwBaseline(values As Double()()()) As Double()()
            Dim nRecords As Integer = values.Length
            Dim nGenes As Integer = If(nRecords = 0, 0, values(0).Length)
            Dim nPerts As Integer = If(nGenes = 0, 0, values(0)(0).Length)
            Dim weights As Double()() = RectangularArray.Matrix(Of Double)(nRecords, nGenes)

            For t As Integer = 0 To nRecords - 1
                For g As Integer = 0 To nGenes - 1
                    Dim any As Boolean = False
                    Dim acc As Double = 0.0
                    Dim acc2 As Double = 0.0

                    For p As Integer = 0 To nPerts - 1
                        Dim v As Double = values(t)(g)(p)

                        If Double.IsNaN(v) Then
                            v = 0.0
                        Else
                            any = True
                        End If

                        acc += v
                        acc2 += v * v
                    Next

                    Dim variance As Double = If(nPerts > 0, acc2 / nPerts - (acc / nPerts) ^ 2, 0.0)

                    weights(t)(g) = If(any, 1.0 / std.Max(variance, 1.0E-8), Double.NaN)
                Next
            Next

            ' 缺失权重用该基因的典型权重回填
            For g As Integer = 0 To nGenes - 1
                Dim column(nRecords - 1) As Double

                For t As Integer = 0 To nRecords - 1
                    column(t) = weights(t)(g)
                Next

                Dim fallback As Double = Typical(column)

                For t As Integer = 0 To nRecords - 1
                    If Double.IsNaN(weights(t)(g)) Then
                        weights(t)(g) = fallback
                    End If
                Next
            Next

            Dim result As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)

            For g As Integer = 0 To nGenes - 1
                For p As Integer = 0 To nPerts - 1
                    Dim num As Double = 0.0
                    Dim den As Double = 0.0

                    For t As Integer = 0 To nRecords - 1
                        Dim w As Double = weights(t)(g)
                        Dim v As Double = values(t)(g)(p)

                        num += w * If(Double.IsNaN(v), 0.0, v)
                        den += w
                    Next

                    result(g)(p) = If(den > 0.0, num / den, 0.0)
                Next
            Next

            Return result
        End Function

        ''' <summary>构造三维锯齿数组：<c>a×b×c</c>。</summary>
        Private Shared Function Cube(Of T)(a As Integer, b As Integer, c As Integer) As T()()()
            Dim result(If(a = 0, -1, a - 1))()() As T

            For i As Integer = 0 To a - 1
                result(i) = RectangularArray.Matrix(Of T)(b, c)
            Next

            Return result
        End Function

        Private Shared Function Typical(values As Double()) As Double
            Dim finite As Double() = values.Where(Function(v) Not Double.IsNaN(v)).ToArray()

            If finite.Length = 0 Then
                Return 1.0
            End If

            Return finite.Average
        End Function

        ''' <summary>逐记录的控制 log1p CPM（只在归一化基因上计算）。</summary>
        Private Shared Function CountsLogCpm(counts As Double()(), normalization As Boolean(),
                                             nRecords As Integer, nGenes As Integer) As Double()()
            Dim logCpm As Double()() = RectangularArray.Matrix(Of Double)(nRecords, nGenes)

            For t As Integer = 0 To nRecords - 1
                For g As Integer = 0 To nGenes - 1
                    logCpm(t)(g) = Double.NaN
                Next
            Next

            For t As Integer = 0 To nRecords - 1
                Dim maximum As Double = 0.0
                Dim observed As Boolean = False

                For g As Integer = 0 To nGenes - 1
                    Dim v As Double = CountsAt(counts, t, g, nRecords, nGenes)

                    If Not Double.IsNaN(v) Then
                        observed = True
                        maximum = std.Max(maximum, v)
                    End If
                Next

                If Not observed OrElse maximum <= 0.0 Then
                    Continue For
                End If

                Dim scaled(nGenes - 1) As Double
                Dim total As Double = 0.0

                For g As Integer = 0 To nGenes - 1
                    Dim v As Double = CountsAt(counts, t, g, nRecords, nGenes)

                    scaled(g) = If(Double.IsNaN(v), 0.0, v / maximum)
                    total += scaled(g)
                Next

                If total <= 0.0 Then
                    Continue For
                End If

                For g As Integer = 0 To nGenes - 1
                    Dim v As Double = CountsAt(counts, t, g, nRecords, nGenes)

                    If Not Double.IsNaN(v) AndAlso normalization(g) Then
                        logCpm(t)(g) = std.Log(1.0 + 1000000.0 * scaled(g) / total)
                    End If
                Next
            Next

            Return logCpm
        End Function

        Private Shared Function CountsAt(counts As Double()(), t As Integer, g As Integer,
                                         nRecords As Integer, nGenes As Integer) As Double
            If counts Is Nothing OrElse t >= counts.Length Then
                Return Double.NaN
            End If

            Dim row As Double() = counts(t)

            If row Is Nothing OrElse g >= row.Length Then
                Return Double.NaN
            End If

            Return row(g)
        End Function

        ''' <summary>由训练坐标构造"残差 → 低秩坐标"的线性投影。</summary>
        Private Shared Function Projection(vtrain As Double()(), rank As Integer,
                                           nPerts As Integer, ridge As Double) As Double()()
            Dim gram As Double()() = RectangularArray.Matrix(Of Double)(rank, rank)

            For a As Integer = 0 To rank - 1
                For b As Integer = 0 To rank - 1
                    Dim acc As Double = 0.0

                    For p As Integer = 0 To nPerts - 1
                        acc += vtrain(a)(p) * vtrain(b)(p)
                    Next

                    gram(a)(b) = acc + If(a = b, ridge, 0.0)
                Next
            Next

            Dim rhs As Double()() = RectangularArray.Matrix(Of Double)(rank, nPerts)

            For a As Integer = 0 To rank - 1
                For p As Integer = 0 To nPerts - 1
                    rhs(a)(p) = vtrain(a)(p)
                Next
            Next

            Dim solution As GeneralMatrix = New NumericMatrix(gram, rank, rank).Solve(New NumericMatrix(rhs, rank, nPerts))
            Dim packed As Double()() = solution.ArrayPack()
            Dim result As Double()() = RectangularArray.Matrix(Of Double)(nPerts, rank)

            For a As Integer = 0 To rank - 1
                For p As Integer = 0 To nPerts - 1
                    result(p)(a) = packed(a)(p)
                Next
            Next

            Return result
        End Function

        ''' <summary>由新上下文的控制计数构造特征矩阵（基因×特征）。</summary>
        Private Function TargetFeatures(counts As Double()) As Double()()
            Dim nGenes As Integer = m_normalizationGenes.Length
            Dim featureCount As Integer = 1 + m_globalLoadings(0).Length
            Dim result As Double()() = RectangularArray.Matrix(Of Double)(nGenes, featureCount)

            Dim aligned(If(nGenes = 0, -1, nGenes - 1)) As Double

            For g As Integer = 0 To nGenes - 1
                aligned(g) = If(m_normalizationGenes(g) AndAlso g < counts.Length, counts(g), Double.NaN)
            Next

            Dim logCpm As Double()() = CountsLogCpm(New Double()() {aligned}, m_normalizationGenes, 1, nGenes)
            Dim local(nGenes - 1) As Double

            For g As Integer = 0 To nGenes - 1
                local(g) = If(Double.IsNaN(logCpm(0)(g)), 0.0, (logCpm(0)(g) - m_controlMean(g)) / m_controlScale(g))
            Next

            For g As Integer = 0 To nGenes - 1
                result(g)(0) = local(g) - m_featureCenter(g)(0)

                For c As Integer = 0 To m_globalLoadings(0).Length - 1
                    Dim value As Double = local(g) * m_globalLoadings(g)(c) / m_globalScale(c)

                    result(g)(1 + c) = value - m_featureCenter(g)(1 + c)
                Next

                For f As Integer = 0 To featureCount - 1
                    result(g)(f) = std.Max(std.Min(result(g)(f), m_options.ResidualClip), -m_options.ResidualClip)
                Next
            Next

            Return result
        End Function

#End Region

    End Class
End Namespace
