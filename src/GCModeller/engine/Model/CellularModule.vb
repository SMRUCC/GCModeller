#Region "Microsoft.VisualBasic::ee0f0e07abee8f92a78007d51a96fb8e, Model\CellularModule.vb"

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

    ' Structure CellularModule
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Metagenomics

''' <summary>
''' A cellular system consist of three features:
''' 
''' + Genotype
''' + Phenotype
''' + Regulations
''' 
''' (GCMarkup或者Tabular模型文件加载之后会被转换为这个对象，然后计算核心加载这个对象模型来进行计算分析)
''' </summary>
Public Structure CellularModule

    Public Taxonomy As Taxonomy

    ''' <summary>
    ''' Genome
    ''' </summary>
    Public Genotype As Genotype
    ''' <summary>
    ''' Metabolome
    ''' </summary>
    Public Phenotype As Phenotype
    Public Regulations As Regulation()

End Structure

