#Region "Microsoft.VisualBasic::b81c94aa19636b16b392f13a4a29c5ba, data\RCSB PDB\PDB\Keywords\Headers\HetAtom\HetAtom.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 247
    '    Code Lines: 152 (61.54%)
    ' Comment Lines: 41 (16.60%)
    '    - Xml Docs: 17.07%
    ' 
    '   Blank Lines: 54 (21.86%)
    '     File Size: 11.21 KB


    '     Class HETATM
    ' 
    '         Properties: Keys, Keyword
    ' 
    '         Function: Append, GenericEnumerator
    '         Class HETATMRecord
    ' 
    '             Properties: AlternateLocation, AtomName, AtomNumber, ChainID, ElementSymbol
    '                         Occupancy, ResidueName, ResidueSequenceNumber, TemperatureFactor, XCoord
    '                         YCoord, ZCoord
    ' 
    '             Constructor: (+2 Overloads) Sub New
    '             Function: FormatCoordinate, FormatOccupancyTemp, ToPdbHETATMLine, ToString
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Globalization
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq

Namespace Keywords

    Public Class HETATM : Inherits Keyword
        Implements Enumeration(Of HETATMRecord)

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return KEYWORD_HETATM
            End Get
        End Property

        ''' <summary>
        ''' 表示解析后的HETATM记录信息
        ''' </summary>
        ''' <remarks>
        ''' 与 <see cref="AtomUnit"/> 一样，所有字段存储都上移到了基类
        ''' <see cref="Structures.Atom"/>，此处仅保留 PDB 记录术语命名的兼容别名属性，
        ''' 使得 <see cref="CovalentRadii.MeasureBonds"/> 与 ``PDBQt\ComplexGenerator``
        ''' 等既有消费方无需改动。
        ''' </remarks>
        Public Class HETATMRecord : Inherits AtomUnit
            Implements PointF3D

            ''' <summary>
            ''' 原子序号（PDB 列 7-11）
            ''' </summary>
            ''' <returns></returns>
            Public Property AtomNumber As Integer
                Get
                    Return Serial
                End Get
                Set(value As Integer)
                    Serial = value
                End Set
            End Property

            ''' <summary>
            ''' 交替位置指示符（PDB 列 17）
            ''' </summary>
            ''' <returns></returns>
            Public Property AlternateLocation As String
                Get
                    Return AltLoc
                End Get
                Set(value As String)
                    AltLoc = value
                End Set
            End Property

            ''' <summary>
            ''' 残基名称（PDB 列 18-20）
            ''' </summary>
            ''' <returns></returns>
            Public Property ResidueName As String
                Get
                    Return ResName
                End Get
                Set(value As String)
                    ResName = value
                End Set
            End Property

            ''' <summary>
            ''' 残基序列号（PDB 列 23-26）
            ''' </summary>
            ''' <returns></returns>
            Public Property ResidueSequenceNumber As Integer
                Get
                    Return ResSeq
                End Get
                Set(value As Integer)
                    ResSeq = value
                End Set
            End Property

            ''' <summary>
            ''' X坐标（PDB 列 31-38）
            ''' </summary>
            ''' <returns></returns>
            Public Property XCoord As Double Implements PointF3D.X
                Get
                    Return X
                End Get
                Set(value As Double)
                    X = value
                End Set
            End Property

            ''' <summary>
            ''' Y坐标（PDB 列 39-46）
            ''' </summary>
            ''' <returns></returns>
            Public Property YCoord As Double Implements PointF3D.Y
                Get
                    Return Y
                End Get
                Set(value As Double)
                    Y = value
                End Set
            End Property

            ''' <summary>
            ''' Z坐标（PDB 列 47-54）
            ''' </summary>
            ''' <returns></returns>
            Public Property ZCoord As Double Implements PointF3D.Z
                Get
                    Return Z
                End Get
                Set(value As Double)
                    Z = value
                End Set
            End Property

            ''' <summary>
            ''' 温度因子（PDB 列 61-66）
            ''' </summary>
            ''' <returns></returns>
            Public Property TemperatureFactor As Double
                Get
                    Return TempFactor
                End Get
                Set(value As Double)
                    TempFactor = value
                End Set
            End Property

            ''' <summary>
            ''' 元素符号（PDB 列 77-78）
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks>
            ''' 该列的缺失值不再留空：<see cref="Structures.PdbLineParser"/> 会回退到
            ''' 由原子名/残基名推测的元素符号。
            ''' </remarks>
            Public Property ElementSymbol As String
                Get
                    Return Element
                End Get
                Set(value As String)
                    Element = value
                End Set
            End Property

            Sub New()
            End Sub

            ''' <summary>
            ''' copy valye from atom model data
            ''' </summary>
            ''' <param name="atom"></param>
            ''' <remarks>
            ''' 修正：旧实现把残基名（``AtomUnit.AA_ID``）同时填给了 ``AtomName`` 与
            ''' ``ResidueName``，并把原子名（``AtomUnit.Atom``）当成了元素符号，
            ''' 三者全部取错字段。
            ''' </remarks>
            Sub New(atom As AtomUnit)
                MyBase.New()

                If Not atom Is Nothing Then
                    Call CopyFrom(atom)
                End If
            End Sub

            Public Overrides Function ToString() As String
                Return $"HETATM {AtomNumber} {AtomName} {ResidueName} {ChainID} {ResidueSequenceNumber} " &
                   $"{XCoord:F3} {YCoord:F3} {ZCoord:F3} {Occupancy:F2} {TemperatureFactor:F2} {ElementSymbol}"
            End Function

            Public Function ToPdbHETATMLine() As String
                ' 确保关键数据存在，若无可提供默认值或抛出异常，此处以生成固定格式字符串为主
                ' 根据PDB格式规范[7](@ref)进行字符串格式化
                Dim sb As New Text.StringBuilder()

                ' 1. 记录类型 (1-6列)
                sb.Append("HETATM".PadRight(6)) ' 确保占满6列

                ' 2. 原子序号 (7-11列)，整数，右对齐
                sb.Append(AtomNumber.ToString().PadLeft(5).PadRight(6)) ' 占5列，右对齐，并确保与下一字段有空格(第12列)

                ' 3. 原子名称 (13-16列)，字符，左对齐
                ' 原子名称书写有特定规则：元素符号一般右对齐于13-14列[7](@ref)
                Dim paddedAtomName As String = AtomName.PadLeft(4) ' 确保原子名称占4位。例如"C" -> "   C", "FE" -> "  FE"
                sb.Append(paddedAtomName.PadRight(5)) ' 占4列，并确保与下一字段有空格(第17列)

                ' 4. 交替位置指示符 (17列)，字符，通常为空或单个字母
                Dim altLoc As String = If(String.IsNullOrEmpty(AlternateLocation), " ", AlternateLocation.Substring(0, 1))
                sb.Append(altLoc.PadRight(2)) ' 占1列，并确保与下一字段有空格(第18列)

                ' 5. 残基名称 (18-20列)，字符，右对齐？(PDB说明中为字符类型，未明确对齐方式，通常左对齐存放)
                sb.Append(ResidueName.PadRight(4)) ' 占3列，并确保与下一字段有空格(第22列)

                ' 6. 链标识符 (22列)，字符
                Dim chainId As String = If(String.IsNullOrEmpty(Me.ChainID), " ", Me.ChainID.Substring(0, 1))
                sb.Append(chainId.PadRight(2)) ' 占1列，并确保与下一字段有空格(第23列)

                ' 7. 残基序列号 (23-26列)，整数，右对齐
                sb.Append(ResidueSequenceNumber.ToString().PadLeft(4).PadRight(5)) ' 占4列，右对齐，并确保与下一字段有空格(第27列)

                ' 8. 残基插入码 (27列)，字符，通常为空
                sb.Append(" ".PadRight(5)) ' 第27列通常为空，且之后有3列空(28-30列)，这里用5个空格代表27列及之后的3列空(共4列空位)

                ' 9. X坐标 (31-38列)，浮点数，右对齐，格式为8.3（含小数点）
                sb.Append(FormatCoordinate(XCoord).PadLeft(9)) ' 占8列

                ' 10. Y坐标 (39-46列)，浮点数，右对齐，格式为8.3
                sb.Append(FormatCoordinate(YCoord).PadLeft(9)) ' 占8列

                ' 11. Z坐标 (47-54列)，浮点数，右对齐，格式为8.3
                sb.Append(FormatCoordinate(ZCoord).PadLeft(9)) ' 占8列

                ' 12. 占据率 (55-60列)，浮点数，右对齐，格式为6.2
                sb.Append(FormatOccupancyTemp(Occupancy).PadLeft(7)) ' 占6列

                ' 13. 温度因子 (61-66列)，浮点数，右对齐，格式为6.2
                sb.Append(FormatOccupancyTemp(TemperatureFactor).PadLeft(7)) ' 占6列

                ' 14. 留空 (67-72列)
                sb.Append("      ") ' 6个空格

                ' 15. 段标识符 (73-76列)，可选，您的类中未定义，留空
                sb.Append("    ") ' 4个空格

                ' 16. 元素符号 (77-78列)，字符，右对齐
                sb.Append(If(String.IsNullOrEmpty(ElementSymbol), "  ", ElementSymbol.PadLeft(2)).PadRight(3)) ' 占2列，右对齐

                ' 17. 原子电荷 (79-80列)，可选，您的类中未定义，留空
                sb.Append("  ") ' 2个空格

                Return sb.ToString()
            End Function

            ' 辅助函数：格式化坐标值（8.3格式）
            Private Function FormatCoordinate(coord As Double) As String
                Return coord.ToString("F3", CultureInfo.InvariantCulture).PadLeft(8) ' 确保总长度8位，小数点后3位
            End Function

            ' 辅助函数：格式化占据率或温度因子（6.2格式）
            Private Function FormatOccupancyTemp(value As Double) As String
                Return value.ToString("F2", CultureInfo.InvariantCulture).PadLeft(6) ' 确保总长度6位，小数点后2位
            End Function
        End Class

        ReadOnly atomList As New Dictionary(Of String, List(Of HETATMRecord))

        Default Public ReadOnly Property Molecule(key As String) As HETATMRecord()
            Get
                Return atomList.TryGetValue(key).SafeQuery.ToArray
            End Get
        End Property

        Public ReadOnly Property Keys As String()
            Get
                Return atomList.Keys.ToArray
            End Get
        End Property

        Friend Shared Function Append(ByRef hetatom As Atom, line As String) As Atom
            If hetatom Is Nothing Then
                hetatom = New Atom
            End If
            If hetatom.HetAtoms Is Nothing Then
                hetatom.HetAtoms = New HETATM
            End If

            Dim record As New HETATMRecord()

            ' 列解析复用全库唯一的固定列实现（Structures.PdbLineParser）；
            ' 入参必须是原始整行，不能先 Trim 或剥离记录名前缀。
            ' 坐标列非法时跳过该行，而不是在原点点位上补一个假原子。
            If Not Structures.PdbLineParser.ParseLine(line, record, isHet:=True) Then
                Return hetatom
            End If

            Dim key As String = $"{record.ResidueName}-{record.ResidueSequenceNumber}"

            If Not hetatom.HetAtoms.atomList.ContainsKey(key) Then
                Call hetatom.HetAtoms.atomList.Add(key, New List(Of HETATMRecord))
            End If

            Call hetatom.HetAtoms.atomList(key).Add(record)

            Return hetatom
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of HETATMRecord) Implements Enumeration(Of HETATMRecord).GenericEnumerator
            For Each tuple In atomList
                For Each atom As HETATMRecord In tuple.Value
                    Yield atom
                Next
            Next
        End Function
    End Class

End Namespace
