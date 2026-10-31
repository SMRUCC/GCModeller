' 氨基酸编码与 BLOSUM62 快速打分表 (Amino Acid Codes & Fast Score Table)
'
' 原始实现中,无空位延伸与带状 SW 的每一个残基比对都要走:
'     Blosum.GetDistance(char, char)
'       -> Index(Of String).IndexOf(Char.ToUpper(a))   '(字符串集合线性查找)
'       -> matrix(i)(j)                                '(锯齿数组两级间接)
' 在 5.9 亿残基级别的比对任务中,这条路径是最大的性能瓶颈之一。
'
' 本模块把字符替换矩阵预编译成扁平查表:
'   1) Codes(256)         : 氨基酸字符 -> 紧凑编码 (0..N-1,即 blosum.keys 的下标);
'                            非标准残基 (U/O/J/其它) 统一落到通配编码 X;
'   2) Scores(32 * 32)    : 编码对 -> BLOSUM62 得分 (扁平一维,单次内存访问);
'   3) ReducedOfCode(32)  : 编码 -> 缩减字母表类别 (供 Hamming 初筛与种子编码复用)。
'
' 所有表在构造时一次性构建并且之后只读,可安全地在所有并行线程之间共享。

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment

Namespace DIAMOND

    ''' <summary>
    ''' 氨基酸编码表 + BLOSUM62 扁平快速打分表(只读,线程安全)。
    ''' </summary>
    Public NotInheritable Class AminoAcidScorer

        ''' <summary>编码表的跨度(2 的幂,便于用位移计算二维下标)。</summary>
        Public Const TableStride As Integer = 32

        Private Shared _default As AminoAcidScorer
        Private Shared ReadOnly sync As New Object

        ''' <summary>
        ''' 默认打分表(由内嵌 BLOSUM62 资源构建),进程内只构建一次。
        ''' </summary>
        Public Shared ReadOnly Property DefaultScorer As AminoAcidScorer
            Get
                If _default Is Nothing Then
                    SyncLock sync
                        If _default Is Nothing Then
                            _default = New AminoAcidScorer(Blosum.FromInnerBlosum62)
                        End If
                    End SyncLock
                End If

                Return _default
            End Get
        End Property

        ''' <summary>字符 -> 氨基酸编码 (长度 256)。</summary>
        Public ReadOnly Codes As Byte()

        ''' <summary>氨基酸编码 -> 缩减字母表类别 (长度 32)。</summary>
        Public ReadOnly ReducedOfCode As Byte()

        ''' <summary>编码对 -> BLOSUM62 得分 (长度 32 * 32,扁平)。</summary>
        Public ReadOnly Scores As Integer()

        ''' <summary>
        ''' 编码对是否属于同一缩减字母类别 (长度 32 * 32,1 = 同类)。
        ''' Hamming 初筛用它把"两次缩减映射 + 一次比较"压缩为一次查表。
        ''' </summary>
        Public ReadOnly SameReduced As Byte()

        ''' <summary>通配(未知残基)编码。</summary>
        Public ReadOnly Wildcard As Byte

        Sub New(blosum As Blosum)
            Dim keys As String() = blosum.keys
            Dim n As Integer = Math.Min(keys.Length, TableStride)

            Me.Codes = New Byte(255) {}
            Me.ReducedOfCode = New Byte(TableStride - 1) {}
            Me.Scores = New Integer(TableStride * TableStride - 1) {}
            Me.SameReduced = New Byte(TableStride * TableStride - 1) {}

            ' 通配编码:优先取 'X',没有则退化为 0
            Dim wild As Integer = 0

            For i As Integer = 0 To n - 1
                If keys(i).Length = 1 AndAlso Char.ToUpper(keys(i)(0)) = "X"c Then
                    wild = i
                    Exit For
                End If
            Next

            Me.Wildcard = CByte(wild)

            ' 字符 -> 编码(大小写不敏感;未登记字符落入通配编码)
            For c As Integer = 0 To 255
                Dim ch As Char = Char.ToUpper(ChrW(c))
                Dim code As Integer = -1

                For i As Integer = 0 To n - 1
                    If keys(i).Length = 1 AndAlso keys(i)(0) = ch Then
                        code = i
                        Exit For
                    End If
                Next

                Me.Codes(c) = If(code < 0, CByte(wild), CByte(code))
            Next

            ' 编码 -> 缩减字母表类别
            For i As Integer = 0 To TableStride - 1
                If i < n AndAlso keys(i).Length = 1 Then
                    Me.ReducedOfCode(i) = CByte(ReducedAlphabet.Map(keys(i)(0)))
                Else
                    Me.ReducedOfCode(i) = CByte(ReducedAlphabet.WildcardClass)
                End If
            Next

            ' 编码对 -> 得分(扁平化,避免两级数组间接寻址)
            Dim rows As Integer = blosum.matrix.Length

            For a As Integer = 0 To n - 1
                Dim row As Integer() = blosum.matrix(Math.Min(a, rows - 1))

                For b As Integer = 0 To n - 1
                    Me.Scores((a << 5) Or b) = row(Math.Min(b, row.Length - 1))
                Next
            Next

            ' 编码对 -> 是否同属一个缩减字母类别
            For a As Integer = 0 To TableStride - 1
                For b As Integer = 0 To TableStride - 1
                    Me.SameReduced((a << 5) Or b) =
                        If(Me.ReducedOfCode(a) = Me.ReducedOfCode(b), CByte(1), CByte(0))
                Next
            Next
        End Sub

        ''' <summary>
        ''' 两个氨基酸编码是否属于同一缩减字母类别。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function IsSameReducedClass(a As Byte, b As Byte) As Boolean
            Return SameReduced((CInt(a) << 5) Or CInt(b)) <> 0
        End Function

        ''' <summary>
        ''' 取两个氨基酸编码之间的替换得分(单次数组访问)。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Score(a As Byte, b As Byte) As Integer
            Return Scores((CInt(a) << 5) Or CInt(b))
        End Function

        ''' <summary>
        ''' 字符形式的替换得分(供字符串路径的兼容实现使用)。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Score(a As Char, b As Char) As Integer
            Dim ia = AscW(a)
            Dim ib = AscW(b)

            If ia < 0 OrElse ia > 255 OrElse ib < 0 OrElse ib > 255 Then
                Return Score(Wildcard, Wildcard)
            End If

            Return Score(Codes(ia), Codes(ib))
        End Function
    End Class
End Namespace
