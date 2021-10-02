#Region "Microsoft.VisualBasic::12a083a8799ea7f275f4d829d1fb7df4, markdown2pdf\JavaScript\highcharts.js\Common\chart.vb"

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

    ' Class chart
    ' 
    '     Properties: BarChart3D, ColumnChart, height, inverted, margin
    '                 options3d, PieChart3D, plotBackgroundColor, plotBorderWidth, plotShadow
    '                 polar, PolarChart, reflow, renderTo, showAxes
    '                 type, VariWide, width, zoomType
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.WebCloud.JavaScript.highcharts.viz3D

Public Class chart

    ''' <summary>
    ''' 图表的类型，默认为line，还有bar/column/pie……
    ''' </summary>
    ''' <returns></returns>
    Public Property type As String
    Public Property options3d As options3d
    ''' <summary>
    ''' 图表中数据报表的放大类型，可以以X轴放大，或是以Y轴放大，还可以以XY轴同时放大。
    ''' </summary>
    ''' <returns></returns>
    Public Property zoomType As String
    ''' <summary>
    ''' 图表中的x，y轴对换。
    ''' </summary>
    ''' <returns></returns>
    Public Property inverted As Boolean?
    Public Property renderTo As String
    Public Property margin As Double?
    ''' <summary>
    ''' 是否为极性图表。
    ''' </summary>
    ''' <returns></returns>
    Public Property polar As Boolean?
    Public Property plotBackgroundColor As String
    Public Property plotBorderWidth As String
    Public Property plotShadow As Boolean?
    ''' <summary>
    ''' 在空白图表中，是否显示坐标轴。
    ''' </summary>
    ''' <returns></returns>
    Public Property showAxes As Boolean?
    ''' <summary>
    ''' 所绘制图表的高度值。
    ''' </summary>
    ''' <returns></returns>
    Public Property height As Double?
    ''' <summary>
    ''' 图表绘图区的宽度，默认为自适应。
    ''' </summary>
    ''' <returns></returns>
    Public Property width As Double?
    ''' <summary>
    ''' 当窗口大小改变时，图表宽度自适应窗口大小改变。
    ''' </summary>
    ''' <returns></returns>
    Public Property reflow As Boolean?

#Region "Chart Profiles"
    Public Shared ReadOnly Property PieChart3D As chart
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return ChartProfiles.PieChart3D
        End Get
    End Property

    Public Shared ReadOnly Property BarChart3D As chart
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return ChartProfiles.BarChart3D
        End Get
    End Property

    Public Shared ReadOnly Property PolarChart As chart
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return ChartProfiles.PolarChart
        End Get
    End Property

    Public Shared ReadOnly Property VariWide As chart
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return ChartProfiles.VariWide
        End Get
    End Property

    Public Shared ReadOnly Property ColumnChart As chart
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return ChartProfiles.ColumnChart
        End Get
    End Property
#End Region

    Public Overrides Function ToString() As String
        If options3d Is Nothing OrElse Not options3d.enabled Then
            Return type
        Else
            Return $"[3D] {type}"
        End If
    End Function
End Class
