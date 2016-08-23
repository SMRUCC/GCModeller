#Region "Microsoft.VisualBasic::40a0638be703941881d2e54d9a714ae5, ..\R.Bioconductor\RDotNet.Extensions.Bioinformatics\Declares\plot3D\plot3D.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract

Namespace plot3D

    ''' <summary>
    ''' Plots arrows, segments, points, lines, polygons, rectangles and boxes in a 3D perspective plot or in 2D.
    ''' </summary>
    Public MustInherit Class plot3D : Inherits IRToken

        Sub New()
            MyBase.Requires = {"plot3D"}
        End Sub
    End Class
End Namespace
