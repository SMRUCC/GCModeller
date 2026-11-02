Imports G = SMRUCC.genomics.Molecule.SMILES.Graph
Imports System.Text

Namespace SmilesSyntax

    ''' <summary>
    ''' 把分子图确定性地写回 SMILES 字符串。
    ''' </summary>
    ''' <remarks>
    ''' 写出的是 Kekulé 形式（芳香体系以交替的单/双键表达），因此与
    ''' <see cref="SmilesParser"/> 的输出可以互相 round-trip。
    ''' 遍历顺序由 <see cref="G.MoleculeGraph.MorganRanks"/> 决定，同一分子图多次写出结果一致。
    ''' 环闭合号按"先生成先生效"的顺序分配，超过 9 时使用 <c>%nn</c> 记号。
    ''' </remarks>
    Public Module SmilesWriter

        ''' <summary>
        ''' 把分子图写出为 SMILES 字符串（确定性遍历，Kekulé 形式）。
        ''' </summary>
        ''' <param name="graph">待写出的分子图。</param>
        ''' <returns>SMILES 文本；多组分之间以 <c>.</c> 分隔。</returns>
        Public Function Write(graph As G.MoleculeGraph) As String
            Dim builder As New SmilesBuilder(graph)

            Return builder.Build()
        End Function

        ''' <summary>
        ''' 按指定选项把分子图写出为 SMILES 字符串。
        ''' </summary>
        ''' <param name="graph">待写出的分子图。</param>
        ''' <param name="options">
        ''' 写出选项；<c>Nothing</c> 或 <c>SmilesWriteOptions.Default</c> 等价于
        ''' <c>Write(graph)</c> 的确定性行为。
        ''' </param>
        ''' <returns>SMILES 文本；多组分之间以 <c>.</c> 分隔。</returns>
        Public Function Write(graph As G.MoleculeGraph, options As SmilesWriteOptions) As String
            Dim builder As New SmilesBuilder(graph, options)

            Return builder.Build()
        End Function

    End Module

    Friend Class SmilesBuilder

        Private ReadOnly graph As G.MoleculeGraph
        Private ReadOnly ranks As List(Of String)
        Private ReadOnly adjacency As List(Of List(Of Integer))
        Private ReadOnly componentSet As New HashSet(Of Integer)
        Private ReadOnly inTree As New HashSet(Of Integer)
        Private ReadOnly parentBond As Dictionary(Of Integer, Integer)
        Private ReadOnly digits As New Dictionary(Of Integer, Integer)
        Private ReadOnly ringWritten As New Dictionary(Of Integer, Integer)

        ''' <summary>写出选项（永不为 Nothing）。</summary>
        Private ReadOnly options As SmilesWriteOptions

        ''' <summary>随机化遍历使用的随机数发生器（仅当 <see cref="SmilesWriteOptions.RandomizeNeighborOrder"/> 打开时使用）。</summary>
        Private ReadOnly rnd As Random

        ''' <summary>
        ''' 每条键的随机排序键（惰性生成并缓存）。
        ''' </summary>
        ''' <remarks>
        ''' 必须缓存：<see cref="buildTree"/> 与 <see cref="emit"/> 会多次查询同一条键的邻接顺序，
        ''' 若每次重新取随机数，两次排序结果会不一致，生成树与写出顺序就会互相矛盾。
        ''' </remarks>
        Private ReadOnly randomKeys As New Dictionary(Of Integer, String)

        ''' <summary>
        ''' 为一个分子图创建一个写出器（确定性遍历，与历史行为一致）。
        ''' </summary>
        ''' <param name="graph">待写出的分子图。</param>
        Public Sub New(graph As G.MoleculeGraph)
            Me.New(graph, Nothing)
        End Sub

        ''' <summary>
        ''' 为一个分子图创建一个写出器。
        ''' </summary>
        ''' <param name="graph">待写出的分子图。</param>
        ''' <param name="options">写出选项；<c>Nothing</c> 表示使用默认（确定性）选项。</param>
        Public Sub New(graph As G.MoleculeGraph, options As SmilesWriteOptions)
            Me.graph = graph
            Me.options = If(options, New SmilesWriteOptions())
            Me.ranks = graph.MorganRanks()
            Me.adjacency = graph.Adjacency()
            Me.parentBond = New Dictionary(Of Integer, Integer)
            Me.rnd = If(Me.options.Seed.HasValue, New Random(Me.options.Seed.Value), New Random())
        End Sub

        ''' <summary>
        ''' 执行写出。
        ''' </summary>
        Public Function Build() As String
            Dim sb As New StringBuilder
            Dim components As List(Of List(Of Integer)) = graph.Components()
            Dim first As Boolean = True

            For Each component As List(Of Integer) In components
                If Not first Then sb.Append("."c)

                first = False

                componentSet.Clear()
                inTree.Clear()
                parentBond.Clear()
                digits.Clear()
                ringWritten.Clear()
                randomKeys.Clear()

                For Each atom As Integer In component
                    componentSet.Add(atom)
                Next

                Dim start As Integer = selectStartAtom(component)

                Call buildTree(start)
                Call assignRingDigits()

                Call emit(start, -1, sb)
            Next

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' 选择写出一个分量的起点原子。
        ''' </summary>
        ''' <param name="component">当前分量的原子下标集合（已排序）。</param>
        ''' <remarks>
        ''' 优先级：显式指定的 <see cref="SmilesWriteOptions.StartAtom"/>（且属于当前分量）
        ''' → 随机起点（<see cref="SmilesWriteOptions.RandomizeNeighborOrder"/>）
        ''' → Morgan 秩最小者（确定性，与历史行为一致）。
        ''' </remarks>
        Private Function selectStartAtom(component As List(Of Integer)) As Integer
            If options.StartAtom.HasValue AndAlso componentSet.Contains(options.StartAtom.Value) Then
                Return options.StartAtom.Value
            End If

            If options.RandomizeNeighborOrder Then
                Return component(rnd.Next(component.Count))
            End If

            Return component.
                OrderBy(Function(a) ranks(a), StringComparer.Ordinal).
                ThenBy(Function(a) a).
                First()
        End Function

        Private Sub buildTree(start As Integer)
            Dim visited As New HashSet(Of Integer)
            Dim queue As New Queue(Of Integer)

            visited.Add(start)
            queue.Enqueue(start)

            While queue.Count > 0
                Dim a As Integer = queue.Dequeue()

                For Each id As Integer In adjacency(a).
                    OrderBy(Function(i) neighborRank(a, i), StringComparer.Ordinal).
                    ThenBy(Function(i) i)

                    Dim bond As G.Bond = graph.Bonds(id)
                    Dim other As Integer = If(bond.a = a, bond.b, bond.a)

                    If Not componentSet.Contains(other) Then Continue For
                    If visited.Contains(other) Then Continue For

                    visited.Add(other)
                    inTree.Add(id)
                    parentBond(other) = id
                    queue.Enqueue(other)
                Next
            End While
        End Sub

        Private Function neighborRank(atom As Integer, bondIndex As Integer) As String
            If options.RandomizeNeighborOrder Then Return getRandomKey(bondIndex)

            Dim bond As G.Bond = graph.Bonds(bondIndex)
            Dim other As Integer = If(bond.a = atom, bond.b, bond.a)

            Return ranks(other)
        End Function

        ''' <summary>
        ''' 取（并缓存）一条键的随机排序键。
        ''' </summary>
        ''' <param name="bondIndex">键下标。</param>
        Private Function getRandomKey(bondIndex As Integer) As String
            Dim key As String = Nothing

            If Not randomKeys.TryGetValue(bondIndex, key) Then
                key = rnd.Next().ToString("D10")
                randomKeys(bondIndex) = key
            End If

            Return key
        End Function

        Private Sub assignRingDigits()
            Dim next_ As Integer = 1
            Dim ringBonds As New List(Of Integer)

            For id = 0 To graph.Bonds.Count - 1
                If inTree.Contains(id) Then Continue For

                Dim bond As G.Bond = graph.Bonds(id)

                If Not componentSet.Contains(bond.a) OrElse Not componentSet.Contains(bond.b) Then Continue For

                ringBonds.Add(id)
            Next

            ' 随机化时环闭合号的分配顺序也参与随机，进一步扩大枚举空间
            If options.RandomizeNeighborOrder Then shuffle(ringBonds)

            For Each id As Integer In ringBonds
                digits(id) = next_
                next_ += 1
            Next
        End Sub

        ''' <summary>原地洗牌（Fisher-Yates）。</summary>
        ''' <param name="list">待洗牌的列表。</param>
        Private Sub shuffle(list As List(Of Integer))
            For i = list.Count - 1 To 1 Step -1
                Dim j As Integer = rnd.Next(i + 1)
                Dim tmp As Integer = list(i)

                list(i) = list(j)
                list(j) = tmp
            Next
        End Sub

        Private Sub emit(a As Integer, incoming As Integer, sb As StringBuilder)
            sb.Append(atomSymbol(a))

            ' 先写出该原子上的环闭合号（每个环键在两端各写一次）
            For Each id As Integer In adjacency(a)
                If Not digits.ContainsKey(id) Then Continue For

                Dim written As Integer = 0

                ringWritten.TryGetValue(id, written)

                If written >= 2 Then Continue For

                sb.Append(bondSymbol(graph.Bonds(id))).Append(ringDigit(digits(id)))
                ringWritten(id) = written + 1
            Next

            ' 再深度优先展开生成树上的子节点
            Dim children As New List(Of Integer)

            For Each id As Integer In adjacency(a)
                If id = incoming Then Continue For
                If Not inTree.Contains(id) Then Continue For

                children.Add(id)
            Next

            children.Sort(Function(x, y) String.CompareOrdinal(neighborRank(a, x), neighborRank(a, y)))

            For i = 0 To children.Count - 2
                sb.Append("("c)
                sb.Append(bondSymbol(graph.Bonds(children(i))))
                Call emit(childOf(a, children(i)), children(i), sb)
                sb.Append(")"c)
            Next

            If children.Count > 0 Then
                Dim last As Integer = children(children.Count - 1)

                sb.Append(bondSymbol(graph.Bonds(last)))
                Call emit(childOf(a, last), last, sb)
            End If
        End Sub

        Private Function childOf(atom As Integer, bondIndex As Integer) As Integer
            Dim bond As G.Bond = graph.Bonds(bondIndex)

            Return If(bond.a = atom, bond.b, bond.a)
        End Function

        Private Shared Function ringDigit(digit As Integer) As String
            If digit <= 9 Then Return digit.ToString()

            Return "%" & digit.ToString("00")
        End Function

        ''' <summary>
        ''' 键的书写符号：芳香键省略符号、单键可带 <c>/</c> 或 <c>\</c> 立体记号，
        ''' 其余按 Kekulé 键级输出 <c>=</c> / <c>#</c>。
        ''' </summary>
        ''' <param name="bond">待写出的键。</param>
        Private Function bondSymbol(bond As G.Bond) As String
            If options.EmitAromatic AndAlso bond.IsAromatic Then Return ""

            If options.EmitStereo AndAlso bond.order = 1 Then
                Select Case bond.Stereo
                    Case G.BondStereo.Up
                        Return "/"
                    Case G.BondStereo.Down
                        Return "\"
                End Select
            End If

            Select Case bond.OrderInt
                Case 2
                    Return "="
                Case 3
                    Return "#"
                Case Else
                    Return ""
            End Select
        End Function

        Private Function atomSymbol(index As Integer) As String
            Dim atom As G.Atom = graph.Atoms(index)
            Dim aromatic As Boolean = options.EmitAromatic AndAlso atom.IsAromatic
            Dim stereo As String = If(options.EmitStereo, stereoTag(atom), "")
            Dim needsBracket As Boolean =
                atom.Charge <> 0 OrElse
                atom.Isotope <> 0 OrElse
                atom.AtomClass <> 0 OrElse
                atom.ExplicitH > 0 OrElse
                atom.NoImplicitH OrElse
                stereo.Length > 0 OrElse
                Not G.ElementTable.IsOrganicSubset(atom.Element)

            Dim symbol As String = If(aromatic, aromaticSymbol(atom.Element), atom.Element)

            If Not needsBracket Then Return symbol

            Dim sb As New StringBuilder("[")

            If atom.Isotope <> 0 Then sb.Append(atom.Isotope.ToString)

            sb.Append(symbol)

            If stereo.Length > 0 Then sb.Append(stereo)

            Dim h As Integer = graph.TotalH(index)

            If h > 0 Then
                sb.Append("H")

                If h > 1 Then sb.Append(h.ToString)
            End If

            If atom.Charge > 0 Then
                sb.Append("+")

                If atom.Charge > 1 Then sb.Append(atom.Charge.ToString)
            ElseIf atom.Charge < 0 Then
                sb.Append("-")

                If atom.Charge < -1 Then sb.Append((-atom.Charge).ToString)
            End If

            If atom.AtomClass <> 0 Then sb.Append(":").Append(atom.AtomClass.ToString)

            sb.Append("]")

            Return sb.ToString()
        End Function

        ''' <summary>芳香原子的 SMILES 小写符号。</summary>
        ''' <param name="element">元素符号（标准大小写）。</param>
        Private Shared Function aromaticSymbol(element As String) As String
            Select Case element
                Case "B"
                    Return "b"
                Case "C"
                    Return "c"
                Case "N"
                    Return "n"
                Case "O"
                    Return "o"
                Case "P"
                    Return "p"
                Case "S"
                    Return "s"
                Case "Se"
                    Return "se"
                Case "As"
                    Return "as"
                Case "Te"
                    Return "te"
                Case Else
                    Return element
            End Select
        End Function

        ''' <summary>四面体手性标记文本（<c>@</c> / <c>@@</c>）。</summary>
        ''' <param name="atom">待写出的原子。</param>
        Private Shared Function stereoTag(atom As G.Atom) As String
            Select Case atom.Stereo
                Case G.AtomStereo.Clockwise
                    Return "@@"
                Case G.AtomStereo.CounterClockwise
                    Return "@"
                Case G.AtomStereo.Unsupported
                    Return If(String.IsNullOrEmpty(atom.StereoText), "", atom.StereoText)
                Case Else
                    Return ""
            End Select
        End Function

    End Class

End Namespace
