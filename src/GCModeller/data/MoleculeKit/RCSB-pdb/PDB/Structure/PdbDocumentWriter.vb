' ============================================================================
' PdbDocumentWriter.vb — PDB 固定列文本写出器（唯一的 ATOM/HETATM 列格式化入口）
' ----------------------------------------------------------------------------
' 与 PdbLineParser 互为正反面：解析器按列切片读取，本模块按列填充写出。
' 列偏移遵循 PDB v3.30：
'   1-6   记录名     7-11 原子序号   13-16 原子名  17   交替位置
'   18-20 残基名     22    链标识     23-26 残基序号
'   31-38 X          39-46 Y        47-54 Z       55-60 占据率
'   61-66 温度因子   77-78 元素符号   79-80 电荷
' 由于 CONECT 记录无法表达键级，键级会写入 REMARK 901（`a-b:order`），
' 结构式原文写入 REMARK 900，下游可据此无损还原分子图。
' ============================================================================

Imports System.Globalization
Imports System.IO
Imports System.Text

Namespace Structures

    ''' <summary>
    ''' The PDB (v3.30) fixed-column document writer.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' This module is the write-side counterpart of <see cref="PdbLineParser"/>: the parser
    ''' slices the fixed columns, this writer fills them. Both of them must be changed together
    ''' when a column offset is adjusted.
    ''' </para>
    ''' <para>
    ''' The writer only depends on <see cref="Atom"/> and <see cref="Bond"/>, so it works for any
    ''' molecule container derived from <see cref="Molecule(Of T)"/> -- for example the AutoDock
    ''' Vina atom model that carries extra atom typing fields.
    ''' </para>
    ''' <para>
    ''' A ``CONECT`` record only carries the connectivity and never the bond order, so the bond
    ''' orders are emitted as ``REMARK 901`` records, and the original structure string (SMILES /
    ''' InChI) is emitted as ``REMARK 900``.
    ''' </para>
    ''' <code>
    ''' Dim mol As Molecule(Of Atom) = ...
    ''' Call PdbDocumentWriter.Save(mol, "./ligand.pdb", "phenol", "c1ccccc1O")
    ''' </code>
    ''' </remarks>
    Public Module PdbDocumentWriter

        ''' <summary>
        ''' The standard PDB record width in characters.
        ''' </summary>
        Public Const RECORD_WIDTH As Integer = 80

        ''' <summary>
        ''' The maximum number of bonded atoms that a single ``CONECT`` record can hold.
        ''' </summary>
        Private Const CONECT_CAPACITY As Integer = 4

        ''' <summary>
        ''' Format the given molecule object as a complete PDB document text.
        ''' </summary>
        ''' <typeparam name="T">The atom model type, derived from <see cref="Atom"/>.</typeparam>
        ''' <param name="mol">The molecule object that will be formatted.</param>
        ''' <param name="title">
        ''' The structure title, written as the ``TITLE`` record; the <see cref="Molecule(Of T).Id"/>
        ''' value is used when this parameter is empty.
        ''' </param>
        ''' <param name="sourceText">
        ''' The original structure string (SMILES/InChI), written as ``REMARK 900`` records.
        ''' </param>
        ''' <returns>The PDB document text, every line ends with the platform new line.</returns>
        Public Function Compose(Of T As {Atom, New})(mol As Molecule(Of T),
                                                     Optional title As String = Nothing,
                                                     Optional sourceText As String = Nothing) As String
            If mol Is Nothing Then Throw New ArgumentNullException(NameOf(mol))

            Dim text As New StringBuilder()

            Call text.AppendLine(headerLine(mol))
            Call appendLines(text, titleLines(If(String.IsNullOrEmpty(title), mol.Id, title)))

            If Not String.IsNullOrEmpty(sourceText) Then
                Call appendLines(text, remarkLines(900, "SOURCE STRUCTURE", sourceText))
            End If

            Call appendLines(text, bondOrderRemarks(mol))
            Call appendLines(text, hetnamLines(mol))

            For Each atom As Atom In mol.Atoms
                Call text.AppendLine(atomLine(atom))
            Next

            Call appendLines(text, conectLines(mol))
            Call text.AppendLine("END")

            Return text.ToString()
        End Function

        ''' <summary>
        ''' Format the given molecule object and write it to a PDB file.
        ''' </summary>
        ''' <typeparam name="T">The atom model type, derived from <see cref="Atom"/>.</typeparam>
        ''' <param name="mol">The molecule object that will be written.</param>
        ''' <param name="path">The output file path, the parent folder is created when missing.</param>
        ''' <param name="title">The structure title.</param>
        ''' <param name="sourceText">The original structure string.</param>
        ''' <returns>The full path of the written file.</returns>
        Public Function Save(Of T As {Atom, New})(mol As Molecule(Of T), path As String,
                                                  Optional title As String = Nothing,
                                                  Optional sourceText As String = Nothing) As String

            ' 注意：VB 标识符不区分大小写，此处必须写全限定名，
            ' 否则 Path 会绑定到同名的 path 参数上。
            Dim fullPath As String = System.IO.Path.GetFullPath(path)
            Dim folder As String = System.IO.Path.GetDirectoryName(fullPath)

            If Not String.IsNullOrEmpty(folder) AndAlso Not Directory.Exists(folder) Then
                Call Directory.CreateDirectory(folder)
            End If

            Call File.WriteAllText(fullPath, Compose(mol, title, sourceText), New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False))

            Return fullPath
        End Function

