﻿#Region "Microsoft.VisualBasic::ab9a47b45a70875ee21a563fa07b4ca4, data\RCSB PDB\PDB\Keywords\Headers\NUMMDL.vb"

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

    '   Total Lines: 34
    '    Code Lines: 27 (79.41%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 7 (20.59%)
    '     File Size: 923 B


    '     Class NUMMDL
    ' 
    '         Properties: Keyword, NUMMDL
    ' 
    '         Function: Parse, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Keywords

    Public Class NUMMDL : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "NUMMDL"
            End Get
        End Property

        Public Property NUMMDL As Integer

        Friend Shared Function Parse(ByRef NUMMDL As NUMMDL, str As String) As NUMMDL
            If NUMMDL Is Nothing Then
                NUMMDL = New NUMMDL
            End If
            NUMMDL.NUMMDL = CInt(Val(str))
            Return NUMMDL
        End Function

        Public Overrides Function ToString() As String
            Return $"#{NUMMDL} NUMMDL"
        End Function

        Public Shared Narrowing Operator CType(n As NUMMDL) As Integer
            If n Is Nothing Then
                Return 1
            Else
                Return n.NUMMDL
            End If
        End Operator

    End Class
End Namespace
