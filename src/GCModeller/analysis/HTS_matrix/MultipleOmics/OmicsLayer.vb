Namespace MultipleOmics

    ''' <summary>
    ''' 单个组学层的「特征 × 样本」矩阵模型。
    ''' </summary>
    ''' <remarks>
    ''' 每个组学层独立持有自己的特征集合与样本向量，组学之间只在样本轴上对齐，
    ''' 特征轴保持独立（这正是"保留组学来源"的实现基础）。
    ''' </remarks>
    Public Class OmicsLayer

        ''' <summary>
        ''' 组学层名称，例如 <c>mRNA</c> / <c>methylation</c> / <c>protein</c>。
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String

        ''' <summary>
        ''' 原始（或经过层内标准化之后的）表达/丰度矩阵，行是特征、列是样本。
        ''' </summary>
        ''' <returns></returns>
        Public Property Expr As Matrix

        ''' <summary>
        ''' 当前矩阵已经施加过的层内标准化方法名，仅用于溯源与日志。
        ''' </summary>
        ''' <returns></returns>
        Public Property Normalization As String = NameOf(OmicsNormalization.None)

        ''' <summary>
        ''' 方向约定：<c>+1</c> 表示数值升高代表通路活性升高（mRNA / protein），
        ''' <c>-1</c> 表示数值升高代表通路活性下降（例如甲基化 beta 值），
        ''' 打分完成后会对该层的 ES 取负以统一到"活性方向"。
        ''' </summary>
        ''' <returns></returns>
        Public Property Direction As Integer = 1

        ''' <summary>
        ''' 与 <see cref="Expr"/> 列一一对应的样本 id 向量。
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property SampleID As String()
            Get
                Return Expr.sampleID
            End Get
        End Property

        ''' <summary>
        ''' 与 <see cref="Expr"/> 行一一对应的特征 id 向量（基因符号）。
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Genes As String()
            Get
                Return Expr.rownames
            End Get
        End Property

        ''' <summary>
        ''' 特征（行）数量。
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property FeatureCount As Integer
            Get
                Return Expr.size
            End Get
        End Property

        ''' <summary>
        ''' 样本（列）数量。
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property SampleCount As Integer
            Get
                Return Expr.sample_count
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{Name}: {FeatureCount} features x {SampleCount} samples"
        End Function

        ''' <summary>
        ''' 由表达矩阵构造组学层：校验样本轴、对重复特征做行内均值聚合。
        ''' </summary>
        ''' <param name="name">组学层名称</param>
        ''' <param name="expr">特征 × 样本 矩阵</param>
        ''' <param name="verbose">是否输出被合并/丢弃的重复特征统计</param>
        Public Shared Function Create(name As String, expr As Matrix, Optional verbose As Boolean = False) As OmicsLayer
            If expr Is Nothing Then
                Throw New ArgumentNullException(NameOf(expr))
            End If
            If String.IsNullOrWhiteSpace(name) Then
                Throw New ArgumentException("the omics layer name can not be empty!", NameOf(name))
            End If
            If expr.sampleID Is Nothing OrElse expr.sampleID.Length < 2 Then
                Throw New ArgumentException($"omics layer '{name}' requires at least two samples!")
            End If
            If expr.expression Is Nothing OrElse expr.expression.Length = 0 Then
                Throw New ArgumentException($"omics layer '{name}' contains no feature row!")
            End If

            Return New OmicsLayer With {
                .Name = name.Trim,
                .Expr = Deduplicate(expr, name, verbose),
                .Normalization = NameOf(OmicsNormalization.None)
            }
        End Function

        ''' <summary>
        ''' 按目标样本轴重排（或截取）列。
        ''' </summary>
        ''' <param name="axis">目标样本 id 序列，必须全部存在于当前层</param>
        Public Function AlignSamples(axis As String()) As OmicsLayer
            Dim pos As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For i As Integer = 0 To SampleID.Length - 1
                If Not pos.ContainsKey(SampleID(i)) Then
                    pos(SampleID(i)) = i
                End If
            Next

            Dim missing As String() = axis.Where(Function(id) Not pos.ContainsKey(id)).ToArray

            If missing.Length > 0 Then
                Throw New InvalidOperationException($"omics layer '{Name}' is missing {missing.Length} sample(s), for example: {String.Join(", ", missing.Take(3))}")
            End If

            Dim take As Integer() = axis.Select(Function(id) pos(id)).ToArray

            Return New OmicsLayer With {
                .Name = Name,
                .Expr = SliceColumns(Expr, take),
                .Normalization = Normalization,
                .Direction = Direction
            }
        End Function

        ''' <summary>
        ''' 只保留给定特征集合中的行（保持原有行顺序）。
        ''' </summary>
        ''' <param name="genes">需要保留的特征 id 集合</param>
        Public Function SubsetGenes(genes As IEnumerable(Of String)) As OmicsLayer
            Dim keep As New HashSet(Of String)(genes, StringComparer.Ordinal)
            Dim rows As DataFrameRow() = Expr.expression _
                .Where(Function(r) keep.Contains(r.geneID)) _
                .ToArray

            Return New OmicsLayer With {
                .Name = Name,
                .Expr = New Matrix With {
                    .sampleID = Expr.sampleID,
                    .tag = Expr.tag,
                    .expression = rows
                },
                .Normalization = Normalization,
                .Direction = Direction
            }
        End Function

        ''' <summary>
        ''' 深拷贝当前层（行数据独立，避免下游标准化污染上游）。
        ''' </summary>
        Public Function Clone() As OmicsLayer
            Dim rows As DataFrameRow() = Expr.expression _
                .Select(Function(r)
                            Return New DataFrameRow With {
                                .geneID = r.geneID,
                                .experiments = DirectCast(r.experiments.Clone(), Double())
                            }
                        End Function) _
                .ToArray

            Return New OmicsLayer With {
                .Name = Name,
                .Expr = New Matrix With {
                    .sampleID = DirectCast(Expr.sampleID.Clone(), String()),
                    .tag = Expr.tag,
                    .expression = rows
                },
                .Normalization = Normalization,
                .Direction = Direction
            }
        End Function

        ''' <summary>
        ''' 按列下标切片，返回新矩阵（不复制未被选中的列）。
        ''' </summary>
        Public Shared Function SliceColumns(expr As Matrix, take As Integer()) As Matrix
            Dim rows As DataFrameRow() = expr.expression _
                .Select(Function(r)
                            Dim v As Double() = New Double(take.Length - 1) {}
                            Dim src As Double() = r.experiments

                            For j As Integer = 0 To take.Length - 1
                                v(j) = src(take(j))
                            Next

                            Return New DataFrameRow With {
                                .geneID = r.geneID,
                                .experiments = v
                            }
                        End Function) _
                .ToArray

            Return New Matrix With {
                .sampleID = take.Select(Function(i) expr.sampleID(i)).ToArray,
                .tag = expr.tag,
                .expression = rows
            }
        End Function

        ''' <summary>
        ''' 合并重复特征：同名特征在样本轴上取算术平均。
        ''' </summary>
        ''' <remarks>
        ''' GSVA 内部通过 <c>expr.rownames.Indexing</c> 建立特征索引，
        ''' 重复的行名会让 <c>Index(Of String)</c> 行为不可预期（甚至抛异常），
        ''' 因此必须在进入打分流程之前完成去重。
        ''' </remarks>
        Private Shared Function Deduplicate(expr As Matrix, name As String, verbose As Boolean) As Matrix
            Dim order As New List(Of String)
            Dim sums As New Dictionary(Of String, Double())(StringComparer.Ordinal)
            Dim counts As New Dictionary(Of String, Integer)(StringComparer.Ordinal)
            Dim duplicates As Integer = 0
            Dim dropped As Integer = 0

            For Each row As DataFrameRow In expr.expression
                Dim key As String = If(row.geneID, "").Trim()

                If key.Length = 0 OrElse row.experiments Is Nothing Then
                    dropped += 1
                    Continue For
                End If

                Dim buffer As Double() = Nothing

                If sums.TryGetValue(key, buffer) Then
                    Dim v As Double() = row.experiments

                    For j As Integer = 0 To buffer.Length - 1
                        buffer(j) += v(j)
                    Next

                    counts(key) += 1
                    duplicates += 1
                Else
                    sums(key) = DirectCast(row.experiments.Clone(), Double())
                    counts(key) = 1
                    order.Add(key)
                End If
            Next

            If verbose AndAlso dropped > 0 Then
                Call $"[moGSVA] omics layer '{name}': {dropped} feature row(s) without a valid id were dropped.".warning
            End If
            If verbose AndAlso duplicates > 0 Then
                Call $"[moGSVA] omics layer '{name}': {duplicates} duplicated feature row(s) were merged by mean.".warning
            End If

            Dim rows As DataFrameRow() = order _
                .Select(Function(key)
                            Dim buffer As Double() = sums(key)
                            Dim n As Integer = counts(key)

                            If n > 1 Then
                                For j As Integer = 0 To buffer.Length - 1
                                    buffer(j) /= n
                                Next
                            End If

                            Return New DataFrameRow With {
                                .geneID = key,
                                .experiments = buffer
                            }
                        End Function) _
                .ToArray

            Return New Matrix With {
                .sampleID = expr.sampleID,
                .tag = If(String.IsNullOrEmpty(expr.tag), $"{name}:deduplicated", expr.tag),
                .expression = rows
            }
        End Function

    End Class

End Namespace
