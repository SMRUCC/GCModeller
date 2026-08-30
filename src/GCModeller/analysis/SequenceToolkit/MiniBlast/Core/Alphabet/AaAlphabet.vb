Namespace Core

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