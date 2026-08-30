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

    Public Module AaAlphabet

        ''' <summary>标准 20 氨基酸顺序（与打分矩阵行列对应）</summary>
        Public Const Std20 As String = "ARNDCQEGHILKMFPSTWYV"

        Public Const CodeB As Int32 = 20
        Public Const CodeZ As Int32 = 21
        Public Const CodeX As Int32 = 22
        Public Const CodeStop As Int32 = 23

        ''' <summary>蛋白编码（未知字符 → X）</summary>
        Public Function Encode(seq As String) As Int32()
            Dim codes(seq.Length - 1) As Int32
            For i As Integer = 0 To seq.Length - 1
                Dim idx As Int32 = CodeX
                Dim c As Char = seq(i)
                Select Case c
                    Case "A"c : idx = 0
                    Case "R"c : idx = 1
                    Case "N"c : idx = 2
                    Case "D"c : idx = 3
                    Case "C"c : idx = 4
                    Case "Q"c : idx = 5
                    Case "E"c : idx = 6
                    Case "G"c : idx = 7
                    Case "H"c : idx = 8
                    Case "I"c : idx = 9
                    Case "L"c : idx = 10
                    Case "K"c : idx = 11
                    Case "M"c : idx = 12
                    Case "F"c : idx = 13
                    Case "P"c : idx = 14
                    Case "S"c : idx = 15
                    Case "T"c : idx = 16
                    Case "W"c : idx = 17
                    Case "Y"c : idx = 18
                    Case "V"c : idx = 19
                    Case "B"c : idx = CodeB
                    Case "Z"c : idx = CodeZ
                    Case "X"c : idx = CodeX
                    Case "*"c : idx = CodeStop
                    Case Else : idx = CodeX
                End Select
                codes(i) = idx
            Next
            Return codes
        End Function

        ''' <summary>单字符编码（未知 → X）</summary>
        Public Function EncodeChar(ch As Char) As Int32
            Select Case ch
                Case "A"c : Return 0
                Case "R"c : Return 1
                Case "N"c : Return 2
                Case "D"c : Return 3
                Case "C"c : Return 4
                Case "Q"c : Return 5
                Case "E"c : Return 6
                Case "G"c : Return 7
                Case "H"c : Return 8
                Case "I"c : Return 9
                Case "L"c : Return 10
                Case "K"c : Return 11
                Case "M"c : Return 12
                Case "F"c : Return 13
                Case "P"c : Return 14
                Case "S"c : Return 15
                Case "T"c : Return 16
                Case "W"c : Return 17
                Case "Y"c : Return 18
                Case "V"c : Return 19
                Case "B"c : Return CodeB
                Case "Z"c : Return CodeZ
                Case "*"c : Return CodeStop
                Case Else : Return CodeX
            End Select
        End Function

        ''' <summary>解码单个氨基酸码</summary>
        Public Function Decode(code As Int32) As Char
            If code >= 0 AndAlso code <= 19 Then Return Std20(code)
            Select Case code
                Case CodeB : Return "B"c
                Case CodeZ : Return "Z"c
                Case CodeStop : Return "*"c
                Case Else : Return "X"c
            End Select
        End Function

    End Module

End Namespace
