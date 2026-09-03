' ============================================================================
' Alphabet.vb — 字母表定义、序列编码、歧义字符、反向互补
' ----------------------------------------------------------------------------
' [em.md §1] 核酸 {A,C,G,T}（U→T）与氨基酸 20 标准（ACDEFGHIKLMNPQRSTVWY）。
' 歧义字符（核酸 N/R/Y/...；蛋白 B/Z/X/J/O/U）编码为 -1：包含它们的候选窗口
' 不参与 E 步（Z=0），与 MEME 行为一致。
' 反义链 [em.md §9 -revcomp]：仅核酸支持；负链窗口第 k 列 = 原串第 (j+W-1-k)
' 位碱基的互补。
' ============================================================================

Imports System
Imports System.Collections.Generic

Namespace EmMotif.Core

    Public Enum AlphabetKind
        Dna = 0
        Protein = 1
    End Enum

    Public Enum SiteModel
        Oops = 0     ' [em.md §6] 每条序列恰好 1 个实例：Σ_j Z_ij = 1
        Zoops = 1    ' 每条序列最多 1 个：Σ_j Z_ij ≤ 1（无 motif 状态）
        Anr = 2      ' 任意多个（窗口独立，Bailey & Elkan 1994 窗口形式）
    End Enum

    Public Class Alphabet

        Public ReadOnly Kind As AlphabetKind
        Public ReadOnly Letters As String
        Public ReadOnly Size As Int32
        Private ReadOnly _encode As Dictionary(Of Char, Int32)
        Private ReadOnly _compMap As Int32()      ' 反向互补映射（仅核酸）
        Public ReadOnly SupportsRevcomp As Boolean

        Public Sub New(kind As AlphabetKind)
            Me.Kind = kind
            If kind = AlphabetKind.Dna Then
                Letters = "ACGT"
                SupportsRevcomp = True
            Else
                Letters = "ACDEFGHIKLMNPQRSTVWY"
                SupportsRevcomp = False
            End If
            Size = Letters.Length
            _encode = New Dictionary(Of Char, Int32)()
            For i = 0 To Size - 1
                _encode(Letters(i)) = i
            Next
            ' 尿嘧啶并入 T
            If kind = AlphabetKind.Dna Then _encode("U"c) = 1   ' T 的索引是 1
            _compMap = New Int32(Size - 1) {}
            If kind = AlphabetKind.Dna Then
                ' A<->T, C<->G
                _compMap(0) = 3 : _compMap(1) = 2 : _compMap(2) = 1 : _compMap(3) = 0
            End If
        End Sub

        Public Function EncodeChar(c As Char) As Int32
            Dim v As Int32 = -1
            If _encode.TryGetValue(Char.ToUpperInvariant(c), v) Then Return v
            Return -1
        End Function

        ''' <summary>序列编码；歧义字符 → −1</summary>
        Public Function Encode(seq As String) As Int32()
            Dim outArr(seq.Length - 1) As Int32
            For i = 0 To seq.Length - 1
                outArr(i) = EncodeChar(seq(i))
            Next
            Return outArr
        End Function

        Public Function Decode(a As Int32) As String
            Return Letters(a).ToString()
        End Function

        ''' <summary>互补碱基索引（仅核酸有效）</summary>
        Public Function Complement(a As Int32) As Int32
            Return _compMap(a)
        End Function

        ''' <summary>反向互补（字符串形式，用于输出）</summary>
        Public Function Revcomp(seq As String) As String
            If Kind <> AlphabetKind.Dna Then Return seq
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

        ''' <summary>自动判定字母表：出现 ACGT 之外的有效字符（非歧义）→ 蛋白</summary>
        Public Shared Function Detect(seq As String) As AlphabetKind
            Dim dnaAlpha As New Alphabet(AlphabetKind.Dna)
            Dim aaOnly As Int32 = 0
            For Each c In seq.ToUpperInvariant()
                Dim v = dnaAlpha.EncodeChar(c)
                If v < 0 Then
                    ' 不属于 DNA 表（含 U 归并后）——若属蛋白 20 字母则为蛋白序列
                    If "ACDEFGHIKLMNPQRSTVWY".IndexOf(c) >= 0 Then aaOnly += 1
                End If
            Next
            Return If(aaOnly > 0, AlphabetKind.Protein, AlphabetKind.Dna)
        End Function

    End Class

End Namespace
