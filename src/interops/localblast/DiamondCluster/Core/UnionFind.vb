' ============================================================================
' UnionFind.vb — 内存映射并查集（Disjoint Set Union）
' ----------------------------------------------------------------------------
' 使用 MemoryMappedFile 实现的并查集数据结构，支持十亿级元素。
'
' 核心设计：
'   1. 父指针数组存储在磁盘文件中，通过内存映射访问
'   2. 每个元素占 4 字节（int32），2B 元素 = 8GB 磁盘
'   3. 初始化用 -1 (0xFFFFFFFF) 作为"未初始化"标记
'   4. Find 操作：路径压缩（path compression）
'   5. Union 操作：直接挂接（无 union-by-rank，省去第二个数组）
'
' 内存分析（16GB 物理内存）：
'   - DSU 文件：N × 4 字节，OS 按需分页调入
'   - 活跃页：通常 < 2GB（路径压缩使树很浅，根节点集中在少数页）
'   - DIAMOND 进程：~2-4GB（block-size=0.5）
'   - 程序本身：~500MB
'   - 合计 < 7GB，16GB 充裕
'
' 复杂度分析：
'   - Find：均摊 O(α(N)) ≈ O(1)（路径压缩）
'   - Union：O(α(N)) ≈ O(1)
'   - 初始化：O(N/64K) 次大块写入（1MB 缓冲区）
' ============================================================================

Imports System.IO
Imports System.IO.MemoryMappedFiles

