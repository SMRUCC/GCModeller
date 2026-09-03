' ============================================================================
' Alphabet.vb — 字母表定义、序列编码、歧义字符、反向互补
' ----------------------------------------------------------------------------
' [em.md §1] 核酸 {A,C,G,T}（U→T）与氨基酸 20 标准（ACDEFGHIKLMNPQRSTVWY）。
' 歧义字符（核酸 N/R/Y/...；蛋白 B/Z/X/J/O/U）编码为 -1：包含它们的候选窗口
' 不参与 E 步（Z=0），与 MEME 行为一致。
' 反义链 [em.md §9 -revcomp]：仅核酸支持；负链窗口第 k 列 = 原串第 (j+W-1-k)
' 位碱基的互补。
' ============================================================================

Imports SMRUCC.genomics.SequenceModel

Namespace EmMotif.Core

    Public Enum SiteModel
        Oops = 0     ' [em.md §6] 每条序列恰好 1 个实例：Σ_j Z_ij = 1
        Zoops = 1    ' 每条序列最多 1 个：Σ_j Z_ij ≤ 1（无 motif 状态）
        Anr = 2      ' 任意多个（窗口独立，Bailey & Elkan 1994 窗口形式）
    End Enum

    Public Class Alphabet

        Public ReadOnly Kind As SeqTypes
        Public ReadOnly Letters As String
        Public ReadOnly Size As Int32
        Private ReadOnly _encode As Dictionary(Of Char, Int32)
        Private ReadOnly _compMap As Int32()      ' 反向互补映射（仅核酸）
        Public ReadOnly SupportsRevcomp As Boolean

        Public Sub New(kind As SeqTypes)
            ' RNA 与 DNA 共用核酸字母表（U 并入 T）；无法识别的类型直接失败，
            ' 而不是被静默当成蛋白质处理 [缺陷 #12]
            Select Case kind
                Case SeqTypes.DNA, SeqTypes.RNA
                    Letters = "ACGT"
                    SupportsRevcomp = True
                    Me.Kind = SeqTypes.DNA
                Case SeqTypes.Protein
                    Letters = "ACDEFGHIKLMNPQRSTVWY"
                    SupportsRevcomp = False
                    Me.Kind = SeqTypes.Protein
                Case Else
                    Throw New ArgumentException(
                        $"无法识别的序列类型：{kind}（仅支持 DNA / RNA / Protein）", NameOf(kind))
            End Select

            Size = Letters.Length
            _encode = New Dictionary(Of Char, Int32)()
            For i = 0 To Size - 1
                _encode(Letters(i)) = i
            Next
            ' 尿嘧啶并入 T：直接复用 T 的索引，避免手写下标的错误 [缺陷 #5]
            If SupportsRevcomp Then _encode("U"c) = _encode("T"c)
            _compMap = New Int32(Size - 1) {}
            If kind = SeqTypes.DNA Then
                ' A<->T, C<->G
                _compMap(0) = 3 : _compMap(1) = 2 : _compMap(2) = 1 : _compMap(3) = 0
            End If
        End Sub

        Public Function EncodeChar(c As Char) As Int32
            Dim v As Int32 = -1
            If _encode.TryGetValue(Char.ToUpperInvariant(c), v) Then Return v
            Return -1
        End Function

        ''' <summary>序列编码；歧义字符 → −1；空串 → 空数组 [缺陷 #13]</summary>
        Public Function Encode(seq As String) As Int32()
            If String.IsNullOrEmpty(seq) Then Return New Int32() {}
            Dim outArr(seq.Length - 1) As Int32
            For i = 0 To seq.Length - 1
                outArr(i) = EncodeChar(seq(i))
            Next
            Return outArr
        End Function

        ''' <summary>索引 → 字母；歧义（−1）返回 N（核酸）/ X（蛋白）[缺陷 #13]</summary>
        Public Function Decode(a As Int32) As String
            If a < 0 OrElse a >= Size Then
                Return If(SupportsRevcomp, "N", "X")
            End If
            Return Letters(a).ToString()
        End Function

        ''' <summary>
        ''' 互补碱基索引（仅核酸有效）。歧义/越界索引返回 −1，
        ''' 使调用方可以用「a &lt; 0」统一判断，而不会越界崩溃 [缺陷 #6]。
        ''' </summary>
        Public Function Complement(a As Int32) As Int32
            If a < 0 OrElse a >= Size Then Return -1
            Return _compMap(a)
        End Function

        ''' <summary>反向互补（字符串形式，用于输出）</summary>
        Public Function Revcomp(seq As String) As String
            If Kind <> SeqTypes.DNA Then Return seq
            Dim ch = seq.ToCharArray()
            Array.Reverse(ch)
            For i = 0 To ch.Length - 1
                Dim v = EncodeChar(ch(i))
                If v >= 0 Then
                    ch(i) = Letters(_compMap(v))
                End If
            Next
            Return New String(ch)
        End Function
    End Class

End Namespace
