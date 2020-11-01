#Region "Microsoft.VisualBasic::959d1154b3aa525e9944a7d7886c6a4d, RDotNET\Graphics\Enum.vb"

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

    '     Enum LineEnd
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    '     Enum LineJoin
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    '     Enum LineType
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    '     Enum Unit
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    '     Enum Adjustment
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    '     Enum FontFace
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Graphics

    Public Enum LineEnd
        Round = 1
        Butt = 2
        Square = 3
    End Enum

    Public Enum LineJoin
        Round = 1
        Miter = 2
        Beveled = 3
    End Enum

    Public Enum LineType
        Blank = -1
        Solid = 0
        Dashed = 4 + (4 << 4)
        Dotted = 1 + (3 << 4)
        DotDash = 1 + (3 << 4) + (4 << 8) + (3 << 12)
        LongDash = 7 + (3 << 4)
        TwoDash = 2 + (2 << 4) + (6 << 8) + (2 << 12)
    End Enum

    Public Enum Unit
        Device = 0
        NormalizedDeviceCoordinates = 1
        Inches = 2
        Centimeters = 3
    End Enum

    Public Enum Adjustment
        None = 0
        Half = 1
        All = 2
    End Enum

    Public Enum FontFace
        Plain = 1
        Bold = 2
        Italic = 3
        BoldItalic = 4
    End Enum
End Namespace
