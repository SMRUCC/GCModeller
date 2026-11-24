Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' the abstract viewer model of the in-memory matrix object and the
''' huge binary matrix file
''' </summary>
''' <remarks>
''' <para>
''' 这里定义了"矩阵视图"的最小契约：既可以被内存之中的 <see cref="Matrix"/> 实现
''' （<see cref="HTSMatrixViewer"/>），也可以被磁盘上的超大型二进制文件实现
''' （<see cref="HTSMatrixReader"/>），上层算法只依赖本抽象即可在"全内存"与
''' "按需读取"之间自由切换。
''' </para>
''' <para>
''' 新增的 <see cref="GetRow(Integer)"/>、<see cref="GetRows(Integer, Integer)"/>、
''' <see cref="Rows"/>、<see cref="GetSample(Integer)"/> 都以可覆盖的默认实现提供
''' （默认实现退化为逐基因查询），因此既有的子类不会被破坏，
''' 而随机访问型的实现可以覆盖它们拿到 O(1) 或 O(块) 的性能。
''' </para>
''' </remarks>
Public MustInherit Class MatrixViewer

    Public ReadOnly Property tag As String
        Get
            Return tagString
        End Get
    End Property

    Protected tagString As String

    Public MustOverride ReadOnly Property SampleIDs As IEnumerable(Of String)

    ''' <summary>
    ''' get gene row features name
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride ReadOnly Property FeatureIDs As IEnumerable(Of String)

    Public Overridable ReadOnly Property Size As (nsample As Integer, nfeature As Integer)
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return (SampleIDs.Count, FeatureIDs.Count)
        End Get
    End Property

    ''' <summary>
    ''' 当前视图是否支持按行号随机访问？
    ''' </summary>
    ''' <remarks>
    ''' 内存视图与未压缩的二进制文件恒为 <c>True</c>；不可 seek 的流（例如 zip 的
    ''' deflate 流）为 <c>False</c>，此时只能通过 <see cref="Rows"/> 顺序读取。
    ''' </remarks>
    Public Overridable ReadOnly Property CanRandomAccess As Boolean
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return True
        End Get
    End Property

    Public MustOverride Function GetSampleOrdinal(sampleID As String) As Integer
    ''' <summary>
    ''' get gene expression across all sample data
    ''' </summary>
    ''' <param name="geneID">a specific gene target</param>
    ''' <returns>
    ''' expression data is aligned with the <see cref="SampleIDs"/>
    ''' </returns>
    Public MustOverride Function GetGeneExpression(geneID As String) As Double()
    ''' <summary>
    ''' get a set of gene expression across a specific sample data
    ''' </summary>
    ''' <param name="geneID">a set of target gene</param>
    ''' <param name="sampleOrdinal">the order index of the specific sample data</param>
    ''' <returns>
    ''' expression data is aligned with the <paramref name="geneID"/> set.
    ''' </returns>
    Public MustOverride Function GetGeneExpression(geneID As String(), sampleOrdinal As Integer) As Double()
    Public MustOverride Sub SetNewGeneIDs(geneIDs As String())
    Public MustOverride Sub SetNewSampleIDs(sampleIDs As String())

    ''' <summary>
    ''' 按行号取一行（默认实现退化为按基因名查询）
    ''' </summary>
    ''' <param name="index">行号，从 0 开始</param>
    ''' <returns></returns>
    Public Overridable Function GetRow(index As Integer) As DataFrameRow
        Dim name As String = FeatureIDs.ElementAt(index)

        Return New DataFrameRow With {
            .geneID = name,
            .experiments = GetGeneExpression(name)
        }
    End Function

    ''' <summary>
    ''' 按行号区间批量取行（默认实现逐行调用 <see cref="GetRow(Integer)"/>）
    ''' </summary>
    ''' <param name="start">起始行号，从 0 开始</param>
    ''' <param name="count">行数</param>
    ''' <returns></returns>
    Public Overridable Function GetRows(start As Integer, count As Integer) As DataFrameRow()
        Dim rows(count - 1) As DataFrameRow

        For i As Integer = 0 To count - 1
            rows(i) = GetRow(start + i)
        Next

        Return rows
    End Function

    ''' <summary>
    ''' 顺序惰性枚举所有行（默认实现逐行调用 <see cref="GetRow(Integer)"/>）
    ''' </summary>
    ''' <returns></returns>
    Public Overridable Iterator Function Rows() As IEnumerable(Of DataFrameRow)
        Dim n As Integer = Size.nfeature

        For i As Integer = 0 To n - 1
            Yield GetRow(i)
        Next
    End Function

    ''' <summary>
    ''' 取一个样本列（默认实现逐行读取之后取该列）
    ''' </summary>
    ''' <param name="sampleOrdinal">样本的列号，从 0 开始</param>
    ''' <returns>与 <see cref="FeatureIDs"/> 对齐的表达值</returns>
    Public Overridable Function GetSample(sampleOrdinal As Integer) As Double()
        Dim n As Integer = Size.nfeature
        Dim v As Double() = New Double(n - 1) {}

        For i As Integer = 0 To n - 1
            v(i) = GetRow(i).experiments(sampleOrdinal)
        Next

        Return v
    End Function

    ''' <summary>
    ''' 把当前视图物化成完整的内存矩阵（大文件慎用）
    ''' </summary>
    ''' <returns></returns>
    Public Overridable Function AsMatrix() As Matrix
        Return New Matrix With {
            .tag = tag,
            .sampleID = SampleIDs.ToArray,
            .expression = Rows().ToArray
        }
    End Function

    Public Overrides Function ToString() As String
        Dim size = Me.Size
        Return $"{tag}({size.nsample}x{size.nfeature})"
    End Function

End Class
