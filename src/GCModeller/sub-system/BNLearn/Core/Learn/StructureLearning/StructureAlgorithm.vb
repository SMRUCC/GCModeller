#Region "Microsoft.VisualBasic::d3d77b0644ff026892a73077b6518d15, sub-system\BNLearn\StructureLearning\StructureAlgorithm.vb"

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

    '   Total Lines: 14
    '    Code Lines: 7 (50.00%)
    ' Comment Lines: 6 (42.86%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 1 (7.14%)
    '     File Size: 388 B


    '     Enum StructureAlgorithm
    ' 
    '         HillClimbing, MMHC, Tabu
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace StructureLearning

    ''' <summary>
    ''' 结构学习算法类型
    ''' </summary>
    Public Enum StructureAlgorithm
        ''' <summary>Hill-Climbing 贪心搜索</summary>
        HillClimbing
        ''' <summary>Tabu 禁忌搜索</summary>
        Tabu
        ''' <summary>MMHC 混合算法（推荐）</summary>
        MMHC
    End Enum
End Namespace
