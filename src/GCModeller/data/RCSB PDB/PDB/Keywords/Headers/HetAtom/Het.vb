#Region "Microsoft.VisualBasic::253df0992cdcdf55f09b787640c9176a, data\RCSB PDB\PDB\Keywords\Headers\HetAtom\Het.vb"

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

    '   Total Lines: 88
    '    Code Lines: 46 (52.27%)
    ' Comment Lines: 24 (27.27%)
    '    - Xml Docs: 95.83%
    ' 
    '   Blank Lines: 18 (20.45%)
    '     File Size: 2.84 KB


    '     Class Het
    ' 
    '         Properties: HetAtoms, Keyword
    ' 
    '         Function: Append
    '         Class HETRecord
    ' 
    '             Properties: AtomCount, ChainID, ResidueType, SequenceNumber
    ' 
    '             Constructor: (+2 Overloads) Sub New
    '             Function: ToPdbHETLine, ToString
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

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

            Sub New()
            End Sub

            Public Overrides Function ToString() As String
                Return $"[{SequenceNumber}] {ResidueType}"
            End Function

            ''' <summary>
            ''' 生成PDB格式的HET记录行
            ''' </summary>
            ''' <returns>符合PDB格式的HET记录字符串</returns>
            Public Function ToPdbHETLine() As String
                ' 格式: "HET  残基类型 链标识符序列号 原子数量      "
                Return $"HET   {ResidueType.PadRight(3)} {ChainID.PadRight(1)}{SequenceNumber.ToString().PadLeft(4)} {AtomCount.ToString().PadLeft(4)}          "
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

            line = Strings.LTrim(line)

            Dim atom As New HETRecord(line)
            Dim record As New NamedValue(Of HETRecord)(atom.ResidueType, atom, line)

            Call het.hetList.Add(record)

            Return het
        End Function

    End Class

End Namespace
