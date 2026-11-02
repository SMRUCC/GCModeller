Imports System.Runtime.CompilerServices
Imports System.Text

Namespace Clustal

    ''' <summary>
    ''' 比对时使用的序列类型
    ''' </summary>
    Public Enum SequenceType
        ''' <summary>
        ''' 由输入的序列内容自动判定
        ''' </summary>
        Auto = 0
        ''' <summary>
        ''' 蛋白质序列(20 种标准氨基酸 + 通配残基 X)
        ''' </summary>
        Protein = 1
        ''' <summary>
        ''' 脱氧核糖核酸序列(ACGT + 通配碱基 N)
        ''' </summary>
        DNA = 2
    End Enum

    ''' <summary>
    ''' 序列字母表：负责大小写/非标准字符归一化、字符到编码的映射以及 k-mer 的默认长度。
    ''' </summary>
    ''' <remarks>
    ''' 编码约定与 <c>DIAMOND.ReducedAlphabet</c> 一致：使用 256 项的字节表把任意字符映射到
    ''' [0, Size) 的编码，未登记字符落到通配编码，从而让后续所有热路径都退化为一次数组访问，
    ''' 避免在内层循环里做字典查找或者 Char 比较。
    ''' </remarks>
    Public Class SequenceAlphabet

        ''' <summary>
        ''' 蛋白质字母表，末尾的 <c>X</c> 是通配残基
        ''' </summary>
        Public Const ProteinLetters As String = "ACDEFGHIKLMNPQRSTVWYX"
        ''' <summary>
        ''' 核酸字母表，末尾的 <c>N</c> 是通配碱基
        ''' </summary>
        Public Const DNALetters As String = "ACGTN"

        ''' <summary>
        ''' 序列类型
        ''' </summary>
        Public ReadOnly Property Kind As SequenceType
        ''' <summary>
        ''' 字母表字符串，索引即为编码
        ''' </summary>
        Public ReadOnly Property Letters As String
        ''' <summary>
        ''' 字母表的大小(含通配编码)
        ''' </summary>
        Public ReadOnly Property Size As Integer
        ''' <summary>
        ''' 通配编码
        ''' </summary>
        Public ReadOnly Property Wildcard As Byte
        ''' <summary>
        ''' 该序列类型下的默认 k-mer 长度
        ''' </summary>
        Public ReadOnly Property DefaultKmer As Integer

        Private ReadOnly codes As Byte()

        Private Sub New(kind As SequenceType, letters As String, wildcard As Integer, defaultKmer As Integer)
            Me.Kind = kind
            Me.Letters = letters
            Me.Size = letters.Length
            Me.Wildcard = CByte(wildcard)
            Me.DefaultKmer = defaultKmer
            Me.codes = New Byte(255) {}

            For i As Integer = 0 To 255
                codes(i) = Me.Wildcard
            Next

            For i As Integer = 0 To letters.Length - 1
                codes(AscW(letters(i))) = CByte(i)
                codes(AscW(Char.ToLowerInvariant(letters(i)))) = CByte(i)
            Next

            If kind = SequenceType.Protein Then
                ' IUPAC 建议的非标准残基归并规则：
                ' U(硒代半胱氨酸)->C, O(吡咯赖氨酸)->K, B(Asx)->N, Z(Glx)->Q, J(Xle)->L
                Call [Remap]("U"c, "C"c)
                Call [Remap]("O"c, "K"c)
                Call [Remap]("B"c, "N"c)
                Call [Remap]("Z"c, "Q"c)
                Call [Remap]("J"c, "L"c)
            Else
                Call [Remap]("U"c, "T"c)
            End If
        End Sub

        Private Sub [Remap](source As Char, target As Char)
            codes(AscW(Char.ToUpperInvariant(source))) = codes(AscW(Char.ToUpperInvariant(target)))
            codes(AscW(Char.ToLowerInvariant(source))) = codes(AscW(Char.ToUpperInvariant(target)))
        End Sub

        ''' <summary>
        ''' 把字符映射为编码(大小写不敏感，未知字符返回 <see cref="Wildcard"/>)
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function CodeOf(ch As Char) As Byte
            Dim i As Integer = AscW(ch)

            If i < 0 OrElse i > 255 Then
                Return Wildcard
            Else
                Return codes(i)
            End If
        End Function

        ''' <summary>
        ''' 把编码还原为字母表里面的字符
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function LetterOf(code As Byte) As Char
            If code >= Size Then
                Return Letters(Wildcard)
            Else
                Return Letters(CInt(code))
            End If
        End Function

        ''' <summary>
        ''' 归一化一条未经比对的原始序列：去掉空白与对齐符号，统一大写，并把非标准字符映射为通配字符。
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 注意：本方法会丢弃 <c>-</c> 与 <c>.</c>，因此只可以对**未比对**的输入序列调用，
        ''' 已经比对好的行必须原样保留空格列。
        ''' </remarks>
        Public Function Normalize(seq As String) As String
            If String.IsNullOrEmpty(seq) Then
                Return ""
            End If

            Dim sb As New StringBuilder(seq.Length)
            Dim wild As Char = Letters(Wildcard)

            For Each ch As Char In seq
                Select Case ch
                    Case "-"c, "."c, " "c, vbTab, vbCr, vbLf
                        Continue For
                    Case Else
                        Dim i As Integer = AscW(ch)

                        If i < 0 OrElse i > 255 Then
                            Call sb.Append(wild)
                        Else
                            Call sb.Append(If(codes(i) = Wildcard, wild, Char.ToUpperInvariant(ch)))
                        End If
                End Select
            Next

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' 蛋白质字母表
        ''' </summary>
        Public Shared ReadOnly Property Protein As SequenceAlphabet
            Get
                Return New SequenceAlphabet(SequenceType.Protein, ProteinLetters,
                                            wildcard:=ProteinLetters.Length - 1, defaultKmer:=2)
            End Get
        End Property

        ''' <summary>
        ''' 核酸字母表
        ''' </summary>
        Public Shared ReadOnly Property DNA As SequenceAlphabet
            Get
                Return New SequenceAlphabet(SequenceType.DNA, DNALetters,
                                            wildcard:=DNALetters.Length - 1, defaultKmer:=5)
            End Get
        End Property

        ''' <summary>
        ''' 依据序列内容判定序列类型：若碱基字符占全部有效字符的比例不低于 <paramref name="nucleotideThreshold"/>
        ''' 则认为是核酸序列，否则认为是蛋白质序列。
        ''' </summary>
        ''' <param name="seqs"></param>
        ''' <param name="nucleotideThreshold">判定为核酸的碱基占比阈值，默认 85%</param>
        Public Shared Function Detect(seqs As IEnumerable(Of String),
                                      Optional nucleotideThreshold As Double = 0.85) As SequenceAlphabet

            Dim nucleotide As Long = 0
            Dim total As Long = 0

            If seqs Is Nothing Then
                Return DNA
            End If

            For Each seq As String In seqs
                If seq Is Nothing Then
                    Continue For
                End If

                For Each ch As Char In seq
                    Select Case ch
                        Case "-"c, "."c, " "c, vbTab, vbCr, vbLf
                            Continue For
                        Case "A"c, "C"c, "G"c, "T"c, "U"c, "N"c,
                             "a"c, "c"c, "g"c, "t"c, "u"c, "n"c
                            nucleotide += 1
                            total += 1
                        Case Else
                            total += 1
                    End Select
                Next
            Next

            If total = 0 Then
                Return DNA
            ElseIf CDbl(nucleotide) / total >= nucleotideThreshold Then
                Return DNA
            Else
                Return Protein
            End If
        End Function

        Public Overrides Function ToString() As String
            Return $"{Kind}(A={Size}, k={DefaultKmer})"
        End Function
    End Class
End Namespace
