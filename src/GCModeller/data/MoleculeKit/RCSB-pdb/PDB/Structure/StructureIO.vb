' ============================================================================
' StructureIO.vb — PDB 结构文件的轻量读取入口
' ----------------------------------------------------------------------------
' 与 PDB.Load/Parser.Load 的完整关键字解析路径不同，本模块只做逐行扫描：
'   - 不解析 HEADER/TITLE/SEQRES 等元数据，也不要求文件以 END 结束；
'   - 遇到不认识的记录行直接跳过，不会因为未知关键字抛异常；
'   - 支持 MODEL/ENDMDL 多模型（NMR 结构与对接轨迹）。
' 因此适合作为计算型流水线（分子对接/MM-GBSA）的输入读取器。
'
' 连接性：PDB 不显式给出小分子与 HETATM 的键连，按共价半径距离感知
'   （r &lt; 1.3·(r_cov_i + r_cov_j)，见 PerceiveBonds）。
' ============================================================================

Imports System.IO

Namespace Structures

    ''' <summary>
    ''' The lightweight PDB structure file reader.
    ''' </summary>
    Public Module StructureIO

        ''' <summary>
        ''' Read the first ``MODEL`` frame of the given PDB file.
        ''' </summary>
        ''' <typeparam name="T">The atom model type, derived from <see cref="Atom"/>.</typeparam>
        ''' <param name="path">The file path of the PDB structure file.</param>
        ''' <returns></returns>
        Public Function ReadPdb(Of T As {Atom, New})(path As String) As Molecule(Of T)
            Return ReadPdb(Of T, Molecule(Of T))(path)
        End Function

        ''' <summary>
        ''' Read the first ``MODEL`` frame of the given PDB file into a caller supplied
        ''' container type.
        ''' </summary>
        ''' <typeparam name="T">The atom model type, derived from <see cref="Atom"/>.</typeparam>
        ''' <typeparam name="TMol">
        ''' The molecule container type, derived from <see cref="Molecule(Of T)"/>. Naming the
        ''' concrete container type lets the caller keep a strongly typed subclass
        ''' (e.g. the AutoDock Vina molecule) without any down-casting.
        ''' </typeparam>
        ''' <param name="path">The file path of the PDB structure file.</param>
        ''' <returns></returns>
        Public Function ReadPdb(Of T As {Atom, New}, TMol As {Molecule(Of T), New})(path As String) As TMol
            Dim frames = ReadPdbFrames(Of T, TMol)(path)

            If frames.Count = 0 Then Throw New InvalidDataException("PDB 无 ATOM/HETATM 记录: " & path)

            Return frames(0)
        End Function

        ''' <summary>
        ''' Read all of the ``MODEL`` frames of the given PDB file, each frame is returned as
        ''' an individual molecule object.
        ''' </summary>
        ''' <typeparam name="T">The atom model type, derived from <see cref="Atom"/>.</typeparam>
        ''' <param name="path">The file path of the PDB structure file.</param>
        ''' <returns></returns>
        Public Function ReadPdbFrames(Of T As {Atom, New})(path As String) As List(Of Molecule(Of T))
            Return ReadPdbFrames(Of T, Molecule(Of T))(path)
        End Function

        ''' <summary>
        ''' Read all of the ``MODEL`` frames of the given PDB file into a caller supplied
        ''' container type.
        ''' </summary>
        ''' <typeparam name="T">The atom model type, derived from <see cref="Atom"/>.</typeparam>
        ''' <typeparam name="TMol">The molecule container type, derived from <see cref="Molecule(Of T)"/>.</typeparam>
        ''' <param name="path">The file path of the PDB structure file.</param>
        ''' <returns></returns>
        Public Function ReadPdbFrames(Of T As {Atom, New}, TMol As {Molecule(Of T), New})(path As String) As List(Of TMol)
            Dim frames As New List(Of TMol)()
            ' 注意：VB 标识符不区分大小写，此处必须写全限定名，
            ' 否则 Path 会绑定到同名的 path 参数上。
            Dim mol As New TMol() With {
                .Id = System.IO.Path.GetFileNameWithoutExtension(path)
            }
            Dim inModel As Boolean = False
            Dim firstModelDone = False

            For Each line In File.ReadLines(path)
                Dim rec = If(line.Length >= 6, line.Substring(0, 6).TrimEnd(), line.TrimEnd())

                If rec = "ENDMDL" Then
                    If mol.Atoms.Count > 0 Then frames.Add(mol)
                    inModel = False
                    firstModelDone = True
                    mol = New TMol() With {.Id = mol.Id}
                    Continue For
                End If
                If rec = "MODEL" Then
                    inModel = True
                    Continue For
                End If
                If rec <> "ATOM" AndAlso rec <> "HETATM" Then Continue For
                If inModel = False AndAlso firstModelDone Then Continue For   ' 只取第一模型

                Dim atom As New T()

                If Not PdbLineParser.ParseLine(line, atom, isHet:=rec = "HETATM") Then
                    Continue For
                End If
                If Not PdbLineParser.AcceptAltLoc(atom.AltLoc) Then
                    Continue For
                End If

                mol.Atoms.Add(atom)
            Next

            If frames.Count = 0 OrElse mol.Atoms.Count > 0 Then
                If mol.Atoms.Count > 0 Then frames.Add(mol)
            End If

            Return frames
        End Function

        ''' <summary>
        ''' Perceive the covalent connectivity of the given molecule by the distance criterion:
        ''' two heavy atoms are bonded when ``r &lt; 1.3 * (r_cov_i + r_cov_j)``.
        ''' </summary>
        ''' <typeparam name="T">The atom model type, derived from <see cref="Atom"/>.</typeparam>
        ''' <param name="mol">The molecule object that will be modified in place.</param>
        ''' <remarks>
        ''' The covalent radii come from the <see cref="CovalentRadii"/> table of this library,
        ''' which covers 118 elements. The ``1.3`` tolerance factor is kept from the original
        ''' implementation on purpose: switching to the ``theoretical bond length +/- tolerance``
        ''' criterion of <see cref="CovalentRadii.MeasureBonds"/> would introduce the bond order
        ''' 2/3 into the result and then change the downstream rotatable bond detection.
        ''' </remarks>
        Public Sub PerceiveBonds(Of T As {Atom, New})(mol As Molecule(Of T))
            mol.Bonds.Clear()

            Dim n = mol.Atoms.Count

            For i = 0 To n - 1
                Dim ei = mol.Atoms(i).Element
                Dim ri As Double = CovalentRadii.SingleBondRadius(ei)

                For j = i + 1 To n - 1
                    Dim ej = mol.Atoms(j).Element
                    Dim rj As Double = CovalentRadii.SingleBondRadius(ej)

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

    End Module
End Namespace