Namespace Core

    ''' <summary>
    ''' 内存映射文件支持的并查集。
    ''' 支持 int.MaxValue（~21 亿）个元素，内存占用由 OS 分页管理。
    ''' </summary>
    Public Class UnionFind
        Implements IDisposable

        Private ReadOnly _filePath As String
        Private ReadOnly _capacity As Integer
        Private _mmf As MemoryMappedFile
        Private _accessor As MemoryMappedViewAccessor
        Private _unionCount As Long = 0
        Private _findCount As Long = 0
        Private _disposed As Boolean = False

        ' ---- 统计 ----
        Private _cacheHits As Long = 0
        Private _cacheMisses As Long = 0

        ''' <summary>
        ''' 小型 LRU 缓存：最近访问的元素→根的映射
        ''' 用于加速重复 Find（同一序列在多条比对结果中出现时）
        ''' </summary>
        Private ReadOnly _cacheSize As Integer = 65536
        Private _cacheKeys As Integer()
        Private _cacheVals As Integer()
        Private _cacheValid As Boolean()

        ''' <summary>并集操作总数</summary>
        Public ReadOnly Property UnionCount As Long
            Get
                Return _unionCount
            End Get
        End Property

        ''' <summary>Find 操作总数</summary>
        Public ReadOnly Property FindCount As Long
            Get
                Return _findCount
            End Get
        End Property

        ''' <summary>容量（元素总数）</summary>
        Public ReadOnly Property Capacity As Integer
            Get
                Return _capacity
            End Get
        End Property

        ''' <summary>
        ''' 构造函数：创建并初始化并查集文件
        ''' </summary>
        ''' <param name="filePath">DSU 文件路径（将创建此文件）</param>
        ''' <param name="capacity">元素总数（序列数）</param>
        Public Sub New(filePath As String, capacity As Integer)
            If capacity <= 0 Then
                Throw New ArgumentException("容量必须 > 0", NameOf(capacity))
            End If
            If capacity > Integer.MaxValue Then
                Throw New ArgumentException(
                    $"序列数 {capacity} 超过 int.MaxValue ({Integer.MaxValue})，" &
                    "请将输入文件拆分后分别处理。")
            End If

            _filePath = filePath
            _capacity = capacity

            ' 初始化缓存
            _cacheKeys = New Integer(_cacheSize - 1) {}
            _cacheVals = New Integer(_cacheSize - 1) {}
            _cacheValid = New Boolean(_cacheSize - 1) {}

            ' 创建并初始化文件
            InitializeFile()

            ' 打开内存映射
            OpenMappedFile()
        End Sub

        ''' <summary>
        ''' 初始化 DSU 文件：全部填充 0xFFFFFFFF（-1，表示未初始化）
        ''' </summary>
        Private Sub InitializeFile()
            Dim fileSize As Long = CLng(_capacity) * 4L

            Using fs As New FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize:=1 << 20)
                fs.SetLength(fileSize)

                ' 用 1MB 缓冲区填充 0xFF
                Dim bufferSize As Integer = 1 << 20 ' 1 MB
                Dim buffer(bufferSize - 1) As Byte
                For i = 0 To buffer.Length - 1
                    buffer(i) = &HFF
                Next

                Dim remaining As Long = fileSize
                Dim written As Long = 0
                Do While remaining > 0
                    Dim toWrite As Integer = CInt(Math.Min(bufferSize, remaining))
                    fs.Write(buffer, 0, toWrite)
                    remaining -= toWrite
                    written += toWrite

                    ' 进度报告（每 1GB 报告一次）
                    If written Mod (1L << 30) = 0 AndAlso written > 0 Then
                        Console.Error.Write($"    DSU 初始化: {written \ (1 << 20)} MB / {fileSize \ (1 << 20)} MB" & ControlChars.Cr)
                    End If
                Loop
            End Using

            Console.Error.WriteLine($"    DSU 初始化完成: {_capacity:N0} 元素, {fileSize \ (1 << 20)} MB")
        End Sub

        ''' <summary>
        ''' 打开内存映射文件
        ''' </summary>
        Private Sub OpenMappedFile()
            Dim fileSize As Long = CLng(_capacity) * 4L
            _mmf = MemoryMappedFile.CreateFromFile(
                _filePath, FileMode.Open, Nothing, fileSize,
                MemoryMappedFileAccess.ReadWrite)
            _accessor = _mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.ReadWrite)
        End Sub

        ''' <summary>
        ''' 查找元素 x 的根节点（带路径压缩）
        ''' </summary>
        ''' <param name="x">元素 ID</param>
        ''' <returns>根节点 ID</returns>
        Public Function Find(x As Integer) As Integer
            _findCount += 1

            ' ---- 检查缓存 ----
            Dim cacheIdx = x And (_cacheSize - 1) ' x mod cacheSize（位运算，要求 cacheSize 是 2 的幂）
            If _cacheValid(cacheIdx) AndAlso _cacheKeys(cacheIdx) = x Then
                _cacheHits += 1
                Return _cacheVals(cacheIdx)
            End If
            _cacheMisses += 1

            ' ---- 读取父指针 ----
            Dim offset As Long = CLng(x) * 4L
            Dim p = _accessor.ReadInt32(offset)

            ' 未初始化 → 自身为根
            If p = -1 Then
                _accessor.Write(offset, x)
                CacheSet(x, x)
                Return x
            End If

            ' ---- 找根 ----
            Dim root = x
            Do While p <> root
                root = p
                p = _accessor.ReadInt32(CLng(root) * 4L)
                If p = -1 Then
                    ' root 未初始化 → 自身为根
                    _accessor.Write(CLng(root) * 4L, root)
                    Exit Do
                End If
            Loop

            ' ---- 路径压缩：将路径上的所有节点直接指向根 ----
            Dim curr = x
            Do While curr <> root
                Dim nextOffset = CLng(curr) * 4L
                Dim nextP = _accessor.ReadInt32(nextOffset)
                _accessor.Write(nextOffset, root)
                curr = nextP
            Loop

            CacheSet(x, root)
            Return root
        End Function

        ''' <summary>
        ''' 合并 x 和 y 所在的集合
        ''' </summary>
        Public Sub Union(x As Integer, y As Integer)
            Dim rx = Find(x)
            Dim ry = Find(y)
            If rx = ry Then Return

            ' 直接挂接（无 union-by-rank）
            ' 注意：为了减少深度，可选择将较大的根挂到较小的根
            ' 此处简单挂接，路径压缩会处理后续深度
            _accessor.Write(CLng(ry) * 4L, rx)
            _unionCount += 1

            ' 更新缓存
            CacheSet(ry, rx)
        End Sub

        ''' <summary>设置缓存</summary>
        Private Sub CacheSet(key As Integer, val As Integer)
            Dim idx = key And (_cacheSize - 1)
            _cacheKeys(idx) = key
            _cacheVals(idx) = val
            _cacheValid(idx) = True
        End Sub

        ''' <summary>缓存命中率</summary>
        Public ReadOnly Property CacheHitRate As Double
            Get
                Dim total = _cacheHits + _cacheMisses
                If total = 0 Then Return 0.0
                Return CDbl(_cacheHits) / total
            End Get
        End Property

        ''' <summary>释放资源</summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            If _disposed Then Return
            _accessor?.Dispose()
            _mmf?.Dispose()
            _disposed = True
        End Sub

    End Class

End Namespace
