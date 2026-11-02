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
        ''' 把分子图写出为 SMILES 字符串。
        ''' </summary>
        ''' <param name="graph">待写出的分子图。</param>
        ''' <returns>SMILES 文本；多组分之间以 <c>.</c> 分隔。</returns>
        Public Function Write(graph As G.MoleculeGraph) As String
            Dim builder As New SmilesBuilder(graph)

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

        ''' <summary>
        ''' 为一个分子图创建一个写出器。
        ''' </summary>
        ''' <param name="graph">待写出的分子图。</param>
        Public Sub New(graph As G.MoleculeGraph)
            Me.graph = graph
            Me.ranks = graph.MorganRanks()
            Me.adjacency = graph.Adjacency()
            Me.parentBond = New Dictionary(Of Integer, Integer)
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

                For Each atom As Integer In component
                    componentSet.Add(atom)
                Next

                Dim start As Integer = component.
                    OrderBy(Function(a) ranks(a), StringComparer.Ordinal).
                    ThenBy(Function(a) a).
                    First()

                Call buildTree(start)
                Call assignRingDigits()

                Call emit(start, -1, sb)
            Next

            Return sb.ToString()
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
            Dim bond As G.Bond = graph.Bonds(bondIndex)
            Dim other As Integer = If(bond.a = atom, bond.b, bond.a)

            Return ranks(other)
        End Function

        Private Sub assignRingDigits()
            Dim next_ As Integer = 1

            For id = 0 To graph.Bonds.Count - 1
                If inTree.Contains(id) Then Continue For

                Dim bond As G.Bond = graph.Bonds(id)

                If Not componentSet.Contains(bond.a) OrElse Not componentSet.Contains(bond.b) Then Continue For

                digits(id) = next_
                next_ += 1
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

        Private Shared Function bondSymbol(bond As G.Bond) As String
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
            Dim needsBracket As Boolean =
                atom.Charge <> 0 OrElse
                atom.Isotope <> 0 OrElse
                atom.AtomClass <> 0 OrElse
                atom.ExplicitH > 0 OrElse
                atom.NoImplicitH OrElse
                Not G.ElementTable.IsOrganicSubset(atom.Element)

            If Not needsBracket Then Return atom.Element

            Dim sb As New StringBuilder("[")

            If atom.Isotope <> 0 Then sb.Append(atom.Isotope.ToString)

            sb.Append(atom.Element)

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

    End Class

End Namespace
