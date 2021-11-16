#Region "Microsoft.VisualBasic::4edfd2202970baf22ff909a3f5f760ba, markdown2pdf\JavaScript\highcharts.js\Common\ChartProfiles.vb"

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

    ' Module ChartProfiles
    ' 
    '     Function: AreaSpline, BarChart3D, ColumnChart, PieChart3D, PolarChart
    '               profileBase, VariWide
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.WebCloud.JavaScript.highcharts.viz3D

Friend Module ChartProfiles

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function profileBase(type As ChartTypes) As chart
        Return New chart With {
            .type = type.Description
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function PieChart3D() As chart
        Return New chart With {
            .type = "pie",
            .options3d = New options3d With {
                .enabled = True,
                .alpha = 45,
                .beta = 0
            }
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function BarChart3D() As chart
        Return New chart With {
            .type = "column",
            .options3d = New options3d With {
                .enabled = True,
                .alpha = 5,
                .beta = 20,
                .depth = 70
            }
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function PolarChart() As chart
        Return New chart With {
            .polar = True
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function VariWide() As chart
        Return profileBase(ChartTypes.variwide)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function AreaSpline() As chart
        Return profileBase(ChartTypes.areaspline)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function ColumnChart() As chart
        Return profileBase(ChartTypes.column)
    End Function
End Module
