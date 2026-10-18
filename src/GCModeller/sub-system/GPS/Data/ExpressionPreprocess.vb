Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.GPS.Config

Namespace Data

    ''' <summary>
    ''' 预处理后的时序表达数据：矩阵已按配置完成文库归一化、log 变换与逐基因标准化，
    ''' 并保留反标准化所需的均值与标准差。
    ''' </summary>
    ''' <remarks>
    ''' 反标准化只回到"log-CPM"尺度（<c>z · sd + mean</c>），
    ''' 若要回到原始计数单位还需要按样本的文库大小反算，
    ''' 该步骤在虚拟扰动结果解读中通常不需要，因此不默认执行。
    ''' </remarks>
    Public Class PreprocessedExpression

        ''' <summary>基因名称，顺序与 <see cref="Matrix"/> 的行一一对应</summary>
        Public Property GeneNames As String()

        ''' <summary>样本名称，顺序与 <see cref="Matrix"/> 的列一一对应</summary>
        Public Property SampleNames As String()

        ''' <summary>每个样本对应的时间点标签</summary>
        Public Property TimePoints As Double()

        ''' <summary>预处理后的表达矩阵 [基因, 样本]</summary>
        Public Property Matrix As Double(,)

        ''' <summary>每个基因在标准化前的均值（log-CPM 尺度）</summary>
        Public Property GeneMeans As Double()

        ''' <summary>每个基因在标准化前的标准差（样本数不足时为 1）</summary>
        Public Property GeneSDs As Double()

        ''' <summary>每个样本的原始文库大小（CPM 归一化的分母）</summary>
        Public Property LibrarySizes As Double()

        ''' <summary>基因数量</summary>
        ''' <returns>矩阵行数</returns>
        Public ReadOnly Property NGene As Integer
            Get
                Return GeneNames.Length
            End Get
        End Property

        ''' <summary>样本数量</summary>
        ''' <returns>矩阵列数</returns>
        Public ReadOnly Property NSample As Integer
            Get
                Return SampleNames.Length
            End Get
        End Property

        ''' <summary>
        ''' 把标准化值反算回 log-CPM 尺度。
        ''' </summary>
        ''' <param name="geneIndex">基因行号</param>
        ''' <param name="normalized">标准化后的取值</param>
        ''' <returns>log-CPM 尺度的取值</returns>
        Public Function Denormalize(geneIndex As Integer, normalized As Double) As Double
            If GeneSDs Is Nothing OrElse GeneMeans Is Nothing Then
                Return normalized
            End If

            Return normalized * GeneSDs(geneIndex) + GeneMeans(geneIndex)
        End Function

        ''' <summary>
        ''' 把标准化值反算回原始技术尺度（log-CPM → CPM → 计数近似）。
        ''' </summary>
        ''' <param name="geneIndex">基因行号</param>
        ''' <param name="sampleIndex">样本列号（用于取文库大小）</param>
        ''' <param name="normalized">标准化后的取值</param>
        ''' <param name="logTransformed">预处理时是否做过 log2(x+1)</param>
        ''' <returns>原始尺度下的近似取值</returns>
        Public Function BackTransform(geneIndex As Integer,
                                       sampleIndex As Integer,
                                       normalized As Double,
                                       Optional logTransformed As Boolean = True) As Double

            Dim logValue As Double = Denormalize(geneIndex, normalized)
            Dim cpm As Double = If(logTransformed, System.Math.Pow(2.0, logValue) - 1.0, logValue)

            If LibrarySizes IsNot Nothing AndAlso sampleIndex >= 0 AndAlso sampleIndex < LibrarySizes.Length Then
                Dim librarySize As Double = LibrarySizes(sampleIndex)

                If librarySize > 0.0 Then
                    Return cpm * librarySize / 1000000.0
                End If
            End If

            Return cpm
        End Function

        ''' <summary>
        ''' 按时间点标签归并，返回去重并升序排列的时间点。
        ''' </summary>
        ''' <returns>唯一时间点数组</returns>
        Public Function UniqueTimes() As Double()
            If TimePoints Is Nothing OrElse TimePoints.Length = 0 Then
                Return New Double() {}
            End If

            Dim sorted As Double() = CType(TimePoints.Clone(), Double())
            Call System.Array.Sort(sorted)

            Dim result As New List(Of Double)()

            For Each t As Double In sorted
                If result.Count = 0 OrElse System.Math.Abs(t - result(result.Count - 1)) > 1.0E-09 Then
                    Call result.Add(t)
                End If
            Next

            Return result.ToArray()
        End Function

    End Class

    ''' <summary>
    ''' 时序表达数据的预处理管线（method.md §2.2）。
    ''' </summary>
    ''' <remarks>
    ''' 处理顺序固定为：CPM 文库归一化（按样本列）→ log2(x+1) → 高变异基因过滤 →
    ''' 逐基因 z-score（跨时间点）。任何一步都可以通过 <see cref="GPSConfig"/> 关闭。
    ''' 高变异过滤会额外保留调用方指定的基因（通常是先验网络中的转录因子），
    ''' 避免先验网络被过滤成空图。
    ''' </remarks>
    Public Module ExpressionPreprocess

        ''' <summary>
        ''' 对时序表达数据执行完整预处理。
        ''' </summary>
        ''' <param name="data">原始时序表达数据</param>
        ''' <param name="config">配置对象（决定是否归一化、log 变换、标准化与过滤规模）</param>
        ''' <param name="forcedGenes">
        ''' 过滤时必须保留的基因名（大小写不敏感）；通常传入先验网络的转录因子集合
        ''' </param>
        ''' <returns>预处理后的表达数据</returns>
        Public Function Preprocess(data As GeneExpressionData,
                                   config As GPSConfig,
                                   Optional forcedGenes As IEnumerable(Of String) = Nothing) As PreprocessedExpression

            If data Is Nothing Then Throw New ArgumentNullException(NameOf(data))
            If config Is Nothing Then Throw New ArgumentNullException(NameOf(config))
            If data.Matrix Is Nothing Then Throw New ArgumentException("表达矩阵为空", NameOf(data))

            Dim nGene As Integer = data.NGene
            Dim nSample As Integer = data.NSample

            If nGene <= 0 OrElse nSample <= 0 Then
                Throw New ArgumentException($"表达矩阵尺寸非法：{nGene} 个基因 × {nSample} 个样本")
            End If

            Dim work As Double(,) = CType(data.Matrix.Clone(), Double(,))
            Dim librarySizes As Double() = New Double(nSample - 1) {}

            ' ---- 1. CPM 文库归一化（按样本列） ----
            Dim rawLibrary As Double() = New Double(nSample - 1) {}

            For j As Integer = 0 To nSample - 1
                Dim total As Double = 0.0

                For i As Integer = 0 To nGene - 1
                    total += System.Math.Max(0.0, work(i, j))
                Next

                rawLibrary(j) = total
                librarySizes(j) = total

                If config.CpmNormalize AndAlso total > 0.0 Then
                    Dim scale As Double = 1000000.0 / total

                    For i As Integer = 0 To nGene - 1
                        work(i, j) = System.Math.Max(0.0, work(i, j)) * scale
                    Next
                End If
            Next

            ' ---- 2. log2(x + 1) ----
            If config.LogTransform Then
                For i As Integer = 0 To nGene - 1
                    For j As Integer = 0 To nSample - 1
                        work(i, j) = System.Math.Log(work(i, j) + 1.0, 2.0)
                    Next
                Next
            End If

            ' ---- 3. 高变异基因过滤（保留既定的强制基因） ----
            Dim selected As Integer() = SelectGenes(work, data.GeneNames, config.MaxGenes, forcedGenes)

            If selected.Length <> nGene Then
                Dim filtered As Double(,) = New Double(selected.Length - 1, nSample - 1) {}
                Dim names As String() = New String(selected.Length - 1) {}

                For r As Integer = 0 To selected.Length - 1
                    names(r) = data.GeneNames(selected(r))

                    For j As Integer = 0 To nSample - 1
                        filtered(r, j) = work(selected(r), j)
                    Next
                Next

                work = filtered
                nGene = selected.Length
            End If

            Dim keptNames As String()

            If selected.Length <> data.GeneNames.Length Then
                keptNames = New String(nGene - 1) {}

                For r As Integer = 0 To nGene - 1
                    keptNames(r) = data.GeneNames(selected(r))
                Next
            Else
                keptNames = CType(data.GeneNames.Clone(), String())
            End If

            ' ---- 4. 逐基因 z-score（跨样本） ----
            Dim means As Double() = New Double(nGene - 1) {}
            Dim sds As Double() = New Double(nGene - 1) {}

            For i As Integer = 0 To nGene - 1
                Dim sum As Double = 0.0

                For j As Integer = 0 To nSample - 1
                    sum += work(i, j)
                Next

                Dim mean As Double = sum / nSample
                Dim ss As Double = 0.0

                For j As Integer = 0 To nSample - 1
                    Dim d As Double = work(i, j) - mean
                    ss += d * d
                Next

                Dim sd As Double = If(nSample > 1, System.Math.Sqrt(ss / (nSample - 1)), 1.0)

                If sd < 1.0E-12 Then sd = 1.0

                means(i) = mean
                sds(i) = sd

                If config.ZScore Then
                    For j As Integer = 0 To nSample - 1
                        work(i, j) = (work(i, j) - mean) / sd
                    Next
                End If
            Next

            Return New PreprocessedExpression With {
                .GeneNames = keptNames,
                .SampleNames = If(data.SampleNames IsNot Nothing, CType(data.SampleNames.Clone(), String()), Nothing),
                .TimePoints = If(data.TimePoints IsNot Nothing, CType(data.TimePoints.Clone(), Double()), Nothing),
                .Matrix = work,
                .GeneMeans = means,
                .GeneSDs = sds,
                .LibrarySizes = librarySizes
            }
        End Function

        ''' <summary>
        ''' 按跨样本方差选择基因，并保证强制保留的基因入选。
        ''' </summary>
        ''' <param name="matrix">已做 log 变换的矩阵 [基因, 样本]</param>
        ''' <param name="geneNames">基因名</param>
        ''' <param name="maxGenes">保留的基因数上限；0 表示全部保留</param>
        ''' <param name="forcedGenes">必须保留的基因名</param>
        ''' <returns>升序排列的基因行号；全部保留时返回完整序号</returns>
        Private Function SelectGenes(matrix As Double(,),
                                     geneNames As String(),
                                     maxGenes As Integer,
                                     forcedGenes As IEnumerable(Of String)) As Integer()

            Dim nGene As Integer = matrix.GetLength(0)
            Dim nSample As Integer = matrix.GetLength(1)

            If maxGenes <= 0 OrElse maxGenes >= nGene Then
                Return Enumerable.Range(0, nGene).ToArray()
            End If

            ' ---- 计算方差并降序排名 ----
            Dim variance As Double() = New Double(nGene - 1) {}

            For i As Integer = 0 To nGene - 1
                Dim sum As Double = 0.0

                For j As Integer = 0 To nSample - 1
                    sum += matrix(i, j)
                Next

                Dim mean As Double = sum / nSample
                Dim ss As Double = 0.0

                For j As Integer = 0 To nSample - 1
                    Dim d As Double = matrix(i, j) - mean
                    ss += d * d
                Next

                variance(i) = ss / If(nSample > 1, nSample - 1, 1)
            Next

            Dim ranked As Integer() = Enumerable.Range(0, nGene).
                OrderByDescending(Function(i) variance(i)).
                ToArray()

            Dim keep As New HashSet(Of Integer)()

            ' ---- 强制保留的基因优先 ----
            If forcedGenes IsNot Nothing Then
                Dim lookup As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

                For i As Integer = 0 To nGene - 1
                    lookup(geneNames(i)) = i
                Next

                For Each name As String In forcedGenes
                    Dim idx As Integer = -1

                    If Not String.IsNullOrEmpty(name) AndAlso lookup.TryGetValue(name, idx) Then
                        Call keep.Add(idx)
                    End If
                Next
            End If

            ' ---- 按方差从高到低补足 ----
            For Each idx As Integer In ranked
                If keep.Count >= maxGenes Then Exit For

                Call keep.Add(idx)
            Next

            Dim result As Integer() = keep.ToArray()
            Call System.Array.Sort(result)

            Return result
        End Function

    End Module

End Namespace
