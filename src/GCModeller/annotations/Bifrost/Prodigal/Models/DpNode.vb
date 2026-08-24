#Region "Microsoft.VisualBasic::4bce70a0dab85a471525eae48322df3b, annotations\Bifrost\Prodigal\Models\DpNode.vb"

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

    '   Total Lines: 16
    '    Code Lines: 6 (37.50%)
    ' Comment Lines: 7 (43.75%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 3 (18.75%)
    '     File Size: 525 B


    ' Class DpNode
    ' 
    '     Properties: OrfIndex, Position, PrevNode, Score
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' DP节点（用于动态规划选基因）
''' </summary>
Public Class DpNode
    ''' <summary>位置（基因组坐标）</summary>
    Public Property Position As Integer

    ''' <summary>到该位置的最优累积得分</summary>
    Public Property Score As Double

    ''' <summary>前驱节点索引</summary>
    Public Property PrevNode As Integer = -1

    ''' <summary>关联的ORF索引（-1表示非基因节点）</summary>
    Public Property OrfIndex As Integer = -1
End Class
