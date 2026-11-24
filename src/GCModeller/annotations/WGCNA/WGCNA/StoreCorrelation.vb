#Region "Microsoft.VisualBasic::StoreCorrelation, annotations\WGCNA\WGCNA\StoreCorrelation.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.IO
Imports System.Threading.Tasks
Imports Microsoft.VisualBasic.Math.Matrix

''' <summary>
''' 预先计算相关矩阵存储（bicor/pearson）的读取句柄。
''' </summary>
''' <remarks>
''' 存储由 <c>BicorMatrix.CreateMatrixStore</c> 写入：数据文件按相关类型命名为
''' <c>bicor.dat</c> / <c>pearson.dat</c>，索引为 <c>index.dat</c>
''' （见 <see cref="CorrelationStoreFiles"/>）。
'''
''' <para>
''' 打开方式采用 <see cref="CorrelationMatrixStore"/> 的<b>外部流模式</b>重载：
''' 文件模式只认 <c>{path}.index</c> 索引布局，与本约定的双文件布局不符。
''' 流模式下 store 不拥有外部流——若数据流是 <see cref="FileStream"/>，store 只提取其
''' SafeFileHandle 走 <c>RandomAccess</c> 无锁并发读；因此本类<b>以字段持有
''' 两个 FileStream</b>，防止句柄被 GC 回收后读取失败。
''' </para>
''' </remarks>
Public Class CorrelationStoreHandle
    Implements IDisposable

    ''' 数据文件流（句柄保命：必须与 store 同生命周期）
    ReadOnly _data As FileStream
    ''' 索引文件流（Open 返回后即不再使用，随句柄统一释放）
    ReadOnly _index As FileStream
    ReadOnly _store As CorrelationMatrixStore
    ReadOnly _kind As CorrelationKind
    ReadOnly _repo As String
    Dim _disposed As Boolean = False

    ''' <summary>底层相关矩阵存储对象</summary>
    Public ReadOnly Property Store As CorrelationMatrixStore
        Get
            Return _store
        End Get
    End Property

    ''' <summary>存储中相关系数的类型（由数据文件名判别）</summary>
    Public ReadOnly Property Kind As CorrelationKind
        Get
            Return _kind
        End Get
    End Property

    ''' <summary>存储目录（repo）的完整路径</summary>
    Public ReadOnly Property Repo As String
        Get
            Return _repo
        End Get
    End Property

    ''' <summary>存储中的基因数量（矩阵阶数 N）</summary>
    Public ReadOnly Property Size As Integer
        Get
            Return _store.size
        End Get
    End Property

    ''' <summary>计算相关系数时使用的样本数</summary>
    Public ReadOnly Property SampleN As Integer
        Get
            Return _store.SampleN
        End Get
    End Property

    ''' <summary>存储中的基因 ID 表（行序与写入时一致）</summary>
    Public ReadOnly Property Genes As String()
        Get
            Return _store.genes
        End Get
    End Property

    Private Sub New(repo As String, kind As CorrelationKind,
                    data As FileStream, index As FileStream, store As CorrelationMatrixStore)
        _repo = repo
        _kind = kind
        _data = data
        _index = index
        _store = store
    End Sub

    ''' <summary>
    ''' 打开一个已完成的存储目录。
    ''' </summary>
    ''' <param name="repo">存储目录（包含 bicor.dat/pearson.dat 与 index.dat）</param>
    ''' <param name="expectKind">
    ''' 期望的相关类型。指定时要求对应的数据文件存在；
    ''' 为 Nothing 时按文件名探测——两个数据文件并存将抛出歧义异常。
    ''' </param>
    ''' <param name="cacheRows">热行 LRU 缓存容量（行数），默认 64</param>
    ''' <returns>读取句柄；调用方负责 <see cref="Dispose"/></returns>
    Public Shared Function Open(repo As String,
                                Optional expectKind As CorrelationKind? = Nothing,
                                Optional cacheRows As Integer = 64) As CorrelationStoreHandle
        If String.IsNullOrEmpty(repo) Then
            Throw New ArgumentException("存储目录不能为空", NameOf(repo))
        End If

        Dim dir As String = IO.Path.GetFullPath(repo)

        If Not IO.Directory.Exists(dir) Then
            Throw New DirectoryNotFoundException($"相关矩阵存储目录不存在: '{dir}'")
        End If

        Dim indexPath As String = IO.Path.Combine(dir, CorrelationStoreFiles.IndexFile)

        If Not IO.File.Exists(indexPath) Then
            Throw New FileNotFoundException(
                $"相关矩阵存储的索引文件不存在（写入端未正常 Complete）: '{indexPath}'", indexPath)
        End If

        Dim kind As CorrelationKind
        Dim dataPath As String

        If expectKind.HasValue Then
            kind = expectKind.Value
            dataPath = IO.Path.Combine(dir, CorrelationStoreFiles.DataFileOf(kind))

            If Not IO.File.Exists(dataPath) Then
                Throw New FileNotFoundException(
                    $"相关矩阵存储目录中缺少 {kind} 类型的数据文件: '{dataPath}'", dataPath)
            End If
        Else
            Dim hasBicor As Boolean = IO.File.Exists(IO.Path.Combine(dir, CorrelationStoreFiles.BicorDataFile))
            Dim hasPearson As Boolean = IO.File.Exists(IO.Path.Combine(dir, CorrelationStoreFiles.PearsonDataFile))

            If hasBicor AndAlso hasPearson Then
                Throw New InvalidDataException(
                    $"相关矩阵存储目录 '{dir}' 同时存在 {CorrelationStoreFiles.BicorDataFile} 与 " &
                    $"{CorrelationStoreFiles.PearsonDataFile}，无法按文件名判别相关类型；请指定 expectKind 或移除其中一个文件")
            ElseIf hasBicor Then
                kind = CorrelationKind.Bicor
                dataPath = IO.Path.Combine(dir, CorrelationStoreFiles.BicorDataFile)
            ElseIf hasPearson Then
                kind = CorrelationKind.Pearson
                dataPath = IO.Path.Combine(dir, CorrelationStoreFiles.PearsonDataFile)
            Else
                Throw New FileNotFoundException(
                    $"相关矩阵存储目录 '{dir}' 中既没有 {CorrelationStoreFiles.BicorDataFile} 也没有 " &
                    $"{CorrelationStoreFiles.PearsonDataFile}")
            End If
        End If

        Dim data As New FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read,
                                   bufferSize:=4096, options:=FileOptions.RandomAccess)
        Dim index As New FileStream(indexPath, FileMode.Open, FileAccess.Read, FileShare.Read)

        Try
            Dim store As CorrelationMatrixStore = CorrelationMatrixStore.Open(data, index, cacheRows, storePath:=dataPath)

            Return New CorrelationStoreHandle(dir, kind, data, index, store)
        Catch
            Call data.Dispose()
            Call index.Dispose()
            Throw
        End Try
    End Function

    ''' <summary>
    ''' 查询基因在存储中的行号（不存在返回 -1）
    ''' </summary>
    Public Function IndexOf(geneId As String) As Integer
        Return _store.IndexOf(geneId)
    End Function

    ''' <summary>是否包含指定基因</summary>
    Public Function Contains(geneId As String) As Boolean
        Return _store.Contains(geneId)
    End Function

    ''' <summary>释放存储与文件句柄（流由本对象创建并持有，负责关闭）</summary>
    Public Sub Dispose() Implements IDisposable.Dispose
        If _disposed Then
            Return
        End If

        _disposed = True

        Try
            Call _store.Dispose()
        Catch ex As Exception
            Call $"failed to dispose correlation store '{_repo}': {ex.Message}".warning
        End Try

        Try
            Call _index.Dispose()
        Catch ex As Exception
            Call ex.Message.warning
        End Try

        Try
            Call _data.Dispose()
        Catch ex As Exception
            Call ex.Message.warning
        End Try
    End Sub
End Class

''' <summary>
''' 相关矩阵存储的子矩阵装配：供 WGCNA blockwise 从磁盘 store 切片出块内相关矩阵，
''' 替代 <see cref="TensorCorrelation.Create(Double()())"/> 的表达式重算。
''' </summary>
''' <remarks>
''' 装配结果与 GEMM 路径的数据契约完全一致（行优先 n x n <see cref="Double"/>，
''' 经 <see cref="TensorCorrelation.FromBuffer"/> 注入既有管线，替代
''' <c>TensorCorrelation.Create</c> 的表达式重算），并做三处数值规整：
''' <list type="bullet">
''' <item><description>严格对称化：取上下三角均值，消除逐对双算 + Float32 截留的 ULP 级不对称；</description></item>
''' <item><description>NaN 归零：与 <c>TensorOps.ClampUnitInPlace</c> 的 GEMM 路径语义一致；</description></item>
''' <item><description>值域截断到 [-1, 1]：与 <c>ClampUnitInPlace</c> 一致（两参数 GetPearson 不做截断）。</description></item>
''' </list>
''' 对角线置 1（与写入端约定一致；下游邻接构建会自行去对角线）。
''' </remarks>
Public Module StoreCorrelation

    ''' <summary>
    ''' 把输入基因 ID 列表映射为 store 行号（ID 忽略大小写匹配）。
    ''' </summary>
    ''' <param name="store">已打开的相关矩阵存储</param>
    ''' <param name="geneIds">输入矩阵的基因 ID（行序）</param>
    ''' <returns>
    ''' 与输入等长的 store 行号数组；任一基因不在 store 中、或输入存在重复/空基因 ID 时返回 Nothing
    ''' （调用方应据此回退到表达式重算）。
    ''' </returns>
    Public Function MapGenes(store As CorrelationMatrixStore, geneIds As String()) As Integer()
        If store Is Nothing Then
            Return Nothing
        End If
        If geneIds Is Nothing OrElse geneIds.Length = 0 Then
            Return Nothing
        End If

        Dim rows(geneIds.Length - 1) As Integer
        Dim seen As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To geneIds.Length - 1
            Dim gid As String = geneIds(i)

            If String.IsNullOrEmpty(gid) OrElse Not seen.Add(gid) Then
                Return Nothing
            End If

            Dim row As Integer = store.IndexOf(gid)

            If row < 0 Then
                Return Nothing
            End If

            rows(i) = row
        Next

        Return rows
    End Function

    ''' <summary>
    ''' 返回输入中不在 store 内的基因 ID（校验失败的诊断信息）
    ''' </summary>
    Public Function MissingGenes(store As CorrelationMatrixStore, geneIds As String()) As String()
        If store Is Nothing OrElse geneIds Is Nothing Then
            Return New String() {}
        End If

        Return geneIds _
            .Where(Function(gid) String.IsNullOrEmpty(gid) OrElse store.IndexOf(gid) < 0) _
            .ToArray()
    End Function

    ''' <summary>
    ''' 返回输入中重复出现的基因 ID（校验失败的诊断信息）
    ''' </summary>
    Public Function DuplicateGenes(geneIds As String()) As String()
        If geneIds Is Nothing Then
            Return New String() {}
        End If

        Return geneIds _
            .GroupBy(Function(gid) gid, StringComparer.OrdinalIgnoreCase) _
            .Where(Function(g) g.Count() > 1) _
            .Select(Function(g) g.Key) _
            .ToArray()
    End Function

    ''' <summary>
    ''' 从 store 中抽取给定基因子集的相关子矩阵（行优先 n x n）。
    ''' </summary>
    ''' <param name="store">已打开的相关矩阵存储</param>
    ''' <param name="storeRows">子集内每个基因在 store 中的行号（由 <see cref="MapGenes"/> 生成后按块内顺序索引）</param>
    ''' <returns>
    ''' 相关矩阵对象（严格对称、NaN→0、对角线 1，sampleSize 取 <see cref="CorrelationMatrixStore.SampleN"/>）。
    ''' 每行只做一次全行块读 + 列切片（O(n·N)），多线程并发读由 store 的
    ''' SafeFileHandle + RandomAccess 无锁路径保证安全。
    ''' </returns>
    Public Function Correlation(store As CorrelationMatrixStore, storeRows As Integer()) As TensorCorrelation
        If store Is Nothing Then
            Throw New ArgumentNullException(NameOf(store))
        End If
        If storeRows Is Nothing OrElse storeRows.Length = 0 Then
            Throw New ArgumentException("子集行号列表不能为空", NameOf(storeRows))
        End If

        Dim n As Integer = storeRows.Length
        Dim buf(n * n - 1) As Double

        ' ① 逐行读取 + 列切片（读完全行后立即丢弃，峰值内存仅为 并发度 x N 个 Single）
        Call Parallel.For(0, n,
            Sub(i)
                Dim full As Single() = store.ReadRow(storeRows(i))
                Dim offset As Integer = i * n

                For j As Integer = 0 To n - 1
                    Dim v As Double = full(storeRows(j))

                    If Double.IsNaN(v) Then
                        v = 0.0
                    ElseIf v > 1.0 Then
                        v = 1.0
                    ElseIf v < -1.0 Then
                        v = -1.0
                    End If

                    buf(offset + j) = v
                Next
            End Sub)

        ' ② 严格对称化 + 对角线置 1（每个上三角配对只被一个 i 处理，无写冲突）
        Call Parallel.For(0, n,
            Sub(i)
                Dim offset As Integer = i * n

                For j As Integer = i + 1 To n - 1
                    Dim m As Double = 0.5 * (buf(offset + j) + buf(j * n + i))

                    buf(offset + j) = m
                    buf(j * n + i) = m
                Next

                buf(offset + i) = 1.0
            End Sub)

        Return TensorCorrelation.FromBuffer(buf, n, store.SampleN)
    End Function
End Module
