' ============================================================================
' PdbLineParser.vb — ATOM/HETATM 固定列解析器（唯一入口）
' ----------------------------------------------------------------------------
' PDB 是固定列文本格式，元素/坐标必须按列偏移截取，不能用空白分词：
' 当链标识符（列 22）为空、或占据率与温度因子粘连时，分词会导致列整体错位。
' 本模块是全库唯一的 ATOM/HETATM 列解析实现，被两条读取路径共用：
'   1. Parser.ReadLine  → Keywords.Atom / Keywords.HETATM （完整关键字解析）
'   2. Structures.StructureIO.ReadPdbFrames → Molecule(Of T) （轻量逐行扫描）
'
' 列偏移（0 基，参考 PDB v3.30 格式规范）：
'   1-6   记录名   7-11 原子序号  13-16 原子名   17   交替位置
'   18-20 残基名   22    链标识   23-26 残基序号  27   插入码
'   31-38 X        39-46 Y        47-54 Z        55-60 占据率
'   61-66 温度因子 77-78 元素符号 79-80 电荷
' ============================================================================

Imports System.Globalization

Namespace Structures

    ''' <summary>
    ''' The fixed-column parser of the PDB ``ATOM``/``HETATM`` records.
    ''' </summary>
    Public Module PdbLineParser

        ' 列偏移常量（0 基）
        Private Const ColSerial As Integer = 6
        Private Const ColAtomName As Integer = 12
        Private Const ColAltLoc As Integer = 16
        Private Const ColResName As Integer = 17
        Private Const ColChainID As Integer = 21
        Private Const ColResSeq As Integer = 22
        Private Const ColX As Integer = 30
        Private Const ColY As Integer = 38
        Private Const ColZ As Integer = 46
        Private Const ColOccupancy As Integer = 54
        Private Const ColTempFactor As Integer = 60
        Private Const ColElement As Integer = 76

        ''' <summary>
        ''' Parse one raw ``ATOM``/``HETATM`` line into the given atom instance.
        ''' </summary>
        ''' <typeparam name="T">The atom model type, derived from <see cref="Atom"/>.</typeparam>
        ''' <param name="rawLine">
        ''' The **raw** record line, the record name prefix must be kept intact, otherwise the
        ''' column offsets will be broken.
        ''' </param>
        ''' <param name="atom">The atom instance that accepts the parsed field values.</param>
        ''' <param name="isHet">
        ''' Is current line comes from a ``HETATM`` record? (False means the ``ATOM`` record)
        ''' </param>
        ''' <returns>
        ''' Returns True if the coordinate columns are parsed successfully; returns False if the
        ''' line is too short or the coordinates are not valid numbers -- the caller should skip
        ''' such a line instead of appending an atom at the origin point.
        ''' </returns>
        ''' <remarks>
        ''' The alternate location indicator is **not** filtered here, the caller decides whether
        ''' to keep the alternate conformations via <see cref="AcceptAltLoc"/>.
        ''' </remarks>
        Public Function ParseLine(Of T As Atom)(rawLine As String, atom As T, isHet As Boolean) As Boolean
            If rawLine Is Nothing OrElse atom Is Nothing Then
                Return False
            End If

            atom.AtomName = SafeSub(rawLine, ColAtomName, 4).Trim()
            atom.AltLoc = SafeSub(rawLine, ColAltLoc, 1).Trim()
            atom.ResName = SafeSub(rawLine, ColResName, 3).Trim().ToUpperInvariant()
            atom.ChainID = SafeSub(rawLine, ColChainID, 1).Trim()

            If atom.ChainID.Length = 0 Then
                atom.ChainID = " "
            End If

            Dim serial As Integer = 0
            If Integer.TryParse(SafeSub(rawLine, ColSerial, 5).Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, serial) Then
                atom.Serial = serial
            End If

            Dim resSeq As Integer = 0
            If Integer.TryParse(SafeSub(rawLine, ColResSeq, 4).Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, resSeq) Then
                atom.ResSeq = resSeq
            End If

            Dim cx = SafeSub(rawLine, ColX, 8).Trim()
            Dim cy = SafeSub(rawLine, ColY, 8).Trim()
            Dim cz = SafeSub(rawLine, ColZ, 8).Trim()

            Dim x As Double, y As Double, z As Double

            If Not Double.TryParse(cx, NumberStyles.Float, CultureInfo.InvariantCulture, x) Then Return False
            If Not Double.TryParse(cy, NumberStyles.Float, CultureInfo.InvariantCulture, y) Then Return False
            If Not Double.TryParse(cz, NumberStyles.Float, CultureInfo.InvariantCulture, z) Then Return False

            atom.X = x
            atom.Y = y
            atom.Z = z

            Dim occupancy As Double = 1.0
            If Double.TryParse(SafeSub(rawLine, ColOccupancy, 6).Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, occupancy) Then
                atom.Occupancy = occupancy
            End If

            Dim tempFactor As Double = 0.0
            If Double.TryParse(SafeSub(rawLine, ColTempFactor, 6).Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, tempFactor) Then
                atom.TempFactor = tempFactor
            End If

            ' 元素符号：优先取列 77-78；该列缺失时由原子名/残基名推测
            Dim elemRaw = SafeSub(rawLine, ColElement, 2)

            If elemRaw.Trim().Length > 0 Then
                atom.Element = NormalizeElement(elemRaw)
            Else
                atom.Element = GuessElement(atom.AtomName, atom.ResName)
            End If

            atom.IsHet = isHet
            atom.IsWater = (atom.ResName = "HOH" OrElse atom.ResName = "WAT")

            Return True
        End Function

        ''' <summary>
        ''' Is the given alternate location indicator accepted?
        ''' </summary>
        ''' <param name="altLoc">The alternate location indicator (PDB column 17).</param>
        ''' <returns>
        ''' Only the empty indicator and the primary conformation ``A`` are accepted.
        ''' </returns>
        Public Function AcceptAltLoc(altLoc As String) As Boolean
            Return altLoc Is Nothing OrElse
                   altLoc.Length = 0 OrElse
                   altLoc = " " OrElse
                   altLoc = "A"
        End Function

        ''' <summary>
        ''' Normalize the raw element symbol text into the canonical form.
        ''' </summary>
        ''' <param name="raw">The raw element symbol text, e.g. ``C``/``cl``/``FE``.</param>
        ''' <returns>
        ''' The canonical element symbol: the first character is in upper case and the second
        ''' character (if it is a two-char element) is normalized to upper case too.
        ''' </returns>
        ''' <remarks>
        ''' NOTE: the two-char element detection requires the second character to be a **lower**
        ''' case letter, so the all-upper-case PDB element column value (e.g. ``CL``, ``BR``)
        ''' falls back to the single char element. This behaviour is kept identical to the
        ''' original implementation to avoid changing the downstream atom typing result.
        ''' </remarks>
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

        ''' <summary>
        ''' Guess the element symbol from the atom name when the element column is missing.
        ''' </summary>
        ''' <param name="atomName">The atom name (PDB column 13-16).</param>
        ''' <param name="resName">The residue name (PDB column 18-20).</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' PDB 原子名约定：列 13-14，元素从首位（单字符元素时首位是元素+空格）
        ''' </remarks>
        Public Function GuessElement(atomName As String, resName As String) As String
            Dim t = atomName.TrimStart(" "c)
            If t.Length = 0 Then Return "C"
            If resName = "HOH" AndAlso (t = "O" OrElse t.StartsWith("O")) Then Return "O"
            ' PDB 原子名约定：列 13-14，元素从首位（单字符元素时首位是元素+空格）
            Dim first = t(0)
            If Char.IsDigit(first) AndAlso t.Length > 1 Then first = t(1)
            Return NormalizeElement(first.ToString())
        End Function

        ''' <summary>
        ''' Take a sub string from the fixed-column text line in a safe way: the short lines
        ''' never throw an <see cref="ArgumentOutOfRangeException"/>.
        ''' </summary>
        ''' <param name="line">The raw record line.</param>
        ''' <param name="start">The zero based column offset.</param>
        ''' <param name="len">The expected field width.</param>
        ''' <returns></returns>
        Public Function SafeSub(line As String, start As Integer, len As Integer) As String
            If line.Length <= start Then Return ""
            If line.Length <= start + len Then Return line.Substring(start)
            Return line.Substring(start, len)
        End Function

    End Module
End Namespace
