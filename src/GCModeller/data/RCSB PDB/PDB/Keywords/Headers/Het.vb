#Region "Microsoft.VisualBasic::6ed554e398bc2515372ef7713c015934, data\RCSB PDB\PDB\Keywords\Headers\Het.vb"

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

'   Total Lines: 347
'    Code Lines: 260 (74.93%)
' Comment Lines: 3 (0.86%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 84 (24.21%)
'     File Size: 9.18 KB


'     Class Het
' 
'         Properties: AnnotationText, Keyword
' 
'         Function: Append
' 
'     Class HetName
' 
'         Properties: Keyword
' 
'         Function: Append
' 
'     Class Formula
' 
'         Properties: Keyword
' 
'         Function: Append
' 
'     Class Link
' 
'         Properties: Keyword
' 
'         Function: Append
' 
'     Class CISPEP
' 
'         Properties: Keyword
' 
'         Function: Append
' 
'     Class HETATM
' 
'         Properties: Keyword
' 
'         Function: Append
' 
'     Class HETSYN
' 
'         Properties: Keyword
' 
'         Function: Append
' 
'     Class CONECT
' 
'         Properties: Keyword
' 
'         Function: Append
' 
'     Class MODRES
' 
'         Properties: Keyword
' 
'         Function: Append
' 
'     Class SSBOND
' 
'         Properties: Keyword
' 
'         Function: Append
' 
'     Class SPRSDE
' 
'         Properties: Keyword
' 
'         Function: Append
' 
'     Class CAVEAT
' 
'         Properties: Keyword
' 
'         Function: Append
' 
'     Class MDLTYP
' 
'         Properties: Keyword
' 
'         Function: Append
' 
'     Class ANISOU
' 
'         Properties: Keyword
' 
'         Function: Append
' 
'     Class SIGATM
' 
'         Properties: Keyword
' 
'         Function: Append
' 
'     Class SIGUIJ
' 
'         Properties: Keyword
' 
'         Function: Append
' 
'     Class SPLIT
' 
'         Properties: Keyword
' 
'         Function: Append
' 
' 
' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Keywords

    ''' <summary>
    ''' 非标准残基注释
    ''' </summary>
    Public Class Het : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return KEYWORD_HET
            End Get
        End Property

        Public Class HETRecord

            ''' <summary>
            ''' 残基类型 (FMN, MPD等)
            ''' </summary>
            ''' <returns></returns>
            Public Property ResidueType As String
            ''' <summary>
            ''' 链标识符 (A, B等)
            ''' </summary>
            ''' <returns></returns>
            Public Property ChainID As String
            ''' <summary>
            ''' 序列号
            ''' </summary>
            ''' <returns></returns>
            Public Property SequenceNumber As Integer
            ''' <summary>
            ''' 原子数量
            ''' </summary>
            ''' <returns></returns>
            Public Property AtomCount As Integer

            Sub New(line As String)
                ResidueType = Strings.Mid(line, 1, 3)
                ChainID = Strings.Mid(line, 6, 1)
                SequenceNumber = Strings.Mid(line, 8, 4).ParseInteger
                AtomCount = Strings.Mid(line, 17, 4).ParseInteger
            End Sub

            Public Overrides Function ToString() As String
                Return $"[{SequenceNumber}] {ResidueType}"
            End Function
        End Class

        ReadOnly hetList As New List(Of NamedValue(Of HETRecord))

        Public ReadOnly Property HetAtoms As NamedValue(Of HETRecord)()
            Get
                Return hetList.ToArray
            End Get
        End Property

        Friend Shared Function Append(ByRef het As Het, line As String) As Het
            If het Is Nothing Then
                het = New Het
            End If

            Dim atom As New HETRecord(line)
            Dim record As New NamedValue(Of HETRecord)(atom.ResidueType, atom, line)

            Call het.hetList.Add(record)

            Return het
        End Function

    End Class

    Public Class HetName : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return KEYWORD_HETNAM
            End Get
        End Property

        Public ReadOnly Property Residues As NamedValue(Of String)()
            Get
                Return residueList.ToArray
            End Get
        End Property

        ReadOnly residueList As New List(Of NamedValue(Of String))

        Friend Shared Function Append(ByRef hetname As HetName, line As String) As HetName
            If hetname Is Nothing Then
                hetname = New HetName
            End If

            Dim residueType = Strings.Mid(line, 1, 3).Trim()
            Dim chemicalName = Strings.Mid(line, 5).Trim()
            Dim data As New NamedValue(Of String)(residueType, chemicalName, line)

            Call hetname.residueList.Add(data)

            Return hetname
        End Function

    End Class

    Public Class Formula : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return KEYWORD_FORMUL
            End Get
        End Property

        Dim str As New List(Of String)

        Public Shared Function Append(ByRef formula As Formula, line As String) As Formula
            If formula Is Nothing Then
                formula = New Formula
            End If
            formula.str.Append(line)
            Return formula
        End Function

    End Class

    Public Class Link : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "LINK"
            End Get
        End Property

        Dim str As New List(Of String)

        Public Shared Function Append(ByRef links As Link, line As String) As Link
            If links Is Nothing Then
                links = New Link
            End If
            links.str.Append(line)
            Return links
        End Function

    End Class

    Public Class CISPEP : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "CISPEP"
            End Get
        End Property

        Dim str As New List(Of String)

        Public Shared Function Append(ByRef CISPEP As CISPEP, line As String) As CISPEP
            If CISPEP Is Nothing Then
                CISPEP = New CISPEP
            End If
            CISPEP.str.Append(line)
            Return CISPEP
        End Function
    End Class

    Public Class HETATM : Inherits Keyword

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

    End Class

    Public Class HETSYN : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "HETSYN"
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef hetatom As HETSYN, str As String) As HETSYN
            If hetatom Is Nothing Then
                hetatom = New HETSYN
            End If
            hetatom.str.Add(str)
            Return hetatom
        End Function

    End Class

    ''' <summary>
    ''' ### PDB文件中的CONECT字段用于明确指定原子间的特殊连接关系
    ''' 
    ''' CONECT记录提供了原子之间除标准共价键以外的特定连接信息，例如二硫键、配位键、氢键（在某些情况下）、盐桥，
    ''' 以及非标准残基（如辅因子、抑制剂）内部的键合关系或它们与蛋白质/核酸之间的连接。
    ''' </summary>
    Public Class CONECT : Inherits Keyword
        Implements Enumeration(Of NamedCollection(Of Integer))

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return KEYWORD_CONECT
            End Get
        End Property

        ' CONECT 1198 355 1248 1256 1266 1267 1268
        '
        ' 列 1-6: CONECT(记录类型标识)
        ' 列 7-11: 参考原子的序列号 (例如 1198)
        ' 列 12-16, 17-21, 22-26, 27-31, ...: 与该参考原子相连的最多4个原子的序列号 (例如 355, 1248, 1256, 1266)。如果连接原子超过4个，会使用多个CONECT记录来描述同一个参考原子的所有连接。

        Dim links As New List(Of NamedCollection(Of Integer))

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder

            For Each link In Me.AsEnumerable
                Call sb.AppendLine(link.name & " => " & link.value.GetJson)
            Next

            Return sb.ToString
        End Function

        Friend Shared Function Append(ByRef conect As CONECT, str As String) As CONECT
            If conect Is Nothing Then
                conect = New CONECT
            End If

            Dim tokens As String() = str.StringSplit("\s+")
            Dim ref As String = tokens(0)
            Dim linksTo As Integer() = tokens.Skip(1).AsInteger

            Call conect.links.Add(New NamedCollection(Of Integer)(ref, linksTo))

            Return conect
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of NamedCollection(Of Integer)) Implements Enumeration(Of NamedCollection(Of Integer)).GenericEnumerator
            For Each link As NamedCollection(Of Integer) In links
                Yield link
            Next
        End Function
    End Class

    Public Class MODRES : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "MODRES"
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef word As MODRES, str As String) As MODRES
            If word Is Nothing Then
                word = New MODRES
            End If
            word.str.Add(str)
            Return word
        End Function

    End Class

    Public Class SSBOND : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "SSBOND"
            End Get
        End Property

        Dim str As New List(Of String)

        Public Shared Function Append(ByRef bond As SSBOND, str As String) As SSBOND
            If bond Is Nothing Then
                bond = New SSBOND
            End If
            bond.str.Add(str)
            Return bond
        End Function

    End Class

    Public Class SPRSDE : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "SPRSDE"
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef sp As SPRSDE, str As String) As SPRSDE
            If sp Is Nothing Then
                sp = New SPRSDE
            End If
            sp.str.Add(str)
            Return sp
        End Function

    End Class

    Public Class CAVEAT : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "CAVEAT"
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef cav As CAVEAT, str As String) As CAVEAT
            If cav Is Nothing Then
                cav = New CAVEAT
            End If
            cav.str.Add(str)
            Return cav
        End Function

    End Class

    Public Class MDLTYP : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "MDLTYP"
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef type As MDLTYP, str As String) As MDLTYP
            If type Is Nothing Then
                type = New MDLTYP
            End If
            type.str.Add(str)
            Return type
        End Function

    End Class

    Public Class ANISOU : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "ANISOU"
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef ani As ANISOU, str As String) As ANISOU
            If ani Is Nothing Then
                ani = New ANISOU
            End If
            ani.str.Add(str)
            Return ani
        End Function

    End Class

    Public Class SIGATM : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "SIGATM"
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef sig As SIGATM, str As String) As SIGATM
            If sig Is Nothing Then
                sig = New SIGATM
            End If
            sig.str.Add(str)
            Return sig
        End Function

    End Class

    Public Class SIGUIJ : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "SIGUIJ"
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef sig As SIGUIJ, str As String) As SIGUIJ
            If sig Is Nothing Then
                sig = New SIGUIJ
            End If
            sig.str.Add(str)
            Return sig
        End Function

    End Class

    Public Class SPLIT : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "SPLIT"
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef split As SPLIT, str As String) As SPLIT
            If split Is Nothing Then
                split = New SPLIT
            End If
            split.str.Add(str)
            Return split
        End Function

    End Class
End Namespace
