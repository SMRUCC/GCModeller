' 序列集合的扁平化只读视图 (Flat Sequence Db View)
'
' 参考库达到「百万条序列 / 数亿残基」量级时,原有实现会在每条查询上重复执行
'     subjects.Select(Function(s) s.SequenceData).ToArray
' 并把整库序列反复传给每一个索引构建过程,产生巨量重复分配。
'
' 本类型把参考库(或查询集)一次性压平为:
'   - Seqs/Titles      : 原始字符串与标题(仅引用,不复制内容);
'   - Packed           : 全部残基的氨基酸编码扁平缓冲区 (Byte,单条序列连续存放);
'   - Offsets          : 每条序列在 Packed 中的起始下标 (Count + 1,末位为总长);
'   - Blocks           : 按残基预算切分的序列区间,作为并行调度的最小工作单元。
'
' 构建完成后整个对象只读,可安全地被所有并行线程共享。

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace DIAMOND

    ''' <summary>
    ''' 参考库序列区间(序列下标,半开区间)。
    ''' </summary>
    Public Structure SequenceRange
        Public ReadOnly Start As Integer
        Public ReadOnly [End] As Integer

        Sub New(start As Integer, [end] As Integer)
            Me.Start = start
            Me.[End] = [end]
        End Sub

        Public ReadOnly Property Count As Integer
            Get
                Return [End] - Start
            End Get
        End Property
    End Structure

    ''' <summary>
    ''' 扁平化的只读序列集合视图(参考库 / 查询集通用)。
    ''' </summary>
    Public Class SequenceDbView

        ''' <summary>序列标题(与 <see cref="Seqs"/> 一一对应)。</summary>
        Public ReadOnly Titles As String()

        ''' <summary>原始序列字符串(仅引用)。</summary>
        Public ReadOnly Seqs As String()

        ''' <summary>氨基酸编码扁平缓冲区(单条序列连续存放)。</summary>
        Public ReadOnly Packed As Byte()

        ''' <summary>每条序列在 <see cref="Packed"/> 中的起始下标,长度为 Count + 1。</summary>
        Public ReadOnly Offsets As Integer()

        ''' <summary>序列条数。</summary>
        Public ReadOnly Property Count As Integer
            Get
                Return Seqs.Length
            End Get
        End Property

        ''' <summary>总残基数。</summary>
        Public ReadOnly Property Residues As Integer
            Get
                Return Offsets(Seqs.Length)
            End Get
        End Property

        Sub New(seqs As String(), titles As String(), packed As Byte(), offsets As Integer())
            Me.Seqs = seqs
            Me.Titles = titles
            Me.Packed = packed
            Me.Offsets = offsets
        End Sub

        ''' <summary>
        ''' 一次性构建扁平视图(编码阶段按序列区间并行填充)。
        ''' </summary>
        Public Shared Function Build(subjects As IList(Of FastaSeq), scorer As AminoAcidScorer) As SequenceDbView
            Dim n As Integer = subjects.Count
            Dim seqs As String() = New String(n - 1) {}
            Dim titles As String() = New String(n - 1) {}
            Dim offsets As Integer() = New Integer(n) {}
            Dim total As Long = 0

            For i As Integer = 0 To n - 1
                Dim s As String = subjects(i).SequenceData
                seqs(i) = If(s, "")
                titles(i) = subjects(i).Title
                offsets(i) = CInt(total)
                total += seqs(i).Length
            Next

            If total > Integer.MaxValue Then
                Throw New InvalidOperationException($"Sequence collection is too large to flatten: {total} residues.")
            End If

            offsets(n) = CInt(total)

            Dim packed As Byte() = New Byte(total - 1) {}
            Dim codes As Byte() = scorer.Codes

            ' 按序列分片并行编码:各分片写入互不重叠的区间,无竞争
            Dim chunk As Integer = Math.Max(1, n \ Math.Max(1, Environment.ProcessorCount * 4))

            For start As Integer = 0 To n - 1 Step chunk
                Dim last As Integer = Math.Min(n, start + chunk) - 1

                Call System.Threading.Tasks.Parallel.For(start, last + 1,
                    Sub(i As Integer)
                        Dim seq As String = seqs(i)
                        Dim off As Integer = offsets(i)

                        For k As Integer = 0 To seq.Length - 1
                            Dim c As Integer = AscW(seq(k))
                            packed(off + k) = If(c < 0 OrElse c > 255, codes(0), codes(c))
                        Next
                    End Sub)
            Next

            Return New SequenceDbView(seqs, titles, packed, offsets)
        End Function

        ''' <summary>
        ''' 按残基预算把序列集合切分为若干区间(每区间至少包含 1 条序列)。
        ''' 这些区间即为并行扫描调度的工作单元。
        ''' </summary>
        Public Function Blocks(blockResidues As Integer) As List(Of SequenceRange)
            Dim ranges As New List(Of SequenceRange)
            Dim n As Integer = Count

            If n = 0 Then
                Return ranges
            End If

            If blockResidues < 1 Then
                blockResidues = 1
            End If

            Dim start As Integer = 0
            Dim acc As Long = 0

            For i As Integer = 0 To n - 1
                Dim len As Integer = Offsets(i + 1) - Offsets(i)

                If i > start AndAlso acc + len > blockResidues Then
                    Call ranges.Add(New SequenceRange(start, i))
                    start = i
                    acc = 0
                End If

                acc += len
            Next

            Call ranges.Add(New SequenceRange(start, n))
            Return ranges
        End Function

        ''' <summary>
        ''' 取指定序列的长度。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function LengthOf(i As Integer) As Integer
            Return Offsets(i + 1) - Offsets(i)
        End Function
    End Class
End Namespace
