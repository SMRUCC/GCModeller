' ============================================================================
' PerturbationDataset.vb - 扰动实验的数据容器
'
' 单细胞扰动实验的数据是「细胞 × 基因」的计数矩阵 + 每个细胞的条件标签
' （对照组 / 各种扰动）。本类负责：
'   - 计数矩阵、条件标签、条件名称、可选的协变量；
'   - 文库大小（每个细胞的总计数）的计算与归一化；
'   - 条件标签的 one-hot 编码；
'   - 按条件切片（便于做「对照 vs 扰动」的反事实对比）。
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' 单细胞扰动实验数据集。
''' </summary>
Public Class PerturbationDataset

    ''' <summary>计数矩阵 [细胞数, 基因数]。</summary>
    Public Property Counts As Tensor

    ''' <summary>每个细胞的条件标签 [细胞数]。</summary>
    Public Property Conditions As Integer()

    ''' <summary>条件名称；下标即条件标签。</summary>
    Public Property ConditionNames As String()

    ''' <summary>每个细胞的文库大小（总计数）。</summary>
    Public Property LibrarySizes As Double()

    ''' <summary>可选协变量 [细胞数, 协变量数]。</summary>
    Public Property Covariates As Tensor

    ''' <summary>细胞数量。</summary>
    Public ReadOnly Property CellCount As Integer
        Get
            Return If(Counts Is Nothing, 0, Counts.Shape(0))
        End Get
    End Property

    ''' <summary>基因数量。</summary>
    Public ReadOnly Property GeneCount As Integer
        Get
            Return If(Counts Is Nothing, 0, Counts.Shape(1))
        End Get
    End Property

    ''' <summary>条件数量。</summary>
    Public ReadOnly Property ConditionCount As Integer
        Get
            Return If(ConditionNames Is Nothing, 0, ConditionNames.Length)
        End Get
    End Property

    ''' <summary>由计数矩阵与条件标签构造数据集，并自动计算文库大小。</summary>
    Public Shared Function FromCounts(counts As Tensor, conditions As Integer(), names As String()) As PerturbationDataset
        If counts Is Nothing OrElse counts.Rank <> 2 Then
            Throw New ArgumentException("计数矩阵必须是二维张量 [细胞数, 基因数]", NameOf(counts))
        End If

        If conditions Is Nothing OrElse conditions.Length <> counts.Shape(0) Then
            Throw New ArgumentException("条件标签数量必须与细胞数量一致", NameOf(conditions))
        End If

        Dim dataset As New PerturbationDataset With {
            .Counts = counts,
            .Conditions = conditions,
            .ConditionNames = If(names, Array.Empty(Of String)())
        }

        Call dataset.ComputeLibrarySizes()

        Return dataset
    End Function

    ''' <summary>计算每个细胞的总计数作为文库大小。</summary>
    Public Sub ComputeLibrarySizes()
        Dim cells As Integer = CellCount
        Dim genes As Integer = GeneCount
        Dim data As Double() = Counts.Data
        Dim sizes As Double() = New Double(cells - 1) {}

        For i As Integer = 0 To cells - 1
            Dim acc As Double = 0
            Dim baseIndex As Integer = i * genes

            For g As Integer = 0 To genes - 1
                acc += data(baseIndex + g)
            Next

            sizes(i) = If(acc > 0, acc, 1.0)
        Next

        LibrarySizes = sizes
    End Sub

    ''' <summary>
    ''' 按文库大小归一化到指定总计数（默认 1 万），返回新矩阵，不修改原数据。
    ''' </summary>
    Public Function NormalizeLibrarySize(Optional targetSum As Double = 10000) As Tensor
        Dim cells As Integer = CellCount
        Dim genes As Integer = GeneCount
        Dim data As Double() = Counts.Data
        Dim result As New Tensor(cells, genes)
        Dim output As Double() = result.Data

        For i As Integer = 0 To cells - 1
            Dim scale As Double = targetSum / LibrarySizes(i)
            Dim baseIndex As Integer = i * genes

            For g As Integer = 0 To genes - 1
                output(baseIndex + g) = data(baseIndex + g) * scale
            Next
        Next

        Return result
    End Function

    ''' <summary>
    ''' 编码器的标准输入：log1p(按文库大小归一化后的计数)。
    ''' </summary>
    Public Function LogNormalize(Optional targetSum As Double = 10000) As Tensor
        Dim normalized As Tensor = NormalizeLibrarySize(targetSum)

        Return normalized.Apply(Function(x) std.Log(1.0 + std.Max(x, 0)))
    End Function

    ''' <summary>平均文库大小（生成新细胞时用作默认测序深度）。</summary>
    Public Function MeanLibrarySize() As Double
        If LibrarySizes Is Nothing OrElse LibrarySizes.Length = 0 Then
            Return 1.0
        End If

        Dim acc As Double = 0

        For i As Integer = 0 To LibrarySizes.Length - 1
            acc += LibrarySizes(i)
        Next

        Return acc / LibrarySizes.Length
    End Function

    ''' <summary>
    ''' 找出对照条件的下标：名称中包含 control / ctrl / mock / wt（不区分大小写）。
    ''' 找不到时返回 0。
    ''' </summary>
    Public Function ControlCondition() As Integer
        If ConditionNames Is Nothing Then
            Return 0
        End If

        Dim keywords As String() = {"control", "ctrl", "mock", "wildtype", "wild-type", "wt"}

        For i As Integer = 0 To ConditionNames.Length - 1
            Dim name As String = If(ConditionNames(i), "")

            For Each keyword As String In keywords
                If name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 Then
                    Return i
                End If
            Next
        Next

        Return 0
    End Function

    ''' <summary>把条件标签转换为 one-hot 矩阵 [细胞数, 条件数]。</summary>
    Public Function OneHot(conditions As Integer()) As Tensor
        Dim source As Integer() = If(conditions, Me.Conditions)
        Dim cells As Integer = source.Length
        Dim result As New Tensor(cells, ConditionCount)
        Dim output As Double() = result.Data

        For i As Integer = 0 To cells - 1
            Dim index As Integer = source(i)

            If index < 0 OrElse index >= ConditionCount Then
                Throw New ArgumentOutOfRangeException(NameOf(conditions), $"第 {i} 个细胞的条件下标 {index} 越界")
            End If

            output(i * ConditionCount + index) = 1.0
        Next

        Return result
    End Function

    ''' <summary>筛选出指定条件的细胞，返回子数据集。</summary>
    Public Function SelectCondition(condition As Integer) As PerturbationDataset
        Dim rows As New List(Of Integer)

        For i As Integer = 0 To Conditions.Length - 1
            If Conditions(i) = condition Then
                rows.Add(i)
            End If
        Next

        If rows.Count = 0 Then
            Throw New ArgumentException($"条件 {condition} 下没有任何细胞", NameOf(condition))
        End If

        Dim genes As Integer = GeneCount
        Dim subset As New Tensor(rows.Count, genes)
        Dim labels As Integer() = New Integer(rows.Count - 1) {}

        For i As Integer = 0 To rows.Count - 1
            Array.Copy(Counts.Data, rows(i) * genes, subset.Data, i * genes, genes)
            labels(i) = condition
        Next

        Dim dataset As New PerturbationDataset With {
            .Counts = subset,
            .Conditions = labels,
            .ConditionNames = ConditionNames
        }

        Call dataset.ComputeLibrarySizes()

        Return dataset
    End Function

    Public Overrides Function ToString() As String
        Return $"PerturbationDataset(cells={CellCount}, genes={GeneCount}, conditions={ConditionCount})"
    End Function

End Class
