#Region "Microsoft.VisualBasic::3635fd0990a0e78420e5f6fec28361c1, data\RCSB PDB\PDB\Keywords\Headers\HetAtom\HetName.vb"

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

    '   Total Lines: 62
    '    Code Lines: 42 (67.74%)
    ' Comment Lines: 6 (9.68%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 14 (22.58%)
    '     File Size: 1.96 KB


    '     Class HetName
    ' 
    '         Properties: Keyword, Residues
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: Append, ToPDBText
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Keywords

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

        Sub New()
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(ParamArray ligands As NamedValue(Of String)())
            Call residueList.AddRange(ligands)
        End Sub

        Public Function ToPDBText() As String
            Dim sb As New StringBuilder
            For Each residue As NamedValue(Of String) In residueList
                Call sb.AppendLine($"HETNAM     {residue.Name} {residue.Value}")
            Next
            Return sb.ToString
        End Function

        ''' <summary>
        ''' append a new line of hetname record to the current hetname object
        ''' </summary>
        ''' <param name="hetname"></param>
        ''' <param name="line"></param>
        ''' <returns></returns>
        Friend Shared Function Append(ByRef hetname As HetName, line As String) As HetName
            If hetname Is Nothing Then
                hetname = New HetName
            End If

            line = Strings.LTrim(line)

            Dim residueType = Strings.Mid(line, 1, 3).Trim()
            Dim chemicalName = Strings.Mid(line, 5).Trim()
            Dim data As New NamedValue(Of String)(residueType, chemicalName, line)

            Call hetname.residueList.Add(data)

            Return hetname
        End Function

    End Class
End Namespace
