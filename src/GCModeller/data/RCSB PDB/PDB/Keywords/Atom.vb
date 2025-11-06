#Region "Microsoft.VisualBasic::94dde673cb4d5642c51ae4dbedf05050, data\RCSB PDB\PDB\Keywords\Atom.vb"

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

'   Total Lines: 60
'    Code Lines: 41 (68.33%)
' Comment Lines: 7 (11.67%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 12 (20.00%)
'     File Size: 1.96 KB


'     Class Atom
' 
'         Properties: AminoAcidSequenceData, Atoms, HetAtoms, Keyword, ModelId
' 
'         Function: Append, GetEnumerator, GetEnumerator1
' 
'         Sub: Flush
' 
' 
' /********************************************************************************/

#End Region

Namespace Keywords

    ''' <summary>
    ''' structure data model
    ''' </summary>
    Public Class Atom : Inherits Keyword
        Implements IEnumerable(Of AtomUnit)

        Public Property Atoms As AtomUnit()
        Public Property HetAtoms As HETATM

        ''' <summary>
        ''' the model id
        ''' </summary>
        ''' <returns></returns>
        Public Property ModelId As String

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_ATOM
            End Get
        End Property

        Public ReadOnly Property AminoAcidSequenceData As AminoAcid()
            Get
                Return AminoAcid.SequenceGenerator(Me)
            End Get
        End Property

        Dim cache As New List(Of (key As Integer, value As String))
        Dim ter As New List(Of Terminator)

        Friend Shared Function Append(ByRef atoms As Atom, str As String) As Atom
            If atoms Is Nothing Then
                atoms = New Atom
            End If
            Dim index = str.Trim.GetTagValue(" ", trim:=True)
            atoms.cache.Add((CInt(Val(index.Name)), index.Value))
            Return atoms
        End Function

        Friend Shared Function AppendTerminator(ByRef atoms As Atom, str As String) As Atom
            If atoms Is Nothing Then
                atoms = New Atom
            End If

            ' 确保行长度足够，不足时填充空格以避免索引越界
            Dim paddedLine As String = ("TER    " & Strings.LTrim(str)).PadRight(27) ' 至少填充到残基序号之后
            Dim record As New Terminator()

            ' 提取各字段（注意VB.NET字符串索引从0开始）
            record.RecordType = paddedLine.Substring(0, 6).Trim()          ' 列 1-6
            record.SerialNumber = Integer.Parse(paddedLine.Substring(6, 5).Trim()) ' 列 7-11
            record.ResidueName = paddedLine.Substring(17, 3).Trim()         ' 列 18-20
            record.ChainID = paddedLine(21)                                 ' 列 22 (索引21)
            record.ResidueNumber = Integer.Parse(paddedLine.Substring(22, 4).Trim()) ' 列 23-26

            atoms.ter.Add(record)

            Return atoms
        End Function

        Friend Overrides Sub Flush()
            Atoms = (From item As (key As Integer, value As String)
                     In cache.AsParallel
                     Let aa As AtomUnit = AtomUnit.InternalParser(item.value, InternalIndex:=item.key)
                     Where Not aa Is Nothing
                     Select aa).ToArray

            If Not HetAtoms Is Nothing Then
                HetAtoms.Flush()
            End If
        End Sub

        Public Iterator Function GetEnumerator() As IEnumerator(Of AtomUnit) Implements IEnumerable(Of AtomUnit).GetEnumerator
            For Each Atom As AtomUnit In Me.Atoms
                Yield Atom
            Next
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class

    ''' <summary>
    ''' The chain model terminator flag
    ''' </summary>
    Public Class Terminator : Inherits Keyword

        Public Property RecordType As String    ' 记录类型 (列 1-6)
        Public Property SerialNumber As Integer ' 序列号 (列 7-11)
        Public Property ResidueName As String   ' 残基名称 (列 18-20)
        Public Property ChainID As Char        ' 链标识符 (列 22)
        Public Property ResidueNumber As Integer ' 残基序号 (列 23-26)
        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "TER"
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"TER: Serial={SerialNumber}, ResName={ResidueName}, Chain={ChainID}, ResNum={ResidueNumber}"
        End Function
    End Class
End Namespace
