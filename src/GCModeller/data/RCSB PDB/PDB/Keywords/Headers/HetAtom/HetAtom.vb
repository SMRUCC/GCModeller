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
        End Class

        ReadOnly atomList As New Dictionary(Of String, List(Of HETATMRecord))

        Default Public ReadOnly Property Molecule(key As String) As HETATMRecord()
            Get
                Return atomList.TryGetValue(key).SafeQuery.ToArray
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