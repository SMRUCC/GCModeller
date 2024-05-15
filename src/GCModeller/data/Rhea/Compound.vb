﻿#Region "Microsoft.VisualBasic::88c8406b05b3e458167d27bdf8d655e0, data\Rhea\Compound.vb"

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

    '   Total Lines: 13
    '    Code Lines: 10
    ' Comment Lines: 0
    '   Blank Lines: 3
    '     File Size: 324 B


    ' Class Compound
    ' 
    '     Properties: entry, enzyme, formula, name, reactions
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Public Class Compound

    Public Property entry As String
    Public Property name As String
    Public Property formula As String
    Public Property reactions As String()
    Public Property enzyme As String()

    Public Overrides Function ToString() As String
        Return name
    End Function

End Class
