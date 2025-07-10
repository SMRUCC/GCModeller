﻿#Region "Microsoft.VisualBasic::c0860bf4d4c7b8920a4166871f423453, engine\Model\Cellular\CellularModule.vb"

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

    '   Total Lines: 48
    '    Code Lines: 14 (29.17%)
    ' Comment Lines: 28 (58.33%)
    '    - Xml Docs: 89.29%
    ' 
    '   Blank Lines: 6 (12.50%)
    '     File Size: 1.79 KB


    '     Structure CellularModule
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Molecule
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process
Imports SMRUCC.genomics.Metagenomics

Namespace Cellular

    ''' <summary>
    ''' A cellular system consist of three features:
    ''' 
    ''' + Genotype
    ''' + Phenotype
    ''' + Regulations
    ''' 
    ''' This object class is just a object model of the target cellular system, 
    ''' and only contains the network structure information in it. The expression 
    ''' profiles data is not includes in this object model.
    ''' 
    ''' (GCMarkup或者Tabular模型文件加载之后会被转换为这个对象，然后计算核心加载这个对象模型来进行计算分析)
    ''' </summary>
    ''' <remarks>
    ''' 注意，这个模型对象之中仅包含有生物学功能定义，如果需要获取得到序列信息，还需要借助于ID编号从其他的数据源之中进行查询
    ''' </remarks>
    Public Structure CellularModule

        ''' <summary>
        ''' The organism taxonomy information
        ''' </summary>
        Public Taxonomy As Taxonomy

        ''' <summary>
        ''' Genome information, usually be the gene expression event data
        ''' </summary>
        Public Genotype As Genotype
        ''' <summary>
        ''' Metabolome, usually be the cellular chemical reaction data
        ''' </summary>
        Public Phenotype As Phenotype

        ''' <summary>
        ''' 转录表达调控以及代谢调控
        ''' </summary>
        Public Regulations As Regulation()

        ''' <summary>
        ''' the compartment id of the intracellular environment
        ''' </summary>
        Public CellularEnvironmentName As String

        Public Overrides Function ToString() As String
            Return Taxonomy.scientificName
        End Function

    End Structure
End Namespace