#Region "记录的列格式化"

        Private Function headerLine(Of T As {Atom, New})(mol As Molecule(Of T)) As String
            Dim line As New StringBuilder(New String(" "c, RECORD_WIDTH))

            Call setField(line, 0, "HEADER")
            Call setField(line, 10, "GENERATED MOLECULE STRUCTURE")
            Call setField(line, 50, DateTime.Now.ToString("dd-MMM-yy", CultureInfo.InvariantCulture).ToUpperInvariant())
            Call setField(line, 62, idCode(mol))

            Return line.ToString().TrimEnd()
        End Function

        Private Function titleLines(title As String) As IEnumerable(Of String)
            Dim lines As New List(Of String)
            Dim text As String = If(title, "").Trim()

            If text.Length = 0 Then Return lines

            Const WIDTH As Integer = 70

            Dim page As Integer = 1
            Dim cursor As Integer = 0

            While cursor < text.Length
                Dim length As Integer = Math.Min(WIDTH, text.Length - cursor)
                Dim line As New StringBuilder(New String(" "c, RECORD_WIDTH))

                Call setField(line, 0, "TITLE")
                Call setField(line, 8, page.ToString().PadLeft(2))

                If page > 1 Then
                    Call setField(line, 10, " ")
                    Call setField(line, 11, text.Substring(cursor, length))
                Else
                    Call setField(line, 10, text.Substring(cursor, length))
                End If

                lines.Add(line.ToString().TrimEnd())
                cursor += length
                page += 1
            End While

            Return lines
        End Function

        Private Function hetnamLines(Of T As {Atom, New})(mol As Molecule(Of T)) As IEnumerable(Of String)
            Dim lines As New List(Of String)

            If mol.Atoms.Count = 0 Then Return lines

            Dim line As New StringBuilder(New String(" "c, RECORD_WIDTH))

            Call setField(line, 0, "HETNAM")
            Call setField(line, 8, "  ")
            Call setField(line, 11, mol.Atoms(0).ResName)
            Call setField(line, 15, If(String.IsNullOrEmpty(mol.Id), "", mol.Id))

            lines.Add(line.ToString().TrimEnd())

            Return lines
        End Function

        Private Function atomLine(atom As Atom) As String
            Dim line As New StringBuilder(New String(" "c, RECORD_WIDTH))

            Call setField(line, 0, If(atom.IsHet, "HETATM", "ATOM  "))
            Call setRight(line, 6, 5, atom.Serial.ToString(CultureInfo.InvariantCulture))
            Call setField(line, 12, formatAtomName(atom))
            Call setField(line, 16, If(String.IsNullOrEmpty(atom.AltLoc), " ", atom.AltLoc.Substring(0, 1)))
            Call setField(line, 17, atom.ResName)
            Call setField(line, 21, If(String.IsNullOrEmpty(atom.ChainID), " ", atom.ChainID.Substring(0, 1)))
            Call setRight(line, 22, 4, atom.ResSeq.ToString(CultureInfo.InvariantCulture))
            Call setNumber(line, 30, 8, 3, atom.X)
            Call setNumber(line, 38, 8, 3, atom.Y)
            Call setNumber(line, 46, 8, 3, atom.Z)
            Call setNumber(line, 54, 6, 2, atom.Occupancy)
            Call setNumber(line, 60, 6, 2, atom.TempFactor)
            Call setRight(line, 76, 2, If(String.IsNullOrEmpty(atom.Element), "C", atom.Element))
            Call setField(line, 78, chargeField(atom.Charge))

            Return line.ToString().TrimEnd()
        End Function

        ''' <summary>
        ''' 生成 <c>CONECT</c> 记录：每个原子一行，列出与它相连的原子序号（每行最多 4 个）。
        ''' </summary>
        Private Function conectLines(Of T As {Atom, New})(mol As Molecule(Of T)) As IEnumerable(Of String)
            Dim lines As New List(Of String)
            Dim partners As New SortedDictionary(Of Integer, List(Of Integer))

            For Each bond As Bond In mol.Bonds
                If bond.A < 0 OrElse bond.A >= mol.Atoms.Count Then Continue For
                If bond.B < 0 OrElse bond.B >= mol.Atoms.Count Then Continue For

                Dim serialA As Integer = mol.Atoms(bond.A).Serial
                Dim serialB As Integer = mol.Atoms(bond.B).Serial

                Call link(partners, serialA, serialB)
                Call link(partners, serialB, serialA)
            Next

            For Each serial As Integer In partners.Keys
                Dim list As List(Of Integer) = partners(serial)
                Dim cursor As Integer = 0

                Do
                    Dim line As New StringBuilder(New String(" "c, RECORD_WIDTH))

                    Call setField(line, 0, "CONECT")
                    Call setRight(line, 6, 5, serial.ToString(CultureInfo.InvariantCulture))

                    For k = 0 To CONECT_CAPACITY - 1
                        If cursor >= list.Count Then Exit For

                        Call setRight(line, 11 + k * 5, 5, list(cursor).ToString(CultureInfo.InvariantCulture))
                        cursor += 1
                    Next

                    lines.Add(line.ToString().TrimEnd())
                Loop While cursor < list.Count
            Next

            Return lines
        End Function

        Private Sub link(store As SortedDictionary(Of Integer, List(Of Integer)), serial As Integer, partner As Integer)
            Dim list As List(Of Integer) = Nothing

            If Not store.TryGetValue(serial, list) Then
                list = New List(Of Integer)
                store(serial) = list
            End If

            If Not list.Contains(partner) Then list.Add(partner)
        End Sub

        ''' <summary>
        ''' 把键级写入 <c>REMARK 901</c>：每行列出若干 <c>a-b:order</c> 片段。
        ''' </summary>
        Private Function bondOrderRemarks(Of T As {Atom, New})(mol As Molecule(Of T)) As IEnumerable(Of String)
            Dim lines As New List(Of String)

            If mol.Bonds.Count = 0 Then Return lines

            Dim buffer As New StringBuilder()
            Dim started As Boolean = False

            For Each bond As Bond In mol.Bonds
                Dim serialA As Integer = mol.Atoms(bond.A).Serial
                Dim serialB As Integer = mol.Atoms(bond.B).Serial
                Dim order As String = bond.Order.ToString("0.##", CultureInfo.InvariantCulture)
                Dim token As String = $"{serialA}-{serialB}:{order}  "

                If started AndAlso buffer.Length + token.Length > 62 Then
                    lines.Add(remarkLine(901, buffer.ToString().TrimEnd()))
                    buffer.Clear()
                End If

                Call buffer.Append(token)
                started = True
            Next

            If buffer.Length > 0 Then
                lines.Add(remarkLine(901, buffer.ToString().TrimEnd()))
            End If

            Return lines
        End Function

        Private Function remarkLines(number As Integer, caption As String, value As String) As IEnumerable(Of String)
            Dim lines As New List(Of String)
            Dim text As String = If(value, "").Replace(vbCrLf, " ").Replace(vbLf, " ").Replace(vbCr, " ").Trim()
            Dim header As String = $"{caption}: "
            Dim cursor As Integer = 0

            Do
                Dim capacity As Integer = 80 - 11 - header.Length

                If capacity < 10 Then capacity = 10

                Dim length As Integer = Math.Min(capacity, text.Length - cursor)
                Dim segment As String = text.Substring(cursor, length)

                lines.Add(remarkLine(number, header & segment))

                cursor += length
                header = New String(" "c, header.Length)
            Loop While cursor < text.Length

            Return lines
        End Function

        Private Function remarkLine(number As Integer, value As String) As String
            Dim line As New StringBuilder(New String(" "c, RECORD_WIDTH))

            Call setField(line, 0, "REMARK")
            Call setField(line, 7, number.ToString().PadLeft(3))
            Call setField(line, 11, value)

            Return line.ToString().TrimEnd()
        End Function

        ''' <summary>
        ''' 生成 PDB 风格的原子名（列 13-16）：单字符元素的原子名从第 14 列开始，
        ''' 双字符元素从第 13 列开始，这与 <see cref="PdbLineParser.GuessElement"/> 的约定一致。
        ''' </summary>
        Private Function formatAtomName(atom As Atom) As String
            Dim name As String = If(String.IsNullOrEmpty(atom.AtomName), atom.Element, atom.AtomName).ToUpperInvariant()

            If name.Length > 4 Then name = name.Substring(0, 4)

            If atom.Element.Length = 1 AndAlso name.Length < 4 Then
                Return (" " & name).PadRight(4)
            End If

            Return name.PadRight(4)
        End Function

        ''' <summary>
        ''' 形式电荷 → PDB 列 79-80 的 <c>1+</c>/<c>2-</c> 记法。
        ''' </summary>
        Private Function chargeField(charge As Double) As String
            Dim value As Integer = CInt(Math.Round(charge))

            If value = 0 Then Return "  "

            Dim magnitude As Integer = Math.Min(Math.Abs(value), 9)

            Return $"{magnitude}{If(value > 0, "+", "-")}"
        End Function

        ''' <summary>
        ''' 生成 4 位的 PDB ID 代码；Id 过长（例如分子式）时回退到固定的 <c>MOL1</c>，
        ''' 避免写出被截断而看起来像另一个分子式的代码。
        ''' </summary>
        Private Function idCode(Of T As {Atom, New})(mol As Molecule(Of T)) As String
            Dim clean As New StringBuilder()

            For Each c As Char In If(mol.Id, "").ToUpperInvariant()
                If Char.IsLetterOrDigit(c) Then Call clean.Append(c)
            Next

            If clean.Length = 0 OrElse clean.Length > 4 Then Return "MOL1"

            Return clean.ToString().PadRight(4)
        End Function

        Private Sub appendLines(text As StringBuilder, lines As IEnumerable(Of String))
            For Each line As String In lines
                Call text.AppendLine(line)
            Next
        End Sub

        Private Sub setField(line As StringBuilder, start As Integer, value As String)
            If String.IsNullOrEmpty(value) Then Return

            For i = 0 To value.Length - 1
                If start + i < 0 Then Continue For
                If start + i >= line.Length Then Exit For

                line(start + i) = value(i)
            Next
        End Sub

        Private Sub setRight(line As StringBuilder, start As Integer, width As Integer, value As String)
            Dim text As String = If(value, "")

            If text.Length > width Then text = text.Substring(text.Length - width)

            Call setField(line, start + width - text.Length, text)
        End Sub

        Private Sub setNumber(line As StringBuilder, start As Integer, width As Integer, decimals As Integer, value As Double)
            Dim text As String = value.ToString("F" & decimals, CultureInfo.InvariantCulture)

            If text.Length > width Then text = text.Substring(0, width)

            Call setRight(line, start, width, text)
        End Sub

#End Region

    End Module

End Namespace
