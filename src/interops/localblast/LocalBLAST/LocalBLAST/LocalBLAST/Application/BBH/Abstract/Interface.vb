#Region "Microsoft.VisualBasic::e8654ef4b901bc5245788dd2ae9294a5, localblast\LocalBLAST\LocalBLAST\LocalBLAST\Application\BBH\Abstract\Interface.vb"

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

    '     Interface IBlastHit
    ' 
    '         Properties: Address, locusId
    ' 
    '     Interface IQueryHits
    ' 
    '         Properties: identities
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace LocalBLAST.Application.BBH.Abstract

    Public Interface IBlastHit
        Property locusId As String
        Property Address As String
    End Interface

    Public Interface IQueryHits : Inherits IBlastHit
        ReadOnly Property identities As Double
    End Interface
End Namespace
