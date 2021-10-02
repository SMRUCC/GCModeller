#Region "Microsoft.VisualBasic::7dde580fecdffb37ac598b3f054f66f3, markdown2pdf\JavaScript\highcharts.js\ChartTypes.vb"

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

    ' Enum ChartTypes
    ' 
    '     area, areaspline, bar, column, columnrange
    '     pie, variwide
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Public Enum ChartTypes As Integer
    area
    ''' <summary>
    ''' 水平的条子
    ''' </summary>
    bar
    ''' <summary>
    ''' 垂直的柱子
    ''' </summary>
    column
    pie
    columnrange
    areaspline
    variwide
End Enum
