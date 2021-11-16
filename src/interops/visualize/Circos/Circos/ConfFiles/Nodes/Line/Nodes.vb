#Region "Microsoft.VisualBasic::2a61c9623aac4eca9a5f308f56cc7ccb, visualize\Circos\Circos\ConfFiles\Nodes\Line\Nodes.vb"

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

    '     Class Axis
    ' 
    '         Properties: color, spacing, thickness
    ' 
    '         Function: Build
    ' 
    '     Class Background
    ' 
    '         Properties: color, y0, y1
    ' 
    '         Function: Build
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Visualize.Circos.Configurations.ComponentModel

Namespace Configurations.Nodes.Plots.Lines

    Public Class Axis : Inherits CircosDocument
        Implements ICircosDocNode

        <Circos> Public Property color As String = "lgrey_a2"
        <Circos> Public Property thickness As String = "1"
        <Circos> Public Property spacing As String = "0.025r"

        Public Overrides Function Build(IndentLevel As Integer, directory$) As String
            Return Me.GenerateCircosDocumentElement("axis", IndentLevel, Nothing, directory)
        End Function
    End Class

    Public Class Background : Inherits CircosDocument
        Implements ICircosDocNode

        <Circos> Public Property color As String = "vvlred"
        <Circos> Public Property y1 As String = "0.002"
        <Circos> Public Property y0 As String = "0.006"

        Public Overrides Function Build(IndentLevel As Integer, directory$) As String
            Return Me.GenerateCircosDocumentElement("background", IndentLevel, Nothing, directory)
        End Function
    End Class
End Namespace
