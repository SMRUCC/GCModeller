#Region "Microsoft.VisualBasic::9f0d7c7a8e5fa515ed6e6134707334cc, ..\GNUplot\GNUplot\Enums.vb"

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


Public Enum PointStyles
    Dot = 0
    Plus = 1
    X = 2
    Star = 3
    DotSquare = 4
    SolidSquare = 5
    DotCircle = 6
    SolidCircle = 7
    DotTriangleUp = 8
    SolidTriangleUp = 9
    DotTriangleDown = 10
    SolidTriangleDown = 11
    DotDiamond = 12
    SolidDiamond = 13
End Enum

Public Enum PlotTypes
    PlotFileOrFunction
    PlotY
    PlotXY
    ContourFileOrFunction
    ContourXYZ
    ContourZZ
    ContourZ
    ColorMapFileOrFunction
    ColorMapXYZ
    ColorMapZZ
    ColorMapZ
    SplotFileOrFunction
    SplotXYZ
    SplotZZ
    SplotZ
End Enum

