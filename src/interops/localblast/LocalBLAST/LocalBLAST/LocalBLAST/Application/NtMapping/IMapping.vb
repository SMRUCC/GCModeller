#Region "Microsoft.VisualBasic::e50eebf8d846518cc1670894abe4c835, LocalBLAST\LocalBLAST\LocalBLAST\Application\NtMapping\IMapping.vb"

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

    '     Interface IMapping
    ' 
    '         Properties: Qname, Qstart, Qstop, Sname, Sstart
    '                     Sstop
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace LocalBLAST.Application.NtMapping

    Public Interface IMapping

        Property Qname As String
        Property Sname As String

        Property Qstart As Integer
        Property Qstop As Integer
        Property Sstart As Integer
        Property Sstop As Integer
    End Interface
End Namespace
