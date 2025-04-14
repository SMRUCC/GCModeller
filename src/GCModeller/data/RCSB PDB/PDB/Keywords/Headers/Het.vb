#Region "Microsoft.VisualBasic::a38debb4b680d687ebd17e3da830865d, data\RCSB PDB\PDB\Keywords\Headers\Het.vb"

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

    '   Total Lines: 25
    '    Code Lines: 17 (68.00%)
    ' Comment Lines: 3 (12.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (20.00%)
    '     File Size: 645 B


    '     Class Het
    ' 
    '         Properties: AnnotationText, Keyword
    ' 
    '         Function: Append
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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

        Public Property AnnotationText As New List(Of String)

        Friend Shared Function Append(ByRef het As Het, line As String) As Het
            If het Is Nothing Then
                het = New Het
            End If
            het.AnnotationText.Add(line)
            Return het
        End Function

    End Class

    Public Class HetName : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return KEYWORD_HETNAM
            End Get
        End Property

        Dim str As New List(Of String)

        Friend Shared Function Append(ByRef hetname As HetName, line As String) As HetName
            If hetname Is Nothing Then
                hetname = New HetName
            End If
            hetname.str.Add(line)
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
End Namespace
