#Region "Microsoft.VisualBasic::12ff251fa8579895236b0be7124dc933, RNA-Seq\RNA-seq.Data\Simulator\RegionHotspot.vb"

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

    '   Total Lines: 19
    '    Code Lines: 5 (26.32%)
    ' Comment Lines: 12 (63.16%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 2 (10.53%)
    '     File Size: 575 B


    ' Class RegionHotspot
    ' 
    '     Properties: [End], Start, Weight
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' 用于定义模拟测序区域的“热点”，即高丰度区域。
''' </summary>
Public Class RegionHotspot
    ''' <summary>
    ''' 热点区域的起始位置（0-based索引）。
    ''' </summary>
    Public Property Start As Integer

    ''' <summary>
    ''' 热点区域的结束位置（0-based索引）。
    ''' </summary>
    Public Property [End] As Integer

    ''' <summary>
    ''' 该热点的相对权重。权重越高，被选中的概率越大。
    ''' </summary>
    Public Property Weight As Double
End Class
