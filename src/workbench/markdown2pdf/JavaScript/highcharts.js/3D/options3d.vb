#Region "Microsoft.VisualBasic::a74858991de8ffb5d73d622a36df0bf3, markdown2pdf\JavaScript\highcharts.js\3D\options3d.vb"

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

    '     Class options3d
    ' 
    '         Properties: alpha, beta, depth, enabled, fitToPlot
    '                     frame, viewDistance
    ' 
    '         Function: ToString
    ' 
    '     Class frame3DOptions
    ' 
    '         Properties: back, bottom, side
    ' 
    '     Class frameOptions
    ' 
    '         Properties: color, size
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace viz3D

    Public Class options3d

        Public Property enabled As Boolean?
        Public Property alpha As Double?
        Public Property beta As Double?
        Public Property depth As Double?
        Public Property viewDistance As Double?
        Public Property fitToPlot As Boolean?
        Public Property frame As frame3DOptions

        Public Overrides Function ToString() As String
            If enabled Then
                Return NameOf(enabled)
            Else
                Return $"Not {NameOf(enabled)}"
            End If
        End Function
    End Class

    Public Class frame3DOptions
        Public Property bottom As frameOptions
        Public Property back As frameOptions
        Public Property side As frameOptions
    End Class

    Public Class frameOptions
        Public Property size As Double?
        Public Property color As String
    End Class

End Namespace
