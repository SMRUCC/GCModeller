' ============================================================================
' Alphabet.vb — 序列字母表编码
' ----------------------------------------------------------------------------
' 将字符序列编码为 Int32 数组以加速 word 打包与打分表索引。
'
' 核酸：A=0, C=1, G=2, T=3（U 归并为 T），其余歧义字符 = 4
' 蛋白：20 标准氨基酸 0..19（ARNDCQEGHILKMFPSTWYV 顺序），
'       B=20, Z=21, X=22, '*'=23，未知字符归并为 X(22)
' ============================================================================

Namespace Core

    Public Module NtAlphabet

        Public Const Ambiguous As Int32 = 4

        ''' <summary>解码核酸码（0-3 → ACGT，其余 → N）</summary>
        Public Function Decode(code As Int32) As Char
            Select Case code
                Case 0 : Return "A"c
                Case 1 : Return "C"c
                Case 2 : Return "G"c
                Case 3 : Return "T"c
                Case Else : Return "N"c
            End Select
        End Function

        ''' <summary>单字符编码</summary>
        Public Function EncodeChar(ch As Char) As Int32
            Select Case ch
                Case "A"c : Return 0
                Case "C"c : Return 1
                Case "G"c : Return 2
                Case "T"c, "U"c : Return 3
                Case Else : Return Ambiguous
            End Select
        End Function

        ''' <summary>核酸编码（非 ACGTU 字符 → 4）</summary>
        Public Function Encode(seq As String) As Int32()
            Dim codes(seq.Length - 1) As Int32
            For i As Integer = 0 To seq.Length - 1
                Select Case seq(i)
                    Case "A"c : codes(i) = 0
                    Case "C"c : codes(i) = 1
                    Case "G"c : codes(i) = 2
                    Case "T"c, "U"c : codes(i) = 3
                    Case Else : codes(i) = Ambiguous
                End Select
            Next
            Return codes
        End Function

        ''' <summary>反向互补（自动搜索负链时备用；当前实现按用户给定方向搜索）</summary>
        Public Function ReverseComplement(seq As String) As String
            Dim buf(seq.Length - 1) As Char
            For i As Integer = 0 To seq.Length - 1
                Select Case seq(i)
                    Case "A"c : buf(seq.Length - 1 - i) = "T"c
                    Case "C"c : buf(seq.Length - 1 - i) = "G"c
                    Case "G"c : buf(seq.Length - 1 - i) = "C"c
                    Case "T"c, "U"c : buf(seq.Length - 1 - i) = "A"c
                    Case Else : buf(seq.Length - 1 - i) = "N"c
                End Select
            Next
            Return New String(buf)
        End Function

    End Module

End Namespace
