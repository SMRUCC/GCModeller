#Region "Microsoft.VisualBasic::190c01b1aef3a25bab79e7ca6b3db42a, data\RCSB PDB\PDB\Keywords\Headers\3D.vb"

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

    '   Total Lines: 45
    '    Code Lines: 32 (71.11%)
    ' Comment Lines: 3 (6.67%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (22.22%)
    '     File Size: 1.17 KB


    '     Class Spatial3D
    ' 
    '         Properties: factor, x, y, z
    ' 
    '         Function: Parse
    ' 
    '     Class ORIGX123
    ' 
    '         Properties: Keyword
    ' 
    '     Class SCALE123
    ' 
    '         Properties: Keyword
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Keywords

    ''' <summary>
    ''' Axis information
    ''' </summary>
    Public MustInherit Class Spatial3D : Inherits Keyword

        Public Property x As Double
        Public Property y As Double
        Public Property z As Double
        Public Property factor As Double

        Friend Shared Function Parse(Of T As {New, Spatial3D})(str As String) As Spatial3D
            Dim cols As String() = str.StringSplit("\s+")
            Dim offset As Integer = If(GetType(T) Is GetType(MTRIX123), 1, 0)
            Dim s As New T With {
                .x = Val(cols(0 + offset)),
                .y = Val(cols(1 + offset)),
                .z = Val(cols(2 + offset)),
                .factor = Val(cols.ElementAtOrDefault(3 + offset, [default]:="1"))
            }

            Return s
        End Function
    End Class

    Public Class ORIGX123 : Inherits Spatial3D

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "ORIGX1"
            End Get
        End Property

    End Class

    Public Class SCALE123 : Inherits Spatial3D

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "SCALE1"
            End Get
        End Property

    End Class

    Public Class MTRIX123 : Inherits Spatial3D

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return "MTRIX1"
            End Get
        End Property

    End Class
End Namespace
