#Region "Microsoft.VisualBasic::c5dfb4b7eca2033cd291560053f78f9c, annotations\WGCNA\WGCNA\ResultTables\ModuleEigengeneResult.vb"

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

    '   Total Lines: 25
    '    Code Lines: 6 (24.00%)
    ' Comment Lines: 15 (60.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 4 (16.00%)
    '     File Size: 609 B


    ' Class ModuleEigengeneResult
    ' 
    '     Properties: Eigengene, GeneCount, ModuleName, VarianceExplained
    ' 
    ' /********************************************************************************/

#End Region


''' <summary>
''' 模块特征基因计算结果
''' </summary>
Public Class ModuleEigengeneResult
    ''' <summary>
    ''' 模块名称
    ''' </summary>
    Public Property ModuleName As String

    ''' <summary>
    ''' 模块特征基因值（每个样本一个值）
    ''' </summary>
    Public Property Eigengene As Double()

    ''' <summary>
    ''' 第一主成分解释的方差比例
    ''' </summary>
    Public Property VarianceExplained As Double

    ''' <summary>
    ''' 模块内基因数量
    ''' </summary>
    Public Property GeneCount As Integer
End Class
