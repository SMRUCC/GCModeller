Namespace SmilesSyntax

    ''' <summary>
    ''' SMILES 分词器：把结构式字符串拆成词表 token（字符级）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 该实现逐行等价于 smiles-transformer 的 <c>utils.split</c>，而不是"按分子图的原子/键切分"。
    ''' 两者不可互换：例如 <c>[N+]</c> 在词表里是 <c>[ N + ]</c> 四个 token，
    ''' 方括号内的手性记号 <c>[C@@H]</c> 也会被拆成 <c>[ C @ @ H ]</c>。
    ''' </para>
    ''' <para>
    ''' 只有下列二字母组合会被合并成一个 token：
    ''' <c>Cl Ca Cu Br Be Ba Bi Si Se Sr Na Ni Rb Ra Xe Li Al As Ag Au Mg Mn Te Zn</c>、
    ''' 它们的芳香小写形式 <c>si se te</c>、惰性气体/金属 <c>He Kr Fe</c>，
    ''' 以及电荷后缀 <c>+2 +3 +4 -2 -3 -4</c>；<c>%nn</c> 形式的环闭合号整体作为一个 token。
    ''' 其余字符（含 <c>( ) [ ] = # @ / \ . 数字</c>）一律逐字符拆分。
    ''' </para>
    ''' </remarks>
    ''' <example>
    ''' <code>
    ''' Dim tokens$() = SmilesTokenizer.Split("C(=O)CC(Br)C[N+]CN")
    ''' ' { "C", "(", "=", "O", ")", "C", "C", "(", "Br", ")", "C", "[", "N", "+", "]", "C", "N" }
    ''' </code>
    ''' </example>
    Public Module SmilesTokenizer

        ''' <summary>
        ''' 可能需要与后一个字符合并的首字符集合（与 Python 版 if 链完全一致）。
        ''' </summary>
        Private ReadOnly MultiCharLeads As New HashSet(Of Char) From {
            "%"c, "C"c, "B"c, "S"c, "N"c, "R"c, "X"c, "L"c, "A"c, "M"c,
            "T"c, "Z"c, "s"c, "t"c, "H"c, "+"c, "-"c, "K"c, "F"c
        }

        ''' <summary>
        ''' 会与首字符合并成一个 token 的二字符序列。
        ''' </summary>
        ''' <remarks>
        ''' 区分大小写：<c>Si</c>（硅）与 <c>si</c>（芳香硅）是两个不同的 token。
        ''' </remarks>
        Private ReadOnly TwoCharTokens As New HashSet(Of String)(StringComparer.Ordinal) From {
            "Cl", "Ca", "Cu", "Br", "Be", "Ba", "Bi", "Si", "Se", "Sr",
            "Na", "Ni", "Rb", "Ra", "Xe", "Li", "Al", "As", "Ag", "Au",
            "Mg", "Mn", "Te", "Zn", "si", "se", "te", "He", "Kr", "Fe",
            "+2", "+3", "+4", "-2", "-3", "-4"
        }

        ''' <summary>
        ''' 把 SMILES 拆成 token 数组。
        ''' </summary>
        ''' <param name="smiles">SMILES 文本。</param>
        ''' <returns>token 数组；空输入返回空数组。</returns>
        Public Function Split(smiles As String) As String()
            If String.IsNullOrEmpty(smiles) Then Return New String() {}

            Dim tokens As New List(Of String)
            Dim i As Integer = 0

            ' 与 Python 一致：循环只处理前 len-1 个字符，最后一个字符单独处理，
            ' 这样二字母判断永远不会越界。
            While i < smiles.Length - 1
                Dim c As Char = smiles(i)

                If c = "%"c Then
                    tokens.Add(smiles.Substring(i, Math.Min(3, smiles.Length - i)))
                    i += 3
                ElseIf MultiCharLeads.Contains(c) AndAlso TwoCharTokens.Contains(smiles.Substring(i, 2)) Then
                    tokens.Add(smiles.Substring(i, 2))
                    i += 2
                Else
                    tokens.Add(c.ToString())
                    i += 1
                End If
            End While

            If i = smiles.Length - 1 Then tokens.Add(smiles.Chars(i).ToString())

            Return tokens.ToArray()
        End Function

        ''' <summary>
        ''' 把 SMILES 拆成 token 并用单个空格连接（等价于 Python 的 <c>split()</c> 返回值）。
        ''' </summary>
        ''' <param name="smiles">SMILES 文本。</param>
        Public Function SplitJoined(smiles As String) As String
            Return String.Join(" ", Split(smiles))
        End Function

    End Module

End Namespace
