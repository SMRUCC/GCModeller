Imports G = SMRUCC.genomics.Molecule.SMILES.Graph
Namespace SmilesSyntax

    ''' <summary>
    ''' SMILES 结构式字符串解析器（单遍状态机）。
    ''' </summary>
    ''' <remarks>
    ''' 支持的范围：分支 <c>(...)</c>、环闭合（单位数字与 <c>%nn</c>）、全部键符号
    ''' <c>- = # $ : / \</c>、方括号原子（同位素 / 元素 / 手性 / 氢数 / 电荷 / 原子类别）、
    ''' 芳香小写原子（<c>b c n o p s se as</c>）、<c>@</c> 与 <c>@@</c> 立体标记、<c>.</c> 多组分。
    ''' 解析完成后统一执行芳香性 Kekulé 化，使键级回到 1/2/3，隐式氢则按价态按需反推。
    ''' </remarks>
    Public Module SmilesParser

        ''' <summary>
        ''' 解析一个 SMILES 字符串。
        ''' </summary>
        ''' <param name="smiles">SMILES 文本。</param>
        ''' <returns>统一的分子图对象。</returns>
        ''' <exception cref="MoleculeParseException">遇到无法识别的记号时抛出，异常中带有出错位置。</exception>
        Public Function Parse(smiles As String) As G.MoleculeGraph
            If String.IsNullOrWhiteSpace(smiles) Then
                Throw New MoleculeParseException("empty SMILES input", 0, smiles)
            End If

            Dim reader As New SmilesReader(smiles.Trim)

            Return reader.Read()
        End Function

    End Module

    ''' <summary>
    ''' SMILES 状态机的实现细节。
    ''' </summary>
    Friend Class SmilesReader

        Private Structure RingClosure
            Public Property Atom As Integer
            Public Property Order As Double
            Public Property Aromatic As Boolean
            Public Property Stereo As G.BondStereo
        End Structure

        Const TWO_LETTER_ELEMENTS As String = "He,Li,Be,Ne,Na,Mg,Al,Si,Cl,Ar,Ca,Sc,Ti,Cr,Mn,Fe,Co,Ni,Cu,Zn,Ga,Ge,As,Se,Br,Kr,Rb,Sr,Zr,Nb,Mo,Tc,Ru,Rh,Pd,Ag,Cd,In,Sn,Sb,Te,Xe,Cs,Ba,La,Ce,Pr,Nd,Pm,Sm,Eu,Gd,Tb,Dy,Ho,Er,Tm,Yb,Lu,Hf,Ta,Re,Os,Ir,Pt,Au,Hg,Tl,Pb,Bi,Po,At,Rn"

        Private ReadOnly text As String
        Private ReadOnly graph As New G.MoleculeGraph
        Private ReadOnly twoLetter As New HashSet(Of String)(TWO_LETTER_ELEMENTS.Split(","c), StringComparer.OrdinalIgnoreCase)
        Private ReadOnly ringOpen As New Dictionary(Of String, RingClosure)
        Private ReadOnly stereoNeighbors As New Dictionary(Of Integer, List(Of Integer))
        Private ReadOnly branches As New Stack(Of Integer)

        Private cursor As Integer = 0
        Private previous As Integer = -1
        Private pendingOrder As Double = 0
        Private pendingStereo As G.BondStereo = G.BondStereo.None
        Private bondPending As Boolean = False
        Private pendingAromaticBond As Boolean = False

        ''' <summary>
        ''' 以原始文本创建一个读取器。
        ''' </summary>
        ''' <param name="smiles">SMILES 文本。</param>
        Public Sub New(smiles As String)
            text = smiles
        End Sub

        ''' <summary>
        ''' 执行解析，返回构建好的分子图。
        ''' </summary>
        Public Function Read() As G.MoleculeGraph
            graph.Format = G.MoleculeFormat.Smiles
            graph.SourceText = text

            While cursor < text.Length
                Dim c As Char = text(cursor)

                Select Case c
                    Case "("c
                        If previous < 0 Then Throw fail("branch without a preceding atom")
                        branches.Push(previous)
                        cursor += 1
                    Case ")"c
                        If branches.Count = 0 Then Throw fail("unbalanced branch parenthesis")

                        previous = branches.Pop()
                        cursor += 1
                    Case "."c
                        previous = -1
                        cursor += 1
                    Case "-"c
                        pendingOrder = 1
                        bondPending = True
                        pendingStereo = G.BondStereo.None
                        cursor += 1
                    Case "="c
                        pendingOrder = 2
                        bondPending = True
                        cursor += 1
                    Case "#"c
                        pendingOrder = 3
                        bondPending = True
                        cursor += 1
                    Case "$"c
                        pendingOrder = 4
                        bondPending = True
                        cursor += 1
                    Case ":"c
                        pendingOrder = 1
                        bondPending = True
                        pendingAromaticBond = True
                        cursor += 1
                    Case "/"c
                        pendingOrder = 1
                        bondPending = True
                        pendingStereo = G.BondStereo.Up
                        cursor += 1
                    Case "\"c
                        pendingOrder = 1
                        bondPending = True
                        pendingStereo = G.BondStereo.Down
                        cursor += 1
                    Case "%"c
                        Call readPercentRing()
                    Case "["c
                        Call readBracketAtom()
                    Case "]"c, "}"c, "{"c
                        Throw fail($"unexpected character '{c}'")
                    Case Else
                        If Char.IsDigit(c) Then
                            Call readRingNumber(c.ToString, 1)
                        ElseIf Char.IsLetter(c) OrElse c = "*"c Then
                            Call readOrganicAtom()
                        Else
                            Throw fail($"unexpected character '{c}'")
                        End If
                End Select
            End While

            If branches.Count > 0 Then Throw fail("unbalanced branch parenthesis")

            If ringOpen.Count > 0 Then
                Throw fail("unclosed ring bond: " & String.Join(", ", ringOpen.Keys))
            End If

            If graph.AtomCount = 0 Then Throw fail("no atom was read")

            Call finishStereo()

            Call G.ChemistryRules.Kekulize(graph)

            For Each bad As Integer In graph.ValenceViolations()
                graph.Warnings.Add($"valence overflow on atom #{bad} ({graph.Atoms(bad).Element})")
            Next

            Return graph
        End Function

        Private Function fail(message As String) As MoleculeParseException
            Return New MoleculeParseException(message, cursor, text)
        End Function

#Region "原子"

        Private Sub readOrganicAtom()
            Dim start As Integer = cursor
            Dim symbol As String = Nothing
            Dim aromatic As Boolean = False

            Dim c As Char = text(cursor)

            If c = "*"c Then
                symbol = "*"
                cursor += 1
            ElseIf Char.IsLower(c) Then
                ' 芳香小写原子
                Dim letter As String = c.ToString
                cursor += 1

                If cursor < text.Length AndAlso Char.IsLower(text(cursor)) Then
                    Dim pair As String = letter & text(cursor)

                    If pair = "se" OrElse pair = "as" OrElse pair = "te" Then
                        letter = pair
                        cursor += 1
                    End If
                End If

                symbol = letter.Substring(0, 1).ToUpper & letter.Substring(1)
                aromatic = True
            Else
                Dim symbolText As String = c.ToString
                cursor += 1

                If cursor < text.Length AndAlso Char.IsLower(text(cursor)) Then
                    Dim pair As String = symbolText & text(cursor)

                    If twoLetter.Contains(pair) Then
                        symbolText = pair
                        cursor += 1
                    End If
                End If

                symbol = symbolText
            End If

            If symbol Is Nothing Then Throw fail("missing element symbol at " & start)

            Dim atom As G.Atom = graph.AddAtom(symbol, 0)

            atom.IsAromatic = aromatic
            atom.NoImplicitH = False

            Call connect(atom)
        End Sub

        Private Sub readBracketAtom()
            cursor += 1  ' skip '['

            Dim isotope As Integer = 0
            Dim symbol As String = Nothing
            Dim aromatic As Boolean = False
            Dim hcount As Integer = 0
            Dim charge As Integer = 0
            Dim atomClass As Integer = 0
            Dim stereo As G.AtomStereo = G.AtomStereo.None
            Dim stereoText As String = Nothing

            Dim digits As String = readDigits()

            If digits.Length > 0 Then isotope = Integer.Parse(digits)

            If cursor >= text.Length Then Throw fail("unterminated bracket atom")

            Dim c As Char = text(cursor)

            If c = "*"c Then
                symbol = "*"
                cursor += 1
            ElseIf Char.IsUpper(c) Then
                Dim symbolText As String = c.ToString

                cursor += 1

                If cursor < text.Length AndAlso Char.IsLower(text(cursor)) Then
                    Dim pair As String = symbolText & text(cursor)

                    If twoLetter.Contains(pair) Then
                        symbolText = pair
                        cursor += 1
                    End If
                End If

                symbol = symbolText
            ElseIf Char.IsLower(c) Then
                Dim letter As String = c.ToString

                cursor += 1

                If cursor < text.Length AndAlso Char.IsLower(text(cursor)) Then
                    Dim pair As String = letter & text(cursor)

                    If pair = "se" OrElse pair = "as" OrElse pair = "te" Then
                        letter = pair
                        cursor += 1
                    End If
                End If

                symbol = letter.Substring(0, 1).ToUpper & letter.Substring(1)
                aromatic = True
            Else
                Throw fail("missing element symbol in bracket atom")
            End If

            ' 手性
            If cursor < text.Length AndAlso text(cursor) = "@"c Then
                cursor += 1

                If cursor < text.Length AndAlso text(cursor) = "@"c Then
                    cursor += 1
                    stereoText = "@@"
                    stereo = G.AtomStereo.Clockwise
                ElseIf isStereoTagPrefix() Then
                    ' @TH1 / @AL2 / @SP1 / @TB10 / @OH22 之类的扩展记号
                    Dim tag As New Text.StringBuilder("@")

                    While cursor < text.Length AndAlso Char.IsLetterOrDigit(text(cursor))
                        tag.Append(text(cursor))
                        cursor += 1
                    End While

                    stereoText = tag.ToString()
                    stereo = G.AtomStereo.Unsupported
                Else
                    stereoText = "@"
                    stereo = G.AtomStereo.CounterClockwise
                End If
            End If

            ' 氢数
            If cursor < text.Length AndAlso text(cursor) = "H"c Then
                cursor += 1

                Dim count As String = readDigits()

                hcount = If(count.Length = 0, 1, Integer.Parse(count))
            End If

            ' 电荷
            If cursor < text.Length AndAlso (text(cursor) = "+"c OrElse text(cursor) = "-"c) Then
                Dim sign As Integer = If(text(cursor) = "+"c, 1, -1)
                Dim mark As Char = text(cursor)

                cursor += 1

                Dim count As Integer = 1
                Dim num As String = readDigits()

                If num.Length > 0 Then
                    count = Integer.Parse(num)
                Else
                    While cursor < text.Length AndAlso text(cursor) = mark
                        count += 1
                        cursor += 1
                    End While
                End If

                charge = sign * count
            End If

            ' 原子类别
            If cursor < text.Length AndAlso text(cursor) = ":"c Then
                cursor += 1

                Dim num As String = readDigits()

                If num.Length > 0 Then atomClass = Integer.Parse(num)
            End If

            If cursor >= text.Length OrElse text(cursor) <> "]"c Then
                Throw fail("unterminated bracket atom")
            End If

            cursor += 1

            Dim atom As G.Atom = graph.AddAtom(symbol, charge)

            atom.Isotope = isotope
            atom.IsAromatic = aromatic
            atom.ExplicitH = hcount
            atom.NoImplicitH = True
            atom.AtomClass = atomClass
            atom.Stereo = stereo
            atom.StereoText = stereoText

            Call connect(atom)
        End Sub

        Private Function readDigits() As String
            Dim start As Integer = cursor

            While cursor < text.Length AndAlso Char.IsDigit(text(cursor))
                cursor += 1
            End While

            Return text.Substring(start, cursor - start)
        End Function

        ''' <summary>
        ''' 判断当前位置是否是 SMILES 扩展手性记号（<c>TH</c>/<c>AL</c>/<c>SP</c>/<c>TB</c>/<c>OH</c>）的起始位置。
        ''' 这一点很重要：<c>[C@H]</c> 中的 <c>H</c> 是氢数，不能被当成手性记号的一部分。
        ''' </summary>
        Private Function isStereoTagPrefix() As Boolean
            If cursor + 1 >= text.Length Then Return False

            Dim prefix As String = text.Substring(cursor, 2).ToUpper()

            Select Case prefix
                Case "TH", "AL", "SP", "TB", "OH"
                    Return True
                Case Else
                    Return False
            End Select
        End Function

#End Region

#Region "键与环闭合"

        Private Sub connect(atom As G.Atom)
            If previous >= 0 Then
                Dim order As Double = If(bondPending, pendingOrder, 1)
                Dim aromatic As Boolean = pendingAromaticBond OrElse
                    (Not bondPending AndAlso graph.Atoms(previous).IsAromatic AndAlso atom.IsAromatic)

                If order <= 0 Then order = 1

                Dim bond As G.Bond = graph.AddBond(previous, atom.Index, If(aromatic, 1, order), aromatic)

                bond.Stereo = pendingStereo

                Call registerStereoNeighbor(previous, atom.Index)
                Call registerStereoNeighbor(atom.Index, previous)
            End If

            pendingOrder = 0
            bondPending = False
            pendingAromaticBond = False
            pendingStereo = G.BondStereo.None
            previous = atom.Index
        End Sub

        Private Sub registerStereoNeighbor(atom As Integer, neighbor As Integer)
            Dim list As List(Of Integer) = Nothing

            If Not stereoNeighbors.TryGetValue(atom, list) Then
                list = New List(Of Integer)
                stereoNeighbors(atom) = list
            End If

            If Not list.Contains(neighbor) Then list.Add(neighbor)
        End Sub

        Private Sub readPercentRing()
            If cursor + 2 >= text.Length Then Throw fail("incomplete '%nn' ring bond")

            Dim digits As String = text.Substring(cursor + 1, 2)
            Dim number As Integer

            If Not Integer.TryParse(digits, number) Then Throw fail("incomplete '%nn' ring bond")

            Call readRingNumber(digits, 3)
        End Sub

        Private Sub readRingNumber(ringId As String, tokenLength As Integer)
            cursor += tokenLength

            If previous < 0 Then Throw fail("ring closure without a preceding atom")

            Dim closure As RingClosure

            If ringOpen.TryGetValue(ringId, closure) Then
                Dim aromatic As Boolean = pendingAromaticBond OrElse
                    (Not bondPending AndAlso graph.Atoms(closure.Atom).IsAromatic AndAlso graph.Atoms(previous).IsAromatic)
                Dim order As Double = If(bondPending, pendingOrder, If(closure.Order > 0, closure.Order, 1))

                If order <= 0 Then order = 1

                Dim bond As G.Bond = graph.AddBond(closure.Atom, previous, If(aromatic, 1, order), aromatic)

                bond.Stereo = If(pendingStereo <> G.BondStereo.None, pendingStereo, closure.Stereo)

                ringOpen.Remove(ringId)
            Else
                ringOpen(ringId) = New RingClosure With {
                    .Atom = previous,
                    .Order = If(bondPending, pendingOrder, 0),
                    .Aromatic = pendingAromaticBond OrElse (Not bondPending AndAlso graph.Atoms(previous).IsAromatic),
                    .Stereo = pendingStereo
                }
            End If

            pendingOrder = 0
            bondPending = False
            pendingAromaticBond = False
            pendingStereo = G.BondStereo.None
        End Sub

#End Region

        Private Sub finishStereo()
            For Each item In stereoNeighbors
                Dim atom As G.Atom = graph.Atoms(item.Key)

                If atom.Stereo = G.AtomStereo.None Then Continue For

                Dim list As New List(Of Integer)(item.Value)
                Dim h As Integer = atom.ExplicitH + graph.ImplicitH(atom.Index)

                If list.Count = 3 AndAlso h > 0 Then list.Add(-1)

                atom.StereoNeighbors = list.ToArray()
            Next
        End Sub

    End Class

End Namespace
