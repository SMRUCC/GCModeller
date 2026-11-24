Imports System.Text
Imports SMRUCC.genomics.Molecule.SMILES.Coordinates
Imports SMRUCC.genomics.Molecule.SMILES.Graph
Imports PdbStructures = SMRUCC.genomics.Data.RCSB.PDB.Structures

Namespace PDBExport

    ''' <summary>
    ''' 把 SMILES/InChI 解析得到的分子图转换成 RCSB PDB 库的分子对象。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 转换是"无损到可表达"的映射：<see cref="MoleculeGraph"/> 的原子、键（含键级）、
    ''' 3D 坐标被逐一搬进 <see cref="SMRUCC.genomics.Data.RCSB.PDB.Structures.Molecule(Of T)"/>，
    ''' 元素符号、残基名、链标识、原子序号按 PDB 约定补齐。
    ''' </para>
    ''' <para>
    ''' PDB 的 <c>CONECT</c> 记录只能表达连通性、不能表达键级，因此转换得到的
    ''' <see cref="PdbStructures.Bond.Order"/> 会随对象保留，写出文件时由
    ''' <see cref="PdbStructures.PdbDocumentWriter"/> 额外写入 <c>REMARK</c> 注释。
    ''' </para>
    ''' <code>
    ''' Dim mol = MoleculeIO.Parse("c1ccccc1O")
    ''' Dim pdb = MoleculeToPdb.Convert(mol)
    ''' Call MoleculeToPdb.SaveAsPdb(mol, "./phenol.pdb")
    ''' </code>
    ''' </remarks>
    Public Module MoleculeToPdb

        ''' <summary>
        ''' 把分子图转换成 PDB 分子对象。
        ''' </summary>
        ''' <param name="graph">源分子图。</param>
        ''' <param name="options">导出选项，为空时使用默认值。</param>
        ''' <returns>可以直接交给 <see cref="PdbStructures.PdbDocumentWriter"/> 写出的 PDB 分子对象。</returns>
        ''' <exception cref="ArgumentNullException"><paramref name="graph"/> 为空时抛出。</exception>
        Public Function Convert(graph As MoleculeGraph, Optional options As PdbExportOptions = Nothing) As PdbStructures.Molecule(Of PdbStructures.Atom)
            If graph Is Nothing Then Throw New ArgumentNullException(NameOf(graph))

            Dim opt As PdbExportOptions = If(options, New PdbExportOptions)
            Dim source As MoleculeGraph = prepare(graph, opt)

            Dim molecule As New PdbStructures.Molecule(Of PdbStructures.Atom) With {
                .Id = If(String.IsNullOrEmpty(opt.Title), graph.MolecularFormula(), opt.Title)
            }

            Dim serials As New Dictionary(Of String, Integer)(StringComparer.Ordinal)
            Dim offset As Vec3 = centerOffset(source, opt)
            Dim resName As String = padResName(opt.ResName)
            Dim chain As String = padChain(opt.ChainID)

            For i = 0 To source.AtomCount - 1
                Dim atom As Atom = source.Atoms(i)
                Dim element As String = normalizeElement(atom.Element)

                molecule.Atoms.Add(New PdbStructures.Atom With {
                    .Serial = i + 1,
                    .AtomName = uniqueAtomName(element, serials),
                    .Element = element,
                    .AltLoc = "",
                    .ResName = resName,
                    .ChainID = chain,
                    .ResSeq = opt.ResSeq,
                    .X = atom.X - offset.X,
                    .Y = atom.Y - offset.Y,
                    .Z = atom.Z - offset.Z,
                    .Occupancy = 1.0,
                    .TempFactor = 0.0,
                    .Charge = atom.Charge,
                    .IsHet = opt.UseHetatm,
                    .IsWater = False
                })
            Next

            For Each bond As Bond In source.Bonds
                molecule.Bonds.Add(New PdbStructures.Bond(bond.a, bond.b, bond.order))
            Next

            Return molecule
        End Function

        ''' <summary>
        ''' 把分子图转换并保存为 PDB 文件。
        ''' </summary>
        ''' <param name="graph">源分子图。</param>
        ''' <param name="path">输出文件路径（目录不存在时自动创建）。</param>
        ''' <param name="options">导出选项，为空时使用默认值。</param>
        ''' <returns>写出的文件全路径。</returns>
        Public Function SaveAsPdb(graph As MoleculeGraph, path As String, Optional options As PdbExportOptions = Nothing) As String
            If graph Is Nothing Then Throw New ArgumentNullException(NameOf(graph))

            Dim opt As PdbExportOptions = If(options, New PdbExportOptions)
            Dim molecule As PdbStructures.Molecule(Of PdbStructures.Atom) = Convert(graph, opt)
            Dim title As String = If(String.IsNullOrEmpty(opt.Title), graph.MolecularFormula(), opt.Title)

            Return PdbStructures.PdbDocumentWriter.Save(molecule, path, title, graph.SourceText)
        End Function

        ''' <summary>
        ''' 把 SMILES/InChI 文本直接保存为 PDB 文件。
        ''' </summary>
        ''' <param name="text">SMILES 或 InChI 文本（自动识别格式）。</param>
        ''' <param name="path">输出文件路径。</param>
        ''' <param name="options">导出选项。</param>
        Public Function SaveTextAsPdb(text As String, path As String, Optional options As PdbExportOptions = Nothing) As String
            Return SaveAsPdb(MoleculeIO.Parse(text), path, options)
        End Function

        ''' <summary>
        ''' 准备导出用的分子图：必要时在副本上补全 3D 坐标、移除氢原子。
        ''' </summary>
        ''' <param name="graph">源分子图（不会被修改）。</param>
        ''' <param name="options">导出选项。</param>
        Private Function prepare(graph As MoleculeGraph, options As PdbExportOptions) As MoleculeGraph
            Dim source As MoleculeGraph = graph

            If options.Ensure3D AndAlso Not has3D(source) Then
                source = source.Clone()
                Call source.Embed3D()
            End If

            If Not options.IncludeHydrogens AndAlso containsHydrogen(source) Then
                If source Is graph Then source = source.Clone()

                Call source.RemoveHydrogens()
            End If

            Return source
        End Function

        Private Function has3D(graph As MoleculeGraph) As Boolean
            For Each atom As Atom In graph.Atoms
                If Math.Abs(atom.Z) > 1.0E-6 Then Return True
            Next

            Return False
        End Function

        Private Function containsHydrogen(graph As MoleculeGraph) As Boolean
            For Each atom As Atom In graph.Atoms
                If atom.IsH Then Return True
            Next

            Return False
        End Function

        ''' <summary>
        ''' 计算把几何中心移到原点的平移量。
        ''' </summary>
        Private Function centerOffset(graph As MoleculeGraph, options As PdbExportOptions) As Vec3
            If Not options.CenterAtOrigin OrElse graph.AtomCount = 0 Then Return Vec3.Zero

            Dim cx As Double = 0
            Dim cy As Double = 0
            Dim cz As Double = 0

            For Each atom As Atom In graph.Atoms
                cx += atom.X
                cy += atom.Y
                cz += atom.Z
            Next

            Return New Vec3(cx / graph.AtomCount, cy / graph.AtomCount, cz / graph.AtomCount)
        End Function

        ''' <summary>
        ''' 生成 PDB 风格的原子名：元素符号 + 该元素在分子内的出现序号（如 <c>C1</c>、<c>CL2</c>）。
        ''' </summary>
        Private Function uniqueAtomName(element As String, serials As Dictionary(Of String, Integer)) As String
            Dim index As Integer

            If Not serials.TryGetValue(element, index) Then index = 0

            Dim name As String

            Do
                index += 1
                name = (element & index.ToString()).ToUpperInvariant()
            Loop While name.Length > 4

            serials(element) = index

            Return name
        End Function

        ''' <summary>
        ''' 规范化元素符号：首字母大写、次字母小写（<c>CL</c> → <c>Cl</c>）。
        ''' </summary>
        Private Function normalizeElement(element As String) As String
            If String.IsNullOrEmpty(element) Then Return "C"

            Dim text As String = element.Trim()

            If text.Length = 1 Then Return text.ToUpperInvariant()

            Return text.Substring(0, 1).ToUpperInvariant() & text.Substring(1, 1).ToLowerInvariant()
        End Function

        Private Function padResName(resName As String) As String
            Dim text As String = If(String.IsNullOrEmpty(resName), "UNL", resName.Trim().ToUpperInvariant())

            If text.Length > 3 Then text = text.Substring(0, 3)

            Return text.PadRight(3)
        End Function

        Private Function padChain(chainID As String) As String
            Dim text As String = If(String.IsNullOrEmpty(chainID), "A", chainID.Trim())

            If text.Length > 1 Then text = text.Substring(0, 1)

            Return text
        End Function

    End Module

End Namespace
