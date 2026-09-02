' ============================================================================
' SdfIO.vb — SDF/MOL (V2000) 小分子读取
' ----------------------------------------------------------------------------
' [README §2.1 输入] 小分子代谢物 → SDF(V2000) 或 PDB(HETATM)。
' PDB 读取已合并到基础库 SMRUCC.genomics.Data.RCSB.PDB.Structures.StructureIO，
' 本文件只保留与 PDB 语义无关的 SDF 部分。
'
' SDF 的连接性由键块直接给出（order 4 = 芳香，此处归一化成 1.5）。
' ============================================================================

Imports System.Globalization
Imports System.IO
Imports SMRUCC.genomics.Data.RCSB.PDB.Structures

Namespace Core

    Public Module SdfIO

        ''' <summary>读取 SDF 第一个分子（支持 M CHG 形式电荷）</summary>
        Public Function ReadSdf(path As String) As VinaMolecule
            Dim lines = File.ReadAllLines(path)
            If lines.Length < 4 Then Throw New InvalidDataException("SDF 文件过短: " & path)
            Dim mol As New VinaMolecule With {.Id = If(lines(0).Trim().Length > 0, lines(0).Trim(),
                                                     IO.Path.GetFileNameWithoutExtension(path))}

            Dim counts = lines(3)
            Dim nAtoms = Integer.Parse(counts.Substring(0, 3).Trim(), CultureInfo.InvariantCulture)
            Dim nBonds = Integer.Parse(counts.Substring(3, 3).Trim(), CultureInfo.InvariantCulture)

            For i = 0 To nAtoms - 1
                Dim ln = lines(4 + i)
                Dim atom As New VinaAtom()
                Dim cx = Double.Parse(ln.Substring(0, 10).Trim(), CultureInfo.InvariantCulture)
                Dim cy = Double.Parse(ln.Substring(10, 10).Trim(), CultureInfo.InvariantCulture)
                Dim cz = Double.Parse(ln.Substring(20, 10).Trim(), CultureInfo.InvariantCulture)
                atom.X = cx : atom.Y = cy : atom.Z = cz
                atom.Element = PdbLineParser.NormalizeElement(ln.Substring(31, 3).Trim())
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
