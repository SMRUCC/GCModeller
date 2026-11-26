Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

Namespace Data

    ''' <summary>
    ''' 流式装配：把一个"基因 × 细胞"的 CSV 变成 CellularAI 需要的"细胞 × 基因"对照矩阵。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 流程只有一次磁盘扫描：先读表头拿到细胞轴，再让 <see cref="Document.StreamRows"/>
    ''' 逐行吐出基因，直接交给 <c>AsCsrMatrix</c> 转置成"细胞 × 基因"的 CSR。
    ''' </para>
    ''' <para>
    ''' <b>基因轴怎么来</b>：转换函数只返回矩阵、不返回行名，因此在枚举外层套一个
    ''' "记录 + 透传"的迭代器（<see cref="Trace"/>）在同一个枚举过程中收集 <c>geneID</c>，
    ''' 既不二次遍历，也不把行缓存下来。
    ''' </para>
    ''' <para>
    ''' <b>列过滤</b>：<see cref="Document.StreamRows"/> 只支持"排除列"，
    ''' 因此按期别过滤时把"不保留的 barcode"整体作为 <c>excludes</c> 传进去，
    ''' 行内的 <c>experiments</c> 会自动只包含保留列。
    ''' </para>
    ''' </remarks>
    Public Class ExpressionStream

        ''' <summary>基因轴（顺序 = CSV 行序）。</summary>
        Public Property Genes As String()

        ''' <summary>细胞轴（顺序 = 保留列顺序）。</summary>
        Public Property Cells As String()

        ''' <summary>对照矩阵：细胞 × 基因。</summary>
        Public Property Controls As CsrMatrix

        ''' <summary>细胞元数据。</summary>
        Public Property Metadata As CellMetadata

        ''' <summary>实际使用的期别过滤（未过滤时为空数组）。</summary>
        Public Property StageFilter As String()

        ''' <summary>没有元数据记录的细胞。</summary>
        Public Property MissingMetadata As String()

        ''' <summary>
        ''' 流式加载表达矩阵并装配成"细胞 × 基因"。
        ''' </summary>
        ''' <param name="exprPath">表达矩阵 CSV（行=基因，列=细胞）</param>
        ''' <param name="metaPath">细胞元数据 CSV</param>
        ''' <param name="stageFilter">只保留这些期别的细胞；<c>Nothing</c>/空表示全量</param>
        ''' <param name="verbose">是否打印 nnz / 稀疏率 / 耗时</param>
        Public Shared Function Load(exprPath As String, metaPath As String,
                                    Optional stageFilter As String() = Nothing,
                                    Optional verbose As Boolean = True,
                                    Optional limitPerStage As Integer = 0) As ExpressionStream
            If Not File.Exists(exprPath) Then
                Throw New FileNotFoundException($"expression matrix not found: {exprPath}", exprPath)
            End If

            Dim metadata As CellMetadata = CellMetadata.Load(metaPath)
            Dim header As String() = ReadHeader(exprPath)
            Dim allCells As String() = header.Skip(1).ToArray

            Dim keptCells As String() = allCells

            If stageFilter IsNot Nothing AndAlso stageFilter.Length > 0 Then
                keptCells = FilterCells(allCells, metadata, stageFilter)
            End If
            If limitPerStage > 0 Then
                keptCells = SamplePerStage(keptCells, metadata, limitPerStage)
            End If

            Dim excludes As Index(Of String) = Nothing

            If keptCells.Length <> allCells.Length Then
                Dim keep As New HashSet(Of String)(keptCells, StringComparer.Ordinal)
                Dim dropped As String() = allCells.Where(Function(id) Not keep.Contains(id)).ToArray

                excludes = New Index(Of String)(dropped)
            End If

            If keptCells.Length = 0 Then
                Throw New InvalidOperationException(
                    $"no cell survives the stage filter ({String.Join(", ", If(stageFilter, New String() {}))})")
            End If

            Dim genes As New List(Of String)
            Dim source As IEnumerable(Of DataFrameRow) = Document.StreamRows(exprPath, excludes)

            ' 单遍：记录基因名 + 原样透传给转换函数
            Dim controls As CsrMatrix = MatrixBuilder.AsCsrMatrix(Trace(source, genes), keptCells, verbose)

            Return New ExpressionStream With {
                .Genes = genes.ToArray,
                .Cells = keptCells,
                .Controls = controls,
                .Metadata = metadata,
                .StageFilter = If(stageFilter, New String() {}),
                .MissingMetadata = metadata.MissingBarcodes(keptCells)
            }
        End Function

#Region "kernels"

        ''' <summary>读取表头（首个非空行）并切分细胞轴。</summary>
        Private Shared Function ReadHeader(path As String) As String()
            Using reader As New StreamReader(path)
                Dim line As String = reader.ReadLine()

                If line Is Nothing Then
                    Throw New InvalidDataException($"the expression matrix is empty: {path}")
                End If

                Return line.Split(","c).
                    Select(Function(s) s.Trim().Trim(""""c)).
                    ToArray
            End Using
        End Function

        ''' <summary>按期别过滤细胞（保持表内顺序）。</summary>
        Private Shared Function FilterCells(cells As String(), metadata As CellMetadata,
                                            stages As String()) As String()
            Dim wanted As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Dim result As New List(Of String)

            For Each stage As String In stages
                Call wanted.Add(If(stage, "").Trim())
            Next

            For Each barcode As String In cells
                Dim stage As String = If(metadata.StageOf(barcode), "").Trim()

                If wanted.Contains(stage) Then
                    Call result.Add(barcode)
                End If
            Next

            Return result.ToArray
        End Function

        ''' <summary>
        ''' 按期别等距抽样（保留表内顺序与时间序，避免随机抽样引入的块效应）。
        ''' </summary>
        Private Shared Function SamplePerStage(cells As String(), metadata As CellMetadata,
                                                limit As Integer) As String()
            Dim groups As New Dictionary(Of String, List(Of String))(StringComparer.OrdinalIgnoreCase)

            For Each barcode As String In cells
                Dim stage As String = If(metadata.StageOf(barcode), "").Trim()

                If Not groups.ContainsKey(stage) Then
                    groups(stage) = New List(Of String)
                End If

                Call groups(stage).Add(barcode)
            Next

            Dim result As New List(Of String)

            For Each stage As String In cells.
                Select(Function(id) If(metadata.StageOf(id), "").Trim()).
                Distinct()

                Dim members As List(Of String) = groups(stage)

                If members.Count <= limit Then
                    Call result.AddRange(members)
                    Continue For
                End If

                Dim stride As Double = members.Count / CDbl(limit)

                For i As Integer = 0 To limit - 1
                    Call result.Add(members(CInt(std.Floor(i * stride))))
                Next
            Next

            ' 恢复全局顺序
            Dim order As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For i As Integer = 0 To cells.Length - 1
                order(cells(i)) = i
            Next

            Return result.OrderBy(Function(id) order(id)).ToArray
        End Function

        ''' <summary>透传迭代器：在枚举过程中记录基因名，不缓存行数据。</summary>
        Private Shared Iterator Function Trace(source As IEnumerable(Of DataFrameRow),
                                               sink As List(Of String)) As IEnumerable(Of DataFrameRow)
            For Each row As DataFrameRow In source
                Call sink.Add(If(row.geneID, "gene_" & sink.Count))

                Yield row
            Next
        End Function

#End Region

    End Class
End Namespace
