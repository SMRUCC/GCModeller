Imports G = SMRUCC.genomics.Molecule.SMILES.Graph
Imports System.Text
Imports System.Text.RegularExpressions

Namespace InchiSyntax

    ''' <summary>
    ''' InChI 字符串的逐层解析器。
    ''' </summary>
    ''' <remarks>
    ''' 标准 InChI 的结构为 <c>InChI=1S/&lt;formula&gt;/c…/h…/q…/p…/t…/m…/s…</c>。
    ''' 本读取器把各层切成原始字符串并解析出分子式、连接关系、氢分布、电荷与手性中心，
    ''' 化学推断（键级还原）留给 <see cref="InChIBuilder"/>。
    ''' </remarks>
    Public Module InChIReader

        ''' <summary>
        ''' 解析一个 InChI 字符串。
        ''' </summary>
        ''' <param name="inchi">InChI 文本。</param>
        ''' <returns>逐层解析之后的中间表示。</returns>
        ''' <exception cref="MoleculeParseException">格式不符合 InChI 规范时抛出。</exception>
        Public Function Parse(inchi As String) As InChIData
            If String.IsNullOrWhiteSpace(inchi) Then
                Throw New MoleculeParseException("empty InChI input", 0, inchi)
            End If

            Dim body As String = inchi.Trim

            If body.StartsWith("InChI=", StringComparison.OrdinalIgnoreCase) Then
                body = body.Substring("InChI=".Length)
            End If

            Dim tokens As String() = body.Split("/"c)
            Dim data As New InChIData With {
                .Version = tokens(0)
            }

            For i = 1 To tokens.Length - 1
                Dim token As String = tokens(i)

                If token.Length = 0 Then Continue For

                Dim marker As Char = token(0)

                If Char.IsLower(marker) Then
                    data.Layers(marker) = token.Substring(1)
                Else
                    data.Formula = token
                End If
            Next

            If String.IsNullOrEmpty(data.Formula) Then
                Throw New MoleculeParseException("missing formula layer", 0, inchi)
            End If

            Call parseFormula(data)
            Call parseConnection(data)
            Call parseHydrogen(data)
            Call parseCharge(data)
            Call parseStereo(data)

            Return data
        End Function

        ''' <summary>
        ''' 判断一个字符串是否看起来像 InChI。
        ''' </summary>
        ''' <param name="text">待判断的文本。</param>
        Public Function LooksLikeInChI(text As String) As Boolean
            If String.IsNullOrWhiteSpace(text) Then Return False

            Return text.Trim.StartsWith("InChI=", StringComparison.OrdinalIgnoreCase)
        End Function

        Private Sub parseFormula(data As InChIData)
            Dim parts As String() = data.Formula.Split("."c)
            Dim offset As Integer = 0
            Dim atom As New Regex("([A-Z][a-z]?)(\d*)")

            For Each part As String In parts
                data.Components.Add(part)

                Dim start As Integer = offset

                For Each match As Match In atom.Matches(part)
                    Dim symbol As String = match.Groups(1).Value
                    Dim countText As String = match.Groups(2).Value
                    Dim count As Integer = If(countText.Length = 0, 1, Integer.Parse(countText))

                    If symbol = "H" Then
                        data.HydrogenCount += count
                    Else
                        For k = 1 To count
                            data.Elements.Add(symbol)
                        Next
                    End If
                Next

                data.ComponentOffsets.Add((start, data.Elements.Count - 1))
                data.ComponentCharges.Add(0)
                data.ComponentProtons.Add(0)

                offset = data.Elements.Count
            Next
        End Sub

        Private Sub parseConnection(data As InChIData)
            If Not data.Layers.ContainsKey("c"c) Then Return

            Call parseConnectionLayer(data.Layers("c"c), data.Bonds)
        End Sub

        ''' <summary>
        ''' 取连接层解析出来的键表。
        ''' </summary>
        ''' <param name="data">InChI 层数据。</param>
        Public Function BondsOf(data As InChIData) As List(Of (a As Integer, b As Integer))
            Return data.Bonds
        End Function

        ''' <summary>
        ''' 解析连接层：<c>1-2(3-4)5</c> 这类路径/分支表达式。
        ''' </summary>
        Private Sub parseConnectionLayer(layer As String, bonds As List(Of (a As Integer, b As Integer)))
            Dim current As Integer = -1
            Dim stack As New Stack(Of Integer)
            Dim number As New StringBuilder

            For i = 0 To layer.Length - 1
                Dim c As Char = layer(i)

                If Char.IsDigit(c) Then
                    number.Append(c)
                    Continue For
                End If

                If number.Length > 0 Then
                    Dim index As Integer = Integer.Parse(number.ToString) - 1

                    If current >= 0 Then bonds.Add((current, index))

                    current = index
                    number.Clear()
                End If

                Select Case c
                    Case "-"c
                    Case "("c
                        stack.Push(current)
                    Case ")"c
                        If stack.Count > 0 Then current = stack.Pop()
                    Case ","c, ";"c
                        current = -1
                End Select
            Next

            If number.Length > 0 Then
                Dim index As Integer = Integer.Parse(number.ToString) - 1

                If current >= 0 Then bonds.Add((current, index))
            End If
        End Sub

        Private Sub parseHydrogen(data As InChIData)
            If Not data.Layers.ContainsKey("h"c) Then Return

            Dim layer As String = "h" & data.Layers("h"c)
            Dim mobile As New Regex("\(([^)]*)\)")

            For Each match As Match In mobile.Matches(layer)
                Call parseMobileGroup(match.Groups(1).Value, data)
            Next

            Dim text As String = mobile.Replace(layer, "").Substring(1)
            Dim pending As New List(Of Integer)
            Dim item As New Regex("^([0-9\-]+)(H(\d*))?$")

            For Each token As String In text.Split(","c)
                Dim trimmed As String = token.Trim()

                If trimmed.Length = 0 Then Continue For

                Dim match As Match = item.Match(trimmed)

                If Not match.Success Then Continue For

                Dim atoms As List(Of Integer) = expandRange(match.Groups(1).Value)

                If match.Groups(2).Value.Length > 0 Then
                    Dim countText As String = match.Groups(3).Value
                    Dim count As Integer = If(countText.Length = 0, 1, Integer.Parse(countText))

                    pending.AddRange(atoms)

                    For Each atom As Integer In pending
                        data.Hydrogens(atom) = count
                    Next

                    pending.Clear()
                Else
                    pending.AddRange(atoms)
                End If
            Next

            For Each atom As Integer In pending
                If Not data.Hydrogens.ContainsKey(atom) Then data.Hydrogens(atom) = 1
            Next
        End Sub

        Private Sub parseMobileGroup(content As String, data As InChIData)
            Dim count As Integer = 1
            Dim atoms As New List(Of Integer)

            For Each token As String In content.Split(","c)
                Dim trimmed As String = token.Trim()

                If trimmed.Length = 0 Then Continue For

                If trimmed.StartsWith("H") Then
                    Dim rest As String = trimmed.Substring(1)

                    count = If(rest.Length = 0, 1, Integer.Parse(rest))
                Else
                    atoms.AddRange(expandRange(trimmed))
                End If
            Next

            If atoms.Count = 0 Then Return

            data.MobileHydrogens.Add(New MobileHydrogen With {
                .Count = count,
                .Atoms = atoms.ToArray
            })

            ' 移动氢不计入固定氢分布，避免与价态还原冲突
            For Each atom As Integer In atoms
                data.Hydrogens.Remove(atom)
            Next
        End Sub

        Private Function expandRange(text As String) As List(Of Integer)
            Dim result As New List(Of Integer)
            Dim dash As Integer = text.IndexOf("-"c)

            If dash > 0 Then
                Dim from As Integer = Integer.Parse(text.Substring(0, dash))
                Dim [to] As Integer = Integer.Parse(text.Substring(dash + 1))

                For i = from To [to]
                    result.Add(i - 1)
                Next
            Else
                For Each part As String In text.Split(","c)
                    If part.Length > 0 Then result.Add(Integer.Parse(part) - 1)
                Next
            End If

            Return result
        End Function

        Private Sub parseCharge(data As InChIData)
            If data.Layers.ContainsKey("q"c) Then
                Dim values As List(Of Integer) = parseSignedList(data.Layers("q"c))

                For i = 0 To values.Count - 1
                    If i < data.ComponentCharges.Count Then data.ComponentCharges(i) = values(i)
                Next
            End If

            If data.Layers.ContainsKey("p"c) Then
                Dim values As List(Of Integer) = parseSignedList(data.Layers("p"c))

                For i = 0 To values.Count - 1
                    If i < data.ComponentProtons.Count Then data.ComponentProtons(i) = values(i)
                Next
            End If

            If data.Layers.ContainsKey("i"c) Then data.Isotope = data.Layers("i"c)
        End Sub

        Private Function parseSignedList(layer As String) As List(Of Integer)
            Dim result As New List(Of Integer)

            For Each part As String In layer.Split(";"c)
                Dim trimmed As String = part.Trim()

                If trimmed.Length = 0 Then
                    result.Add(0)
                    Continue For
                End If

                Dim value As Integer

                If Integer.TryParse(trimmed, value) Then
                    result.Add(value)
                Else
                    result.Add(0)
                End If
            Next

            Return result
        End Function

        Private Sub parseStereo(data As InChIData)
            If data.Layers.ContainsKey("t"c) Then
                data.Tetrahedral = data.Layers("t"c)

                Dim item As New Regex("(\d+)([+-])")

                For Each match As Match In item.Matches(data.Tetrahedral)
                    Dim index As Integer = Integer.Parse(match.Groups(1).Value) - 1

                    data.StereoCenters(index) = If(match.Groups(2).Value = "+", 1, -1)
                Next
            End If

            Dim stereo As New List(Of String)

            For Each marker As Char In New Char() {"b"c, "m"c, "s"c}
                If data.Layers.ContainsKey(marker) Then
                    stereo.Add(marker & data.Layers(marker))
                End If
            Next

            data.StereoLayer = String.Join("/", stereo)
        End Sub

    End Module

End Namespace
