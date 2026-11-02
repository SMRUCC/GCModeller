Imports G = SMRUCC.genomics.Molecule.SMILES.Graph
Imports System.Text
Imports Coordinates = SMRUCC.genomics.Molecule.SMILES.Coordinates

Namespace Graph

    ''' <summary>
    ''' 统一的分子图对象：SMILES 与 InChI 两种结构式字符串解析之后都汇入此类型。
    ''' </summary>
    ''' <remarks>
    ''' <list type="bullet">
    ''' <item>原子以 0 基下标标识，<see cref="Atoms"/> 与键表 <see cref="Bonds"/> 共同构成分子；</item>
    ''' <item>氢以"记号声明的氢"（<see cref="Atom.ExplicitH"/>）与"按价态反推的隐式氢"
    ''' （<see cref="ImplicitH"/>）两段表示，二者之和由 <see cref="TotalH"/> 给出；</item>
    ''' <item>芳香性同时以 <see cref="Bond.IsAromatic"/> / <see cref="Atom.IsAromatic"/> 与
    ''' Kekulé 化之后的 1/2/3 键级两种形式保留，供化学计算与结构绘图各取所需；</item>
    ''' <item>坐标只有一份，写入 <see cref="Atom.X"/>/<see cref="Atom.Y"/>（2D）与
    ''' <see cref="Atom.Z"/>（3D），由 <see cref="Layout2D"/> / <see cref="Embed3D"/> 生成。</item>
    ''' </list>
    ''' 化学上无法判定的情形不会抛异常，而是追加到 <see cref="Warnings"/>。
    ''' </remarks>
    Public Class MoleculeGraph

        ''' <summary>
        ''' 分子内的全部原子，下标即 <see cref="Atom.Index"/>。
        ''' </summary>
        Public Property Atoms As List(Of Atom) = New List(Of Atom)

        ''' <summary>
        ''' 分子内的全部化学键。
        ''' </summary>
        Public Property Bonds As List(Of Bond) = New List(Of Bond)

        ''' <summary>
        ''' 结构式字符串的原始格式。
        ''' </summary>
        Public Property Format As MoleculeFormat = MoleculeFormat.Unknown

        ''' <summary>
        ''' 解析时所使用的原始字符串。
        ''' </summary>
        Public Property SourceText As String

        ''' <summary>
        ''' 解析或坐标重建过程中的非致命问题（芳香性无法 Kekulé 化、价态无法满足等）。
        ''' </summary>
        Public ReadOnly Property Warnings As New List(Of String)

        Private _adjacency As List(Of List(Of Integer)) = Nothing
        Private _rings As List(Of Ring) = Nothing
        Private _formula As String = Nothing
        Private _molKey As String = Nothing
        Private _morgan As List(Of String) = Nothing

        ''' <summary>
        ''' 创建一个空的分子图。
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' 从已有的原子与键表创建一个分子图（会重建索引）。
        ''' </summary>
        ''' <param name="atoms">原子列表。</param>
        ''' <param name="bonds">键列表。</param>
        Public Sub New(atoms As IEnumerable(Of Atom), bonds As IEnumerable(Of Bond))
            Me.Atoms = atoms.ToList()

            For i = 0 To Me.Atoms.Count - 1
                Me.Atoms(i).Index = i
            Next

            Me.Bonds = bonds.ToList()
        End Sub

        ''' <summary>
        ''' 原子总数。
        ''' </summary>
        Public ReadOnly Property AtomCount As Integer
            Get
                Return Atoms.Count
            End Get
        End Property

        ''' <summary>
        ''' 化学键总数。
        ''' </summary>
        Public ReadOnly Property BondCount As Integer
            Get
                Return Bonds.Count
            End Get
        End Property

        ''' <summary>
        ''' 是否不含任何原子。
        ''' </summary>
        Public ReadOnly Property IsEmpty As Boolean
            Get
                Return Atoms.Count = 0
            End Get
        End Property

        ''' <summary>
        ''' 作废全部派生缓存（邻接表 / 环 / 分子式 / 指纹）。
        ''' 任何改动原子或键的操作之后都必须调用。
        ''' </summary>
        Public Sub Invalidate()
            _adjacency = Nothing
            _rings = Nothing
            _formula = Nothing
            _molKey = Nothing
            _morgan = Nothing
        End Sub

        ''' <summary>
        ''' 追加一个原子并返回它（<see cref="Atom.Index"/> 已赋值）。
        ''' </summary>
        ''' <param name="element">元素符号。</param>
        ''' <param name="charge">形式电荷。</param>
        Public Function AddAtom(element As String, Optional charge As Integer = 0) As Atom
            Dim atom As New Atom(element, charge) With {
                .Index = Atoms.Count
            }

            Atoms.Add(atom)
            Invalidate()

            Return atom
        End Function

        ''' <summary>
        ''' 在指定原子之间建立一条键。
        ''' </summary>
        ''' <param name="a">一端原子下标。</param>
        ''' <param name="b">另一端原子下标。</param>
        ''' <param name="order">键级。</param>
        ''' <param name="aromatic">是否属于芳香体系。</param>
        Public Function AddBond(a As Integer, b As Integer, Optional order As Double = 1, Optional aromatic As Boolean = False) As Bond
            Dim bond As New Bond(a, b, order) With {
                .IsAromatic = aromatic
            }

            Bonds.Add(bond)
            Invalidate()

            Return bond
        End Function

        ''' <summary>
        ''' 邻接表（惰性构建）：每个原子一个"键下标"列表。
        ''' </summary>
        Public Function Adjacency() As List(Of List(Of Integer))
            If _adjacency IsNot Nothing Then Return _adjacency

            Dim adjacencyList As New List(Of List(Of Integer))

            For i = 0 To Atoms.Count - 1
                adjacencyList.Add(New List(Of Integer))
            Next

            For i = 0 To Bonds.Count - 1
                Dim bond As Bond = Bonds(i)

                If bond.a >= 0 AndAlso bond.a < adjacencyList.Count Then adjacencyList(bond.a).Add(i)
                If bond.b >= 0 AndAlso bond.b < adjacencyList.Count Then adjacencyList(bond.b).Add(i)
            Next

            _adjacency = adjacencyList

            Return adjacencyList
        End Function

        ''' <summary>
        ''' 枚举与指定原子相连的全部键。
        ''' </summary>
        ''' <param name="index">原子下标。</param>
        Public Iterator Function BondsOf(index As Integer) As IEnumerable(Of Bond)
            For Each id As Integer In Adjacency()(index)
                Yield Bonds(id)
            Next
        End Function

        ''' <summary>
        ''' 枚举指定原子的全部邻接原子及其键级。
        ''' </summary>
        ''' <param name="index">原子下标。</param>
        Public Function Neighbors(index As Integer) As List(Of (neighbor As Integer, order As Double))
            Dim list As New List(Of (neighbor As Integer, order As Double))

            For Each bond As Bond In BondsOf(index)
                list.Add((If(bond.a = index, bond.b, bond.a), bond.order))
            Next

            Return list
        End Function

        ''' <summary>
        ''' 重原子度（成键原子个数，键级不参与计数）。
        ''' </summary>
        ''' <param name="index">原子下标。</param>
        Public Function Degree(index As Integer) As Integer
            Return Adjacency()(index).Count
        End Function

        ''' <summary>
        ''' 查询两个原子之间的键级。
        ''' </summary>
        ''' <param name="a">第一个原子下标。</param>
        ''' <param name="b">第二个原子下标。</param>
        ''' <returns>键级；无键时返回 <c>0</c>。</returns>
        Public Function BondOrder(a As Integer, b As Integer) As Double
            Dim bond As Bond = BondBetween(a, b)

            Return If(bond Is Nothing, 0, bond.order)
        End Function

        ''' <summary>
        ''' 取两个原子之间的键对象。
        ''' </summary>
        ''' <param name="a">第一个原子下标。</param>
        ''' <param name="b">第二个原子下标。</param>
        ''' <returns>键对象；不存在时返回 <c>Nothing</c>。</returns>
        Public Function BondBetween(a As Integer, b As Integer) As Bond
            For Each bond As Bond In BondsOf(a)
                If (bond.a = a AndAlso bond.b = b) OrElse (bond.a = b AndAlso bond.b = a) Then Return bond
            Next

            Return Nothing
        End Function

        ''' <summary>
        ''' 指定原子的隐式氢数目（按价态反推）。
        ''' </summary>
        ''' <param name="index">原子下标。</param>
        Public Function ImplicitH(index As Integer) As Integer
            Return ChemistryRules.ImplicitHydrogens(Me, index)
        End Function

        ''' <summary>
        ''' 指定原子上的氢总数 = 记号声明的氢 + 隐式氢 + 图中真实存在的氢原子邻居。
        ''' </summary>
        ''' <param name="index">原子下标。</param>
        Public Function TotalH(index As Integer) As Integer
            Dim total As Integer = Atoms(index).ExplicitH + ImplicitH(index)

            For Each bond As Bond In BondsOf(index)
                Dim other As Integer = If(bond.a = index, bond.b, bond.a)

                If Atoms(other).IsH Then total += CInt(Math.Round(bond.order))
            Next

            Return total
        End Function

        ''' <summary>
        ''' 指定原子的杂化类型。
        ''' </summary>
        ''' <param name="index">原子下标。</param>
        Public Function Hybridization(index As Integer) As Hybridization
            Return ChemistryRules.HybridizationOf(Me, index)
        End Function

        ''' <summary>
        ''' 价态校验：返回价态超限的原子下标列表。
        ''' </summary>
        ''' <returns>违规原子下标；为空表示整个分子价态合法。</returns>
        Public Function ValenceViolations() As List(Of Integer)
            Dim bad As New List(Of Integer)

            For i = 0 To Atoms.Count - 1
                Dim atom As Atom = Atoms(i)

                If atom.IsH Then Continue For

                Dim used As Integer = 0

                For Each bond As Bond In BondsOf(i)
                    Dim other As Integer = If(bond.a = i, bond.b, bond.a)

                    If Not Atoms(other).IsH Then used += CInt(Math.Round(bond.order))
                Next

                Dim target As Integer = ChemistryRules.EffectiveValence(atom.Element, atom.Charge, used)

                If target > 0 AndAlso used > target Then bad.Add(i)
            Next

            Return bad
        End Function

        ''' <summary>
        ''' 连通分量：把多组分分子（<c>A.B</c>）拆成若干独立片段的原子下标集合。
        ''' </summary>
        Public Function Components() As List(Of List(Of Integer))
            Dim visited(Atoms.Count - 1) As Boolean
            Dim parts As New List(Of List(Of Integer))
            Dim adjacencyList As List(Of List(Of Integer)) = Adjacency()

            For a = 0 To Atoms.Count - 1
                If visited(a) Then Continue For

                Dim component As New List(Of Integer)
                Dim stack As New Stack(Of Integer)

                stack.Push(a)
                visited(a) = True

                While stack.Count > 0
                    Dim current As Integer = stack.Pop()
                    component.Add(current)

                    For Each id As Integer In adjacencyList(current)
                        Dim bond As Bond = Bonds(id)
                        Dim nextNode As Integer = If(bond.a = current, bond.b, bond.a)

                        If Not visited(nextNode) Then
                            visited(nextNode) = True
                            stack.Push(nextNode)
                        End If
                    Next
                End While

                component.Sort()
                parts.Add(component)
            Next

            Return parts
        End Function

        ''' <summary>
        ''' 最小环基（惰性计算并缓存）。
        ''' </summary>
        Public Function Rings() As List(Of Ring)
            If _rings Is Nothing Then _rings = G.Rings.FindRings(Me)

            Return _rings
        End Function

        ''' <summary>
        ''' 环系：由共享键的环合并而成的稠合环集合。
        ''' </summary>
        Public Function RingSystems() As List(Of List(Of Ring))
            Return G.Rings.RingSystems(Rings())
        End Function

        ''' <summary>
        ''' 分子式（Hill 排序：先 C、再 H、其余按字母序），含隐式氢。
        ''' </summary>
        Public Function MolecularFormula() As String
            If _formula IsNot Nothing Then Return _formula

            Dim counts As New Dictionary(Of String, Integer)

            For i = 0 To Atoms.Count - 1
                Dim atom As Atom = Atoms(i)

                If atom.IsH Then
                    Call bump(counts, "H", 1)
                    Continue For
                End If

                Call bump(counts, atom.Element, 1)

                Dim h As Integer = TotalH(i)

                If h > 0 Then Call bump(counts, "H", h)
            Next

            Dim order As New List(Of String)

            If counts.ContainsKey("C") Then
                order.Add("C")
                If counts.ContainsKey("H") Then order.Add("H")
            ElseIf counts.ContainsKey("H") Then
                order.Add("H")
            End If

            order.AddRange(counts.Keys _
                .Where(Function(k) Not order.Contains(k)) _
                .OrderBy(Function(k) k, StringComparer.Ordinal))

            Dim sb As New StringBuilder

            For Each element As String In order
                Dim n As Integer = counts(element)

                sb.Append(element)
                If n > 1 Then sb.Append(n.ToString)
            Next

            _formula = sb.ToString()

            Return _formula
        End Function

        ''' <summary>
        ''' 相对分子质量（按元素原子量求和，含隐式氢）。
        ''' </summary>
        Public Function MolecularWeight() As Double
            Dim mass As Double = 0

            For i = 0 To Atoms.Count - 1
                mass += ElementTable.AtomicMass(Atoms(i).Element)
            Next

            For i = 0 To Atoms.Count - 1
                If Atoms(i).IsH Then Continue For

                mass += TotalH(i) * ElementTable.AtomicMass("H")
            Next

            Return mass
        End Function

        Private Shared Sub bump(counts As Dictionary(Of String, Integer), key As String, n As Integer)
            Dim current As Integer = 0

            counts.TryGetValue(key, current)
            counts(key) = current + n
        End Sub

        ''' <summary>
        ''' 把全部隐式氢展开为显式氢原子（3D 坐标重建与空间填充渲染之前调用）。
        ''' </summary>
        ''' <returns>新增的氢原子个数。</returns>
        Public Function AddHydrogens() As Integer
            Dim added As Integer = 0
            Dim plan As New List(Of (parent As Integer, count As Integer))

            For i = 0 To Atoms.Count - 1
                Dim h As Integer = ImplicitH(i)

                If h > 0 Then plan.Add((i, h))
            Next

            For Each item In plan
                For k = 1 To item.count
                    Dim hydrogen As Atom = AddAtom("H", 0)
                    hydrogen.IsHydrogen = True
                    hydrogen.NoImplicitH = True
                    Call AddBond(item.parent, hydrogen.Index, 1)
                    added += 1
                Next
            Next

            Return added
        End Function

        ''' <summary>
        ''' 移除图中的全部氢原子（保留重原子骨架）。
        ''' </summary>
        ''' <returns>被移除的氢原子个数。</returns>
        Public Function RemoveHydrogens() As Integer
            Dim removed As Integer = 0
            Dim mapping(Atoms.Count - 1) As Integer
            Dim survivors As New List(Of Atom)

            For i = 0 To Atoms.Count - 1
                If Atoms(i).IsH Then
                    mapping(i) = -1
                    removed += 1
                Else
                    mapping(i) = survivors.Count
                    survivors.Add(Atoms(i))
                End If
            Next

            If removed = 0 Then Return 0

            Dim survivingBonds As New List(Of Bond)

            For Each bond As Bond In Bonds
                Dim a As Integer = mapping(bond.a)
                Dim b As Integer = mapping(bond.b)

                If a < 0 OrElse b < 0 Then Continue For

                survivingBonds.Add(New Bond(a, b, bond.order) With {
                    .IsAromatic = bond.IsAromatic,
                    .Stereo = bond.Stereo,
                    .IsRing = bond.IsRing
                })
            Next

            Atoms = survivors
            Bonds = bonds

            For i = 0 To Atoms.Count - 1
                Atoms(i).Index = i
            Next

            Invalidate()

            Return removed
        End Function

        ''' <summary>
        ''' 基于分子图重建 2D 平面坐标（写入 <see cref="Atom.X"/> / <see cref="Atom.Y"/>）。
        ''' </summary>
        ''' <param name="options">布局参数，为空时使用默认值。</param>
        Public Sub Layout2D(Optional options As Layout2DOptions = Nothing)
            Call Coordinates.PlaneLayout.Run(Me, If(options, New Layout2DOptions()))
        End Sub

        ''' <summary>
        ''' 基于分子图重建 3D 立体坐标（写入 <see cref="Atom.X"/>/<see cref="Atom.Y"/>/<see cref="Atom.Z"/>）。
        ''' </summary>
        ''' <param name="options">重建参数，为空时使用默认值。</param>
        Public Sub Embed3D(Optional options As Embed3DOptions = Nothing)
            Dim config As Embed3DOptions = If(options, New Embed3DOptions())

            If config.AddHydrogens Then Call AddHydrogens()

            Call Coordinates.StereoEmbedding.Run(Me, config)
        End Sub

        ''' <summary>
        ''' 计算各原子的 Morgan/扩展连通性（EC）标签，用于确定性的规范化排序。
        ''' </summary>
        ''' <param name="rounds">最大迭代轮数；分区数收敛时提前退出。</param>
        Public Function MorganRanks(Optional rounds As Integer = 8) As List(Of String)
            If _morgan IsNot Nothing Then Return _morgan

            Dim adjacencyList As List(Of List(Of Integer)) = Adjacency()
            Dim labels As New List(Of String)

            For a = 0 To Atoms.Count - 1
                Dim orders As List(Of Integer) = adjacencyList(a).
                    Select(Function(id) CInt(Math.Round(Bonds(id).order))).
                    OrderBy(Function(x) x).
                    ToList()

                labels.Add($"{Atoms(a).Element}|{Atoms(a).Charge}|{TotalH(a)}|{adjacencyList(a).Count}|{String.Join(",", orders)}")
            Next

            labels = compress(labels)

            Dim previous As Integer = labels.Distinct().Count()

            For round_ As Integer = 1 To rounds
                Dim next_ As New List(Of String)

                For a = 0 To Atoms.Count - 1
                    Dim parts As New List(Of String)

                    For Each id As Integer In adjacencyList(a)
                        Dim bond As Bond = Bonds(id)
                        Dim other As Integer = If(bond.a = a, bond.b, bond.a)

                        parts.Add(labels(other) & ":" & CInt(Math.Round(bond.order)))
                    Next

                    parts.Sort(StringComparer.Ordinal)
                    next_.Add(labels(a) & "#" & String.Join(";", parts))
                Next

                labels = compress(next_)

                Dim distinct As Integer = labels.Distinct().Count()

                If distinct = previous Then Exit For

                previous = distinct
            Next

            _morgan = labels

            Return labels
        End Function

        Private Shared Function compress(labels As List(Of String)) As List(Of String)
            Dim unique As List(Of String) = labels.
                Distinct().
                OrderBy(Function(x) x, StringComparer.Ordinal).
                ToList()

            Dim map As New Dictionary(Of String, Integer)

            For i = 0 To unique.Count - 1
                map(unique(i)) = i
            Next

            Return labels.Select(Function(x) map(x).ToString).ToList()
        End Function

        ''' <summary>
        ''' 与同构无关的规范指纹：同构分子必然相同，异构分子几乎必然不同。
        ''' </summary>
        ''' <remarks>
        ''' 由 Morgan 秩编号后的原子不变量与键三元组排序拼接而成，
        ''' 用于 round-trip 校验与去重（并非严格意义的规范 SMILES）。
        ''' </remarks>
        Public Function MolKey() As String
            If _molKey IsNot Nothing Then Return _molKey

            Dim ranks As List(Of String) = MorganRanks()
            Dim unique As List(Of String) = ranks.
                Distinct().
                OrderBy(Function(x) x, StringComparer.Ordinal).
                ToList()

            Dim map As New Dictionary(Of String, Integer)

            For i = 0 To unique.Count - 1
                map(unique(i)) = i
            Next

            Dim atomEntries As New List(Of String)

            For a = 0 To Atoms.Count - 1
                atomEntries.Add($"{Atoms(a).Element}|{Atoms(a).Charge}|{TotalH(a)}|{map(ranks(a))}")
            Next

            atomEntries.Sort(StringComparer.Ordinal)

            Dim bondEntries As New List(Of String)

            For Each bond As Bond In Bonds
                Dim ra As Integer = map(ranks(bond.a))
                Dim rb As Integer = map(ranks(bond.b))

                bondEntries.Add($"{Math.Min(ra, rb)}-{Math.Max(ra, rb)}:{CInt(Math.Round(bond.order))}")
            Next

            bondEntries.Sort(StringComparer.Ordinal)
            _molKey = String.Join(";", atomEntries) & "#" & String.Join(";", bondEntries)

            Return _molKey
        End Function

        ''' <summary>
        ''' 深拷贝当前分子图（原子、坐标、键与告警各自独立）。
        ''' </summary>
        Public Function Clone() As MoleculeGraph
            Dim copy As New MoleculeGraph With {
                .Format = Format,
                .SourceText = SourceText
            }

            For Each atom As Atom In Atoms
                copy.Atoms.Add(New Atom With {
                    .Index = atom.Index,
                    .Element = atom.Element,
                    .Isotope = atom.Isotope,
                    .Charge = atom.Charge,
                    .ExplicitH = atom.ExplicitH,
                    .NoImplicitH = atom.NoImplicitH,
                    .IsAromatic = atom.IsAromatic,
                    .AtomClass = atom.AtomClass,
                    .Stereo = atom.Stereo,
                    .StereoText = atom.StereoText,
                    .IsHydrogen = atom.IsHydrogen,
                    .X = atom.X,
                    .Y = atom.Y,
                    .Z = atom.Z
                })
            Next

            For Each bond As Bond In Bonds
                copy.Bonds.Add(New Bond(bond.a, bond.b, bond.order) With {
                    .IsAromatic = bond.IsAromatic,
                    .Stereo = bond.Stereo,
                    .IsRing = bond.IsRing
                })
            Next

            copy.Warnings.AddRange(Warnings)

            Return copy
        End Function

        ''' <summary>
        ''' 生成用于调试的概要描述。
        ''' </summary>
        Public Overrides Function ToString() As String
            Return $"{MolecularFormula()} [{Atoms.Count} atom(s), {Bonds.Count} bond(s), {Rings().Count} ring(s)]"
        End Function

    End Class

End Namespace
