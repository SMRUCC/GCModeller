#Region "Microsoft.VisualBasic::76084fa58bee006dc6f4dce52dccd885, data\RCSB PDB\PDB\Keywords\Headers\NUMMDL.vb"

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

    '   Total Lines: 26
    '    Code Lines: 20 (76.92%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (23.08%)
    '     File Size: 696 B


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

    End Class
End Namespace
