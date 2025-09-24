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
