﻿#Region "Microsoft.VisualBasic::38dbd95bedbb85f79aebf62f7eef0172, visualize\Circos\Circos\Colors\Mappings.vb"

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

    '     Structure Mappings
    ' 
    '         Properties: CircosColor, color, level, value
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing

Namespace Colors

    Public Structure Mappings

        Public Property value As Double
        Public Property level As Integer
        Public Property color As Color

        Public ReadOnly Property CircosColor As String
            Get
                Return $"({color.R},{color.G},{color.B})"
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{value} ==> {level}   @{color.ToString}"
        End Function
    End Structure
End Namespace
