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
        Public Class HETATMRecord : Implements PointF3D

            Public Property AtomNumber As Integer   ' 原子序号
            Public Property AtomName As String      ' 原子名称
            Public Property AlternateLocation As String ' 交替位置指示符
            Public Property ResidueName As String   ' 残基名称
            Public Property ChainID As String       ' 链标识符
            Public Property ResidueSequenceNumber As Integer ' 残基序列号
            Public Property XCoord As Double Implements PointF3D.X        ' X坐标
            Public Property YCoord As Double Implements PointF3D.Y       ' Y坐标
            Public Property ZCoord As Double Implements PointF3D.Z        ' Z坐标
            Public Property Occupancy As Double     ' 占据率
            Public Property TemperatureFactor As Double ' 温度因子
            Public Property ElementSymbol As String ' 元素符号

            Sub New()
            End Sub

            ''' <summary>
            ''' copy valye from atom model data
            ''' </summary>
            ''' <param name="atom"></param>
            Sub New(atom As AtomUnit)
                With atom.Location
                    XCoord = .X
                    YCoord = .Y
                    ZCoord = .Z
                End With

                ChainID = atom.ChianID
                AtomName = atom.AA_ID
                ResidueName = atom.AA_ID
                AtomNumber = atom.Index
                ElementSymbol = atom.Atom
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

            line = "HETATM " & line

            ' 提取原子序号（第7-11列，索引6-10）
            If line.Length >= 11 Then
                Dim atomNumStr As String = line.Substring(6, 5).Trim()
                Integer.TryParse(atomNumStr, record.AtomNumber)
            End If

            ' 提取原子名称（第13-16列，索引12-15）
            If line.Length >= 16 Then
                record.AtomName = line.Substring(12, 4).Trim()
            End If

            ' 提取交替位置指示符（第17列，索引16）
            If line.Length >= 17 Then
                record.AlternateLocation = line.Substring(16, 1).Trim()
            End If

            ' 提取残基名称（第18-20列，索引17-19）
            If line.Length >= 20 Then
                record.ResidueName = line.Substring(17, 3).Trim()
            End If

            ' 提取链标识符（第22列，索引21）
            If line.Length >= 22 Then
                record.ChainID = line.Substring(21, 1).Trim()
            End If

            ' 提取残基序列号（第23-26列，索引22-25）
            If line.Length >= 26 Then
                Dim resSeqStr As String = line.Substring(22, 4).Trim()
                Integer.TryParse(resSeqStr, record.ResidueSequenceNumber)
            End If

            ' 提取X坐标（第31-38列，索引30-37）
            If line.Length >= 38 Then
                Dim xCoordStr As String = line.Substring(30, 8).Trim()
                Double.TryParse(xCoordStr, record.XCoord)
            End If

            ' 提取Y坐标（第39-46列，索引38-45）
            If line.Length >= 46 Then
                Dim yCoordStr As String = line.Substring(38, 8).Trim()
                Double.TryParse(yCoordStr, record.YCoord)
            End If

            ' 提取Z坐标（第47-54列，索引46-53）
            If line.Length >= 54 Then
                Dim zCoordStr As String = line.Substring(46, 8).Trim()
                Double.TryParse(zCoordStr, record.ZCoord)
            End If

            ' 提取占据率（第55-60列，索引54-59）
            If line.Length >= 60 Then
                Dim occupancyStr As String = line.Substring(54, 6).Trim()
                Double.TryParse(occupancyStr, record.Occupancy)
            End If

            ' 提取温度因子（第61-66列，索引60-65）
            If line.Length >= 66 Then
                Dim tempFactorStr As String = line.Substring(60, 6).Trim()
                Double.TryParse(tempFactorStr, record.TemperatureFactor)
            End If

            ' 提取元素符号（第77-78列，索引76-77）
            If line.Length >= 78 Then
                record.ElementSymbol = line.Substring(76, 2).Trim()
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