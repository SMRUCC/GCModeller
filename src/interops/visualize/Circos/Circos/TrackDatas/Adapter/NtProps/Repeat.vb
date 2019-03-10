#Region "Microsoft.VisualBasic::eb38befd6ce17033748cd26c54e58208, visualize\Circos\Circos\TrackDatas\Adapter\NtProps\Repeat.vb"

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

    '     Class Repeat
    ' 
    '         Properties: Direction, Length, Maximum, Minimum, Name
    '                     Strands
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.Loci

Namespace TrackDatas.NtProps

    Public Class Repeat
        Public Property Name As String
        Public Property Minimum As String
        Public Property Maximum As String
        Public Property Length As Integer
        Public Property Direction As String

        Public ReadOnly Property Strands As Strands
            Get
                Return GetStrand(Direction)
            End Get
        End Property
    End Class
End Namespace
