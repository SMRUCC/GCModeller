' 查询侧紧凑种子索引 (Compact Query Seed Table)
'
' DIAMOND 的双索引哈希连接有两端:查询侧(小)与参考侧(巨大)。
' 实测参考库可达 13.4 万条以上、数亿残基,若把参考侧也建成键值集合
' (Dictionary(Of Long, List(Of ...))),索引规模会达到十亿条量级,
' 内存与 GC 压力会彻底压垮整条流水线。
'
' 因此本实现把连接方向反转:
'   1) 只对查询集(数千条、数百万个种子)建立紧凑索引 —— 本类型;
'   2) 参考库改为"流式扫描":顺序推进参考库的每个种子位置,到查询索引中探测。
'
' 索引结构为链式哈希的四个扁平数组:
'   codes : 种子编码(用于确认桶内命中);
'   vals  : (查询下标 << 32) Or 查询内位置;
'   nexts : 同一桶内的下一条目;
'   heads : 桶头。
' 全部为值类型数组,构建期零对象分配;查询期 O(1) 探测且无迭代器状态机。

Imports System.Runtime.CompilerServices

Namespace DIAMOND

    ''' <summary>
    ''' 查询集种子索引(编码 -> 查询序列内的出现位置),链式哈希扁平实现。
    ''' </summary>
    Public NotInheritable Class QuerySeedTable

        ''' <summary>种子编码。</summary>
        Friend ReadOnly codes As Long()

        ''' <summary>(查询下标 &lt;&lt; 32) Or 查询内起始位置。</summary>
        Friend ReadOnly vals As Long()

        ''' <summary>同桶下一条目下标(-1 表示结束)。</summary>
        Friend ReadOnly nexts As Integer()

        ''' <summary>桶头下标(-1 表示空桶)。</summary>
        Friend ReadOnly heads As Integer()

        Private ReadOnly mask As Integer

        ''' <summary>索引条目总数。</summary>
        Public ReadOnly Property Count As Integer
            Get
                Return codes.Length
            End Get
        End Property

        ''' <summary>哈希桶数量。</summary>
        Public ReadOnly Property Buckets As Integer
            Get
                Return heads.Length
            End Get
        End Property

        Private Sub New(codes As Long(), vals As Long(), nexts As Integer(), heads As Integer(), mask As Integer)
            Me.codes = codes
            Me.vals = vals
            Me.nexts = nexts
            Me.heads = heads
            Me.mask = mask
        End Sub

        ''' <summary>
        ''' 为整个查询集构建某个种子形状的索引。
        ''' </summary>
        Public Shared Function Build(queries As SequenceDbView, plan As SeedPlan, reducedOfCode As Byte()) As QuerySeedTable
            Dim n As Integer = queries.Count
            Dim span As Integer = plan.Span
            Dim total As Long = 0

            For i As Integer = 0 To n - 1
                Dim len As Integer = queries.LengthOf(i)

                If len >= span Then
                    total += len - span + 1
                End If
            Next

            If total > Integer.MaxValue Then
                Throw New InvalidOperationException($"Query seed index is too large: {total} seeds.")
            End If

            Dim entries As Integer = CInt(total)
            Dim capacity As Integer = NextPowerOfTwo(Math.Max(1024, entries))
            Dim codes As Long() = New Long(Math.Max(0, entries - 1)) {}
            Dim vals As Long() = New Long(Math.Max(0, entries - 1)) {}
            Dim nexts As Integer() = New Integer(Math.Max(0, entries - 1)) {}
            Dim heads As Integer() = New Integer(capacity - 1) {}

            For i As Integer = 0 To capacity - 1
                heads(i) = -1
            Next

            Dim mask As Integer = capacity - 1
            Dim packed As Byte() = queries.Packed
            Dim offsets As Integer() = queries.Offsets
            Dim e As Integer = 0

            For i As Integer = 0 To n - 1
                Dim off As Integer = offsets(i)
                Dim last As Integer = queries.LengthOf(i) - span

                For p As Integer = 0 To last
                    Dim code As Long = plan.Encode(packed, reducedOfCode, off + p)
                    Dim b As Integer = BucketOf(code, mask)

                    codes(e) = code
                    vals(e) = (CLng(i) << 32) Or CLng(p)
                    nexts(e) = heads(b)
                    heads(b) = e
                    e += 1
                Next
            Next

            Return New QuerySeedTable(codes, vals, nexts, heads, mask)
        End Function

        ''' <summary>
        ''' 计算编码对应的桶下标(廉价 XOR 折叠混合,无乘法以避免溢出检查开销)。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Friend Shared Function BucketOf(code As Long, mask As Integer) As Integer
            Dim h As Long = code
            h = h Xor (h >> 21) Xor (h >> 42)
            Return CInt(h And mask)
        End Function

        ''' <summary>
        ''' 取编码对应的桶头。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Friend Function HeadOf(code As Long) As Integer
            Return heads(BucketOf(code, mask))
        End Function

        Friend Shared Function NextPowerOfTwo(value As Integer) As Integer
            Dim p As Integer = 1

            While p < value
                p <<= 1
            End While

            Return p
        End Function
    End Class
End Namespace
