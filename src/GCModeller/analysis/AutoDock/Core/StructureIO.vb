' ============================================================================
' StructureIO.vb — 分子结构模型 + PDB/SDF 读取
' ----------------------------------------------------------------------------
' [README §2.1 输入] 蛋白 → PDB；小分子代谢物 → SDF(V2000) 或 PDB(HETATM)。
' 重原子模型（united-atom，忽略氢）——Vina 打分在重原子上定义。
'
' PDB 连接性：标准残基用内置模板（MolBuilder.ResidueTemplates），
'   配体/HETATM 用共价半径距离感知（r < 1.3·(r_cov_i + r_cov_j)）。
' SDF 连接性：键块直接给出（order 4 = 芳香）。
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Text

Namespace MiniDock.Core

    ''' <summary>原子（重原子）</summary>
    Public Class Atom

        Public X As Double
        Public Y As Double
        Public Z As Double
        Public Element As String            ' 规范元素符号（首字母大写）
        Public VinaType As Int32            ' VinaAtomTypes 编码
        Public Charge As Double             ' PEOE 电荷（mmgbsa 用）
        Public LjEps As Double              ' Amber 简化 LJ ε（kcal/mol）
        Public LjRmin As Double             ' Amber 简化 LJ 最小距离半径 R*（Å）
        ' 来源信息（PDB）
        Public ChainId As String = " "
        Public ResName As String = ""
        Public ResSeq As Integer = 0
        Public AtomName As String = ""
        Public IsWater As Boolean = False
        Public FromReceptor As Boolean = True

    End Class

    ''' <summary>共价键</summary>
    Public Structure Bond

        Public A As Int32
        Public B As Int32
        Public Order As Double              ' 1 / 1.5(芳香) / 2 / 3

        Public Sub New(a As Int32, b As Int32, order As Double)
            Me.A = a : Me.B = b : Me.Order = order
        End Sub

    End Structure

    ''' <summary>分子（或蛋白链集合）</summary>
    Public Class Molecule

        Public Atoms As New List(Of Atom)()
        Public Bonds As New List(Of Bond)()
        Public Id As String = ""

        Public Function AtomCount() As Integer
            Return Atoms.Count
        End Function

    End Class

    Public Module StructureIO

        Private ReadOnly CovalentRadius As New Dictionary(Of String, Double) From {
            {"C", 0.77}, {"N", 0.75}, {"O", 0.73}, {"S", 1.02}, {"P", 1.06},
            {"F", 0.71}, {"Cl", 0.99}, {"Br", 1.14}, {"I", 1.33}, {"H", 0.37},
            {"MG", 1.36}, {"CA", 1.74}, {"FE", 1.25}, {"ZN", 1.25}, {"MN", 1.39}, {"CU", 1.32}}

        ''' <summary>规范化元素符号</summary>
        Public Function NormalizeElement(raw As String) As String
            Dim s = raw.Trim()
            If s.Length = 0 Then Return "C"
            Dim c0 = Char.ToUpper(s(0))
            If s.Length = 1 Then
                Return c0.ToString()
            End If
            ' 两字符：仅当第二位小写才视为两字符元素
            Dim c1 = s(1)
            If Char.IsLetter(c1) AndAlso Char.IsLower(c1) Then
                Return c0.ToString() & Char.ToUpper(c1)
            End If
            Return c0.ToString()
        End Function

        ' ---------------- PDB ----------------

        ''' <summary>
        ''' 读取 PDB：返回 (分子, 是否多模型)。支持 MODEL/ENDMDL（只取第一模型用于 dock；
        ''' mmgbsa 命令用 ReadPdbFrames 读取全部模型）。
        ''' </summary>
        Public Function ReadPdb(path As String) As Molecule
            Dim frames = ReadPdbFrames(path)
            If frames.Count = 0 Then Throw New InvalidDataException("PDB 无 ATOM/HETATM 记录: " & path)
            Return frames(0)
        End Function

        ''' <summary>读取 PDB 全部 MODEL 帧（每帧一个 Molecule）</summary>
        Public Function ReadPdbFrames(path As String) As List(Of Molecule)
            Dim frames As New List(Of Molecule)()
            Dim mol As New Molecule With {.Id = IO.Path.GetFileNameWithoutExtension(path)}
            Dim inModel As Boolean = False
            Dim firstModelDone = False

            For Each line In File.ReadLines(path)
                Dim rec = If(line.Length >= 6, line.Substring(0, 6).TrimEnd(), line.TrimEnd())
                If rec = "ENDMDL" Then
                    If mol.Atoms.Count > 0 Then frames.Add(mol)
                    inModel = False
                    firstModelDone = True
                    mol = New Molecule With {.Id = mol.Id}
                    Continue For
                End If
                If rec = "MODEL" Then
                    inModel = True
                    Continue For
                End If
                If rec <> "ATOM" AndAlso rec <> "HETATM" Then Continue For
                If inModel = False AndAlso firstModelDone Then Continue For   ' 只取第一模型

                Dim atom As New Atom()
                atom.AtomName = SafeSub(line, 12, 4).Trim()
                Dim altLoc = SafeSub(line, 16, 1)
                If altLoc <> " "c AndAlso altLoc <> "A"c AndAlso altLoc <> "" Then Continue For
                atom.ResName = SafeSub(line, 17, 3).Trim()
                atom.ChainId = If(SafeSub(line, 21, 1).Trim().Length = 0, " ", SafeSub(line, 21, 1).Trim())
                Integer.TryParse(SafeSub(line, 22, 4).Trim(), atom.ResSeq)
                Dim cx = SafeSub(line, 30, 8).Trim()
                Dim cy = SafeSub(line, 38, 8).Trim()
                Dim cz = SafeSub(line, 46, 8).Trim()
                Dim elemRaw = SafeSub(line, 76, 2)
                If elemRaw.Trim().Length > 0 Then
                    atom.Element = NormalizeElement(elemRaw)
                Else
                    atom.Element = GuessElement(atom.AtomName, atom.ResName)
                End If
                Dim ci As Double
                If Not Double.TryParse(cx, NumberStyles.Float, CultureInfo.InvariantCulture, ci) Then Continue For
                Dim cyy As Double
                If Not Double.TryParse(cy, NumberStyles.Float, CultureInfo.InvariantCulture, cyy) Then Continue For
                Dim cz As Double
                If Not Double.TryParse(cz, NumberStyles.Float, CultureInfo.InvariantCulture, cz) Then Continue For
                atom.X = ci : atom.Y = cyy : atom.Z = cz
                atom.ResName = atom.ResName.ToUpperInvariant()
                atom.IsWater = (atom.ResName = "HOH" OrElse atom.ResName = "WAT")
                atom.FromReceptor = True
                mol.Atoms.Add(atom)
            End For
            If frames.Count = 0 OrElse mol.Atoms.Count > 0 Then
                If mol.Atoms.Count > 0 Then frames.Add(mol)
            End If
            Return frames
        End Function

        ''' <summary>按距离感知配体/HETATM 连接性</summary>
        Public Sub PerceiveBonds(mol As Molecule)
            mol.Bonds.Clear()
            Dim n = mol.Atoms.Count
            For i = 0 To n - 1
                Dim ei = mol.Atoms(i).Element
                Dim ri As Double = 0
                If Not CovalentRadius.TryGetValue(ei, ri) Then ri = 0.77
                For j = i + 1 To n - 1
                    Dim ej = mol.Atoms(j).Element
                    Dim rj As Double = 0
                    If Not CovalentRadius.TryGetValue(ej, rj) Then rj = 0.77
                    If ei = "H" OrElse ej = "H" Then Continue For
                    Dim dx = mol.Atoms(i).X - mol.Atoms(j).X
                    Dim dy = mol.Atoms(i).Y - mol.Atoms(j).Y
                    Dim dz = mol.Atoms(i).Z - mol.Atoms(j).Z
                    Dim r2 = dx * dx + dy * dy + dz * dz
                    Dim cut = 1.3 * (ri + rj)
                    If r2 < cut * cut Then
                        mol.Bonds.Add(New Bond(i, j, 1.0))
                    End If
                Next
            Next
        End Sub

        Private Function GuessElement(atomName As String, resName As String) As String
            Dim t = atomName.TrimStart(" "c)
            If t.Length = 0 Then Return "C"
            If resName = "HOH" AndAlso (t = "O" OrElse t.StartsWith("O")) Then Return "O"
            ' PDB 原子名约定：列 13-14，元素从首位（单字符元素时首位是元素+空格）
            Dim first = t(0)
            If Char.IsDigit(first) AndAlso t.Length > 1 Then first = t(1)
            Return NormalizeElement(first.ToString())
        End Function

        Private Function SafeSub(line As String, start As Integer, len As Integer) As String
            If line.Length <= start Then Return ""
            If line.Length <= start + len Then Return line.Substring(start)
            Return line.Substring(start, len)
        End Function

        ' ---------------- SDF (V2000) ----------------

        ''' <summary>读取 SDF 第一个分子（支持 M CHG 形式电荷）</summary>
        Public Function ReadSdf(path As String) As Molecule
            Dim lines = File.ReadAllLines(path)
            If lines.Length < 4 Then Throw New InvalidDataException("SDF 文件过短: " & path)
            Dim mol As New Molecule With {.Id = If(lines(0).Trim().Length > 0, lines(0).Trim(),
                                                     IO.Path.GetFileNameWithoutExtension(path))}

            Dim counts = lines(3)
            Dim nAtoms = Integer.Parse(counts.Substring(0, 3).Trim(), CultureInfo.InvariantCulture)
            Dim nBonds = Integer.Parse(counts.Substring(3, 3).Trim(), CultureInfo.InvariantCulture)

            For i = 0 To nAtoms - 1
                Dim ln = lines(4 + i)
                Dim atom As New Atom()
                Dim cx = Double.Parse(ln.Substring(0, 10).Trim(), CultureInfo.InvariantCulture)
                Dim cy = Double.Parse(ln.Substring(10, 10).Trim(), CultureInfo.InvariantCulture)
                Dim cz = Double.Parse(ln.Substring(20, 10).Trim(), CultureInfo.InvariantCulture)
                atom.X = cx : atom.Y = cy : atom.Z = cz
                atom.Element = NormalizeElement(ln.Substring(31, 3).Trim())
                atom.FromReceptor = False
                mol.Atoms.Add(atom)
            Next

            Dim baseAtom = 4 + nAtoms
            For i = 0 To nBonds - 1
                Dim ln = lines(baseAtom + i)
                Dim a = Integer.Parse(ln.Substring(0, 3).Trim(), CultureInfo.InvariantCulture) - 1
                Dim b = Integer.Parse(ln.Substring(3, 3).Trim(), CultureInfo.InvariantCulture) - 1
                Dim ordRaw = Integer.Parse(ln.Substring(6, 3).Trim(), CultureInfo.InvariantCulture)
                Dim order As Double = If(ordRaw = 4, 1.5, CDbl(ordRaw))
                mol.Bonds.Add(New Bond(a, b, order))
            Next

            ' M CHG 形式电荷
            For i = baseAtom + nBonds To lines.Length - 1
                Dim ln = lines(i)
                If ln.StartsWith("M  CHG") Then
                    Dim cnt = Integer.Parse(ln.Substring(6, 3).Trim(), CultureInfo.InvariantCulture)
                    For k = 0 To cnt - 1
                        Dim pos = 9 + k * 8
                        If pos + 7 < ln.Length + 1 AndAlso pos + 3 < ln.Length Then
                            Dim ai = Integer.Parse(ln.Substring(pos, 3).Trim(), CultureInfo.InvariantCulture) - 1
                            Dim chg = Integer.Parse(ln.Substring(pos + 4, 3).Trim(), CultureInfo.InvariantCulture)
                            If ai >= 0 AndAlso ai < mol.Atoms.Count Then
                                mol.Atoms(ai).Charge = chg
                            End If
                        End If
                    Next
                ElseIf ln.StartsWith("$$$$") Then
                    Exit For
                End If
            Next
            Return mol
        End Function

    End Module

End Namespace
