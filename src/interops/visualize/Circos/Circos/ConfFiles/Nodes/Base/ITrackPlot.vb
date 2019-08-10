#Region "Microsoft.VisualBasic::69d1c9de09add05de7798adc54b0a67d, visualize\Circos\Circos\ConfFiles\Nodes\Base\ITrackPlot.vb"

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

    '     Enum orientations
    ' 
    '         [in], out
    ' 
    '  
    ' 
    ' 
    ' 
    '     Interface ITrackPlot
    ' 
    '         Properties: file, fill_color, orientation, r0, r1
    '                     stroke_color, stroke_thickness, thickness, tracksData, type
    ' 
    '         Function: Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports SMRUCC.genomics.Visualize.Circos.Configurations.ComponentModel
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas

Namespace Configurations.Nodes.Plots

    Public Enum orientations
        [in]
        out
    End Enum

    ''' <summary>
    ''' Abstract model of the tracks plot
    ''' </summary>
    ''' <remarks>Using this interface to solved the problem of generics type</remarks>
    Public Interface ITrackPlot : Inherits ICircosDocNode

        <Circos> ReadOnly Property type As String

        ''' <summary>
        ''' 输入的路径会根据配置情况转换为相对路径或者绝对路径
        ''' </summary>
        ''' <returns></returns>
        <Circos> Property file As String
        ''' <summary>
        ''' 圈外径(单位 r，请使用格式"&lt;double>r")
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Circos> Property r1 As String

        ''' <summary>
        ''' 圈内径(单位 r，请使用格式"&lt;double>r")
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' The track is confined within r0/r1 radius limits. When using the
        ''' relative "r" suffix, the values are relative To the position Of the
        ''' ideogram.
        ''' </remarks>
        <Circos> Property r0 As String

        Property orientation As orientations
        Property fill_color As String
        Property stroke_thickness As String
        Property stroke_color As String
        Property thickness As String

        ReadOnly Property tracksData As Idata

        Function Save(filePath As String, Encoding As Encoding) As Boolean
    End Interface
End Namespace
