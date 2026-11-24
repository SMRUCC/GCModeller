Imports System.Buffers

''' <summary>
''' 全仓库统一的临时缓冲租借入口。
'''
''' <para>
''' <see cref="ArrayPool(Of T).Shared"/> 的单个桶最大长度为 1 MiB：当 <c>Rent</c> 的请求
''' 长度超过该阈值时，BCL 会退化为直接 <c>new T[]</c> 分配，并且 <c>Return</c> 时数组会被
''' 直接丢弃，池化完全失效。因此本模块按照 1 MiB 边界把请求分流到两个池：
''' </para>
''' <list type="bullet">
''' <item><description>不超过 1 MiB：使用 <see cref="ArrayPool(Of T).Shared"/>。</description></item>
''' <item><description>超过 1 MiB：使用本模块自建的 <see cref="LargeBytes"/> 大块池（上限 64 MiB）。</description></item>
''' </list>
'''
''' <para>
''' 使用契约（与 <c>Protobuf/Wire/ProtoWriter.WriteString</c> 保持一致）：
''' <c>Rent</c> 得到的数组长度可能大于请求长度，所有业务逻辑必须使用逻辑长度，
''' 禁止用 <c>buffer.Length</c> 界定数据边界；归还必须放在 <c>Finally</c> 中，
''' 且同一个数组实例只允许归还一次。
''' </para>
''' </summary>
''' <remarks>
''' 本文件位于项目根命名空间 <c>Microsoft.VisualBasic.Data.IO</c> 之内，因此这里不显式声明
''' 命名空间：重复声明会在根命名空间下再嵌套一层同名空间，从而干扰同程序集内
''' <c>Microsoft.VisualBasic.*</c> 的标识符解析。
''' </remarks>
Public Module BufferPool

    ''' <summary>
    ''' <see cref="ArrayPool(Of T).Shared"/> 的单个桶最大长度（1 MiB）。
    ''' 超过该长度的租借请求会走 <see cref="LargeBytes"/>。
    ''' </summary>
    Public Const MaxSharedArrayLength As Integer = 1024 * 1024

    ''' <summary>
    ''' 大块池的单个桶最大长度（64 MiB），用于 page / 整列 / 整表级别的缓冲。
    ''' </summary>
    Public Const MaxLargeArrayLength As Integer = 64 * 1024 * 1024

    ''' <summary>
    ''' 大块池每个桶缓存的数组个数。大块数组常驻内存代价高，仅保留少量。
    ''' </summary>
    Private Const LargeArraysPerBucket As Integer = 4

    Private ReadOnly _largeBytes As ArrayPool(Of Byte) =
        ArrayPool(Of Byte).Create(MaxLargeArrayLength, LargeArraysPerBucket)

    ''' <summary>
    ''' 大块字节池，用于超过 <see cref="MaxSharedArrayLength"/> 的临时缓冲。
    ''' </summary>
    Public ReadOnly Property LargeBytes As ArrayPool(Of Byte)
        Get
            Return _largeBytes
        End Get
    End Property

    ''' <summary>
    ''' 判断指定长度的租借请求是否能够被 <see cref="ArrayPool(Of T).Shared"/> 命中。
    ''' </summary>
    <Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)>
    Public Function IsSharedPoolable(minimumLength As Integer) As Boolean
        Return minimumLength <= MaxSharedArrayLength
    End Function

    ''' <summary>
    ''' 租用一个长度不小于 <paramref name="minimumLength"/> 的字节数组。
    ''' 返回数组的长度可能大于请求长度，调用方必须使用逻辑长度。
    ''' </summary>
    <Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)>
    Public Function RentBytes(minimumLength As Integer) As Byte()
        If minimumLength <= MaxSharedArrayLength Then
            Return ArrayPool(Of Byte).Shared.Rent(minimumLength)
        Else
            Return _largeBytes.Rent(minimumLength)
        End If
    End Function

    ''' <summary>
    ''' 归还由 <see cref="RentBytes"/> 租用的字节数组。
    ''' </summary>
    ''' <param name="buffer">待归还的数组；<c>Nothing</c> 会被安全忽略。</param>
    ''' <param name="clear">
    ''' 归还前是否清零。<c>True</c> 仅用于可能承载用户敏感数据的缓冲
    ''' （文件字段文本、数据库记录、导出文本等）；压缩中间缓冲无需清零，
    ''' 避免无谓的 memset 开销。
    ''' </param>
    Public Sub ReturnBytes(buffer As Byte(), Optional clear As Boolean = False)
        If buffer Is Nothing Then
            Return
        End If

        If buffer.Length > MaxSharedArrayLength Then
            _largeBytes.Return(buffer, clear)
        Else
            ArrayPool(Of Byte).Shared.Return(buffer, clear)
        End If
    End Sub

    ''' <summary>
    ''' 租用一个长度不小于 <paramref name="minimumLength"/> 的字符数组。
    ''' 字符缓冲的长度远小于 1 MiB 阈值，直接使用共享池。
    ''' </summary>
    Public Function RentChars(minimumLength As Integer) As Char()
        Return ArrayPool(Of Char).Shared.Rent(minimumLength)
    End Function

    ''' <summary>
    ''' 归还由 <see cref="RentChars"/> 租用的字符数组。
    ''' </summary>
    Public Sub ReturnChars(buffer As Char(), Optional clear As Boolean = False)
        If buffer Is Nothing Then
            Return
        End If

        ArrayPool(Of Char).Shared.Return(buffer, clear)
    End Sub

    ''' <summary>
    ''' 租用一个长度不小于 <paramref name="minimumLength"/> 的 32 位整数数组。
    ''' </summary>
    ''' <remarks>
    ''' <see cref="ArrayPool(Of T).Shared"/> 不清零返还的数组，依赖初始值的逻辑
    ''' （例如 Snappy 哈希表填充 <c>-1</c>、MyersDiff 填充 <c>NEG_INF</c>）
    ''' 必须保留显式初始化循环。
    ''' </remarks>
    Public Function RentInt32s(minimumLength As Integer) As Integer()
        Return ArrayPool(Of Integer).Shared.Rent(minimumLength)
    End Function

    ''' <summary>
    ''' 归还由 <see cref="RentInt32s"/> 租用的整数数组。
    ''' </summary>
    Public Sub ReturnInt32s(buffer As Integer(), Optional clear As Boolean = False)
        If buffer Is Nothing Then
            Return
        End If

        ArrayPool(Of Integer).Shared.Return(buffer, clear)
    End Sub

    ''' <summary>
    ''' 租用一个长度不小于 <paramref name="minimumLength"/> 的长整数数组。
    ''' </summary>
    Public Function RentInt64s(minimumLength As Integer) As Long()
        Return ArrayPool(Of Long).Shared.Rent(minimumLength)
    End Function

    ''' <summary>
    ''' 归还由 <see cref="RentInt64s"/> 租用的长整数数组。
    ''' </summary>
    Public Sub ReturnInt64s(buffer As Long(), Optional clear As Boolean = False)
        If buffer Is Nothing Then
            Return
        End If

        ArrayPool(Of Long).Shared.Return(buffer, clear)
    End Sub

End Module
