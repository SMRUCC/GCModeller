#Region "Microsoft.VisualBasic::35f4e5465652352a409cff1ba67a7506, sub-system\BNLearn\StructureLearning\StructureLearningParams.vb"

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

    '   Total Lines: 36
    '    Code Lines: 13 (36.11%)
    ' Comment Lines: 12 (33.33%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 11 (30.56%)
    '     File Size: 1.21 KB


    '     Class StructureLearningParams
    ' 
    '         Properties: Algorithm, Alpha, BICPenalty, MaxIterations, MaxParents
    '                     RandomSeed, TabuLength, UseBlacklist, UseWhitelist
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace StructureLearning

    ''' <summary>
    ''' 结构学习参数
    ''' </summary>
    Public Class StructureLearningParams

        ''' <summary>算法类型</summary>
        Public Property Algorithm As StructureAlgorithm = StructureAlgorithm.MMHC

        ''' <summary>显著性水平 alpha（用于独立性检验）</summary>
        Public Property Alpha As Double = 0.05

        ''' <summary>最大父节点数</summary>
        Public Property MaxParents As Integer = 5

        ''' <summary>Tabu 搜索的禁忌表长度</summary>
        Public Property TabuLength As Integer = 20

        ''' <summary>最大迭代次数</summary>
        Public Property MaxIterations As Integer = 500

        ''' <summary>BIC 惩罚系数（>1 更稀疏）</summary>
        Public Property BICPenalty As Double = 1.0

        ''' <summary>是否使用白名单先验</summary>
        Public Property UseWhitelist As Boolean = True

        ''' <summary>是否使用黑名单</summary>
        Public Property UseBlacklist As Boolean = True

        ''' <summary>随机种子</summary>
        Public Property RandomSeed As Integer = 42

    End Class
End Namespace
