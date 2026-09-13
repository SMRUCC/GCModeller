#Region "Microsoft.VisualBasic::53d7949d9dff936184848564950c8290, localblast\PanGenome\Output\PangenomeCurveData.vb"

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

    '   Total Lines: 11
    '    Code Lines: 8 (72.73%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 3 (27.27%)
    '     File Size: 322 B


    ' Class PangenomeCurveData
    ' 
    '     Properties: CoreGenes, GenomeCount, SoftCoreGenes, TotalGenes
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' 泛基因组曲线之上的一个数据点：表示在加入<see cref="GenomeCount"/>个基因组的时候，
''' 泛基因组(全部基因家族)、核心基因家族以及软核心基因家族的规模
''' </summary>
''' <remarks>
''' 核心基因要求在已经加入的基因组之中全部都出现，判定条件非常严格，
''' 在基因组数量增加的时候核心基因的数量会迅速衰减到零；
''' 软核心基因只要求出现比例不低于分析上下文所设定的阈值(<see cref="GenomeAnalyzer.SoftCoreThreshold"/>)，
''' 因此能够更好地反映基因组数量增加的时候仍然高度保守的那一部分基因。
''' </remarks>
Public Class PangenomeCurveData

    ''' <summary>
    ''' 已经加入的基因组的数量
    ''' </summary>
    ''' <returns></returns>
    Public Property GenomeCount As Integer
    ''' <summary>
    ''' 泛基因组的大小（出现过的全部基因家族数量）
    ''' </summary>
    ''' <returns></returns>
    Public Property TotalGenes As Integer
    ''' <summary>
    ''' 核心基因家族的数量（在所有已经加入的基因组之中都存在）
    ''' </summary>
    ''' <returns></returns>
    Public Property CoreGenes As Integer

    ''' <summary>
    ''' 软核心基因家族的数量（在已经加入的基因组之中的出现比例不低于软核心阈值）
    ''' </summary>
    ''' <returns></returns>
    Public Property SoftCoreGenes As Integer

    Public Overrides Function ToString() As String
        Return $"({GenomeCount}, [total:{TotalGenes}, core:{CoreGenes}, softcore:{SoftCoreGenes}])"
    End Function

End Class
