Namespace Evolution.Models

    ''' <summary>
    ''' 离散字符状态集合：定义进化树构建算法所使用的状态字母表，以及 gap/未知字符的判定规则。
    ''' 默认提供蛋白质（20 种标准氨基酸）与核酸（DNA，4 种碱基）两种字符集。
    ''' </summary>
    ''' <remarks>
    ''' 蛋白质字母表的排列顺序采用与 Dayhoff/JTT/WAG/LG 等经验替换模型一致的经典顺序：
    ''' ``A R N D C Q E G H I L K M F P S T W Y V``，从而保证字符状态索引与替换矩阵索引一一对应。
    ''' </remarks>
    Public Class CharacterSet

        ''' <summary>
        ''' 表示缺失状态（gap / 未知残基）的编码值。
        ''' </summary>
        Public Const Missing As Integer = -1

        Public ReadOnly Property Name As String
        Public ReadOnly Property Alphabet As Char()
        Public ReadOnly Property GapCharacters As Char()

        Private ReadOnly _index As Dictionary(Of Char, Integer)

        Private Sub New(name As String, alphabet As Char(), gap As Char())
            Me.Name = name
            Me.Alphabet = alphabet
            Me.GapCharacters = gap
            Me._index = New Dictionary(Of Char, Integer)

            For i As Integer = 0 To alphabet.Length - 1
                Me._index(alphabet(i)) = i
            Next
        End Sub

        ''' <summary>
        ''' 状态空间的大小（氨基酸为 20，核酸为 4）
        ''' </summary>
        Public ReadOnly Property Size As Integer
            Get
                Return Alphabet.Length
            End Get
        End Property

        ''' <summary>
        ''' 返回字符对应的状态编码，缺失/未知时返回 <see cref="Missing"/>。
        ''' </summary>
        Public Function IndexOf(c As Char) As Integer
            Dim i As Integer

            If _index.TryGetValue(Char.ToUpper(c), i) Then
                Return i
            Else
                Return Missing
            End If
        End Function

        ''' <summary>
        ''' 判断字符是否为 gap / 未知字符。
        ''' </summary>
        Public Function IsGap(c As Char) As Boolean
            Return Array.IndexOf(GapCharacters, Char.ToUpper(c)) >= 0
        End Function

        Private Shared ReadOnly _protein As CharacterSet = New CharacterSet(
            "Protein",
            New Char() {"A"c, "R"c, "N"c, "D"c, "C"c, "Q"c, "E"c, "G"c, "H"c, "I"c, "L"c, "K"c, "M"c, "F"c, "P"c, "S"c, "T"c, "W"c, "Y"c, "V"c},
            New Char() {"-"c, "."c, "*"c, "?"c, "X"c, "B"c, "Z"c, "J"c, "U"c, "O"c})

        Private Shared ReadOnly _dna As CharacterSet = New CharacterSet(
            "DNA",
            New Char() {"A"c, "C"c, "G"c, "T"c},
            New Char() {"-"c, "."c, "N"c, "?"c})

        ''' <summary>
        ''' 蛋白质字符集（20 态）
        ''' </summary>
        Public Shared ReadOnly Property Protein As CharacterSet
            Get
                Return _protein
            End Get
        End Property

        ''' <summary>
        ''' DNA 字符集（4 态）
        ''' </summary>
        Public Shared ReadOnly Property DNA As CharacterSet
            Get
                Return _dna
            End Get
        End Property

        ''' <summary>
        ''' 依据序列内容自动猜测字符集：若序列仅包含 DNA 字母表字符则返回 <see cref="DNA"/>，否则返回 <see cref="Protein"/>。
        ''' </summary>
        Public Shared Function Guess(sequence As String) As CharacterSet
            Dim dnaChars As Char() = {"A"c, "C"c, "G"c, "T"c, "U"c, "N"c, "-"c, "."c, "?"c}

            For Each c As Char In sequence
                If Array.IndexOf(dnaChars, Char.ToUpper(c)) < 0 Then
                    Return Protein
                End If
            Next

            Return DNA
        End Function
    End Class

End Namespace
