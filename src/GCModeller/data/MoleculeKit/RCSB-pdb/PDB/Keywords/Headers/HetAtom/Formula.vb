#Region "Microsoft.VisualBasic::e5337ca69bb1125347ff9afc0f2d9191, data\RCSB PDB\PDB\Keywords\Headers\HetAtom\Formula.vb"

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

    '   Total Lines: 22
    '    Code Lines: 17 (77.27%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (22.73%)
    '     File Size: 583 B


    '     Class Formula
    ' 
    '         Properties: Keyword
    ' 
    '         Function: Append
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Keywords

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
End Namespace
