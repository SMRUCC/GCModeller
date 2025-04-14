#Region "Microsoft.VisualBasic::559b77bd15fe1e5715f80f7ea46c37c7, data\RCSB PDB\PDB\Keywords\Headers\Structure.vb"

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

    '   Total Lines: 61
    '    Code Lines: 47 (77.05%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 14 (22.95%)
    '     File Size: 1.65 KB


    '     Class Sequence
    ' 
    '         Properties: Keyword
    ' 
    '         Function: Append
    ' 
    '     Class Helix
    ' 
    '         Properties: Keyword
    ' 
    '         Function: Append
    ' 
    '     Class Sheet
    ' 
    '         Properties: Keyword
    ' 
    '         Function: Append
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Keywords

    Public Class Sequence : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_SEQRES
            End Get
        End Property

        Dim cache As New List(Of String)

        Friend Shared Function Append(ByRef res As Sequence, str As String) As Sequence
            If res Is Nothing Then
                res = New Sequence
            End If
            res.cache.Add(str)
            Return res
        End Function

    End Class

    Public Class Helix : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_HELIX
            End Get
        End Property

        Dim strs As New List(Of String)

        Friend Shared Function Append(ByRef helix As Helix, str As String) As Helix
            If helix Is Nothing Then
                helix = New Helix
            End If
            helix.strs.Add(str)
            Return helix
        End Function
    End Class

    Public Class Sheet : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_SHEET
            End Get
        End Property

        Dim strs As New List(Of String)

        Friend Shared Function Append(ByRef sheet As Sheet, str As String) As Sheet
            If sheet Is Nothing Then
                sheet = New Sheet
            End If
            sheet.strs.Add(str)
            Return sheet
        End Function
    End Class

End Namespace
