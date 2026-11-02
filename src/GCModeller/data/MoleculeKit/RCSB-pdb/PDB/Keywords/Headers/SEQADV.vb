#Region "Microsoft.VisualBasic::12e6770194b1907f2f454a04ca14dee3, data\RCSB PDB\PDB\Keywords\Headers\SEQADV.vb"

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
    '     File Size: 630 B


    '     Class SEQADV
    ' 
    '         Properties: Keyword, Text
    ' 
    '         Function: Append
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Keywords

    ''' <summary>
    ''' Sequence Advisory
    ''' </summary>
    Public Class SEQADV : Inherits Keyword

        Public Property Text As New List(Of String)

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "SEQADV"
            End Get
        End Property

        Friend Shared Function Append(ByRef seq As SEQADV, line As String) As SEQADV
            If seq Is Nothing Then
                seq = New SEQADV
            End If
            seq.Text.Add(line)
            Return seq
        End Function

    End Class
End Namespace
