#Region "Microsoft.VisualBasic::08a6a9ac455d037cd2dff29ce0cc0d85, visualize\Circos\Circos\ConfFiles\Nodes\Ticks.vb"

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

    '     Class Ticks
    ' 
    '         Properties: color, label_offset, label_separation, label_size, min_label_distance_to_edge
    '                     multiplier, radius, size, skip_first_label, skip_last_label
    '                     thickness, tick_separation, ticks
    ' 
    '         Function: Build, DefaultConfiguration
    ' 
    '     Class Tick
    ' 
    '         Properties: color, format, grid, grid_color, grid_thickness
    '                     label_offset, label_size, show_label, size, spacing
    '                     suffix
    ' 
    '         Function: Build
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Visualize.Circos.Configurations.ComponentModel
Imports Microsoft.VisualBasic.Language

Namespace Configurations.Nodes

    Public Class Ticks : Inherits CircosDocument
        Implements ICircosDocNode

        <Circos> Public Property skip_first_label As String = no
        <Circos> Public Property skip_last_label As String = no
        <Circos> Public Property radius As String = "dims(ideogram,radius_outer)"
        <Circos> Public Property tick_separation As String = "2p"
        <Circos> Public Property min_label_distance_to_edge As String = "0p"
        <Circos> Public Property label_separation As String = "5p"
        <Circos> Public Property label_offset As String = "5p"
        <Circos> Public Property label_size As String = "36p"
        <Circos> Public Property color As String = "black"
        ''' <summary>
        ''' the tick label is derived by multiplying the tick position
        ''' by ``<see cref="multiplier"/>`` and casting it in ``<see cref="format"/>``:
        '''
        ''' ```
        ''' sprintf(format,position*multiplier)
        ''' ```
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property multiplier As String = "0.001"
        <Circos> Public Property thickness As String = "3p"
        <Circos> Public Property size As String = "20p"

        Public Property ticks As New List(Of Tick)

        Public Shared Function DefaultConfiguration() As Ticks
            Dim ticks As New List(Of Tick)

            ' sub ticks
            ticks += New Tick With {.spacing = "1u", .show_label = no, .grid_thickness = "1p"}
            ' main ticks
            ticks += New Tick With {.spacing = "5u", .show_label = yes, .label_size = "28p", .format = "%d"}

            Return New Ticks With {
                .ticks = ticks
            }
        End Function

        Public Overrides Function Build(IndentLevel As Integer, directory$) As String
            Return Me.GenerateCircosDocumentElement("ticks", IndentLevel, inserts:=ticks, directory:=directory)
        End Function
    End Class

    ''' <summary>
    ''' Rule unit and displaying
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Tick : Inherits CircosDocument
        Implements ICircosDocNode

        <Circos> Public Property size As String = "20p"
        <Circos> Public Property spacing As String = "500u"
        <Circos> Public Property color As String = "black"
        <Circos> Public Property show_label As String = yes
        <Circos> Public Property suffix As String = """ kb"""
        <Circos> Public Property label_size As String = "36p"
        ''' <summary>
        ''' Example as: ``label_offset = 10p``
        ''' </summary>
        ''' <returns></returns>
        Public Property label_offset As String
        ''' <summary>
        ''' |format control|types                  |
        ''' |--------------|-----------------------|
        ''' |%d            |integer                |
        ''' |%f            |float                  |
        ''' |%.1f          |float With one Decimal |
        ''' |%.2f          |float With two decimals|
        '''
        ''' For other formats, see http://perldoc.perl.org/functions/sprintf.html
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property format As String = "%s"
        <Circos> Public Property grid As String = yes
        <Circos> Public Property grid_color As String = "black"
        <Circos> Public Property grid_thickness As String = "4p"

        Public Overrides Function Build(IndentLevel As Integer, directory$) As String
            Return Me.GenerateCircosDocumentElement("tick", IndentLevel, Nothing, directory)
        End Function
    End Class
End Namespace
