Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports SMRUCC.genomics.Molecule.SMILES
Imports SMRUCC.genomics.Molecule.SMILES.Graph
Imports SMRUCC.genomics.Molecule.SMILES.PDBExport
Imports PdbStructures = SMRUCC.genomics.Data.RCSB.PDB.Structures

''' <summary>
''' SMILES/InChI → 分子图 → PDB 分子对象 → PDB 文件 的完整转换流程示例与验证。
''' </summary>
''' <remarks>
''' 每个用例都会：
''' <list type="number">
''' <item>解析结构式字符串得到统一的分子图；</item>
''' <item>把分子图转换成 <see cref="SMRUCC.genomics.Data.RCSB.PDB.Structures.Molecule(Of T)"/> 并写出 PDB 文件；</item>
''' <item>用 RCSB PDB 库把文件读回来，逐项校验原子数、元素序列、坐标、连通性；</item>
''' <item>另外直接解析 <c>CONECT</c>/<c>REMARK</c> 记录，校验键的往返一致性。</item>
''' </list>
''' </remarks>
Friend Module PdbDemo

    ''' <summary>坐标往返允许的最大偏差（Å，PDB 坐标列只有 3 位小数）。</summary>
    Const COORDINATE_TOLERANCE As Double = 0.0011

    ''' <summary>
    ''' 执行全部用例的 PDB 转换与回读校验。
    ''' </summary>
    ''' <param name="cases">用例表。</param>
    ''' <param name="folder">输出目录，PDB 文件写入其下的 <c>pdb</c> 子目录。</param>
    ''' <returns>成功写出的 PDB 文件个数。</returns>
    Public Function Run(cases As List(Of MoleculeCase), folder As String) As Integer
        Dim target As String = System.IO.Path.Combine(folder, "pdb")
        Dim written As Integer = 0

        If Not Directory.Exists(target) Then Call Directory.CreateDirectory(target)

        For Each item As MoleculeCase In cases
            Console.WriteLine()
            Console.WriteLine($"> {item.Name}  ->  pdb")

            Dim graph As MoleculeGraph = Nothing
            Dim reason As String = Nothing

            If Not MoleculeIO.TryParse(item.Text, graph, item.Format, reason) Then
                Call DebugAudit.Test(False, $"{item.Name}: pdb conversion parse failed -> {reason}")
                Continue For
            End If

            Dim path As String = System.IO.Path.Combine(target, $"{item.Name}.pdb")

            Try
                ' 第一步：分子图 -> RCSB PDB 分子对象
                Dim exported As PdbStructures.Molecule(Of PdbStructures.Atom) = MoleculeToPdb.Convert(graph)

                ' 第二步：PDB 分子对象 -> PDB 文件
                Call PdbStructures.PdbDocumentWriter.Save(exported, path, graph.MolecularFormula(), graph.SourceText)

                written += 1

                Call DebugAudit.Test(File.Exists(path) AndAlso New FileInfo(path).Length > 0,
                    $"{item.Name}: pdb file written")

                Call verify(item, exported, path)
            Catch ex As Exception
                Call DebugAudit.Test(False, $"{item.Name}: pdb export failed -> {ex.GetType().Name}: {ex.Message}")
            End Try
        Next

        ' 一次性便捷入口：文本 -> PDB 文件
        Dim quick As String = System.IO.Path.Combine(target, $"{cases(0).Name}.one-step.pdb")

        Try
            Call MoleculeToPdb.SaveTextAsPdb(cases(0).Text, quick)
            Call DebugAudit.Test(File.Exists(quick), "MoleculeToPdb.SaveTextAsPdb writes the pdb file directly")
        Catch ex As Exception
            Call DebugAudit.Test(False, $"MoleculeToPdb.SaveTextAsPdb failed -> {ex.Message}")
        End Try

        ' 导出选项演示：去掉氢原子后再写 PDB
        Dim heavyPath As String = System.IO.Path.Combine(target, $"{cases(0).Name}.no-hydrogen.pdb")

        Try
            Dim options As New PdbExportOptions With {.IncludeHydrogens = False}

            Call MoleculeToPdb.SaveAsPdb(MoleculeIO.Parse(cases(0).Text), heavyPath, options)

            Dim heavy As PdbStructures.Molecule(Of PdbStructures.Atom) =
                PdbStructures.StructureIO.ReadPdb(Of PdbStructures.Atom)(heavyPath)
            Dim hasHydrogen As Boolean = False

            For Each atom As PdbStructures.Atom In heavy.Atoms
                If atom.Element.ToUpperInvariant() = "H" Then hasHydrogen = True
            Next

            Call DebugAudit.Test(Not hasHydrogen, "PdbExportOptions.IncludeHydrogens=False drops hydrogen atoms")
            Call DebugAudit.Test(heavy.Atoms.Count > 0, "hydrogen-free pdb still contains the heavy atoms")
        Catch ex As Exception
            Call DebugAudit.Test(False, $"hydrogen-free pdb export failed -> {ex.Message}")
        End Try

        Call DebugAudit.Mark("pdb export", $"{written}/{cases.Count} structures written to {target}")

        Return written
    End Function

    ''' <summary>
    ''' 用 RCSB PDB 库回读 PDB 文件，并逐项校验与源分子图的一致性。
    ''' </summary>
    Private Sub verify(item As MoleculeCase, exported As PdbStructures.Molecule(Of PdbStructures.Atom), path As String)
        Dim reload As PdbStructures.Molecule(Of PdbStructures.Atom)

        Try
            reload = PdbStructures.StructureIO.ReadPdb(Of PdbStructures.Atom)(path)
        Catch ex As Exception
            Call DebugAudit.Test(False, $"{item.Name}: pdb file cannot be re-read -> {ex.Message}")
            Return
        End Try

        Call DebugAudit.AreEqual(exported.Atoms.Count, reload.Atoms.Count, $"{item.Name}: pdb atom count (round trip)")

        If exported.Atoms.Count <> reload.Atoms.Count Then Return

        Dim sameElements As Boolean = True
        Dim sameNames As Boolean = True
        Dim maxDeviation As Double = 0
        Dim names As New HashSet(Of String)(StringComparer.Ordinal)

        For i = 0 To exported.Atoms.Count - 1
            Dim expected As PdbStructures.Atom = exported.Atoms(i)
            Dim actual As PdbStructures.Atom = reload.Atoms(i)

            ' RCSB PDB 的固定列解析器会把双字符元素规范化成大写（Na -> NA），故此处忽略大小写
            If Not String.Equals(expected.Element, actual.Element, StringComparison.OrdinalIgnoreCase) Then
                sameElements = False
            End If
            If expected.AtomName <> actual.AtomName Then sameNames = False
            If Not names.Add(actual.AtomName) Then sameNames = False

            maxDeviation = Math.Max(maxDeviation,
                Math.Abs(expected.X - actual.X) +
                Math.Abs(expected.Y - actual.Y) +
                Math.Abs(expected.Z - actual.Z))
        Next

        Call DebugAudit.Test(sameElements, $"{item.Name}: pdb element sequence preserved")
        Call DebugAudit.Test(sameNames, $"{item.Name}: pdb atom names unique and preserved")
        Call DebugAudit.Test(maxDeviation <= COORDINATE_TOLERANCE * 3,
            $"{item.Name}: pdb coordinates preserved (max deviation {maxDeviation:F5} A)")

        ' ---- 直接校验 CONECT 记录表达的连通性 ----
        Dim expectedPairs As HashSet(Of String) = bondPairs(exported)
        Dim actualPairs As HashSet(Of String) = conectPairs(File.ReadAllLines(path))

        Call DebugAudit.AreEqual(expectedPairs.Count, actualPairs.Count, $"{item.Name}: CONECT bond count")
        Call DebugAudit.Test(expectedPairs.SetEquals(actualPairs),
            $"{item.Name}: CONECT connectivity identical to the molecular graph")

        If exported.Bonds.Count > 0 Then
            Dim text As String = File.ReadAllText(path)

            Call DebugAudit.Test(text.Contains("REMARK 901"), $"{item.Name}: bond orders are recorded in REMARK 901")
        End If

        Call DebugAudit.Test(File.ReadAllText(path).Contains("END"), $"{item.Name}: pdb record terminator present")

        Call DebugAudit.Mark($"{item.Name}: pdb written",
            $"{System.IO.Path.GetFileName(path)}, atoms={reload.Atoms.Count}, bonds={expectedPairs.Count}")
    End Sub

    ''' <summary>
    ''' 把分子对象的键转换为 <c>小序号-大序号</c> 形式的无序对集合。
    ''' </summary>
    Private Function bondPairs(mol As PdbStructures.Molecule(Of PdbStructures.Atom)) As HashSet(Of String)
        Dim pairs As New HashSet(Of String)(StringComparer.Ordinal)

        For Each bond In mol.Bonds
            Dim a As Integer = mol.Atoms(bond.A).Serial
            Dim b As Integer = mol.Atoms(bond.B).Serial

            Call pairs.Add($"{Math.Min(a, b)}-{Math.Max(a, b)}")
        Next

        Return pairs
    End Function

    ''' <summary>
    ''' 解析 PDB 文本中的 <c>CONECT</c> 记录，返回 <c>小序号-大序号</c> 形式的无序对集合。
    ''' </summary>
    Private Function conectPairs(lines As String()) As HashSet(Of String)
        Dim pairs As New HashSet(Of String)(StringComparer.Ordinal)

        For Each line As String In lines
            If Not line.StartsWith("CONECT") Then Continue For

            Dim serial As Integer

            If Not Integer.TryParse(line.Substring(6, 5).Trim(), serial) Then Continue For

            Dim cursor As Integer = 11

            While cursor + 5 <= line.Length
                Dim partner As Integer

                If Integer.TryParse(line.Substring(cursor, 5).Trim(), partner) Then
                    Call pairs.Add($"{Math.Min(serial, partner)}-{Math.Max(serial, partner)}")
                End If

                cursor += 5
            End While
        Next

        Return pairs
    End Function

End Module
